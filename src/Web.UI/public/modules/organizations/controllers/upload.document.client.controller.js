(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("UploadDocumentController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "FranchiseeService", "FileService", "Toaster", "FranchiseeDocumentService",
            "Clock", "$q", '$filter',
        function ($scope, $rootScope, $state, $uibModalInstance, modalParam, franchiseeService, fileService, toaster, franchiseeDocumentService,
            clock, $q, $filter) {

            var vm = this;
            vm.info = {};
            vm.franchiseeIds = [];
            vm.save = save;
            vm.currentDate = moment(clock.now());
            vm.Roles = DataHelper.Role;
            vm.DocumentType = DataHelper.DocumentName;
            vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
            vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
            vm.isFrontOffice = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
            vm.categoryId = modalParam.CategoryId;
            vm.getFranchiseeDocument = getFranchiseeDocument;
            vm.info.id = modalParam.DocumentId != null ? modalParam.DocumentId : 0;
            vm.IsFranchiseeSelected = false;
            var currentDate = moment(clock.now());
            vm.isEdit = false;
            if (vm.isSuperAdmin) {
                vm.franchiseeId = $rootScope.identity.organizationId;
            }
            if (!vm.isSuperAdmin) {
                vm.IsFranchiseeSelected = true;
                vm.franchiseeId = $rootScope.identity.organizationId;
            }
            if (vm.isFrontOffice) {
                vm.franchiseeId = $rootScope.identity.loggedInOrganizationId;
            }
            function getFranchiseeDocument(franchiseeId) {
                vm.IsFranchiseeSelected = true;
                getdocumentTypeForFranchisee(franchiseeId);
            }
            function getdocumentTypeForFranchisee(franchiseeId) {
                return franchiseeDocumentService.GetNationalTypeForFranchisee(franchiseeId).then(function (result) {
                    vm.documentType = result.data;
                    if (vm.info.id != 0) {
                        var document = $filter('filter')(vm.documentType, { value: vm.documentInfo.documentTypeId.toString() }, true);
                        if (document[0] != undefined) {
                            vm.info.documentTypeId = document[0].value.toString();
                        }
                    }
                });
            }
            $scope.settings = {
                scrollable: true,
                enableSearch: true,
                selectedToTop: true,
                displayProp: "display",
                buttonClasses: 'btn btn-primary leader_btn',
            };
            $scope.translationTexts = {
                checkAll: 'Select All',
                uncheckAll: 'Deselect All',
                selectGroup: 'Select All',
                buttonDefaultText: "Select Franchisee(s)",
                dynamicButtonTextSuffix: 'Selected'
            };

            vm.uploadFile = function (file) {
                if (vm.info.id != 0) {
                    toaster.error("File Cannot be Uploaded.");

                }
                else {
                    if (file != null) {
                        return fileService.upload(file).then(function (result) {
                            toaster.show("File uploaded.");
                            vm.info.fileModel = result.data;
                        });
                    }
                }
            }

            function getFranchiseeCollection() {
                return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                    vm.franchiseeCollection = result.data;
                    if (vm.info.id != 0) {
                        vm.isEdit = true;
                        getdocumentInfo(vm.info.id);
                    }
                    else {
                        if (!vm.isSuperAdmin) {
                            vm.isEdit = false;
                            getdocumentTypeForFranchisee(vm.franchiseeId);
                        }
                        else {
                            vm.isEdit = false;
                            getdocumentType();
                        }
                    }
                });
            }

            function getdocumentType() {
                if (vm.categoryId == null) {
                    return franchiseeDocumentService.getDocumentType().then(function (result) {
                        vm.documentType = result.data;
                        if (vm.info.id != 0) {
                            var document = $filter('filter')(vm.documentType, { value: vm.documentInfo.documentTypeId }, true);
                            vm.info.documentTypeId = document[0].value.toString();
                        }
                    });
                }
                else {
                    return franchiseeDocumentService.getNationalDocumentType().then(function (result) {
                        vm.documentType = result.data;
                    });
                }
            }

            function save() {
                var currentDateToCompare = moment(currentDate).format('MM/DD/YYYY');
                var ExpiryDateToCompare = moment(vm.info.expiryDate).format('MM/DD/YYYY');
                var currentDateToCompareDateTime = new Date(currentDateToCompare);
                var ExpiryDateToCompareDateTime = new Date(ExpiryDateToCompare);
                //if (ExpiryDateToCompareDateTime < currentDateToCompareDateTime)
                //{
                //    toaster.error("Expiry Date should not be of past Time!");
                //    return;
                //}
                vm.info.franchiseeIds = [];
                if (!vm.isSuperAdmin && !vm.isFrontOffice) {
                    vm.info.franchiseeIds.push($rootScope.identity.organizationId.toString());
                }
                if (vm.isFrontOffice) {
                    vm.info.franchiseeIds.push($rootScope.identity.loggedInOrganizationId.toString());
                }
                if (vm.isSuperAdmin) {
                    vm.info.franchiseeIds.push(vm.franchiseeIds);
                }
                if (vm.info.franchiseeIds == null || vm.info.franchiseeIds <= 0) {
                    toaster.error("Please select a Franchisee!");
                    return;
                }

                if (vm.info.fileModel == null && vm.info.id == 0) {
                    toaster.error("Please upload a file");
                    return;
                }

                if (vm.info.documentTypeId == null) {

                    toaster.error("Select Document Name!");
                    return;
                }

                vm.isProcessing = true;
                return franchiseeDocumentService.saveDocuments(vm.info).then(function (result) {
                    if (result.data != null) {
                        if (result.data) {
                            toaster.show(result.message.message);
                            $uibModalInstance.close();
                        }
                        else
                            toaster.error(result.message.message);
                        vm.isProcessing = false;
                    }
                }).catch(function (err) {
                    vm.isProcessing = false;
                });
            }

            vm.close = function () {
                $uibModalInstance.dismiss();
            };
            function getdocumentInfo(id) {
                return franchiseeDocumentService.editDoc(id).then(function (result) {
                    vm.documentInfo = result.data;
                    vm.info.isImportant = vm.documentInfo.isImportant;
                    vm.info.showToUser = vm.documentInfo.showToUser;
                    vm.info.fileModel = vm.documentInfo.fileModel;
                    if (vm.documentInfo.expiryDate != null)
                        vm.info.expiryDate = new Date(vm.documentInfo.expiryDate);
                    var franchisee = $filter('filter')(vm.franchiseeCollection, { id: vm.documentInfo.franchiseeId }, true);
                    vm.franchiseeIds = franchisee[0].id.toString();
                    getFranchiseeDocument(vm.franchiseeIds);

                });
            }
            if (vm.isSuperAdmin) {
                $q.all([getFranchiseeCollection()]);
            }
            if (!vm.isSuperAdmin) {
                $q.all([getFranchiseeCollection()]);
            }
        }]);
}());