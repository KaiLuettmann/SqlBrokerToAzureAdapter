using System.Collections.Generic;

namespace SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues
{
    /// <summary>
    /// The configuration to setup one sql broker queue
    /// </summary>
    public interface ISqlBrokerQueueGenerationConfiguration
    {
        /// <summary>
        /// The name of the database of the subscribed table
        /// </summary>
        public string DatabaseName { get; }
        /// <summary>
        /// The name of the schema of the subscribed table
        /// </summary>
        public string SchemaName { get; }
        /// <summary>
        /// The name of the receiver queue
        /// </summary>
        public string ReceiverQueueName { get; }
        /// <summary>
        /// The name of the sender queue
        /// </summary>
        public string SenderQueueName { get; }
        /// <summary>
        /// The name of the subscribed table
        /// </summary>
        public SqlBrokerTableSubscriptionGenerationConfiguration[] TableSubscriptions { get; }
    }
}