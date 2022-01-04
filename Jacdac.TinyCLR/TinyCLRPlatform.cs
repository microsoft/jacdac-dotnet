using GHIElectronics.TinyCLR.Data.Json;
using GHIElectronics.TinyCLR.Devices.Jacdac.Transport;
using GHIElectronics.TinyCLR.Native;
using Jacdac.TinyCLR.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Jacdac
{
    public partial class Platform
    {
        static Platform()
        {
            Platform.Crc16 = JacdacSerialWireController.Crc;
            var id = DeviceInformation.GetUniqueId();
            // TODO: compress device id into 8 bytes
            var jid = Util.Slice(id, 0, 8);
            Platform.DeviceId = jid;
            for (var i = 8; i < id.Length; ++i)
            {
                jid[i % jid.Length] |= (byte)(id[i] << 4);
            }
            Platform.DeviceId = jid;
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
        }
    }

    public partial class ServiceSpecificationCatalog
    {
        static ServiceSpecificationCatalog()
        {
            SpecificationReader = ServiceTwinReader;
            SpecificationResolver = WebGet;
        }

        static ServiceSpec ServiceTwinReader(byte[] buffer)
        {
            try
            {
                var text = System.Text.UTF8Encoding.UTF8.GetString(buffer);
                return (ServiceSpec)JsonConverter.DeserializeObject(text, typeof(ServiceSpec),
                    (string instancePath, JToken token, Type baseType, string fieldName, int length) =>
                    {
                        if (instancePath == "/")
                            return new ServiceSpec();
                        return null;
                    });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        static byte[] WebGet(string url)
        {
            var certificates = Resources.GetBytes(Resources.BinaryResources.GitHubCertificate);
            var certx509 = new X509Certificate[] { new X509Certificate(certificates) };
            using (var req = HttpWebRequest.Create(url) as HttpWebRequest)
            {
                req.KeepAlive = false;
                req.ReadWriteTimeout = 2000;
                req.Headers.Add("Accept", "application/json");
                //req.HttpsAuthentCerts = certx509;
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
        }
    }
}