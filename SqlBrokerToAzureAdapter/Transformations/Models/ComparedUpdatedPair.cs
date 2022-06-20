using System.Collections.Generic;
using System.Linq;
using SqlBrokerToAzureAdapter.MessageContracts;

namespace SqlBrokerToAzureAdapter.Transformations.Models
{
    internal class ComparedUpdatedPair<T>
    {
        public UpdatedPair<T> UpdatedPair { get; }
        public IEnumerable<Difference> Differences { get; }
        public bool IsEqual => !Differences.Any();

        public ComparedUpdatedPair(UpdatedPair<T> updatedPair, IEnumerable<Difference> differences)
        {
            UpdatedPair = updatedPair;
            Differences = differences;
        }
    }
}