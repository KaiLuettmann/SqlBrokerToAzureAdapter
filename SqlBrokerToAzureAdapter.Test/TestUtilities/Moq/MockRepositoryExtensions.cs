using System.Collections.Generic;
using Moq;

namespace SqlBrokerToAzureAdapter.Test.TestUtilities.Moq
{
    public static class MockRepositoryExtensions
    {
        public static IEnumerable<Mock<T>> CreateMany<T>(this MockRepository self, int count) where T : class
        {
            for (var i = 0; i < count; i++)
            {
                yield return self.Create<T>();
            }
        }
    }
}