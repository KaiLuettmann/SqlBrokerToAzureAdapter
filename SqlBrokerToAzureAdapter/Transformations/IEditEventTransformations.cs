using System.Collections.Generic;

namespace SqlBrokerToAzureAdapter.Transformations
{
    /// <summary>
    /// A collection of type <see cref="IEditEventTransformation{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of the message body.</typeparam>
    public interface IEditEventTransformations<T> : IEnumerable<IEditEventTransformation<T>>
    {

    }
}