(function () {
    'use strict';
    angular.module(UsersConfiguration.moduleName).controller("EditUserController",
        ["$state", "$stateParams", "$scope", "$q", "$rootScope", "UserService", "FranchiseeService", "AddressService",
            "Notification", "PhoneService", "Toaster", "ColorCodeService", "FileService", "SchedulerService", "$uibModal",
            "LocalStorageService", "URLAuthenticationServiceForEncryption",
            function ($state, $stateParams, $scope, $q, $rootScope, userService, franchiseeService, addressService,
                notification, phoneService, toaster, colorCodeService, fileService, schedulerService, $uibModal,
                LocalStorageService, uRLAuthenticationServiceForEncryption) {
                var vm = this;
                vm.userId = $stateParams.id != null ? $stateParams.id : 0;
                vm.franchiseeId = $stateParams.franchiseeId != null ? $stateParams.franchiseeId : 0;
                vm.isEdit = uRLAuthenticationServiceForEncryption.decrypt(vm.userId) > 0;
                vm.WasEquipment = false;
                vm.showUserName = true;
                vm.user = {};
                vm.roleHasSuperAdmin = false;
                vm.isProcessing = false;
                $scope.isOperationManager = false;
                $scope.isEquipment = false;
                vm.save = save;
                vm.isImageEmpty = true;
                vm.getRole = getRole;
                vm.cancel = cancel;
                vm.upload = upload;
                vm.delete = deleteImage;
                vm.DocumentType = DataHelper.DocumentType;
                vm.defaultFranchiseId = $rootScope.identity.organizationId;
                vm.defaultFranchise = $rootScope.identity.organizationName;
                vm.currentRole = $rootScope.identity.roleId;
                vm.setDefaultUserName = setDefaultUserName;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.isSales = $rootScope.identity.roleId == vm.Roles.SalesRep;
                vm.isTech = $rootScope.identity.roleId == vm.Roles.Technician;
                vm.clearPassword = clearPassword;
                vm.executiveRoleId = vm.Roles.FrontOfficeExecutive;
                vm.roleIds = [];
                vm.userEdit = [];
                vm.userEdit.userId = vm.userId;
                vm.userEdit.franchiseeId = vm.franchiseeId;
                vm.selectedRoles = [];
                vm.selectedRolesForAllRoles = [];
                vm.imagesrc = "/Content/images/no_image_thumb.gif";
                vm.info = {};
                vm.info.fileList = [];
                vm.info.fileUploadModel = [];
                vm.franchiseeIds = [];
                $scope.isInValidImage = false;
                $scope.isImage = false;
                $scope.isSelectAll = true;
                $scope.isImageChanged = false;
                vm.download = download;
                vm.uploadDoc = uploadDoc;
                vm.myFunc = myFunc;
                vm.isUploaded = false;
                vm.degree = 1;
                vm.index = 0;
                vm.rotate = rotate;
                vm.css = "";
                vm.user.isUserNameChanged = false;
                vm.index = 0;
                vm.fileName = "";
                vm.downloadUserImage = downloadUserImage;
                function myFunc() {
                    vm.showError = false;
                    vm.isUserNameChanged = true;
                }
                $scope.tempFile = {
                    attachTempImageFile: ""
                };
                $scope.file = {
                    attachedImageFile: ""
                };
                $scope.yourEvents = {
                    onInitDone: function (item) {
                    },
                    onItemSelect: function (item) {
                        var roleName = DataHelper.Role.getValue(item.id);
                        vm.selectedRolesForAllRoles.push(roleName);
                        if (roleName == 'Technician') {
                            vm.selectedRoles.push(roleName);
                            if (vm.user.personEditModel.Color == '' || vm.user.personEditModel.Color==null)
                            vm.user.personEditModel.Color = "#3126e5";
                            $scope.isOperationManager = true;
                        }
                        if (roleName == 'SalesRep') {
                            vm.selectedRoles.push(roleName);
                            if (vm.user.personEditModel.colorCodeSale == '' || vm.user.personEditModel.colorCodeSale == null)
                            vm.user.personEditModel.colorCodeSale = "#15f246";
                            $scope.isSalesPresentForColor = true;
                        }
                        if (roleName == 'Equipment') {
                            vm.selectedRoles.push(roleName);
                            $scope.isEquipment = true;
                            vm.showUserName = false;
                        }
                        if (vm.selectedRolesForAllRoles.length > 1) {
                            $scope.isEquipment = false;
                            vm.showUserName = true;
                        }
                        if (vm.selectedRolesForAllRoles.length == 1 && vm.selectedRolesForAllRoles[0] !== 'Equipment') {
                            $scope.isEquipment = false;
                        }
                        getRolesPermisssions(vm.roleIds);
                        angular.forEach(vm.roleIds, function (value) {
                            var roleName = DataHelper.Role.getValue(value.id);
                            if (roleName == "SuperAdmin" && vm.roleIds.length == 1)
                                vm.roleHasSuperAdmin = true;
                            else
                                vm.roleHasSuperAdmin = false;
                        });
                    },
                    onItemDeselect: function (item) {
                        var roleName = DataHelper.Role.getValue(item.id);
                        var indexOf = vm.selectedRolesForAllRoles.indexOf(roleName);
                        vm.selectedRolesForAllRoles.splice(indexOf, 1);
                        if (roleName == 'Technician') {
                            vm.selectedRoles.splice(roleName, 1);
                            if (vm.selectedRoles.length == 0) {
                                $scope.isOperationManager = false;
                            }
                        }
                        if (roleName == 'SalesRep') {
                            vm.selectedRoles.splice(roleName, 1);
                            if (vm.selectedRoles.length == 0) {
                                $scope.isSalesPresentForColor = false;
                            }
                        }
                        var alreadyPresentRoleName = (vm.selectedRolesForAllRoles[0]);
                        if (vm.selectedRolesForAllRoles.length == 1
                            && alreadyPresentRoleName == 'Equipment') {
                            $scope.isEquipment = true;
                            $scope.isOperationManager = false;
                            vm.showUserName = false;
                        }
                        if (vm.selectedRolesForAllRoles.length == 0) {
                            $scope.isEquipment = false;
                        }
                        if (vm.selectedRolesForAllRoles.length > 1) {
                            $scope.isEquipment = false;
                            vm.showUserName = true;
                        }
                        getRolesPermisssions(vm.roleIds);
                        angular.forEach(vm.roleIds, function (value) {
                            var roleName = DataHelper.Role.getValue(value.id);
                            if (roleName == "SuperAdmin" && vm.roleIds.length == 1)
                                vm.roleHasSuperAdmin = true;
                            else
                                vm.roleHasSuperAdmin = false;
                        });
                    },
                    onSelectAll: function (item) {
                        $scope.isOperationManager = true;
                        $scope.isSalesPresentForColor = true;
                        vm.selectedRoles = [];
                        vm.selectedRoles.push('SalesRep');
                        vm.selectedRoles.push('Technician');
                        $scope.isEquipment = false;
                        getRolesPermisssions();
                        angular.forEach(vm.roleIds, function (value) {
                            var roleName = DataHelper.Role.getValue(value.id);
                            if (roleName == "SuperAdmin" && vm.roleIds.length == 1)
                                vm.roleHasSuperAdmin = true;
                            else
                                vm.roleHasSuperAdmin = false;
                        });
                    },
                    onDeselectAll: function (item) {
                        $scope.isSalesPresentForColor = false;
                        $scope.isEquipment = false;
                        $scope.isOperationManager = false;
                        vm.selectedRoles = [];
                        vm.selectedRolesForAllRoles = [];
                        vm.roleHasSuperAdmin = false;
                    }
                };
                if (vm.currentRole == vm.Roles.FrontOfficeExecutive)
                    vm.defaultFranchiseId = $stateParams.franchiseeId != null ?
                        uRLAuthenticationServiceForEncryption.decrypt($stateParams.franchiseeId.toString()) : 0;
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
                    buttonDefaultText: "Select Role(s)",
                    dynamicButtonTextSuffix: 'Selected'
                };
                $scope.settingsFR = {
                    scrollable: true,
                    enableSearch: true,
                    selectedToTop: true,
                    displayProp: "display",
                    //selectionLimit: 5,
                    buttonClasses: 'btn btn-primary leader_btn',
                };
                $scope.translationTextsFR = {
                    checkAll: 'Select All',
                    uncheckAll: 'Deselect All',
                    selectGroup: 'Select All',
                    buttonDefaultText: "Select Franchisee(s)",
                    dynamicButtonTextSuffix: 'Selected'
                };
                $rootScope.$on('roleChanged', function (event, data) {
                    angular.forEach(data, function (value, key) {
                        var roleName = DataHelper.Role.getValue(value.id);
                        if (roleName == 'SalesRep' || roleName == 'Technician') {
                            $scope.isOperationManager = true;
                        }
                        else {
                            $scope.isOperationManager = false;
                        }
                    });
                })
                function save() {
                    var isEquipmentPresent = false;
                    vm.user.roleIds = [];
                    vm.user.organizationIds = [];
                    vm.user.isImageChanged = $scope.isImageChanged;
                    var roleIds = vm.roleIds;
                    //vm.user.organizationId = uRLAuthenticationServiceForEncryption.decrypt(vm.user.organizationId.toString());
                    if (vm.WasEquipment && !$scope.isEquipment && (vm.user.userLoginEditModel.password == undefined || vm.user.userLoginEditModel.password == '')) {
                        notification.showAlert("Please Enter Password");
                        return;
                    }
                    if (($scope.tempFile.attachTempImageFile.type != "image/png" && $scope.tempFile.attachTempImageFile.type != "image/jpg" && $scope.tempFile.attachTempImageFile.type != "image/jpeg"
                        && $scope.tempFile.attachTempImageFile.type != "image/psd") && $scope.isImage) {
                        notification.showAlert("Please Upload Correct Format Image");
                        return;
                    }
                    if (phoneService.checkNumberTypeCombination(vm.user.personEditModel.phoneNumbers) == false && !$scope.isEquipment) {
                        notification.showAlert("Please Enter a valid Phone Number Type");
                        return;
                    }
                    if (colorCodeService.checkUserColorCode(vm.user.personEditModel.Color) == false && $scope.isOperationManager) {
                        notification.showAlert("Please Enter Correct Color Code For Technician");
                        return;
                    }
                    if (colorCodeService.checkUserColorCode(vm.user.personEditModel.colorCodeSale) == false && $scope.isSalesPresentForColor) {
                        notification.showAlert("Please Enter Correct Color Code For Sales");
                        return;
                    }
                    vm.user.personEditModel.phoneNumbers = phoneService.sanitizePhoneNumbers(vm.user.personEditModel.phoneNumbers);
                    if (phoneService.checkInvalidPhoneNo(vm.user.personEditModel.phoneNumbers) == false && !$scope.isEquipment) {
                        notification.showAlert("Please Enter a valid Phone Number ");
                        return;
                    }
                    vm.user.personEditModel.address = addressService.sanitizeAddress(vm.user.personEditModel.address);
                    if (addressService.checkAddressInComplete(vm.user.personEditModel.address)) {
                        notification.showAlert("Please fill complete address");
                        return;
                    }
                    if (vm.user.roleId == vm.Roles.FranchiseeAdmin) {
                        if (vm.user.organizationId <= 0) {
                            notification.showAlert("Please select a Franchisee");
                            return;
                        }
                    }
                    if (vm.currentRole == vm.Roles.FranchiseeAdmin || vm.currentRole == vm.Roles.FrontOfficeExecutive) {
                        vm.user.organizationId = vm.defaultFranchiseId;
                    }
                    if (vm.user.isExecutive) {
                        if (vm.franchiseeIds == null || vm.franchiseeIds.length <= 0) {
                            notification.showAlert("Please select atLeast one Franchisee!");
                            return;
                        }
                        angular.forEach(vm.franchiseeIds, function (value) {
                            vm.user.organizationIds.push(value.id);
                        });
                        vm.user.roleIds.push(vm.executiveRoleId);
                    }
                    else {
                        if (vm.user.organizationId <= 0) {
                            notification.showAlert("Please select a Franchisee");
                            return;
                        }
                        if (vm.roleIds == null || vm.roleIds.length <= 0) {
                            notification.showAlert("Please select atLeast one Role!");
                            return;
                        }
                        angular.forEach(vm.roleIds, function (value) {
                            vm.user.roleIds.push(value.id);
                            if (value.id == vm.Roles.SuperAdmin) {
                                vm.user.organizationId = vm.defaultFranchiseId;
                            }
                            var roleName = DataHelper.Role.getValue(value.id);
                            if (roleName == 'Equipment') {
                                $scope.isEquipment = true;
                            }
                            else {
                                $scope.isOperationManager = false;
                            }
                        });
                        if (vm.roleIds.length > 1) $scope.isEquipment = false;
                        vm.user.organizationIds.push(vm.user.organizationId);
                    }
                    if (vm.user.organizationIds.length <= 0) {
                        notification.showAlert("Please select a Franchisee");
                        return;
                    }
                    vm.isProcessing = true;
                    vm.user.id = uRLAuthenticationServiceForEncryption.decrypt(vm.userId);
                    vm.user.fileUploadModel = vm.info;
                    if (!$scope.isEquipment) {
                        return userService.saveUser(vm.user, true).then(function (result) {
                            vm.isProcessing = false;
                            if (result.data) {
                                toaster.show(result.message.message);
                                if (vm.currentRole == vm.Roles.FrontOfficeExecutive)
                                    $state.go('core.layout.user.list', { franchiseeId: vm.defaultFranchiseId });
                                else
                                    $state.go('core.layout.user.list');
                            }
                            else {
                                if (result.message.message == 'UserName not Unique') {
                                    vm.franchiseeIds = vm.user.organizationId.toString();
                                    vm.showError = true;
                                }
                                else
                                    toaster.error(result.message.message);
                            }
                        }).catch(function (err) {
                            vm.isProcessing = false;
                        });
                    }
                    else {
                        return userService.saveUserForEquipment(vm.user, true).then(function (result) {
                            vm.isProcessing = false;
                            if (result.data) {
                                toaster.show(result.message.message);
                                if (vm.currentRole == vm.Roles.FrontOfficeExecutive)
                                    $state.go('core.layout.user.list', { franchiseeId: vm.defaultFranchiseId });
                                else
                                    $state.go('core.layout.user.list');
                            }
                        }).catch(function (err) {
                            vm.isProcessing = false;
                        });
                    }
                }
                function rotate() {
                    var splt = "";
                    var deg = 1;
                    var id = "logo-image";
                    if (vm.css == null) {
                        vm.css = "";
                    }
                    if ((vm.css != "") && vm.index != 0) {
                        splt = vm.css.split('(')[1].split(')')[0].replace("deg", "");
                        splt = parseInt(splt, 10);
                        vm.index = splt;
                    }
                    else {
                        if (vm.index == 0) {
                            splt = 0;
                        }
                    }
                    if ($scope.file.attachedImageFile != null) {
                        var id = "logo-image";
                        var myElem = document.getElementById(id);
                        vm.css = "";
                        var img;
                        splt = vm.index + 90;
                        vm.index = splt;
                        myElem.style.transform = "rotate(" + splt + "deg)";
                        vm.user.css = "rotate(" + splt + "deg)";
                    }
                }
                function getRole() {
                    var idModel = "";
                }
                function clearPassword() {
                    if (vm.user.userLoginEditModel.changePassword == false) {
                        vm.user.userLoginEditModel.password = null;
                        vm.user.userLoginEditModel.confirmPassword = null;
                    }
                }
                function deleteImage() {
                    vm.isImageEmpty = true;
                    vm.user.fileId = null;
                    var imgs = document.getElementsByTagName('img');
                    var src = imgs[2].getAttribute("src");
                    imgs[3].src = "/Content/images/no_image_thumb.gif";
                    $scope.isImageChanged = true;
                    var id = "logo-image";
                    $scope.file.attachedImageFile = ""
                    var myElem = document.getElementById(id);
                    myElem.style.transform = "rotate(" + -0 + "deg)";
                    vm.index = 0;
                    vm.css = "";
                    vm.user.css = "";
                }
                function upload() {
                    $('#file_input').click();
                }
                function cancel() {
                    if (vm.currentRole == vm.Roles.FrontOfficeExecutive)
                        $state.go('core.layout.user.list', { franchiseeId: vm.defaultFranchiseId });
                    else
                        $state.go('core.layout.user.list', { query: vm.query });
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                function getRoles() {
                    return userService.getRoles().then(function (result) {
                        vm.roles = result.data;
                    });
                }

                function getUsers() {
                    var idDecrypt = uRLAuthenticationServiceForEncryption.decrypt(vm.userId);
                    var franchiseeId = uRLAuthenticationServiceForEncryption.decrypt(vm.franchiseeId);
                    if (idDecrypt == "" || franchiseeId == "") {
                        $state.go("core.layout.pageNotFound");
                        return;
                    }
                    else {
                        return userService.getUserById(vm.userId, vm.franchiseeId).then(function (result) {
                            var imageCss = "";
                            if (result == undefined || result.data == null) {
                                $state.go("core.layout.pageNotFound");
                            }
                            else {
                                if (result.data.personEditModel.fileName != null) {
                                    if (result.data.personEditModel.fileName != "") {
                                        vm.fileName = result.data.personEditModel.fileName;
                                        fileService.getFileStreamByUrl(result.data.personEditModel.fileName).then(function (result) {
                                            $scope.imageUrl = fileService.getStreamUrl(result);
                                            vm.imagesrc = $scope.imageUrl;
                                            vm.isImageEmpty = false;
                                            vm.index = 1;
                                        })
                                    }
                                }
                                angular.forEach(result.data.roleIds, function (value, key) {
                                    var roleName = DataHelper.Role.getValue(value);
                                    vm.selectedRolesForAllRoles.push(roleName);
                                    if (roleName == "Technician") {
                                        $scope.isOperationManager = true;
                                        //vm.isUserSales = true;
                                        vm.showUserName = true;
                                    }
                                    if (roleName == "SalesRep") {
                                        $scope.isSalesPresentForColor = true;
                                        vm.isUserSales = true;
                                        vm.showUserName = true;
                                    }
                                    if (roleName == "Equipment" && result.data.roleIds.length == 1) {
                                        $scope.isEquipment = true;
                                        vm.WasEquipment = true;
                                        vm.showUserName = false;
                                    }
                                })
                                if ((result.data.css != "" || result.data.css != null) && !$scope.isEquipment) {
                                    var id = "logo-image";
                                    var myElem = document.getElementById(id);
                                    myElem.style.transform = result.data.css;
                                    vm.css = result.data.css;
                                }
                                if (result != null && result.data != null) {
                                    vm.user = result.data;
                                    if (result.data.franchiseeDocument != null) {
                                        vm.isUploaded = true;
                                    }
                                    else {
                                        vm.isUploaded = false;
                                    }

                                    if (vm.isUploaded) {
                                        vm.fileId = result.data.franchiseeDocument.fileId;
                                        vm.fileName = result.data.franchiseeDocument.fileName;
                                        vm.documentId = result.data.franchiseeDocument.documentId;
                                    }

                                    vm.user.roleId = vm.user.roleId.toString();
                                    vm.user.organizationId = vm.user.organizationId.toString();
                                    if (vm.user.personEditModel.color != null)
                                        vm.user.personEditModel.Color = vm.user.personEditModel.color.toString();
                                    //console.log(vm.user.personEditModel.color.toString());
                                    // fill roleIds
                                    vm.user.rolePermissions = [];
                                    if (vm.user.roleIds != null && vm.user.roleIds.length > 0) {
                                        angular.forEach(vm.user.roleIds, function (value) {
                                            vm.roleIds.push({ id: value });
                                            var roleName = DataHelper.Role.getValue(value);
                                            if (roleName == "Technician") {
                                                vm.user.rolePermissions.push("Technician can create/update personal events like leave, holiday on scheduler. He can also create/update meeting with any other user. Technician can upload before/after, building exterior images and manage invoices.");
                                            }
                                            if (roleName == "SalesRep") {
                                                vm.user.rolePermissions.push("SalesRep can see the scheduler of all the techincians of the same franchisee.Also, he can create / update personal events like leave, holiday on scheduler.He can also create / update meeting with any other user.SalesRep can upload before / after, building exterior images and manage invoices.Can view Home Advisor and Back Up Call Report.");
                                            }
                                            if (roleName == "OperationsManager") {
                                                vm.user.rolePermissions.push("Operations Manager can see the scheduler of all the techincians of the same franchisee. Also, he can create / update personal events like leave, holiday on scheduler. He can also create / update meeting with any other user. He can also create / update estimate and job. Operations Manager can upload before / after, building exterior images and manage invoices.");
                                            }
                                            if (roleName == "FranchiseeAdmin") {
                                                vm.user.rolePermissions.push("Franchisee Admin can manage all the users of the franchisee. Can Manage price / time estimates. Can manage all the sales of the franchisee. View all the reports in the application. Have create / update permissions of the scheduler.");
                                            }

                                        });
                                    }
                                    // fill orgIds
                                    if (vm.user.organizationIds != null && vm.user.organizationIds.length > 0) {
                                        angular.forEach(vm.user.organizationIds, function (value) {
                                            vm.franchiseeIds.push({ id: value });
                                        });
                                    }

                                    //convert to string
                                    if (vm.user.personEditModel != null && vm.user.personEditModel.phoneNumbers != null) {
                                        angular.forEach(vm.user.personEditModel.phoneNumbers, function (phone) {
                                            if (phone.phoneType != null)
                                                phone.phoneType = phone.phoneType.toString();
                                        });
                                    }
                                }
                            }
                        });

                        if (!$scope.isEquipment) {
                            var id = "logo-image";
                            var myElem = document.getElementById(id);
                            myElem.style.transform = "rotate(" + -0 + "deg)";
                        }
                    }
                    
                }
                $scope.$watch('vm.roleIds', function (newValue, oldValue) {
                    var a = "";
                });

                function setDefaultUserName(userName) {
                    if (!vm.isEdit) {
                        vm.user.userLoginEditModel.userName = vm.user.personEditModel.email;
                    }
                }
                function download() {
                    return fileService.getExcel(vm.fileId).then(function (result) {
                        fileService.downloadFile(result.data, vm.fileName);
                    });
                }

                function downloadUserImage(fileId) {
                    return fileService.getFileForDownload(fileId).then(function (result) {
                        fileService.downloadFileImage(result.data, vm.fileName);
                    });
                }

                function uploadDoc() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/organizations/views/upload-franchisee-document.client.view.html',
                        controller: 'UploadFranchiseeDocumentController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'md',
                        resolve: {
                            modalParam: function () {
                                return {
                                    FranchiseeId: vm.user.organizationId,
                                    CategoryId: null,
                                    DocumentId: vm.documentId,
                                    UploadFrom: 1,
                                    UserId: vm.userId,
                                    isFromUser: true
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        getUsers();
                    }, function () {

                    });
                }
                $scope.readUrl = fileService.readLocalFile;

                $scope.$watch('tempFile.attachTempImageFile', function (newValue, oldValue) {
                    $scope.file.attachedImageFile = $scope.tempFile.attachTempImageFile;
                    if (newValue != null && newValue && newValue != "") {
                        if ($scope.tempFile.attachTempImageFile.type == "image/png" || $scope.tempFile.attachTempImageFile.type == "image/jpg"
                            || $scope.tempFile.attachTempImageFile.type == "image/jpeg" || $scope.tempFile.attachTempImageFile.type == "image/psd"
                            || $scope.tempFile.attachTempImageFile.type != undefined) {
                            $scope.isImage = true;
                            var image = document.getElementById('logo-image');
                            image.style.transform = "rotate(" + -0 + "deg)";
                            vm.index = 0;
                            vm.css = "";
                            vm.user.css = "";
                            fileService.uploadFile($scope.file.attachedImageFile).then(function (result) {
                                var width = image.naturalWidth;
                                vm.isImageEmpty = false;
                                var height = image.naturalHeight;
                                $scope.isImageChanged = true;
                                vm.info.fileList.push(result.data);
                                $scope.isInValidImage = false;
                            });
                        }
                        else {
                            vm.imagesrc = "/Content/images/no_image_thumb.gif";
                        }
                    }
                });
                $scope.$watch('vm.user.isExecutive', function (newValue, oldValue) {
                    if (newValue) {
                        vm.roleIds = [];
                        vm.selectedRolesForAllRoles = [];
                        $scope.isEquipment = false;
                    }
                    else {

                    }
                });


                function getRolesPermisssions(selectedRoles) {
                    vm.user.rolePermissions = [];
                    var roleNames = [];
                    if (selectedRoles == undefined) {
                        angular.forEach(vm.roles, function (item) {
                            var roleName = DataHelper.Role.getValue(parseInt(item.value));
                            roleNames.push(roleName);
                        });
                    }
                    else {
                        angular.forEach(selectedRoles, function (value) {
                            var roleName = DataHelper.Role.getValue(value.id);
                            roleNames.push(roleName);
                        });
                    }
                    angular.forEach(roleNames, function (value) {
                        if (value == "Technician") {
                            vm.user.rolePermissions.push("Technician can create/update personal events like leave, holiday on scheduler. He can also create/update meeting with any other user. Technician can upload before/after, building exterior images and manage invoices.");
                        }
                        if (value == "SalesRep") {
                            vm.user.rolePermissions.push("SalesRep can see the scheduler of all the techincians of the same franchisee.Also, he can create / update personal events like leave, holiday on scheduler.He can also create / update meeting with any other user.SalesRep can upload before / after, building exterior images and manage invoices.Can view Home Advisor and Back Up Call Report.");
                        }
                        if (value == "OperationsManager") {
                            vm.user.rolePermissions.push("Operations Manager can see the scheduler of all the techincians of the same franchisee. Also, he can create / update personal events like leave, holiday on scheduler. He can also create / update meeting with any other user. He can also create / update estimate and job. Operations Manager can upload before / after, building exterior images and manage invoices.");
                        }
                        if (value == "FranchiseeAdmin") {
                            vm.user.rolePermissions.push("Franchisee Admin can manage all the users of the franchisee. Can Manage price / time estimates. Can manage all the sales of the franchisee. View all the reports in the application. Has create / update permissions of the scheduler.");
                        }
                    });
                }


                function init() {
                    $q.all([getRoles(), getFranchiseeCollection(), getUsers()]);
                }
                $scope.$emit("update-title", "EDIT USER");

                init();
            }]);
}());