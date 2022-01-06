using Jacdac;
using System;

namespace Jacdac.Clients
{

    public partial class BuzzerClient
    {
        /// <summary>
        /// Play a note at the given frequency and volume.
        /// </summary>
        public void PlayNote(uint frequency, float volume, uint duration)
        {
            var period = 1000000 / frequency;
            uint v = (uint)Math.Round(Math.Max(0, Math.Min(1, volume) * 0xff));
            var duty = (period * v) >> 11;
            this.PlayTone(period, duty, duration);
        }
    }
}