using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlBrokerToAzureAdapter.Adapter;
using SqlBrokerToAzureAdapter.Consumers;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues;
using SqlBrokerToAzureAdapter.Producers.AzureTopics;
using SqlBrokerToAzureAdapter.Transformations;

namespace SqlBrokerToAzureAdapter
{
    /// <summary>
    /// Extension methods for adding services to an <see cref="IServiceCollection" />.
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Adds all services to execute <see cref="SqlBrokerQueueConsumer"/>,
        /// <see cref="AzureTopicProducer"/> and <see cref="SqlBrokerToAzureAdapter"/> (without given generic contracts)
        /// to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configurationRoot"></param>
        /// <returns></returns>
        public static IServiceCollection AddSqlBrokerToAzureAdapter(this IServiceCollection collection,
            IConfigurationRoot configurationRoot)
        {
            var sqlBrokerQueueConfigurationSection = configurationRoot.GetSection("Execution:SqlBrokerQueueConsumer");
            collection.AddSqlBrokerQueueConsumer(sqlBrokerQueueConfigurationSection);

            var azureTopicConfigurationSection = configurationRoot.GetSection("Execution:AzureTopicProducer");
            collection.AddAzureTopicProducer(azureTopicConfigurationSection);

            var sqlBrokerToAzureAdapterSection = configurationRoot.GetSection("Execution:SqlBrokerToAzureAdapter");
            collection.AddAdapter(sqlBrokerToAzureAdapterSection);

            return collection;
        }

        /// <summary>
        /// Adds all services to execute <see cref="SqlBrokerToAzureAdapter"/>
        /// with <typeparamref name="TDatabaseContract"/>
        /// to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <typeparam name="TDatabaseContract">The type of contract to operate.</typeparam>
        public static void AddSqlBrokerToAzureAdapter<TDatabaseContract>(this IServiceCollection collection)
        {
            collection.AddScoped<ISqlBrokerMessageReceiver<TDatabaseContract>, SqlBrokerToAzureAdapter<TDatabaseContract>>();
            collection.AddTransformations<TDatabaseContract>();
        }
    }
}