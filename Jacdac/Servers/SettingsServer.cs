namespace Jacdac.Servers
{
    public sealed class SettingsServer : JDServiceServer
    {
        public readonly IKeyStorage Storage;
        public SettingsServer(IKeyStorage storage, JDServiceServerOptions options = null)
            : base(SettingsConstants.ServiceClass, options)
        {
            this.Storage = storage;

            this.AddCommand((ushort)SettingsCmd.List, this.handleList);
            this.AddCommand((ushort)SettingsCmd.ListKeys, this.handleListKeys);
            this.AddCommand((ushort)SettingsCmd.Get, this.handleGet);
            this.AddCommand((ushort)SettingsCmd.Set, this.handleSet);
            this.AddCommand((ushort)SettingsCmd.Delete, this.handleDelete);
        }

        private void handleList(JDNode sender, PacketEventArgs args)
        {
            var pkt = args.Packet;
            var pipe = OutPipe.From(this.Device.Bus, pkt);
            var keys = this.Storage.GetKeys();
            pipe?.RespondForEach(keys, k =>
            {
                var key = (string)k;
                var isSecret = key[0] == '$';
                var value = isSecret ? new byte[1] : (this.Storage.Read(key) ?? new byte[0]);
                return PacketEncoding.Pack("s b", new object[] { key, value });
            });
        }

        private void handleListKeys(JDNode sender, PacketEventArgs args)
        {
            var pkt = args.Packet;
            var pipe = OutPipe.From(this.Device.Bus, pkt);
            var keys = this.Storage.GetKeys();
            pipe?.RespondForEach(keys, key => PacketEncoding.Pack("s", new object[] { key }));
        }

        private void handleGet(JDNode sender, PacketEventArgs args)
        {
            var pkt = args.Packet;
            var key = (string)PacketEncoding.UnPack(SettingsCmdPack.Get, pkt.Data)[0];
            var isSecret = key[0] == '$';
            var value = isSecret ? new byte[1] : (this.Storage.Read(key) ?? new byte[0]);

            var resData = PacketEncoding.Pack(SettingsCmdPack.GetReport, new object[] { key, value });
            var res = Packet.From((ushort)SettingsCmd.Get, resData);
            res.DeviceId = this.Device.DeviceId;
            this.Device.SendPacket(res);
        }

        private void handleSet(JDNode sender, PacketEventArgs args)
        {
            var pkt = args.Packet;
            var values = PacketEncoding.UnPack(SettingsCmdPack.Set, pkt.Data);
            var key = (string)values[0];
            var value = (byte[])values[1];
            this.Storage.Write(key, value);
            this.RaiseChanged();
        }

        private void handleDelete(JDNode sender, PacketEventArgs args)
        {
            var pkt = args.Packet;
            var key = (string)PacketEncoding.UnPack(SettingsCmdPack.Delete, pkt.Data)[0];
            this.Storage.Delete(key);
            this.RaiseChanged();
        }

    }
}