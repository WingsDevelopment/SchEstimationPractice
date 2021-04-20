using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Common.Utils.Exceptions
{
    public class EstimationPracticeException : Exception
    {
        public string EstimationPracticeExceptionMessage { get; set; }

        public EstimationPracticeException()
        {
        }

        public EstimationPracticeException(string message = null, string debugMessage = null) : base(debugMessage)
        {
            EstimationPracticeExceptionMessage = message;
        }

        public EstimationPracticeException(string message) : base(message)
        {
        }

        public EstimationPracticeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EstimationPracticeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
