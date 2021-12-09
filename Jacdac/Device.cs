using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace Jacdac {
    public class JDDevice {
        byte[] services;

        public JDDevice() {
            this.services = new byte[0];
        }

        public uint ServiceAt(uint idx) {
            if (idx == 0)
                return 0;
            idx <<= 2;
            if (this.services == null || idx + 4 > this.services.Length)
                return 0xFFFFFFFF;
            return Util.Read32(this.services, (int)idx);
        }

    }
}
