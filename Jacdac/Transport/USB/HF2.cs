using Jacdac;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jacdac.Transport
{
    internal class HF2
    {
        internal enum HF2BootMode
        {
            Bootloader = 1,
            Application = 2
        }

        private short sequentialNumber = 0;
        private ConcurrentQueue<HF2Response> responses =  new ConcurrentQueue<HF2Response>();
        private USBTransport usbTransport;
        private bool inMultiPartResponse = false;
        private HF2Response currentResponse;

        public HF2(USBTransport usbTransport)
        {
            this.usbTransport = usbTransport;
        }

        public async Task<byte[]> SendCommand(int commandId, byte[]? data = null, bool waitForResponse = true)
        {
            if (data != null && data.Length > 54)
                throw new NotSupportedException("Data longer than 54 bytes currently not supported");

            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            var currentCommandTag = sequentialNumber++;

            bw.Write(commandId);
            bw.Write(currentCommandTag);
            bw.Write((short)0);
            if(data != null)
                bw.Write(data);
            bw.Flush();

            var commandPayload = ms.ToArray();
            bw.Dispose();
            ms.Dispose();

            var packet = new HF2Packet(HF2PacketType.FinalCommandPacket, commandPayload);
            var packetData = packet.ToByteArray();
            await usbTransport.SendData(packetData);

            if (!waitForResponse)
                return Array.Empty<byte>();

            HF2Response response;
            do
            {
                var dequeueSuccess = responses.TryDequeue(out response);
                if (dequeueSuccess)
                {
                    break;
                }

                await Task.Delay(10);
            } while (true);

            if (response.Status != HF2Response.HF2CommandResponseStatus.Success)
                throw new Exception($"Command failed with status {response.Status}");

            return response.Payload;
        }

        public async Task<HF2BootMode> GetBootMode()
        {
            var commandResponse = await SendCommand(0x0001);
            return (HF2BootMode) commandResponse[0];
        }

        public async Task ResetIntoApp()
        {
            await SendCommand(0x0003, Array.Empty<byte>(), false);
        }

        public async Task ResetIntoBootloader()
        {
            await SendCommand(0x0004, Array.Empty<byte>(), false);
        }

        public async Task<string> GetDeviceInfo()
        {
            var commandResponse = await SendCommand(2);
            return Encoding.Default.GetString(commandResponse);
        }

        public async Task<string> GetLogBuffer()
        {
            var commandResponse = await SendCommand(0x0010);
            return Encoding.Default.GetString(commandResponse);
        }

        private void HandleEventPacket(int eventId, byte[] payload)
        {
            if(eventId == 0x20)
            {
                OnJacdacMessage?.Invoke(payload);
            }
        }

        public void ProcessPacket(byte[] data)
        {
            var packet = HF2Packet.Parse(data);

            if (packet.PacketType == HF2PacketType.SerialStdout || packet.PacketType == HF2PacketType.SerialStderr)
            {
                Console.WriteLine($"SERIAL> {Encoding.Default.GetString(packet.Payload)}");
                return;
            }

            if(packet.PacketType == HF2PacketType.FinalCommandPacket && (HF2Response.HF2CommandResponseStatus)packet.Payload[2] == HF2Response.HF2CommandResponseStatus.Event)
            {
                HandleEventPacket(packet.Payload[0] | packet.Payload[1] << 8, packet.Payload.Skip(4).ToArray());
                return;
            }

            if (!inMultiPartResponse)
                currentResponse = HF2Response.Parse(packet.Payload);
            else
                currentResponse.AppendPayload(packet.Payload);

            inMultiPartResponse = true;

            if(packet.PacketType == HF2PacketType.FinalCommandPacket)
            {
                currentResponse.Complete();
                inMultiPartResponse = false;
                responses.Enqueue(currentResponse);
            }
        }

        public delegate void HF2EventArgs(byte[] data);
        public event HF2EventArgs OnJacdacMessage;
    }
}
