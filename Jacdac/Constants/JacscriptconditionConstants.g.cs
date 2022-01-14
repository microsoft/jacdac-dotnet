namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint JacscriptCondition = 0x1196796d;
    }
    public enum JacscriptConditionCmd : ushort {
        /// <summary>
        /// No args. Triggers a `signalled` event.
        /// </summary>
        Signal = 0x80,
    }

    public enum JacscriptConditionEvent : ushort {
        /// <summary>
        /// Triggered by `signal` command.
        /// </summary>
        Signalled = 0x3,
    }

}
