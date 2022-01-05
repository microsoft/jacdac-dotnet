namespace Jacdac.Clients
{
    public sealed partial class HumidityClient : SensorClient
    {
        readonly float defaultHumidity;

        public HumidityClient(JDBus bus, string name, float defaultHumidity = -1)
            : base(bus, name, Jacdac.HumidityConstants.ServiceClass)
        {
            this.defaultHumidity = defaultHumidity;
        }

        /// <summary>
        /// The relative humidity in percentage of full water saturation., _: %RH
        /// </summary>
        public float Humidity
        {
            get
            {
                return (float)this.GetRegisterValue((ushort)HumidityReg.Humidity, HumidityRegPack.Humidity, this.defaultHumidity);
            }
        }

        /// <summary>
        /// (Optional) The real humidity is between `humidity - humidity_error` and `humidity + humidity_error`., _: %RH
        /// </summary>
        /// <returns>a float, or null if not available.</returns>
        public object HumidityError
        {
            get
            {
                return this.GetRegisterValue((ushort)HumidityReg.Humidity, HumidityRegPack.Humidity);
            }
        }
    }
}
