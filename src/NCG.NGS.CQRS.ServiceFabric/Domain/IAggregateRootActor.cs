﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using NCG.NGS.CQRS.ServiceFabric.Messaging;

namespace NCG.NGS.CQRS.ServiceFabric.Domain
{
    public interface IAggregateRootActor : IActor
    {
        Task Initialize(InitializeMessage message, CancellationToken cancellationToken);

        Task Handle(CommandMessage message, CancellationToken cancellationToken);
    }
}