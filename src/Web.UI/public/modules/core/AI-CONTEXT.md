# Core Module

## Overview
The Core module provides shared services, directives, filters, and utilities used across all other modules in the application. It contains the foundational infrastructure for HTTP communication, notifications, data management, and common UI components.

## Module Structure

```
/core
├── module.js                    # Module definition and routing
├── /controllers
│   ├── default.layout.client.controller.js
│   ├── header.client.controller.js
│   ├── footer.client.controller.js
│   └── home.client.controller.js
├── /services
│   ├── http.wrapper.client.service.js     # Centralized HTTP service
│   ├── toaster.client.service.js          # Toast notifications
│   ├── notification.client.service.js     # Advanced notifications
│   ├── local.storage.client.service.js    # Browser storage
│   ├── util.client.service.js             # Utility functions
│   ├── data.helper.client.service.js      # Data transformation
│   ├── geo.client.service.js              # Geolocation services
│   ├── address.client.service.js          # Address management
│   ├── phone.client.service.js            # Phone formatting
│   ├── file.manage.client.service.js      # File operations
│   ├── clock.client.service.js            # Time management
│   ├── color.code.service.js              # Color utilities
│   └── dashboard.client.service.js        # Dashboard data
├── /directives
│   └── [Various reusable UI components]
├── /filters
│   └── [Data transformation filters]
└── /views
    ├── default.client.view.html
    ├── header.client.view.html
    ├── footer.client.view.html
    └── home.client.view.html
```

## Routing Configuration

```javascript
angular.module('core').config(["$stateProvider", "$urlRouterProvider", routeConfig]);

function routeConfig($stateProvider, $urlRouterProvider) {
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
        });
    
    // Default routes
    $urlRouterProvider.when('', '/dashboard');
    $urlRouterProvider.when('/', '/dashboard');
    $urlRouterProvider.otherwise('/dashboard');
}
```

## Core Services

### HttpWrapper Service
Centralized HTTP communication with standardized error handling, notifications, and request management.

```javascript
angular.module('core').factory("HttpWrapper", 
    ["$http", "$q", "Toaster", "APP_CONFIG", "$cookies", "Upload", "Notification", "Util", "usSpinnerService", "$rootScope",
    function ($http, $q, toaster, App_Config, $cookies, Upload, Notification, Util, usSpinnerService, $rootScope) {
        
    var HttpMethod = { Get: 1, Post: 2, Put: 3, Delete: 4 };

    // Default options for HTTP requests
    var defaultOptions = function (httpMethod) {
        return {
            url: '',
            data: '',
            showOnSuccess: true,
            showOnFailure: true,
            method: httpMethod,
            success: function (deffered, payload, def) {
                if (def.showOnSuccess) {
                    if (payload.data != null && payload.message != null) {
                        if (!def.disableNotification && def.method != HttpMethod.Get) {
                            toaster.show(payload.message.message);
                        }
                    }
                }
                deffered.resolve(payload);
            },
            error: function (deffered, payload, def) {
                if (def.showOnFailure) {
                    if (payload != null && payload.message != null) {
                        toaster.error(payload.message.message);
                    }
                    else if (payload != null && payload.modelValidation != null) {
                        Notification.showValidations(payload.modelValidation);
                    }
                }
                deffered.reject();
            }
        };
    };

    // Automatic spinner management
    $rootScope.$watch(function () {
        return $http.pendingRequests.length > 0;
    }, function (hasPending) {
        if (!hasPending && $http.pendingRequests.length === 0) {
            usSpinnerService.stop('secure-spinner');
        }
    });

    // Cancel pending requests on state change
    $rootScope.$on('$stateChangeSuccess', function () {
        $http.pendingRequests.length = 0;
    });

    return {
        get: function (opts) {
            var deferred = $q.defer();
            var def = defaultOptions(HttpMethod.Get);
            $.extend(def, opts);
            
            usSpinnerService.spin('secure-spinner');
            
            var params = { 
                method: 'get', 
                url: App_Config.apiUrl + def.url 
            };

            if (opts.skipFullPageLoader == true) {
                params.headers = { 'skipFullPageLoader': true };
            }

            $http(params).then(
                function (result) {
                    Util.hookToAdjustLabels();
                    parseReturnResult(result, deferred, def);
                },
                function (result) {
                    def.error(deferred, result.data, def);
                }
            );

            return deferred.promise;
        },
        
        post: function (opts) {
            var deferred = $q.defer();
            var def = defaultOptions(HttpMethod.Post);
            $.extend(def, opts);
            
            usSpinnerService.spin('secure-spinner');
            
            $http({
                method: 'post',
                url: App_Config.apiUrl + def.url,
                data: def.data
            }).then(
                function (result) {
                    parseReturnResult(result, deferred, def);
                },
                function (result) {
                    def.error(deferred, result.data, def);
                }
            );

            return deferred.promise;
        },
        
        put: function (opts) {
            // Similar to post
        },
        
        delete: function (opts) {
            // Similar to get but with DELETE method
        },
        
        postFile: function (opts) {
            // File upload using ng-file-upload
            return Upload.upload({
                url: App_Config.apiUrl + opts.url,
                data: opts.data
            });
        },
        
        getFileByPost: function (opts) {
            // File download
            return $http({
                method: 'post',
                url: App_Config.apiUrl + opts.url,
                data: opts.data,
                responseType: 'arraybuffer'
            });
        }
    };
}]);
```

