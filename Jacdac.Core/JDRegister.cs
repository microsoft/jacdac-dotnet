using Jacdac.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jacdac
{
    public class JDRegister<T> : JDRegister
    {
        private const int REFRESH_TIMEOUT_MS = 500;

        private T cachedValue;

        public T Value => GetValue().GetAwaiter().GetResult();

        public DateTime LastFetched { get; private set; }

        public JDRegister(ushort address, RegisterFlags flags, JDService service) : base(address, flags, service)
        {
        }

        public async Task<T> GetValue(bool forceRefresh = false)
        {
            var requiresRefresh = forceRefresh;

            if ((DateTime.Now - LastFetched).TotalMilliseconds > REFRESH_TIMEOUT_MS)
                requiresRefresh = true;

            if (requiresRefresh)
                cachedValue = await FetchValue();

            return cachedValue;
        } 

        private async Task<T> FetchValue()
        {
            var resultBuffer = await Service.ReadRegister(Address);
            // Parse the result buffer here
            return default(T);
        }
    }

    public class JDRegister
    {
        public JDRegister(ushort address, RegisterFlags flags, JDService service)
        {
            Address = address;
            Service = service;
            Flags = flags;
        }

        public ushort Address { get; }
        public JDService Service { get; }

        public RegisterFlags Flags { get; }
    }

    [Flags]
    public enum RegisterFlags
    {
        Read,
        Write,
        Const,
        Client
    }
}
