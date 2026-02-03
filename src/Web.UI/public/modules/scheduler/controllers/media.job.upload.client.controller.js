(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("MediaJobUploadController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SchedulerService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "AddressService", "modalParam", "FileService",
        function ($scope, $rootScope, $state, $q, $uibModalInstance, schedulerService, franchiseeService,
            notification, clock, toaster, addressService, modalParam, fileService) {

            var vm = this;
            vm.jobInfo = modalParam.JobInfo;
            vm.estimateInfo = modalParam.EstimateInfo;
            vm.vacationInfo = modalParam.VacationInfo;
            vm.meetingInfo = modalParam.MeetingInfo;
            vm.isMultiple = modalParam.IsFromInvoice != undefined ? modalParam.IsFromInvoice : false;
            vm.isFromBeforeAfter = modalParam.isFromBeforeAfter != undefined ? modalParam.isFromBeforeAfter : false;
            vm.alreadyUploaded = modalParam.alreadyUploaded != undefined ? modalParam.alreadyUploaded : false;
            vm.IsFromBuilding = modalParam.IsFromBuilding != undefined ? modalParam.IsFromBuilding : false;
            vm.isImageRemoved = false;
            vm.isUploaded = false;
            vm.save = save;
            vm.isProcessing = true;
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
                if ((vm.isMultiple || vm.IsFromBuilding) && files != null) {
                    angular.forEach(files, function (file) {
                        return fileService.upload(file).then(function (result) {
                            if (isFromBeforeAfter) {
                                vm.isUploaded = true;
                            }
                            toaster.show("File uploaded.");
                            vm.info.fileList.push(result.data);
                        });
                    });
                }
                else if (file != null && !vm.isUploaded && !vm.alreadyUploaded) {
                    return fileService.upload(file).then(function (result) {
                        if (isFromBeforeAfter) {
                            vm.isUploaded = true;
                        }
                        toaster.show("File uploaded.");
                        vm.info.fileList.push(result.data);
                    });
                }
                else {
                    if (file!=null)
                    toaster.error("Multiple Images Not Allowed.");
                }
            }

            //vm.uploadFileForInvoice = function (file) {
            //    if (file != null) {
            //        var extension = file.name.substr(file.name.lastIndexOf('.') + 1);
            //        if (extension == 'docx' || extension == 'pdf' || extension == 'xlsx') {
            //            return fileService.uploadedExceltoImage(file).then(function (result) {
            //                toaster.show("File uploaded.");
            //                vm.info.fileList.push(result.data);
            //            });
            //        }
            //        else {
            //            return fileService.upload(file).then(function (result) {
            //                toaster.show("File uploaded.");
            //                vm.info.fileList.push(result.data);
            //            });
            //        }
            //    }
            //}

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