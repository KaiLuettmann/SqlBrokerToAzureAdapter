using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using SqlBrokerToAzureAdapter.MessageContracts;
using SqlBrokerToAzureAdapter.Transformations.Models;

namespace SqlBrokerToAzureAdapter.Transformations
{
    internal sealed class ObjectComparer<T> : IObjectComparer<T>
    {
        private readonly ILogger<ObjectComparer<T>> _logger;
        private readonly ObjectsComparer.Comparer<T> _comparer;

        public ObjectComparer(ILogger<ObjectComparer<T>> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _comparer = new ObjectsComparer.Comparer<T>();
        }

        private bool Compare(T oldValue, T newValue, out IEnumerable<Difference> differences)
        {
            var isEqual = _comparer.Compare(oldValue, newValue, out var o);

            differences = o.Select(difference =>
                new Difference(difference.MemberPath, difference.Value1, difference.Value2)).ToList();

            _logger.LogDebug(
                $"The following members are changed: {string.Join(", ", differences.Select(x => x.MemberPath))}");

            return isEqual;
        }

        private ComparedUpdatedPair<T> Compare(UpdatedPair<T> updatedPair)
        {
            var isEqual = Compare(updatedPair.OldValue, updatedPair.NewValue, out var differences);
            return new ComparedUpdatedPair<T>(updatedPair, differences);
        }

        public IEnumerable<ComparedUpdatedPair<T>> Compare(IEnumerable<UpdatedPair<T>> updatedPairs)
        {
            return updatedPairs.Select(Compare);
        }
    }
}