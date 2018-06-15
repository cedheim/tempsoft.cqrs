﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.Mocks
{
    public class AThingAggregateRoot : AggregateRoot<AThingAggregateRoot>, IAggregateRootWithReadModel
    {
        private readonly List<StuffEntity> _stuff = new List<StuffEntity>();

        public IEnumerable<StuffEntity> Stuff => _stuff;
        public int A { get; private set; }
        public string B { get; private set; }

        public IAggregateRootReadModel GetReadModel()
        {
            return new AThingReadModel
            {
                A = A,
                B = B,
                Version = Version,
                Id = Id,
                Stuff = Stuff.Select(s => new StuffReadModel {Id = s.Id, Message = s.Message}).ToArray()
            };
        }

        [CommandHandler(typeof(DoSomething))]
        public async Task DoSomething(int a, string b, CancellationToken cancellationToken)
        {
            ApplyChange(new ChangedAValue(a));
            ApplyChange(new ChangedBValue(b));
        }

        [EventHandler(typeof(ChangedAValue))]
        private void Apply(ChangedAValue @event)
        {
            A = @event.A;
        }

        [EventHandler(typeof(ChangedBValue))]
        private void Apply(ChangedBValue @event)
        {
            B = @event.B;
        }

        [EventHandler(typeof(CreatedAThing))]
        private void Apply(CreatedAThing @event)
        {
        }

        [CommandHandler(typeof(InitializeAThing))]
        public void Initialize()
        {
            ApplyChange(new CreatedAThing());
        }

        [CommandHandler(typeof(AddStuff))]
        public async Task AddStuff(string entityId, string message, CancellationToken cancellationToken)
        {
            ApplyChange(new AddedStuff(entityId, message));
        }

        [EventHandler(typeof(AddedStuff))]
        public void Apply(AddedStuff @event)
        {
            var entity = new StuffEntity(this, @event.EntityId, @event.Message);
            _stuff.Add(entity);
        }
    }

    public class StuffEntity : AggregateRoot<AThingAggregateRoot>.Entity<StuffEntity>
    {
        public StuffEntity(AThingAggregateRoot root, string id, string message) : base(root, id)
        {
            Message = message;
        }

        public string Message { get; private set; }

        [CommandHandler(typeof(SetStuffMessage))]
        public async Task SetMessage(string message, CancellationToken cancellationToken)
        {
            ApplyChange(new StuffMessageSet(Id, message));
        }

        [EventHandler(typeof(StuffMessageSet))]
        public void Apply(StuffMessageSet @event)
        {
            Message = @event.Message;
        }
    }

    public class InitializeAThing : CommandBase
    {
        private InitializeAThing()
        {
        }

        public InitializeAThing(Guid aggregateRootId)
        {
            AggregateRootId = aggregateRootId;
        }

        public Guid AggregateRootId { get; }
    }

    public class AddStuff : CommandBase
    {
        private AddStuff()
        {
        }

        public AddStuff(string entityId, string message)
        {
            Message = message;
            EntityId = entityId;
        }

        public string Message { get; }

        public string EntityId { get; }
    }

    public class SetStuffMessage : EntityCommandBase
    {
        private SetStuffMessage()
        {
        }

        public SetStuffMessage(string entityId, string message)
        {
            Message = message;
            EntityId = entityId;
        }

        public string Message { get; }
    }

    public class DoSomething : CommandBase
    {
        private DoSomething()
        {
        }

        public DoSomething(int a, string b)
        {
            A = a;
            B = b;
        }

        public int A { get; }
        public string B { get; }
    }

    public class ChangedAValue : EventBase
    {
        private ChangedAValue()
        {
        }

        public ChangedAValue(int a)
        {
            A = a;
        }

        public int A { get; }
    }

    public class ChangedBValue : EventBase
    {
        private ChangedBValue()
        {
        }

        public ChangedBValue(string b)
        {
            B = b;
        }

        public string B { get; }
    }

    public class CreatedAThing : EventBase, IInitializationEvent
    {
        public CreatedAThing()
        {
        }
    }

    public class AddedStuff : EventBase
    {
        private AddedStuff()
        {
        }

        public AddedStuff(string entityId, string message)
        {
            EntityId = entityId;
            Message = message;
        }

        public string EntityId { get; }
        public string Message { get; }
    }

    public class StuffMessageSet : EntityEventBase
    {
        private StuffMessageSet()
        {
        }

        public StuffMessageSet(string entityId, string message)
        {
            EntityId = entityId;
            Message = message;
        }

        public string Message { get; }
    }

    public class StuffReadModel
    {
        public string Id { get; set; }

        public string Message { get; set; }
    }

    public class AThingReadModel : IAggregateRootReadModel
    {
        public int A { get; set; }

        public string B { get; set; }

        public StuffReadModel[] Stuff { get; set; }
        public Guid Id { get; set; }
        public int Version { get; set; }
    }

    public class AThingProjector : ProjectorBase<AThingProjector>
    {
        private readonly IProjectionModelRepository _repository;

        public AThingProjector(IProjectionModelRepository repository)
        {
            _repository = repository;
        }

        [Projector(typeof(CreatedAThing))]
        public async Task Created(Guid aggregateRootId, CancellationToken cancellationToken)
        {
            var id = $"{nameof(AThingProjection)}_{aggregateRootId}";
            var projection = new AThingProjection(id, ProjectorId);

            await _repository.Save(projection, cancellationToken);
        }

        [Projector(typeof(ChangedAValue))]
        public async Task ChangedAValue(Guid aggregateRootId, int a, CancellationToken cancellationToken)
        {
            var id = $"{nameof(AThingProjection)}_{aggregateRootId}";
            var projection = await _repository.Get<AThingProjection>(id, ProjectorId, cancellationToken);

            if (object.ReferenceEquals(projection, default(AThingProjection)))
            {
                throw new Exception("Can not project on unitialized event");
            }

            projection.A = a;
        }

        [Projector(typeof(ChangedBValue))]
        public async Task ChangedBValue(Guid aggregateRootId, string b, CancellationToken cancellationToken)
        {
            var id = $"{nameof(AThingProjection)}_{aggregateRootId}";
            var projection = await _repository.Get<AThingProjection>(id, ProjectorId, cancellationToken);

            if (object.ReferenceEquals(projection, default(AThingProjection)))
            {
                throw new Exception("Can not project on unitialized event");
            }

            projection.B = b;
        }

        [Query(typeof(AThingListQuery))]
        public async Task<IQueryResult> List(AThingListQuery query, CancellationToken cancellationToken)
        {
            var list = new List<AThingProjection>();

            await _repository.List(ProjectorId, (projection, token) =>
            {
                if (projection is AThingProjection aThingProjection)
                {
                    list.Add(aThingProjection);
                }
                return Task.FromResult(true);
            }, cancellationToken);

            return new AThingListResult()
            {
                ProjectorId = ProjectorId,
                Projections = list.ToArray()
            };
        }

        [Query(typeof(AThingEmptyQuery))]
        public IQueryResult Empty()
        {
            return new AThingListResult()
            {
                ProjectorId = ProjectorId,
                Projections = new AThingProjection[0]
            };
        }
    }

    public class AThingProjection : IProjection
    {
        [JsonConstructor]
        private AThingProjection()
        {
        }

        public AThingProjection(string id, string projectorId)
        {
            Id = id;
            ProjectorId = projectorId;
        }

        public string Id { get; set; }

        public string ProjectorId { get; set; }

        public int A { get; set; }

        public string B { get; set; }
    }

    public class AThingEmptyQuery : IQuery
    {

    }

    public class AThingListQuery : IQuery
    {
    }

    public class AThingListResult : IQueryResult
    {
        public string ProjectorId { get; set; }

        public AThingProjection[] Projections { get; set; }
    }
}