﻿using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.Domain.AggregateRoot
{
    [TestFixture]
    public class When_invoking_a_command_which_already_has_been_run
    {
        private AThingAggregateRoot _root;
        private IEvent[] _events;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _root = new AThingAggregateRoot();
            await _root.Initialize(Data.RootId, CancellationToken.None);
            _root.Commit();

            var command = new DoSomething(Data.AValue, Data.BValue);

            await _root.Handle(command, CancellationToken.None);
            await _root.Handle(command, CancellationToken.None);

            _events = _root.Commit().Events;
        }

        [Test]
        public void Should_have_updated_the_values()
        {
            _root.A.Should().Be(Data.AValue);
            _root.B.Should().Be(Data.BValue);
        }

        [Test]
        public void Should_have_triggered_events()
        {
            _events.Should().ContainSingle(e => e is ChangedAValue);
            _events.Should().ContainSingle(e => e is ChangedBValue);
        }


        [Test]
        public void Should_have_updated_version()
        {
            _root.Version.Should().Be(3);
        }

        private static class Data
        {
            public static readonly Guid RootId = Guid.NewGuid();
            public const int AValue = 5;
            public const string BValue = "FLEUF";
        }


    }
}