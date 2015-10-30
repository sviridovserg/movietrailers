using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieTrailers.Models
{
    public class DataSearchRequest
    {
        public string Query { get; set; }

        public DataSearchRequest() { }
        public DataSearchRequest(SearchRequest request) :this() 
        {
            this.Query = request.Query;
        }

        public static implicit operator DataSearchRequest(SearchRequest request)
        {
            return new DataSearchRequest(request);
        }
    }
}