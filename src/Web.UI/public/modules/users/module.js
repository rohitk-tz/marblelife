'use strict';
var UsersConfiguration = function () {
    return {
        moduleName: "user"
    };
}();
(function () {
    // Use application configuration module to register a new module
    ApplicationConfiguration.registerModule(UsersConfiguration.moduleName);
    angular.module(UsersConfiguration.moduleName).config(["$stateProvider", "$urlRouterProvider", routeConfig]);
    /* @ngInject */
    function routeConfig($stateProvider, $urlRouterProvider) {
        // Setup the apps routes
        var viewsPathPrefix = '/modules/users/views/';
        $stateProvider
            .state('core.layout.user', {
                url: '/users',
                //abstract: true,
                template: '<div ui-view></div>',
                controller: ['$state', function ($state) {
                    if ($state.current.name == 'core.layout.user') {
                        $state.go('core.layout.user.list');
                    }
                }]
            })
            .state('core.layout.user.list', {
                url: '/list/:franchiseeId',
                templateUrl: viewsPathPrefix + 'list-user.client.view.html',
                controller: 'ListUserController',
                controllerAs: 'vm',
                params: { franchiseeId: null ,query:null},
            })
            .state('core.layout.user.create', {
                url: '/create/:franchiseeId',
                templateUrl: viewsPathPrefix + 'edit-user.client.view.html',
                controller: 'CreateUserController',
                controllerAs: 'vm',
                params: { franchiseeId: null },
            })
            .state('core.layout.user.edit', {
                url: '/:id/edit/:franchiseeId',
                templateUrl: viewsPathPrefix + 'edit-user.client.view.html',
                controller: 'EditUserController',
                controllerAs: 'vm',
                params: { franchiseeId: null, query: null},
            });
        $urlRouterProvider.when('', '/error404');
        $urlRouterProvider.when('/', '/error404');
        $urlRouterProvider.otherwise('/error404');
    }
}());
