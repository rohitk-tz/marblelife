(function () {
    'use strict';
    angular.module(OrganizationsConfiguration.moduleName).directive("royaltySlabs", ["APP_CONFIG",
        function (config) {

            return {
                restrict: 'E',
                templateUrl: 'modules/organizations/views/royalty-slabs.client.view.html',
                scope: {
                    list: '='
                },
                link: function (scope, element) {
                    scope.maxSlabsCount = config.maxSlabsCount;

                    if (scope.list != null && scope.list.length < 1)
                        scope.list.push({});

                    scope.addSlab = function ()
                    {
                        if (scope.list.length >= scope.maxSlabsCount) return;
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

                        var previousMaxVal = scope.list[index -1].maxValue;
                        var minValue = scope.list[index].minValue;
                        if (minValue != previousMaxVal + 1) {
                            form.minamt.$setValidity("invalid", false);
                        }
                        else {
                            form.minamt.$setValidity("invalid", true);
                        }
                    }

                    scope.validateMax = function(form, index, isLast)
                    {
                        if (scope.list == null || scope.list.length < 1) return;

                        if (index < 1 && isLast == true) return;

                        if (isLast == true) return;

                        if (scope.list[index].maxValue == null || scope.list[index].maxValue.length < 1)
                            form.maxamt.$setValidity("requiredMax", false);
                        else
                            form.maxamt.$setValidity("requiredMax", true);

                    }

                }
            };
        }
    ]);

}());