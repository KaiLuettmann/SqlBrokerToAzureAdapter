using System.Collections;
using System.Collections.Generic;

namespace SqlBrokerToAzureAdapter.Transformations
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RemoveEventTransformations<T> : IRemoveEventTransformations<T>
    {
        private readonly List<IRemoveEventTransformation<T>> _removeEventTransformationsImplementation;

        /// <summary>
        /// Creates a new instance of <see cref="RemoveEventTransformations{T}"/>
        /// </summary>
        public RemoveEventTransformations()
        {
            _removeEventTransformationsImplementation = new List<IRemoveEventTransformation<T>>();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator of type <see cref="IRemoveEventTransformation{T}"/>.</returns>
        public IEnumerator<IRemoveEventTransformation<T>> GetEnumerator()
        {
            return _removeEventTransformationsImplementation.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _removeEventTransformationsImplementation).GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The item which should be a added</param>
        public void Add(IRemoveEventTransformation<T> item)
        {
            _removeEventTransformationsImplementation.Add(item);
        }

        /// <summary>
        /// Adds an items to the collection.
        /// </summary>
        /// <param name="items">The items which should be a added</param>
        public void AddRange(IEnumerable<IRemoveEventTransformation<T>> items)
        {
            _removeEventTransformationsImplementation.AddRange(items);
        }
    }
}