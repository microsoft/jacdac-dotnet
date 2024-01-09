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
        /// const [cpuTemp] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        CpuTemp = 0x191,

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
        /// const [usage, temp] = jdunpack<[number, number]>(buf, "u8 u8")
        /// ```
        /// </summary>
        GpuInfo = 0x193,

        /// <summary>
        /// Network transmit/receive speed in Kbytes per second.
        ///
        /// ```
        /// const [tx, rx] = jdunpack<[number, number]>(buf, "u16 u16")
        /// ```
        /// </summary>
        NetInfo = 0x195,
    }

    public static class PCMonitorRegPack {
        /// <summary>
        /// Pack format for 'cpu_usage' data.
        /// </summary>
        public const string CpuUsage = "u8";

        /// <summary>
        /// Pack format for 'cpu_temp' data.
        /// </summary>
        public const string CpuTemp = "u8";

        /// <summary>
        /// Pack format for 'ram_usage' data.
        /// </summary>
        public const string RamUsage = "u8";

        /// <summary>
        /// Pack format for 'gpu_info' data.
        /// </summary>
        public const string GpuInfo = "u8 u8";

        /// <summary>
        /// Pack format for 'net_info' data.
        /// </summary>
        public const string NetInfo = "u16 u16";
    }

}
