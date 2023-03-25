using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SqlBrokerToAzureAdapter.Producers.Common.Exceptions;

namespace SqlBrokerToAzureAdapter.Adapter.Models
{
    /// <summary>
    /// A collection of events which should be published
    /// </summary>
    public sealed class Events : IEnumerable<Event>
    {
        /// <summary>
        /// Creates a new instance of Events
        /// </summary>
        /// <param name="events"></param>
        public Events(IEnumerable<Event> events)
        {
            _events = events;
            EnsurePayloadTypeIsUnique();
        }

        private readonly IEnumerable<Event> _events;

        internal Type PayloadType => _events.First().Payload.GetType();

        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Event> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_events).GetEnumerator();
        }

        private void EnsurePayloadTypeIsUnique()
        {
            var payloadTypes = _events.Select(x => x.Payload.GetType()).Distinct();

            if ( payloadTypes.Count() > 1)
            {
                throw new UnexpectedPayloadTypeException("Ensure all payloads of the delivered events have the same type of payload.");
            }
        }
    }
}