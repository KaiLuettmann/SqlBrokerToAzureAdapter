using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using SqlBrokerToAzureAdapter.Adapter;
using SqlBrokerToAzureAdapter.Adapter.Models;
using SqlBrokerToAzureAdapter.Extensions;
using SqlBrokerToAzureAdapter.MessageContracts;
using SqlBrokerToAzureAdapter.Producers.Common.Models;

namespace SqlBrokerToAzureAdapter.Producers.MassTransit
{
    internal sealed class MasstransitTopicProducer : ITopicProducer
    {
        private readonly IBus _bus;
        private readonly Dictionary<Type, MethodInfo> _sendAsyncMethodForPayloadTypeRegistry;
        private readonly ILogger<MasstransitTopicProducer> _logger;

        private static readonly MethodInfo genericSendAsyncMethod = typeof(MasstransitTopicProducer)
                         .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                         .Where(m => m.Name == nameof(SendAsync))
                         .Select(m => new {
                                              Method = m,
                                              Params = m.GetParameters(),
                                              Args = m.GetGenericArguments()
                                          })
                         .Where(x => x.Params.Length == 2
                                     && x.Args.Length == 1)
                         .Select(x => x.Method)
                         .Single();

        public MasstransitTopicProducer(
            ILogger<MasstransitTopicProducer> logger,
            IBus bus)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _sendAsyncMethodForPayloadTypeRegistry = new Dictionary<Type, MethodInfo>();
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

            await SendAsync(metadata, events);
            _logger.LogInformation("Events published to topic");
        }

        private async Task SendAsync(Metadata metadata, Events events)
        {
            MethodInfo sendAsyncMethodForPayloadType = GetSendAsyncMethodForPayloadType(events.PayloadType);
            await sendAsyncMethodForPayloadType.InvokeAsync(this, new object []{metadata, events});
        }

        private MethodInfo GetSendAsyncMethodForPayloadType(Type payloadType)
        {
            if(!_sendAsyncMethodForPayloadTypeRegistry.TryGetValue(payloadType, out var sendAsyncMethodForPayloadType))
            {
                 sendAsyncMethodForPayloadType = genericSendAsyncMethod.MakeGenericMethod(payloadType);
                _sendAsyncMethodForPayloadTypeRegistry.Add(payloadType, sendAsyncMethodForPayloadType);
            }
           return sendAsyncMethodForPayloadType;
        }

        private async Task SendAsync<T>(Metadata metadata, Events events) where T : class
        {
            var tasks = new List<Task>();
            foreach (var @event in events)
            {
                tasks.Add(SendAsync<T>(metadata, @event));
            }
            await Task.WhenAll(tasks);
        }

        private Task SendAsync<T>(Metadata metadata, Adapter.Models.Event @event) where T : class
        {
            return _bus.Publish<T>(@event.Payload, context =>
            {
                context.CorrelationId = metadata.CorrelationId;
                context.Headers.Set("Label", @event.PayloadType.FullName);
                context.MessageId = CreateMessageId(metadata, @event);
                context.ContentType = new ContentType(MediaTypeNames.Application.Json);
                context.Headers.Set("Timestamp", metadata.Timestamp);
            });
        }

        private MessageId CreateMessageId(Metadata metadata, Adapter.Models.Event @event)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            return new MessageId(metadata.CorrelationId, @event.EntityId, @event.PayloadType);
        }
    }
}