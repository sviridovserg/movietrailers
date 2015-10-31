angular.module('movieTrailersApp')
  .controller('showTrailerCtrl', ['$scope', '$mdDialog', '$sce', 'movie', function ($scope, $mdDialog, $sce, movie) {
      $scope.movie = movie;
      if (movie.videoUrl == '') {
          $scope.videoUnavailable = true;
      }
      $scope.videoUrl = $sce.trustAsResourceUrl(movie.videoUrl + '?autoplay=1&width=480&height=360');
      $scope.cancel = function () {
          $mdDialog.cancel();
      };

      $scope.showDescription = function () {
          $scope.isDescriptionOpen = !$scope.isDescriptionOpen;
      }
  }]);