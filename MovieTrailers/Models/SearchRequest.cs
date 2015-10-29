using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieTrailers.Models
{
    public class SearchRequest
    {
        public string Query { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}