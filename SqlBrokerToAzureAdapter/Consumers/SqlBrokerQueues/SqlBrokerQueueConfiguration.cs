namespace SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public sealed class SqlBrokerQueueConfiguration : ISqlBrokerQueueConfiguration
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string DatabaseName { get; set; }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string SchemaName { get; set; }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ReceiverQueueName { get; set; }
        /// <summary>
        /// <inheritdoc/>
        /// Default value is 10000 milliseconds
        /// </summary>
        public int LongPollingTimeout { get; set; } = 10000;

        /// <summary>
        /// <inheritdoc/>
        /// Default value is true
        /// </summary>
        public bool SkipCurrentBrokerMessageOnException { get; set; } = true;
    }
}