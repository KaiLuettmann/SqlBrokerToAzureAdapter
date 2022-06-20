using System.Collections;
using System.Collections.Generic;

namespace SqlBrokerToAzureAdapter.Transformations
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AddEventTransformations<T> : IAddEventTransformations<T>
    {
        private readonly List<IAddEventTransformation<T>> _editAddTransformationsImplementation;

        /// <summary>
        /// Creates a new instance of <see cref="AddEventTransformations{T}"/>
        /// </summary>
        public AddEventTransformations()
        {
            _editAddTransformationsImplementation = new List<IAddEventTransformation<T>>();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator of type <see cref="IAddEventTransformation{T}"/>.</returns>
        public IEnumerator<IAddEventTransformation<T>> GetEnumerator()
        {
            return _editAddTransformationsImplementation.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _editAddTransformationsImplementation).GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The item which should be added.</param>
        public void Add(IAddEventTransformation<T> item)
        {
            _editAddTransformationsImplementation.Add(item);
        }

        /// <summary>
        /// Adds items to the collection.
        /// </summary>
        /// <param name="items">The items which should be added.</param>
        public void AddRange(IEnumerable<IAddEventTransformation<T>> items)
        {
            _editAddTransformationsImplementation.AddRange(items);
        }
    }
}