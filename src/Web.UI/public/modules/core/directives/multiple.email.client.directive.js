(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).directive("multipleEmail", ["$rootScope", "APP_CONFIG",
        function ($rootScope, config) {

            return {
                restrict: "E",
                replace: true,
                scope: {
                    emails: '=',
                },
                templateUrl: '/modules/core/views/multiple-email.client.view.html',
                link: function ($scope, $element, $attrs) {
                    $scope.maxEmailCount = 4;

                    $scope.addMore = function () {
                        if ($scope.emails.length >= $scope.maxEmailCount) return;
                        $scope.emails.push({});
                    };

                    $scope.remove = function (index) {
                        $scope.emails.splice(index, 1);

                        if ($scope.emails.length == 0)
                            $scope.emails.push({});
                    };
                }
            }

        }]);
}());