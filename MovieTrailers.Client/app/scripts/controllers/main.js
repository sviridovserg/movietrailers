'use strict';

/**
 * @ngdoc function
 * @name designerWorkplaceApp.controller:MainCtrl
 * @description
 * # MainCtrl
 * Controller of the designerWorkplaceApp
 */
angular.module('movieTrailersApp')
  .controller('mainCtrl', ['$scope', 'trailersService', function ($scope, trailersService) {
      

      $scope.search = function (query, $event)
      {
          if ($event.keyCode !== 13) {
              return;
          }
          trailersService.search(query).then(function (result) {
              $scope.searchResult = result;
          });
      }
  }]);
