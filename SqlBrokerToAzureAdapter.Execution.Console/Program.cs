using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlBrokerToAzureAdapter.Adapter;
using SqlBrokerToAzureAdapter.Consumers;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues;
using SqlBrokerToAzureAdapter.Producers.AzureTopics;
using SqlBrokerToAzureAdapter.Producers.Common;
using SqlBrokerToAzureAdapter.Producers.MassTransit;
using SqlBrokerToAzureAdapter.Setup;
using SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues;

namespace SqlBrokerToAzureAdapter
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var configurationBuilder = ConfigureConfiguration(new ConfigurationBuilder());
            var config = configurationBuilder.Build();
            var host = new HostBuilder()
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddConfiguration(config.GetSection("Logging"));
                })
                .ConfigureAppConfiguration(configurationBuilder => ConfigureConfiguration(configurationBuilder))
                .ConfigureServices(serviceCollection =>
                {
                    ConfigureHostServices(serviceCollection, config);
                    serviceCollection.AddHostedService<SqlBrokerQueueConsumerHostedService>();
                })
                .Build();

            ConfigureAppServices(host.Services);

            switch (args[0])
            {
                case "run":
                    await host.RunAsync();
                    break;
                case "install":
                    var consumerInstallation = host.Services.GetRequiredService<ISqlBrokerQueueInstallation>();
                    await consumerInstallation.InstallAsync(CancellationToken.None);
                    break;
                default:
                    throw new NotSupportedException("please use parameter 'run' oder 'install'");
            }
        }

        internal static IServiceProvider ConfigureAppServices(IServiceProvider serviceProvider)
        {
            var brokerMessageHandlers = serviceProvider.GetRequiredService<ISqlBrokerMessageHandlerCollection>();
            brokerMessageHandlers.AddSqlBrokerMessageHandlers(serviceProvider);

            //var topicRegistrations = serviceProvider.GetRequiredService<ITopicRegistry>();
            //topicRegistrations.AddTopicRegistrations();

            return serviceProvider;
        }

        internal static IServiceCollection ConfigureHostServices(IServiceCollection serviceCollection, IConfigurationRoot config)
        {
            var loggerFactory = CreateLoggerFactory(config);
            var sqlBrokerQueueConfigurationSection = config.GetSection("Execution:SqlBrokerQueueConsumer");
            var azureTopicConfigurationSection = config.GetSection("Execution:AzureTopicProducer");
            var sqlBrokerToAzureAdapterSection = config.GetSection("Execution:SqlBrokerToAzureAdapter");

            return serviceCollection
                .AddLogging()
                .AddSingleton(x => loggerFactory)
                .AddSqlBrokerToAzureAdapterSetup(config)
                .AddSqlBrokerToAzureAdapter(config)
                .AddSqlBrokerQueueConsumer(sqlBrokerQueueConfigurationSection)
                //.AddAzureTopicProducer(azureTopicConfigurationSection)
                .AddRabbitMqTopicProducer()
                .AddAdapter(sqlBrokerToAzureAdapterSection)
                .AddTransformations();
        }

        private static ILoggerFactory CreateLoggerFactory(IConfigurationRoot config)
        {
            return LoggerFactory.Create(builder => builder.AddConsole().AddConfiguration(config.GetSection("Logging")));
        }

        private static IConfigurationBuilder ConfigureConfiguration(IConfigurationBuilder configurationBuilder)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = configurationBuilder
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
                .AddEnvironmentVariables();

            return builder;
        }
    }
}