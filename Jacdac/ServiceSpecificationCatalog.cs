using System;
using System.Diagnostics;
using System.IO;

namespace Jacdac
{
    [Flags]
    [Serializable]
    public enum ServiceRegisterFlag : ushort
    {
        Const = 0x0001,
        Volatile = 0x0002,
    }

    [Serializable]
    public sealed class ServiceRegisterSpec
    {
        public ushort code; // code <= 255 => ro, otherwise rw
        public string name;
        public ServiceRegisterFlag flags;
        public string packf;
        public string[] fields;

        public override string ToString()
        {
            var c = this.code.ToString("x2");
            return $"{this.name} ({c})";
        }
    }

    [Serializable]
    public sealed class ServiceSpec
    {
        public uint serviceClass;
        public string name;
        public ServiceRegisterSpec[] registers;

        public override string ToString()
        {
            var sc = this.serviceClass.ToString("x8");
            return $"{this.name} (0x{sc})";
        }
    }

    public delegate ServiceSpec ServiceSpecReader(byte[] buffer);
    public delegate byte[] ServiceSpecResolver(string url);

    public sealed partial class ServiceSpecificationCatalog : JDNode
    {
        public string Root = "https://microsoft.github.io/jacdac-docs/";
        public readonly IKeyStorage Storage;
        public static ServiceSpecReader SpecificationReader;
        public static ServiceSpecResolver SpecificationResolver;

        private ServiceSpec[] specifications;

        public ServiceSpecificationCatalog(IKeyStorage storage = null)
        {
            Debug.Assert(SpecificationReader != null);
            Debug.Assert(SpecificationResolver != null);

            this.Storage = storage;
            this.specifications = new ServiceSpec[0];
        }

        public bool TryGetSpecification(uint serviceClass, out ServiceSpec spec)
        {
            // in memory cache, need limit?
            var specifications = this.specifications;
            foreach (var specification in specifications)
                if (specification.serviceClass == serviceClass)
                {
                    spec = specification.registers != null ? specification : null;
                    return spec != null;
                }

            spec = null;
            return false;
        }

        public ServiceSpec ResolveSpecification(uint serviceClass)
        {
            lock (this)
            {
                // in memory cache, need limit?
                var specifications = this.specifications;
                foreach (var specification in specifications)
                    if (specification.serviceClass == serviceClass)
                        return specification.registers != null ? specification : null;

                // look cached spec
                var spec = this.ReadFromStorage(serviceClass);
                if (spec == null)
                    spec = this.DownloadSpecification(serviceClass);
                if (spec == null)
                    spec = new ServiceSpec { serviceClass = serviceClass };

                var newSpecs = new ServiceSpec[specifications.Length + 1];
                specifications.CopyTo(newSpecs, 0);
                newSpecs[newSpecs.Length - 1] = spec;
                this.specifications = newSpecs;

                this.RaiseChanged();
                return spec;
            }
        }

        private ServiceSpec ReadFromStorage(uint serviceClass)
        {
            var key = serviceClass.ToString("x8");
            var buffer = this.Storage?.Read(key);
            if (buffer == null) return null;
            else return SpecificationReader(buffer);
        }

        private ServiceSpec DownloadSpecification(uint serviceClass)
        {
            var key = serviceClass.ToString("x8");
            var url = $"http://microsoft.github.io/jacdac-docs/services/twin/x{serviceClass.ToString("x")}.json";
            Debug.WriteLine($"fetch {url}");
            try
            {
                var buffer = SpecificationResolver(url);
                if (buffer != null)
                {
                    var spec = SpecificationReader(buffer);
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

