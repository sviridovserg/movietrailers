using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieTrailers.Models
{
    public enum Source 
    {
        Youtube,
        OMDB
    }

    public class Movie
    {
        public Movie()
        {
        }

        public string SourceId { get; set; }

        public Source Source { get; set; }

        public string Title { get; set; }

        public int? ReleaseYear { get; set; }

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

            return SourceId == obj.SourceId && Source == obj.Source && Title == obj.Title && ReleaseYear == obj.ReleaseYear;
        }

        public override int GetHashCode()
        {
            return SourceId.GetHashCode() ^ Source.GetHashCode() ^ Title.GetHashCode() ^ ReleaseYear.GetHashCode();
        }

    }
}