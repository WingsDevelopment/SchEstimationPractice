using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Infrastructure.Services.RakicRaiffeisenBrosBankService.Mock.Exceptions
{
    public class RakicRaiffeisenBrosException : Exception
    {
        public RakicRaiffeisenBrosException()
        {
        }

        public RakicRaiffeisenBrosException(string message) : base(message)
        {
        }

        public RakicRaiffeisenBrosException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RakicRaiffeisenBrosException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
