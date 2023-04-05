using SqlBrokerToAzureAdapter.Adapter.Models;

namespace SqlBrokerToAzureAdapter.Test.TestModelBuilders
{
    internal class EventBuilder : TestObjectBuilder<Event>
    {
        internal EventBuilder WithPayload(object payload)
        {
            WithConstructorArgumentFor(nameof(payload), payload);
            return this;
        }

        public EventBuilder WithEntityId(string entityId)
        {
            WithConstructorArgumentFor(nameof(entityId), entityId);
            return this;
        }
    }
}