(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("EstimateWorthController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "SchedulerService", "Toaster",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, schedulerService, toaster) {



                var vm = this;
                //vm.save = save;
                vm.query = {};
                $scope.editMode = false;
                vm.franchisee = modalParam.Franchisee;
                vm.franchiseeId = modalParam.FranchiseeId;
                vm.query.jobSchedulerId = modalParam.JobSchedulerId;
                vm.query.worth = modalParam.Worth != null ? modalParam.Worth : 0;

                vm.updateToDoByStatus = updateToDoByStatus;
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
                function updateToDoByStatus(toDo) {


                    return schedulerService.saveEstimateWorth(vm.query).then(function (result) {
                        if (result.data != true)
                            toaster.error("Error in Saving Estimate Worth Successfully");
                        else {
                            toaster.show("Estimate Worth Saved Successfully");
                            $uibModalInstance.dismiss();
                        }
                        vm.isProcessing = false;
                    });
                }
            }]);
}());