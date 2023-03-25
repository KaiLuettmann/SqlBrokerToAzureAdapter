using System.Collections.Generic;
using System.Linq;
using SqlBrokerToAzureAdapter.MessageContracts;
using SqlBrokerToAzureAdapter.Transformations;
using SqlBrokerToAzureAdapter.Transformations.Models;

namespace SqlBrokerToAzureAdapter.Test.TestModelBuilders
{
    internal class ComparedUpdatedPairBuilder<T> : TestObjectBuilder<ComparedUpdatedPair<T>>
    {
        public ComparedUpdatedPairBuilder<T> WithUpdatedPair(UpdatedPair<T> updatedPair)
        {
            WithConstructorArgumentFor(nameof(updatedPair), updatedPair);
            return this;
        }

        private ComparedUpdatedPairBuilder<T> WithDifferences(IEnumerable<Difference> differences)
        {
            WithConstructorArgumentFor(nameof(differences), differences);
            return this;
        }

        public ComparedUpdatedPairBuilder<T> WithoutDifferences()
        {
            return WithDifferences(Enumerable.Empty<Difference>());
        }
    }
}