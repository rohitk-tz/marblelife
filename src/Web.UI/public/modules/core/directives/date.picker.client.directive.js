(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).directive('datePickerTs', ["$timeout", function ($timeout) {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/modules/core/views/date-picker.client.view.html',
            scope: {
                model: '=',
                change: '=',
                changeRef: '&',
                maxDate: '=',
                minDate: '=',
                disabled: '=',
                reset: '=',
                controlId: '@',
                label: '@',
                triggerInputChange: '@',
                showButtonBar: "@"
            },
            link: function (scope, elem, attrs, ctrl) {

                var modelChangeListener;
                scope.open = function () {
                    scope.opened = true;
                };
                scope.format = 'MM/dd/yyyy';

                $timeout(function () {
                    setWatch();
                }, 2000);

                function setWatch() {
                    modelChangeListener = scope.$watch("model", onChange);
                }

                function clearWatch() {
                    if (modelChangeListener != null) {
                        modelChangeListener();
                    }
                }

                function onChange(nv, ov) {
                    if (nv == ov) return;

                    if (nv != null && ov != null) {
                        if ((new Date(nv)).getTime() == (new Date(ov)).getTime())
                            return;
                    }

                    if (scope.change != null)
                        scope.change(scope.model);

                    if (scope.changeRef != null)
                        scope.changeRef();

                    if (scope.triggerInputChange != null && (scope.triggerInputChange == true || scope.triggerInputChange == "true")) {
                        $(elem).find("input[type=text]:first").change();
                    }
                }

                scope.$watch("reset", function (nv, ov) {
                    if (nv == ov) return;
                    if (nv == true) {
                        clearWatch();
                        scope.model = null;
                        scope.reset = false;

                        $timeout(function () {
                            setWatch();
                        }, 500);
                    }
                });

            },
            controller: ["$scope", function (scope) {
                scope.alternateInputFormats = [];
                scope.alternateInputFormats.push('MM/dd/yyyy');
                scope.alternateInputFormats.push('MM-dd-yyyy');
                scope.format = 'MM/dd/yyyy';
                scope.opened = false;

                if (scope.showButtonBar == null) {
                    scope.showButtonBar = true;
                }

                scope.dateOptions = {
                    //dateDisabled: disabled,
                    datepickerMode: 'day',
                    formatYear: 'yyyy',
                    showWeeks: false,
                    maxDate: scope.maxDate == null ? new Date("01/01/2050") : new Date(scope.maxDate),
                    minDate: scope.minDate == null ? new Date("01/01/1900") : new Date(scope.minDate),
                    startingDay: 1
                };

                scope.$watch("minDate", function () {
                    scope.dateOptions.minDate = (scope.minDate == null ? new Date("01/01/1900") : new Date(scope.minDate));
                });

                scope.$watch("maxDate", function () {
                    scope.dateOptions.maxDate = (scope.maxDate == null ? new Date("01/01/2050") : new Date(scope.maxDate));
                });
            }]
        }
    }]);
})();
