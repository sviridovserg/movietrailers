angular.module('movieTrailersApp')
  .controller('showTrailerCtrl', ['$scope', '$mdDialog', 'trailersService', 'notificationService', 'movie', function ($scope, $mdDialog, trailersService, notificationService, movie) {
      $scope.movie = movie;
      $scope.cancel = function () {
          $mdDialog.cancel();
      };
  }]);