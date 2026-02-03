(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("ShowFollowUpController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "FranchiseeService", "ToDoService","Toaster",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, franchiseeService, toDoService, toaster) {

                var vm = this;
                vm.save = save;
                vm.followUpInfo = {};
                vm.toDoList = toDoList;
                vm.followUpInfo = modalParam.CustomerInformation;
                vm.followUpInfo.franchiseeId = modalParam.FranchiseeId;
                vm.franchiseeId = modalParam.FranchiseeId;
                vm.franchiseeName = modalParam.Franchisee;
                vm.followUpInfo.customerId = modalParam.CustomerInformation.customerId;
                vm.followUpInfo.schedulerId = modalParam.JobSchedulerId;
                vm.followUpInfo.franchiseeId = vm.franchiseeId;
                vm.followUpInfo.statusId = 231;
                vm.followUpInfo.typeId = 33;
                vm.followUpInfo.isFranchiseeLevel = true;
                vm.followUpInfo.task = '';
                vm.followUpInfo.comment = '';
                vm.followUpInfo.date = '';
                $scope.$broadcast("reset-dates");
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };

                function toDoList(text) {
                    return toDoService.toDoList(text).then(function (result) {
                        if (result != null) {
                            return result.data;
                        }
                    });
                }

                function save() {
                    if (vm.followUpInfo.date == null || vm.followUpInfo.date == '') {
                        toaster.error("Please Enter Date!!");
                        return;
                    }
                    vm.followUpInfo.actualDate = moment((vm.followUpInfo.date)).format("MM/DD/YYYY");
                    return toDoService.saveToDo(vm.followUpInfo).then(function (result) {
                        if (result != null) {
                            toaster.show("Follow Up Created Successfully!!");
                            vm.followUpInfos = {};
                            vm.isAddingNew = false;
                            $uibModalInstance.dismiss();
                        }
                    });
                }

            }]);
}());