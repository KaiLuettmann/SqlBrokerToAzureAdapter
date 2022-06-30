using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using SqlBrokerToAzureAdapter.Common;

namespace SqlBrokerToAzureAdapter.Producers.Common.Models
{
    internal class MessageId : PrimitiveWrapper<Guid, MessageId>
    {
        public MessageId(Guid correlationId, string entityId, Type payloadType) : base(CreateMd5(correlationId, entityId, payloadType))
        {
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

            using var md5 = MD5.Create();
            var inputBytes = new List<byte>();
            inputBytes.AddRange(correlationId.ToByteArray());
            inputBytes.AddRange(Encoding.ASCII.GetBytes(entityId));
            inputBytes.AddRange(Encoding.ASCII.GetBytes(payloadType.FullName));
            var hashBytes = md5.ComputeHash(inputBytes.ToArray());
            return new Guid(hashBytes);
        }
    }
}