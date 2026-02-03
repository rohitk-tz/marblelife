(function () {
    'use strict';
    var SortColumns = {
        Id: 'Id',
        Franchisee: 'Franchisee',
        UploadedDate: 'UploadedDate',
        StartDate: 'StartDate',
        EndDate: 'EndDate',
        Frequency: 'Frequency',
        TotalAmount: 'TotalAmount',
        PaidAmount: 'PaidAmount',
        Customers: 'Customers',
        Invoices: 'Invoices',
        LastInvoiceId: 'LastInvoiceId',
        Status: 'Status'
    };
    angular.module(SchedulerConfiguration.moduleName).controller("ModalAddCountryController",
        ["$scope", "$rootScope", "$state", "$q", "APP_CONFIG", "$uibModal", "FranchiseeService", "GeoCodeService", "FileService", "Notification", "Toaster", "$filter", "modalParam","$uibModalInstance",
            function ($scope, $rootScope, $state, $q, config, $uibModal, franchiseeService, geoCodeService, fileService, notification, toaster, $filter, modalParam, $uibModalInstance) {

                var vm = this;
                vm.countryName="USA/Canada/Other"
                vm.query = modalParam.Query;
                vm.close = close;
                vm.countryList = [];
                vm.downloadAllGeoCode = downloadAllGeoCode;
                vm.resetSeachOption = resetSeachOption;
                vm.countryList.push({ display: 'All', value: 0 },{ display: 'USA/CANADA/Others', value: 1 }, { display: 'Mexico', value: 8 });

                function close() {
                    $uibModalInstance.dismiss();
                }

                function downloadAllGeoCode() {
                    vm.downloading = true;
                    return geoCodeService.downloadAllGeoCode(vm.query).then(function (result) {
                        var date = $filter('date')(new Date(), "dd_MM_yyyy");
                        var fileName = "Geo_Code_" + date + ".xlsx";
                        fileService.downloadFile(result.data, fileName);
                        vm.downloading = false;
                        vm.selectedDownload = false;
                        vm.downloadAll = true;
                        $('#exampleModalCenter').modal("hide");
                    }, function () { vm.downloading = false; });
                }


                function resetSeachOption(query) {
                    if (query.countryId == 0) {
                        vm.countryName = "For All Countries";
                    }
                    else if (query.countryId == 1) {
                        vm.countryName = "USA/CANADA/Others";
                    }
                    else if (query.countryId == 8) {
                        vm.countryName = "Mexico";
                    }
                }

                $scope.$emit("update-title", "Manage Geo Code");

                $q.all([]);

            }]);
}());