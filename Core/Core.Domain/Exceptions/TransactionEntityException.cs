using Common.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Domain.Exceptions
{
    public class TransactionEntityException : EstimationPracticeException
    {
        public TransactionEntityException()
        {
        }

        public TransactionEntityException(string message) : base(message)
        {
        }

        public TransactionEntityException(string message = null, string debugMessage = null) : base(message, debugMessage)
        {
        }

        public TransactionEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TransactionEntityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
