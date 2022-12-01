using Microsoft.Extensions.Logging;
using Nethereum.Contracts;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Reactive.Eth.Subscriptions;

namespace EthTestTask.Detecters
{
    public class ERC20TokenDetector : DetectorBase
    {
        public ERC20TokenDetector(DetectorConfiguration config, ILoggerFactory logger) : base(config, logger) { }

        protected override async Task DetectInternal(StreamingWebSocketClient client, CancellationToken cancellationToken)
        {
            var filterTransfers = Event<TransferEventDTO>.GetEventABI().CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);

            subscription.GetSubscriptionDataResponsesAsObservable().
                         Subscribe(EventHandler, cancellationToken);

            await client.StartAsync();
            subscription.GetSubscribeResponseAsObservable().Subscribe(id =>
                logger.LogInformation("ERC20TokenDetector subscripted with Id: {subscribptionID}", id));
            await subscription.SubscribeAsync(filterTransfers);
        }

        private void EventHandler(FilterLog log)
        {
            try
            {
                EventLog<TransferEventDTO> decoded = Event<TransferEventDTO>.DecodeEvent(log);
                if (decoded != null || toAddressesForMonistoring.Contains(decoded.Event.To))
                {
                    logger.LogInformation("Detected ERC-20 token transfer to {address}", decoded.Event.To);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("During decodind exception happened {message}", ex.Message);
            }
        }
    }
}
