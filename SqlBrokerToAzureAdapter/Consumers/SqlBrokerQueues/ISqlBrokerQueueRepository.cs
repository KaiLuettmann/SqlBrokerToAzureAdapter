using System;
using System.Threading.Tasks;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.Models;

namespace SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues
{
    internal interface ISqlBrokerQueueRepository : IAsyncDisposable
    {
        Task OpenSqlConnectionAsync();
        Task BeginTransactionAsync();
        Task<BrokerMessage> ReceiveNextMessageAsync();
        Task RollbackTransactionAsync();
        Task ResetSkippedBrokerMessagesAsync();
        Task EndConversationAsync(Guid messageConversationHandle);
        Task CommitTransactionAsync();
        Task SkipCurrentBrokerMessageAsync();
        Task CloseSqlConnectionAsync();
    }
}