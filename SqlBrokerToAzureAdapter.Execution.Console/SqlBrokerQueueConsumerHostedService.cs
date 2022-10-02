using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues;

namespace SqlBrokerToAzureAdapter{
    public class SqlBrokerQueueConsumerHostedService : IHostedService
    {
        private readonly ISqlBrokerQueueConsumer _consumer;

        public SqlBrokerQueueConsumerHostedService(ISqlBrokerQueueConsumer consumer)
        {
            _consumer = consumer;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _consumer.RunAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}


