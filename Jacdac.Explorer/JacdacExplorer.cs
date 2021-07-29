using Jacdac.Services;
using Jacdac.Transport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Jacdac.Transport.USBTransport;

namespace Jacdac.Explorer
{
    public partial class JacdacExplorer : Form
    {
        USBTransport transport;
        JDBus bus;

        public JacdacExplorer()
        {
            InitializeComponent();

            PopulateDeviceList();

            usbDeviceSelectMenu.DropDown.ItemClicked += async (s, e) =>
            {
                try
                {
                    await ConnectToUSBDevice((USBDeviceDescription)e.ClickedItem.Tag);
                }
                catch
                {
                    MessageBox.Show("Connection failed. Try again");
                    PopulateDeviceList();
                }
            };

            deviceTree.NodeMouseClick += DeviceTree_NodeMouseClick;

        }

        private void PopulateDeviceList()
        {
            var usbDevices = USBTransport.GetDevices();
            var usbDeviceToolStripItems = usbDevices.Select(d => new ToolStripMenuItem()
            {
                Text = d.DeviceName,
                Tag = d
            }).ToArray();
            usbDeviceSelectMenu.DropDown.Items.Clear();
            usbDeviceSelectMenu.DropDown.Items.AddRange(usbDeviceToolStripItems);
        }

        private void DeviceTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag?.GetType().IsAssignableTo(typeof(JDService)) ?? false)
                DisplayService((JDService)e.Node.Tag);
        }

        public async Task ConnectToUSBDevice(USBDeviceDescription uSBDevice)
        {
            if (transport != null)
                transport.Close();

            transport = new USBTransport(uSBDevice);
            await transport.Connect();
            bus = new JDBus(transport);

            bus.DeviceListChanged += async (i, d) => await UpdateDeviceTree();
        }

        public async Task UpdateDeviceTree()
        {
            if (bus == null)
                return;

            var busNode = new TreeNode("Jacdac Bus");

            foreach (var device in bus.Devices)
            {
                var name = "";
                try
                {
                    name = await device.Value.ControlService.GetDeviceDescription();
                } catch{}

                if(String.IsNullOrWhiteSpace(name))
                    name = $"Unknown ({device.Key})"; ;

                var deviceNode = new TreeNode(name);
                foreach (var service in device.Value.Services)
                {
                    var serviceInstanceName = service.GetType().Name;

                    if (service.GetType() == typeof(JDService))
                        serviceInstanceName += $" (0x{service.ServiceId.ToString("X").ToLower()})";

                    var serviceNode = new TreeNode(serviceInstanceName);
                    serviceNode.Tag = service;
                    deviceNode.Nodes.Add(serviceNode);
                }
                busNode.Nodes.Add(deviceNode);
            }

            BeginInvoke(new MethodInvoker(() => {
                deviceTree.Nodes.Clear();
                deviceTree.Nodes.Add(busNode);
                busNode.ExpandAll();
            }));
        }

        public void DisplayService(JDService service)
        {
            registerListView.Items.Clear();
            TypeInfo serviceTypeInfo = service.GetType().GetTypeInfo();
            var registerReadMethods = serviceTypeInfo.DeclaredMethods.Where(m => m.GetCustomAttribute(typeof(JDReadRegister), false) != null);
            foreach(var registerReadMethod in registerReadMethods)
            {
                var listItem = new ListViewItem(new[] { "Register", registerReadMethod.Name, "Double click to read" });
                registerListView.Items.Add(listItem);
                listItem.Tag = (service, registerReadMethod);
            }

            var events = serviceTypeInfo.DeclaredEvents.Where(m => m.GetCustomAttribute(typeof(JDEvent), false) != null);
            eventListView.Items.Clear();
            foreach (var eventDecl in events)
            {
                var dele = (Action)(() => {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        var timestamp = DateTime.Now.ToString("HH:mm:ss fff");
                        eventListView.Items.Add(new ListViewItem(new string[] { timestamp, eventDecl.Name }));
                    }));
                });
                eventDecl.AddEventHandler(service, dele);
            }
        }

        public void HandleEvent()
        {
            MessageBox.Show("ok");
        }

        private async void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (registerListView.SelectedItems.Count == 0)
                return;

            var sv = ((JDService, MethodInfo))registerListView.SelectedItems[0].Tag;
            if (sv.Item2.GetParameters().Length != 0)
            {
                BeginInvoke(new MethodInvoker(() => {
                    registerListView.SelectedItems[0].SubItems[1].Text = "Not possible";
                }));
                return;
            }
            Task resultTask = (Task)sv.Item2.Invoke(sv.Item1, null);
            try
            {
                await resultTask;
                object result = resultTask.GetType().GetProperty("Result").GetValue(resultTask);
                BeginInvoke(new MethodInvoker(() => {
                    registerListView.SelectedItems[0].SubItems[2].Text = result.ToString();
                }));
            } catch(Exception ex)
            {
                registerListView.SelectedItems[0].SubItems[2].Text = resultTask.Exception.InnerException.Message;
            }
        }

        private void packetTracerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (transport == null)
            {
                MessageBox.Show("No device connected");
                return;
            }

            var pt = new PacketTracer(transport);
            pt.Show();
        }
    }
}
