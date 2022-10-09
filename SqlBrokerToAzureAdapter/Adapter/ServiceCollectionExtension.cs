using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SqlBrokerToAzureAdapter.Adapter
{
    /// <summary>
    /// Extensions for the <see cref="IServiceCollection"/> to add adapter
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Adds the adapter
        /// </summary>
        /// <param name="collection">the service collection</param>
        /// <param name="configuration">The configuration</param>
        public static IServiceCollection AddAdapter(this IServiceCollection collection,
            IConfigurationSection configuration)
        {
            var config = configuration.Get<SqlBrokerToAzureAdapterConfiguration>() ?? new SqlBrokerToAzureAdapterConfiguration();
            collection.AddScoped<ISqlBrokerToAzureAdapterConfiguration>(_ => config);

            return collection;
        }
    }
}