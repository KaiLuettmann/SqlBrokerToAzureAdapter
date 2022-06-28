using System.Collections.Generic;
using SqlBrokerToAzureAdapter.MessageContracts;
using SqlBrokerToAzureAdapter.Producers;
using SqlBrokerToAzureAdapter.Producers.Common.Models;

namespace SqlBrokerToAzureAdapter.Transformations
{
    /// <summary>
    /// A transformation for an event based on an edit-event
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEditEventTransformation<T>
    {
        /// <summary>
        /// Checks if the transformation is responsible for the occured differences.
        /// </summary>
        /// <param name="differences"></param>
        /// <returns>The decision if the transformation is responsible for the differences.</returns>
        public bool IsResponsibleFor(IEnumerable<Difference> differences);

        /// <summary>
        /// Transforms a an event.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The event.</returns>
        Event Transform(UpdatedPair<T> value);
    }
}