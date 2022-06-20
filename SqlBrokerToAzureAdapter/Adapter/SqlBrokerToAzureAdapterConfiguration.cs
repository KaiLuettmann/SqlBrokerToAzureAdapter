namespace SqlBrokerToAzureAdapter.Adapter
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class SqlBrokerToAzureAdapterConfiguration : ISqlBrokerToAzureAdapterConfiguration
    {
        /// <summary>
        /// <inheritdoc/>
        /// Default value is 'true'
        /// </summary>
        public bool ThrowIfNoAddEventTransformationIsPresent { get; set; } = true;
        /// <summary>
        /// <inheritdoc/>
        /// Default value is 'true'
        /// </summary>
        public bool ThrowIfNoEditEventTransformationIsPresent { get; set; } = true;
        /// <summary>
        /// <inheritdoc/>
        /// Default value is 'true'
        /// </summary>
        public bool ThrowIfNoRemoveEventTransformationIsPresent { get; set; } = true;
    }
}