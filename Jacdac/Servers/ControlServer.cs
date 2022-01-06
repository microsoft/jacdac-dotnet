using System;

namespace Jacdac.Servers
{
    public sealed partial class ControlServer : JDServiceServer
    {
        private SetStatusLightHandler setStatusLight;

        internal ControlServer(JDBusOptions options)
            : base(ServiceClasses.Control, null)
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

            this.setStatusLight = options?.SetStatusLight;
            if (this.setStatusLight != null)
                this.AddCommand((ushort)Jacdac.ControlCmd.SetStatusLight, this.handleSetStatusLight);
        }

        private void handleSetStatusLight(JDNode node, PacketEventArgs args)
        {
            var pkt = args.Packet;
            var values = PacketEncoding.UnPack(ControlCmdPack.SetStatusLight, pkt.Data);
            var red = (byte)values[0];
            var green = (byte)values[1];
            var blue = (byte)values[2];
            var speed = (byte)values[3];
            this.setStatusLight(red, green, blue, speed);
        }
    }
}
