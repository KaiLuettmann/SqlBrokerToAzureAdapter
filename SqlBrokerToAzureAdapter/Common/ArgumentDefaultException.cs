using System;

namespace SqlBrokerToAzureAdapter.Common
{
    [Serializable]
    internal class ArgumentDefaultException : ArgumentException
    {
        private const string ExceptionMessage = "Default value is not supported";
        public ArgumentDefaultException(string paramName) : base(ExceptionMessage, paramName)
        {
        }

        public ArgumentDefaultException() : base()
        {
        }

        public ArgumentDefaultException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ArgumentDefaultException(string message, string paramName) : base(message, paramName)
        {
        }

        public ArgumentDefaultException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }
    }
}