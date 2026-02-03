'use strict';
var AuthenticationConfiguration = function () {
    return {
        moduleName: "authentication"
    };
}();

(function () {

    // Use application configuration module to register a new module
    ApplicationConfiguration.registerModule(AuthenticationConfiguration.moduleName);

    angular.module(AuthenticationConfiguration.moduleName).config(["$stateProvider", "$urlRouterProvider", routeConfig]);

    /* @ngInject */
    function routeConfig($stateProvider, $urlRouterProvider) {
        // Setup the apps routes
        var viewsPathPrefix = '/modules/authentication/views/';

        $stateProvider
            .state('authentication', {
                abstract: true,
                templateUrl: viewsPathPrefix + 'default.client.view.html',
                controller: 'AuthenticationLayoutController',
                controllerAs: 'vm'
            })
            .state('authentication.login', {
                url: '/login',
                templateUrl: viewsPathPrefix + 'login.client.view.html',
                controller: 'LoginController',
                controllerAs: 'vm'
            })
            .state('authentication.loginCustomer', {
                url: '/login/customer',
                templateUrl: viewsPathPrefix + 'login-customer.client.view.html',
                controller: 'LoginCustomerController',
                controllerAs: 'vm'
            })
            .state('authentication.forget', {
                url: '/password/forget',
                templateUrl: viewsPathPrefix + 'forget-password.client.view.html',
                controller: 'ForgetPasswordController',
                controllerAs: 'vm'
            })
            .state('authentication.reset', {
                url: '/password/reset/:token',
                templateUrl: viewsPathPrefix + 'reset-password.client.view.html',
                controller: 'ResetPasswordController',
                controllerAs: 'vm'
            })
            .state('authentication.confirmation', {
                url: '/confirmation/:id/:status',
                templateUrl: viewsPathPrefix + 'confirmation.client.view.html',
                controller: 'ConfirmationController',
                controllerAs: 'vm'
            })
            .state('authentication.invalidLink', {
                url: '/invalidLink',
                templateUrl: viewsPathPrefix + 'invalid-link.client.view.html',
                //controller: 'ResetPasswordController',
                controllerAs: 'vm'
            })
                //.state('authentication.pageNotFound', {
                //    url: '/error404',
                //    templateUrl: viewsPathPrefix + 'page.not.found.html',
                //    controllerAs: 'vm'
                //}
         //);
    }
}());
