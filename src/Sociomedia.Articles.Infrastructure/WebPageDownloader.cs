﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using FluentAsync;
using Sociomedia.Articles.Domain;
using Sociomedia.Core.Domain;

namespace Sociomedia.Articles.Infrastructure
{
    public class WebPageDownloader : IWebPageDownloader
    {
        private readonly ILogger _logger;

        public WebPageDownloader(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<string> Download(string url)
        {
            return await url
                .Pipe(DownloadWebPageContent)
                .PipeAsync(x => x.ReadAsStringAsync());
        }

        public async Task<Stream> DownloadStream(string url)
        {
            return await url
                .Pipe(DownloadWebPageContent)
                .PipeAsync(x => x.ReadAsStreamAsync());
        }

        private async Task<HttpContent> DownloadWebPageContent(string url)
        {
            try {
                using var client = new HttpClient();
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode) {
                    return response.Content;
                }
                if (response.StatusCode == HttpStatusCode.MovedPermanently) {
                    var movedLocation = response.Headers.Location?.AbsoluteUri;
                    if (!string.IsNullOrWhiteSpace(movedLocation)) {
                        return await DownloadWebPageContent(movedLocation);
                    }
                }
                throw new InvalidOperationException($"Unable to get web page '{url}', http code : {response.StatusCode}");
            }
            catch (Exception ex) {
                Error(ex, url);
                throw new UnreachableWebDocumentException(ex);
            }
        }

        private void Error(Exception exception, string url)
        {
            _logger.Error($"[HTTP_DOWNLOADER] Unable to download web document '{url}' : {exception.Message}");
        }
    }
}