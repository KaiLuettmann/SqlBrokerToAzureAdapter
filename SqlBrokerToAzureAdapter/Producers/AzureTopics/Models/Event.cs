using System;

namespace SqlBrokerToAzureAdapter.Producers.AzureTopics.Models
{
    /// <summary>
    /// The Event which should be published
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Creates a new instance of an Event
        /// </summary>
        /// <param name="entityId">The identification for the entity</param>
        /// <param name="payload">The payload of the event</param>
        public Event(string entityId, object payload)
        {
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
            if (string.IsNullOrEmpty(entityId))
            {
                throw new ArgumentNullException(nameof(entityId));
            }
            EntityId = entityId;
        }

        /// <summary>
        /// The identification for the entity
        /// This value is used to ensure duplicate-check is possible.
        /// </summary>
        public string EntityId { get; }

        /// <summary>
        /// The payload of the event
        /// </summary>
        public object Payload { get;  }
    }
}