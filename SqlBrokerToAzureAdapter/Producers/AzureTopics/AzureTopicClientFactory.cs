using System;
using System.Collections.Generic;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using SqlBrokerToAzureAdapter.Producers.Common;

namespace SqlBrokerToAzureAdapter.Producers.AzureTopics
{
    internal class AzureTopicClientFactory : IAzureTopicClientFactory
    {
        private readonly ITopicRegistry _topicRegistry;
        private readonly IAzureTopicConfiguration _configuration;
        private readonly Dictionary<string, ITopicClient> _topicClients = new Dictionary<string, ITopicClient>();
        private readonly ILogger<AzureTopicClientFactory> _logger;
        public AzureTopicClientFactory(ILogger<AzureTopicClientFactory> logger, IAzureTopicConfiguration configuration, ITopicRegistry topicRegistry)
        {
            _topicRegistry = topicRegistry ?? throw new ArgumentNullException(nameof(topicRegistry));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public ITopicClient Get(Type payloadType)
        {
            if (!_topicRegistry.TryGetValue(payloadType, out var topic))
                throw new InvalidOperationException($"Contract type {payloadType.Name} has no topic registration.");

            _logger.LogTrace($"Registered topic for payload type '{payloadType.Name}' is '{topic}'.");

            if (_topicClients.TryGetValue(topic, out var topicClient))
            {
                _logger.LogTrace($"Client for topic '{topic}' fetched from cache.");
                return topicClient;
            }

            topicClient = new TopicClient(_configuration.ConnectionString, topic);
            _logger.LogTrace($"Client for topic '{topic}' created.");
            _topicClients.Add(topic, topicClient);

            return topicClient;
        }
    }
}