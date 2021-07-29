using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Jacdac.JDPacket;

namespace Jacdac.Transport
{
    public abstract class JDTransport
    {
        protected ulong GetFrameIdentifier(JDFrame frame) => GetFrameIdentifier(frame.DeviceIdentifier, frame.ExpectedCRC);
        protected ulong GetFrameIdentifier(ulong deviceId, ushort crc) => (deviceId & ~(ulong)0xFFFF) | (ulong)crc;

        protected Dictionary<ulong, TaskCompletionSource<JDPacket>> outstandingAcks = new Dictionary<ulong, TaskCompletionSource<JDPacket>>();

        public virtual async Task SendJDPacket(JDPacket packet, int timeout = 100, int retries = 3) {
            var frame = JDFrame.FromPacket(packet);

            OnJacdacPacketSent?.Invoke(frame, packet);

            if (!frame.RequiresAck)
            {
                await InternalSendFrame(frame);
                return;
            }

            var retryCount = 0;

            while(retryCount < retries)
            {
                var tcs = new TaskCompletionSource<JDPacket>();
                lock (outstandingAcks)
                {
                    outstandingAcks.Add(GetFrameIdentifier(frame), tcs);
                }

                await InternalSendFrame(frame);
                var completedTask = await Task.WhenAny(Task.Delay(timeout), tcs.Task);

                lock (outstandingAcks)
                {
                    outstandingAcks.Remove(GetFrameIdentifier(frame));
                }
                
                if (tcs.Task == completedTask)
                    return;         

                retryCount++;
            }

            throw new Exception($"Could not send frame after {retries} retries");
        }

        protected virtual void InternalReceiveFrame(JDFrame frame)
        {
            var packets = frame.ToPackets();
            foreach (var packet in packets.Where(p => p.IsAck))
            {
                lock (outstandingAcks)
                {
                    var ackIdentifier = GetFrameIdentifier(packet.DeviceIdentifier, packet.ServiceCommand);
                    if (outstandingAcks.ContainsKey(ackIdentifier))
                        outstandingAcks[ackIdentifier].TrySetResult(packet);
                }
            }

            foreach (var packet in packets)
                OnJacdacPacket?.Invoke(frame, packet);
        }

        protected abstract Task InternalSendFrame(JDFrame frame);

        public event JacdacPacketEventHandler OnJacdacPacket;
        public event JacdacPacketEventHandler OnJacdacPacketSent;

        public delegate void JacdacPacketEventHandler(JDFrame frame, JDPacket packet);
    }
}
