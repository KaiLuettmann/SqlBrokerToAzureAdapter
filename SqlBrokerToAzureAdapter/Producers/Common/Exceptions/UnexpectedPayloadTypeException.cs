using System;

namespace SqlBrokerToAzureAdapter.Producers.Common.Exceptions
{
    /// <summary>
    /// Errors caused when the type of payload was not expected.
    /// </summary>
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
    }
}