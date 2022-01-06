namespace Jacdac.Servers
{
    public sealed class ProtoTestServer : JDServiceServer
    {
        private JDStaticRegisterServer rwBytes;

        public ProtoTestServer(JDServiceServerOptions options = null)
            : base(ServiceClasses.ProtoTest, options)
        {
            this.init(
                ProtoTestRegPack.RwBool,
              (ushort)ProtoTestReg.RwBool,
              (ushort)ProtoTestReg.RoBool,
              (ushort)ProtoTestCmd.CBool,
              (ushort)ProtoTestEvent.EBool,
                new object[] { false }
            );

            this.init(
                ProtoTestRegPack.RwI32,
              (ushort)ProtoTestReg.RwI32,
              (ushort)ProtoTestReg.RoI32,
              (ushort)ProtoTestCmd.CI32,
              (ushort)ProtoTestEvent.EI32,
            new object[] { 0 }
        );
            this.init(
                ProtoTestRegPack.RwU32,
          (ushort)ProtoTestReg.RwU32,
          (ushort)ProtoTestReg.RoU32,
          (ushort)ProtoTestCmd.CU32,
          (ushort)ProtoTestEvent.EU32,
            new object[] { 0 }
        );
            this.init(
                ProtoTestRegPack.RwString,
          (ushort)ProtoTestReg.RwString,
          (ushort)ProtoTestReg.RoString,
          (ushort)ProtoTestCmd.CString,
          (ushort)ProtoTestEvent.EString,
            new string[] { "" }
        );

            this.rwBytes = this.init(
                ProtoTestRegPack.RwBytes,
          (ushort)ProtoTestReg.RwBytes,
          (ushort)ProtoTestReg.RoBytes,
          (ushort)ProtoTestCmd.CBytes,
          (ushort)ProtoTestEvent.EBytes,
            new object[] { Packet.EmptyData }
        );

            this.init(
                ProtoTestRegPack.RwI8U8U16I32,
          (ushort)ProtoTestReg.RwI8U8U16I32,
          (ushort)ProtoTestReg.RoI8U8U16I32,
          (ushort)ProtoTestCmd.CI8U8U16I32,
          (ushort)ProtoTestEvent.EI8U8U16I32,
            new object[] { 0,
            0,
            0,
            0 }
        );

            this.init(
                ProtoTestRegPack.RwU8String,
          (ushort)ProtoTestReg.RwU8String,
          (ushort)ProtoTestReg.RoU8String,
          (ushort)ProtoTestCmd.CU8String,
          (ushort)ProtoTestEvent.EU8String,
            new object[] { 0,
            "" }
        );

            this.AddCommand(
                (ushort)ProtoTestCmd.CReportPipe,
                this.handleReportPipe
            );
        }

        private JDStaticRegisterServer init(
            string fmt,
            ushort rwi,
            ushort roi,
            ushort ci,
            ushort ei,
            object[] values
        )
        {
            var rw = this.AddRegister(rwi, fmt, values);
            var ro = this.AddRegister(roi, fmt, values);
            rw.Changed += (sender, args) =>
              {
                  ro.SetValues(rw.GetValues());
                  this.SendEvent(ei, rw.Data);
              };
            this.AddCommand(ci, (sender, args) =>
            {
                var pkt = args.Packet;
                rw.SetValues(PacketEncoding.UnPack(fmt, pkt.Data));
            });
            return rw;
        }

        private void handleReportPipe(JDNode sender, PacketEventArgs args)
        {
            var pkt = args.Packet;
            var pipe = OutPipe.From(this.Device.Bus, pkt);
            var data = this.rwBytes.Data;
            var bytes = new object[data.Length];
            for (var i = 0; i < data.Length; ++i)
                bytes[i] = (object)data[i];
            pipe?.RespondForEach(bytes, bo =>
            {
                var b = (byte)bo;
                var buf = new byte[] { b };
                return PacketEncoding.Pack("b", new object[] { buf });
            });
        }
    }
}
