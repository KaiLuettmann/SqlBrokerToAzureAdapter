using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SqlBrokerToAzureAdapter.Adapter
{
    internal static class ServiceCollectionExtension
    {
        internal static void AddAdapter(this IServiceCollection collection,
            IConfigurationSection configuration)
        {
            var config = configuration.Get<SqlBrokerToAzureAdapterConfiguration>() ?? new SqlBrokerToAzureAdapterConfiguration();
            collection.AddScoped<ISqlBrokerToAzureAdapterConfiguration>(x => config);
        }
    }
}