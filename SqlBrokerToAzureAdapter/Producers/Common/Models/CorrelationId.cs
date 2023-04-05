using System;
using SqlBrokerToAzureAdapter.Common;

namespace SqlBrokerToAzureAdapter.Producers.Common.Models
{
    internal class CorrelationId : PrimitiveWrapper<Guid, CorrelationId>
    {
        public CorrelationId(Guid correlationId) : base(correlationId)
        {
            if(correlationId == default)
            {
                throw new ArgumentDefaultException(nameof(correlationId));
            }
        }

        public static implicit operator Guid(CorrelationId correlationId) => correlationId.Value;

        public static implicit operator CorrelationId(Guid correlationId) => new CorrelationId(correlationId);

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}