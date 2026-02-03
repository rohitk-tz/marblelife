(function () {

    var SortColumns = {
        ID: 'ID',
        PhoneNumber: 'PhoneNumber',
        PhoneLabel: 'PhoneLabel',
        Franchisee: 'Franchisee',
        Tag: 'Tag'
    };

    angular.module(ReportsConfiguration.moduleName).controller("ManagePhoneLabelController",
        ["$scope", "$rootScope", "$state", "$q", "RoutingNumberService", "FranchiseeService", "Toaster", "APP_CONFIG", "FileService",
    function ($scope, $rootScope, $state, $q, routingNumberService, franchiseeService, toaster, config, fileService) {

        var vm = this;
        vm.query = {
            pageNumber: 1,
            franchiseeId: 0,
            tagId: 0,
            pageSize: config.defaultPageSize,
            text: '',
            sort: { order: 0, propName: '' }
        };
        vm.getRoutingNumbers = getRoutingNumbers;
        vm.pagingOptions = config.pagingOptions;
        vm.currentPage = vm.query.pageNumber;
        vm.count = 0;
        vm.sorting = sorting;
        vm.SortColumns = SortColumns;
        vm.pageChange = pageChange;
        vm.updateFranchisee = updateFranchisee;
        vm.Roles = DataHelper.Role;
        vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
        vm.resetSearch = resetSearch;
        vm.refresh = refresh;
        vm.downloadPhoneLabels = downloadPhoneLabels;
        vm.ids = [];
        vm.updateTag = updateTag;
        vm.goToList = goToList;

        function goToList() {
            $state.go('core.layout.report.callDetail');
        }

        function getRoutingNumbers() {
            return routingNumberService.getRoutingNumbers(vm.query).then(function (result) {
                if (result != null && result.data != null) {
                    vm.ids = [];
                    vm.routingNumbers = result.data.collection;
                    vm.count = result.data.pagingModel.totalRecords;
                    vm.query.sort.order = result.data.filter.sortingOrder;
                    addResultToList(vm.routingNumbers);
                }
            });
        }

        function downloadPhoneLabels() {
            vm.downloading = true;
            return routingNumberService.downloadPhoneLabels(vm.query).then(function (result) {
                var fileName = "phoneLabels.xlsx";
                fileService.downloadFile(result.data, fileName);
                vm.downloading = false;
            }, function () { vm.downloading = false; });
        }

        function addResultToList() {
            angular.forEach(vm.routingNumbers, function (value, key) {
                vm.ids.push(value.id);
            })
        }

        function resetSearch() {
            vm.query.text = '';
            vm.query.franchiseeId = 0;
            vm.query.tagId = 0;
            vm.query.pageNumber = 1;
            getRoutingNumbers();
        }
        function refresh() {
            getRoutingNumbers();
        }

        function getFranchiseeCollection() {
            return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                vm.franchiseeCollection = result.data;
                vm.franchiseeCollection.push({ display: "None", value: null });
            });
        }

        function getTagList() {
            return routingNumberService.getTagList().then(function (result) {
                vm.tagList = result.data;
                vm.tagList.push({ display: "None", value: null });
            });
        }

        function pageChange() {
            getRoutingNumbers();
        };

        function updateTag(id, tagId) {
            vm.isProcessing = true;
            if (tagId <= 0) {
                tagId = null;
            }
            return routingNumberService.updateTag(id, tagId).then(function (result) {
                if (result.data != true)
                    toaster.error(result.message.message);
                else
                    toaster.show(result.message.message);

                getRoutingNumbers();
                vm.isProcessing = false;
            });
        }

        function updateFranchisee(id, franchiseeId) {
            vm.isProcessing = true;
            if (franchiseeId <= 0) {
                franchiseeId = null;
            }
            return routingNumberService.updateFranchisee(id, franchiseeId).then(function (result) {
                if (result.data != true)
                    toaster.error(result.message.message);
                else
                    toaster.show(result.message.message);

                getRoutingNumbers();
                vm.isProcessing = false;
            });
        }

        function sorting(propName) {
            vm.query.sort.propName = propName;
            vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
            getRoutingNumbers();
        };

        $scope.$emit("update-title", "Manage Phone Labels");
        $q.all([getRoutingNumbers(), getFranchiseeCollection(), getTagList()]);
    }]);
}());