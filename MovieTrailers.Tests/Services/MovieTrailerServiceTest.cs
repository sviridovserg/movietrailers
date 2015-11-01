using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MovieTrailers.DataAccess.Interfaces;
using MovieTrailers.Models;
using MovieTrailers.Services;
using System.Linq;

namespace MovieTrailers.Tests.Services
{
    [TestClass]
    public class MovieTrailerServiceTest
    {
        private Mock<IMovieDataAccess> _omdbDataSourceMock;
        private Mock<IMovieDataAccess> _youtubeDataSourceMock;
        private MovieTrailerService _service;

        private IEnumerable<Movie> GenerateTestData(Source source, int count) 
        {
            List<Movie> result = new List<Movie>();
            for (int i = 0; i < count; i++)
            {
                result.Add(new Movie()
                {
                    Source = source,
                    SourceId = string.Format("{0}-{1}", source, i)
                });
            }
            return result;
        }

        [TestInitialize]
        public void TestInit() 
        {
            _omdbDataSourceMock = new Mock<IMovieDataAccess>();
            _omdbDataSourceMock.SetupGet(s => s.Source).Returns(Source.OMDB);

            var omdbData = GenerateTestData(Source.OMDB, 10);
            _omdbDataSourceMock.Setup(s => s.Search(It.IsAny<DataSearchRequest>(), It.IsAny<int>())).ReturnsAsync(new DataSearchResponse() { Movies = omdbData, TotalResults = 10 });
            
            var omdbTrailer = new MovieTrailer() { SourceId = "omId", Title = "OMDBTrailer", ReleaseYear = 2015 };
            _omdbDataSourceMock.Setup(s => s.Get(It.IsAny<string>())).ReturnsAsync(omdbTrailer);
            
            _youtubeDataSourceMock = new Mock<IMovieDataAccess>();
            _youtubeDataSourceMock.SetupGet(s => s.Source).Returns(Source.Youtube);

            _youtubeDataSourceMock.Setup(s => s.Search(It.IsAny<DataSearchRequest>(), It.IsAny<int>()))
                .Returns<DataSearchRequest, int>(
                (r, c) => Task <DataSearchResponse>.Run(() => new DataSearchResponse() { Movies = GenerateTestData(Source.Youtube, c > 100 ? 100 : c), TotalResults = 100 }));

            var youtubeTrailer = new MovieTrailer() { SourceId = "youtubeId", Title = "YoutubeTrailer", ReleaseYear = 2013 };
            _youtubeDataSourceMock.Setup(s => s.Get(It.IsAny<string>())).ReturnsAsync(youtubeTrailer);

            _service = new MovieTrailerService(new IMovieDataAccess[] { _omdbDataSourceMock.Object, _youtubeDataSourceMock.Object });
        }

        [TestMethod]
        public async Task GetTrailerOMDBDataFromCorrectService()
        {
            var response = await _service.GetTrailer("omId", Source.OMDB);
            Assert.AreEqual("omId", response.SourceId);
            Assert.AreEqual("OMDBTrailer", response.Title);
            Assert.AreEqual(2015, response.ReleaseYear);
        }

        [TestMethod]
        public async Task GetTrailerYoutubeDataFromCorrectService()
        {
            var response = await _service.GetTrailer("youtubeId", Source.Youtube);
            Assert.AreEqual("youtubeId", response.SourceId);
            Assert.AreEqual("YoutubeTrailer", response.Title);
            Assert.AreEqual(2013, response.ReleaseYear);
        }

        [TestMethod]
        public async Task SearchReturnsCombinedData() 
        {
            var response = await _service.Search(new SearchRequest() { PageIndex = 0, PageSize = 10, Query = "Test Query" });
            Assert.IsTrue(response.Movies.Where(m => m.Source == Source.OMDB).Count() > 0);
            Assert.IsTrue(response.Movies.Where(m => m.Source == Source.Youtube).Count() > 0);
        }

        [TestMethod]
        public async Task SearchPagesNotChangeWhenPaging()
        {
            Action<string[], SearchResponse, Action<string, Movie>> checkPage = new Action<string[], SearchResponse, Action<string, Movie>>(
                (pageIds, response, checkFunc) => 
                {
                    var page = response.Movies.ToList();
                    for (var i = 0; i < page.Count; i++) 
                    {
                        checkFunc(pageIds[i], page[i]);
                    }
                });
            var firstPageIds = new string[] { "OMDB-0", "OMDB-1", "OMDB-2", "OMDB-3", "OMDB-4", "Youtube-0", "Youtube-1", "Youtube-2", "Youtube-3", "Youtube-4" };
            var request = new SearchRequest() { PageIndex = 0, PageSize = 10, Query = "Test Query" };
            checkPage(firstPageIds, await _service.Search(request), (id, m) => Assert.AreEqual(id, m.SourceId));
            request.PageIndex = 3;
            checkPage(firstPageIds, await _service.Search(request), (id, m) => Assert.AreNotEqual(id, m.SourceId));
            request.PageIndex = 0;
            checkPage(firstPageIds, await _service.Search(request), (id, m) => Assert.AreEqual(id, m.SourceId));
        }

        [TestMethod]
        public async Task SearchNotExistingPage()
        {
            var response = await _service.Search(new SearchRequest() { PageIndex = 20, PageSize = 10, Query= "Q" });
            Assert.AreEqual(0, response.Movies.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task IncorrectSearchPageIndexThrowsException()
        {
            await _service.Search(new SearchRequest() { PageIndex = -1, PageSize=10, Query="Q" });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task IncorrectSearchPageSizeThrowsException()
        {
            await _service.Search(new SearchRequest() { PageIndex = 1, PageSize=-10, Query="Q" });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task IncorrectSearchEmptyQueryThrowsException()
        {
            await _service.Search(new SearchRequest() { PageIndex = 1, PageSize=10, Query="" });
        }

        [TestMethod]
        public async Task SearchTotalsCountIsCorrect()
        {
            var request = new SearchRequest() { PageIndex = 0, PageSize = 10, Query = "Test Query" };
            var response = await _service.Search(request);
            Assert.AreEqual(110, response.TotalResults);
        }

    }
}
