using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieTrailers.Models
{
    public class DataSearchResponse
    {
        public int TotalResults { get; set; }
        public IEnumerable<Movie> Movies { get; set; }
    }
}