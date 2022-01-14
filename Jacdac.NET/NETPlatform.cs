using DeviceId;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;

namespace Jacdac
{
    public partial class Platform
    {
        static Platform()
        {
            var deviceId = new DeviceIdBuilder()
                .AddMachineName()
                .AddOsVersion()
                .ToString();
            var sha = System.Security.Cryptography.SHA1.Create();
            sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(deviceId));
            var jid = sha.Hash.Take(8).ToArray();
            Platform.DeviceId = jid;
            Platform.RealTimeClock = RealTimeClockVariant.Computer;
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

        static ServiceSpec ServiceTwinReader(string s)
        {
            try
            {
                return JsonSerializer.Deserialize<ServiceSpec>(s, new JsonSerializerOptions
                {
                    AllowTrailingCommas = true,
                    IncludeFields = true
                });
            }
            catch (System.Text.Json.JsonException ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        static string WebGet(string url)
        {
            try
            {
                var req = HttpWebRequest.Create(url) as HttpWebRequest;
                {
                    req.KeepAlive = false;
                    req.ReadWriteTimeout = 2000;
                    req.Headers[HttpRequestHeader.Accept] = "application/json";
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
                                    mem.Write(buf, 0, read);
                                } while (read > 0);
                                var bytes = mem.ToArray();
                                return System.Text.UTF8Encoding.UTF8.GetString(bytes);
                            }
                    }
                }
            }
            catch (System.Net.WebException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }
    }
}