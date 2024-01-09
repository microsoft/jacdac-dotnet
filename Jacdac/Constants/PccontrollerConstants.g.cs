namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint PCController = 0x113d0987;
    }
    public enum PCControllerCmd : ushort {
        /// <summary>
        /// Argument: url string (bytes). Open a URL in the default browser.
        ///
        /// ```
        /// const [url] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        OpenUrl = 0x80,

        /// <summary>
        /// Argument: name string (bytes). Start an app.
        ///
        /// ```
        /// const [name] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        StartApp = 0x81,

        /// <summary>
        /// Argument: text string (bytes). Send text to the active window.
        ///
        /// ```
        /// const [text] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        SendText = 0x82,

        /// <summary>
        /// Argument: script string (bytes). Run a script.
        ///
        /// ```
        /// const [script] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        RunScript = 0x83,
    }

    public static class PCControllerCmdPack {
        /// <summary>
        /// Pack format for 'open_url' data.
        /// </summary>
        public const string OpenUrl = "s";

        /// <summary>
        /// Pack format for 'start_app' data.
        /// </summary>
        public const string StartApp = "s";

        /// <summary>
        /// Pack format for 'send_text' data.
        /// </summary>
        public const string SendText = "s";

        /// <summary>
        /// Pack format for 'run_script' data.
        /// </summary>
        public const string RunScript = "s";
    }

}
