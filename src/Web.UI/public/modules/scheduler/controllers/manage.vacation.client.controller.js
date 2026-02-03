(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("ManageVacationController",
        ["$scope", "$rootScope", "$state", "$q", "SchedulerService", "FranchiseeService", "$uibModal",
            "Notification", "Clock", "Toaster", "$stateParams", "FileService", "EstimateService",
        function ($scope, $rootScope, $state, $q, schedulerService, franchiseeService, $uibModal,
            notification, clock, toaster, $stateParams, fileService, estimateService) {

            var vm = this;
            vm.vacationId = $stateParams.id != null ? $stateParams.id : 0;
            vm.getVacationInfo = getVacationInfo;
            vm.addNote = addNote;
            vm.previousView = $stateParams.previousView != null ? $stateParams.previousView : 0;
            vm.editVacation = editVacation;
            vm.uploadMedia = uploadMedia;
            $scope.myInterval = 5000;
            $scope.noWrapSlides = false;
            $scope.active = 0;
            var slides = $scope.slides = [];
            var currIndex = 0;
            vm.MediaType = DataHelper.ScheduleType;
            vm.repeatVacation = repeatVacation;
            vm.Roles = DataHelper.Role;
            vm.isOpsMgr = $rootScope.identity.roleId == vm.Roles.OperationsManager;

            function repeatVacation() {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'modules/scheduler/views/vacation-repeat.client.view.html',
                    controller: 'VacationRepeatController',
                    controllerAs: 'vm',
                    size: 'lg',
                    resolve: {
                        modalParam: function () {
                            return {
                                VacationInfo: vm.vacationInfo,
                            };
                        }
                    },
                    backdrop: 'static',
                });
                modalInstance.result.then(function () {
                    getVacationInfo();
                });
            }

            function editVacation(date, query) {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'modules/scheduler/views/create-vacation.client.view.html',
                    controller: 'CreateVacationController',
                    controllerAs: 'vm',
                    size: 'lg',
                    resolve: {
                        modalParam: function () {
                            return {
                                VacationId: vm.vacationId,
                                FranchiseeId: vm.vacationInfo.franchiseeId
                            };
                        }
                    },
                    backdrop: 'static',
                });
                modalInstance.result.then(function () {
                    getVacationInfo();
                }, function () {
                });
            }

            function uploadMedia() {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'modules/scheduler/views/media-upload.client.view.html',
                    controller: 'MediaUploadController',
                    controllerAs: 'vm',
                    size: 'lg',
                    resolve: {
                        modalParam: function () {
                            return {
                                VacationInfo: vm.vacationInfo,
                            };
                        }
                    },
                    backdrop: 'static',
                });
                modalInstance.result.then(function () {
                    getVacationInfo();
                });
            }

            function addNote() {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'modules/scheduler/views/note-add.client.view.html',
                    controller: 'SchedulerNoteController',
                    controllerAs: 'vm',
                    size: 'lg',
                    resolve: {
                        modalParam: function () {
                            return {
                                VacationInfo: vm.vacationInfo,
                            };
                        }
                    },
                    backdrop: 'static',
                });
                modalInstance.result.then(function () {
                    getVacationInfo();
                });
            }

            function getVacationInfo() {
                return estimateService.getVacationInfo(vm.vacationId).then(function (result) {
                    vm.vacationInfo = result.data;
                    $scope.$emit("update-title", "Manage Vacation:" + vm.vacationInfo.franchisee);
                    //vm.vacationInfo.startDate = moment(new Date(vm.vacationInfo.startDate + "Z")).format("MM/DD/YYYY HH:mm");
                    //vm.vacationInfo.endDate = moment(new Date(vm.vacationInfo.endDate + "Z")).format("MM/DD/YYYY HH:mm");
                    vm.vacationInfo.startDate = moment((vm.vacationInfo.actualStartDateString)).format("MM/DD/YYYY HH:mm");
                    vm.vacationInfo.endDate = moment((vm.vacationInfo.actualEndDateString)).format("MM/DD/YYYY HH:mm");
                    vm.vacationInfo.id = vm.vacationId;
                    getMedia();
                });
            }

            function getMedia() {
                $scope.slides = [];
                vm.mediaModel.rowId = vm.vacationId;
                vm.mediaModel.MediaType = vm.MediaType.Vacation;
                return schedulerService.getMedia(vm.mediaModel).then(function (result) {
                    vm.media = result.data;
                    getSlides();
                });
            }

            function getSlides() {
                angular.forEach(vm.media.resources, function (value) {
                    fileService.getFileStreamByUrl(value.relativeLocation).then(function (result) {
                        $scope.imageUrl = fileService.getStreamUrl(result);
                        $scope.slides.push({
                            url: $scope.imageUrl, name: value.caption, size: value.size, status: value.status,
                            createdBy: value.createdBy, createdOn: value.createdOn
                        });
                    });
                });
            }

            $scope.$emit("update-title", "Manage Personal Time");

            $q.all([getVacationInfo()]);
        }]);
}());
