﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sociomedia.Application.Domain;
using Sociomedia.Application.Infrastructure.EventStoring;
using Sociomedia.Articles.Domain;

namespace Sociomedia.Articles.Infrastructure
{
    public class ReflectionTypeLocator : ITypeLocator
    {
        private static readonly IDictionary<string, Type> Types = typeof(ArticleEvent)
            .Assembly
            .GetTypes()
            .Where(x => x.IsDomainEvent())
            .ToDictionary(x => x.Name);

        public Type FindEventType(string typeName)
        {
            if (Types.TryGetValue(typeName, out var type)) {
                return type;
            }
            return null;
        }
    }
}