using System.Collections.Generic;
using SqlBrokerToAzureAdapter.MessageContracts;

namespace SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues
{
    internal interface IMessageBodyDeserializer<TDatabaseContract>
    {
        IEnumerable<TDatabaseContract> ToInsertContract(string messageMessageBody);
        IEnumerable<TDatabaseContract> ToDeletedContract(string messageMessageBody);
        IEnumerable<UpdatedPair<TDatabaseContract>> ToUpdateContract(string messageMessageBody);
    }
}