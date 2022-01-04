using System;
using System.Collections.Concurrent;
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
        readonly ConcurrentQueue<byte[]> sendQueue;
        readonly ConcurrentQueue<byte[]> receiveQueue;

        public static SpiTransport CreateRaspberryPiJacdapterTransport()
        {
            return new SpiTransport(new GpioController(PinNumberingScheme.Logical),
                RPI_PIN_TX_READY, RPI_PIN_RX_READY, RPI_PIN_RST, RPI_SPI_BUS_ID);
        }

        public SpiTransport(GpioController controller, int txReadyPin, int rxReadyPin, int resetPin, int spiBusId)
            : base("spi")
        {
            this.controller = controller;
            this.txReadyPin = txReadyPin;
            this.rxReadyPin = rxReadyPin;
            this.resetPin = resetPin;
            this.spiBusId = spiBusId;
            this.sendQueue = new ConcurrentQueue<byte[]>();
            this.receiveQueue = new ConcurrentQueue<byte[]>();
        }

        public override string ToString()
        {
            return "spi bridge";
        }

        public override event FrameReceivedEvent FrameReceived;
        public override event TransportErrorReceivedEvent ErrorReceived;

        protected override void InternalConnect()
        {
            Console.WriteLine($"connecting to jacdapter...");
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

            Console.WriteLine($"jacdapter ready.");
            this.SetConnectionState(ConnectionState.Connected);

            // initiate
            this.transfer();
        }

        protected override void InternalDisconnect()
        {
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
        }

        public override void SendFrame(byte[] data)
        {
            //Console.WriteLine($"send frame {HexEncoding.ToString(data)}");
            this.sendQueue.Enqueue(data);

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
                lock (this.sendQueue)
                {
                    transfer = this.transferFrame();
                }
                byte[] recv;
                while (this.receiveQueue.TryDequeue(out recv))
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
            bool sendtx = this.sendQueue.Count > 0 && txReady;

            if (!sendtx && !rxReady)
                return false;

            // allocate transfer buffers
            byte[] txqueue = new byte[XFER_SIZE];
            byte[] rxqueue = new byte[txqueue.Length]; // .net requires same length buffers

            // assemble packets into send buffer
            int txq_ptr = 0;
            byte[] pkt;
            while (this.sendQueue.TryPeek(out pkt) && txq_ptr + pkt.Length < XFER_SIZE)
            {
                this.sendQueue.TryDequeue(out pkt);
                Array.Copy(pkt, 0, txqueue, txq_ptr, pkt.Length);
                txq_ptr += (pkt.Length + 3) & ~3;
            }

            // attempt transfer
            var ok = this.attemptTransferBuffers(txqueue, rxqueue);
            if (!ok)
            {
                Console.WriteLine("transfer failed");
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
                    /*
                    if (framep == 0 && frame2 > 0)
                    {
                        Console.WriteLine($"tx {txReady}, rx {rxReady}, send {this.sendQueue.Count}, recv {this.receiveQueue.Count}");
                        Console.WriteLine($"rx {HexEncoding.ToString(rxqueue)}");
                    }
                    */
                    if (frame2 == 0)
                        break;
                    int sz = frame2 + 12;
                    if (framep + sz > XFER_SIZE)
                    {
                        Console.WriteLine($"packet overflow {framep} + {sz} > {XFER_SIZE}");
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
                        this.receiveQueue.Enqueue(frame);
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
                    Console.WriteLine(ex);
                    Thread.Sleep(1);
                }
            }
            return false;
        }

        private void raiseError(TransportError error, byte[] data)
        {
            var err = this.ErrorReceived;
            if (err != null)
            {
                var now = DateTime.Now;
                err(this, new TransportErrorReceivedEventArgs(error, now, data));
            }
        }
    }
}
