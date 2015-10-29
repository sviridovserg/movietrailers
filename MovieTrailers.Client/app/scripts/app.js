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
    'ngMaterial',
  ])
.constant('appConfig', {
    //serviceUrl: 'http://188.227.19.222/DesignerWorkplace/api'
    serviceUrl: 'http://localhost:83/MovieTrailers/api/Trailers'
});
