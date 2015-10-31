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


        private IAppCache _appCache;
        private static string _cahcePrefix = "omdb";

        public OMDBService(IAppCache appCache) 
        {
            _appCache = appCache;
        }

        public Source Source 
        {
            get { return Models.Source.OMDB; }
        }

        public async Task<MovieTrailer> Get(string id) 
        {
            var movieTrailer = await ParseTrailerResponse(await RequestMovieResult(id));
            movieTrailer.VideoUrl = await GetVideoUrl(movieTrailer.SourceId);
            return movieTrailer;
        }

        public async Task<DataSearchResponse> Search(DataSearchRequest q, int count)
        {
            IEnumerable<Movie> searchResult = _appCache.Get(GetCacheKey(q.Query)) as IEnumerable<Movie>;
            if (searchResult == null)
            {
                var response = await RequestSearchResult(q.Query);
                searchResult = await ParseListResponse(response);
                _appCache.Put(GetCacheKey(q.Query), searchResult);
            }
            
            return new DataSearchResponse() { Movies = searchResult.Take(count), TotalResults = searchResult.Count() };
        }

        private string GetCacheKey(string q)
        {
            return _cahcePrefix + q;
        }

        private Task<string> RequestMovieResult(string id)
        {
            return RequestResult("http://www.omdbapi.com/?plot=short&r=xml&i=" + Uri.EscapeDataString(id));
        }

        private Task<string> RequestSearchResult(string query)
        {
            return RequestResult("http://www.omdbapi.com/?r=xml&type=movie&s=" + Uri.EscapeDataString(query));
        }

        private async Task<string> RequestResult(string uri)
        {
            var result = String.Empty;
            using (var httpClient = new HttpClient {  })
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/xml");
                using (var response = await httpClient.GetAsync(uri))
                {
                    result = await response.Content.ReadAsStringAsync();
                }
            }
            return result;
        }

        private async Task<string> GetVideoUrl(string id)
        {
            var response = await RequestResult("http://www.imdb.com/title/" + id);

            return await Task<string>.Run(() =>
                {
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(response);

                    var node = doc.DocumentNode.SelectSingleNode("//a[contains(@class,'title-trailer') and @data-video]");
                    if (node == null)
                    {
                        node = doc.DocumentNode.SelectSingleNode("//a[@data-video]");
                    }
                    if (node == null || node.Attributes["data-video"] == null) {
                        return string.Empty;
                    }
                    var videoId =  node.Attributes["data-video"].Value;
                    return "http://www.imdb.com/video/wab/" + videoId + "/imdb/embed";
                });
        }
        
        private Task<MovieTrailer> ParseTrailerResponse(string response)
        {
            return Task.Run<MovieTrailer>(() =>
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);
                var node = doc.SelectSingleNode("//movie");
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

        private Task<IEnumerable<Movie>> ParseListResponse(string response)
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