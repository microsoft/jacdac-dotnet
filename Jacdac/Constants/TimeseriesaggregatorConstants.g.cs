namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint TimeseriesAggregator = 0x1192bdcc;
    }
    public enum TimeseriesAggregatorCmd : ushort {
        /// <summary>
        /// No args. Remove all pending timeseries.
        /// </summary>
        Clear = 0x80,

        /// <summary>
        /// Add a data point to a timeseries.
        ///
        /// ```
        /// const [value, label] = jdunpack<[number, string]>(buf, "f64 s")
        /// ```
        /// </summary>
        Update = 0x83,

        /// <summary>
        /// Set aggregation window.
        /// Setting to `0` will restore default.
        ///
        /// ```
        /// const [duration, label] = jdunpack<[number, string]>(buf, "u32 s")
        /// ```
        /// </summary>
        SetWindow = 0x84,

        /// <summary>
        /// Set whether or not the timeseries will be uploaded to the cloud.
        /// The `stored` reports are generated regardless.
        ///
        /// ```
        /// const [upload, label] = jdunpack<[number, string]>(buf, "u8 s")
        /// ```
        /// </summary>
        SetUpload = 0x85,

        /// <summary>
        /// Indicates that the average, minimum and maximum value of a given
        /// timeseries are as indicated.
        /// It also says how many samples were collected, and the collection period.
        /// Timestamps are given using device's internal clock, which will wrap around.
        /// Typically, `end_time` can be assumed to be "now".
        /// `end_time - start_time == window`
        ///
        /// ```
        /// const [numSamples, avg, min, max, startTime, endTime, label] = jdunpack<[number, number, number, number, number, number, string]>(buf, "u32 x[4] f64 f64 f64 u32 u32 s")
        /// ```
        /// </summary>
        Stored = 0x90,
    }

    public static class TimeseriesAggregatorCmdPack {
        /// <summary>
        /// Pack format for 'update' register data.
        /// </summary>
        public const string Update = "f64 s";

        /// <summary>
        /// Pack format for 'set_window' register data.
        /// </summary>
        public const string SetWindow = "u32 s";

        /// <summary>
        /// Pack format for 'set_upload' register data.
        /// </summary>
        public const string SetUpload = "u8 s";

        /// <summary>
        /// Pack format for 'stored' register data.
        /// </summary>
        public const string Stored = "u32 b[4] f64 f64 f64 u32 u32 s";
    }

    public enum TimeseriesAggregatorReg : ushort {
        /// <summary>
        /// Read-only μs uint32_t. This can queried to establish local time on the device.
        ///
        /// ```
        /// const [now] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        Now = 0x180,

        /// <summary>
        /// Read-write bool (uint8_t). When `true`, the windows will be shorter after service reset and gradually extend to requested length.
        /// This is ensure valid data is being streamed in program development.
        ///
        /// ```
        /// const [fastStart] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        FastStart = 0x80,

        /// <summary>
        /// Read-write ms uint32_t. Window for timeseries for which `set_window` was never called.
        /// Note that windows returned initially may be shorter if `fast_start` is enabled.
        ///
        /// ```
        /// const [defaultWindow] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        DefaultWindow = 0x81,

        /// <summary>
        /// Read-write bool (uint8_t). Whether labelled timeseries for which `set_upload` was never called should be automatically uploaded.
        ///
        /// ```
        /// const [defaultUpload] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        DefaultUpload = 0x82,

        /// <summary>
        /// Read-write bool (uint8_t). Whether automatically created timeseries not bound in role manager should be uploaded.
        ///
        /// ```
        /// const [uploadUnlabelled] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        UploadUnlabelled = 0x83,

        /// <summary>
        /// Read-write ms uint32_t. If no data is received from any sensor within given period, the device is rebooted.
        /// Set to `0` to disable (default).
        /// Updating user-provided timeseries does not reset the watchdog.
        ///
        /// ```
        /// const [sensorWatchdogPeriod] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        SensorWatchdogPeriod = 0x84,
    }

    public static class TimeseriesAggregatorRegPack {
        /// <summary>
        /// Pack format for 'now' register data.
        /// </summary>
        public const string Now = "u32";

        /// <summary>
        /// Pack format for 'fast_start' register data.
        /// </summary>
        public const string FastStart = "u8";

        /// <summary>
        /// Pack format for 'default_window' register data.
        /// </summary>
        public const string DefaultWindow = "u32";

        /// <summary>
        /// Pack format for 'default_upload' register data.
        /// </summary>
        public const string DefaultUpload = "u8";

        /// <summary>
        /// Pack format for 'upload_unlabelled' register data.
        /// </summary>
        public const string UploadUnlabelled = "u8";

        /// <summary>
        /// Pack format for 'sensor_watchdog_period' register data.
        /// </summary>
        public const string SensorWatchdogPeriod = "u32";
    }

}
