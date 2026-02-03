(function () {
    'use strict';
    angular.module(AuthenticationConfiguration.moduleName).service("UserAuthenticationService",
        ["HttpWrapper", "$cookies", "APP_CONFIG", "$rootScope",
        function (httpWrapper, $cookies, config, $rootScope) {

            var login = function (userName, password) {
                var model = { userName: userName, password: password };
                return httpWrapper.post({
                    url: "/user/login",
                    data: model,
                    showOnSuccess: false
                });
            };

            var logout = function () {
                return httpWrapper.get({
                    url: "/user/logout",
                    showOnSuccess: false,
                    showOnFailure: false
                }).then(function (result) {
                    setTokenAndIdentity($rootScope, null);
                    return result;
                });
            };

            var getUserIdentity = function (sessionId) {
                return httpWrapper.get({
                    url: "/users/identity/" + sessionId,
                    showOnSuccess: false,
                    showOnFailure: false
                });
            };

            function getUserSessionId(callback) {
                var sessionId = $cookies.get(config.clientTokenName);              
                getUserIdentity(sessionId).then(function (result) {
                    var data = result.data;
                    if (data != null) {
                        setTokenAndIdentity($rootScope, data, sessionId);                       
                    }
                    if (callback != null) {
                        callback();
                    }
                });

            }


            var setTokenAndIdentity = function ($rootScope, identity, token) {
                if (identity == null) {
                    $cookies.remove(config.clientTokenName);
                    $rootScope.identity = null;
                }
                else {
                    $cookies.put(config.clientTokenName, token);
                    $rootScope.identity = identity;
                    $rootScope.$broadcast('identity');
                }
            };

            function forgotPassword(email) {
                return httpWrapper.post({ url: "/users/login/SendPasswordLink?email=" + email });
            }

            function resetPassword(user) {
                return httpWrapper.post({ url: "/users/login/ResetPassword", data: user });
            }

            function resetPasswordExpire(data) {
                return httpWrapper.post({ url: "/users/login/ResetPasswordExpire", data: data });
            }

            var loginCustomer = function (code) {
                var model = { Code: code };
                return httpWrapper.post({
                    url: "/user/login/customer",
                    data: model,
                    showOnSuccess: false
                });
            };

            return {
                login: login,
                logout: logout,
                getUserIdentity: getUserIdentity,
                setTokenAndIdentity: setTokenAndIdentity,
                forgotPassword: forgotPassword,
                resetPasswordExpire: resetPasswordExpire,
                resetPassword: resetPassword,
                getUserSessionId: getUserSessionId,
                loginCustomer: loginCustomer
            };

        }]);
}());
