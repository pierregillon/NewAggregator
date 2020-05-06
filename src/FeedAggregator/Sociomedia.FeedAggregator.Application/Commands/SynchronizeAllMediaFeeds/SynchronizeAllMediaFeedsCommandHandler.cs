﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Domain;
using Sociomedia.FeedAggregator.Application.Queries;
using Sociomedia.FeedAggregator.Domain.Articles;
using Sociomedia.FeedAggregator.Domain.Medias;

namespace Sociomedia.FeedAggregator.Application.Commands.SynchronizeAllMediaFeeds
{
    public class SynchronizeAllMediaFeedsCommandHandler : ICommandHandler<SynchronizeAllMediaFeedsCommand>
    {
        private readonly IRepository _repository;
        private readonly IFeedReader _feedReader;
        private readonly ArticleFactory _articleFactory;
        private readonly IMediaFeedFinder _mediaFeedFinder;

        public SynchronizeAllMediaFeedsCommandHandler(
            IRepository repository,
            IFeedReader feedReader,
            ArticleFactory articleFactory,
            IMediaFeedFinder mediaFeedFinder)
        {
            _repository = repository;
            _feedReader = feedReader;
            _articleFactory = articleFactory;
            _mediaFeedFinder = mediaFeedFinder;
        }

        public async Task Handle(SynchronizeAllMediaFeedsCommand command)
        {
            var mediaFeeds = await _mediaFeedFinder.GetAll();
            foreach (var feed in mediaFeeds) {
                var externalArticles = await _feedReader.ReadNewArticles(feed.FeedUrl, feed.LastSynchronizationDate);
                if (externalArticles.Any()) {
                    await SynchronizeArticles(feed.MediaId, externalArticles);
                    await UpdateLastSynchronizationDate(feed);
                }
            }
        }

        private async Task SynchronizeArticles(Guid mediaId, IEnumerable<ExternalArticle> externalArticles)
        {
            foreach (var externalArticle in externalArticles) {
                var article = await _articleFactory.Build(mediaId, externalArticle);
                await _repository.Save(article);
            }
        }

        private async Task UpdateLastSynchronizationDate(MediaFeedReadModel mediaFeed)
        {
            var media = await _repository.Get<Media>(mediaFeed.MediaId);

            media.SynchronizeFeed(mediaFeed.FeedUrl);

            await _repository.Save(media);
        }
    }
}