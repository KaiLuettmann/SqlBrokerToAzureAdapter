using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using SqlBrokerToAzureAdapter.Adapter;

namespace SqlBrokerToAzureAdapter.Producers.MassTransit
{
    /// <summary>
    /// Extensions for the <see cref="IServiceCollection"/> to add producer
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Adds RabbitMq producer
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMqTopicProducer(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ITopicProducer, MasstransitTopicProducer>()
                .AddMassTransit(x =>
                {
                    x.UsingRabbitMq((_, cfg) =>
                    {
                        cfg.Host("localhost", 49154, "/", h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                            h.ConfigureBatchPublish(x => x.Enabled = true);
                        });
                    });
                });
            return serviceCollection;
        }
    }
}