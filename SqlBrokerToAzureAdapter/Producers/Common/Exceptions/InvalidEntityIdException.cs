using System;

namespace SqlBrokerToAzureAdapter.Producers.Common.Exceptions
{
    /// <summary>
    /// Errors caused when the entity id is invalid
    /// </summary>
    public class InvalidEntityIdException : Exception
    {
        internal InvalidEntityIdException() : base()
        {
        }

        internal InvalidEntityIdException(string message) : base(message)
        {
        }

        internal InvalidEntityIdException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}