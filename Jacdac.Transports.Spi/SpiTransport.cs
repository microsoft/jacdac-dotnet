using System;
using System.Collections.Concurrent;
using System.Device.Gpio;
using System.Device.Spi;
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

        const int RPI_PIN_TX_READY = 24;   // RST; G0 is ready for data from Pi
        const int RPI_PIN_RX_READY = 25;// AN; G0 has data for Pi
        const int RPI_PIN_BRIDGE_RST = 22;// nRST of the bridge G0 MCU
        const int RPI_PIN_SPI_BUS_ID = 0; // "/dev/spidev0.0"

        readonly int txReadyPin;
        readonly int rxReadyPin;
        readonly int brigeResetPin;
        readonly int spiBusId;
        readonly GpioController controller;
        SpiDevice spi;
        readonly ConcurrentQueue<byte[]> sendQueue;
        readonly ConcurrentQueue<byte[]> receiveQueue;

        public static SpiTransport CreateRaspberryPiJacdapterTransport()
        {
            return new SpiTransport(new GpioController(PinNumberingScheme.Logical),
                RPI_PIN_TX_READY, RPI_PIN_RX_READY, RPI_PIN_BRIDGE_RST, RPI_PIN_SPI_BUS_ID);
        }

        public SpiTransport(GpioController controller, int txReadyPin, int rxReadyPin, int brigeResetPin, int spiBusId)
            : base("spi")
        {
            this.controller = controller;
            this.txReadyPin = txReadyPin;
            this.rxReadyPin = rxReadyPin;
            this.brigeResetPin = brigeResetPin;
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
            this.controller.OpenPin(txReadyPin, PinMode.InputPullDown);
            this.controller.OpenPin(rxReadyPin, PinMode.InputPullDown);
            this.controller.OpenPin(brigeResetPin, PinMode.Output);

            this.controller.Write(brigeResetPin, 0);
            Thread.Sleep(10);
            this.controller.Write(brigeResetPin, 1);
            this.controller.SetPinMode(brigeResetPin, PinMode.Input);

            this.spi = SpiDevice.Create(new SpiConnectionSettings(spiBusId)
            {
                DataBitLength = 8,
                ClockFrequency = 16 * 1000 * 1000,
                Mode = SpiMode.Mode0,

            });

            this.controller.RegisterCallbackForPinValueChangedEvent(rxReadyPin, PinEventTypes.Rising, this.handlePinRising);
            this.controller.RegisterCallbackForPinValueChangedEvent(txReadyPin, PinEventTypes.Rising, this.handlePinRising);

            // initiate
            this.transfer();
        }

        protected override void InternalDisconnect()
        {
            this.controller.UnregisterCallbackForPinValueChangedEvent(rxReadyPin, this.handlePinRising);
            this.controller.UnregisterCallbackForPinValueChangedEvent(txReadyPin, this.handlePinRising);

            this.controller.ClosePin(txReadyPin);
            this.controller.ClosePin(rxReadyPin);
            this.controller.ClosePin(brigeResetPin);

            var spi = this.spi;
            if (spi != null)
            {
                this.spi = null;
                spi.Dispose();
            }
        }

        public override void SendFrame(byte[] data)
        {
            var len = data.Length;
            if (len < 12)
                throw new ArgumentOutOfRangeException("invalid frame size");
            ushort crc = Platform.Crc16(data, 2, len - 2);
            if ((data[0] | (data[1] << 8)) != crc)
                throw new ArgumentOutOfRangeException("invalid CRC");
            this.sendQueue.Enqueue(data);
            this.transfer();
        }

        private void handlePinRising(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
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
            bool sendtx = this.sendQueue.Count > 0 && txReady;
            bool rxReady = this.controller.Read(this.rxReadyPin) == PinValue.High;
            if (!sendtx && !rxReady)
                return false;

            // allocate transfer buffers
            byte[] txqueue = new byte[XFER_SIZE + 4];
            byte[] rxqueue = new byte[XFER_SIZE];

            // assemble packets into send buffer
            int txq_ptr = 0;
            byte[] pkt;
            while (this.sendQueue.TryPeek(out pkt) && txq_ptr + pkt.Length > txqueue.Length)
            {
                this.sendQueue.TryDequeue(out pkt);
                Array.Copy(pkt, 0, txqueue, txq_ptr, pkt.Length);
                txq_ptr += (pkt.Length + 3) & ~3;
            }

            // attempt transfer
            var ok = this.attemptTransferBuffers(txqueue, rxqueue);
            if (!ok)
            {
                this.raiseError(TransportError.Frame, pkt);
                return false;
            }

            // consume received frame if any
            if (rxReady)
                this.receiveQueue.Enqueue(rxqueue);
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
                catch (Exception)
                {
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
