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
            public int TotalResults { get; set; }
            public string NextPageToken { get; set; }
        }
        private const string CAHCE_PREFIX = "youtube";
        //The restriction of youtube service for pagesize is [4, 50]
        private const int PAGE_SIZE = 50;

        private Converter _converter;
        private IAppCache _appCache;
        

        public YoutubeService(IAppCache appCache) 
        {
            _converter = new Converter();
            _appCache = appCache;
        }

        public Source Source
        {
            get { return Models.Source.Youtube; }
        }

        public async Task<MovieTrailer> Get(string id)
        {
            var youtubeService = CreateService();
            var searchListRequest = youtubeService.Videos.List("snippet,statistics");
            searchListRequest.Id = id;
            var searchResult = await searchListRequest.ExecuteAsync();
            return _converter.Convert(searchResult.Items.First());
        }

        public async Task<DataSearchResponse> Search(DataSearchRequest q, int count)
        {
            IEnumerable<Movie> allLoadedMovies = GetCachedMovies(q);
            if (allLoadedMovies.Count() < count)
            {
                await RequestSearch(q, count);
                allLoadedMovies = GetCachedMovies(q);
            }
            return new DataSearchResponse() { Movies = allLoadedMovies.Take(count), TotalResults = GetTotalResults(q) };
        }

        private async Task RequestSearch(DataSearchRequest q, int count)
        {
            var loadedMoviesCount = GetCachedMovies(q).Count();
            int needToLoadPageCount = System.Convert.ToInt32(Math.Ceiling(((double)count - loadedMoviesCount) / PAGE_SIZE));
            var youtubeService = CreateService();
            for (int i = 0; i < needToLoadPageCount; i++)
            {
                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = GetTrailersQuery(q.Query);
                searchListRequest.MaxResults = PAGE_SIZE;
                searchListRequest.Type = "video";
                searchListRequest.PageToken = GetNextPageToken(q);
                var searchResult = await searchListRequest.ExecuteAsync();
                var movies = searchResult.Items.Select(_converter.Convert);
                AddMoviesToCache(q.Query, movies, searchResult.PageInfo.TotalResults.GetValueOrDefault(0) , searchResult.NextPageToken);
            }
        }

        private IEnumerable<Movie> GetCachedMovies(DataSearchRequest q)
        {
            CacheInfo cachedResult = _appCache.Get(GetCacheKey(q.Query)) as CacheInfo;
            if (cachedResult == null)
            {
                return new List<Movie>();
            }
            return cachedResult.Movies;
        }

        private string GetNextPageToken(DataSearchRequest q)
        {
            CacheInfo cachedResult = _appCache.Get(GetCacheKey(q.Query)) as CacheInfo;
            return cachedResult == null ? string.Empty : cachedResult.NextPageToken;
        }

        private int GetTotalResults(DataSearchRequest q) 
        {
            CacheInfo cachedResult = _appCache.Get(GetCacheKey(q.Query)) as CacheInfo;
            return cachedResult == null ? 0 : cachedResult.TotalResults;
        }

        private void AddMoviesToCache(string query, IEnumerable<Movie> movieList, int totalResults, string nextPageToken)
        {
            CacheInfo cachedResult = _appCache.Get(GetCacheKey(query)) as CacheInfo;
            if (cachedResult == null) 
            {
                cachedResult = new CacheInfo() { Movies = new List<Movie>() };
            }
            cachedResult.NextPageToken = nextPageToken;
            cachedResult.TotalResults = totalResults;
            cachedResult.Movies.AddRange(movieList);
            _appCache.Put(GetCacheKey(query), cachedResult);
        }

        private string GetCacheKey(string q) 
        {
            return CAHCE_PREFIX + q;
        }

        private string GetTrailersQuery(string query) 
        {
            if (query.Contains("trailer"))
            {
                return query;
            }
            return query + " trailer";
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