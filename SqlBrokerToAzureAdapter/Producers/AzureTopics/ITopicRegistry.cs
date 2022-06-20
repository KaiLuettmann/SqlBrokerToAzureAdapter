using System;
using System.Collections.Generic;

namespace SqlBrokerToAzureAdapter.Producers.AzureTopics
{
    /// <summary>
    /// The Registry of topics.
    /// <value><see cref="Type"/>: The type of a contract which should be published.</value>
    /// <value><see cref="string"/>: The name of the topic where the message should be published.</value>
    /// </summary>
    public interface ITopicRegistry : IDictionary<Type, string>
    {
    }
}