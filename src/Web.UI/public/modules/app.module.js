
'use strict';

var config = {
    appErrorPrefix: '[HT Error] ', //Configure the exceptionHandler decorator
    docTitle: 'Marble Life ',
    version: '0.0.0',
    apiUrl: '/api'
};

var providers = {};

var ApplicationConfiguration = (function () {
    var applicationModuleName = 'makalu';
    var applicationModuleVendorDependencies = ['ngAnimate', 'ngCookies', 'ngSanitize', 'ui.router', 'ui.bootstrap', 'ngFileUpload', 'amChartsDirective', 'ui.slimscroll',
        'fixed.table.header', 'ngRateIt', 'angularjs-dropdown-multiselect', 'angularSpinner', 'ui.calendar', 'ui.bootstrap.datetimepicker', 'colorpicker.module', 'angular.circular.datetimepicker','ngImgCrop'];

    // Add a new vertical module
    var registerModule = function (moduleName) {
        // Create angular module
        angular.module(moduleName, []);

        // Add the module to the AngularJS configuration file
        angular.module(applicationModuleName).requires.push(moduleName);
        angular.module(moduleName).constant('APP_CONFIG', {
            apiUrl: config.apiUrl,
            clientTokenName: window.appTokenName,
            serverTokenName: 'Token',
            timezoneoffset: 'Timezoneoffset',
            timeZoneName :'TimeZoneName',
            defaultPageSize: 100,
            maxSlabsCount: 4,
            maxPhoneCount: 10,
            maxOccurenceCount: 10,
            defaultClassTypeId: 4,
            pagingOptions: [
                 { display: 1, value: 1 },
                { display: 5, value: 5 },
                { display: 10, value: 10 },
                { display: 15, value: 15 },
                { display: 20, value: 20 },
                { display: 25, value: 25 },
                { display: 50, value: 50 },
                { display: 100, value: 100 }
            ],
            loadMoreContents: 'load-more-contents',
            applyDateValidation: true,
            currencyExchangeRateReferenceSite: 'https://finance.yahoo.com/currency-converter',
            defaultCurrency: 'USD',
            primaryContact: 'Bruce Jordan',
            secondaryContact: 'Bob Heid',
            contactNumber: '407-330-6245',
            defaultServiceTypeIds: [{ id: 1 }, { id: 2 }, { id: 3 }, { id: 5 }, { id: 6 }, { id: 15 }],
            defaultClassTypeIds: [{ id: 1 }, { id: 2 }, { id: 3 }, { id: 4 }, { id: 12 }, { id: 13 }],
            defaultBookkeepingAmount: 250,
            defaultSalesPercentage: 2,
            defaultPayrollAmount: 50,
            defaultRecruitmentAmount: 19, //per person (tech/sales)
            defaultOneTimeProjectAmount: 100,
            defaultNationalChargePercentage: 10,
            defaultFrequency: 32,
        });

    };
    return {
        applicationModuleName: applicationModuleName,
        applicationModuleVendorDependencies: applicationModuleVendorDependencies,
        registerModule: registerModule
    };
})();


