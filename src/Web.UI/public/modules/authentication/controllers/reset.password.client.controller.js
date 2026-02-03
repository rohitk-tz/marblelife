(function () {
    'use strict';

    angular.module(AuthenticationConfiguration.moduleName).controller("ResetPasswordController", ["$scope", "$state", "$stateParams", "$rootScope", "UserAuthenticationService", "APP_CONFIG",
        function ($scope, $state, $stateParams, $rootScope, UserAuthenticationService, cfg) {
            var vm = this;
            vm.user = { Password: '', ConfirmPassword: '' };
            vm.reset = reset;

            function reset() {
                vm.user.key = $stateParams.token;
                UserAuthenticationService.resetPassword(vm.user).then(function (response) {
                    if (response.data == true) {
                        $state.go("authentication.login");
                    }
                });
            };

            function checkResetExpire() {
                vm.user.key = $stateParams.token;
                UserAuthenticationService.resetPasswordExpire(vm.user).then(function (response) {
                    if (response.data != true) {
                        //logger.showToasterError("Link has been expired.Please try again");
                        $state.go("authentication.invalidLink");
                    }
                });
            }

            checkResetExpire();
        }]);

}());