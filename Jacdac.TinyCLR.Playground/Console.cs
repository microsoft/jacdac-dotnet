
using GHIElectronics.TinyCLR.Devices.Uart;
using GHIElectronics.TinyCLR.Pins;
using System.Diagnostics;
using System.Threading;
using Jacdac;
using System;
using Jacdac.Servers;
using GHIElectronics.TinyCLR.Devices.Jacdac.Transport;
using Jacdac.Transports;
using Jacdac.Storage;
using GHIElectronics.TinyCLR.Devices.Storage;
using GHIElectronics.TinyCLR.Devices.Gpio;
using Jacdac.Clients;
using Jacdac.Samples;

namespace System
{
    public static class Console
    {
        public static void WriteLine(string msg)
        {
            Jacdac.Playground.Display.WriteLine(msg);
        }

        public static void WriteLine()
        {
            Jacdac.Playground.Display.WriteLine("");
        }
    }
}
