using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieTrailers.DataAccess.OMDB;

namespace MovieTrailers.Tests.DataAccess.OMDB
{
    [TestClass]
    public class ResponseParserTest
    {
        private ResponseParser _parser = new ResponseParser();

        [TestMethod]
        public async Task ParseListResponseCorrectNumberOfResults() 
        {
            string response = @"<root response=""True"">
<result Title=""Title1"" Year=""1991"" imdbID=""id1"" Type=""movie"" Poster=""testUrl1""/>
<result Title=""Title2"" Year=""1992"" imdbID=""id2"" Type=""movie"" Poster=""testUrl2""/>
<result Title=""Title3"" Year=""1993"" imdbID=""id3"" Type=""movie"" Poster=""testUrl3""/>
</root>";
            var result = (await _parser.ParseListResponse(response)).ToList();
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("id1", result[0].SourceId);
            Assert.AreEqual("id2", result[1].SourceId);
            Assert.AreEqual("id3", result[2].SourceId);
        }

        [TestMethod]
        public async Task ParseListResponseCorrectDataInItem()
        {
            string response = @"<root response=""True"">
<result Title=""Title1"" Year=""1991"" imdbID=""id1"" Type=""movie"" Poster=""testUrl1""/>
</root>";
            var result = (await _parser.ParseListResponse(response)).ToList();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("id1", result[0].SourceId);
            Assert.AreEqual("Title1", result[0].Title);
            Assert.AreEqual("testUrl1", result[0].CoverUrl);
            Assert.AreEqual(1991, result[0].ReleaseYear.Value);
        }

        [TestMethod]
        public async Task ParseListResponsNoPosterUrl()
        {
            string response = @"<root response=""True"">
<result Title=""Title1"" Year=""1991"" imdbID=""id1"" Type=""movie"" />
</root>";
            var result = (await _parser.ParseListResponse(response)).ToList();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("", result[0].CoverUrl);
        }

        [TestMethod]
        public async Task ParseListResponseNoResults() 
        {
            string response = @"<root response=""False""><error>Movie not found!</error></root>";
            var result = await  _parser.ParseListResponse(response);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task ParseTrailerResponseNoResults()
        {
            string response = @"<root response=""False""><error>Incorrect IMDb ID</error></root>";
            var result = await _parser.ParseTrailerResponse(response);
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task ParseTrailerResponseCorrectFileds()
        {
            string response = @"<root response=""True"">
<movie title=""title1"" year=""2001"" imdbVotes=""673,435"" imdbID=""id1"" plot=""Test Plot"" poster=""posterUrl"" released=""01 Apr 2001"" runtime=""N/A"" genre=""Short, Action"" director=""Ryan McDonald"" actors=""Joshua Miller"" language=""English"" country=""USA"" imdbRating=""4.2"" type=""movie""/>
</root>";
            var result = await _parser.ParseTrailerResponse(response);
            Assert.AreEqual("id1", result.SourceId);
            Assert.AreEqual(2001, result.ReleaseYear);
            Assert.AreEqual("title1", result.Title);
            Assert.AreEqual((ulong)673435, result.Votes);
            Assert.AreEqual("Test Plot", result.Description);
            Assert.AreEqual("posterUrl", result.CoverUrl);
        }

        [TestMethod]
        public async Task ParseTrailerResponseNoCover()
        {
            string response = @"<root response=""True"">
<movie title=""title1"" year=""2001"" imdbVotes=""673,435"" imdbID=""id1"" plot=""Test Plot"" released=""01 Apr 2001"" runtime=""N/A"" genre=""Short, Action"" director=""Ryan McDonald"" actors=""Joshua Miller"" language=""English"" country=""USA"" imdbRating=""4.2"" type=""movie""/>
</root>";
            var result = await _parser.ParseTrailerResponse(response);
            Assert.AreEqual("", result.CoverUrl);
        }

        [TestMethod]
        public async Task ParseVideoUrlHasTrailer() 
        {
            string response = @"<!DOCTYPE html><html><head></head><body><div></div><a href='video/imdb/vi2052129305/'
class='btn2 title-trailer video-colorbox' data-type='recommends' data-tconst='tt0114709' data-video='vi2052129305' > <span class='btn2_text'>Watch Trailer</span>
</a></body></html>";
            var result = await _parser.ParseVideoUrl(response);
            Assert.IsTrue(result.Contains("vi2052129305"));
        }

        [TestMethod]
        public async Task ParseVideoUrlHasVideoNotTrailer()
        {
            string response = @"<!DOCTYPE html><html><head></head><body><div></div><a href='video/imdb/vi2052129305/'
class='btn2 video-colorbox' data-type='recommends' data-tconst='tt0114709' data-video='vi2052129305' > <span class='btn2_text'>Watch Trailer</span>
</a></body></html>";
            var result = await _parser.ParseVideoUrl(response);
            Assert.IsTrue(result.Contains("vi2052129305"));
        }

        [TestMethod]
        public async Task ParseVideoUrlNoTrailer()
        {
            string response = @"<!DOCTYPE html><html><head></head><body><div></div><a href='video/imdb/vi2052129305/'
class='btn2 video-colorbox' data-type='recommends' data-tconst='tt0114709' > <span class='btn2_text'>Watch Trailer</span>
</a></body></html>";
            var result = await _parser.ParseVideoUrl(response);
            Assert.AreEqual("", result);
        }
    }
}
