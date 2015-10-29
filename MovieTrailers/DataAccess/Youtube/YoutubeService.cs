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
        private class CacheInfo 
        {
            public List<Movie> Movies { get; set; }
            public string NextPageToken { get; set; }
        }

        //Youtube embedded video
        //<iframe id="ytplayer" type="text/html" width="640" height="390"  src="http://www.youtube.com/embed/M7lc1UVf-VE?autoplay=1&origin=http://example.com"  frameborder="0"/>
        private IIdGenerator _idGenerator;
        private IAppCache _appCache;
        private static string _cachePrefix = "youtube";

        public YoutubeService(IIdGenerator idGenerator, IAppCache appCache) 
        {
            _idGenerator = idGenerator;
            _appCache = appCache;
        }

        public async Task<IEnumerable<Movie>> Search(SearchRequest q)
        {
            CacheInfo cachedResult = _appCache.Get(GetCacheKey(q.Query)) as CacheInfo;
            string nextPageToken = string.Empty;
            if (cachedResult != null)
            {
                if (cachedResult.Movies.Count >= (q.PageIndex + 1) * q.PageSize)
                {
                    return cachedResult.Movies.Skip(q.PageIndex * q.PageSize).Take(q.PageSize);
                }
                else 
                {
                    nextPageToken = cachedResult.NextPageToken;
                }
                
            }
            var youtubeService = CreateService();
            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = GetTrailersQuery(q.Query);
            searchListRequest.MaxResults = q.PageSize;
            searchListRequest.Type = "video";
            searchListRequest.PageToken = nextPageToken;

            var searchResult = await searchListRequest.ExecuteAsync();

            var movies = searchResult.Items.Select(Convert);
            if (cachedResult == null) 
            {
                cachedResult = new CacheInfo() { Movies = new List<Movie>() };
            }
            cachedResult.NextPageToken = searchResult.NextPageToken;
            cachedResult.Movies.AddRange(movies);
            return cachedResult.Movies.Skip(q.PageIndex * q.PageSize).Take(q.PageSize);
        }

        private string GetCacheKey(string q) 
        {
            return _cachePrefix + q;
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
                Source = Models.Source.Youtube,
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