﻿using System;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Infrastructure;
using TempSoft.CQRS.ServiceFabric.Commands;
using TempSoft.CQRS.ServiceFabric.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Domain;
using TempSoft.CQRS.ServiceFabric.Interfaces.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Projectors;
using TempSoft.CQRS.ServiceFabric.Tools;

namespace TempSoft.CQRS.ServiceFabric.Infrastructure
{
    public static class ServiceFabricBootstrapperExtensions
    {
        public static FluentBootstrapper UseServiceFabricService(this FluentBootstrapper bootstrapper)
        {
            bootstrapper.UseService<ICommandRouter, ServiceFabricCommandRouter>(true);
            bootstrapper.UseService<IEventBus, ServiceFabricEventBus>(true);
            bootstrapper.UseService<IUriHelper, UriHelper>(true);
            bootstrapper.UseService<IActorProxyFactory, ActorProxyFactory>(true);
            bootstrapper.UseService<IServiceProxyFactory, ServiceProxyFactory>(true);

            return bootstrapper;
        }

        public static FluentBootstrapper UseEventBusUri(this FluentBootstrapper bootstrapper, Uri uri)
        {
            var uriHelper = bootstrapper.Resolve<IUriHelper>();
            uriHelper.RegisterUri<IEventBusService>(uri);

            return bootstrapper;
        }

        public static FluentBootstrapper UseAggregateRootActorUri(this FluentBootstrapper bootstrapper, Uri uri)
        {
            var uriHelper = bootstrapper.Resolve<IUriHelper>();
            uriHelper.RegisterUri<IAggregateRootActor>(uri);

            return bootstrapper;
        }

        public static FluentBootstrapper UseProjectorActorrUri(this FluentBootstrapper bootstrapper, Uri uri)
        {
            var uriHelper = bootstrapper.Resolve<IUriHelper>();
            uriHelper.RegisterUri<IProjectorActor>(uri);

            return bootstrapper;
        }
    }
}