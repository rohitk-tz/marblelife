(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("MediaInvoiceLineImageGalleryController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SchedulerService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "AddressService", "modalParam", "FileService", "$filter",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, schedulerService, franchiseeService,
                notification, clock, toaster, addressService, modalParam, fileService, $filter) {

                var vm = this;
                vm.estimateInfo = modalParam.EstimateInfo;
                vm.service = modalParam.Service;
                vm.allServices = modalParam.AllServices;
                vm.location = modalParam.Location;
                vm.surfaceType = modalParam.SurfaceType;
                vm.materialType = modalParam.MaterialType;
                vm.surfaceColor = modalParam.SurfaceColor;
                vm.surfaceFinish = modalParam.SurfaceFinish;
                vm.serviceType = modalParam.ServiceType;
                vm.isInvoiceLineSubItem = modalParam.IsInvoiceLineSubItem;
                vm.invoiceLineSubItem = modalParam.InvoiceLineSubItem;
                vm.isMultiple = false;
                vm.isImageRemoved = false;
                vm.isUploaded = false;
                vm.save = save;
                vm.isProcessing = false;
                vm.info = {};
                vm.info.fileList = [];
                vm.noImageAvailable = [];
                vm.addFile = addFile;
                vm.isAddedInLineItemForSave = false;
                vm.selectedImageList = [];
                vm.uniqueImageListId = [];
                vm.uniqueImageList = [];
                vm.removeSelectedImages = removeSelectedImages;
                vm.noImageAvailable.push({
                    imgUrl: '/Content/images/no_image_thumb.gif',
                    caption: "No Image Availabe",
                    isUploadedImage: false
                });

                angular.forEach(vm.allServices, function (value1) {
                    if (value1.imageList != undefined) {
                        angular.forEach(value1.imageList, function (image) {
                            if (value1.typeOfStoneType2 == vm.materialType && value1.serviceType == vm.serviceType) {
                                image.isUploadedImage = true;
                                image.isDeleted = false;
                                if (image.imgUrl == undefined || image.imgUrl == '') {
                                    fileService.getFileStreamByUrl(image.relativeLocation).then(function (result) {
                                        image.imgUrl = fileService.getStreamUrl(result);
                                    });
                                }
                                vm.info.fileList.push(image);
                            }

                        });
                        angular.forEach(value1.subItem, function (value2) {
                            angular.forEach(value2.imageList, function (image) {
                                if (value1.typeOfStoneType2 == vm.materialType && value1.serviceType == vm.serviceType) {
                                    image.isUploadedImage = true;
                                    image.isDeleted = false;
                                    if (image.imgUrl == undefined || image.imgUrl == '') {
                                        fileService.getFileStreamByUrl(image.relativeLocation).then(function (result) {
                                            image.imgUrl = fileService.getStreamUrl(result);
                                        });
                                    }
                                    vm.info.fileList.push(image);
                                }

                            });
                        });


                    }
                    else {
                        value1.imageList = [];
                    }
                });
                
                if (vm.info.fileList.length > 0) {
                    //for (let i = 0; i < vm.info.fileList.length - 1; i++) {
                    //    const f = vm.info.fileList[i]?.['fileId'] || null
                    //    let flag = vm.info.fileList[0]?.['fileId']
                    //    let dub = null
                    //    for (let j = i + 1; j < vm.info.fileList.length; j++) {
                    //        const s = vm.info.fileList[j]?.['fileId'] || null
                    //        if (f === s || flag === s) {
                    //            vm.info.fileList.splice(j, 1)
                    //            if (dub === s) {
                    //                vm.info.fileList.splice(j, 1)
                    //            }
                    //            dub = s
                    //        } else {
                    //            flag = vm.info.fileList[j].fileId
                    //        }

                    //    }
                    //}
                    vm.info.fileList.forEach( function (item) {
                        if (vm.uniqueImageListId.indexOf(item.fileId) === -1) {
                            vm.uniqueImageListId.push(item.fileId);
                            vm.uniqueImageList.push(item);
                        }
                    });
                }
                

                if (vm.estimateInfo != null)
                    vm.info.franchisee = vm.estimateInfo.franchisee;

                vm.close = function () {
                    removeSelectedImages();
                    $uibModalInstance.dismiss();
                };



                vm.removeFile = function (index) {
                    //var indexreverse = vm.info.fileList.length - index - 1;
                    //vm.info.fileList.splice(indexreverse, 1);
                    //if (vm.service.imageList.length != 0) {
                    //    vm.service.imageList[indexreverse - 1].isDeleted = true;
                    //}
                    //vm.isUploaded = true;
                    //toaster.show("File Removed Successfully!");
                }

                function save() {
                    if (vm.selectedImageList.length <= 0) {
                        notification.showAlert("No Image Selected To Add In Line Item Image!");
                        return;
                    }
                    vm.isProcessing = true;
                    removeSelectedImages();
                    angular.forEach(vm.selectedImageList, function (file) {
                        if (vm.service.imageList == undefined) {
                            vm.service.imageList = [];
                        }
                        vm.service.imageList.push(file);
                    });
                    vm.isProcessing = false;
                    toaster.show("Image Added Successfully!");
                    vm.selectedImageList = [];
                    //$uibModalInstance.close();
                }

                function addFile(index, file) {
                    if (!file.isAddedInLineItem || file.isAddedInLineItem == undefined) {
                        file.isAddedInLineItem = true;
                        vm.selectedImageList.push(file);
                    }
                    else {
                        if (file.isAddedInLineItem) {
                            var fileIndex = vm.selectedImageList.indexOf(file);
                            vm.selectedImageList.splice(fileIndex, 1)
                        }
                        file.isAddedInLineItem = false;
                    }
                    if (vm.selectedImageList.length <= 0) {
                        vm.isAddedInLineItemForSave = false;
                    }
                    else {
                        vm.isAddedInLineItemForSave = true;
                    }
                }
                function removeSelectedImages() {
                    angular.forEach(vm.info.fileList, function (value) {
                        delete value.isAddedInLineItem;
                    });
                }
                $q.all();
            }]);
}());