using System;
using System.Reflection;
using AutoFixture.Kernel;

namespace SqlBrokerToAzureAdapter.Test.TestUtilities.AutoFixture
{
    public class ConstructorArgumentRelay<TTarget,TValueType> : ISpecimenBuilder
    {
        private readonly string _paramName;
        private readonly TValueType _value;

        public ConstructorArgumentRelay(string paramName, TValueType value)
        {
            _paramName = paramName;
            _value = value;
        }

        public object Create(object request, ISpecimenContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            ParameterInfo parameter = request as ParameterInfo;
            if (parameter == null)
            {
                return new NoSpecimen();
            }

            if (parameter.Member.DeclaringType != typeof(TTarget) ||
                parameter.Member.MemberType != MemberTypes.Constructor ||
                parameter.ParameterType != typeof(TValueType) ||
                parameter.Name != _paramName)
            {
                return new NoSpecimen();
            }

            return _value;
        }
    }
}
