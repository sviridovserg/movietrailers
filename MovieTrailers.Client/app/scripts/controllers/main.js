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
      $scope.isLoading = false;
      $scope.isYearHidden = true;

      $scope.search = function ($event) {
          if ($event && $event.keyCode !== 13) {
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
          return Math.ceil($scope.totalResult / $scope.pageSize) - 1;
      }

      $scope.showYearSelection = function () {
          $scope.isYearHidden = !$scope.isYearHidden;
      }

      $scope.$watch('selectedYear', function () {
          if ($scope.selectedYear === "na" || $scope.query == '' || $scope.query == null) {
              if ($scope.selectedYear === "na") {
                  $scope.selectedYear = undefined;
              }
              return;
          }
          search($scope.query)
      })

      function search(query) {
          if (query == '' || query == null) {
              notificationService.info('Trailer title should be provided');
              return;
          }
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
              year: $scope.selectedYear,
              query: queryText,
              pageIndex: $scope.currentPage,
              pageSize: $scope.pageSize
          };
      }

      var minYear = 1939;
      function getYears() {
          return  _.range(minYear, (new Date()).getFullYear()+2).reverse();
      }

      $scope.years = getYears();
  }]);
