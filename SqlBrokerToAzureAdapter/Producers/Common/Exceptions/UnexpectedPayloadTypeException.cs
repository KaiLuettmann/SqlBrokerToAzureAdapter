using System;
using System.Runtime.Serialization;

namespace SqlBrokerToAzureAdapter.Producers.Common.Exceptions
{
    /// <summary>
    /// Errors caused when the type of payload was not expected.
    /// </summary>
    [Serializable]
    public class UnexpectedPayloadTypeException : Exception
    {
        internal UnexpectedPayloadTypeException(string message) : base(message)
        {
        }

        internal UnexpectedPayloadTypeException() : base()
        {
        }

        internal UnexpectedPayloadTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc/>
        protected UnexpectedPayloadTypeException(SerializationInfo info, StreamingContext ctxt)
        : base(info, ctxt)
        {
        }
    }
}