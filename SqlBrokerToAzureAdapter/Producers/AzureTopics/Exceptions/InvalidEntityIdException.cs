using System;

namespace SqlBrokerToAzureAdapter.Producers.AzureTopics.Exceptions
{
    /// <summary>
    /// Errors caused when the entity id is invalid
    /// </summary>
    public class InvalidEntityIdException : Exception
    {
        internal InvalidEntityIdException(string message) : base(message)
        {

        }
    }
}