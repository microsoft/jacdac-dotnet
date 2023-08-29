namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Ros = 0x1524f42c;
    }
    public enum RosCmd : ushort {
        /// <summary>
        /// Publishes a JSON-encoded message to the given topic.
        ///
        /// ```
        /// const [node, topic, message] = jdunpack<[string, string, string]>(buf, "z z s")
        /// ```
        /// </summary>
        PublishMessage = 0x81,

        /// <summary>
        /// Subscribes to a message topic. Subscribed topics will emit message reports.
        ///
        /// ```
        /// const [node, topic] = jdunpack<[string, string]>(buf, "z s")
        /// ```
        /// </summary>
        SubscribeMessage = 0x82,

        /// <summary>
        /// A message published on the bus. Message is JSON encoded.
        ///
        /// ```
        /// const [node, topic, message] = jdunpack<[string, string, string]>(buf, "z z s")
        /// ```
        /// </summary>
        Message = 0x83,
    }

    public static class RosCmdPack {
        /// <summary>
        /// Pack format for 'publish_message' data.
        /// </summary>
        public const string PublishMessage = "z z s";

        /// <summary>
        /// Pack format for 'subscribe_message' data.
        /// </summary>
        public const string SubscribeMessage = "z s";

        /// <summary>
        /// Pack format for 'message' data.
        /// </summary>
        public const string Message = "z z s";
    }

}
