using System.Collections.Generic;

namespace SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public sealed class SqlBrokerQueueGenerationConfiguration : ISqlBrokerQueueGenerationConfiguration
    {
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
        /// </summary>
        public string SenderQueueName { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public SqlBrokerTableSubscriptionGenerationConfiguration[] TableSubscriptions { get; set; }
    }
}