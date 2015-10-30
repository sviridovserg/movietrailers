angular.module('movieTrailersApp').factory('notificationService', ['$mdToast', function ($mdToast) {
    'use strict';
    return {
        info: function (text) {
            $mdToast.show(
                $mdToast.simple()
                .content(text)
                .position('top right')
                .hideDelay(3000)
            );
        },
        error: function (text) {
            $mdToast.show(
                $mdToast.simple()
                .content(text)
                .theme('error-toast')
                .position('top right')
                .action('Ок')
                .hideDelay(0)
            );
        }
    };
}]);