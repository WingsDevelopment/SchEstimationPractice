using Common.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Domain.Exceptions
{
    public class WalletEntityException : EstimationPracticeException
    {
        public WalletEntityException()
        {
        }

        public WalletEntityException(string message) : base(message)
        {
        }

        public WalletEntityException(string message = null, string debugMessage = null) : base(message, debugMessage)
        {
        }

        public WalletEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WalletEntityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
