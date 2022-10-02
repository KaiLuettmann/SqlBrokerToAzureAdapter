using Microsoft.Extensions.DependencyInjection;

namespace SqlBrokerToAzureAdapter.Producers.Common
{
    internal static class ServiceCollectionExtension
    {
        internal static void AddCommon(this IServiceCollection collection)
        {
            collection.AddScoped<ITopicRegistry>(x => new TopicRegistry());
        }
    }
}