(function () {
    //Start by defining the main module and adding the module dependencies
    var app = angular.module(ApplicationConfiguration.applicationModuleName,
        ApplicationConfiguration.applicationModuleVendorDependencies);

    app.constant("BAR_COLOR", '#43689D');
    app.constant('PREFERENCE_KEY', {
        "LIST": "makalu-list-preference"
    });

    app.constant("PREFERENCE_TYPE", {
        "LIST": 4
    });

    app.config(['$httpProvider',
        function ($httpProvider) {
            $httpProvider.interceptors.push('customInterceptor');
            //$httpProvider.interceptors.push('loaderInterceptor');
        }]);

    app.factory("customInterceptor", ["APP_CONFIG", '$cookies', '$window', '$rootScope', function (cg, $cookies, $window, $rootScope) {
        return {
            request: function (cfg) {
                var cookie = $cookies.get(cg.clientTokenName);
                var version = $rootScope.version;

                if (version)
                    version = version.replace(/\./g, 'X');
                else
                    version = Math.random().toString(36).substr(2, 10);

                //Code to add some random string to HTML URLs, so that every time it fetches from server
                if (cfg.url.indexOf('.html') > -1
                    && cfg.url.indexOf('/views/') > -1
                    && cfg.url.indexOf('client.view') > -1) cfg.url += ('?p=' + version);
                //Done

                if (cookie == null && ($rootScope.UIStates || $rootScope.identity)) {
                    $rootScope.UIStates = null;
                    $rootScope.identity = null;
                    $window.location.reload();
                }
                else {
                    cfg.headers[cg.serverTokenName] = $cookies.get(cg.clientTokenName);
                    cfg.headers[cg.timezoneoffset] = -(new Date().getTimezoneOffset());
                    var timeZoneName =  /\((.*)\)/.exec(new Date().toString())[1];
                    cfg.headers[cg.timeZoneName] = timeZoneName;
                    return cfg;
                }
            }
        };
    }]);


    app.factory("loaderInterceptor", ["APP_CONFIG", '$rootScope', "$q", function (cg, $rootScope, $q) {
        // Active request count
        var requestCount = 0;

        function startRequest(config) {

            //If header contains skipFullPageLoader header, return
            if (config.headers != null && config.headers.skipFullPageLoader == true) return config;

            // If no request ongoing, then broadcast start event
            if (!requestCount) {
                $rootScope.$broadcast('httpLoaderStart');
            }

            requestCount++;
            return config;
        }

        function endRequest(arg) {

            //If header contains skipFullPageLoader header, return
            var headers = arg.headers();
            if (headers != null && headers.skipfullpageloader == "true") return arg;

            // No request ongoing, so make sure we don’t go to negative count
            if (!requestCount)
                return arg;

            requestCount--;
            // If it was last ongoing request, broadcast event
            if (!requestCount) {
                $rootScope.$broadcast('httpLoaderEnd');
            }

            return arg;
        }

        function responseEnd(arg) {
            var res = endRequest(arg);
            return res || $q.when(res);
        }

        function responseErr(arg) {
            var res = endRequest(arg);
            //throw new Error(arg);
            return res;
        }

        // Return interceptor configuration object
        return {
            'request': startRequest,
            'requestError': endRequest,
            'response': responseEnd,
            'responseError': responseErr
        };
    }]);


    app.factory('checkRouting', ['$rootScope', '$location', 'UserAuthenticationService', 'APP_CONFIG', '$cookies', checkRouting]);

    function checkRouting($rootScope, $location, authService, config, $cookies) {

        if ($rootScope.identity) {
            return true;
        }

        var token = $cookies.get(config.clientTokenName);
        if (token == null) {
            $location.path("/login");
            return false;
        }

        return authService.getUserIdentity(token).then(
            function (result) {
                authService.setTokenAndIdentity($rootScope, result.data, token);

                if ($rootScope.identity && $rootScope.UIStates && $rootScope.UIStates.data.length > 0) {
                    var promise = authService.getUserAuthorizedUIStates();
                    promise.then(function (result) {
                        $rootScope.UIStates = result;
                    });
                }

                if (result.data != null && result.data.userId > 0)
                    return true;
                else {
                    $location.path("/login");
                    return false;
                }

            },
            function () {
                $location.path("/login");
                return false;
            });
    }

    app.run(["$rootScope", /*"Keepalive", "Idle",*/
        function ($rootScope /*Keepalive, Idle*/) {

            var settings = {
                layout: {
                    pageAutoScrollOnLoad: 1000 // auto scroll to top on page load
                },
                layoutImgPath: '/content/images/layout/',
                layoutCssPath: '/content/styles/'
            };

            $rootScope.settings = settings;

            if (!String.prototype.endsWith) {
                String.prototype.endsWith = function (pattern) {
                    var d = this.length - pattern.length;
                    return d >= 0 && this.lastIndexOf(pattern) === d;
                };
            }

        }]);

}());
