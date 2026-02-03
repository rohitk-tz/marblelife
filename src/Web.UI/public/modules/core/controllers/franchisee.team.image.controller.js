(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("FranchiseeTeamImageController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "modalParam", "FileService", "Toaster", "FranchiseeService",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, modalParam, fileService, toaster, franchiseeService) {

            var vm = this;
           
            vm.franchiseeId = modalParam.FranchiseeId;
            vm.calendarModel = {};
            vm.delete = deleteImage;
            vm.isImageChanged = false;
            vm.ImageSrc = "/Content/images/no_image_thumb.gif";
            vm.identity = $rootScope.identity;
            vm.info = {};
            vm.upload = upload;
            vm.save = save;
            vm.downloadUserImage = downloadUserImage;
            vm.info.fileList = [];
            vm.franchisee = {};
            vm.info.fileUploadModel = [];
            vm.isImageDeleted = false;
            vm.fileId = '';
            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            $scope.tempFile = {
                attachTempImageFile: ""
            };
            $scope.file = {
                attachedImageFile: ""
            };
            vm.options = [];
            function prepareOptions() {
                vm.options.push({ display: 'Technician', value: 1 }),
                vm.options.push({ display: 'Sales Rep.', value: 0 });
            };

            function getTeamImage() {
                return franchiseeService.getFranchiseeTeamImage(vm.franchiseeId).then(function (result) {
                    if (result != null && result.data!=null) {
                        vm.fileId = result.data.fileId;
                        vm.ImageName = result.data.imageName;
                        vm.franchiseeName = result.data.franchiseeName;
                        fileService.getFileStreamByUrl(result.data.franchiseeTeamImage).then(function (result) {
                            $scope.ImageSrc = fileService.getStreamUrl(result);
                            vm.ImageSrc = $scope.ImageSrc;
                            vm.isImageDeleted = false;
                        })
                    }
                    else {
                        vm.ImageSrc = "/content/images/layout/avatar9.jpg";
                    }
                });
            }

            function deleteImage() {
                var id = "team-image";
                var myElem = document.getElementById(id);
                myElem.src = "/Content/images/no_image_thumb.gif";
                vm.fileId = null;
                $scope.file.attachedImageFile = "";
                vm.isImageChanged = true;;
                vm.isImageDeleted = true;
            }

            function upload() {
                $('#file_input').click();
            }
            function downloadUserImage(fileId) {
                return fileService.getFileForDownload(fileId).then(function (result) {
                    fileService.downloadFileImage(result.data, vm.ImageName);
                });
            }
            function save() {
                vm.franchisee.fileUploadModel = vm.info;
                vm.franchisee.isImageChanged = $scope.isImageChanged;
                vm.franchisee.franchiseeId = vm.franchiseeId;
                return franchiseeService.saveFranchiseeTeamImage(vm.franchisee).then(function (result) {
                    toaster.show('Franchisee Team Image Uploaded Successfully');
                    $uibModalInstance.close();
                }).catch(function (err) {
                    vm.isDisabled = false;
                });
            }
            $scope.readUrl = fileService.readLocalFile;
            $scope.$watch('tempFile.attachTempImageFile', function (newValue, oldValue) {
                $scope.file.attachedImageFile = $scope.tempFile.attachTempImageFile;
                if ($scope.tempFile.attachTempImageFile.type == "image/png" || $scope.tempFile.attachTempImageFile.type == "image/jpg"
                            || $scope.tempFile.attachTempImageFile.type == "image/jpeg" || $scope.tempFile.attachTempImageFile.type == "image/psd"
                                                        || $scope.tempFile.attachTempImageFile.type != undefined) {
                    $scope.isImage = true;
                    var image = document.getElementById('team-image');
                    image.style.transform = "rotate(" + -0 + "deg)";
                    fileService.uploadFile($scope.file.attachedImageFile).then(function (result) {
                        var width = image.naturalWidth;
                        var height = image.naturalHeight;
                        vm.isImageChanged = true;
                        vm.fileId = null;
                        //if (height >= width) {
                        vm.info.fileList.push(result.data);
                        $scope.isInValidImage = false;

                    });
                }
                else {
                    vm.ImageSrc = "/Content/images/no_image_thumb.gif";
                }
            });

            $q.all(getTeamImage());
        }]);
}());