using Microsoft.Extensions.Logging;
using Nethereum.Contracts;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Reactive.Eth.Subscriptions;
using Nethereum.Web3;
using System.Text;

namespace EthTestTask.Detecters
{
    public class TransferFromContractDetector : DetectorBase
    {
        public TransferFromContractDetector(DetectorConfiguration config, ILoggerFactory logger) : base(config, logger)
        {
        }

        protected override async Task DetectInternal(StreamingWebSocketClient client, CancellationToken cancellationToken)
        {
            var subscription = new EthLogsObservableSubscription(client);

            subscription.GetSubscriptionDataResponsesAsObservable().
                         Subscribe(EventHandler, cancellationToken);

            await client.StartAsync();
            subscription.GetSubscribeResponseAsObservable().Subscribe(id =>
                logger.LogInformation("TransferFromContractDetector subscripted with Id: {subscribptionID}", id));
            await subscription.SubscribeAsync();
        }

        private void EventHandler(FilterLog log)
        {
            try
            {
                var supposedAddresses = log.Topics.Select(
                    x =>
                        {
                            var r = (string)x;
                            return "0x" + r.Substring(r.Length - 40, 40);
                        }
                    );
                var intersect = toAddressesForMonistoring.Intersect(supposedAddresses);
                if (intersect.Any())
                {
                    foreach (var addressTo in intersect)
                    {
                        logger.LogInformation("Detected supposed incoming transfer from smart contract to {address}. txHash {txHash}",
                                addressTo, log.TransactionHash);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("During decodind exception happened {message}", ex.Message);
            }
        }
    }
}
