using System;

namespace Jacdac
{
    public sealed class ControlServer : JDServiceServer
    {
        internal ControlServer(JDBusOptions options)
            : base(Jacdac.ControlConstants.ServiceClass, null)
        {
            this.AddRegister(new JDDynamicRegisterServer(
                (ushort)Jacdac.ControlReg.Uptime, Jacdac.ControlRegPack.Uptime,
                (reg) => new object[] { (ulong)(reg.Service.Device.Bus.Timestamp.TotalMilliseconds * 1000) })
            );
            if (Platform.McuTemperature != null)
                this.AddRegister(new JDDynamicRegisterServer(
                    (ushort)Jacdac.ControlReg.McuTemperature, Jacdac.ControlRegPack.McuTemperature,
                    (reg) => new object[] { Platform.McuTemperature() })
                );
            if (options?.FirmwareVersion != null)
                this.AddRegister(new JDStaticRegisterServer((ushort)Jacdac.ControlReg.FirmwareVersion, Jacdac.ControlRegPack.FirmwareVersion, new object[] { options.FirmwareVersion }));
            if (options?.Description != null)
                this.AddRegister(new JDStaticRegisterServer((ushort)Jacdac.ControlReg.DeviceDescription, Jacdac.ControlRegPack.DeviceDescription, new object[] { options.Description }));
            if (options != null && options.ProductIdentifier != 0)
                this.AddRegister(new JDStaticRegisterServer((ushort)Jacdac.ControlReg.ProductIdentifier, Jacdac.ControlRegPack.ProductIdentifier, new object[] { options.ProductIdentifier }));
        }
    }
}
