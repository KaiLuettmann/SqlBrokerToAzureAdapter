using System;

namespace SqlBrokerToAzureAdapter.Adapter.Exceptions
{
    /// <summary>
    /// Errors caused when transformations for remove-events are missing
    /// </summary>
    public class MissingRemoveEventTransformationException : Exception
    {
        internal MissingRemoveEventTransformationException() : base()
        {
        }

        internal MissingRemoveEventTransformationException(string message) : base(message)
        {
        }

        internal MissingRemoveEventTransformationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}