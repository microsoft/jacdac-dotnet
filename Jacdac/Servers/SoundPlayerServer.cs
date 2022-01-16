using System;

namespace Jacdac.Servers
{
    public interface ISoundPlayer
    {
        float Volume { get; set; }
        void Play(string name);
        void Cancel();
        string[] ListSounds();
    }

    public sealed class SoundPlayerServer : JDServiceServer
    {
        private readonly ISoundPlayer soundPlayer;
        private readonly JDDynamicRegisterServer volumeRegister;

        public SoundPlayerServer(ISoundPlayer soundPlayer, JDServiceServerOptions options = null)
            : base(ServiceClasses.SoundPlayer, options)
        {
            if (soundPlayer == null)
                throw new ArgumentNullException(nameof(soundPlayer));
            this.soundPlayer = soundPlayer;

            RegisterGetHandler volumeGetter = this.handleGetVolume;
            RegisterSetHandler volumeSetter = this.handleSetVolume;
            this.AddRegister(this.volumeRegister = new JDDynamicRegisterServer((ushort)SoundPlayerReg.Volume, SoundPlayerRegPack.Volume, volumeGetter, volumeSetter));
            this.AddCommand((ushort)SoundPlayerCmd.Play, this.handlePlay);
            this.AddCommand((ushort)SoundPlayerCmd.Cancel, this.handleCancel);
            this.AddCommand((ushort)SoundPlayerCmd.ListSounds, this.handleListSounds);
        }

        private object[] handleGetVolume(JDRegisterServer sender)
        {
            var volume = this.soundPlayer.Volume;
            if (volume < 0)
                return null;
            return new object[] { volume };
        }

        private void handleSetVolume(JDRegisterServer sender, object[] values)
        {
            this.soundPlayer.Volume = Util.UnboxFloat(values[0]);
        }

        private void handlePlay(JDNode sender, PacketEventArgs args)
        {
            var values = PacketEncoding.UnPack(SoundPlayerCmdPack.Play, args.Packet.Data);
            var name = (string)values[0];
            this.soundPlayer.Play(name);
        }

        private void handleCancel(JDNode sender, PacketEventArgs args)
        {
            this.soundPlayer.Cancel();
        }

        private void handleListSounds(JDNode sender, PacketEventArgs args)
        {
            var pipe = OutPipe.From(this.Bus, args.Packet);
            pipe.RespondForEach(this.soundPlayer.ListSounds(), name => PacketEncoding.Pack("u32 s", new object[] { 0, name }));
        }
    }
}
