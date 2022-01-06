/** Autogenerated file. Do not edit. */
using Jacdac;
using System;

namespace Jacdac {

    /// <summary>
    /// A sensor approximating the heart rate. 
     /// 
     /// 
     /// **Jacdac is NOT suitable for medical devices and should NOT be used in any kind of device to diagnose or treat any medical conditions.**
    /// Implements a client for the Heart Rate service.
    /// </summary>
    /// <seealso cref="https://microsoft.github.io/jacdac-docs/services/heartrate/" />
    public partial class HeartRateClient : SensorClient
    {
        public HeartRateClient(JDBus bus, string name)
            : base(bus, name, ServiceClasses.HeartRate)
        {
        }

        /// <summary>
        /// The estimated heart rate., _: bpm
        /// </summary>
        public float HeartRate
        {
            get
            {
                return (float)this.GetRegisterValue((ushort)HeartRateReg.HeartRate, HeartRateRegPack.HeartRate);
            }
        }

        /// <summary>
        /// (Optional) The estimated error on the reported sensor data., _: bpm
        /// </summary>
        public float HeartRateError
        {
            get
            {
                return (float)this.GetRegisterValue((ushort)HeartRateReg.HeartRateError, HeartRateRegPack.HeartRateError);
            }
        }

        /// <summary>
        /// (Optional) The type of physical sensor, 
        /// </summary>
        public HeartRateVariant Variant
        {
            get
            {
                return (HeartRateVariant)this.GetRegisterValue((ushort)HeartRateReg.Variant, HeartRateRegPack.Variant);
            }
        }


    }
}