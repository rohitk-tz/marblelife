(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("ViewCommentsController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "FranchiseeService", "ToDoService",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, franchiseeService, toDoService) {

                var vm = this;
                vm.commentInfo = {};
                vm.getComment = getComment;
                vm.commentInfo = {};
                vm.commentInfo.todoId = modalParam.ToDoId;
                vm.todoComments = [];
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };


                function getComment() {
                    return toDoService.getCommentToDo(vm.commentInfo.todoId).then(function (result) {
                        if (result != null) {
                            vm.todoComments = result.data.commentList;
                        }
                    });
                }
                getComment();

            }]);
}());