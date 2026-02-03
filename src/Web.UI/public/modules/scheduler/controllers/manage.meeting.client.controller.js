(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("ManageMeetingController",
        ["$scope", "$rootScope", "$state", "$q", "SchedulerService", "FranchiseeService", "$uibModal",
            "Notification", "Clock", "Toaster", "$stateParams", "FileService", "EstimateService",
        function ($scope, $rootScope, $state, $q, schedulerService, franchiseeService, $uibModal,
            notification, clock, toaster, $stateParams, fileService, estimateService) {

            var vm = this;
            vm.meetingId = $stateParams.id != null ? $stateParams.id : 0;
            $scope.meetingId = vm.meetingId;
            vm.getVacationInfo = getVacationInfo;
            vm.previousView = $stateParams.previousView != null ? $stateParams.previousView : 0;
            vm.addNote = addNote;
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
                    templateUrl: 'modules/scheduler/views/meeting-repeat-client.view.html',
                    controller: 'MeetingRepeatController',
                    controllerAs: 'vm',
                    size: 'lg',
                    resolve: {
                        modalParam: function () {
                            return {
                                VacationInfo: vm.meetingInfo,
                            };
                        }
                    },
                    backdrop: 'static',
                });
                modalInstance.result.then(function () {
                    getVacationInfo();
                });
            }

            function editVacation() {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'modules/scheduler/views/create-meeting.client.view.html',
                    controller: 'CreateMeetingController',
                    controllerAs: 'vm',
                    size: 'lg',
                    resolve: {
                        modalParam: function () {
                            return {
                                MeetingId: vm.meetingId,
                                franchiseeId: vm.meetingInfo.franchiseeId
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
                                MeetingInfo: vm.meetingInfo,
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
                                MeetingInfo: vm.meetingInfo,
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
                return estimateService.getMeetingInfo($scope.meetingId).then(function (result) {
                    vm.meetingInfo = result.data;
                    $scope.$emit("update-title", "Manage Meeting:" + vm.meetingInfo.franchisee);
                    vm.meetingInfo.startDate = moment((vm.meetingInfo.actualStartDateString)).format("MM/DD/YYYY HH:mm");
                    vm.meetingInfo.endDate = moment((vm.meetingInfo.actualEndDateString)).format("MM/DD/YYYY HH:mm");
                    vm.meetingInfo.id = $scope.meetingId;
                    getMedia();
                });
            }

            function getMedia() {
                $scope.slides = [];
                vm.mediaModel.rowId = $scope.meetingId;
                vm.mediaModel.MediaType = vm.MediaType.Meeting;
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

            $scope.$emit("update-title", "Manage Meeting Time");

            $q.all([getVacationInfo()]);
        }]);
}());
