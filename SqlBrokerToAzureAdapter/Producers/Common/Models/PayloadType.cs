using System;
using SqlBrokerToAzureAdapter.Producers.Common.Exceptions;

namespace SqlBrokerToAzureAdapter.Producers.Common.Models
{
    internal class PayloadType
    {
        private readonly Type value;
        public string FullName
        {
            get
            {
                return value.FullName;
            }
        }

        public PayloadType(Type payloadType)
        {
            value = payloadType ?? throw new ArgumentNullException(nameof(payloadType));

            if (payloadType?.FullName == null)
            {
                throw new InvalidPayloadTypeException("PayloadType must have a FullName.");
            }
        }

        public static implicit operator Type(PayloadType payloadType) => payloadType.value;

        public static implicit operator PayloadType(Type payloadType) => new PayloadType(payloadType);
    }
}