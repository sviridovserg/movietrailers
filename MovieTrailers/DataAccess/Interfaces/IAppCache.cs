using System.Collections.Generic;

namespace MovieTrailers.DataAccess.Interfaces
{
    internal interface IAppCache
    {
        bool HasData(string key);
        void Put(string key, object value);
        object Get(string key);
    }
}