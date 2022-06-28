using System.Collections.Generic;
using System.Threading.Tasks;
using SqlBrokerToAzureAdapter.MessageContracts;
using SqlBrokerToAzureAdapter.Producers.Common.Models;

namespace SqlBrokerToAzureAdapter.Producers.AzureTopics
{
    /// <summary>
    /// Produces messages to a topic of Azure Service Bus <seealso href="https://docs.microsoft.com/de-de/azure/"/>>
    /// </summary>
    public interface IAzureTopicProducer
    {
        /// <summary>
        /// Publishes messages based on the given contract to one topic of Azure Service Bus
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="events">The events which should be published.</param>
        Task PublishAsync(Metadata metadata, IList<Event> events);
    }
}