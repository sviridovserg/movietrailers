'use strict';

/**
 * @ngdoc function
 * @name movieTrailersApp.controller:mainCtrl
 * @description
 * # MainCtrl
 * Controller that manages search results and paging
 */
angular.module('movieTrailersApp')
  .controller('mainCtrl', ['$scope', '$mdDialog', 'trailersService', 'notificationService', function ($scope, $mdDialog, trailersService, notificationService) {
      $scope.currentPage = 0;
      $scope.pageSize = 20;
      $scope.totalResult = 0;

      $scope.search = function (query, $event) {
          if ($event.keyCode !== 13) {
              return;
          }
          search($scope.query);
      };

      $scope.nextPage = function () {
          if ($scope.currentPage == $scope.getLastPageIndex()) {
              return;
          }
          $scope.currentPage++;
          search($scope.query);
      };

      $scope.prevPage = function () {
          if ($scope.currentPage == 0) {
              return;
          }
          $scope.currentPage--;
          search($scope.query);
      };

      $scope.openVideo = function (selectedMovie) {
          $mdDialog.show({
              controller: 'showTrailerCtrl',
              templateUrl: 'views/showTrailer.html',
              parent: angular.element(document.body),
              clickOutsideToClose: true,
              locals: {
                  movie: selectedMovie,
              }
          });
      }

      $scope.getLastPageIndex = function() {
          return Math.ceil($scope.totalResult / $scope.pageSize);
      }

      function search(query) {
          trailersService.search(getSearchQuery(query)).then(function (result) {
              $scope.searchResult = result.movies;
              $scope.totalResult = result.totalResults;
          },
          function () {
              notificationService.error('Failed to retriev results for your request');
          });
      }

      

      function getSearchQuery(queryText) {
          return {
              query: queryText,
              pageIndex: $scope.currentPage,
              pageSize: $scope.pageSize
          };
      }
  }]);
