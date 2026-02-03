(function () {
    'use strict';
    var SortColumns = {
        Id: 'Id',
        Name: 'Name',
        ExpiryDate: 'ExpiryDate',
        UploadDate: 'UploadDate',
        UploadedBy: 'UploadedBy',
        Franchisee: 'Franchisee',
        Type: 'Type'
    }

    angular.module(OrganizationsConfiguration.moduleName).controller("NationalDocumentsController",
        ["$scope", "$rootScope", "FranchiseeService", "FileService", "Notification",
            "APP_CONFIG", "$uibModal",
            "FranchiseeDocumentService", "$q", "Toaster", "URLAuthenticationServiceForEncryption",
            function ($scope, $rootScope, franchiseeService, fileService, notification, config, $uibModal,
                franchiseeDocumentService, $q, toaster, URLAuthenticationServiceForEncryption) {
                var vm = this;
                vm.query = {
                    pageNumber: 1,
                    franchiseeId: 0,
                    text: '',
                    email: '',
                    periodStartDate: null,
                    periodEndDate: null,
                    isImportant: '',
                    pageSize: config.defaultPageSize,
                    categoryId: DataHelper.DocumentType.NationalAccountDocuments,
                    documentTypeId: 0,
                    sort: { order: 0, propName: '' }
                };
                vm.editFranchisee = editFranchisee;
                vm.pagingOptions = config.pagingOptions;
                vm.currentPage = vm.query.pageNumber;
                vm.Roles = DataHelper.Role;
                vm.userId = $rootScope.identity.userId;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
                vm.isFrontOffice = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
                vm.pageChange = pageChange;
                vm.sorting = sorting;
                vm.editDoc = editDoc;
                vm.SortColumns = SortColumns;
                vm.resetSearch = resetSearch;
                vm.getDocumentList = getDocumentList;
                vm.upload = upload;
                vm.deleteDoc = deleteDoc;
                vm.download = download;
                vm.refresh = refresh;
                vm.searchOptions = [];

                function prepareSearchOptions() {
                    vm.searchOptions.push({ display: 'Yes', value: 'true' },
                        { display: 'No', value: 'false' });
                }

                function getDocumentList() {
                    if (!vm.isSuperAdmin)
                        vm.query.franchiseeId = $rootScope.identity.organizationId;
                    if (vm.isFrontOffice)
                        vm.query.franchiseeId = $rootScope.identity.loggedInOrganizationId;

                    return franchiseeDocumentService.getDocumentList(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.list = result.data.collection;
                            vm.count = result.data.pagingModel.totalRecords;
                            vm.query.order = result.data.filter.order;
                        }
                    });
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                function download(fileId, fileName) {
                    return fileService.getExcel(fileId).then(function (result) {
                        fileService.downloadFile(result.data, fileName);
                    });
                }

                function upload() {
                    if (!vm.isSuperAdmin)
                        vm.query.franchiseeId = $rootScope.identity.organizationId;
                    else if (vm.isFrontOffice)
                        vm.query.franchiseeId = $rootScope.identity.loggedInOrganizationId;
                    else
                        vm.query.franchiseeId = null;
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/organizations/views/upload-document.client.view.html',
                        controller: 'UploadDocumentController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'md',
                        resolve: {
                            modalParam: function () {
                                return {
                                    FranchiseeId: vm.query.franchiseeId,
                                    CategoryId: DataHelper.DocumentType.NationalAccountDocuments,
                                    DocumentId: vm.docId
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        vm.getDocumentList();
                    }, function () {

                    });
                }

                function deleteDoc(id) {
                    notification.showConfirm("Are you sure about deleting the record?", "Delete Document", function () {
                        return franchiseeDocumentService.deleteDoc(id).then(function (result) {
                            if (result.data != true)
                                toaster.error(result.message.message);
                            else
                                toaster.show(result.message.message);
                            getDocumentList();
                        });
                    });
                }

                function resetSearch() {
                    vm.query.text = '';
                    vm.query.email = '';
                    vm.query.franchiseeId = '';
                    vm.searchOption = '';
                    vm.query.pageNumber = 1;
                    vm.query.isImportant = '';
                    vm.query.periodStartDate = null;
                    vm.query.periodEndDate = null;
                    vm.query.documentTypeId = 0,
                        $scope.$broadcast("reset-dates");
                    getDocumentList();
                }

                $scope.$on('clearDates', function (event) {
                    vm.query.periodStartDate = null;
                    vm.query.periodEndDate = null;
                    getDocumentList();
                });

                function pageChange() {
                    getDocumentList();
                };

                function refresh() {
                    getDocumentList();
                }

                function sorting(propName) {
                    vm.query.sort.propName = propName;
                    vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                    getDocumentList();
                };
                function editDoc(id) {
                    vm.docId = id;
                    upload();
                    vm.docId = 0;
                }
                function getdocumentType() {
                    return franchiseeDocumentService.getNationalDocumentType().then(function (result) {
                        vm.documentType = result.data;
                    });
                }

                function editFranchisee(franchiseeId) {
                    franchiseeId = URLAuthenticationServiceForEncryption.encrypt(franchiseeId.toString());
                    $state.go("core.layout.franchisee.edit", { id: franchiseeId });
                }

                $scope.$emit("update-title", "National Account Documents");

                $q.all([getDocumentList(), getFranchiseeCollection(), prepareSearchOptions(), getdocumentType()]);
            }]);
}());