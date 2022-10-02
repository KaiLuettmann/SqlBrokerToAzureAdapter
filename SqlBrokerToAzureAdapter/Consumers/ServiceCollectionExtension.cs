using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues;

namespace SqlBrokerToAzureAdapter.Consumers
{
    internal static class ServiceCollectionExtension
    {
        internal static void AddSqlBrokerQueueConsumer(this IServiceCollection collection,
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
        }
    }
}