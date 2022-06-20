using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues;

namespace SqlBrokerToAzureAdapter.Setup
{
    /// <summary>
    /// Extension methods for adding services to an <see cref="IServiceCollection" />.
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Adds all services to install the SqlBrokerToAzureAdapter.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configurationRoot"></param>
        /// <returns></returns>
        public static IServiceCollection AddSqlBrokerToAzureAdapterSetup(this IServiceCollection collection,
            IConfigurationRoot configurationRoot)
        {
            collection.AddSqlBrokerQueueInstallation(configurationRoot);

            return collection;
        }
    }
}