using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SqlBrokerToAzureAdapter.MessageContracts;
using SqlBrokerToAzureAdapter.Producers.AzureTopics;
using SqlBrokerToAzureAdapter.Producers.AzureTopics.Models;

namespace SqlBrokerToAzureAdapter.Testkit.Producers
{
    public class AzureTopicProducerFixture
    {
        public void SetupTopicPublishesToFile(IServiceCollection serviceCollection, string publishesJsonFilePath)
        {
            var jsonFileTopicProducer = new JsonFileTopicProducer(publishesJsonFilePath);
            jsonFileTopicProducer.EnsureFileExists();
            jsonFileTopicProducer.ClearFileContent();
            serviceCollection.AddScoped<IAzureTopicProducer>(x => jsonFileTopicProducer);
        }

        private class JsonFileTopicProducer : IAzureTopicProducer
        {
            private readonly string _filePath;

            internal JsonFileTopicProducer(string publishesJsonFilePath)
            {
                _filePath = publishesJsonFilePath;
            }

            public Task PublishAsync(Metadata metadata, IList<Event> events)
            {
                var json = JsonSerializer.Serialize(events);

                using var storage = File.AppendText(_filePath);
                storage.Write(json);

                return Task.CompletedTask;
            }

            public void EnsureFileExists()
            {
                if (!File.Exists(_filePath))
                {
                    File.Create(_filePath);
                }
            }

            public void ClearFileContent()
            {
                File.WriteAllText(_filePath,string.Empty);
            }
        }
    }
}