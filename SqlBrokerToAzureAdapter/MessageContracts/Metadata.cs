using System;

namespace SqlBrokerToAzureAdapter.MessageContracts
{
    /// <summary>
    /// The metadata of an broker message
    /// </summary>
    public sealed class Metadata
    {
        internal Metadata(Guid correlationId, in DateTime timestamp)
        {
            CorrelationId = correlationId;
            Timestamp = timestamp;
        }

        /// <summary>
        /// A unique identifier for the database operation which created the broker message.
        /// </summary>
        public Guid CorrelationId { get; }

        /// <summary>
        /// The timestamp of creation of the broker message.
        /// </summary>
        public DateTime Timestamp { get; }
    }
}