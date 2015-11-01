using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieTrailers.Models
{
    internal class DataSearchRequest
    {
        public string Query { get; set; }
        public int? Year { get; set; }

        public DataSearchRequest() { }
        public DataSearchRequest(SearchRequest request) :this() 
        {
            this.Query = request.Query;
            this.Year = request.Year;
        }

        public static implicit operator DataSearchRequest(SearchRequest request)
        {
            return new DataSearchRequest(request);
        }
    }
}