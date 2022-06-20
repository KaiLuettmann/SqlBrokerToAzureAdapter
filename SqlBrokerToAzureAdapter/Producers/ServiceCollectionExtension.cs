using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlBrokerToAzureAdapter.Producers.AzureTopics;
using SqlBrokerToAzureAdapter.Producers.AzureTopics.Models;

namespace SqlBrokerToAzureAdapter.Producers
{
    internal static class ServiceCollectionExtension
    {
        internal static void AddAzureQueueProducer(this IServiceCollection collection,
            IConfigurationSection configuration)
        {
            var config = configuration.Get<AzureTopicConfiguration>();
            if (config == null)
            {
                throw new ConfigurationErrorsException("Could not load the configuration for the 'AzureTopicProducer'. Did you forget to create an appsettings.json file?");
            }
            collection
                .AddScoped<IAzureTopicConfiguration>(x => configuration.Get<AzureTopicConfiguration>())
                .AddScoped<ITopicRegistry>(x => new TopicRegistry())
                .AddScoped<ITopicClientFactory, TopicClientFactory>()
                .AddScoped<IAzureTopicProducer, AzureTopicProducer>();
        }
    }
}