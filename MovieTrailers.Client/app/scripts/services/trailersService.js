angular.module('movieTrailersApp').factory('trailersService', ['$http', 'appConfig', function ($http, appConfig) {
    'use strict';
    var noPosterUri = "http://ia.media-imdb.com/images/G/01/imdb/images/poster/movie_large-2652508870._V_.png";
    function convertItem(item) {
        return {
            id: item.Id,
            title: item.Title,
            coverUrl: item.CoverUrl == "" ? noPosterUri : item.CoverUrl
        }
    }
    function search(query) {
        return $http.post(appConfig.serviceUrl+"/Search", { Query: query, PageSize: 20 }).then(
            function (response) {
                return _.map(response.data.Data.Movies, convertItem);
            });
    }

    return {
        search: search
    };
}]);