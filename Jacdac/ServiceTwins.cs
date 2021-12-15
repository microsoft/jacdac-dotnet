using System;
using System.Diagnostics;
using System.IO;

namespace Jacdac
{
    [Flags]
    [Serializable]
    public enum ServiceTwinRegisterFlag : ushort
    {
        Const = 0x0001,
        Volatile = 0x0002,
    }

    [Serializable]
    public sealed class ServiceTwinRegisterSpec
    {
        public ushort code; // code <= 255 => ro, otherwise rw
        public string name;
        public ServiceTwinRegisterFlag flags;
        public string packf;
        public string[] fields;
    }

    [Serializable]
    public sealed class ServiceTwinSpec
    {
        public uint serviceClass;
        public string name;
        public ServiceTwinRegisterSpec[] registers;
    }

    public delegate ServiceTwinSpec ServiceTwinSpecReader(byte[] buffer);


    public sealed class ServiceTwins
    {
        public string Root = "https://microsoft.github.io/jacdac-docs/";
        public readonly IWebClient WebClient;
        public readonly IKeyStorage Storage;
        public readonly ServiceTwinSpecReader SpecificationReader;

        public ServiceTwins(IWebClient webClient, IKeyStorage storage, ServiceTwinSpecReader specificationReader)
        {
            this.WebClient = webClient;
            this.Storage = storage;
            this.SpecificationReader = specificationReader;
        }

        public ServiceTwinSpec ResolveSpecification(uint serviceClass)
        {
            var spec = this.ReadFromStorage(serviceClass);
            if (spec == null)
                spec = this.DownloadSpecification(serviceClass);
            return spec;
        }

        private ServiceTwinSpec ReadFromStorage(uint serviceClass)
        {
            var key = serviceClass.ToString("x8");
            var buffer = this.Storage.Read(key);
            if (buffer == null) return null;
            else return this.SpecificationReader(buffer);
        }

        private ServiceTwinSpec DownloadSpecification(uint serviceClass)
        {
            var key = serviceClass.ToString("x8");
            var url = $"https://microsoft.github.io/jacdac-docs/services/twin/x{serviceClass.ToString("x8")}.json";
            Debug.WriteLine($"fetch {url}");
            try
            {
                var buffer = this.WebClient.Get(url);
                if (buffer != null)
                {
                    this.Storage.Write(key, buffer);
                    return this.ReadFromStorage(serviceClass);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }
    }
}

