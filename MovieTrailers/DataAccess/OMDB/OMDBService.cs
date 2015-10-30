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
using System.Web.Caching;

namespace MovieTrailers.DataAccess.OMDB
{
    class OMDBService : IMovieDataAccess
    {
        //IMDB embedded  video
        //<iframe src="http://www.imdb.com/video/wab/vi3065290777/imdb/embed?autoplay=false&width=480" width="480" height="270" allowfullscreen="true" mozallowfullscreen="true" webkitallowfullscreen="true" frameborder="no" scrolling="no"></iframe>


        private IIdGenerator _idGenerator;
        private IAppCache _appCache;
        private static string _cahcePrefix = "omdb";

        public OMDBService(IIdGenerator idGenerator, IAppCache appCache) 
        {
            _idGenerator = idGenerator;
            _appCache = appCache;
        }

        public async Task<DataSearchResponse> Search(DataSearchRequest q, int count)
        {
            IEnumerable<Movie> searchResult = _appCache.Get(GetCacheKey(q.Query)) as IEnumerable<Movie>;
            if (searchResult == null)
            {
                var response = await RequestSearchResult(q.Query);
                searchResult = await ParseResponse(response);
                _appCache.Put(GetCacheKey(q.Query), searchResult);
            }
            return new DataSearchResponse() { Movies = searchResult.Take(count), TotalResults = searchResult.Count() };
        }

        private string GetCacheKey(string q)
        {
            return _cahcePrefix + q;
        }

        private async Task<string> RequestSearchResult(string query)
        {
            var result = String.Empty;
            using (var httpClient = new HttpClient {  })
            {

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/xml");

                using (var response = await httpClient.GetAsync("http://www.omdbapi.com/?r=xml&type=movie&s=" + Uri.EscapeDataString(query)))
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
    }
}