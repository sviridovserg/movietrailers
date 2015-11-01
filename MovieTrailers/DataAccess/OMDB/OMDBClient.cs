using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieTrailers.DataAccess.OMDB
{
    internal  class OMDBClient
    {
        private const string REQUEST_MOVIE_URL = "http://www.omdbapi.com/?plot=short&r=xml&i=";
        private const string REQUEST_LIST_URL = "http://www.omdbapi.com/?r=xml&type=movie&s=";
        private const string REQUEST_MOVIE_PAGE_URL = "http://www.imdb.com/title/";
        

        public Task<string> RequestMovieResult(string id)
        {
            return RequestResult(REQUEST_MOVIE_URL + Uri.EscapeDataString(id));
        }

        public Task<string> RequestSearchResult(string query)
        {
            return RequestResult(REQUEST_LIST_URL + Uri.EscapeDataString(query));
        }

        public async Task<string> RequestVideoUrl(string id)
        {
            return await RequestResult(REQUEST_MOVIE_PAGE_URL + id);
        }

        private async Task<string> RequestResult(string uri)
        {
            var result = String.Empty;
            using (var httpClient = new HttpClient { })
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/xml");
                using (var response = await httpClient.GetAsync(uri))
                {
                    result = await response.Content.ReadAsStringAsync();
                }
            }
            return result;
        }
    }
}