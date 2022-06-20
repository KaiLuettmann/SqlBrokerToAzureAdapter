using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlBrokerToAzureAdapter.Adapter;

namespace SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues
{
    /// <summary>
    /// A factory to create <see cref="ISqlBrokerMessageHandler"/>
    /// </summary>
    /// <typeparam name="TDatabaseContract"></typeparam>
    public sealed class SqlBrokerMessageHandlerFactory<TDatabaseContract>
    {
        private readonly ISqlBrokerMessageReceiver<TDatabaseContract> _adapter;
        private readonly IMessageBodyDeserializer<TDatabaseContract> _messageBodySerializer;
        private readonly ILogger<SqlBrokerMessageHandler<TDatabaseContract>> _logger;

        /// <summary>
        /// Creates a new instance of a <see cref="SqlBrokerMessageHandlerFactory{T}"/>
        /// </summary>
        /// <param name="provider">The service provider.</param>
        /// <exception cref="InvalidOperationException">Throws if no implementation of <see cref="ILoggerFactory"/>
        /// or <see cref="ISqlBrokerMessageReceiver{T}"/> is registered in the service provider</exception>
        public SqlBrokerMessageHandlerFactory(IServiceProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            if (loggerFactory == null)
            {
                throw new InvalidOperationException($"Could not get '{nameof(ILoggerFactory)}'. Please register a the '{nameof(ILoggerFactory)}'.");
            }
            _adapter = provider.GetRequiredService<ISqlBrokerMessageReceiver<TDatabaseContract>>();
            if (_adapter == null)
            {
                throw new InvalidOperationException($"Could not get '{nameof(ISqlBrokerMessageReceiver<TDatabaseContract>)}' for type '{typeof(TDatabaseContract).Name}'. Please add 'collection.AddSingleton<{nameof(ISqlBrokerMessageReceiver<TDatabaseContract>)}<{typeof(TDatabaseContract).Name}>, {nameof(SqlBrokerToAzureAdapter<TDatabaseContract>)}<{typeof(TDatabaseContract).Name}>>();' to your registration.");
            }
            _messageBodySerializer = new MessageBodyDeserializer<TDatabaseContract>(loggerFactory.CreateLogger<MessageBodyDeserializer<TDatabaseContract>>());
            _logger = loggerFactory.CreateLogger<SqlBrokerMessageHandler<TDatabaseContract>>();
        }

        /// <summary>
        /// Creates an instance of a message handler. See <see cref="ISqlBrokerMessageHandler"/>
        /// </summary>
        /// <param name="brokerMessageTypeName">The name of the broker message type defined with
        /// <see href="https://docs.microsoft.com/de-de/sql/t-sql/statements/create-message-type-transact-sql"/>
        /// which the handler handles</param>
        /// <param name="sqlBrokerMessageType">The type which describes how the brokerMessageTypeName is handled.</param>
        /// <returns>The message handler.</returns>
        public ISqlBrokerMessageHandler Get(string brokerMessageTypeName, SqlBrokerMessageType sqlBrokerMessageType)
        {
            return new SqlBrokerMessageHandler<TDatabaseContract>(_logger, _adapter, _messageBodySerializer,
                brokerMessageTypeName, sqlBrokerMessageType);
        }
    }
}