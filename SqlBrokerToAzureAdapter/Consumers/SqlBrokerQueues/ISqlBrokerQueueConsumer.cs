using System;
using System.Threading;
using System.Threading.Tasks;

namespace SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues
{
    /// <summary>
    /// The consumer of a SQL Server Service Broker queue
    /// </summary>
    public interface ISqlBrokerQueueConsumer : IAsyncDisposable
    {
        /// <summary>
        /// Starts the consumption of messages from the SQL Server Service Broker queue
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task RunAsync(CancellationToken cancellationToken);
    }
}