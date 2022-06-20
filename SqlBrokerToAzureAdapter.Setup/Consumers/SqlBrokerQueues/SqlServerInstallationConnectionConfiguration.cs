namespace SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class SqlServerInstallationConnectionConfiguration : ISqlServerInstallationConnectionConfiguration
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ConnectionString { get; set; }
    }
}