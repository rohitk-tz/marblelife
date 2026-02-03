(function () {
    'use strict';
    angular.module(OrganizationsConfiguration.moduleName).directive("minRoyaltySlabs", ["APP_CONFIG",
        function (config) {

            return {
                restrict: 'E',
                templateUrl: 'modules/organizations/views/min-royalty-slabs.client.view.html',
                scope: {
                    list: '='
                },
                link: function (scope, element) {
                    scope.maxSlabsCount = config.maxSlabsCount;

                    if (scope.list != null && scope.list.length < 1)
                        scope.list.push({});

                    scope.addSlab = function ()
                    {
                        scope.list.push({});
                    };

                    scope.removeSlab = function (index)
                    {
                        scope.list.splice(index, 1);

                        if (scope.list.length == 0)
                            scope.list.push({});
                    };

                    scope.validate = function(form, index)
                    {
                        if (index < 1) return;

                        var previousMaxVal = scope.list[index - 1].endValue;
                        var minValue = scope.list[index].startValue;
                        if (minValue != previousMaxVal + 1) {
                            form.startValue.$setValidity("invalid", false);
                        }
                        else {
                            form.startValue.$setValidity("invalid", true);
                        }
                    }

                    scope.validateMax = function(form, index, isLast)
                    {
                        if (scope.list == null || scope.list.length < 1) return;

                        if (index < 1 && isLast == true) return;

                        if (isLast == true) return;

                        if (scope.list[index].endValue == null || scope.list[index].endValue.length < 1)
                            form.endValue.$setValidity("requiredMax", false);
                        else
                            form.endValue.$setValidity("requiredMax", true);

                    }

                }
            };
        }
    ]);

}());