### Toaster Service
Displays toast notifications for user feedback.

```javascript
angular.module('core').service("Toaster", [function() {
    return {
        show: function(message, type) {
            toastr.success(message, '', {
                positionClass: 'toast-top-right',
                timeOut: 3000,
                closeButton: true
            });
        },
        error: function(message) {
            toastr.error(message, '', {
                positionClass: 'toast-top-right',
                timeOut: 5000,
                closeButton: true
            });
        },
        warning: function(message) {
            toastr.warning(message);
        },
        info: function(message) {
            toastr.info(message);
        }
    };
}]);
```

### Notification Service
Advanced notification system with validation error display.

```javascript
angular.module('core').service("Notification", ['Toaster', function(Toaster) {
    return {
        showValidations: function(modelValidation) {
            if (modelValidation && modelValidation.errors) {
                var messages = modelValidation.errors.map(function(err) {
                    return err.message;
                }).join('<br>');
                Toaster.error(messages);
            }
        },
        success: function(message) {
            Toaster.show(message, 'success');
        },
        error: function(message) {
            Toaster.error(message);
        }
    };
}]);
```

### LocalStorage Service
Browser local storage management with JSON serialization.

```javascript
angular.module('core').service("LocalStorage", ['$window', function($window) {
    return {
        set: function(key, value) {
            $window.localStorage[key] = JSON.stringify(value);
        },
        get: function(key) {
            var item = $window.localStorage[key];
            return item ? JSON.parse(item) : null;
        },
        remove: function(key) {
            $window.localStorage.removeItem(key);
        },
        clear: function() {
            $window.localStorage.clear();
        }
    };
}]);
```

### Util Service
Common utility functions for the application.

```javascript
angular.module('core').service("Util", function() {
    return {
        hookToAdjustLabels: function() {
            // Adjust UI labels and layout after DOM updates
            setTimeout(function() {
                $('.form-control').trigger('change');
            }, 100);
        },
        
        generateGuid: function() {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
                var r = Math.random() * 16 | 0,
                    v = c == 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        },
        
        formatCurrency: function(value) {
            return '$' + parseFloat(value).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
        },
        
        parseDate: function(dateString) {
            return new Date(dateString);
        },
        
        formatDate: function(date, format) {
            return moment(date).format(format || 'MM/DD/YYYY');
        }
    };
});
```

### Geo Service
Geolocation and address geocoding services.

```javascript
angular.module('core').service("GeoService", ["HttpWrapper", function(httpWrapper) {
    return {
        geocodeAddress: function(address) {
            return httpWrapper.post({
                url: "/geo/geocode",
                data: { address: address }
            });
        },
        
        reverseGeocode: function(lat, lng) {
            return httpWrapper.get({
                url: "/geo/reverse?lat=" + lat + "&lng=" + lng
            });
        },
        
        getCurrentLocation: function() {
            return new Promise(function(resolve, reject) {
                if (navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition(resolve, reject);
                } else {
                    reject(new Error('Geolocation not supported'));
                }
            });
        }
    };
}]);
```

