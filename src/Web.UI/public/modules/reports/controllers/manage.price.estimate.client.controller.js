(function () {
    'use strict';

    var SortColumns = {
        Material: 'Material',
        CorporatePrice: 'CorporatePrice',
        AverageFranchiseePrice: 'AverageFranchiseePrice',
        MaximumFranchiseePrice: 'MaximumFranchiseePrice',
        FranchiseeName: 'FranchiseeName'
    };

    angular.module(SchedulerConfiguration.moduleName).controller("ManagePriceEstimateController",
        ["$scope", "$rootScope", "$state", "$q", "APP_CONFIG", "$uibModal", "ManagePriceEstimateService", "FranchiseeService", "FileService", "Notification", "Toaster", "$filter", "SalesService", "MarketingLeadService",
            function ($scope, $rootScope, $state, $q, config, $uibModal, managePriceEstimateService, franchiseeService, fileService, notification, toaster, $filter, salesService, marketingLeadService) {
                var vm = this;

                vm.isPriceEstimateTab = true;
                vm.isTimeEstimateTab = false;
                vm.seoHistry = seoHistry;
                vm.changeTab = changeTab;
                vm.changeTimeEstimateTab = changeTimeEstimateTab;
                vm.Roles = DataHelper.Role;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.save = save;
                vm.savePriceOfFranchisee = savePriceOfFranchisee;
                vm.getFranchiseePrice = getFranchiseePrice;
                vm.changeServiceActive = changeServiceActive;
                vm.changeFranchiseeActive = changeFranchiseeActive;
                vm.getCategoryCollection = getCategoryCollection;
                vm.resetDropdowns = resetDropdowns;
                vm.disableOtherCategories = disableOtherCategories;
                vm.showPriceEstimates = showPriceEstimates;
                vm.openBulkUpdatePopUp = openBulkUpdatePopUp;
                vm.checkuncheckAllFranchisee = checkuncheckAllFranchisee;
                vm.getShiftCharges = getShiftCharges;
                vm.saveShiftCharges = saveShiftCharges;
                vm.getReplacementCharges = getReplacementCharges;
                vm.calculateTotalReplacementCost = calculateTotalReplacementCost;
                vm.saveReplacementCharges = saveReplacementCharges;
                vm.getMaintenanceCharges = getMaintenanceCharges;
                vm.saveMaintenanceCharges = saveMaintenanceCharges;
                vm.getFloorGrindingAdjustment = getFloorGrindingAdjustment;
                vm.saveFloorGrindingAdjustmentNote = saveFloorGrindingAdjustmentNote;
                vm.saveAndNext = saveAndNext;
                vm.finish = finish;
                vm.corporateValueChanged = corporateValueChanged;
                vm.uploadExcel = uploadExcel;
                vm.getUploadHistory = getUploadHistory;
                vm.sorting = sorting;
                vm.addViewNotes = addViewNotes;
                vm.listOfServiceList = [];
                vm.serviceTypeIds = [];
                vm.listOfServiceIds = [];
                vm.exportExcel = exportExcel;
                vm.downloading = false;
                vm.disableSaveButton = true;
                vm.floorGrindingAdjustment = {
                    floorGrindingAdjustmentList: [],
                    note: ''
                }
                vm.floorGrindingAdjustmentIsChanged = false;
                vm.isCategorySelected = false;
                vm.forEachFranchisee = false;
                vm.idList = [];
                vm.franchiseeIdList = [];
                if (vm.isFranchiseeAdmin) {
                    vm.disableCoorporatePrice = true;
                }
                else {
                    vm.disableCoorporatePrice = false;
                }
                if (vm.isSuperAdmin) {
                    vm.disableFranchiseePrice = true;
                }
                else {
                    vm.disableFranchiseePrice = false;
                }

                $scope.settings = {
                    scrollable: true,
                    enableSearch: true,
                    selectedToTop: true,
                    buttonClasses: 'btn btn-primary leader_btn'
                };
                $scope.translationTexts = {
                    checkAll: 'Select All',
                    uncheckAll: 'Deselect All',
                    selectGroup: 'Select All',
                    buttonDefaultText: "Select",
                    dynamicButtonTextSuffix: 'Selected'
                };
                $scope.selectEvents = {
                    onItemSelect: function (item) {
                        vm.getPriceEstimateQuery.serviceTypeId = [];
                        var Ids = vm.serviceTypeIds;
                        angular.forEach(Ids, function (value, key) {
                            vm.getPriceEstimateQuery.serviceTypeId.push(value.id);
                        });
                        resetDropdowns();
                    },
                    onItemDeselect: function (item) {
                        vm.getPriceEstimateQuery.serviceTypeId = [];
                        var Ids = vm.serviceTypeIds;
                        angular.forEach(Ids, function (value, key) {
                            vm.getPriceEstimateQuery.serviceTypeId.push(value.id);
                        });
                        resetDropdowns();
                    },
                    onSelectAll: function (item) {
                        vm.getPriceEstimateQuery.serviceTypeId = [];
                        var Ids = vm.serviceTypeIds;
                        angular.forEach(Ids, function (value, key) {
                            vm.getPriceEstimateQuery.serviceTypeId.push(value.id);
                        });
                        resetDropdowns();
                    },
                    onDeselectAll: function (item) {
                        vm.getPriceEstimateQuery.serviceTypeId = [];
                        resetDropdowns();
                    }
                }

                $scope.translationTextsListOfServices = {
                    checkAll: 'Select All',
                    uncheckAll: 'Deselect All',
                    selectGroup: 'Select All',
                    buttonDefaultText: "Select",
                    dynamicButtonTextSuffix: 'Selected'
                };

                $scope.selectEventsListOfService = {
                    onItemSelect: function (item) {
                        vm.getPriceEstimateQuery.listOfService = [];
                        var listOfServices = vm.listOfServiceIds;
                        angular.forEach(listOfServices, function (value, key) {
                            vm.getPriceEstimateQuery.listOfService.push(value.id);
                        });
                        resetDropdowns();
                    },
                    onItemDeselect: function (item) {
                        vm.getPriceEstimateQuery.listOfService = [];
                        var listOfServices = vm.listOfServiceIds;
                        angular.forEach(listOfServices, function (value, key) {
                            vm.getPriceEstimateQuery.listOfService.push(value.id);
                        });
                        resetDropdowns();
                    },
                    onSelectAll: function (item) {
                        vm.getPriceEstimateQuery.listOfService = [];
                        var listOfServices = vm.listOfServiceIds;
                        angular.forEach(listOfServices, function (value, key) {
                            vm.getPriceEstimateQuery.listOfService.push(value.id);
                        });
                        resetDropdowns();
                    },
                    onDeselectAll: function (item) {
                        vm.getPriceEstimateQuery.listOfService = [];
                        resetDropdowns();
                    }
                }

                vm.SortColumns = SortColumns;

                function changeTab(tabNo) {
                    if (tabNo == 1) {
                        vm.isPriceEstimateTab = true;
                        vm.isTimeEstimateTab = false;
                        vm.isTimeEstimateShiftChargesTab = false;
                        vm.isTimeEstimateReplacementChargesTab = false;
                        vm.isTimeEstimateMaintenanceChargesTab = false;
                        vm.timeEstimateFloorGrindingAdjustment = false;
                        angular.element(document.querySelector("#timeEstimate")).removeClass("active");
                        angular.element(document.querySelector("#priceEstimate")).addClass("active");
                        angular.element(document.querySelector("#timeEstimateShiftCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateReplacementCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateMaintenanceCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateFloorGrindingAdjustment")).removeClass("active");
                    }
                    if (tabNo == 2) {
                        vm.isPriceEstimateTab = false;
                        vm.isTimeEstimateTab = true;
                        vm.isTimeEstimateShiftChargesTab = true;
                        vm.isTimeEstimateReplacementChargesTab = false;
                        vm.isTimeEstimateMaintenanceChargesTab = false;
                        vm.timeEstimateFloorGrindingAdjustment = false;
                        angular.element(document.querySelector("#timeEstimate")).addClass("active");
                        angular.element(document.querySelector("#priceEstimate")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateShiftCharges")).addClass("active");
                        angular.element(document.querySelector("#timeEstimateReplacementCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateMaintenanceCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateFloorGrindingAdjustment")).removeClass("active");
                        getShiftCharges();
                    }
                }

                function changeTimeEstimateTab(tabNo) {
                    if (tabNo == 1) {
                        vm.isPriceEstimateTab = false;
                        vm.isTimeEstimateTab = true;
                        vm.isTimeEstimateShiftChargesTab = true;
                        vm.isTimeEstimateReplacementChargesTab = false;
                        vm.isTimeEstimateMaintenanceChargesTab = false;
                        vm.timeEstimateFloorGrindingAdjustment = false;
                        angular.element(document.querySelector("#timeEstimate")).addClass("active");
                        angular.element(document.querySelector("#priceEstimate")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateShiftCharges")).addClass("active");
                        angular.element(document.querySelector("#timeEstimateReplacementCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateMaintenanceCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateFloorGrindingAdjustment")).removeClass("active");
                        getShiftCharges();
                    }
                    if (tabNo == 2) {
                        vm.isPriceEstimateTab = false;
                        vm.isTimeEstimateTab = true;
                        vm.isTimeEstimateShiftChargesTab = false;
                        vm.isTimeEstimateReplacementChargesTab = true;
                        vm.isTimeEstimateMaintenanceChargesTab = false;
                        vm.timeEstimateFloorGrindingAdjustment = false;
                        angular.element(document.querySelector("#timeEstimate")).addClass("active");
                        angular.element(document.querySelector("#priceEstimate")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateShiftCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateReplacementCharges")).addClass("active");
                        angular.element(document.querySelector("#timeEstimateMaintenanceCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateFloorGrindingAdjustment")).removeClass("active");
                        getReplacementCharges();
                    }
                    if (tabNo == 3) {
                        vm.isPriceEstimateTab = false;
                        vm.isTimeEstimateTab = true;
                        vm.isTimeEstimateShiftChargesTab = false;
                        vm.isTimeEstimateReplacementChargesTab = false;
                        vm.isTimeEstimateMaintenanceChargesTab = true;
                        vm.timeEstimateFloorGrindingAdjustment = false;
                        angular.element(document.querySelector("#timeEstimate")).addClass("active");
                        angular.element(document.querySelector("#priceEstimate")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateShiftCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateReplacementCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateMaintenanceCharges")).addClass("active");
                        angular.element(document.querySelector("#timeEstimateFloorGrindingAdjustment")).removeClass("active");
                        getMaintenanceCharges();
                    }
                    if (tabNo == 4) {
                        vm.isPriceEstimateTab = false;
                        vm.isTimeEstimateTab = true;
                        vm.isTimeEstimateShiftChargesTab = false;
                        vm.isTimeEstimateReplacementChargesTab = false;
                        vm.isTimeEstimateMaintenanceChargesTab = false;
                        vm.timeEstimateFloorGrindingAdjustment = true;
                        angular.element(document.querySelector("#timeEstimate")).addClass("active");
                        angular.element(document.querySelector("#priceEstimate")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateShiftCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateReplacementCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateMaintenanceCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateFloorGrindingAdjustment")).addClass("active");
                        getFloorGrindingAdjustment();
                    }
                }

                function getCategoryCollection() {
                    return managePriceEstimateService.getCategoryCollection().then(function (result) {
                        vm.categories = result.data;
                    });
                }

                function getServicesCollection() {
                    vm.serviceTypeList = [];
                    return franchiseeService.getServiceTypeCollectionForInvoice().then(function (result) {
                        vm.serviceTypeForInvoices = result.data;
                        angular.forEach(vm.serviceTypeForInvoices, function (value, key) {
                            vm.serviceTypeList.push({ label: value.display, id: value.value });
                        });
                    });
                }

                function getListOfServices() {
                    vm.listOfServiceList = [];
                    return franchiseeService.getListOfServiceCollection().then(function (result) {
                        vm.listOfServiceForInvoices = result.data;
                        angular.forEach(vm.listOfServiceForInvoices, function (value, key) {
                            vm.listOfServiceList.push({ label: value.display, id: value.value });
                        });
                    });
                }

                function getFranchiseePrice(subitem) {
                    if (subitem.isFranchiseePriceZero) {
                        subitem.franchiseePrice = subitem.corporatePrice
                    }
                    if (subitem.isFranchiseeAdditionalPriceZero) {
                        subitem.franchiseeAdditionalPrice = subitem.corporateAdditionalPrice
                    }
                }

                function resetDropdowns() {
                    vm.idList = [];
                    vm.activeAllServices = false;
                    if (vm.getPriceEstimateQuery.categoryId != 0) {
                        vm.isCategorySelected = true;
                    }
                    return managePriceEstimateService.getPriceEstimateCollection(vm.getPriceEstimateQuery).then(function (result) {
                        vm.priceEstimateCollection = result.data.priceEstimateViewModelList;
                        vm.taxForServices = result.data.taxForServices,
                            vm.taxForProducts = result.data.taxForProducts;
                        angular.forEach(vm.priceEstimateCollection, function (value, key) {
                            value.isActiveService = false;
                            value.isAllServiceSelected = false;
                            value.isDisabledService = false;
                        });
                    });
                }

                vm.getPriceEstimateQuery = {
                    categoryId: 0,
                    serviceTypeId: [],
                    serviceTagId: 0,
                    listOfService: [],
                    sortingColumn: '',
                    sortingOrder: 0,
                    showAllFranchisee: false,
                    isSuperAdmin: vm.isSuperAdmin,
                    isFranchiseeAdmin: vm.isFranchiseeAdmin
                }

                function getList() {
                    vm.getPriceEstimateQuery.serviceTagSelectedIds = vm.idList;
                    vm.getPriceEstimateQuery.selectedCategoryId = vm.selectedCategoryId;
                    vm.getPriceEstimateQuery.showAllFranchisee = false;
                    return managePriceEstimateService.getPriceEstimateCollection(vm.getPriceEstimateQuery).then(function (result) {
                        vm.priceEstimateCollection = result.data.priceEstimateViewModelList;
                        if (vm.idList.length > 0) {
                            vm.getPriceEstimateQuery.serviceTagId = vm.idList[0];
                            vm.getPriceEstimateQuery.showAllFranchisee = true;
                            vm.getPriceEstimateQuery.selectedFranchiseeIds = vm.franchiseeIdList;
                            return managePriceEstimateService.getPriceEstimate(vm.getPriceEstimateQuery).then(function (result) {
                                vm.priceEstimate = result.data;
                                vm.priceEstimateServices = vm.priceEstimate.priceEstimateServices;
                                vm.activeAllFranchisee = false;
                            });
                        }
                        else {
                            vm.isCategorySelected = false;
                        }
                    });
                }

                vm.savePriceEstimateFranchiseeWiseQuery = {
                    serviceTagId: 0,
                    franchiseeId: 0,
                    corporatePrice: 0,
                    corporateAdditionalPrice: 0,
                    franchiseePrice: 0,
                    franchiseeAdditionalPrice: 0,
                    alternativeSolution: null
                };

                function savePriceOfFranchisee() {
                    vm.savePriceEstimateFranchiseeWiseQuery.serviceTagId = vm.idList;
                    var nonZeroPriceEstimateServices = [];
                    angular.forEach(vm.priceEstimateServices, function (value, key) {
                        if (value.franchiseePrice != null
                            || value.franchiseeAdditionalPrice != null
                            || value.corporatePrice != null
                            || value.corporateAdditionalPrice != null
                        ) {
                            nonZeroPriceEstimateServices.push(value);
                        }
                    });
                    if (nonZeroPriceEstimateServices.length == 0) {
                        toaster.error("Please Enter Price for Atleast One Franchisee!!");
                        return;
                    }
                    vm.savePriceEstimateFranchiseeWiseQuery.priceEstimateServices = nonZeroPriceEstimateServices;
                    return managePriceEstimateService.savePriceEstimateFranchiseeWise(vm.savePriceEstimateFranchiseeWiseQuery).then(function (result) {
                        if (result != null && result.data != null) {
                            if (result.data) {
                                toaster.show("Changes done successfully!!");
                                getList();
                            }
                            else {
                                toaster.error("Error in Saving Changes!!");
                            }
                        }
                    });
                }

                function save() {
                    vm.savePriceEstimateList = [];
                    var model = {};
                    vm.disableSaveButton = true;
                    angular.forEach(vm.priceEstimateCollection, function (value, key) {
                        if (value.bulkCorporatePrice != null) {
                            if (value.isChanged) {
                                vm.savePriceEstimateList.push({ bulkCorporatePrice: value.bulkCorporatePrice, serviceTagId: value.serviceTagId, bulkCorporateAdditionalPrice: value.bulkCorporateAdditionalPrice });
                            }
                        }
                    });
                    if (vm.savePriceEstimateList.length == 0) {
                        return;
                    }
                    else {
                        model.priceEstimateSaveBulkModel = vm.savePriceEstimateList;
                        return managePriceEstimateService.saveBulkCorporatePrice(model).then(function (result) {
                            if (result != null && result.data != null) {
                                if (result.data) {
                                    toaster.show("Changes done successfully!!");
                                    getList();
                                }
                                else {
                                    toaster.error("Error in Saving Changes!!");
                                }
                            }
                        });
                    }
                }

                function changeServiceActive() {
                    vm.idList = [];
                    angular.forEach(vm.priceEstimateCollection, function (value, key) {
                        if (vm.activeAllServices && !value.isDisabledService) {
                            vm.idList.push(value.serviceTagId);
                            vm.selectedCategoryId = value.categoryId;
                            value.isActiveService = true;
                            value.isAllServiceSelected = true;
                            vm.hasTwoPriceColumns = value.hasTwoPriceColumns;
                        }
                        else if (vm.activeAllServices && value.isDisabledService) {
                            value.isDisabledService = value.isDisabledService;
                        }
                        else {
                            value.isActiveService = false;
                            value.isAllServiceSelected = false;
                            value.isDisabledService = false;
                            vm.hasTwoPriceColumns = false;
                        }
                    });
                    if (vm.idList.length > 0) {
                        vm.getPriceEstimateQuery.serviceTagId = vm.idList[0];
                        vm.getPriceEstimateQuery.showAllFranchisee = true;
                        return managePriceEstimateService.getPriceEstimate(vm.getPriceEstimateQuery).then(function (result) {
                            vm.priceEstimate = result.data;
                            vm.priceEstimateServices = vm.priceEstimate.priceEstimateServices;
                            vm.activeAllFranchisee = false;
                        });
                    }
                    if (vm.getPriceEstimateQuery.categoryId == 0) {
                        vm.isCategorySelected = false;
                    }
                    else {
                        vm.isCategorySelected = true;
                    }
                }

                function disableOtherCategories(item) {
                    vm.selectedCategoryId = item.categoryId;
                    vm.hasTwoPriceColumns = item.hasTwoPriceColumns;
                    if (vm.idList == undefined) {
                        vm.idList = [];
                        vm.idList.push(item.serviceTagId);
                        vm.isCategorySelected = true;
                    }
                    else {
                        if (item.isActiveService) {
                            vm.idList.push(item.serviceTagId);
                            vm.isCategorySelected = true;
                        }
                        else {
                            var index = vm.idList.indexOf($filter('filter')(vm.idList, item.serviceTagId, true)[0]);
                            vm.idList.splice(index, 1);
                            vm.activeAllServices = false;
                        }
                    }
                    angular.forEach(vm.priceEstimateCollection, function (value, key) {
                        if (value.categoryId.toString() == item.categoryId || vm.idList.length == 0) {
                            value.isDisabledService = false;
                        }
                        else {
                            value.isDisabledService = true;
                        }
                    });
                    if (vm.idList.length > 0) {
                        vm.getPriceEstimateQuery.serviceTagId = vm.idList[0];
                        vm.getPriceEstimateQuery.showAllFranchisee = true;
                        return managePriceEstimateService.getPriceEstimate(vm.getPriceEstimateQuery).then(function (result) {
                            vm.priceEstimate = result.data;
                            vm.priceEstimateServices = vm.priceEstimate.priceEstimateServices;
                            vm.activeAllFranchisee = false;
                        });
                    }
                    else {
                        vm.isCategorySelected = false;
                    }
                }

                function changeFranchiseeActive() {
                    vm.franchiseeIdList = [];
                    angular.forEach(vm.priceEstimateServices, function (value, key) {
                        if (vm.activeAllFranchisee) {
                            vm.franchiseeIdList.push(value.franchiseeId);
                            value.isActiveFranchisee = true;
                        }
                        else {
                            value.isActiveFranchisee = false;
                        }
                    });
                }

                function checkuncheckAllFranchisee(subitem) {
                    if (vm.franchiseeIdList == undefined) {
                        vm.franchiseeIdList = [];
                        vm.franchiseeIdList.push(subitem.franchiseeId);
                    }
                    else {
                        if (subitem.isActiveFranchisee) {
                            vm.franchiseeIdList.push(subitem.franchiseeId);
                        }
                        else {
                            var index = vm.franchiseeIdList.indexOf($filter('filter')(vm.franchiseeIdList, subitem.franchiseeId, true)[0]);
                            vm.franchiseeIdList.splice(index, 1);
                            vm.activeAllFranchisee = false;
                        }
                    }
                }

                function openBulkUpdatePopUp(isBulkCorporatePrice) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/modal.bulk.price.estimates.client.view.html',
                        controller: 'BulkPriceEstimatesModal',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    IsBulkCorporatePrice: isBulkCorporatePrice,
                                    SelectedFranchisees: vm.franchiseeIdList,
                                    SelectedServices: vm.idList,
                                    HasTwoPriceColumns: vm.hasTwoPriceColumns,
                                    DisableCoorporatePrice: vm.disableCoorporatePrice,
                                    DisableFranchiseePrice: vm.disableFranchiseePrice
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                        getList();
                    });
                }

                function showPriceEstimates(serviceTagId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/modal.show.price.estimates.client.view.html',
                        controller: 'ShowPriceEstimatesModal',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    ServiceTagId: serviceTagId
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                    });
                }

                function getShiftCharges() {
                    return managePriceEstimateService.getShiftCharges().then(function (result) {
                        vm.shiftCharges = result.data;
                    });
                }

                function saveShiftCharges() {
                    return managePriceEstimateService.saveShiftCharges(vm.shiftCharges).then(function (result) {
                        if (result.data) {
                            toaster.show("Changes Saved Successfully!")
                        }
                    });
                }

                function getReplacementCharges() {
                    return managePriceEstimateService.getReplacementCharges().then(function (result) {
                        vm.replacementCharges = result.data.replacementChargesList;
                    });
                }

                function calculateTotalReplacementCost(index) {
                    var replacementCharge = vm.replacementCharges[index];
                    vm.replacementCharges[index].isChanged = true;
                    if (replacementCharge.costOfRemovingTile == '') {
                        vm.replacementCharges[index].totalReplacementCost = replacementCharge.costOfInstallingTile + replacementCharge.costOfTileMaterial;
                    }
                    else if (replacementCharge.costOfInstallingTile == '') {
                        vm.replacementCharges[index].totalReplacementCost = replacementCharge.costOfRemovingTile + replacementCharge.costOfTileMaterial;

                    }
                    else if (replacementCharge.costOfTileMaterial == '') {
                        vm.replacementCharges[index].totalReplacementCost = replacementCharge.costOfRemovingTile + replacementCharge.costOfInstallingTile;
                    }
                    else {
                        vm.replacementCharges[index].totalReplacementCost = replacementCharge.costOfRemovingTile + replacementCharge.costOfInstallingTile + replacementCharge.costOfTileMaterial;
                    }
                }

                function saveReplacementCharges() {
                    var model = {
                        replacementChargesList: []
                    }
                    var replacementChargesList = $filter('filter')(vm.replacementCharges, { isChanged: true }, true);
                    model.replacementChargesList = replacementChargesList;
                    if (model.replacementChargesList.length > 0) {
                        return managePriceEstimateService.saveReplacementCharges(model).then(function (result) {
                            if (result.data) {
                                toaster.show("Changes Saved Successfully!")
                            }
                        });
                    }
                }

                function getMaintenanceCharges() {
                    return managePriceEstimateService.getMaintenanceCharges().then(function (result) {
                        vm.maintenanceCharges = result.data.maintenanceChargesList;
                    });
                }

                function saveMaintenanceCharges() {
                    var model = {
                        maintenanceChargesList: []
                    }
                    var maintenanceChargesList = $filter('filter')(vm.maintenanceCharges, { isChanged: true }, true);
                    model.maintenanceChargesList = maintenanceChargesList;
                    if (model.maintenanceChargesList.length > 0) {
                        return managePriceEstimateService.saveMaintenanceCharges(model).then(function (result) {
                            if (result.data) {
                                toaster.show("Changes Saved Successfully!")
                            }
                        });
                    }
                }

                function getFloorGrindingAdjustment() {
                    return managePriceEstimateService.getFloorGrindingAdjustment().then(function (result) {
                        vm.floorGrindingAdjustment.floorGrindingAdjustmentList = result.data.floorGrindingAdjustmentList;
                        vm.floorGrindingAdjustment.note = result.data.note;
                    });
                }

                function saveAndNext() {
                    if (vm.isTimeEstimateShiftChargesTab) {
                        saveShiftCharges();
                        vm.isPriceEstimateTab = false;
                        vm.isTimeEstimateTab = true;
                        vm.isTimeEstimateShiftChargesTab = false;
                        vm.isTimeEstimateReplacementChargesTab = true;
                        vm.isTimeEstimateMaintenanceChargesTab = false;
                        vm.timeEstimateFloorGrindingAdjustment = false;
                        angular.element(document.querySelector("#timeEstimate")).addClass("active");
                        angular.element(document.querySelector("#priceEstimate")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateShiftCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateReplacementCharges")).addClass("active");
                        angular.element(document.querySelector("#timeEstimateMaintenanceCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateFloorGrindingAdjustment")).removeClass("active");
                        getReplacementCharges();
                    }
                    else if (vm.isTimeEstimateReplacementChargesTab) {
                        saveReplacementCharges();
                        vm.isPriceEstimateTab = false;
                        vm.isTimeEstimateTab = true;
                        vm.isTimeEstimateShiftChargesTab = false;
                        vm.isTimeEstimateReplacementChargesTab = false;
                        vm.isTimeEstimateMaintenanceChargesTab = true;
                        vm.timeEstimateFloorGrindingAdjustment = false;
                        angular.element(document.querySelector("#timeEstimate")).addClass("active");
                        angular.element(document.querySelector("#priceEstimate")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateShiftCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateReplacementCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateMaintenanceCharges")).addClass("active");
                        angular.element(document.querySelector("#timeEstimateFloorGrindingAdjustment")).removeClass("active");
                        getMaintenanceCharges();
                    }
                    else if (vm.isTimeEstimateMaintenanceChargesTab) {
                        saveMaintenanceCharges();
                        vm.isPriceEstimateTab = false;
                        vm.isTimeEstimateTab = true;
                        vm.isTimeEstimateShiftChargesTab = false;
                        vm.isTimeEstimateReplacementChargesTab = false;
                        vm.isTimeEstimateMaintenanceChargesTab = false;
                        vm.timeEstimateFloorGrindingAdjustment = true;
                        angular.element(document.querySelector("#timeEstimate")).addClass("active");
                        angular.element(document.querySelector("#priceEstimate")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateShiftCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateReplacementCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateMaintenanceCharges")).removeClass("active");
                        angular.element(document.querySelector("#timeEstimateFloorGrindingAdjustment")).addClass("active");
                        getFloorGrindingAdjustment();
                    }
                }

                function saveFloorGrindingAdjustmentNote() {
                    var model = {
                        note: ""
                    }
                    model.note = vm.floorGrindingAdjustment.note;
                    if (vm.floorGrindingAdjustmentIsChanged) {
                        return managePriceEstimateService.saveFloorGrindingAdjustmentNote(model).then(function (result) {
                            if (result.data) {
                                toaster.show("Changes Saved Successfully!");
                            }
                        });
                    }
                }

                function finish() {
                    vm.isPriceEstimateTab = true;
                    vm.isTimeEstimateTab = false;
                    vm.isTimeEstimateShiftChargesTab = false;
                    vm.isTimeEstimateReplacementChargesTab = false;
                    vm.isTimeEstimateMaintenanceChargesTab = false;
                    vm.timeEstimateFloorGrindingAdjustment = false;
                    angular.element(document.querySelector("#priceEstimate")).addClass("active");
                    angular.element(document.querySelector("#timeEstimate")).removeClass("active");
                    angular.element(document.querySelector("#timeEstimateShiftCharges")).removeClass("active");
                    angular.element(document.querySelector("#timeEstimateReplacementCharges")).removeClass("active");
                    angular.element(document.querySelector("#timeEstimateMaintenanceCharges")).removeClass("active");
                    angular.element(document.querySelector("#timeEstimateFloorGrindingAdjustment")).removeClass("active");
                    if (vm.floorGrindingAdjustmentIsChanged) {
                        saveFloorGrindingAdjustmentNote();
                    }
                }

                function corporateValueChanged(index) {
                    vm.priceEstimateCollection[index].isChanged = true;
                    vm.disableSaveButton = false;
                }

                function seoHistry(searchText) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/modal-seo-price-estimates.client.view.html',
                        controller: 'SeoPriceModal',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    SearchText: searchText,
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                    });
                }

                function exportExcel() {
                    vm.downloading = true;
                    return managePriceEstimateService.downloadPriceEstimateData(vm.getPriceEstimateQuery).then(function (result) {
                        var fileName = "PriceEstimateData.xlsx";
                        fileService.downloadFile(result.data, fileName);
                        vm.downloading = false;
                    }, function () { vm.downloading = false; });
                }

                function uploadExcel() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/upload-priceestimate.client.view.html',
                        controller: 'UploadPriceEstimateController',
                        controllerAs: 'vm',
                        backdrop: 'static'
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                    });
                }

                function getUploadHistory() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/modal.showpriceestimatehistory.client.view.html',
                        controller: 'ShowPriceEstimateHistoryModal',
                        controllerAs: 'vm',
                        size: 'lg',
                        backdrop: 'static'
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                    });
                }

                function sorting(propName) {
                    vm.getPriceEstimateQuery.sortingColumn = propName;
                    vm.getPriceEstimateQuery.sortingOrder = (vm.getPriceEstimateQuery.sortingOrder == 0) ? 1 : 0;
                    getList();
                }

                function addViewNotes(item) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/modal.price.estimate.notes.client.html',
                        controller: 'ModalPriceEstimateNotesController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    PriceEstimate: item,
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                    });
                    
                }

                $scope.$emit("update-title", "Manage Price/Time Estimate");
                $q.all([getList(), getCategoryCollection(), getServicesCollection(), getListOfServices()]);
            }]);
}());