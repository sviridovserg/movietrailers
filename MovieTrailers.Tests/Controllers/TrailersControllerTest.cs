using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MovieTrailers;
using MovieTrailers.Controllers;
using MovieTrailers.Interfaces;
using MovieTrailers.Models;

namespace MovieTrailers.Tests.Controllers
{
    [TestClass]
    public class TrailersControllerTest
    {
        private TrailersController _controller;
        private Mock<IMovieTrailerService> _serviceMock;
        [TestInitialize]
        public void InitTest()
        {
            _serviceMock = new Mock<IMovieTrailerService>();
            _controller = new TrailersController(_serviceMock.Object);
        }

        [TestMethod]
        public async Task SearchIfExceptionIsErrorTrue() 
        {
            _serviceMock.Setup((s) => s.Search(It.IsAny<SearchRequest>())).Throws(new Exception());
            var response = await _controller.Search(new SearchRequest());
            Assert.IsTrue(response.IsError);
        }

        [TestMethod]
        public async Task SearchResultInDataProp()
        {
            var testResponse = new SearchResponse() { TotalResults=10, Movies = new List<Movie>() };
            _serviceMock.Setup(s => s.Search(It.IsAny<SearchRequest>())).ReturnsAsync(testResponse);
            var response = await _controller.Search(new SearchRequest());
            Assert.IsFalse(response.IsError);
            Assert.AreEqual(testResponse.TotalResults, response.Data.TotalResults);
            Assert.AreEqual(testResponse.Movies, response.Data.Movies);
        }

        [TestMethod]
        public async Task GetTrailerExceptionIsErrorTrue()
        {
            _serviceMock.Setup((s) => s.GetTrailer(It.IsAny<string>(), It.IsAny<Source>())).Throws(new Exception());
            var response = await _controller.GetTrailer("1", Source.OMDB);
            Assert.IsTrue(response.IsError);
        }

        [TestMethod]
        public async Task GetTrailerResultInDataProp()
        {
            var testTrailer = new MovieTrailer() { SourceId = "id", Title = "MyTrailer", ReleaseYear = 2015 };
            _serviceMock.Setup(s => s.GetTrailer(It.IsAny<string>(), It.IsAny<Source>())).ReturnsAsync(testTrailer);
            var response = await _controller.GetTrailer("id", Source.OMDB);
            Assert.IsFalse(response.IsError);
            Assert.AreEqual(testTrailer.SourceId, response.Data.SourceId);
            Assert.AreEqual(testTrailer.Title, response.Data.Title);
            Assert.AreEqual(testTrailer.ReleaseYear, response.Data.ReleaseYear);
        }
    }
}
