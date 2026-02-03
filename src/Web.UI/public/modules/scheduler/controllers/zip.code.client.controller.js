(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("ZipCodeController", ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "modalParam",
        "FileService", "Toaster", "GeoCodeService", "CalendarService", "FranchiseeService", "UserAuthenticationService", "$window","GeoService",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, modalParam, fileService, toaster, geoCodeService, calendarService, franchiseeService, userAuthenticationService, $window, GeoService) {

            var vm = this;
            vm.query = {
                zipCode: '',
                countryId: 1,
                userId: ''
            };
            vm.isFromSearch = false;
            vm.isVisible = true;
            vm.canSchedule = false;
            vm.franchisee = modalParam.Franchisee;
            vm.franchiseeId = modalParam.FranchiseeId;
            vm.query.userId = modalParam.loggedInUserId;
            vm.isSchedulerHidden = modalParam.isSchedulerHidden == undefined || modalParam.isSchedulerHidden == null ? false : modalParam.isSchedulerHidden;
            vm.Scheduler = Scheduler;
            vm.calendarModel = {};
            vm.isProcessing = false;
            vm.search = search;
            vm.Roles = DataHelper.Role;
            vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
            vm.isExecutive = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
            vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
            vm.CountryIds = DataHelper.CountryId;
            vm.keypress = keypress;
            
            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            vm.options = [];
            function keypress(event) {
                if (event.key === "Enter") {
                    vm.isFromSearch = true;
                    search(vm.query.zipCode);
                }
            }
            function Scheduler(franchiseeId) {
                manageFranchisee(franchiseeId);
            }

            function search(form) {
                var firstChar = vm.query.zipCode.charAt(0);
                $scope.zipCode = vm.query.zipCode;
                if (!isNaN(firstChar) && angular.isNumber(+firstChar)) {
                    if (vm.query.zipCode.length < 5) {
                        var zipCodePrefix = appendZipCode(vm.query.zipCode.length);
                        vm.query.zipCode = zipCodePrefix + vm.query.zipCode;
                    }
                    else {
                        if (vm.query.zipCode.length >= 6) {
                            var zipCode = vm.query.zipCode;
                            vm.query.zipCode = zipCode;
                        }
                    }
                    //vm.query.countryId = vm.CountryIds.USA;
                }
                else {
                    if (vm.query.zipCode.length >= 3) {
                        var zipCode = vm.query.zipCode.substring(0, 3);
                        vm.query.zipCode = zipCode;

                    }
                    //vm.query.countryId = vm.CountryIds.Canada;
                }
                return geoCodeService.getGeoCode(vm.query).then(function (result) {
                    vm.geoCodeList = result.data.collection;
                    vm.query.zipCode = $scope.zipCode;
                    var submit_Button = $window.document.getElementById('btn_submit');
                    if (vm.geoCodeList.length > 0) {
                        vm.canSchedule = vm.geoCodeList[0].canSchedule;
                        vm.franchiseeIdFromAPI = vm.geoCodeList[0].franchiseeId;
                    }
                    else {
                        vm.canSchedule = false;
                        vm.franchiseeIdFromAPI = 0;
                    }
                    submit_Button.focus();
                });
            }

            function appendZipCode(length) {
                var defaultVal = "00000";
                return defaultVal.substring(length, 5);
            }
            function manageFranchisee(franchiseeId) {
                if (vm.isFromSearch)
                    return;
                if (franchiseeId <= 0 || franchiseeId == null)
                    return;

                if (vm.franchiseeId == franchiseeId)
                    return;

                if (vm.isSuperAdmin || vm.isExecutive) {
                    $uibModalInstance.close();
                    $state.go('core.layout.scheduler.manage', { franchiseeId: franchiseeId });
                }
                if (vm.isFranchiseeAdmin) {
                    if (franchiseeId == $rootScope.identity.organizationId)
                        return;

                    manageFranchiseeLogin(franchiseeId);
                }
            }
            vm.newfranchiseeId = 0;
            function manageFranchiseeLogin(franchiseeId) {
                vm.model = { franchiseeId: franchiseeId, userId: modalParam.loggedInUserId, currentFranchiseeId: $rootScope.identity.organizationId };
                return franchiseeService.manageFranchisee(vm.model).then(function (result) {
                    var data = result.data;
                    if (result.data) {
                        vm.newfranchiseeId = franchiseeId;
                        userAuthenticationService.getUserSessionId(navigateToSate);
                    }
                });
            }

            function countryList() {
                GeoService.getCountries().then(function (arr) {
                    vm.allCountries = arr;
                });
            }
            function navigateToSate() {
                $state.go('core.layout.scheduler.manage', { franchiseeId: vm.newfranchiseeId });
                $uibModalInstance.close();
            }

            $q.all([countryList()]);
        }]);
}());