using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SqlBrokerToAzureAdapter.Adapter;
using SqlBrokerToAzureAdapter.Adapter.Models;
using SqlBrokerToAzureAdapter.MessageContracts;
using SqlBrokerToAzureAdapter.Producers.Common;
using SqlBrokerToAzureAdapter.Producers.Common.Exceptions;
using SqlBrokerToAzureAdapter.Producers.Common.Models;

namespace SqlBrokerToAzureAdapter.Producers.AzureTopics
{
    internal sealed class AzureTopicProducer : ITopicProducer
    {
        private readonly ITopicRegistry _topicRegistry;
        private readonly IAzureTopicClientFactory _azureTopicClientFactory;
        private readonly ILogger<AzureTopicProducer> _logger;

        public AzureTopicProducer(
            ILogger<AzureTopicProducer> logger,
            ITopicRegistry topicRegistry,
            IAzureTopicClientFactory azureTopicClientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _topicRegistry = topicRegistry ?? throw new ArgumentNullException(nameof(topicRegistry));
            _azureTopicClientFactory = azureTopicClientFactory ?? throw new ArgumentNullException(nameof(azureTopicClientFactory));
        }

        public async Task PublishAsync(Metadata metadata, Events events)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            if (!events.Any())
            {
                return;
            }

            var topicClient = _azureTopicClientFactory.Get(events.PayloadType);
            var messages = BuildMessages(metadata, events).ToList();
            await SendAsync(topicClient, messages);
            _logger.LogInformation($"Azure-Events published to Azure on topic '{topicClient.TopicName}'");
        }

        private async Task SendAsync(ISenderClient topicClient, IList<Message> messages)
        {
            try
            {
                await topicClient.SendAsync(messages);
            }
            //unless this issue is open there is not good option to calculate the size of a message: https://github.com/Azure/azure-service-bus-dotnet/issues/538
            //one option is described in https://weblogs.asp.net/sfeldman/asb-batching-brokered-messages
            catch (MessageSizeExceededException e)
            {
                if (messages.Count <= 1)
                {
                    throw;
                }
                _logger.LogDebug(e,$"Send {messages.Count}  messages to topic failed. Split messages and retry...");
                var nextMessageCount = messages.Count / 2;
                await SendAsync(topicClient, messages.Take(nextMessageCount).ToList());
                await SendAsync(topicClient, messages.Skip(nextMessageCount).ToList());
            }
        }

        private static void EnsureEntityIdIsNotEqualToCorrelationId(Metadata metadata, Event @event)
        {
            if (metadata.CorrelationId.ToString() == @event.EntityId)
            {
                throw new InvalidEntityIdException($"The entity id '{@event.EntityId}' should not be equal to the correlation id '{metadata.CorrelationId}'.");
            }
        }

        private IEnumerable<Message> BuildMessages(Metadata metadata, IEnumerable<Event> events)
        {
            return events.Select(@event => BuildMessage(metadata, @event));
        }

        private Message BuildMessage(Metadata metadata, Event @event)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            EnsureEntityIdIsNotEqualToCorrelationId(metadata, @event);

            var messageId = new MessageId(metadata.CorrelationId, @event.EntityId, @event.PayloadType);
            var messageBody = JsonConvert.SerializeObject(@event.Payload);
            _logger.LogTrace("Azure-Event serialized.");

            return new Message(Encoding.UTF8.GetBytes(messageBody))
            {
                CorrelationId = metadata.CorrelationId.ToString(),
                Label = @event.PayloadType.FullName,
                MessageId = messageId.ToString(),
                ContentType = MediaTypeNames.Application.Json,
                UserProperties = {{"Timestamp",metadata.Timestamp}}
            };
        }
    }
}