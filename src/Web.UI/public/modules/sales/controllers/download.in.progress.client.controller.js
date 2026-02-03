(function () {
    'use strict';
    angular.module(SalesConfiguration.moduleName).controller("DownloadInProgressController", ["$state", "$stateParams", "$scope", "$q", "APP_CONFIG",
        "CustomerService", "FileService", "$rootScope", "$uibModal", "Toaster", "FranchiseeService", "LocalStorageService",
        function ($state, $stateParams, $scope, $q, config, customerService, fileService, $rootScope, $uibModal, toaster, franchiseeService, LocalStorageService) {
            var vm = this;
            vm.customers = [];
            vm.query = {
                franchiseeId: 0,
                text: '',
                pageNumber: 1,
                pageSize: config.defaultPageSize,
                sort: { order: 0, propName: '' },
                dateCreated: null,
                dateModified: null,
                receiveNotification: '',
                toDate: null,
                fromDate: null,
                advancedText: '',
                advancedSearchBy: ''
            };
            vm.query.franchiseeId = $stateParams.franchiseeId;
            vm.query.text = $stateParams.text;
            vm.query.pageNumber = $stateParams.pageNumber;
            vm.query.pageSize = $stateParams.pageSize;
            vm.query.sort.order = $stateParams.sortOrder;
            vm.query.sort.propName = $stateParams.sortPropName;
            vm.query.dateCreated = $stateParams.dateCreated;
            vm.query.dateModified = $stateParams.dateModified;
            vm.query.receiveNotification = $stateParams.receiveNotification;
            vm.query.toDate = $stateParams.toDate;
            vm.query.fromDate = $stateParams.fromDate;
            vm.query.advancedText = $stateParams.advancedText;
            vm.query.advancedSearchBy = $stateParams.advancedSearchBy;

            $scope.editMode = false;
            vm.downloadCustomerList = downloadCustomerList;
            function downloadCustomerList() {
                vm.downloading = true;
                return customerService.downloadCustomerList(vm.query).then(function (result) {
                    var fileName = "customer.xlsx";
                    fileService.downloadFile(result.data, fileName);
                    vm.downloading = false;
                    window.close();

                },
                    function () {
                        vm.downloading = false;
                        window.close();
                    });
            }

            //$scope.$emit("update-title", "Customer List");
            $q.all([downloadCustomerList()]);

        }]);
}());