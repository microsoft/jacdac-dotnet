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
            return $"{this.name} (0x{c})";
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

    public delegate ServiceSpec ServiceSpecReader(string source);
    public delegate string ServiceSpecResolver(string url);

    public interface ISpecificationStorage
    {
        uint[] GetServiceClasses();
        string Read(uint serviceClass);
        void Write(uint serviceClass, string spec);

        void Clear();
    }

    public sealed partial class ServiceSpecificationCatalog : JDNode
    {
        public string Root = "https://microsoft.github.io/jacdac-docs/";
        public readonly ISpecificationStorage Storage;
        public static ServiceSpecReader SpecificationReader;
        public static ServiceSpecResolver SpecificationResolver;

        private ServiceSpec[] specifications;

        public ServiceSpecificationCatalog(ISpecificationStorage storage = null)
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
            ServiceSpec spec;
            if (this.TryGetSpecification(serviceClass, out spec))
                return spec;

            lock (this)
            {
                // look cached spec
                spec = this.ReadFromStorage(serviceClass);
                if (spec == null)
                    spec = this.DownloadSpecification(serviceClass);

                ServiceSpec existingSpec;
                if (spec != null && !this.TryGetSpecification(serviceClass, out existingSpec))
                {
                    var newSpecs = new ServiceSpec[specifications.Length + 1];
                    specifications.CopyTo(newSpecs, 0);
                    newSpecs[newSpecs.Length - 1] = spec;
                    this.specifications = newSpecs;
                    this.RaiseChanged();
                }

                return spec;
            }
        }

        private ServiceSpec ReadFromStorage(uint serviceClass)
        {
            var buffer = this.Storage?.Read(serviceClass);
            if (buffer == null) return null;
            else return SpecificationReader(buffer);
        }

        private ServiceSpec DownloadSpecification(uint serviceClass)
        {
            var url = $"https://microsoft.github.io/jacdac-docs/services/twin/x{serviceClass.ToString("x1")}.json";
            Debug.WriteLine($"fetch {url}");
            try
            {
                var source = SpecificationResolver(url);
                if (source != null)
                {
                    var spec = SpecificationReader(source);
                    this.Storage?.Write(serviceClass, source);
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

