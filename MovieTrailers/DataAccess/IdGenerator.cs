using MovieTrailers.DataAccess.Interfaces;
using System;

namespace MovieTrailers.DataAccess
{
    class IdGenerator: IIdGenerator
    {
        public string GetId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}