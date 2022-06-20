using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.Models;

namespace SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues
{
    internal sealed class SqlBrokerQueueConsumer : ISqlBrokerQueueConsumer
    {
        private readonly Dictionary<string, ISqlBrokerMessageHandler> _brokerMessageHandlers;
        private readonly ILogger<SqlBrokerQueueConsumer> _logger;
        private readonly ISqlBrokerQueueConfiguration _sqlBrokerQueueConfiguration;
        private readonly ISqlBrokerQueueRepository _repository;

        public SqlBrokerQueueConsumer(
            ILogger<SqlBrokerQueueConsumer> logger,
            ISqlBrokerQueueConfiguration sqlBrokerQueueConfiguration,
            ISqlBrokerMessageHandlerCollection brokerMessageHandlers,
            ISqlBrokerQueueRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sqlBrokerQueueConfiguration = sqlBrokerQueueConfiguration ?? throw new ArgumentNullException(nameof(sqlBrokerQueueConfiguration));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            if (brokerMessageHandlers == null)
            {
                throw new ArgumentNullException(nameof(brokerMessageHandlers));
            }
            _brokerMessageHandlers = brokerMessageHandlers.ToDictionary(key => key.BrokerMessageTypeName);
        }

        public async ValueTask DisposeAsync()
        {
            if (_repository == null)
            {
                return;
            }
            await _repository.DisposeAsync();
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Start fetching messages from [{_sqlBrokerQueueConfiguration.DatabaseName}].[{_sqlBrokerQueueConfiguration.SchemaName}].[{_sqlBrokerQueueConfiguration.ReceiverQueueName}]");
            while (!cancellationToken.IsCancellationRequested)
            {
                await _repository.OpenSqlConnectionAsync();
                try
                {
                    await _repository.BeginTransactionAsync();
                    var message = await _repository.ReceiveNextMessageAsync();
                    if (message == null)
                    {
                        await _repository.RollbackTransactionAsync();
                        if (_sqlBrokerQueueConfiguration.SkipCurrentBrokerMessageOnException)
                        {
                            await _repository.ResetSkippedBrokerMessagesAsync();
                        }

                        continue;
                    }

                    await HandleMessageAsync(message);
                    //await _repository.EndConversation(message.ConversationHandle);
                    await _repository.CommitTransactionAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception occured... rollback transaction");
                    await _repository.RollbackTransactionAsync();
                    if (_sqlBrokerQueueConfiguration.SkipCurrentBrokerMessageOnException)
                    {
                        await _repository.SkipCurrentBrokerMessageAsync();
                        _logger.LogInformation("Erroneous broker message is skipped at first.");
                    }
                }
                finally
                {
                    await _repository.CloseSqlConnectionAsync();
                }
            }
        }

        private async Task HandleMessageAsync(BrokerMessage message)
        {
            if (!_brokerMessageHandlers.TryGetValue(message.MessageTypeName, out var brokerMessageHandler))
            {
                throw new InvalidOperationException($"The message-type '{message.MessageTypeName}' has no registered message-handler.");
            }

            await brokerMessageHandler.HandleAsync(message);
        }
    }
}