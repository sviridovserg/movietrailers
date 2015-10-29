using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTrailers.Models;

namespace MovieTrailers.Interfaces
{
    public interface IMovieTrailerService
    {
        Task<SearchResponse> Search(SearchRequest q);
    }
}
