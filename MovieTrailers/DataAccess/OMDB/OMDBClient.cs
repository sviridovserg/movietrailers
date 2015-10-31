using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace MovieTrailers.DataAccess.OMDB
{
    class OMDBClient
    {
        public Task<string> RequestMovieResult(string id)
        {
            return RequestResult("http://www.omdbapi.com/?plot=short&r=xml&i=" + Uri.EscapeDataString(id));
        }

        public Task<string> RequestSearchResult(string query)
        {
            return RequestResult("http://www.omdbapi.com/?r=xml&type=movie&s=" + Uri.EscapeDataString(query));
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

        public async Task<string> GetVideoUrl(string id)
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
                if (node == null || node.Attributes["data-video"] == null)
                {
                    return string.Empty;
                }
                var videoId = node.Attributes["data-video"].Value;
                return "http://www.imdb.com/video/wab/" + videoId + "/imdb/embed";
            });
        }
    }
}