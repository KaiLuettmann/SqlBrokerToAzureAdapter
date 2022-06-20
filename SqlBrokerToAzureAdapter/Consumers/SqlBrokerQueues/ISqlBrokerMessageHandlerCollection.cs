using System.Collections.Generic;

namespace SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues
{
    /// <summary>
    /// A collection for all registered broker message handlers
    /// </summary>
    public interface ISqlBrokerMessageHandlerCollection : IEnumerable<ISqlBrokerMessageHandler>
    {
        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The item which should be added.</param>
        public void Add(ISqlBrokerMessageHandler item);
    }
}