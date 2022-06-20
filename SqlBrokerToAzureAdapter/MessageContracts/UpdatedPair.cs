namespace SqlBrokerToAzureAdapter.MessageContracts
{
    /// <summary>
    /// A class to wrap the old and the new value for an update operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class UpdatedPair<T>
    {
        /// <summary>
        /// The old value.
        /// </summary>
        public T OldValue { get; set; }

        /// <summary>
        /// The new value.
        /// </summary>
        public T NewValue { get; set; }
    }
}