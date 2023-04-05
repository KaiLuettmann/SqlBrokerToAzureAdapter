using System.Threading.Tasks;
using SqlBrokerToAzureAdapter.Adapter.Models;
using SqlBrokerToAzureAdapter.MessageContracts;

namespace SqlBrokerToAzureAdapter.Adapter
{
    /// <summary>
    /// Produces messages to a topic of Azure Service Bus <seealso href="https://docs.microsoft.com/de-de/azure/"/>>
    /// </summary>
    public interface ITopicProducer
    {
        /// <summary>
        /// Publishes messages based on the given contract to one topic of Azure Service Bus
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="events">The events which should be published.</param>
        Task PublishAsync(Metadata metadata, Events events);
    }
}