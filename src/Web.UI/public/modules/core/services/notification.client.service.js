(function () {

    angular.module(CoreConfiguration.moduleName).factory("Notification", ["$rootScope", "$uibModal",
        function ($rootScope, $uibModal) {

            var showValidations = function (validation) {

                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'modules/core/views/validations-alert-dialog.client.view.html',
                    controller: 'ValidationsController',
                    controllerAs: 'vm',
                    size: 'lg',
                    backdrop: 'static',
                    resolve: {
                        data: function () {
                            return {
                                ValidationType: $rootScope.Validation,
                                validation: validation
                            }
                        }
                    }
                });

                modalInstance.result.then(function () {
                }, function () {
                });
            };

            var showAlert = function (message, onClose) {

                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: '/modules/core/views/alert-dialog.client.view.html',
                    controller: 'AlertDialogController',
                    controllerAs: 'vm',
                    size: 'sm',
                    backdrop: 'static',
                    resolve: {
                        message: function () {
                            return message;
                        }
                    }
                });

                modalInstance.result.then(function () {
                    if (onClose != null)
                        onClose();
                }, function () {
                });

            };

            var showLogoutWarning = function (timeLeft, onClose) {

                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: '/modules/core/views/alert-dialog.client.view.html',
                    controller: ["data", "$scope", "$timeout", "$uibModalInstance", function (data, $scope, $timeout, $uibModalInstance) {
                        var vm = this;
                        vm.timeLeft = Number(data);
                        vm.message = getMessage();

                        vm.interval = setInterval(function () {
                            vm.message = getMessage();

                            if (vm.timeLeft == 0) {
                                clearInterval(vm.interval);
                                vm.cancel();
                            }

                            $timeout(function () {
                                $scope.$apply();
                            });

                        }, 1000);

                        vm.cancel = function () {
                            $uibModalInstance.close();
                        };

                        function getMessage() {
                            return "Your session will log out in " + (vm.timeLeft--) + " seconds. Please click here to keep working.";
                        }

                    }],
                    controllerAs: 'vm',
                    size: 'sm',
                    backdrop: 'static',
                    resolve: {
                        data: function () {
                            return timeLeft;
                        }
                    }
                });

                modalInstance.result.then(function () {
                    if (onClose != null)
                        onClose();
                });

                return modalInstance;
            };

            var showConfirm = function (message, title, onOk, onCancel) {

                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: '/modules/core/views/confirm-dialog.client.view.html',
                    controller: 'ConfirmDialogController',
                    controllerAs: 'vm',
                    size: 'sm',
                    backdrop: 'static',
                    resolve: {
                        data: function () {
                            return {
                                message: message,
                                title: title
                            };
                        }
                    }
                });

                modalInstance.result.then(function () {
                    if (onOk != null)
                        onOk();
                }, function () {
                    if (onCancel != null)
                        onCancel();
                });
            };

            return {
                showValidations: showValidations,
                showAlert: showAlert,
                showConfirm: showConfirm,
                showLogoutWarning: showLogoutWarning
            };
        }]);

}());