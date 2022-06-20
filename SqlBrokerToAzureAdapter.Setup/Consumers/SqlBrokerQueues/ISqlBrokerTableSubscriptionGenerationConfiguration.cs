namespace SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues
{
    /// <summary>
    /// The configuration to setup one subscription to one table
    /// </summary>
    public interface ISqlBrokerTableSubscriptionGenerationConfiguration
    {
        /// <summary>
        /// The name of the subscribed table
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// The name of the primary key column owned by the subscribed table
        /// </summary>
        public string PrimaryKeyColumnName { get; }

        /// <summary>
        /// The broker message type name for an insert operation
        /// </summary>
        public string InsertBrokerMessageTypeName { get; }

        /// <summary>
        /// The broker message type name for a update operation
        /// </summary>
        public string UpdateBrokerMessageTypeName { get; }

        /// <summary>
        /// The broker message type name for a delete operation
        /// </summary>
        public string DeleteBrokerMessageTypeName { get; }
    }
}