using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues;

namespace SqlBrokerToAzureAdapter.Consumers
{
    /// <summary>
    /// Extensions for the <see cref="IServiceCollection"/> to add consumer
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Adds sql broker consumer
        /// </summary>
        /// <param name="collection">the service collection</param>
        /// <param name="configuration">The configuration</param>
        /// <returns></returns>
        public static IServiceCollection AddSqlBrokerQueueConsumer(this IServiceCollection collection,
            IConfigurationSection configuration)
        {
            var config = configuration.Get<SqlBrokerQueueConfiguration>();
            if (config == null)
            {
                throw new ConfigurationErrorsException("Could not load the configuration for the 'SqlBrokerQueue'. Did you forget to create an appsettings.json file?");
            }
            collection
                .AddScoped<ISqlBrokerMessageHandlerCollection>(_ => new SqlBrokerMessageHandlerCollection())
                .AddScoped<ISqlBrokerQueueConfiguration>(_ => config)
                .AddScoped<ISqlBrokerQueueRepository, SqlBrokerQueueRepository>()
                .AddScoped<ISqlBrokerQueueConsumer, SqlBrokerQueueConsumer>();

            return collection;
        }
    }
}