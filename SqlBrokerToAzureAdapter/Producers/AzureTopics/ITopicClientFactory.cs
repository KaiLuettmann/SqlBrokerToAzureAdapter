using System;
using Microsoft.Azure.ServiceBus;

namespace SqlBrokerToAzureAdapter.Producers.AzureTopics
{
    internal interface ITopicClientFactory
    {
        ITopicClient Get(Type payloadType);
    }
}