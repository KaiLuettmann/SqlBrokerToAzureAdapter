using System.Collections.Generic;

namespace SqlBrokerToAzureAdapter.Transformations
{
    /// <summary>
    /// A collection of type <see cref="IRemoveEventTransformation{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of the message body.</typeparam>
    public interface IRemoveEventTransformations<in T> : IEnumerable<IRemoveEventTransformation<T>>
    {
    }
}