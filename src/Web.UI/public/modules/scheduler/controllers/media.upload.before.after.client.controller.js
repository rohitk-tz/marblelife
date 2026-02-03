(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("MediaUploadBeforeAfterController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SchedulerService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "AddressService", "modalParam", "FileService",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, schedulerService, franchiseeService,
                notification, clock, toaster, addressService, modalParam, fileService) {

                var vm = this;
                vm.isProcessing = true;
                vm.isImageForSamePane = false;
                vm.jobInfo = modalParam.JobInfo;
                vm.estimateInfo = modalParam.EstimateInfo;
                vm.vacationInfo = modalParam.VacationInfo;
                vm.meetingInfo = modalParam.MeetingInfo;
                vm.isMultiple = modalParam.isMultiple != undefined ? modalParam.isMultiple : false;
                vm.isFromBeforeAfter = modalParam.isFromBeforeAfter != undefined ? modalParam.isFromBeforeAfter : false;
                vm.alreadyUploaded = modalParam.alreadyUploaded != undefined ? modalParam.alreadyUploaded : false;
                vm.IsFromBuilding = modalParam.IsFromBuilding != undefined ? modalParam.IsFromBuilding : false;
                vm.isImageRemoved = false;
                vm.isUploaded = false;
                vm.save = save;
                vm.info = {};
                vm.info.fileList = [];
                if (vm.jobInfo != null)
                    vm.info.franchisee = vm.jobInfo.franchisee;
                else if (vm.estimateInfo != null)
                    vm.info.franchisee = vm.estimateInfo.franchisee;
                else if (vm.vacationInfo != null)
                    vm.info.franchisee = vm.vacationInfo.franchisee;

                vm.close = function () {
                    $uibModalInstance.dismiss();
                };

                vm.uploadFile = function (files, isFromBeforeAfter) {
                    var isMessageParsed = false;
                    if (files != null && vm.isMultiple) {
                        angular.forEach(files, function (file) {
                            var fileName = file.name.split('.');
                            var fileExtention = fileName.slice(-1);

                            if (fileExtention[0] == "heic" || fileExtention[0] == "HEIC") {
                                toaster.error("System Does Not Accept Images Of .heic Format!");
                                return;
                            }
                            else if (fileExtention[0] == "jpg" || fileExtention[0] == "JPG" || fileExtention[0] == "png" || fileExtention[0] == "PNG" || fileExtention[0] == "jpeg" || fileExtention[0] == "JPEG") {
                                return fileService.uploadForBeforeAfter(file).then(function (result) {
                                    if (isFromBeforeAfter) {
                                        vm.isUploaded = true;
                                    }
                                    vm.isProcessing = false;
                                    if (!isMessageParsed) {
                                        isMessageParsed = true;
                                        toaster.show("File uploaded.");
                                    }
                                    vm.info.fileList.push(result.data);
                                });
                            }
                            else {
                                toaster.error("System Does Not Accept Images Of Current Format!");
                                return;
                            }
                        })
                    }
                    else {
                        var fileName = files.name.split('.');
                        var fileExtention = fileName.slice(-1);

                        if (fileExtention[0] == "heic" || fileExtention[0] == "HEIC") {
                            toaster.error("System Does Not Accept Images Of .heic Format!");
                            return;
                        }
                        else if (fileExtention[0] == "jpg" || fileExtention[0] == "JPG" || fileExtention[0] == "png" || fileExtention[0] == "PNG" || fileExtention[0] == "jpeg" || fileExtention[0] == "JPEG") {
                            return fileService.uploadForBeforeAfter(files).then(function (result) {
                                if (isFromBeforeAfter) {
                                    vm.isUploaded = true;
                                }
                                if (!isMessageParsed) {
                                    toaster.show("File uploaded.");
                                }
                                vm.info.fileList.push(result.data);
                            });
                        }
                        else {
                            toaster.error("System Does Not Accept Images Of Current Format!");
                            return;
                        }
                    }
                }


                function save() {
                    vm.info.jobId = vm.jobInfo != null ? vm.jobInfo.jobId : null;
                    vm.info.estimateId = vm.estimateInfo != null ? vm.estimateInfo.id : null;
                    vm.info.vacationId = vm.vacationInfo != null ? vm.vacationInfo.id : null;
                    vm.info.meetingId = vm.meetingInfo != null ? vm.meetingInfo.id : null;

                    vm.info.statusId = vm.jobInfo != null ? vm.jobInfo.statusId : null;
                    vm.isProcessing = true;
                    return schedulerService.saveMediaBeforeAfterForUser(vm.info).then(function (result) {
                        if (result.data != null) {
                            $rootScope.$emit("FileId", result.data);
                            $rootScope.$emit("IsImageForSamePane", vm.isImageForSamePane);
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