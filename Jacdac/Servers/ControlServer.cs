using System;

namespace Jacdac.Servers
{
    public sealed class ControlServer : JDServer
    {
        internal ControlServer(JDBusOptions options)
            : base(Jacdac.ControlConstants.ServiceClass)
        {
            this.AddRegister(new JDDynamicRegisterServer(
                (ushort)Jacdac.ControlReg.Uptime, "u64", 
                (reg) => new object[] { reg.Server.Bus.Timestamp.TotalMilliseconds * 1000 })
            );
            if (Platform.McuTemperature != null)
                this.AddRegister(new JDDynamicRegisterServer(
                    (ushort)Jacdac.ControlReg.McuTemperature, "i16", 
                    (reg) => new object[] { Platform.McuTemperature() })
                );
            if (options?.FirmwareVersion != null)
                this.AddRegister(new JDStaticRegisterServer((ushort)Jacdac.ControlReg.FirmwareVersion, "s", new object[] { options.FirmwareVersion }));
            if (options?.Description != null)
                this.AddRegister(new JDStaticRegisterServer((ushort)Jacdac.ControlReg.DeviceDescription, "s", new object[] { options.Description }));
            if (options != null && options.ProductIdentifier != 0)
                this.AddRegister(new JDStaticRegisterServer((ushort)Jacdac.ControlReg.ProductIdentifier, "u32", new object[] { options.ProductIdentifier }));
        }
    }
}
