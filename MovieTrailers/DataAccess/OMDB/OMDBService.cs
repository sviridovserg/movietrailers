using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieTrailers.DataAccess.Interfaces;
using MovieTrailers.Models;

namespace MovieTrailers.DataAccess.OMDB
{
    class OMDBService : IMovieDataAccess
    {
        private const string CACHE_PREFIX = "omdb";
        private IAppCache _appCache;
        private ResponseParser _parser;
        private OMDBClient _client;
        
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
            movieTrailer.VideoUrl =  await _parser.ParseVideoUrl(await _client.RequestVideoUrl(movieTrailer.SourceId));
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
            searchResult = RefineSearch(searchResult, q);
            return new DataSearchResponse() { Movies = searchResult.Take(count), TotalResults = searchResult.Count() };
        }

        private IEnumerable<Movie> RefineSearch(IEnumerable<Movie> list, DataSearchRequest q)
        {
            if (!q.Year.HasValue)
            {
                return list;
            }
            return list.Where(m => m.ReleaseYear == q.Year.Value);
        }

        private string GetCacheKey(string q)
        {
            return CACHE_PREFIX + q;
        }
    }
}