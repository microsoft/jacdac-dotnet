/** Autogenerated file. Do not edit. */
using Jacdac;
using System;

namespace Jacdac.Clients {

    /// <summary>
    /// A sensor that detects light and dark surfaces, commonly used for line following robots.
    /// Implements a client for the Reflected light service.
    /// </summary>
    /// <seealso cref="https://microsoft.github.io/jacdac-docs/services/reflectedlight/" />
    public partial class ReflectedLightClient : SensorClient
    {
        public ReflectedLightClient(JDBus bus, string name)
            : base(bus, name, ServiceClasses.ReflectedLight)
        {
        }

        /// <summary>
        /// Reports the reflected brightness. It may be a digital value or, for some sensor, analog value., _: /
        /// </summary>
        public float Brightness
        {
            get
            {
                return (float)this.GetRegisterValue((ushort)ReflectedLightReg.Brightness, ReflectedLightRegPack.Brightness);
            }
        }

        /// <summary>
        /// (Optional) Type of physical sensor used, 
        /// </summary>
        public ReflectedLightVariant Variant
        {
            get
            {
                return (ReflectedLightVariant)this.GetRegisterValue((ushort)ReflectedLightReg.Variant, ReflectedLightRegPack.Variant);
            }
        }


    }
}