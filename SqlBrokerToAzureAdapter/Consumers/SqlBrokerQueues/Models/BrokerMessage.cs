using System;

namespace SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.Models
{
    /// <summary>
    /// The representation of a SQL Server Service Broker message
    /// </summary>
    public sealed class BrokerMessage
    {
        /// <summary>
        /// Creates an new instance of a broker message.
        /// </summary>
        /// <param name="conversationHandle"><see cref="ConversationHandle"/></param>
        /// <param name="messageEnqueueTime"><see cref="MessageEnqueueTime"/></param>
        /// <param name="messageTypeName"><see cref="MessageTypeName"/></param>
        /// <param name="messageBody"><see cref="MessageBody"/></param>
        public BrokerMessage(Guid conversationHandle, DateTime messageEnqueueTime, string messageTypeName, string messageBody)
        {
            if (conversationHandle == default)
            {
                throw new NotSupportedException($"A {nameof(conversationHandle)} with default value '{conversationHandle}' is not supported.");
            }
            if (messageEnqueueTime == default)
            {
                throw new NotSupportedException($"A {nameof(messageEnqueueTime)} with default value '{messageEnqueueTime}' is not supported.");
            }
            ConversationHandle = conversationHandle;
            MessageEnqueueTime = messageEnqueueTime;
            MessageTypeName = messageTypeName ?? throw new ArgumentNullException(nameof(messageTypeName));
            MessageBody = messageBody ?? throw new ArgumentNullException(nameof(messageBody));
        }
        /// <summary>
        /// The name of the broker message type defined with
        /// <see href="https://docs.microsoft.com/de-de/sql/t-sql/statements/create-message-type-transact-sql"/>
        /// </summary>
        public string MessageTypeName { get; }
        /// <summary>
        /// The message body in the format of JSON
        /// </summary>
        public string MessageBody { get; }
        /// <summary>
        /// The conversation handle delivered with the message of the SQL Server Service Broker
        /// </summary>
        public Guid ConversationHandle { get; }
        /// <summary>
        /// The enqueue time of the message
        /// </summary>
        public DateTime MessageEnqueueTime { get; }

        /// <summary>
        /// The display of the message without payload
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                $"ConversationHandle:'{ConversationHandle}', MessageEnqueueTime: '{MessageEnqueueTime}', MessageTypeName: '{MessageTypeName}'";
        }
    }
}