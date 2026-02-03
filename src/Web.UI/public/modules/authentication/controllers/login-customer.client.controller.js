(function () {
    'use strict';

    angular.module(AuthenticationConfiguration.moduleName).controller("LoginCustomerController", ["$scope", "$state", "$rootScope", "UserAuthenticationService", "APP_CONFIG",
        function ($scope, $state, $rootScope, UserAuthenticationService, cfg) {
            var vm = this;
            vm.isProcessing = false;
            vm.user = {
                code: ''
            };

            vm.login = login;
            vm.forgetPassword = forgetPassword;
            vm.enterKey = enterKey;
            function login() {
                vm.isProcessing = true;
                return UserAuthenticationService.loginCustomer(vm.user.code).then(function (result) {
                    var sessionId = result.data;
                    vm.isProcessing = false;

                    if (sessionId != null) {
                        return UserAuthenticationService.getUserIdentity(sessionId).then(function (result) {
                            var data = result.data;
                            if (data != null) {
                                UserAuthenticationService.setTokenAndIdentity($rootScope, data, sessionId);
                                $state.go('core.layout.scheduler.esignature');
                                //setTimeout(function () {
                                //    window.location.reload(true);
                                //}, 2000);

                            }

                        });
                    }
                }).catch(function (err) {
                    vm.isProcessing = false;
                });
            }
            function enterKey() {
                if (vm.user.userName != "" && vm.user.password != "") {
                    vm.login();
                }
            }
            function forgetPassword() {
                $state.go('authentication.forget');
            }

        }]);

}());