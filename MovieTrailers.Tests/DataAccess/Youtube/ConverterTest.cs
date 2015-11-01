using System;
using Google.Apis.YouTube.v3.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MovieTrailers.DataAccess.Youtube;
using MovieTrailers.Models;

namespace MovieTrailers.Tests.DataAccess.Youtube
{
    [TestClass]
    public class ConverterTest
    {
        private Converter _coverter = new Converter();

        private SearchResult GetTestSearchResult() 
        {
            SearchResult item = new SearchResult()
            {
                Id = new ResourceId() { VideoId = "id" },
                Snippet = new SearchResultSnippet()
                {
                    Title = "TestTitle",
                    Thumbnails = new ThumbnailDetails() { Default__ = new Thumbnail() { Url = "TestCoverUrl" } }
                }
            };
            return item;
        }

        private Video GetTestVideo()
        {
            Video v = new Video() { Id = "id"};
            ThumbnailDetails thumbnails = new ThumbnailDetails() { Default__ = new Thumbnail() { Url = "TestCoverUrl" } };
            v.Snippet = new VideoSnippet() 
            { 
                Title = "TestTitle", 
                Description = "TestDescription", 
                PublishedAt = new DateTime(2013, 1, 23), 
                Thumbnails = thumbnails,
            };
            v.Statistics = new VideoStatistics() { LikeCount = 20 };
            return v;
        }

        [TestMethod]
        public void ConvertSearchItemCorrectFields()
        {
            var movie = _coverter.Convert(GetTestSearchResult());
            Assert.AreEqual("id", movie.SourceId);
            Assert.AreEqual(Source.Youtube, movie.Source);
            Assert.AreEqual("TestTitle", movie.Title);
            Assert.AreEqual("TestCoverUrl", movie.CoverUrl);
        }

        [TestMethod]
        public void ConvertVideoCorrectFields() 
        {
            var movie = _coverter.Convert(GetTestVideo());
            Assert.AreEqual("id", movie.SourceId);
            Assert.AreEqual(Source.Youtube, movie.Source);
            Assert.AreEqual("TestTitle", movie.Title);
            Assert.AreEqual("TestCoverUrl", movie.CoverUrl);
            Assert.AreEqual("TestDescription", movie.Description);
            Assert.AreEqual(2013, movie.ReleaseYear);
            Assert.AreEqual((ulong)20, movie.Votes);
            Assert.IsTrue(movie.VideoUrl.Contains("id"));
        }
    }
}
