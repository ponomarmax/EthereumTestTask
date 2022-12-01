using Microsoft.Extensions.Logging;
using Nethereum.Contracts;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Reactive.Eth.Subscriptions;
using Nethereum.Web3;
using System;

namespace EthTestTask.Detecters
{
    public class EthTransferDetector : DetectorBase
    {
        private readonly Web3 web3;
        public EthTransferDetector(DetectorConfiguration config, ILoggerFactory logger) : base(config, logger)
        {
            web3 = new Web3(config.httpUrl);
        }

        protected override async Task DetectInternal(StreamingWebSocketClient client, CancellationToken cancellationToken)
        {
            
            var subscription = new EthNewBlockHeadersObservableSubscription(client);

            subscription.GetSubscribeResponseAsObservable().Subscribe(subscriptionId =>
                logger.LogInformation("EthTransferDetector  subscripted with Id: {subscribptionID}", subscriptionId));

            subscription.GetSubscriptionDataResponsesAsObservable().Subscribe(EventHandler, cancellationToken);

            await client.StartAsync();

            await subscription.SubscribeAsync();
        }

        private void EventHandler(Block blockq)
        {
            var block = web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(blockq.Number);
            block.Wait();

            if (block != null && block.Result.Transactions != null)
            {
                foreach (var transaction in block.Result.Transactions)
                {
                    if (toAddressesForMonistoring.Contains(transaction.To))
                    {
                        logger.LogInformation("Detected ETH tranfer from {from} to {to}", transaction.From, transaction.To);
                    }
                }
            }
        }
    }
}
