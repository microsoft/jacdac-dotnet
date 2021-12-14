using Jacdac.Services;

namespace Jacdac
{
    public class JDDevice
    {
        public ulong DeviceIdentifier { get; }

        public ControlService.AnnounceFlags DeviceFlags { get; }

        protected JDBus Bus { get; }
        public DateTime LastSeen { get; set; }

        public JDService[] Services { get; private set; }

        public ControlService ControlService => (ControlService)Services[0];

        private SemaphoreSlim sendSemaphore = new SemaphoreSlim(1);
        private JDPacket currentPacket;
        private TaskCompletionSource<JDPacket> responseTask;
        private short[] eventCounters = new short[128];
        private byte expectedEventCounter = 127;


        public JDDevice(ulong identifier, ControlService.AnnouncementReport announcementReport, JDBus bus)
        {
            DeviceIdentifier = identifier;
            Bus = bus;
            LastSeen = DateTime.Now;
            DeviceFlags = announcementReport.AnnounceFlags;

            Services = new JDService[announcementReport.Services.Length + 1];
            Services[0] = ServiceRegistry.InstantiateService(0, 0, this);
            for (byte i = 0; i < announcementReport.Services.Length; i++)
            {
                Services[i + 1] = ServiceRegistry.InstantiateService(announcementReport.Services[i], (byte)(i + 1), this);
            }
        }

        public T[] GetServices<T>() where T: JDService
        {
            return (T[])Services.Where(s => s.GetType() == typeof(T));
        }

        public async Task<JDPacket> SendPacketWithResponse(JDPacket packet, int timeout = 100, int retries = 3)
        {
            await sendSemaphore.WaitAsync();

            var retryCount = 0;
            currentPacket = packet;

            while (retryCount < retries)
            {
                responseTask = new TaskCompletionSource<JDPacket>();
                await Bus.SendPacket(packet);
                var completedTask = await Task.WhenAny(responseTask.Task, Task.Delay(timeout));
                if (completedTask == responseTask.Task)
                {
                    sendSemaphore.Release();
                    return responseTask.Task.Result;
                }

                retryCount++;
            }

            sendSemaphore.Release();
            throw new Exception($"No response after {retries} retries");
        }

        public async Task SendPacket(JDPacket packet)
        {
            await sendSemaphore.WaitAsync();
            await Bus.SendPacket(packet);
            sendSemaphore.Release();
        }

        internal void HandlePacket(JDPacket packet)
        {
            short GetEventLogId(byte serviceIndex, byte eventId) => (short)((serviceIndex << 8) | eventId);

            if (packet.IsReport && packet.ServiceIndex == currentPacket.ServiceIndex && packet.ServiceCommand == currentPacket.ServiceCommand)
                responseTask?.TrySetResult(packet);

            if (packet.OperationType == JDPacket.CommandType.Event && Services.Length > packet.ServiceIndex)
            {
                if (packet.EventCounter <= expectedEventCounter + 1 && eventCounters[expectedEventCounter] != GetEventLogId(packet.ServiceIndex, packet.EventCode))
                {
                    eventCounters[packet.EventCounter] = GetEventLogId(packet.ServiceIndex, packet.EventCode);
                    expectedEventCounter = packet.EventCounter;
                    Services[packet.ServiceIndex].HandleEvent(packet);
                }

                if (expectedEventCounter == 127)
                    expectedEventCounter = 0;
            }
        }
    }
}
