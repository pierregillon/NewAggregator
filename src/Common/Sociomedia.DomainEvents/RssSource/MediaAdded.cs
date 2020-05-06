﻿using System;

namespace Sociomedia.DomainEvents.RssSource
{
    public class MediaAdded : DomainEvent
    {
        public string Name { get; }
        public string ImageUrl { get; }
        public PoliticalOrientation PoliticalOrientation { get; }

        public MediaAdded(Guid id, string name, string imageUrl, PoliticalOrientation politicalOrientation) : base(id)
        {
            Name = name;
            ImageUrl = imageUrl;
            PoliticalOrientation = politicalOrientation;
        }
    }
}