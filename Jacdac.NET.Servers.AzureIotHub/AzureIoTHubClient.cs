using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace Jacdac.Servers
{
    public class AzureIoTHubClient
        : IAzureIoTHubHealth
    {
        private string connectionString;

        private readonly ISettingsStorage _storage;
        private readonly TransportType _transportType;
        private readonly ClientOptions _clientOptions = new ClientOptions { SdkAssignsMessageId = SdkAssignsMessageId.WhenUnset };

        // An UnauthorizedException is handled in the connection status change handler through its corresponding status change event.
        // We will ignore this exception when thrown by the client API operation.
        private readonly Dictionary<Type, string> _exceptionsToBeIgnored = new Dictionary<Type, string> { { typeof(UnauthorizedException), "Unauthorized exceptions are handled by the ConnectionStatusChangeHandler." } };

        private readonly ILogger _logger;

        // Mark these fields as volatile so that their latest values are referenced.
        private volatile DeviceClient _deviceClient;
        private volatile ConnectionStatus _connectionStatus = ConnectionStatus.Disconnected;

        private CancellationTokenSource _cancellationTokenSource;

        public event EventHandler ConnectionStatusChanged;
        public event EventHandler MessageSent;

        public AzureIoTHubClient(TransportType transportType, ISettingsStorage storage = null, ILogger logger = null)
        {
            this._storage = storage;
            this._logger = logger;
            this._transportType = transportType;
            this._logger?.LogInformation($"Using {_transportType} transport.");

            var cbytes = this._storage?.Read(SETTING_NAME);
            if (cbytes != null)
                this.connectionString = UTF8Encoding.UTF8.GetString(cbytes);
        }

        public ConnectionStatus ConnectionStatus
        {
            get { return this._connectionStatus; }
        }

        private bool IsDeviceConnected => this._connectionStatus == ConnectionStatus.Connected;

        public async Task ConnectAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                await InitializeAndSetupClientAsync();
            }
            catch (OperationCanceledException)
            {
                // User canceled the operation. Nothing to do here.
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Unrecoverable exception caught, user action is required, so exiting: \n{ex}");
                _cancellationTokenSource.Cancel();
            }
        }

        public async Task DisconnectAsync()
        {
            var tokenSource = this._cancellationTokenSource;
            if (tokenSource != null)
            {
                this._cancellationTokenSource = null;
                if (!tokenSource.IsCancellationRequested)
                    tokenSource.Cancel();
                var client = this._deviceClient;
                // If the device client instance has been previously initialized, then dispose it.
                if (client != null)
                {
                    this._deviceClient = null;

                    await client.CloseAsync(tokenSource.Token);
                    client.Dispose();
                }
            }
        }

        private async Task InitializeAndSetupClientAsync()
        {
            var cancellationToken = this._cancellationTokenSource.Token;
            if (ShouldClientBeInitialized(_connectionStatus))
            {
                if (ShouldClientBeInitialized(_connectionStatus))
                {
                    _logger?.LogDebug($"Attempting to initialize the client instance, current status={_connectionStatus}");

                    // If the device client instance has been previously initialized, then dispose it.
                    if (_deviceClient != null)
                    {
                        await _deviceClient.CloseAsync(cancellationToken);
                        _deviceClient.Dispose();
                        _deviceClient = null;
                    }

                    _deviceClient = DeviceClient.CreateFromConnectionString(this.connectionString, this._transportType, this._clientOptions);
                    _deviceClient.SetConnectionStatusChangesHandler(HandleConnectionStatusChanged);
                    _logger?.LogDebug("Initialized the client instance.");
                }

                // Force connection now.
                // We have set the "shouldExecuteOperation" function to always try to open the connection.
                // OpenAsync() is an idempotent call, it has the same effect if called once or multiple times on the same client.
                await RetryOperationHelper.RetryTransientExceptionsAsync(
                    operationName: "OpenConnection",
                    asyncOperation: async () => await this._deviceClient.OpenAsync(cancellationToken),
                    shouldExecuteOperation: () => true,
                    logger: this._logger,
                    exceptionsToBeIgnored: this._exceptionsToBeIgnored,
                    cancellationToken: cancellationToken);
                _logger.LogDebug($"The client instance has been opened.");

                // You will need to subscribe to the client callbacks any time the client is initialized.
                await RetryOperationHelper.RetryTransientExceptionsAsync(
                    operationName: "SubscribeTwinUpdates",
                    asyncOperation: async () => await this._deviceClient.SetDesiredPropertyUpdateCallbackAsync(HandleTwinUpdateNotification, cancellationToken),
                    shouldExecuteOperation: () => this.IsDeviceConnected,
                    logger: this._logger,
                    exceptionsToBeIgnored: this._exceptionsToBeIgnored,
                    cancellationToken: cancellationToken);
                _logger?.LogDebug("The client has subscribed to desired property update notifications.");
            }
        }

        // It is not good practice to have async void methods, however, DeviceClient.SetConnectionStatusChangesHandler() event handler signature has a void return type.
        // As a result, any operation within this block will be executed unmonitored on another thread.
        // To prevent multi-threaded synchronization issues, the async method InitializeClientAsync being called in here first grabs a lock
        // before attempting to initialize or dispose the device client instance.
        private async void HandleConnectionStatusChanged(ConnectionStatus status, ConnectionStatusChangeReason reason)
        {
            _logger?.LogDebug($"Connection status changed: status={status}, reason={reason}");
            _connectionStatus = status;
            this.ConnectionStatusChanged?.Invoke(this, EventArgs.Empty);
            switch (status)
            {
                case ConnectionStatus.Connected:
                    _logger?.LogDebug("### The DeviceClient is CONNECTED; all operations will be carried out as normal.");
                    break;

                case ConnectionStatus.Disconnected_Retrying:
                    _logger?.LogDebug("### The DeviceClient is retrying based on the retry policy. Do NOT close or open the DeviceClient instance");
                    break;

                case ConnectionStatus.Disabled:
                    _logger?.LogDebug("### The DeviceClient has been closed gracefully." +
                        "\nIf you want to perform more operations on the device client, you should dispose (DisposeAsync()) and then open (OpenAsync()) the client.");
                    break;

                case ConnectionStatus.Disconnected:
                    switch (reason)
                    {
                        case ConnectionStatusChangeReason.Bad_Credential:
                            _logger?.LogWarning("### The supplied credentials are invalid. Update the parameters and run again.");
                            _cancellationTokenSource.Cancel();
                            break;

                        case ConnectionStatusChangeReason.Device_Disabled:
                            _logger?.LogWarning("### The device has been deleted or marked as disabled (on your hub instance)." +
                                "\nFix the device status in Azure and then create a new device client instance.");
                            _cancellationTokenSource.Cancel();
                            break;

                        case ConnectionStatusChangeReason.Retry_Expired:
                            _logger?.LogWarning("### The DeviceClient has been disconnected because the retry policy expired." +
                                "\nIf you want to perform more operations on the device client, you should dispose (DisposeAsync()) and then open (OpenAsync()) the client.");
                            this.Reconnect();
                            break;

                        case ConnectionStatusChangeReason.Communication_Error:
                            _logger?.LogWarning("### The DeviceClient has been disconnected due to a non-retry-able exception. Inspect the exception for details." +
                                "\nIf you want to perform more operations on the device client, you should dispose (DisposeAsync()) and then open (OpenAsync()) the client.");
                            this.Reconnect();
                            break;

                        default:
                            _logger?.LogError("### This combination of ConnectionStatus and ConnectionStatusChangeReason is not expected, contact the client library team with logs.");
                            break;
                    }

                    break;

                default:
                    _logger?.LogError("### This combination of ConnectionStatus and ConnectionStatusChangeReason is not expected, contact the client library team with logs.");
                    break;
            }
        }

        private async Task HandleTwinUpdateNotification(TwinCollection twinUpdateRequest, object userContext)
        {
            CancellationToken cancellationToken = (CancellationToken)userContext;

            if (!cancellationToken.IsCancellationRequested)
            {
                /*
                var reportedProperties = new TwinCollection();

                _logger.LogInformation($"Twin property update requested: \n{twinUpdateRequest.ToJson()}");

                // For the purpose of this sample, we'll blindly accept all twin property write requests.
                foreach (KeyValuePair<string, object> desiredProperty in twinUpdateRequest)
                {
                    _logger.LogInformation($"Setting property {desiredProperty.Key} to {desiredProperty.Value}.");
                    reportedProperties[desiredProperty.Key] = desiredProperty.Value;
                }

                // For the purpose of this sample, we'll blindly accept all twin property write requests.
                await RetryOperationHelper.RetryTransientExceptionsAsync(
                    operationName: "UpdateReportedProperties",
                    asyncOperation: async () => await _deviceClient.UpdateReportedPropertiesAsync(reportedProperties, cancellationToken),
                    shouldExecuteOperation: () => IsDeviceConnected,
                    logger: _logger,
                    exceptionsToBeIgnored: _exceptionsToBeIgnored,
                    cancellationToken: cancellationToken);
                */
            }
        }

        public async Task SendMessagesAsync(Message message)
        {
            var cancellationToken = this._cancellationTokenSource.Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                if (this.IsDeviceConnected)
                {
                    await RetryOperationHelper.RetryTransientExceptionsAsync(
                        operationName: $"send_message_{message.MessageId}",
                        asyncOperation: async () => await _deviceClient.SendEventAsync(message),
                        shouldExecuteOperation: () => IsDeviceConnected,
                        logger: _logger,
                        exceptionsToBeIgnored: _exceptionsToBeIgnored,
                        cancellationToken: cancellationToken);
                    this.MessageSent?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        // If the client reports Connected status, it is already in operational state.
        // If the client reports Disconnected_retrying status, it is trying to recover its connection.
        // If the client reports Disconnected status, you will need to dispose and recreate the client.
        // If the client reports Disabled status, you will need to dispose and recreate the client.
        private bool ShouldClientBeInitialized(ConnectionStatus connectionStatus)
        {
            return (connectionStatus == ConnectionStatus.Disconnected || connectionStatus == ConnectionStatus.Disabled)
                && !String.IsNullOrEmpty(this.connectionString);
        }

        string IAzureIoTHubHealth.HubName
        {
            get
            {
                var parts = this.connectionString?.Split(";").Select(part => part.Split("=")).ToDictionary(kv => kv[0], kv => kv[1]);
                string res;
                if (parts != null && parts.TryGetValue("HostName", out res))
                    return res;
                return "";
            }
        }

        string IAzureIoTHubHealth.HubDeviceId
        {
            get
            {
                var parts = this.connectionString?.Split(";").Select(part => part.Split("=")).ToDictionary(kv => kv[0], kv => kv[1]);
                string res;
                if (parts != null && parts.TryGetValue("DeviceId", out res))
                    return res;
                return "";
            }
        }

        AzureIotHubHealthConnectionStatus IAzureIoTHubHealth.ConnectionStatus
        {
            get
            {
                switch (this.ConnectionStatus)
                {
                    case ConnectionStatus.Connected: return AzureIotHubHealthConnectionStatus.Connected;
                    case ConnectionStatus.Disabled: return AzureIotHubHealthConnectionStatus.Disconnected;
                    case ConnectionStatus.Disconnected_Retrying: return AzureIotHubHealthConnectionStatus.Connecting;
                    default: return AzureIotHubHealthConnectionStatus.Disconnected;
                }
            }
        }

        void IAzureIoTHubHealth.Connect()
        {
            this.ConnectAsync();
        }

        void IAzureIoTHubHealth.Disconnect()
        {
            this.DisconnectAsync();
        }

        const string SETTING_NAME = "azureiothub_connectionstring";

        void IAzureIoTHubHealth.SetConnectionString(string connectionString)
        {
            this._storage?.Write(SETTING_NAME, UTF8Encoding.UTF8.GetBytes(connectionString));
            this.connectionString = connectionString;
            this.Reconnect();
        }

        private async void Reconnect()
        {
            await this.DisconnectAsync();
            await this.ConnectAsync();
        }
    }
}
