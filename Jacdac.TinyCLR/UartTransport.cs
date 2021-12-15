using GHIElectronics.TinyCLR.Data.Json;
using GHIElectronics.TinyCLR.Native;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Jacdac
{
    public sealed class UartTransport : Transport
    {
        static UartTransport()
        {
            Platform.Crc16 = GHIElectronics.TinyCLR.Devices.Jacdac.Util.CRC;
            var id = DeviceInformation.GetUniqueId();
            // TODO: compress device id into 8 bytes
            Platform.DeviceId = Util.Slice(id, 0, 8);
            Platform.DeviceDescription = DeviceInformation.DeviceName;
            var version = DeviceInformation.Version;
            var major = (ushort)((version >> 48) & 0xFFFF);
            var minor = (ushort)((version >> 32) & 0xFFFF);
            var build = (ushort)((version >> 16) & 0xFFFF);
            var revision = (ushort)((version >> 0) & 0xFFFF);
            Platform.FirmwareVersion = major + "." + minor + "." + build + "." + revision;
            Platform.RealTimeClock = RealTimeClockVariant.Crystal;
            Platform.CreateClock = () =>
            {
                var start = DateTime.Now;
                return () => DateTime.Now - start;
            };
            Platform.ServiceTwinReader = (byte[] buffer) =>
            {
                try
                {
                    var text = System.Text.UTF8Encoding.UTF8.GetString(buffer);
                    return (ServiceTwinSpec)JsonConverter.DeserializeObject(text, typeof(ServiceTwinSpec),
                        (string instancePath, JToken token, Type baseType, string fieldName, int length) =>
                            {
                                if (instancePath == "/")
                                    return new ServiceTwinSpec();
                                return null;
                            });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            };
            Platform.WebGet = (string url) =>
            {
                var certx509 = new X509Certificate[] { new X509Certificate(certificates) };
                using (var req = HttpWebRequest.Create(url) as HttpWebRequest)
                {
                    req.KeepAlive = false;
                    req.ReadWriteTimeout = 2000;
                    req.HttpsAuthentCerts = certx509;
                    using (var res = req.GetResponse() as HttpWebResponse)
                    {
                        if (res.StatusCode == HttpStatusCode.OK)
                            using (var stream = res.GetResponseStream())
                            {
                                var mem = new MemoryStream();
                                var read = 0;
                                var buf = new byte[512];
                                do
                                {
                                    read = stream.Read(buf, 0, buf.Length);
                                    mem.Write(buf, 0, buf.Length);
                                } while (read != 0);
                                return mem.ToArray();
                            }
                    }
                }
                return null;
            };
        }

        public readonly GHIElectronics.TinyCLR.Devices.Jacdac.JacdacController controller;

        public UartTransport(GHIElectronics.TinyCLR.Devices.Jacdac.JacdacController controller)
        {
            this.controller = controller;
        }

        public override event FrameReceivedEvent FrameReceived
        {
            add
            {
                this.controller.PacketReceived += (GHIElectronics.TinyCLR.Devices.Jacdac.JacdacController sender, GHIElectronics.TinyCLR.Devices.Jacdac.Packet packet) =>
                {
                    var pkt = Packet.FromBinary(packet.ToBuffer(), true);
                    var frame = Packet.ToFrame(new Packet[] { pkt });
                    value(this, frame);
                };
            }
            remove
            {
                throw new InvalidOperationException();
                // not supported
            }
        }


        public override event TransportErrorReceivedEvent ErrorReceived
        {
            add
            {
                this.controller.ErrorReceived += (GHIElectronics.TinyCLR.Devices.Jacdac.JacdacController sender, GHIElectronics.TinyCLR.Devices.Jacdac.ErrorReceivedEventArgs args) =>
                {
                    value(this, new TransportErrorReceivedEventArgs((TransportError)(uint)args.Error, args.Timestamp, args.Data));
                };
            }
            remove
            {
                throw new InvalidOperationException();
                // not supported
            }
        }

        protected override void InternalConnect()
        {
            this.controller.Enable();
        }

        protected override void InternalDisconnect()
        {
            this.controller.Disable();
        }

        public override void Dispose()
        {
            this.controller.Dispose();
        }

        public override void SendFrame(byte[] data)
        {
            this.controller.SendData(data);
        }
    }
}