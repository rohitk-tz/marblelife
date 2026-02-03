'use strict';
var SchedulerConfiguration = function () {
    return {
        moduleName: "scheduler"
    };
}();

(function () {
    // Use application configuration module to register a new module
    ApplicationConfiguration.registerModule(SchedulerConfiguration.moduleName);

    angular.module(SchedulerConfiguration.moduleName).config(["$stateProvider", "$urlRouterProvider", routeConfig]);

    var ngSchedulerConfiguration = angular.module(SchedulerConfiguration.moduleName)
    ngSchedulerConfiguration.config(['$compileProvider', function ($compileProvider) {
        //if ($compileProvider.imgSrcSanitizationWhitelist) $compileProvider.imgSrcSanitizationWhitelist(/^\s*(https?|ftp|mailto|tel|webcal|local|file|data|blob):/);
        if ($compileProvider.aHrefSanitizationWhitelist) $compileProvider.aHrefSanitizationWhitelist(/^\s*(https?|ftp|mailto|tel|webcal|local|file|data|blob|geo):/);
    }]);

    /* @ngInject */
    function routeConfig($stateProvider, $urlRouterProvider) {
        // Setup the apps routes

        var viewsPathPrefix = '/modules/scheduler/views/';
        $stateProvider
            .state('core.layout.scheduler', {
                url: '/scheduler',
                template: '<div ui-view></div>',
                controller: ['$state', function ($state) {
                    if ($state.current.name == 'core.layout.scheduler') {
                        $state.go('core.layout.scheduler.manage');
                    }
                }],
            })
            .state('core.layout.scheduler.manage', {
                url: '/manage/:franchiseeId',
                templateUrl: viewsPathPrefix + 'scheduler-calendar.client.view.html',
                controller: 'SchedulerController',
                params: { franchiseeId: null, previousView: null, startDate: null, nativateDate: null },
                controllerAs: 'vm'
            }).state('core.layout.scheduler.job', {
                url: '/:id/edit/:rowId',
                templateUrl: viewsPathPrefix + 'manage-job.client.view.html',
                controller: 'ManageJobController',
                params: { id: null, previousView: null, rowId: null },
                controllerAs: 'vm'
            })
            .state('core.layout.scheduler.list', {
                url: '/list/:franchiseeId',
                templateUrl: viewsPathPrefix + 'list-job.client.view.html',
                controller: 'ListSchedulerController',
                params: { franchiseeId: null, startDate: null, previousView: null, endDate: null },
                controllerAs: 'vm'
            })
            .state('core.layout.scheduler.estimate', {
                url: '/:id/manage/:rowId',
                templateUrl: viewsPathPrefix + 'manage-estimate.client.view.html',
                controller: 'ManageEstimateController',
                params: { id: null, previousView: null, rowId: null },
                controllerAs: 'vm'
            }).state('core.layout.scheduler.vacation', {
                url: '/:id/vacation',
                templateUrl: viewsPathPrefix + 'manage-vacation.client.view.html',
                controller: 'ManageVacationController',
                params: { id: null, previousView: null },
                controllerAs: 'vm'
            }).state('core.layout.scheduler.vacation1', {
                url: '/:id/vacation+',
                templateUrl: viewsPathPrefix + 'manage-vacation.client.view.html',
                controller: 'ManageVacationController',
                params: { id: null, previousView: null },
                controllerAs: 'vm'
            })

            .state('core.layout.scheduler.estimate11', {
                url: '/map/:customerAddress',
                templateUrl: viewsPathPrefix + 'scheduler-map-client-view.html',
                controller: 'SchedulerMapController',
                params: { customerAddress: null },
                controllerAs: 'vm'
            }).state('core.layout.scheduler.meeting', {
                url: '/:id/meeting',
                templateUrl: viewsPathPrefix + 'manage-meeting.client.view.html',
                controller: 'ManageMeetingController',
                params: { id: null, previousView: null },
                controllerAs: 'vm'
            }).state('core.layout.scheduler.meeting1', {
                url: '/:id/meeting+',
                templateUrl: viewsPathPrefix + 'manage-meeting.client.view.html',
                controller: 'ManageMeetingController',
                params: { id: null, previousView: null },
                controllerAs: 'vm'
            })

            .state('core.layout.scheduler.zip', {
                parent: 'core.layout.scheduler',
                url: '/zipcode/',
                templateUrl: viewsPathPrefix + 'manage-zip-locator.client.view.html',
                controller: 'ZipLocatorController',
                params: { franchiseeId: null, LogFileId: null, LogForCountyFileId: null },
                controllerAs: 'vm'
            }).state('core.layout.scheduler.mail', {
                url: '/mail/:franchiseeId',
                templateUrl: viewsPathPrefix + 'list-mail.client.view.html',
                controller: 'MailController',
                params: { franchiseeId: null },
                controllerAs: 'vm'
            }).state('core.layout.scheduler.beforeAfter', {
                url: '/beforeAfter/bestFitMark',
                templateUrl: viewsPathPrefix + 'before-after.client.view.html',
                controller: 'BeforeAfterBestMarkController',
                params: { franchiseeId: null },
                controllerAs: 'vm'
            }).state('core.layout.scheduler.beforeAfterFa', {
                url: '/beforeAfter/franchiseeAdmin',
                templateUrl: viewsPathPrefix + 'before-after-fa.client.view.html',
                controller: 'BeforeAfterForFAController',
                params: { franchiseeId: null, isBeforeAfterTabActive: false },
                controllerAs: 'vm'
            }).state('core.layout.scheduler.todoJobEstimate', {
                url: '/scheduler/todoList',
                templateUrl: viewsPathPrefix + 'to-do-job-estimate.controller.client.view.html',
                controller: 'ToDoJobEstimateController',
                params: { franchiseeId: null },
                controllerAs: 'vm'
            }).state('core.layout.scheduler.esignature', {
                url: '/scheduler/esignature',
                templateUrl: viewsPathPrefix + 'esignature.client.view.html',
                controller: 'eSignatureController',
                params: { franchiseeId: null },
                controllerAs: 'vm'
            })

    }
}());
