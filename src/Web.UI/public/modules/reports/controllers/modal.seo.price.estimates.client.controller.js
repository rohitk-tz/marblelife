(function () {
    angular.module(SchedulerConfiguration.moduleName).controller("SeoPriceModal",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "Toaster", "ManagePriceEstimateService", "$q", "$state", "APP_CONFIG", "$filter",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, toaster, managePriceEstimateService, $q, $state, config, $filter) {
                var vm = this;
                vm.save = save;
                vm.count = 0;
                vm.pagingOptions = config.pagingOptions;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.goToEstimate = goToEstimate;
                vm.searchText = modalParam.SearchText;
                vm.getSeoHistry = getSeoHistry;
                vm.pageChange = pageChange;
                vm.seoSearchFilter = {};
                vm.seoSearchFilter.text = vm.searchText;
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
                vm.seoSearchFilter = {
                    text: vm.searchText,
                    pageNumber: 1,
                    pageSize: config.defaultPageSize,
                }
                vm.seoSave = {
                    note: ''

                }
                function getSeoHistry() {
                    return managePriceEstimateService.getSeoHistry(vm.seoSearchFilter).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.seoHistryList = result.data.seoHistryListModel;
                            //vm.seoHistryList = $filter('orderBy')(result.data.seoHistryListModel, reverse)
                            vm.count = result.data.count;
                        }
                    });
                }
                function goToEstimate(seoCharges) {
                    var url = $state.href('core.layout.scheduler.estimate', { id: seoCharges.estimateId, previousView: 'Month', rowId: seoCharges.schedulerId });
                    window.open(url, '_blank');
                }

                function save(item) {
                    var seoModel = {};
                    seoModel.hoiningMeasurementId = item.hoiningMeasurementId;
                    seoModel.notes = item.notes;
                    return managePriceEstimateService.saveSeoNotes(item).then(function (result) {
                        if (result != null && result.data != null) {
                            toaster.show('Notes added Successfully!!');
                        }
                    });
                }

                function pageChange() {
                    getSeoHistry();
                };
                $q.all([getSeoHistry()]);
            }
        ]
    );
}());