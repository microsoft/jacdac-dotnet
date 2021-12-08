using GHIElectronics.TinyCLR.Networking.Mqtt;
using System;
using System.Collections;
using System.Security.Authentication;
using System.Text;
using System.Threading;

namespace Jacdac_RgbLed
{
    internal class Adafruit
    {
        public delegate void ToggleEventHandler(bool value);

        public static event ToggleEventHandler ToggleEvent;

        public static void Connect(string username, string key, string dashboardId, string feed)
        {

            var mqttHost = "io.adafruit.com";
            var mqttPort = 1883; //Default SSL port is 8883, default insecure port is 1883.          

            try
            {
                var clientSetting = new MqttClientSetting
                {
                    BrokerName = mqttHost,
                    BrokerPort = mqttPort,
                    ClientCertificate = null,
                    CaCertificate = null,
                    SslProtocol = SslProtocols.None,
                };

                var client = new Mqtt(clientSetting);

                var connectSetting = new MqttConnectionSetting
                {
                    ClientId = dashboardId,
                    UserName = username,
                    Password = key
                };

                // Connect to host
                var returnCode = client.Connect(connectSetting);

                if (returnCode != ConnectReturnCode.ConnectionAccepted)
                {
                    throw new Exception("Could not connect to Adafruit. Make sure datetime is up to date");
                }
                   

                var packetId = 1;

                // Subscribe to a feed
                client.Subscribe(new string[] { feed }, new QoSLevel[] { QoSLevel.ExactlyOnce },
                    (ushort)packetId++);

                // Publish to a feed
                client.PublishReceivedChanged += Client_PublishReceivedChanged;
                
            }

            catch (Exception e)
            {

            }            
        }

        private static void Client_PublishReceivedChanged(object sender, string topic, byte[] data, bool duplicate, QoSLevel qosLevel, bool retain)
        {
            if (data != null)
            {                
                ToggleEvent?.Invoke(data[0] == 48 ? false : true);
            }
               
        }
    }
}
