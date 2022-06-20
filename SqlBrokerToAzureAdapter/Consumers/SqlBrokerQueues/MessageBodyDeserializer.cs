using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SqlBrokerToAzureAdapter.MessageContracts;

namespace SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues
{
    internal sealed class MessageBodyDeserializer<TDatabaseContract> : IMessageBodyDeserializer<TDatabaseContract>
    {
        private readonly JsonSerializerOptions _options;
        private readonly ILogger<MessageBodyDeserializer<TDatabaseContract>> _logger;

        internal MessageBodyDeserializer(ILogger<MessageBodyDeserializer<TDatabaseContract>> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };
        }

        public IEnumerable<TDatabaseContract> ToInsertContract(string messageJsonMessageBody)
        {
            _logger.LogDebug($"Deserializing message body {messageJsonMessageBody}.");
            return JsonSerializer.Deserialize<IEnumerable<TDatabaseContract>>(messageJsonMessageBody, _options);
        }

        public IEnumerable<UpdatedPair<TDatabaseContract>> ToUpdateContract(string messageJsonMessageBody)
        {
            _logger.LogDebug($"Deserializing message body {messageJsonMessageBody}.");
            return JsonSerializer.Deserialize<IEnumerable<UpdatedPair<TDatabaseContract>>>(messageJsonMessageBody, _options);
        }

        public IEnumerable<TDatabaseContract> ToDeletedContract(string messageJsonMessageBody)
        {
            _logger.LogDebug($"Deserializing message body {messageJsonMessageBody}.");
            return JsonSerializer.Deserialize<IEnumerable<TDatabaseContract>>(messageJsonMessageBody, _options);
        }
    }
}