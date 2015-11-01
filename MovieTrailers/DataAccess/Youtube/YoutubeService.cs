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
       
        //The restriction of youtube service for pagesize is [4, 50]
        private const int PAGE_SIZE = 50;

        private Converter _converter;
        private YoutubeCache _cache;
        

        public YoutubeService(IAppCache appCache) 
        {
            _converter = new Converter();
            _cache = new YoutubeCache(appCache);
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
            IEnumerable<Movie> allLoadedMovies = _cache.GetCachedMovies(q);
            if (allLoadedMovies.Count() < count)
            {
                await RequestSearch(q, count);
                allLoadedMovies = _cache.GetCachedMovies(q);
            }
            return new DataSearchResponse() { Movies = allLoadedMovies.Take(count), TotalResults = _cache.GetTotalResults(q) };
        }

        private async Task RequestSearch(DataSearchRequest q, int count)
        {
            var loadedMoviesCount = _cache.GetCachedMovies(q).Count();
            int needToLoadPageCount = System.Convert.ToInt32(Math.Ceiling(((double)count - loadedMoviesCount) / PAGE_SIZE));
            var youtubeService = CreateService();
            for (int i = 0; i < needToLoadPageCount; i++)
            {
                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = GetTrailersQuery(q.Query);
                searchListRequest.MaxResults = PAGE_SIZE;
                searchListRequest.Type = "video";
                searchListRequest.PageToken = _cache.GetNextPageToken(q);
                var searchResult = await searchListRequest.ExecuteAsync();
                var movies = searchResult.Items.Select(_converter.Convert);
                _cache.AddMoviesToCache(q.Query, movies, searchResult.PageInfo.TotalResults.GetValueOrDefault(0) , searchResult.NextPageToken);
            }
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