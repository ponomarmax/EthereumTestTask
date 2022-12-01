using Microsoft.Extensions.Logging;
using Nethereum.JsonRpc.WebSocketStreamingClient;

namespace EthTestTask.Detecters
{
    public abstract class DetectorBase
    {
        protected readonly DetectorConfiguration config;
        protected readonly ILogger logger;
        protected HashSet<string> toAddressesForMonistoring;
        public DetectorBase(DetectorConfiguration config, ILoggerFactory loggerFactory)
        {
            this.config = config ?? throw new ArgumentNullException("Configuration is empty");
            if (config.httpUrl == null || config.wsUrl == null || config.toAddressMonitorList == null)
                throw new ArgumentNullException("One of configuration parameters is null");
            if (!config.toAddressMonitorList.Any())
                throw new ArgumentNullException("the list of monitoring address is empty");
            toAddressesForMonistoring = new HashSet<string>(config.toAddressMonitorList.Select(x => x.ToLower()));
            this.logger = loggerFactory.CreateLogger(this.GetType());
            this.logger.LogInformation("Detector initialized");
        }

        public async Task Detect(CancellationToken cancellationToken)
        {
            try
            {
                using var client = new StreamingWebSocketClient(config.wsUrl);
                await DetectInternal(client, cancellationToken);
                while (!cancellationToken.IsCancellationRequested) await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Something went wrong during detection");
            }
        }

        protected abstract Task DetectInternal(StreamingWebSocketClient client, CancellationToken cancellationToken);
    }
}
