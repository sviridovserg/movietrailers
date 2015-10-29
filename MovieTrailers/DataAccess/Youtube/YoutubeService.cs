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
using MovieTrailers.DataAccess.Interfaces;

namespace MovieTrailers.DataAccess.Youtube
{
    class YoutubeService : IMovieDataAccess
    {
        //Youtube embedded video
        //<iframe id="ytplayer" type="text/html" width="640" height="390"  src="http://www.youtube.com/embed/M7lc1UVf-VE?autoplay=1&origin=http://example.com"  frameborder="0"/>
        private IIdGenerator _idGenerator;
        public YoutubeService(IIdGenerator idGenerator) 
        {
            _idGenerator = idGenerator;
        }

        public async Task<IEnumerable<Movie>> Search(SearchRequest q)
        {
            var youtubeService = CreateService();

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = GetTrailersQuery(q.Query);
            searchListRequest.MaxResults = q.PageSize;
            searchListRequest.Type = "video";

            var searchResult = await searchListRequest.ExecuteAsync();

            return searchResult.Items.Select(Convert);
        }

        private string GetTrailersQuery(string query) 
        {
            if (query.Contains("trailer"))
            {
                return query;
            }
            return query + " trailer";
        }

        private Movie Convert(SearchResult result)
        {
            return new Movie()
            {
                Id = _idGenerator.GetId(),
                SourceId = result.Id.VideoId,
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