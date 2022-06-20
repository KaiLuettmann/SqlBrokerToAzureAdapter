using SqlBrokerToAzureAdapter.Producers.AzureTopics.Models;

namespace SqlBrokerToAzureAdapter.Test.TestModelBuilders
{
    internal class EventBuilder : TestObjectBuilder<Event>
    {
        internal EventBuilder WithPayload(object payload)
        {
            WithConstructorArgumentFor("payload", payload);
            return this;
        }

        public EventBuilder WithEntityId(string entityId)
        {
            WithConstructorArgumentFor("entityId", entityId);
            return this;
        }
    }
}