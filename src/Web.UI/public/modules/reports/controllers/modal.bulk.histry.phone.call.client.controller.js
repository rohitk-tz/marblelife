(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("ModalBulkHistryPhoneCallController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SchedulerService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "modalParam", "MarketingLeadService",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, schedulerService, franchiseeService,
                notification, clock, toaster, modalParam, marketingLeadService) {

                var vm = this;
                vm.change = change;
                
                vm.sorting = sorting;
                vm.isReset = false;
                vm.histryByFranchisee = true;
                vm.option1 = '1';
                vm.franchiseeName = "MI Detriot";
                vm.refresh = refresh;
                vm.resetSearch = resetSearch;
                vm.option = [{ display: "Franchisee", value: "1" }, { display: "Date", value: "2" }];
                var mlist = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

                

                vm.SortColumns = {
                    MonthYear: 'MonthYear',
                    FranchiseeName: 'FranchiseeName',
                    Timestamp: 'Timestamp'
                };

                vm.query = {
                    Order: 0,
                    propName: '',
                    date: '',
                    franchiseeId: "62",
                    sort: { order: 0, propName: '' }
                };

                $scope.dateOptions = {
                    showWeeks: false
                };
                vm.close = close;
                vm.month_name = month_name;
                vm.isValid = true;

                function month_name(dt) {
                    mlist = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
                    return mlist[dt.getMonth()];
                };
                //vm.addBreak = addBreak;
                vm.imgs = modalParam.Imgs;
                vm.isEditable = modalParam.IsEditable;
                vm.idList = modalParam.IdList;
                vm.info = {};
                vm.emailList = [];
                vm.selectedFranchiseeId = modalParam.SelectedFranchiseeId;

                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
                
                function getFranchiseePhoneCallsList() {
                    if ($rootScope.franchiseeForBulkId != undefined) {
                        //vm.query.franchiseeId = $rootScope.franchiseeForBulkId;
                        if (vm.selectedFranchiseeId.length > 0) {
                            vm.query.franchiseeId = vm.selectedFranchiseeId[0].toString();
                        }
                        else {
                            vm.query.franchiseeId = $rootScope.franchiseeForBulkId;
                        }
                        
                    }
                    return marketingLeadService.getFranchiseePhoneCallsBulkList(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.franchiseePhoneCall = result.data.collection;

                        }
                    });
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                function refresh() {
                    getFranchiseePhoneCallsList();
                }

                function resetSearch() {
                    vm.isReset = true;
                    vm.histryByFranchisee = true;
                    vm.histryByDate = false;
                    vm.option1 = "1";
                    vm.query.date = null;
                    vm.query.franchiseeId = "62";
                    getFranchiseePhoneCallsList();
                }
                $scope.$watch('vm.query.franchiseeId', function (nv, ov) {
                    //if (vm.isReset) {
                    //    return;
                    //}
                    if (nv == ov) return;
                    else if (nv == 0 && ov != 0) return;
                    else if (nv == 0 && ov != 0) { nv = ov; vm.query.franchiseeId = ov; return };


                    //vm.query.date = null;
                    getFranchiseePhoneCallsList();
                });
                $scope.$watch('vm.query.date', function (nv, ov) {
                    if (nv == ov) return;
                    //vm.query.franchiseeId = 0;
                    getFranchiseePhoneCallsList();
                });


                function sorting(propName) {
                    vm.query.propName = propName;
                    vm.query.sort.propName = propName;
                    vm.query.order = (vm.query.order == 0) ? 1 : 0;
                    vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                    getFranchiseePhoneCallsList();
                }

                $scope.$watch('vm.query.franchiseeId', function (nv, ov) {

                    if (nv != undefined) {
                        $rootScope.franchiseeForBulkId = vm.query.franchiseeId;
                    }
                });

                function change() {
                    $rootScope.franchiseeForBulkId = vm.query.franchiseeId;
                }

                $q.all(getFranchiseePhoneCallsList(), getFranchiseeCollection());
            }]);
}());