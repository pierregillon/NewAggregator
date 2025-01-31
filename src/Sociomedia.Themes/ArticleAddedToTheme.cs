﻿using System;
using Sociomedia.Core.Domain;

namespace Sociomedia.Themes.Domain
{
    public class ArticleAddedToTheme : DomainEvent
    {
        public ArticleAddedToTheme(Guid themeId, ThemeArticle article) : base(themeId, "theme")
        {
            Article = article;
        }

        public ThemeArticle Article { get; }
    }
}