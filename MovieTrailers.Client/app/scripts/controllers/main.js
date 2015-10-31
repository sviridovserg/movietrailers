'use strict';

/**
 * @ngdoc function
 * @name movieTrailersApp.controller:mainCtrl
 * @description
 * # MainCtrl
 * Controller that manages search results and paging
 */
angular.module('movieTrailersApp')
  .controller('mainCtrl', ['$scope', '$mdDialog', 'trailersService', 'notificationService', '$document', function ($scope, $mdDialog, trailersService, notificationService, $document) {
      $scope.currentPage = 0;
      $scope.pageSize = 20;
      $scope.totalResult = 0;
      $scope.isLoading = false;

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
          $scope.isLoading = true;
          trailersService.getTrailer(selectedMovie).then(
              function (trailer) {
                  $mdDialog.show({
                      controller: 'showTrailerCtrl',
                      templateUrl: 'views/showTrailer.html',
                      parent: angular.element(document.body),
                      clickOutsideToClose: true,
                      locals: {
                          movie: trailer
                      }
                  });
                  $scope.isLoading = false;
              },
              function () {
                  notificationService.error('Failed to retrieve movie trailer');
                  $scope.isLoading = false;

              });
      }

      $scope.getLastPageIndex = function() {
          return Math.ceil($scope.totalResult / $scope.pageSize);
      }

      function search(query) {
          $scope.isLoading = true;
          trailersService.search(getSearchQuery(query)).then(function (result) {
              $scope.searchResult = result.movies;
              $scope.totalResult = result.totalResults;
              $scope.isLoading = false;
          },
          function () {
              notificationService.error('Failed to retrieve results for your request');
              $scope.isLoading = false;
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
