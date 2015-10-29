using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using MovieTrailers.DataAccess.Interfaces;

namespace MovieTrailers.DataAccess
{
    public sealed class AppCache : IAppCache
    {
        private static readonly AppCache _instance = new AppCache(HttpContext.Current.Cache);

        private readonly Cache _cache;
        private readonly object _syncObject = new object();

        private AppCache(Cache cache)
        {
            _cache = cache;
        }

        public static AppCache Instance
        {
            get { return _instance; }
        }


        public bool HasData(string key) 
        {
            return _cache[key] != null;
        }

        public void Put(string key, object value)
        {
            lock (_syncObject)
            {
                _cache[key] = value;
            }
        }

        public object Get(string key)
        {
            return _cache[key];
        }
    }
}