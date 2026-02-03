(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("MediaInvoiceLineUploadController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SchedulerService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "AddressService", "modalParam", "FileService", "$filter",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, schedulerService, franchiseeService,
                notification, clock, toaster, addressService, modalParam, fileService, $filter) {

                var vm = this;
                vm.estimateInfo = modalParam.EstimateInfo;
                vm.service = modalParam.Service;
                vm.location =  modalParam.Location;
                vm.surfaceType = modalParam.SurfaceType;
                vm.materialType = modalParam.MaterialType;
                vm.surfaceColor = modalParam.SurfaceColor;
                vm.surfaceFinish = modalParam.SurfaceFinish;
                vm.serviceType = modalParam.ServiceType;
                vm.isMultiple = false;
                vm.isImageRemoved = false;
                vm.isUploaded = false;
                vm.save = save;
                vm.isProcessing = false;
                vm.info = {};
                vm.info.fileList = [];
                vm.info.fileList.push({
                    imgUrl: '/Content/images/upload_image_thumb.gif',
                    caption: "Upload Image",
                    isUploadedImage: false
                });
                if (vm.service.imageList != undefined) {
                    angular.forEach(vm.service.imageList, function (image) {
                        image.isUploadedImage = true;
                        image.isDeleted = false;
                        if (image.imgUrl == undefined || image.imgUrl == '') {
                            fileService.getFileStreamByUrl(image.relativeLocation).then(function (result) {
                                image.imgUrl = fileService.getStreamUrl(result);
                            });
                        }
                        vm.info.fileList.push(image);
                    });
                }
                else {
                    vm.service.imageList = [];
                }
                var locationList = [];
                if (vm.location != undefined) {
                    angular.forEach(vm.location, function (item) {
                        locationList.push(item.id);
                    });
                }
                vm.locationNames = locationList.join(", ");

                var listOfServiceName = '';
                if (vm.service.serviceIds.length >= 1) {
                    listOfServiceName = vm.service.serviceIds[0].id;
                }
                else {
                    listOfServiceName = vm.service.serviceIds.id;
                }
                vm.listOfServiceName = listOfServiceName;

                if (vm.estimateInfo != null)
                    vm.info.franchisee = vm.estimateInfo.franchisee;

                vm.close = function () {
                    $uibModalInstance.dismiss();
                };

                $scope.uploadFile = function (files) {
                    if (files != null) {
                        angular.forEach(files, function (file) {
                            var fileName = file.name.split('.');
                            var fileExtention = fileName.slice(-1);

                            if (fileExtention[0] == "heic" || fileExtention[0] == "HEIC") {
                                toaster.error("System Does Not Accept Images Of .heic Format!");
                                return;
                            }
                            return fileService.upload(file).then(function (result) {
                                toaster.show("File uploaded!");
                                result.data.imgUrl = URL.createObjectURL(file);
                                result.data.isUploadedImage = true;
                                vm.info.fileList.push(result.data);
                                vm.isUploaded = true;
                            });
                        });
                    }
                    else {
                        if (file != null)
                            toaster.error("Please select a file!");
                    }
                }

                vm.removeFile = function (index) {
                    var indexreverse = vm.info.fileList.length - index - 1;
                    vm.info.fileList.splice(indexreverse, 1);
                    if (vm.service.imageList.length != 0) {
                        vm.service.imageList[indexreverse-1].isDeleted = true;
                    }
                    vm.isUploaded = true;
                    toaster.show("File Removed Successfully!");
                }

                function save() {
                    vm.info.estimateId = vm.estimateInfo != null ? vm.estimateInfo.id : null;
                    vm.info.serviceId = vm.service != null ? vm.service.id : null;
                    vm.isProcessing = true;
                    vm.info.fileList.splice(0, 1);
                    if (vm.info.fileList == 0 && vm.service.imageList > 0) {
                        notification.showAlert("You will have to upload a image to Save!");
                        return;
                    }

                    var localImageList = angular.copy(vm.service.imageList);
                    angular.forEach(localImageList, function (image, index) {
                        if (image.isDeleted == true) {
                            vm.service.imageList.splice(index, 1);
                        }
                        if (index == 1 && vm.service.imageList.length == 1 && image.isDeleted==true) {
                            vm.service.imageList.splice(0, 1);
                        }
                    });
                    return schedulerService.saveInvoiceLineItemMedia(vm.info).then(function (result) {
                        if (result.data != null) {
                            angular.forEach(result.data, function (file) {
                                vm.service.imageList.push(file);
                            });
                            toaster.show(result.message.message);
                            $uibModalInstance.close();
                            vm.isProcessing = false;
                        }
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }
                $q.all();
            }]);
}());