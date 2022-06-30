using SqlBrokerToAzureAdapter.Adapter.Models;

namespace SqlBrokerToAzureAdapter.Transformations
{
    /// <summary>
    /// A transformation for an event based on an remove-event
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRemoveEventTransformation<in T>
    {
        /// <summary>
        /// Transforms to an event.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The event.</returns>
        Event Transform(T value);
    }
}