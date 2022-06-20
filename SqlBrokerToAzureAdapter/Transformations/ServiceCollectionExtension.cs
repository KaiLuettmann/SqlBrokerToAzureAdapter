using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues;

namespace SqlBrokerToAzureAdapter.Transformations
{
    internal static class ServiceCollectionExtension
    {
        internal static void AddTransformations<T>(this IServiceCollection collection)
        {
            collection.AddScoped<IObjectComparer<T>, ObjectComparer<T>>();
        }
    }
}