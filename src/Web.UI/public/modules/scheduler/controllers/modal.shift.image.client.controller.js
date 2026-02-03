(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("ShiftImageController",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "SchedulerService", "Toaster",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, schedulerService, toaster) {



                var vm = this;
                //vm.save = save;
                vm.query = {};
                vm.imageShiftType = [{ display: "Building Exterior", value: 121 }, { display: "Invoice/ Drawing", value: 122 }];
                $scope.editMode = false;
                vm.shiftImagesModel = modalParam.ShiftImagesModel;
                vm.imagesInfo = modalParam.ImagesInfo;
                $scope.$broadcast("isShiftSave", false);
                vm.shiftImages = shiftImages;
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
                function shiftImages() {

                    $scope.$broadcast("isShiftSave", true);
                    $rootScope.$emit('isShiftSave', true);
                    $uibModalInstance.dismiss();
                }
            }]);
}());