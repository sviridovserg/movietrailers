using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieTrailers.Models
{
    public class Movie
    {
        public Movie()
        {
        }

        public string CacheId { get; set; }

        public string Id { get; set; }

        public string Title { get; set; }

        public int? ReleaseYear { get; set; }

        public string Genre { get; set; }

        public int Rating { get; set; }

        public string Classification { get; set; }

        public string CoverUrl { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Movie);
        }

        public bool Equals(Movie obj)
        {
            if (obj == null)
            {
                return false;
            }

            return Id == obj.Id && Title == obj.Title && ReleaseYear == obj.ReleaseYear && Genre == obj.Genre &&
                   Rating == obj.Rating &&
                   Classification == obj.Classification;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Title.GetHashCode() ^ ReleaseYear.GetHashCode() ^ Genre.GetHashCode() ^
                   Rating.GetHashCode() ^
                   Classification.GetHashCode();
        }

    }
}