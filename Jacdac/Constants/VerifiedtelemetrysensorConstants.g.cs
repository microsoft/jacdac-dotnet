namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint VerifiedTelemetry = 0x2194841f;
    }

    public enum VerifiedTelemetryStatus: byte { // uint8_t
        Unknown = 0x0,
        Working = 0x1,
        Faulty = 0x2,
    }


    public enum VerifiedTelemetryFingerprintType: byte { // uint8_t
        FallCurve = 0x1,
        CurrentSense = 0x2,
        Custom = 0x3,
    }

    public enum VerifiedTelemetryReg : ushort {
        /// <summary>
        /// Read-only Status (uint8_t). Reads the telemetry working status, where ``true`` is working and ``false`` is faulty.
        ///
        /// ```
        /// const [telemetryStatus] = jdunpack<[VerifiedTelemetryStatus]>(buf, "u8")
        /// ```
        /// </summary>
        TelemetryStatus = 0x180,

        /// <summary>
        /// Read-write ms uint32_t. Specifies the interval between computing the fingerprint information.
        ///
        /// ```
        /// const [telemetryStatusInterval] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        TelemetryStatusInterval = 0x80,

        /// <summary>
        /// Constant FingerprintType (uint8_t). Type of the fingerprint.
        ///
        /// ```
        /// const [fingerprintType] = jdunpack<[VerifiedTelemetryFingerprintType]>(buf, "u8")
        /// ```
        /// </summary>
        FingerprintType = 0x181,

        /// <summary>
        /// Template Fingerprint information of a working sensor.
        ///
        /// ```
        /// const [confidence, template] = jdunpack<[number, Uint8Array]>(buf, "u16 b")
        /// ```
        /// </summary>
        FingerprintTemplate = 0x182,
    }

    public static class VerifiedTelemetryRegPack {
        /// <summary>
        /// Pack format for 'telemetry_status' data.
        /// </summary>
        public const string TelemetryStatus = "u8";

        /// <summary>
        /// Pack format for 'telemetry_status_interval' data.
        /// </summary>
        public const string TelemetryStatusInterval = "u32";

        /// <summary>
        /// Pack format for 'fingerprint_type' data.
        /// </summary>
        public const string FingerprintType = "u8";

        /// <summary>
        /// Pack format for 'fingerprint_template' data.
        /// </summary>
        public const string FingerprintTemplate = "u16 b";
    }

    public enum VerifiedTelemetryCmd : ushort {
        /// <summary>
        /// No args. This command will clear the template fingerprint of a sensor and collect a new template fingerprint of the attached sensor.
        /// </summary>
        ResetFingerprintTemplate = 0x80,

        /// <summary>
        /// No args. This command will append a new template fingerprint to the `fingerprintTemplate`. Appending more fingerprints will increase the accuracy in detecting the telemetry status.
        /// </summary>
        RetrainFingerprintTemplate = 0x81,
    }

    public enum VerifiedTelemetryEvent : ushort {
        /// <summary>
        /// Argument: telemetry_status Status (uint8_t). The telemetry status of the device was updated.
        ///
        /// ```
        /// const [telemetryStatus] = jdunpack<[VerifiedTelemetryStatus]>(buf, "u8")
        /// ```
        /// </summary>
        TelemetryStatusChange = 0x3,

        /// <summary>
        /// The fingerprint template was updated
        /// </summary>
        FingerprintTemplateChange = 0x80,
    }

    public static class VerifiedTelemetryEventPack {
        /// <summary>
        /// Pack format for 'telemetry_status_change' data.
        /// </summary>
        public const string TelemetryStatusChange = "u8";
    }

}
