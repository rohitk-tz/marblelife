(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).directive("multiplePhone", ["$rootScope", "GeoService", "APP_CONFIG",
        function ($rootScope, geoService, config) {


            return {
                restrict: "E",
                replace: true,
                scope: {
                    phoneNumbers: '=',
                    isUserPhone:'='
                },
                templateUrl: '/modules/core/views/multiple-phone.client.view.html',
                link: function ($scope, $element, $attrs) {
                    $scope.maxPhoneCount = config.maxPhoneCount;

                    $scope.phoneTypes = [];

                    $scope.addMore = function () {
                        if ($scope.phoneNumbers.length >= $scope.maxPhoneCount) return;
                        $scope.phoneNumbers.push({});
                    };

                    $scope.remove = function (index) {
                        if ($scope.phoneNumbers.length == 1) {
                            $scope.phoneNumbers.splice(index, 1);
                        }
                        if ($scope.phoneNumbers.length == 2) {
                            $scope.phoneNumbers.splice(1, 1);
                        }
                        if ($scope.phoneNumbers.length > 2) {
                            $scope.phoneNumbers.splice(index, 1);
                        }

                        if ($scope.phoneNumbers.length == 0)
                            $scope.phoneNumbers.push({});
                    };

                    function getPhoneTypes() {
                        return geoService.getPhoneTypes().then(function (result) {
                            $scope.phoneTypes = result.data;
                            if($scope.isUserPhone == true)
                            {
                                removeTollFree();
                            }
                        });
                    }

                    function removeTollFree()
                    {
                        angular.forEach($scope.phoneTypes, function (type, index) {
                            if (type.value == 4) // replace with lookup
                                $scope.phoneTypes.splice(index, 1);
                        });
                    }

                    getPhoneTypes();
                }
            }

        }]);
}());