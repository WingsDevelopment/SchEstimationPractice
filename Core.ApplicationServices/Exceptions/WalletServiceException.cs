using Common.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.ApplicationServices.Exceptions
{
    public class WalletServiceException : EstimationPracticeException
    {
        public WalletServiceException()
        {
        }

        public WalletServiceException(string message) : base(message)
        {
        }

        public WalletServiceException(string message = null, string debugMessage = null) : base(message, debugMessage)
        {
        }

        public WalletServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WalletServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
