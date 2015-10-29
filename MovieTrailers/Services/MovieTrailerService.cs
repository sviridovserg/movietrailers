using MovieTrailers.DataAccess.Interfaces;
using MovieTrailers.Interfaces;
using MovieTrailers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MovieTrailers.Services
{
    public class MovieTrailerService: IMovieTrailerService
    {
        private IEnumerable<IMovieDataAccess> _dataServices;
        private IAppCache _appCache;

        public MovieTrailerService(IEnumerable<IMovieDataAccess> dataServices, IAppCache appCache) 
        {
            _dataServices = dataServices;
            _appCache = appCache;
        }

        public async Task<SearchResponse> Search(SearchRequest q)
        {
            List<Movie> result = new List<Movie>();
            foreach (var dataService in _dataServices) 
            {
                result.AddRange(await dataService.Search(q));
            }

            return new SearchResponse()
            {
                Movies = result,
                PageIndex = q.PageIndex
            };
        }
    }
}