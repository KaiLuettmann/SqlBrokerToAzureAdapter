namespace SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues
{
    /// <summary>
    /// Configuration for the execution of <see cref="ISqlBrokerQueueConfiguration" />.
    /// </summary>
    public interface ISqlBrokerQueueConfiguration
    {
        /// <summary>
        /// The connection string to connect to an SQL-Server Service Broker <seealso href="https://docs.microsoft.com/de-de/sql/database-engine/configure-windows/sql-server-service-broker"/>>
        /// </summary>
        string ConnectionString { get; }
        /// <summary>
        /// The name of the database in which the queue is stored to which you want to connect.
        /// </summary>
        string DatabaseName { get; }
        /// <summary>
        /// The name of the schema in which the queue is stored to which you want to connect.
        /// </summary>
        string SchemaName { get; }
        /// <summary>
        /// The name of the queue to which receives the sql broker messages
        /// </summary>
        string ReceiverQueueName { get; }
        /// <summary>
        /// The timeout of the long polling in milliseconds
        /// </summary>
        int LongPollingTimeout { get; }

        /// <summary>
        /// Enables skipping of failed broker messages. The broker message will be retried after the queue is empty.
        /// </summary>
        bool SkipCurrentBrokerMessageOnException { get; }
    }
}