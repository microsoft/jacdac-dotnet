using Jacdac.Transport;
using System;
using System.Threading.Tasks;

namespace Jacdac
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var potentialDevices = USBTransport.GetDevices();
            if (potentialDevices.Length == 0)
                return;

            var transport = new USBTransport(potentialDevices[0]);
            await transport.Connect();

            var bus = new JDBus(transport);

            Console.ReadKey();
        }
    }
}
