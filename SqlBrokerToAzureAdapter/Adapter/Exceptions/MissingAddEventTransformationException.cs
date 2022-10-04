using System;

namespace SqlBrokerToAzureAdapter.Adapter.Exceptions
{
    /// <summary>
    /// Errors caused when transformations for add-events are missing
    /// </summary>
    public class MissingAddEventTransformationException : Exception
    {
        internal MissingAddEventTransformationException() : base()
        {
        }

        internal MissingAddEventTransformationException(string message) : base(message)
        {
        }

        internal MissingAddEventTransformationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}