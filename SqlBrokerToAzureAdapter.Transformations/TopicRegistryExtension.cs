using SqlBrokerToAzureAdapter.Producers.Common;
using SqlBrokerToAzureAdapter.Users;

namespace SqlBrokerToAzureAdapter
{
    public static class TopicRegistryExtension
    {
        public static void AddTopicRegistrations(this ITopicRegistry configuration)
        {
            configuration.AddUsers();
        }
    }
}