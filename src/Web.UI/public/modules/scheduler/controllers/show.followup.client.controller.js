(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("ViewFollowUpController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "FranchiseeService", "ToDoService", "Toaster",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, franchiseeService, toDoService, toaster) {

                var vm = this;
                //vm.save = save;
                vm.followUpInfo = {};
                $scope.editMode = false;
                vm.followUpList = modalParam.FollowUpList;
                vm.customerInfo = modalParam.CustomerInfo;
                vm.isFromScheduler = modalParam.IsFromScheduler;
                vm.followUpInfo.isFranchiseeLevel = true;
                vm.statusOptions = [];
                vm.statusOptions.push({ display: 'Open', value: 231 });
                vm.statusOptions.push({ display: 'In-Progress', value: 232 });
                vm.statusOptions.push({ display: 'Completed', value: 233 });
                vm.statusOptions.push({ display: 'Retired', value: 234 });
                vm.updateToDoByStatus = updateToDoByStatus;
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
                function updateToDoByStatus(toDo) {
                    $scope.editMode = false;
                    vm.isProcessing = true;
                    if (toDo.statusId == 231) {
                        toDo.status = 'Open';
                    }
                    else if (toDo.statusId == 232) {
                        toDo.status = 'In-Progress';
                    }
                    else if (toDo.statusId == 233) {
                        toDo.status = 'Completed';
                    }
                    else if (toDo.statusId == 234) {
                        toDo.status = 'Retired';
                    }
                    toDo.actualDate = moment((toDo.date)).format("MM/DD/YYYY");
                    toDo.task = toDo.toDo;
                    return toDoService.saveCommentToDoByStatus(toDo).then(function (result) {
                        if (result.data != true)
                            toaster.error("Error in Updating Status Successfully");
                        else
                            toaster.show("Status Updated Successfully");

                        //$scope.editMode = !($scope.editMode);
                        //getToDo();
                        vm.isProcessing = false;
                    });
                }
            }]);
}());