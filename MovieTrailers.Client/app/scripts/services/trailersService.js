angular.module('movieTrailersApp').factory('trailersService', ['$http', '$q', 'appConfig', function ($http, $q, appConfig) {
    'use strict';
    var noPosterUri = "http://ia.media-imdb.com/images/G/01/imdb/images/poster/movie_large-2652508870._V_.png";
    function convertItem(item) {
        return {
            id: item.Id,
            title: item.Title,
            coverUrl: item.CoverUrl == "" ? noPosterUri : item.CoverUrl
        }
    }
    function search(searchRequest) {
        var defer = $q.defer();

        $http.post(appConfig.serviceUrl + "/Search", { Query: searchRequest.query, PageSize: searchRequest.pageSize, PageIndex: searchRequest.pageIndex }).then(
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

    return {
        search: search
    };
}]);