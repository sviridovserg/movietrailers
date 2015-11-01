using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieTrailers.DataAccess.Interfaces;
using MovieTrailers.Models;

namespace MovieTrailers.DataAccess.Youtube
{
    internal class YoutubeCache
    {
        private class CacheInfo
        {
            public List<Movie> Movies { get; set; }
            public int TotalResults { get; set; }
            public string NextPageToken { get; set; }
        }
        private const string CAHCE_PREFIX = "youtube";

        private IAppCache _appCache;

        public YoutubeCache(IAppCache appCache)
        {
            _appCache = appCache;
        }

        public IEnumerable<Movie> GetCachedMovies(DataSearchRequest q)
        {
            CacheInfo cachedResult = _appCache.Get(GetCacheKey(q.Query)) as CacheInfo;
            if (cachedResult == null)
            {
                return new List<Movie>();
            }
            return cachedResult.Movies;
        }

        public string GetNextPageToken(DataSearchRequest q)
        {
            CacheInfo cachedResult = _appCache.Get(GetCacheKey(q.Query)) as CacheInfo;
            return cachedResult == null ? string.Empty : cachedResult.NextPageToken;
        }

        public int GetTotalResults(DataSearchRequest q)
        {
            CacheInfo cachedResult = _appCache.Get(GetCacheKey(q.Query)) as CacheInfo;
            return cachedResult == null ? 0 : cachedResult.TotalResults;
        }

        public void AddMoviesToCache(string query, IEnumerable<Movie> movieList, int totalResults, string nextPageToken)
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

    }
}