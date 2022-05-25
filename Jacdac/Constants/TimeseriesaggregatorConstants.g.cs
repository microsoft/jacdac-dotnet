namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint TimeseriesAggregator = 0x1192bdcc;
    }

    public enum TimeseriesAggregatorDataMode: byte { // uint8_t
        Continuous = 0x1,
        Discrete = 0x2,
    }

    public enum TimeseriesAggregatorCmd : ushort {
        /// <summary>
        /// No args. Remove all pending timeseries.
        /// </summary>
        Clear = 0x80,

        /// <summary>
        /// Starts a new timeseries.
        /// `service_number` is the number of services with the same `service_class`
        /// and lower service index on `sensor_id`.
        /// If `sensor_id` or `service_class` are unknown they can be `0`.
        /// If label is missing, it can be empty string.
        /// As for `mode`,
        /// `Continuous` has default aggregation window of 60s,
        /// and `Discrete` only stores the data if it has changed since last store,
        /// and has default window of 1s.
        ///
        /// ```
        /// const [id, serviceClass, sensorId, serviceNumber, mode, label] = jdunpack<[number, number, Uint8Array, number, TimeseriesAggregatorDataMode, string]>(buf, "u32 u32 b[8] u8 u8 s")
        /// ```
        /// </summary>
        StartTimeseries = 0x81,

        /// <summary>
        /// Add a data point to a timeseries.
        ///
        /// ```
        /// const [value, id] = jdunpack<[number, number]>(buf, "f64 u32")
        /// ```
        /// </summary>
        Update = 0x83,

        /// <summary>
        /// Set aggregation window.
        ///
        /// ```
        /// const [id, duration] = jdunpack<[number, number]>(buf, "u32 u32")
        /// ```
        /// </summary>
        SetWindow = 0x84,

        /// <summary>
        /// Indicates that the average, minimum and maximum value of a given
        /// timeseries are as indicated.
        /// It also says how many samples were collected, and the collection period.
        /// Timestamps are given using device's internal clock, which will wrap around.
        /// Typically, `end_time` can be assumed to be "now".
        ///
        /// ```
        /// const [id, numSamples, avg, min, max, startTime, endTime] = jdunpack<[number, number, number, number, number, number, number]>(buf, "u32 u32 f64 f64 f64 u32 u32")
        /// ```
        /// </summary>
        Stored = 0x85,
    }

    public static class TimeseriesAggregatorCmdPack {
        /// <summary>
        /// Pack format for 'start_timeseries' register data.
        /// </summary>
        public const string StartTimeseries = "u32 u32 b[8] u8 u8 s";

        /// <summary>
        /// Pack format for 'update' register data.
        /// </summary>
        public const string Update = "f64 u32";

        /// <summary>
        /// Pack format for 'set_window' register data.
        /// </summary>
        public const string SetWindow = "u32 u32";

        /// <summary>
        /// Pack format for 'stored' register data.
        /// </summary>
        public const string Stored = "u32 u32 f64 f64 f64 u32 u32";
    }

    public enum TimeseriesAggregatorReg : ushort {
        /// <summary>
        /// Read-only ms uint32_t. This register is automatically broadcast and can be also queried to establish local time on the device.
        ///
        /// ```
        /// const [now] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        Now = 0x180,
    }

    public static class TimeseriesAggregatorRegPack {
        /// <summary>
        /// Pack format for 'now' register data.
        /// </summary>
        public const string Now = "u32";
    }

}
