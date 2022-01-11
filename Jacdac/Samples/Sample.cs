using System;
using System.Reflection;

namespace Jacdac.Samples
{
    public interface ISample
    {
        void Run(JDBus bus);
    }

    public static class SampleExtensions
    {
        static Type GetSampleType(string name)
        {
            var types = typeof(SampleExtensions).Assembly.GetTypes();
            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces();
                foreach (var itf in interfaces)
                    if (itf == typeof(ISample) && type.Name.ToLower().Equals(name.ToLower()))
                        return type;
            }
            return null;
        }

        public static ISample GetSample(string[] args)
        {
            foreach (var arg in args)
            {
                var sampleType = GetSampleType(arg);
                if (sampleType != null)
                {
                    var ctor = sampleType.GetConstructor(new Type[0]);
                    var obj = ctor.Invoke(new object[] { });
                    return (ISample)obj;
                }
            }
            return null;
        }
    }
}
