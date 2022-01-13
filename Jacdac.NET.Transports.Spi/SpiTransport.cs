using System;
using System.Collections;
using System.Device.Gpio;
using System.Device.Spi;
using System.Diagnostics;
using System.Threading;

namespace Jacdac.Transports.Spi
{
    /**
     * A transport that communicates with a SPI bridge.
     */
    public sealed class SpiTransport : Transport
    {
        const uint XFER_SIZE = 256;
        const uint SPI_TRANSFER_ATTEMPT_COUNT = 10;

        const int RPI_PIN_TX_READY = 24;
        const int RPI_PIN_RX_READY = 25;
        const int RPI_PIN_RST = 22;
        const int RPI_SPI_BUS_ID = 0;

        readonly int txReadyPin;
        readonly int rxReadyPin;
        readonly int resetPin;
        readonly int spiBusId;
        readonly GpioController controller;
        SpiDevice spi;
        Timer reconnectTimer = null;
        // must be synched with sendQueue
        readonly Queue sendQueue;
        // must be synched with receiveQueue
        readonly Queue receiveQueue;

        static class QueueExtensions
        {
            public static bool TryDequeue(Queue queue, out byte[] value)
            {
                lock (queue)
                {
                    if (queue.Count > 0)
                    {
                        value = (byte[])queue.Dequeue();
                        return true;
                    }
                    else
                    {
                        value = null;
                        return false;
                    }
                }
            }

            public static bool TryPeekAndDequeue(Queue queue, int maxLength, out byte[] value)
            {
                lock (queue)
                {
                    if (queue.Count > 0)
                    {
                        value = (byte[])queue.Peek();
                        if (value.Length <= maxLength)
                        {
                            queue.Dequeue();
                            return true;
                        }
                    }

                    value = null;
                    return false;
                }
            }

            public static bool IsEmpty(Queue queue)
            {
                lock (queue)
                {
                    return queue.Count == 0;
                }
            }

            public static void Enqueue(Queue queue, byte[] value)
            {
                lock (queue)
                    queue.Enqueue(value);
            }
        }

        public static SpiTransport Create()
        {
            return new SpiTransport(new GpioController(PinNumberingScheme.Logical),
                RPI_PIN_TX_READY, RPI_PIN_RX_READY, RPI_PIN_RST, RPI_SPI_BUS_ID);
        }

        internal SpiTransport(GpioController controller, int txReadyPin, int rxReadyPin, int resetPin, int spiBusId)
            : base("spi")
        {
            this.controller = controller;
            this.txReadyPin = txReadyPin;
            this.rxReadyPin = rxReadyPin;
            this.resetPin = resetPin;
            this.spiBusId = spiBusId;
            this.sendQueue = new Queue();
            this.receiveQueue = new Queue();
        }

        public override string ToString()
        {
            return "spi bridge";
        }

        public override event FrameEventHandler FrameReceived;
        public override event TransportErrorReceivedEvent ErrorReceived;

        protected override void InternalConnect()
        {
            Console.WriteLine($"spi: connecting");

            if (this.reconnectTimer != null)
            {
                this.reconnectTimer.Dispose();
                this.reconnectTimer = null;
            }

            this.controller.OpenPin(txReadyPin, PinMode.Input); // pull down
            this.controller.OpenPin(rxReadyPin, PinMode.Input); // pull down

            this.controller.OpenPin(resetPin, PinMode.Output);
            this.controller.Write(resetPin, 0);
            Thread.Sleep(10);
            this.controller.Write(resetPin, 1);

            this.controller.SetPinMode(resetPin, PinMode.Input);

            this.spi = SpiDevice.Create(new SpiConnectionSettings(spiBusId)
            {
                DataBitLength = 8,
                ClockFrequency = 16 * 1000 * 1000,
                Mode = SpiMode.Mode0,

            });

            this.controller.RegisterCallbackForPinValueChangedEvent(rxReadyPin, PinEventTypes.Rising, this.handleRxPinRising);
            this.controller.RegisterCallbackForPinValueChangedEvent(txReadyPin, PinEventTypes.Rising, this.handleTxPinRising);

            Console.WriteLine($"spi: ready");
            this.SetConnectionState(ConnectionState.Connected);

            // initiate
            this.transfer();
        }

