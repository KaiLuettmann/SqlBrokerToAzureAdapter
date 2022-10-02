using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlBrokerToAzureAdapter.Adapter;
using SqlBrokerToAzureAdapter.Producers.Common;

namespace SqlBrokerToAzureAdapter.Producers.AzureTopics
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

            collection.AddCommon();
            collection
                .AddScoped<IAzureTopicConfiguration>(x => configuration.Get<AzureTopicConfiguration>())
                .AddScoped<IAzureTopicClientFactory, AzureTopicClientFactory>()
                .AddScoped<ITopicProducer, AzureTopicProducer>();
        }
    }
}