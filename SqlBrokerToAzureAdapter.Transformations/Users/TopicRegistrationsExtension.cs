using SqlBrokerToAzureAdapter.Producers.AzureTopics;
using SqlBrokerToAzureAdapter.Users.Added.V1;
using SqlBrokerToAzureAdapter.Users.Edited.V1;
using SqlBrokerToAzureAdapter.Users.Removed.V1;

namespace SqlBrokerToAzureAdapter.Users
{
    public static class TopicRegistrationsExtension
    {
        public static void AddUsers(this ITopicRegistry registry)
        {
            registry.Add(typeof(UserAddedContract), "sqlBroker-to-azureAdapter-example-v1");
            registry.Add(typeof(UserRemovedContract), "sqlBroker-to-azureAdapter-example-v1");
            registry.Add(typeof(UserNameChangedContract), "sqlBroker-to-azureAdapter-example-v1");
            registry.Add(typeof(UserContactInfoChangedContract), "sqlBroker-to-azureAdapter-example-v1");
        }
    }
}