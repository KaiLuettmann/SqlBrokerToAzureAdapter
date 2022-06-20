using System.Threading.Tasks;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.Models;

namespace SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues
{
    /// <summary>
    /// The handler of a broker message
    /// </summary>
    public interface ISqlBrokerMessageHandler
    {
        /// <summary>
        /// The name of the broker message type defined with
        /// <see href="https://docs.microsoft.com/de-de/sql/t-sql/statements/create-message-type-transact-sql"/>
        /// which the handler handles
        /// </summary>
        string BrokerMessageTypeName { get; }
        /// <summary>
        /// Handles a message of type <see cref="BrokerMessage"/>
        /// </summary>
        /// <param name="message"></param>
        Task HandleAsync(BrokerMessage message);
    }
}