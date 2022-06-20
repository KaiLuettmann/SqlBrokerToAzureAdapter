namespace SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues
{
    /// <summary>
    /// The connection configuration for the installation of the sql broker queue ('sa' privileges needed)
    /// </summary>
    public interface ISqlServerInstallationConnectionConfiguration
    {
        /// <summary>
        /// The connection string for the sql server ('sa' privileges needed)
        /// </summary>
        public string ConnectionString { get; }
    }
}