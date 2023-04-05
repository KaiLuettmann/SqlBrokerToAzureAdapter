using Microsoft.Extensions.DependencyInjection;

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