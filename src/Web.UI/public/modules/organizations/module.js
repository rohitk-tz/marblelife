'use strict';
var OrganizationsConfiguration = function () {
    return {
        moduleName: "organization"
    };
}();

(function () {

    // Use application configuration module to register a new module
    ApplicationConfiguration.registerModule(OrganizationsConfiguration.moduleName);

    angular.module(OrganizationsConfiguration.moduleName).config(["$stateProvider", "$urlRouterProvider", routeConfig]);

    /* @ngInject */
    function routeConfig($stateProvider, $urlRouterProvider) {
        // Setup the apps routes

        var viewsPathPrefix = '/modules/organizations/views/';

        $stateProvider
            .state('core.layout.franchisee', {
                url: '/franchisee',
                //abstract: true,
                template: '<div ui-view></div>',
                controller: ['$state', function ($state) {
                    if ($state.current.name == 'core.layout.franchisee') {
                        $state.go('core.layout.franchisee.list');
                    }
                }],
            })
            .state('core.layout.franchisee.list', {
                url: '/list',
                templateUrl: viewsPathPrefix + 'list-franchisee.client.view.html',
                params: { franchiseeId: null },
                controller: 'ListFranchiseeController',
                controllerAs: 'vm'
            })
            .state('core.layout.franchisee.create', {
                url: '/create',
                templateUrl: viewsPathPrefix + 'edit-franchisee.client.view.html',
                controller: 'CreateFranchiseeController',
                controllerAs: 'vm'
            })
            .state('core.layout.franchisee.edit', {
                url: '/:id/edit',
                templateUrl: viewsPathPrefix + 'edit-franchisee.client.view.html',
                controller: 'EditFranchiseeController',
                controllerAs: 'vm'
            })
            .state('core.layout.franchisee.document', {
                url: '/list/document/:franchiseeId',
                templateUrl: viewsPathPrefix + 'list-documents.client.view.html',
                controller: 'ManageDocumentsController',
                controllerAs: 'vm',
                params: { franchiseeId: null },
            }).state('core.layout.franchisee.national', {
                url: '/list/document/national/:franchiseeId',
                templateUrl: viewsPathPrefix + 'list-document-national.client.view.html',
                controller: 'NationalDocumentsController',
                controllerAs: 'vm',
                params: { franchiseeId: null },
            }).state('core.layout.franchisee.lists', {
                url: '/list/franchiseeDirectoryList',
                templateUrl: viewsPathPrefix + 'list-franchisee.directory.client.view.html',
                params: { franchiseeId: null },
                controller: 'ListFranchiseeDirectoryController',
                controllerAs: 'vm'
            }).state('core.layout.franchisee.documentReport', {
                url: '/list/document',
                templateUrl: viewsPathPrefix + 'report-franchisee-document-upload.client.view.html',
                params: { franchiseeId: null },
                controller: 'ReportFranchiseeDocumentController',
                controllerAs: 'vm'
            });
    }
}());
