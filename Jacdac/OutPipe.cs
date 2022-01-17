using System;
using System.Threading;

namespace Jacdac
{
    public delegate byte[] ObjectEncoder(object item);

    public sealed class OutPipe : JDNode
    {
        private JDDevice device;
        private ushort port;
        private ushort _count = 0;

        public OutPipe(JDDevice device, ushort port)
        {
            this.device = device;
            this.port = port;
            this.LogDebug($"open");
        }

        public override JDBus Bus => this.device?.Bus;

        public override string ToString()
        {
            return $"pipe:${this.device}:${this.port}";
        }

        public static OutPipe From(JDBus bus, Packet pkt)
        {
            if (bus == null) return null;

            var values = PacketEncoding.UnPack("b[8] u16", pkt.Data);
            var id = HexEncoding.ToString((byte[])values[0]);
            var port = (ushort)(uint)values[1];
            JDDevice device;
            if (!bus.TryGetDevice(id, out device))
                return null;
            return new OutPipe(device, port);
        }

        public uint Count
        {
            get { return this._count; }
        }

        public bool isOpen
        {
            get { return this.device != null; }
        }

        private void send(byte[] buf)
        {
            this.sendData(buf, 0);
        }

        public void sendMeta(byte[] buf)
        {
            this.sendData(buf, Constants.PIPE_METADATA_MASK);
        }
        private void sendData(byte[] buf, ushort flags)
        {
            if (this.device == null)
                throw new InvalidOperationException("sending data over closed pipe");
            ushort cmd = (ushort)(
                (this.port << Constants.PIPE_PORT_SHIFT) |
                flags |
                (this._count & Constants.PIPE_COUNTER_MASK));
            var pkt = Packet.FromCmd(cmd, buf);
            pkt.RequiresAck = true;
            pkt.ServiceIndex = Constants.JD_SERVICE_INDEX_PIPE;
            try
            {
                this.device.SendPacket(pkt);
            }
            catch (AckException ex)
            {
                this.free();
                throw ex;
            }
            this._count++;
        }

        private void free()
        {
            this.device = null;
            this.port = 0;
        }

        private void close()
        {
            this.sendData(Packet.EmptyData, Constants.PIPE_CLOSE_MASK);
            this.free();
        }

        public void RespondForEach(
            object[] items,
            ObjectEncoder converter
        )
        {
            new Thread(() =>
            {
                try
                {
                    var n = items.Length;
                    for (var i = 0; i < n; ++i)
                    {
                        var item = items[i];
                        var data = converter(item);
                        this.send(data);
                    }
                    this.close();
                }
                catch (AckException)
                {
                    this.LogDebug("pipe failed");
                }
                finally
                {
                    this.free();
                }
            }).Start();
        }
    }

}
