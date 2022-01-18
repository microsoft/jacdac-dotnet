using NetCoreAudio;
using System;
using System.IO;
using System.Linq;

namespace Jacdac.Servers.SoundPlayer
{
    /// <summary>
    /// A sound player for .mp3 files to run a sound player server instance
    /// </summary>
    public sealed class NetCoreAudioSoundPlayer : ISoundPlayer
    {
        private readonly Player player;
        private readonly string soundDirectory;

        /// <summary>
        /// Creates a sound player instance for the given folder of mp3s and wav.
        /// Subfolders are not supported
        /// </summary>
        /// <param name="soundDirectory"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public NetCoreAudioSoundPlayer(string soundDirectory)
        {
            if (soundDirectory == null)
                throw new ArgumentNullException(nameof(soundDirectory));
            if (!Directory.Exists(soundDirectory))
                throw new ArgumentException(nameof(soundDirectory));
            this.player = new Player();
            this.soundDirectory = soundDirectory;
        }

        public float Volume
        {
            get { return -1; }
            set
            { }
        }

        public void Cancel()
        {
            this.player.Stop();
        }

        public string[] ListSounds()
        {
            var mp3s = Directory.GetFiles(this.soundDirectory, "*.mp3");
            var wavs = Directory.GetFiles(this.soundDirectory, "*.wav");
            var files = Array.Sort(
                mp3s.Concat(wavs)
                    .Select(Path.GetFileNameWithoutExtension)
                    .Distinct().ToArray()
            );
            return files;
        }

        public void Play(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var fileName = Path.GetFileNameWithoutExtension(name); // filter out rooted paths
            var mp3 = Path.Combine(this.soundDirectory, fileName + ".mp3");
            if (File.Exists(mp3))
                this.player.Play(mp3);
            else {
                var wav = Path.Combine(this.soundDirectory, fileName + ".wav");
                if (File.Exists(wav))
                    this.player.Play(wav);
            }
        }
    }
}
