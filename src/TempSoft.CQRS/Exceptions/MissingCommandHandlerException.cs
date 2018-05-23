﻿using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Exceptions
{
    [Serializable]
    public class MissingCommandHandlerException : DomainException
    {
        public MissingCommandHandlerException()
        {
        }

        public MissingCommandHandlerException(string message) : base(message)
        {
        }

        public MissingCommandHandlerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingCommandHandlerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}