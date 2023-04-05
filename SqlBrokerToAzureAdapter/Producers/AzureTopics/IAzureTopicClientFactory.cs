using System;
using Microsoft.Azure.ServiceBus;

namespace SqlBrokerToAzureAdapter.Producers.AzureTopics
{
    internal interface IAzureTopicClientFactory
    {
        ITopicClient Get(Type payloadType);
    }
}