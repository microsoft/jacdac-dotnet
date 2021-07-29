using Jacdac.Transport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jacdac.Explorer
{
    public partial class PacketTracer : Form
    {
        private readonly JDTransport transport;
        private bool showAnnouncementPackets = false;
        private bool showEventPackets = false;
        private bool autoScroll = true;

        public PacketTracer()
        {
            InitializeComponent();
        }

        public PacketTracer(JDTransport transport)
        {
            InitializeComponent();
            this.transport = transport;

            transport.OnJacdacPacket += Transport_OnJacdacPacket;
            transport.OnJacdacPacketSent += Transport_OnJacdacPacketSent;

            DoubleBuffered = true;
        }

        private void Transport_OnJacdacPacketSent(JDFrame frame, JDPacket packet)
        {
            BeginInvoke(new MethodInvoker(() => AddPacket(packet, true)));
        }

        private void Transport_OnJacdacPacket(JDFrame frame, JDPacket packet)
        {
            BeginInvoke(new MethodInvoker(() => AddPacket(packet, false)));
        }

        private void AddPacket(JDPacket packet, bool outgoing = false)
        {
            if (!showAnnouncementPackets && packet.IsAnnouncement)
                return;

            if (!showEventPackets && packet.OperationType == JDPacket.CommandType.Event)
                return;

            var timestamp = packet.Timestamp.ToString("HH:mm:ss fff");
            var dirString = outgoing ? ">" : "<";
            var deviceId = string.Format("{0:X}", packet.DeviceIdentifier);
            var serviceId = packet.ServiceIndex.ToString();
            var type = packet.IsAck ? "Ack" : packet.OperationType.ToString();
            var command = string.Format("{0:X}", packet.OperationCode);
            var payloadSize = packet.Data.Length.ToString();
            var payload = string.Join(" ", packet.Data.Select(d => d.ToString("X2")));

            var packetListItem = new ListViewItem(new string[] { timestamp, dirString, deviceId, serviceId, type, command, payloadSize, payload });
            packetListItem.Tag = packet;

            if (packet.IsAnnouncement)
                packetListItem.BackColor = Color.LightYellow;
            else if (packet.OperationType == JDPacket.CommandType.Event)
                packetListItem.BackColor = Color.LightGreen;
            else if (packet.IsAck)
                packetListItem.BackColor = Color.LightGray;
            else if (packet.IsReport)
                packetListItem.BackColor = Color.LightBlue;
            listView.Items.Add(packetListItem);

            if(autoScroll)
                listView.TopItem = packetListItem;
        } 

        private void showAnnouncementsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showAnnouncementsToolStripMenuItem.Checked = !showAnnouncementsToolStripMenuItem.Checked;
            showAnnouncementPackets = showAnnouncementsToolStripMenuItem.Checked;
        }

        private void PacketTracer_FormClosing(object sender, FormClosingEventArgs e)
        {
            transport.OnJacdacPacket -= Transport_OnJacdacPacket;
            transport.OnJacdacPacketSent -= Transport_OnJacdacPacketSent;
        }

        private void showEventsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showEventsToolStripMenuItem.Checked = !showEventsToolStripMenuItem.Checked;
            showEventPackets = showEventsToolStripMenuItem.Checked;
        }

        private void autoscrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoscrollToolStripMenuItem.Checked = !autoscrollToolStripMenuItem.Checked;
            autoScroll = autoscrollToolStripMenuItem.Checked;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView.Items.Clear();
        }
    }
}
