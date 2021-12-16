namespace Jacdac
{
    public sealed class SensorServerOptions
    {
        public uint StreamingInterval;
        public uint StreamingPreferredInterval;
    }

    public abstract class JDSensorServer : JDServiceServer
    {
        public readonly JDStaticRegisterServer Reading;
        public readonly JDStaticRegisterServer StreamingSamples;
        public readonly JDStaticRegisterServer StreamingInterval;

        protected JDSensorServer(uint serviceClass, string readingFormat, object[] reading, SensorServerOptions options = null)
            : base(serviceClass)
        {
            this.AddRegister(this.Reading = new JDStaticRegisterServer((ushort)SystemReg.Reading, readingFormat, reading));
            this.AddRegister(this.StreamingSamples = new JDStaticRegisterServer((ushort)SystemReg.StreamingSamples, "u8", new object[] { 0 }));
            if (options != null)
            {
                if (options.StreamingInterval != 0)
                    this.AddRegister(this.StreamingInterval = new JDStaticRegisterServer((ushort)SystemReg.StreamingInterval, "u32", new object[] { options.StreamingInterval }));
                if (options.StreamingPreferredInterval != 0)
                    this.AddRegister(new JDStaticRegisterServer((ushort)SystemReg.StreamingPreferredInterval, "u32", new object[] { options.StreamingPreferredInterval })
                    {
                        IsConst = true
                    });
            }
        }
    }
}
