﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieTrailers.DataAccess.Interfaces;
using MovieTrailers.Interfaces;
using MovieTrailers.Models;

namespace MovieTrailers.Services
{
    internal class MovieTrailerService : IMovieTrailerService
    {
        private IEnumerable<IMovieDataAccess> _dataServices;

        public MovieTrailerService(IEnumerable<IMovieDataAccess> dataServices) 
        {
            _dataServices = dataServices;
        }

        public Task<MovieTrailer> GetTrailer(string sourceId, Source source)
        {
            var sourceDataService = _dataServices.Where(ds => ds.Source == source).FirstOrDefault();
            return sourceDataService.Get(sourceId);
        }

        public async Task<SearchResponse> Search(SearchRequest q)
        {
            if (q.PageIndex < 0 || q.PageSize < 0 || string.IsNullOrEmpty(q.Query))
            {
                throw new ArgumentException("q", "Search Request is incorrect. PageIndex, PageSize cannot be less than 0, Query cannot be empty.");
            }
            List<IEnumerable<Movie>> result = new List<IEnumerable<Movie>>();
            int totalCount = 0;
            foreach (var dataService in _dataServices) 
            {
                var searchRes = await dataService.Search(q, (q.PageIndex + 1) * q.PageSize);
                result.Add(searchRes.Movies);
                totalCount += searchRes.TotalResults;
            }
                
            return new SearchResponse()
            {
                Movies = GetPage(q.PageIndex, q.PageSize, result),
                TotalResults = totalCount
            };
        }

        private IEnumerable<Movie> GetPage(int pageIndex, int pageSize, List<IEnumerable<Movie>> searchResults)
        {
            int mergeBucketSize = Convert.ToInt32(Math.Ceiling((double)pageSize / searchResults.Count));
            List<Movie> result = new List<Movie>();
            bool hasDataToProcess = true;
            int processedItems = 0;
            while (hasDataToProcess) 
            {
                bool processedAnything = false;
                for (int i = 0; i < searchResults.Count; i++)
                {
                    var bucket = searchResults[i].Skip(processedItems).Take(mergeBucketSize);
                    if (bucket.Count() > 0)
                    {
                        processedAnything = true;
                        result.AddRange(bucket);
                    }
                }
                processedItems += mergeBucketSize;
                hasDataToProcess = processedAnything;
            }
            return result.Skip(pageIndex * pageSize).Take(pageSize);
        }
    }
}