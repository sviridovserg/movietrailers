using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieTrailers.Models
{
    public class Response<T>
    {
        public bool IsError { get; set; }
        public T Data { get; set; }
    }
}