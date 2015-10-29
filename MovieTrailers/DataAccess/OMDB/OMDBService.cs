using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieTrailers.Interfaces;
using System.Threading.Tasks;
using MovieTrailers.Models;
using System.Net.Http;
using System.Xml;
using System.Xml.Linq;
using MovieTrailers.DataAccess.Interfaces;

namespace MovieTrailers.DataAccess.OMDB
{
    class OMDBService : IMovieDataAccess
    {
        //IMDB embedded  video
        //<iframe src="http://www.imdb.com/video/wab/vi3065290777/imdb/embed?autoplay=false&width=480" width="480" height="270" allowfullscreen="true" mozallowfullscreen="true" webkitallowfullscreen="true" frameborder="no" scrolling="no"></iframe>

        private Uri baseAddress = new Uri("http://www.omdbapi.com/?r=xml");

        private IIdGenerator _idGenerator;
        public OMDBService(IIdGenerator idGenerator) 
        {
            _idGenerator = idGenerator;
        }

        public async Task<IEnumerable<Movie>> Search(SearchRequest q)
        {
            var response = await RequestSearchResult(q.Query);
            var searchResult = await ParseResponse(response);
            return searchResult;
        }

        private async Task<string> RequestSearchResult(string query)
        {
            var result = String.Empty;
            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/xml");

                using (var response = await httpClient.GetAsync("&type=movie&s=" + Uri.EscapeDataString(query)))
                {
                    result = await response.Content.ReadAsStringAsync();
                }
            }
            return result;
        }

        private Task<IEnumerable<Movie>> ParseResponse(string response)
        {
            return Task.Run<IEnumerable<Movie>>(() =>
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);
                var nodeList  = doc.SelectNodes("//result");
                var result = new List<Movie>();
                
                for (var i = 0; i < nodeList.Count; i++) 
                {
                    var node = nodeList[i];
                    var movie = new Movie();
                    movie.Id = _idGenerator.GetId();
                    movie.SourceId = node.Attributes["imdbID"].Value;
                    movie.Title = node.Attributes["imdbID"].Value;
                    movie.CoverUrl = node.Attributes["Poster"].Value;
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
    }
}