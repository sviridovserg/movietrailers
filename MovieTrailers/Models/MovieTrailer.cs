﻿
namespace MovieTrailers.Models
{
    public class MovieTrailer: Movie
    {
        public string VideoUrl { get; set; }
        public string Description { get; set; }
        public ulong Votes { get; set; }

        public MovieTrailer() { }

        public MovieTrailer(Movie m)
        {
            this.Source = m.Source;
            this.SourceId = m.SourceId;
            this.ReleaseYear = m.ReleaseYear;
            this.Title = m.Title;
            this.CoverUrl = m.CoverUrl;
        }
    }
}