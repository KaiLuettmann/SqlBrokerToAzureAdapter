namespace SqlBrokerToAzureAdapter.Producers.AzureTopics
{
    /// <summary>
    /// Configuration for the execution of <see cref="IAzureTopicProducer" />.
    /// </summary>
    public interface IAzureTopicConfiguration
    {
        /// <summary>
        /// The connection string to connect to an Azure Service Bus <seealso href="https://docs.microsoft.com/de-de/azure/"/>>
        /// </summary>
        string ConnectionString { get; }
    }
}