'use strict';
var ReportsConfiguration = function () {
    return {
        moduleName: "report"
    };
}();

(function () {

    // Use application configuration module to register a new module
    ApplicationConfiguration.registerModule(ReportsConfiguration.moduleName);

    angular.module(ReportsConfiguration.moduleName).config(["$stateProvider", "$urlRouterProvider", routeConfig]);

    /* @ngInject */
    function routeConfig($stateProvider, $urlRouterProvider) {
        // Setup the apps routes

        var viewsPathPrefix = '/modules/reports/views/';

        $stateProvider
            .state('core.layout.report', {
                url: '/report',
                //abstract: true,
                template: '<div ui-view></div>',
                controller: ['$state', function ($state) {
                    if ($state.current.name == 'core.layout.reports') {
                        $state.go('core.layout.report.list');
                    }
                }],
            })
            .state('core.layout.report.list', {
                url: '/list',
                templateUrl: viewsPathPrefix + 'list-sales-report.client.view.html',
                controller: 'ListReportController',
                controllerAs: 'vm'
            })
            .state('core.layout.report.lateFees', {
                url: '/lateFee',
                templateUrl: viewsPathPrefix + 'list-lateFees-report.client.view.html',
                controller: 'ListLateFeesController',
                controllerAs: 'vm'
            })
            .state('core.layout.report.email', {
                url: '/customer/email',
                templateUrl: viewsPathPrefix + 'list-customer-email-report.client.view.html',
                controller: 'CustomerEmailReportController',
                controllerAs: 'vm'
            })
            .state('core.layout.report.feedback', {
                url: '/customer/feedback',
                templateUrl: viewsPathPrefix + 'list-customer-feedback.client.view.html',
                controller: 'CustomerFeedbackController',
                controllerAs: 'vm'
            })
            .state('core.layout.report.callDetail', {
                url: '/callDetail',
                templateUrl: viewsPathPrefix + 'list-callDetail.client.view.html',
                controller: 'MarketingLeadCallDetailController',
                controllerAs: 'vm'
            })
         .state('core.layout.report.webLead', {
             url: '/webLead',
             templateUrl: viewsPathPrefix + 'list-webLead.client.view.html',
             controller: 'WebLeadController',
             controllerAs: 'vm'
         })
         .state('core.layout.report.service', {
             url: '/service',
             templateUrl: viewsPathPrefix + 'report-service.client.view.html',
             controller: 'ServiceReportController',
             controllerAs: 'vm'
         })
            .state('core.layout.report.marketingClass', {
                url: '/marketingClass',
                templateUrl: viewsPathPrefix + 'report-marketingClass.client.view.html',
                controller: 'MarketingClassReportController',
                controllerAs: 'vm'
            })
            .state('core.layout.report.growth', {
                url: '/growth',
                templateUrl: viewsPathPrefix + 'report-growth.client.view.html',
                controller: 'GrowthReportController',
                controllerAs: 'vm'
            })
            .state('core.layout.report.batch', {
                url: '/batch',
                templateUrl: viewsPathPrefix + 'report-upload-batch.client.view.html',
                controller: 'UploadBatchReportController',
                controllerAs: 'vm'
            }).state('core.layout.report.phoneLabel', {
                url: '/phoneLabel',
                templateUrl: viewsPathPrefix + 'manage-phoneLabel.client.view.html',
                controller: 'ManagePhoneLabelController',
                controllerAs: 'vm'
            }).state('core.layout.report.product', {
                url: '/product/channel',
                templateUrl: viewsPathPrefix + 'report-product-channel.client.view.html',
                controller: 'ProductChannelReportController',
                controllerAs: 'vm'
            }).state('core.layout.report.callDetailAnalysis', {
                url: '/callDetail/analysis',
                templateUrl: viewsPathPrefix + 'analysis-callDetail.client.view.html',
                controller: 'CallDetailAnalysisController',
                controllerAs: 'vm'
            })
         .state('core.layout.report.webLeadAnalysis', {
             url: '/webLead/analysis',
             templateUrl: viewsPathPrefix + 'analysis-webLead.client.view.html',
             controller: 'WebLeadAnalysisController',
             controllerAs: 'vm'
         })
           
        .state('core.layout.report.arReport', {
            url: '/arReport',
            templateUrl: viewsPathPrefix + 'report-ar-report.client.view.html',
            controller: 'ARReportController',
            controllerAs: 'vm'
        }).state('core.layout.report.franchiseeMails', {
            url: '/franchisee/mails',
            templateUrl: viewsPathPrefix + 'list-customer-email-report.client.view.html',
            controller: 'FranchiseeMailsController',
            params: { franchiseeId: null },
            controllerAs: 'vm'
        }).state('core.layout.report.franchiseeListfranchiseeAdmin', {
            url: '/franchisee/list',
            templateUrl: viewsPathPrefix + 'list-franchisee-for-franchisee-admin.client.view.html',
            controller: 'ListFranchiseeForFranchiseeAdminController',
            params: { franchiseeId: null },
            controllerAs: 'vm'
        }).state('core.layout.report.leadPerformanceReport', {
            url: '/leadperformance',
            templateUrl: viewsPathPrefix + 'lead-performance-report.client.view.html',
            controller: 'LeadPerformanceReportController',
            controllerAs: 'vm'
        }).state('core.layout.report.mlfs', {
            url: '/mlfsReport',
            templateUrl: viewsPathPrefix + 'view-mlfs-report.client.view.html',
            controller: 'MLFSReportController',
            controllerAs: 'vm'
        }).state('core.layout.report.mlfsConfiguration', {
            url: '/mlfsConfiguration',
            templateUrl: viewsPathPrefix + 'mlfs-configuration.client.view.html',
            controller: 'MLFSConfigurationController',
            controllerAs: 'vm'
        }).state('core.layout.report.homeAdvisor', {
            url: '/homeAdvisor',
            templateUrl: viewsPathPrefix + 'homes-advisor.client.view.html',
            controller: 'HomeAdvisorController',
            controllerAs: 'vm'
        }).state('core.layout.report.managePhoneCall', {
            url: '/manage/phoneCall',
            templateUrl: viewsPathPrefix + 'manage-phone-call.client.view.html',
            controller: 'ManagePhoneCallController',
            controllerAs: 'vm'
        }).state('core.layout.report.automationBackUp', {
            url: '/manage/automationBackUpCall',
            templateUrl: viewsPathPrefix + 'report.automation.backup.client.view.html',
            controller: 'AutomationBackUpCallController',
            controllerAs: 'vm'
        }).state('core.layout.report.managePriceEstimate', {
            url: '/manage/priceEstimate',
            templateUrl: viewsPathPrefix + 'manage-price-estimate.client.view.html',
            controller: 'ManagePriceEstimateController',
            controllerAs: 'vm'
        });
    }
}());
