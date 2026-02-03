'use strict';
var SalesConfiguration = function () {
    return {
        moduleName: "sales"
    };
}();

(function () {

    // Use application configuration module to register a new module
    ApplicationConfiguration.registerModule(SalesConfiguration.moduleName);

    angular.module(SalesConfiguration.moduleName).config(["$stateProvider", "$urlRouterProvider", routeConfig]);

    /* @ngInject */
    function routeConfig($stateProvider, $urlRouterProvider) {
        // Setup the apps routes

        var viewsPathPrefix = '/modules/sales/views/';

        $stateProvider
            .state('core.layout.sales', {
                url: '/sales',
                //abstract: true,
                template: '<div ui-view></div>',
                controller: ['$state', function ($state) {
                    if ($state.current.name == 'core.layout.sales') {
                        $state.go('core.layout.sales.list');
                    }
                }],
            })
            .state('core.layout.sales.list', {
                url: '/list/batch/:salesDataUploadId',
                templateUrl: viewsPathPrefix + 'list-sales.client.view.html',
                controller: 'ListSalesController',
                params: { salesDataUploadId: null, franchiseeId: null },
                controllerAs: 'vm'
            })
            .state('core.layout.sales.customerSales', {
                url: '/list/customer/:customerId',
                templateUrl: viewsPathPrefix + 'list-sales.client.view.html',
                controller: 'ListSalesController',
                params: { customerId: null },
                controllerAs: 'vm'
            })
            .state('core.layout.sales.batch', {
                url: '/batch',
                template: '<div ui-view></div>',
                controller: ['$state', function ($state) {
                    if ($state.current.name == 'core.layout.sales.batch') {
                        $state.go('core.layout.sales.batch.list');
                    }
                }]
            })
            .state('core.layout.sales.batch.list', {
                url: '/list',
                templateUrl: viewsPathPrefix + 'list-batch.client.view.html',
                controller: 'ListBatchController',
                controllerAs: 'vm'
            })
            .state('core.layout.sales.customer', {
                url: '/customer',
                templateUrl: viewsPathPrefix + 'list-customer.client.view.html',
                controller: 'ListCustomerController',
                controllerAs: 'vm'
            }).state('core.layout.sales.customerEdit', {
                url: '/:id/edit',
                templateUrl: viewsPathPrefix + 'edit-customer.client.view.html',
                controller: 'EditCustomerController',
                params: { id: null },
                controllerAs: 'vm'
            })
            .state('core.layout.sales.invoice', {
                url: '/invoice/:salesDataUploadId',
                templateUrl: viewsPathPrefix + 'list-invoice.client.view.html',
                controller: 'ListInvoiceController',
                params: { salesDataUploadId: null },
                controllerAs: 'vm'
            })
            .state('core.layout.sales.royalty', {
                url: '/royalty/{salesDataUploadId}',
                templateUrl: viewsPathPrefix + 'list-batch-sales.client.view.html',
                controller: 'ListBatchSalesController',
                controllerAs: 'vm'
            })
            //.state('core.layout.sales.batch.upload', {
            //    url: '/upload',
            //    templateUrl: viewsPathPrefix + 'upload-batch.client.view.html',
            //    controller: 'UploadBatchController',
            //    controllerAs: 'vm'
            //})
            .state('core.layout.sales.batch.log', {
                url: '/logs',
                templateUrl: viewsPathPrefix + 'view-logs.client.view.html',
                controller: 'BatchLogsController',
                controllerAs: 'vm'
            }).state('core.layout.sales.credit', {
                url: '/account/credit',
                templateUrl: viewsPathPrefix + 'list-franchisee-account-credit.client.view.html',
                controller: 'ListFranchiseeAccountCreditController',
                controllerAs: 'vm'
            }).state('core.layout.sales.franchisee', {
                url: '/account/credit/:franchiseeId',
                templateUrl: viewsPathPrefix + 'list-account-credit.client.view.html',
                controller: 'ListAccountCreditController',
                controllerAs: 'vm'
            })
            .state('core.layout.sales.annual', {
                url: '/annual/list',
                templateUrl: viewsPathPrefix + 'list-annual-batch.client.view.html',
                controller: 'ListAnnualBatchController',
                controllerAs: 'vm'
            })
            .state('core.layout.sales.details', {
                url: '/detail/:annualDataUploadId',
                templateUrl: viewsPathPrefix + 'list-annual-sales.client.view.html',
                controller: 'ListAnnualSalesController',
                params: { annualDataUploadId: null, franchiseeId: null },
                controllerAs: 'vm'
            })
            .state('core.layout.sales.audit', {
                url: '/audit/:invoiceId',
                templateUrl: viewsPathPrefix + 'detail-audit-invoice.client.view.html',
                controller: 'AuditInvoiceDetailController',
                params: { invoiceId: null, auditInvoiceId: null, annualUploadId: null },
                controllerAs: 'vm'
            })
            .state('core.layout.sales.annualcustomer', {
                url: '/annual/Customer/list',
                templateUrl: viewsPathPrefix + 'list-annual-batch-address.client.view.html',
                controller: 'ListAnnualBatchAddressController',
                controllerAs: 'vm'
            })
            .state('core.layout.sales.macroSales', {
                url: '/macroFunnel/National/list',
                templateUrl: viewsPathPrefix + 'sales-macro-funnel-national.client.view.html',
                controller: 'SalesMacroFunnelNationalController',
                controllerAs: 'vm'
            })
            .state('core.layout.sales.macroSalesLocal', {
                url: '/macroFunnel/local/list',
                templateUrl: viewsPathPrefix + 'sales-micro-funnel-local.client.view.html',
                controller: 'SalesMacroFunnelLocalController',
                controllerAs: 'vm'
            }).state('core.layout.sales.updateMarketingClass', {
                url: '/update/marketingClass',
                templateUrl: viewsPathPrefix + 'manage-update-marketingclass.client.view.html',
                controller: 'UpdateMarketingClassController',
                controllerAs: 'vm'
            })
            .state('core.layout.sales.downloadInProgress', {
                url: '/downloadInProgress?franchiseeId?text?pageNumber?pageSize?sortOrder?sortPropName?dateCreated?dateModified?receiveNotification?toDate?fromDate?advancedText?advancedSearchBy',
                templateUrl: viewsPathPrefix + 'download-in-progress.client.view.html',
                controller: 'DownloadInProgressController',
                controllerAs: 'vm',
                params: {
                    'franchiseeId': null,
                    'text': null,
                    'pageNumber': null,
                    'pageSize': null,
                    'sortOrder': null,
                    'sortPropName': null,
                    'dateCreated': null,
                    'dateModified': null,
                    'receiveNotification': null,
                    'toDate': null,
                    'fromDate': null,
                    'advancedText': null,
                    'advancedSearchBy': null
                },
            });
    }

}());
