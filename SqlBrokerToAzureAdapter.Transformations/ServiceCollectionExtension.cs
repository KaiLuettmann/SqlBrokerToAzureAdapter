using Microsoft.Extensions.DependencyInjection;
using SqlBrokerToAzureAdapter.Users;

namespace SqlBrokerToAzureAdapter
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddTransformations(this IServiceCollection collection)
        {
            collection.AddUsers();
            return collection;
        }
    }
}