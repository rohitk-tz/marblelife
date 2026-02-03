'use strict';
var CoreConfiguration = function () {
    return {
        moduleName: "core"
    };
}();
(function () {    // Use application configuration module to register a new module
    ApplicationConfiguration.registerModule(CoreConfiguration.moduleName);
    angular.module(CoreConfiguration.moduleName).config(["$stateProvider", "$urlRouterProvider", routeConfig]);
    /* @ngInject */
    function routeConfig($stateProvider, $urlRouterProvider) {
        // Setup the apps routes
        var viewsPathPrefix = '/modules/core/views/';
        $stateProvider
            .state('core', {
                abstract: true,
                templateUrl: viewsPathPrefix + 'default.client.view.html',
                controller: 'DefaultLayoutController',
                controllerAs: 'vm',
                resolve: { factory: 'checkRouting' }
            })
            .state('core.layout', {
                abstract: true,
                views: {
                    header: {
                        templateUrl: viewsPathPrefix + 'header.client.view.html',
                        controller: 'HeaderController',
                        controllerAs: 'vm'
                    },
                    footer: {
                        templateUrl: viewsPathPrefix + 'footer.client.view.html',
                        controller: 'FooterController',
                        controllerAs: 'vm'
                    },
                    content: {
                        template: '<div id="admin-panel-content-view" ui-view></div>'
                    }
                }
            })
            .state('core.layout.home', {
                url: '/dashboard',
                templateUrl: viewsPathPrefix + 'home.client.view.html',
                controller: "HomeController",
                controllerAs: 'vm',
                resolve: { factory: 'checkRouting' }
            })
            .state('core.layout.pageNotFound', {
                url: '/error404',
                templateUrl: viewsPathPrefix + 'page.not.found.html',
                controllerAs: 'vm'
            });
        // set default routes when no path specified
        $urlRouterProvider.when('', '/dashboard');
        $urlRouterProvider.when('/', '/dashboard');
        $urlRouterProvider.otherwise('/dashboard');
    }
}());
