(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).directive("multiplePhoneOrganization", ["$rootScope", "GeoService", "APP_CONFIG", "$filter",
        function ($rootScope, geoService, config, $filter) {


            return {
                restrict: "E",
                replace: true,
                scope: {
                    phoneNumbers: '=',
                    isUserPhone: '='
                },
                templateUrl: '/modules/core/views/multiple-phone-organization.client.view.html',
                link: function ($scope, $element, $attrs) {
                    $scope.maxPhoneCount = config.maxPhoneCount;

                    $scope.phoneTypes = [];

                    $scope.addMore = function () {
                        if ($scope.phoneNumbers.length >= $scope.maxPhoneCount) return;
                        var tempId=$scope.phoneNumbers.length;
                        $scope.phoneNumbers.push({ tempId: tempId++});
                    };
                    $scope.transferableClick = function (pn) {
                        var isTransferable = pn.isTransferable;
                        angular.forEach($scope.phoneNumbers, function (phone) {
                            phone.isTransferable = false;
                        });

                        angular.forEach($scope.phoneNumbers, function (phone) {
                            if (phone.tempId == pn.tempId)
                                phone.isTransferable = isTransferable;
                        });
                    };

                    $scope.remove = function (index) {
                        if ($scope.phoneNumbers.length == 1) {
                            $scope.phoneNumbers.splice(index, 1);
                        }
                        if ($scope.phoneNumbers.length == 2) {
                            $scope.phoneNumbers.splice(index, 1);
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
                            if ($scope.isUserPhone == true) {
                                removeTollFree();
                            }
                        });
                    }

                    function removeTollFree() {
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