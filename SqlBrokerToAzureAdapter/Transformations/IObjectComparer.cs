using System.Collections.Generic;
using SqlBrokerToAzureAdapter.MessageContracts;
using SqlBrokerToAzureAdapter.Transformations.Models;

namespace SqlBrokerToAzureAdapter.Transformations
{
    internal interface IObjectComparer<T>
    {
        IEnumerable<ComparedUpdatedPair<T>> Compare(IEnumerable<UpdatedPair<T>> updatedPairs);
    }
}