using System;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues;
using SqlBrokerToAzureAdapter.Users;

namespace SqlBrokerToAzureAdapter
{
    public static class SqlBrokerMessageHandlerCollectionExtension
    {
        public static void AddSqlBrokerMessageHandlers(this ISqlBrokerMessageHandlerCollection collection,
            IServiceProvider provider)
        {
            collection.AddUsers(provider);
        }
    }
}