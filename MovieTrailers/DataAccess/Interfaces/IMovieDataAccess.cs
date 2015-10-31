using MovieTrailers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTrailers.DataAccess.Interfaces
{
    public interface IMovieDataAccess
    {
        Task<DataSearchResponse> Search(DataSearchRequest q, int count);
        Task<MovieTrailer> Get(string id);
        Source Source { get; }
    }
}
