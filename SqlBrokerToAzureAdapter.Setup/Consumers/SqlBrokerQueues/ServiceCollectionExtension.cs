using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues;

namespace SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues
{
    /// <summary>
    /// Extension methods for adding services to an <see cref="IServiceCollection" />.
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Adds all services to execute <see cref="SqlBrokerQueueInstallation"/>
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configurationRoot"></param>
        /// <returns></returns>
        internal static IServiceCollection AddSqlBrokerQueueInstallation(this IServiceCollection collection,
            IConfigurationRoot configurationRoot)
        {
            collection.AddScoped<ISqlServerInstallationConnectionConfiguration>(x => configurationRoot.GetSection("Setup:SqlBrokerQueueConsumer:Connection").Get<SqlServerInstallationConnectionConfiguration>());
            collection.AddScoped<ISqlBrokerQueueGenerationConfiguration>(x => configurationRoot.GetSection("Setup:SqlBrokerQueueConsumer:Generation").Get<SqlBrokerQueueGenerationConfiguration>());
            collection.AddScoped<ISqlBrokerQueueInstallation, SqlBrokerQueueInstallation>();
            collection.AddScoped<ISqlBrokerQueueSetupRepository, SqlBrokerQueueSetupRepository>();
            collection.AddScoped<ISqlBrokerQueueSqlScriptGenerator, SqlBrokerQueueSqlScriptGenerator>();

            return collection;
        }
    }
}