### Address Service
Address validation and formatting.

```javascript
angular.module('core').service("AddressService", ["HttpWrapper", function(httpWrapper) {
    return {
        validateAddress: function(address) {
            return httpWrapper.post({
                url: "/address/validate",
                data: address
            });
        },
        
        formatAddress: function(address) {
            var parts = [
                address.street1,
                address.street2,
                address.city,
                address.state,
                address.zipCode
            ].filter(Boolean);
            return parts.join(', ');
        }
    };
}]);
```

### Phone Service
Phone number formatting and validation.

```javascript
angular.module('core').service("PhoneService", function() {
    return {
        format: function(phoneNumber, format) {
            // Format: (XXX) XXX-XXXX
            var cleaned = phoneNumber.replace(/\D/g, '');
            if (cleaned.length === 10) {
                return '(' + cleaned.substr(0,3) + ') ' + 
                       cleaned.substr(3,3) + '-' + 
                       cleaned.substr(6,4);
            }
            return phoneNumber;
        },
        
        validate: function(phoneNumber) {
            var cleaned = phoneNumber.replace(/\D/g, '');
            return cleaned.length === 10;
        }
    };
});
```

### FileManage Service
File upload and download management.

```javascript
angular.module('core').service("FileManageService", ["HttpWrapper", function(httpWrapper) {
    return {
        uploadFile: function(file, category) {
            var formData = new FormData();
            formData.append('file', file);
            formData.append('category', category);
            
            return httpWrapper.postFile({
                url: "/files/upload",
                data: formData
            });
        },
        
        downloadFile: function(fileId) {
            return httpWrapper.getFileByPost({
                url: "/files/download/" + fileId
            });
        },
        
        deleteFile: function(fileId) {
            return httpWrapper.delete({
                url: "/files/delete/" + fileId
            });
        }
    };
}]);
```

## Layout Controllers

### DefaultLayoutController
Manages the overall application layout and global state.

### HeaderController
Controls header navigation, user menu, and notifications.

```javascript
angular.module('core').controller('HeaderController',
    ['$scope', '$rootScope', 'UserAuthenticationService', '$state',
    function($scope, $rootScope, authService, $state) {
        var vm = this;
        
        vm.identity = $rootScope.identity;
        
        vm.logout = function() {
            authService.logout().then(function() {
                $state.go('authentication.login');
            });
        };
        
        // Listen for identity changes
        $rootScope.$on('identity', function() {
            vm.identity = $rootScope.identity;
        });
}]);
```

### FooterController
Manages footer content and copyright information.

### HomeController
Dashboard controller with summary widgets and navigation.

## Common Directives

Reusable UI components:
- Date pickers
- Time pickers
- Address input with autocomplete
- Phone number input with formatting
- File upload with preview
- Data grids
- Modal dialogs
- Form validation

## Common Filters

Data transformation filters:
- Date formatting
- Currency formatting
- Phone formatting
- Text truncation
- HTML sanitization

## Route Protection

### checkRouting Resolver
Validates authentication before allowing route access:

```javascript
angular.module('core').factory('checkRouting',
    ['$q', '$state', 'UserAuthenticationService',
    function($q, $state, authService) {
        return function() {
            var deferred = $q.defer();
            
            authService.getUserSessionId(function() {
                if ($rootScope.identity) {
                    deferred.resolve();
                } else {
                    $state.go('authentication.login');
                    deferred.reject();
                }
            });
            
            return deferred.promise;
        };
}]);
```

## Best Practices

1. **Use HttpWrapper** for all API calls - Ensures consistent error handling
2. **Show notifications** for user actions - Provide feedback
3. **Handle errors gracefully** - User-friendly error messages
4. **Validate input** - Both client and server side
5. **Use services** for business logic - Keep controllers thin
6. **Reuse directives** - Don't duplicate UI components
7. **Clean up resources** - Remove event listeners on destroy

## Related Documentation
- See parent `/Web.UI/AI-CONTEXT.md` for application architecture
- See `/modules/authentication/AI-CONTEXT.md` for authentication
- See other module documentation for feature-specific details
