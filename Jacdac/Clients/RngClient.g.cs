/** Autogenerated file. Do not edit. */
using Jacdac;
using System;

namespace Jacdac.Clients 
{
    /// <summary>
    /// Generates random numbers using entropy sourced from physical processes.
     /// 
     /// This typically uses a cryptographical pseudo-random number generator (for example [Fortuna](<https://en.wikipedia.org/wiki/Fortuna_(PRNG)>)),
     /// which is periodically re-seeded with entropy coming from some hardware source.
    /// Implements a client for the Random Number Generator service.
    /// </summary>
    /// <seealso cref="https://microsoft.github.io/jacdac-docs/services/rng/" />
    public partial class RngClient : Client
    {
        public RngClient(JDBus bus, string name)
            : base(bus, name, ServiceClasses.Rng)
        {
        }

        /// <summary>
        /// Reads the <c>random</c> register value.
        /// A register that returns a 64 bytes random buffer on every request.
        /// This never blocks for a long time. If you need additional random bytes, keep querying the register., 
        /// </summary>
        public byte[] Random
        {
            get
            {
                return (byte[])this.GetRegisterValue((ushort)RngReg.Random, RngRegPack.Random);
            }
        }

        /// <summary>
        /// Tries to read the <c>variant</c> register value.
        /// The type of algorithm/technique used to generate the number.
        /// `Quantum` refers to dedicated hardware device generating random noise due to quantum effects.
        /// `ADCNoise` is the noise from quick readings of analog-digital converter, which reads temperature of the MCU or some floating pin.
        /// `WebCrypto` refers is used in simulators, where the source of randomness comes from an advanced operating system., 
        /// </summary>
        bool TryGetVariant(out RngVariant value)
        {
            object[] values;
            if (this.TryGetRegisterValues((ushort)RngReg.Variant, RngRegPack.Variant, out values)) 
            {
                value = (RngVariant)values[0];
                return true;
            }
            else
            {
                value = default(RngVariant);
                return false;
            }
        }


    }
}