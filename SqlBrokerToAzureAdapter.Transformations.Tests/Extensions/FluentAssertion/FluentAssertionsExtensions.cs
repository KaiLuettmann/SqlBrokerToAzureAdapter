using FluentAssertions;
using FluentAssertions.Primitives;

namespace SqlBrokerToAzureAdapter.Transformations.Tests.Extensions.FluentAssertion
{
    public static class FluentAssertionsExtensions
    {
        public static AndConstraint<PropertiesOfReferenceTypeAssertions<TSubject>> NotHavePropertyWithDefaultValue<TSubject, TAssertions>(this ReferenceTypeAssertions<TSubject, TAssertions> self, string because = "",
            params object[] becauseArgs)
            where TAssertions : ReferenceTypeAssertions<TSubject, TAssertions>
        {
            return new PropertiesOfReferenceTypeAssertions<TSubject>(self.Subject)
                .NotHavePropertyWithDefaultValue(because, becauseArgs);
        }

        public static AndConstraint<PropertiesOfReferenceTypeAssertions<TSubject>> NotHavePropertyWithEmptyValue<TSubject, TAssertions>(this ReferenceTypeAssertions<TSubject, TAssertions> self, string because = "",
            params object[] becauseArgs)
            where TAssertions : ReferenceTypeAssertions<TSubject, TAssertions>
        {
            return new PropertiesOfReferenceTypeAssertions<TSubject>(self.Subject)
                .NotHavePropertyWithEmptyValue(because, becauseArgs);
        }

        public static AndConstraint<PropertiesOfReferenceTypeAssertions<TSubject>> NotHavePropertyWithNullValue<TSubject, TAssertions>(this ReferenceTypeAssertions<TSubject, TAssertions> self, string because = "",
            params object[] becauseArgs)
            where TAssertions : ReferenceTypeAssertions<TSubject, TAssertions>
        {
            return new PropertiesOfReferenceTypeAssertions<TSubject>(self.Subject)
                .NotHavePropertyWithNullValue(because, becauseArgs);
        }
    }
}