using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using SqlBrokerToAzureAdapter.Common;
using SqlBrokerToAzureAdapter.Producers.Common.Exceptions;

namespace SqlBrokerToAzureAdapter.Producers.Common.Models
{
    internal class MessageId : PrimitiveWrapper<Guid, MessageId>
    {
        public MessageId(Guid correlationId, string entityId, Type payloadType) : base(CreateMd5(correlationId, entityId, payloadType))
        {
            EnsureEntityIdIsNotEqualToCorrelationId(correlationId, entityId);
        }

        private static void EnsureEntityIdIsNotEqualToCorrelationId(Guid correlationId, string @entityId)
        {
            if (correlationId.ToString() == entityId)
            {
                throw new InvalidEntityIdException($"The entity id '{entityId}' should not be equal to the correlation id '{correlationId}'.");
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        private static Guid CreateMd5(Guid correlationId, string entityId, Type payloadType)
        {
            if (payloadType?.FullName == null)
            {
                throw new ArgumentNullException(nameof(payloadType));
            }
            #pragma warning disable S4790
            using var md5 = MD5.Create();
            #pragma warning restore S4790

            var inputBytes = new List<byte>();
            inputBytes.AddRange(correlationId.ToByteArray());
            inputBytes.AddRange(Encoding.ASCII.GetBytes(entityId));
            inputBytes.AddRange(Encoding.ASCII.GetBytes(payloadType.FullName));
            var hashBytes = md5.ComputeHash(inputBytes.ToArray());
            return new Guid(hashBytes);
        }
    }
}