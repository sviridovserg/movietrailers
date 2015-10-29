angular.module('movieTrailersApp').factory('trailersService', ['$http', 'appConfig', function ($http, appConfig) {
    'use strict';
    
    function convertItem(item) {
        return {
            id: item.Id,
            title: item.Title,
            coverUrl: item.CoverUrl
        }
    }
    function search(query) {
        return $http.post(appConfig.serviceUrl+"/Search", { Query: query, PageSize: 20 }).then(
            function (response) {
                return _.map(response.data, convertItem);
            });
    }

    return {
        search: search
    };
}]);