namespace SqlBrokerToAzureAdapter.Producers.AzureTopics
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public sealed class AzureTopicConfiguration : IAzureTopicConfiguration
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ConnectionString { get; set; }
    }
}