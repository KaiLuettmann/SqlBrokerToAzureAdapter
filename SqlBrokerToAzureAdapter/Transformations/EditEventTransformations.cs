using System.Collections;
using System.Collections.Generic;

namespace SqlBrokerToAzureAdapter.Transformations
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditEventTransformations<T> : IEditEventTransformations<T>
    {
        private readonly List<IEditEventTransformation<T>> _editEventTransformationsImplementation;

        /// <summary>
        /// Creates a new instance of <see cref="EditEventTransformations{T}"/>
        /// </summary>
        public EditEventTransformations()
        {
            _editEventTransformationsImplementation = new List<IEditEventTransformation<T>>();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator of type <see cref="IEditEventTransformation{T}"/>.</returns>
        public IEnumerator<IEditEventTransformation<T>> GetEnumerator()
        {
            return _editEventTransformationsImplementation.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _editEventTransformationsImplementation).GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The item which should be added.</param>
        public void Add(IEditEventTransformation<T> item)
        {
            _editEventTransformationsImplementation.Add(item);
        }

        /// <summary>
        /// Adds items to the collection.
        /// </summary>
        /// <param name="items">The items which should be added.</param>
        public void AddRange(IEnumerable<IEditEventTransformation<T>> items)
        {
            _editEventTransformationsImplementation.AddRange(items);
        }
    }
}