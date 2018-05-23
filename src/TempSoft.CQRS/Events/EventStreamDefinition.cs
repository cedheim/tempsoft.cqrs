﻿using System;

namespace TempSoft.CQRS.Events
{
    public class EventStreamDefinition : IComparable, IComparable<EventStreamDefinition>,
        IEquatable<EventStreamDefinition>
    {
        public EventStreamDefinition(string name, EventFilter filter)
        {
            Name = name;
            Filter = filter;
        }

        public EventFilter Filter { get; }

        public string Name { get; }

        public int CompareTo(object o)
        {
            if (o is EventStreamDefinition other) return CompareTo(other);

            return -1;
        }

        public int CompareTo(EventStreamDefinition other)
        {
            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }

        public bool Equals(EventStreamDefinition other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object o)
        {
            return CompareTo(o) == 0;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}