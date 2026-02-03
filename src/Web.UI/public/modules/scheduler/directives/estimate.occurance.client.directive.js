(function () {
    'use strict';
    angular.module(OrganizationsConfiguration.moduleName).directive("estimateOccurence", ["APP_CONFIG", "SchedulerService", "Clock", "$rootScope",
        function (config, schedulerService, clock, $rootScope) {

            return {
                restrict: 'E',
                templateUrl: 'modules/scheduler/views/estimate.occurence.client.view.html',
                scope: {
                    list: '=',
                    techList: '='
                },
                link: function (scope, element) {
                    scope.dateOptions = {
                        showWeeks: false
                    };
                    scope.maxOccurenceCount = config.maxOccurenceCount;

                    if (scope.list != null && scope.list.length < 1)
                        scope.list.push({});

                    scope.addOccurence = function () {
                        //if (scope.list.length >= scope.maxOccurenceCount) return;
                        scope.start = moment(moment(clock.now(), "MM/DD/YYYY").add(1, 'days'));
                        scope.startDate = moment(moment(scope.start).format("L")).add(8, 'hours');
                        scope.endDate = moment(moment(scope.start).format("L")).add(17, 'hours');
                        scope.list.push({ startDate: scope.startDate, endDate: scope.endDate, isNew: true });
                    };

                    scope.removeOccurence = function (index) {
                        if (scope.list.length == 1) {
                            scope.isNonRemovable = true;
                            return;
                        }
                        scope.list.splice(index, 1);

                        if (scope.list.length == 0)
                            scope.list.push({});
                    };

                    scope.validateStart = function (form, index) {
                        if (index < 0) return;

                        var currentDate = moment(clock.now()).subtract(2, 'month');
                        var startDate = scope.list[index].startDate;
                        var startDate = scope.list[index].startDate;
                        var momentStart = moment(scope.list[index].startDate).format('DD-MMM-YYYY');
                        var momentCurrentStart = currentDate.format('DD-MMM-YYYY');
                        var momentStartFinal = Date.parse(momentStart);
                        var momentCurrentStartFinal = Date.parse(momentCurrentStart);
                        if (startDate == null || momentStartFinal < momentCurrentStartFinal) {
                            form.startDate.$setValidity("invalid", false);

                        }
                        else {
                            if (!form.startDate.$untouched) {
                                $rootScope.$broadcast('isUpdated', "true");
                            }
                            form.startDate.$setValidity("invalid", true);
                        }
                    }

                    $rootScope.$on('timechanged', function (event, data) {
                        if (data) {
                            $rootScope.$broadcast('isUpdated', "true");
                        }
                    });


                    scope.validateEnd = function (form, index) {
                        if (index < 0) return;

                        var currentDate = moment(clock.now()).subtract(2, 'month');
                        var endDate = scope.list[index].endDate;
                        var startDate = scope.list[index].startDate;
                        var momentStart = moment(scope.list[index].startDate).format('DD-MMM-YYYY');
                        var momentEnd = moment(scope.list[index].endDate).format('DD-MMM-YYYY');
                        var momentCurrentStart = currentDate.format('DD-MMM-YYYY');
                        var momentStartFinal = Date.parse(momentStart);
                        var momentEndFinal = Date.parse(momentEnd);
                        var momentCurrentStartFinal = Date.parse(momentCurrentStart);
                        if (endDate == null || momentEndFinal < momentCurrentStartFinal || momentEndFinal < momentStartFinal) {
                            form.endDate.$setValidity("invalid", false);
                        }
                        else {

                            if (!form.endDate.$untouched) {
                                $rootScope.$broadcast('isUpdated', "true");
                            }
                            form.endDate.$setValidity("invalid", true);
                        }
                    }

                    scope.validateSchedule = function (form, assignee) {
                        scope.error = '';
                        if (assignee.assigneeId <= 0)
                            return;
                        var model = { jobId: assignee.jobId, assigneeId: assignee.assigneeId, startDate: assignee.startDate, endDate: assignee.endDate };
                        schedulerService.checkAvailability(model).then(function (result) {
                            if (!result.data) {
                                scope.isNotAvailable = true;
                                scope.error = "Assignee is not available , try scheduling after 15 mins.";
                                assignee.assigneeId = null;
                            }
                            if (form.$dirty) {
                                $rootScope.$broadcast('isUpdated', "true");
                            }
                        });
                    }
                }
            };
        }
    ]);

}());