using System.Linq;
using AutoFixture;

namespace SqlBrokerToAzureAdapter.Test.TestUtilities.AutoFixture
{
    public static class AutoFixtureExtensions
    {
        public static IFixture ConstructorArgumentFor<TTargetType, TValueType>(
            this IFixture fixture,
            string paramName,
            TValueType value)
        {
            fixture.Customizations.Add(
                new ConstructorArgumentRelay<TTargetType, TValueType>(paramName, value)
            );
            return fixture;
        }
    }
}
