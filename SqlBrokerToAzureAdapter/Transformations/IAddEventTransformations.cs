using System.Collections.Generic;

namespace SqlBrokerToAzureAdapter.Transformations
{
    /// <summary>
    /// A collection of type <see cref="IAddEventTransformation{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of the message body.</typeparam>
    public interface IAddEventTransformations<in T> : IEnumerable<IAddEventTransformation<T>>
    {

    }
}