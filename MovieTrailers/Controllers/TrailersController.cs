﻿using System;
using System.Threading.Tasks;
using System.Web.Http;
using MovieTrailers.Interfaces;
using MovieTrailers.Models;

namespace MovieTrailers.Controllers
{
    public class TrailersController : ApiController
    {
        private IMovieTrailerService _movieService;
        public TrailersController(IMovieTrailerService movieService)
        {
            _movieService = movieService;
        }

        [HttpPost]
        public async Task<Response<SearchResponse>> Search(SearchRequest query)
        {
            Response<SearchResponse> result = new Response<SearchResponse>();
            try
            {
                result.Data = await _movieService.Search(query);
            }
            catch (Exception) 
            {
                result.IsError = true;
            }
            return result;
        }
        
        [HttpGet]
        public async Task<Response<MovieTrailer>> GetTrailer(string sourceId, Source source)
        {
            Response<MovieTrailer> result = new Response<MovieTrailer>();
            try
            {
                result.Data = await _movieService.GetTrailer(sourceId, source);
            }
            catch (Exception) 
            {
                result.IsError = true;
            }
            return result;
        }
    }
}
