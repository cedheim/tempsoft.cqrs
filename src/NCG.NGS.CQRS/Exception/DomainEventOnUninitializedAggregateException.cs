﻿using System;
using System.Runtime.Serialization;

namespace NCG.NGS.CQRS.Exception
{
    public class DomainEventOnUninitializedAggregateException : DomainException
    {
        public DomainEventOnUninitializedAggregateException()
        {
        }

        public DomainEventOnUninitializedAggregateException(string message) : base(message)
        {
        }

        public DomainEventOnUninitializedAggregateException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected DomainEventOnUninitializedAggregateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}