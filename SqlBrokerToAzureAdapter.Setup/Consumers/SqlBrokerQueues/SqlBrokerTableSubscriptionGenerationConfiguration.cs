namespace SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class SqlBrokerTableSubscriptionGenerationConfiguration : ISqlBrokerTableSubscriptionGenerationConfiguration
    {
        private string _insertBrokerMessageTypeName;
        private string _deleteBrokerMessageTypeName;
        private string _updateBrokerMessageTypeName;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string PrimaryKeyColumnName { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// Default value is 'TableName.Inserted'
        /// </summary>
        public string InsertBrokerMessageTypeName {
            get => _insertBrokerMessageTypeName ?? $"{TableName}.Inserted";
            set => _insertBrokerMessageTypeName = value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Default value is 'TableName.Updated'
        /// </summary>
        public string UpdateBrokerMessageTypeName {
            get => _updateBrokerMessageTypeName ?? $"{TableName}.Updated";
            set => _updateBrokerMessageTypeName = value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Default value is 'TableName.Deleted'
        /// </summary>
        public string DeleteBrokerMessageTypeName {
            get => _deleteBrokerMessageTypeName ?? $"{TableName}.Deleted";
            set => _deleteBrokerMessageTypeName = value;
        }
    }
}