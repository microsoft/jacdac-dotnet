namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Power = 0x1fa4c95a;
    }

    public enum PowerPowerStatus: byte { // uint8_t
        Disallowed = 0x0,
        Powering = 0x1,
        Overload = 0x2,
        Overprovision = 0x3,
    }

    public enum PowerReg : ushort {
        /// <summary>
        /// Read-write bool (uint8_t). Can be used to completely disable the service.
        /// When allowed, the service may still not be providing power, see
        /// `power_status` for the actual current state.
        ///
        /// ```
        /// const [allowed] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Allowed = 0x1,

        /// <summary>
        /// Read-write mA uint16_t. Limit the power provided by the service. The actual maximum limit will depend on hardware.
        /// This field may be read-only in some implementations - you should read it back after setting.
        ///
        /// ```
        /// const [maxPower] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        MaxPower = 0x7,

        /// <summary>
        /// Read-only PowerStatus (uint8_t). Indicates whether the power provider is currently providing power (`Powering` state), and if not, why not.
        /// `Overprovision` means there was another power provider, and we stopped not to overprovision the bus.
        ///
        /// ```
        /// const [powerStatus] = jdunpack<[PowerPowerStatus]>(buf, "u8")
        /// ```
        /// </summary>
        PowerStatus = 0x181,

        /// <summary>
        /// Read-only mA uint16_t. Present current draw from the bus.
        ///
        /// ```
        /// const [currentDraw] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        CurrentDraw = 0x101,

        /// <summary>
        /// Read-only mV uint16_t. Voltage on input.
        ///
        /// ```
        /// const [batteryVoltage] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        BatteryVoltage = 0x180,

        /// <summary>
        /// Read-only ratio u0.16 (uint16_t). Fraction of charge in the battery.
        ///
        /// ```
        /// const [batteryCharge] = jdunpack<[number]>(buf, "u0.16")
        /// ```
        /// </summary>
        BatteryCharge = 0x182,

        /// <summary>
        /// Constant mWh uint32_t. Energy that can be delivered to the bus when battery is fully charged.
        /// This excludes conversion overheads if any.
        ///
        /// ```
        /// const [batteryCapacity] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        BatteryCapacity = 0x183,

        /// <summary>
        /// Read-write ms uint16_t. Many USB power packs need current to be drawn from time to time to prevent shutdown.
        /// This regulates how often and for how long such current is drawn.
        /// Typically a 1/8W 22 ohm resistor is used as load. This limits the duty cycle to 10%.
        ///
        /// ```
        /// const [keepOnPulseDuration] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        KeepOnPulseDuration = 0x80,

        /// <summary>
        /// Read-write ms uint16_t. Many USB power packs need current to be drawn from time to time to prevent shutdown.
        /// This regulates how often and for how long such current is drawn.
        /// Typically a 1/8W 22 ohm resistor is used as load. This limits the duty cycle to 10%.
        ///
        /// ```
        /// const [keepOnPulsePeriod] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        KeepOnPulsePeriod = 0x81,
    }

    public static class PowerRegPack {
        /// <summary>
        /// Pack format for 'allowed' register data.
        /// </summary>
        public const string Allowed = "u8";

        /// <summary>
        /// Pack format for 'max_power' register data.
        /// </summary>
        public const string MaxPower = "u16";

        /// <summary>
        /// Pack format for 'power_status' register data.
        /// </summary>
        public const string PowerStatus = "u8";

        /// <summary>
        /// Pack format for 'current_draw' register data.
        /// </summary>
        public const string CurrentDraw = "u16";

        /// <summary>
        /// Pack format for 'battery_voltage' register data.
        /// </summary>
        public const string BatteryVoltage = "u16";

        /// <summary>
        /// Pack format for 'battery_charge' register data.
        /// </summary>
        public const string BatteryCharge = "u0.16";

        /// <summary>
        /// Pack format for 'battery_capacity' register data.
        /// </summary>
        public const string BatteryCapacity = "u32";

        /// <summary>
        /// Pack format for 'keep_on_pulse_duration' register data.
        /// </summary>
        public const string KeepOnPulseDuration = "u16";

        /// <summary>
        /// Pack format for 'keep_on_pulse_period' register data.
        /// </summary>
        public const string KeepOnPulsePeriod = "u16";
    }

    public enum PowerCmd : ushort {
        /// <summary>
        /// No args. Sent by the power service periodically, as broadcast.
        /// </summary>
        Shutdown = 0x80,
    }

    public enum PowerEvent : ushort {
        /// <summary>
        /// Argument: power_status PowerStatus (uint8_t). Emitted whenever `power_status` changes.
        ///
        /// ```
        /// const [powerStatus] = jdunpack<[PowerPowerStatus]>(buf, "u8")
        /// ```
        /// </summary>
        PowerStatusChanged = 0x3,
    }

    public static class PowerEventPack {
        /// <summary>
        /// Pack format for 'power_status_changed' register data.
        /// </summary>
        public const string PowerStatusChanged = "u8";
    }

}
