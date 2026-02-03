(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("ReportFranchiseeDocumentController",
        ["$scope", "$rootScope", "$state", "FranchiseeService", "Toaster", "Notification", "APP_CONFIG", "$q", "$uibModal","FileService",
            function ($scope, $rootScope, $state, franchiseeService, toaster, notification, config, $q, $uibModal, fileService) {

                var vm = this;
                vm.isinRoyality = true;
                vm.isinAdFund = false;
                vm.createOtp = false;
                vm.disableOption = false;
                vm.isProcessing = false;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.typeId = DataHelper.ServiceFeeType.OneTimeProject;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
                vm.searchOptions = [];
                vm.getFranchiseeDocumentList = getFranchiseeDocumentList;
                vm.resetSearch = resetSearch;
                vm.showHistry = showHistry;
                vm.downloadFile = downloadFile;
                vm.query =
                {
                    franchiseeId: 0,
                    documentTypeId: 0,
                    uploadedOn: "2021"
                }

                function getFranchiseeList() {
                    return franchiseeService.getFranchiseeListWithOut0ML().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                function getDocumentList() {
                    return franchiseeService.getFranchiseeDocumentList().then(function (result) {
                        vm.documentType = result.data;
                    });
                }


                function getFranchiseeDocumentList() {
                    return franchiseeService.getFranchiseeDocumentReport(vm.query).then(function (result) {
                        if (result != null) {
                            vm.list = result.data.franchiseeViewModel;
                            vm.documentList = result.data.documentList;
                        }
                    });
                }

                function prepareSearchOptions() {
                    var index = 0;
                    for (var i = 1989; i <= 2022; i++) {
                        vm.searchOptions.push({ alias: '' + i, display: '' + i, id: '' + index, value: '' + i });
                        ++index;
                    }
                }

                function resetSearch() {
                    vm.query =
                    {
                        franchiseeId: 0,
                        documentTypeId: 0,
                        uploadedOn: "2021"
                    }
                    getFranchiseeDocumentList();
                }

                function showHistry(documentStatusViewModel,franchiseeName) {
                    //alert(documentStatusViewModel);
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/organizations/views/show-document-histry.client.view.html',
                        controller: 'ShowDocumentHistryController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    FranchiseeName: franchiseeName,
                                    List: documentStatusViewModel,
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                        //getEvents();
                    }, function () {
                    });
                }

                function downloadFile() {
                    return franchiseeService.downloadTaxReport(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            var fileName = "Download_franchisee.xlsx";
                            fileService.downloadFile(result.data, fileName);
                        }
                    });
                }
                $scope.$emit("update-title", "Tax Document Report");
                $q.all([prepareSearchOptions(), getFranchiseeDocumentList(), getFranchiseeList(), getDocumentList()]);
            }]);
}());