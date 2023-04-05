using System;

namespace SqlBrokerToAzureAdapter.Producers.Common.Exceptions
{
    /// <summary>
    /// Errors caused when the entity id is invalid
    /// </summary>
    [Serializable]
    public class InvalidPayloadTypeException : Exception
    {
        internal InvalidPayloadTypeException() : base()
        {
        }

        internal InvalidPayloadTypeException(string message) : base(message)
        {
        }

        internal InvalidPayloadTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}