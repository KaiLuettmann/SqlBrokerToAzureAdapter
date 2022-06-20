using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.Models;
using SqlBrokerToAzureAdapter.MessageContracts;

namespace SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues
{
    /// <summary>
    /// <inheritdoc/>>
    /// </summary>
    /// <typeparam name="TDatabaseContract">The contract of the message body</typeparam>
    internal sealed class SqlBrokerMessageHandler<TDatabaseContract> : ISqlBrokerMessageHandler
    {
        private readonly ILogger<SqlBrokerMessageHandler<TDatabaseContract>> _logger;
        private readonly IMessageBodyDeserializer<TDatabaseContract> _messageBodyDeserializer;
        private readonly ISqlBrokerMessageReceiver<TDatabaseContract> _receiver;
        private readonly SqlBrokerMessageType _sqlBrokerMessageType;

        internal SqlBrokerMessageHandler(ILogger<SqlBrokerMessageHandler<TDatabaseContract>> logger,
            ISqlBrokerMessageReceiver<TDatabaseContract> receiver,
            IMessageBodyDeserializer<TDatabaseContract> messageBodyDeserializer,
            string brokerMessageTypeName,
            SqlBrokerMessageType sqlBrokerMessageType)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            BrokerMessageTypeName = brokerMessageTypeName ?? throw new ArgumentNullException(nameof(brokerMessageTypeName));
            _sqlBrokerMessageType = sqlBrokerMessageType;
            _messageBodyDeserializer = messageBodyDeserializer  ?? throw new ArgumentNullException(nameof(messageBodyDeserializer));
            _receiver = receiver  ?? throw new ArgumentNullException(nameof(receiver));
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string BrokerMessageTypeName { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task HandleAsync(BrokerMessage message)
        {
            _logger.LogInformation($"Start handling of broker message with id '{message.ConversationHandle}'.");
            switch (_sqlBrokerMessageType)
            {
                case SqlBrokerMessageType.Inserted:
                    _logger.LogInformation("Insert detected.");
                    await HandleInsertAsync(message);
                    break;
                case SqlBrokerMessageType.Updated:
                    _logger.LogInformation("Update detected.");
                    await HandleUpdateAsync(message);
                    break;
                case SqlBrokerMessageType.Deleted:
                    _logger.LogInformation("Delete detected.");
                    await HandleDeleteAsync(message);
                    break;
                default:
                    throw new NotImplementedException(_sqlBrokerMessageType.ToString());
            }

            _logger.LogInformation($"Handling of broker message with id '{message.ConversationHandle}' finished.");
        }

        private async Task HandleInsertAsync(BrokerMessage message)
        {
            var inserted = _messageBodyDeserializer.ToInsertContract(message.MessageBody);
            await _receiver.ReceiveInsertedAsync(new Metadata(message.ConversationHandle, message.MessageEnqueueTime), inserted);
        }

        private async Task HandleUpdateAsync(BrokerMessage message)
        {
            var updated = _messageBodyDeserializer.ToUpdateContract(message.MessageBody);
            await _receiver.ReceiveUpdatedAsync(new Metadata(message.ConversationHandle, message.MessageEnqueueTime), updated);
        }

        private async Task HandleDeleteAsync(BrokerMessage message)
        {
            var deleted = _messageBodyDeserializer.ToDeletedContract(message.MessageBody);
            await _receiver.ReceiveDeletedAsync(new Metadata(message.ConversationHandle, message.MessageEnqueueTime), deleted);
        }
    }
}