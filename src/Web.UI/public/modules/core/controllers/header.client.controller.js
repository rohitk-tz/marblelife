(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).controller("HeaderController",
        ["$scope", "$rootScope", "$state", "UserAuthenticationService", "$uibModal", "APP_CONFIG", "FileService", "$stateParams", "$q", "FranchiseeService", "DashboardService", "$window",
            function ($scope, $rootScope, $state, UserAuthenticationService, $uibModal, config, fileService, $stateParams, $q, franchiseeService, dashboardService, $window) {

                var vm = this;
                vm.identity = $rootScope.identity;
                vm.Roles = DataHelper.Role;

                vm.isFrontOfficeExe = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
                vm.isToDoCount = $rootScope.identity.todayToDoCount;

                vm.closeDropDown = closeDropDown;
                vm.currentRoleMenuItems = [];
                vm.goToDoList = goToDoList;
                vm.goToFranchiseeStore = goToFranchiseeStore;
                vm.allNavItems = getDefaultMenuItems();
                vm.manageFranchisee = manageFranchisee;
                vm.mlfsConfiguration = mlfsConfiguration;
                vm.currentRole = $rootScope.identity.roleId;
                vm.managePayment = managePayment;
                vm.franchiseeId = $rootScope.identity.organizationId;
                vm.accountCredit = accountCredit;
                vm.currencyExchangeReferenceSite = config.currencyExchangeRateReferenceSite;
                vm.goToReferenceSite = goToReferenceSite;
                vm.geMarketingClass = geMarketingClass;
                vm.getServiceType = getServiceType;
                vm.getTeamImageAfterUplaod = getTeamImageAfterUplaod;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
                vm.isFrontOfficeExecutive = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
                vm.isSalesRep = $rootScope.identity.roleId == vm.Roles.SalesRep;
                vm.isTech = $rootScope.identity.roleId == vm.Roles.Technician;
                vm.getFranchiseeLoan = getFranchiseeLoan;
                vm.openImportModal = openImportModal;
                vm.configureEmailSignatures = configureEmailSignatures;

                vm.loan = loan;
                vm.oneTimeProjectFee = oneTimeProjectFee;
                vm.redirectToBulkPhotoUpload = redirectToBulkPhotoUpload;

                if (vm.isFrontOfficeExe) {
                    if ($state.params != null && $state.params.franchiseeId > 1)
                        vm.franchiseeId = $state.params.franchiseeId;
                    else {
                        vm.franchiseeId = $rootScope.identity.loggedInOrganizationId;
                    }
                }

                function getFranchiseeLoan() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/organizations/views/loan-service.client.view.html',
                        controller: 'LoanServiceController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    FranchiseeId: vm.franchiseeId
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        $state.reload();
                    }, function () {
                    });
                }

                function openImportModal() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/core/views/franchisee-team-image.client.view.html',
                        controller: 'FranchiseeTeamImageController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Franchisee: vm.franchiseeName,
                                    FranchiseeId: vm.franchiseeId
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        getTeamImageAfterUplaod();
                    }, function () {
                    });
                }

                function loan(franchiseeId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/organizations/views/loan-service.client.view.html',
                        controller: 'LoanServiceController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    FranchiseeId: franchiseeId
                                };
                            }
                        }
                    });
                }

                function oneTimeProjectFee(franchiseeId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/organizations/views/oneTimeProject-fee.client.view.html',
                        controller: 'OneTimeProjectFeeController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    FranchiseeId: franchiseeId
                                };
                            }
                        }
                    });
                }
                function goToReferenceSite() {
                    window.open(vm.currencyExchangeReferenceSite, '_blank');
                }
                function goToFranchiseeStore() {
                    window.open('http://www.marblelifefranchisestore.com/', '_blank');
                }

                vm.logout = function () {
                    UserAuthenticationService.logout().then(function (d) {
                        if (d.data == true) {
                            $rootScope.$broadcast('logout');
                            $state.go("authentication.login");
                        }
                    });
                };

                function accountCredit(franchiseeId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/franchisee-account-credit.client.view.html',
                        controller: 'FranchiseeAccountCreditController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    FranchiseeId: franchiseeId
                                };
                            }
                        }
                    });
                }

                function managePayment(franchiseeId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/manage-payment.client.view.html',
                        controller: 'ManagePaymentController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    FranchiseeId: franchiseeId
                                };
                            }
                        }
                    });
                }

                function manageFranchisee() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/core/views/franchisee-list-login.client.view.html',
                        controller: 'ManageFranchiseeController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    FranchiseeId: vm.franchiseeId
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        if (vm.isFranchiseeAdmin)
                            $state.reload();
                    }, function () {
                    });
                }

                function getServiceType() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/core/views/list-serviceType.client.view.html',
                        controller: 'ServiceTypeController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg'
                    });
                    modalInstance.result.then(function () {
                        $state.reload();
                    }, function () {
                    });
                }

                function geMarketingClass() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/core/views/list-marketingClass.client.view.html',
                        controller: 'MarketingClassController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg'
                    });
                    modalInstance.result.then(function () {
                        $state.reload();
                    }, function () {
                    });
                }


                $scope.$on('$includeContentLoaded', function () {
                    Layout.initHeader(); //init header
                });

                $scope.$on("identity", function (ev, data) {
                    vm.identity = $rootScope.identity;
                    prepareMenu();
                });

                // Whenever user logs out, clear root scope context for the user
                $scope.$on("logout", function (ev, data) {
                    $rootScope.identity = null;
                });


                function isStateAccessible(targetState) {

                    return (targetState.roles.indexOf(vm.identity.roleId) >= 0) ? true : false;
                }

                function filterMenuBasedOnRole(data) {
                    for (var cnt = 0; cnt < data.length; cnt++) {
                        if (!data[cnt].children || data[cnt].children.length < 1) continue;
                        //Process Top level menus with sub-menus to display
                        for (var i = 0; i < data[cnt].children.length; i++) {
                            if (isStateAccessible(data[cnt].children[i]) === false) {
                                data[cnt].children.splice(i, 1);
                                i--;
                            }
                        }

                        // After the top level menu is processed, check if it has al-least one sub menu to display,
                        // else, we do not show this top level Menu altogether
                        if (!data[cnt].children || data[cnt].children.length < 1) {
                            data.splice(cnt, 1);
                            cnt--;
                        }

                    }

                    return data;
                }


                function prepareMenu() {
                    if ($rootScope.identity == null) return;
                    vm.currentRoleMenuItems = filterMenuBasedOnRole(vm.allNavItems);

                }

                function mlfsConfiguration() {

                }
                function getProfileImage() {
                    if (vm.identity.fileName != null && vm.identity.fileName != "") {
                        fileService.getFileStreamByUrl(vm.identity.fileName).then(function (result) {
                            $scope.imageUrl = fileService.getStreamUrl(result);
                            vm.imagesrc = $scope.imageUrl;
                            if (vm.identity.css != "" || vm.identity.css != null) {
                                var id = "profile-image";
                                var myElem = document.getElementById(id);
                                myElem.style.transform = vm.identity.css;
                            }
                        })
                    }
                    else {
                        vm.imagesrc = "/content/images/layout/avatar.jpg";
                    }

                }
                function getTeamImage() {
                    if (vm.identity.teamFileName != null && vm.identity.teamFileName != "") {
                        fileService.getFileStreamByUrl(vm.identity.teamFileName).then(function (result) {
                            $scope.teamImageUrl = fileService.getStreamUrl(result);
                            vm.teamImageSrc = $scope.teamImageUrl;
                        })
                    }
                    else {
                        vm.teamImageSrc = "/content/images/layout/avatar.jpg";
                    }

                }
                function getDefaultMenuItems() {
                    return angular.copy([
                        {
                            icon: 'fa fa-home',
                            name: 'Dashboard',
                            state: 'core.layout.home',
                            type: 'link',
                            priority: 2.1,
                            id: 1,
                            class: 'active',
                            roles: [1]
                        },
                        {
                            name: 'Franchisee',
                            icon: 'fa fa-home',
                            type: 'dropdown',
                            priority: 2.2,
                            id: 2,
                            roles: [1, 2],
                            children: [
                                {
                                    name: 'Manage All Franchisee(s)',
                                    state: 'core.layout.franchisee.list',
                                    type: 'link',
                                    roles: [1]
                                },
                                {
                                    name: 'Create Franchisee',
                                    state: 'core.layout.franchisee.create',
                                    type: 'link',
                                    roles: [1]
                                },
                            ]
                        },
                        {
                            name: 'Users',
                            icon: 'fa fa-user',
                            type: 'dropdown',
                            priority: 2.3,
                            id: 3,
                            roles: [1, 2, 5],
                            children: [
                                {
                                    name: 'Manage All User(s)',
                                    state: 'core.layout.user.list({ franchiseeId: ' + vm.franchiseeId + ' })',
                                    type: 'link',
                                    roles: [1, 2, 5],
                                },
                                {
                                    name: 'Create User',
                                    state: 'core.layout.user.create',
                                    type: 'link',
                                    roles: [1, 2],
                                }
                            ]
                        },
                        {
                            name: 'Sales',
                            icon: 'fa fa-user',
                            type: 'dropdown',
                            priority: 2.4,
                            id: 4,
                            roles: [1, 2],
                            children: [
                                {
                                    name: 'All Sales Data',
                                    state: 'core.layout.sales.list',
                                    type: 'link',
                                    roles: [1, 2]
                                },
                                {
                                    name: 'Update Sales Data',
                                    state: 'core.layout.sales.updateMarketingClass',
                                    type: 'link',
                                    roles: [1, 2]
                                },
                                {
                                    name: 'Manage Uploads',
                                    state: 'core.layout.sales.batch.list',
                                    type: 'link',
                                    roles: [1, 2]
                                },
                                {
                                    name: 'Customer List',
                                    state: 'core.layout.sales.customer',
                                    type: 'link',
                                    roles: [1, 2]
                                },
                                {
                                    name: 'Invoice List',
                                    state: 'core.layout.sales.invoice',
                                    type: 'link',
                                    roles: [1, 2]
                                },
                                {
                                    name: 'Annual Uploads',
                                    state: 'core.layout.sales.annual',
                                    type: 'link',
                                    roles: [1, 2]
                                },
                                {
                                    name: 'Annual Customer Details',
                                    state: 'core.layout.sales.annualcustomer',
                                    type: 'link',
                                    roles: [1, 2]
                                }
                            ]
                        },
                        {
                            name: 'Reports',
                            icon: 'fa fa-user',
                            type: 'dropdown',
                            priority: 2.5,
                            id: 5,
                            roles: [1, 2],
                            children: [
                                {
                                    name: 'Sales Report',
                                    state: 'core.layout.report.list',
                                    type: 'link',
                                    roles: [1]
                                },
                                {
                                    name: 'Late Fee Report',
                                    state: 'core.layout.report.lateFees',
                                    type: 'link',
                                    roles: [1]
                                },
                                //{
                                //    name: 'Email Coverage',
                                //    state: 'core.layout.report.email',
                                //    type: 'link',
                                //    roles: [1]
                                //},
                                {
                                    name: 'Customer Feedback',
                                    state: 'core.layout.report.feedback',
                                    type: 'link',
                                    roles: [1]
                                },
                                {
                                    name: 'Leaderboard - Service',
                                    state: 'core.layout.report.service',
                                    type: 'link',
                                    roles: [1, 2]
                                },
                                {
                                    name: 'Leaderboard - Marketing Class',
                                    state: 'core.layout.report.marketingClass',
                                    type: 'link',
                                    roles: [1, 2]
                                },
                                {
                                    name: 'Franchisee Growth Report',
                                    state: 'core.layout.report.growth',
                                    type: 'link',
                                    roles: [1]
                                },
                                {
                                    name: 'Sales Data Upload Report',
                                    state: 'core.layout.report.batch',
                                    type: 'link',
                                    roles: [1]
                                },
                                {
                                    name: 'Product Channel Analysis',
                                    state: 'core.layout.report.product',
                                    type: 'link',
                                    roles: [1]
                                },
                                {
                                    name: 'AR Report',
                                    state: 'core.layout.report.arReport',
                                    type: 'link',
                                    roles: [1]
                                },
                                {
                                    name: 'MLFS Report',
                                    state: 'core.layout.report.mlfs',
                                    type: 'link',
                                    roles: [1]
                                }
                                //,{
                                //    name: 'Franchisee List',
                                //    state: 'core.layout.report.franchiseeListfranchiseeAdmin',
                                //    type: 'link',
                                //    roles: [2]
                                //}
                            ]
                        },
                        {
                            name: 'Marketing Lead',
                            icon: 'fa fa-user',
                            type: 'dropdown',
                            priority: 2.6,
                            id: 6,
                            roles: [1, 2],
                            children: [
                                {
                                    name: 'Call Details Report',
                                    state: 'core.layout.report.callDetail',
                                    type: 'link',
                                    roles: [1, 2, 5]
                                },
                                {
                                    name: 'Web Leads Report',
                                    state: 'core.layout.report.webLead',
                                    type: 'link',
                                    roles: [1, 2]
                                },
                                {
                                    name: 'Marketing Lead Analysis',
                                    state: 'core.layout.report.callDetailAnalysis',
                                    type: 'link',
                                    roles: [1, 2]
                                }, {
                                    name: 'Macro-Sales Funnel National',
                                    state: 'core.layout.sales.macroSales',
                                    type: 'link',
                                    roles: [1]
                                },
                                {
                                    name: 'Macro-Sales Funnel Local',
                                    state: 'core.layout.sales.macroSalesLocal',
                                    type: 'link',
                                    roles: [1, 2, 5]
                                },
                                {
                                    name: 'Lead Performance Report',
                                    state: 'core.layout.report.leadPerformanceReport',
                                    type: 'link',
                                    roles: [1, 2, 5]
                                },
                                {
                                    name: 'Home Advisor Report',
                                    state: 'core.layout.report.homeAdvisor',
                                    type: 'link',
                                    roles: [1, 2, 3]
                                },
                                {
                                    name: 'Back-Up Call Report',
                                    state: 'core.layout.report.automationBackUp',
                                    type: 'link',
                                    roles: [1, 2, 3]
                                }

                            ]
                        },
                        {
                            name: 'Documents',
                            icon: 'fa fa-user',
                            type: 'dropdown',
                            priority: 2.7,
                            id: 7,
                            roles: [1, 2, 3, 4, 5],
                            children: [
                                {
                                    name: 'Franchisee Management Document',
                                    state: 'core.layout.franchisee.document({ franchiseeId: ' + vm.franchiseeId + ' })',
                                    type: 'link',
                                    roles: [1, 2, 3, 4, 5]
                                },
                                {
                                    name: 'National Account Document',
                                    state: 'core.layout.franchisee.national({ franchiseeId: ' + vm.franchiseeId + ' })',
                                    type: 'link',
                                    roles: [1, 2, 3, 4, 5]
                                },
                                {
                                    name: 'Tax Document Report',
                                    state: 'core.layout.franchisee.documentReport({ franchiseeId: ' + vm.franchiseeId + ' })',
                                    type: 'link',
                                    roles: [1]
                                },]
                        }]);
                }
                function getTeamImageAfterUplaod() {
                    return franchiseeService.getFranchiseeTeamImage(vm.franchiseeId).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.fileId = result.data.fileId;
                            if (result.data.imageName != "" && result.data.imageName != null) {
                                vm.ImageName = result.data.imageName;
                                vm.franchiseeName = result.data.franchiseeName;
                                fileService.getFileStreamByUrl(result.data.franchiseeTeamImage).then(function (result) {
                                    $scope.ImageSrc = fileService.getStreamUrl(result);
                                    vm.teamImageSrc = $scope.ImageSrc;
                                })
                            }
                            else {
                                vm.teamImageSrc = "/content/images/layout/avatar.jpg";
                            }

                        }
                        else {
                            vm.ImageSrc = "/content/images/layout/avatar.jpg";
                        }
                    });
                }

                function goToDoList() {
                    $window.sessionStorage.setItem("IsFromFranchiseeLevel", false);
                    $window.sessionStorage.setItem("IsFromFranchiseeId", 0);
                    $window.sessionStorage.setItem("IsFromFranchiseeName", "");
                    $state.go('core.layout.scheduler.todoJobEstimate', { franchiseeId: null });
                }

                function closeDropDown(index) {
                    var element = document.getElementById("dropDown_" + index);
                    element.classList.remove("open");
                }

                function configureEmailSignatures() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/core/views/configure-emailSignature.client.view.html',
                        controller: 'EmailSignatureController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg'
                    });
                    modalInstance.result.then(function () {
                        $state.reload();
                    }, function () {
                    });
                }

                function redirectToBulkPhotoUpload() {
                    //window.open('http://localhost:4200', '_blank');
                    return dashboardService.redirectionToBulkPhotoUpload().then(function (result) {
                        if (result) {
                            window.open(result.data, '_blank');
                        }
                    });
                }

                $q.all([prepareMenu(), getProfileImage(), getTeamImageAfterUplaod()]);
            }]);

}());