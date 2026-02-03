(function () {

    'use strict';
    var HttpMethod = { Get: 1, Post: 2, Put: 3, Delete: 4 };

    angular.module(CoreConfiguration.moduleName).factory("HttpWrapper", ["$http", "$q", "Toaster", "APP_CONFIG", "$cookies", "Upload", "Notification", "Util", "usSpinnerService", "$rootScope",
        function ($http, $q, toaster, App_Config, $cookies, Upload, Notification, Util, usSpinnerService, $rootScope) {
            $rootScope.$on('$stateChangeSuccess',
function (event, toState, toParams, fromState, fromParams) {
    $http.pendingRequests.length = 0;
})
            var defaultOptions = function (httpMethod) {
                return {
                    url: '',
                    data: '',
                    showOnSuccess: true,
                    showOnFailure: true,
                    method: httpMethod,
                    success: function (deffered, payload, def) {
                        //if ($http.pendingRequests.length == 0) {
                        //    usSpinnerService.stop('secure-spinner');
                        //}
                        if (def.showOnSuccess) {
                            if (payload.data != null && payload.message != null && payload.message.length > 0) {
                                if (!def.disableNotification && def.method != HttpMethod.Get) {
                                    toaster.show(payload.message.message);
                                }
                            }
                        }
                        if (payload.data == null && payload.message != null && payload.message.messageType == 3) {
                            if (!def.disableNotification && def.method != HttpMethod.Get) {
                                toaster.error(payload.message.message);
                            }
                        }
                        deffered.resolve(payload);
                    },
                    error: function (deffered, payload, def) {
                        //if ($http.pendingRequests.length == 0) {
                        //    usSpinnerService.stop('secure-spinner');
                        //}
                        if (def.showOnFailure) {
                            if (payload != null && payload.errorCode != 3 && payload.message != null) {
                                toaster.error(payload.message.message);
                            }
                            else if (payload != null && payload.modelValidation != null && payload.modelValidation.errors.length > 0) {
                                Notification.showValidations(payload.modelValidation);
                            }
                        }

                        deffered.reject();
                    }
                };
            };

            function parseReturnResult(result, deferred, def) {
                if (result.status != 200) {
                    def.error(deferred, result.data, def);
                }
                else {
                    def.success(deferred, result.data, def);
                }
            }

            $rootScope.$watch(function () {
                return $http.pendingRequests.length > 0;
            }, function (hasPending) {
                if (hasPending) {
                    // show wait dialog
                }
                else {
                    // hide wait dialog

                    if ($http.pendingRequests.length === 0) {
                        usSpinnerService.stop('secure-spinner');
                    }
                }
            });

            return {
                get: function (opts) {
                    var deferred = $q.defer();
                    var def = defaultOptions(HttpMethod.Get);
                    $.extend(def, opts);
                    usSpinnerService.spin('secure-spinner');
                    var params = { method: 'get', url: App_Config.apiUrl + def.url };

                    if (opts.skipFullPageLoader == true) {
                        params.headers = { 'skipFullPageLoader': true };
                    }

                    $http(params).then(function (result, status, header) {
                        Util.hookToAdjustLabels();
                        parseReturnResult(result, deferred, def);
                    }, function (result) {
                        def.error(deferred, result.data, def);
                    });

                    return deferred.promise;
                },
                post: function (opts) {
                    var deferred = $q.defer();
                    var def = defaultOptions(HttpMethod.Post);
                    $.extend(def, opts);
                    usSpinnerService.spin('secure-spinner');
                    var params = {};
                    if (opts.skipFullPageLoader == true) {
                        params.headers = { 'skipFullPageLoader': true };
                    }

                    $http.post(App_Config.apiUrl + def.url, def.data, params).then(function (result, status, header) {
                        parseReturnResult(result, deferred, def);
                    }, function (result) {
                        def.error(deferred, result.data, def);
                    });

                    return deferred.promise;
                },
                put: function (opts) {
                    var deferred = $q.defer();
                    var def = defaultOptions(HttpMethod.Post);
                    $.extend(def, opts);
                    usSpinnerService.spin('secure-spinner');
                    var params = {};
                    if (opts.skipFullPageLoader == true) {
                        params.headers = { 'skipFullPageLoader': true };
                    }

                    $http.put(App_Config.apiUrl + def.url, def.data, params).then(function (result, status, header) {
                        parseReturnResult(result, deferred, def);
                    }, function (result) {
                        def.error(deferred, result.data, def);
                    });

                    return deferred.promise;
                },
                delete: function (opts) {
                    var deferred = $q.defer();
                    var def = defaultOptions(HttpMethod.Delete);
                    $.extend(def, opts);
                    usSpinnerService.spin('secure-spinner');
                    var params = {};
                    if (opts.skipFullPageLoader == true) {
                        params.headers = { 'skipFullPageLoader': true };
                    }

                    $http.delete(App_Config.apiUrl + def.url, params).then(function (result, status, header) {
                        parseReturnResult(result, deferred, def);
                    }, function (result) {
                        def.error(deferred, result.data, def);
                    });

                    return deferred.promise;
                },

                upload: function (opts, file) {
                    var deferred = $q.defer();
                    var def = defaultOptions(HttpMethod.POST);
                    $.extend(def, opts);
                    usSpinnerService.spin('secure-spinner');
                    var headers = {};
                    headers[App_Config.serverTokenName] = $cookies.get(App_Config.clientTokenName);

                    if (opts.skipFullPageLoader == true) {
                        headers['skipFullPageLoader'] = true;
                    }

                    Upload.upload({
                        data: { file: file },
                        url: App_Config.apiUrl + def.url,
                        headers: headers,
                    }).then(function (result) {
                        parseReturnResult(result, deferred, def);
                    }, function (result) {
                        def.error(deferred, result.data, def);
                    });

                    return deferred.promise;
                },
                file: function (opts) {
                    var deferred = $q.defer();
                    var def = defaultOptions(HttpMethod.Post);
                    $.extend(def, opts);
                    usSpinnerService.spin('secure-spinner');
                    var form = new FormData();
                    form.append('file', def.data);
                    $http.post(App_Config.apiUrl + def.url, form, { transformRequest: angular.identity, headers: { 'Content-Type': undefined } }).then(function (result) {
                        def.success(deferred, result, def);
                    }, function (data) {
                        def.error(deferred, data, def);
                    });

                    return deferred.promise;
                },
                getFile: function (opts) {
                    var deferred = $q.defer();
                    var def = defaultOptions(HttpMethod.Get);
                    $.extend(def, opts);
                    usSpinnerService.spin('secure-spinner');
                    delete $http.defaults.headers.common['X-Requested-With'];
                    $http({ method: 'get', url: App_Config.apiUrl + def.url, responseType: 'arraybuffer' }).then(function (result, status, header) {
                        deferred.resolve(result);
                    }, function (data) {
                        deferred.reject();
                    });

                    return deferred.promise;
                },
                getFileByPost: function (opts) {
                    var deferred = $q.defer();
                    var def = defaultOptions(HttpMethod.Post);
                    $.extend(def, opts);
                    usSpinnerService.spin('secure-spinner');
                    delete $http.defaults.headers.common['X-Requested-With'];
                    $http({ method: 'post', url: App_Config.apiUrl + def.url, data: def.data, responseType: 'arraybuffer' }).then(function (result, status, header) {
                        deferred.resolve(result);
                    }, function (data) {
                        deferred.reject();
                    });

                    return deferred.promise;
                }

                //getFiles: function (opts) {
                //	var deferred = $q.defer();
                //	var def = defaultOptions(HttpMethod.GET);
                //	$.extend(def, opts);
                //	delete $http.defaults.headers.common['X-Requested-With'];

                //	var params = { method: 'get', url: '/api' + def.url, responseType: 'arraybuffer' };

                //	if (opts.skipFullPageLoader == true) {
                //		params.headers = { 'skipFullPageLoader': true };
                //	}

                //	$http(params).then(function (result, status, header) {
                //		deferred.resolve(result);
                //	}, function (data) {
                //		deferred.reject();
                //	});

                //	return deferred.promise;
                //},

                //sanitizeParam: function (param) {
                //	if (param == null) return "";
                //	return param.toString().trim();
                //}
            };
        }]);
}());