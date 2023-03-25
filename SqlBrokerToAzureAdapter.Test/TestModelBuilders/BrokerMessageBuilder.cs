using System;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.Models;

namespace SqlBrokerToAzureAdapter.Test.TestModelBuilders
{
    internal class BrokerMessageBuilder : TestObjectBuilder<BrokerMessage>
    {
        internal BrokerMessageBuilder WithConversationHandle(Guid conversationHandle)
        {
            WithConstructorArgumentFor(nameof(conversationHandle), conversationHandle);
            return this;
        }

        public BrokerMessageBuilder WithMessageEnqueueTime(DateTime messageEnqueueTime)
        {
            WithConstructorArgumentFor(nameof(messageEnqueueTime), messageEnqueueTime);
            return this;
        }

        public BrokerMessageBuilder WithMessageTypeName(string messageTypeName)
        {
            WithConstructorArgumentFor(nameof(messageTypeName), messageTypeName);
            return this;
        }

        public BrokerMessageBuilder WithMessageBody(string messageBody)
        {
            WithConstructorArgumentFor(nameof(messageBody), messageBody);
            return this;
        }
    }
}