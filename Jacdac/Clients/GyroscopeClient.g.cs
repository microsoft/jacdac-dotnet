/** Autogenerated file. Do not edit. */
using Jacdac;
using System;

namespace Jacdac {

    /// <summary>
    /// A 3-axis gyroscope.
    /// Implements a client for the Gyroscope service.
    /// </summary>
    /// <seealso cref="https://microsoft.github.io/jacdac-docs/services/gyroscope/" />
    public partial class GyroscopeClient : SensorClient
    {
        public GyroscopeClient(JDBus bus, string name)
            : base(bus, name, ServiceClasses.Gyroscope)
        {
        }

        /// <summary>
        /// Indicates the current rates acting on gyroscope., x: °/s,y: °/s,z: °/s
        /// </summary>
        public object[] /*(float, float, float)*/ RotationRates
        {
            get
            {
                return (object[] /*(float, float, float)*/)this.GetRegisterValue((ushort)GyroscopeReg.RotationRates, GyroscopeRegPack.RotationRates);
            }
        }

        /// <summary>
        /// (Optional) Error on the reading value., _: °/s
        /// </summary>
        public float RotationRatesError
        {
            get
            {
                return (float)this.GetRegisterValue((ushort)GyroscopeReg.RotationRatesError, GyroscopeRegPack.RotationRatesError);
            }
        }

        /// <summary>
        /// (Optional) Configures the range of rotation rates.
        /// The value will be "rounded up" to one of `max_rates_supported`., _: °/s
        /// </summary>
        public float MaxRate
        {
            get
            {
                return (float)this.GetRegisterValue((ushort)GyroscopeReg.MaxRate, GyroscopeRegPack.MaxRate);
            }
            set
            {
                
                this.SetRegisterValue((ushort)GyroscopeReg.MaxRate, GyroscopeRegPack.MaxRate, value);
            }

        }


    }
}