        protected override void InternalDisconnect()
        {
            Console.WriteLine($"spi: disconnecting");
            this.controller.UnregisterCallbackForPinValueChangedEvent(rxReadyPin, this.handleRxPinRising);
            this.controller.UnregisterCallbackForPinValueChangedEvent(txReadyPin, this.handleTxPinRising);

            this.controller.ClosePin(txReadyPin);
            this.controller.ClosePin(rxReadyPin);
            this.controller.ClosePin(resetPin);

            var spi = this.spi;
            if (spi != null)
            {
                this.spi = null;
                spi.Dispose();
            }

            // start reconnect timer
            if (this.reconnectTimer == null)
                this.reconnectTimer = new Timer(state =>
                {
                    if (this.ConnectionState == ConnectionState.Connected)
                        return;
                    Console.WriteLine($"spi: reconnecting");
                    this.Connect();
                }, null, 5000, 5000);
        }

        public override void SendFrame(byte[] data)
        {
            //Console.WriteLine($"send frame {HexEncoding.ToString(data)}");
            QueueExtensions.Enqueue(this.sendQueue, data);

            this.transfer();
        }

        private void handleRxPinRising(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            //Console.WriteLine($"rx rise");
            this.transfer();
        }

        private void handleTxPinRising(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            //Console.WriteLine($"tx rise");
            this.transfer();
        }

        private void transfer()
        {
            bool transfer = true;
            while (transfer)
            {
                transfer = this.transferFrame();
                byte[] recv;
                while (QueueExtensions.TryDequeue(this.receiveQueue, out recv))
                {
                    var ev = this.FrameReceived;
                    if (ev != null)
                        ev(this, recv);
                }
            }
        }

        private bool transferFrame()
        {
            // much be in a locked context
            bool txReady = this.controller.Read(this.txReadyPin) == PinValue.High;
            bool rxReady = this.controller.Read(this.rxReadyPin) == PinValue.High;
            bool sendtx = txReady && !QueueExtensions.IsEmpty(this.sendQueue) && txReady;

            if (!sendtx && !rxReady)
                return false;

            // allocate transfer buffers
            byte[] txqueue = new byte[XFER_SIZE];
            byte[] rxqueue = new byte[txqueue.Length]; // .net requires same length buffers

            // assemble packets into send buffer
            int txq_ptr = 0;
            byte[] pkt;
            while (QueueExtensions.TryPeekAndDequeue(this.sendQueue, (int)(XFER_SIZE - txq_ptr), out pkt))
            {
                Array.Copy(pkt, 0, txqueue, txq_ptr, pkt.Length);
                txq_ptr += (pkt.Length + 3) & ~3;
            }

            if (txq_ptr == 0 && !rxReady)
                return false; // nothing to transfer, nothing to receive

            // attempt transfer
            var ok = this.attemptTransferBuffers(txqueue, rxqueue);
            if (!ok)
            {
                Console.WriteLine("spi: transfer failed");
                this.raiseError(TransportError.Frame, pkt);
                return false;
            }

            if (rxReady)
            {
                // consume received frame if any
                int framep = 0;
                while (framep < XFER_SIZE)
                {
                    var frame2 = rxqueue[framep + 2];
                    if (frame2 == 0)
                        break;
                    int sz = frame2 + 12;
                    if (framep + sz > XFER_SIZE)
                    {
                        Console.WriteLine($"spi: packet overflow {framep} + {sz} > {XFER_SIZE}");
                        break;
                    }
                    var frame0 = rxqueue[framep];
                    var frame1 = rxqueue[framep + 1];
                    var frame3 = rxqueue[framep + 3];

                    if (frame0 == 0xff && frame1 == 0xff && frame3 == 0xff)
                    {
                        // skip bogus packet
                    }
                    else
                    {
                        var frame = new byte[sz];
                        Array.Copy(rxqueue, framep, frame, 0, sz);
                        //Console.WriteLine($"recv frame {HexEncoding.ToString(frame)}");
                        QueueExtensions.Enqueue(this.receiveQueue, frame);
                    }
                    sz = (sz + 3) & ~3;
                    framep += sz;
                }
            }
            return true;
        }

        private bool attemptTransferBuffers(byte[] txqueue, byte[] rxqueue)
        {
            // attempt transfer
            for (var i = 0; i < SPI_TRANSFER_ATTEMPT_COUNT; i++)
            {
                try
                {
                    this.spi.TransferFullDuplex(txqueue, rxqueue);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Thread.Sleep(1);
                }
            }
            return false;
        }

        private void raiseError(TransportError error, byte[] data)
        {
            var err = this.ErrorReceived;
            if (err != null)
                err(this, new TransportErrorReceivedEventArgs(error, data));
        }
    }
}
