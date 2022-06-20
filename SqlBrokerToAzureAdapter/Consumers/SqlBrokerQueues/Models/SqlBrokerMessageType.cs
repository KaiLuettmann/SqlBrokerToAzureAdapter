namespace SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues
{
    /// <summary>
    /// The type of a broker message
    /// </summary>
    public enum SqlBrokerMessageType
    {
        /// <summary>
        /// Indicates that the message is created through an SQL insert operation.
        /// </summary>
        Inserted,

        /// <summary>
        /// Indicates that the message is created through an SQL update operation.
        /// </summary>
        Updated,

        /// <summary>
        /// Indicates that the message is created through an SQL delete operation.
        /// </summary>
        Deleted
    }
}