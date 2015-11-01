angular.module('movieTrailersApp').factory('trailersService', ['$http', '$q', 'appConfig', function ($http, $q, appConfig) {
    'use strict';
    var noPosterUri = "http://ia.media-imdb.com/images/G/01/imdb/images/poster/movie_large-2652508870._V_.png";
    function convertItem(item) {
        return {
            id: item.SourceId,
            source: item.Source,
            title: item.Title,
            coverUrl: item.CoverUrl == "" ? noPosterUri : item.CoverUrl,
        }
    }
    function search(searchRequest) {
        var defer = $q.defer();
        $http.post(appConfig.serviceUrl + "/Search",
            {
                Query: searchRequest.query,
                Year: searchRequest.year,
                PageSize: searchRequest.pageSize,
                PageIndex: searchRequest.pageIndex
            }).then(
            function (response) {
                if (response.data.IsError) {
                    defer.reject();
                    return;
                }
                defer.resolve(
                    {
                        movies: _.map(response.data.Data.Movies, convertItem),
                        totalResults: response.data.Data.TotalResults
                    });
            },
            function () {
                defer.reject();
            });

        return defer.promise;
    }

    function getTrailer(movie)
    {
        var defer = $q.defer();
        $http.get(appConfig.serviceUrl + "/GetTrailer?sourceId=" + movie.id + "&source=" + movie.source)
            .then(function (response) {
                if (response.data.IsError) {
                    defer.reject();
                    return;
                }
                var item = response.data.Data;
                defer.resolve({
                    id: item.SourceId,
                    source: item.Source,
                    title: item.Title,
                    coverUrl: item.CoverUrl == "" ? noPosterUri : item.CoverUrl,
                    videoUrl: item.VideoUrl,
                    description: item.Description,
                    year: item.ReleaseYear,
                    votes: item.Votes
                });
            },
            function () {
                defer.reject();
            });
        return defer.promise;
    }

    return {
        search: search,
        getTrailer: getTrailer
    };
}]);