using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues;
using SqlBrokerToAzureAdapter.Producers.AzureTopics;
using SqlBrokerToAzureAdapter.Setup;
using SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues;

namespace SqlBrokerToAzureAdapter
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var program = new Program();
            var serviceCollection = program.ConfigureServices();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            switch (args[0])
            {
                case "run":
                    var consumer = Configure(serviceProvider);
                    await consumer.RunAsync(CancellationToken.None);
                    break;
                case "install":
                    var consumerInstallation = serviceProvider.GetRequiredService<ISqlBrokerQueueInstallation>();
                    await consumerInstallation.InstallAsync(CancellationToken.None);
                    break;
                default:
                    throw new NotSupportedException("please use parameter 'run' oder 'install'");
            }
        }

        internal static ISqlBrokerQueueConsumer Configure(ServiceProvider serviceProvider)
        {
            var brokerMessageHandlers = serviceProvider.GetRequiredService<ISqlBrokerMessageHandlerCollection>();
            brokerMessageHandlers.AddSqlBrokerMessageHandlers(serviceProvider);

            var topicRegistrations = serviceProvider.GetRequiredService<ITopicRegistry>();
            topicRegistrations.AddTopicRegistrations();

            var consumer = serviceProvider.GetRequiredService<ISqlBrokerQueueConsumer>();
            return consumer;
        }

        internal IServiceCollection ConfigureServices()
        {
            var config = GetConfiguration();
            var loggerFactory = CreateLoggerFactory(config);

            return new ServiceCollection()
                .AddLogging()
                .AddSingleton(x => loggerFactory)
                .AddSqlBrokerToAzureAdapterSetup(config)
                .AddSqlBrokerToAzureAdapter(config)
                .AddTransformations();
        }

        private static ILoggerFactory CreateLoggerFactory(IConfigurationRoot config)
        {
            return LoggerFactory.Create(builder => builder.AddConsole().AddConfiguration(config.GetSection("Logging")));
        }

        private static IConfigurationRoot GetConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}