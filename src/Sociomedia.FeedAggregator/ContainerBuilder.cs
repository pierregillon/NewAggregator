﻿using Sociomedia.Articles.Infrastructure;
using Sociomedia.Core.Infrastructure;
using Sociomedia.Core.Infrastructure.EventStoring;
using Sociomedia.FeedAggregator.Application;
using Sociomedia.FeedAggregator.Infrastructure;
using Sociomedia.Themes.Infrastructure;
using StructureMap;

namespace Sociomedia.FeedAggregator
{
    public class ContainerBuilder
    {
        public static Container Build(Configuration configuration)
        {
            return new Container(registry => {
                registry.IncludeRegistry(new CoreRegistry(configuration.EventStore));
                registry.IncludeRegistry<ArticlesRegistry>();
                registry.IncludeRegistry<ThemesRegistry>();
                registry.For<Aggregator>().Singleton();
                registry.For<IEventBus>().Use<EventStoreOrgBus>().Singleton();
                registry.For<IEventPositionRepository>().Use<EventPositionRepository>();
                registry.For<Configuration>().Use(configuration).Singleton();
            });
        }
    }
}