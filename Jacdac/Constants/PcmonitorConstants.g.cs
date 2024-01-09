namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint PCMonitor = 0x18627b15;
    }
    public enum PCMonitorReg : ushort {
        /// <summary>
        /// Read-only % uint8_t. CPU usage in percent.
        ///
        /// ```
        /// const [cpuUsage] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        CpuUsage = 0x190,

        /// <summary>
        /// Read-only °C uint8_t. CPU temperature in Celsius.
        ///
        /// ```
        /// const [cpuTemperature] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        CpuTemperature = 0x191,

        /// <summary>
        /// Read-only % uint8_t. RAM usage in percent.
        ///
        /// ```
        /// const [ramUsage] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        RamUsage = 0x192,

        /// <summary>
        /// GPU info.
        ///
        /// ```
        /// const [usage, temperature] = jdunpack<[number, number]>(buf, "u8 u8")
        /// ```
        /// </summary>
        GpuInformation = 0x193,

        /// <summary>
        /// Network transmit/receive speed in Kbytes per second.
        ///
        /// ```
        /// const [tx, rx] = jdunpack<[number, number]>(buf, "u16 u16")
        /// ```
        /// </summary>
        NetworkInformation = 0x195,
    }

    public static class PCMonitorRegPack {
        /// <summary>
        /// Pack format for 'cpu_usage' data.
        /// </summary>
        public const string CpuUsage = "u8";

        /// <summary>
        /// Pack format for 'cpu_temperature' data.
        /// </summary>
        public const string CpuTemperature = "u8";

        /// <summary>
        /// Pack format for 'ram_usage' data.
        /// </summary>
        public const string RamUsage = "u8";

        /// <summary>
        /// Pack format for 'gpu_information' data.
        /// </summary>
        public const string GpuInformation = "u8 u8";

        /// <summary>
        /// Pack format for 'network_information' data.
        /// </summary>
        public const string NetworkInformation = "u16 u16";
    }

}
