'use strict';

/**
 * @ngdoc overview
 * @name movieTrailesApp
 * @description
 * # movieTrailesApp
 *
 * Main module of the application.
 */
angular
  .module('movieTrailersApp', [
      'ngAnimate',
    'ngMaterial',
  ])
.constant('appConfig', {
    serviceUrl: 'http://localhost:9011/MovieTrailers/api/Trailers'
    //serviceUrl: 'http://localhost:83/MovieTrailers/api/Trailers'
});
