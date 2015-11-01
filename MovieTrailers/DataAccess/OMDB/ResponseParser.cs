using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using MovieTrailers.Models;

namespace MovieTrailers.DataAccess.OMDB
{
    internal class ResponseParser
    {
        private const string MOVIE_URL_FORMAT = "http://www.imdb.com/video/wab/{0}/imdb/embed";

        public Task<MovieTrailer> ParseTrailerResponse(string response)
        {
            return Task.Run<MovieTrailer>(() =>
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);
                var node = doc.SelectSingleNode("//movie");
                if (node == null) 
                {
                    return null;
                }
                MovieTrailer movie = new MovieTrailer();
                movie.SourceId = node.Attributes["imdbID"].Value;
                movie.Title = node.Attributes["title"].Value;
                movie.CoverUrl = node.Attributes["poster"] == null ? string.Empty : node.Attributes["poster"].Value;
                movie.Description = node.Attributes["plot"].Value;
                movie.Source = Models.Source.OMDB;
                int releaseYear;
                if (int.TryParse(node.Attributes["year"].Value, out releaseYear))
                {
                    movie.ReleaseYear = releaseYear;
                }
                ulong votes;
                if (ulong.TryParse(node.Attributes["imdbVotes"].Value, System.Globalization.NumberStyles.AllowThousands, new System.Globalization.CultureInfo("en-US"), out votes))
                {
                    movie.Votes = votes;
                }

                return movie;
            });
        }

        public Task<IEnumerable<Movie>> ParseListResponse(string response)
        {
            return Task.Run<IEnumerable<Movie>>(() =>
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);
                var nodeList = doc.SelectNodes("//result");
                var result = new List<Movie>();

                for (var i = 0; i < nodeList.Count; i++)
                {
                    var node = nodeList[i];
                    var movie = new Movie();
                    movie.SourceId = node.Attributes["imdbID"].Value;
                    movie.Title = node.Attributes["Title"].Value;
                    movie.CoverUrl = node.Attributes["Poster"] == null ? string.Empty : node.Attributes["Poster"].Value;
                    movie.Source = Models.Source.OMDB;
                    int releaseYear;
                    if (int.TryParse(node.Attributes["Year"].Value, out releaseYear))
                    {
                        movie.ReleaseYear = releaseYear;
                    }
                    result.Add(movie);
                }
                return result;
            });
        }

        public Task<string> ParseVideoUrl(string response)
        {
            return Task<string>.Run(() =>
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(response);

                var node = doc.DocumentNode.SelectSingleNode("//a[contains(@class,'title-trailer') and @data-video]");
                if (node == null)
                {
                    node = doc.DocumentNode.SelectSingleNode("//a[@data-video]");
                }
                if (node == null || node.Attributes["data-video"] == null)
                {
                    return string.Empty;
                }
                var videoId = node.Attributes["data-video"].Value;
                return string.Format(MOVIE_URL_FORMAT, videoId);
            });
        }
    }
}