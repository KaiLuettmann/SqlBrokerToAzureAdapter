using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlBrokerToAzureAdapter.Adapter;
using SqlBrokerToAzureAdapter.Producers.Common;

namespace SqlBrokerToAzureAdapter.Producers.AzureTopics
{
    /// <summary>
    /// Extensions for the <see cref="IServiceCollection"/> to add producer
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Adds Azure topic producer
        /// </summary>
        /// <param name="collection">the service collection</param>
        /// <param name="configuration">The configuration</param>
        /// <returns></returns>
        public static IServiceCollection AddAzureTopicProducer(this IServiceCollection collection,
            IConfigurationSection configuration)
        {
            var config = configuration.Get<AzureTopicConfiguration>();
            if (config == null)
            {
                throw new ConfigurationErrorsException("Could not load the configuration for the 'AzureTopicProducer'. Did you forget to create an appsettings.json file?");
            }

            collection.AddCommon();
            collection
                .AddScoped<IAzureTopicConfiguration>(_ => configuration.Get<AzureTopicConfiguration>())
                .AddScoped<IAzureTopicClientFactory, AzureTopicClientFactory>()
                .AddScoped<ITopicProducer, AzureTopicProducer>();

            return collection;
        }
    }
}