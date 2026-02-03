(function () {
    angular.module(SchedulerConfiguration.moduleName).controller("AddMeasurementsModal",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "Toaster", "$filter", "CustomerService", "$sce",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, toaster, $filter, customerService, $sce) {
                var vm = this;
                vm.viewSuggestion = viewSuggestion;
                $(function () {
                    $('[data-toggle="tooltip"]').tooltip()
                })
                vm.getHoiningMeasurementCheck = getHoiningMeasurementCheck;
                vm.addNewHoningMeasurement = addNewHoningMeasurement;
                vm.isClicked = false;
                vm.clearanceCheck = false;
                vm.clearanceValue = clearanceValue;
                vm.changeShift = changeShift;
                vm.calculateTimeCalcuationChangingShiftPrice = calculateTimeCalcuationChangingShiftPrice;
                vm.serviceTypeService = DataHelper.ServiceTypeInvoice;
                vm.calculateTimeCalcuation = calculateTimeCalcuation;
                vm.calculateSumTotalArea = calculateSumTotalArea;
                vm.getOption1Value = getOption1Value;
                vm.schedulerId = modalParam.JobSchedulerId;
                vm.priceServiceList = modalParam.PriceServiceList;
                vm.maintenanceChargesList = modalParam.MaintenanceChargesList;
                vm.estimateInvoiceInfo = modalParam.EstimateInvoiceInfo;
                vm.service = modalParam.Service;
                vm.sumOfAreaOriginal = vm.service.sumTotalArea;
                vm.sumTotalArea = 0;
                vm.ServiceName = modalParam.ServiceName;
                vm.unitTypeId = vm.service.unitType;
                vm.serviceTypeId = vm.service.serviceTypeId;
                vm.serviceType = DataHelper.ServiceTypeInvoice;
                vm.totalUnit = 0;
                vm.maintenanceChargesListCurrent = $filter('filter')(vm.maintenanceChargesList, { material: vm.service.typeOfStoneType2 }, true);
                vm.location = vm.service.locationIds;
                vm.franchiseeShiftPrice = modalParam.ShiftPrice;
                vm.classType = modalParam.ClassType;
                vm.changeTab = changeTab;
                vm.isCycleRestorationTabDisabled = true;
                vm.getCycleRestorationPlan = getCycleRestorationPlan;
                vm.getmarketingClassCollection = getmarketingClassCollection;
                vm.marketingClassChanged = marketingClassChanged;
                vm.getHoiningMeasurementLoad = getHoiningMeasurementLoad;
                vm.removeRows = removeRows;
                vm.rowsCount = 0;
                vm.rowsButtonDisable = true;
                vm.getShiftPriceForHoningMeasurement = getShiftPriceForHoningMeasurement;
                vm.calculateAreaofAService = calculateAreaofAService;
                vm.addedRows = 0;
                vm.timeEstimateDisable = true;
                vm.locationList = [];
                vm.honingMeasurementList = [];
                vm.isSealing = false;
                vm.isMachineSize = false;
                vm.cycleRestorationPlan = {
                    cycleRestorationPlanMonthlyList: []
                };
                vm.isCycleRestorationVisible = false;

                //Measurement Type Set
                vm.validItemsForTime = [
                    { serviceType: "CLEANSHIELD", typeOfStoneType2: "Granite", serviceName: "Hone" },
                    { serviceType: "CLEANSHIELD", typeOfStoneType2: "Limestone", serviceName: "Hone" },
                    { serviceType: "CLEANSHIELD", typeOfStoneType2: "Marble", serviceName: "Hone" },
                    { serviceType: "CLEANSHIELD", typeOfStoneType2: "Quartz", serviceName: "Hone" },
                    { serviceType: "CLEANSHIELD", typeOfStoneType2: "Travertine", serviceName: "Hone" },

                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep & Coat Epoxy Only" },
                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep & Coat Epoxy-Epoxy" },
                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep & Coat Epoxy-MVB Only" },
                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep & Coat Epoxy-Poly Spartic" },
                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep & Coat Poly Spartic Only" },

                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep & Epoxy Chip -(No MVB) Epoxy - ACS Chip - Epoxy" },
                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep & Epoxy Chip -(No MVB) Epoxy - Quartz Chip - Epoxy" },
                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep & Epoxy Chip -(No MVB) Epoxy - Vinyl Chip - Epoxy" },

                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep - (Epoxy-MVB) - PolySpartic Semiron - ACS Chip - Poly Spartic" },
                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep - (Epoxy-MVB) - PolySpartic Semiron - Quartz Chip - Poly Spartic" },
                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep - (Epoxy-MVB) - PolySpartic Semiron - Vinyl Chip - Poly Spartic" },

                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep - Epoxy Chip - Poly Sprtic (No MVB) EPOXY - Vinyl Chip - Poly Spartic" },
                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep - Epoxy Chip - Poly Sprtic (No MVB) Epoxy - ACS Chip - Poly Spartic" },
                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep - Epoxy Chip - Poly Sprtic (No MVB) Epoxy - Quartz Chip - Poly Spartic" },

                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep - Marblized Marblize" },

                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep - MVB - Epoxy Chip - Poly Spartic MVB - Epoxy - ACS Chip - Poly Spartic" },
                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep - MVB - Epoxy Chip - Poly Spartic MVB - Epoxy - Quartz Chip - Poly Spartic" },
                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep - MVB - Epoxy Chip - Poly Spartic MVB - Epoxy - Vinyl Chip - Poly Spartic" },

                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep - MVB - Poly Spartic - Poly Spartic Semiron - ACS Chip - Poly Spartic" },
                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep - MVB - Poly Spartic - Poly Spartic Semiron - Quartz Chip - Poly Spartic" },
                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep - MVB - Poly Spartic - Poly Spartic Semiron - Vinyl Chip - Poly Spartic" },

                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep - Poly Spactic - Chip - Poly Spartic - No MVB Poly Spartic - ACS Chip - Poly Spartic" },
                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep - Poly Spactic - Chip - Poly Spartic - No MVB Poly Spartic - Quartz Chip - Poly Spartic" },
                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Bundle Prep - Poly Spactic - Chip - Poly Spartic - No MVB Poly Spartic - Vinyl Chip - Poly Spartic" },

                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Hone Surface Prep" },

                    { serviceType: "CONCRETE-COUNTERTOPS", typeOfStoneType2: "Brick", serviceName: "Hone" },
                    { serviceType: "CONCRETE-COUNTERTOPS", typeOfStoneType2: "Concrete", serviceName: "Hone" },
                    { serviceType: "CONCRETE-COUNTERTOPS", typeOfStoneType2: "Concrete", serviceName: "Polish" },

                    { serviceType: "CONCRETE-OVERLAYMENTS", typeOfStoneType2: "Brick", serviceName: "Hone" },
                    { serviceType: "CONCRETE-OVERLAYMENTS", typeOfStoneType2: "Brick", serviceName: "Polish" },
                    { serviceType: "CONCRETE-OVERLAYMENTS", typeOfStoneType2: "Concrete", serviceName: "Hone" },
                    { serviceType: "CONCRETE-OVERLAYMENTS", typeOfStoneType2: "Concrete", serviceName: "Polish" },

                    { serviceType: "CONCRETE-REPAIR", typeOfStoneType2: "Concrete", serviceName: "Hone" },
                    { serviceType: "CONCRETE-REPAIR", typeOfStoneType2: "Concrete", serviceName: "Polish" },
                    { serviceType: "CONCRETE-REPAIR", typeOfStoneType2: "Terrazzo", serviceName: "Hone" },
                    { serviceType: "CONCRETE-REPAIR", typeOfStoneType2: "Terrazzo", serviceName: "Polish" },

                    { serviceType: "CONCRETE-STAIN", typeOfStoneType2: "Concrete", serviceName: "Bundle (Honing & Polishing)" },

                    { serviceType: "CONCRETE-POLISHING(ENDURACRETE)", typeOfStoneType2: "Concrete", serviceName: "Bundle (Honing & Polishing)" },
                    { serviceType: "CONCRETE-POLISHING(ENDURACRETE)", typeOfStoneType2: "Concrete", serviceName: "Honing" },
                    { serviceType: "CONCRETE-POLISHING(ENDURACRETE)", typeOfStoneType2: "Concrete", serviceName: "Polishing" },
                    { serviceType: "CONCRETE-POLISHING(ENDURACRETE)", typeOfStoneType2: "Terrazzo", serviceName: "Bundle (Honing & Polishing)" },

                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Brick", serviceName: "Bundle Clean & Protect" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Concrete", serviceName: "Hone" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Concrete", serviceName: "Polish" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Flagstone", serviceName: "Bundle Clean & Protect" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Granite", serviceName: "Bundle Clean & Protect" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Limestone", serviceName: "Bundle Clean & Protect" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Marble", serviceName: "Bundle Clean & Protect" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Bundle Clean & Protect" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Mexican Tile", serviceName: "Bundle Clean & Protect" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Quartz", serviceName: "Hone" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Quartz", serviceName: "Polish" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Slate", serviceName: "Hone" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Slate", serviceName: "Polish" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Terrazzo", serviceName: "Hone" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Terrazzo", serviceName: "Polish" },

                    { serviceType: "MetalLIFE", typeOfStoneType2: "Glass", serviceName: "Metal Scratch Removal" },
                    { serviceType: "MetalLIFE", typeOfStoneType2: "Metal", serviceName: "Metal Scratch Removal" },

                    { serviceType: "STONELIFE", typeOfStoneType2: "Brick", serviceName: "Bundle Clean & Protect" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Brick", serviceName: "Bundle Clean & Seal" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Brick", serviceName: "Bundle Matte (Hone-Seal)" },

                    { serviceType: "STONELIFE", typeOfStoneType2: "Flagstone", serviceName: "Bundle Clean & Protect" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Flagstone", serviceName: "Bundle Clean & Seal" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Flagstone", serviceName: "Polishing" },

                    { serviceType: "STONELIFE", typeOfStoneType2: "Glass", serviceName: "Hone" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Glass", serviceName: "Polish" },

                    { serviceType: "STONELIFE", typeOfStoneType2: "Granite", serviceName: "Bundle Clean & Protect" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Granite", serviceName: "Bundle Clean & Seal" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Granite", serviceName: "Bundle Gloss-1 (Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Granite", serviceName: "Bundle Gloss-2 (Hone-Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Granite", serviceName: "Bundle Matte (Hone-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Granite", serviceName: "Bundle Satin-1 (Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Granite", serviceName: "Bundle Satin-2 (Hone-Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Granite", serviceName: "Polishing" },

                    { serviceType: "STONELIFE", typeOfStoneType2: "Limestone", serviceName: "Bundle Clean & Protect" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Limestone", serviceName: "Bundle Clean & Seal" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Limestone", serviceName: "Bundle Gloss-1 (Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Limestone", serviceName: "Bundle Gloss-2 (Hone-Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Limestone", serviceName: "Bundle Matte (Hone-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Limestone", serviceName: "Bundle Satin-1 (Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Limestone", serviceName: "Bundle Satin-2 (Hone-Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Limestone", serviceName: "Polishing" },

                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble", serviceName: "Bundle Clean & Protect" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble", serviceName: "Bundle Clean & Seal" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble", serviceName: "Bundle Gloss-1 (Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble", serviceName: "Bundle Gloss-2 (Hone-Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble", serviceName: "Bundle Matte (Hone-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble", serviceName: "Bundle Satin-1 (Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble", serviceName: "Bundle Satin-2 (Hone-Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble", serviceName: "Polishing" },

                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Bundle Clean & Protect" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Bundle Clean & Seal" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Bundle Gloss-1 (Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Bundle Gloss-2 (Hone-Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Bundle Matte (Hone-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Bundle Satin-1 (Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Bundle Satin-2 (Hone-Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Polishing" },

                    { serviceType: "STONELIFE", typeOfStoneType2: "Mexican Tile", serviceName: "Bundle Clean & Protect" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Mexican Tile", serviceName: "Bundle Clean & Seal" },

                    { serviceType: "STONELIFE", typeOfStoneType2: "Quartz", serviceName: "Hone" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Quartz", serviceName: "Polish" },

                    { serviceType: "STONELIFE", typeOfStoneType2: "Slate", serviceName: "Hone" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Slate", serviceName: "Polish" },

                    { serviceType: "STONELIFE", typeOfStoneType2: "Travertine", serviceName: "Bundle Clean & Protect" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Travertine", serviceName: "Bundle Clean & Seal" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Travertine", serviceName: "Bundle Gloss-1 (Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Travertine", serviceName: "Bundle Gloss-2 (Hone-Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Travertine", serviceName: "Bundle Matte (Hone-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Travertine", serviceName: "Bundle Satin-1 (Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Travertine", serviceName: "Bundle Satin-2 (Hone-Polish-Harden-Seal)" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Travertine", serviceName: "Polishing" },

                    { serviceType: "STONELIFE", typeOfStoneType2: "Vinyl", serviceName: "Hone" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Vinyl", serviceName: "Polish" },

                    { serviceType: "WOODLIFE", typeOfStoneType2: "Wood", serviceName: "Scratch & Wear Removal (Sanding)" }
                ];
                vm.validItemsForEvent = [
                    { serviceType: "CONCRETE-COATINGS", typeOfStoneType2: "Concrete", serviceName: "Concrete Floor Prep - Repairs" },
                    { serviceType: "CONCRETE-COUNTERTOPS", typeOfStoneType2: "Brick", serviceName: "Chip Repair" },
                    { serviceType: "CONCRETE-COUNTERTOPS", typeOfStoneType2: "Concrete", serviceName: "Chip Repair" },
                    { serviceType: "CONCRETE-OVERLAYMENTS", typeOfStoneType2: "Brick", serviceName: "Chip Repair" },
                    { serviceType: "CONCRETE-OVERLAYMENTS", typeOfStoneType2: "Concrete", serviceName: "Chip Repair" },
                    { serviceType: "CONCRETE-REPAIR", typeOfStoneType2: "Concrete", serviceName: "Chip Repair" },
                    { serviceType: "CONCRETE-REPAIR", typeOfStoneType2: "Terrazzo", serviceName: "Chip Repair" },
                    { serviceType: "CONCRETE-POLISHING(ENDURACRETE)", typeOfStoneType2: "Terrazzo", serviceName: "Concrete Spot Stain Removal" },

                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Brick", serviceName: "Chip Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Ceramic (tile and grout)", serviceName: "Chip Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Concrete", serviceName: "Chip Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Engineered Stone:Cesar", serviceName: "Chip Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Engineered Stone:Corian", serviceName: "Chip Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Engineered Stone:Other", serviceName: "Chip Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Engineered Stone:Zodiaq", serviceName: "Chip Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Flagstone", serviceName: "Chip Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Granite", serviceName: "Chip Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Limestone", serviceName: "Chip Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Marble", serviceName: "Chip Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Chip Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Mexican Tile", serviceName: "Chip Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Porcelain", serviceName: "Chip Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Quartz", serviceName: "Chip Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Slate", serviceName: "Chip Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Terrazzo", serviceName: "Chip Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Travertine", serviceName: "Chip Repair" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Brick", serviceName: "Stain Removal" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Brick", serviceName: "Tile Rebond" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Brick", serviceName: "Tile Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Ceramic (tile and grout)", serviceName: "Stain Removal" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Ceramic (tile and grout)", serviceName: "Tile Rebond" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Ceramic (tile and grout)", serviceName: "Tile Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Granite", serviceName: "Stain Removal" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Granite", serviceName: "Tile Rebond" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Granite", serviceName: "Tile Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Limestone", serviceName: "Stain Removal" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Limestone", serviceName: "Tile Rebond" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Limestone", serviceName: "Tile Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Marble", serviceName: "Stain Removal" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Marble", serviceName: "Tile Rebond" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Marble", serviceName: "Tile Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Stain Removal" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Tile Rebond" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Tile Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Mexican Tile", serviceName: "Stain Removal" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Mexican Tile", serviceName: "Tile Rebond" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Mexican Tile", serviceName: "Tile Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Porcelain", serviceName: "Stain Removal" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Porcelain", serviceName: "Tile Rebond" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Porcelain", serviceName: "Tile Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Quartz", serviceName: "Stain Removal" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Quartz", serviceName: "Tile Rebond" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Quartz", serviceName: "Tile Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Slate", serviceName: "Stain Removal" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Slate", serviceName: "Tile Rebond" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Slate", serviceName: "Tile Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Terrazzo", serviceName: "Stain Removal" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Terrazzo", serviceName: "Tile Rebond" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Terrazzo", serviceName: "Tile Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Travertine", serviceName: "Stain Removal" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Travertine", serviceName: "Tile Rebond" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Travertine", serviceName: "Tile Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Vinyl", serviceName: "Stain Removal" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Vinyl", serviceName: "Tile Rebond" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Vinyl", serviceName: "Tile Replacement" },

                    { serviceType: "STONELIFE", typeOfStoneType2: "Brick", serviceName: "Chip Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Flagstone", serviceName: "Chip Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Glass", serviceName: "Chip Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Granite", serviceName: "Chip Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Limestone", serviceName: "Chip Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble", serviceName: "Chip Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Chip Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Mexican Tile", serviceName: "Chip Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Quartz", serviceName: "Chip Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Slate", serviceName: "Chip Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Travertine", serviceName: "Chip Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Vinyl", serviceName: "Chip Repair" },

                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Ceramic (tile and grout)", serviceName: "Tile Installation" },
                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Ceramic (tile and grout)", serviceName: "Tile Removal" },
                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Concrete", serviceName: "Tile Installation" },
                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Concrete", serviceName: "Tile Removal" },

                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Granite", serviceName: "Tile Installation" },
                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Granite", serviceName: "Tile Removal" },
                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Limestone", serviceName: "Tile Installation" },
                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Limestone", serviceName: "Tile Removal" },
                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Marble", serviceName: "Tile Installation" },
                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Marble", serviceName: "Tile Removal" },

                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Porcelain", serviceName: "Tile Installation" },
                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Porcelain", serviceName: "Tile Removal" },
                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Quartz", serviceName: "Tile Installation" },
                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Quartz", serviceName: "Tile Removal" },
                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Slate", serviceName: "Tile Installation" },
                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Slate", serviceName: "Tile Removal" },

                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Terrazzo", serviceName: "Tile Installation" },
                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Terrazzo", serviceName: "Tile Removal" },
                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Travertine", serviceName: "Tile Installation" },
                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Travertine", serviceName: "Tile Removal" },
                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Vinyl", serviceName: "Tile Installation" },
                    { serviceType: "TILEINSTALL", typeOfStoneType2: "Vinyl", serviceName: "Tile Removal" },

                    { serviceType: "VINYLGUARD", typeOfStoneType2: "Vinyl", serviceName: "Chip Repair" }
                ];
                vm.validItemsForLinerFt = [
                    { serviceType: "CONCRETE-COUNTERTOPS", typeOfStoneType2: "Brick", serviceName: "Crack Repair" },
                    { serviceType: "CONCRETE-COUNTERTOPS", typeOfStoneType2: "Concrete", serviceName: "Crack Repair" },

                    { serviceType: "CONCRETE-OVERLAYMENTS", typeOfStoneType2: "Brick", serviceName: "Crack Repair" },
                    { serviceType: "CONCRETE-OVERLAYMENTS", typeOfStoneType2: "Concrete", serviceName: "Crack Repair" },

                    { serviceType: "CONCRETE-REPAIR", typeOfStoneType2: "Concrete", serviceName: "Crack Repair" },
                    { serviceType: "CONCRETE-REPAIR", typeOfStoneType2: "Terrazzo", serviceName: "Crack Repair" },

                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Brick", serviceName: "Crack Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Ceramic (tile and grout)", serviceName: "Crack Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Concrete", serviceName: "Crack Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Engineered Stone:Cesar", serviceName: "Crack Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Engineered Stone:Corian", serviceName: "Crack Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Engineered Stone:Other", serviceName: "Crack Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Engineered Stone:Zodiaq", serviceName: "Crack Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Flagstone", serviceName: "Crack Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Granite", serviceName: "Crack Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Limestone", serviceName: "Crack Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Marble", serviceName: "Crack Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Crack Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Porcelain", serviceName: "Crack Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Quartz", serviceName: "Crack Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Slate", serviceName: "Crack Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Terrazzo", serviceName: "Crack Repair" },
                    { serviceType: "COUNTERLIFE", typeOfStoneType2: "Travertine", serviceName: "Crack Repair" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Brick", serviceName: "Caulking" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Brick", serviceName: "Grout Repair" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Brick", serviceName: "Grout Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Ceramic (tile and grout)", serviceName: "Caulking" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Ceramic (tile and grout)", serviceName: "Grout Repair" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Ceramic (tile and grout)", serviceName: "Grout Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Flagstone", serviceName: "Caulking" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Granite", serviceName: "Caulking" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Granite", serviceName: "Grout Repair" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Granite", serviceName: "Grout Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Limestone", serviceName: "Caulking" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Limestone", serviceName: "Grout Repair" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Limestone", serviceName: "Grout Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Marble", serviceName: "Caulking" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Marble", serviceName: "Grout Repair" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Marble", serviceName: "Grout Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Caulking" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Grout Repair" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Grout Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Mexican Tile", serviceName: "Caulking" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Mexican Tile", serviceName: "Grout Repair" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Mexican Tile", serviceName: "Grout Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Porcelain", serviceName: "Caulking" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Porcelain", serviceName: "Grout Repair" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Porcelain", serviceName: "Grout Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Quartz", serviceName: "Caulking" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Quartz", serviceName: "Grout Repair" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Quartz", serviceName: "Grout Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Slate", serviceName: "Caulking" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Slate", serviceName: "Grout Repair" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Slate", serviceName: "Grout Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Terrazzo", serviceName: "Caulking" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Terrazzo", serviceName: "Grout Repair" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Terrazzo", serviceName: "Grout Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Travertine", serviceName: "Caulking" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Travertine", serviceName: "Grout Repair" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Travertine", serviceName: "Grout Replacement" },

                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Vinyl", serviceName: "Caulking" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Vinyl", serviceName: "Grout Repair" },
                    { serviceType: "GROUTLIFE/TILELOK", typeOfStoneType2: "Vinyl", serviceName: "Grout Replacement" },

                    { serviceType: "STONELIFE", typeOfStoneType2: "Brick", serviceName: "Crack Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Flagstone", serviceName: "Crack Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Granite", serviceName: "Crack Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Limestone", serviceName: "Crack Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble", serviceName: "Crack Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Marble (Tumbled)", serviceName: "Crack Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Mexican Tile", serviceName: "Crack Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Quartz", serviceName: "Crack Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Slate", serviceName: "Crack Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Travertine", serviceName: "Crack Repair" },
                    { serviceType: "STONELIFE", typeOfStoneType2: "Vinyl", serviceName: "Crack Repair" },

                    { serviceType: "VINYLGUARD", typeOfStoneType2: "Vinyl", serviceName: "Crack Repair" }
                ];
                vm.validItemsForProductPrice = [
                    { serviceType: "PRODUCT"}
                ];
                vm.validItemsForTaxRate = [
                    { serviceName: "Products" },
                    { serviceName: "Service" }
                ];
                vm.validItemsForMaintenance = [
                    { serviceType: "MAINTENANCE:BI-MONTHLY" },
                    { serviceType: "MAINTENANCE:MONTHLY" },
                    { serviceType: "MAINTENANCE:OTHER" },
                    { serviceType: "MAINTENANCE:QUARTERLY" }
                ];

                function buildRules(list) {
                    return list.map(function (item) {
                        return {
                            serviceType: item.serviceType || null,
                            serviceName: item.serviceName || null,
                            typeOfStoneType2: item.typeOfStoneType2 || null
                        };
                    });
                }

                // Build lookups
                vm.rulesTime = buildRules(vm.validItemsForTime);
                vm.rulesEvent = buildRules(vm.validItemsForEvent);
                vm.rulesLinerFt = buildRules(vm.validItemsForLinerFt);
                vm.rulesProductPrice = buildRules(vm.validItemsForProductPrice);
                vm.rulesTaxRate = buildRules(vm.validItemsForTaxRate);
                vm.rulesMaintenance = buildRules(vm.validItemsForMaintenance);

                function isValid(rules) {
                    return rules.some(function (rule) {

                        if (rule.serviceType && rule.serviceType !== vm.service.serviceType)
                            return false;

                        if (rule.serviceName && rule.serviceName !== vm.service.serviceName)
                            return false;

                        if (rule.typeOfStoneType2 && rule.typeOfStoneType2 !== vm.service.typeOfStoneType2)
                            return false;

                        return true; // All provided fields matched
                    });
                }

                if (isValid(vm.rulesTime)) {
                    vm.serviceTypeId = 285;
                    vm.service.serviceTypeId = 285;
                }
                else if (isValid(vm.rulesEvent)) {
                    vm.serviceTypeId = 284;
                    vm.service.serviceTypeId = 284;
                }
                else if (isValid(vm.rulesLinerFt)) {
                    vm.serviceTypeId = 283;
                    vm.service.serviceTypeId = 283;
                }
                else if (isValid(vm.rulesProductPrice)) {
                    vm.serviceTypeId = 287;
                    vm.service.serviceTypeId = 287;
                }
                else if (isValid(vm.rulesTaxRate)) {
                    vm.serviceTypeId = 288;
                    vm.service.serviceTypeId = 288;
                }
                else if (isValid(vm.rulesMaintenance)) {
                    vm.serviceTypeId = 286;
                    vm.service.serviceTypeId = 286;
                }
                else {
                    vm.serviceTypeId = 282;
                    vm.service.serviceTypeId = 282;
                }










                if (vm.service.serviceName == 'Concrete Floor Prep - Grind') {
                    vm.isCycleRestorationVisible = true;
                }
                if (vm.service.serviceName == 'Bundle Clean & Seal' || vm.service.serviceName == 'Bundle Gloss-1 (Polish-Harden-Seal)' || vm.service.serviceName == 'Bundle Gloss-2 (Hone-Polish-Harden-Seal)'
                    || vm.service.serviceName == 'Bundle Matte (Hone-Seal)' || vm.service.serviceName == 'Bundle Satin-1 (Polish-Harden-Seal)' || vm.service.serviceName == 'Bundle Satin-2 (Hone-Polish-Harden-Seal)' || vm.service.serviceName == "Bundle Grout Clean & Seal") {
                    vm.isSealing = true;
                }
                if (vm.service.typeOfStoneColor.length <= 0) {
                    vm.service.typeOfStoneColor = "";
                }
                if (vm.service.typeOfStoneType.length <= 0) {
                    vm.service.typeOfStoneType = "";
                }
                if (vm.service.serviceName != null && vm.service.serviceName == 'Colorseal') {
                    vm.isMachineSize = true;
                }
                if (vm.service.serviceName == 'Hardening') {
                    vm.isHardening = true;
                    vm.isHoning = false;
                    vm.isTimeEstimateTab = false;
                    vm.honingMeasurement = {};
                    vm.isCycleRestorationTabDisabled = true;
                    getLocationList(vm.location);
                    getShiftPrice();
                    if (vm.service.honingMeasurement != undefined || (vm.service.honingMeasurement != null)) {
                        vm.honingMeasurement = vm.service.honingMeasurement;
                        vm.honingMeasurementList = $filter('filter')(vm.service.honingMeasurementList, { isSaved: true }, true);
                        getHoningMeasurementHoverValues();
                        calculatingMaintainanceCost();
                        ChangingZeroToNull();
                        getShiftPriceForHoningMeasurement();
                    }
                    else {
                        vm.honingMeasurement = {};
                        if (vm.service.honingMeasurement != null) {
                            vm.honingMeasurement = vm.service.honingMeasurement;
                        }
                        getLocationList(vm.location);
                        getShiftPrice();
                        changingDiv();
                        getHoiningMeasurementLoad();
                        ChangingZeroToNull();
                    }
                }
                else {
                    vm.isHardening = false;
                    if (vm.service.honingMeasurement != undefined || (vm.service.honingMeasurement != null)) {
                        if (!getHoiningMeasurementCheck()) {
                            vm.isHoning = false;
                            vm.calculateSumTotalArea();
                        }
                        else {
                            vm.isHoning = true;
                            vm.isTimeEstimateTab = true;
                            vm.honingMeasurement = vm.service.honingMeasurement;
                            vm.honingMeasurementList = $filter('filter')(vm.service.honingMeasurementList, { isSaved: true }, true);
                            if (vm.honingMeasurement.area > 0) {
                                vm.isCycleRestorationTabDisabled = false;
                            }
                            else {
                                vm.isCycleRestorationTabDisabled = true;
                            }
                            getHoiningMeasurementOriginal();
                            getHoningMeasurementHoverValues();
                            calculatingMaintainanceCost();
                            ChangingNullToZeroClearance();
                            clearanceValue();
                            ChangingZeroToNull();
                            ChangingZeroToNullForOrginal();
                            getShiftPriceForHoningMeasurement();
                            getHoiningMeasurementLoadCompare();
                        }
                    }
                    else {
                        vm.honingMeasurement = {};
                        if (getHoiningMeasurementCheck()) {
                            if (vm.service.honingMeasurement != null) {
                                vm.honingMeasurement = vm.service.honingMeasurement;
                            }
                            vm.isHoning = true;
                            vm.isTimeEstimateTab = true;
                            vm.isCycleRestorationTabDisabled = true;
                            getLocationList(vm.location);
                            getShiftPrice();
                            changingDiv();
                            getHoiningMeasurementLoad();
                            clearanceValue();
                            getHoiningMeasurementOriginal();
                            ChangingZeroToNull();
                            ChangingZeroToNullForOrginal();
                            getHoiningMeasurementLoadCompare();
                        }
                        else {
                            vm.isHoning = false;
                            vm.isTimeEstimateTab = false;
                            vm.honingMeasurement = {};
                            vm.isCycleRestorationTabDisabled = true;
                            getLocationList(vm.location);
                            getShiftPrice();
                        }
                    }
                }
                vm.dimension = [{ display: "0.22", value: "0.22" }, { display: "1.00", value: "1.00" },
                    { display: "1.38", value: "1.38" }, { display: "1.67", value: "1.67" }, 
                    { display: "3.54", value: "3.54" }, { display: "10.63", value: "10.63" }];
                if (vm.ServiceName == "Honing" || vm.ServiceName == "Polishing" || vm.service.serviceType == "MAINTAINCE:BI-MONTHLY" || vm.service.serviceType == "MAINTAINCE:Monthly" || vm.service.serviceType == "MAINTAINCE:(Other)") {
                    changingDiv();
                }
                vm.dimensionForColorseal = [{ display: "1", value: "1" }, { display: "2", value: "2" }, { display: "4", value: "4" }, { display: "6", value: "6" },
                                            { display: "8", value: "8" }, { display: "10", value: "10" }, { display: "12", value: "12" }, { display: "14", value: "14" },
                                            { display: "16", value: "16" }, { display: "18", value: "18" }, { display: "20", value: "20" }, { display: "22", value: "22" },
                                            { display: "24", value: "24" }]
                vm.close = function () {
                    vm.service.sumTotalArea = vm.sumOfAreaOriginal;
                    vm.honingMeasurementList;
                    angular.forEach(vm.honingMeasurementList, function (value) {
                        value.area = vm.sumOfAreaOriginal;
                        calculateTimeCalcuation(value);
                    });
                    $uibModalInstance.dismiss();
                };
                if (vm.service.measurements == null && vm.service.measurements == undefined) {
                    vm.service.measurements = [];
                    vm.service.measurements.push({
                        length: null,
                        width: null,
                        area: null,
                        dimension: '12',
                        description: '',
                        unitType: vm.service.unitType,
                        unitTypeId: vm.service.unitTypeId,
                        isSaved: false
                    });
                }
                else {
                    //vm.service.measurements = $filter('filter')(vm.service.measurements, { isSaved: true }, true);
                    if (vm.service.measurements.length == 0) {
                        vm.service.measurements.push({
                            length: null,
                            width: null,
                            area: null,
                            dimension: '12',
                            description: '',
                            unitType: vm.service.unitType,
                            unitTypeId: vm.service.unitTypeId,
                            isSaved: false
                        });
                    }
                    else {
                        angular.forEach(vm.service.measurements, function (value, index) {
                            if (value.length == 0) {
                                value.length = null;
                            }
                            if (value.width == 0) {
                                value.width = null;
                            }
                        })
                    }
                }
                if (vm.classType != null) {
                    getmarketingClassCollection();
                }
                vm.addMore = function () {
                    vm.service.measurements.push({
                        length: null,
                        width: null,
                        area: null,
                        dimension: '12',
                        description: '',
                        isSaved: false
                    });
                };
                vm.remove = function (index) {
                    if (vm.service.measurements.length == 1) {
                        vm.service.measurements.splice(index, 1);
                    }
                    if (vm.service.measurements.length == 2) {
                        vm.service.measurements.splice(1, 1);
                    }
                    if (vm.service.measurements.length > 2) {
                        vm.service.measurements.splice(index, 1);
                    }
                    if (vm.service.measurements.length == 0)
                        vm.service.measurements.push({});
                    vm.calculateSumTotalArea();
                };
                vm.saveMeasurements = function () {
                    vm.service.sumTotalArea = 0;
                    vm.service.option1 = 0;
                    var countForNullArea = 0;
                    if (!vm.isHoning && !vm.isHardening) {
                        vm.calculateSumTotalArea();
                        vm.getOption1Value(vm.service);
                        var count = 0;
                        if ((vm.serviceTypeId == vm.serviceType.AREA) || (vm.serviceTypeId == vm.serviceType.EVENT) || (vm.serviceTypeId == vm.serviceType.LINEARFT)) {
                            angular.forEach(vm.service.measurements, function (value, index) {
                                value.length = 0;
                                value.width = 0;
                            })
                        }
                        else if (vm.serviceTypeId == vm.serviceType.EVENT) {
                            angular.forEach(vm.service.measurements, function (value, index) {
                                value.length = 0;
                                value.width = 0;
                                value.area = 0;
                            })
                        }
                        else if (vm.serviceTypeId == vm.serviceType.MAINTAINANCE) {
                            angular.forEach(vm.service.measurements, function (value, index) {
                                value.length = 0;
                                value.width = 0;
                            })
                        }
                        else if (vm.serviceTypeId == vm.serviceType.PRODUCTPRICE) {
                            angular.forEach(vm.service.measurements, function (value, index) {
                                value.length = 0;
                                value.width = 0;
                            })
                        }
                        else {
                            angular.forEach(vm.service.measurements, function (value, index) {
                                value.length = 0;
                                value.width = 0;
                                value.setPrice = vm.honingMeasurement.shiftPrice;
                            })
                        }
                        angular.forEach(vm.service.measurements, function (value, index) {
                            if (vm.serviceTypeId == vm.serviceType.AREA) {
                                if (value.setPrice == '' || value.setPrice == null || value.setPrice == undefined) {
                                    toaster.error("Area is not filled!");
                                    count = count + 1;
                                    return;
                                }
                            }
                            else if (vm.serviceTypeId == vm.serviceType.MAINTAINANCE) {
                                if (value.areaTime == '' || value.areaTime == null || value.areaTime == undefined) {
                                    toaster.error("Area is not filled!");
                                    count = count + 1;
                                    return;
                                }
                            }
                            else if (vm.serviceTypeId == vm.serviceType.LINEARFT) {
                                if (value.area == '' || value.area == null) {
                                    toaster.error("Linear Feet  is not filled!");
                                    count = count + 1;
                                    return;
                                }
                            }
                            else if (vm.serviceTypeId == vm.serviceType.PRODUCTPRICE) {
                                if (value.area == '' || value.area == null) {
                                    toaster.error("Linear Feet  is not filled!");
                                    count = count + 1;
                                    return;
                                }
                            }
                            else if (vm.serviceTypeId == vm.serviceType.EVENT) {
                                if (value.setPrice == null || value.setPrice == '' || value.setPrice == undefined) {
                                    toaster.error("Number of Unit is not filled!");
                                    count = count + 1;
                                    return;
                                }
                            }
                            else {
                                if (value.setPrice == '' && value.setPrice == null && value.setPrice != undefined) {
                                    toaster.error("Area Price  is not filled!");
                                    toaster.error("Shift Price  is not filled!");
                                    count = count + 1;
                                    return;
                                }
                            }
                        });
                        if (count == 0) {
                            $uibModalInstance.dismiss();
                        }
                    }
                    else {
                        angular.forEach(vm.honingMeasurementList, function (value, index) {
                            if (value.area == null || value.area == '') {
                                toaster.error("Area is not filled!");
                                countForNullArea += 1;
                                return;
                            }
                            else {
                                value.isSaved = true;
                            }
                            if (value.shiftPrice == null || value.shiftPrice == '' || value.shiftPrice == 0) {
                                toaster.error("Shift Price is not filled!");
                                return;
                            }

                            if (vm.cycleRestorationPlan.startingPointTechShiftEstimates == undefined) {
                                value.startingPointTechShiftEstimates = Math.round(value.averageNightRequired * 1.2);
                            }
                            else if (value.startingPointTechShiftEstimates == undefined) {
                                value.startingPointTechShiftEstimates = vm.cycleRestorationPlan.startingPointTechShiftEstimates;
                            }
                            else {
                                value.startingPointTechShiftEstimates = value.startingPointTechShiftEstimates;
                            }
                            vm.service.sumTotalArea += value.area;
                        });
                        if (countForNullArea == 0) {
                            if (vm.isHoning || vm.isHardening) {
                                vm.service.honingMeasurement = vm.honingMeasurement;
                                vm.service.honingMeasurementList = vm.honingMeasurementList;
                                getTotalProjectedValue();
                                vm.estimateInvoiceInfo.marketingClass = vm.classType;
                                angular.forEach(vm.service.measurements, function (value, index) {
                                    value.length = 0;
                                    value.width = 0;
                                    value.setPrice = vm.honingMeasurement.shiftPrice;
                                })
                            }
                            else {
                            }
                            ChangingNullToZero();
                            $uibModalInstance.dismiss();
                            return;
                            toaster.show("Measurements have been added Successfully!!");
                        }
                    };
                }

                vm.totalArea = function (index) {
                    var measurement = vm.service.measurements[index];
                    if (vm.serviceTypeId == vm.serviceType.AREA) {
                        if (measurement.length != null && measurement.width != null && measurement.length != undefined && measurement.width != undefined && measurement.length != '' && measurement.width != '') {
                            measurement.area = measurement.length * measurement.width;
                            measurement.setPrice = measurement.length * measurement.width;
                        }
                    }
                    else {
                        if (measurement.setPrice != null) {
                            measurement.area = measurement.setPrice;
                        }
                    }
                    vm.calculateSumTotalArea();
                }

                vm.calculateSumTotalArea = function () {
                    vm.sumTotalArea = 0;
                    vm.totalUnit = 0;
                    if (vm.honingMeasurementList.length > 0) {
                        angular.forEach(vm.honingMeasurementList, function (value, index) {
                            vm.totalUnit += value.setPrice;
                            if (vm.serviceTypeId == vm.serviceType.AREA) {
                                if (value.area != '' || value.area != null) {
                                    vm.sumTotalArea += value.setPrice;
                                    value.area = value.setPrice;
                                }
                            }
                            else {
                                if (value.setPrice != null) {
                                    vm.sumTotalArea += value.setPrice;
                                    value.area = value.setPrice;
                                }
                                else if (vm.serviceTypeId == vm.serviceType.TIME || (vm.serviceTypeId == vm.serviceType.MAINTAINANCE)) {
                                    vm.sumTotalArea += value.areaTime;
                                    value.area = value.areaTime;
                                }
                                else if ((vm.serviceTypeId == vm.serviceType.LINEARFT)) {
                                    vm.sumTotalArea += value.area;
                                    value.area = value.area;
                                }
                            }
                        });
                    }
                    if (vm.service.measurements.length > 0) {
                        angular.forEach(vm.service.measurements, function (value, index) {
                            if (vm.serviceTypeId == vm.serviceType.MAINTAINANCE) {
                                vm.totalUnit += value.setPrice;
                                vm.sumTotalArea += value.areaTime;
                                value.area = value.areaTime;
                            }
                            else if (vm.serviceTypeId == vm.serviceType.AREA) {
                                vm.totalUnit += value.setPrice;
                                vm.sumTotalArea += value.setPrice;
                                value.area = value.setPrice;
                            }
                            else if (vm.serviceTypeId == vm.serviceType.EVENT) {
                                vm.totalUnit += value.setPrice;
                                vm.sumTotalArea += value.areaTime;
                                value.area = value.areaTime;
                            }
                            else if (vm.serviceTypeId == vm.serviceType.LINEARFT) {
                                vm.totalUnit += value.area;
                                vm.sumTotalArea += value.area;
                            }
                            else if (vm.serviceTypeId == vm.serviceType.PRODUCTPRICE) {
                                vm.totalUnit += value.setPrice;
                                vm.sumTotalArea += value.setPrice;
                                value.area = value.setPrice;
                            }
                            else if (vm.serviceTypeId == vm.serviceType.TAXRATE) {

                            }
                        });
                    }
                    vm.service.sumTotalArea = vm.sumTotalArea;
                    if (isNaN(vm.service.sumTotalArea)) {
                        vm.service.sumTotalArea = 0;
                    }
                }

                function getOption1Value(service) {
                    vm.area = service.sumTotalArea;
                    vm.serviceTypess = vm.priceServiceList;
                    vm.franchiseePrice = 0;
                    vm.franchiseeAdditionalPrice = 0;
                    if (isNaN(vm.totalUnit)) {
                        vm.totalUnit = vm.area;
                    }
                    if (service.serviceTypeId == vm.serviceTypeService.AREA) {
                        vm.serviceTypessLocal = $filter('filter')(vm.priceServiceList, { category: "AREA" }, true);
                        vm.serviceTypessLocal = $filter('filter')(vm.serviceTypessLocal, { materialType: service.typeOfStoneType2, serviceType: service.serviceType }, true);
                        if (vm.ServiceName == "Colorseal") {

                            //vm.serviceTypessLocal = vm.serviceTypessLocal.filter(function (item) {
                            //    return service.measurements.some(function (m) {
                            //        return item.service === vm.ServiceName + " " + m.dimension;
                            //    });
                            //});
                            var result = [];
                            service.measurements.forEach(function (m) {
                                var match = null;
                                for (var i = 0; i < vm.serviceTypessLocal.length; i++) {
                                    if (vm.serviceTypessLocal[i].service === vm.ServiceName + " " + m.dimension) {
                                        match = vm.serviceTypessLocal[i];
                                        break;
                                    }
                                }
                                if (match) {
                                    result.push(match);
                                }
                            });

                            vm.serviceTypessLocal = result;

                            if (vm.serviceTypessLocal.length > 0) {

                                vm.franchiseePrice = 0;
                                vm.franchiseeAdditionalPrice = 0;

                                var aliasText = "";
                                var finalPrice = 0;
                                vm.serviceTypessLocal.forEach(function (item, index) {
                                    var localArea = service.measurements[index];
                                    vm.franchiseePrice += item.franchiseeCorporatePrice;
                                    vm.franchiseeAdditionalPrice += item.franchiseeAdditionalCorporatePrice;

                                    var rowPrice =
                                        item.franchiseeCorporatePrice +
                                        (item.franchiseeAdditionalCorporatePrice * (localArea.area - 1));
                                    finalPrice += rowPrice;

                                    var rowAlias =
                                        "Unit Price + (Franchisee Additional Price * (Total Area - 1)) = " +
                                        item.franchiseeCorporatePrice + " + (" +
                                        item.franchiseeAdditionalCorporatePrice + " * (" +
                                        localArea.area + " - 1))";

                                    aliasText +=
                                        "Row " + (index + 1) + ": " +
                                        rowAlias + " = " + rowPrice.toFixed(2) + "\n";
                                });

                                service.option1 = finalPrice.toFixed(2);

                                service.alias = aliasText;
                                service.isAlias = true;
                            }
                        }
                        else {
                            vm.serviceTypessLocal = $filter('filter')(vm.serviceTypessLocal, { service: vm.ServiceName }, true);
                            if (vm.serviceTypessLocal.length > 0) {
                                vm.franchiseePrice += vm.serviceTypessLocal[0].franchiseeCorporatePrice;
                                vm.franchiseeAdditionalPrice += vm.serviceTypessLocal[0].franchiseeAdditionalCorporatePrice;
                                var price = ((vm.franchiseePrice) + (vm.franchiseeAdditionalPrice * (vm.totalUnit - 1)));
                                service.option1 = price.toFixed(2);
                                service.alias = ('Unit Price +(Franchisee Additional Price * (Total Area-1)) = ') + ((vm.franchiseePrice) + '+' + (vm.franchiseeAdditionalPrice + '*' + '(' + (vm.totalUnit + '-' + 1) + ')'));
                                service.isAlias = true;
                            }
                        }
                    }
                    else if (service.serviceTypeId == vm.serviceTypeService.MAINTAINANCE) {
                        service.option1 = parseFloat(
                            vm.sumTotalArea /
                            (
                                (vm.maintenanceChargesListCurrent[0].low + vm.maintenanceChargesListCurrent[0].high) / 2
                            ) *
                            vm.franchiseeShiftPrice.maintainanceTechNightShiftPrice
                        ).toFixed(2);
                        service.alias = ('Total Area / Avg(Max & Min) * Maintenance Tech Night-Shift Price = ') + ((vm.sumTotalArea) + ' / ' + '(' + (vm.maintenanceChargesListCurrent[0].low) + '+' + (vm.maintenanceChargesListCurrent[0].high) + ') / 2 * ' + (vm.franchiseeShiftPrice.maintainanceTechNightShiftPrice));
                        service.isAlias = true;
                    }
                    else if (service.serviceTypeId == vm.serviceTypeService.LINEARFT) {
                        vm.serviceTypessLocal = $filter('filter')(vm.priceServiceList, { category: "LINEAR FT" }, true);
                        vm.serviceTypessLocal = $filter('filter')(vm.serviceTypessLocal, { materialType: service.typeOfStoneType2, serviceType: service.serviceType }, true);
                        vm.serviceTypessLocal = $filter('filter')(vm.serviceTypessLocal, { service: vm.ServiceName }, true);
                        if (vm.serviceTypessLocal.length > 0) {
                            vm.franchiseePrice += vm.serviceTypessLocal[0].franchiseeCorporatePrice;
                            vm.franchiseeAdditionalPrice += vm.serviceTypessLocal[0].franchiseeAdditionalCorporatePrice;
                            var price = ((vm.franchiseePrice) + (vm.franchiseeAdditionalPrice * (vm.totalUnit - 1)));
                            service.option1 = price.toFixed(2);
                            service.alias = ('Unit Price +(Franchisee Additional Price * (Total LINEAR FT-1)) = ') + ((vm.franchiseePrice) + '+' + (vm.franchiseeAdditionalPrice + '*' + '(' + (vm.totalUnit + '-' + 1) + ')'));
                            service.isAlias = true;
                        }
                    }
                    else if (service.serviceTypeId == vm.serviceTypeService.PRODUCTPRICE) {
                        //vm.totalUnit = vm.service.measurements[0].setPrice;
                        vm.serviceTypessLocal = $filter('filter')(vm.priceServiceList, { category: "PRODUCT PRICE" }, true);
                        vm.serviceTypessLocal = $filter('filter')(vm.serviceTypessLocal, { serviceType: service.serviceType }, true);
                        vm.serviceTypessLocal = $filter('filter')(vm.serviceTypessLocal, { service: vm.ServiceName }, true);
                        if (vm.serviceTypessLocal.length > 0) {
                            vm.franchiseePrice += vm.serviceTypessLocal[0].franchiseeCorporatePrice;
                            vm.franchiseeAdditionalPrice += vm.serviceTypessLocal[0].franchiseeAdditionalCorporatePrice;
                            var price = ((vm.franchiseePrice) * (vm.totalUnit));
                            service.option1 = price.toFixed(2);;
                            service.alias = ('Product Price * (Total No of Units) = ') + ((vm.franchiseePrice) + '*' + (vm.totalUnit));
                            service.isAlias = true;
                        }
                    }
                    else if (service.serviceTypeId == vm.serviceTypeService.EVENT) {
                        vm.serviceTypessLocal = $filter('filter')(vm.priceServiceList, { category: "EVENT" }, true);
                        vm.serviceTypessLocal = $filter('filter')(vm.serviceTypessLocal, { materialType: service.typeOfStoneType2, serviceType: service.serviceType }, true);
                        vm.serviceTypessLocal = $filter('filter')(vm.serviceTypessLocal, { service: vm.ServiceName }, true);
                        if (vm.serviceTypessLocal.length > 0) {
                            vm.franchiseePrice += vm.serviceTypessLocal[0].franchiseeCorporatePrice;
                            vm.franchiseeAdditionalPrice += vm.serviceTypessLocal[0].franchiseeAdditionalCorporatePrice;
                            var price = ((vm.franchiseePrice) + (vm.franchiseeAdditionalPrice * (vm.totalUnit - 1)));
                            service.option1 = price.toFixed(2);
                            service.alias = ('Unit Price +(Franchisee Additional Price * (Total Unit-1)) = ') + ((vm.franchiseePrice) + '+' + (vm.franchiseeAdditionalPrice + '*' + '(' + (vm.totalUnit + '-' + 1) + ')'));
                            service.isAlias = true;
                        }
                    }
                }

                function getHoiningMeasurementLoad() {
                    if (vm.isHoning) {
                        if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && vm.service.typeOfStoneType == "Ultimate Finish" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ugc = 20;
                            vm.honingMeasurement.thirty = 10;
                            vm.honingMeasurement.fifty = 5;
                            vm.honingMeasurement.hundred = 5;
                            vm.honingMeasurement.twoHundred = 5;
                            vm.honingMeasurement.fourHundred = 5;
                            vm.honingMeasurement.eightHundred = 5;
                            vm.honingMeasurement.fifteenHundred = 0;
                            vm.honingMeasurement.threeThousand = 0;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 4;
                            vm.honingMeasurement.ihg = 2;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ugc + vm.honingMeasurement.thirty + vm.honingMeasurement.fifty + vm.honingMeasurement.hundred + vm.honingMeasurement.twoHundred +
                                vm.honingMeasurement.fourHundred + vm.honingMeasurement.eightHundred + vm.honingMeasurement.fifteenHundred + vm.honingMeasurement.threeThousand + vm.honingMeasurement.eightThousand + vm.honingMeasurement.elevenThousand + vm.honingMeasurement.caco + vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinute = "65%";
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && vm.service.typeOfStoneType == "Gloss" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 5;
                            vm.honingMeasurement.fourHundred = 5;
                            vm.honingMeasurement.eightHundred = 5;
                            vm.honingMeasurement.fifteenHundred = 0;
                            vm.honingMeasurement.threeThousand = 0;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 4;
                            vm.honingMeasurement.ihg = 2;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ugc + vm.honingMeasurement.thirty + vm.honingMeasurement.fifty + vm.honingMeasurement.hundred + vm.honingMeasurement.twoHundred +
                                vm.honingMeasurement.fourHundred + vm.honingMeasurement.eightHundred + vm.honingMeasurement.fifteenHundred + vm.honingMeasurement.threeThousand + vm.honingMeasurement.eightThousand + vm.honingMeasurement.elevenThousand + vm.honingMeasurement.caco + vm.honingMeasurement.ihg;

                            vm.honingMeasurement.totalMinute = "65%";
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneType == "Satin" || vm.service.typeOfStoneType == "Semi-Gloss") && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 5;
                            vm.honingMeasurement.fourHundred = 5;
                            vm.honingMeasurement.eightHundred = 5;
                            vm.honingMeasurement.fifteenHundred = 0;
                            vm.honingMeasurement.threeThousand = 0;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 0;
                            vm.honingMeasurement.ihg = 2;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ugc + vm.honingMeasurement.thirty + vm.honingMeasurement.fifty + vm.honingMeasurement.hundred + vm.honingMeasurement.twoHundred +
                                vm.honingMeasurement.fourHundred + vm.honingMeasurement.eightHundred + vm.honingMeasurement.fifteenHundred + vm.honingMeasurement.threeThousand + vm.honingMeasurement.eightThousand + vm.honingMeasurement.elevenThousand + vm.honingMeasurement.caco + vm.honingMeasurement.ihg;

                            vm.honingMeasurement.totalMinute = "65%";
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneType == "Matte") && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 5;
                            vm.honingMeasurement.fourHundred = 5;
                            vm.honingMeasurement.eightHundred = 5;
                            vm.honingMeasurement.fifteenHundred = 0;
                            vm.honingMeasurement.threeThousand = 0;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 0;
                            vm.honingMeasurement.ihg = 0;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ugc + vm.honingMeasurement.thirty + vm.honingMeasurement.fifty + vm.honingMeasurement.hundred + vm.honingMeasurement.twoHundred +
                                vm.honingMeasurement.fourHundred + vm.honingMeasurement.eightHundred + vm.honingMeasurement.fifteenHundred + vm.honingMeasurement.threeThousand + vm.honingMeasurement.eightThousand + vm.honingMeasurement.elevenThousand + vm.honingMeasurement.caco + vm.honingMeasurement.ihg;

                            vm.honingMeasurement.totalMinute = "65%";
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 0;
                            vm.honingMeasurement.fourHundred = 0;
                            vm.honingMeasurement.eightHundred = 20;
                            vm.honingMeasurement.fifteenHundred = 10;
                            vm.honingMeasurement.threeThousand = 10;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 10;
                            vm.honingMeasurement.ihg = 2;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ugc + vm.honingMeasurement.thirty + vm.honingMeasurement.fifty + vm.honingMeasurement.hundred + vm.honingMeasurement.twoHundred +
                                vm.honingMeasurement.fourHundred + vm.honingMeasurement.eightHundred + vm.honingMeasurement.fifteenHundred + vm.honingMeasurement.threeThousand + vm.honingMeasurement.eightThousand + vm.honingMeasurement.elevenThousand + vm.honingMeasurement.caco + vm.honingMeasurement.ihg;

                            vm.honingMeasurement.totalMinute = "65%";
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.serviceName == "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 4;
                            vm.honingMeasurement.fourHundred = 0;
                            vm.honingMeasurement.eightHundred = 0;
                            vm.honingMeasurement.fifteenHundred = 0;
                            vm.honingMeasurement.threeThousand = 0;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 0;
                            vm.honingMeasurement.ihg = 0;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ugc + vm.honingMeasurement.thirty + vm.honingMeasurement.fifty + vm.honingMeasurement.hundred + vm.honingMeasurement.twoHundred +
                                vm.honingMeasurement.fourHundred + vm.honingMeasurement.eightHundred + vm.honingMeasurement.fifteenHundred + vm.honingMeasurement.threeThousand + vm.honingMeasurement.eightThousand + vm.honingMeasurement.elevenThousand + vm.honingMeasurement.caco + vm.honingMeasurement.ihg;

                            vm.honingMeasurement.totalMinute = "65%";
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if (vm.service.typeOfStoneType2 == "Concrete" && vm.service.typeOfSurface == "Floor" && vm.service.serviceName != "Concrete Floor Prep - Grind") {
                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 4;
                            vm.honingMeasurement.fifty = 4;
                            vm.honingMeasurement.hundred = 4;
                            vm.honingMeasurement.twoHundred = 4;
                            vm.honingMeasurement.fourHundred = 4;
                            vm.honingMeasurement.eightHundred = 4;
                            vm.honingMeasurement.fifteenHundred = 4;
                            vm.honingMeasurement.threeThousand = 4;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 0;
                            vm.honingMeasurement.ihg = 2;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ugc + vm.honingMeasurement.thirty + vm.honingMeasurement.fifty + vm.honingMeasurement.hundred + vm.honingMeasurement.twoHundred +
                                vm.honingMeasurement.fourHundred + vm.honingMeasurement.eightHundred + vm.honingMeasurement.fifteenHundred + vm.honingMeasurement.threeThousand + vm.honingMeasurement.eightThousand + vm.honingMeasurement.elevenThousand + vm.honingMeasurement.caco + vm.honingMeasurement.ihg;

                            vm.honingMeasurement.totalMinute = "65%";
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if ((vm.service.serviceType != "ENDURACRETE" && vm.service.serviceType != "CONCRETE-STAIN") && (vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 5;
                            vm.honingMeasurement.fourHundred = 5;
                            vm.honingMeasurement.eightHundred = 5;
                            vm.honingMeasurement.fifteenHundred = 0;
                            vm.honingMeasurement.threeThousand = 0;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 4;
                            vm.honingMeasurement.ihg = 2;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 5;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ugc + vm.honingMeasurement.thirty + vm.honingMeasurement.fifty + vm.honingMeasurement.hundred + vm.honingMeasurement.twoHundred +
                                vm.honingMeasurement.fourHundred + vm.honingMeasurement.eightHundred + vm.honingMeasurement.fifteenHundred + vm.honingMeasurement.threeThousand + vm.honingMeasurement.eightThousand + vm.honingMeasurement.elevenThousand + vm.honingMeasurement.caco + vm.honingMeasurement.ihg;

                            vm.honingMeasurement.totalMinuteValue = 50;
                            vm.honingMeasurement.sectionsDivision = 5;
                            vm.honingMeasurement.totalMinute = "50%";
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 50) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = true;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 0;
                            vm.honingMeasurement.fourHundred = 20;
                            vm.honingMeasurement.eightHundred = 10;
                            vm.honingMeasurement.fifteenHundred = 10;
                            vm.honingMeasurement.threeThousand = 0;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 10;
                            vm.honingMeasurement.ihg = 2;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 5;
                            vm.honingMeasurement.totalMinuteValue = 50;
                            vm.honingMeasurement.sectionsDivision = 5;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ugc + vm.honingMeasurement.thirty + vm.honingMeasurement.fifty + vm.honingMeasurement.hundred + vm.honingMeasurement.twoHundred +
                                vm.honingMeasurement.fourHundred + vm.honingMeasurement.eightHundred + vm.honingMeasurement.fifteenHundred + vm.honingMeasurement.threeThousand + vm.honingMeasurement.eightThousand + vm.honingMeasurement.elevenThousand + vm.honingMeasurement.caco + vm.honingMeasurement.ihg;

                            vm.honingMeasurement.totalMinute = "50%";
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 50) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = true;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 4;
                            vm.honingMeasurement.fifty = 4;
                            vm.honingMeasurement.hundred = 4;
                            vm.honingMeasurement.twoHundred = 4;
                            vm.honingMeasurement.fourHundred = 4;
                            vm.honingMeasurement.eightHundred = 4;
                            vm.honingMeasurement.fifteenHundred = 4;
                            vm.honingMeasurement.threeThousand = 4;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 0;
                            vm.honingMeasurement.ihg = 2;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 5;
                            vm.honingMeasurement.totalMinuteValue = 50;
                            vm.honingMeasurement.sectionsDivision = 5;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ugc + vm.honingMeasurement.thirty + vm.honingMeasurement.fifty + vm.honingMeasurement.hundred + vm.honingMeasurement.twoHundred +
                                vm.honingMeasurement.fourHundred + vm.honingMeasurement.eightHundred + vm.honingMeasurement.fifteenHundred + vm.honingMeasurement.threeThousand + vm.honingMeasurement.eightThousand + vm.honingMeasurement.elevenThousand
                                + vm.honingMeasurement.caco + vm.honingMeasurement.ihg;

                            vm.honingMeasurement.totalMinute = "50%";
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 50) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = true;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }

                        else if ((vm.service.serviceType == "ENDURACRETE" || vm.service.serviceType == "CONCRETE-STAIN") && vm.service.serviceName == "Bundle (Honing & Polishing)") {
                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 4;
                            vm.honingMeasurement.fifty = 4;
                            vm.honingMeasurement.hundred = 4;
                            vm.honingMeasurement.twoHundred = 4;
                            vm.honingMeasurement.fourHundred = 4;
                            vm.honingMeasurement.eightHundred = 4;
                            vm.honingMeasurement.fifteenHundred = 4;
                            vm.honingMeasurement.threeThousand = 4;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 0;
                            vm.honingMeasurement.ihg = 2;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ugc + vm.honingMeasurement.thirty + vm.honingMeasurement.fifty + vm.honingMeasurement.hundred + vm.honingMeasurement.twoHundred +
                                vm.honingMeasurement.fourHundred + vm.honingMeasurement.eightHundred + vm.honingMeasurement.fifteenHundred + vm.honingMeasurement.threeThousand + vm.honingMeasurement.eightThousand + vm.honingMeasurement.elevenThousand + vm.honingMeasurement.caco + vm.honingMeasurement.ihg;

                            vm.honingMeasurement.totalMinute = "65%";
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if (vm.service.serviceName == "Hone" && vm.service.typeOfStoneType2 == "Terrazzo") {
                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 5;
                            vm.honingMeasurement.fourHundred = 5;
                            vm.honingMeasurement.eightHundred = 5;
                            vm.honingMeasurement.fifteenHundred = 0;
                            vm.honingMeasurement.threeThousand = 0;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 4;
                            vm.honingMeasurement.ihg = 2;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ugc + vm.honingMeasurement.thirty + vm.honingMeasurement.fifty + vm.honingMeasurement.hundred + vm.honingMeasurement.twoHundred +
                                vm.honingMeasurement.fourHundred + vm.honingMeasurement.eightHundred + vm.honingMeasurement.fifteenHundred + vm.honingMeasurement.threeThousand + vm.honingMeasurement.eightThousand + vm.honingMeasurement.elevenThousand + vm.honingMeasurement.caco + vm.honingMeasurement.ihg;

                            vm.honingMeasurement.totalMinute = "65%";
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.serviceName == "Hone" && (vm.service.typeOfStoneType2 == "Carpet" || vm.service.typeOfStoneType2 == "Ceramic" ||
                            vm.service.typeOfStoneType2 == "Concrete" || vm.service.typeOfStoneType2 == "Corian" || vm.service.typeOfStoneType2 == "Glass" ||
                            vm.service.typeOfStoneType2 == "Limestone" || vm.service.typeOfStoneType2 == "Metal" || vm.service.typeOfStoneType2 == "Porcelain" ||
                            vm.service.typeOfStoneType2 == "Quartz" || vm.service.typeOfStoneType2 == "Slate" || vm.service.typeOfStoneType2 == "Vinyl" ||
                            vm.service.typeOfStoneType2 == "Wood")) {
                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 5;
                            vm.honingMeasurement.fourHundred = 5;
                            vm.honingMeasurement.eightHundred = 5;
                            vm.honingMeasurement.fifteenHundred = 0;
                            vm.honingMeasurement.threeThousand = 0;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 0;
                            vm.honingMeasurement.ihg = 0;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ugc + vm.honingMeasurement.thirty + vm.honingMeasurement.fifty + vm.honingMeasurement.hundred + vm.honingMeasurement.twoHundred + vm.honingMeasurement.fourHundred + vm.honingMeasurement.eightHundred + vm.honingMeasurement.fifteenHundred + vm.honingMeasurement.threeThousand + vm.honingMeasurement.eightThousand + vm.honingMeasurement.elevenThousand + vm.honingMeasurement.caco + vm.honingMeasurement.ihg;

                            vm.honingMeasurement.totalMinute = "65%";
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        if (vm.service.serviceName == "Honing" && vm.service.typeOfStoneType2 == "Granite") {
                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 0;
                            vm.honingMeasurement.fourHundred = 0;
                            vm.honingMeasurement.eightHundred = 20;
                            vm.honingMeasurement.fifteenHundred = 10;
                            vm.honingMeasurement.threeThousand = 10;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 0;
                            vm.honingMeasurement.ihg = 0;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ugc + vm.honingMeasurement.thirty + vm.honingMeasurement.fifty + vm.honingMeasurement.hundred + vm.honingMeasurement.twoHundred + vm.honingMeasurement.fourHundred + vm.honingMeasurement.eightHundred + vm.honingMeasurement.fifteenHundred + vm.honingMeasurement.threeThousand + vm.honingMeasurement.eightThousand + vm.honingMeasurement.elevenThousand + vm.honingMeasurement.caco + vm.honingMeasurement.ihg;

                            vm.honingMeasurement.totalMinute = "65%";
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        ChangingNullToZero();
                        calculationAccordingToColor();
                        ChangingZeroToNull();
                        ChangingZeroToNullForOrginal();
                        addingArea();
                        vm.honingMeasurement.hasOriginalValues = false;
                        var list = angular.copy(vm.honingMeasurement);
                        vm.honingMeasurementList.push(list);
                    }
                    if (vm.isHardening) {
                        if (vm.service.typeOfStoneType2 == "Marble" && vm.service.typeOfStoneType == "Ultimate Finish" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ihg = 2;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinute = "65%";
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && vm.service.typeOfStoneType == "Gloss" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ihg = 2;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinute = "65%";
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneType == "Satin" || vm.service.typeOfStoneType == "Semi-Gloss") && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ihg = 2;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinute = "65%";
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneType == "Matte") && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ihg = 0;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinute = "65%";
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ihg = 2;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinute = "65%";
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Concrete" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ihg = 2;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinute = "65%";
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {
                            vm.honingMeasurement.ihg = 2;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 5;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinuteValue = 50;
                            vm.honingMeasurement.sectionsDivision = 5;
                            vm.honingMeasurement.totalMinute = "50%";
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 50) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = true;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {
                            vm.honingMeasurement.ihg = 2;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 5;
                            vm.honingMeasurement.totalMinuteValue = 50;
                            vm.honingMeasurement.sectionsDivision = 5;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinute = "50%";
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 50) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = true;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if (vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {
                            vm.honingMeasurement.ihg = 2;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 5;
                            vm.honingMeasurement.totalMinuteValue = 50;
                            vm.honingMeasurement.sectionsDivision = 5;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinute = "50%";
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 50) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = true;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        addingArea();
                        var list = angular.copy(vm.honingMeasurement);
                        vm.honingMeasurementList.push(list);
                        ChangingZeroToNull();
                    }
                }

                function addNewHoningMeasurement() {
                    if (vm.isHoning) {
                        if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && vm.service.typeOfStoneType == "Ultimate Finish" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 0;
                            vm.honingMeasurement.fourHundred = 0;
                            vm.honingMeasurement.eightHundred = 0;
                            vm.honingMeasurement.fifteenHundred = 0;
                            vm.honingMeasurement.threeThousand = 0;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 0;
                            vm.honingMeasurement.ihg = 0;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = '';
                            vm.honingMeasurement.totalMinute = '';
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            vm.honingMeasurement.totalCost = 0.00;
                            vm.honingMeasurement.totalCostPerSquare = 0.00;
                            vm.honingMeasurement.dimensionClearance = 1;
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && vm.service.typeOfStoneType == "Gloss" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 0;
                            vm.honingMeasurement.fourHundred = 0;
                            vm.honingMeasurement.eightHundred = 0;
                            vm.honingMeasurement.fifteenHundred = 0;
                            vm.honingMeasurement.threeThousand = 0;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 0;
                            vm.honingMeasurement.ihg = 0;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = '';
                            vm.honingMeasurement.totalMinute = '';
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            vm.honingMeasurement.totalCost = 0.00;
                            vm.honingMeasurement.totalCostPerSquare = 0.00;
                            vm.honingMeasurement.dimensionClearance = 1;
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneType == "Satin" || vm.service.typeOfStoneType == "Semi-Gloss") && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 0;
                            vm.honingMeasurement.fourHundred = 0;
                            vm.honingMeasurement.eightHundred = 0;
                            vm.honingMeasurement.fifteenHundred = 0;
                            vm.honingMeasurement.threeThousand = 0;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 0;
                            vm.honingMeasurement.ihg = 0;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = '';
                            vm.honingMeasurement.totalMinute = '';
                            vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / 65) * 100;
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            vm.honingMeasurement.totalCost = 0.00;
                            vm.honingMeasurement.totalCostPerSquare = 0.00;
                            vm.honingMeasurement.dimensionClearance = 1;
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneType == "Matte") && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 0;
                            vm.honingMeasurement.fourHundred = 0;
                            vm.honingMeasurement.eightHundred = 0;
                            vm.honingMeasurement.fifteenHundred = 0;
                            vm.honingMeasurement.threeThousand = 0;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 0;
                            vm.honingMeasurement.ihg = 0;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.produtivityRate = '';
                            vm.honingMeasurement.totalMinute = '';
                            vm.honingMeasurement.seventeenBase = '';
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            vm.honingMeasurement.totalCost = 0.00;
                            vm.honingMeasurement.totalCostPerSquare = 0.00;
                            vm.honingMeasurement.dimensionClearance = 1;
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 0;
                            vm.honingMeasurement.fourHundred = 0;
                            vm.honingMeasurement.eightHundred = 0;
                            vm.honingMeasurement.fifteenHundred = 0;
                            vm.honingMeasurement.threeThousand = 0;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 0;
                            vm.honingMeasurement.ihg = 0;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = '';
                            vm.honingMeasurement.totalMinute = '';
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.seventeenBase = '';
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            vm.honingMeasurement.totalCost = 0.00;
                            vm.honingMeasurement.totalCostPerSquare = 0.00;
                            vm.honingMeasurement.dimensionClearance = 1;
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Concrete" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 0;
                            vm.honingMeasurement.fourHundred = 0;
                            vm.honingMeasurement.eightHundred = 0;
                            vm.honingMeasurement.fifteenHundred = 0;
                            vm.honingMeasurement.threeThousand = 0;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 0;
                            vm.honingMeasurement.ihg = 0;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = '';
                            vm.honingMeasurement.totalMinute = '';
                            vm.honingMeasurement.seventeenBase = '';
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.totalMinuteValue = 65;
                            vm.honingMeasurement.sectionsDivision = 10;
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            vm.honingMeasurement.totalCost = 0.00;
                            vm.honingMeasurement.totalCostPerSquare = 0.00;
                            vm.honingMeasurement.dimensionClearance = 1;
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 0;
                            vm.honingMeasurement.fourHundred = 0;
                            vm.honingMeasurement.eightHundred = 0;
                            vm.honingMeasurement.fifteenHundred = 0;
                            vm.honingMeasurement.threeThousand = 0;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 0;
                            vm.honingMeasurement.ihg = 0;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 5;
                            vm.honingMeasurement.produtivityRate = '';
                            vm.honingMeasurement.totalMinuteValue = 50;
                            vm.honingMeasurement.sectionsDivision = 5;
                            vm.honingMeasurement.totalMinute = '';
                            vm.honingMeasurement.seventeenBase = '';
                            vm.honingMeasurement.dimension = '1.00';
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = true;
                            vm.honingMeasurement.rowDescription = '';
                            vm.honingMeasurement.totalCost = 0.00;
                            vm.honingMeasurement.totalCostPerSquare = 0.00;
                            vm.honingMeasurement.dimensionClearance = 1;
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 0;
                            vm.honingMeasurement.fourHundred = 0;
                            vm.honingMeasurement.eightHundred = 0;
                            vm.honingMeasurement.fifteenHundred = 0;
                            vm.honingMeasurement.threeThousand = 0;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 0;
                            vm.honingMeasurement.ihg = 0;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 5;
                            vm.honingMeasurement.totalMinuteValue = 50;
                            vm.honingMeasurement.sectionsDivision = 5;
                            vm.honingMeasurement.produtivityRate = '';
                            vm.honingMeasurement.totalMinute = '';
                            vm.honingMeasurement.seventeenBase = '';
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = true;
                            vm.honingMeasurement.rowDescription = '';
                            vm.honingMeasurement.dimensionClearance = 1;
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if (vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.ugc = 0;
                            vm.honingMeasurement.thirty = 0;
                            vm.honingMeasurement.fifty = 0;
                            vm.honingMeasurement.hundred = 0;
                            vm.honingMeasurement.twoHundred = 0;
                            vm.honingMeasurement.fourHundred = 0;
                            vm.honingMeasurement.eightHundred = 0;
                            vm.honingMeasurement.fifteenHundred = 0;
                            vm.honingMeasurement.threeThousand = 0;
                            vm.honingMeasurement.eightThousand = 0;
                            vm.honingMeasurement.elevenThousand = 0;
                            vm.honingMeasurement.caco = 0;
                            vm.honingMeasurement.ihg = 0;
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 5;
                            vm.honingMeasurement.totalMinuteValue = 50;
                            vm.honingMeasurement.sectionsDivision = 5;
                            vm.honingMeasurement.produtivityRate = '';
                            vm.honingMeasurement.totalMinute = '';
                            vm.honingMeasurement.seventeenBase = '';
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            vm.honingMeasurement.isDiameterVisible = true;
                            vm.honingMeasurement.rowDescription = '';
                            vm.honingMeasurement.totalCost = 0.00;
                            vm.honingMeasurement.totalCostPerSquare = 0.00;
                            vm.honingMeasurement.dimensionClearance = 1;
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        ChangingNullToZero();
                        ChangingZeroToNull();
                        addingArea();
                        vm.honingMeasurement.isSaved = false;
                        var list = angular.copy(vm.honingMeasurement);
                        vm.honingMeasurementList.push(list);
                        ChangingZeroToNull();
                        ChangingZeroToNull();
                    }
                    if (vm.isHardening) {
                        if (vm.service.typeOfStoneType2 == "Marble" && vm.service.typeOfStoneType == "Ultimate Finish" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ihg = '';
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinute = '';
                            vm.honingMeasurement.totalMinuteValue = '';
                            vm.honingMeasurement.sectionsDivision = '';
                            vm.honingMeasurement.seventeenBase = '';
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            vm.honingMeasurement.totalCost = 0.00;
                            vm.honingMeasurement.totalCostPerSquare = 0.00;
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && vm.service.typeOfStoneType == "Gloss" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ihg = '';
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinute = '';
                            vm.honingMeasurement.totalMinuteValue = '';
                            vm.honingMeasurement.sectionsDivision = '';
                            vm.honingMeasurement.seventeenBase = '';
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            vm.honingMeasurement.totalCost = 0.00;
                            vm.honingMeasurement.totalCostPerSquare = 0.00;
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneType == "Satin" || vm.service.typeOfStoneType == "Semi-Gloss") && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ihg = '';
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinute = '';
                            vm.honingMeasurement.totalMinuteValue = '';
                            vm.honingMeasurement.sectionsDivision = '';
                            vm.honingMeasurement.seventeenBase = '';
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            vm.honingMeasurement.totalCost = 0.00;
                            vm.honingMeasurement.totalCostPerSquare = 0.00;
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneType == "Matte") && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ihg = '';
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinute = '';
                            vm.honingMeasurement.totalMinuteValue = '';
                            vm.honingMeasurement.sectionsDivision = '';
                            vm.honingMeasurement.seventeenBase = '';
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            vm.honingMeasurement.totalCost = 0.00;
                            vm.honingMeasurement.totalCostPerSquare = 0.00;
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ihg = '';
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinute = '';
                            vm.honingMeasurement.totalMinuteValue = '';
                            vm.honingMeasurement.sectionsDivision = '';
                            vm.honingMeasurement.seventeenBase = '';
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            calculatingMaintainanceCost();
                            vm.honingMeasurement.totalCost = 0.00;
                            vm.honingMeasurement.totalCostPerSquare = 0.00;
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Concrete" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ihg = '';
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinute = '';
                            vm.honingMeasurement.totalMinuteValue = '';
                            vm.honingMeasurement.sectionsDivision = '';
                            vm.honingMeasurement.seventeenBase = '';
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.isDiameterVisible = false;
                            vm.honingMeasurement.rowDescription = '';
                            vm.honingMeasurement.totalCost = 0.00;
                            vm.honingMeasurement.totalCostPerSquare = 0.00;
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {
                            vm.honingMeasurement.ihg = '';
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinute = '';
                            vm.honingMeasurement.totalMinuteValue = '';
                            vm.honingMeasurement.sectionsDivision = '';
                            vm.honingMeasurement.seventeenBase = '';
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.isDiameterVisible = true;
                            vm.honingMeasurement.rowDescription = '';
                            vm.honingMeasurement.totalCost = 0.00;
                            vm.honingMeasurement.totalCostPerSquare = 0.00;
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {
                            vm.honingMeasurement.ihg = '';
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinute = '';
                            vm.honingMeasurement.totalMinuteValue = '';
                            vm.honingMeasurement.sectionsDivision = '';
                            vm.honingMeasurement.seventeenBase = '';
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.isDiameterVisible = true;
                            vm.honingMeasurement.rowDescription = '';
                            vm.honingMeasurement.totalCost = 0.00;
                            vm.honingMeasurement.totalCostPerSquare = 0.00;
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if (vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {
                            vm.honingMeasurement.ihg = '';
                            vm.honingMeasurement.area = 0;
                            vm.honingMeasurement.sections = vm.honingMeasurement.area / 10;
                            vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ihg;
                            vm.honingMeasurement.totalMinute = '';
                            vm.honingMeasurement.totalMinuteValue = '';
                            vm.honingMeasurement.sectionsDivision = '';
                            vm.honingMeasurement.seventeenBase = '';
                            vm.honingMeasurement.dimension = "1.00";
                            vm.honingMeasurement.isDiameterVisible = true;
                            vm.honingMeasurement.rowDescription = '';
                            vm.honingMeasurement.totalCost = 0.00;
                            vm.honingMeasurement.totalCostPerSquare = 0.00;
                            vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                            vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                            vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        addingArea();
                        vm.honingMeasurement.isSaved = false;
                        var list = angular.copy(vm.honingMeasurement);
                        vm.honingMeasurementList.push(list);
                        ChangingZeroToNull();
                    }
                }

                function calculateTimeCalcuation(service) {
                    if (vm.isHoning) {
                        service.isClicked = false;
                        ChangingNullToZero();
                        if (service.area > 0) {
                            vm.isCycleRestorationTabDisabled = false;
                        }
                        else {
                            vm.isCycleRestorationTabDisabled = true;
                        }
                        if (vm.service.typeOfStoneType2 == "Marble" && vm.service.typeOfStoneType == "Ultimate Finish" && vm.service.typeOfSurface == "Floor") {
                            service.sections = (service.area / 10);
                            service.produtivityRate = service.ugc + service.thirty + service.fifty + service.hundred + service.twoHundred +
                                service.fourHundred + service.eightHundred + service.fifteenHundred + service.threeThousand + service.eightThousand + service.elevenThousand + service.caco + service.ihg;
                            service.totalMinute = "65%";
                            service.seventeenBase = (service.produtivityRate / 65) * 100;
                            service.dimension = "1.00";
                            service.totalArea = (service.seventeenBase * service.sections) / (service.dimension);
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            service.totalCostPerSquare = service.totalCost / service.area;
                            if (isNaN(service.totalCostPerSquare)) {
                                service.totalCostPerSquare = '';
                            }
                            service.isDiameterVisible = false;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            if (vm.isSealing) {
                                service.totalCostwithSealing = service.totalCost + service.area;
                                service.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice + Sealing = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2) + " + " + service.area;
                                service.totalCostPerSquare = (service.totalCost + service.area) / service.area;
                                service.totalCostPerSquareHover = "Total Cost / Area = " + (service.totalCost + service.area).toFixed(2) + " / " + service.area.toFixed(2);
                            }
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && vm.service.typeOfStoneType == "Gloss" && vm.service.typeOfSurface == "Floor") {
                            service.sections = (service.area / 10);
                            service.produtivityRate = service.ugc + service.thirty + service.fifty + service.hundred + service.twoHundred +
                                service.fourHundred + service.eightHundred + service.fifteenHundred + service.threeThousand + service.eightThousand + service.elevenThousand + service.caco + service.ihg;
                            service.totalMinute = "65%";
                            service.seventeenBase = (service.produtivityRate / 65) * 100;
                            service.dimension = "1.00";
                            service.totalArea = (service.seventeenBase * service.sections) / (service.dimension);
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            service.totalCostPerSquare = service.totalCost / service.area;
                            if (isNaN(service.totalCostPerSquare)) {
                                service.totalCostPerSquare = '';
                            }
                            service.isDiameterVisible = false;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            if (vm.isSealing) {
                                service.totalCostwithSealing = service.totalCost + service.area;
                                service.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice + Sealing = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2) + " + " + service.area;
                                service.totalCostPerSquare = (service.totalCost + service.area) / service.area;
                                service.totalCostPerSquareHover = "Total Cost / Area = " + (service.totalCost + service.area).toFixed(2) + " / " + service.area.toFixed(2);
                            }
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }

                        else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneType == "Satin" || vm.service.typeOfStoneType == "Semi-Gloss") && vm.service.typeOfSurface == "Floor") {
                            service.sections = (service.area / 10);
                            service.produtivityRate = service.ugc + service.thirty + service.fifty + service.hundred + service.twoHundred +
                                service.fourHundred + service.eightHundred + service.fifteenHundred + service.threeThousand + service.eightThousand + service.elevenThousand + service.caco + service.ihg;
                            service.totalMinute = "65%";
                            service.seventeenBase = (service.produtivityRate / 65) * 100;
                            service.dimension = "1.00";
                            service.totalArea = (service.seventeenBase * service.sections) / (service.dimension);
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            service.totalCostPerSquare = service.totalCost / service.area;
                            if (isNaN(service.totalCostPerSquare)) {
                                service.totalCostPerSquare = '';
                            }
                            service.isDiameterVisible = false;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            if (vm.isSealing) {
                                service.totalCostwithSealing = service.totalCost + service.area;
                                service.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice + Sealing = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2) + " + " + service.area;
                                service.totalCostPerSquare = (service.totalCost + service.area) / service.area;
                                service.totalCostPerSquareHover = "Total Cost / Area = " + (service.totalCost + service.area).toFixed(2) + " / " + service.area.toFixed(2);
                            }
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneType == "Matte") && vm.service.typeOfSurface == "Floor") {
                            service.sections = (service.area / 10);
                            service.produtivityRate = service.ugc + service.thirty + service.fifty + service.hundred + service.twoHundred +
                                service.fourHundred + service.eightHundred + service.fifteenHundred + service.threeThousand + service.eightThousand + service.elevenThousand + service.caco + service.ihg;

                            service.totalMinute = "65%";
                            service.seventeenBase = (service.produtivityRate / 65) * 100;
                            service.dimension = "1.00";
                            service.totalArea = (service.seventeenBase * service.sections) / (service.dimension);
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            service.totalCostPerSquare = service.totalCost / service.area;
                            if (isNaN(service.totalCostPerSquare)) {
                                service.totalCostPerSquare = '';
                            }
                            service.isDiameterVisible = false;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            if (vm.isSealing) {
                                service.totalCostwithSealing = service.totalCost + service.area;
                                service.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice + Sealing = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2) + " + " + service.area;
                                service.totalCostPerSquare = (service.totalCost + service.area) / service.area;
                                service.totalCostPerSquareHover = "Total Cost / Area = " + (service.totalCost + service.area).toFixed(2) + " / " + service.area.toFixed(2);
                            }
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && vm.service.typeOfSurface == "Floor") {
                            service.sections = (service.area / 10);
                            service.produtivityRate = service.ugc + service.thirty + service.fifty + service.hundred + service.twoHundred +
                                service.fourHundred + service.eightHundred + service.fifteenHundred + service.threeThousand + service.eightThousand + service.elevenThousand + service.caco + service.ihg;

                            service.totalMinute = "65%";
                            service.seventeenBase = (service.produtivityRate / 65) * 100;
                            service.dimension = "1.00";
                            service.totalArea = (service.seventeenBase * service.sections) / (service.dimension);
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            service.totalCostPerSquare = service.totalCost / service.area;
                            if (isNaN(service.totalCostPerSquare)) {
                                service.totalCostPerSquare = '';
                            }
                            service.isDiameterVisible = false;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            if (vm.isSealing) {
                                service.totalCostwithSealing = service.totalCost + service.area;
                                service.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice + Sealing = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2) + " + " + service.area;
                                service.totalCostPerSquare = (service.totalCost + service.area) / service.area;
                                service.totalCostPerSquareHover = "Total Cost / Area = " + (service.totalCost + service.area).toFixed(2) + " / " + service.area.toFixed(2);
                            }
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if (vm.service.typeOfStoneType2 == "Concrete" && vm.service.typeOfSurface == "Floor") {
                            service.sections = (service.area / 10);
                            service.produtivityRate = service.ugc + service.thirty + service.fifty + service.hundred + service.twoHundred +
                                service.fourHundred + service.eightHundred + service.fifteenHundred + service.threeThousand + service.eightThousand + service.elevenThousand + service.caco + service.ihg;

                            service.totalMinute = "65%";
                            service.seventeenBase = (service.produtivityRate / 65) * 100;
                            service.dimension = "1.00";
                            service.totalArea = (service.seventeenBase * service.sections) / (service.dimension);
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            service.totalCostPerSquare = service.totalCost / service.area;
                            if (isNaN(service.totalCostPerSquare)) {
                                service.totalCostPerSquare = '';
                            }
                            service.isDiameterVisible = false;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            if (vm.isSealing) {
                                service.totalCostwithSealing = service.totalCost + service.area;
                                service.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice + Sealing = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2) + " + " + service.area;
                                service.totalCostPerSquare = (service.totalCost + service.area) / service.area;
                                service.totalCostPerSquareHover = "Total Cost / Area = " + (service.totalCost + service.area).toFixed(2) + " / " + service.area.toFixed(2);
                            }
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                            if (vm.service.typeOfSurface == "Floor") {
                                service.sections = (service.area / 10);
                            }
                            else {
                                service.sections = (service.area / 5);
                            }
                            service.produtivityRate = service.ugc + service.thirty + service.fifty + service.hundred + service.twoHundred +
                                service.fourHundred + service.eightHundred + service.fifteenHundred + service.threeThousand + service.eightThousand + service.elevenThousand + service.caco + service.ihg;

                            service.totalMinute = "50%";
                            service.seventeenBase = (service.produtivityRate / 50) * 100;
                            service.dimension = "1.00";
                            service.totalArea = (service.seventeenBase * service.sections) / (service.dimension);
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            service.totalCostPerSquare = service.totalCost / service.area;
                            if (isNaN(service.totalCostPerSquare)) {
                                service.totalCostPerSquare = '';
                            }
                            service.isDiameterVisible = true;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            if (vm.isSealing) {
                                service.totalCostwithSealing = service.totalCost + service.area;
                                service.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice + Sealing = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2) + " + " + service.area;
                                service.totalCostPerSquare = (service.totalCost + service.area) / service.area;
                                service.totalCostPerSquareHover = "Total Cost / Area = " + (service.totalCost + service.area).toFixed(2) + " / " + service.area.toFixed(2);
                            }
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                            if (vm.service.typeOfSurface == "Floor") {
                                service.sections = (service.area / 10);
                            }
                            else {
                                service.sections = (service.area / 5);
                            }
                            service.produtivityRate = service.ugc + service.thirty + service.fifty + service.hundred + service.twoHundred +
                                service.fourHundred + service.eightHundred + service.fifteenHundred + service.threeThousand + service.eightThousand + service.elevenThousand + service.caco + service.ihg;

                            service.totalMinute = "50%";
                            service.seventeenBase = (service.produtivityRate / 50) * 100;
                            service.dimension = "1.00";
                            service.totalArea = ((service.seventeenBase * service.sections) / (service.dimension));
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            service.totalCostPerSquare = service.totalCost / service.area;
                            if (isNaN(service.totalCostPerSquare)) {
                                service.totalCostPerSquare = '';
                            }
                            service.isDiameterVisible = true;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            if (vm.isSealing) {
                                service.totalCostwithSealing = service.totalCost + service.area;
                                service.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice + Sealing = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2) + " + " + service.area;
                                service.totalCostPerSquare = (service.totalCost + service.area) / service.area;
                                service.totalCostPerSquareHover = "Total Cost / Area = " + (service.totalCost + service.area).toFixed(2) + " / " + service.area.toFixed(2);
                            }
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();

                        }
                        else if (vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                            if (vm.service.typeOfSurface == "Floor") {
                                service.sections = (service.area / 10);
                            }
                            else {
                                service.sections = (service.area / 5);
                            }
                            service.produtivityRate = service.ugc + service.thirty + service.fifty + service.hundred + service.twoHundred +
                                service.fourHundred + service.eightHundred + service.fifteenHundred + service.threeThousand + service.eightThousand + service.elevenThousand
                                + service.caco + service.ihg;
                            service.totalMinute = "50%";
                            service.seventeenBase = (service.produtivityRate / 50) * 100;
                            service.dimension = "1.00";
                            service.totalArea = (service.seventeenBase * service.sections) / (service.dimension);
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            service.totalCostPerSquare = service.totalCost / service.area;
                            if (isNaN(service.totalCostPerSquare)) {
                                service.totalCostPerSquare = '';
                            }
                            service.isDiameterVisible = true;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            if (vm.isSealing) {
                                service.totalCostwithSealing = service.totalCost + service.area;
                                service.totalCostPerSquare = (service.totalCost + service.area) / service.area;
                                service.totalCostPerSquareHover = "Total Cost / Area = " + (service.totalCost + service.area).toFixed(2) + " / " + service.area.toFixed(2); service.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice + Sealing = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2) + " + " + service.area;
                            }
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.serviceName == "Hone" && vm.service.typeOfStoneType2 == "Terrazzo") {
                            service.sections = (service.area / 10);
                            service.produtivityRate = service.ugc + service.thirty + service.fifty + service.hundred + service.twoHundred +
                                service.fourHundred + service.eightHundred + service.fifteenHundred + service.threeThousand + service.eightThousand + service.elevenThousand + service.caco + service.ihg;
                            service.totalMinute = "65%";
                            service.seventeenBase = (service.produtivityRate / 65) * 100;
                            service.dimension = "1.00";
                            service.totalArea = (service.seventeenBase * service.sections) / (service.dimension);
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            service.totalCostPerSquare = service.totalCost / service.area;
                            if (isNaN(service.totalCostPerSquare)) {
                                service.totalCostPerSquare = '';
                            }
                            service.isDiameterVisible = false;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            if (vm.isSealing) {
                                service.totalCostwithSealing = service.totalCost + service.area;
                                service.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice + Sealing = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2) + " + " + service.area;
                                service.totalCostPerSquare = (service.totalCost + service.area) / service.area;
                                service.totalCostPerSquareHover = "Total Cost / Area = " + (service.totalCost + service.area).toFixed(2) + " / " + service.area.toFixed(2);
                            }
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.serviceName == "Hone" && (vm.service.typeOfStoneType2 == "Carpet" || vm.service.typeOfStoneType2 == "Ceramic" ||
                            vm.service.typeOfStoneType2 == "Concrete" || vm.service.typeOfStoneType2 == "Corian" || vm.service.typeOfStoneType2 == "Glass" ||
                            vm.service.typeOfStoneType2 == "Limestone" || vm.service.typeOfStoneType2 == "Metal" || vm.service.typeOfStoneType2 == "Porcelain" ||
                            vm.service.typeOfStoneType2 == "Quartz" || vm.service.typeOfStoneType2 == "Slate" || vm.service.typeOfStoneType2 == "Vinyl" ||
                            vm.service.typeOfStoneType2 == "Wood")) {
                            service.sections = (service.area / 10);
                            service.produtivityRate = service.ugc + service.thirty + service.fifty + service.hundred + service.twoHundred + service.fourHundred + service.eightHundred + service.fifteenHundred + service.threeThousand + service.eightThousand + service.elevenThousand + service.caco + service.ihg;
                            service.totalMinute = "65%";
                            service.seventeenBase = (service.produtivityRate / 65) * 100;
                            service.dimension = "1.00";
                            service.totalArea = (service.seventeenBase * service.sections) / (service.dimension);
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            service.totalCostPerSquare = service.totalCost / service.area;
                            if (isNaN(service.totalCostPerSquare)) {
                                service.totalCostPerSquare = '';
                            }
                            service.isDiameterVisible = false;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            if (vm.isSealing) {
                                service.totalCostwithSealing = service.totalCost + service.area;
                                service.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice + Sealing = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2) + " + " + service.area;
                                service.totalCostPerSquare = (service.totalCost + service.area) / service.area;
                                service.totalCostPerSquareHover = "Total Cost / Area = " + (service.totalCost + service.area).toFixed(2) + " / " + service.area.toFixed(2);
                            }
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        clearanceValue();
                        ChangingNullToZero();
                        if ((service.area > 0) || (service.ugc > 0) || (service.thirty > 0) || (service.fifty > 0) || (service.hundred > 0) || (service.twoHundred > 0) ||
                            (service.fourHundred > 0) || (service.eightHundred > 0) || (service.fifteenHundred > 0) || (service.threeThousand > 0) || (service.eightThousand > 0) || (service.elevenThousand > 0) || (service.caco > 0) || (service.ihg > 0)) {
                            getHoiningMeasurementOriginal();

                        }
                        else {
                            getHoiningMeasurementOriginalV2(service);
                        }
                        //if (service.ugc >= vm.honingMeasurement.ugcCompare) {
                        //    service.ugcOriginal = service.ugc;
                        //}
                        //if (service.thirty >= vm.honingMeasurement.thirtyCompare) {
                        //    service.thirtyOriginal = service.thirty;
                        //}
                        //if (service.fifty >= vm.honingMeasurement.fiftyCompare) {
                        //    service.fiftyOriginal = service.fifty;
                        //}
                        //if (service.hundred >= vm.honingMeasurement.hundredCompare) {
                        //    service.hundredOriginal = service.hundred;
                        //}
                        //if (service.twoHundred >= vm.honingMeasurement.twoHundredCompare) {
                        //    service.twoHundredOriginal = service.twoHundred;
                        //}
                        //if (service.fourHundred >= vm.honingMeasurement.fourHundredCompare) {
                        //    service.fourHundredOriginal = service.fourHundred;
                        //}
                        //if (service.eightHundred >= vm.honingMeasurement.eightHundredCompare) {
                        //    service.eightHundredOriginal = service.eightHundred;
                        //}
                        //if (service.fifteenHundred >= vm.honingMeasurement.fifteenHundredCompare) {
                        //    service.fifteenHundredOriginal = service.fifteenHundred;
                        //}
                        //if (service.threeThousand >= vm.honingMeasurement.threeThousandCompare) {
                        //    service.threeThousandOriginal = service.threeThousand;
                        //}
                        //if (service.eightThousand >= vm.honingMeasurement.eightThousandCompare) {
                        //    service.eightThousandOriginal = service.eightThousand;
                        //}
                        //if (service.elevenThousand >= vm.honingMeasurement.elevenThousandCompare) {
                        //    service.elevenThousandOriginal = service.elevenThousand;
                        //}
                        //if (service.caco >= vm.honingMeasurement.cacoCompare) {
                        //    service.cacoOriginal = service.caco;
                        //}
                        //if (service.ihg >= vm.honingMeasurement.ihgCompare) {
                        //    service.ihgOriginal = service.ihg;
                        //}
                        getHoiningMeasurementOriginalV2(service);
                        ChangingZeroToNull();
                        ChangingNullToZeroOrginal();
                        ChangingZeroToNullForOrginal();
                    }
                    if (vm.isHardening) {
                        service.isClicked = false;
                        ChangingNullToZero();
                        if (service.area > 0) {
                            vm.isCycleRestorationTabDisabled = false;
                        }
                        else {
                            vm.isCycleRestorationTabDisabled = true;
                        }
                        if (vm.service.typeOfStoneType2 == "Marble" && vm.service.typeOfStoneType == "Ultimate Finish" && vm.service.typeOfSurface == "Floor") {
                            service.sections = (service.area / 10);
                            service.produtivityRate = service.ihg;
                            service.totalMinute = "65%";
                            service.seventeenBase = (service.produtivityRate / 65) * 100;
                            service.dimension = "1.00";
                            service.totalArea = (service.seventeenBase * service.sections) / (service.dimension);
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            if (service.area != 0) {
                                service.totalCostPerSquare = service.totalCost / service.area;
                            }
                            service.isDiameterVisible = false;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && vm.service.typeOfStoneType == "Gloss" && vm.service.typeOfSurface == "Floor") {
                            service.sections = (service.area / 10);
                            service.produtivityRate = service.ihg;
                            service.totalMinute = "65%";
                            service.seventeenBase = (service.produtivityRate / 65) * 100;
                            service.dimension = "1.00";
                            service.totalArea = (service.seventeenBase * service.sections) / (service.dimension);
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            if (service.area != 0) {
                                service.totalCostPerSquare = service.totalCost / service.area;
                            }
                            service.isDiameterVisible = false;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneType == "Satin" || vm.service.typeOfStoneType == "Semi-Gloss") && vm.service.typeOfSurface == "Floor") {
                            service.sections = (service.area / 10);
                            service.produtivityRate = service.ihg;
                            service.totalMinute = "65%";
                            service.seventeenBase = (service.produtivityRate / 65) * 100;
                            service.dimension = "1.00";
                            service.totalArea = (service.seventeenBase * service.sections) / (service.dimension);
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            if (service.area != 0) {
                                service.totalCostPerSquare = service.totalCost / service.area;
                            }
                            service.isDiameterVisible = false;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneType == "Matte") && vm.service.typeOfSurface == "Floor") {
                            service.sections = (service.area / 10);
                            service.produtivityRate = service.ihg;
                            service.totalMinute = "65%";
                            service.seventeenBase = (service.produtivityRate / 65) * 100;
                            service.dimension = "1.00";
                            service.totalArea = (service.seventeenBase * service.sections) / (service.dimension);
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            if (service.area != 0) {
                                service.totalCostPerSquare = service.totalCost / service.area;
                            }
                            service.isDiameterVisible = false;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && vm.service.typeOfSurface == "Floor") {
                            service.sections = (service.area / 10);
                            service.produtivityRate = service.ihg;
                            service.totalMinute = "65%";
                            service.seventeenBase = (service.produtivityRate / 65) * 100;
                            service.dimension = "1.00";
                            service.totalArea = (service.seventeenBase * service.sections) / (service.dimension);
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            if (service.area != 0) {
                                service.totalCostPerSquare = service.totalCost / service.area;
                            }
                            service.isDiameterVisible = false;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Concrete" && vm.service.typeOfSurface == "Floor") {
                            service.sections = (service.area / 10);
                            service.produtivityRate = service.ihg;
                            service.totalMinute = "65%";
                            service.seventeenBase = (service.produtivityRate / 65) * 100;
                            service.dimension = "1.00";
                            service.totalArea = (service.seventeenBase * service.sections) / (service.dimension);
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            if (service.area != 0) {
                                service.totalCostPerSquare = service.totalCost / service.area;
                            }
                            service.isDiameterVisible = false;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {
                            service.sections = (service.area / 5);
                            service.produtivityRate = service.ihg;
                            service.totalMinute = "50%";
                            service.seventeenBase = (service.produtivityRate / 50) * 100;
                            service.dimension = "1.00";
                            service.totalArea = (service.seventeenBase * service.sections) / (service.dimension);
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            if (service.area != 0) {
                                service.totalCostPerSquare = service.totalCost / service.area;
                            }
                            service.isDiameterVisible = true;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {
                            service.sections = (service.area / 5);
                            service.produtivityRate = service.ihg;
                            service.totalMinute = "50%";
                            service.seventeenBase = (service.produtivityRate / 50) * 100;
                            service.dimension = "1.00";
                            service.totalArea = ((service.seventeenBase * service.sections) / (service.dimension));
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            if (service.area != 0) {
                                service.totalCostPerSquare = service.totalCost / service.area;
                            }
                            service.isDiameterVisible = true;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {
                            service.sections = (service.area / 5);
                            service.produtivityRate = service.ihg;
                            service.totalMinute = "50%";
                            service.seventeenBase = (service.produtivityRate / 50) * 100;
                            service.dimension = "1.00";
                            service.totalArea = (service.seventeenBase * service.sections) / (service.dimension);
                            service.totalAreaInHour = service.totalArea / 60;
                            service.totalAreaInShift = service.totalAreaInHour / 8;
                            service.totalCost = service.totalAreaInShift * service.shiftPrice;
                            if (service.area != 0) {
                                service.totalCostPerSquare = service.totalCost / service.area;
                            }
                            service.isDiameterVisible = true;
                            service.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (service.seventeenBase.toFixed(2) + " * " + service.sections) + " / " + (service.dimension);
                            service.totalAreaInHourHover = "Total Time(MIN) / 60 = " + service.totalArea.toFixed(2) + " / " + 60;
                            service.totalAreaInShiftHover = "Total Time(HR) / 8 = " + service.totalAreaInHour.toFixed(2) + " / " + 8;
                            service.totalCostHover = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShift.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                            service.totalCostPerSquareHover = "Total Cost / Area = " + service.totalCost.toFixed(2) + " / " + service.area.toFixed(2);
                            calculatingMaintainanceCost();
                            ChangingZeroToNull();
                        }
                        addingArea();
                    }
                }

                function getLocationList(location) {
                    angular.forEach(location, function (value, index) {
                        vm.locationList.push(value.id);
                    })
                }

                function changeShift() {
                    if (vm.honingMeasurement.shiftName == 'TECH DAY-SHIFT') {
                        vm.honingMeasurement.shiftPrice = vm.franchiseeShiftPrice.techDayShiftPrice;
                    }
                    else if (vm.honingMeasurement.shiftName == 'COMMERCIAL RESTORATION SHIFT') {
                        vm.honingMeasurement.shiftPrice = vm.franchiseeShiftPrice.commercialRestorationShiftPrice;
                    }
                    else if (vm.honingMeasurement.shiftName == 'MAINTENANCE TECH-NIGHT SHIFT') {
                        vm.honingMeasurement.shiftPrice = vm.franchiseeShiftPrice.maintainanceTechNightShiftPrice;
                    }
                }

                function getShiftPrice() {
                    if (vm.classType == 'CONDO' || vm.classType == 'RESIDENTIAL' && (vm.service.serviceType != "MAINTAINCE:BI-MONTHLY" && vm.service.serviceType != "MAINTAINCE:Monthly" && vm.service.serviceType != "MAINTAINCE:(Other)")) {
                        var index = vm.franchiseeShiftPrice.shiftChargesViewValues.indexOf($filter('filter')(vm.franchiseeShiftPrice.shiftChargesViewValues, { display: 'MAINTENANCE TECH-NIGHT SHIFT' }, true)[0]);
                        if (index == -1) {
                            vm.franchiseeShiftPrice.shiftChargesViewValues.push({ display: 'MAINTENANCE TECH-NIGHT SHIFT', value: 'MAINTENANCE TECH-NIGHT SHIFT' });
                        }
                        vm.honingMeasurement.shiftPrice = vm.franchiseeShiftPrice.techDayShiftPrice;
                        vm.honingMeasurement.shiftName = 'TECH DAY-SHIFT';
                        vm.franchiseeShiftPrice.disabled = true;
                    }
                    else if (vm.service.serviceType == "MAINTAINCE:BI-MONTHLY" || vm.service.serviceType == "MAINTAINCE:Monthly" || vm.service.serviceType == "MAINTAINCE:(Other)") {
                        var index = vm.franchiseeShiftPrice.shiftChargesViewValues.indexOf($filter('filter')(vm.franchiseeShiftPrice.shiftChargesViewValues, { display: 'MAINTENANCE TECH-NIGHT SHIFT' }, true)[0]);
                        if (index == -1) {
                            vm.franchiseeShiftPrice.shiftChargesViewValues.push({ display: 'MAINTENANCE TECH-NIGHT SHIFT', value: 'MAINTENANCE TECH-NIGHT SHIFT' });
                        }
                        vm.honingMeasurement.shiftPrice = vm.franchiseeShiftPrice.maintainanceTechNightShiftPrice;
                        vm.honingMeasurement.shiftName = 'MAINTENANCE TECH-NIGHT SHIFT';
                        vm.franchiseeShiftPrice.disabled = true;
                    }
                    else if (vm.classType == 'BUILDER' || vm.classType == 'FLOORING(CONTRACTOR&RETAIL)' || vm.classType == 'INTERIORDESIGN' || vm.classType == 'HOMEMANAGEMENT' || vm.classType == 'JANITORIAL' && (vm.service.serviceType != "MAINTAINCE:BI-MONTHLY" && vm.service.serviceType != "MAINTAINCE:Monthly" && vm.service.serviceType != "MAINTAINCE:(Other)")) {
                        var index = vm.franchiseeShiftPrice.shiftChargesViewValues.indexOf($filter('filter')(vm.franchiseeShiftPrice.shiftChargesViewValues, { display: 'MAINTENANCE TECH-NIGHT SHIFT' }, true)[0]);
                        if (index != -1) {
                            vm.franchiseeShiftPrice.shiftChargesViewValues.splice(index, 1);
                        }
                        vm.honingMeasurement.shiftPrice = vm.franchiseeShiftPrice.techDayShiftPrice;
                        vm.honingMeasurement.shiftName = 'TECH DAY-SHIFT';
                        vm.franchiseeShiftPrice.disabled = false;
                    }
                    else if (vm.classType != 'CONDO' && vm.classType != 'RESIDENTIAL' && (vm.service.serviceType != "MAINTAINCE:BI-MONTHLY" && vm.service.serviceType != "MAINTAINCE:Monthly" && vm.service.serviceType != "MAINTAINCE:(Other)")) {
                        var index = vm.franchiseeShiftPrice.shiftChargesViewValues.indexOf($filter('filter')(vm.franchiseeShiftPrice.shiftChargesViewValues, { display: 'MAINTENANCE TECH-NIGHT SHIFT' }, true)[0]);
                        if (index == -1) {
                            vm.franchiseeShiftPrice.shiftChargesViewValues.push({ display: 'MAINTENANCE TECH-NIGHT SHIFT', value: 'MAINTENANCE TECH-NIGHT SHIFT' });
                        }
                        vm.honingMeasurement.shiftPrice = vm.franchiseeShiftPrice.commercialRestorationShiftPrice;
                        vm.honingMeasurement.shiftName = 'COMMERCIAL RESTORATION SHIFT';
                        vm.franchiseeShiftPrice.disabled = true;
                    }
                }

                function getShiftPriceForHoningMeasurement() {
                    if (vm.classType == 'CONDO' || vm.classType == 'RESIDENTIAL' && (vm.service.serviceType != "MAINTAINCE:BI-MONTHLY" && vm.service.serviceType != "MAINTAINCE:Monthly" && vm.service.serviceType != "MAINTAINCE:(Other)")) {
                        var index = vm.franchiseeShiftPrice.shiftChargesViewValues.indexOf($filter('filter')(vm.franchiseeShiftPrice.shiftChargesViewValues, { display: 'MAINTENANCE TECH-NIGHT SHIFT' }, true)[0]);
                        if (index == -1) {
                            vm.franchiseeShiftPrice.shiftChargesViewValues.push({ display: 'MAINTENANCE TECH-NIGHT SHIFT', value: 'MAINTENANCE TECH-NIGHT SHIFT' });
                        }
                        vm.franchiseeShiftPrice.disabled = true;
                    }
                    else if (vm.service.serviceType == "MAINTAINCE:BI-MONTHLY" || vm.service.serviceType == "MAINTAINCE:Monthly" || vm.service.serviceType == "MAINTAINCE:(Other)") {
                        var index = vm.franchiseeShiftPrice.shiftChargesViewValues.indexOf($filter('filter')(vm.franchiseeShiftPrice.shiftChargesViewValues, { display: 'MAINTENANCE TECH-NIGHT SHIFT' }, true)[0]);
                        if (index == -1) {
                            vm.franchiseeShiftPrice.shiftChargesViewValues.push({ display: 'MAINTENANCE TECH-NIGHT SHIFT', value: 'MAINTENANCE TECH-NIGHT SHIFT' });
                        }
                        vm.franchiseeShiftPrice.disabled = true;
                    }
                    else if (vm.classType == 'BUILDER' || vm.classType == 'FLOORING(CONTRACTOR&RETAIL)' || vm.classType == 'INTERIORDESIGN' || vm.classType == 'HOMEMANAGEMENT' || vm.classType == 'JANITORIAL' && (vm.service.serviceType != "MAINTAINCE:BI-MONTHLY" && vm.service.serviceType != "MAINTAINCE:Monthly" && vm.service.serviceType != "MAINTAINCE:(Other)")) {
                        var index = vm.franchiseeShiftPrice.shiftChargesViewValues.indexOf($filter('filter')(vm.franchiseeShiftPrice.shiftChargesViewValues, { display: 'MAINTENANCE TECH-NIGHT SHIFT' }, true)[0]);
                        if (index != -1) {
                            vm.franchiseeShiftPrice.shiftChargesViewValues.splice(index, 1);
                        }
                        vm.franchiseeShiftPrice.disabled = false;
                    }
                    else if (vm.classType != 'CONDO' && vm.classType != 'RESIDENTIAL' && (vm.service.serviceType != "MAINTAINCE:BI-MONTHLY" && vm.service.serviceType != "MAINTAINCE:Monthly" && vm.service.serviceType != "MAINTAINCE:(Other)")) {
                        var index = vm.franchiseeShiftPrice.shiftChargesViewValues.indexOf($filter('filter')(vm.franchiseeShiftPrice.shiftChargesViewValues, { display: 'MAINTENANCE TECH-NIGHT SHIFT' }, true)[0]);
                        if (index == -1) {
                            vm.franchiseeShiftPrice.shiftChargesViewValues.push({ display: 'MAINTENANCE TECH-NIGHT SHIFT', value: 'MAINTENANCE TECH-NIGHT SHIFT' });
                        }
                        vm.honingMeasurement.shiftName = 'COMMERCIAL RESTORATION SHIFT';
                        vm.franchiseeShiftPrice.disabled = true;
                    }
                }

                function calculateTimeCalcuationChangingShiftPrice() {
                    if (vm.isHoning) {
                        angular.forEach(vm.honingMeasurementList, function (value, index) {
                            if (vm.service.typeOfStoneType2 == "Marble" && vm.service.typeOfStoneType == "Ultimate Finish" && vm.service.typeOfSurface == "Floor") {
                                //value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                value.isDiameterVisible = false;
                                value.shiftPrice = vm.honingMeasurement.shiftPrice;
                                if (value.area != '') {
                                    //value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalArea = value.seventeenBase * value.sections / value.dimension;
                                    value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                                    value.totalAreaInHour = value.totalArea / 60;
                                    value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                                    value.totalAreaInShift = value.totalAreaInHour / 8;
                                    value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                                    value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                    value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                                    if (vm.isSealing) {
                                        value.totalCostwithSealing = value.totalCost + value.area;
                                        value.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2) + " + " + value.area;
                                        value.totalCostPerSquare = (value.totalCost + value.area) / value.area;
                                        value.totalCostPerSquareHover = "Total Cost / Area = " + (value.totalCost + value.area).toFixed(2) + " / " + value.area.toFixed(2);
                                    }
                                    value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                                    calculatingMaintainanceCost();
                                }
                            }
                            else if (vm.service.typeOfStoneType2 == "Marble" && vm.service.typeOfStoneType == "Gloss" && vm.service.typeOfSurface == "Floor") {
                                //value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                value.isDiameterVisible = false;
                                value.shiftPrice = vm.honingMeasurement.shiftPrice;
                                if (value.area != '') {
                                    //value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalArea = value.seventeenBase * value.sections / value.dimension;
                                    value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                                    value.totalAreaInHour = value.totalArea / 60;
                                    value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                                    value.totalAreaInShift = value.totalAreaInHour / 8;
                                    value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                                    value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                    value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                                    if (vm.isSealing) {
                                        value.totalCostwithSealing = value.totalCost + value.area;
                                        value.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2) + " + " + value.area;
                                        value.totalCostPerSquare = (value.totalCost + value.area) / value.area;
                                        value.totalCostPerSquareHover = "Total Cost / Area = " + (value.totalCost + value.area).toFixed(2) + " / " + value.area.toFixed(2);
                                    }
                                    value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                                    calculatingMaintainanceCost();
                                }
                            }
                            else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneType == "Satin" || vm.service.typeOfStoneType == "Semi-Gloss") && vm.service.typeOfSurface == "Floor") {
                                //value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                value.isDiameterVisible = false;
                                value.shiftPrice = vm.honingMeasurement.shiftPrice;
                                if (value.area != '') {
                                    //value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalArea = value.seventeenBase * value.sections / value.dimension;
                                    value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                                    value.totalAreaInHour = value.totalArea / 60;
                                    value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                                    value.totalAreaInShift = value.totalAreaInHour / 8;
                                    value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                                    value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                    value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                                    if (vm.isSealing) {
                                        value.totalCostwithSealing = value.totalCost + value.area;
                                        value.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2) + " + " + value.area;
                                        value.totalCostPerSquare = (value.totalCost + value.area) / value.area;
                                        value.totalCostPerSquareHover = "Total Cost / Area = " + (value.totalCost + value.area).toFixed(2) + " / " + value.area.toFixed(2);
                                    }
                                    value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                                    calculatingMaintainanceCost();
                                }
                            }
                            else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneType == "Matte") && vm.service.typeOfSurface == "Floor") {
                                //value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                value.isDiameterVisible = false;
                                value.shiftPrice = vm.honingMeasurement.shiftPrice;
                                if (value.area != '') {
                                    //value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalArea = value.seventeenBase * value.sections / value.dimension;
                                    value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                                    value.totalAreaInHour = value.totalArea / 60;
                                    value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                                    value.totalAreaInShift = value.totalAreaInHour / 8;
                                    value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                                    value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                    value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                                    if (vm.isSealing) {
                                        value.totalCostwithSealing = value.totalCost + value.area;
                                        value.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2) + " + " + value.area;
                                        value.totalCostPerSquare = (value.totalCost + value.area) / value.area;
                                        value.totalCostPerSquareHover = "Total Cost / Area = " + (value.totalCost + value.area).toFixed(2) + " / " + value.area.toFixed(2);
                                    }
                                    value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                                    calculatingMaintainanceCost();
                                }
                            }
                            else if (vm.service.typeOfStoneType2 == "Granite" && vm.service.typeOfSurface == "Floor") {
                                //value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                value.isDiameterVisible = false;
                                value.shiftPrice = vm.honingMeasurement.shiftPrice;
                                if (value.area != '') {
                                    //value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalArea = value.seventeenBase * value.sections / value.dimension;
                                    value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                                    value.totalAreaInHour = value.totalArea / 60;
                                    value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                                    value.totalAreaInShift = value.totalAreaInHour / 8;
                                    value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                                    value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                    value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                                    if (vm.isSealing) {
                                        value.totalCostwithSealing = value.totalCost + value.area;
                                        value.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2) + " + " + value.area;
                                        value.totalCostPerSquare = (value.totalCost + value.area) / value.area;
                                        value.totalCostPerSquareHover = "Total Cost / Area = " + (value.totalCost + value.area).toFixed(2) + " / " + value.area.toFixed(2);
                                    }
                                    value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                                    calculatingMaintainanceCost();
                                }
                            }
                            else if (vm.service.typeOfStoneType2 == "Concrete" && vm.service.typeOfSurface == "Floor") {
                                //value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                value.isDiameterVisible = false;
                                value.shiftPrice = vm.honingMeasurement.shiftPrice;
                                if (value.area != '') {
                                    //value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalArea = value.seventeenBase * value.sections / value.dimension;
                                    value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                                    value.totalAreaInHour = value.totalArea / 60;
                                    value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                                    value.totalAreaInShift = value.totalAreaInHour / 8;
                                    value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                                    value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                    value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                                    if (vm.isSealing) {
                                        value.totalCostwithSealing = value.totalCost + value.area;
                                        value.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2) + " + " + value.area;
                                        value.totalCostPerSquare = (value.totalCost + value.area) / value.area;
                                        value.totalCostPerSquareHover = "Total Cost / Area = " + (value.totalCost + value.area).toFixed(2) + " / " + value.area.toFixed(2);
                                    }
                                    value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                                    calculatingMaintainanceCost();
                                }
                            }
                            else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                                || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                                || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                                || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                                //value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                value.isDiameterVisible = false;
                                value.shiftPrice = vm.honingMeasurement.shiftPrice;
                                if (value.area != '') {
                                    //value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalArea = value.seventeenBase * value.sections / value.dimension;
                                    value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                                    value.totalAreaInHour = value.totalArea / 60;
                                    value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                                    value.totalAreaInShift = value.totalAreaInHour / 8;
                                    value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                                    value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                    value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                                    if (vm.isSealing) {
                                        value.totalCostwithSealing = value.totalCost + value.area;
                                        value.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2) + " + " + value.area;
                                        value.totalCostPerSquare = (value.totalCost + value.area) / value.area;
                                        value.totalCostPerSquareHover = "Total Cost / Area = " + (value.totalCost + value.area).toFixed(2) + " / " + value.area.toFixed(2);
                                    }
                                    value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                                    calculatingMaintainanceCost();
                                }
                            }
                            else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                                || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                                || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                                || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                                //value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                value.isDiameterVisible = false;
                                value.shiftPrice = vm.honingMeasurement.shiftPrice;
                                if (value.area != '') {
                                    //value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalArea = value.seventeenBase * value.sections / value.dimension;
                                    value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                                    value.totalAreaInHour = value.totalArea / 60;
                                    value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                                    value.totalAreaInShift = value.totalAreaInHour / 8;
                                    value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                                    value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                    value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                                    if (vm.isSealing) {
                                        value.totalCostwithSealing = value.totalCost + value.area;
                                        value.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2) + " + " + value.area;
                                        value.totalCostPerSquare = (value.totalCost + value.area) / value.area;
                                        value.totalCostPerSquareHover = "Total Cost / Area = " + (value.totalCost + value.area).toFixed(2) + " / " + value.area.toFixed(2);
                                    }
                                    value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                                    calculatingMaintainanceCost();
                                }
                            }
                            else if (vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                                || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                                || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                                || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                                //value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                value.isDiameterVisible = false;
                                value.shiftPrice = vm.honingMeasurement.shiftPrice;
                                if (value.area != '') {
                                    //value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalArea = value.seventeenBase * value.sections / value.dimension;
                                    value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                                    value.totalAreaInHour = value.totalArea / 60;
                                    value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                                    value.totalAreaInShift = value.totalAreaInHour / 8;
                                    value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                                    value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                    value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                                    if (vm.isSealing) {
                                        value.totalCostwithSealing = value.totalCost + value.area;
                                        value.totalCostHoverWithSealing = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2) + " + " + value.area;
                                        value.totalCostPerSquare = (value.totalCost + value.area) / value.area;
                                        value.totalCostPerSquareHover = "Total Cost / Area = " + (value.totalCost + value.area).toFixed(2) + " / " + value.area.toFixed(2);
                                    }
                                    value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                                    calculatingMaintainanceCost();
                                }
                            }
                        });
                        getHoiningMeasurementOriginal();
                        ChangingZeroToNullForOrginal();
                        ChangingZeroToNull();
                    }
                    if (vm.isHardening) {
                        ChangingNullToZero();
                        angular.forEach(vm.honingMeasurementList, function (value, index) {
                            if (vm.service.typeOfStoneType2 == "Marble" && vm.service.typeOfStoneType == "Ultimate Finish" && vm.service.typeOfSurface == "Floor") {
                                value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                value.isDiameterVisible = false;
                                value.shiftPrice = vm.honingMeasurement.shiftPrice;
                                if (value.area != '') {
                                    value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                                    value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                                    value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                                    value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                                    value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                                    calculatingMaintainanceCost();
                                }
                            }
                            else if (vm.service.typeOfStoneType2 == "Marble" && vm.service.typeOfStoneType == "Gloss" && vm.service.typeOfSurface == "Floor") {
                                value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                value.isDiameterVisible = false;
                                value.shiftPrice = vm.honingMeasurement.shiftPrice;
                                if (value.area != '') {
                                    value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                                    value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                                    value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                                    value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                                    value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                                    calculatingMaintainanceCost();
                                }
                            }
                            else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneType == "Satin" || vm.service.typeOfStoneType == "Semi-Gloss") && vm.service.typeOfSurface == "Floor") {
                                value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                value.isDiameterVisible = false;
                                value.shiftPrice = vm.honingMeasurement.shiftPrice;
                                if (value.area != '') {
                                    value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                                    value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                                    value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                                    value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                                    value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                                    calculatingMaintainanceCost();
                                }
                            }
                            else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneType == "Matte") && vm.service.typeOfSurface == "Floor") {
                                value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                value.isDiameterVisible = false;
                                value.shiftPrice = vm.honingMeasurement.shiftPrice;
                                if (value.area != '') {
                                    value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                                    value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                                    value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                                    value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                                    value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                                    calculatingMaintainanceCost();
                                }
                            }
                            else if (vm.service.typeOfStoneType2 == "Granite" && vm.service.typeOfSurface == "Floor") {
                                value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                value.isDiameterVisible = false;
                                value.shiftPrice = vm.honingMeasurement.shiftPrice;
                                if (value.area != '') {
                                    value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                                    value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                                    value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                                    value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                                    value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                                    calculatingMaintainanceCost();
                                }
                            }
                            else if (vm.service.typeOfStoneType2 == "Concrete" && vm.service.typeOfSurface == "Floor") {
                                value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                value.isDiameterVisible = false;
                                value.shiftPrice = vm.honingMeasurement.shiftPrice;
                                if (value.area != '') {
                                    value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                                    value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                                    value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                                    value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                                    value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                                    calculatingMaintainanceCost();
                                }
                            }
                            else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                                || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                                || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                                || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                                value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                value.isDiameterVisible = false;
                                value.shiftPrice = vm.honingMeasurement.shiftPrice;
                                if (value.area != '') {
                                    value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                                    value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                                    value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                                    value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                                    value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                                    calculatingMaintainanceCost();
                                }
                            }
                            else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                                || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                                || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                                || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                                value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                value.isDiameterVisible = false;
                                value.shiftPrice = vm.honingMeasurement.shiftPrice;
                                if (value.area != '') {
                                    value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                                    value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                                    value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                                    value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                                    value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                                    calculatingMaintainanceCost();
                                }
                            }
                            else if (vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                                || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                                || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                                || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                                value.totalCost = value.totalAreaInShift * vm.honingMeasurement.shiftPrice;
                                value.isDiameterVisible = false;
                                value.shiftPrice = vm.honingMeasurement.shiftPrice;
                                if (value.area != '') {
                                    value.totalCostPerSquare = value.totalCost / value.area;
                                    value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                                    value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                                    value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                                    value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                                    value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                                    calculatingMaintainanceCost();
                                }
                            }
                        });
                        ChangingZeroToNull();
                    }
                }

                function changingDiv() {
                    $(document).ready(function () {
                        var modalDialog = document.getElementsByClassName('modal-dialog');
                        if (modalDialog != null && modalDialog.length > 0) {
                            if (vm.isHoning) {
                                modalDialog[0].classList.add('show');
                            }
                        }
                        else {
                            modalDialog[0].classList.remove('show');
                        }
                    });
                }

                function calculatingMaintainanceCost() {
                    angular.forEach(vm.honingMeasurementList, function (value, index) {
                        if (vm.maintenanceChargesListCurrent != null && vm.maintenanceChargesListCurrent.length > 0) {
                            value.highDivision = Math.round(value.area / vm.maintenanceChargesListCurrent[0].high);
                            value.lowDivision = Math.round(value.area / vm.maintenanceChargesListCurrent[0].low);
                            value.averageNightRequired = Math.round((value.highDivision + value.lowDivision) / 2);
                            value.averageNightPerMonth = value.averageNightRequired * vm.franchiseeShiftPrice.maintainanceTechNightShiftPrice;
                            value.averageNightPerMonthPerSqFT = value.averageNightPerMonth / value.area;
                            value.annualMaintainceCost = (value.averageNightPerMonth * 12);
                            value.minRestorationValue = (value.annualMaintainceCost / (1 - 0.15));
                            value.highValue = vm.maintenanceChargesListCurrent[0].high;
                            value.materialForFormula = vm.maintenanceChargesListCurrent[0].material;
                            value.lowValue = vm.maintenanceChargesListCurrent[0].low;
                        }
                    })
                }

                function calculatingMaintainanceCostForClearance() {
                    angular.forEach(vm.honingMeasurementList, function (value, index) {
                        if (vm.maintenanceChargesListCurrent != null && vm.maintenanceChargesListCurrent.length > 0) {
                            value.highDivisionClearance = Math.round(value.area / vm.maintenanceChargesListCurrent[0].high);
                            value.lowDivisionClearance = Math.round(value.area / vm.maintenanceChargesListCurrent[0].low);
                            value.averageNightRequiredClearance = Math.round((value.highDivisionClearance + value.lowDivisionClearance) / 2);
                            value.averageNightPerMonthClearance = value.averageNightRequiredClearance * vm.franchiseeShiftPrice.maintainanceTechNightShiftPrice
                            value.averageNightPerMonthPerSqFTClearance = value.averageNightPerMonthClearance / value.area;
                            value.annualMaintainceCostClearance = (value.averageNightPerMonthClearance * 12);
                            value.minRestorationValueClearance = (value.annualMaintainceCostClearance / (1 - 0.15));
                            value.highValueClearance = vm.maintenanceChargesListCurrent[0].high;
                            value.materialForFormulaClearance = vm.maintenanceChargesListCurrent[0].material;
                            value.lowValueClearance = vm.maintenanceChargesListCurrent[0].low;
                        }
                    })
                }

                function ChangingZeroToNull() {
                    angular.forEach(vm.honingMeasurementList, function (value, index) {
                        if (value.ugc == 0) {
                            value.ugc = '';
                        }
                        if (value.thirty == 0) {
                            value.thirty = '';
                        }
                        if (value.fifty == 0) {
                            value.fifty = '';
                        }
                        if (value.hundred == 0) {
                            value.hundred = '';
                        }
                        if (value.twoHundred == 0) {
                            value.twoHundred = '';
                        }
                        if (value.fourHundred == 0) {
                            value.fourHundred = '';
                        }
                        if (value.eightHundred == 0) {
                            value.eightHundred = '';
                        }
                        if (value.fifteenHundred == 0) {
                            value.fifteenHundred = '';
                        }
                        if (value.threeThousand == 0) {
                            value.threeThousand = '';
                        }
                        if (value.eightThousand == 0) {
                            value.eightThousand = '';
                        }
                        if (value.elevenThousand == 0) {
                            value.elevenThousand = '';
                        }
                        if (value.caco == 0) {
                            value.caco = '';
                        }
                        if (value.ihg == 0) {
                            value.ihg = '';
                        }
                        if (value.area == 0) {
                            value.area = '';
                        }
                    }
                    )
                }

                function ChangingNullToZero() {
                    angular.forEach(vm.honingMeasurementList, function (value, index) {
                        if (value.ugc == '') {
                            value.ugc = 0;
                        }
                        if (value.thirty == '') {
                            value.thirty = 0;
                        }
                        if (value.fifty == '') {
                            value.fifty = 0;
                        }
                        if (value.hundred == '') {
                            value.hundred = 0;
                        }
                        if (value.twoHundred == '') {
                            value.twoHundred = 0;
                        }
                        if (value.fourHundred == '') {
                            value.fourHundred = 0;
                        }
                        if (value.eightHundred == '') {
                            value.eightHundred = 0;
                        }
                        if (value.fifteenHundred == '') {
                            value.fifteenHundred = 0;
                        }
                        if (value.threeThousand == '') {
                            value.threeThousand = 0;
                        }
                        if (value.eightThousand == '') {
                            value.eightThousand = 0;
                        }
                        if (value.elevenThousand == '') {
                            value.elevenThousand = 0;
                        }
                        if (value.caco == '') {
                            value.caco = 0;
                        }
                        if (value.ihg == '') {
                            value.ihg = 0;
                        }
                        if (value.area == '') {
                            value.area = 0;
                        }
                    }
                    )
                }

                function ChangingNullToZeroClearance() {
                    angular.forEach(vm.honingMeasurementList, function (value, index) {
                        if (value.ugcClearance == '') {
                            value.ugcClearance = 0;
                        }
                        if (value.thirtyClearance == '') {
                            value.thirtyClearance = 0;
                        }
                        if (value.fiftyClearance == '') {
                            value.fiftyClearance = 0;
                        }
                        if (value.hundredClearance == '') {
                            value.hundredClearance = 0;
                        }
                        if (value.twoHundredClearance == '') {
                            value.twoHundredClearance = 0;
                        }
                        if (value.fourHundredClearance == '') {
                            value.fourHundredClearance = 0;
                        }
                        if (value.eightHundredClearance == '') {
                            value.eightHundredClearance = 0;
                        }
                        if (value.fifteenHundredClearance == '') {
                            value.fifteenHundredClearance = 0;
                        }
                        if (value.threeThousandClearance == '') {
                            value.threeThousandClearance = 0;
                        }
                        if (value.eightThousandClearance == '') {
                            value.eightThousandClearance = 0;
                        }
                        if (value.elevenThousandClearance == '') {
                            value.elevenThousandClearance = 0;
                        }
                        if (value.cacoClearance == '') {
                            value.cacoClearance = 0;
                        }
                        if (value.ihgClearance == '') {
                            value.ihgClearance = 0;
                        }
                        if (value.areaClearance == '') {
                            value.areaClearance = 0;
                        }
                    }
                    )
                }

                function ChangingNullToZeroOrginal() {
                    angular.forEach(vm.honingMeasurementList, function (value, index) {
                        if (value.ugcOriginal == '' || value.ugcOriginal == undefined) {
                            value.ugcOriginal = 0;
                        }
                        if (value.thirtyOriginal == '' || value.thirtyOriginal == undefined) {
                            value.thirtyOriginal = 0;
                        }
                        if (value.fiftyOriginal == '' || value.fiftyOriginal == undefined) {
                            value.fiftyOriginal = 0;
                        }
                        if (value.hundredOriginal == '' || value.hundredOriginal == undefined) {
                            value.hundredOriginal = 0;
                        }
                        if (value.twoHundredOriginal == '' || value.twoHundredOriginal == undefined) {
                            value.twoHundredOriginal = 0;
                        }
                        if (value.fourHundredOriginal == '' || value.fourHundredOriginal == undefined) {
                            value.fourHundredOriginal = 0;
                        }
                        if (value.eightHundredOriginal == '' || value.eightHundredOriginal == undefined) {
                            value.eightHundredOriginal = 0;
                        }
                        if (value.fifteenHundredOriginal == '' || value.fifteenHundredOriginal == undefined) {
                            value.fifteenHundredOriginal = 0;
                        }
                        if (value.threeThousandOriginal == '' || value.threeThousandOriginal == undefined) {
                            value.threeThousandOriginal = 0;
                        }
                        if (value.eightThousandOriginal == '' || value.eightThousandOriginal == undefined) {
                            value.eightThousandOriginal = 0;
                        }
                        if (value.elevenThousandOriginal == '' || value.elevenThousandOriginal == undefined) {
                            value.elevenThousandOriginal = 0;
                        }
                        if (value.cacoOriginal == '' || value.cacoOriginal == undefined) {
                            value.cacoOriginal = 0;
                        }
                        if (value.ihgOriginal == '' || value.ihgOriginal == undefined) {
                            value.ihgOriginal = 0;
                        }
                        if (value.areaOriginal == '' || value.areaOriginal == undefined) {
                            value.areaOriginal = 0;
                        }
                    })
                }

                function changeTab(tabNo) {
                    if (tabNo == 1) {
                        vm.isTimeEstimateTab = true;
                        vm.isCycleRestorationTab = false;
                        angular.element(document.querySelector("#timeEstimate")).addClass("active");
                        angular.element(document.querySelector("#cycleRestoration")).removeClass("active");
                        vm.timeEstimateDisable = true;
                    }
                    if (tabNo == 2) {
                        vm.isTimeEstimateTab = false;
                        vm.isCycleRestorationTab = true;
                        angular.element(document.querySelector("#timeEstimate")).removeClass("active");
                        angular.element(document.querySelector("#cycleRestoration")).addClass("active");
                        getCycleRestorationPlan(false);
                        vm.timeEstimateDisable = false;
                    }
                }

                function getCycleRestorationPlan(isChanged) {

                    vm.cycleRestorationPlan.area = vm.service.sumTotalArea;
                    vm.cycleRestorationPlan.high = vm.maintenanceChargesListCurrent[0].high;
                    vm.cycleRestorationPlan.low = vm.maintenanceChargesListCurrent[0].low;
                    vm.cycleRestorationPlan.highLow = vm.maintenanceChargesListCurrent[0].high + vm.maintenanceChargesListCurrent[0].low;
                    if (!isChanged) {
                        vm.cycleRestorationPlan.startingPointTechShiftEstimates = 0;
                    }
                    vm.cycleRestorationPlan.restorationCost = 0;
                    vm.cycleRestorationPlan.minimumRestorationCost = 0;
                    vm.cycleRestorationPlan.timeFor1_10SqFtSection = 0;
                    vm.cycleRestorationPlan.timeFor1Day = 0;
                    vm.cycleRestorationPlan.averageNightRequired = 0;
                    angular.forEach(vm.honingMeasurementList, function (value, index) {
                        vm.cycleRestorationPlan.averageNightRequired += value.averageNightRequired;
                        if (!isChanged) {
                            if (value.startingPointTechShiftEstimates != undefined) {
                                vm.cycleRestorationPlan.startingPointTechShiftEstimates += value.startingPointTechShiftEstimates
                            }
                            else {
                                vm.cycleRestorationPlan.startingPointTechShiftEstimates += Math.round(vm.cycleRestorationPlan.averageNightRequired * 1.2);
                            }
                        }
                        vm.cycleRestorationPlan.restorationCost += value.totalCost;
                        vm.cycleRestorationPlan.minimumRestorationCost += value.minRestorationValue;
                        vm.cycleRestorationPlan.timeFor1_10SqFtSection += Math.round(value.seventeenBase);
                        vm.cycleRestorationPlan.timeFor1Day += Math.round(480 / value.seventeenBase) * 10;
                    });
                    var restorationCost = vm.cycleRestorationPlan.restorationCost;
                    vm.cycleRestorationPlan.restorationCost = restorationCost.toFixed(2);
                    var minimumRestorationCost = vm.cycleRestorationPlan.minimumRestorationCost;
                    vm.cycleRestorationPlan.minimumRestorationCost = minimumRestorationCost.toFixed(2);
                    vm.cycleRestorationPlan.maintenanceTargetPerMo = (vm.cycleRestorationPlan.averageNightRequired * vm.franchiseeShiftPrice.maintainanceTechNightShiftPrice).toFixed(2);
                    vm.cycleRestorationPlan.cycleRestorationPerMo = (vm.cycleRestorationPlan.startingPointTechShiftEstimates * vm.franchiseeShiftPrice.maintainanceTechNightShiftPrice).toFixed(2);
                    vm.cycleRestorationPlan.annualCycleRestorationPerMo = (12 * vm.cycleRestorationPlan.cycleRestorationPerMo).toFixed(2);

                    vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList = [];
                    vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList.push({
                        month: 1,
                        totalNights: vm.cycleRestorationPlan.startingPointTechShiftEstimates,
                        nightRestoration: vm.cycleRestorationPlan.startingPointTechShiftEstimates,
                        amountRestoredInMonth: (vm.cycleRestorationPlan.timeFor1Day * vm.cycleRestorationPlan.startingPointTechShiftEstimates),
                        amountRestoredToThisPoint: vm.cycleRestorationPlan.timeFor1Day * vm.cycleRestorationPlan.startingPointTechShiftEstimates,
                        completionRate: Math.round((vm.cycleRestorationPlan.timeFor1Day * vm.cycleRestorationPlan.startingPointTechShiftEstimates * 100) / vm.cycleRestorationPlan.area)
                    });
                    vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList.push({
                        month: 2,
                        totalNights: vm.cycleRestorationPlan.startingPointTechShiftEstimates,
                        nightRestoration: vm.cycleRestorationPlan.startingPointTechShiftEstimates,
                        amountRestoredInMonth: (vm.cycleRestorationPlan.timeFor1Day * vm.cycleRestorationPlan.startingPointTechShiftEstimates),
                        amountRestoredToThisPoint: 2 * vm.cycleRestorationPlan.timeFor1Day * vm.cycleRestorationPlan.startingPointTechShiftEstimates,
                        completionRate: Math.round((2 * vm.cycleRestorationPlan.timeFor1Day * vm.cycleRestorationPlan.startingPointTechShiftEstimates * 100) / vm.cycleRestorationPlan.area)
                    });
                    vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList.push({
                        month: 3,
                        totalNights: vm.cycleRestorationPlan.startingPointTechShiftEstimates,
                        nightRestoration: vm.cycleRestorationPlan.startingPointTechShiftEstimates,
                        amountRestoredInMonth: (vm.cycleRestorationPlan.timeFor1Day * vm.cycleRestorationPlan.startingPointTechShiftEstimates),
                        amountRestoredToThisPoint: 3 * vm.cycleRestorationPlan.timeFor1Day * vm.cycleRestorationPlan.startingPointTechShiftEstimates,
                        completionRate: Math.round((3 * vm.cycleRestorationPlan.timeFor1Day * vm.cycleRestorationPlan.startingPointTechShiftEstimates * 100) / vm.cycleRestorationPlan.area)
                    });

                    var nightMaintainanceForFour = ((vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[0].amountRestoredToThisPoint / vm.cycleRestorationPlan.highLow) / 2);
                    var nightRestorationForFour = vm.cycleRestorationPlan.startingPointTechShiftEstimates - nightMaintainanceForFour
                    var amountRestoredForFour = Math.round(vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[0].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[1].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[2].amountRestoredInMonth +
                        (vm.cycleRestorationPlan.timeFor1Day * nightRestorationForFour))
                    vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList.push({
                        month: 4,
                        totalNights: vm.cycleRestorationPlan.startingPointTechShiftEstimates,
                        nightMaintenance: nightMaintainanceForFour,
                        nightRestoration: nightRestorationForFour,
                        amountRestoredInMonth: Math.round(vm.cycleRestorationPlan.timeFor1Day * nightRestorationForFour),
                        amountRestoredToThisPoint: amountRestoredForFour,
                        completionRate: Math.round((100 * amountRestoredForFour) / vm.cycleRestorationPlan.area)
                    });


                    var nightMaintainanceForFive = ((vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[1].amountRestoredToThisPoint / vm.cycleRestorationPlan.highLow) / 2);
                    var nightRestorationForFive = vm.cycleRestorationPlan.startingPointTechShiftEstimates - nightMaintainanceForFive;
                    var amountRestoredForFive = Math.round(vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[0].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[1].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[2].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[3].amountRestoredInMonth +
                        (vm.cycleRestorationPlan.timeFor1Day * nightRestorationForFive))
                    vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList.push({
                        month: 5,
                        totalNights: vm.cycleRestorationPlan.startingPointTechShiftEstimates,
                        nightMaintenance: nightMaintainanceForFive,
                        nightRestoration: nightRestorationForFive,
                        amountRestoredInMonth: Math.round(vm.cycleRestorationPlan.timeFor1Day * nightRestorationForFive),
                        amountRestoredToThisPoint: amountRestoredForFive,
                        completionRate: Math.round((100 * amountRestoredForFive) / vm.cycleRestorationPlan.area)
                    });


                    var nightMaintainanceForSix = ((vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[2].amountRestoredToThisPoint / vm.cycleRestorationPlan.highLow) / 2);
                    var nightRestorationForSix = vm.cycleRestorationPlan.startingPointTechShiftEstimates - nightMaintainanceForSix;
                    var amountRestoredForSix = Math.round(vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[0].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[1].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[2].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[3].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[4].amountRestoredInMonth +
                        (vm.cycleRestorationPlan.timeFor1Day * nightRestorationForSix))
                    vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList.push({
                        month: 6,
                        totalNights: vm.cycleRestorationPlan.startingPointTechShiftEstimates,
                        nightMaintenance: nightMaintainanceForSix,
                        nightRestoration: nightRestorationForSix,
                        amountRestoredInMonth: Math.round(vm.cycleRestorationPlan.timeFor1Day * nightRestorationForSix),
                        amountRestoredToThisPoint: amountRestoredForSix,
                        completionRate: Math.round((100 * amountRestoredForSix) / vm.cycleRestorationPlan.area)
                    });

                    var nightMaintainanceForSeven = ((vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[3].amountRestoredToThisPoint / vm.cycleRestorationPlan.highLow) / 2);
                    var nightRestorationForSeven = vm.cycleRestorationPlan.startingPointTechShiftEstimates - nightMaintainanceForSeven;
                    var amountRestoredForSeven = Math.round(vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[0].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[1].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[2].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[3].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[4].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[5].amountRestoredInMonth +
                        (vm.cycleRestorationPlan.timeFor1Day * nightRestorationForSeven))
                    vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList.push({
                        month: 7,
                        totalNights: vm.cycleRestorationPlan.startingPointTechShiftEstimates,
                        nightMaintenance: nightMaintainanceForSeven,
                        nightRestoration: nightRestorationForSeven,
                        amountRestoredInMonth: Math.round(vm.cycleRestorationPlan.timeFor1Day * nightRestorationForSeven),
                        amountRestoredToThisPoint: amountRestoredForSeven,
                        completionRate: Math.round((100 * amountRestoredForSeven) / vm.cycleRestorationPlan.area)
                    });

                    var nightMaintainanceForEight = ((vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[4].amountRestoredToThisPoint / vm.cycleRestorationPlan.highLow) / 2);
                    var nightRestorationForEight = vm.cycleRestorationPlan.startingPointTechShiftEstimates - nightMaintainanceForEight;
                    var amountRestoredForEight = Math.round(vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[0].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[1].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[2].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[3].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[4].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[5].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[6].amountRestoredInMonth +
                        (vm.cycleRestorationPlan.timeFor1Day * nightRestorationForEight))
                    vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList.push({
                        month: 8,
                        totalNights: vm.cycleRestorationPlan.startingPointTechShiftEstimates,
                        nightMaintenance: nightMaintainanceForEight,
                        nightRestoration: nightRestorationForEight,
                        amountRestoredInMonth: Math.round(vm.cycleRestorationPlan.timeFor1Day * nightRestorationForEight),
                        amountRestoredToThisPoint: amountRestoredForEight,
                        completionRate: Math.round((100 * amountRestoredForEight) / vm.cycleRestorationPlan.area)
                    });

                    var nightMaintainanceForNine = ((vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[5].amountRestoredToThisPoint / vm.cycleRestorationPlan.highLow) / 2);
                    var nightRestorationForNine = vm.cycleRestorationPlan.startingPointTechShiftEstimates - nightMaintainanceForNine;
                    var amountRestoredForNine = Math.round(vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[0].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[1].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[2].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[3].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[4].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[5].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[6].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[7].amountRestoredInMonth +
                        (vm.cycleRestorationPlan.timeFor1Day * nightRestorationForNine))
                    vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList.push({
                        month: 9,
                        totalNights: vm.cycleRestorationPlan.startingPointTechShiftEstimates,
                        nightMaintenance: nightMaintainanceForNine,
                        nightRestoration: nightRestorationForNine,
                        amountRestoredInMonth: Math.round(vm.cycleRestorationPlan.timeFor1Day * nightRestorationForNine),
                        amountRestoredToThisPoint: amountRestoredForNine,
                        completionRate: Math.round((100 * amountRestoredForNine) / vm.cycleRestorationPlan.area)
                    });

                    var nightMaintainanceForTen = ((vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[6].amountRestoredToThisPoint / vm.cycleRestorationPlan.highLow) / 2);
                    var nightRestorationForTen = vm.cycleRestorationPlan.startingPointTechShiftEstimates - nightMaintainanceForTen;
                    var amountRestoredForTen = Math.round(vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[0].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[1].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[2].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[3].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[4].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[5].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[6].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[7].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[8].amountRestoredInMonth +
                        (vm.cycleRestorationPlan.timeFor1Day * nightRestorationForTen))
                    vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList.push({
                        month: 10,
                        totalNights: vm.cycleRestorationPlan.startingPointTechShiftEstimates,
                        nightMaintenance: nightMaintainanceForTen,
                        nightRestoration: nightRestorationForTen,
                        amountRestoredInMonth: Math.round(vm.cycleRestorationPlan.timeFor1Day * nightRestorationForTen),
                        amountRestoredToThisPoint: amountRestoredForTen,
                        completionRate: Math.round((100 * amountRestoredForTen) / vm.cycleRestorationPlan.area)
                    });

                    var nightMaintainanceForEleven = ((vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[7].amountRestoredToThisPoint / vm.cycleRestorationPlan.highLow) / 2);
                    var nightRestorationForEleven = vm.cycleRestorationPlan.startingPointTechShiftEstimates - nightMaintainanceForEleven;
                    var amountRestoredForEleven = Math.round(vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[0].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[1].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[2].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[3].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[4].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[5].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[6].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[7].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[8].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[9].amountRestoredInMonth +
                        (vm.cycleRestorationPlan.timeFor1Day * nightRestorationForEleven))
                    vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList.push({
                        month: 11,
                        totalNights: vm.cycleRestorationPlan.startingPointTechShiftEstimates,
                        nightMaintenance: nightMaintainanceForEleven,
                        nightRestoration: nightRestorationForEleven,
                        amountRestoredInMonth: Math.round(vm.cycleRestorationPlan.timeFor1Day * nightRestorationForEleven),
                        amountRestoredToThisPoint: amountRestoredForEleven,
                        completionRate: Math.round((100 * amountRestoredForEleven) / vm.cycleRestorationPlan.area)
                    });

                    var nightMaintainanceForTwelve = ((vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[8].amountRestoredToThisPoint / vm.cycleRestorationPlan.highLow) / 2);
                    var nightRestorationForTwelve = vm.cycleRestorationPlan.startingPointTechShiftEstimates - nightMaintainanceForTwelve;
                    var amountRestoredForTwelve = Math.round(vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[0].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[1].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[2].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[3].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[4].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[5].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[6].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[7].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[8].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[9].amountRestoredInMonth +
                        vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList[10].amountRestoredInMonth +
                        (vm.cycleRestorationPlan.timeFor1Day * nightRestorationForTwelve))
                    vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList.push({
                        month: 12,
                        totalNights: vm.cycleRestorationPlan.startingPointTechShiftEstimates,
                        nightMaintenance: nightMaintainanceForTwelve,
                        nightRestoration: nightRestorationForTwelve,
                        amountRestoredInMonth: Math.round(vm.cycleRestorationPlan.timeFor1Day * nightRestorationForTwelve),
                        amountRestoredToThisPoint: amountRestoredForTwelve,
                        completionRate: Math.round((100 * amountRestoredForTwelve) / vm.cycleRestorationPlan.area)
                    });
                    var count = 0;
                    angular.forEach(vm.cycleRestorationPlan.cycleRestorationPlanMonthlyList, function (value, index) {
                        if (count == 0 && value.completionRate > 100) {
                            value.isCompleted = true;
                            value.completed = 'Completed';
                            count = count + 1;
                        }
                    });
                }

                function getHoiningMeasurementOriginalV2(service) {
                    if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && vm.service.typeOfStoneType == "Ultimate Finish" && vm.service.typeOfSurface == "Floor") {
                        ChangingNullToZeroOrginal();
                        service.sectionsOriginalDivision = 10;
                        service.sectionsOriginal = service.area / 10;
                        service.produtivityRateOriginal = service.ugcOriginal + service.thirtyOriginal + service.fiftyOriginal + service.hundredOriginal + service.twoHundredOriginal +
                            service.fourHundredOriginal + service.eightHundredOriginal + service.fifteenHundredOriginal + service.threeThousandOriginal + service.eightThousandOriginal + service.elevenThousandOriginal + service.cacoOriginal + service.ihgOriginal;
                        service.totalMinuteOriginal = "65%";
                        service.totalMinuteserviceOriginal = 65;
                        service.seventeenBaseOriginal = (service.produtivityRateOriginal / 65) * 100;
                        service.totalAreaOriginal = (service.seventeenBaseOriginal * service.sectionsOriginal) / (service.dimension);
                        service.totalAreaInHourOriginal = service.totalAreaOriginal / 60;
                        service.totalAreaInShiftOriginal = service.totalAreaInHourOriginal / 8;
                        service.totalCostOriginal = service.totalAreaInShiftOriginal * service.shiftPrice;
                        service.totalCostPerSquareOriginal = service.totalCostOriginal / service.area;
                        service.isDiameterVisible = false;
                        calculatingMaintainanceCostForOriginal();
                        ChangingZeroToNull();
                    }
                    else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && vm.service.typeOfStoneType == "Gloss" && vm.service.typeOfSurface == "Floor") {
                        ChangingNullToZeroOrginal();
                        service.sectionsOriginal = service.area / 10;
                        service.sectionsOriginalDivision = 10;
                        service.produtivityRateOriginal = service.ugcOriginal + service.thirtyOriginal + service.fiftyOriginal + service.hundredOriginal + service.twoHundredOriginal +
                            service.fourHundredOriginal + service.eightHundredOriginal + service.fifteenHundredOriginal + service.threeThousandOriginal + service.eightThousandOriginal + service.elevenThousandOriginal + service.cacoOriginal + service.ihgOriginal;

                        service.totalMinuteserviceOriginal = 65;
                        service.totalMinuteOriginal = "65%";
                        service.seventeenBaseOriginal = (service.produtivityRateOriginal / 65) * 100;
                        service.totalAreaOriginal = (service.seventeenBaseOriginal * service.sections) / (service.dimension);
                        service.totalAreaInHourOriginal = service.totalAreaOriginal / 60;
                        service.totalAreaInShiftOriginal = service.totalAreaInHourOriginal / 8;
                        service.totalCostOriginal = service.totalAreaInShiftOriginal * service.shiftPrice;
                        service.totalCostPerSquareOriginal = service.totalCostOriginal / service.area;
                        service.isDiameterVisible = false;
                        calculatingMaintainanceCostForOriginal();
                        ChangingZeroToNull();
                    }
                    else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneType == "Satin" || vm.service.typeOfStoneType == "Semi-Gloss") && vm.service.typeOfSurface == "Floor") {
                        ChangingNullToZeroOrginal();
                        service.sectionsOriginal = service.area / 10;
                        service.sectionsOriginalDivision = 10;
                        service.produtivityRateOriginal = service.ugcOriginal + service.thirtyOriginal + service.fiftyOriginal + service.hundredOriginal + service.twoHundredOriginal +
                            service.fourHundredOriginal + service.eightHundredOriginal + service.fifteenHundredOriginal + service.threeThousandOriginal + service.eightThousandOriginal + service.elevenThousandOriginal + service.cacoOriginal + service.ihgOriginal;

                        service.totalMinuteserviceOriginal = 65;
                        service.totalMinuteOriginal = "65%";
                        service.seventeenBaseOriginal = (service.produtivityRateOriginal / 65) * 100;
                        service.totalAreaOriginal = (service.seventeenBaseOriginal * service.sections) / (service.dimension);
                        service.totalAreaInHourOriginal = service.totalAreaOriginal / 60;
                        service.totalAreaInShiftOriginal = service.totalAreaInHourOriginal / 8;
                        service.totalCostOriginal = service.totalAreaInShiftOriginal * service.shiftPrice;
                        service.totalCostPerSquareOriginal = service.totalCostOriginal / service.area;
                        service.isDiameterVisible = false;
                        calculatingMaintainanceCostForOriginal();
                        ChangingZeroToNull();
                    }
                    else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneType == "Matte") && vm.service.typeOfSurface == "Floor") {
                        ChangingNullToZeroOrginal();
                        service.sectionsOriginal = service.area / 10;
                        service.sectionsOriginalDivision = 10;
                        service.produtivityRateOriginal = service.ugcOriginal + service.thirtyOriginal + service.fiftyOriginal + service.hundredOriginal + service.twoHundredOriginal +
                            service.fourHundredOriginal + service.eightHundredOriginal + service.fifteenHundredOriginal + service.threeThousandOriginal + service.eightThousandOriginal + service.elevenThousandOriginal + service.cacoOriginal + service.ihgOriginal;

                        service.totalMinuteserviceOriginal = 65;
                        service.totalMinuteOriginal = "65%";
                        service.seventeenBaseOriginal = (service.produtivityRateOriginal / 65) * 100;
                        service.totalAreaOriginal = (service.seventeenBaseOriginal * service.sections) / (service.dimension);
                        service.totalAreaInHourOriginal = service.totalAreaOriginal / 60;
                        service.totalAreaInShiftOriginal = service.totalAreaInHourOriginal / 8;
                        service.totalCostOriginal = service.totalAreaInShiftOriginal * service.shiftPrice;
                        service.totalCostPerSquareOriginal = service.totalCostOriginal / service.area;
                        service.isDiameterVisible = false;
                        calculatingMaintainanceCostForOriginal();
                        ChangingZeroToNull();
                    }
                    else if (vm.service.typeOfStoneType2 == "Granite" && vm.service.typeOfSurface == "Floor") {
                        ChangingNullToZeroOrginal();
                        service.sectionsOriginal = service.area / 10;
                        service.sectionsOriginalDivision = 10;
                        service.produtivityRateOriginal = service.ugcOriginal + service.thirtyOriginal + service.fiftyOriginal + service.hundredOriginal + service.twoHundredOriginal +
                            service.fourHundredOriginal + service.eightHundredOriginal + service.fifteenHundredOriginal + service.threeThousandOriginal + service.eightThousandOriginal + service.elevenThousandOriginal + service.cacoOriginal + service.ihgOriginal;

                        service.totalMinuteserviceOriginal = 65;
                        service.totalMinuteOriginal = "65%";
                        service.seventeenBaseOriginal = (service.produtivityRateOriginal / 65) * 100;
                        service.totalAreaOriginal = (service.seventeenBaseOriginal * service.sectionsOriginal) / (service.dimension);
                        service.totalAreaInHourOriginal = service.totalAreaOriginal / 60;
                        service.totalAreaInShiftOriginal = service.totalAreaInHourOriginal / 8
                        service.isDiameterVisible = false;
                        service.totalCostOriginal = service.totalAreaInShiftOriginal * service.shiftPrice;
                        service.totalCostPerSquareOriginal = service.totalCostOriginal / service.area;
                        calculatingMaintainanceCostForOriginal();
                        ChangingZeroToNull();
                    }
                    else if (vm.service.typeOfStoneType2 == "Concrete" && vm.service.typeOfSurface == "Floor") {
                        ChangingNullToZeroOrginal();
                        service.sectionsOriginal = service.area / 10;
                        service.sectionsOriginalDivision = 10;
                        service.produtivityRateOriginal = service.ugcOriginal + service.thirtyOriginal + service.fiftyOriginal + service.hundredOriginal + service.twoHundredOriginal +
                            service.fourHundredOriginal + service.eightHundredOriginal + service.fifteenHundredOriginal + service.threeThousandOriginal + service.eightThousandOriginal +
                            service.elevenThousandOriginal + service.cacoOriginal + service.ihgOriginal;
                        service.totalMinuteserviceOriginal = 65;
                        service.totalMinuteOriginal = "65%";
                        service.seventeenBaseOriginal = (service.produtivityRateOriginal / 65) * 100;
                        service.totalAreaOriginal = (service.seventeenBaseOriginal * service.sections) / (service.dimension);
                        service.totalAreaInHourOriginal = service.totalAreaOriginal / 60;
                        service.totalAreaInShiftOriginal = service.totalAreaInHourOriginal / 8;
                        service.isDiameterVisible = false;
                        service.totalCostOriginal = service.totalAreaInShiftOriginal * service.shiftPrice;
                        service.totalCostPerSquareOriginal = service.totalCostOriginal / service.area;
                        calculatingMaintainanceCostForOriginal();
                        ChangingZeroToNull();
                    }
                    else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                        || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                        || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                        || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {
                        ChangingNullToZeroOrginal();
                        service.sectionsOriginal = service.area / 10;
                        service.sectionsOriginalDivision = 10;
                        service.produtivityRateOriginal = service.ugcOriginal + service.thirtyOriginal + service.fiftyOriginal +
                            service.hundredOriginal + service.twoHundredOriginal + service.fourHundredOriginal +
                            service.eightHundredOriginal + service.fifteenHundredOriginal + service.threeThousandOriginal +
                            service.eightThousandOriginal + service.elevenThousandOriginal + service.cacoOriginal + service.ihgOriginal;
                        service.totalMinuteserviceOriginal = 50;
                        service.totalMinuteOriginal = "50%";
                        service.seventeenBaseOriginal = (service.produtivityRateOriginal / 50) * 100;
                        service.totalAreaOriginal = (service.seventeenBaseOriginal * service.sectionsOriginal) / (service.dimension);
                        service.totalAreaInHourOriginal = service.totalAreaOriginal / 60;
                        service.totalAreaInShiftOriginal = service.totalAreaInHourOriginal / 8;
                        service.isDiameterVisible = true;
                        service.totalCostOriginal = service.totalAreaInShiftOriginal * service.shiftPrice;
                        service.totalCostPerSquareOriginal = service.totalCostOriginal / service.area;
                        calculatingMaintainanceCostForOriginal();
                        ChangingZeroToNull();
                    }
                    else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                        || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                        || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                        || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                        ChangingNullToZeroOrginal();
                        service.sectionsOriginal = service.area / 10;
                        service.sectionsOriginalDivision = 10;
                        service.produtivityRateOriginal = service.ugcOriginal + service.thirtyOriginal + service.fiftyOriginal + service.hundredOriginal + service.twoHundredOriginal +
                            service.fourHundredOriginal + service.eightHundredOriginal + service.fifteenHundredOriginal +
                            service.threeThousandOriginal + service.eightThousandOriginal + service.elevenThousandOriginal +
                            service.cacoOriginal + service.ihgOriginal;
                        service.totalMinuteserviceOriginal = 50;
                        service.totalMinuteOriginal = "50%";
                        service.seventeenBaseOriginal = (service.produtivityRate / 50) * 100;
                        service.totalAreaOriginal = (service.seventeenBaseOriginal * service.sectionsOriginal) / (service.dimension);
                        service.totalAreaInHourOriginal = service.totalAreaOriginal / 60;
                        service.totalAreaInShiftOriginal = service.totalAreaInHourOriginal / 8;
                        service.isDiameterVisible = true;
                        service.totalCostOriginal = service.totalAreaInShiftOriginal * service.shiftPrice;
                        service.totalCostPerSquareOriginal = service.totalCostOriginal / service.area;
                        calculatingMaintainanceCostForOriginal();
                        ChangingZeroToNull();
                    }
                    else if (vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                        || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                        || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                        || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                        ChangingNullToZeroOrginal();
                        service.sectionsOriginal = service.area / 10;
                        service.sectionsOriginalDivision = 10;
                        service.produtivityRateOriginal = service.ugcOriginal + service.thirtyOriginal + service.fiftyOriginal + service.hundredOriginal + service.twoHundredOriginal +
                            service.fourHundredOriginal + service.eightHundredOriginal + service.fifteenHundredOriginal + service.threeThousandOriginal + service.eightThousandOriginal + service.elevenThousandOriginal + service.cacoOriginal + service.ihgOriginal;

                        service.totalMinuteserviceOriginal = 50;
                        service.totalMinuteOriginal = "50%";
                        service.seventeenBaseOriginal = (service.produtivityRate / 50) * 100;
                        service.totalAreaOriginal = (service.seventeenBaseOriginal * service.sectionsOriginal) / (service.dimension);
                        service.totalAreaInHourOriginal = service.totalAreaOriginal / 60;
                        service.totalAreaInShiftOriginal = service.totalAreaInHourOriginal / 8;
                        service.totalCostOriginal = service.totalAreaInShiftOriginal * service.shiftPrice;
                        service.totalCostPerSquareOriginal = service.totalCostOriginal / service.area;
                        service.isDiameterVisible = true;
                        calculatingMaintainanceCostForOriginal();
                        ChangingZeroToNull();
                    }
                    else if (vm.service.serviceName == "Hone" && vm.service.typeOfStoneType2 == "Terrazzo") {
                        ChangingNullToZeroOrginal();
                        service.sectionsOriginal = service.area / 10;
                        service.sectionsOriginalDivision = 10;
                        service.produtivityRateOriginal = service.ugcOriginal + service.thirtyOriginal + service.fiftyOriginal + service.hundredOriginal + service.twoHundredOriginal +
                            service.fourHundredOriginal + service.eightHundredOriginal + service.fifteenHundredOriginal + service.threeThousandOriginal + service.eightThousandOriginal + service.elevenThousandOriginal + service.cacoOriginal + service.ihgOriginal;

                        service.totalMinuteserviceOriginal = 65;
                        service.totalMinuteOriginal = "65%";
                        service.seventeenBaseOriginal = (service.produtivityRateOriginal / 65) * 100;
                        service.totalAreaOriginal = (service.seventeenBaseOriginal * service.sections) / (service.dimension);
                        service.totalAreaInHourOriginal = service.totalAreaOriginal / 60;
                        service.totalAreaInShiftOriginal = service.totalAreaInHourOriginal / 8;
                        service.totalCostOriginal = service.totalAreaInShiftOriginal * service.shiftPrice;
                        service.totalCostPerSquareOriginal = service.totalCostOriginal / service.area;
                        service.isDiameterVisible = false;
                        calculatingMaintainanceCostForOriginal();
                        ChangingZeroToNull();
                    }
                    else if (vm.service.serviceName == "Hone" && (vm.service.typeOfStoneType2 == "Carpet" || vm.service.typeOfStoneType2 == "Ceramic" ||
                        vm.service.typeOfStoneType2 == "Concrete" || vm.service.typeOfStoneType2 == "Corian" || vm.service.typeOfStoneType2 == "Glass" ||
                        vm.service.typeOfStoneType2 == "Limestone" || vm.service.typeOfStoneType2 == "Metal" || vm.service.typeOfStoneType2 == "Porcelain" ||
                        vm.service.typeOfStoneType2 == "Quartz" || vm.service.typeOfStoneType2 == "Slate" || vm.service.typeOfStoneType2 == "Vinyl" ||
                        vm.service.typeOfStoneType2 == "Wood")) {
                        ChangingNullToZeroOrginal();
                        service.sectionsOriginal = service.area / 10;
                        service.sectionsOriginalDivision = 10;
                        service.produtivityRateOriginal = service.ugcOriginal + service.thirtyOriginal + service.fiftyOriginal + service.hundredOriginal + service.twoHundredOriginal +
                            service.fourHundredOriginal + service.eightHundredOriginal + service.fifteenHundredOriginal + service.threeThousandOriginal + service.eightThousandOriginal + service.elevenThousandOriginal + service.cacoOriginal + service.ihgOriginal;

                        service.totalMinuteserviceOriginal = 65;
                        service.totalMinuteOriginal = "65%";
                        service.seventeenBaseOriginal = (service.produtivityRateOriginal / 65) * 100;
                        service.totalAreaOriginal = (service.seventeenBaseOriginal * service.sections) / (service.dimension);
                        service.totalAreaInHourOriginal = service.totalAreaOriginal / 60;
                        service.totalAreaInShiftOriginal = service.totalAreaInHourOriginal / 8;
                        service.totalCostOriginal = service.totalAreaInShiftOriginal * service.shiftPrice;
                        service.totalCostPerSquareOriginal = service.totalCostOriginal / service.area;
                        service.isDiameterVisible = false;
                        calculatingMaintainanceCostForOriginal();
                        ChangingZeroToNull();
                    }
                    ChangingZeroToNullForOrginal();
                    if (service.area != "")
                    {
                        service.totalAreaHoverOriginal = "SeventeenBase * Sections / Diameter = " + (service.seventeenBaseOriginal.toFixed(2) + " * " + service.sections.toFixed(2)) + " / " + (service.dimension);
                        service.totalAreaInHourHoverOriginal = "Total Time(MIN) / 60 = " + service.totalAreaOriginal.toFixed(2) + " / " + 60;
                        service.totalAreaInShiftHoverOriginal = "Total Time(HR) / 8 = " + service.totalAreaInHourOriginal.toFixed(2) + " / " + 8;
                        service.totalCostHoverOriginal = "Total Time(Shift) * ShiftPrice = " + service.totalAreaInShiftOriginal.toFixed(2) + " * " + service.shiftPrice.toFixed(2);
                        service.totalCostPerSquareHoverOriginal = "Total Cost / Area = " + service.totalCostOriginal.toFixed(2) + " / " + service.area.toFixed(2);
                    }
                    service.hasOriginalValues = true;
                }

                function getHoiningMeasurementOriginal() {
                    var honingMeasurementList = $filter('filter')(vm.honingMeasurementList, { hasOriginalValues: false }, true);
                    angular.forEach(honingMeasurementList, function (value, index) {
                        if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && vm.service.typeOfStoneType == "Ultimate Finish" && vm.service.typeOfSurface == "Floor") {
                            value.ugcOriginal = 20;
                            value.thirtyOriginal = 10;
                            value.fiftyOriginal = 5;
                            value.hundredOriginal = 5;
                            value.twoHundredOriginal = 5;
                            value.fourHundredOriginal = 5;
                            value.eightHundredOriginal = 5;
                            value.fifteenHundredOriginal = 0;
                            value.threeThousandOriginal = 0;
                            value.eightThousandOriginal = 0;
                            value.elevenThousandOriginal = 0;
                            value.cacoOriginal = 4;
                            value.ihgOriginal = 2;
                            value.sectionsOriginalDivision = 10;
                            value.sectionsOriginal = value.area / 10;
                            value.produtivityRateOriginal = value.ugcOriginal + value.thirtyOriginal + value.fiftyOriginal + value.hundredOriginal + value.twoHundredOriginal +
                                value.fourHundredOriginal + value.eightHundredOriginal + value.fifteenHundredOriginal + value.threeThousandOriginal + value.eightThousandOriginal + value.elevenThousandOriginal + value.cacoOriginal + value.ihgOriginal;
                            value.totalMinuteOriginal = "65%";
                            value.totalMinuteValueOriginal = 65;
                            value.seventeenBaseOriginal = (value.produtivityRateOriginal / 65) * 100;
                            value.totalAreaOriginal = (value.seventeenBaseOriginal * value.sectionsOriginal) / (value.dimension);
                            value.totalAreaInHourOriginal = value.totalAreaOriginal / 60;
                            value.totalAreaInShiftOriginal = value.totalAreaInHourOriginal / 8;
                            value.totalCostOriginal = value.totalAreaInShiftOriginal * value.shiftPrice;
                            value.totalCostPerSquareOriginal = value.totalCost / value.area;
                            value.isDiameterVisible = false;
                            calculatingMaintainanceCostForOriginal();
                            ChangingZeroToNull();
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && vm.service.typeOfStoneType == "Gloss" && vm.service.typeOfSurface == "Floor") {
                            value.ugcOriginal = 0;
                            value.thirtyOriginal = 0;
                            value.fiftyOriginal = 0;
                            value.hundredOriginal = 0;
                            value.twoHundredOriginal = 5;
                            value.fourHundredOriginal = 5;
                            value.eightHundredOriginal = 5;
                            value.fifteenHundredOriginal = 0;
                            value.threeThousandOriginal = 0;
                            value.eightThousandOriginal = 0;
                            value.elevenThousandOriginal = 0;
                            value.cacoOriginal = 4;
                            value.ihgOriginal = 2;
                            value.sectionsOriginal = value.area / 10;
                            value.sectionsOriginalDivision = 10;
                            value.produtivityRateOriginal = value.ugcOriginal + value.thirtyOriginal + value.fiftyOriginal + value.hundredOriginal + value.twoHundredOriginal +
                                value.fourHundredOriginal + value.eightHundredOriginal + value.fifteenHundredOriginal + value.threeThousandOriginal + value.eightThousandOriginal + value.elevenThousandOriginal + value.cacoOriginal + value.ihgOriginal;

                            value.totalMinuteValueOriginal = 65;
                            value.totalMinuteOriginal = "65%";
                            value.seventeenBaseOriginal = (value.produtivityRateOriginal / 65) * 100;
                            value.totalAreaOriginal = (value.seventeenBaseOriginal * value.sections) / (value.dimension);
                            value.totalAreaInHourOriginal = value.totalAreaOriginal / 60;
                            value.totalAreaInShiftOriginal = value.totalAreaInHourOriginal / 8;
                            value.totalCostOriginal = value.totalAreaInShiftOriginal * value.shiftPrice;
                            value.totalCostPerSquareOriginal = value.totalCostOriginal / value.area;
                            value.isDiameterVisible = false;
                            calculatingMaintainanceCostForOriginal();
                            ChangingZeroToNull();
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneType == "Satin" || vm.service.typeOfStoneType == "Semi-Gloss") && vm.service.typeOfSurface == "Floor") {
                            value.ugcOriginal = 0;
                            value.thirtyOriginal = 0;
                            value.fiftyOriginal = 0;
                            value.hundredOriginal = 0;
                            value.twoHundredOriginal = 5;
                            value.fourHundredOriginal = 5;
                            value.eightHundredOriginal = 5;
                            value.fifteenHundredOriginal = 0;
                            value.threeThousandOriginal = 0;
                            value.eightThousandOriginal = 0;
                            value.elevenThousandOriginal = 0;
                            value.cacoOriginal = 0;
                            value.ihgOriginal = 2;
                            value.sectionsOriginal = value.area / 10;
                            value.sectionsOriginalDivision = 10;
                            value.produtivityRateOriginal = value.ugcOriginal + value.thirtyOriginal + value.fiftyOriginal + value.hundredOriginal + value.twoHundredOriginal +
                                value.fourHundredOriginal + value.eightHundredOriginal + value.fifteenHundredOriginal + value.threeThousandOriginal + value.eightThousandOriginal + value.elevenThousandOriginal + value.cacoOriginal + value.ihgOriginal;

                            value.totalMinuteValueOriginal = 65;
                            value.totalMinuteOriginal = "65%";
                            value.totalAreaOriginal = (value.seventeenBaseOriginal * value.sections) / (value.dimension);
                            value.totalAreaInHourOriginal = value.totalAreaOriginal / 60;
                            value.totalAreaInShiftOriginal = value.totalAreaInHourOriginal / 8;
                            value.totalCostOriginal = value.totalAreaInShiftOriginal * value.shiftPrice;
                            value.totalCostPerSquareOriginal = value.totalCostOriginal / value.area;
                            value.isDiameterVisible = false;
                            calculatingMaintainanceCostForOriginal();
                            ChangingZeroToNull();
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneType == "Matte") && vm.service.typeOfSurface == "Floor") {
                            value.ugcOriginal = 0;
                            value.thirtyOriginal = 0;
                            value.fiftyOriginal = 0;
                            value.hundredOriginal = 0;
                            value.twoHundredOriginal = 5;
                            value.fourHundredOriginal = 5;
                            value.eightHundredOriginal = 5;
                            value.fifteenHundredOriginal = 0;
                            value.threeThousandOriginal = 0;
                            value.eightThousandOriginal = 0;
                            value.elevenThousandOriginal = 0;
                            value.cacoOriginal = 0;
                            value.ihgOriginal = 0;
                            value.sectionsOriginal = value.area / 10;
                            value.sectionsOriginalDivision = 10;
                            value.produtivityRateOriginal = value.ugcOriginal + value.thirtyOriginal + value.fiftyOriginal + value.hundredOriginal + value.twoHundredOriginal +
                                value.fourHundredOriginal + value.eightHundredOriginal + value.fifteenHundredOriginal + value.threeThousandOriginal + value.eightThousandOriginal + value.elevenThousandOriginal + value.cacoOriginal + value.ihgOriginal;
                            value.totalMinuteValueOriginal = 65;
                            value.totalMinuteOriginal = "65%";
                            value.seventeenBaseOriginal = (value.produtivityRateOriginal / 65) * 100;
                            value.totalAreaOriginal = (value.seventeenBaseOriginal * value.sections) / (value.dimension);
                            value.totalAreaInHourOriginal = value.totalAreaOriginal / 60;
                            value.totalAreaInShiftOriginal = value.totalAreaInHourOriginal / 8;
                            value.totalCostOriginal = value.totalAreaInShiftOriginal * value.shiftPrice;
                            value.totalCostPerSquareOriginal = value.totalCostOriginal / value.area;
                            value.isDiameterVisible = false;
                            calculatingMaintainanceCostForOriginal();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && vm.service.typeOfSurface == "Floor") {
                            value.ugcOriginal = 0;
                            value.thirtyOriginal = 0;
                            value.fiftyOriginal = 0;
                            value.hundredOriginal = 0;
                            value.twoHundredOriginal = 0;
                            value.fourHundredOriginal = 0;
                            value.eightHundredOriginal = 20;
                            value.fifteenHundredOriginal = 10;
                            value.threeThousandOriginal = 10;
                            value.eightThousandOriginal = 0;
                            value.elevenThousandOriginal = 0;
                            value.cacoOriginal = 10;
                            value.ihgOriginal = 2;
                            value.sectionsOriginal = value.area / 10;
                            value.sectionsOriginalDivision = 10;
                            value.produtivityRateOriginal = value.ugcOriginal + value.thirtyOriginal + value.fiftyOriginal + value.hundredOriginal + value.twoHundredOriginal +
                                value.fourHundredOriginal + value.eightHundredOriginal + value.fifteenHundredOriginal + value.threeThousandOriginal + value.eightThousandOriginal + value.elevenThousandOriginal + value.cacoOriginal + value.ihgOriginal;

                            value.totalMinuteValueOriginal = 65;
                            value.totalMinuteOriginal = "65%";
                            value.seventeenBaseOriginal = (value.produtivityRateOriginal / 65) * 100;
                            value.totalAreaOriginal = (value.seventeenBaseOriginal * value.sectionsOriginal) / (value.dimension);
                            value.totalAreaInHourOriginal = value.totalAreaOriginal / 60;
                            value.totalAreaInShiftOriginal = value.totalAreaInHour / 8
                            value.isDiameterVisible = false;
                            value.totalCostOriginal = value.totalAreaInShiftOriginal * value.shiftPrice;
                            value.totalCostPerSquareOriginal = value.totalCostOriginal / value.area;
                            calculatingMaintainanceCostForOriginal();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.serviceName == "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && vm.service.typeOfSurface == "Floor") {
                            value.ugcOriginal = 0;
                            value.thirtyOriginal = 0;
                            value.fiftyOriginal = 0;
                            value.hundredOriginal = 0;
                            value.twoHundredOriginal = 4;
                            value.fourHundredOriginal = 0;
                            value.eightHundredOriginal = 0;
                            value.fifteenHundredOriginal = 0;
                            value.threeThousandOriginal = 0;
                            value.eightThousandOriginal = 0;
                            value.elevenThousandOriginal = 0;
                            value.cacoOriginal = 0;
                            value.ihgOriginal = 0;
                            value.sectionsOriginal = value.area / 10;
                            value.sectionsOriginalDivision = 10;
                            value.produtivityRateOriginal = value.ugcOriginal + value.thirtyOriginal + value.fiftyOriginal + value.hundredOriginal + value.twoHundredOriginal +
                                value.fourHundredOriginal + value.eightHundredOriginal + value.fifteenHundredOriginal + value.threeThousandOriginal + value.eightThousandOriginal + value.elevenThousandOriginal + value.cacoOriginal + value.ihgOriginal;
                            value.totalMinuteValueOriginal = 65;
                            value.totalMinuteOriginal = "65%";
                            value.seventeenBaseOriginal = (value.produtivityRateOriginal / 65) * 100;
                            value.totalAreaOriginal = (value.seventeenBaseOriginal * value.sections) / (value.dimension);
                            value.totalAreaInHourOriginal = value.totalArea / 60;
                            value.totalAreaInShiftOriginal = value.totalAreaInHourOriginal / 8;
                            value.isDiameterVisible = false;
                            value.totalCostOriginal = value.totalAreaInShiftOriginal * value.shiftPrice;
                            value.totalCostPerSquareOriginal = value.totalCostOriginal / value.area;
                            calculatingMaintainanceCostForOriginal();
                            ChangingZeroToNull();
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                            value.ugcOriginal = 0;
                            value.thirtyOriginal = 0;
                            value.fiftyOriginal = 0;
                            value.hundredOriginal = 0;
                            value.twoHundredOriginal = 5;
                            value.fourHundredOriginal = 5;
                            value.eightHundredOriginal = 5;
                            value.fifteenHundredOriginal = 0;
                            value.threeThousandOriginal = 0;
                            value.eightThousandOriginal = 0;
                            value.elevenThousandOriginal = 0;
                            value.cacoOriginal = 4;
                            value.ihgOriginal = 2;
                            value.areaOriginal = 0;
                            value.sectionsOriginal = value.area / 5;
                            value.sectionsOriginalDivision = 5;
                            value.produtivityRateOriginal = value.ugcOriginal + value.thirtyOriginal + value.fiftyOriginal + value.hundredOriginal + value.twoHundredOriginal +
                                value.fourHundredOriginal + value.eightHundredOriginal + value.fifteenHundredOriginal + value.threeThousandOriginal + value.eightThousandOriginal + value.elevenThousandOriginal + value.cacoOriginal + value.ihgOriginal;

                            value.totalMinuteValueOriginal = 50;
                            value.totalMinuteOriginal = "50%";
                            value.seventeenBaseOriginal = (value.produtivityRateOriginal / 50) * 100;
                            value.totalAreaOriginal = (value.seventeenBaseOriginal * value.sectionsOriginal) / (value.dimension);
                            value.totalAreaInHourOriginal = value.totalAreaOriginal / 60;
                            value.totalAreaInShiftOriginal = value.totalAreaInHourOriginal / 8;
                            value.isDiameterVisible = true;
                            value.totalCostOriginal = value.totalAreaInShiftOriginal * value.shiftPrice;
                            value.totalCostPerSquareOriginal = value.totalCostOriginal / value.area;
                            calculatingMaintainanceCostForOriginal();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Concrete" && vm.service.typeOfSurface == "Floor" && vm.service.serviceName != "Concrete Floor Prep - Grind") {
                            value.ugcOriginal = 0;
                            value.thirtyOriginal = 4;
                            value.fiftyOriginal = 4;
                            value.hundredOriginal = 4;
                            value.twoHundredOriginal = 4;
                            value.fourHundredOriginal = 4;
                            value.eightHundredOriginal = 4;
                            value.fifteenHundredOriginal = 4;
                            value.threeThousandOriginal = 4;
                            value.eightThousandOriginal = 0;
                            value.elevenThousandOriginal = 0;
                            value.cacoOriginal = 0;
                            value.ihgOriginal = 2;
                            value.sectionsOriginal = value.area / 10;
                            value.sectionsOriginalDivision = 10;
                            value.produtivityRateOriginal = value.ugcOriginal + value.thirtyOriginal + value.fiftyOriginal + value.hundredOriginal + value.twoHundredOriginal +
                                value.fourHundredOriginal + value.eightHundredOriginal + value.fifteenHundredOriginal + value.threeThousandOriginal + value.eightThousandOriginal + value.elevenThousandOriginal + value.cacoOriginal + value.ihgOriginal;
                            value.totalMinuteValueOriginal = 65;
                            value.totalMinuteOriginal = "65%";
                            value.seventeenBaseOriginal = (value.produtivityRateOriginal / 65) * 100;
                            value.totalAreaOriginal = (value.seventeenBaseOriginal * value.sections) / (value.dimension);
                            value.totalAreaInHourOriginal = value.totalArea / 60;
                            value.totalAreaInShiftOriginal = value.totalAreaInHourOriginal / 8;
                            value.isDiameterVisible = false;
                            value.totalCostOriginal = value.totalAreaInShiftOriginal * value.shiftPrice;
                            value.totalCostPerSquareOriginal = value.totalCostOriginal / value.area;
                            calculatingMaintainanceCostForOriginal();
                            ChangingZeroToNull();
                        }


                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                            value.ugcOriginal = 0;
                            value.thirtyOriginal = 0;
                            value.fiftyOriginal = 0;
                            value.hundredOriginal = 0;
                            value.twoHundredOriginal = 0;
                            value.fourHundredOriginal = 20;
                            value.eightHundredOriginal = 10;
                            value.fifteenHundredOriginal = 10;
                            value.threeThousandOriginal = 0;
                            value.eightThousandOriginal = 0;
                            value.elevenThousandOriginal = 0;
                            value.cacoOriginal = 10;
                            value.ihgOriginal = 2;
                            value.areaOriginal = 0;
                            value.sectionsOriginal = value.area / 5;
                            value.sectionsOriginalDivision = 5;
                            value.produtivityRateOriginal = value.ugcOriginal + value.thirtyOriginal + value.fiftyOriginal + value.hundredOriginal + value.twoHundredOriginal +
                                value.fourHundredOriginal + value.eightHundredOriginal + value.fifteenHundredOriginal + value.threeThousandOriginal + value.eightThousandOriginal + value.elevenThousandOriginal + value.cacoOriginal + value.ihgOriginal;

                            value.totalMinuteValueOriginal = 50;
                            value.totalMinuteOriginal = "50%";
                            value.seventeenBaseOriginal = (value.produtivityRate / 50) * 100;
                            value.totalAreaOriginal = (value.seventeenBaseOriginal * value.sectionsOriginal) / (value.dimension);
                            value.totalAreaInHourOriginal = value.totalAreaOriginal / 60;
                            value.totalAreaInShiftOriginal = value.totalAreaInHourOriginal / 8;
                            value.isDiameterVisible = true;
                            value.totalCostOriginal = value.totalAreaInShiftOriginal * value.shiftPrice;
                            value.totalCostPerSquareOriginal = value.totalCostOriginal / value.area;
                            calculatingMaintainanceCostForOriginal();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                            value.sectionsOriginal = value.area / 10;
                            value.ugcOriginal = 0;
                            value.thirtyOriginal = 4;
                            value.fiftyOriginal = 4;
                            value.hundredOriginal = 4;
                            value.twoHundredOriginal = 4;
                            value.fourHundredOriginal = 4;
                            value.eightHundredOriginal = 4;
                            value.fifteenHundredOriginal = 4;
                            value.threeThousandOriginal = 4;
                            value.eightThousandOriginal = 0;
                            value.elevenThousandOriginal = 0;
                            value.cacoOriginal = 0;
                            value.ihgOriginal = 2;
                            value.sectionsOriginal = value.area / 5;
                            value.sectionsOriginalDivision = 5;
                            value.produtivityRateOriginal = value.ugcOriginal + value.thirtyOriginal + value.fiftyOriginal + value.hundredOriginal + value.twoHundredOriginal +
                                value.fourHundredOriginal + value.eightHundredOriginal + value.fifteenHundredOriginal + value.threeThousandOriginal + value.eightThousandOriginal + value.elevenThousandOriginal + value.cacoOriginal + value.ihgOriginal;

                            value.totalMinuteValueOriginal = 50;
                            value.totalMinuteOriginal = "50%";
                            value.seventeenBaseOriginal = (value.produtivityRate / 50) * 100;
                            value.totalAreaOriginal = (value.seventeenBaseOriginal * value.sectionsOriginal) / (value.dimension);
                            value.totalAreaInHourOriginal = value.totalAreaOriginal / 60;
                            value.totalAreaInShiftOriginal = value.totalAreaInHourOriginal / 8;
                            value.totalCostOriginal = value.totalAreaInShiftOriginal * value.shiftPrice;
                            value.totalCostPerSquareOriginal = value.totalCostOriginal / value.area;
                            value.isDiameterVisible = true;
                            calculatingMaintainanceCostForOriginal();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.serviceName == "Hone" && vm.service.typeOfStoneType2 == "Terrazzo") {
                            value.ugcOriginal = 0;
                            value.thirtyOriginal = 0;
                            value.fiftyOriginal = 0;
                            value.hundredOriginal = 0;
                            value.twoHundredOriginal = 5;
                            value.fourHundredOriginal = 5;
                            value.eightHundredOriginal = 5;
                            value.fifteenHundredOriginal = 0;
                            value.threeThousandOriginal = 0;
                            value.eightThousandOriginal = 0;
                            value.elevenThousandOriginal = 0;
                            value.cacoOriginal = 4;
                            value.ihgOriginal = 2;
                            value.sectionsOriginal = value.area / 10;
                            value.sectionsOriginalDivision = 10;
                            value.produtivityRateOriginal = value.ugcOriginal + value.thirtyOriginal + value.fiftyOriginal + value.hundredOriginal + value.twoHundredOriginal +
                                value.fourHundredOriginal + value.eightHundredOriginal + value.fifteenHundredOriginal + value.threeThousandOriginal + value.eightThousandOriginal + value.elevenThousandOriginal + value.cacoOriginal + value.ihgOriginal;

                            value.totalMinuteValueOriginal = 65;
                            value.totalMinuteOriginal = "65%";
                            value.seventeenBaseOriginal = (value.produtivityRateOriginal / 65) * 100;
                            value.totalAreaOriginal = (value.seventeenBaseOriginal * value.sections) / (value.dimension);
                            value.totalAreaInHourOriginal = value.totalAreaOriginal / 60;
                            value.totalAreaInShiftOriginal = value.totalAreaInHourOriginal / 8;
                            value.totalCostOriginal = value.totalAreaInShiftOriginal * value.shiftPrice;
                            value.totalCostPerSquareOriginal = value.totalCostOriginal / value.area;
                            value.isDiameterVisible = false;
                            calculatingMaintainanceCostForOriginal();
                            ChangingZeroToNull();
                        }
                        else if (vm.service.serviceName == "Hone" && (vm.service.typeOfStoneType2 == "Carpet" || vm.service.typeOfStoneType2 == "Ceramic" ||
                            vm.service.typeOfStoneType2 == "Concrete" || vm.service.typeOfStoneType2 == "Corian" || vm.service.typeOfStoneType2 == "Glass" ||
                            vm.service.typeOfStoneType2 == "Limestone" || vm.service.typeOfStoneType2 == "Metal" || vm.service.typeOfStoneType2 == "Porcelain" ||
                            vm.service.typeOfStoneType2 == "Quartz" || vm.service.typeOfStoneType2 == "Slate" || vm.service.typeOfStoneType2 == "Vinyl" ||
                            vm.service.typeOfStoneType2 == "Wood")) {
                            value.ugcOriginal = 0;
                            value.thirtyOriginal = 0;
                            value.fiftyOriginal = 0;
                            value.hundredOriginal = 0;
                            value.twoHundredOriginal = 5;
                            value.fourHundredOriginal = 5;
                            value.eightHundredOriginal = 5;
                            value.fifteenHundredOriginal = 0;
                            value.threeThousandOriginal = 0;
                            value.eightThousandOriginal = 0;
                            value.elevenThousandOriginal = 0;
                            value.cacoOriginal = 0;
                            value.ihgOriginal = 0;
                            value.sectionsOriginal = value.area / 10;
                            value.sectionsOriginalDivision = 10;
                            value.produtivityRateOriginal = value.ugcOriginal + value.thirtyOriginal + value.fiftyOriginal + value.hundredOriginal + value.twoHundredOriginal +
                                value.fourHundredOriginal + value.eightHundredOriginal + value.fifteenHundredOriginal + value.threeThousandOriginal + value.eightThousandOriginal + value.elevenThousandOriginal + value.cacoOriginal + value.ihgOriginal;

                            value.totalMinuteValueOriginal = 65;
                            value.totalMinuteOriginal = "65%";
                            value.seventeenBaseOriginal = (value.produtivityRateOriginal / 65) * 100;
                            value.totalAreaOriginal = (value.seventeenBaseOriginal * value.sections) / (value.dimension);
                            value.totalAreaInHourOriginal = value.totalAreaOriginal / 60;
                            value.totalAreaInShiftOriginal = value.totalAreaInHourOriginal / 8;
                            value.totalCostOriginal = value.totalAreaInShiftOriginal * value.shiftPrice;
                            value.totalCostPerSquareOriginal = value.totalCostOriginal / value.area;
                            value.isDiameterVisible = false;
                            calculatingMaintainanceCostForOriginal();
                            ChangingZeroToNull();
                        }
                        if (vm.service.serviceName == "Honing" && vm.service.typeOfStoneType2 == "Granite") {
                            value.ugcOriginal = 0;
                            value.thirtyOriginal = 0;
                            value.fiftyOriginal = 0;
                            value.hundredOriginal = 0;
                            value.twoHundredOriginal = 0;
                            value.fourHundredOriginal = 0;
                            value.eightHundredOriginal = 20;
                            value.fifteenHundredOriginal = 10;
                            value.threeThousandOriginal = 10;
                            value.eightThousandOriginal = 0;
                            value.elevenThousandOriginal = 0;
                            value.cacoOriginal = 0;
                            value.ihgOriginal = 0;
                            value.sectionsOriginal = value.area / 10;
                            value.sectionsOriginalDivision = 10;
                            value.produtivityRateOriginal = value.ugcOriginal + value.thirtyOriginal + value.fiftyOriginal + value.hundredOriginal + value.twoHundredOriginal + value.fourHundredOriginal + value.eightHundredOriginal + value.fifteenHundredOriginal + value.threeThousandOriginal + value.eightThousandOriginal + value.elevenThousandOriginal + value.cacoOriginal + value.ihgOriginal;

                            value.totalMinuteValueOriginal = 65;
                            value.totalMinuteOriginal = "65%";
                            value.seventeenBaseOriginal = (value.produtivityRateOriginal / 65) * 100;
                            value.totalAreaOriginal = (value.seventeenBaseOriginal * value.sectionsOriginal) / (value.dimension);
                            value.totalAreaInHourOriginal = value.totalAreaOriginal / 60;
                            value.totalAreaInShiftOriginal = value.totalAreaInHour / 8
                            value.isDiameterVisible = false;
                            value.totalCostOriginal = value.totalAreaInShiftOriginal * value.shiftPrice;
                            value.totalCostPerSquareOriginal = value.totalCostOriginal / value.area;
                            calculatingMaintainanceCostForOriginal();
                            ChangingZeroToNull();
                        }
                        calculationAccordingToColorOriginal();
                    })
                }

                function calculatingMaintainanceCostForOriginal() {
                    angular.forEach(vm.honingMeasurementList, function (value, index) {
                        if (vm.maintenanceChargesListCurrent != null && vm.maintenanceChargesListCurrent.length > 0) {
                            value.highDivisionOriginal = Math.round((value.area / vm.maintenanceChargesListCurrent[0].high));
                            value.lowDivisionOriginal = Math.round((value.area / vm.maintenanceChargesListCurrent[0].low));
                            value.averageNightRequiredOriginal = (value.highDivisionOriginal + value.lowDivisionOriginal) / 2;
                            value.averageNightPerMonthOriginal = value.averageNightRequiredOriginal * vm.franchiseeShiftPrice.maintainanceTechNightShiftPrice;
                            value.averageNightPerMonthPerSqFTOriginal = value.averageNightPerMonthOriginal / value.area;
                            value.annualMaintainceCostOriginal = (value.averageNightPerMonthOriginal * 12);
                            value.minRestorationValueOriginal = (value.annualMaintainceCostOriginal / (1 - 0.15));
                            value.highValueOriginal = vm.maintenanceChargesListCurrent[0].high;
                            value.materialForFormulaOriginal = vm.maintenanceChargesListCurrent[0].material;
                            value.lowValueOriginal = vm.maintenanceChargesListCurrent[0].low;
                            addingArea();
                        }
                    })
                }

                function clearanceValue() {
                    angular.forEach(vm.honingMeasurementList, function (value, index) {
                        ChangingNullToZeroClearance();
                        value.dimensionClearance = 1;
                        if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && vm.service.typeOfStoneType == "Ultimate Finish" && vm.service.typeOfSurface == "Floor") {
                            value.ugcClearance = 0;
                            value.thirtyClearance = 0;
                            value.fiftyClearance = 0;
                            value.hundredClearance = 0;
                            value.twoHundredClearance = 0;
                            value.fourHundredClearance = 5;
                            value.eightHundredClearance = 0;
                            value.fifteenHundredClearance = 0;
                            value.threeThousandClearance = 0;
                            value.eightThousandClearance = 0;
                            value.elevenThousandClearance = 0;
                            value.cacoClearance = 4;
                            value.ihgClearance = 2;
                            value.sectionsClearance = (value.area / 10);
                            value.produtivityRateClearance = value.ugcClearance + value.thirtyClearance + value.fiftyClearance + value.hundredClearance + value.twoHundredClearance +
                                value.fourHundredClearance + value.eightHundredClearance + value.fifteenHundredClearance + value.threeThousandClearance + value.eightThousandClearance + value.elevenThousandClearance + value.cacoClearance + value.ihgClearance;

                            value.totalMinuteClearance = "65%";
                            value.seventeenBaseClearance = (value.produtivityRateClearance / 65) * 100;
                            value.totalAreaClearance = (value.seventeenBaseClearance * value.sectionsClearance) / (value.dimensionClearance);
                            value.totalAreaInHourClearance = value.totalAreaClearance / 60;
                            value.totalAreaInShiftClearance = value.totalAreaInHourClearance / 8;
                            value.totalCostClearance = value.totalAreaInShiftClearance * value.shiftPrice;
                            value.totalCostPerSquareClearance = value.totalCost / value.area;
                            value.totalCostClearance = value.totalCostPerSquareClearance * value.shiftPrice;
                            value.totalCostPerSquareClearance = value.totalCostClearance / value.area;
                            value.isDiameterVisible = false;
                            calculatingMaintainanceCostForClearance();
                            ChangingZeroToNullForClearance();
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && vm.service.typeOfStoneType == "Gloss" && vm.service.typeOfSurface == "Floor") {
                            value.ugcClearance = 0;
                            value.thirtyClearance = 0;
                            value.fiftyClearance = 0;
                            value.hundredClearance = 0;
                            value.twoHundredClearance = 0;
                            value.fourHundredClearance = 5;
                            value.eightHundredClearance = 0;
                            value.fifteenHundredClearance = 0;
                            value.threeThousandClearance = 0;
                            value.eightThousandClearance = 0;
                            value.elevenThousandClearance = 0;
                            value.cacoClearance = 4;
                            value.ihgClearance = 2;
                            value.sectionsClearance = (value.area / 10);
                            value.produtivityRateClearance = value.ugcClearance + value.thirtyClearance + value.fiftyClearance + value.hundredClearance + value.twoHundredClearance +
                                value.fourHundredClearance + value.eightHundredClearance + value.fifteenHundredClearance + value.threeThousandClearance + value.eightThousandClearance + value.elevenThousandClearance + value.cacoClearance + value.ihgClearance;

                            value.totalMinuteClearance = "65%";
                            value.seventeenBaseClearance = (value.produtivityRateClearance / 65) * 100;
                            value.totalAreaClearance = (value.seventeenBaseClearance * value.sectionsClearance) / (value.dimensionClearance);
                            value.totalAreaInHourClearance = value.totalAreaClearance / 60;
                            value.totalAreaInShiftClearance = value.totalAreaInHourClearance / 8;
                            value.totalCostClearance = value.totalAreaInShiftClearance * value.shiftPrice;
                            value.totalCostPerSquareClearance = value.totalCost / value.area;
                            value.isDiameterVisible = false;
                            calculatingMaintainanceCostForClearance();
                            ChangingZeroToNullForClearance();

                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneType == "Satin" || vm.service.typeOfStoneType == "Semi-Gloss") && vm.service.typeOfSurface == "Floor") {
                            value.ugcClearance = 0;
                            value.thirtyClearance = 0;
                            value.fiftyClearance = 0;
                            value.hundredClearance = 0;
                            value.twoHundredClearance = 0;
                            value.fourHundredClearance = 0;
                            value.eightHundredClearance = 5;
                            value.fifteenHundredClearance = 0;
                            value.threeThousandClearance = 0;
                            value.eightThousandClearance = 0;
                            value.elevenThousandClearance = 0;
                            value.cacoClearance = 4;
                            value.ihgClearance = 2;
                            value.sectionsClearance = (value.area / 10);
                            value.produtivityRateClearance = value.ugcClearance + value.thirtyClearance + value.fiftyClearance + value.hundredClearance + value.twoHundredClearance +
                                value.fourHundredClearance + value.eightHundredClearance + value.fifteenHundredClearance + value.threeThousandClearance + value.eightThousandClearance + value.elevenThousandClearance + value.cacoClearance + value.ihgClearance;
                            value.totalMinuteClearance = "65%";
                            value.seventeenBaseClearance = (value.produtivityRateClearance / 65) * 100;
                            value.totalAreaClearance = (value.seventeenBaseClearance * value.sectionsClearance) / (value.dimensionClearance);
                            value.totalAreaInHourClearance = value.totalAreaClearance / 60;
                            value.totalAreaInShiftClearance = value.totalAreaInHourClearance / 8;
                            value.totalCostClearance = value.totalAreaInShiftClearance * value.shiftPrice;
                            value.totalCostPerSquareClearance = value.totalCostClearance / value.area;
                            value.isDiameterVisible = false;
                            calculatingMaintainanceCostForClearance();
                            ChangingZeroToNullForClearance();
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneType == "Matte") && vm.service.typeOfSurface == "Floor") {
                            value.ugcClearance = 0;
                            value.thirtyClearance = 0;
                            value.fiftyClearance = 0;
                            value.hundredClearance = 0;
                            value.twoHundredClearance = 0;
                            value.fourHundredClearance = 5;
                            value.eightHundredClearance = 0;
                            value.fifteenHundredClearance = 0;
                            value.threeThousandClearance = 0;
                            value.eightThousandClearance = 0;
                            value.elevenThousandClearance = 0;
                            value.cacoClearance = 4;
                            value.ihgClearance = 2;
                            value.sectionsClearance = (value.area / 10);
                            value.produtivityRateClearance = value.ugcClearance + value.thirtyClearance + value.fiftyClearance + value.hundredClearance + value.twoHundredClearance +
                                value.fourHundredClearance + value.eightHundredClearance + value.fifteenHundredClearance + value.threeThousandClearance + value.eightThousandClearance + value.elevenThousandClearance + value.cacoClearance + value.ihgClearance;

                            value.totalMinuteClearance = "65%";
                            value.seventeenBaseClearance = (value.produtivityRateClearance / 65) * 100;
                            value.totalAreaClearance = (value.seventeenBaseClearance * value.sectionsClearance) / (value.dimensionClearance);
                            value.totalAreaInHourClearance = value.totalAreaClearance / 60;
                            value.totalAreaInShiftClearance = value.totalAreaInHourClearance / 8;
                            value.totalCostClearance = value.totalAreaInShiftClearance * value.shiftPrice;
                            value.totalCostPerSquareClearance = value.totalCostClearance / value.area;
                            value.totalCostClearance = value.totalCostPerSquareClearance * value.shiftPrice;
                            value.totalCostPerSquareClearance = value.totalCostClearance / value.area;
                            value.isDiameterVisible = false;
                            calculatingMaintainanceCostForClearance();
                            ChangingZeroToNullForClearance();
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && vm.service.typeOfSurface == "Floor") {
                            value.ugcClearance = 0;
                            value.thirtyClearance = 0;
                            value.fiftyClearance = 0;
                            value.hundredClearance = 0;
                            value.twoHundredClearance = 20;
                            value.fourHundredClearance = 20;
                            value.eightHundredClearance = 20;
                            value.fifteenHundredClearance = 10;
                            value.threeThousandClearance = 0;
                            value.eightThousandClearance = 0;
                            value.elevenThousandClearance = 0;
                            value.cacoClearance = 10;
                            value.ihgClearance = 2;
                            value.sectionsClearance = (value.area / 10);
                            value.produtivityRateClearance = value.ugcClearance + value.thirtyClearance + value.fiftyClearance + value.hundredClearance + value.twoHundredClearance +
                                value.fourHundredClearance + value.eightHundredClearance + value.fifteenHundredClearance + value.threeThousandClearance + value.eightThousandClearance + value.elevenThousandClearance + value.cacoClearance + value.ihgClearance;

                            value.totalMinuteClearance = "65%";
                            value.seventeenBaseClearance = (value.produtivityRateClearance / 65) * 100;
                            value.totalAreaClearance = (value.seventeenBaseClearance * value.sectionsClearance) / (value.dimensionClearance);
                            value.totalAreaInHourClearance = value.totalAreaClearance / 60;
                            value.totalAreaInShiftClearance = value.totalAreaInHour / 8
                            value.isDiameterVisible = false;
                            value.totalCostClearance = value.totalAreaInShiftClearance * value.shiftPrice;
                            value.totalCostPerSquareClearance = value.totalCostClearance / value.area;
                            calculatingMaintainanceCostForClearance();
                            ChangingZeroToNullForClearance();
                        }

                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                            value.ugcClearance = 0;
                            value.thirtyClearance = 0;
                            value.fiftyClearance = 0;
                            value.hundredClearance = 0;
                            value.twoHundredClearance = 0;
                            value.fourHundredClearance = 5;
                            value.eightHundredClearance = 0;
                            value.fifteenHundredClearance = 0;
                            value.threeThousandClearance = 0;
                            value.eightThousandClearance = 0;
                            value.elevenThousandClearance = 0;
                            value.cacoClearance = 4;
                            value.ihgClearance = 2;
                            value.areaClearance = 0;
                            if (vm.service.typeOfSurface == "Floor") {
                                value.sectionsClearance = (value.area / 10);
                            }
                            else {
                                value.sectionsClearance = (value.area / 5);
                            }
                            value.produtivityRateClearance = value.ugcClearance + value.thirtyClearance + value.fiftyClearance + value.hundredClearance + value.twoHundredClearance +
                                value.fourHundredClearance + value.eightHundredClearance + value.fifteenHundredClearance + value.threeThousandClearance + value.eightThousandClearance + value.elevenThousandClearance + value.cacoClearance + value.ihgClearance;
                            value.totalMinuteClearance = "50%";
                            value.seventeenBaseClearance = (value.produtivityRateClearance / 50) * 100;
                            value.totalAreaClearance = (value.seventeenBaseClearance * value.sectionsClearance) / (value.dimensionClearance);
                            value.totalAreaInHourClearance = value.totalAreaClearance / 60;
                            value.totalAreaInShiftClearance = value.totalAreaInHourClearance / 8;
                            value.isDiameterVisible = true;
                            value.totalCostClearance = value.totalAreaInShiftClearance * value.shiftPrice;
                            value.totalCostPerSquareClearance = value.totalCostClearance / value.area;
                            calculatingMaintainanceCostForClearance();
                            ChangingZeroToNullForClearance();
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                            value.ugcClearance = 0;
                            value.thirtyClearance = 0;
                            value.fiftyClearance = 0;
                            value.hundredClearance = 0;
                            value.twoHundredClearance = 20;
                            value.fourHundredClearance = 20;
                            value.eightHundredClearance = 20;
                            value.fifteenHundredClearance = 10;
                            value.threeThousandClearance = 0;
                            value.eightThousandClearance = 0;
                            value.elevenThousandClearance = 0;
                            value.cacoClearance = 10;
                            value.ihgClearance = 2;
                            value.areaClearance = 0;
                            if (vm.service.typeOfSurface == "Floor") {
                                value.sectionsClearance = (value.area / 10);
                            }
                            else {
                                value.sectionsClearance = (value.area / 5);
                            }
                            value.produtivityRateClearance = value.ugcClearance + value.thirtyClearance + value.fiftyClearance + value.hundredClearance + value.twoHundredClearance +
                                value.fourHundredClearance + value.eightHundredClearance + value.fifteenHundredClearance + value.threeThousandClearance + value.eightThousandClearance + value.elevenThousandClearance + value.cacoClearance + value.ihgClearance;

                            value.totalMinuteClearance = "50%";
                            value.seventeenBaseClearance = (value.produtivityRate / 50) * 100;
                            value.totalAreaClearance = (value.seventeenBaseClearance * value.sectionsClearance) / (value.dimensionClearance);
                            value.totalAreaInHourClearance = value.totalAreaClearance / 60;
                            value.totalAreaInShiftClearance = value.totalAreaInHourClearance / 8;
                            value.isDiameterVisible = true;
                            value.totalCostClearance = value.totalAreaInShiftClearance * value.shiftPrice;
                            value.totalCostPerSquareClearance = value.totalCostClearance / value.area;
                            calculatingMaintainanceCostForClearance();
                            ChangingZeroToNullForClearance();
                        }
                        else if(vm.service.serviceName == "Hone" && vm.service.typeOfStoneType2 == "Terrazzo"){
                            value.ugcClearance = 0;
                            value.thirtyClearance = 0;
                            value.fiftyClearance = 0;
                            value.hundredClearance = 0;
                            value.twoHundredClearance = 0;
                            value.fourHundredClearance = 5;
                            value.eightHundredClearance = 0;
                            value.fifteenHundredClearance = 0;
                            value.threeThousandClearance = 0;
                            value.eightThousandClearance = 0;
                            value.elevenThousandClearance = 0;
                            value.cacoClearance = 4;
                            value.ihgClearance = 2;
                            value.sectionsClearance = (value.area / 10);
                            value.produtivityRateClearance = value.ugcClearance + value.thirtyClearance + value.fiftyClearance + value.hundredClearance + value.twoHundredClearance +
                                value.fourHundredClearance + value.eightHundredClearance + value.fifteenHundredClearance + value.threeThousandClearance + value.eightThousandClearance + value.elevenThousandClearance + value.cacoClearance + value.ihgClearance;

                            value.totalMinuteClearance = "65%";
                            value.seventeenBaseClearance = (value.produtivityRateClearance / 65) * 100;
                            value.totalAreaClearance = (value.seventeenBaseClearance * value.sectionsClearance) / (value.dimensionClearance);
                            value.totalAreaInHourClearance = value.totalAreaClearance / 60;
                            value.totalAreaInShiftClearance = value.totalAreaInHourClearance / 8;
                            value.totalCostClearance = value.totalAreaInShiftClearance * value.shiftPrice;
                            value.totalCostPerSquareClearance = value.totalCost / value.area;
                            value.isDiameterVisible = false;
                            calculatingMaintainanceCostForClearance();
                            ChangingZeroToNullForClearance();
                        }
                        else if (vm.service.serviceName == "Hone" && (vm.service.typeOfStoneType2 == "Carpet" || vm.service.typeOfStoneType2 == "Ceramic" ||
                            vm.service.typeOfStoneType2 == "Concrete" || vm.service.typeOfStoneType2 == "Corian" || vm.service.typeOfStoneType2 == "Glass" ||
                            vm.service.typeOfStoneType2 == "Limestone" || vm.service.typeOfStoneType2 == "Metal" || vm.service.typeOfStoneType2 == "Porcelain" ||
                            vm.service.typeOfStoneType2 == "Quartz" || vm.service.typeOfStoneType2 == "Slate" || vm.service.typeOfStoneType2 == "Vinyl" ||
                            vm.service.typeOfStoneType2 == "Wood")) {
                            value.ugcClearance = 0;
                            value.thirtyClearance = 0;
                            value.fiftyClearance = 0;
                            value.hundredClearance = 0;
                            value.twoHundredClearance = 0;
                            value.fourHundredClearance = 5;
                            value.eightHundredClearance = 0;
                            value.fifteenHundredClearance = 0;
                            value.threeThousandClearance = 0;
                            value.eightThousandClearance = 0;
                            value.elevenThousandClearance = 0;
                            value.cacoClearance = 0;
                            value.ihgClearance = 0;
                            value.sectionsClearance = (value.area / 10);
                            value.produtivityRateClearance = value.ugcClearance + value.thirtyClearance + value.fiftyClearance + value.hundredClearance + value.twoHundredClearance +
                                value.fourHundredClearance + value.eightHundredClearance + value.fifteenHundredClearance + value.threeThousandClearance + value.eightThousandClearance + value.elevenThousandClearance + value.cacoClearance + value.ihgClearance;

                            value.totalMinuteClearance = "65%";
                            value.seventeenBaseClearance = (value.produtivityRateClearance / 65) * 100;
                            value.totalAreaClearance = (value.seventeenBaseClearance * value.sectionsClearance) / (value.dimensionClearance);
                            value.totalAreaInHourClearance = value.totalAreaClearance / 60;
                            value.totalAreaInShiftClearance = value.totalAreaInHourClearance / 8;
                            value.totalCostClearance = value.totalAreaInShiftClearance * value.shiftPrice;
                            value.totalCostPerSquareClearance = value.totalCost / value.area;
                            value.isDiameterVisible = false;
                            calculatingMaintainanceCostForClearance();
                            ChangingZeroToNullForClearance();
                        }
                        else {
                            value.ugcClearance = 0;
                            value.thirtyClearance = 0;
                            value.fiftyClearance = 0;
                            value.hundredClearance = 0;
                            value.twoHundredClearance = 0;
                            value.fourHundredClearance = 0;
                            value.eightHundredClearance = 0;
                            value.fifteenHundredClearance = 0;
                            value.threeThousandClearance = 0;
                            value.eightThousandClearance = 0;
                            value.elevenThousandClearance = 0;
                            value.cacoClearance = 0;
                            value.ihgClearance = 0;
                            value.areaClearance = 0;
                            if (vm.service.typeOfSurface == "Floor") {
                                value.sectionsClearance = (value.area / 10);
                            }
                            else {
                                value.sectionsClearance = (value.area / 5);
                            }
                            value.produtivityRateClearance = value.ugcClearance + value.thirtyClearance + value.fiftyClearance + value.hundredClearance + value.twoHundredClearance +
                                value.fourHundredClearance + value.eightHundredClearance + value.fifteenHundredClearance + value.threeThousandClearance + value.eightThousandClearance + value.elevenThousandClearance + value.cacoClearance + value.ihgClearance;
                            value.totalMinuteClearance = "50%";
                            value.seventeenBaseClearance = (value.produtivityRate / 50) * 100;
                            value.totalAreaClearance = (value.seventeenBaseClearance * value.sectionsClearance) / (value.dimensionClearance);
                            value.totalAreaInHourClearance = value.totalAreaClearance / 60;
                            value.totalAreaInShiftClearance = value.totalAreaInHourClearance / 8;
                            value.isDiameterVisible = true;
                            value.totalCostClearance = value.totalAreaInShiftClearance * value.shiftPrice;
                            value.totalCostPerSquareClearance = value.totalCostClearance / value.area;
                            calculatingMaintainanceCostForClearance();
                            ChangingZeroToNullForClearance();
                        }
                        if (vm.service.serviceName == "Honing" && vm.service.typeOfStoneType2 == "Granite") {
                            value.ugcClearance = 0;
                            value.thirtyClearance = 0;
                            value.fiftyClearance = 0;
                            value.hundredClearance = 0;
                            value.twoHundredClearance = 20;
                            value.fourHundredClearance = 20;
                            value.eightHundredClearance = 20;
                            value.fifteenHundredClearance = 10;
                            value.threeThousandClearance = 0;
                            value.eightThousandClearance = 0;
                            value.elevenThousandClearance = 0;
                            value.cacoClearance = 0;
                            value.ihgClearance = 0;
                            value.sectionsClearance = (value.area / 10);
                            value.produtivityRateClearance = value.ugcClearance + value.thirtyClearance + value.fiftyClearance + value.hundredClearance + value.twoHundredClearance + value.fourHundredClearance + value.eightHundredClearance + value.fifteenHundredClearance + value.threeThousandClearance + value.eightThousandClearance + value.elevenThousandClearance + value.cacoClearance + value.ihgClearance;

                            value.totalMinuteClearance = "65%";
                            value.seventeenBaseClearance = (value.produtivityRateClearance / 65) * 100;
                            value.totalAreaClearance = (value.seventeenBaseClearance * value.sectionsClearance) / (value.dimensionClearance);
                            value.totalAreaInHourClearance = value.totalAreaClearance / 60;
                            value.totalAreaInShiftClearance = value.totalAreaInHour / 8
                            value.isDiameterVisible = false;
                            value.totalCostClearance = value.totalAreaInShiftClearance * value.shiftPrice;
                            value.totalCostPerSquareClearance = value.totalCostClearance / value.area;
                            calculatingMaintainanceCostForClearance();
                            ChangingZeroToNullForClearance();
                        }
                    })
                }

                function viewSuggestion() {
                    vm.isClicked = true;
                    vm.clearanceCheck = true;
                    clearanceValue();
                }

                function ChangingZeroToNullForOrginal() {
                    angular.forEach(vm.honingMeasurementList, function (value, index) {
                        if (value.ugcOriginal == 0) {
                            value.ugcOriginal = '';
                        }
                        if (value.thirtyOriginal == 0) {
                            value.thirtyOriginal = '';
                        }
                        if (value.fiftyOriginal == 0) {
                            value.fiftyOriginal = '';
                        }
                        if (value.hundredOriginal == 0) {
                            value.hundredOriginal = '';
                        }
                        if (value.twoHundredOriginal == 0) {
                            value.twoHundredOriginal = '';
                        }
                        if (value.fourHundredOriginal == 0) {
                            value.fourHundredOriginal = '';
                        }
                        if (value.eightHundredOriginal == 0) {
                            value.eightHundredOriginal = '';
                        }
                        if (value.fifteenHundredOriginal == 0) {
                            value.fifteenHundredOriginal = '';
                        }
                        if (value.threeThousandOriginal == 0) {
                            value.threeThousandOriginal = '';
                        }
                        if (value.eightThousandOriginal == 0) {
                            value.eightThousandOriginal = '';
                        }
                        if (value.elevenThousandOriginal == 0) {
                            value.elevenThousandOriginal = '';
                        }
                        if (value.cacoOriginal == 0) {
                            value.cacoOriginal = '';
                        }
                        if (value.ihgOriginal == 0) {
                            value.ihgOriginal = '';
                        }
                        if (value.areaOriginal == 0) {
                            value.areaOriginal = '';
                        }
                    })
                }

                function ChangingZeroToNullForClearance() {
                    angular.forEach(vm.honingMeasurementList, function (value, index) {
                        if (value.ugcClearance == 0) {
                            value.ugcClearance = '';
                        }
                        if (value.thirtyClearance == 0) {
                            value.thirtyClearance = '';
                        }
                        if (value.fiftyClearance == 0) {
                            value.fiftyClearance = '';
                        }
                        if (value.hundredClearance == 0) {
                            value.hundredClearance = '';
                        }
                        if (value.twoHundredClearance == 0) {
                            value.twoHundredClearance = '';
                        }
                        if (value.fourHundredClearance == 0) {
                            value.fourHundredClearance = '';
                        }
                        if (value.eightHundredClearance == 0) {
                            value.eightHundredClearance = '';
                        }
                        if (value.fifteenHundredClearance == 0) {
                            value.fifteenHundredClearance = '';
                        }
                        if (value.threeThousandClearance == 0) {
                            value.threeThousandClearance = '';
                        }
                        if (value.eightThousandClearance == 0) {
                            value.eightThousandClearance = '';
                        }
                        if (value.elevenThousandClearance == 0) {
                            value.elevenThousandClearance = '';
                        }
                        if (value.cacoClearance == 0) {
                            value.cacoClearance = '';
                        }
                        if (value.ihgClearance == 0) {
                            value.ihgClearance = '';
                        }
                        if (value.areaClearance == 0) {
                            value.areaClearance = '';
                        }
                    })
                }

                function addingArea() {
                    vm.service.sumTotalArea = 0;
                    vm.totalAreaForInvoice = 0;
                    angular.forEach(vm.honingMeasurementList, function (value, index) {
                        vm.service.measurements = [];
                        vm.totalAreaForInvoice += value.area;
                        vm.service.measurements.push({
                            length: 0,
                            width: 0,
                            area: vm.totalAreaForInvoice,
                            dimension: '',
                            description: '',
                            unitType: vm.service.unitType,
                            unitTypeId: vm.service.unitTypeId
                        });
                        vm.service.sumTotalArea += value.area;
                    })
                }

                function calculateSumTotalArea() {
                    vm.sumTotalArea = 0;
                    vm.totalUnit = 0;
                    if (vm.honingMeasurementList.length > 0) {
                        angular.forEach(vm.honingMeasurementList, function (value, index) {
                            vm.totalUnit += value.setPrice;
                            if (vm.serviceTypeId == vm.serviceType.AREA) {
                                if (value.area != '' || value.area != null) {
                                    vm.sumTotalArea += value.area;
                                }
                            }
                            else {
                                if (value.setPrice != null) {
                                    vm.sumTotalArea += value.setPrice;
                                    value.area = value.setPrice;

                                }
                                else if (vm.serviceTypeId == vm.serviceType.TIME || (vm.serviceTypeId == vm.serviceType.MAINTAINANCE)) {
                                    vm.sumTotalArea += value.areaTime;
                                    value.area = value.areaTime;
                                }
                                else if ((vm.serviceTypeId == vm.serviceType.LINEARFT)) {
                                    vm.sumTotalArea += value.setPrice;
                                    value.area = value.setPrice;
                                }
                            }
                        });
                    }
                    if (vm.service.measurements.length > 0) {
                        angular.forEach(vm.service.measurements, function (value, index) {
                            /*vm.totalUnit += value.setPrice;*/
                            if (vm.serviceTypeId == vm.serviceType.AREA) {
                                if (value.area != '' || value.area != null) {
                                    vm.sumTotalArea += value.area;
                                    value.area += value.areaTime;
                                }
                            }
                            else {
                                if (value.setPrice != null) {
                                    //vm.sumTotalArea += value.setPrice;
                                    //value.area = value.setPrice;
                                }
                                if (vm.serviceTypeId == vm.serviceType.TIME || (vm.serviceTypeId == vm.serviceType.MAINTAINANCE) || (vm.serviceTypeId == vm.serviceType.EVENT)) {
                                    vm.sumTotalArea += value.areaTime;
                                    value.area = value.areaTime;
                                    //value.area += value.areaTime;
                                }
                                if (vm.serviceTypeId == vm.serviceType.Event) {
                                    vm.sumTotalArea += value.setPrice;
                                    value.area = value.setPrice;
                                }
                                if (vm.serviceTypeId == vm.serviceType.LINEARFT) {
                                    vm.sumTotalArea += value.area;
                                    value.area = value.area;
                                }
                            }
                        });
                    }
                    vm.service.sumTotalArea = vm.sumTotalArea;
                    if (isNaN(vm.service.sumTotalArea)) {
                        vm.service.sumTotalArea = 0;
                    }
                }

                function getHoiningMeasurementCheck() {
                    if (vm.service.typeOfStoneType2 == "Marble" && vm.service.typeOfStoneType == "Ultimate Finish" && vm.service.typeOfSurface == "Floor" && (vm.ServiceName == "Honing" || vm.ServiceName == "Polishing")) {
                        return true;
                    }
                    else if (vm.service.typeOfStoneType2 == "Marble" && vm.service.typeOfStoneType == "Gloss" && vm.service.typeOfSurface == "Floor" && (vm.ServiceName == "Honing" || vm.ServiceName == "Polishing")) {
                        return true;
                    }
                    else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneType == "Satin" || vm.service.typeOfStoneType == "Semi-Gloss") && vm.service.typeOfSurface == "Floor" && (vm.ServiceName == "Honing" || vm.ServiceName == "Polishing")) {
                        return true;
                    }
                    else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneType == "Matte") && vm.service.typeOfSurface == "Floor" && (vm.service.serviceType == "Honing" || vm.service.serviceType == "Polishing")) {
                        return true;
                    }
                    else if (vm.service.typeOfStoneType2 == "Granite" && vm.service.typeOfSurface == "Floor" && (vm.ServiceName == "Honing" || vm.ServiceName == "Polishing")) {
                        return true;
                    }
                    else if (vm.service.typeOfStoneType2 == "Concrete" && vm.service.typeOfSurface == "Floor" && (vm.ServiceName == "Honing" || vm.ServiceName == "Polishing")) {
                        return true;

                    }
                    else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                        || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                        || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                        || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor") && (vm.ServiceName == "Honing" || vm.ServiceName == "Polishing")) {

                        return true;

                    }
                    else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                        || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                        || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                        || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor") && (vm.ServiceName == "Honing" || vm.ServiceName == "Polishing")) {

                        return true;

                    }
                    else if (vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                        || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                        || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                        || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor") && (vm.ServiceName == "Honing" || vm.ServiceName == "Polishing")) {

                        return true;
                    }
                    else if ((vm.service.serviceType == "CONCRETE-OVERLAYMENTS" || vm.service.serviceType == "CLEANSHIELD" || vm.service.serviceType == "CONCRETE-COATINGS" || vm.service.serviceType == "STONELIFE" || vm.service.serviceType == "COUNTERLIFE" || vm.service.serviceType == "ENDURACRETE" || vm.service.serviceType == "METALLIFE" || vm.service.serviceType == "WOODLIFE")
                        && (vm.ServiceName == "Concrete Preparations" || vm.ServiceName == "Hone" || vm.ServiceName == "Honing" || vm.ServiceName == "Hone Surface Prep" || vm.ServiceName == "Metal Scratch Removal" || vm.ServiceName == "Polishing" || vm.ServiceName == "Scratch & Wear Removal (Sanding)" || vm.ServiceName == "Scratch & Wear Removal (Sanding)")) {
                        return true;
                    }
                    else if (vm.ServiceName.startsWith("Bundle") && vm.serviceTypeId != vm.serviceType.AREA) {
                        return true;
                    }
                    else if (vm.serviceTypeId == vm.serviceType.TIME) {
                        return true;
                    }

                    return false;
                }

                function calculationAccordingToColor() {
                    ChangingNullToZero();
                    if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneColor == "Black")) {
                        vm.honingMeasurement.fifteenHundred = 5;
                    }
                    else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneColor == "Brown")) {
                        vm.honingMeasurement.fourHundred = 5;
                    }
                    else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneColor == "Gray")) {
                        vm.honingMeasurement.eightHundred = 5;
                    }
                    else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneColor == "Green")) {
                        vm.honingMeasurement.eightHundred = 5;
                    }
                    else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneColor == "Red")) {
                        vm.honingMeasurement.fourHundred = 5;
                    }
                    else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneColor == "Tan")) {
                        vm.honingMeasurement.twoHundred = 5;
                    }
                    else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneColor == "White")) {
                        vm.honingMeasurement.fourHundred = 5;
                    }
                    else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "Black")) {
                        vm.honingMeasurement.eightThousand = 10;
                    }
                    else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "Brown")) {
                        vm.honingMeasurement.threeThousand = 5;
                    }
                    else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "Gray")) {
                        vm.honingMeasurement.threeThousand = 5;
                    }
                    else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "Green")) {
                        vm.honingMeasurement.threeThousand = 5;
                    }
                    else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "Red")) {
                        vm.honingMeasurement.fifteenHundred = 5;
                    }
                    else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "Tan")) {
                        vm.honingMeasurement.fifteenHundred = 5;
                    }
                    else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "White")) {
                        vm.honingMeasurement.eightHundred = 5;
                    }
                    else if (vm.service.serviceName != "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Black")) {
                        vm.honingMeasurement.fifteenHundred = 5;
                    }
                    else if (vm.service.serviceName != "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Brown")) {
                        vm.honingMeasurement.fifteenHundred = 5;
                    }
                    else if (vm.service.serviceName != "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Gray")) {
                        vm.honingMeasurement.fifteenHundred = 5;
                    }
                    else if (vm.service.serviceName != "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Green")) {
                        vm.honingMeasurement.fifteenHundred = 5;
                    }
                    else if (vm.service.serviceName != "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Red")) {
                        vm.honingMeasurement.fifteenHundred = 5;
                    }
                    else if (vm.service.serviceName != "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Tan")) {
                        vm.honingMeasurement.fifteenHundred = 5;
                    }
                    else if (vm.service.serviceName != "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "White")) {
                        vm.honingMeasurement.fifteenHundred = 5;
                    }
                    else if (vm.service.serviceName == "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Black")) {
                        vm.honingMeasurement.fifteenHundred = 0;
                    }
                    else if (vm.service.serviceName == "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Brown")) {
                        vm.honingMeasurement.fifteenHundred = 0;
                    }
                    else if (vm.service.serviceName == "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Gray")) {
                        vm.honingMeasurement.fifteenHundred = 0;
                    }
                    else if (vm.service.serviceName == "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Green")) {
                        vm.honingMeasurement.fifteenHundred = 0;
                    }
                    else if (vm.service.serviceName == "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Red")) {
                        vm.honingMeasurement.fifteenHundred = 0;
                    }
                    else if (vm.service.serviceName == "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Tan")) {
                        vm.honingMeasurement.fifteenHundred = 0;
                    }
                    else if (vm.service.serviceName == "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "White")) {
                        vm.honingMeasurement.fifteenHundred = 0;
                    }
                    else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "Black")) {
                        vm.honingMeasurement.fifteenHundred = 5;
                    }
                    else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "Brown")) {
                        vm.honingMeasurement.fifteenHundred = 5;
                    }
                    else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "Gray")) {
                        vm.honingMeasurement.fifteenHundred = 5;
                    }
                    else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "Green")) {
                        vm.honingMeasurement.fifteenHundred = 5;
                    }
                    else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "Red")) {
                        vm.honingMeasurement.fifteenHundred = 5;
                    }
                    else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "Tan")) {
                        vm.honingMeasurement.fifteenHundred = 5;
                    }
                    else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "White")) {
                        vm.honingMeasurement.fifteenHundred = 5;
                    }
                    else if (vm.service.serviceName == "Hone" && (vm.service.typeOfStoneColor == "Black")) {
                        vm.honingMeasurement.fifteenHundred = 5;
                    }
                    else if (vm.service.serviceName == "Hone" && (vm.service.typeOfStoneColor == "Brown")) {
                        vm.honingMeasurement.fourHundred = 5;
                    }
                    else if (vm.service.serviceName == "Hone" && (vm.service.typeOfStoneColor == "Gray")) {
                        vm.honingMeasurement.eightHundred = 5;
                    }
                    else if (vm.service.serviceName == "Hone" && (vm.service.typeOfStoneColor == "Green")) {
                        vm.honingMeasurement.eightHundred = 5;
                    }
                    else if (vm.service.serviceName == "Hone" && (vm.service.typeOfStoneColor == "Red")) {
                        vm.honingMeasurement.fourHundred = 5;
                    }
                    else if (vm.service.serviceName == "Hone" && (vm.service.typeOfStoneColor == "Tan")) {
                        vm.honingMeasurement.twoHundred = 5;
                    }
                    else if (vm.service.serviceName == "Hone" && (vm.service.typeOfStoneColor == "White")) {
                        vm.honingMeasurement.fourHundred = 5;
                    }
                    vm.honingMeasurement.sections = vm.honingMeasurement.area / vm.honingMeasurement.sectionsDivision;
                    vm.honingMeasurement.produtivityRate = vm.honingMeasurement.ugc + vm.honingMeasurement.thirty + vm.honingMeasurement.fifty + vm.honingMeasurement.hundred + vm.honingMeasurement.twoHundred +
                        vm.honingMeasurement.fourHundred + vm.honingMeasurement.eightHundred + vm.honingMeasurement.fifteenHundred + vm.honingMeasurement.threeThousand + vm.honingMeasurement.eightThousand + vm.honingMeasurement.elevenThousand + vm.honingMeasurement.caco + vm.honingMeasurement.ihg;
                    vm.honingMeasurement.seventeenBase = (vm.honingMeasurement.produtivityRate / vm.honingMeasurement.totalMinuteValue) * 100;
                    vm.honingMeasurement.totalArea = (vm.honingMeasurement.seventeenBase * vm.honingMeasurement.sections) / (vm.honingMeasurement.dimension);
                    vm.honingMeasurement.totalAreaInHour = vm.honingMeasurement.totalArea / 60;
                    vm.honingMeasurement.totalAreaInShift = vm.honingMeasurement.totalAreaInHour / 8;
                    ChangingZeroToNull();
                }

                function calculationAccordingToColorOriginal() {
                    angular.forEach(vm.honingMeasurementList, function (value, index) {
                        ChangingNullToZero();
                        if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneColor == "Black")) {
                            value.fifteenHundredOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneColor == "Brown")) {
                            value.fourHundredOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneColor == "Gray")) {
                            value.eightHundredOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneColor == "Green")) {
                            value.eightHundredOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneColor == "Red")) {
                            value.fourHundredOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneColor == "Tan")) {
                            value.twoHundredOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Marble" && (vm.service.typeOfStoneColor == "White")) {
                            value.fourHundredOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "Black")) {
                            value.eightThousandOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "Brown")) {
                            value.threeThousandOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "Gray")) {
                            value.threeThousandOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "Green")) {
                            value.threeThousandOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "Red")) {
                            value.fifteenHundredOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "Tan")) {
                            value.fifteenHundredOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "White")) {
                            value.eightHundred = 5;
                        }
                        else if (vm.service.serviceName != 'Concrete Floor Prep - Grind' && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Black")) {
                            value.fifteenHundredOriginal = 5;
                        }
                        else if (vm.service.serviceName != 'Concrete Floor Prep - Grind' && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Brown")) {
                            value.fifteenHundredOriginal = 5;
                        }
                        else if (vm.service.serviceName != 'Concrete Floor Prep - Grind' && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Gray")) {
                            value.fifteenHundredOriginal = 5;
                        }
                        else if (vm.service.serviceName != 'Concrete Floor Prep - Grind' && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Green")) {
                            value.fifteenHundredOriginal = 5;
                        }
                        else if (vm.service.serviceName != 'Concrete Floor Prep - Grind' && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Red")) {
                            value.fifteenHundredOriginal = 5;
                        }
                        else if (vm.service.serviceName != 'Concrete Floor Prep - Grind' && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Tan")) {
                            value.fifteenHundredOriginal = 5;
                        }
                        else if (vm.service.serviceName != 'Concrete Floor Prep - Grind' && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "White")) {
                            value.fifteenHundredOriginal = 5;
                        }
                        else if (vm.service.serviceName == 'Concrete Floor Prep - Grind' && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Black")) {
                            value.fifteenHundredOriginal = 0;
                        }
                        else if (vm.service.serviceName == 'Concrete Floor Prep - Grind' && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Brown")) {
                            value.fifteenHundredOriginal = 0;
                        }
                        else if (vm.service.serviceName == 'Concrete Floor Prep - Grind' && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Gray")) {
                            value.fifteenHundredOriginal = 0;
                        }
                        else if (vm.service.serviceName == 'Concrete Floor Prep - Grind' && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Green")) {
                            value.fifteenHundredOriginal = 0;
                        }
                        else if (vm.service.serviceName == 'Concrete Floor Prep - Grind' && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Red")) {
                            value.fifteenHundredOriginal = 0;
                        }
                        else if (vm.service.serviceName == 'Concrete Floor Prep - Grind' && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Tan")) {
                            value.fifteenHundredOriginal = 0;
                        }
                        else if (vm.service.serviceName == 'Concrete Floor Prep - Grind' && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "White")) {
                            value.fifteenHundredOriginal = 0;
                        }
                        else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "Black")) {
                            value.fifteenHundredOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "Brown")) {
                            value.fifteenHundredOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "Gray")) {
                            value.fifteenHundredOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "Green")) {
                            value.fifteenHundredOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "Red")) {
                            value.fifteenHundredOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "Tan")) {
                            value.fifteenHundredOriginal = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "White")) {
                            value.fifteenHundredOriginal = 5;
                        }
                        value.sectionsOriginal = value.area / value.sectionsOriginalDivision;;
                        value.produtivityRateOriginal = value.ugcOriginal + value.thirtyOriginal + value.fiftyOriginal + value.hundredOriginal + value.twoHundredOriginal +
                            value.fourHundredOriginal + value.eightHundredOriginal + value.fifteenHundredOriginal + value.threeThousandOriginal + value.eightThousandOriginal + value.elevenThousandOriginal + value.cacoOriginal + value.ihgOriginal;
                        value.seventeenBaseOriginal = (value.produtivityRateOriginal / value.totalMinuteValueOriginal) * 100;
                        value.totalAreaOriginal = (value.seventeenBaseOriginal * value.sectionsOriginal) / (value.dimension);
                        value.totalAreaInHourOriginal = value.totalAreaOriginal / 60;
                        value.totalAreaInShiftOriginal = value.totalAreaInHourOriginal / 8;
                        value.totalCostOriginal = value.totalAreaInShiftOriginal * value.shiftPrice;
                        if (value.area != 0) {
                            value.totalCostPerSquareOriginal = value.totalCostOriginal / value.area;
                        }
                        if (isNaN(value.totalCostPerSquareOriginal)) {
                            value.totalCostPerSquareOriginal = 0;
                        }
                    })
                }

                function calculatingMaintainanceCostForClearance() {
                    angular.forEach(vm.honingMeasurementList, function (value, index) {
                        if (vm.maintenanceChargesListCurrent != null && vm.maintenanceChargesListCurrent.length > 0) {
                            value.highDivisionClearance = Math.round(value.area / vm.maintenanceChargesListCurrent[0].high);
                            value.lowDivisionClearance = Math.round(value.area / vm.maintenanceChargesListCurrent[0].low);
                            value.averageNightRequiredClearance = Math.round((value.highDivisionClearance + value.lowDivisionClearance) / 2);
                            value.averageNightPerMonthClearance = value.averageNightRequiredClearance * vm.franchiseeShiftPrice.maintainanceTechNightShiftPrice;
                            value.averageNightPerMonthPerSqFTClearance = value.averageNightPerMonthClearance / value.area;
                            value.annualMaintainceCostClearance = (value.averageNightPerMonthClearance * 12);
                            value.minRestorationValueClearance = (value.annualMaintainceCostClearance / (1 - 0.15));
                            value.highValueClearance = vm.maintenanceChargesListCurrent[0].high;
                            value.materialForFormulaClearance = vm.maintenanceChargesListCurrent[0].material;
                            value.lowValueClearance = vm.maintenanceChargesListCurrent[0].low;
                        }
                    })
                }

                function getmarketingClassCollection() {
                    return customerService.getmarketingClassCollection().then(function (result) {
                        vm.marketingClass = result.data;
                        var marketingClassIndex = vm.marketingClass.indexOf($filter('filter')(vm.marketingClass, { display: vm.classType }, true)[0]);
                        vm.classTypeValue = vm.marketingClass[marketingClassIndex].value;
                    });
                }

                function marketingClassChanged() {
                    var marketingClassIndex = vm.marketingClass.indexOf($filter('filter')(vm.marketingClass, { value: vm.classTypeValue }, true)[0]);
                    vm.classType = vm.marketingClass[marketingClassIndex].display;
                    getShiftPrice();
                }

                function removeRows(index) {
                    vm.honingMeasurementList.splice(index, 1);
                    addingArea();
                };

                function getHoningMeasurementHoverValues() {
                    angular.forEach(vm.honingMeasurementList, function (value, index) {
                        value.totalAreaHover = "SeventeenBase * Sections / Diameter = " + (value.seventeenBase.toFixed(2) + " * " + value.sections) + " / " + (value.dimension);
                        value.totalAreaInHourHover = "Total Time(MIN) / 60 = " + value.totalArea.toFixed(2) + " / " + 60;
                        value.totalAreaInShiftHover = "Total Time(HR) / 8 = " + value.totalAreaInHour.toFixed(2) + " / " + 8;
                        value.totalCostHover = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShift.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                        //value.totalCostPerSquareHover = "Total Cost / Area = " + value.totalCost.toFixed(2) + " / " + value.area.toFixed(2);
                        //value.totalAreaHoverOriginal = "SeventeenBase * Sections / Diameter = " + (value.seventeenBaseOriginal.toFixed(2) + " * " + value.sections.toFixed(2)) + " / " + (value.dimension);
                        //value.totalAreaInHourHoverOriginal = "Total Time(MIN) / 60 = " + value.totalAreaOriginal.toFixed(2) + " / " + 60;
                        //value.totalAreaInShiftHoverOriginal = "Total Time(HR) / 8 = " + value.totalAreaInHourOriginal.toFixed(2) + " / " + 8;
                        //value.totalCostHoverOriginal = "Total Time(Shift) * ShiftPrice = " + value.totalAreaInShiftOriginal.toFixed(2) + " * " + value.shiftPrice.toFixed(2);
                        //value.totalCostPerSquareHoverOriginal = "Total Cost / Area = " + value.totalCostOriginal.toFixed(2) + " / " + value.area.toFixed(2);
                    })
                }

                function getTotalProjectedValue() {
                    vm.service.option1 = 0;
                    vm.totalProjectedValue = 0;
                    angular.forEach(vm.honingMeasurementList, function (value, index) {
                        vm.totalProjectedValue += value.totalCost;
                    })
                    vm.service.option1 = vm.totalProjectedValue.toFixed(2);
                    if (vm.isSealing) {
                        vm.service.option1 = (vm.totalProjectedValue + vm.service.sumTotalArea).toFixed(2);
                    }
                }

                function calculateAreaofAService(service) {
                    if (service.length != null && service.width != null && service.length != undefined && service.width != undefined && service.length != '' && service.width != '') {
                        service.area = service.length * service.width;
                        calculateTimeCalcuation(service);
                    }
                }

                function getHoiningMeasurementLoadCompare() {
                    if (vm.isHoning) {
                        if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && vm.service.typeOfStoneType == "Ultimate Finish" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ugcCompare = 20;
                            vm.honingMeasurement.thirtyCompare = 10;
                            vm.honingMeasurement.fiftyCompare = 5;
                            vm.honingMeasurement.hundredCompare = 5;
                            vm.honingMeasurement.twoHundredCompare = 5;
                            vm.honingMeasurement.fourHundredCompare = 5;
                            vm.honingMeasurement.eightHundredCompare = 5;
                            vm.honingMeasurement.fifteenHundredCompare = 0;
                            vm.honingMeasurement.threeThousandCompare = 0;
                            vm.honingMeasurement.eightThousandCompare = 0;
                            vm.honingMeasurement.elevenThousandCompare = 0;
                            vm.honingMeasurement.cacoCompare = 4;
                            vm.honingMeasurement.ihgCompare = 2;
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && vm.service.typeOfStoneType == "Gloss" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ugcCompare = 0;
                            vm.honingMeasurement.thirtyCompare = 0;
                            vm.honingMeasurement.fiftyCompare = 0;
                            vm.honingMeasurement.hundredCompare = 0;
                            vm.honingMeasurement.twoHundredCompare = 5;
                            vm.honingMeasurement.fourHundredCompare = 5;
                            vm.honingMeasurement.eightHundredCompare = 5;
                            vm.honingMeasurement.fifteenHundredCompare = 0;
                            vm.honingMeasurement.threeThousandCompare = 0;
                            vm.honingMeasurement.eightThousandCompare = 0;
                            vm.honingMeasurement.elevenThousandCompare = 0;
                            vm.honingMeasurement.cacoCompare = 4;
                            vm.honingMeasurement.ihgCompare = 2;
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneType == "Satin" || vm.service.typeOfStoneType == "Semi-Gloss") && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ugcCompare = 0;
                            vm.honingMeasurement.thirtyCompare = 0;
                            vm.honingMeasurement.fiftyCompare = 0;
                            vm.honingMeasurement.hundredCompare = 0;
                            vm.honingMeasurement.twoHundredCompare = 5;
                            vm.honingMeasurement.fourHundredCompare = 5;
                            vm.honingMeasurement.eightHundredCompare = 5;
                            vm.honingMeasurement.fifteenHundredCompare = 0;
                            vm.honingMeasurement.threeThousandCompare = 0;
                            vm.honingMeasurement.eightThousandCompare = 0;
                            vm.honingMeasurement.elevenThousandCompare = 0;
                            vm.honingMeasurement.cacoCompare = 0;
                            vm.honingMeasurement.ihgCompare = 2;
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneType == "Matte") && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ugcCompare = 0;
                            vm.honingMeasurement.thirtyCompare = 0;
                            vm.honingMeasurement.fiftyCompare = 0;
                            vm.honingMeasurement.hundredCompare = 0;
                            vm.honingMeasurement.twoHundredCompare = 5;
                            vm.honingMeasurement.fourHundredCompare = 5;
                            vm.honingMeasurement.eightHundredCompare = 5;
                            vm.honingMeasurement.fifteenHundredCompare = 0;
                            vm.honingMeasurement.threeThousandCompare = 0;
                            vm.honingMeasurement.eightThousandCompare = 0;
                            vm.honingMeasurement.elevenThousandCompare = 0;
                            vm.honingMeasurement.cacoCompare = 0;
                            vm.honingMeasurement.ihgCompare = 0;
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && vm.service.typeOfSurface == "Floor") {
                            vm.honingMeasurement.ugcCompare = 0;
                            vm.honingMeasurement.thirtyCompare = 0;
                            vm.honingMeasurement.fiftyCompare = 0;
                            vm.honingMeasurement.hundredCompare = 0;
                            vm.honingMeasurement.twoHundredCompare = 0;
                            vm.honingMeasurement.fourHundredCompare = 0;
                            vm.honingMeasurement.eightHundredCompare = 20;
                            vm.honingMeasurement.fifteenHundredCompare = 10;
                            vm.honingMeasurement.threeThousandCompare = 10;
                            vm.honingMeasurement.eightThousandCompare = 0;
                            vm.honingMeasurement.elevenThousandCompare = 0;
                            vm.honingMeasurement.cacoCompare = 10;
                            vm.honingMeasurement.ihgCompare = 2;
                        }
                        else if (vm.service.typeOfStoneType2 == "Concrete" && vm.service.typeOfSurface == "Floor" && vm.service.serviceName == "Concrete Floor Prep - Grind") {
                            vm.honingMeasurement.ugcCompare = 0;
                            vm.honingMeasurement.thirtyCompare = 0;
                            vm.honingMeasurement.fiftyCompare = 0;
                            vm.honingMeasurement.hundredCompare = 0;
                            vm.honingMeasurement.twoHundredCompare = 4;
                            vm.honingMeasurement.fourHundredCompare = 0;
                            vm.honingMeasurement.eightHundredCompare = 0;
                            vm.honingMeasurement.fifteenHundredCompare = 0;
                            vm.honingMeasurement.threeThousandCompare = 0;
                            vm.honingMeasurement.eightThousandCompare = 0;
                            vm.honingMeasurement.elevenThousandCompare = 0;
                            vm.honingMeasurement.cacoCompare = 0;
                            vm.honingMeasurement.ihgCompare = 2;
                        }
                        else if (vm.service.typeOfStoneType2 == "Concrete" && vm.service.typeOfSurface == "Floor" && vm.service.serviceName != "Concrete Floor Prep - Grind") {
                            vm.honingMeasurement.ugcCompare = 0;
                            vm.honingMeasurement.thirtyCompare = 4;
                            vm.honingMeasurement.fiftyCompare = 4;
                            vm.honingMeasurement.hundredCompare = 4;
                            vm.honingMeasurement.twoHundredCompare = 4;
                            vm.honingMeasurement.fourHundredCompare = 4;
                            vm.honingMeasurement.eightHundredCompare = 4;
                            vm.honingMeasurement.fifteenHundredCompare = 4;
                            vm.honingMeasurement.threeThousandCompare = 4;
                            vm.honingMeasurement.eightThousandCompare = 0;
                            vm.honingMeasurement.elevenThousandCompare = 0;
                            vm.honingMeasurement.cacoCompare = 0;
                            vm.honingMeasurement.ihgCompare = 2;
                        }
                        else if ((vm.service.serviceType != "ENDURACRETE" && vm.service.serviceType != "CONCRETE-STAIN") && (vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                            vm.honingMeasurement.ugcCompare = 0;
                            vm.honingMeasurement.thirtyCompare = 0;
                            vm.honingMeasurement.fiftyCompare = 0;
                            vm.honingMeasurement.hundredCompare = 0;
                            vm.honingMeasurement.twoHundredCompare = 5;
                            vm.honingMeasurement.fourHundredCompare = 5;
                            vm.honingMeasurement.eightHundredCompare = 5;
                            vm.honingMeasurement.fifteenHundredCompare = 0;
                            vm.honingMeasurement.threeThousandCompare = 0;
                            vm.honingMeasurement.eightThousandCompare = 0;
                            vm.honingMeasurement.elevenThousandCompare = 0;
                            vm.honingMeasurement.cacoCompare = 4;
                            vm.honingMeasurement.ihgCompare = 2;
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                            vm.honingMeasurement.ugcCompare = 0;
                            vm.honingMeasurement.thirtyCompare = 0;
                            vm.honingMeasurement.fiftyCompare = 0;
                            vm.honingMeasurement.hundredCompare = 0;
                            vm.honingMeasurement.twoHundredCompare = 0;
                            vm.honingMeasurement.fourHundredCompare = 20;
                            vm.honingMeasurement.eightHundredCompare = 10;
                            vm.honingMeasurement.fifteenHundredCompare = 10;
                            vm.honingMeasurement.threeThousandCompare = 0;
                            vm.honingMeasurement.eightThousandCompare = 0;
                            vm.honingMeasurement.elevenThousandCompare = 0;
                            vm.honingMeasurement.cacoCompare = 10;
                            vm.honingMeasurement.ihgCompare = 2;
                        }
                        else if (vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfSurface == "Shower Wall" || vm.service.typeOfSurface == "Wall"
                            || vm.service.typeOfSurface == "Counter (Kitchen)" || vm.service.typeOfSurface == "Sign" || vm.service.typeOfSurface == "Vanity" || vm.service.typeOfSurface == "Building Exterior"
                            || vm.service.typeOfSurface == "Building Exterior" || vm.service.typeOfSurface == "Other" || vm.service.typeOfSurface == "Fireplace" || vm.service.typeOfSurface == "Pool Deck" || vm.service.typeOfSurface == "Vanity (Bathroom)"
                            || vm.service.typeOfSurface == "Walkway" || vm.service.typeOfSurface == "Patio" || vm.service.typeOfSurface == "Floor")) {

                            vm.honingMeasurement.ugcCompare = 0;
                            vm.honingMeasurement.thirtyCompare = 4;
                            vm.honingMeasurement.fiftyCompare = 4;
                            vm.honingMeasurement.hundredCompare = 4;
                            vm.honingMeasurement.twoHundredCompare = 4;
                            vm.honingMeasurement.fourHundredCompare = 4;
                            vm.honingMeasurement.eightHundredCompare = 4;
                            vm.honingMeasurement.fifteenHundredCompare = 4;
                            vm.honingMeasurement.threeThousandCompare = 4;
                            vm.honingMeasurement.eightThousandCompare = 0;
                            vm.honingMeasurement.elevenThousandCompare = 0;
                            vm.honingMeasurement.cacoCompare = 0;
                            vm.honingMeasurement.ihgCompare = 2;
                        }
                        else if ((vm.service.serviceType == "ENDURACRETE" || vm.service.serviceType == "CONCRETE-STAIN") && vm.service.serviceName == "Bundle (Honing & Polishing)") {
                            vm.honingMeasurement.ugcCompare = 0;
                            vm.honingMeasurement.thirtyCompare = 4;
                            vm.honingMeasurement.fiftyCompare = 4;
                            vm.honingMeasurement.hundredCompare = 4;
                            vm.honingMeasurement.twoHundredCompare = 4;
                            vm.honingMeasurement.fourHundredCompare = 4;
                            vm.honingMeasurement.eightHundredCompare = 4;
                            vm.honingMeasurement.fifteenHundredCompare = 4;
                            vm.honingMeasurement.threeThousandCompare = 4;
                            vm.honingMeasurement.eightThousandCompare = 0;
                            vm.honingMeasurement.elevenThousandCompare = 0;
                            vm.honingMeasurement.cacoCompare = 0;
                            vm.honingMeasurement.ihgCompare = 2;
                        }
                        else if (vm.service.serviceName == "Hone" && vm.service.typeOfStoneType2 == "Terrazzo") {
                            vm.honingMeasurement.ugcCompare = 0;
                            vm.honingMeasurement.thirtyCompare = 0;
                            vm.honingMeasurement.fiftyCompare = 0;
                            vm.honingMeasurement.hundredCompare = 0;
                            vm.honingMeasurement.twoHundredCompare = 5;
                            vm.honingMeasurement.fourHundredCompare = 5;
                            vm.honingMeasurement.eightHundredCompare = 5;
                            vm.honingMeasurement.fifteenHundredCompare = 0;
                            vm.honingMeasurement.threeThousandCompare = 0;
                            vm.honingMeasurement.eightThousandCompare = 0;
                            vm.honingMeasurement.elevenThousandCompare = 0;
                            vm.honingMeasurement.cacoCompare = 4;
                            vm.honingMeasurement.ihgCompare = 2;
                        }
                        else {

                        }
                        if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneColor == "Black")) {
                            vm.honingMeasurement.fifteenHundredCompare = 5;
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneColor == "Brown")) {
                            vm.honingMeasurement.fourHundredCompare = 5;
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneColor == "Gray")) {
                            vm.honingMeasurement.eightHundredCompare = 5;
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneColor == "Green")) {
                            vm.honingMeasurement.eightHundredCompare = 5;
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneColor == "Red")) {
                            vm.honingMeasurement.fourHundredCompare = 5;
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneColor == "Tan")) {
                            vm.honingMeasurement.twoHundredCompare = 5;
                        }
                        else if ((vm.service.typeOfStoneType2 == "Marble" || vm.service.typeOfStoneType2 == "Travertine") && (vm.service.typeOfStoneColor == "White")) {
                            vm.honingMeasurement.fourHundredCompare = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "Black")) {
                            vm.honingMeasurement.eightThousandCompare = 10;
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "Brown")) {
                            vm.honingMeasurement.threeThousandCompare = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "Gray")) {
                            vm.honingMeasurement.threeThousandCompare = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "Green")) {
                            vm.honingMeasurement.threeThousandCompare = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "Red")) {
                            vm.honingMeasurement.fifteenHundredCompare = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "Tan")) {
                            vm.honingMeasurement.fifteenHundredCompare = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Granite" && (vm.service.typeOfStoneColor == "White")) {
                            vm.honingMeasurement.eightHundredCompare = 5;
                        }
                        else if (vm.service.serviceName != "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Black")) {
                            vm.honingMeasurement.fifteenHundredCompare = 5;
                        }
                        else if (vm.service.serviceName != "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Brown")) {
                            vm.honingMeasurement.fifteenHundredCompare = 5;
                        }
                        else if (vm.service.serviceName != "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Gray")) {
                            vm.honingMeasurement.fifteenHundredCompare = 5;
                        }
                        else if (vm.service.serviceName != "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Green")) {
                            vm.honingMeasurement.fifteenHundredCompare = 5;
                        }
                        else if (vm.service.serviceName != "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Red")) {
                            vm.honingMeasurement.fifteenHundredCompare = 5;
                        }
                        else if (vm.service.serviceName != "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Tan")) {
                            vm.honingMeasurement.fifteenHundredCompare = 5;
                        }
                        else if (vm.service.serviceName != "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "White")) {
                            vm.honingMeasurement.fifteenHundredCompare = 5;
                        }
                        else if (vm.service.serviceName == "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Black")) {
                            vm.honingMeasurement.fifteenHundredCompare = 0;
                        }
                        else if (vm.service.serviceName == "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Brown")) {
                            vm.honingMeasurement.fifteenHundredCompare = 0;
                        }
                        else if (vm.service.serviceName == "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Gray")) {
                            vm.honingMeasurement.fifteenHundredCompare = 0;
                        }
                        else if (vm.service.serviceName == "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Green")) {
                            vm.honingMeasurement.fifteenHundredCompare = 0;
                        }
                        else if (vm.service.serviceName == "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Red")) {
                            vm.honingMeasurement.fifteenHundredCompare = 0;
                        }
                        else if (vm.service.serviceName == "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "Tan")) {
                            vm.honingMeasurement.fifteenHundredCompare = 0;
                        }
                        else if (vm.service.serviceName == "Concrete Floor Prep - Grind" && vm.service.typeOfStoneType2 == "Concrete" && (vm.service.typeOfStoneColor == "White")) {
                            vm.honingMeasurement.fifteenHundredCompare = 0;
                        }
                        else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "Black")) {
                            vm.honingMeasurement.fifteenHundredCompare = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "Brown")) {
                            vm.honingMeasurement.fifteenHundredCompare = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "Gray")) {
                            vm.honingMeasurement.fifteenHundredCompare = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "Green")) {
                            vm.honingMeasurement.fifteenHundredCompare = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "Red")) {
                            vm.honingMeasurement.fifteenHundredCompare = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "Tan")) {
                            vm.honingMeasurement.fifteenHundredCompare = 5;
                        }
                        else if (vm.service.typeOfStoneType2 == "Terrazzo" && (vm.service.typeOfStoneColor == "White")) {
                            vm.honingMeasurement.fifteenHundredCompare = 5;
                        }
                    }
                }
            }
        ]
    );
}());