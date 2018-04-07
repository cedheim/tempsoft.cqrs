﻿using System;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Enums;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Commands
{
    public class RemoveMovieProperty : EntityCommandBase
    {
        private RemoveMovieProperty() { }

        public RemoveMovieProperty(Guid entityId, MovieProperties movieProperty)
        {
            MovieProperty = movieProperty;
            EntityId = entityId;
        }

        public MovieProperties MovieProperty { get; private set; }
    }
}