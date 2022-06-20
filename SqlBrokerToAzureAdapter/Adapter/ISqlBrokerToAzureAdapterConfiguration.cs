using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues;

namespace SqlBrokerToAzureAdapter.Adapter
{
    /// <summary>
    /// Configuration for the execution of <see cref="ISqlBrokerMessageReceiver{T}" />.
    /// </summary>
    public interface ISqlBrokerToAzureAdapterConfiguration
    {
        /// <summary>
        /// Set this value to 'false' if you don't have any add-events you want to publish
        /// </summary>
        public bool ThrowIfNoAddEventTransformationIsPresent { get; }
        /// <summary>
        /// Set this value to 'false' if you don't have any edit-events you want to publish
        /// </summary>
        public bool ThrowIfNoEditEventTransformationIsPresent { get; }
        /// <summary>
        /// Set this value to 'false' if you don't have any remove-events you want to publish
        /// </summary>
        public bool ThrowIfNoRemoveEventTransformationIsPresent { get; }
    }
}