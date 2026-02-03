(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("UploadFranchiseeDocumentController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "FranchiseeService", "FileService", "Toaster", "FranchiseeDocumentService",
            "Clock", "$q", '$filter',
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, franchiseeService, fileService, toaster, franchiseeDocumentService,
                clock, $q, $filter) {

                var vm = this;
                vm.isOtherDocument = false;
                vm.isFromEdit = false;
                vm.label = 'Upload For';
                vm.perpetuity = false;
                vm.info = {};
                vm.info.id = 0;
                vm.info.isRejected = false;
                vm.franchiseeIds = [];
                vm.isExpiry = {};
                vm.save = save;
                vm.currentDate = moment(clock.now());
                vm.Roles = DataHelper.Role;
                vm.DocumentName = DataHelper.DocumentName;

                vm.yearCollection = [];
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
                vm.isFrontOffice = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
                vm.categoryId = modalParam.CategoryId;
                vm.info.id = modalParam.DocumentId != null ? modalParam.DocumentId : 0;
                vm.uploadFrom = modalParam.UploadFrom != null ? modalParam.UploadFrom : 0;
                vm.userId = modalParam.UserId != null ? modalParam.UserId : 0;
                vm.franchiseeIdFromEdit = modalParam.FranchiseeId != null ? modalParam.FranchiseeId : 0;
                vm.getFranchiseeDocument = getFranchiseeDocument;
                vm.info.isFromUser = modalParam.isFromUser != null || modalParam.isFromUser != undefined ? modalParam.isFromUser : false;
                vm.changeDocument = changeDocument;
                vm.isEdit = false;
                vm.isNCA = false;
                vm.isOthers = false;
                vm.IsFranchiseeSelected = false;
                vm.perpetuity = false;
                var currentDate = moment(clock.now());

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
                function changeDocument(documentId) {
                    vm.perpetuity = false;
                    if (documentId == vm.DocumentName.NCA) {
                        vm.isNCA = true;
                        vm.info.showToUser = true;
                        vm.info.expiryDate = null;
                        getFranchiseeUserCollection(vm.franchiseeId);
                        vm.info.isRejected = false;
                    }
                    if (documentId == vm.DocumentName.RESALECERTIFICATE) {
                        var reSalesCertificate = $filter('filter')(vm.documentType, { value: documentId }, true);
                        if (reSalesCertificate.length > 0)
                            vm.perpetuity = reSalesCertificate[0].isPerpetuity;
                    }
                    else if (documentId == vm.DocumentName.Others) {
                        vm.isOthers = true;
                        vm.isNCA = false;
                        vm.info.expiryDate = null;
                        vm.info.userId = null;
                        vm.info.showToUser = false;
                        getFranchiseeUserCollection(vm.franchiseeId);
                        vm.info.isRejected = false;
                    }
                    else if (documentId != vm.DocumentName.NCA && documentId != vm.DocumentName.Others) {
                        vm.isNCA = false;
                        vm.isOthers = false;
                        vm.info.showToUser = false;
                        vm.info.userId = null;
                        vm.info.isRejected = false;
                    }
                    if (documentId != vm.DocumentName.RESALECERTIFICATE) {
                        vm.info.isRejected = false;
                    }
                }
                function getFranchiseeDocument(franchiseeId) {
                    if (!vm.isFromEdit) {
                        vm.isFromEdit = false;
                        vm.info.isRejected = false;
                        vm.perpetuity = false;
                    }
                    vm.IsFranchiseeSelected = true;
                    vm.franchiseeId = franchiseeId;
                    vm.info.documentTypeId = null;

                    vm.info.showToUser = false;
                    vm.info.isImportant = false;

                    if (!vm.isFromEdit) {
                        vm.info.uploadFor = null;
                        vm.info.fileModel = null;
                        vm.info.expiryDate = null;
                    }
                    getdocumentTypeForFranchisee(franchiseeId);

                }

                vm.uploadFile = function (file) {
                    if (vm.info.id != 0 && !vm.uploadFrom) {
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
                        if (vm.info.id != 0 || vm.uploadFrom) {
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

                function getFranchiseeUserCollection(franchiseeId) {

                    return franchiseeService.getFranchiseeUserCollection(franchiseeId).then(function (result) {
                        vm.userCollection = result.data;
                        if (vm.info.id != 0) {
                            var user = $filter('filter')(vm.userCollection, { id: vm.documentInfo.userId }, true);
                            if (user.length > 0) {
                                vm.info.userId = user[0].id;
                            }
                        }
                        else if (vm.info.id == 0 && vm.uploadFrom) {
                            var userId = parseInt(vm.userId, 10)
                            var user = $filter('filter')(vm.userCollection, { id: userId }, true);
                            if (user.length > 0) {
                                vm.info.userId = user[0].id;
                            }
                        }
                    });
                }
                function getdocumentTypeForFranchisee(franchiseeId) {
                    return franchiseeDocumentService.getDocumentTypeForFranchisee(franchiseeId).then(function (result) {
                        vm.documentType = result.data;
                        if (vm.info.id != 0) {
                            var document = $filter('filter')(vm.documentType, { value: vm.documentInfo.documentTypeId.toString() }, true);
                            if (document.length > 0) {
                                vm.info.documentTypeId = document[0].value.toString();
                            }
                            if (vm.documentInfo.documentTypeId == vm.DocumentName.NCA) {
                                vm.isNCA = true;
                                getFranchiseeUserCollection(vm.documentInfo.franchiseeId);
                            }
                            if (vm.documentInfo.documentTypeId == vm.DocumentName.Others) {
                                vm.isOthers = true;
                                getFranchiseeUserCollection(vm.documentInfo.franchiseeId);
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
                                if (document.length > 0) {
                                    vm.info.documentTypeId = document[0].value.toString();
                                }
                            }
                            else if (vm.info.id == 0 && vm.uploadFrom) {
                                var document = $filter('filter')(vm.documentType, { value: vm.DocumentName.NCA.toString() }, true);
                                if (document.length > 0) {
                                    vm.info.documentTypeId = document[0].value.toString();
                                }
                            }
                            //vm.info.isRejected = false;
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
                    vm.info.franchiseeIds = [];
                    var expirydate = moment(vm.info.expiryDate)
                    if (!vm.isSuperAdmin && !vm.isFrontOffice) {
                        vm.info.franchiseeIds.push($rootScope.identity.organizationId.toString());
                    }
                    if (vm.isFrontOffice) {
                        //vm.info.franchiseeIds.push($rootScope.identity.loggedInOrganizationId.toString());
                        vm.info.franchiseeIds.push(vm.franchiseeId);
                    }
                    if (vm.isSuperAdmin) {
                        vm.info.franchiseeIds.push(vm.franchiseeId);
                    }
                    if (vm.info.expiryDate == null && (!vm.isNCA && !vm.isOthers) && !vm.perpetuity && !vm.info.isRejected) {
                        if (vm.DocumentName.LICENSE == vm.info.documentTypeId || vm.DocumentName.RESALECERTIFICATE == vm.info.documentTypeId
                            || vm.DocumentName.COI == vm.info.documentTypeId || vm.DocumentName.W9 == vm.info.documentTypeId || vm.DocumentName.FranchiseeContract == vm.info.documentTypeId
                            || vm.DocumentName.FRANCHISEAGREEMENTSRENEWALS == vm.info.documentTypeId) {
                            toaster.error("Select Expiry Date!");
                            return;
                        }
                        else {
                            vm.isOtherDocument = true;
                        }
                    }

                     
                    if ((vm.info.uploadFor == null || vm.info.uploadFor == '' || vm.info.uploadFor == undefined) && !vm.perpetuity && vm.info.isRejected) {
                        toaster.error("Select Upload For!");
                        return;
                    }

                    if (vm.info.userId == null && vm.isNCA) {
                        toaster.error("Please select a User!");
                        return;
                    }
                    if (vm.info.franchiseeIds == null || vm.info.franchiseeIds <= 0) {
                        toaster.error("Please select a Franchisee!");
                        return;
                    }

                    if (vm.info.fileModel == null && vm.info.id == 0 && !vm.info.isRejected) {
                        toaster.error("Please upload a file");
                        return;
                    }

                    if (vm.info.documentTypeId == null) {
                        toaster.error("Select Document Name!");
                        return;
                    }

                    if ((vm.info.expiryDate != null || vm.isNCA || vm.isOthers || vm.perpetuity || vm.info.isRejected || vm.isOtherDocument)) {
                        vm.isExpiry.franchiseeId = vm.franchiseeId;
                        vm.isExpiry.documentTypeId = vm.info.documentTypeId;
                        vm.isExpiry.expiryDate = vm.info.expiryDate;
                        vm.isExpiry.userId = vm.info.userId;
                        saveValue();
                    }
                }


                function saveValue() {
                    if (vm.franchiseeIdFromEdit != 0) {
                        vm.info.franchiseeIds[0] = parseInt(vm.franchiseeIdFromEdit);
                    }
                    if (vm.isFrontOffice) {
                        vm.info.franchiseeIds[0] = parseInt(vm.franchiseeId);
                    }
                    vm.info.isPerpetuity = vm.perpetuity;
                    if (vm.info.isRejected) {
                        vm.info.expiryDate = null;
                        vm.info.showToUser = false;
                        vm.info.isImportant = false;
                        vm.info.fileModel = null;
                    }
                    vm.isFromEdit = false;
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
                    if (vm.categoryId == null) {
                        if (id == 0) {
                            var franchisee = $filter('filter')(vm.franchiseeCollection, { value: vm.franchiseeIdFromEdit }, true);
                            if (franchisee.length > 0) {
                                vm.franchiseeIds = franchisee[0].id.toString();
                            }
                            vm.IsFranchiseeSelected = true;
                            vm.isNCA = true;
                            getdocumentType();
                            getFranchiseeUserCollection(vm.franchiseeIds);
                        }
                        else {
                            return franchiseeDocumentService.editDoc(id).then(function (result) {
                                vm.documentInfo = result.data;
                                vm.info.isImportant = vm.documentInfo.isImportant;
                                vm.info.showToUser = vm.documentInfo.showToUser;
                                vm.info.isRejected = vm.documentInfo.isRejected;
                                vm.perpetuity = vm.documentInfo.isPerpetuity;
                                if (vm.documentInfo.expiryDate != null)
                                    vm.info.expiryDate = new Date(vm.documentInfo.expiryDate);
                                var franchisee = $filter('filter')(vm.franchiseeCollection, { id: vm.documentInfo.franchiseeId }, true);
                                if (franchisee.length > 0) {
                                    vm.franchiseeIds = franchisee[0].id.toString();
                                }
                                vm.info.uploadFor = vm.documentInfo.uploadFor;
                                vm.info.fileModel = vm.documentInfo.fileModel;
                                vm.isFromEdit = true;
                                getFranchiseeDocument(vm.franchiseeIds);

                            });
                        }
                    }
                    else {
                        return franchiseeDocumentService.getNationalDocumentType().then(function (result) {
                            vm.documentType = result.data;
                        });
                    }
                }


                function fillCollection() {
                    vm.yearCollection = [];
                    var index = 0;
                    var year = new Date().getFullYear() + 1;
                    for (var i = year; i >= 1989; i--) {
                        vm.yearCollection.push({ alias: '' + i, display: '' + i, id: '' + index, value: '' + i });
                        ++index;
                    }
                }

                if (vm.isSuperAdmin) {
                    $q.all([getFranchiseeCollection(), fillCollection()]);
                }
                if (!vm.isSuperAdmin) {
                    $q.all([getFranchiseeCollection(), fillCollection()]);
                }

            }]);
}());