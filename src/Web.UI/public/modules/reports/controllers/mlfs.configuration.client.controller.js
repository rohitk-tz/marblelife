angular.module(ReportsConfiguration.moduleName).controller("MLFSConfigurationController",
    ["$state", "$stateParams", "$q", "$scope", "MarketingLeadService", "WebLeadService", "APP_CONFIG", "$rootScope", "FileService", "FranchiseeService", "$uibModal", "Clock",
        "DashboardService", "FranchiseeGrowthReportService", "MarketingLeadGraphService", "$filter", "MlfsReportService", "LateFeeReportService", "Toaster",
        function ($state, $stateParams, $q, $scope, marketingLeadService, webLeadService, config, $rootScope, fileService, franchiseeService, $uibModal, clock,
            dashboardService, franchiseeGrowthReportService, marketingLeadGraphService, $filter, mlfsReportService, lateFeeReportService, toaster) {
            var vm = this;
            vm.addMore = addMore;
            vm.deleteFromList = deleteFromList;
            vm.mlfStatus = DataHelper.MLFSStatus;
            vm.saveMLFSConfiguration = saveMLFSConfiguration;
            vm.getMLFSConfiguration = getMLFSConfiguration;
            vm.isValid = false;
            vm.minValue = [];
            vm.maxValue = [];
            vm.errorRowIndex = null;
            vm.statusError = 0;
            vm.query = {
                franchiseeId: 0,
                text: 'null',
            };
            vm.saveQuery = {
                statusList: [],
                userId: 0,
            };
            vm.statusList = [{ name: "", minValue: "", maxValue: "", colorCode: "", id: 0, isActive: 1 }];

            function getMLFSConfiguration() {
                return mlfsReportService.getConfiguration(vm.query).then(function (result) {
                    vm.saveQuery.statusList = result.data.mlfsReportConfigurationViewModel;
                    if (vm.saveQuery.statusList.length == 0) {
                        vm.saveQuery.statusList = [{ name: "", minValue: "", maxValue: "", colorCode: "", id: 0, isActive: 1 }];
                    }
                });
            }
            function addMore() {
                vm.saveQuery.statusList.push({ name: "", minValue: "", maxValue: "", colorCode: "", id: 0 });
            }

            function deleteFromList(index) {
                vm.saveQuery.statusList.splice(index, 1);
            }
            function saveMLFSConfiguration() {
                vm.errorRowIndex = null;
                CheckForValue(vm.saveQuery.statusList);

                if (!vm.isValid) {
                    var errorRow = vm.saveQuery.statusList[vm.errorRowIndex];
                    if (vm.statusError == vm.mlfStatus.BlankValue) {
                        toaster.error("Value(s) Cannot be Blank!!");
                    }
                    else if (vm.statusError == vm.mlfStatus.MaxMinValue) {

                        toaster.error("Max Value Should be Greater Than Min Value !!");
                    }
                    else if (vm.statusError == vm.mlfStatus.GreaterValue) {
                        toaster.error("Max or Min Value  !!");
                    }
                    //else if (vm.statusError == vm.mlfStatus.WholeNumber) {
                    //    toaster.error("Max And Min Value Should be Whole Number!!");
                    //}
                    else if (vm.statusError == vm.mlfStatus.ColorBlank) {
                        toaster.error("Color Cannot be Blank!!");
                    }
                    else if (vm.statusError == vm.mlfStatus.LessThan0) {
                        toaster.error("Entered value should be greater than 0!!");
                    }
                    return;
                }
                else {
                vm.errorRowIndex = null;
                }
                return mlfsReportService.saveMLFSConfiguration(vm.saveQuery).then(function (result) {
                    if (!result.data) {
                        toaster.error(result.message.message);
                    }
                    else {
                        toaster.show(result.message.message);
                        if (vm.saveQuery.statusList.length == 0) {
                            vm.saveQuery.statusList = [{ name: "", minValue: "", maxValue: "", colorCode: "", id: 0, isActive: 1 }];
                        }
                    }
                });
            }

            function CheckForValue(statusList) {
                var index = 0;
                var errorIndex = 0;
                vm.isInValidFromLoop = false;
                vm.minValue = [];
                vm.maxValue = [];
                angular.forEach(statusList, function (item) {
                    
                    if (item.name == null || item.name == undefined || item.minValue == null || item.maxValue == null || item.colorCode == null || item.colorCode == "") {
                        vm.isValid = false;
                        vm.statusError = vm.mlfStatus.BlankValue;
                        vm.isInValidFromLoop = true;
                        vm.errorRowIndex = errorIndex;
                        if (item.colorCode == '' || item.colorCode == null) {
                            vm.statusError = vm.mlfStatus.ColorBlank;
                            vm.errorRowIndex = errorIndex;
                        }
                        return false;
                    }
                   

                    //else if (item.minValue % 1 != 0 || item.maxValue % 1 != 0) {
                    //    vm.isValid = false;
                    //    vm.statusError = vm.mlfStatus.WholeNumber;
                    //    vm.isInValidFromLoop = true;
                    //    vm.errorRowIndex = errorIndex;
                    //    return false;
                    //}

                    else if (item.minValue <= 0 || item.maxValue <=  0) {
                        vm.isValid = false;
                        vm.statusError = vm.mlfStatus.LessThan0;
                        vm.isInValidFromLoop = true;
                        vm.errorRowIndex = errorIndex;
                        return false;
                    }

                    else if (item.minValue != null && item.maxValue != null) {
                        if (item.minValue >= item.maxValue) {
                            vm.isValid = false;
                            vm.statusError = vm.mlfStatus.GreaterValue;
                            vm.isInValidFromLoop = true;
                            vm.errorRowIndex = errorIndex;
                            return false;
                        }
                    }
                    else if (item.minValue != null && item.maxValue != null) {
                        if (item.minValue > item.maxValue) {
                            vm.isValid = false;
                            vm.statusError = vm.mlfStatus.GreaterValue;
                            vm.isInValidFromLoop = true;
                            vm.errorRowIndex = errorIndex;
                            return false;
                        }
                    }

                    index = 0;
                    angular.forEach(vm.minValue, function (minValue) {
                        if (minValue <= item.minValue && vm.maxValue[index] >= item.minValue) {
                            vm.isValid = false;
                            vm.isInValidFromLoop = true;
                            vm.statusError = vm.mlfStatus.MaxMinValue;
                        }
                        else if (minValue <= item.maxValue && vm.maxValue[index] >= item.maxValue) {
                            vm.isValid = false;
                            vm.isInValidFromLoop = true;
                            vm.statusError = vm.mlfStatus.MaxMinValue;
                        }
                        vm.errorRowIndex = index;
                        ++index;

                    });
                    vm.minValue.push(item.minValue); vm.maxValue.push(item.maxValue);
                    if (!vm.isInValidFromLoop) {
                        vm.isValid = true;
                    }
                    errorIndex++;
                    return true;
                });

            }

            $scope.$emit("update-title", "");
            $q.all([getMLFSConfiguration()]);
        }]);
