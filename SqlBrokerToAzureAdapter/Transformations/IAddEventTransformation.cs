using SqlBrokerToAzureAdapter.MessageContracts;
using SqlBrokerToAzureAdapter.Producers;
using SqlBrokerToAzureAdapter.Producers.AzureTopics.Models;

namespace SqlBrokerToAzureAdapter.Transformations
{
    /// <summary>
    /// A transformation for an event based on an add-event
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAddEventTransformation<in T>
    {
        /// <summary>
        /// Transforms an event.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The event.</returns>
        Event Transform(T value);
    }
}