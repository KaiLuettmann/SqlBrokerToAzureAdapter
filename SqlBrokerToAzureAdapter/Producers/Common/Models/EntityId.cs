using System;
using SqlBrokerToAzureAdapter.Producers.Common.Exceptions;

namespace SqlBrokerToAzureAdapter.Producers.Common.Models
{
    internal class EntityId
    {
        private readonly string value;

        public EntityId(string entityId)
        {
            if(entityId == null)
                throw new ArgumentNullException(nameof(entityId));

            if(string.IsNullOrEmpty(entityId))
                throw new InvalidEntityIdException(nameof(entityId));

            value = entityId;
        }

        public static implicit operator string(EntityId entityId) => entityId.value;

        public static implicit operator EntityId(string entityId) => new EntityId(entityId);

        public override string ToString()
        {
            return value;
        }
    }
}