using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace SqlBrokerToAzureAdapter.Transformations.Tests.Extensions.FluentAssertion
{
    public class PropertiesOfReferenceTypeAssertions<TSubject> :
        ReferenceTypeAssertions<TSubject, PropertiesOfReferenceTypeAssertions<TSubject>>
    {
        public PropertiesOfReferenceTypeAssertions(TSubject instance)
        {
            Subject = instance;
        }

        protected override string Identifier => "PropertiesOfReferenceType";

        public AndConstraint<PropertiesOfReferenceTypeAssertions<TSubject>> NotHavePropertyWithDefaultValue(
            string because = "", params object[] becauseArgs)
        {
            var allProperties = Subject.GetType().GetProperties();
            foreach (var property in allProperties)
            {
                var propertyValue = property.GetValue(Subject);
                var defaultValue = GetDefaultValueForProperty(property);
                if (propertyValue == null)
                {
                    continue;
                }

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(!propertyValue.IsSameOrEqualTo(defaultValue))
                    .FailWith($"Did not expect '{property.Name}' to be default value '{propertyValue}'.");
            }

            return new AndConstraint<PropertiesOfReferenceTypeAssertions<TSubject>>(this);
        }

        public AndConstraint<PropertiesOfReferenceTypeAssertions<TSubject>> NotHavePropertyWithNullValue(string because = "",
            params object[] becauseArgs)
        {
            var allProperties = Subject.GetType().GetProperties();
            foreach (var property in allProperties)
            {
                var propertyValue = property.GetValue(Subject);

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(propertyValue != null)
                    .FailWith($"Did not expect '{property.Name}' to be null.");
            }

            return new AndConstraint<PropertiesOfReferenceTypeAssertions<TSubject>>(this);
        }

        public AndConstraint<PropertiesOfReferenceTypeAssertions<TSubject>> NotHavePropertyWithEmptyValue(string because = "",
            params object[] becauseArgs)
        {
            var allProperties = Subject.GetType().GetProperties();
            foreach (var property in allProperties)
            {
                var isEnumerable = property.PropertyType.GetInterface("IEnumerable") != null;
                if (!isEnumerable)
                {
                    continue;
                }

                if (!(property.GetValue(Subject) is IEnumerable enumerable))
                {
                    continue;
                }

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(!IsEmpty(enumerable))
                    .FailWith($"Did not expect '{property.Name}' to be empty.");
            }

            return new AndConstraint<PropertiesOfReferenceTypeAssertions<TSubject>>(this);
        }

        private static bool IsEmpty(IEnumerable source)
        {
            foreach (var _ in source)
            {
                return false;
            }

            return true;
        }

        private static object GetDefaultValueForProperty(PropertyInfo property)
        {
            var defaultAttr = property.GetCustomAttribute(typeof(DefaultValueAttribute));
            if (defaultAttr != null)
                return (defaultAttr as DefaultValueAttribute)?.Value;

            var propertyType = property.PropertyType;
            return propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
        }
    }
}