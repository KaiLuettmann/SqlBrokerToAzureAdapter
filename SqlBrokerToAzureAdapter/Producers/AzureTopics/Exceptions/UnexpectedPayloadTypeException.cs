using System;

namespace SqlBrokerToAzureAdapter.Producers.AzureTopics.Exceptions
{
    /// <summary>
    /// Errors caused when the type of payload was not expected.
    /// </summary>
    public class UnexpectedPayloadTypeException : Exception
    {
        internal UnexpectedPayloadTypeException(string message) : base(message)
        {

        }
    }
}