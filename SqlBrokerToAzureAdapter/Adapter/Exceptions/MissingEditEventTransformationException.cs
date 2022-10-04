using System;

namespace SqlBrokerToAzureAdapter.Adapter.Exceptions
{
    /// <summary>
    /// Errors caused when transformations for edit-events are missing
    /// </summary>
    public class MissingEditEventTransformationException : Exception
    {
        internal MissingEditEventTransformationException() : base()
        {
        }

        internal MissingEditEventTransformationException(string message) : base(message)
        {
        }

        internal MissingEditEventTransformationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}