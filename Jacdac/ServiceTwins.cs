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
        public readonly IKeyStorage Storage;
        public readonly ServiceTwinSpecReader SpecificationReader;

        public ServiceTwins(IKeyStorage storage)
        {
            this.Storage = storage;
            this.SpecificationReader = Platform.ServiceTwinReader;
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
            var buffer = this.Storage?.Read(key);
            if (buffer == null) return null;
            else return this.SpecificationReader(buffer);
        }

        private ServiceTwinSpec DownloadSpecification(uint serviceClass)
        {
            var key = serviceClass.ToString("x8");
            var url = $"http://microsoft.github.io/jacdac-docs/services/twin/x{serviceClass.ToString("x8")}.json";
            Debug.WriteLine($"fetch {url}");
            try
            {
                var buffer = Platform.WebGet(url);
                if (buffer != null)
                {
                    var spec = this.SpecificationReader(buffer);
                    this.Storage?.Write(key, buffer);
                    return spec;
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

