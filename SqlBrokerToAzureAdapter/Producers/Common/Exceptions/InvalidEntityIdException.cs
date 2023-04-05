using System;
using System.Runtime.Serialization;

namespace SqlBrokerToAzureAdapter.Producers.Common.Exceptions
{
    /// <summary>
    /// Errors caused when the entity id is invalid
    /// </summary>
    [Serializable]
    public class InvalidEntityIdException : Exception
    {
        /// <inheritdoc/>
        public InvalidEntityIdException()
        {
        }

        /// <inheritdoc/>
        public InvalidEntityIdException(string message) : base(message)
        {
        }

        /// <inheritdoc/>
        public InvalidEntityIdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc/>
        protected InvalidEntityIdException(SerializationInfo info, StreamingContext ctxt)
        : base(info, ctxt)
        {
        }
    }
}