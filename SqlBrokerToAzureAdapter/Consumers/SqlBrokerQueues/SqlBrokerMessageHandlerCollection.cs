using System.Collections;
using System.Collections.Generic;

namespace SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public sealed class SqlBrokerMessageHandlerCollection : ISqlBrokerMessageHandlerCollection
    {
        private readonly IList<ISqlBrokerMessageHandler> _sqlBrokerMessageHandlerCollectionImplementation;

        internal SqlBrokerMessageHandlerCollection()
        {
            _sqlBrokerMessageHandlerCollectionImplementation = new List<ISqlBrokerMessageHandler>();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator of type <see cref="ISqlBrokerMessageHandler"/>.</returns>
        public IEnumerator<ISqlBrokerMessageHandler> GetEnumerator()
        {
            return _sqlBrokerMessageHandlerCollectionImplementation.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _sqlBrokerMessageHandlerCollectionImplementation).GetEnumerator();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        public void Add(ISqlBrokerMessageHandler item)
        {
            _sqlBrokerMessageHandlerCollectionImplementation.Add(item);
        }
    }
}