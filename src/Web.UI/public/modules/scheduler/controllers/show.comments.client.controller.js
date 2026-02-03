(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("ShowCommentsController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "FranchiseeService", "ToDoService","Toaster",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, franchiseeService, toDoService, toaster) {

                var vm = this;
                vm.save = save;
                vm.commentInfo = {};
                vm.commentInfo.todoId = modalParam.ToDoId;
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };



                function save() {
                    return toDoService.saveCommentToDo(vm.commentInfo).then(function (result) {
                        if (result != null) {
                            vm.commentInfo = {};
                            toaster.show("Note Created Successfully!!");
                            vm.isAddingNew = false;
                            $uibModalInstance.dismiss();
                        }
                    });
                }

            }]);
}());