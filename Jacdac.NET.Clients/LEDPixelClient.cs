using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jacdac.Clients
{
    public partial class LedPixelClient
    {
        /// <summary>
        /// Runs an encoded light command
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        public void Run(string source, object[] args)
        {
            var payload = LedPixelEncoder.ToBuffer(source, args);
            this.SendCmdPacked((ushort)LedPixelCmd.Run, LedPixelCmdPack.Run, new object[] { payload });
        }
    }
}
