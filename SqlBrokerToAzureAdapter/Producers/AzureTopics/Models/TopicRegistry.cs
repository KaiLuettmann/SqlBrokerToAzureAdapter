using System;
using System.Collections;
using System.Collections.Generic;

namespace SqlBrokerToAzureAdapter.Producers.AzureTopics.Models
{
    internal sealed class TopicRegistry : ITopicRegistry
    {
        private readonly IDictionary<Type, string> _topicRegistrationsImplementation;

        internal TopicRegistry()
        {
            _topicRegistrationsImplementation = new Dictionary<Type, string>();
        }

        public IEnumerator<KeyValuePair<Type, string>> GetEnumerator()
        {
            return _topicRegistrationsImplementation.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _topicRegistrationsImplementation).GetEnumerator();
        }

        public void Add(KeyValuePair<Type, string> item)
        {
            _topicRegistrationsImplementation.Add(item);
        }

        public void Clear()
        {
            _topicRegistrationsImplementation.Clear();
        }

        public bool Contains(KeyValuePair<Type, string> item)
        {
            return _topicRegistrationsImplementation.Contains(item);
        }

        public void CopyTo(KeyValuePair<Type, string>[] array, int arrayIndex)
        {
            _topicRegistrationsImplementation.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<Type, string> item)
        {
            return _topicRegistrationsImplementation.Remove(item);
        }

        public int Count => _topicRegistrationsImplementation.Count;

        public bool IsReadOnly => _topicRegistrationsImplementation.IsReadOnly;

        public void Add(Type key, string value)
        {
            _topicRegistrationsImplementation.Add(key, value);
        }

        public bool ContainsKey(Type key)
        {
            return _topicRegistrationsImplementation.ContainsKey(key);
        }

        public bool Remove(Type key)
        {
            return _topicRegistrationsImplementation.Remove(key);
        }

        public bool TryGetValue(Type key, out string value)
        {
            return _topicRegistrationsImplementation.TryGetValue(key, out value);
        }

        public string this[Type key]
        {
            get => _topicRegistrationsImplementation[key];
            set => _topicRegistrationsImplementation[key] = value;
        }

        public ICollection<Type> Keys => _topicRegistrationsImplementation.Keys;

        public ICollection<string> Values => _topicRegistrationsImplementation.Values;
    }
}