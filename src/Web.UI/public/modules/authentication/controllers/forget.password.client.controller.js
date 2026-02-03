(function () {
    'use strict';

    angular.module(AuthenticationConfiguration.moduleName).controller("ForgetPasswordController",
        ["$scope", "$state", "$rootScope", "UserAuthenticationService", "APP_CONFIG", "Toaster",
        function ($scope, $state, $rootScope, UserAuthenticationService, cfg, toaster) {
            var vm = this;
            vm.goToLogin = goToLogin;
            vm.sendLink = sendLink;
            vm.goToResetPassword = goToResetPassword;
            vm.model = { email: '' };

            function goToLogin() {
                $state.go("authentication.login");
            }

            function sendLink(model) {
                if (model == undefined || model.email == null) {
                    return;
                }
                UserAuthenticationService.forgotPassword(model.email).then(function (data) {
                    if (data.data == true) {
                        toaster.show(data.message.message);
                        goToLogin();
                    }

                })
            };
            function goToResetPassword() {
                $state.go("authentication.reset");
            }

        }]);

}());