using Jacdac.Services;
using Jacdac.Transport;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static Jacdac.JDPacket;

namespace Jacdac
{
    public class JDBus
    {
        private JDTransport Transport { get; }

        public ConcurrentDictionary<ulong, JDDevice> Devices { get; private set; } = new ConcurrentDictionary<ulong, JDDevice>();

        private System.Timers.Timer deviceCheckTimer;

        public JDBus(JDTransport transport)
        {
            deviceCheckTimer = new System.Timers.Timer(1000);
            deviceCheckTimer.AutoReset = true;
            deviceCheckTimer.Elapsed += DeviceCheckTimer_Elapsed;
            deviceCheckTimer.Start();

            Transport = transport;
            transport.OnJacdacPacket += Transport_OnJacdacPacket;
        }

        private void DeviceCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            deviceCheckTimer.Stop();

            List<ulong> disconnectedDevices = new List<ulong>();
            foreach(var device in Devices)
            {
                if ((DateTime.Now - device.Value.LastSeen).TotalMilliseconds > 1000)
                    disconnectedDevices.Add(device.Key);
            }

            foreach(var disconnectedDevice in disconnectedDevices)
            {
                OnDeviceDisconnect?.Invoke(disconnectedDevice, null);
                Devices.Remove(disconnectedDevice, out _);
            }

            if (disconnectedDevices.Count > 0)
                DeviceListChanged?.Invoke(this, null);

            deviceCheckTimer.Start();
        }

        private void HandleDeviceAnnouncement(JDPacket packet)
        {
            var announcementPacket = ControlService.AnnouncementReport.Parse(packet.Data);
            if (!Devices.ContainsKey(packet.DeviceIdentifier))
            {
                var device = new JDDevice(packet.DeviceIdentifier, announcementPacket, this);
                Devices.TryAdd(packet.DeviceIdentifier, device);
                OnDeviceConnect?.Invoke(packet.DeviceIdentifier, device);
                DeviceListChanged?.Invoke(this, null);
            }

            Devices[packet.DeviceIdentifier].LastSeen = packet.Timestamp;
        }

        public async Task SendPacket(JDPacket packet)
        {
            await Transport.SendJDPacket(packet);
        }

        private void Transport_OnJacdacPacket(JDFrame frame, JDPacket packet)
        {
            // Console.WriteLine(packet.ToString());

            if (packet.IsAnnouncement)
            {
                HandleDeviceAnnouncement(packet);
            }

            if (Devices.ContainsKey(packet.DeviceIdentifier) && packet.IsReport)
                Devices[packet.DeviceIdentifier].HandlePacket(packet);

            OnJacdacPacket?.Invoke(packet);
        }

        public event JacdacPacketEventHandler OnJacdacPacket;

        public delegate void JacdacDeviceEventHandler(ulong deviceId, JDDevice? device);
        public event JacdacDeviceEventHandler OnDeviceConnect;
        public event JacdacDeviceEventHandler OnDeviceDisconnect;
        public event EventHandler DeviceListChanged;
    }
}
