using System;
using System.Threading.Tasks;

namespace SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues
{
    internal interface ISqlBrokerQueueSetupRepository : IAsyncDisposable
    {
        Task OpenSqlConnectionAsync();
        Task BeginTransactionAsync();
        Task ExecuteSqlScriptAsync(string sqlScript);
        Task RollbackTransactionAsync();
        Task CommitTransactionAsync();
        Task CloseSqlConnectionAsync();
    }
}