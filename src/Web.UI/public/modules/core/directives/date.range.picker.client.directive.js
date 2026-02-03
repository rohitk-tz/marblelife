(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).directive("dateRangePicker", ["$rootScope",
        function ($rootScope) {


            return {
                restrict: "A",
                scope: {
                    startDate: '=',
                    endDate: '=',
                    change: '='
                },
                link: function ($scope, $element, $attrs) {

                    function init()
                    {
                        setTimeout(function () {

                            var options = {
                                autoUpdateInput: false,
                                locale: {
                                    firstDay: 1
                                }
                            };

                            if ($scope.startDate != null)
                                options.startDate = $scope.startDate;

                            if ($scope.endDate != null)
                                options.endDate = $scope.endDate;

                            $($element).daterangepicker(
                                options,
                                function (start, end) {
                                    var isChange = $scope.startDate != start || $scope.endDate != end;

                                    $scope.startDate = moment(start).format("MM/DD/YYYY");
                                    $scope.endDate = moment(end).format("MM/DD/YYYY");
                                    $scope.$apply();

                                   // if (isChange == true && $scope.change != null)
                                      //  $scope.change();
                                });

                            $($element).on('apply.daterangepicker', function (ev, picker) {
                                $(this).val(picker.startDate.format('MM/DD/YYYY') + ' - ' + picker.endDate.format('MM/DD/YYYY'));
                                $(this).removeClass('edited').addClass('edited');
                                var isChange = $scope.startDate != picker.startDate || $scope.endDate != picker.endDate;
                                 if (isChange == true && $scope.change != null)
                                  $scope.change();
                            });

                            $($element).on('cancel.daterangepicker', function (ev, picker) {
                                $(this).val('');
                                $(this).removeClass('edited');
                                $scope.$emit('clearDates');
                                init();
                            });

                        }, 500);
                    }
                    

                    $scope.$on("reset-dates", function () {
                        $($element).trigger('cancel.daterangepicker');
                    });

                    init();
                }
            }

        }]);
}());