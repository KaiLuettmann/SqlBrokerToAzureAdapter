using System.Collections.Generic;
using System.Threading.Tasks;
using SqlBrokerToAzureAdapter.MessageContracts;

namespace SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues
{
    internal interface ISqlBrokerMessageReceiver<TDatabaseContract>
    {
        Task ReceiveInsertedAsync(Metadata metadata, IEnumerable<TDatabaseContract> values);

        Task ReceiveUpdatedAsync(Metadata metadata, IEnumerable<UpdatedPair<TDatabaseContract>> values);

        Task ReceiveDeletedAsync(Metadata metadata, IEnumerable<TDatabaseContract> values);
    }
}