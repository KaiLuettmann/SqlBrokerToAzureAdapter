using System;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues;
using SqlBrokerToAzureAdapter.Users.SqlBrokerMessageContracts;

namespace SqlBrokerToAzureAdapter.Users
{
    public static class SqlBrokerMessageHandlerCollectionExtension
    {
        public static void AddUsers(this ISqlBrokerMessageHandlerCollection collection,
            IServiceProvider provider)
        {
            var handlerFactory = new SqlBrokerMessageHandlerFactory<UserContract>(provider);
            collection.Add(handlerFactory.Get("User.Deleted", SqlBrokerMessageType.Deleted));
            collection.Add(handlerFactory.Get("User.Inserted", SqlBrokerMessageType.Inserted));
            collection.Add(handlerFactory.Get("User.Updated", SqlBrokerMessageType.Updated));
        }
    }
}