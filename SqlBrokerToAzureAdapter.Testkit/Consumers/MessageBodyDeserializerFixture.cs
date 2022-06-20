using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues;
using SqlBrokerToAzureAdapter.MessageContracts;

namespace SqlBrokerToAzureAdapter.Testkit.Consumers
{
    public class MessageBodyDeserializerFixture<TDatabaseContract>
    {
        private readonly MessageBodyDeserializer<TDatabaseContract> _messageBodySerializer;

        public MessageBodyDeserializerFixture()
        {
            var loggerFactory = new LoggerFactory();
            var logger = loggerFactory.CreateLogger<MessageBodyDeserializer<TDatabaseContract>>();
            _messageBodySerializer = new MessageBodyDeserializer<TDatabaseContract>(logger);
        }
        public IEnumerable<TDatabaseContract> Deserialize(string messageJsonMessageBody)
        {
            return _messageBodySerializer.ToInsertContract(messageJsonMessageBody);
        }
    }
}