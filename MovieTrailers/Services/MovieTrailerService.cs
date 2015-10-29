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
        public MovieTrailerService(IEnumerable<IMovieDataAccess> dataServices) 
        {
            _dataServices = dataServices;
        }

        public async Task<IEnumerable<Movie>> Search(SearchQuery q)
        {
            List<Movie> result = new List<Movie>();
            foreach (var dataService in _dataServices) 
            {
                result.AddRange(await dataService.Search(q));
            }

            return result;
        }
    }
}