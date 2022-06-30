using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SqlBrokerToAzureAdapter.Common
{
    [DebuggerDisplay("{Value}")]
    internal abstract class PrimitiveWrapper<TValue, TDerivedClass> : IEquatable<TDerivedClass>, IFormattable
        where TValue : struct
        where TDerivedClass : PrimitiveWrapper<TValue, TDerivedClass>
    {
        protected PrimitiveWrapper(TValue value)
        {
            Value = value;
        }

        public TValue Value { get; }

        public static implicit operator TValue?(PrimitiveWrapper<TValue, TDerivedClass> primitiveWrapper) => primitiveWrapper?.Value;

        public static implicit operator TValue(PrimitiveWrapper<TValue, TDerivedClass> primitiveWrapper) => primitiveWrapper.Value;

        public static bool operator ==(PrimitiveWrapper<TValue, TDerivedClass> left, PrimitiveWrapper<TValue, TDerivedClass> right) => EqualityComparer<PrimitiveWrapper<TValue, TDerivedClass>>.Default.Equals(left, right);

        public static bool operator !=(PrimitiveWrapper<TValue, TDerivedClass> left, PrimitiveWrapper<TValue, TDerivedClass> right) => !(left == right);

        public override bool Equals(object obj) => Equals(obj as TDerivedClass);

        public bool Equals(TDerivedClass other) => other != null && EqualityComparer<TValue>.Default.Equals(Value, other.Value);

        public override int GetHashCode() => HashCode.Combine(Value);

        public override string ToString() => $"{Value}";

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (Value is IFormattable formattableValue)
            {
                return formattableValue.ToString(format, formatProvider);
            }

            return ToString();
        }
    }
}