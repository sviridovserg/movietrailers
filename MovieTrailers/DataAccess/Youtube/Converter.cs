using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Apis.YouTube.v3.Data;
using MovieTrailers.Models;

namespace MovieTrailers.DataAccess.Youtube
{
    internal class Converter
    {
        private const string MOVIE_URL_FORMAT = "http://www.youtube.com/embed/";
        public Movie Convert(SearchResult result)
        {
            return new Movie()
            {
                SourceId = result.Id.VideoId,
                Source = Models.Source.Youtube,
                CoverUrl = result.Snippet.Thumbnails.Default__.Url,
                Title = result.Snippet.Title
            };
        }

        public MovieTrailer Convert(Video result)
        {
            return new MovieTrailer()
            {
                SourceId = result.Id,
                Source = Models.Source.Youtube,
                CoverUrl = result.Snippet.Thumbnails.Default__.Url,
                Title = result.Snippet.Title,
                Description = result.Snippet.Description,
                ReleaseYear = result.Snippet.PublishedAt.HasValue ? (int?)result.Snippet.PublishedAt.Value.Year : null,
                VideoUrl = GetVideoUrl(result.Id),
                Votes = result.Statistics.LikeCount.GetValueOrDefault()
            };
        }

        private string GetVideoUrl(string id)
        {
            return MOVIE_URL_FORMAT + id;
        }
    }
}