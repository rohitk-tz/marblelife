(function () {
    'use strict';

    angular.module(SalesConfiguration.moduleName).controller("ListBatchSalesController",

      ["$scope", "$rootScope", "$state", "$stateParams", "$q", "APP_CONFIG", "SalesService",
  function ($scope, $rootScope, $state, $stateParams, $q, config, salesService) {

      var vm = this;
      vm.salesDataUploadId = $stateParams.salesDataUploadId;
      vm.getBatchSalesData = getBatchSalesData;
      vm.ServiceTypeCategory = DataHelper.ServiceTypeCategory;

      function getBatchSalesData(salesDataUploadId) {
          return salesService.getBatchSalesData(salesDataUploadId).then(function (result) {
              if (result != null && result.data != null) {
                  vm.batchSalesData = result.data;
              }
          });
      }

      function mergeClassCollections() {
          vm.mergedList = [];
          angular.forEach(vm.batchSalesData.classes, function (item) {
              angular.forEach(vm.batchSalesData.collection, function (innerItem) {
                  if (item.value == innerItem.classId) {
                      vm.mergedList.push(item);
                      vm.mergedList.push(innerItem);
                  }
              });
          });
      }

      function init() {
          getBatchSalesData(vm.salesDataUploadId).then(function () {
              mergeClassCollections();
              processServiceTypes();
          });

      }

      function processServiceTypes() {
          vm.restorationFieldsLength = 0;
          vm.maintenaceFieldsLength = 0;

          if (vm.batchSalesData.services == null || vm.batchSalesData.services.length < 1) return;

          angular.forEach(vm.batchSalesData.services, function (service) {
              if (service.typeId == vm.ServiceTypeCategory.Restoration) {
                  vm.restorationFieldsLength += 1;
              }
              else {
                  vm.maintenaceFieldsLength += 1;
              }
          });
      }

      $scope.$emit("update-title", "Royalty Report");

      init();
  }]);

}());