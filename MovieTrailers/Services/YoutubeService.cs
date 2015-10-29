using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using MovieTrailers.Interfaces;
using MovieTrailers.Models;

namespace MovieTrailers.Services
{
    public class YoutubeService: IMovieTrailerService
    {
        public async Task<IEnumerable<Movie>> Search(SearchQuery q)
        {
            var youtubeService = CreateService();

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = q.Query;
            searchListRequest.MaxResults = q.PageSize;
            searchListRequest.Type = "video";

            var searchResult = await searchListRequest.ExecuteAsync();

            return searchResult.Items.Select(Convert);
        }

        private Movie Convert(SearchResult result)
        {
            return new Movie()
            {
                Id = result.Id.VideoId,
                CoverUrl = result.Snippet.Thumbnails.Default__.Url,
                Title = result.Snippet.Title,
            };
        }

        private YouTubeService CreateService()
        {
            return new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyBjMGIDIy2PropLG5GMKEU1GWKx-_VzCcM",
                ApplicationName = "movietrailer"
            });
        }
    }
}