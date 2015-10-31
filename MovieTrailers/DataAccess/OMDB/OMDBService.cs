using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieTrailers.Interfaces;
using System.Threading.Tasks;
using MovieTrailers.Models;
using System.Net.Http;
using MovieTrailers.DataAccess.Interfaces;

namespace MovieTrailers.DataAccess.OMDB
{
    class OMDBService : IMovieDataAccess
    {
        private IAppCache _appCache;
        private ResponseParser _parser;
        private OMDBClient _client;
        private static string CACHE_PREFIX = "omdb";

        public OMDBService(IAppCache appCache) 
        {
            _parser = new ResponseParser();
            _client = new OMDBClient();
            _appCache = appCache;
        }

        public Source Source 
        {
            get { return Models.Source.OMDB; }
        }

        public async Task<MovieTrailer> Get(string id) 
        {
            var movieTrailer = await _parser.ParseTrailerResponse(await _client.RequestMovieResult(id));
            movieTrailer.VideoUrl = await _client.GetVideoUrl(movieTrailer.SourceId);
            return movieTrailer;
        }

        public async Task<DataSearchResponse> Search(DataSearchRequest q, int count)
        {
            IEnumerable<Movie> searchResult = _appCache.Get(GetCacheKey(q.Query)) as IEnumerable<Movie>;
            if (searchResult == null)
            {
                var response = await _client.RequestSearchResult(q.Query);
                searchResult = await _parser.ParseListResponse(response);
                _appCache.Put(GetCacheKey(q.Query), searchResult);
            }
            return new DataSearchResponse() { Movies = searchResult.Take(count), TotalResults = searchResult.Count() };
        }

        private string GetCacheKey(string q)
        {
            return CACHE_PREFIX + q;
        }
    }
}