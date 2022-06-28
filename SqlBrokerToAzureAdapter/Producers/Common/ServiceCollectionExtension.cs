using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlBrokerToAzureAdapter.Producers.AzureTopics;
using SqlBrokerToAzureAdapter.Producers.Common.Models;

namespace SqlBrokerToAzureAdapter.Producers.Common
{
    internal static class ServiceCollectionExtension
    {
        internal static void AddCommon(this IServiceCollection collection)
        {
            collection.AddScoped<ITopicRegistry>(x => new TopicRegistry());
        }
    }
}