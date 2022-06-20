using System;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.Models;

namespace SqlBrokerToAzureAdapter.Test.TestModelBuilders
{
    internal class BrokerMessageBuilder : TestObjectBuilder<BrokerMessage>
    {
        internal BrokerMessageBuilder WithConversationHandle(Guid conversationHandle)
        {
            WithConstructorArgumentFor("conversationHandle", conversationHandle);
            return this;
        }

        public BrokerMessageBuilder WithMessageEnqueueTime(DateTime messageEnqueueTime)
        {
            WithConstructorArgumentFor("messageEnqueueTime", messageEnqueueTime);
            return this;
        }

        public BrokerMessageBuilder WithMessageTypeName(string messageTypeName)
        {
            WithConstructorArgumentFor("messageTypeName", messageTypeName);
            return this;
        }

        public BrokerMessageBuilder WithMessageBody(string messageBody)
        {
            WithConstructorArgumentFor("messageBody", messageBody);
            return this;
        }
    }
}