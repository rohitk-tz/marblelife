(function () {
    'use strict';

    angular.module(UsersConfiguration.moduleName).controller("CreateUserController",

        ["$state", "$stateParams", "$scope", "$q", "$rootScope", "UserService", "FranchiseeService", "AddressService",
            "Notification", "PhoneService", "Toaster", "ColorCodeService", "FileService", "SchedulerService",
            "URLAuthenticationServiceForEncryption",
            function ($state, $stateParams, $scope, $q, $rootScope, userService, franchiseeService, addressService,
                notification, phoneService, toaster, colorCodeService, fileService, schedulerService, uRLAuthenticationServiceForEncryption) {
                var vm = this
                vm.userId = $stateParams.id != null ? $stateParams.id : 0;
                vm.franchiseeId = 0;
                vm.isEdit = vm.userId > 0;
                vm.user = {};
                vm.user.filename = "";
                vm.isProcessing = false;
                $scope.isOperationManager = false;
                $scope.isSalesPresentForColor = false;
                vm.selectedRolesForAllRoles = [];
                vm.roleHasSuperAdmin = false;
                $scope.isEquipment = false;
                $scope.fileName = "";
                vm.save = save;
                vm.cancel = cancel;
                vm.defaultFranchiseId = $rootScope.identity.organizationId;
                vm.currentRole = $rootScope.identity.roleId;
                vm.defaultFranchise = $rootScope.identity.organizationName;
                vm.setDefaultUserName = setDefaultUserName;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.executiveRoleId = vm.Roles.FrontOfficeExecutive;
                vm.imagesrc = "/Content/images/no_image_thumb.gif";
                vm.imagesrcs = ""
                vm.user.fileModel = "";
                vm.user.mimeType = "";
                vm.user.size = "";
                vm.info = {};
                vm.selectedRoles = [];
                vm.user.info = {};
                vm.info.fileList = [];
                vm.info.fileUploadModel = [];
                vm.roleIds = [];
                vm.myFunc = myFunc;
                vm.franchiseeIds = [];
                vm.showError = false;
                $scope.isInValidImage = false;
                $scope.isImage = false;
                vm.upload = upload;
                vm.delete = deleteImage;
                vm.index = 0;
                vm.rotate = rotate;
                vm.deg = 1;
                vm.isImageEmpty = true;
                vm.user.rolePermissions = [];
                vm.selectedRoleIds = [];
                //if (vm.currentRole == vm.Roles.FrontOfficeExecutive)
                //{
                //    vm.defaultFranchiseId = $rootScope.identity.loggedIn;
                //}
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
                            $scope.isOperationManager = true;
                        }
                        if (roleName == 'SalesRep') {
                            vm.selectedRoles.push(roleName);
                            $scope.isSalesPresentForColor = true;
                        }
                        if (roleName == 'Equipment') {
                            vm.selectedRoles.push(roleName);
                            $scope.isEquipment = true;
                        }
                        if (vm.selectedRolesForAllRoles.length > 1) {
                            $scope.isEquipment = false;
                        }
                        var isTechPresent = vm.selectedRolesForAllRoles.lastIndexOf("Technician");
                        var isSalesPresent = vm.selectedRolesForAllRoles.lastIndexOf("SalesRep");
                        if (isTechPresent != -1) {
                            vm.user.personEditModel.Color = "#3126e5";
                        }
                        if (isSalesPresent != -1) {
                            vm.user.personEditModel.colorCodeSale = "#15f246";
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
                        var indexOfRoleId = vm.selectedRoleIds.indexOf(item.id);
                        vm.selectedRoleIds.splice(indexOfRoleId,1);
                        var indexOf = vm.selectedRolesForAllRoles.indexOf(roleName);
                        vm.selectedRolesForAllRoles.splice(indexOf, 1);
                        if (roleName == 'SalesRep' || roleName == 'Technician') {
                            vm.selectedRoles.splice(roleName, 1);
                            if (vm.selectedRoles.length == 0) {
                                $scope.isOperationManager = false;
                                $scope.isSalesPresentForColor = false;
                            }
                        }
                        if (roleName == 'Equipment') {
                            vm.selectedRoles.push(roleName);
                            $scope.isEquipment = false;
                        }
                        var alreadyPresentRoleName = (vm.selectedRolesForAllRoles[0]);
                        if (vm.selectedRolesForAllRoles.length == 1
                            && alreadyPresentRoleName == 'Equipment') {
                            $scope.isEquipment = true;
                            $scope.isOperationManager = false;
                            $scope.isSalesPresentForColor = false;
                        }
                        if (vm.selectedRolesForAllRoles.length == 0) {
                            $scope.isEquipment = false;
                        }
                        var isTechPresent = vm.selectedRolesForAllRoles.lastIndexOf("Technician");
                        var isSalesPresent = vm.selectedRolesForAllRoles.lastIndexOf("SalesRep");
                        if (isTechPresent != -1) {
                            vm.user.personEditModel.Color = "#3126e5";
                        }
                        else if (isSalesPresent != -1) {
                            vm.user.personEditModel.colorCodeSale = "#15f246";
                        }
                        if (isTechPresent == -1) {
                            vm.user.personEditModel.Color = "";
                            $scope.isOperationManager = false;
                        }
                        if (isSalesPresent == -1) {
                            vm.user.personEditModel.colorCodeSale = "";
                            $scope.isSalesPresentForColor = false;
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
                        var Ids = vm.roleIds;
                        $scope.isOperationManager = true;
                        $scope.isSalesPresentForColor = true;
                        vm.selectedRoles = [];
                        vm.selectedRoles.push('SalesRep');
                        vm.selectedRoles.push('Technician');
                        $scope.isEquipment = false;
                        var isTechPresent = vm.selectedRolesForAllRoles.lastIndexOf("Technician");
                        var isSalesPresent = vm.selectedRolesForAllRoles.lastIndexOf("SalesRep");
                        if (isTechPresent != -1) {
                            vm.user.personEditModel.Color = "#3126e5";
                        }
                        else if (isSalesPresent != -1) {
                            vm.user.personEditModel.colorCodeSale = "#15f246";
                        }
                        //vm.selectedRoleIds = [];
                        //angular.forEach(vm.roles, function (item) {
                        //    vm.selectedRoleIds.push(item.value);
                        //});
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
                        vm.user.rolePermissions = [];
                        $scope.isSalesPresentForColor = false;
                        $scope.isOperationManager = false;
                        vm.selectedRoles = [];
                        $scope.isEquipment = false;
                        vm.user.personEditModel.Color = "";
                        vm.roleHasSuperAdmin = false;
                    }
                };
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
                function myFunc() {
                    vm.showError = false;
                }
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

                if (vm.currentRole == vm.Roles.FrontOfficeExecutive) {
                    vm.defaultFranchiseId = $stateParams.franchiseeId != null ?
                        uRLAuthenticationServiceForEncryption.decrypt($stateParams.franchiseeId.toString())
                        : $rootScope.identity.loggedInOrganizationId;
                    if (vm.defaultFranchiseId == 0) {
                        vm.defaultFranchiseId = $rootScope.identity.loggedInOrganizationId;
                    }
                }

                function deleteImage() {
                    vm.isImageEmpty = true;
                    var imgs = document.getElementsByTagName('img');
                    var src = imgs[2].getAttribute("src");
                    imgs[2].src = "/Content/images/no_image_thumb.gif";
                    //vm.imagesrc = "/Content/images/no_image_thumb.gif";
                    var id = "logo-image";
                    $scope.file.attachedImageFile = ""
                    var myElem = document.getElementById(id);
                    myElem.style.transform = "rotate(" + - 0 + "deg)";
                }

                function upload() {
                    $('#file_input').click();

                }

                function save() {

                    if (($scope.tempFile.attachTempImageFile.type != "image/png" && $scope.tempFile.attachTempImageFile.type != "image/jpg" && $scope.tempFile.attachTempImageFile.type != "image/jpeg" && $scope.tempFile.attachTempImageFile.type != "image/psd") && $scope.isImage) {
                        notification.showAlert("Please Upload Correct Format Image");
                        return;
                    }

                    vm.user.roleIds = [];
                    $scope.file.attachedImageFile
                    vm.user.organizationIds = [];
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
                    if (!$scope.isEquipment)
                        vm.user.personEditModel.phoneNumbers = phoneService.sanitizePhoneNumbers(vm.user.personEditModel.phoneNumbers);

                    if (!$scope.isEquipment && phoneService.checkInvalidPhoneNo(vm.user.personEditModel.phoneNumbers) == false) {
                        notification.showAlert("Please Enter a valid Phone Number ");
                        return;
                    }

                    vm.user.personEditModel.address = addressService.sanitizeAddress(vm.user.personEditModel.address);

                    if (addressService.checkAddressInComplete(vm.user.personEditModel.address)) {
                        notification.showAlert("Please fill complete address");
                        return;
                    }
                    if (vm.currentRole == vm.Roles.FranchiseeAdmin || vm.currentRole == vm.Roles.FrontOfficeExecutive)
                        vm.user.organizationId = vm.defaultFranchiseId;

                    if (vm.user.isExecutive) {
                        if (vm.franchiseeIds == null || vm.franchiseeIds.length <= 0) {
                            notification.showAlert("Please select atleast one Franchisee!");
                            return;
                        }
                        angular.forEach(vm.franchiseeIds, function (value) {
                            vm.user.organizationIds.push(value.id);
                        });
                        vm.user.roleIds.push(vm.executiveRoleId);
                    }
                    else {
                        if (vm.roleIds == null || vm.roleIds.length <= 0) {
                            notification.showAlert("Please select atLeast one Role!");
                            return;
                        }
                        angular.forEach(vm.roleIds, function (value) {
                            vm.user.roleIds.push(value.id);
                            if (value.id == vm.Roles.SuperAdmin) {
                                vm.user.organizationId = vm.defaultFranchiseId;
                            }
                        });
                        if (vm.user.organizationId <= 0) {
                            notification.showAlert("Please select a Franchisee");
                            return;
                        }
                        vm.user.organizationIds.push(vm.user.organizationId);
                    }

                    //if (vm.user.roleId != vm.Roles.FranchiseeAdmin) {
                    if (vm.user.organizationIds.length <= 0) {
                        notification.showAlert("Please select a Franchisee");
                        return;
                    }
                    //}

                    vm.isProcessing = true;
                    //SaveMedia();
                    if (vm.roleIds.length > 1 || !$scope.isEquipment) {
                        SaveValue();
                    }
                    else if (vm.roleIds.length == 1 && vm.roleIds[0].id == 7) {
                        SaveValueForEquipment();
                    }
                }
                function rotate() {

                    vm.index = vm.index + 1;
                    var id = "logo-image";
                    if ($scope.file.attachedImageFile != "") {
                        var myElem = document.getElementById(id);

                        if (vm.index === 1) {
                            vm.deg = 90;
                            myElem.style.transform = "rotate(-90deg)";
                            myElem.style.transform = "rotate(90deg)";

                        }

                        else if (vm.index === 2) {
                            vm.deg = 180;
                            myElem.style.transform = "rotate(-90deg)";
                            myElem.style.transform = "rotate(180deg)";
                        }

                        else if (vm.index === 3) {
                            vm.deg = 270;
                            myElem.style.transform = "rotate(-90deg)";
                            myElem.style.transform = "rotate(270deg)";
                        }

                        else if (vm.index === 4) {
                            vm.index = 0;
                            vm.deg = 360;
                            myElem.style.transform = "rotate(-90deg)";
                            myElem.style.transform = "rotate(360deg)";
                        }
                        else {
                            vm.index = 0;
                        }

                        vm.user.css = "rotate(" + vm.deg + "deg)";
                    }
                }
                function SaveValue() {
                    vm.user.fileUploadModel = vm.info;
                    return userService.saveUser(vm.user).then(function (result) {
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

                function cancel() {
                    if (vm.currentRole == vm.Roles.FrontOfficeExecutive)
                        $state.go('core.layout.user.list', { franchiseeId: vm.defaultFranchiseId });
                    else
                        $state.go('core.layout.user.list');
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getActiveFranchiseeList().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                function getRoles() {
                    return userService.getRoles().then(function (result) {
                        vm.roles = result.data;
                    });
                }

                function setDefaultUserName(userName) {
                    vm.user.userLoginEditModel.userName = vm.user.personEditModel.email;
                }

                function init() {
                    if (vm.FrontOfficeExecutive && $rootScope.identity.loggedInOrganizationId != null) {
                        vm.franchiseeId = uRLAuthenticationServiceForEncryption.decrypt($rootScope.identity.loggedInOrganizationId.toString());
                    }
                    if ($stateParams.franchiseeId != null && !vm.FrontOfficeExecutive) {
                        vm.franchiseeId = uRLAuthenticationServiceForEncryption.decrypt($stateParams.franchiseeId.toString());
                    }
                    if (!angular.isNumber(+vm.franchiseeId)) {
                        $state.go("core.layout.pageNotFound");
                        return;
                    }
                    else {
                        vm.userId = uRLAuthenticationServiceForEncryption.encrypt(vm.userId.toString());
                        vm.franchiseeId = uRLAuthenticationServiceForEncryption.encrypt(vm.franchiseeId.toString());
                        return userService.getUserById(vm.userId, vm.franchiseeId).then(function (result) {
                            if (result != null && result.data != null) {
                                vm.user = result.data;
                            }
                        });
                    }                    
                }

                $scope.downloadFile = function (fileId, filename, actualFileName) {
                    fileService.getFileStreamById(fileId).then(function (result) {
                        var fileExtension = actualFileName.split(".").pop();
                        fileService.downloadFile(result, filename + "." + fileExtension);
                    });
                };

                $scope.readUrl = fileService.readLocalFile;
                $scope.$watch('tempFile.attachTempImageFile', function (newValue, oldValue) {
                    $scope.file.attachedImageFile = $scope.tempFile.attachTempImageFile;
                    if ($scope.tempFile.attachTempImageFile.type == "image/png" || $scope.tempFile.attachTempImageFile.type == "image/jpg"
                        || $scope.tempFile.attachTempImageFile.type == "image/jpeg" || $scope.tempFile.attachTempImageFile.type == "image/psd"
                        || $scope.tempFile.attachTempImageFile.type != undefined) {
                        $scope.isImage = true;
                        var image = document.getElementById('logo-image');
                        image.style.transform = "rotate(" + -0 + "deg)";
                        fileService.uploadFile($scope.file.attachedImageFile).then(function (result) {
                            var width = image.naturalWidth;
                            var height = image.naturalHeight;
                            vm.isImageEmpty = false;
                            //if (height >= width) {
                            vm.info.fileList.push(result.data);
                            $scope.isInValidImage = false;

                        });
                    }
                    else {
                        vm.imagesrc = "/Content/images/no_image_thumb.gif";
                    }
                });
                $scope.$emit("update-title", "CREATE USER");

                function getImageUrl() {
                    //return userService.getImageUrl().then(function (result) {
                    //    //vm.imagesrc = result.data+"/Content/images/no_image_thumb.gif";
                    //    //console.log()
                    //});
                }
                init();

                $scope.$watch('vm.user.isExecutive', function (newValue, oldValue) {

                });
                function SaveValueForEquipment() {
                    return userService.saveUserForEquipment(vm.user, false).then(function (result) {
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
                        //$state.go('core.layout.user.list');
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }
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
                $q.all([getFranchiseeCollection(), getRoles(), getImageUrl()]);
            }
        ]
    );
}());