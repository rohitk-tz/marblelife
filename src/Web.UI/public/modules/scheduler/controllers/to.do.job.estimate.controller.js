(function () {

    angular.module(SchedulerConfiguration.moduleName).controller("ToDoJobEstimateController",
        ["$scope", "$rootScope", "$state", "$q", "FranchiseeService",
            "GeoCodeService", "FileService", "Toaster", "Clock", "Notification", "APP_CONFIG", "ToDoService", "$stateParams", "LocalStorageService", "$window", "$uibModal",
            function ($scope, $rootScope, $state, $q, franchiseeService,
                geoCodeService, fileService, toaster, clock, notification, config, toDoService, $stateParams, LocalStorageService, $window, $uibModal) {

                var vm = this;
                vm.processing = true;
                vm.isFromInfo = false;
                vm.toDoInfo = toDoInfo;
                vm.isFromInfo = false;
                vm.toDoList = toDoList;
                vm.isDivClicked = false;
                vm.deleteToDo = deleteToDo;
                vm.resetSearch = resetSearch;
                vm.updateToDoByStatus = updateToDoByStatus;
                $scope.editMode = false;
                vm.editAddToDo = 'Add To-Do';
                vm.todoInfo = {};
                vm.customerList = customerList;
                vm.openCommentModel = openCommentModel;
                vm.openViewCommentModel = openViewCommentModel;
                vm.customerInfo = customerInfo;
                vm.todoInfo.isCustomerUse = false;
                vm.isAllDataExpand = true;
                vm.isTodayDataExpand = true;
                vm.isExpiredDataExpand = true;
                vm.isAddingNew = false;
                vm.close = close;
                vm.pageChange = pageChange;
                vm.getToDo = getToDo;
                vm.getToDoListById = getToDoListById;
                vm.todoInfo = {};
                //vm.currentYear = moment(clock.now(), "DD/MM/YYYY").year();
                vm.Roles = DataHelper.Role;
                vm.save = save;
                vm.dataRecorderMetaDataId = null;
                vm.FranchiseeCollectionList = [];

                vm.SalesDataUploadStatus = DataHelper.SalesDataUploadStatus;
                vm.pagingOptions = config.pagingOptions;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin; //To do replace with lookup
                vm.statusOptions = [];
                vm.statusOptions.push({ display: 'Open', value: 231 });
                vm.statusOptions.push({ display: 'In-Progress', value: 232 });
                vm.statusOptions.push({ display: 'Completed', value: 233 });
                vm.statusOptions.push({ display: 'Retired', value: 234 });

                vm.taskOptions = [];
                vm.taskOptions.push({ display: 'To-Do', value: 235 });
                vm.taskOptions.push({ display: 'Cold-Calling', value: 236 });
                vm.jobEstimateList = [
                    { display: 'Job', value: 0 },
                    { display: 'Estimate', value: 1 }
                ]
                vm.searchOption = 0;
                vm.schedulerList = []
                vm.getScheduler = getScheduler;
                vm.isFranchiseeAdmin = false;
                vm.FranchiseeAdminId = null;

                //vm.searchOptions.push({ display: 'Status', value: '2' });

                vm.query = {
                    franchiseeId: null,
                    startDate: null,
                    endDate: null,
                    pageNumber: 1,
                    pageSize: config.defaultPageSize,
                    isFranchiseeLevel: true,
                    statusId: null
                };

                vm.schedulerQuery = {
                    franchiseeId: null,
                    customerName: "",
                    isJobOrEstimate: 1
                }
                vm.isFromFranchiseeLevel = LocalStorageService.getToDoListIsFromFranchiseeLevel();
                vm.query.franchiseeId = $stateParams.franchiseeId != null ? $stateParams.franchiseeId : 0;
                if (!vm.isSuperAdmin)
                    vm.query.franchiseeId = $rootScope.identity.organizationId;


                function save() {
                    vm.processing = true;
                    vm.todoInfo.customerName = vm.customerName;
                    if (vm.todoInfo.date == null) {
                        toaster.error("Please Enter Date!!");
                        return;
                    }
                    if (vm.todoInfo.statusId == null) {
                        toaster.error("Please Enter Status!!");
                        return;
                    }
                    if (vm.todoInfo.task == null || vm.todoInfo.task == '') {
                        toaster.error("Please Enter To-Do!!");
                        return;
                    }
                    if (vm.todoInfo.isCustomerUse == true && (vm.todoInfo.customerName == null || vm.todoInfo.customerName == '')) {
                        toaster.error("Please Enter Customer Name!!");
                        return;
                    }
                    vm.todoInfo.customerName = vm.customerName;
                    vm.todoInfo.franchiseeId = vm.query.franchiseeId;
                    vm.todoInfo.isFranchiseeLevel = true;
                    settingToDoListFranchisee();
                    vm.todoInfo.typeId = 33;
                    vm.isNew = true;
                    if (vm.todoInfo.id > 0) {
                        vm.isNew = false;
                        vm.todoInfo.dataRecorderMetaDataId = vm.dataRecorderMetaDataId;
                    }
                    vm.todoInfo.schedulerId = vm.schedulerTitle;
                    vm.todoInfo.selectedFranchiseeId = vm.franchiseeId;
                    vm.todoInfo.actualDate = moment((vm.todoInfo.date)).format("MM/DD/YYYY");
                    return toDoService.saveToDo(vm.todoInfo).then(function (result) {
                        if (result != null) {
                            if (result.data) {
                                if (vm.isNew)
                                    toaster.show("To Do Created Successfully!!");
                                else
                                    toaster.show("Update Successfully!!");
                            }
                            //vm.isToDoCount = $window.sessionStorage.getItem("todayToDoCount");
                            //$window.location.reload();
                            getToDo();
                            vm.todoInfo = {};
                            vm.isAddingNew = false;
                            vm.customerName = "";
                        }
                    });
                }
                function getToDo() {
                    vm.processing = true;
                    settingToDoListFranchisee();
                    $scope.editMode = false;
                    vm.todoInfoListForToday = [];
                    vm.todoInfoList = [];
                    vm.todoInfoListExpired = [];
                    return toDoService.getToDoList(vm.query).then(function (result) {
                        if (result != null) {

                            vm.todoInfoListForToday = result.data.todaysCollection;
                            vm.todoInfoList = result.data.allCollection;
                            vm.todoInfoListExpired = result.data.expiredToDoCollection;
                            vm.expiredCollectionCount = result.data.expiredToDoCount
                            vm.count = result.data.totalCount;
                            vm.todayToDoCount = result.data.todayToDoCount;
                            vm.isFranchiseeAdmin = result.data.isFranchiseeAdmin;
                            vm.FranchiseeAdminId = result.data.franchiseeId;
                            if (vm.FranchiseeAdminId) {
                                vm.franchiseeId = result.data.franchiseeId.toString();
                            }
                            $rootScope.identity.todayToDoCount = vm.todayToDoCount;
                            vm.processing = false;
                        }
                    });
                }

                function getToDoListById(id) {
                    vm.processing = true;
                    vm.isFromEdit = true;
                    vm.editAddToDo = 'Edit To-Do';
                    return toDoService.getToDoListById(id).then(function (result) {
                        if (result != null) {
                            //vm.customerName = vm.todoInfo.customerName;
                            vm.todoInfo = result.data;
                            var startDate = moment((vm.todoInfo.date));
                            vm.todoInfo.date = startDate._d;
                            vm.isAddingNew = true;
                            $window.scrollTo(0, 0);
                            vm.customerName = vm.todoInfo.customerName;
                            vm.processing = false;
                            vm.dataRecorderMetaDataId = vm.todoInfo.dataRecorderMetaData.Id;
                            if (vm.todoInfo.franchiseeId != null) {
                                vm.franchiseeId = vm.todoInfo.franchiseeId.toString();
                            }
                            if (vm.todoInfo.isJobOrEstimate == "Estimate") {
                                vm.isJobEsimate = 1;
                            }
                            else if (vm.todoInfo.isJobOrEstimate == "Job") {
                                vm.isJobEsimate = 0;
                            }
                            else {
                                vm.isJobEsimate = null;
                            }
                            getScheduler();
                            if (vm.todoInfo.isJobOrEstimate != "" && vm.todoInfo.isJobOrEstimate != undefined) {
                                vm.schedulerTitle = vm.todoInfo.schedulerId;
                            }
                        }
                    });
                }

                function pageChange() {
                    getToDo();
                }

                function close() {
                    vm.todoInfo = {};
                    vm.isAddingNew = false;
                    if (!vm.isFranchiseeAdmin) {
                        vm.franchiseeId = "";
                    }
                    vm.isJobEsimate = "";
                    vm.schedulerTitle = null;
                    vm.schedulerList = [];
                }

                function settingToDoListFranchisee() {
                    isFromFranchiseeLevel = $window.sessionStorage.getItem("IsFromFranchiseeLevel");

                    if (isFromFranchiseeLevel == 'true') {
                        vm.todoInfo.franchiseeId = $window.sessionStorage.getItem("IsFromFranchiseeId");
                        vm.franchiseeName = "- " + $window.sessionStorage.getItem("IsFromFranchiseeName");
                        vm.query.franchiseeId = $window.sessionStorage.getItem("IsFromFranchiseeId");
                        vm.todoInfo.isFranchiseeLevel = true;
                        vm.query.isFranchiseeLevel = true;
                    }
                    else {
                        vm.query.isFranchiseeLevel = false;
                        vm.todoInfo.isFranchiseeLevel = false;
                        vm.todoInfo.franchiseeId = 0;
                        vm.franchiseeName = "- " + 'National Level';
                        vm.query.franchiseeId = 0;
                    }
                }

                function customerList(text) {
                    return toDoService.customerList(text).then(function (result) {
                        if (result != null) {
                            return result.data;
                        }
                    });
                }
                function toDoList(text) {
                    return toDoService.toDoList(text).then(function (result) {
                        if (result != null) {
                            return result.data;
                        }
                    });
                }

                function customerInfo(text) {
                    return toDoService.customerInfo(text).then(function (result) {
                        if (result != null) {
                            vm.todoInfo.phoneNumber = result.data.phoneNumber;
                            vm.todoInfo.email = result.data.email;
                        }
                    });
                }

                function toDoInfo(text) {
                    return toDoService.toDoInfo(text).then(function (result) {
                        if (result.data != null) {
                            vm.todoInfo.task = result.data.task;
                        }
                    });
                }

                function openCommentModel(item) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/show-comments.client.view.html',
                        controller: 'ShowCommentsController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Franchisee: item.franchisee,
                                    FranchiseeId: item.franchiseeId,
                                    ToDoId: item.id
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        getToDo();
                        //getList();
                    }, function () {
                        getToDo();
                    })
                };

                function openViewCommentModel(item) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/view-comments.client.view.html',
                        controller: 'ViewCommentsController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    ToDoId: item.id
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        //vm.query.franchiseeId = result;
                        getList();
                    }, function () {

                    })
                };


                $scope.$watch('vm.customerName', function (nv, ov) {
                    if (vm.isFromEdit == true) {
                        vm.isFromEdit = false;
                        return;
                    }
                    if (nv == '' || nv == undefined) {
                        return;
                    }
                    vm.customerInfo(nv);
                });

                $scope.$watch('vm.todoInfo.task', function (nv, ov) {
                    if (!vm.isDivClicked) {
                        return;
                    }
                    vm.isDivClicked = false;
                    if (vm.isFromEdit == true) {
                        vm.isFromEdit = false;
                        return;
                    }
                    if (nv == '' || nv == undefined) {
                        return;
                    }
                    vm.toDoInfo(nv);
                });

                function resetSearch() {
                    $scope.$broadcast("reset-dates");
                    vm.query = {
                        franchiseeId: 0,
                        startDate: null,
                        endDate: null,
                        pageNumber: 1,
                        pageSize: config.defaultPageSize,
                        isFranchiseeLevel: true,
                        statusId: null
                    };
                    vm.getToDo();
                }

                function updateToDoByStatus(toDo) {
                    vm.isProcessing = true;
                    toDo.actualDate = moment((toDo.date)).format("MM/DD/YYYY");
                    return toDoService.saveCommentToDoByStatus(toDo).then(function (result) {
                        if (result.data != true)
                            toaster.error("Error in Updating Status Successfully");
                        else
                            toaster.show("Status Updated Successfully");

                        //$scope.editMode = !($scope.editMode);
                        getToDo();
                        vm.isProcessing = false;
                    });
                }
                function deleteToDo(id) {
                    notification.showConfirm("You are about to delete a To-Do/FollowUp . Do you want to proceed?", "Warning Message:", function () {
                        return toDoService.deleteToDo(id).then(function (result) {
                            if (result.data != true)
                                toaster.error("Error in Deleting To-Do/FollowUp");
                            else {
                                toaster.show("To-Do/FollowUp Deleted Successfully");
                                getToDo();
                            }

                            //$scope.editMode = !($scope.editMode);
                            getToDo();
                            vm.isProcessing = false;
                        });
                    })
                }

                function getFranchiseeList() {
                    return franchiseeService.getActiveFranchiseeList().then(function (result) {
                        if (result != null) {
                            vm.FranchiseeCollectionList = result.data;
                        }
                    });
                }

                function getScheduler() {
                    if (vm.franchiseeId != "" && vm.customerName != "" && (vm.isJobEsimate == 0 || vm.isJobEsimate == 1)) {
                        vm.schedulerQuery.franchiseeId = vm.franchiseeId;
                        vm.schedulerQuery.customerName = vm.customerName;
                        vm.schedulerQuery.isJobOrEstimate = vm.isJobEsimate;
                        return toDoService.getSchedulerListForToDo(vm.schedulerQuery).then(function (result) {
                            if (result != null) {
                                vm.schedulerList = result.data.jobSchedulerForToDo;
                            }
                        });
                    }
                }

                $(document).on('click', 'div#taskList ul', function () {
                    vm.isDivClicked = true;
                    vm.toDoInfo(vm.todoInfo.task);
                });
                $scope.$emit("update-title", "");
                $q.all([getToDo(), settingToDoListFranchisee(), getFranchiseeList()]);
            }]);
}());