using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

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

        public bool IsEmpty
        {
            get { return this.registers == null; }
        }

        public static ServiceSpec Empty(uint serviceClass)
        {
            return new ServiceSpec() { serviceClass = serviceClass, registers = null };
        }

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
                    spec = specification.IsEmpty ? null : specification;
                    return spec != null;
                }

            spec = null;
            return false;
        }

        private ServiceSpec FindSpecification(uint serviceClass)
        {
            var specifications = this.specifications;
            foreach (var specification in specifications)
                if (specification.serviceClass == serviceClass)
                    return specification;
            return null;
        }

        public void BeginResolveSpecification(uint serviceClass)
        {
            new Thread(() =>
            {
                this.ResolveSpecification(serviceClass);
            }).Start();
        }

        public ServiceSpec ResolveSpecification(uint serviceClass)
        {
            var spec = this.FindSpecification(serviceClass);
            if (spec != null)
                return spec.IsEmpty ? null : spec;

            lock (this)
            {
                // look cached spec
                spec = this.ReadFromStorage(serviceClass);
                if (spec == null)
                    spec = this.DownloadSpecification(serviceClass);

                var existingSpec = this.FindSpecification(serviceClass);
                if (existingSpec == null)
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
                    if (!spec.IsEmpty)
                        this.Storage?.Write(serviceClass, source);
                    return spec;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return ServiceSpec.Empty(serviceClass);
        }
    }
}

