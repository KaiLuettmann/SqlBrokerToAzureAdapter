using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SqlBrokerToAzureAdapter.Extensions;
using SqlBrokerToAzureAdapter.MessageContracts;
using SqlBrokerToAzureAdapter.Producers.Common.Exceptions;
using SqlBrokerToAzureAdapter.Producers.Common.Models;

namespace SqlBrokerToAzureAdapter.Producers.AzureTopics
{
    internal sealed class AzureTopicProducer : IAzureTopicProducer
    {
        private readonly ITopicRegistry _topicRegistry;
        private readonly ITopicClientFactory _topicClientFactory;
        private readonly ILogger<AzureTopicProducer> _logger;

        public AzureTopicProducer(
            ILogger<AzureTopicProducer> logger,
            ITopicRegistry topicRegistry,
            ITopicClientFactory topicClientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _topicRegistry = topicRegistry ?? throw new ArgumentNullException(nameof(topicRegistry));
            _topicClientFactory = topicClientFactory ?? throw new ArgumentNullException(nameof(topicClientFactory));
        }

        public async Task PublishAsync(Metadata metadata, IList<Event> events)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            if (!events.Any())
            {
                return;
            }
            var payloadType = events.First().Payload.GetType();
            var topicClient = _topicClientFactory.Get(payloadType);
            var messages = BuildMessages(metadata, payloadType, events).ToList();
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

        private static void EnsurePayloadHasPayloadType(Type expectedPayloadType, Event @event)
        {
            var actualPayloadType = @event.Payload.GetType();
            if ( actualPayloadType != expectedPayloadType)
            {
                throw new UnexpectedPayloadTypeException($"The payload of the event has unexpected type of payload. Expected payload is '{expectedPayloadType.FullName}' but was '{actualPayloadType}'. Ensure all payloads of the delivered events have the same type of payload.");
            }
        }

        private IEnumerable<Message> BuildMessages(Metadata metadata, Type payloadType, IEnumerable<Event> events)
        {
            return events.Select(@event => BuildMessage(metadata, payloadType, @event));
        }

        private Message BuildMessage(Metadata metadata, Type payloadType, Event @event)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            EnsureEntityIdIsNotEqualToCorrelationId(metadata, @event);
            EnsurePayloadHasPayloadType(payloadType, @event);
            var messageBody = JsonConvert.SerializeObject(@event.Payload);
            _logger.LogTrace("Azure-Event serialized.");
            return new Message(Encoding.UTF8.GetBytes(messageBody))
            {
                CorrelationId = metadata.CorrelationId.ToString(),
                Label = _topicRegistry[payloadType],
                MessageId = GetMessageId(metadata, @event),
                ContentType = payloadType.FullName,
                UserProperties = {{"Timestamp",metadata.Timestamp}}
            };
        }

        private static string GetMessageId(Metadata metadata, Event @event)
        {
            var traversedFullname = TraverseAtDots(@event.Payload.GetType().FullName);
            return $"{metadata.CorrelationId}-{@event.EntityId}-{traversedFullname}".Truncate(128);
        }

        private static string TraverseAtDots(string fullName)
        {
            return string.Join(".",fullName.Split(".").Reverse());
        }
    }
}