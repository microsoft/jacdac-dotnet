namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint MidiOutput = 0x1a848cd7;
    }
    public enum MidiOutputReg {
        /// <summary>
        /// Read-write bool (uint8_t). Opens or closes the port to the MIDI device
        ///
        /// ```
        /// const [enabled] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Enabled = 0x1,
    }

    public static class MidiOutputRegPack {
        /// <summary>
        /// Pack format for 'enabled' register data.
        /// </summary>
        public const string Enabled = "u8";
    }

    public enum MidiOutputCmd {
        /// <summary>
        /// No args. Clears any pending send data that has not yet been sent from the MIDIOutput's queue.
        /// </summary>
        Clear = 0x80,

        /// <summary>
        /// Argument: data bytes. Enqueues the message to be sent to the corresponding MIDI port
        ///
        /// ```
        /// const [data] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        Send = 0x81,
    }

    public static class MidiOutputCmdPack {
        /// <summary>
        /// Pack format for 'send' register data.
        /// </summary>
        public const string Send = "b";
    }

}
