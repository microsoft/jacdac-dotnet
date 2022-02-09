using System;

namespace Jacdac.Clients
{
    public partial class LedStripClient
    {
        /// <summary>
        /// Runs an encoded light command
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        public void Run(string source, object[] args)
        {
            var payload = LedStripProgramEncoding.ToBuffer(source, args);
            this.SendCmdPacked((ushort)LedStripCmd.Run, LedStripCmdPack.Run, new object[] { payload });
        }
    }
}
