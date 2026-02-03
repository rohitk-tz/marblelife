(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("ManageJobController",
        ["$scope", "$rootScope", "$state", "$q", "SchedulerService", "FranchiseeService", "$uibModal",
            "Notification", "Clock", "Toaster", "$stateParams", "FileService", "$filter", "CustomerService", "$sce", "$location", "ToDoService", "$window", "EstimateService", "BeforeAfterService",
            function ($scope, $rootScope, $state, $q, schedulerService, franchiseeService, $uibModal,
                notification, clock, toaster, $stateParams, fileService, $filter, customerService, $sce, $location, toDoService, $window, estimateService, beforeAfterService) {

                var vm = this;
                vm.cropImage = cropImage;
                vm.changeAvailability = changeAvailability;
                vm.saveCustomerInvoice = saveCustomerInvoice;
                vm.isInvoiceEnhancement = false;
                vm.openSignatureModal = openSignatureModal;
                vm.sendMailToCustomer = sendMailToCustomer;
                vm.openSelectInvoicesModal = openSelectInvoicesModal;
                vm.isInvoiceEnhancement = false;
                vm.openNotesModal = openNotesModal;
                vm.changeInNumberOfInvoices = changeInNumberOfInvoices;
                vm.addingNotes = addingNotes;
                vm.isInvoicePresent = false;
                vm.serviceTypeForInvoiceList = [];
                vm.getEstimateInvoiceInfo = getEstimateInvoiceInfo;
                vm.isInvoiceEnhancement = false;
                vm.downloadInvoice = downloadInvoice;
                $scope.shiftingImageActive = false;
                vm.jobEstimateImageActive = {};
                vm.shiftImagesModel = {};
                vm.shiftingImage = shiftingImage;
                vm.shiftModal = shiftModal;
                vm.swapImages = swapImages;
                vm.openEstimateWorthModal = openEstimateWorthModal;
                vm.getImageSize = getImageSize;
                vm.index = 0;
                vm.isFromBeforeImage = true;
                vm.zoomClass = null;
                vm.zoomin = zoomin;
                vm.zoomout = zoomout;
                vm.creatingLightBox = creatingLightBox;
                vm.zoom = zoom;
                vm.downloadImage = downloadImage;
                vm.downloadImages = downloadImages;
                vm.uploadMediaAfterBeforeForCamera = uploadMediaAfterBeforeForCamera;
                vm.info = {};
                $scope.isMobile = navigator.userAgent.match(/(iPhone|iPod|iPad|Android|webOS|BlackBerry|IEMobile|Opera Mini)/i);
                vm.info.fileList = [];
                vm.deleteFromUI = deleteFromUI;
                vm.rotate = rotate;
                vm.openFollowUpModal = openFollowUpModal;
                vm.openViewFollowUpModal = openViewFollowUpModal;
                vm.getFollowUp = getFollowUp;
                vm.isGenernalTab = true;
                vm.isInvoiceTab = false;
                vm.isBuildinglTab = false;
                vm.isBeforeAfterlTab = false;
                vm.googleApi = googleApi;
                vm.magnify = magnify;
                vm.mouseOver = false;
                vm.changeTab = changeTab;
                vm.removeMagnify = removeMagnify;
                vm.isDeleted = false;
                vm.isRowDelete = false;

                vm.isPresentRequiredClass = false;
                vm.isSeletedIndex = -1;
                vm.bestPictureSelected = bestPictureSelected;
                vm.getIdName = getIdName;
                vm.jobEstimateImageDoublicate = [];
                vm.openPopUp = openPopUp;
                vm.isImageRemoved = false;
                vm.beforeImagesSizes = [];
                vm.afterImagesSizes = [];
                vm.beforeImagesHeight = [];
                vm.afterImagesHeight = [];
                vm.deleteToDo = deleteToDo;
                vm.getAllServicesCollection = getAllServicesCollection;
                vm.showJobDetails = false;   // collapsed by default

                vm.beforeAfterImagesFirstUploadedImages = [
                    [0, 0]
                ]
                // for uploading images in before and after having images already
                vm.countForHavingPaneWithImages = [
                    [0, 0] // first one for Before Image and Second One for After Image
                ]

                vm.imagePairMarkedQuery = {
                    isBestPairMarked: false,
                    isAddToLocalSiteGallery: false,
                    beforeImageserviceId: null,
                    afterImageserviceId: null,
                    beforeImagecategoryId: null,
                    afterImagecategoryId: null,
                    beforeImageFileId: null,
                    afterImageFileId: null
                }

                vm.surfaceMaterial = [
                    { display: "Brick", value: "Brick" },
                    { display: "Carpet", value: "Carpet" },
                    { display: "Ceramic (tile and grout)", value: "Ceramic (tile and grout)" },
                    { display: "Concrete", value: "Concrete" },
                    { display: "Engineered Stone:Cesar", value: "Engineered Stone:Cesar" },
                    { display: "Engineered Stone:Corian", value: "Engineered Stone:Corian" },
                    { display: "Engineered Stone:Other", value: "Engineered Stone:Other" },
                    { display: "Engineered Stone:Zodiaq", value: "Engineered Stone:Zodiaq" },
                    { display: "Flagstone", value: "Flagstone" },
                    { display: "Glass", value: "Glass" },
                    { display: "Granite", value: "Granite" },
                    { display: "Limestone", value: "Limestone" },
                    { display: "Marble", value: "Marble" },
                    { display: "Marble (Tumbled)", value: "Marble (Tumbled)" },
                    { display: "Metal", value: "Metal" },
                    { display: "Mexican Tile", value: "Mexican Tile" },
                    { display: "Porcelain", value: "Porcelain" },
                    { display: "Quartz", value: "Quartz" },
                    { display: "Slate", value: "Slate" },
                    { display: "Terrazzo", value: "Terrazzo" },
                    { display: "Travertine", value: "Travertine" },
                    { display: "Vinyl", value: "Vinyl" },
                    { display: "Wood", value: "Wood" },
                    { display: "Other", value: "Other" }
                ];

                vm.surfaceType = [
                    { display: "Baseboards", value: "Baseboards" },
                    { display: "Building Exterior", value: "Building Exterior" },
                    { display: "Counter (Bar)", value: "Counter (Bar)" },
                    { display: "Counter (Bathroom Vanity)", value: "Counter (Bathroom Vanity)" },
                    { display: "Counter (Butler Bar)", value: "Counter (Butler Bar)" },
                    { display: "Counter (Island)", value: "Counter (Island)" },
                    { display: "Counter (Kitchen)", value: "Counter (Kitchen)" },
                    { display: "Counter (Snack Bar)", value: "Counter (Snack Bar)" },
                    { display: "Fireplace", value: "Fireplace" },
                    { display: "Floor", value: "Floor" },
                    { display: "Floor (Garage)", value: "Floor (Garage)" },
                    { display: "Floor (Warehouse)", value: "Floor (Warehouse)" },
                    { display: "Fountain", value: "Fountain" },
                    { display: "Patio", value: "Patio" },
                    { display: "Pool Deck", value: "Pool Deck" },
                    { display: "Shower Bench", value: "Shower Bench" },
                    { display: "Shower Ceiling", value: "Shower Ceiling" },
                    { display: "Shower Curb", value: "Shower Curb" },
                    { display: "Shower Floor", value: "Shower Floor" },
                    { display: "Shower Wall", value: "Shower Wall" },
                    { display: "Sign", value: "Sign" },
                    { display: "Sink", value: "Sink" },
                    { display: "Stairs", value: "Stairs" },
                    { display: "Table", value: "Table" },
                    { display: "Table (Conference Room)", value: "Table (Conference Room)" },
                    { display: "Table (Vanity)", value: "Table (Vanity)" },
                    { display: "Threshold", value: "Threshold" },
                    { display: "Threshold (Door)", value: "Threshold (Door)" },
                    { display: "Threshold (Window)", value: "Threshold (Window)" },
                    { display: "Tub Deck", value: "Tub Deck" },
                    { display: "Walkway", value: "Walkway" },
                    { display: "Wall", value: "Wall" },
                    { display: "Window Sill", value: "Window Sill" },
                    { display: "Other", value: "Other" }
                ];

                vm.buildingLocation = [
                    { display: "Bar", value: "Bar" },
                    { display: "Basement", value: "Basement" },
                    { display: "Bathroom", value: "Bathroom" },
                    { display: "Bedroom", value: "Bedroom" },
                    { display: "Building Exterior", value: "Building Exterior" },
                    { display: "Conference Room", value: "Conference Room" },
                    { display: "Dining", value: "Dining" },
                    { display: "Elevator", value: "Elevator" },
                    { display: "Exterior Feature", value: "Exterior Feature" },
                    { display: "Floor", value: "Floor" },
                    { display: "Foyer(Entrance)", value: "Foyer(Entrance)" },
                    { display: "Garage", value: "Garage" },
                    { display: "Guest Room", value: "Guest Room" },
                    { display: "Hall", value: "Hall" },
                    { display: "Headstone", value: "Headstone" },
                    { display: "Kitchen", value: "Kitchen" },
                    { display: "Laundry", value: "Laundry" },
                    { display: "Living", value: "Living" },
                    { display: "Lobby", value: "Lobby" },
                    { display: "Patient Room", value: "Patient Room" },
                    { display: "Stairs", value: "Stairs" },
                    { display: "Other", value: "Other" }
                ];

                vm.buildingLocationForInvoice = [
                    { label: "Bar", id: "Bar" },
                    { label: "Basement", id: "Basement" },
                    { label: "Bathroom", id: "Bathroom" },
                    { label: "Bedroom", id: "Bedroom" },
                    { label: "Building Exterior", id: "Building Exterior" },
                    { label: "Conference Room", id: "Conference Room" },
                    { label: "Dining", id: "Dining" },
                    { label: "Elevator", id: "Elevator" },
                    { label: "Exterior Feature", id: "Exterior Feature" },
                    { label: "Floor", id: "Floor" },
                    { label: "Foyer(Entrance)", id: "Foyer(Entrance)" },
                    { label: "Garage", id: "Garage" },
                    { label: "Guest Room", id: "Guest Room" },
                    { label: "Hall", id: "Hall" },
                    { label: "Headstone", id: "Headstone" },
                    { label: "Kitchen", id: "Kitchen" },
                    { label: "Laundry", id: "Laundry" },
                    { label: "Living", id: "Living" },
                    { label: "Lobby", id: "Lobby" },
                    { label: "Patient Room", id: "Patient Room" },
                    { label: "Stairs", id: "Stairs" },
                    { label: "Other", id: "Other" }
                ];

                vm.surfaceColor = [
                    { display: "Black", value: "Black" },
                    { display: "Blue", value: "Blue" },
                    { display: "Brown", value: "Brown" },
                    { display: "Creme Marfil", value: "Creme Marfil" },
                    { display: "Gray", value: "Gray" },
                    { display: "Green", value: "Green" },
                    { display: "Maroon", value: "Maroon" },
                    { display: "Red", value: "Red" },
                    { display: "Tan", value: "Tan" },
                    { display: "White", value: "White" },
                    { display: "Other", value: "Other" }
                ];

                vm.finishMaterial = [
                    { display: "Gloss", value: "Gloss" },
                    { display: "Matte", value: "Matte" },
                    { display: "Satin/Semi-Gloss", value: "Satin/Semi-Gloss" },
                    { display: "Ultimate Finish", value: "Ultimate Finish" },
                    { display: "Other", value: "Other" }
                ];

                vm.stoneTypeForInvoice1 = [
                    { display: "Brick", value: "Brick" },
                    { display: "Carpet", value: "Carpet" },
                    { display: "Ceramic (tile and grout)", value: "Ceramic (tile and grout)" },
                    { display: "Concrete", value: "Concrete" },
                    { display: "Engineered Stone:Cesar", value: "Engineered Stone:Cesar" },
                    { display: "Engineered Stone:Corian", value: "Engineered Stone:Corian" },
                    { display: "Engineered Stone:Other", value: "Engineered Stone:Other" },
                    { display: "Engineered Stone:Zodiaq", value: "Engineered Stone:Zodiaq" },
                    { display: "Flagstone", value: "Flagstone" },
                    { display: "Glass", value: "Glass" },
                    { display: "Granite", value: "Granite" },
                    { display: "Limestone", value: "Limestone" },
                    { display: "Marble", value: "Marble" },
                    { display: "Marble (Tumbled)", value: "Marble (Tumbled)" },
                    { display: "Metal", value: "Metal" },
                    { display: "Mexican Tile", value: "Mexican Tile" },
                    { display: "Porcelain", value: "Porcelain" },
                    { display: "Quartz", value: "Quartz" },
                    { display: "Slate", value: "Slate" },
                    { display: "Terrazzo", value: "Terrazzo" },
                    { display: "Travertine", value: "Travertine" },
                    { display: "Vinyl", value: "Vinyl" },
                    { display: "Wood", value: "Wood" },
                    { display: "Other", value: "Other" }
                ];


                vm.serviceTypeForInvoice = [
                    { label: "Repair Chip", id: "Repair Chip" },
                    { label: "Repair Crack", id: "Repair Crack" },
                    { label: "Hone", id: "Hone" },
                    { label: "Polish", id: "Polish" },
                    { label: "Seal", id: "Seal" }
                ];

                vm.serviceTypeForInvoiceList.push(vm.serviceTypeForInvoice);
                vm.locationForInvoice = [
                    { display: "Bar", value: "Bar" },
                    { display: "Basement", value: "Basement" },
                    { display: "Bathroom", value: "Bathroom" },
                    { display: "Bedroom", value: "Bedroom" },
                    { display: "Building Exterior", value: "Building Exterior" },
                    { display: "Conference Room", value: "Conference Room" },
                    { display: "Dining", value: "Dining" },
                    { display: "Elevator", value: "Elevator" },
                    { display: "Exterior Feature", value: "Exterior Feature" },
                    { display: "Floor", value: "Floor" },
                    { display: "Foyer(Entrance)", value: "Foyer(Entrance)" },
                    { display: "Garage", value: "Garage" },
                    { display: "Guest Room", value: "Guest Room" },
                    { display: "Hall", value: "Hall" },
                    { display: "Headstone", value: "Headstone" },
                    { display: "Kitchen", value: "Kitchen" },
                    { display: "Laundry", value: "Laundry" },
                    { display: "Living", value: "Living" },
                    { display: "Lobby", value: "Lobby" },
                    { display: "Patient Room", value: "Patient Room" },
                    { display: "Stairs", value: "Stairs" },
                    { display: "Other", value: "Other" }
                ];

                vm.surfaceTypeForInvoice = [
                    { display: "Baseboards", value: "Baseboards" },
                    { display: "Building Exterior", value: "Building Exterior" },
                    { display: "Counter (Bar)", value: "Counter (Bar)" },
                    { display: "Counter (Bathroom Vanity)", value: "Counter (Bathroom Vanity)" },
                    { display: "Counter (Butler Bar)", value: "Counter (Butler Bar)" },
                    { display: "Counter (Island)", value: "Counter (Island)" },
                    { display: "Counter (Kitchen)", value: "Counter (Kitchen)" },
                    { display: "Counter (Snack Bar)", value: "Counter (Snack Bar)" },
                    { display: "Fireplace", value: "Fireplace" },
                    { display: "Floor", value: "Floor" },
                    { display: "Floor (Garage)", value: "Floor (Garage)" },
                    { display: "Floor (Warehouse)", value: "Floor (Warehouse)" },
                    { display: "Fountain", value: "Fountain" },
                    { display: "Patio", value: "Patio" },
                    { display: "Pool Deck", value: "Pool Deck" },
                    { display: "Shower Bench", value: "Shower Bench" },
                    { display: "Shower Ceiling", value: "Shower Ceiling" },
                    { display: "Shower Curb", value: "Shower Curb" },
                    { display: "Shower Floor", value: "Shower Floor" },
                    { display: "Shower Wall", value: "Shower Wall" },
                    { display: "Sign", value: "Sign" },
                    { display: "Sink", value: "Sink" },
                    { display: "Stairs", value: "Stairs" },
                    { display: "Table", value: "Table" },
                    { display: "Table (Conference Room)", value: "Table (Conference Room)" },
                    { display: "Table (Vanity)", value: "Table (Vanity)" },
                    { display: "Threshold", value: "Threshold" },
                    { display: "Threshold (Door)", value: "Threshold (Door)" },
                    { display: "Threshold (Window)", value: "Threshold (Window)" },
                    { display: "Tub Deck", value: "Tub Deck" },
                    { display: "Walkway", value: "Walkway" },
                    { display: "Wall", value: "Wall" },
                    { display: "Window Sill", value: "Window Sill" },
                    { display: "Other", value: "Other" }
                ];

                vm.surfaceType = [
                    { display: "Bar", value: "Bar" },
                    { display: "Building Exterior", value: "Building Exterior" },
                    { display: "Butler Bar", value: "Butler Bar" },
                    { display: "Concrete", value: "Concrete" },
                    { display: "Conference Room", value: "Conference Room" },
                    { display: "Counter (Kitchen)", value: "Counter (Kitchen)" },
                    { display: "Fireplace", value: "Fireplace" },
                    { display: "Floor", value: "Floor" },
                    { display: "Floor (Garage)", value: "Floor (Garage)" },
                    { display: "Floor (Warehouse)", value: "Floor (Warehouse)" },
                    { display: "Fountain", value: "Fountain" },
                    { display: "Island", value: "Island" },
                    { display: "Patio", value: "Patio" },
                    { display: "Pool Deck", value: "Pool Deck" },
                    { display: "Shower Bench", value: "Shower Bench" },
                    { display: "Shower Ceiling", value: "Shower Ceiling" },
                    { display: "Shower Curb", value: "Shower Curb" },
                    { display: "Shower Floor", value: "Shower Floor" },
                    { display: "Shower Wall", value: "Shower Wall" },
                    { display: "Sign", value: "Sign" },
                    { display: "Snack Bar", value: "Snack Bar" },
                    { display: "Stairs", value: "Stairs" },
                    { display: "Table", value: "Table" },
                    { display: "Threshold", value: "Threshold" },
                    { display: "Threshold (Door)", value: "Threshold (Door)" },
                    { display: "Threshold (Window)", value: "Threshold (Window)" },
                    { display: "Tub Deck", value: "Tub Deck" },
                    { display: "Vanity (Bathroom)", value: "Vanity (Bathroom)" },
                    { display: "Walkway", value: "Walkway" },
                    { display: "Wall", value: "Wall" },
                    { display: "Window Sill", value: "Window Sill" },
                    { display: "Other", value: "Other" }
                ];


                vm.stoneTypeForInvoice = [
                    { display: "Gloss", value: "Gloss" },
                    { display: "Matte", value: "Matte" },
                    { display: "Satin/Semi-Gloss", value: "Satin/Semi-Gloss" },
                    { display: "Ultimate Finish", value: "Ultimate Finish" },
                    { display: "Other", value: "Other" }
                ];


                vm.stoneColorForInvoice = [
                    { display: "Black", value: "Black" },
                    { display: "Blue", value: "Blue" },
                    { display: "Brown", value: "Brown" },
                    { display: "Creme Marfil", value: "Creme Marfil" },
                    { display: "Gray", value: "Gray" },
                    { display: "Green", value: "Green" },
                    { display: "Maroon", value: "Maroon" },
                    { display: "Red", value: "Red" },
                    { display: "Tan", value: "Tan" },
                    { display: "White", value: "White" },
                    { display: "Other", value: "Other" }
                ];


                vm.invoiceNumberForInvoice = [];
                vm.invoiceNumberForInvoice = [{ display: "1", value: "1" }];

                $scope.settings = {
                    scrollable: true,
                    enableSearch: true,
                    selectedToTop: true,
                    buttonClasses: 'btn btn-primary leader_btn',

                };
                $scope.translationTexts = {
                    checkAll: 'Select All',
                    uncheckAll: 'Deselect All',
                    selectGroup: 'Select All',
                    buttonDefaultText: "Select",
                    dynamicButtonTextSuffix: 'Selected'
                };

                $scope.translationTextsLocation = {
                    checkAll: 'Select All',
                    uncheckAll: 'Deselect All',
                    selectGroup: 'Select All',
                    buttonDefaultText: "Select",
                    dynamicButtonTextSuffix: 'Selected'
                };

                $scope.selectEventsLocation = {
                    onItemSelect: function (item) {

                    },
                    onItemDeselect: function (item) {
                    },
                    onSelectAll: function (item) {
                    },
                    onDeselectAll: function (item) {
                    }
                }
                $scope.selectEvents = {
                    onItemSelect: function (item) {
                        var service = vm.estimateInvoiceInfo.serviceList[vm.index];

                        var bundleIndex = item.id.indexOf("Bundle");
                        if (service.description == '') {
                            service.serviceIds[0] = (item);
                            if (bundleIndex > -1) {
                                service.isExpand = true;
                                var services = vm.estimateInvoiceInfo.serviceList[vm.index];
                                services.description = item.id;
                                vm.addingBundles(service, item.id, false, vm.index);
                                isContainStripping(service);
                                return;
                            }
                            vm.changeService(vm.index);
                            getSalesPersonNotes(service, vm.index);
                        }
                        else if (service.serviceIds.length > 1) {
                            service.isExpand = true;
                            if (bundleIndex > -1) {
                                service.subItem.push({
                                    serviceIds: { id: item.id },
                                    option1: 0,
                                    option2: 0,
                                    option3: 0,
                                    description: item.id,
                                    notes: '',
                                    serviceType: service.serviceType,
                                    id: 0,
                                    isBundle: false,
                                    isActive: true,
                                    isMainBundle: true
                                });
                                vm.addingBundles(service, item.id, true, vm.index);
                                isContainStripping(service);
                                return;
                            }
                            service.subItem.push({
                                serviceIds: { id: item.id },
                                option1: 0,
                                option2: 0,
                                option3: 0,
                                description: '',
                                notes: '',
                                serviceType: service.serviceType,
                                id: 0,
                                isBundle: false,
                                isMainBundle: false,
                                bundleName: '',
                                isActive: true,
                                salesPersonNote: getBundleSalesPersonNotes(service.serviceType, service.typeOfStoneType2, item.id)[0],
                                informationNote: getBundleSalesPersonNotes(service.serviceType, service.typeOfStoneType2, item.id)[1],
                                class: "blinka"
                            });
                            changeServiceForSubItem(vm.index);
                            isContainStripping(service);
                        }
                        else {
                            if (bundleIndex > -1) {
                                service.isExpand = true;
                                vm.addingBundles(service, item.id, false, vm.index);
                                return;
                            }
                            vm.changeService(vm.index);
                            getSalesPersonNotes(service, vm.index);
                            isContainStripping(service);
                        }

                        isContainStripping(service);
                    },
                    onItemDeselect: function (item) {
                        var bundleIndex = item.id.indexOf("Bundle");
                        var serviceMain = vm.estimateInvoiceInfo.serviceList[vm.index];
                        var service = vm.estimateInvoiceInfo.serviceList[vm.index].subItem;

                        if (bundleIndex > -1) {
                            var serviceList = $filter('filter')(service, { bundleName: item.id }, true);
                            angular.forEach(serviceList, function (value1) {
                                var childServiceIndex = service.indexOf($filter('filter')(service, value1, true)[0]);
                                service.splice(childServiceIndex, 1);
                            });
                            var index = service.indexOf($filter('filter')(service, { serviceIds: item }, true)[0]);
                            service.splice(index, 1);
                            var mainBundleCount = $filter('filter')(service, { isMainBundle: true }, true);

                            var mainBundleIndex = service.indexOf($filter('filter')(service, { isMainBundle: true }, true)[0]);

                            if (serviceMain.serviceIds.length == mainBundleCount.length) {
                                serviceMain.description = service[mainBundleIndex].description;
                                serviceMain.serviceIds[0] = service[mainBundleIndex].serviceIds;
                                service.splice(mainBundleIndex, 1);
                            }
                            return;
                        }

                        index = service.indexOf($filter('filter')(service, { serviceIds: item, isBundle: false }, true)[0]);
                        if (index > -1) {
                            service.splice(index, 1);
                        }
                        else {
                            var serviceLocal = vm.estimateInvoiceInfo.serviceList[vm.index];
                            index = serviceLocal.serviceIds.indexOf($filter('filter')(serviceLocal.serviceIds, { id: item }, true)[0]);
                            if (index > -1) {
                                serviceLocal.splice(index, 1);
                                serviceLocal.serviceIds = [];
                                serviceLocal.description = '';
                            }
                            else {
                                var serviceLocal = vm.estimateInvoiceInfo.serviceList[vm.index];
                                //serviceLocal.serviceIds = [];
                                var serviceSubLocalList = vm.estimateInvoiceInfo.serviceList[vm.index].subItem;
                                serviceLocal.description = '';
                                serviceLocal.salesPersonNote = '';
                                serviceLocal.informationNote = '';
                                if (serviceSubLocalList.length > 0) {
                                    var serviceSubLocal = serviceSubLocalList[0];
                                    serviceLocal.option1 = serviceSubLocal.option1;
                                    serviceLocal.option2 = serviceSubLocal.option2;
                                    serviceLocal.option3 = serviceSubLocal.option3;
                                    serviceLocal.priceNotes = serviceSubLocal.priceNotes;
                                    serviceLocal.description = serviceSubLocal.description;
                                    serviceLocal.salesPersonNote = serviceSubLocal.salesPersonNote;
                                    serviceLocal.informationNote = serviceSubLocal.informationNote;
                                    serviceLocal.class = '';
                                    serviceLocal.subItem.splice(0, 1);
                                }
                            }
                            totalSumForOptions();
                        }
                        totalSumForOptions();
                    },
                    onSelectAll: function (item) {
                        var servicessList = [];
                        var serviceList = vm.serviceTypeForInvoiceList[vm.index];
                        var service = vm.estimateInvoiceInfo.serviceList[vm.index];
                        angular.forEach(serviceList, function (value1) {
                            servicessList.push(value1);
                            service.subItem.push({
                                serviceIds: { id: value1.id },
                                option1: 0,
                                option2: 0,
                                option3: 0,
                                description: '',
                                notes: '',
                                serviceType: service.serviceType,
                                salesPersonNote: getBundleSalesPersonNotes(service.serviceType, service.typeOfStoneType2, value1.id)[0],
                                informationNote: getBundleSalesPersonNotes(service.serviceType, service.typeOfStoneType2, value1.id)[1],
                                class: "blinka"
                            });
                        });
                        service.serviceIds = servicessList;
                        changeServiceForSubItem(vm.index);
                    },
                    onDeselectAll: function (item) {
                        var services = vm.estimateInvoiceInfo.serviceList[vm.index];
                        services.serviceIds = [];
                        services.subItem = [];
                        services.description = '';
                        services.notes = '';
                        services.option1 = 0;
                        services.option2 = 0;
                        services.option3 = 0;
                        vm.option1 = 0;
                        vm.option2 = 0;
                        vm.option3 = 0;
                        changeServiceForSubItem(vm.index);
                    }
                }


                var isBeforeAfterActiveFromSession = ($window.sessionStorage.getItem("IsBeforeAfterTabAtive"));
                vm.getListForBeforeAfterDropDown = getListForBeforeAfterDropDown;
                $scope.franchiseeId = "";
                vm.dropDownForBeforeAfter = dropDownForBeforeAfter;
                vm.isImageForSamePane = false;
                vm.navigationDate = moment(new Date());
                vm.isCopy = false;
                vm.getTitle = getTitle;
                vm.isConfirmDisable = false;
                vm.jobId = $stateParams.id != null ? $stateParams.id : 0;
                vm.rowId = $stateParams.rowId != null ? $stateParams.rowId : 0;
                vm.previousView = $stateParams.previousView != null ? $stateParams.previousView : null;
                vm.showMediaTab = false;
                vm.changeJobStatus = changeJobStatus;
                vm.validQB = false;
                vm.updateQbInvoice = updateQbInvoice;
                vm.gotoScheduler = gotoScheduler;
                vm.editJob = editJob;
                vm.deleteJob = deleteJob;
                vm.jobStatus = [];
                $scope.myInterval = 5000;
                $scope.noWrapSlides = false;
                $scope.active = 0;
                vm.jobNoteEdit = jobNoteEdit;
                vm.jobNoteDelete = jobNoteDelete;
                var slidesJobs = $scope.slidesJobs = [];
                var slides = $scope.slides = [];
                var currIndex = 0;
                vm.checkQBNumber = checkQbNumber;
                vm.uploadMedia = uploadMedia;
                vm.goToEstimate = goToEstimate;
                vm.addNote = addNote;
                vm.Roles = DataHelper.Role;
                vm.isTech = $rootScope.identity.roleId == vm.Roles.Technician;
                vm.isSalesRep = $rootScope.identity.roleId == vm.Roles.SalesRep;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.isFranchiseeAdmin = $rootScope.identity.roleId == vm.Roles.FranchiseeAdmin;
                vm.isOpsMgr = $rootScope.identity.roleId == vm.Roles.OperationsManager;
                vm.isFrontOffice = $rootScope.identity.roleId == vm.Roles.FrontOfficeExecutive;
                vm.MediaType = DataHelper.ScheduleType;
                vm.marketingClassForNote = DataHelper.NonResidentialCommercialClass;
                vm.isRepeative = false;
                vm.addNew = addNew;
                vm.repeatJob = repeatJob;
                vm.confirmChanging = confirmChanging;
                vm.confirmJob = confirmJob;
                vm.remove = remove;
                vm.removeImage = removeImage;
                vm.query = {};
                vm.confirmationQuery = {};;
                vm.jobEstimateImage = [];
                vm.jobEstimateImageForPrint = [];
                vm.BeforeAfterImages = DataHelper.BeforeAfterImages;
                vm.ConfirmationEnum = DataHelper.Confirmation;
                vm.isEdit = true;
                vm.invoiceMail = invoiceMail;
                vm.manageJob = manageJob;
                vm.printSection = [];
                vm.isAfterBeforeConditionSet = false;
                vm.isAfterConditionSet = false;
                vm.isBeforeConditionSet = false;
                vm.OptionChanged = OptionChanged;
                vm.isEligible = true;
                vm.deleteRow = deleteRow;
                vm.fileModel = null;
                vm.isPrintable = false;
                vm.isResidential = false;
                vm.isMediaSave = false;
                vm.isBeforeAFterSave = false;
                vm.alreadyUploaded = false;
                vm.isFirstAttempt = true;
                vm.printSectionJob = printSectionJob;
                vm.floorNoChange = floorNoChange;
                vm.copyJob = copyJob;
                vm.mediaModel = {};
                vm.isMobile = navigator.userAgent.match(/(iPhone|iPod|iPad|Android|webOS|BlackBerry|IEMobile|Opera Mini)/i);
                vm.imageParentChildListModel = {
                    imagePairs: [{
                        beforeImages: {},
                        afterImages: {}
                    }
                    ],
                };;

                vm.imageList = {
                    imagePairs: [{
                        beforeImages: {},
                        afterImages: {}
                    }
                    ],
                };

                vm.imageFileList = {
                    fileList: []
                };

                vm.save = save;
                vm.beforeAfterMail = beforeAfterMail;
                vm.uploadMediaAfterBefore = uploadMediaAfterBefore;
                vm.uploadMediaBuildingInvoice = uploadMediaBuildingInvoice;
                vm.BeforeAfterLookUp = DataHelper.BeforeAfterImages;
                vm.serviceTypeLookUp = DataHelper.ServiceTypes;
                vm.marketingClassLookUp = DataHelper.MarketingTypes;
                vm.isGroutilife = false;
                vm.serviceSelectionChange = serviceSelectionChange;
                vm.CategoryTypeChange = CategoryTypeChange;
                vm.noOfUnPaired = 0;
                vm.isBeforeAfterImagesSave = true;
                vm.isInvoiceSave = true;
                vm.isExteriorImageSave = true;
                vm.isGeneralTabActive = true;
                vm.isInvoiceTabActive = false;
                vm.isExteriorImageActive = false;
                vm.isBeforeAfterActive = false;
                var isBeforeAfterActiveFromSession = ($window.sessionStorage.getItem("IsBeforeAfterTabAtive"));
                if (isBeforeAfterActiveFromSession == 'true') {
                    changeTabFocus(4);
                    changeTab(4);
                }
                vm.previousTab = "1";

                function printSectionJob(imgs, $index) {
                    vm.jobEstimateImageForPrint = [];
                    vm.jobEstimateImageForPrint.push(imgs[$index]);
                }

                function printElement(elem, id, show) {
                    var isVisible = true;
                    $('#sizer').hide();
                    //$('#' + id).css('width', '50%');
                    var a = $('#' + id).clone();
                    if (a[0].attributes[2] != undefined && a[0].attributes[2].value != null) {
                        a[0].attributes[2].value = "display:block";
                        isVisible = false;
                    }
                    $('body *:visible').addClass("hide-during-print").hide();
                    $('body').append('<html><style> div#sizer {height:0px}@page{size:landscape; margin-bottom:50px;height: 9%;} @media print {md-content {overflow: visible;}}</style><head><title></title></head>');
                    $('body').append(a);
                    $('body').append('</html>');
                    if (show)
                        window.print();
                    if (!isVisible) {
                        a[0].attributes[2].value = "display:none";
                    }
                    $('body .hide-during-print').show().removeClass("hide-during-print");
                    a.remove();
                }

                function deleteRow(imgs, $index) {
                    notification.showConfirm("This image will also be deleted from the Bulk Photo Upload area.\nDo you really want to delete the Before-After Images?", "Delete Before-After Images", function () {
                        vm.isRowDelete = true;
                        vm.isBeforeAfterImagesSave = false;
                        var index = imgs[$index];
                        imgs.splice($index, 1);
                        rearrangePaneNumber();
                        getListForBeforeAfterDropDown();
                        save(true);
                        toaster.show("Media Deleated Successfully");
                    });
                }
                function OptionChanged(imgs, isBeforeImage) {
                    if (isBeforeImage) {
                        vm.isBeforeAfterImagesSave = false;
                        imgs.afterImages.buildingLocation = imgs.beforeImages.buildingLocation;
                        imgs.afterImages.surfaceMaterial = imgs.beforeImages.surfaceMaterial;
                        imgs.afterImages.surfaceType = imgs.beforeImages.surfaceType;
                        imgs.afterImages.isBeforeImage = true;
                        imgs.beforeImages.isBeforeImage = true;
                        vm.isBeforeConditionSet = true;
                        imgs.afterImages.isDisable = true;
                        imgs.beforeImages.isDisable = false;
                    }
                    else {
                        vm.isBeforeAfterImagesSave = false;
                        imgs.beforeImages.buildingLocation = imgs.afterImages.buildingLocation;
                        imgs.beforeImages.surfaceMaterial = imgs.afterImages.surfaceMaterial;
                        imgs.beforeImages.surfaceType = imgs.afterImages.surfaceType;
                        imgs.afterImages.isBeforeImage = false;
                        imgs.beforeImages.isBeforeImage = false;
                        imgs.beforeImages.isDisable = true;
                        imgs.afterImages.isDisable = false;
                        vm.isAfterConditionSet = true;
                    }
                }

                function invoiceMail(imagesInfo) {
                    notification.showConfirm("Kindly save the newly uploaded images, unsaved images will not be sent. Do you still want to continue?", "Send Mail", function () {
                        schedulerService.invoiceSendMail(imagesInfo, vm.rowId).then(function (result) {
                            if (result.data != null) {
                                if (!result.data) {
                                    vm.isProcessing = false;
                                    toaster.error(result.message.message);
                                    getJobInfo();
                                }
                                else {
                                    vm.isProcessing = false;
                                    toaster.show(result.message.message);
                                    vm.imageParentChildListModel.beforeAfterImages = [];
                                    getJobInfo();
                                }
                                vm.isProcessing = false;
                            }
                        }).catch(function (err) {
                            vm.isProcessing = false;
                        });
                    });
                }
                function beforeAfterMail(imgs, id) {
                    if (imgs.beforeImages.id == 0 || imgs.afterImages.id == 0) {
                        toaster.error("Please Save Before/After Pair First!!");
                        return;
                    }
                    notification.showConfirm("Kindly save the newly uploaded images, unsaved images will not be sent. Do you still want to continue?", "Send Mail", function () {

                        var modalInstance = $uibModal.open({
                            animation: true,
                            templateUrl: 'modules/scheduler/views/modal.send.mail.to.customers.client.view.html',
                            controller: 'ModalSendMailToCustomerController',
                            controllerAs: 'vm',
                            size: 'lg',
                            resolve: {
                                modalParam: function () {
                                    return {
                                        Imgs: imgs,
                                        Id: vm.rowId
                                    };
                                }
                            },
                            backdrop: 'static',
                        });
                        modalInstance.result.then(function () {
                            vm.imageParentChildListModel.beforeAfterImages = [];
                            getJobInfo();
                        });
                    });
                }
                function CategoryTypeChange(img) {
                    vm.isBeforeAfterImagesSave = false;
                    if (vm.BeforeAfterLookUp.Before == img.typeId) {
                        img.text = 'Before';

                    }
                    else if (vm.BeforeAfterLookUp.During == img.typeId) {
                        img.text = 'During';
                    }

                }
                function serviceSelectionChange(img, isFrombefore, imgs, mainObject, changedProperty) {
                    vm.isBeforeAfterImagesSave = false;
                    if (vm.serviceTypeLookUp.GROUTLIFE == img.serviceTypeId || vm.serviceTypeLookUp.CONCRETECOUNTERTOPS == img.serviceTypeId
                        || vm.serviceTypeLookUp.CONCRETEOVERLAYMENTS == img.serviceTypeId || vm.serviceTypeLookUp.CONCRETECOATINGS == img.serviceTypeId) {
                        img.isGroutilife = true;

                    }
                    else {
                        img.isGroutilife = false;

                    }

                    if (mainObject.isSelectedProperty == null) {
                        mainObject.isSelectedProperty = isFrombefore == true ? true : false;
                    }

                    if (mainObject.isSelectedProperty != isFrombefore) {
                        if (changedProperty == 'ServiceType') {
                            mainObject.isSelectedServiceTypeProperty = true;

                        }
                        if (changedProperty == 'SurfaceColor') {
                            mainObject.isSelectedSurfaceColorProperty = true;
                        }
                        if (changedProperty == 'FinishMaterial') {
                            mainObject.isSelectedFinishMaterialProperty = true;
                        }
                        if (changedProperty == 'CompanyName') {
                            mainObject.isSelectedCompanyNameProperty = true;
                        }
                        if (changedProperty == 'MaidService') {
                            mainObject.isSelectedMaidServiceProperty = true;
                        }
                        if (changedProperty == 'PropertyManager') {
                            mainObject.isSelectedPropertyManagerProperty = true;
                        }
                        if (changedProperty == 'MaidJanitorial') {
                            mainObject.isSelectedMaidJanitorialProperty = true;
                        }
                    }

                    if (mainObject.isSelectedProperty == isFrombefore && !mainObject.isChanged) {
                        if (mainObject.isSelectedServiceTypeProperty == false && changedProperty == 'ServiceType') {
                            imgs.serviceTypeId = img.serviceTypeId;
                            if (img.isGroutilife) {
                                imgs.isGroutilife = true;
                            }
                            else {
                                imgs.isGroutilife = false;
                            }
                        }
                        if (mainObject.isSelectedFinishMaterialProperty == false && changedProperty == 'FinishMaterial') {
                            imgs.finishMaterial = img.finishMaterial;
                        }
                        if (mainObject.isSelectedSurfaceColorProperty == false && changedProperty == 'SurfaceColor') {
                            imgs.surfaceColor = img.surfaceColor;
                        }
                        if (mainObject.isSelectedCompanyNameProperty == false && changedProperty == 'CompanyName') {
                            imgs.compamyName = img.compamyName;
                        }
                        if (mainObject.isSelectedMaidServiceProperty == false && changedProperty == 'MaidService') {
                            imgs.maidService = img.maidService;
                        }
                        if (mainObject.isSelectedPropertyManagerProperty == false && changedProperty == 'PropertyManager') {
                            imgs.propertyManager = img.propertyManager;
                        }
                        if (mainObject.isSelectedMaidJanitorialProperty == false && changedProperty == 'MaidJanitorial') {
                            imgs.maidJanitorial = img.maidJanitorial;
                        }
                    }
                }
                if (vm.isTech) {
                    vm.isEdit = false;
                }

                vm.typeId = [{ display: "Before", value: vm.BeforeAfterLookUp.Before }, { display: "During", value: vm.BeforeAfterLookUp.During }];

                vm.surfaceMaterial = [
                    { display: "Brick", value: "Brick" },
                    { display: "Carpet", value: "Carpet" },
                    { display: "Ceramic (tile and grout)", value: "Ceramic (tile and grout)" },
                    { display: "Concrete", value: "Concrete" },
                    { display: "Engineered Stone:Cesar", value: "Engineered Stone:Cesar" },
                    { display: "Engineered Stone:Corian", value: "Engineered Stone:Corian" },
                    { display: "Engineered Stone:Other", value: "Engineered Stone:Other" },
                    { display: "Engineered Stone:Zodiaq", value: "Engineered Stone:Zodiaq" },
                    { display: "Flagstone", value: "Flagstone" },
                    { display: "Glass", value: "Glass" },
                    { display: "Granite", value: "Granite" },
                    { display: "Limestone", value: "Limestone" },
                    { display: "Marble", value: "Marble" },
                    { display: "Marble (Tumbled)", value: "Marble (Tumbled)" },
                    { display: "Metal", value: "Metal" },
                    { display: "Mexican Tile", value: "Mexican Tile" },
                    { display: "Porcelain", value: "Porcelain" },
                    { display: "Quartz", value: "Quartz" },
                    { display: "Slate", value: "Slate" },
                    { display: "Terrazzo", value: "Terrazzo" },
                    { display: "Travertine", value: "Travertine" },
                    { display: "Vinyl", value: "Vinyl" },
                    { display: "Wood", value: "Wood" },
                    { display: "Other", value: "Other" }
                ];

                vm.surfaceType = [
                    { display: "Baseboards", value: "Baseboards" },
                    { display: "Building Exterior", value: "Building Exterior" },
                    { display: "Counter (Bar)", value: "Counter (Bar)" },
                    { display: "Counter (Bathroom Vanity)", value: "Counter (Bathroom Vanity)" },
                    { display: "Counter (Butler Bar)", value: "Counter (Butler Bar)" },
                    { display: "Counter (Island)", value: "Counter (Island)" },
                    { display: "Counter (Kitchen)", value: "Counter (Kitchen)" },
                    { display: "Counter (Snack Bar)", value: "Counter (Snack Bar)" },
                    { display: "Fireplace", value: "Fireplace" },
                    { display: "Floor", value: "Floor" },
                    { display: "Floor (Garage)", value: "Floor (Garage)" },
                    { display: "Floor (Warehouse)", value: "Floor (Warehouse)" },
                    { display: "Fountain", value: "Fountain" },
                    { display: "Patio", value: "Patio" },
                    { display: "Pool Deck", value: "Pool Deck" },
                    { display: "Shower Bench", value: "Shower Bench" },
                    { display: "Shower Ceiling", value: "Shower Ceiling" },
                    { display: "Shower Curb", value: "Shower Curb" },
                    { display: "Shower Floor", value: "Shower Floor" },
                    { display: "Shower Wall", value: "Shower Wall" },
                    { display: "Sign", value: "Sign" },
                    { display: "Sink", value: "Sink" },
                    { display: "Stairs", value: "Stairs" },
                    { display: "Table", value: "Table" },
                    { display: "Table (Conference Room)", value: "Table (Conference Room)" },
                    { display: "Table (Vanity)", value: "Table (Vanity)" },
                    { display: "Threshold", value: "Threshold" },
                    { display: "Threshold (Door)", value: "Threshold (Door)" },
                    { display: "Threshold (Window)", value: "Threshold (Window)" },
                    { display: "Tub Deck", value: "Tub Deck" },
                    { display: "Walkway", value: "Walkway" },
                    { display: "Wall", value: "Wall" },
                    { display: "Window Sill", value: "Window Sill" },
                    { display: "Other", value: "Other" }
                ];

                vm.buildingLocation = [
                    { display: "Bar", value: "Bar" },
                    { display: "Basement", value: "Basement" },
                    { display: "Bathroom", value: "Bathroom" },
                    { display: "Bedroom", value: "Bedroom" },
                    { display: "Building Exterior", value: "Building Exterior" },
                    { display: "Conference Room", value: "Conference Room" },
                    { display: "Dining", value: "Dining" },
                    { display: "Elevator", value: "Elevator" },
                    { display: "Exterior Feature", value: "Exterior Feature" },
                    { display: "Floor", value: "Floor" },
                    { display: "Foyer(Entrance)", value: "Foyer(Entrance)" },
                    { display: "Garage", value: "Garage" },
                    { display: "Guest Room", value: "Guest Room" },
                    { display: "Hall", value: "Hall" },
                    { display: "Headstone", value: "Headstone" },
                    { display: "Kitchen", value: "Kitchen" },
                    { display: "Laundry", value: "Laundry" },
                    { display: "Living", value: "Living" },
                    { display: "Lobby", value: "Lobby" },
                    { display: "Patient Room", value: "Patient Room" },
                    { display: "Stairs", value: "Stairs" },
                    { display: "Other", value: "Other" }
                ];


                vm.surfaceColor = [
                    { display: "Black", value: "Black" },
                    { display: "Blue", value: "Blue" },
                    { display: "Brown", value: "Brown" },
                    { display: "Creme Marfil", value: "Creme Marfil" },
                    { display: "Gray", value: "Gray" },
                    { display: "Green", value: "Green" },
                    { display: "Maroon", value: "Maroon" },
                    { display: "Red", value: "Red" },
                    { display: "Tan", value: "Tan" },
                    { display: "White", value: "White" },
                    { display: "Other", value: "Other" }
                ];

                vm.finishMaterial = [
                    { display: "Gloss", value: "Gloss" },
                    { display: "Matte", value: "Matte" },
                    { display: "Satin/Semi-Gloss", value: "Satin/Semi-Gloss" },
                    { display: "Ultimate Finish", value: "Ultimate Finish" },
                    { display: "Other", value: "Other" }
                ];


                function removeImage(slide, $index, value, isFromBefore, parentIndex) {
                    if ((vm.isSalesRep == vm.Roles.SalesRep || vm.isTech == vm.Roles.Technician)
                        && (slide[0].uploadByRoleId != vm.isTech || (slide[0].uploadByRoleId != vm.isSalesRep))) {
                        toaster.error("Permission to remove media is not allowed. Kindly contact Administrator!");
                    }
                    var parentIndexes = parentIndex;
                    vm.query.userId = slide[$index].userId;
                    vm.query.uploadDateTime = slide[$index].createdOn;
                    if (!vm.isSuperAdmin && !vm.isFrontOffice && vm.query.userId != undefined) {
                        return schedulerService.isEligibleForDeletion(vm.query).then(function (result) {
                            vm.isEligible = result.data;
                            if (!vm.isEligible) {
                                toaster.error("Permission to remove media is not allowed. Kindly contact Administrator!");
                            }
                            else {
                                notification.showConfirm("This image will also be deleted from Bulk photo upload area. Do you really want to delete the Image?", "Delete Image", function () {
                                    vm.isBeforeAfterImagesSave = false;
                                    var index = slide[$index];
                                    if (value == 0) {
                                        if (isFromBefore) {
                                            vm.jobEstimateImage[$index].beforeImages.originalId = slide[$index].id;
                                            vm.jobEstimateImage[parentIndexes].beforeImages.filesList = [];
                                        }
                                        else {
                                            vm.jobEstimateImage[$index].afterImages.originalId = slide[$index].id;
                                            vm.jobEstimateImage[parentIndexes].afterImages.filesList = [];
                                        }
                                    }
                                    if (value == 1) {
                                        if (vm.isInvoiceTab) {
                                            vm.isInvoiceSave = false;
                                            vm.isBeforeAfterImagesSave = true;
                                        }
                                        else {
                                            vm.isExteriorImageSave = false;
                                            vm.isBeforeAfterImagesSave = true;
                                        }
                                    }
                                    vm.isImageRemoved = true;
                                    slide.splice($index, 1);
                                    toaster.show("Media Deleated Successfully");
                                    save(true);
                                });
                            }
                        }).catch(function (err) {
                            vm.isProcessing = false;
                        });
                    }
                    else {
                        notification.showConfirm("This image will also be deleted from Bulk photo upload area. Do you really want to delete the Image?", "Delete Image", function () {
                            var index = slide[$index];
                            if (value == 0) {
                                if (isFromBefore) {
                                    vm.isBeforeAfterImagesSave = false;
                                    vm.jobEstimateImage[parentIndexes].beforeImages.filesList = [];
                                }
                                else {
                                    vm.isBeforeAfterImagesSave = false;
                                    vm.jobEstimateImage[parentIndexes].afterImages.filesList = [];
                                }
                            }
                            if (value == 1) {
                                if (vm.isInvoiceTab) {
                                    vm.isInvoiceSave = false;
                                    vm.isBeforeAfterImagesSave = true;
                                }
                                else {
                                    vm.isExteriorImageSave = false;
                                    vm.isBeforeAfterImagesSave = true;
                                }
                            }
                            slide.splice($index, 1);
                            toaster.show("Media Deleated Successfully");
                            save(true);
                        });

                    }
                }
                function remove(slide, $index, value) {
                    vm.isExteriorImageSave = false;
                    var index = vm.jobEstimateImage.indexOf(slide);
                    var jobEstimateImage = vm.jobEstimateImage[index];

                    vm.jobEstimateImage.splice(index, 1);
                    getListForBeforeAfterDropDown();
                }

                function addNew() {

                    var length = vm.jobEstimateImage.length;
                    length = length + 1;
                    vm.jobEstimateImagess = [];
                    var tempArray = [];
                    vm.count = vm.imageList.imagePairs.length;
                    vm.rowid = ++vm.count;
                    vm.myText = "BEFORE";
                    angular.copy(vm.imageList.imagePairs, vm.jobEstimateImagess);

                    if (vm.jobEstimateImage.length > 0) {
                        vm.isBeforeAfterImagesSave = false;
                        var firstElement = [];
                        angular.copy(vm.jobEstimateImage[0], firstElement);
                        firstElement.beforeImages.id = 0;
                        firstElement.afterImages.id = 0;
                        firstElement.beforeImages.filesList = [];
                        firstElement.beforeImages.imagesInfo = [];
                        firstElement.afterImages.filesList = [];
                        firstElement.afterImages.imagesInfo = [];
                    }
                    vm.jobEstimateImagess = ({
                        isSelectedProperty: null,
                        isSelectedServiceTypeProperty: false,
                        isSelectedFinishMaterialProperty: false,
                        isSelectedSurfaceColorProperty: false,
                        isSelectedCompanyNameProperty: false,
                        isSelectedMaidServiceProperty: false,
                        isSelectedPropertyManagerProperty: false,
                        isSelectedMaidJanitorialProperty: false,
                        isChanged: false,
                        beforeImages: { filesList: [], imagesInfo: [], id: 0, isDisable: false, isGroutilife: false, typeId: vm.BeforeAfterImages.Before, isRequired: true, floorNumber: 1, imageIndex: length, originalImageIndex: length, originalId: 0, isBestPicture: false },
                        afterImages: { filesList: [], imagesInfo: [], id: 0, isDisable: false, isGroutilife: false, typeId: vm.BeforeAfterImages.After, isRequired: true, floorNumber: 1, imageIndex: length, originalImageIndex: length, originalId: 0, isBestPicture: false }
                    });
                    if (vm.jobEstimateImage.length > 0) {
                        vm.isBeforeAfterImagesSave = false;
                        vm.jobEstimateImagess.beforeImages = firstElement.beforeImages;
                        vm.jobEstimateImagess.afterImages = firstElement.afterImages;
                        vm.jobEstimateImagess.beforeImages.isDisable = false;
                        vm.jobEstimateImagess.afterImages.isDisable = false;
                        vm.jobEstimateImagess.beforeImages.imageIndex = length;
                        vm.jobEstimateImagess.afterImages.imageIndex = length;

                    }
                    vm.imageParentChildListModel = (vm.jobEstimateImagess);
                    vm.jobEstimateImage.unshift(vm.jobEstimateImagess);
                    vm.imageParentChildListModel = [];
                    vm.jobEstimateImagess = [];
                    getListForBeforeAfterDropDown();
                    rearrangePaneNumber();
                }


                function addNote() {
                    if (!vm.isExteriorImageSave || !vm.isInvoiceSave || !vm.isBeforeAfterImagesSave) {
                        if (!vm.isBeforeAfterActive) {
                            changeTab(4);
                        }
                        else if (!vm.isInvoiceTabActive) {
                            changeTab(2)
                        }
                        else if (!vm.isExteriorImageActive) {
                            changeTab(3)
                        }
                        if (vm.isCancelledClick) {
                            return;
                        }
                    }

                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/note-add.client.view.html',
                        controller: 'SchedulerNoteController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    JobInfo: vm.jobInfo,
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                        getJobInfo();
                    });
                }
                $rootScope.$on("FileId", function (evt, data) {
                    vm.fileModel = data;
                    //alert(data);
                });

                $rootScope.$on("IsImageForSamePane", function (evt, data) {
                    vm.isImageForSamePane = data;
                });

                $scope.trustSrc = function (src) {
                    return $sce.trustAsResourceUrl(src);
                }

                function uploadMediaAfterBefore(afterImages, isFromBeforeAfter, isMultipleUpload,
                    beforeImages, isFromBefore, imageModel, index) {
                    vm.jobEstimateImagess = {};
                    vm.isSeletedIndex = index;
                    var img = [];
                    var beforeImagesProperties = [];
                    var afterImagesProperties = [];
                    afterImagesProperties = afterImages;
                    beforeImagesProperties = beforeImages;
                    angular.copy(imageModel, vm.jobEstimateImagess);
                    if (isFromBefore) {
                        img = beforeImages;
                    }
                    else {
                        img = afterImages;
                    }

                    if (img.imagesInfo.length > 0) {
                        vm.alreadyUploaded = true;
                    }
                    else {
                        vm.alreadyUploaded = false;
                    }
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/media-upload-before-after.client.view.html',
                        controller: 'MediaUploadBeforeAfterController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    JobInfo: vm.jobInfo,
                                    isFromBeforeAfter: isFromBeforeAfter,
                                    alreadyUploaded: vm.alreadyUploaded,
                                    isMultiple: isMultipleUpload
                                };
                            }
                        },
                        backdrop: 'static',
                    });

                    modalInstance.result.then(function () {
                        vm.isFirstElements = vm.jobEstimateImage.length == 1 ? true : false;
                        var length = vm.jobEstimateImage.length;
                        var fileLength = vm.fileModel.length;
                        if (length == 1) {
                            vm.isBeforeAfterImagesSave = false;
                            var beforeImageCount = vm.jobEstimateImage[0].beforeImages.imagesInfo.length;
                            var afterImageCount = vm.jobEstimateImage[0].afterImages.imagesInfo.length;
                            if (isFromBefore) {
                                vm.beforeAfterImagesFirstUploadedImages[0][0] = beforeImageCount;
                            }
                            else {
                                vm.beforeAfterImagesFirstUploadedImages[0][1] = afterImageCount;
                            }
                        }
                        else {
                            angular.forEach(vm.jobEstimateImage, function (value1) {
                                vm.isBeforeAfterImagesSave = false;
                                if (value1.beforeImages.imagesInfo.length > 0) {
                                    vm.beforeAfterImagesFirstUploadedImages[0][0] += 1;
                                }
                                if (value1.afterImages.imagesInfo.length > 0) {
                                    vm.beforeAfterImagesFirstUploadedImages[0][1] += 1;
                                }

                            });
                        }

                        var isParsedImageData = false;
                        var imageLength = vm.jobEstimateImage.length;
                        imageLength = imageLength + 1;

                        // when more than one image is uploaded so that uploaded either for that pane or different pane
                        if (((!isFromBefore && vm.beforeAfterImagesFirstUploadedImages[0][0] > 1) ||
                            (isFromBefore && vm.beforeAfterImagesFirstUploadedImages[0][1] > 1)) && vm.fileModel.length > 1) {
                            var index = -1;
                            GetttingPaneWithHavingBothImages(isFromBefore);
                            angular.forEach(vm.fileModel, function (fileModel) {
                                vm.isImageRemoved = true;
                                ++index;
                                var value = vm.jobEstimateImageDoublicate[index];
                                if (value != null) {
                                    var isImageAlreadyPresentinPane = false;
                                    if (isFromBefore) {
                                        if (value.beforeImages.imagesInfo.length > 0) {
                                            isImageAlreadyPresentinPane = true;
                                            --index;
                                        }
                                    }
                                    else {
                                        if (value.afterImages.imagesInfo.length > 0) {
                                            isImageAlreadyPresentinPane = true;
                                            --index;
                                        }
                                    }
                                    if (!isImageAlreadyPresentinPane) {
                                        var imageLength = vm.jobEstimateImage.length;
                                        imageLength = imageLength + 1;

                                        //var fileModel = vm.fileModel[index];

                                        fileService.getFileStreamByUrl(fileModel.relativeLocation).then(function (result) {
                                            $scope.imageUrl = fileService.getStreamUrl(result);
                                            jobEstimateImagess = {};
                                            angular.copy(imageModel, jobEstimateImagess);
                                            if (isFromBefore) {
                                                jobEstimateImagess.beforeImages.imagesInfo = [];
                                                if (value.beforeImages.imagesInfo.length < 1) {
                                                    value.beforeImages.filesList = [];
                                                    value.afterImages.imageIndex = value.beforeImages.imageIndex;
                                                    value.beforeImages.filesList.push(fileModel.fileId);
                                                    value.beforeImages.imagesInfo = [];
                                                    value.beforeImages.imagesInfo.push({
                                                        url: $scope.imageUrl, name: fileModel.caption, size: fileModel.size, status: fileModel.status,
                                                        createdBy: fileModel.createdBy, createdOn: fileModel.createdOn, id: 0, relativeLocation: null, fileId: fileModel.fileId, isSwapped: false
                                                    });
                                                }
                                                else {
                                                    vm.countForHavingPaneWithImages[0][0] += 1;
                                                }
                                            }
                                            else {
                                                var jobEstimateImagess = {};
                                                angular.copy(imageModel, jobEstimateImagess);
                                                value.afterImages.originalImageIndex = value.afterImages.selectedIndex;
                                                jobEstimateImagess.afterImages.imagesInfo = [];
                                                if (value.afterImages.imagesInfo.length < 1) {
                                                    value.afterImages.filesList = [];
                                                    value.afterImages.filesList.push(fileModel.fileId);
                                                    //value.beforeImages.imagesInfo = [];
                                                    value.afterImages.imagesInfo.push({
                                                        url: $scope.imageUrl, name: fileModel.caption, size: fileModel.size, status: fileModel.status,
                                                        createdBy: fileModel.createdBy, createdOn: fileModel.createdOn, id: 0, relativeLocation: null, fileId: fileModel.fileId, isSwapped: false
                                                    });
                                                }
                                                else {
                                                    vm.countForHavingPaneWithImages[0][1] += 1;
                                                }
                                            }
                                            AddingNewPane(imageModel, value, fileModel);
                                        });
                                    }
                                }
                                else {
                                    if (isFromBefore) {
                                        vm.countForHavingPaneWithImages[0][0] += 1;
                                    }
                                    else {
                                        vm.countForHavingPaneWithImages[0][1] += 1;
                                    }
                                }
                                AddingNewPane(imageModel, value, fileModel);
                            });
                        }

                        else {
                            vm.isDeleted = false;
                            angular.forEach(vm.fileModel, function (value, index) {
                                vm.isImageRemoved = true;
                                img.filesList.push(value.fileId);
                                fileService.getFileStreamByUrl(value.relativeLocation).then(function (result) {
                                    var length = vm.jobEstimateImage.length;
                                    length = length + 1;
                                    $scope.imageUrl = fileService.getStreamUrl(result);
                                    vm.isBeforeAFterSave = true;

                                    // when there is no image pane for first time use
                                    if (img.imagesInfo.length == 0 && vm.beforeAfterImagesFirstUploadedImages[0][0] == 0 && vm.beforeAfterImagesFirstUploadedImages[0][1] == 0) {
                                        if (isFromBefore) {
                                            afterImages.imageIndex = beforeImages.imageIndex;
                                            afterImages.selectedIndex = afterImages.imageIndex;
                                        }
                                        else {
                                            beforeImages.imageIndex = afterImages.imageIndex;
                                            beforeImages.selectedIndex = afterImages.imageIndex;
                                        }
                                        img.filesList = [];
                                        img.filesList.push(value.fileId);
                                        img.imagesInfo = [];

                                        img.imagesInfo.push({
                                            url: $scope.imageUrl, name: value.caption, size: value.size, status: value.status,
                                            createdBy: value.createdBy, createdOn: value.createdOn, id: 0, relativeLocation: null, fileId: value.fileId, isSwapped: false
                                        });
                                        vm.isDeleted = true;
                                    }

                                    // when only one image is uploaded so that it will be replace only in that pane
                                    else if (vm.fileModel.length == 1) {
                                        if (img.imagesInfo.length > 1) {
                                            img.id = 0;
                                            if (isFromBefore) {
                                                afterImages.imageIndex = img.imageIndex;
                                            }
                                            else {
                                                beforeImages.imageIndex = img.imageIndex;
                                            }
                                            img.imagesInfo[0].url = $scope.imageUrl;
                                            img.imagesInfo[0].name = value.caption;
                                            img.imagesInfo[0].size = value.size;
                                            img.imagesInfo[0].status = value.status;
                                            img.imagesInfo[0].createdBy = value.createdBy;
                                            img.imagesInfo[0].createdOn = value.createdOn;
                                            img.imagesInfo[0].id = 0;
                                            img.filesList = [];
                                            img.filesList.push(value.fileId);
                                            img.imagesInfo[0].relativeLocation = null;
                                            img.imagesInfo[0].fileId = value.fileId;
                                        }
                                        else {
                                            img.imagesInfo = [];
                                            img.filesList = [];
                                            img.filesList.push(value.fileId);
                                            img.imagesInfo.push({
                                                url: $scope.imageUrl, name: value.caption, size: value.size, status: value.status,
                                                createdBy: value.createdBy, createdOn: value.createdOn, id: 0, relativeLocation: null, fileId: value.fileId, isSwapped: false
                                            });
                                        }
                                        vm.isDeleted = true;
                                    }

                                    else if (vm.fileModel.length > 1) {
                                        var index11 = -1;
                                        var jobEstimateImagess = {};
                                        var isIndexFound = false;

                                        if (!vm.isDeleted) {
                                            vm.jobEstimateImage.splice(vm.isSeletedIndex, 1);
                                            vm.isDeleted = true;
                                        }

                                        length = vm.jobEstimateImage.length;
                                        length = length + 1;
                                        if (isFromBefore) {
                                            angular.copy(imageModel, jobEstimateImagess);
                                            var count = vm.fileModel.length;
                                            jobEstimateImagess.id = 0;
                                            jobEstimateImagess.beforeImages.imageIndex = length;
                                            jobEstimateImagess.afterImages.imageIndex = length;
                                            jobEstimateImagess.afterImages.selectedIndex = length;
                                            jobEstimateImagess.beforeImages.selectedIndex = length;
                                            jobEstimateImagess.afterImages.id = 0;
                                            jobEstimateImagess.afterImages.imagesInfo = [];
                                            jobEstimateImagess.beforeImages.id = 0;

                                            jobEstimateImagess.beforeImages.isBeforeImage = img.isBeforeImage;
                                            jobEstimateImagess.afterImages.isBeforeImage = img.isBeforeImage;

                                            jobEstimateImagess.beforeImages.imagesInfo = [];
                                            jobEstimateImagess.beforeImages.filesList = [];
                                            jobEstimateImagess.beforeImages.filesList.push(value.fileId);
                                            jobEstimateImagess.beforeImages.imagesInfo = [];
                                            jobEstimateImagess.afterImages.filesList = [];
                                            jobEstimateImagess.beforeImages.imagesInfo.push({
                                                url: $scope.imageUrl, name: value.caption, size: value.size, status: value.status,
                                                createdBy: value.createdBy, createdOn: value.createdOn, id: 0, relativeLocation: null, fileId: value.fileId, isSwapped: false
                                            });
                                            rearrangePaneNumber();
                                        }
                                        else {
                                            angular.copy(imageModel, jobEstimateImagess);
                                            var count = vm.fileModel.length;
                                            jobEstimateImagess.id = 0;
                                            jobEstimateImagess.beforeImages.isBeforeImage = img.isBeforeImage;
                                            jobEstimateImagess.afterImages.isBeforeImage = img.isBeforeImage;
                                            jobEstimateImagess.beforeImages.imageIndex = length;
                                            jobEstimateImagess.afterImages.imageIndex = length;
                                            jobEstimateImagess.beforeImages.id = 0;
                                            jobEstimateImagess.afterImages.id = 0;
                                            jobEstimateImagess.afterImages.imagesInfo = [];
                                            jobEstimateImagess.beforeImages.imagesInfo = [];
                                            jobEstimateImagess.afterImages.filesList = [];
                                            jobEstimateImagess.afterImages.filesList.push(value.fileId);
                                            jobEstimateImagess.afterImages.imagesInfo = [];
                                            jobEstimateImagess.beforeImages.filesList = [];
                                            jobEstimateImagess.afterImages.imagesInfo.push({
                                                url: $scope.imageUrl, name: value.caption, size: value.size, status: value.status,
                                                createdBy: value.createdBy, createdOn: value.createdOn, id: 0, relativeLocation: null, fileId: value.fileId, isSwapped: false
                                            });
                                        }
                                        vm.jobEstimateImage.unshift(jobEstimateImagess);
                                        rearrangePaneNumber();
                                    }

                                    getListForBeforeAfterDropDown();
                                    if (index == vm.fileModel.length - 1) {
                                        vm.save(true);
                                    }
                                });
                            });
                            getListForBeforeAfterDropDown();
                        }

                        // Loop for Adding Pane for Uploading Images more then Pane

                    });
                    getListForBeforeAfterDropDown();
                    //save(true);
                }

                $rootScope.$on("FileId", function (evt, data) {
                    vm.fileModel = data;
                });
                function uploadMediaBuildingInvoice(img, isFromInvoice, IsFromBuilding) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/media-job-upload.client.view.html',
                        controller: 'MediaJobUploadController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    JobInfo: vm.jobInfo,
                                    IsFromInvoice: true,
                                    IsFromBuilding: IsFromBuilding
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {

                        if (vm.fileModel != undefined && vm.fileModel != null && vm.fileModel.length > 0 && isFromInvoice) {
                            angular.forEach(vm.fileModel, function (value) {
                                vm.isInvoiceSave = false
                                img.filesList.push(value.fileId);
                                vm.isInvoiceMedia = true;
                                fileService.getFileStreamByUrl(value.relativeLocation).then(function (result) {
                                    $scope.imageUrl = fileService.getStreamUrl(result);
                                    //img.imagesInfo = [];
                                    var array = [];
                                    var fileExtension;
                                    if (value.iFrameLocation != null) {
                                        array = value.relativeLocation.split('.');
                                        fileExtension = array[array.length - 1];
                                    }
                                    img.imagesInfo.push({
                                        url: $scope.imageUrl, extension: fileExtension, name: value.caption, size: value.size, status: value.status, isSwapped: false,
                                        createdBy: value.createdBy, createdOn: value.createdOn, id: 0, relativeLocation: null, fileId: value.fileId, isiFrame: value.isiFrame,
                                        iframUrl: "https://docs.google.com/gview?url=" + value.iFrameLocation + "&embedded=true"
                                        //iframUrl: value.iFrameLocation
                                    });
                                });
                            });
                        }
                        else if (vm.fileModel != undefined && vm.fileModel != null && vm.fileModel.length > 0 && !isFromInvoice) {
                            angular.forEach(vm.fileModel, function (value) {
                                img.filesList.push(value.fileId);
                                vm.isMediaSave = true;
                                vm.isExteriorImageSave = false
                                fileService.getFileStreamByUrl(value.relativeLocation).then(function (result) {
                                    $scope.imageUrl = fileService.getStreamUrl(result);
                                    img.imagesInfo.push({
                                        url: $scope.imageUrl, name: value.caption, size: value.size, status: value.status,
                                        createdBy: value.createdBy, createdOn: value.createdOn, isSwapped: false
                                    });
                                });
                            });
                        }
                    });
                    //getJobInfo();

                }
                function uploadMedia() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/media-upload.client.view.html',
                        controller: 'MediaJobUploadController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    JobInfo: vm.jobInfo,
                                    IsFromInvoice: true,
                                    IsFromBuilding: false
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                        $rootScope.$on("FileId", function (evt, data) {
                            vm.fileModel = data;
                        });
                        if (vm.fileModel.length > 0) {
                            img.fileId = vm.fileModel[0].fileId;
                            fileService.getFileStreamByUrl(vm.fileModel[0].relativeLocation).then(function (result) {
                                img.src = fileService.getStreamUrl(result);
                            });
                        }
                    });
                    //getJobInfo();

                }

                function getBeforeAfterMedia() {

                }
                function repeatJob() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/job-repeat.client.view.html',
                        controller: 'JobRepeatController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    JobInfo: vm.jobInfo,
                                    InvoiceInfo: vm.estimateInvoiceInfo,
                                    IsFromConvertToJob: vm.estimateInvoiceInfo != null ? true : false
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                        getJobInfo();
                    });
                }

                function goToEstimate() {

                    $state.go('core.layout.scheduler.estimate', { id: vm.jobInfo.estimateId, rowId: vm.jobInfo.estimateSchedulerId });
                }

                function prepareStatusOptions() {
                    vm.jobStatus = [];
                    if (vm.jobInfo.statusId == '2')
                        vm.jobStatus.push({ display: 'In Progress', value: '3' }, { display: 'Completed', value: '4' }, { display: 'Canceled', value: '5' })

                    if (vm.jobInfo.statusId == '3')
                        vm.jobStatus.push({ display: 'Completed', value: '4' }, { display: 'Canceled', value: '5' })
                }

                function editJob() {
                    if (!vm.isExteriorImageSave || !vm.isInvoiceSave || !vm.isBeforeAfterImagesSave) {

                        if (!vm.isBeforeAfterActive) {
                            changeTab(4);
                        }
                        else if (!vm.isInvoiceTabActive) {
                            changeTab(2)
                        }
                        else if (!vm.isExteriorImageActive) {
                            changeTab(3)
                        }
                        if (vm.isCancelledClick) {
                            return;
                        }
                    }
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/create-job.client.view.html',
                        controller: 'CreateJobController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    JobId: vm.rowId,
                                    IsCopy: vm.isCopy,
                                    EstimateInvoiceInfo: vm.estimateInvoiceInfo,
                                    IsFromConvertToJob: vm.estimateInvoiceInfo != null ? true : false,
                                    EditJob: true
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                        getJobInfo();
                    });
                }

                function deleteJob() {
                    notification.showConfirm("Do you really want to delete the Job? the assignee(s) related to this job will be removed!", "Delete Job", function () {
                        return schedulerService.deleteJob(vm.jobInfo.jobId).then(function (result) {
                            if (!result.data)
                                toaster.error(result.message.message);
                            else {
                                toaster.show(result.message.message);
                                $state.go('core.layout.scheduler.list', { franchiseeId: vm.jobInfo.franchiseeId });
                            }
                        });
                    });
                }

                function getJobInfo() {
                    vm.isProcessing = true;
                    return schedulerService.getJobInfo(vm.rowId).then(function (result) {
                        vm.jobInfo = result.data;
                        if (vm.jobInfo.estimateInvoiceId == null) {
                            vm.isInvoicePresent = false;
                        }
                        else {
                            vm.isInvoicePresent = true;
                        }
                        if (vm.jobInfo.jobSchedulerList.length >= 1) {
                            vm.isRepeative = true;
                        }
                        else {
                            vm.isRepeative = false;
                        }
                        angular.forEach(vm.jobInfo.jobSchedulerList, function (item) {
                            if (item.id == vm.rowId) {
                                item.alias = true;
                                vm.navigationDate = item.startDate;
                            }
                            else {
                                item.alias = false;
                            }
                            item.startDate = moment((item.actualStartDateString)).format("MM/DD/YYYY HH:mm");
                            item.endDate = moment((item.actualEndDateString)).format("MM/DD/YYYY HH:mm");
                        });
                        $scope.currentJob = $filter('filter')(vm.jobInfo.jobSchedulerList, { id: vm.rowId })[0];
                        var jobScheduler = $scope.currentJob;
                        if (jobScheduler != null && (jobScheduler.invoiceNumbers == undefined || jobScheduler.invoiceNumbers == null || jobScheduler.invoiceNumbers.length == 0)) {
                            vm.invoiceInJob = false;
                        }
                        else {
                            vm.invoiceInJob = true;
                        }
                        vm.imageList.jobTypeId = vm.jobInfo.jobTypeId.toString();
                        $scope.franchiseeId = result.data.franchiseeId;
                        vm.jobInfo.startDate = moment((result.data.actualStartDateString)).format("MM/DD/YYYY HH:mm");
                        vm.jobInfo.endDate = moment((result.data.actualEndDateString)).format("MM/DD/YYYY HH:mm");
                        if (vm.isSalesRep) {
                            vm.isEdit = (vm.jobInfo.dataRecorderMetaData.createdBy == $rootScope.identity.organizationRoleUserId);
                        }
                        if (vm.jobInfo.allInvoiceNumbersSignedForEstimate != null && vm.jobInfo.allInvoiceNumbersSignedForEstimate.length > 0) {
                            vm.isInvoiceNotSigned = false;
                        }
                        else {
                            vm.isInvoiceNotSigned = true;
                        }
                        getMedia();
                        prepareStatusOptions();
                        checkingForPastJob();
                        getInvoiceInfo();
                        vm.isProcessing = false;
                    });
                }

                function getInvoiceInfo() {
                    if (vm.jobInfo != null && vm.jobInfo.estimateInvoiceId != null) {
                        var afterCompletetionValue = 290;
                        vm.estimateModel = {};
                        vm.estimateModel.id = vm.jobInfo.estimateInvoiceId;
                        vm.estimateModel.typeId = afterCompletetionValue;
                        return schedulerService.getInvoiceInfo(vm.estimateModel).then(function (result) {
                            if (result != null && result.data != null) {
                                vm.estimateInvoiceInfo = result.data;
                                vm.estimateInvoiceInfo.customerName = vm.jobInfo.jobCustomer.customerName;
                                vm.isInvoicePresent = true;
                            }
                            else {
                                vm.estimateInvoiceInfo = null;
                            }
                        });
                    }
                    else {
                        vm.estimateInvoiceInfo = null;
                    }
                    getEstimateInvoiceInfo();
                }

                function getMedia() {
                    vm.noOfUnPaired = 0;
                    $scope.slides = [];
                    vm.jobEstimateImage = [];
                    vm.isPrintable = false;
                    vm.isMediaPrintable = false;
                    vm.isMediaSave = false;
                    vm.mediaModel.rowId = vm.rowId;
                    vm.mediaModel.MediaType = vm.MediaType.Job;
                    vm.mediaModel.estimateId = vm.jobInfo.estimateId;
                    return schedulerService.getMedia(vm.mediaModel).then(function (result) {
                        vm.media = result.data;
                        vm.imageList = vm.media.imageList;
                        vm.imvoiceImagesList = vm.media.imageList.invoiceImages;
                        if (vm.imageList.imagePairs.length > 0) {
                            vm.isBeforeAFterSave = true;
                        }
                        if (vm.imvoiceImagesList.imagesInfo.length > 0) {
                            vm.isPrintable = true;
                        }
                        if (vm.imvoiceImagesList.imagesInfo.length > 0) {
                            vm.isInvoiceMedia = true;
                        }
                        vm.sliderImages = vm.media.imageList.sliderImages;

                        if (vm.media.imageList.sliderImages.imagesInfo.length > 0) {
                            vm.isMediaSave = true;
                        }
                        if (vm.media != null && vm.media.imageList != null) {
                            vm.imageList.marketingClassId = vm.jobInfo.jobTypeId.toString();
                            vm.currentMarketingClass = $filter('filter')(vm.marketingClass, { value: vm.imageList.marketingClassId })[0];

                            if (vm.imageList.marketingClassId == vm.marketingClassLookUp.RESIDENTIAL) {
                                vm.isResidential = true;
                            }

                            angular.forEach(vm.imageList.imagePairs, function (items) {
                                vm.jobEstimateImage.push(items);
                            })
                        }
                        if (vm.imageList.imagePairs.length > 0) {
                            angular.forEach(vm.jobEstimateImage, function (items) {

                                if (items.afterImages.imagesInfo.length != items.beforeImages.imagesInfo.length) {
                                    vm.noOfUnPaired += 1;

                                }
                                if (vm.serviceTypeLookUp.GROUTLIFE == items.afterImages.serviceTypeId || vm.serviceTypeLookUp.CONCRETECOUNTERTOPS == items.afterImages.serviceTypeId
                                    || vm.serviceTypeLookUp.CONCRETEOVERLAYMENTS == items.afterImages.serviceTypeId || vm.serviceTypeLookUp.CONCRETECOATINGS == items.afterImages.serviceTypeId) {
                                    items.afterImages.isGroutilife = true;
                                }
                                else {
                                    items.afterImages.isGroutilife = false;
                                }

                                if (items.beforeImages.serviceTypeId != null) {
                                    var serviceTypeId = items.beforeImages.serviceTypeId.toString();
                                    items.beforeImages.serviceTypeId = serviceTypeId;
                                }
                                if (items.afterImages.serviceTypeId != null) {
                                    var serviceTypeId = items.afterImages.serviceTypeId.toString();
                                    items.afterImages.serviceTypeId = serviceTypeId;
                                }
                                if (vm.serviceTypeLookUp.GROUTLIFE == items.beforeImages.serviceTypeId || vm.serviceTypeLookUp.CONCRETECOUNTERTOPS == items.beforeImages.serviceTypeId
                                    || vm.serviceTypeLookUp.CONCRETEOVERLAYMENTS == items.beforeImages.serviceTypeId || vm.serviceTypeLookUp.CONCRETECOATINGS == items.beforeImages.serviceTypeId) {
                                    items.beforeImages.isGroutilife = true;
                                }
                                else {
                                    items.beforeImages.isGroutilife = false;
                                }
                            });
                        }

                        getJobEstimateSlides();
                        getBuildingExteriorSlides();
                        getInvoiceSlides();
                        getListForBeforeAfterDropDown();
                        checkIfMarketingClassIsAsRequiredOrNot();
                        creatingLightBox();
                    });
                }
                function getJobEstimateSlides() {
                    var len = vm.jobEstimateImage.length;
                    angular.forEach(vm.jobEstimateImage, function (values, index) {

                        angular.forEach(values.beforeImages.imagesInfo, function (value, index) {
                            if (value.s3BucketImageUrl == null && value.s3BucketThumbImageUrl == null) {
                                if (value != null && value.thumbImageUrl != null) {
                                    fileService.getFileStreamByUrl(value.thumbImageUrl).then(function (result) {
                                        value.thumbUrl = fileService.getStreamUrl(result);
                                    });
                                }
                                else if (value != null && value.imageUrl != null) {
                                    fileService.getFileStreamByUrl(value.imageUrl).then(function (result) {
                                        value.url = fileService.getStreamUrl(result);
                                    });
                                }
                                else {
                                    value.url = '';
                                }
                            }
                            else {
                                value.thumbUrl = value.s3BucketThumbImageUrl;
                            }
                            if (value.isImageCropped) {
                                if (value != null && value.croppedImageThumb != null) {
                                    fileService.getFileStreamByUrl(value.croppedImageThumb).then(function (result) {
                                        value.thumbUrlCropped = fileService.getStreamUrl(result);
                                    });
                                }
                            }
                        });
                        angular.forEach(values.afterImages.imagesInfo, function (value, index) {
                            if (value.s3BucketImageUrl == null && value.s3BucketThumbImageUrl == null) {
                                if (value != null && value.thumbImageUrl != null) {
                                    fileService.getFileStreamByUrl(value.thumbImageUrl).then(function (result) {
                                        value.thumbUrl = fileService.getStreamUrl(result);
                                    });
                                }
                                else if (value != null && value.imageUrl != null) {
                                    fileService.getFileStreamByUrl(value.imageUrl).then(function (result) {
                                        value.url = fileService.getStreamUrl(result);
                                    });
                                }
                                else {
                                    value.url = '';
                                }
                            }
                            else {
                                value.thumbUrl = value.s3BucketThumbImageUrl;
                            }
                            if (value.isImageCropped) {
                                if (value != null && value.croppedImageThumb != null) {
                                    fileService.getFileStreamByUrl(value.croppedImageThumb).then(function (result) {
                                        value.thumbUrlCropped = fileService.getStreamUrl(result);
                                    });
                                }
                            }
                        });
                    });
                }



                function getInvoiceSlides() {
                    angular.forEach(vm.imvoiceImagesList.imagesInfo, function (value, index) {
                        if (value.imageUrl != null) {
                            fileService.getFileStreamByUrl(value.imageUrl).then(function (result) {
                                value.url = fileService.getStreamUrl(result);
                                value.isExteriorImageSave = true;
                                var array = [];
                                var extension = '';
                                if (value.name != null) {
                                    array = value.name.split('.');
                                    extension = array[array.length - 1];
                                    value.extension = extension;
                                    if (extension == "jpg" || extension == "jpeg" || extension == "png") {
                                        value.isImage = true;
                                    }
                                }

                                value.isiFrame,
                                    value.iframUrl = "https://docs.google.com/gview?url=" + value.iFrameUrl + "&embedded=true"
                            });
                        }
                        else {
                            value.src = '';
                        }
                        if (value.caption != null && value.createdOn != null) {
                            value.documentName = value.caption + "_" + $filter('date')(value.createdOn, "MM/dd/yyyy") + ".pdf";
                        }
                        else if (value.caption != null) {
                            value.documentName = value.caption + ".pdf";
                        }
                        else if (value.createdOn != null) {
                            value.documentName = $filter('date')(value.createdOn, "MM/dd/yyyy") + ".pdf";
                        }
                        else {
                            value.documentName = '';
                        }
                    });
                }

                function getBuildingExteriorSlides() {
                    angular.forEach(vm.sliderImages.imagesInfo, function (value, index) {
                        if (value.imageUrl != null) {
                            if (value.imageUrl != null && (value.s3BucketImageUrl == null || value.s3BucketImageUrl == "")) {
                                fileService.getFileStreamByUrl(value.imageUrl).then(function (result) {
                                    value.url = fileService.getStreamUrl(result);
                                });
                                value.url = value.imageUrl;
                                value.isExteriorImageSave = true;
                                var array = [];
                                var extension = '';
                                if (value.name != null) {
                                    array = value.name.split('.');
                                    extension = array[array.length - 1];
                                    if (extension == "jpg" || extension == "jpeg" || extension == "png") {
                                        value.isImage = true;
                                    }
                                }
                            }
                            else if ((value.s3BucketImageUrl != null)) {
                                value.url = value.s3BucketImageUrl;
                                value.isImageSaved = true;
                                if (value.name != null) {
                                    array = value.name.split('.');
                                    extension = array[array.length - 1];
                                    if (extension == "jpg" || extension == "jpeg" || extension == "png" || extension == "PNG") {
                                        value.isImage = true;
                                    }
                                }
                            }
                        }
                        else {
                            value.src = '';
                        }
                    });
                }
                function getSlides() {
                    angular.forEach(vm.media.resources, function (value) {
                        fileService.getFileStreamByUrl(value.relativeLocation).then(function (result) {
                            $scope.imageUrl = fileService.getStreamUrl(result);
                            $scope.slides.push({
                                url: $scope.imageUrl, name: value.caption, size: value.size, status: value.status,
                                createdBy: value.createdBy, createdOn: value.createdOn
                            });
                        });
                    });
                }
                function getJobStatus() {
                    return schedulerService.getJobStatus().then(function (result) {
                        vm.jobStatus = result.data;
                    });
                }

                function changeJobStatus(statusId) {
                    $scope.$watch('statusId', function (nv, ov) {
                        if (nv == ov) return;
                    });
                    if (statusId <= 0 || statusId == null)
                        return;
                    return schedulerService.changeJobStatus(vm.jobInfo.jobId, statusId).then(function (result) {
                        if (result.data != null) {
                            toaster.show(result.message.message);
                            getJobInfo();
                        }
                    });
                }

                function checkQbNumber() {
                    vm.validQB = false;
                    if (vm.jobInfo.qbInvoiceNumber == null || vm.jobInfo.qbInvoiceNumber == "") {
                        vm.validQB = true;
                        return;
                    }
                    return schedulerService.checkQbNumberIsValid(vm.jobInfo.qbInvoiceNumber).then(function (result) {
                        if (!result.data) {
                            vm.validQB = false;
                            vm.jobInfo.qbInvoiceNumber = null;
                            notification.showAlert("Invalid QB Number!");
                            return;
                        }
                        else if (result.data) {
                            vm.validQB = true;
                        }
                    });
                }

                function updateQbInvoice() {
                    vm.validQB = true;
                    //editModel = false;
                    vm.isProcessing = true;
                    if (!vm.validQB) {
                        vm.isProcessing = false;
                        return;
                    }
                    return schedulerService.updateQbInvoice(vm.jobInfo.jobId, vm.jobInfo.qbInvoiceNumber, vm.rowId).then(function (result) {
                        if (result.data) {
                            vm.isProcessing = false;
                            attactInvoice(vm.jobInfo.jobId, vm.jobInfo.qbInvoiceNumber);
                            getJobInfo();
                            toaster.show(result.message.message);
                        }
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }

                function attactInvoice(jobId, qbInvoiceNumber) {

                }
                function getmarketingClassCollection() {
                    return customerService.getmarketingClassCollection().then(function (result) {
                        vm.marketingClass = result.data;

                    });
                }
                function getServicesCollection() {
                    //return franchiseeService.getServiceTypeCollection().then(function (result) {
                    //    vm.serviceType = result.data;
                    //    vm.serviceTypeForInvoices = result.data;
                    //    vm.elementInList = $filter('filter')(vm.serviceTypeForInvoices, { display: "COLORSEAL" });
                    //    var index = vm.serviceTypeForInvoices.indexOf($filter('filter')(vm.serviceTypeForInvoices, { display: "COLORSEAL" }, true)[0]);
                    //    vm.serviceTypeForInvoices.splice(index, 1);
                    //});
                }

                function save(isFromBeforeAfterPane) {
                    vm.isInValid = false;
                    vm.isBeforeAfterImagesSave = true;
                    vm.isInvoiceSave = true;
                    vm.isExteriorImageSave = true;
                    if (isFromBeforeAfterPane) {
                        angular.forEach(vm.jobEstimateImage, function (value) {
                            if (value.afterImages.isBeforeImage == undefined) {
                                toaster.error("Please Select the Required Info");
                                vm.isInValid = true;
                                vm.isBeforeAfterImagesSave = false;
                                vm.isInvoiceSave = true;
                                vm.isExteriorImageSave = true;
                                changeTabFocusForCancellation();
                                if ($scope.shiftingImageActive) {
                                    vm.jobEstimateImage = vm.jobEstimateImageActive;
                                    $scope.shiftingImageActive = false;
                                }
                                return 0;
                            }
                            if (value.afterImages.isBeforeImage == true) {
                                if (value.beforeImages.filesList.length <= 0 && value.beforeImages.imagesInfo.length <= 0) {
                                    //toaster.error("Please Select the Required Info");
                                    //vm.isInValid = true;
                                    //vm.isBeforeAfterImagesSave = false;
                                    //vm.isInvoiceSave = true;
                                    //vm.isExteriorImageSave = true;
                                    //changeTabFocusForCancellation();
                                    //if ($scope.shiftingImageActive) {
                                    //    vm.jobEstimateImage = vm.jobEstimateImageActive;
                                    //    $scope.shiftingImageActive = false;
                                    //}
                                    //return 0;
                                }
                                else {
                                    if (value.beforeImages.surfaceMaterial == undefined || value.beforeImages.buildingLocation == undefined || value.beforeImages.surfaceType == undefined) {
                                        toaster.error("Please Select the Required Info");
                                        vm.isInValid = true;
                                        vm.isBeforeAfterImagesSave = false;
                                        vm.isInvoiceSave = true;
                                        vm.isExteriorImageSave = true;
                                        changeTabFocusForCancellation();
                                        if ($scope.shiftingImageActive) {
                                            vm.jobEstimateImage = vm.jobEstimateImageActive;
                                            $scope.shiftingImageActive = false;
                                        }
                                        return;
                                    }
                                }
                            }
                            if (value.afterImages.isBeforeImage == false) {
                                if (value.afterImages.filesList.length <= 0 && value.afterImages.imagesInfo.length <= 0) {
                                    //toaster.error("Please Select the Required Info");
                                    //vm.isInValid = true;
                                    //vm.isBeforeAfterImagesSave = false;
                                    //vm.isInvoiceSave = true;
                                    //vm.isExteriorImageSave = true;
                                    //changeTabFocusForCancellation();
                                    //if ($scope.shiftingImageActive) {
                                    //    vm.jobEstimateImage = vm.jobEstimateImageActive;
                                    //    $scope.shiftingImageActive = false;
                                    //}
                                    //return 0;
                                }
                                else {
                                    if (value.afterImages.surfaceMaterial == undefined || value.afterImages.buildingLocation == undefined || value.afterImages.surfaceType == undefined) {
                                        toaster.error("Please Select the Required Info");
                                        vm.isInValid = true;
                                        vm.isBeforeAfterImagesSave = false;
                                        vm.isInvoiceSave = true;
                                        vm.isExteriorImageSave = true;
                                        changeTabFocusForCancellation();
                                        if ($scope.shiftingImageActive) {
                                            vm.jobEstimateImage = vm.jobEstimateImageActive;
                                            $scope.shiftingImageActive = false;
                                        }
                                        return;
                                    }
                                }
                            }


                        });
                    }
                    if (vm.isInValid) {
                        return 0;
                    }
                    vm.isProcessing = true;
                    if (vm.imageList.id == null) {
                        vm.imageList.id = 0;
                    }
                    vm.imageList.schedulerId = vm.rowId;
                    vm.imageList.jobId = vm.jobId;
                    vm.imageList.sliderImages = vm.sliderImages;
                    vm.imageList.imagePairs = (vm.jobEstimateImage);
                    vm.imageList.isFromBeforeAfterPane = (isFromBeforeAfterPane);
                    vm.imageList.estimateId = vm.jobInfo.estimateId;
                    //vm.imageList.invoiceImages.isFromEstimate = false;
                    schedulerService.saveBeforeAfterImages(vm.imageList).then(function (result) {
                        if (result.data != null) {

                            if (!result.data) {
                                vm.isProcessing = false;
                                toaster.error(result.message.message);
                                getJobInfo();
                            }
                            else {
                                vm.isProcessing = false;
                                if (vm.isImageRemoved) {
                                    vm.isBeforeAfterImagesSave = true;
                                    vm.isInvoiceSave = true;
                                    vm.isExteriorImageSave = true;
                                    if (vm.isImageRemoved && vm.isRowDelete == false) {
                                        toaster.show(result.message.message);
                                    }
                                    else {
                                        toaster.show("Information Saved/Changed Successfully");
                                    }
                                }
                                else {
                                    vm.isBeforeAfterImagesSave = true;
                                    vm.isInvoiceSave = true;
                                    vm.isExteriorImageSave = true;
                                    if (vm.isImageRemoved && vm.isRowDelete == false) {
                                        toaster.show(result.message.message);
                                    }
                                    else {
                                        toaster.show("Information Saved/Changed Successfully");
                                    }
                                }
                                vm.isRowDelete = false;
                                if (vm.isFromShift) {
                                    {
                                        vm.isFromShift = false;
                                        vm.shiftingImage();
                                    }
                                }
                                getJobInfo();
                            }
                            vm.isProcessing = false;
                        }
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });

                }
                function isEligibleForDeletion(userId, uploadDateTime) {
                    vm.query.userId = userId;
                    vm.query.uploadDateTime = uploadDateTime;
                    return schedulerService.isEligibleForDeletion(vm.query).then(function (result) {
                        if (result.data) {
                            vm.isEligible = result.data;
                        }
                    }).catch(function (err) {
                        vm.isProcessing = false;
                    });
                }
                function floorNoChange(imgs, beforeAfterImages, text) {
                    vm.isBeforeAfterImagesSave = false;
                    if (text == 'Before') {
                        if (beforeAfterImages.isBeforeImage) {
                            imgs.afterImages.floorNumber = beforeAfterImages.floorNumber;
                        }
                    }
                    else {
                        if (!beforeAfterImages.isBeforeImage) {
                            imgs.beforeImages.floorNumber = beforeAfterImages.floorNumber;
                        }
                    }
                }

                function gotoScheduler() {
                    var startDate = vm.jobInfo.startDate;
                    angular.forEach(vm.jobInfo.jobSchedulerList, function (value) {
                        if (value.alias) {
                            startDate = value.startDate;
                        }
                    });
                    vm.navigationDate =
                        $rootScope.$broadcast('navigationDate', startDate);
                    $state.go(core.layout.scheduler.manage({ franchiseeId: vm.jobInfo.franchiseeId, previousView: vm.previousView }));
                }
                function copyJob() {
                    vm.isCopy = true;

                    editJob();
                }

                function confirmChanging(isFromConfirm, estimateInvoiceInfo) {
                    confirmJob();
                }

                function confirmJob() {
                    vm.confirmationQueary.schedulerId = vm.rowId;
                    schedulerService.confirmationSchedulerFromUI(vm.confirmationQuery).then(function (response) {
                        if (response.data != null) {
                            var responseData = response.data;
                            if (responseData.confirmationEnum == vm.ConfirmationEnum.confirmed) {
                                toaster.show("Job Confirmed Successfully !!");
                                vm.jobInfo.isActive = true;
                                vm.jobInfo.schedulerStatusName = "Job Confirmed";
                                vm.jobInfo.schedulerStatusColor = "green";
                            }
                            if (responseData.confirmationEnum == vm.ConfirmationEnum.pastScheduler) {
                                toaster.error("Cannot Change Past Job Status!!");
                                vm.jobInfo.isActive = !vm.jobInfo.isActive;
                            }
                            if (responseData.confirmationEnum == vm.ConfirmationEnum.notConfirmed) {
                                toaster.error("Job not confirmed!!");
                                vm.jobInfo.isActive = false;
                                vm.jobInfo.schedulerStatusName = "Not Confirmed";
                                vm.jobInfo.schedulerStatusColor = "red";
                            }
                        }
                    });
                }

                function checkingForPastJob() {
                    var currentDate = moment(clock.now()).format("MM/DD/YYYY HH:mm");
                    if (vm.jobInfo.startDate < currentDate && vm.jobInfo.endDate < currentDate) {
                        vm.isConfirmDisable = true;
                    }
                }

                function jobNoteEdit(id, note) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/edit-job-note.client.view.html',
                        controller: 'EditJobNoteController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Id: id,
                                    Note: note,
                                    FranchiseeName: vm.jobInfo.franchisee,
                                    IsJob: true
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                        getJobInfo();
                    });
                }
                function jobNoteDelete(id) {
                    notification.showConfirm("Do you really want to delete the Job Note?", "Delete Note", function () {
                        return schedulerService.deleteJobNotes(id).then(function (result) {
                            if (!result.data)
                                toaster.error(result.message.message);
                            else {
                                toaster.show(result.message.message);
                                getJobInfo();
                            }
                        });
                    });
                }

                function manageJob(id, rowid) {
                    vm.query.customerName = '';
                    vm.query.text = '';

                    $state.go('core.layout.scheduler.job', {
                        id: id,
                        previousView: vm.previousView,
                        rowId: rowid
                    });
                }

                function getTitle(id) {
                    if (vm.rowId != id)
                        return "Click on Repeat Detail to navigate to it";
                    else
                        return "";
                }

                function dropDownForBeforeAfter(isFromBefore, selectedIndex, currentIndex) {
                    var tobeChangedFrombeforeAfterImage = {};
                    vm.isBeforeAfterImagesSave = false;
                    var beforeFileList = [];
                    var afterFileList = [];
                    var tempOriginalId = 0;
                    var isSelected = false;
                    var index = 0;
                    var tempStorage = [];

                    beforeFileList = vm.jobEstimateImage[currentIndex - 1].beforeImages.filesList;
                    afterFileList = vm.jobEstimateImage[currentIndex - 1].afterImages.filesList;

                    if (isFromBefore) {

                        //vm.jobEstimateImage[currentIndex - 1].beforeImages.filesList = [];
                        //vm.jobEstimateImage[selectedIndex - 1].beforeImages.filesList = [];
                        vm.jobEstimateImage[currentIndex - 1].beforeImages.filesList = (vm.jobEstimateImage[selectedIndex - 1].beforeImages.filesList);

                        vm.jobEstimateImage[selectedIndex - 1].beforeImages.filesList = (beforeFileList);


                        vm.jobEstimateImage[currentIndex - 1].isBestPicture = false;
                        vm.jobEstimateImage[selectedIndex - 1].isBestPicture = false;
                        vm.jobEstimateImage[currentIndex - 1].beforeImages.isSwapped = true;
                        vm.jobEstimateImage[selectedIndex - 1].beforeImages.isSwapped = true;



                        if (vm.jobEstimateImage[selectedIndex - 1].beforeImages.originalImageIndex == selectedIndex) {
                            vm.jobEstimateImage[selectedIndex - 1].beforeImages.isSwapped = false;
                        }
                        if (vm.jobEstimateImage[currentIndex - 1].beforeImages.originalImageIndex == currentIndex) {
                            vm.jobEstimateImage[currentIndex - 1].beforeImages.isSwapped = false;
                        }




                        if (vm.jobEstimateImage[selectedIndex - 1].beforeImages.imagesInfo.length > 0) {

                            if (vm.jobEstimateImage[currentIndex - 1].beforeImages.imagesInfo.length > 0) {
                                tempStorage = vm.jobEstimateImage[currentIndex - 1].beforeImages.imagesInfo;
                                vm.jobEstimateImage[currentIndex - 1].beforeImages.imagesInfo =
                                    vm.jobEstimateImage[selectedIndex - 1].beforeImages.imagesInfo;
                                vm.jobEstimateImage[selectedIndex - 1].beforeImages.imagesInfo =
                                    tempStorage;

                            }
                            else {
                                tempStorage = [];
                                vm.jobEstimateImage[currentIndex - 1].beforeImages.imagesInfo =
                                    vm.jobEstimateImage[selectedIndex - 1].beforeImages.imagesInfo
                                vm.jobEstimateImage[selectedIndex - 1].beforeImages.imagesInfo =
                                    tempStorage;

                            }
                        }
                        else {
                            if (vm.jobEstimateImage[currentIndex - 1].beforeImages.imagesInfo.length > 0) {
                                vm.jobEstimateImage[selectedIndex - 1].beforeImages.imagesInfo =
                                    vm.jobEstimateImage[currentIndex - 1].beforeImages.imagesInfo;
                                vm.jobEstimateImage[currentIndex - 1].beforeImages.imagesInfo = [];
                            }
                            else {
                                vm.jobEstimateImage[selectedIndex - 1].beforeImages.imagesInfo =
                                    [];
                                vm.jobEstimateImage[currentIndex - 1].beforeImages.imagesInfo = [];
                            }
                        }

                        if (vm.jobEstimateImage[selectedIndex - 1].beforeImages.imagesInfo.length > 0) {
                            tempOriginalId = vm.jobEstimateImage[selectedIndex - 1].beforeImages.imagesInfo[0].originalId;
                            if (vm.jobEstimateImage[currentIndex - 1].beforeImages.imagesInfo.length > 0) {
                                vm.jobEstimateImage[selectedIndex - 1].beforeImages.imagesInfo[0].originalId =
                                    vm.jobEstimateImage[currentIndex - 1].beforeImages.imagesInfo[0].originalId;
                                vm.jobEstimateImage[currentIndex - 1].beforeImages.imagesInfo[0].originalId = tempOriginalId;

                            }
                            else {
                                //vm.jobEstimateImage[selectedIndex - 1].beforeImages.imagesInfo[0].originalId = 0;
                            }
                        }
                        else {
                            vm.jobEstimateImage[currentIndex - 1].beforeImages.imagesInfo[0].originalId = tempOriginalId;
                        }
                    }
                    else {

                        vm.jobEstimateImage[currentIndex - 1].afterImages.filesList = (vm.jobEstimateImage[selectedIndex - 1].afterImages.filesList);

                        vm.jobEstimateImage[selectedIndex - 1].afterImages.filesList = (afterFileList);

                        vm.jobEstimateImage[currentIndex - 1].isBestPicture = false;
                        vm.jobEstimateImage[selectedIndex - 1].isBestPicture = false;

                        vm.jobEstimateImage[currentIndex - 1].afterImages.isSwapped = true;
                        vm.jobEstimateImage[selectedIndex - 1].afterImages.isSwapped = true;

                        if (vm.jobEstimateImage[selectedIndex - 1].afterImages.originalImageIndex == selectedIndex) {
                            vm.jobEstimateImage[selectedIndex - 1].afterImages.isSwapped = false;
                        }
                        if (vm.jobEstimateImage[currentIndex - 1].afterImages.originalImageIndex == currentIndex) {
                            vm.jobEstimateImage[currentIndex - 1].afterImages.isSwapped = false;
                        }

                        if (vm.jobEstimateImage[selectedIndex - 1].afterImages.imagesInfo.length > 0) {
                            tempOriginalId = vm.jobEstimateImage[selectedIndex - 1].afterImages.imagesInfo[0].originalId;
                            if (vm.jobEstimateImage[currentIndex - 1].afterImages.imagesInfo.length > 0) {
                                vm.jobEstimateImage[selectedIndex - 1].afterImages.imagesInfo[0].originalId =
                                    vm.jobEstimateImage[currentIndex - 1].afterImages.imagesInfo[0].originalId;
                                vm.jobEstimateImage[currentIndex - 1].afterImages.imagesInfo[0].originalId = tempOriginalId;

                            }
                            else {
                                if (vm.jobEstimateImage[currentIndex - 1].afterImages.imagesInfo.length > 0) {
                                    vm.jobEstimateImage[currentIndex - 1].afterImages.imagesInfo[0].originalId = 0;
                                }
                            }
                        }
                        else {
                            //tempOriginalId = vm.jobEstimateImage[selectedIndex - 1].afterImages.imagesInfo[0].originalId;
                            //vm.jobEstimateImage[selectedIndex - 1].afterImages.imagesInfo[0].originalId = tempOriginalId;
                        }

                        if (vm.jobEstimateImage[selectedIndex - 1].afterImages.imagesInfo.length > 0) {

                            if (vm.jobEstimateImage[currentIndex - 1].afterImages.imagesInfo.length > 0) {
                                tempStorage = vm.jobEstimateImage[currentIndex - 1].afterImages.imagesInfo;
                                vm.jobEstimateImage[currentIndex - 1].afterImages.imagesInfo =
                                    vm.jobEstimateImage[selectedIndex - 1].afterImages.imagesInfo;

                                vm.jobEstimateImage[selectedIndex - 1].afterImages.imagesInfo =
                                    tempStorage;

                            }
                            else {
                                tempStorage = [];
                                vm.jobEstimateImage[currentIndex - 1].afterImages.imagesInfo =
                                    vm.jobEstimateImage[selectedIndex - 1].afterImages.imagesInfo
                                vm.jobEstimateImage[selectedIndex - 1].afterImages.imagesInfo =
                                    tempStorage;

                            }
                        }
                        else {
                            if (vm.jobEstimateImage[currentIndex - 1].afterImages.imagesInfo.length > 0) {
                                vm.jobEstimateImage[selectedIndex - 1].afterImages.imagesInfo =
                                    vm.jobEstimateImage[currentIndex - 1].afterImages.imagesInfo;
                                vm.jobEstimateImage[currentIndex - 1].afterImages.imagesInfo = [];

                            }
                            else {
                                vm.jobEstimateImage[selectedIndex - 1].afterImages.imagesInfo =
                                    [];
                                vm.jobEstimateImage[currentIndex - 1].afterImages.imagesInfo = [];


                            }
                        }
                    }
                }

                function AddingNewPane(imageModel, value, fileModel) {
                    fileService.getFileStreamByUrl(fileModel.relativeLocation).then(function (result) {
                        $scope.imageUrl = fileService.getStreamUrl(result);

                        var imageLength = vm.jobEstimateImage.length;
                        ++imageLength;
                        angular.forEach(vm.countForHavingPaneWithImages, function (value1) {
                            var jobEstimateImagess = {};
                            angular.copy(imageModel, jobEstimateImagess);
                            var beforeAfterImagesCountWithoutImages = value1;
                            for (var i = 0; i < beforeAfterImagesCountWithoutImages[0]; i++) {
                                --beforeAfterImagesCountWithoutImages[0];
                                jobEstimateImagess.id = 0;
                                jobEstimateImagess.afterImages.imageIndex = imageLength;
                                jobEstimateImagess.afterImages.selectedIndex = imageLength;
                                jobEstimateImagess.beforeImages.imageIndex = imageLength;
                                jobEstimateImagess.beforeImages.selectedIndex = imageLength;
                                jobEstimateImagess.afterImages.id = 0;
                                jobEstimateImagess.beforeImages.id = 0;
                                jobEstimateImagess.beforeImages.imagesInfo = [];
                                jobEstimateImagess.afterImages.imagesInfo = [];
                                jobEstimateImagess.afterImages.filesList = [];
                                jobEstimateImagess.beforeImages.filesList = [];
                                jobEstimateImagess.beforeImages.filesList.push(fileModel.fileId);
                                jobEstimateImagess.beforeImages.imagesInfo.push({
                                    url: $scope.imageUrl, name: fileModel.caption, size: fileModel.size, status: fileModel.status,
                                    createdBy: fileModel.createdBy, createdOn: fileModel.createdOn, id: 0, relativeLocation: null, fileId: fileModel.fileId, isSwapped: false
                                });
                                vm.jobEstimateImage.unshift(jobEstimateImagess);
                                rearrangePaneNumber();
                            }

                            for (var i = 0; i < beforeAfterImagesCountWithoutImages[1]; i++) {
                                --beforeAfterImagesCountWithoutImages[1];
                                angular.copy(imageModel, jobEstimateImagess);
                                jobEstimateImagess.id = 0;
                                jobEstimateImagess.afterImages.imageIndex = imageLength;
                                jobEstimateImagess.beforeImages.imageIndex = imageLength;
                                jobEstimateImagess.beforeImages.selectedIndex = imageLength;
                                jobEstimateImagess.afterImages.selectedIndex = imageLength;
                                jobEstimateImagess.beforeImages.id = 0;
                                jobEstimateImagess.afterImages.id = 0;
                                jobEstimateImagess.beforeImages.filesList = [];
                                jobEstimateImagess.beforeImages.imagesInfo = [];
                                jobEstimateImagess.afterImages.imagesInfo = [];
                                jobEstimateImagess.afterImages.filesList = [];
                                jobEstimateImagess.afterImages.filesList.push(fileModel.fileId);
                                jobEstimateImagess.afterImages.imagesInfo.push({
                                    url: $scope.imageUrl, name: fileModel.caption, size: fileModel.size, status: fileModel.status,
                                    createdBy: fileModel.createdBy, createdOn: fileModel.createdOn, id: 0, relativeLocation: null, fileId: fileModel.fileId, isSwapped: false
                                });
                                vm.jobEstimateImage.unshift(jobEstimateImagess);
                                rearrangePaneNumber();
                            }
                        });

                        getListForBeforeAfterDropDown();
                    });


                }

                function bestPictureSelected(selectedIndex, imgs, isSelected) {
                    //angular.forEach(vm.jobEstimateImage, function (value, index) {
                    //    if (index == selectedIndex) {
                    //        value.beforeImages.isBestPicture = imgs.isBestPicture;
                    //        value.afterImages.isBestPicture = imgs.isBestPicture;
                    //        value.isBestPicture = imgs.isBestPicture;
                    //    }
                    //    else {
                    //        value.beforeImages.isBestPicture = false;
                    //        value.afterImages.isBestPicture = false;
                    //        value.isBestPicture = false;
                    //    }
                    //});
                    vm.imagePairMarkedQuery.isBestPairMarked = imgs.isBestPicture;
                    vm.imagePairMarkedQuery.isAddToLocalSiteGallery = imgs.isAddToLocalGallery;
                    vm.imagePairMarkedQuery.beforeImageserviceId = imgs.beforeImages.id;
                    vm.imagePairMarkedQuery.afterImageserviceId = imgs.afterImages.id;
                    vm.imagePairMarkedQuery.beforeImagecategoryId = imgs.beforeImages.categoryId;
                    vm.imagePairMarkedQuery.afterImagecategoryId = imgs.afterImages.categoryId;
                    vm.imagePairMarkedQuery.beforeImageFileId = imgs.beforeImages.imagesInfo[0].id;
                    vm.imagePairMarkedQuery.afterImageFileId = imgs.afterImages.imagesInfo[0].id;
                    if (imgs.beforeImages.imagesInfo[0].id <= 0 && imgs.afterImages.imagesInfo[0].id <= 0) {
                        toaster.error("You can mark only if both images are uploaded!");
                    }
                    beforeAfterService.bestPairMarkedJobEstimateImagePair(vm.imagePairMarkedQuery).then(function (result) {
                        if (result.data) {
                            if (isSelected) {
                                if (imgs.isBestPicture) {
                                    toaster.show("Image Pair Marked As Best Pair Successfully!");
                                }
                                else if (!imgs.isBestPicture) {
                                    toaster.show("Image Pair Unmarked As Best Pair Successfully!");
                                }
                                else {
                                    toaster.show(result.message.message);
                                }
                            }
                            else {
                                if (imgs.isAddToLocalGallery) {
                                    toaster.show("Image Pair Marked As Add To Local Site Gallery Successfully!");
                                }
                                else if (!imgs.isAddToLocalGallery) {
                                    toaster.show("Image Pair Unmarked As Add To Local Site Gallery Successfully!");
                                }
                                else {
                                    toaster.show(result.message.message);
                                }
                            }
                        }
                        else {
                            toaster.error("Error in mark image as Best Pair");
                        }
                    });
                }

                function getListForBeforeAfterDropDown() {
                    var index = 1;
                    vm.listForBeforeAfterDropDown = [];
                    angular.forEach(vm.jobEstimateImage, function (value) {
                        var displayText = index.toString();
                        vm.listForBeforeAfterDropDown.push({ display: displayText, value: index });
                        ++index;
                    });
                }

                function GetttingPaneWithHavingBothImages(isFromBefore) {
                    vm.jobEstimateImageDoublicate = [];
                    angular.forEach(vm.jobEstimateImage, function (value) {
                        if (isFromBefore) {
                            if (value.beforeImages.imagesInfo < 1) {
                                vm.jobEstimateImageDoublicate.push(value);
                            }
                        }
                        else {
                            if (value.afterImages.imagesInfo < 1) {
                                vm.jobEstimateImageDoublicate.push(value);
                            }
                        }
                    });
                }

                function rearrangePaneNumber() {
                    var length = 0;

                    angular.forEach(vm.jobEstimateImage, function (value) {
                        ++length;
                        value.beforeImages.imageIndex = length;
                        value.beforeImages.selectedIndex = length;
                        value.afterImages.imageIndex = length;
                        value.afterImages.selectedIndex = length;
                        getListForBeforeAfterDropDown();
                    });

                }

                function checkIfMarketingClassIsAsRequiredOrNot() {
                    var marketingClassName = vm.marketingClassForNote.getValue(parseInt(vm.currentMarketingClass.value));
                    if (marketingClassName == "NOTFOUND") {
                        vm.isPresentRequiredClass = false;
                    }
                    else {
                        vm.isPresentRequiredClass = true;
                    }
                }

                function magnify(imgID, index, zoom) {
                    imgID = imgID + index;
                    vm.mouseOver = true;
                    var img, glass, w, h, bw;
                    img = document.getElementById(imgID);

                    /* Create magnifier glass: */
                    glass = document.createElement("DIV");
                    glass.setAttribute("class", "img-magnifier-glass");

                    /* Insert magnifier glass: */
                    img.parentElement.insertBefore(glass, img);

                    /* Set background properties for the magnifier glass: */
                    glass.style.backgroundImage = "url('" + img.src + "')";
                    glass.style.backgroundRepeat = "no-repeat";
                    glass.style.backgroundSize = (img.width * zoom) + "px " + (img.height * zoom) + "px";
                    bw = 3;
                    w = glass.offsetWidth / 2;
                    h = glass.offsetHeight / 2;

                    /* Execute a function when someone moves the magnifier glass over the image: */
                    glass.addEventListener("mousemove", moveMagnifier);
                    img.addEventListener("mousemove", moveMagnifier);

                    /*and also for touch screens:*/
                    glass.addEventListener("touchmove", moveMagnifier);
                    img.addEventListener("touchmove", moveMagnifier);
                    function moveMagnifier(e) {
                        var pos, x, y;
                        /* Prevent any other actions that may occur when moving over the image */
                        e.preventDefault();
                        /* Get the cursor's x and y positions: */
                        pos = getCursorPos(e);
                        x = pos.x;
                        y = pos.y;
                        /* Prevent the magnifier glass from being positioned outside the image: */
                        if (x > img.width - (w / zoom)) { x = img.width - (w / zoom); }
                        if (x < w / zoom) { x = w / zoom; }
                        if (y > img.height - (h / zoom)) { y = img.height - (h / zoom); }
                        if (y < h / zoom) { y = h / zoom; }
                        /* Set the position of the magnifier glass: */
                        glass.style.left = (x - w) + "px";
                        glass.style.top = (y - h) + "px";
                        /* Display what the magnifier glass "sees": */
                        glass.style.backgroundPosition = "-" + ((x * zoom) - w + bw) + "px -" + ((y * zoom) - h + bw) + "px";
                    }

                    function getCursorPos(e) {
                        var a, x = 0, y = 0;
                        e = e || window.event;
                        /* Get the x and y positions of the image: */
                        a = img.getBoundingClientRect();
                        /* Calculate the cursor's x and y coordinates, relative to the image: */
                        x = e.pageX - a.left;
                        y = e.pageY - a.top;
                        /* Consider any page scrolling: */
                        x = x - window.pageXOffset;
                        y = y - window.pageYOffset;
                        return { x: x, y: y };
                    }
                }
                function removeMagnify() {
                    if (vm.mouseOver) {
                        vm.mouseOver = false;
                        return;
                    }
                    var lists = document.getElementsByClassName("img-magnifier-glass");
                    angular.forEach(lists, function (list) {
                        list.stylea.display = "none";
                    });

                }



                function getIdName(name, index) {
                    return name + index;
                }

                function googleApi(address) {
                    $scope.isMap = true;
                    $scope.customerAddress = address;
                    var value = getOperatingSystem();
                    var isMobile = navigator.userAgent.match(/(iPhone|iPod|iPad|Android|webOS|BlackBerry|IEMobile|Opera Mini|Win32|Win64)/i);
                    if (value.mobile == true) {
                        if (value.os != "Android") {
                            if (value.os == "iOS") {
                                var url = "https://maps.apple.com?address=" + address;
                                console.log(url);
                                window.open(url, '_blank');
                                //window.location.href = url;
                                $scope.isMap = false;
                            }
                            else {
                                var url = "http://maps.google.com/maps?daddr=" + address;
                                window.open(url, '_blank');
                                $scope.isMap = false;
                            }
                        }

                        else {
                            $scope.isMap = false;
                            var url = "geo:?q=" + address + "&z=15";
                            window.location.href = url;
                        }
                    }
                    else {
                        var url = "https://www.google.com/maps/dir/?api=1&destination=" + address + "&dir_action=navigate";
                        window.open(url, '_blank');
                        $scope.isMap = false;
                    }
                }


                function getOperatingSystem() {
                    var unknown = '-';
                    var width = '';
                    var height = '';
                    // screen
                    var screenSize = '';
                    if (screen.width) {
                        width = (screen.width) ? screen.width : '';
                        height = (screen.height) ? screen.height : '';
                        screenSize += '' + width + " x " + height;
                    }

                    // browser
                    var nVer = navigator.appVersion;
                    var nAgt = navigator.userAgent;
                    var browser = navigator.appName;
                    var version = '' + parseFloat(navigator.appVersion);
                    var majorVersion = parseInt(navigator.appVersion, 10);
                    var nameOffset, verOffset, ix;

                    // Opera
                    if ((verOffset = nAgt.indexOf('Opera')) != -1) {
                        browser = 'Opera';
                        version = nAgt.substring(verOffset + 6);
                        if ((verOffset = nAgt.indexOf('Version')) != -1) {
                            version = nAgt.substring(verOffset + 8);
                        }
                    }
                    // Opera Next
                    if ((verOffset = nAgt.indexOf('OPR')) != -1) {
                        browser = 'Opera';
                        version = nAgt.substring(verOffset + 4);
                    }
                    // Edge
                    else if ((verOffset = nAgt.indexOf('Edge')) != -1) {
                        browser = 'Microsoft Edge';
                        version = nAgt.substring(verOffset + 5);
                    }
                    // MSIE
                    else if ((verOffset = nAgt.indexOf('MSIE')) != -1) {
                        browser = 'Microsoft Internet Explorer';
                        version = nAgt.substring(verOffset + 5);
                    }
                    // Chrome
                    else if ((verOffset = nAgt.indexOf('Chrome')) != -1) {
                        browser = 'Chrome';
                        version = nAgt.substring(verOffset + 7);
                    }
                    // Safari
                    else if ((verOffset = nAgt.indexOf('Safari')) != -1) {
                        browser = 'Safari';
                        version = nAgt.substring(verOffset + 7);
                        if ((verOffset = nAgt.indexOf('Version')) != -1) {
                            version = nAgt.substring(verOffset + 8);
                        }
                    }
                    // Firefox
                    else if ((verOffset = nAgt.indexOf('Firefox')) != -1) {
                        browser = 'Firefox';
                        version = nAgt.substring(verOffset + 8);
                    }
                    // MSIE 11+
                    else if (nAgt.indexOf('Trident/') != -1) {
                        browser = 'Microsoft Internet Explorer';
                        version = nAgt.substring(nAgt.indexOf('rv:') + 3);
                    }
                    // Other browsers
                    else if ((nameOffset = nAgt.lastIndexOf(' ') + 1) < (verOffset = nAgt.lastIndexOf('/'))) {
                        browser = nAgt.substring(nameOffset, verOffset);
                        version = nAgt.substring(verOffset + 1);
                        if (browser.toLowerCase() == browser.toUpperCase()) {
                            browser = navigator.appName;
                        }
                    }
                    // trim the version string
                    if ((ix = version.indexOf(';')) != -1) version = version.substring(0, ix);
                    if ((ix = version.indexOf(' ')) != -1) version = version.substring(0, ix);
                    if ((ix = version.indexOf(')')) != -1) version = version.substring(0, ix);

                    majorVersion = parseInt('' + version, 10);
                    if (isNaN(majorVersion)) {
                        version = '' + parseFloat(navigator.appVersion);
                        majorVersion = parseInt(navigator.appVersion, 10);
                    }

                    // mobile version
                    var mobile = /Mobile|mini|Fennec|Android|iP(ad|od|hone)/.test(nVer);

                    // cookie
                    var cookieEnabled = (navigator.cookieEnabled) ? true : false;

                    if (typeof navigator.cookieEnabled == 'undefined' && !cookieEnabled) {
                        document.cookie = 'testcookie';
                        cookieEnabled = (document.cookie.indexOf('testcookie') != -1) ? true : false;
                    }

                    // system
                    var os = unknown;
                    var clientStrings = [
                        { s: 'Windows 10', r: /(Windows 10.0|Windows NT 10.0)/ },
                        { s: 'Windows 8.1', r: /(Windows 8.1|Windows NT 6.3)/ },
                        { s: 'Windows 8', r: /(Windows 8|Windows NT 6.2)/ },
                        { s: 'Windows 7', r: /(Windows 7|Windows NT 6.1)/ },
                        { s: 'Windows Vista', r: /Windows NT 6.0/ },
                        { s: 'Windows Server 2003', r: /Windows NT 5.2/ },
                        { s: 'Windows XP', r: /(Windows NT 5.1|Windows XP)/ },
                        { s: 'Windows 2000', r: /(Windows NT 5.0|Windows 2000)/ },
                        { s: 'Windows ME', r: /(Win 9x 4.90|Windows ME)/ },
                        { s: 'Windows 98', r: /(Windows 98|Win98)/ },
                        { s: 'Windows 95', r: /(Windows 95|Win95|Windows_95)/ },
                        { s: 'Windows NT 4.0', r: /(Windows NT 4.0|WinNT4.0|WinNT|Windows NT)/ },
                        { s: 'Windows CE', r: /Windows CE/ },
                        { s: 'Windows 3.11', r: /Win16/ },
                        { s: 'Android', r: /Android/ },
                        { s: 'Open BSD', r: /OpenBSD/ },
                        { s: 'Sun OS', r: /SunOS/ },
                        { s: 'Linux', r: /(Linux|X11)/ },
                        { s: 'iOS', r: /(iPhone|iPad|iPod)/ },
                        { s: 'Mac OS X', r: /Mac OS X/ },
                        { s: 'Mac OS', r: /(MacPPC|MacIntel|Mac_PowerPC|Macintosh)/ },
                        { s: 'QNX', r: /QNX/ },
                        { s: 'UNIX', r: /UNIX/ },
                        { s: 'BeOS', r: /BeOS/ },
                        { s: 'OS/2', r: /OS\/2/ },
                        { s: 'Search Bot', r: /(nuhk|Googlebot|Yammybot|Openbot|Slurp|MSNBot|Ask Jeeves\/Teoma|ia_archiver)/ }
                    ];
                    for (var id in clientStrings) {
                        var cs = clientStrings[id];
                        if (cs.r.test(nAgt)) {
                            os = cs.s;
                            break;
                        }
                    }

                    var osVersion = unknown;

                    if (/Windows/.test(os)) {
                        osVersion = /Windows (.*)/.exec(os)[1];
                        os = 'Windows';
                    }

                    switch (os) {
                        case 'Mac OS X':
                            osVersion = /Mac OS X (10[\.\_\d]+)/.exec(nAgt)[1];
                            break;

                        case 'Android':
                            osVersion = /Android ([\.\_\d]+)/.exec(nAgt)[1];
                            break;

                        case 'iOS':
                            osVersion = /OS (\d+)_(\d+)_?(\d+)?/.exec(nVer);
                            osVersion = osVersion[1] + '.' + osVersion[2] + '.' + (osVersion[3] | 0);
                            break;
                    }

                    var flashVersion = 'no check';
                    if (typeof swfobject != 'undefined') {
                        var fv = swfobject.getFlashPlayerVersion();
                        if (fv.major > 0) {
                            flashVersion = fv.major + '.' + fv.minor + ' r' + fv.release;
                        }
                        else {
                            flashVersion = unknown;
                        }
                    }

                    return {
                        screen: screenSize,
                        browser: browser,
                        browserVersion: version,
                        browserMajorVersion: majorVersion,
                        mobile: mobile,
                        os: os,
                        osVersion: osVersion,
                        cookies: cookieEnabled,
                        flashVersion: flashVersion
                    }
                }


                function changeTab(tabNo) {
                    if (!vm.isBeforeAfterImagesSave) {
                        notification.showConfirm("Would you like to save the unsaved before After Image Information?", "Warning Message:", function () {
                            save(true);
                            vm.isCancelledClick = false;
                            changeTabFocus(tabNo);

                        }, function () {
                            vm.isCancelledClick = true;
                            document.getElementById("beforeAfter").classList.add("active");
                            document.getElementById("invoice").classList.remove("active");
                            document.getElementById("building").classList.remove("active");
                            document.getElementById("general").classList.remove("active");
                            changeTabFocusForCancellation(tabNo);
                        })
                    }

                    if (!vm.isInvoiceSave) {
                        notification.showConfirm("Would you like to save the unsaved Invoice Information?", "Warning Message:", function () {
                            save(false);
                            vm.isCancelledClick = false;
                            changeTabFocus(tabNo);
                        }, function () {
                            vm.isCancelledClick = true;
                            changeTabFocus(tabNo);
                            document.getElementById("beforeAfter").classList.remove("active");
                            document.getElementById("invoice").classList.add("active");
                            document.getElementById("building").classList.remove("active");
                            document.getElementById("general").classList.remove("active");
                            changeTabFocusForCancellation(tabNo);
                        })
                    }

                    if (!vm.isExteriorImageSave) {
                        notification.showConfirm(" Would you like to save the unsaved Exterior Image Information?", "Warning Message:", function () {
                            save(false);
                            vm.isCancelledClick = false;
                            changeTabFocus(tabNo);
                        }, function () {
                            vm.isCancelledClick = true;
                            document.getElementById("beforeAfter").classList.remove("active");
                            document.getElementById("invoice").classList.remove("active");
                            document.getElementById("building").classList.add("active");
                            document.getElementById("general").classList.remove("active");
                            changeTabFocusForCancellation(tabNo);
                        })

                    }
                    if (vm.isExteriorImageSave && vm.isInvoiceSave && vm.isBeforeAfterImagesSave) {
                        changeTabFocus(tabNo);
                    }
                }

                function changeTabFocusForCancellation(tabNo) {
                    if (!vm.isBeforeAfterImagesSave) {
                        vm.isGenernalTab = false;
                        vm.isInvoiceTab = false;
                        vm.isBuildinglTab = false;
                        vm.isBeforeAfterlTab = true;
                        document.getElementById("beforeAfter").classList.add("active");
                        document.getElementById("invoice").classList.remove("active");
                        document.getElementById("building").classList.remove("active");
                        document.getElementById("general").classList.remove("active");
                    }

                    if (!vm.isExteriorImageSave) {
                        vm.isGenernalTab = false;
                        vm.isInvoiceTab = false;
                        vm.isBuildinglTab = true;
                        vm.isBeforeAfterlTab = false;
                        document.getElementById("beforeAfter").classList.remove("active");
                        document.getElementById("invoice").classList.remove("active");
                        document.getElementById("building").classList.add("active");
                        document.getElementById("general").classList.remove("active");
                    }

                    if (!vm.isInvoiceSave) {
                        vm.isGenernalTab = false;
                        vm.isInvoiceTab = true;
                        vm.isBuildinglTab = false;
                        vm.isBeforeAfterlTab = false;
                        document.getElementById("beforeAfter").classList.remove("active");
                        document.getElementById("invoice").classList.add("active");
                        document.getElementById("building").classList.remove("active");
                        document.getElementById("general").classList.remove("active");
                    }

                    if (!vm.isGeneralTabActive) {
                        vm.isGenernalTab = true;
                        vm.isInvoiceTab = false;
                        vm.isBuildinglTab = false;
                        vm.isBeforeAfterlTab = false;
                        document.getElementById("beforeAfter").classList.remove("active");
                        document.getElementById("invoice").classList.remove("active");
                        document.getElementById("building").classList.remove("active");
                        document.getElementById("general").classList.add("active");
                    }
                }


                function changeTabFocus(tabNo) {
                    if (vm.isInValid) {
                        changeTabFocusForCancellation(1);
                        vm.isInValid = false;
                        return;
                    }
                    if (tabNo == 4) {
                        vm.isGenernalTab = false;
                        vm.isInvoiceTab = false;
                        vm.isBuildinglTab = false;
                        vm.isBeforeAfterlTab = true;
                        document.getElementById("beforeAfter").classList.add("active");
                        document.getElementById("invoice").classList.remove("active");
                        document.getElementById("building").classList.remove("active");
                        document.getElementById("general").classList.remove("active");
                    }

                    if (tabNo == 3) {
                        vm.isGenernalTab = false;
                        vm.isInvoiceTab = false;
                        vm.isBuildinglTab = true;
                        vm.isBeforeAfterlTab = false;
                        document.getElementById("beforeAfter").removeClass = "active";
                        document.getElementById("invoice").removeClass = "active";
                        document.getElementById("building").addClass = "active";
                        document.getElementById("general").removeClass = "active";
                    }

                    if (tabNo == 2) {
                        vm.isGenernalTab = false;
                        vm.isInvoiceTab = true;
                        vm.isBuildinglTab = false;
                        vm.isBeforeAfterlTab = false;
                        document.getElementById("beforeAfter").removeClass = "active";
                        document.getElementById("invoice").addClass = "active";
                        document.getElementById("building").removeClass = "active";
                        document.getElementById("general").removeClass = "active";
                    }

                    if (tabNo == 1) {
                        vm.isGenernalTab = true;
                        vm.isInvoiceTab = false;
                        vm.isBuildinglTab = false;
                        vm.isBeforeAfterlTab = false;
                        document.getElementById("beforeAfter").removeClass = "active";
                        document.getElementById("invoice").removeClass = "active";
                        document.getElementById("building").removeClass = "active";
                        document.getElementById("general").addClass = "active";
                    }
                }



                $rootScope.$on("$stateChangeStart", function (event, toState, toParams, fromState, fromParams) {
                    if (!vm.isBeforeAfterImagesSave) {
                        event.preventDefault();

                        notification.showConfirm("Would you like to save the unsaved Before After Image Information?", "Warning Message:", function () {
                            vm.isBeforeAfterImagesSave = true;
                            save(true);
                            toState.params.franchiseeId = $scope.franchiseeId;
                            toState.params.rowId = vm.rowId;
                            toState.params.id = vm.jobId;
                            $state.go(toState.name, toState.params);
                        })
                    }

                    if (!vm.isInvoiceSave) {
                        event.preventDefault();
                        notification.showConfirm("Would you like to save the unsaved Invoice Information?", "Warning Message:", function () {
                            vm.isInvoiceSave = true;
                            save(false);
                            toState.params.franchiseeId = $scope.franchiseeId;
                            toState.params.rowId = vm.rowId;
                            toState.params.id = vm.jobId;
                            $state.go(toState.name, toState.params);
                        })
                    }

                    if (!vm.isExteriorImageSave) {
                        event.preventDefault();
                        notification.showConfirm(" Would you like to save the unsaved Exterior Image Information?", "Warning Message:", function () {
                            vm.isExteriorImageSave = true;
                            save(false);
                            toState.params.franchiseeId = $scope.franchiseeId;
                            toState.params.rowId = vm.rowId;
                            toState.params.id = vm.jobId;
                            $state.go(toState.name, toState.params);
                        })
                    }
                });



                function openFollowUpModal() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/show-follow-up.client.view.html',
                        controller: 'ShowFollowUpController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Franchisee: vm.jobInfo.franchisee,
                                    FranchiseeId: vm.jobInfo.franchiseeId,
                                    CustomerInformation: vm.jobInfo.jobCustomer,
                                    JobSchedulerId: vm.rowId
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                        getFollowUp();
                    })
                };


                function openViewFollowUpModal() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/show.followup.client.view.html',
                        controller: 'ViewFollowUpController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    FollowUpList: vm.list,
                                    IsFromScheduler: false,
                                    CustomerInfo: vm.jobInfo.jobCustomer
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        getFollowUp();
                    }, function () {
                        getFollowUp();
                    })
                };

                function getFollowUp() {
                    return toDoService.getCommentToDoForScheduler(vm.rowId).then(function (result) {
                        if (result != null) {
                            vm.followUpList = result.data.commentList;
                            vm.list = result.data.totalCommentList;
                            vm.isMoreThanFive = result.data.isMoreThanFive;
                            $rootScope.identity.todayToDoCount = result.data.todayToDoCount;
                        }
                    });
                }
                function rotate(id, slide, index, parentIndex, from) {
                    var imgs = vm.jobEstimateImage[parentIndex];
                    var splt = "";
                    if (slide.css == null) {
                        slide.css = 0;
                    }
                    if ((slide.css != "")) {
                        splt = slide.css.split('(')[1].split(')')[0].replace("deg", "");
                        splt = parseInt(splt, 10);
                        vm.index = splt;
                        slide.css = vm.index;
                    }
                    var splt = "";
                    var myElem = document.getElementById(id);
                    vm.css = "";
                    var img;
                    splt = slide.css + 90;
                    vm.index = splt;
                    slide.css = "rotate(" + splt + "deg)";
                    if (from == 'after') {
                        imgs.afterImages.css = slide.css;
                    }
                    else {
                        imgs.beforeImages.css = slide.css;
                    }
                }
                function deleteFromUI(followUp, index) {
                    followUp.isVisible = false;
                    var index = 0;
                    var found = false;
                    angular.forEach(vm.list, function (value) {
                        if (!found) {
                            index += 1;
                        }
                        if (value.id == followUp.id) {
                            found = true;
                        }
                    });
                    vm.followUpList = $filter('filter')(vm.followUpList, { isVisible: true });
                    vm.isMoreThanFive = vm.followUpList.length > 5 ? true : false;
                    if (!vm.isMoreThanFive && vm.list.length > 5) {
                        var nextElementAfterDeletion = vm.list[index + 1];
                        if (nextElementAfterDeletion != undefined) {
                            vm.followUpList.push(nextElementAfterDeletion);
                            vm.isMoreThanFive = vm.followUpList.length > 5 ? true : false;
                            if (vm.list.length > index) {
                                vm.isMoreThanFive = true;
                            }
                        }
                    }
                }

                function deleteToDo(followUp, index) {
                    notification.showConfirm("You are about to delete a To-Do/FollowUp . Do you want to proceed?", "Warning Message:", function () {
                        return toDoService.deleteToDo(followUp.id).then(function (result) {
                            if (result.data != true)
                                toaster.error("Error in Deleting To-Do/FollowUp");
                            else {
                                toaster.show("To-Do/FollowUp Deleted Successfully");
                                followUp.isVisible = false;
                                vm.elementInList = $filter('filter')(vm.list, { id: followUp.id });
                                var index = 0;
                                var found = false;
                                angular.forEach(vm.list, function (value) {
                                    if (!found) {
                                        index += 1;
                                    }
                                    if (value.id == followUp.id) {
                                        found = true;
                                    }
                                });
                                vm.followUpList = $filter('filter')(vm.followUpList, { isVisible: true });
                                vm.isMoreThanFive = vm.followUpList.length > 5 ? true : false;
                                if (!vm.isMoreThanFive && vm.list.length > 5) {
                                    var nextElementAfterDeletion = vm.list[index + 1];
                                    if (nextElementAfterDeletion != undefined) {
                                        vm.followUpList.push(nextElementAfterDeletion);
                                        vm.isMoreThanFive = vm.followUpList.length > 5 ? true : false;
                                        if (vm.list.length > index) {
                                            vm.isMoreThanFive = true;
                                        }
                                    }
                                }
                            }
                            //$scope.editMode = !($scope.editMode);
                            vm.isProcessing = false;
                        });
                    })
                }

                function uploadMediaAfterBeforeForCamera(id, imgs, isFromBefore) {
                    const element = document.getElementById(id);
                    vm.imgs = imgs;
                    vm.isFromBefore = isFromBefore;

                    if (element != null) {
                        element.click();
                    }
                }
                $scope.fileNameChanged = function (files) {
                    filesList(files, vm.imgs, vm.isFromBefore);
                }

                function filesList(files) {
                    vm.info = {};
                    vm.info.fileList = [];
                    var img = vm.imgs;
                    var afterImages = img;
                    var beforeImages = img;
                    fileService.uploadForBeforeAfter(files).then(function (result) {
                        if (result.data != null) {
                            vm.info.fileList.push(result.data);
                            vm.isProcessing = false;
                            return schedulerService.saveMediaBeforeAfterForUser(vm.info).then(function (result) {
                                if (result.data != null) {
                                    var isFromBefore = vm.isFromBefore;
                                    $rootScope.$emit("FileId", result.data);
                                    $rootScope.$emit("IsImageForSamePane", vm.isImageForSamePane);
                                    toaster.show(result.message.message);

                                    var length = vm.jobEstimateImage.length;
                                    if (length == 1) {
                                        vm.isBeforeAfterImagesSave = false;
                                        var beforeImageCount = vm.jobEstimateImage[0].beforeImages.imagesInfo.length;
                                        var afterImageCount = vm.jobEstimateImage[0].afterImages.imagesInfo.length;
                                        if (vm.isFromBefore) {
                                            vm.beforeAfterImagesFirstUploadedImages[0][0] = beforeImageCount;
                                        }
                                        else {
                                            vm.beforeAfterImagesFirstUploadedImages[0][1] = afterImageCount;
                                        }
                                    }
                                    else {
                                        angular.forEach(vm.jobEstimateImage, function (value1) {
                                            vm.isBeforeAfterImagesSave = false;
                                            if (value1.beforeImages.imagesInfo.length > 0) {
                                                vm.beforeAfterImagesFirstUploadedImages[0][0] += 1;
                                            }
                                            if (value1.afterImages.imagesInfo.length > 0) {
                                                vm.beforeAfterImagesFirstUploadedImages[0][1] += 1;
                                            }

                                        });
                                    }
                                    var isParsedImageData = false;
                                    var imageLength = vm.jobEstimateImage.length;
                                    imageLength = imageLength + 1;

                                    // when more than one image is uploaded so that uploaded either for that pane or different pane
                                    if (((!vm.isFromBefore && vm.beforeAfterImagesFirstUploadedImages[0][0] > 1) ||
                                        (isFromBefore && vm.beforeAfterImagesFirstUploadedImages[0][1] > 1)) && vm.fileModel.length > 1) {
                                        var index = -1;
                                        GetttingPaneWithHavingBothImages(vm.isFromBefore);
                                        angular.forEach(vm.fileModel, function (fileModel) {
                                            vm.isImageRemoved = true;
                                            ++index;
                                            var value = vm.jobEstimateImageDoublicate[index];
                                            if (value != null) {
                                                var isImageAlreadyPresentinPane = false;
                                                if (isFromBefore) {
                                                    if (value.beforeImages.imagesInfo.length > 0) {
                                                        isImageAlreadyPresentinPane = true;
                                                        --index;
                                                    }
                                                }
                                                else {
                                                    if (value.afterImages.imagesInfo.length > 0) {
                                                        isImageAlreadyPresentinPane = true;
                                                        --index;
                                                    }
                                                }
                                                if (!isImageAlreadyPresentinPane) {
                                                    var imageLength = vm.jobEstimateImage.length;
                                                    imageLength = imageLength + 1;

                                                    //var fileModel = vm.fileModel[index];

                                                    fileService.getFileStreamByUrl(fileModel.relativeLocation).then(function (result) {
                                                        $scope.imageUrl = fileService.getStreamUrl(result);
                                                        jobEstimateImagess = {};
                                                        angular.copy(imageModel, jobEstimateImagess);
                                                        if (vm.isFromBefore) {
                                                            jobEstimateImagess.beforeImages.imagesInfo = [];
                                                            if (value.beforeImages.imagesInfo.length < 1) {
                                                                value.beforeImages.filesList = [];
                                                                value.afterImages.imageIndex = value.beforeImages.imageIndex;
                                                                value.beforeImages.filesList.push(fileModel.fileId);
                                                                value.beforeImages.imagesInfo = [];
                                                                value.beforeImages.imagesInfo.push({
                                                                    url: $scope.imageUrl, name: fileModel.caption, size: fileModel.size, status: fileModel.status,
                                                                    createdBy: fileModel.createdBy, createdOn: fileModel.createdOn, id: 0, relativeLocation: null, fileId: fileModel.fileId, isSwapped: false
                                                                });
                                                            }
                                                            else {
                                                                vm.countForHavingPaneWithImages[0][0] += 1;
                                                            }
                                                        }
                                                        else {
                                                            var jobEstimateImagess = {};
                                                            angular.copy(imageModel, jobEstimateImagess);
                                                            value.afterImages.originalImageIndex = value.afterImages.selectedIndex;
                                                            jobEstimateImagess.afterImages.imagesInfo = [];
                                                            if (value.afterImages.imagesInfo.length < 1) {
                                                                value.afterImages.filesList = [];
                                                                value.afterImages.filesList.push(fileModel.fileId);
                                                                //value.beforeImages.imagesInfo = [];
                                                                value.afterImages.imagesInfo.push({
                                                                    url: $scope.imageUrl, name: fileModel.caption, size: fileModel.size, status: fileModel.status,
                                                                    createdBy: fileModel.createdBy, createdOn: fileModel.createdOn, id: 0, relativeLocation: null, fileId: fileModel.fileId, isSwapped: false
                                                                });
                                                            }
                                                            else {
                                                                vm.countForHavingPaneWithImages[0][1] += 1;
                                                            }
                                                        }
                                                        AddingNewPane(imageModel, value, fileModel);
                                                    });
                                                }
                                            }
                                            else {
                                                if (isFromBefore) {
                                                    vm.countForHavingPaneWithImages[0][0] += 1;
                                                }
                                                else {
                                                    vm.countForHavingPaneWithImages[0][1] += 1;
                                                }
                                            }
                                            AddingNewPane(imageModel, value, fileModel);
                                        });
                                    }

                                    else {
                                        vm.isDeleted = false;
                                        angular.forEach(vm.fileModel, function (value, index) {
                                            vm.isImageRemoved = true;
                                            img.filesList.push(value.fileId);
                                            fileService.getFileStreamByUrl(value.relativeLocation).then(function (result) {
                                                var length = vm.jobEstimateImage.length;
                                                length = length + 1;
                                                $scope.imageUrl = fileService.getStreamUrl(result);
                                                vm.isBeforeAFterSave = true;

                                                // when there is no image pane for first time use
                                                if (img.imagesInfo.length == 0 && vm.beforeAfterImagesFirstUploadedImages[0][0] == 0 && vm.beforeAfterImagesFirstUploadedImages[0][1] == 0) {
                                                    if (vm.isFromBefore) {
                                                        afterImages.imageIndex = beforeImages.imageIndex;
                                                        afterImages.selectedIndex = afterImages.imageIndex;
                                                    }
                                                    else {
                                                        beforeImages.imageIndex = afterImages.imageIndex;
                                                        beforeImages.selectedIndex = afterImages.imageIndex;
                                                    }
                                                    img.filesList = [];
                                                    img.filesList.push(value.fileId);
                                                    img.imagesInfo = [];

                                                    img.imagesInfo.push({
                                                        url: $scope.imageUrl, name: value.caption, size: value.size, status: value.status,
                                                        createdBy: value.createdBy, createdOn: value.createdOn, id: 0, relativeLocation: null, fileId: value.fileId, isSwapped: false
                                                    });
                                                    vm.isDeleted = true;
                                                }

                                                // when only one image is uploaded so that it will be replace only in that pane
                                                else if (vm.fileModel.length == 1) {
                                                    if (img.imagesInfo.length > 1) {
                                                        img.id = 0;
                                                        if (isFromBefore) {
                                                            afterImages.imageIndex = img.imageIndex;
                                                        }
                                                        else {
                                                            beforeImages.imageIndex = img.imageIndex;
                                                        }
                                                        img.imagesInfo[0].url = $scope.imageUrl;
                                                        img.imagesInfo[0].name = value.caption;
                                                        img.imagesInfo[0].size = value.size;
                                                        img.imagesInfo[0].status = value.status;
                                                        img.imagesInfo[0].createdBy = value.createdBy;
                                                        img.imagesInfo[0].createdOn = value.createdOn;
                                                        img.imagesInfo[0].id = 0;
                                                        img.filesList = [];
                                                        img.filesList.push(value.fileId);
                                                        img.imagesInfo[0].relativeLocation = null;
                                                        img.imagesInfo[0].fileId = value.fileId;
                                                    }
                                                    else {
                                                        img.imagesInfo = [];
                                                        img.filesList = [];
                                                        img.filesList.push(value.fileId);
                                                        img.imagesInfo.push({
                                                            url: $scope.imageUrl, name: value.caption, size: value.size, status: value.status,
                                                            createdBy: value.createdBy, createdOn: value.createdOn, id: 0, relativeLocation: null, fileId: value.fileId, isSwapped: false
                                                        });
                                                    }
                                                    vm.isDeleted = true;
                                                }

                                                else if (vm.fileModel.length > 1) {
                                                    var index11 = -1;
                                                    var jobEstimateImagess = {};
                                                    var isIndexFound = false;

                                                    if (!vm.isDeleted) {
                                                        vm.jobEstimateImage.splice(vm.isSeletedIndex, 1);
                                                        vm.isDeleted = true;
                                                    }

                                                    length = vm.jobEstimateImage.length;
                                                    length = length + 1;
                                                    if (vm.isFromBefore) {
                                                        angular.copy(imageModel, jobEstimateImagess);
                                                        var count = vm.fileModel.length;
                                                        jobEstimateImagess.id = 0;
                                                        jobEstimateImagess.beforeImages.imageIndex = length;
                                                        jobEstimateImagess.afterImages.imageIndex = length;
                                                        jobEstimateImagess.afterImages.selectedIndex = length;
                                                        jobEstimateImagess.beforeImages.selectedIndex = length;
                                                        jobEstimateImagess.afterImages.id = 0;
                                                        jobEstimateImagess.afterImages.imagesInfo = [];
                                                        jobEstimateImagess.beforeImages.id = 0;

                                                        jobEstimateImagess.beforeImages.isBeforeImage = img.isBeforeImage;
                                                        jobEstimateImagess.afterImages.isBeforeImage = img.isBeforeImage;

                                                        jobEstimateImagess.beforeImages.imagesInfo = [];
                                                        jobEstimateImagess.beforeImages.filesList = [];
                                                        jobEstimateImagess.beforeImages.filesList.push(value.fileId);
                                                        jobEstimateImagess.beforeImages.imagesInfo = [];
                                                        jobEstimateImagess.afterImages.filesList = [];
                                                        jobEstimateImagess.beforeImages.imagesInfo.push({
                                                            url: $scope.imageUrl, name: value.caption, size: value.size, status: value.status,
                                                            createdBy: value.createdBy, createdOn: value.createdOn, id: 0, relativeLocation: null, fileId: value.fileId, isSwapped: false
                                                        });
                                                        rearrangePaneNumber();
                                                    }
                                                    else {
                                                        angular.copy(imageModel, jobEstimateImagess);
                                                        var count = vm.fileModel.length;
                                                        jobEstimateImagess.id = 0;
                                                        jobEstimateImagess.beforeImages.isBeforeImage = img.isBeforeImage;
                                                        jobEstimateImagess.afterImages.isBeforeImage = img.isBeforeImage;
                                                        jobEstimateImagess.beforeImages.imageIndex = length;
                                                        jobEstimateImagess.afterImages.imageIndex = length;
                                                        jobEstimateImagess.beforeImages.id = 0;
                                                        jobEstimateImagess.afterImages.id = 0;
                                                        jobEstimateImagess.afterImages.imagesInfo = [];
                                                        jobEstimateImagess.beforeImages.imagesInfo = [];
                                                        jobEstimateImagess.afterImages.filesList = [];
                                                        jobEstimateImagess.afterImages.filesList.push(value.fileId);
                                                        jobEstimateImagess.afterImages.imagesInfo = [];
                                                        jobEstimateImagess.deleteRow.filesList = [];
                                                        jobEstimateImagess.afterImages.imagesInfo.push({
                                                            url: $scope.imageUrl, name: value.caption, size: value.size, status: value.status,
                                                            createdBy: value.createdBy, createdOn: value.createdOn, id: 0, relativeLocation: null, fileId: value.fileId, isSwapped: false
                                                        });
                                                    }
                                                    vm.jobEstimateImage.unshift(jobEstimateImagess);
                                                    rearrangePaneNumber();
                                                }

                                                getListForBeforeAfterDropDown();
                                                if (index == vm.fileModel.length - 1) {
                                                    vm.save(true);
                                                }
                                            });
                                        });
                                        getListForBeforeAfterDropDown();
                                    }
                                }
                            }).catch(function (err) {
                                vm.isProcessing = false;
                            });
                        }
                    })
                }


                function downloadImage(slider) {
                    vm.isBeforeAfterImagesSave = true;
                    if (slider.isImageCropped) {
                        return fileService.getFileForDownloadForBeforeAfter(slider.croppedImageFileId).then(function (result) {
                            fileService.downloadFileImage(result.data, "before_after_images");
                        });
                    }
                    else {
                        return fileService.getFileForDownloadForBeforeAfter(slider.fileId).then(function (result) {
                            fileService.downloadFileImage(result.data, "before_after_images");
                        });
                    }
                }
                function downloadImages() {
                    vm.isBeforeAfterImagesSave = true;
                    var filesIds = [];
                    var filesNames = [];
                    angular.forEach(vm.jobEstimateImage, function (value1) {
                        //vm.isBeforeAfterImagesSave = false;
                        angular.forEach(value1.afterImages.imagesInfo, function (value2) {
                            filesIds.push(value2.fileId);
                            filesNames.push(value2.caption)
                        });
                        angular.forEach(value1.beforeImages.imagesInfo, function (value2) {
                            filesIds.push(value2.fileId);
                            filesNames.push(value2.caption)
                        })
                    });
                    //angular.forEach(filesIds, function (fileId, index) {
                    //    var fileName = filesNames[index];
                    //    return fileService.getFileForDownloadForBeforeAfter(fileId).then(function (result) {
                    //        fileService.downloadFileImage(result.data, fileName );
                    //    });
                    //})

                    filesIds = JSON.stringify({ FileModel: filesIds });
                    return fileService.uploadBeforeAfterZipFile(filesIds).then(function (result) {
                        vm.fileName = result.data;
                        return fileService.downloadBeforeAfterZipFile(result.data).then(function (result) {
                            var blob = new Blob([result.data], { type: "application/zip" });
                            var fileName = vm.fileName + ".zip";
                            var a = document.createElement("a");
                            document.body.appendChild(a);
                            a.style = "display:none";
                            var url = window.URL.createObjectURL(blob);
                            a.href = url;
                            a.download = fileName;
                            a.click();
                            window.URL.revokeObjectURL(url);
                            a.remove();
                            toaster.show("Images downloaded successfully. Please check the Download folder to view all the images.");
                        });
                    });
                }

                function creatingLightBox() {
                }
                var zoomImg = function () {
                    // (A) CREATE EVIL IMAGE CLONE
                    var clone = this.cloneNode();
                    clone.classList.remove("zoomD");

                    // (B) PUT EVIL CLONE INTO LIGHTBOX
                    var lb = document.getElementById("lb-img");
                    lb.innerHTML = "";
                    lb.appendChild(clone);

                    // (C) SHOW LIGHTBOX
                    lb = document.getElementById("lb-back");
                    lb.classList.add("show");
                };

                function zoom(className) {
                    var images = document.getElementsByClassName(className);
                    images.removeClass = "zoomD";
                    var clone = images[0].cloneNode(true);;
                    // (B) PUT EVIL CLONE INTO LIGHTBOX
                    var lb = document.getElementById("lb-img");
                    lb.innerHTML = "";
                    lb.appendChild(clone);

                    // (C) SHOW LIGHTBOX
                    lb = document.getElementById("lb-back");
                    lb.classList.add("show");
                }

                function zoomin(className, contailerId, index, sendFrom) {
                    vm.index = index;
                    if (sendFrom == 'before' && vm.beforeImagesSizes[index] == undefined) {
                        vm.beforeImagesSizes[index] = document.getElementsByClassName(className)[0].getBoundingClientRect().width;
                        vm.isFromBeforeImage = true;
                    }

                    if (sendFrom == 'before' && vm.beforeImagesHeight[index] == undefined) {
                        vm.beforeImagesHeight[index] = document.getElementsByClassName(className)[0].getBoundingClientRect().height;
                        vm.isFromBeforeImage = true;
                    }

                    if (sendFrom == 'after' && vm.afterImagesSizes[index] == undefined) {
                        vm.isFromBeforeImage = false;
                        vm.afterImagesSizes[index] = document.getElementsByClassName(className)[0].getBoundingClientRect().width;
                    }
                    if (sendFrom == 'after' && vm.afterImagesHeight[index] == undefined) {
                        vm.isFromBeforeImage = false;
                        vm.afterImagesHeight[index] = document.getElementsByClassName(className)[0].getBoundingClientRect().height;
                    }

                    vm.zoomClass = className;
                    vm.zoomContailerId = contailerId;
                    var GFG = document.getElementsByClassName(className)[0];
                    var container = document.getElementById(contailerId);
                    zoomer.zoom(0.25, index);
                    GFG.addEventListener('mousedown', zoomer.start_drag);
                    container.addEventListener('mousemove', zoomer.while_drag);
                    container.addEventListener('mouseup', zoomer.stop_drag);
                    container.addEventListener('mouseout', zoomer.stop_drag);
                }

                function zoomout(className, contailerId, index, sendFrom) {
                    if (zoomer.zoom_factor == 1) {
                        return;
                    }
                    vm.index = index;
                    vm.zoomClass = className;
                    vm.zoomContailerId = contailerId;
                    var GFG = document.getElementsByClassName(className)[0];
                    var container = document.getElementById(contailerId);
                    zoomer.zoom(-0.25, index);
                    GFG.addEventListener('mousedown', zoomer.start_drag);
                    container.addEventListener('mousemove', zoomer.while_drag);
                    container.addEventListener('mouseup', zoomer.stop_drag);
                    container.addEventListener('mouseout', zoomer.stop_drag);

                }

                var zoomer = (function () {
                    var abc = document.getElementsByClassName(vm.zoomClass);
                    var img_ele = null,
                        x_cursor = 0,
                        y_cursor = 0,
                        x_img_ele = 0,
                        y_img_ele = 0,
                        orig_width = abc.length > 0 ? document.getElementsByClassName(vm.zoomClass)[0].getBoundingClientRect().width : 0,
                        orig_height = abc.length > 0 ? document.getElementsByClassName(vm.zoomClass)[0].getBoundingClientRect().height : 0,
                        current_top = 0,
                        current_left = 0,
                        zoom_factor = 1.0;

                    return {
                        zoom: function (zoomincrement, index) {

                            img_ele = document.getElementsByClassName(vm.zoomClass)[0];
                            orig_width = img_ele != null ? document.getElementsByClassName(vm.zoomClass)[0].getBoundingClientRect().width : 0,
                                orig_height = img_ele != null ? document.getElementsByClassName(vm.zoomClass)[0].getBoundingClientRect().height : 0,
                                zoom_factor = zoom_factor + zoomincrement;
                            if (vm.isFromBeforeImage) {
                                orig_width = vm.beforeImagesSizes[index] != undefined ? vm.beforeImagesSizes[index] : 0;
                                orig_height = vm.beforeImagesHeight[index] != undefined ? vm.beforeImagesHeight[index] : 0;
                            }
                            else {
                                orig_width = vm.afterImagesSizes[index] != undefined ? vm.afterImagesSizes[index] : 0;
                                orig_height = vm.afterImagesHeight[index] != undefined ? vm.afterImagesHeight[index] : 0;
                            }
                            if (zoom_factor <= 1.0) {
                                zoom_factor = 1.0;
                                img_ele.style.top = '0px';
                                img_ele.style.marginLeft = '0px';
                            }
                            if (zoomincrement <= 0 && orig_width == 0) {
                                return;
                            }

                            var new_width = (orig_width * zoom_factor);
                            var new_heigth = (orig_height * zoom_factor);


                            if (current_left < (orig_width - new_width)) {
                                current_left = (orig_width - new_width);
                            }
                            if (current_top < (orig_height - new_heigth)) {
                                current_top = (orig_height - new_heigth);
                            }
                            img_ele.style.left = current_left + 'px';
                            img_ele.style.marginLeft = current_left + 'px';
                            img_ele.style.marginTop = current_top + 'px';
                            img_ele.style.width = new_width + 'px';
                            img_ele.style.height = new_heigth + 'px';

                            img_ele = null;
                        },

                        start_drag: function () {
                            if (zoom_factor <= 1.0) {
                                return;
                            }
                            img_ele = this;
                            x_img_ele = window.event.clientX - document.getElementsByClassName(vm.zoomClass)[0].offsetLeft;
                            y_img_ele = window.event.clientY - document.getElementsByClassName(vm.zoomClass)[0].offsetTop;
                        },

                        stop_drag: function () {
                            //img_ele = document.getElementsByClassName(vm.zoomClass)[0];
                            if (img_ele !== null) {
                                if (zoom_factor <= 1.0) {
                                    img_ele.style.marginLeft = '0px';
                                    img_ele.style.marginTop = '0px';
                                }
                            }
                            img_ele = null;
                        },

                        while_drag: function () {
                            if (img_ele !== null) {
                                if (vm.isFromBeforeImage) {
                                    orig_width = vm.beforeImagesSizes[vm.index];
                                    orig_height = vm.beforeImagesHeight[vm.index];
                                }
                                else {
                                    orig_width = vm.afterImagesSizes[vm.index];
                                    orig_height = vm.afterImagesHeight[vm.index];
                                }
                                var x_cursor = window.event.clientX;
                                var y_cursor = window.event.clientY;
                                var new_left = (x_cursor - x_img_ele);
                                if (new_left > 0) {
                                    new_left = 0;
                                }
                                if (new_left < (orig_width - img_ele.width)) {
                                    new_left = (orig_width - img_ele.width);
                                }
                                var new_top = (y_cursor - y_img_ele);
                                if (new_top > 0) {
                                    new_top = 0;
                                }
                                if (new_top < (orig_height - img_ele.height)) {
                                    new_top = (orig_height - img_ele.height);
                                }
                                current_left = new_left;
                                img_ele.style.marginLeft = new_left + 'px';
                                current_top = new_top;
                                img_ele.style.marginTop = new_top + 'px';

                                //console.log(img_ele.style.left+' - '+img_ele.style.top);
                            }
                        }
                    };
                }());


                function getImageSize() {
                    angular.forEach(vm.jobEstimateImage, function (items, index) {

                    });
                }

                function openEstimateWorthModal() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/modal-estimate-worth.client.view.html',
                        controller: 'EstimateWorthController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Franchisee: vm.jobInfo.franchisee,
                                    FranchiseeId: vm.jobInfo.franchiseeId,
                                    CustomerInformation: vm.jobInfo.jobCustomer,
                                    JobSchedulerId: vm.rowId
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                        getJobInfo();
                    })
                };

                document.getElementById("lb-back").addEventListener("click", function () {
                    this.classList.remove("show");
                });

                function swapImages(beforeImages, afterImages) {
                    var beforeTempImageInfo = beforeImages.imagesInfo;
                    var beforeTempImageFilesList = beforeImages.filesList;
                    beforeImages.imagesInfo = afterImages.imagesInfo;
                    afterImages.imagesInfo = beforeTempImageInfo;
                    beforeImages.filesList = afterImages.filesList;
                    afterImages.filesList = beforeTempImageFilesList;
                }

                function shiftModal(serviceDomain) {

                    vm.serviceDomain = serviceDomain;
                    vm.shiftImagesModel.estimateId = vm.jobId;
                    vm.shiftImagesModel.schedulerId = vm.rowId;
                    vm.shiftImagesModel.marketingClass = vm.jobInfo.marketingClass;
                    vm.shiftImagesModel.marketingClassId = vm.jobInfo.marketingClassId;
                    vm.shiftImagesModel.fileIds = [];
                    vm.shiftImagesModel.shiftId = 121;

                    if (serviceDomain.imagesInfo.length > 0) {
                        angular.forEach(serviceDomain.imagesInfo, function (imagesInfo) {
                            vm.shiftImagesModel.fileIds.push(imagesInfo.fileId);
                        });
                    }

                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/modal.shift.image.client.view.html',
                        controller: 'ShiftImageController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    ShiftImagesModel: vm.shiftImagesModel,
                                    ImagesInfo: serviceDomain.imagesInfo
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                    });
                }

                $rootScope.$on('isShiftSave', function (event, data) {
                    vm.isSaved = true;
                    vm.jobEstimateImageActive = [];
                    angular.copy(vm.jobEstimateImage, vm.jobEstimateImageActive);
                    vm.imagesInfoModel = vm.serviceDomain.imagesInfo;
                    //vm.jobEstimateImageActive = vm.jobEstimateImage;
                    $scope.shiftingImageActive = true;
                    //vm.serviceDomain.imagesInfo = [];
                    vm.save(true);
                    vm.isFromShift = true;

                });

                function shiftingImage() {
                    if (vm.shiftImagesModel) {
                        vm.shiftModel = vm.shiftImagesModel;
                    }
                    return schedulerService.shiftImagesToInvoiceBuildMaterial(vm.shiftModel).then(function (result) {
                        if (result.data) {
                            if (vm.shiftImagesModel.shiftId == 121) {
                                toaster.show("Images(s) shift to Building Exterior image Successfully!!!");
                            }
                            else if (vm.shiftImagesModel.shiftId == 122) {
                                toaster.show("Images(s) shift to Invoice/Drawing Successfully!!!");
                            }
                            //vm.imagesInfo.imagesInfo = [];
                        }
                        else {
                            if (vm.shiftImagesModel.shiftId == 121) {
                                toaster.error("Error in shifting Images(s) to Building Exterior image !!!");
                            }
                            else if (vm.shiftImagesModel.shiftId == 122) {
                                toaster.show("Error in shifting Images(s) to Invoice/Drawing !!!");
                            }
                        }
                    });
                }

                function downloadInvoice() {
                    openSelectInvoicesModal("Download Internal Invoice(s)", "Download Internal Invoice(s)", false, false);
                    //var index = vm.jobInfo.jobSchedulerList.indexOf($filter('filter')(vm.jobInfo.jobSchedulerList, { id: vm.jobInfo.id }, true)[0]);
                    //var jobScheduler = vm.jobInfo.jobSchedulerList[index];
                    //var model = {};
                    //model.schedulerId = vm.jobInfo.estimateSchedulerId;
                    //model.serviceInvoice = jobScheduler.invoiceNumbers;
                    //if (jobScheduler.invoiceNumbers == undefined || jobScheduler.invoiceNumbers == null || jobScheduler.invoiceNumbers.length == 0) {
                    //    toaster.error("No Invoices Attached in the Job. Please Attach the Invoice to Download.");
                    //    return;
                    //}
                    //return schedulerService.uploadInvoicesZipFile(model).then(function (result) {
                    //    vm.fileName = result.data;
                    //    if (vm.fileName == "") {
                    //        toaster.error("Please Save the Invoice to Download.");
                    //        return;
                    //    }
                    //    return fileService.downloadCustomerInvoice(result.data).then(function (result) {
                    //        var blob = new Blob([result.data], { type: "application/zip" });
                    //        var fileName = vm.fileName + ".zip";
                    //        var a = document.createElement("a");
                    //        document.body.appendChild(a);
                    //        a.style = "display:none";
                    //        var url = window.URL.createObjectURL(blob);
                    //        a.href = url;
                    //        a.download = fileName;
                    //        a.click();
                    //        window.URL.revokeObjectURL(url);
                    //        a.remove();
                    //        toaster.show("Invoices downloaded successfully. Please check the Download folder to view all the images.");
                    //    });
                    //});
                    //});
                }


                function getEstimateInvoiceInfo() {
                    if (vm.estimateInvoiceInfo == null) {
                        vm.isInvoicePresent = false;
                        return;
                    }
                    if (vm.estimateInvoiceInfo != null) {
                        vm.isInvoicePresent = true;
                        //vm.estimateInvoiceInfo = result.data;

                        if (vm.estimateInvoiceInfo.invoiceNotesList.length == 0) {
                            vm.estimateInvoiceInfo.invoiceNotesList.push({ id: 0, notes: '', invoiceNumber: 1 });
                        }
                        if (vm.estimateInvoiceInfo.serviceList.length > 0) {
                            vm.isDisableButton = false;
                        }
                        else {
                            vm.isDisableButton = true;
                        }
                        vm.estimateInvoiceInfo.address = vm.jobInfo.jobCustomer.address.addressLine1;
                        vm.estimateInvoiceInfo.city = vm.jobInfo.jobCustomer.address.city + ', ' + vm.jobInfo.jobCustomer.address.state
                            + ',' + vm.jobInfo.jobCustomer.address.zipCode;
                        vm.estimateInvoiceInfo.lessDepositPercentage = vm.jobInfo.lessDeposit;
                        vm.estimateInvoiceInfo.phoneNumber1 = vm.jobInfo.jobCustomer.phoneNumber;
                        vm.estimateInvoiceInfo.customerName = vm.jobInfo.jobCustomer.customerName;
                        vm.estimateInvoiceInfo.email = vm.jobInfo.jobCustomer.email;
                        vm.estimateInvoiceInfo.lessDepositPercentage = vm.jobInfo.lessDeposit;
                        calculatingDataByInvoiceNumber();
                        checkBoxClicked(vm.estimateInvoiceInfo.option);
                        totalSumForOptions();
                        optionWiseTotal();

                        angular.forEach(vm.estimateInvoiceInfo.serviceList, function (value, index) {
                            fillingServiceModel(value, index);
                            angular.forEach(value.subItem, function (sub) {
                                if (!sub.isMainBundle) {
                                    sub.salesPersonNote = getBundleSalesPersonNotes(value.serviceType, value.typeOfStoneType2, sub.serviceIds.id)[0];
                                    sub.informationNote = getBundleSalesPersonNotes(value.serviceType, value.typeOfStoneType2, sub.serviceIds.id)[1];
                                    sub.class = "";
                                }
                            });
                            getSalesPersonNotes(value, index);
                            calculateSumTotalArea(value, index);
                            value.class = "";

                        });
                    }
                    else {
                        vm.estimateInvoiceInfo.invoiceNotesList = [];
                        vm.estimateInvoiceInfo.invoiceNotesList.push({ id: 0, notes: '', invoiceNumber: 1 });
                        vm.isDisableButton = true;
                        totalSumForOptions();
                        checkBoxClicked('option1');
                        optionWiseTotal();
                    }
                    vm.estimateInvoiceInfo.lessDeposit = (vm.estimateInvoiceInfo.lessDepositPercentage * vm.estimateInvoiceInfo.price) / 100;
                    vm.estimateInvoiceInfo.balance = vm.estimateInvoiceInfo.price - vm.estimateInvoiceInfo.lessDeposit;
                    checkBoxClicked(vm.estimateInvoiceInfo.option)
                }
                function getBundleSalesPersonNotes(serviceType, typeOfStoneType2, serviceId) {
                    var informationNote = "";
                    var salesPersonNote = "";
                    if (serviceType == "STONELIFE") {
                        if (typeOfStoneType2 == "Marble") {
                            if (serviceId == "Stripping Carpet Adhesive" || serviceId == "Stripping-Silicone" || serviceId == "Stripping-Urethane" || serviceId == "Stripping-Wax" || serviceId == "Stripping-Unknown") {
                                salesPersonNote = "Recommend this be on a separate invoice from rest of project in order to allow client to source independently, as many do not understand that unlike a janitorial wax stripping where they need to get most of it off, we need to get every corner and grout line.";
                            }
                            else if (serviceId == "Chip Repair") {
                                salesPersonNote = "Chip and Crack repairs can be provided that are fast, but will cup, or overfilled, honed and flat but take more time. Be sure to define the approach you are providing.";
                                informationNote = "INSERT CHIP REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE (MARBLE)";
                            }
                            else if (serviceId == "Crack Repair") {
                                salesPersonNote = "Chip and Crack repairs can be provided that are fast, but will cup, or overfilled, honed and flat but take more time. Be sure to define the approach you are providing.";
                                informationNote = "INSERT CRACK REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE (MARBLE)";
                            }
                            else if (serviceId == "Grout Replacement") {
                                informationNote = "INSERT BEFORE & AFTER IMAGE";
                            }
                            else if (serviceId == "Hard Water Removal") {
                                informationNote = "INSERT BEFORE & AFTER IMAGE (HARD WATER)";
                            }
                            else if (serviceId == "Mold Stain Removal") {
                                informationNote = "INSERT BEFORE & AFTER IMAGE (MOLD)";
                            }
                            else if (serviceId == "Traverfil") {
                                informationNote = "INSERT BEFORE & AFTER IMAGE (TRAVERFIL)";
                            }
                            else if (serviceId == "Stain Removal") {
                                salesPersonNote = "RECOMMEND this be on a separate invoice to be done BEFORE the rest of the project or AFTER the rest of the project in order to avoid having other work held hostage should the stain prove unremovable.";
                            }
                            else {
                                informationNote = "";
                                salesPersonNote = "";
                            }
                        }
                        else if (typeOfStoneType2 == "Granite") {
                            if (serviceId == "Stripping Carpet Adhesive" || serviceId == "Stripping-Silicone" || serviceId == "Stripping-Urethane" || serviceId == "Stripping-Wax" || serviceId == "Stripping-Unknown") {
                                salesPersonNote = "Recommend this be on a separate invoice from rest of project in order to allow client to source independently, as many do not understand that unlike a janitorial wax stripping where they need to get most of it off, we need to get every corner and grout line.";
                            }
                            else if (serviceId == "Chip Repair") {
                                salesPersonNote = "Chip and Crack repairs can be provided that are fast, but will cup, or overfilled, honed and flat but take more time. Be sure to define the approach you are providing.";
                                informationNote = "INSERT CHIP REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE (GRANITE)";
                            }
                            else if (serviceId == "Crack Repair") {
                                salesPersonNote = "Chip and Crack repairs can be provided that are fast, but will cup, or overfilled, honed and flat but take more time. Be sure to define the approach you are providing.";
                                informationNote = "INSERT CRACK REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE (GRANITE)";
                            }
                            else if (serviceId == "Stain Removal") {
                                salesPersonNote = "RECOMMEND this be on a separate invoice to be done BEFORE the rest of the project or AFTER the rest of the project in order to avoid having other work held hostage should the stain prove unremovable.";
                            }
                            else if (serviceId == "Traverfil") {
                                informationNote = "INSERT BEFORE & AFTER IMAGE (TRAVERFIL)";
                            }
                            else if (serviceId == "Stain Removal") {
                                salesPersonNote = "RECOMMEND this be on a separate invoice to be done BEFORE the rest of the project or AFTER the rest of the project in order to avoid having other work held hostage should the stain prove unremovable.";
                            }
                            else if (serviceId == "Hard Water Removal") {
                                informationNote = "INSERT BEFORE & AFTER IMAGE (HARD WATER)";
                            }
                            else if (serviceId == "Honing") {
                                salesPersonNote = "Rarely required on granite. If the granite looks dull it is more likely they are suffering a build-up of wax from use of an inappropriate cleaner such as a glass or all purpose cleaner which includes micro-waxes. Clean first with acetone to remove residual wax before consider applying diamonds. Work backwards starting with an 3000, 1500, 800 in a test area. Never dip below 800. Foot traffic will NOT create damage below 800. Treat any scratches on spot basis.";
                            }
                            else if (serviceId == "Sealing") {
                                salesPersonNote = "OFFER to schedule their ANNUAL Clean-and-Seal visit.";
                            }
                            else {
                                informationNote = "";
                                salesPersonNote = "";
                            }
                        }
                    }
                    else if (serviceType == "COUNTERLIFE") {
                        if (typeOfStoneType2 == "Marble") {
                            if (serviceId == "Chip Repair") {
                                salesPersonNote = "Chip and Crack repairs can be provided that are fast, but will cup, or overfilled, honed and flat but take more time. Be sure to define the approach you are providing.";
                                informationNote = "INSERT CHIP REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE";
                            }
                            else if (serviceId == "Crack Repair") {
                                salesPersonNote = "Chip and Crack repairs can be provided that are fast, but will cup, or overfilled, honed and flat but take more time. Be sure to define the approach you are providing.";
                                informationNote = "INSERT CRACK REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE";
                            }
                            else if (serviceId == "Hard Water Removal") {
                                informationNote = "INSERT BEFORE & AFTER IMAGE (HARD WATER)";
                            }
                            else if (serviceId == "Mold Stain Removal") {
                                informationNote = "INSERT BEFORE & AFTER IMAGE (MOLD)";
                            }
                            else if (serviceId == "Sealing") {
                                salesPersonNote = "OFFER to schedule their ANNUAL Clean-and-Seal visit";
                            }
                            else if (serviceId == "Stain Removal") {
                                salesPersonNote = "RECOMMEND this be on a separate invoice to be done BEFORE the rest of the project or AFTER the rest of the project in order to avoid having other work held hostage should the stain prove unremovable.";
                            }
                            else {
                                informationNote = "";
                                salesPersonNote = "";
                            }
                        }
                        if (typeOfStoneType2 == "Granite") {
                            if (serviceId == "Chip Repair") {
                                informationNote = "INSERT CHIP REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE";
                            }
                            else if (serviceId == "Crack Repair") {
                                informationNote = "INSERT CRACK REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE";
                            }
                            else if (serviceId == "Hard Water Removal") {
                                informationNote = "INSERT BEFORE & AFTER IMAGE (HARD WATER)";
                            }
                            else if (serviceId == "Stain Removal") {
                                salesPersonNote = "RECOMMEND this be on a separate invoice to be done BEFORE the rest of the project or AFTER the rest of the project in order to avoid having other work held hostage should the stain prove unremovable.";
                            }
                            else if (serviceId == "Seam Repair") {
                                informationNote = "INSERT BEFORE & AFTER SEAM REPAIR";
                            }
                            else if (serviceId == "Honing") {
                                salesPersonNote = "Rarely required on granite. If the granite looks dull it is more likely they are suffering a build-up of wax from use of an inappropriate cleaner such as a glass or all purpose cleaner which includes micro-waxes. Clean first with acetone to remove residual wax before consider applying diamonds. Work backwards starting with an 11000, 8000, 3000 in a test area. Never dip below 800.";
                            }
                            else if (serviceId == "Sealing") {
                                salesPersonNote = "OFFER to schedule their ANNUAL Clean-and-Seal visit.";
                            }
                            else {
                                informationNote = "";
                                salesPersonNote = "";
                            }
                        }
                        if (typeOfStoneType2 == "Corian") {
                            if (serviceId == "Corian-Chip Repair") {
                                informationNote = "INSERT CHIP REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE (CORIAN)";
                            }
                            else if (serviceId == "Corian-Crack Repair") {
                                salesPersonNote = "Explain that this is major surgery as a wedge needs to be cut from the counter and replaced. Customer should have additional material from initial install, else there will be a color difference due to age, oxidation and wear. This should run no less than 800 per area. If one fails one purchases the counter OR they understand they need to replace it.";
                                informationNote = "INSERT CRACK REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE (CORIAN)";
                            }
                            else {
                                informationNote = "";
                                salesPersonNote = "";
                            }
                        }
                        if (typeOfStoneType2 == "Engineered Stone") {
                            if (serviceId == "Engineered Stone - Polish") {
                                salesPersonNote = "MARBLELIFE developed the techniques to restore original aligator skin Zodiac surface texturing.";
                                informationNote = "INSERT BEFORE & AFTER EXAMPLE IMAGE (ENGINEERED STONE)";
                            }
                            else {
                                informationNote = "";
                                salesPersonNote = "";
                            }
                        }
                    }
                    else if (serviceType == "CLEANSHIELD") {
                        if (serviceId == "Cleanshield") {
                            informationNote = "INSERT BEFORE & AFTER IMAGE (CLEANSHIELD)";
                        }
                    }
                    else if (serviceType == "MAINTAINANCE:OTHER" || serviceType == "MAINTENANCE:BI-MONTHLY" || serviceType == "MAINTENANCE:MONTHLY" || serviceType == "MAINTENANCE:QUARTERLY") {
                        salesPersonNote = "Typically need 1 man-day per 1500-2000 sqft of floor.  Quarterly service only makes sense for properties less than 700 sqft.  Otherwise quarterly is 5x monthly - due to scratch pattern development.";
                        informationNote = "INSERT AFTER IMAGE OF PROPERTY MAINTAINED BY MARBLELIFE FOR OVER 15 YEARS";
                    }
                    else if (serviceType == "GROUTLIFE/TILELOCK") {
                        if (typeOfStoneType2 == "CERAMIC" || typeOfStoneType2 == "PORCELAIN" || typeOfStoneType2 == "CONCRETE" || typeOfStoneType2 == "TERRAZZO") {
                            if (serviceId == "Tilelok") {
                                informationNote = "INSERT BEFORE & AFTER IMAGES TILELOK";
                            }
                            else {
                                salesPersonNote = "";
                                informationNote = "";
                            }
                        }
                        else {
                            if (serviceId == "Stripping Carpet Adhesive" || serviceId == "Stripping-Silicone" || serviceId == "Stripping-Urethane" || serviceId == "Stripping-Wax" || serviceId == "Stripping-Unknown") {
                                salesPersonNote = "Recommend this be on a separate invoice from rest of project in order to allow client to source independently, as many do not understand that unlike a janitorial wax stripping where they need to get most of it off, we need to get every corner and grout line.";
                            }
                            else if (serviceId == "Grout Replacement") {
                                salesPersonNote = "Grout damage is generally caused by either (1) poorly mixed grout or (2) building settling, or (3) a flexing floor.  If the building is still settling grout could be expected to rebreak.  If the floor is flexing grout will rebreak unless they bring in someone to reinforce the floor.";
                            }
                            else if (serviceId == "Colorseal") {
                                informationNote = "INSERT COLORSEAL BEFORE & AFTER";
                                salesPersonNote = "Be Ready to discuss the root causes that lead to stained grout (failure of initial seal, and use of acidic cleaners that dissolve grout seals)";
                            }
                            else {
                                salesPersonNote = "";
                                informationNote = "";
                            }
                        }
                    }
                    else if (serviceType == "CONCRETE-REPAIR") {
                        if (typeOfStoneType2 == "CONCRETE" || typeOfStoneType2 == "TERRAZZO") {
                            if (serviceId == "Concrete Chip Repair") {
                                informationNote = "INSERT CHIP REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE";
                            }
                            else if (serviceId == "Concrete Crack Repair") {
                                salesPersonNote = "Remember that while a chip and crack repair will prevent water and dirt from collecting in the open area, the resulting repair is still an eyesore and will need an opaque coating to restore a clean uniform finish to the surface";
                                informationNote = "INSERT CRACK REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE";
                            }
                            else if (serviceId == "Large Concrete Area Repairs") {
                                salesPersonNote = "Concrete is used to establish a base to within 1/2\" of desired surface so as to reduce the amount of terrazzo needed AND to reduce chance of pebbles settling below the surface prior to grinding. Surface should be overpacked to prevent an overcream presence";
                            }
                            else if (serviceId == "Terrazzo Chip Repair" || serviceId == "Terrazzo Crack Repair" || serviceId == "Terrazzo Large Area Repair") {
                                salesPersonNote = "Be advised, that even a small terrazzo repair will run $700, as the terrazzo suppliers will not supply chips in less than 25 Lb quantities and resin in less than 5-Gal pails. Reach out to MARBLELIFE R&D for guidance on how to make terrazzo samples for your client to sign-off on the matrix, chip sizes, chip colors and chip variations.";
                            }
                            else if (serviceId == "Stripping Carpet Adhesive" || serviceId == "Stripping-Silicone" || serviceId == "Stripping-Urethane" || serviceId == "Stripping-Wax" || serviceId == "Stripping-Unknown") {
                                salesPersonNote = "Recommend this be on a separate invoice from rest of project in order to allow client to source independently, as many do not understand that unlike a janitorial wax stripping where they need to get most of it off, we need to get every corner and grout line.";
                            }
                            else {
                                salesPersonNote = "";
                                informationNote = "";
                            }
                        }
                    }
                    else if (serviceType == "CONCRETE-COUNTERTOPS") {
                        if (serviceId == "Marblized Top Coat Installation") {
                            salesPersonNote = "It is imperative the counter be protected from touching for 24 hours while it flows and settles.  Alert client that if the surface is touched by person or pet additional charges would apply as the surface must be removed and re-applied.";
                        }
                        else {
                            salesPersonNote = "";
                            informationNote = "";
                        }
                    }
                    else if (serviceType == "CONCRETE-COATINGS") {
                        if (serviceId == "Moisture Vapor Barrier") {
                            salesPersonNote = "This should be required in FL and TX and elsewhere where we have high water tables reported.  This is generally a greater need in vallies than on top of a hillside.  See the water table map insert to show your client as needed.";
                        }
                        else if (serviceId == "Epoxy Base") {
                            informationNote = "INSERT COATING COLOR CHART";
                        }
                        else if (serviceId == "Polyspartic Base") {
                            serviceList.informationNote = "INSERT COATING COLOR CHART";
                        }
                        else if (serviceId == "Vinyl Chips") {
                            informationNote = "INSERT CHIP COLOR CHART";
                        }
                        else if (serviceId == "Epoxy Top Coat") {
                            informationNote = "INSERT CHIP COLOR CHART";
                        }
                        else if (serviceId == "Polyspartic Top Coat") {
                            informationNote = "INSERT COATING COLOR CHART";
                        }
                        else if (serviceId == "Endurachip - Light") {
                            salesPersonNote = "This is a light chip one coat system designed to be a low cost coating - often used in a FOR SALE situation when owner is seeking to cover cracks, pits and stains and clean-up appearance.  Fast ";
                        }
                        else if (serviceId == "Endurachip - Fast") {
                            salesPersonNote = "This is a 1-2 day process designed to allow minimum loss of garage time.";
                        }
                        else if (serviceId == "Endurachip - Durable") {
                            salesPersonNote = "This is a 2-3 day process designed to provide the best durability coating";
                        }
                        else if (serviceId == "Marblized Coating") {
                            informationNote = "Link to web gallery";
                        }
                        else if (serviceId == "Anti-Spauling Driveway Sealing" || serviceId == "Anti-Spauling Sideway Sealing") {
                            informationNote = "Insert images";
                        }
                        else {
                            salesPersonNote = "";
                            informationNote = "";
                        }
                    }
                    else if (serviceType == "CLEANAIR") {
                        if (serviceId == "Clean Air Purification Treatments") {
                            salesPersonNote = "ISSUE - Presently this technology works well, but we do NOT have a test that allows us to confirm its presence.  This service will go live once a test has been found.";
                        }
                        else {
                            salesPersonNote = "";
                            informationNote = "";
                        }
                    }
                    else if (serviceType == "TILEINSTALL") {
                        if (serviceId == "Grouting") {
                            salesPersonNote = "Recommend the client select a standard gray grout and then to seal with a colorizeablle Colorseal as this allows them to confirm everything is indeed seal, and expands the cleaners they can use without damaging their grout.";
                        }
                        else if (serviceId == "Grout Sealing - Clear") {
                            salesPersonNote = "Ceramic and Porcelein do not need sealing.";
                        }
                        else {
                            salesPersonNote = "";
                            informationNote = "";
                        }
                    }
                    else if (serviceType == "VINYLGUARD") {
                        if (serviceId == "Cleaning and Pre-Coat Preparations") {
                            salesPersonNote = "This is where your time is as every corner and edge has to be removed of dirt, dust and hair. Not a bad idea to have team wear a hair net and booties during this process.";
                        }
                        else if (serviceId == "VinylGuard Installation") {
                            salesPersonNote = "Vinylguard does not need to be stripped, and is durable enough that it can go a year without needed attention, unlike wax which scuffs, must be buffed, and periodically tripped. Keep in mind this is a labor savings, which is only realized if they can reduce staff or have the staff do more of something else.";
                        }
                        else {
                            salesPersonNote = "";
                            informationNote = "";
                        }
                    }
                    else if (serviceType == "WOODLIFE") {
                        if (serviceId == "Cleaning") {
                            salesPersonNote = "Wood cleaners contain waxes that build up over time and dull the surface, cleaning in this case means removing these residual waxes.";
                        }
                        else {
                            salesPersonNote = "";
                            informationNote = "";
                        }
                    }
                    else if (serviceType == "SALESTAX") {
                        if (serviceId == "Service") {
                            salesPersonNote = "Most states do not require tax on services.";
                        }
                        else {
                            salesPersonNote = "";
                            informationNote = "";
                        }
                    }
                    else {
                        informationNote = "";
                        salesPersonNote = "";
                    }
                    var notes = [salesPersonNote, informationNote]
                    return notes;
                }

                function fillingServiceModel(serviceList, index) {

                    vm.serviceTypeForInvoice = [
                        { label: "No Service Available", id: "No Service Available" }
                    ];

                    vm.serviceTypeForInvoiceList[index] = [];
                    vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;

                    //For Carpet Material
                    if (serviceList.serviceType == "CARPETLIFE" && serviceList.typeOfStoneType2 == "Carpet") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Carpet Cleaning", id: "Carpet Cleaning" },
                            { label: "Carpet Guard", id: "Carpet Guard" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Ceramic (tile and grout) Material
                    else if (serviceList.serviceType == "COUNTERLIFE" && serviceList.typeOfStoneType2 == "Ceramic (tile and grout)") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Repair Chip", id: "Repair Chip" },
                            { label: "Repair Crack", id: "Repair Crack" },
                            { label: "Seal", id: "Seal" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "GROUTLIFE/TILELOK" && serviceList.typeOfStoneType2 == "Ceramic (tile and grout)") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean/Colorseal/ Interlok", id: "Bundle Clean/Colorseal/ Interlok" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Ceramic", id: "Ceramic" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Colorseal", id: "Colorseal" },
                            { label: "Colorseal-Hydroforce", id: "Colorseal-Hydroforce" },
                            { label: "Colorseal-Powerwash", id: "Colorseal-Powerwash" },
                            { label: "Colorseal-Powerscrub", id: "Colorseal-Powerscrub" },
                            { label: "Grout Repair", id: "Grout Repair" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hydroforce Cleaning", id: "Hydroforce Cleaning" },
                            { label: "Powerwash Cleaning", id: "Powerwash Cleaning" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Stripping Carpet Adhesive", id: "Stripping Carpet Adhesive" },
                            { label: "Stripping-Silicone", id: "Stripping-Silicone" },
                            { label: "Stripping-Unknown", id: "Stripping-Unknown" },
                            { label: "Stripping-Urethane", id: "Stripping-Urethane" },
                            { label: "Stripping-Wax", id: "Stripping-Wax" },
                            { label: "Tile Rebond", id: "Tile Rebond" },
                            { label: "Tile Replacement", id: "Tile Replacement" },
                            { label: "Waxing", id: "Waxing" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Ceramic (tile and grout)") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Floor Cleaner Concentrate Gallon", id: "Floor Cleaner Concentrate Gallon" },
                            { label: "Grout Sealer 16 oz", id: "Grout Sealer 16 oz" },
                            { label: "MaxOut Concentrate Gallon", id: "MaxOut Concentrate Gallon" },
                            { label: "MaxOut Grout Cleaner 32 oz", id: "MaxOut Grout Cleaner 32 oz" },
                            { label: "Mold & Mildew Stain Remover 32oz Spray", id: "Mold & Mildew Stain Remover 32oz Spray" },
                            { label: "Mold & Mildew Stain Remover Gallon", id: "Mold & Mildew Stain Remover Gallon" },
                            { label: "Soap Scum Remover 16oz", id: "Soap Scum Remover 16oz" },
                            { label: "Tile Cleaner 32oz Spray", id: "Tile Cleaner 32oz Spray" },
                            { label: "Tile Cleaner Concentrate 32 oz", id: "Tile Cleaner Concentrate 32 oz" },
                            { label: "Tile Cleaner Refill Gallon", id: "Tile Cleaner Refill Gallon" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "TILEINSTALL" && serviceList.typeOfStoneType2 == "Ceramic (tile and grout)") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Grout & Tile Sealing", id: "Grout & Tile Sealing" },
                            { label: "Grout Haze Removal", id: "Grout Haze Removal" },
                            { label: "Grout Sealing - Clear", id: "Grout Sealing - Clear" },
                            { label: "Grouting", id: "Grouting" },
                            { label: "Tile Installation", id: "Tile Installation" },
                            { label: "Tile Removal", id: "Tile Removal" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Concrete Material
                    else if (serviceList.serviceType == "CLEANSHIELD" && serviceList.typeOfStoneType2 == "Concrete") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Cleanshield", id: "Cleanshield" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "CONCRETE-COATINGS" && serviceList.typeOfStoneType2 == "Concrete") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            // Bundle items first
                            { label: "Bundle Clean & Reseal", id: "Bundle Clean & Reseal" },
                            { label: "Bundle Prep & Coat Epoxy Only", id: "Bundle Prep & Coat Epoxy Only" },
                            { label: "Bundle Prep & Coat Epoxy-Epoxy", id: "Bundle Prep & Coat Epoxy-Epoxy" },
                            { label: "Bundle Prep & Coat Epoxy-MVB Only", id: "Bundle Prep & Coat Epoxy-MVB Only" },
                            { label: "Bundle Prep & Coat Epoxy-Poly Spartic", id: "Bundle Prep & Coat Epoxy-Poly Spartic" },
                            { label: "Bundle Prep & Coat Poly Spartic Only", id: "Bundle Prep & Coat Poly Spartic Only" },
                            { label: "Bundle Prep & Epoxy Chip -(No MVB) Epoxy - ACS Chip - Epoxy", id: "Bundle Prep & Epoxy Chip -(No MVB) Epoxy - ACS Chip - Epoxy" },
                            { label: "Bundle Prep & Epoxy Chip -(No MVB) Epoxy - Quartz Chip - Epoxy", id: "Bundle Prep & Epoxy Chip -(No MVB) Epoxy - Quartz Chip - Epoxy" },
                            { label: "Bundle Prep & Epoxy Chip -(No MVB) Epoxy - Vinyl Chip - Epoxy", id: "Bundle Prep & Epoxy Chip -(No MVB) Epoxy - Vinyl Chip - Epoxy" },
                            { label: "Bundle Prep - (Epoxy-MVB) - PolySpartic Semiron - ACS Chip - Poly Spartic", id: "Bundle Prep - (Epoxy-MVB) - PolySpartic Semiron - ACS Chip - Poly Spartic" },
                            { label: "Bundle Prep - (Epoxy-MVB) - PolySpartic Semiron - Quartz Chip - Poly Spartic", id: "Bundle Prep - (Epoxy-MVB) - PolySpartic Semiron - Quartz Chip - Poly Spartic" },
                            { label: "Bundle Prep - (Epoxy-MVB) - PolySpartic Semiron - Vinyl Chip - Poly Spartic", id: "Bundle Prep - (Epoxy-MVB) - PolySpartic Semiron - Vinyl Chip - Poly Spartic" },
                            { label: "Bundle Prep - Epoxy Chip - Poly Sprtic (No MVB) EPOXY - Vinyl Chip - Poly Spartic", id: "Bundle Prep - Epoxy Chip - Poly Sprtic (No MVB) EPOXY - Vinyl Chip - Poly Spartic" },
                            { label: "Bundle Prep - Epoxy Chip - Poly Sprtic (No MVB) Epoxy - ACS Chip - Poly Spartic", id: "Bundle Prep - Epoxy Chip - Poly Sprtic (No MVB) Epoxy - ACS Chip - Poly Spartic" },
                            { label: "Bundle Prep - Epoxy Chip - Poly Sprtic (No MVB) Epoxy - Quartz Chip - Poly Spartic", id: "Bundle Prep - Epoxy Chip - Poly Sprtic (No MVB) Epoxy - Quartz Chip - Poly Spartic" },
                            { label: "Bundle Prep - Marblized Marblize", id: "Bundle Prep - Marblized Marblize" },
                            { label: "Bundle Prep - MVB - Epoxy Chip - Poly Spartic MVB - Epoxy - ACS Chip - Poly Spartic", id: "Bundle Prep - MVB - Epoxy Chip - Poly Spartic MVB - Epoxy - ACS Chip - Poly Spartic" },
                            { label: "Bundle Prep - MVB - Epoxy Chip - Poly Spartic MVB - Epoxy - Quartz Chip - Poly Spartic", id: "Bundle Prep - MVB - Epoxy Chip - Poly Spartic MVB - Epoxy - Quartz Chip - Poly Spartic" },
                            { label: "Bundle Prep - MVB - Epoxy Chip - Poly Spartic MVB - Epoxy - Vinyl Chip - Poly Spartic", id: "Bundle Prep - MVB - Epoxy Chip - Poly Spartic MVB - Epoxy - Vinyl Chip - Poly Spartic" },
                            { label: "Bundle Prep - MVB - Poly Spartic - Poly Spartic Semiron - ACS Chip - Poly Spartic", id: "Bundle Prep - MVB - Poly Spartic - Poly Spartic Semiron - ACS Chip - Poly Spartic" },
                            { label: "Bundle Prep - MVB - Poly Spartic - Poly Spartic Semiron - Quartz Chip - Poly Spartic", id: "Bundle Prep - MVB - Poly Spartic - Poly Spartic Semiron - Quartz Chip - Poly Spartic" },
                            { label: "Bundle Prep - MVB - Poly Spartic - Poly Spartic Semiron - Vinyl Chip - Poly Spartic", id: "Bundle Prep - MVB - Poly Spartic - Poly Spartic Semiron - Vinyl Chip - Poly Spartic" },
                            { label: "Bundle Prep - Poly Spactic - Chip - Poly Spartic  - No MVB Poly Spartic - ACS Chip - Poly Spartic", id: "Bundle Prep - Poly Spactic - Chip - Poly Spartic  - No MVB Poly Spartic - ACS Chip - Poly Spartic" },
                            { label: "Bundle Prep - Poly Spactic - Chip - Poly Spartic  - No MVB Poly Spartic - Quartz Chip - Poly Spartic", id: "Bundle Prep - Poly Spactic - Chip - Poly Spartic  - No MVB Poly Spartic - Quartz Chip - Poly Spartic" },
                            { label: "Bundle Prep - Poly Spactic - Chip - Poly Spartic  - No MVB Poly Spartic - Vinyl Chip - Poly Spartic", id: "Bundle Prep - Poly Spactic - Chip - Poly Spartic  - No MVB Poly Spartic - Vinyl Chip - Poly Spartic" },
                            { label: "Bundle Strip, Clean & Reseal", id: "Bundle Strip, Clean & Reseal" },

                            // Individual items in alphabetical order
                            { label: "ACS Chips", id: "ACS Chips" },
                            { label: "Anti-Spauling Driveway Sealing", id: "Anti-Spauling Driveway Sealing" },
                            { label: "Anti-Spauling Sideway Sealing", id: "Anti-Spauling Sideway Sealing" },
                            { label: "Base Coat (Epoxy)", id: "Base Coat (Epoxy)" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Concrete Floor Prep - Grind", id: "Concrete Floor Prep - Grind" },
                            { label: "Concrete Floor Prep - Repairs", id: "Concrete Floor Prep - Repairs" },
                            { label: "Endurachip - Durable", id: "Endurachip - Durable" },
                            { label: "Endurachip - Fast", id: "Endurachip - Fast" },
                            { label: "Endurachip - Light", id: "Endurachip - Light" },
                            { label: "Endurachip Durable (ACS Filler)", id: "Endurachip Durable (ACS Filler)" },
                            { label: "Endurachip Durable (Quartz Chip)", id: "Endurachip Durable (Quartz Chip)" },
                            { label: "Endurachip Durable (VinylChip)", id: "Endurachip Durable (VinylChip)" },
                            { label: "Endurachip Fast (ACS Filler)", id: "Endurachip Fast (ACS Filler)" },
                            { label: "Endurachip Fast (Quartz Chip)", id: "Endurachip Fast (Quartz Chip)" },
                            { label: "Endurachip Fast (VinylChip)", id: "Endurachip Fast (VinylChip)" },
                            { label: "Endurachip Slow (ACS Filler)", id: "Endurachip Slow (ACS Filler)" },
                            { label: "Endurachip Slow (Quartz Chip)", id: "Endurachip Slow (Quartz Chip)" },
                            { label: "Endurachip Slow (VinylChip)", id: "Endurachip Slow (VinylChip)" },
                            { label: "Epoxy Base", id: "Epoxy Base" },
                            { label: "Epoxy Top Coat", id: "Epoxy Top Coat" },
                            { label: "Hone Surface Prep", id: "Hone Surface Prep" },
                            { label: "Marblized Coating", id: "Marblized Coating" },
                            { label: "Moisture Vapor Barrier", id: "Moisture Vapor Barrier" },
                            { label: "MVB-Base Coat (SEMIRON)", id: "MVB-Base Coat (SEMIRON)" },
                            { label: "MVB-Base Coat (Semiron-Epoxy)", id: "MVB-Base Coat (Semiron-Epoxy)" },
                            { label: "Polyspartic Base", id: "Polyspartic Base" },
                            { label: "Polyspartic Top Coat", id: "Polyspartic Top Coat" },
                            { label: "Powerwash - Mold Stain Removal", id: "Powerwash - Mold Stain Removal" },
                            { label: "Quartz Chips", id: "Quartz Chips" },
                            { label: "Top Coat (Polyspartic)", id: "Top Coat (Polyspartic)" },
                            { label: "Vinyl Chips", id: "Vinyl Chips" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "CONCRETE-OVERLAYMENTS" && serviceList.typeOfStoneType2 == "Concrete") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Hone", id: "Hone" },
                            { label: "Install", id: "Install" },
                            { label: "Polish", id: "Polish" },
                            { label: "Repair Chip", id: "Repair Chip" },
                            { label: "Repair Crack", id: "Repair Crack" },
                            { label: "Seal", id: "Seal" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "CONCRETE-STAIN" && serviceList.typeOfStoneType2 == "Concrete") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle (Honing & Polishing)", id: "Bundle (Honing & Polishing)" },
                            { label: "Densification", id: "Densification" },
                            { label: "Stain Application", id: "Stain Application" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "CONCRETE-POLISHING(ENDURACRETE)" && serviceList.typeOfStoneType2 == "Concrete") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle (Honing & Polishing)", id: "Bundle (Honing & Polishing)" },
                            { label: "Concrete Microtopping Overlayment", id: "Concrete Microtopping Overlayment" },
                            { label: "Concrete Spot Stain Removal", id: "Concrete Spot Stain Removal" },
                            { label: "Concrete Stain Application", id: "Concrete Stain Application" },
                            { label: "Densification", id: "Densification" },
                            { label: "Guard", id: "Guard" },
                            { label: "Honing", id: "Honing" },
                            { label: "Polishing", id: "Polishing" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Other", id: "Other" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Concrete") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Enduraclean Concrete Cleaner CONCENTRATE 32oz", id: "Enduraclean Concrete Cleaner CONCENTRATE 32oz" },
                            { label: "Enduraclean Concrete Cleaner CONCENTRATE Gal", id: "Enduraclean Concrete Cleaner CONCENTRATE Gal" },
                            { label: "Enduraclean Concrete Cleaner for Driveways, Sidewalks and Patios 32oz", id: "Enduraclean Concrete Cleaner for Driveways, Sidewalks and Patios 32oz" },
                            { label: "Enduraclean Exterio Concree Stain Remover Cleaner 32oz", id: "Enduraclean Exterio Concree Stain Remover Cleaner 32oz" },
                            { label: "Enduraclean Garage & Indoor Concrete READY-TO-USE 32oz", id: "Enduraclean Garage & Indoor Concrete READY-TO-USE 32oz" },
                            { label: "Enduraclean Metallic & Marblized Coating Cleaner Gal", id: "Enduraclean Metallic & Marblized Coating Cleaner Gal" },
                            { label: "Enduraclean Polished Concrete Cleaner CONCENTRATE Gal", id: "Enduraclean Polished Concrete Cleaner CONCENTRATE Gal" },
                            { label: "Enduraclean Polished Concrete Cleaner READY-TO-USE 32oz", id: "Enduraclean Polished Concrete Cleaner READY-TO-USE 32oz" },
                            { label: "Enduraclean Terrazzo & Concrete Cleaner 32oz", id: "Enduraclean Terrazzo & Concrete Cleaner 32oz" },
                            { label: "Enduraseal Concrete Sealer 32oz", id: "Enduraseal Concrete Sealer 32oz" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "TILEINSTALL" && serviceList.typeOfStoneType2 == "Concrete") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Grout & Tile Sealing", id: "Grout & Tile Sealing" },
                            { label: "Grout Haze Removal", id: "Grout Haze Removal" },
                            { label: "Grout Sealing - Clear", id: "Grout Sealing - Clear" },
                            { label: "Grouting", id: "Grouting" },
                            { label: "Tile Installation", id: "Tile Installation" },
                            { label: "Tile Removal", id: "Tile Removal" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "VINYLGUARD" && serviceList.typeOfStoneType2 == "Concrete") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Cleaning and Pre-Coat Preparations", id: "Cleaning and Pre-Coat Preparations" },
                            { label: "Stripping Wax", id: "Stripping Wax" },
                            { label: "VinylGuard Installation", id: "VinylGuard Installation" },
                            { label: "Waxing", id: "Waxing" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Glass Material
                    else if (serviceList.serviceType == "CLEANAIR" && serviceList.typeOfStoneType2 == "Glass") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Clean Air Purification Treatments", id: "Clean Air Purification Treatments" },
                            { label: "Other", id: "Other" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "METALLIFE" && serviceList.typeOfStoneType2 == "Glass") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Metal Cleaning", id: "Metal Cleaning" },
                            { label: "Metal Laquer, Sealing & Protection", id: "Metal Laquer, Sealing & Protection" },
                            { label: "Metal Scratch Removal", id: "Metal Scratch Removal" },
                            { label: "Metal Stripping", id: "Metal Stripping" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Glass") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Soap Scum Remover 16oz", id: "Soap Scum Remover 16oz" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "STONELIFE" && serviceList.typeOfStoneType2 == "Glass") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Hone", id: "Hone" },
                            { label: "Polish", id: "Polish" },
                            { label: "Repair Chip", id: "Repair Chip" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Slate Material
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Slate") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Floor Cleaner Concentrate Gallon", id: "Floor Cleaner Concentrate Gallon" },
                            { label: "Grout Sealer 16 oz", id: "Grout Sealer 16 oz" },
                            { label: "MaxOut Concentrate Gallon", id: "MaxOut Concentrate Gallon" },
                            { label: "MaxOut Grout Cleaner 32 oz", id: "MaxOut Grout Cleaner 32 oz" },
                            { label: "Mold & Mildew Stain Remover 32oz Spray", id: "Mold & Mildew Stain Remover 32oz Spray" },
                            { label: "Mold & Mildew Stain Remover Gallon", id: "Mold & Mildew Stain Remover Gallon" },
                            { label: "Soap Scum Remover 16oz", id: "Soap Scum Remover 16oz" },
                            { label: "Stone Sealer 16 oz", id: "Stone Sealer 16 oz" },
                            { label: "Tile Cleaner 32oz Spray", id: "Tile Cleaner 32oz Spray" },
                            { label: "Tile Cleaner Concentrate 32 oz", id: "Tile Cleaner Concentrate 32 oz" },
                            { label: "Tile Cleaner Refill Gallon", id: "Tile Cleaner Refill Gallon" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "TILEINSTALL" && serviceList.typeOfStoneType2 == "Slate") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Grout & Tile Sealing", id: "Grout & Tile Sealing" },
                            { label: "Grout Haze Removal", id: "Grout Haze Removal" },
                            { label: "Grout Sealing - Clear", id: "Grout Sealing - Clear" },
                            { label: "Grouting", id: "Grouting" },
                            { label: "Tile Installation", id: "Tile Installation" },
                            { label: "Tile Removal", id: "Tile Removal" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Vinyl Material
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Vinyl") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Floor Cleaner Concentrate Gallon", id: "Floor Cleaner Concentrate Gallon" },
                            { label: "MaxOut Concentrate Gallon", id: "MaxOut Concentrate Gallon" },
                            { label: "MaxOut Grout Cleaner 32 oz", id: "MaxOut Grout Cleaner 32 oz" },
                            { label: "Mold & Mildew Stain Remover 32oz Spray", id: "Mold & Mildew Stain Remover 32oz Spray" },
                            { label: "Mold & Mildew Stain Remover Gallon", id: "Mold & Mildew Stain Remover Gallon" },
                            { label: "Soap Scum Remover 16oz", id: "Soap Scum Remover 16oz" },
                            { label: "Tile Cleaner 32oz Spray", id: "Tile Cleaner 32oz Spray" },
                            { label: "Tile Cleaner Concentrate 32 oz", id: "Tile Cleaner Concentrate 32 oz" },
                            { label: "Tile Cleaner Refill Gallon", id: "Tile Cleaner Refill Gallon" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "TILEINSTALL" && serviceList.typeOfStoneType2 == "Vinyl") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Tile Installation", id: "Tile Installation" },
                            { label: "Tile Removal", id: "Tile Removal" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "VINYLGUARD" && serviceList.typeOfStoneType2 == "Vinyl") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Cleaning and Pre-Coat Preparations", id: "Cleaning and Pre-Coat Preparations" },
                            { label: "HONE", id: "HONE" },
                            { label: "REPAIR CHIP", id: "REPAIR CHIP" },
                            { label: "REPAIR CRACK", id: "REPAIR CRACK" },
                            { label: "Stripping Wax", id: "Stripping Wax" },
                            { label: "VinylGuard Installation", id: "VinylGuard Installation" },
                            { label: "Waxing", id: "Waxing" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Terrazzo Material
                    else if (serviceList.serviceType == "CONCRETE-POLISHING(ENDURACRETE)" && serviceList.typeOfStoneType2 == "Terrazzo") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle (Honing & Polishing)", id: "Bundle (Honing & Polishing)" },
                            { label: "Concrete Microtopping Overlayment", id: "Concrete Microtopping Overlayment" },
                            { label: "Concrete Spot Stain Removal", id: "Concrete Spot Stain Removal" },
                            { label: "Concrete Stain Application", id: "Concrete Stain Application" },
                            { label: "Densification", id: "Densification" },
                            { label: "Guard", id: "Guard" },
                            { label: "Honing", id: "Honing" },
                            { label: "Polishing", id: "Polishing" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Other", id: "Other" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Terrazzo") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Enduraclean Terrazzo & Concrete Cleaner 32oz", id: "Enduraclean Terrazzo & Concrete Cleaner 32oz" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "TILEINSTALL" && serviceList.typeOfStoneType2 == "Terrazzo") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Grout & Tile Sealing", id: "Grout & Tile Sealing" },
                            { label: "Grout Haze Removal", id: "Grout Haze Removal" },
                            { label: "Grout Sealing - Clear", id: "Grout Sealing - Clear" },
                            { label: "Grouting", id: "Grouting" },
                            { label: "Tile Installation", id: "Tile Installation" },
                            { label: "Tile Removal", id: "Tile Removal" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Wood Material
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Wood") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Wood & Laminate Cleaner 32oz", id: "Wood & Laminate Cleaner 32oz" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "WOODLIFE" && serviceList.typeOfStoneType2 == "Wood") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Buffing", id: "Buffing" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Re-Coat", id: "Re-Coat" },
                            { label: "Scuffing", id: "Scuffing" },
                            { label: "Scratch & Wear Removal (Sanding)", id: "Scratch & Wear Removal (Sanding)" },
                            { label: "Staining", id: "Staining" },
                            { label: "Top Coat", id: "Top Coat" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Travertine Material
                    else if (serviceList.serviceType == "COUNTERLIFE" && serviceList.typeOfStoneType2 == "Travertine") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Caulking", id: "Caulking" },
                            { label: "Chip Repair", id: "Chip Repair" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Coating", id: "Coating" },
                            { label: "Crack Repair", id: "Crack Repair" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hard Water Removal", id: "Hard Water Removal" },
                            { label: "Honing", id: "Honing" },
                            { label: "Mold Stain Removal", id: "Mold Stain Removal" },
                            { label: "Polishing", id: "Polishing" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Tile Replacement", id: "Tile Replacement" },
                            { label: "Traverfil", id: "Traverfil" },
                            { label: "Other", id: "Other" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Travertine") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Floor Cleaner Concentrate Gallon", id: "Floor Cleaner Concentrate Gallon" },
                            { label: "Grout Sealer 16 oz", id: "Grout Sealer 16 oz" },
                            { label: "Marble Cleaner 32 oz Spray", id: "Marble Cleaner 32 oz Spray" },
                            { label: "Marble Cleaner Conc. 32oz", id: "Marble Cleaner Conc. 32oz" },
                            { label: "Marble Cleaner Refill Gallon", id: "Marble Cleaner Refill Gallon" },
                            { label: "Marble Gloss Conditioner 16oz", id: "Marble Gloss Conditioner 16oz" },
                            { label: "Marble Polish 16 oz", id: "Marble Polish 16 oz" },
                            { label: "MaxOut Concentrate Gallon", id: "MaxOut Concentrate Gallon" },
                            { label: "MaxOut Grout Cleaner 32 oz", id: "MaxOut Grout Cleaner 32 oz" },
                            { label: "Mold & Mildew Stain Remover 32oz Spray", id: "Mold & Mildew Stain Remover 32oz Spray" },
                            { label: "Mold & Mildew Stain Remover Gallon", id: "Mold & Mildew Stain Remover Gallon" },
                            { label: "Soap Scum Remover 16oz", id: "Soap Scum Remover 16oz" },
                            { label: "Stone Sealer 16 oz", id: "Stone Sealer 16 oz" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "STONELIFE" && serviceList.typeOfStoneType2 == "Travertine") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean & Protect", id: "Bundle Clean & Protect" },
                            { label: "Bundle Clean & Seal", id: "Bundle Clean & Seal" },
                            { label: "Bundle Gloss-1 (Polish-Harden-Seal)", id: "Bundle Gloss-1 (Polish-Harden-Seal)" },
                            { label: "Bundle Gloss-2 (Hone-Polish-Harden-Seal)", id: "Bundle Gloss-2 (Hone-Polish-Harden-Seal)" },
                            { label: "Bundle Matte (Hone-Seal)", id: "Bundle Matte (Hone-Seal)" },
                            { label: "Bundle Satin-1 (Polish-Harden-Seal)", id: "Bundle Satin-1 (Polish-Harden-Seal)" },
                            { label: "Bundle Satin-2 (Hone-Polish-Harden-Seal)", id: "Bundle Satin-2 (Hone-Polish-Harden-Seal)" },

                            { label: "Caulking", id: "Caulking" },
                            { label: "Chip Repair", id: "Chip Repair" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Coating", id: "Coating" },
                            { label: "Crack Repair", id: "Crack Repair" },
                            { label: "GRIND", id: "GRIND" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hard Water Removal", id: "Hard Water Removal" },
                            { label: "Honing", id: "Honing" },
                            { label: "Hydroforce Cleaning", id: "Hydroforce Cleaning" },
                            { label: "Mold Stain Removal", id: "Mold Stain Removal" },
                            { label: "Polishing", id: "Polishing" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Stripping Carpet Adhesive", id: "Stripping Carpet Adhesive" },
                            { label: "Stripping-Silicone", id: "Stripping-Silicone" },
                            { label: "Stripping-Unknown", id: "Stripping-Unknown" },
                            { label: "Stripping-Urethane", id: "Stripping-Urethane" },
                            { label: "Stripping-Wax", id: "Stripping-Wax" },
                            { label: "Tile Replacement", id: "Tile Replacement" },
                            { label: "Traverfil", id: "Traverfil" },
                            { label: "Other", id: "Other" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "TILEINSTALL" && serviceList.typeOfStoneType2 == "Travertine") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Grout & Tile Sealing", id: "Grout & Tile Sealing" },
                            { label: "Grout Haze Removal", id: "Grout Haze Removal" },
                            { label: "Grout Sealing - Clear", id: "Grout Sealing - Clear" },
                            { label: "Grouting", id: "Grouting" },
                            { label: "Tile Installation", id: "Tile Installation" },
                            { label: "Tile Removal", id: "Tile Removal" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Granite Material
                    else if (serviceList.serviceType == "COUNTERLIFE" && serviceList.typeOfStoneType2 == "Granite") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean & Protect", id: "Bundle Clean & Protect" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Chip Repair", id: "Chip Repair" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Cleaning - Address Mold Stains", id: "Cleaning - Address Mold Stains" },
                            { label: "Coating", id: "Coating" },
                            { label: "Crack Repair", id: "Crack Repair" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hard Water Removal", id: "Hard Water Removal" },
                            { label: "Honing", id: "Honing" },
                            { label: "Polishing", id: "Polishing" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Seam Repair", id: "Seam Repair" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Tile Replacement", id: "Tile Replacement" },
                            { label: "Traverfil", id: "Traverfil" },
                            { label: "Other", id: "Other" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Granite") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Floor Cleaner Concentrate Gallon", id: "Floor Cleaner Concentrate Gallon" },
                            { label: "Granite Cleaner 32 oz Spray", id: "Granite Cleaner 32 oz Spray" },
                            { label: "Granite Cleaner Refill Gallon", id: "Granite Cleaner Refill Gallon" },
                            { label: "Granite Gloss conditioner 8oz", id: "Granite Gloss conditioner 8oz" },
                            { label: "Granite Sealer 4 oz Spray", id: "Granite Sealer 4 oz Spray" },
                            { label: "Grout Sealer 16 oz", id: "Grout Sealer 16 oz" },
                            { label: "Marble Gloss conditioner 16oz", id: "Marble Gloss conditioner 16oz" },
                            { label: "MaxOut Concentrate Gallon", id: "MaxOut Concentrate Gallon" },
                            { label: "MaxOut Grout Cleaner 32 oz", id: "MaxOut Grout Cleaner 32 oz" },
                            { label: "Mold & Mildew Stain Remover 32oz Spray", id: "Mold & Mildew Stain Remover 32oz Spray" },
                            { label: "Mold & Mildew Stain Remover Gallon", id: "Mold & Mildew Stain Remover Gallon" },
                            { label: "Soap Scum Remover 16oz", id: "Soap Scum Remover 16oz" },
                            { label: "Stone Sealer 16 oz", id: "Stone Sealer 16 oz" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "STONELIFE" && serviceList.typeOfStoneType2 == "Granite") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean & Protect", id: "Bundle Clean & Protect" },
                            { label: "Bundle Clean & Seal", id: "Bundle Clean & Seal" },
                            { label: "Bundle Gloss-1 (Polish-Harden-Seal)", id: "Bundle Gloss-1 (Polish-Harden-Seal)" },
                            { label: "Bundle Gloss-2 (Hone-Polish-Harden-Seal)", id: "Bundle Gloss-2 (Hone-Polish-Harden-Seal)" },
                            { label: "Bundle Matte (Hone-Seal)", id: "Bundle Matte (Hone-Seal)" },
                            { label: "Bundle Satin-1 (Polish-Harden-Seal)", id: "Bundle Satin-1 (Polish-Harden-Seal)" },
                            { label: "Bundle Satin-2 (Hone-Polish-Harden-Seal)", id: "Bundle Satin-2 (Hone-Polish-Harden-Seal)" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Chip Repair", id: "Chip Repair" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Cleaning - Address Mold Stains", id: "Cleaning - Address Mold Stains" },
                            { label: "Crack Repair", id: "Crack Repair" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hard Water Removal", id: "Hard Water Removal" },
                            { label: "Hardening", id: "Hardening" },
                            { label: "Honing", id: "Honing" },
                            { label: "Hydroforce Cleaning", id: "Hydroforce Cleaning" },
                            { label: "Polishing", id: "Polishing" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Stripping Carpet Adhesive", id: "Stripping Carpet Adhesive" },
                            { label: "Stripping-Silicone", id: "Stripping-Silicone" },
                            { label: "Stripping-Unknown", id: "Stripping-Unknown" },
                            { label: "Stripping-Urethane", id: "Stripping-Urethane" },
                            { label: "Stripping-Wax", id: "Stripping-Wax" },
                            { label: "Tile Replacement", id: "Tile Replacement" },
                            { label: "Other", id: "Other" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "TILEINSTALL" && serviceList.typeOfStoneType2 == "Granite") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Grout & Tile Sealing", id: "Grout & Tile Sealing" },
                            { label: "Grout Haze Removal", id: "Grout Haze Removal" },
                            { label: "Grout Sealing - Clear", id: "Grout Sealing - Clear" },
                            { label: "Grouting", id: "Grouting" },
                            { label: "Tile Installation", id: "Tile Installation" },
                            { label: "Tile Removal", id: "Tile Removal" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Marble Material
                    else if (serviceList.serviceType == "COUNTERLIFE" && serviceList.typeOfStoneType2 == "Marble") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean & Protect", id: "Bundle Clean & Protect" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Chip Repair", id: "Chip Repair" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Coating", id: "Coating" },
                            { label: "Crack Repair", id: "Crack Repair" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hard Water Removal", id: "Hard Water Removal" },
                            { label: "Honing", id: "Honing" },
                            { label: "Mold Stain Removal", id: "Mold Stain Removal" },
                            { label: "Polishing", id: "Polishing" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Tile Replacement", id: "Tile Replacement" },
                            { label: "Other", id: "Other" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Marble") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Floor Cleaner Concentrate Gallon", id: "Floor Cleaner Concentrate Gallon" },
                            { label: "Grout Sealer 16 oz", id: "Grout Sealer 16 oz" },
                            { label: "Marble Cleaner 32 oz Spray", id: "Marble Cleaner 32 oz Spray" },
                            { label: "Marble cleaner Conc. 32oz", id: "Marble cleaner Conc. 32oz" },
                            { label: "Marble cleaner Refill Gallon", id: "Marble cleaner Refill Gallon" },
                            { label: "Marble Gloss conditioner 16oz", id: "Marble Gloss conditioner 16oz" },
                            { label: "Marble Polish 16 oz", id: "Marble Polish 16 oz" },
                            { label: "MaxOut Concentrate Gallon", id: "MaxOut Concentrate Gallon" },
                            { label: "MaxOut Grout Cleaner 32 oz", id: "MaxOut Grout Cleaner 32 oz" },
                            { label: "Mold & Mildew Stain Remover 32oz Spray", id: "Mold & Mildew Stain Remover 32oz Spray" },
                            { label: "Mold & Mildew Stain Remover Gallon", id: "Mold & Mildew Stain Remover Gallon" },
                            { label: "Soap Scum Remover 16oz", id: "Soap Scum Remover 16oz" },
                            { label: "Stone Sealer 16 oz", id: "Stone Sealer 16 oz" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "STONELIFE" && serviceList.typeOfStoneType2 == "Marble") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean & Protect", id: "Bundle Clean & Protect" },
                            { label: "Bundle Clean & Seal", id: "Bundle Clean & Seal" },
                            { label: "Bundle Gloss-1 (Polish-Harden-Seal)", id: "Bundle Gloss-1 (Polish-Harden-Seal)" },
                            { label: "Bundle Gloss-2 (Hone-Polish-Harden-Seal)", id: "Bundle Gloss-2 (Hone-Polish-Harden-Seal)" },
                            { label: "Bundle Matte (Hone-Seal)", id: "Bundle Matte (Hone-Seal)" },
                            { label: "Bundle Satin-1 (Polish-Harden-Seal)", id: "Bundle Satin-1 (Polish-Harden-Seal)" },
                            { label: "Bundle Satin-2 (Hone-Polish-Harden-Seal)", id: "Bundle Satin-2 (Hone-Polish-Harden-Seal)" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Chip Repair", id: "Chip Repair" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Coating", id: "Coating" },
                            { label: "Crack Repair", id: "Crack Repair" },
                            { label: "GRIND", id: "GRIND" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hard Water Removal", id: "Hard Water Removal" },
                            { label: "Hardening", id: "Hardening" },
                            { label: "Honing", id: "Honing" },
                            { label: "Hydroforce Cleaning", id: "Hydroforce Cleaning" },
                            { label: "Mold Stain Removal", id: "Mold Stain Removal" },
                            { label: "Polishing", id: "Polishing" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Stripping Carpet Adhesive", id: "Stripping Carpet Adhesive" },
                            { label: "Stripping-Silicone", id: "Stripping-Silicone" },
                            { label: "Stripping-Unknown", id: "Stripping-Unknown" },
                            { label: "Stripping-Urethane", id: "Stripping-Urethane" },
                            { label: "Stripping-Wax", id: "Stripping-Wax" },
                            { label: "Tile Replacement", id: "Tile Replacement" },
                            { label: "Other", id: "Other" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "TILEINSTALL" && serviceList.typeOfStoneType2 == "Marble") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Grout & Tile Sealing", id: "Grout & Tile Sealing" },
                            { label: "Grout Haze Removal", id: "Grout Haze Removal" },
                            { label: "Grout Sealing - Clear", id: "Grout Sealing - Clear" },
                            { label: "Grouting", id: "Grouting" },
                            { label: "Tile Installation", id: "Tile Installation" },
                            { label: "Tile Removal", id: "Tile Removal" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Limestone Material
                    else if (serviceList.serviceType == "COUNTERLIFE" && serviceList.typeOfStoneType2 == "Limestone") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean & Protect", id: "Bundle Clean & Protect" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Chip Repair", id: "Chip Repair" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Coating", id: "Coating" },
                            { label: "Crack Repair", id: "Crack Repair" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hard Water Removal", id: "Hard Water Removal" },
                            { label: "Honing", id: "Honing" },
                            { label: "Mold Stain Removal", id: "Mold Stain Removal" },
                            { label: "Polishing", id: "Polishing" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Tile Replacement", id: "Tile Replacement" },
                            { label: "Other", id: "Other" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Limestone") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Floor Cleaner Concentrate Gallon", id: "Floor Cleaner Concentrate Gallon" },
                            { label: "Grout Sealer 16 oz", id: "Grout Sealer 16 oz" },
                            { label: "Marble cleaner Conc. 32oz", id: "Marble cleaner Conc. 32oz" },
                            { label: "Marble cleaner Refill Gallon", id: "Marble cleaner Refill Gallon" },
                            { label: "Marble Cleaner 32 oz Spray", id: "Marble Cleaner 32 oz Spray" },
                            { label: "Marble Gloss conditioner 16oz", id: "Marble Gloss conditioner 16oz" },
                            { label: "Marble Polish 16 oz", id: "Marble Polish 16 oz" },
                            { label: "MaxOut Concentrate Gallon", id: "MaxOut Concentrate Gallon" },
                            { label: "MaxOut Grout Cleaner 32 oz", id: "MaxOut Grout Cleaner 32 oz" },
                            { label: "Mold & Mildew Stain Remover 32oz Spray", id: "Mold & Mildew Stain Remover 32oz Spray" },
                            { label: "Mold & Mildew Stain Remover Gallon", id: "Mold & Mildew Stain Remover Gallon" },
                            { label: "Soap Scum Remover 16oz", id: "Soap Scum Remover 16oz" },
                            { label: "Stone Sealer 16 oz", id: "Stone Sealer 16 oz" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "STONELIFE" && serviceList.typeOfStoneType2 == "Limestone") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean & Protect", id: "Bundle Clean & Protect" },
                            { label: "Bundle Clean & Seal", id: "Bundle Clean & Seal" },
                            { label: "Bundle Gloss-1 (Polish-Harden-Seal)", id: "Bundle Gloss-1 (Polish-Harden-Seal)" },
                            { label: "Bundle Gloss-2 (Hone-Polish-Harden-Seal)", id: "Bundle Gloss-2 (Hone-Polish-Harden-Seal)" },
                            { label: "Bundle Matte (Hone-Seal)", id: "Bundle Matte (Hone-Seal)" },
                            { label: "Bundle Satin-1 (Polish-Harden-Seal)", id: "Bundle Satin-1 (Polish-Harden-Seal)" },
                            { label: "Bundle Satin-2 (Hone-Polish-Harden-Seal)", id: "Bundle Satin-2 (Hone-Polish-Harden-Seal)" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Chip Repair", id: "Chip Repair" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Coating", id: "Coating" },
                            { label: "Crack Repair", id: "Crack Repair" },
                            { label: "GRIND", id: "GRIND" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hard Water Removal", id: "Hard Water Removal" },
                            { label: "Hardening", id: "Hardening" },
                            { label: "Honing", id: "Honing" },
                            { label: "Hydroforce Cleaning", id: "Hydroforce Cleaning" },
                            { label: "Mold Stain Removal", id: "Mold Stain Removal" },
                            { label: "Polishing", id: "Polishing" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Stripping Carpet Adhesive", id: "Stripping Carpet Adhesive" },
                            { label: "Stripping-Silicone", id: "Stripping-Silicone" },
                            { label: "Stripping-Unknown", id: "Stripping-Unknown" },
                            { label: "Stripping-Urethane", id: "Stripping-Urethane" },
                            { label: "Stripping-Wax", id: "Stripping-Wax" },
                            { label: "Tile Replacement", id: "Tile Replacement" },
                            { label: "Other", id: "Other" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "TILEINSTALL" && serviceList.typeOfStoneType2 == "Limestone") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Grout & Tile Sealing", id: "Grout & Tile Sealing" },
                            { label: "Grout Haze Removal", id: "Grout Haze Removal" },
                            { label: "Grout Sealing - Clear", id: "Grout Sealing - Clear" },
                            { label: "Grouting", id: "Grouting" },
                            { label: "Tile Installation", id: "Tile Installation" },
                            { label: "Tile Removal", id: "Tile Removal" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Metal Material
                    else if (serviceList.serviceType == "METALLIFE" && serviceList.typeOfStoneType2 == "Metal") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Metal Cleaning", id: "Metal Cleaning" },
                            { label: "Metal Laquer, Sealing & Protection", id: "Metal Laquer, Sealing & Protection" },
                            { label: "Metal Scratch Removal", id: "Metal Scratch Removal" },
                            { label: "Metal Stripping", id: "Metal Stripping" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Metal") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Gloss conditioner 16oz", id: "Gloss conditioner 16oz" },
                            { label: "Granite Cleaner 32 oz Spray", id: "Granite Cleaner 32 oz Spray" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Porcelain Material
                    else if (serviceList.serviceType == "COUNTERLIFE" && serviceList.typeOfStoneType2 == "Porcelain") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Repair Chip", id: "Repair Chip" },
                            { label: "Repair Crack", id: "Repair Crack" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Porcelain") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Floor Cleaner Concentrate Gallon", id: "Floor Cleaner Concentrate Gallon" },
                            { label: "Grout Sealer 16 oz", id: "Grout Sealer 16 oz" },
                            { label: "MaxOut Concentrate Gallon", id: "MaxOut Concentrate Gallon" },
                            { label: "MaxOut Grout Cleaner 32 oz", id: "MaxOut Grout Cleaner 32 oz" },
                            { label: "Mold & Mildew Stain Remover 32oz Spray", id: "Mold & Mildew Stain Remover 32oz Spray" },
                            { label: "Mold & Mildew Stain Remover Gallon", id: "Mold & Mildew Stain Remover Gallon" },
                            { label: "Soap Scum Remover 16oz", id: "Soap Scum Remover 16oz" },
                            { label: "Tile Cleaner 32oz Spray", id: "Tile Cleaner 32oz Spray" },
                            { label: "Tile Cleaner Concentrate 32 oz", id: "Tile Cleaner Concentrate 32 oz" },
                            { label: "Tile Cleaner Refill Gallon", id: "Tile Cleaner Refill Gallon" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "TILEINSTALL" && serviceList.typeOfStoneType2 == "Porcelain") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Grout & Tile Sealing", id: "Grout & Tile Sealing" },
                            { label: "Grout Haze Removal", id: "Grout Haze Removal" },
                            { label: "Grout Sealing - Clear", id: "Grout Sealing - Clear" },
                            { label: "Grouting", id: "Grouting" },
                            { label: "Tile Installation", id: "Tile Installation" },
                            { label: "Tile Removal", id: "Tile Removal" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Quartz Material
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Quartz") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Floor Cleaner Concentrate Gallon", id: "Floor Cleaner Concentrate Gallon" },
                            { label: "Gloss conditioner 16oz", id: "Gloss conditioner 16oz" },
                            { label: "Granite Cleaner 32 oz Spray", id: "Granite Cleaner 32 oz Spray" },
                            { label: "Granite Cleaner Refill Gallon", id: "Granite Cleaner Refill Gallon" },
                            { label: "Granite Gloss conditioner 8oz", id: "Granite Gloss conditioner 8oz" },
                            { label: "Granite Sealer 4 oz Spray", id: "Granite Sealer 4 oz Spray" },
                            { label: "Grout Sealer 16 oz", id: "Grout Sealer 16 oz" },
                            { label: "MaxOut Concentrate Gallon", id: "MaxOut Concentrate Gallon" },
                            { label: "MaxOut Grout Cleaner 32 oz", id: "MaxOut Grout Cleaner 32 oz" },
                            { label: "Mold & Mildew Stain Remover 32oz Spray", id: "Mold & Mildew Stain Remover 32oz Spray" },
                            { label: "Mold & Mildew Stain Remover Gallon", id: "Mold & Mildew Stain Remover Gallon" },
                            { label: "Soap Scum Remover 16oz", id: "Soap Scum Remover 16oz" },
                            { label: "Stone Sealer 16 oz", id: "Stone Sealer 16 oz" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "TILEINSTALL" && serviceList.typeOfStoneType2 == "Quartz") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Grout & Tile Sealing", id: "Grout & Tile Sealing" },
                            { label: "Grout Haze Removal", id: "Grout Haze Removal" },
                            { label: "Grout Sealing - Clear", id: "Grout Sealing - Clear" },
                            { label: "Grouting", id: "Grouting" },
                            { label: "Tile Installation", id: "Tile Installation" },
                            { label: "Tile Removal", id: "Tile Removal" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Brick Material
                    else if (serviceList.serviceType == "COLORSEAL" && serviceList.typeOfStoneType2 == "Brick") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "No Service Available", id: "No Service Available" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if ((serviceList.serviceType == "CONCRETE-COATINGS" || serviceList.serviceType == "CONCRETE-COUNTERTOPS") && serviceList.typeOfStoneType2 == "Brick") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Hone", id: "Hone" },
                            { label: "Repair Chip", id: "Repair Chip" },
                            { label: "Repair Crack", id: "Repair Crack" },
                            { label: "Seal", id: "Seal" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "COUNTERLIFE" && serviceList.typeOfStoneType2 == "Brick") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean & Protect", id: "Bundle Clean & Protect" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Chip Repair", id: "Chip Repair" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Coating", id: "Coating" },
                            { label: "Crack Repair", id: "Crack Repair" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hard Water Removal", id: "Hard Water Removal" },
                            { label: "Honing", id: "Honing" },
                            { label: "Mold Stain Removal", id: "Mold Stain Removal" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Tile Replacement", id: "Tile Replacement" },
                            { label: "Other", id: "Other" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Brick") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Floor Cleaner Concentrate Gallon", id: "Floor Cleaner Concentrate Gallon" },
                            { label: "Grout Sealer 16 oz", id: "Grout Sealer 16 oz" },
                            { label: "MaxOut Concentrate Gallon", id: "MaxOut Concentrate Gallon" },
                            { label: "MaxOut Grout Cleaner 32 oz", id: "MaxOut Grout Cleaner 32 oz" },
                            { label: "Mold & Mildew Stain Remover 32oz Spray", id: "Mold & Mildew Stain Remover 32oz Spray" },
                            { label: "Mold & Mildew Stain Remover Gallon", id: "Mold & Mildew Stain Remover Gallon" },
                            { label: "Soap Scum Remover 16oz", id: "Soap Scum Remover 16oz" },
                            { label: "Stone Sealer 16 oz", id: "Stone Sealer 16 oz" },
                            { label: "Tile Cleaner 32oz Spray", id: "Tile Cleaner 32oz Spray" },
                            { label: "Tile Cleaner Concentrate 32 oz", id: "Tile Cleaner Concentrate 32 oz" },
                            { label: "Tile Cleaner Refill Gallon", id: "Tile Cleaner Refill Gallon" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "STONELIFE" && serviceList.typeOfStoneType2 == "Brick") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean & Protect", id: "Bundle Clean & Protect" },
                            { label: "Bundle Clean & Seal", id: "Bundle Clean & Seal" },
                            { label: "Bundle Matte (Hone-Seal)", id: "Bundle Matte (Hone-Seal)" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Chip Repair", id: "Chip Repair" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Coating", id: "Coating" },
                            { label: "Crack Repair", id: "Crack Repair" },
                            { label: "Grind", id: "Grind" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hard Water Removal", id: "Hard Water Removal" },
                            { label: "Hardening", id: "Hardening" },
                            { label: "Honing", id: "Honing" },
                            { label: "Hydroforce Cleaning", id: "Hydroforce Cleaning" },
                            { label: "Mold Stain Removal", id: "Mold Stain Removal" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Stripping Carpet Adhesive", id: "Stripping Carpet Adhesive" },
                            { label: "Stripping-Silicone", id: "Stripping-Silicone" },
                            { label: "Stripping-Unknown", id: "Stripping-Unknown" },
                            { label: "Stripping-Urethane", id: "Stripping-Urethane" },
                            { label: "Stripping-Wax", id: "Stripping-Wax" },
                            { label: "Tile Replacement", id: "Tile Replacement" },
                            { label: "Other", id: "Other" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Engineered Stone:Cesar, Engineered Stone:Corian, Engineered Stone:Other, Engineered Stone:Zodiaq Material
                    else if (serviceList.serviceType == "COUNTERLIFE" && ((serviceList.typeOfStoneType2 == "Engineered Stone:Cesar") || (serviceList.typeOfStoneType2 == "Engineered Stone:Corian") || (serviceList.typeOfStoneType2 == "Engineered Stone:Other") || (serviceList.typeOfStoneType2 == "Engineered Stone:Zodiaq"))) {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Caulking", id: "Caulking" },
                            { label: "Chip Repair", id: "Chip Repair" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Crack Repair", id: "Crack Repair" },
                            { label: "Hard Water Removal", id: "Hard Water Removal" },
                            { label: "Honing", id: "Honing" },
                            { label: "Polishing", id: "Polishing" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Other", id: "Other" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "PRODUCT" && ((serviceList.typeOfStoneType2 == "Engineered Stone:Cesar") || (serviceList.typeOfStoneType2 == "Engineered Stone:Corian") || (serviceList.typeOfStoneType2 == "Engineered Stone:Other") || (serviceList.typeOfStoneType2 == "Engineered Stone:Zodiaq"))) {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Granite Cleaner 32 oz Spray", id: "Granite Cleaner 32 oz Spray" },
                            { label: "Granite Cleaner Refill Gallon", id: "Granite Cleaner Refill Gallon" },
                            { label: "Granite Gloss conditioner 8oz", id: "Granite Gloss conditioner 8oz" },
                            { label: "Granite Sealer 4 oz Spray", id: "Granite Sealer 4 oz Spray" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Flagstone Material
                    else if (serviceList.serviceType == "COUNTERLIFE" && serviceList.typeOfStoneType2 == "Flagstone") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean & Protect", id: "Bundle Clean & Protect" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Chip Repair", id: "Chip Repair" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Coating", id: "Coating" },
                            { label: "Crack Repair", id: "Crack Repair" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hard Water Removal", id: "Hard Water Removal" },
                            { label: "Honing", id: "Honing" },
                            { label: "Mold Stain Removal", id: "Mold Stain Removal" },
                            { label: "Polishing", id: "Polishing" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Other", id: "Other" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "GROUTLIFE/TILELOK" && serviceList.typeOfStoneType2 == "Flagstone") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean/Colorseal/ Interlok", id: "Bundle Clean/Colorseal/ Interlok" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Colorseal", id: "Colorseal" },
                            { label: "Colorseal-Hydroforce", id: "Colorseal-Hydroforce" },
                            { label: "Colorseal-Powerscrub", id: "Colorseal-Powerscrub" },
                            { label: "Colorseal-Powerwash", id: "Colorseal-Powerwash" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Flagstone") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Floor Cleaner Concentrate Gallon", id: "Floor Cleaner Concentrate Gallon" },
                            { label: "Grout Sealer 16 oz", id: "Grout Sealer 16 oz" },
                            { label: "Marble Cleaner 32 oz Spray", id: "Marble Cleaner 32 oz Spray" },
                            { label: "Marble cleaner Conc. 32oz", id: "Marble cleaner Conc. 32oz" },
                            { label: "Marble cleaner Refill Gallon", id: "Marble cleaner Refill Gallon" },
                            { label: "MaxOut Concentrate Gallon", id: "MaxOut Concentrate Gallon" },
                            { label: "MaxOut Grout Cleaner 32 oz", id: "MaxOut Grout Cleaner 32 oz" },
                            { label: "Mold & Mildew Stain Remover 32oz Spray", id: "Mold & Mildew Stain Remover 32oz Spray" },
                            { label: "Mold & Mildew Stain Remover Gallon", id: "Mold & Mildew Stain Remover Gallon" },
                            { label: "Soap Scum Remover 16oz", id: "Soap Scum Remover 16oz" },
                            { label: "Stone Sealer 16 oz", id: "Stone Sealer 16 oz" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "STONELIFE" && serviceList.typeOfStoneType2 == "Flagstone") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean & Protect", id: "Bundle Clean & Protect" },
                            { label: "Bundle Clean & Seal", id: "Bundle Clean & Seal" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Chip Repair", id: "Chip Repair" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Coating", id: "Coating" },
                            { label: "Crack Repair", id: "Crack Repair" },
                            { label: "GRIND", id: "GRIND" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hard Water Removal", id: "Hard Water Removal" },
                            { label: "Hardening", id: "Hardening" },
                            { label: "Honing", id: "Honing" },
                            { label: "Hydroforce Cleaning", id: "Hydroforce Cleaning" },
                            { label: "Mold Stain Removal", id: "Mold Stain Removal" },
                            { label: "Other", id: "Other" },
                            { label: "Polishing", id: "Polishing" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Stripping Carpet Adhesive", id: "Stripping Carpet Adhesive" },
                            { label: "Stripping-Silicone", id: "Stripping-Silicone" },
                            { label: "Stripping-Unknown", id: "Stripping-Unknown" },
                            { label: "Stripping-Urethane", id: "Stripping-Urethane" },
                            { label: "Stripping-Wax", id: "Stripping-Wax" },
                            { label: "Tile Replacement", id: "Tile Replacement" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Marble (Tumbled) Material
                    else if (serviceList.serviceType == "COLORSEAL" && serviceList.typeOfStoneType2 == "Marble (Tumbled)") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "No Service Available", id: "No Service Available" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "COUNTERLIFE" && serviceList.typeOfStoneType2 == "Marble (Tumbled)") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean & Protect", id: "Bundle Clean & Protect" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Chip Repair", id: "Chip Repair" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Crack Repair", id: "Crack Repair" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hard Water Removal", id: "Hard Water Removal" },
                            { label: "Honing", id: "Honing" },
                            { label: "Mold Stain Removal", id: "Mold Stain Removal" },
                            { label: "Polishing", id: "Polishing" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Tile Replacement", id: "Tile Replacement" },
                            { label: "Other", id: "Other" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "GROUTLIFE/TILELOK" && serviceList.typeOfStoneType2 == "Marble (Tumbled)") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean/Colorseal/ Interlok", id: "Bundle Clean/Colorseal/ Interlok" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Colorseal", id: "Colorseal" },
                            { label: "Colorseal-Hydroforce", id: "Colorseal-Hydroforce" },
                            { label: "Colorseal-Powerscrub", id: "Colorseal-Powerscrub" },
                            { label: "Colorseal-Powerwash", id: "Colorseal-Powerwash" },
                            { label: "Grout Repair", id: "Grout Repair" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hydroforce Cleaning", id: "Hydroforce Cleaning" },
                            { label: "Powerwash Cleaning", id: "Powerwash Cleaning" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Stripping Carpet Adhesive", id: "Stripping Carpet Adhesive" },
                            { label: "Stripping-Silicone", id: "Stripping-Silicone" },
                            { label: "Stripping-Unknown", id: "Stripping-Unknown" },
                            { label: "Stripping-Urethane", id: "Stripping-Urethane" },
                            { label: "Stripping-Wax", id: "Stripping-Wax" },
                            { label: "Tile Rebond", id: "Tile Rebond" },
                            { label: "Tile Replacement", id: "Tile Replacement" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Marble (Tumbled)") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Floor Cleaner Concentrate Gallon", id: "Floor Cleaner Concentrate Gallon" },
                            { label: "Grout Sealer 16 oz", id: "Grout Sealer 16 oz" },
                            { label: "Marble Cleaner 32 oz Spray", id: "Marble Cleaner 32 oz Spray" },
                            { label: "Marble cleaner Conc. 32oz", id: "Marble cleaner Conc. 32oz" },
                            { label: "Marble cleaner Refill Gallon", id: "Marble cleaner Refill Gallon" },
                            { label: "Marble Gloss conditioner 16oz", id: "Marble Gloss conditioner 16oz" },
                            { label: "Marble Polish 16 oz", id: "Marble Polish 16 oz" },
                            { label: "MaxOut Concentrate Gallon", id: "MaxOut Concentrate Gallon" },
                            { label: "MaxOut Grout Cleaner 32 oz", id: "MaxOut Grout Cleaner 32 oz" },
                            { label: "Mold & Mildew Stain Remover 32oz Spray", id: "Mold & Mildew Stain Remover 32oz Spray" },
                            { label: "Mold & Mildew Stain Remover Gallon", id: "Mold & Mildew Stain Remover Gallon" },
                            { label: "Soap Scum Remover 16oz", id: "Soap Scum Remover 16oz" },
                            { label: "Stone Sealer 16 oz", id: "Stone Sealer 16 oz" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "STONELIFE" && serviceList.typeOfStoneType2 == "Marble (Tumbled)") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean & Protect", id: "Bundle Clean & Protect" },
                            { label: "Bundle Clean & Seal", id: "Bundle Clean & Seal" },
                            { label: "Bundle Gloss-1 (Polish-Harden-Seal)", id: "Bundle Gloss-1 (Polish-Harden-Seal)" },
                            { label: "Bundle Gloss-2 (Hone-Polish-Harden-Seal)", id: "Bundle Gloss-2 (Hone-Polish-Harden-Seal)" },
                            { label: "Bundle Matte (Hone-Seal)", id: "Bundle Matte (Hone-Seal)" },
                            { label: "Bundle Satin-1 (Polish-Harden-Seal)", id: "Bundle Satin-1 (Polish-Harden-Seal)" },
                            { label: "Bundle Satin-2 (Hone-Polish-Harden-Seal)", id: "Bundle Satin-2 (Hone-Polish-Harden-Seal)" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Chip Repair", id: "Chip Repair" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Coating", id: "Coating" },
                            { label: "Crack Repair", id: "Crack Repair" },
                            { label: "Grind", id: "Grind" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hard Water Removal", id: "Hard Water Removal" },
                            { label: "Hardening", id: "Hardening" },
                            { label: "Honing", id: "Honing" },
                            { label: "Hydroforce Cleaning", id: "Hydroforce Cleaning" },
                            { label: "Mold Stain Removal", id: "Mold Stain Removal" },
                            { label: "Polishing", id: "Polishing" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Stripping Carpet Adhesive", id: "Stripping Carpet Adhesive" },
                            { label: "Stripping-Silicone", id: "Stripping-Silicone" },
                            { label: "Stripping-Unknown", id: "Stripping-Unknown" },
                            { label: "Stripping-Urethane", id: "Stripping-Urethane" },
                            { label: "Stripping-Wax", id: "Stripping-Wax" },
                            { label: "Tile Replacement", id: "Tile Replacement" },
                            { label: "Other", id: "Other" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For Mexican Tile Material
                    else if (serviceList.serviceType == "COLORSEAL" && serviceList.typeOfStoneType2 == "Mexican Tile") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "No Service Available", id: "No Service Available" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "COUNTERLIFE" && serviceList.typeOfStoneType2 == "Mexican Tile") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean & Protect", id: "Bundle Clean & Protect" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Chip Repair", id: "Chip Repair" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hard Water Removal", id: "Hard Water Removal" },
                            { label: "Honing", id: "Honing" },
                            { label: "Mold Stain Removal", id: "Mold Stain Removal" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Tile Replacement", id: "Tile Replacement" },
                            { label: "Other", id: "Other" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "GROUTLIFE/TILELOK" && serviceList.typeOfStoneType2 == "Mexican Tile") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean/Colorseal/ Interlok", id: "Bundle Clean/Colorseal/ Interlok" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Colorseal", id: "Colorseal" },
                            { label: "Colorseal-Hydroforce", id: "Colorseal-Hydroforce" },
                            { label: "Colorseal-Powerscrub", id: "Colorseal-Powerscrub" },
                            { label: "Colorseal-Powerwash", id: "Colorseal-Powerwash" },
                            { label: "Grout Repair", id: "Grout Repair" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hydroforce Cleaning", id: "Hydroforce Cleaning" },
                            { label: "Powerwash Cleaning", id: "Powerwash Cleaning" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Stripping Carpet Adhesive", id: "Stripping Carpet Adhesive" },
                            { label: "Stripping-Silicone", id: "Stripping-Silicone" },
                            { label: "Stripping-Unknown", id: "Stripping-Unknown" },
                            { label: "Stripping-Urethane", id: "Stripping-Urethane" },
                            { label: "Stripping-Wax", id: "Stripping-Wax" },
                            { label: "Tile Rebond", id: "Tile Rebond" },
                            { label: "Tile Replacement", id: "Tile Replacement" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Mexican Tile") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Floor Cleaner Concentrate Gallon", id: "Floor Cleaner Concentrate Gallon" },
                            { label: "Grout Sealer 16 oz", id: "Grout Sealer 16 oz" },
                            { label: "MaxOut Concentrate Gallon", id: "MaxOut Concentrate Gallon" },
                            { label: "MaxOut Grout Cleaner 32 oz", id: "MaxOut Grout Cleaner 32 oz" },
                            { label: "Stone Sealer 16 oz", id: "Stone Sealer 16 oz" },
                            { label: "Tile Cleaner 32oz Spray", id: "Tile Cleaner 32oz Spray" },
                            { label: "Tile Cleaner Concentrate 32 oz", id: "Tile Cleaner Concentrate 32 oz" },
                            { label: "Tile Cleaner Refill Gallon", id: "Tile Cleaner Refill Gallon" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "PRODUCT" && serviceList.typeOfStoneType2 == "Mexican Tile") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean & Protect", id: "Bundle Clean & Protect" },
                            { label: "Bundle Clean & Seal", id: "Bundle Clean & Seal" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Chip Repair", id: "Chip Repair" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Coating", id: "Coating" },
                            { label: "Crack Repair", id: "Crack Repair" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hard Water Removal", id: "Hard Water Removal" },
                            { label: "Hardening", id: "Hardening" },
                            { label: "Honing", id: "Honing" },
                            { label: "Hydroforce Cleaning", id: "Hydroforce Cleaning" },
                            { label: "Mold Stain Removal", id: "Mold Stain Removal" },
                            { label: "Other", id: "Other" },
                            { label: "Sealing", id: "Sealing" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Stripping Carpet Adhesive", id: "Stripping Carpet Adhesive" },
                            { label: "Stripping-Silicone", id: "Stripping-Silicone" },
                            { label: "Stripping-Unknown", id: "Stripping-Unknown" },
                            { label: "Stripping-Urethane", id: "Stripping-Urethane" },
                            { label: "Stripping-Wax", id: "Stripping-Wax" },
                            { label: "Tile Replacement", id: "Tile Replacement" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }


                    //For Common List of Services
                    // For List of Services- Hone, Polish, Repair Chip, Repair Crack, Seal
                    else if (
                        (serviceList.serviceType == "COUNTERLIFE" && serviceList.typeOfStoneType2 == "Slate") ||
                        (serviceList.serviceType == "COUNTERLIFE" && serviceList.typeOfStoneType2 == "Concrete") ||
                        (serviceList.serviceType == "CONCRETE-REPAIR" && serviceList.typeOfStoneType2 == "Concrete") ||
                        (serviceList.serviceType == "CONCRETE-COUNTERTOPS" && serviceList.typeOfStoneType2 == "Concrete") ||
                        (serviceList.serviceType == "STONELIFE" && serviceList.typeOfStoneType2 == "Slate") ||
                        (serviceList.serviceType == "STONELIFE" && serviceList.typeOfStoneType2 == "Vinyl") ||
                        (serviceList.serviceType == "CONCRETE-REPAIR" && serviceList.typeOfStoneType2 == "Terrazzo") ||
                        (serviceList.serviceType == "COUNTERLIFE" && serviceList.typeOfStoneType2 == "Terrazzo") ||
                        (serviceList.serviceType == "COUNTERLIFE" && serviceList.typeOfStoneType2 == "Quartz") ||
                        (serviceList.serviceType == "STONELIFE" && serviceList.typeOfStoneType2 == "Quartz") ||
                        (serviceList.serviceType == "CONCRETE-OVERLAYMENTS" && serviceList.typeOfStoneType2 == "Brick")
                    ) {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Hone", id: "Hone" },
                            { label: "Polish", id: "Polish" },
                            { label: "Repair Chip", id: "Repair Chip" },
                            { label: "Repair Crack", id: "Repair Crack" },
                            { label: "Seal", id: "Seal" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For CLEANSHIELD
                    else if (
                        (serviceList.serviceType == "CLEANSHIELD" && serviceList.typeOfStoneType2 == "Granite") ||
                        (serviceList.serviceType == "CLEANSHIELD" && serviceList.typeOfStoneType2 == "Travertine") ||
                        (serviceList.serviceType == "CLEANSHIELD" && serviceList.typeOfStoneType2 == "Marble") ||
                        (serviceList.serviceType == "CLEANSHIELD" && serviceList.typeOfStoneType2 == "Limestone") ||
                        (serviceList.serviceType == "CLEANSHIELD" && serviceList.typeOfStoneType2 == "Quartz")
                    ) {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Cleanshield", id: "Cleanshield" },
                            { label: "Hone", id: "Hone" }
                        ]
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For GROUTLIFE/TILELOK
                    else if (serviceList.serviceType == "GROUTLIFE/TILELOK" && ((serviceList.typeOfStoneType2 == "Travertine") || (serviceList.typeOfStoneType2 == "Terrazzo") || (serviceList.typeOfStoneType2 == "Vinyl") || (serviceList.typeOfStoneType2 == "Slate") || (serviceList.typeOfStoneType2 == "Granite") || (serviceList.typeOfStoneType2 == "Marble") || (serviceList.typeOfStoneType2 == "Limestone") || (serviceList.typeOfStoneType2 == "Porcelain") || (serviceList.typeOfStoneType2 == "Quartz") || (serviceList.typeOfStoneType2 == "Brick"))) {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Bundle Clean/Colorseal/ Interlok", id: "Bundle Clean/Colorseal/ Interlok" },
                            { label: "Caulking", id: "Caulking" },
                            { label: "Ceramic", id: "Ceramic" },
                            { label: "Cleaning", id: "Cleaning" },
                            { label: "Colorseal", id: "Colorseal" },
                            { label: "Colorseal-Hydroforce", id: "Colorseal-Hydroforce" },
                            { label: "Colorseal-Powerwash", id: "Colorseal-Powerwash" },
                            { label: "Colorseal-Powerscrub", id: "Colorseal-Powerscrub" },
                            { label: "Grout Repair", id: "Grout Repair" },
                            { label: "Grout Replacement", id: "Grout Replacement" },
                            { label: "Hydroforce Cleaning", id: "Hydroforce Cleaning" },
                            { label: "Powerwash Cleaning", id: "Powerwash Cleaning" },
                            { label: "Stain Removal", id: "Stain Removal" },
                            { label: "Stripping Carpet Adhesive", id: "Stripping Carpet Adhesive" },
                            { label: "Stripping-Silicone", id: "Stripping-Silicone" },
                            { label: "Stripping-Unknown", id: "Stripping-Unknown" },
                            { label: "Stripping-Urethane", id: "Stripping-Urethane" },
                            { label: "Stripping-Wax", id: "Stripping-Wax" },
                            { label: "Tile Rebond", id: "Tile Rebond" },
                            { label: "Tile Replacement", id: "Tile Replacement" }
                        ];
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For SALESTAX
                    else if (serviceList.serviceType == "SALESTAX") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Products", id: "Products" },
                            { label: "Service", id: "Service" }
                        ];
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    //For MAINTENANCE
                    else if (serviceList.serviceType == "MAINTENANCE:BI-MONTHLY") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "MAINTENANCE:Bi-Monthly", id: "MAINTENANCE:Bi-Monthly" }
                        ];
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "MAINTENANCE:MONTHLY") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "MAINTENANCE:Monthly", id: "MAINTENANCE:Monthly" }
                        ];
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "MAINTENANCE:OTHER") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "Maintenance:(Other)", id: "Maintenance:(Other)" }
                        ];
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else if (serviceList.serviceType == "MAINTENANCE:QUARTERLY") {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "MAINTENANCE:Quarterly", id: "MAINTENANCE:Quarterly" }
                        ];
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                    else {
                        vm.serviceTypeForInvoice = [];
                        vm.serviceTypeForInvoice = [
                            { label: "No Service Available", id: "No Service Available" }
                        ];
                        vm.serviceTypeForInvoiceList[index] = [];
                        vm.serviceTypeForInvoiceList[index] = vm.serviceTypeForInvoice;
                    }
                }

                function getSalesPersonNotes(serviceList, index) {
                    serviceList.informationNote = "";
                    serviceList.salesPersonNote = "";
                    serviceList.class = "blinka";
                    if (serviceList.serviceType == "STONELIFE") {
                        if (serviceList.typeOfStoneType2 == "Marble") {
                            if (serviceList.serviceIds[0].id == "Stripping Carpet Adhesive" || serviceList.serviceIds[0].id == "Stripping-Silicone" || serviceList.serviceIds[0].id == "Stripping-Urethane" || serviceList.serviceIds[0].id == "Stripping-Wax" || serviceList.serviceIds[0].id == "Stripping-Unknown") {
                                serviceList.salesPersonNote = "Recommend this be on a separate invoice from rest of project in order to allow client to source independently, as many do not understand that unlike a janitorial wax stripping where they need to get most of it off, we need to get every corner and grout line.";
                            }
                            else if (serviceList.serviceIds[0].id == "Chip Repair") {
                                serviceList.salesPersonNote = "Chip and Crack repairs can be provided that are fast, but will cup, or overfilled, honed and flat but take more time. Be sure to define the approach you are providing.";
                                serviceList.informationNote = "INSERT CHIP REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE (MARBLE)";
                            }
                            else if (serviceList.serviceIds[0].id == "Crack Repair") {
                                serviceList.salesPersonNote = "Chip and Crack repairs can be provided that are fast, but will cup, or overfilled, honed and flat but take more time. Be sure to define the approach you are providing.";
                                serviceList.informationNote = "INSERT CRACK REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE (MARBLE)";
                            }
                            else if (serviceList.serviceIds[0].id == "Grout Replacement") {
                                serviceList.informationNote = "INSERT BEFORE & AFTER IMAGE";
                            }
                            else if (serviceList.serviceIds[0].id == "Hard Water Removal") {
                                serviceList.informationNote = "INSERT BEFORE & AFTER IMAGE (HARD WATER)";
                            }
                            else if (serviceList.serviceIds[0].id == "Mold Stain Removal") {
                                serviceList.informationNote = "INSERT BEFORE & AFTER IMAGE (MOLD)";
                            }
                            else if (serviceList.serviceIds[0].id == "Traverfil") {
                                serviceList.informationNote = "INSERT BEFORE & AFTER IMAGE (TRAVERFIL)";
                            }
                            else if (serviceList.serviceIds[0].id == "Stain Removal") {
                                serviceList.salesPersonNote = "RECOMMEND this be on a separate invoice to be done BEFORE the rest of the project or AFTER the rest of the project in order to avoid having other work held hostage should the stain prove unremovable.";
                            }
                            else {
                                serviceList.informationNote = "";
                                serviceList.salesPersonNote = "";
                            }
                        }
                        else if (serviceList.typeOfStoneType2 == "Granite") {
                            if (serviceList.serviceIds[0].id == "Stripping Carpet Adhesive" || serviceList.serviceIds[0].id == "Stripping-Silicone" || serviceList.serviceIds[0].id == "Stripping-Urethane" || serviceList.serviceIds[0].id == "Stripping-Wax" || serviceList.serviceIds[0].id == "Stripping-Unknown") {
                                serviceList.salesPersonNote = "Recommend this be on a separate invoice from rest of project in order to allow client to source independently, as many do not understand that unlike a janitorial wax stripping where they need to get most of it off, we need to get every corner and grout line.";
                            }
                            else if (serviceList.serviceIds[0].id == "Chip Repair") {
                                serviceList.salesPersonNote = "Chip and Crack repairs can be provided that are fast, but will cup, or overfilled, honed and flat but take more time. Be sure to define the approach you are providing.";
                                serviceList.informationNote = "INSERT CHIP REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE (GRANITE)";
                            }
                            else if (serviceList.serviceIds[0].id == "Crack Repair") {
                                serviceList.salesPersonNote = "Chip and Crack repairs can be provided that are fast, but will cup, or overfilled, honed and flat but take more time. Be sure to define the approach you are providing.";
                                serviceList.informationNote = "INSERT CRACK REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE (GRANITE)";
                            }
                            else if (serviceList.serviceIds[0].id == "Stain Removal") {
                                serviceList.salesPersonNote = "RECOMMEND this be on a separate invoice to be done BEFORE the rest of the project or AFTER the rest of the project in order to avoid having other work held hostage should the stain prove unremovable.";
                            }
                            else if (serviceList.serviceIds[0].id == "Traverfil") {
                                serviceList.informationNote = "INSERT BEFORE & AFTER IMAGE (TRAVERFIL)";
                            }
                            else if (serviceList.serviceIds[0].id == "Stain Removal") {
                                serviceList.salesPersonNote = "RECOMMEND this be on a separate invoice to be done BEFORE the rest of the project or AFTER the rest of the project in order to avoid having other work held hostage should the stain prove unremovable.";
                            }
                            else if (serviceId == "Hard Water Removal") {
                                informationNote = "INSERT BEFORE & AFTER IMAGE (HARD WATER)";
                            }
                            else if (serviceList.serviceIds[0].id == "Honing") {
                                serviceList.salesPersonNote = "Rarely required on granite. If the granite looks dull it is more likely they are suffering a build-up of wax from use of an inappropriate cleaner such as a glass or all purpose cleaner which includes micro-waxes. Clean first with acetone to remove residual wax before consider applying diamonds. Work backwards starting with an 3000, 1500, 800 in a test area. Never dip below 800. Foot traffic will NOT create damage below 800. Treat any scratches on spot basis.";
                            }
                            else if (serviceList.serviceIds[0].id == "Sealing") {
                                serviceList.salesPersonNote = "OFFER to schedule their ANNUAL Clean-and-Seal visit.";
                            }
                            else {
                                serviceList.informationNote = "";
                                serviceList.salesPersonNote = "";
                            }
                        }
                    }
                    else if (serviceList.serviceType == "COUNTERLIFE") {
                        if (serviceList.typeOfStoneType2 == "Marble") {
                            if (serviceList.serviceIds[0].id == "Chip Repair") {
                                serviceList.salesPersonNote = "Chip and Crack repairs can be provided that are fast, but will cup, or overfilled, honed and flat but take more time. Be sure to define the approach you are providing.";
                                serviceList.informationNote = "INSERT CHIP REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE";
                            }
                            else if (serviceList.serviceIds[0].id == "Crack Repair") {
                                serviceList.salesPersonNote = "Chip and Crack repairs can be provided that are fast, but will cup, or overfilled, honed and flat but take more time. Be sure to define the approach you are providing.";
                                serviceList.informationNote = "INSERT CRACK REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE";
                            }
                            else if (serviceList.serviceIds[0].id == "Hard Water Removal") {
                                serviceList.informationNote = "INSERT BEFORE & AFTER IMAGE (HARD WATER)";
                            }
                            else if (serviceList.serviceIds[0].id == "Mold Stain Removal") {
                                serviceList.informationNote = "INSERT BEFORE & AFTER IMAGE (MOLD)";
                            }
                            else if (serviceList.serviceIds[0].id == "Sealing") {
                                serviceList.salesPersonNote = "OFFER to schedule their ANNUAL Clean-and-Seal visit";
                            }
                            else if (serviceList.serviceIds[0].id == "Stain Removal") {
                                serviceList.salesPersonNote = "RECOMMEND this be on a separate invoice to be done BEFORE the rest of the project or AFTER the rest of the project in order to avoid having other work held hostage should the stain prove unremovable.";
                            }
                            else {
                                serviceList.informationNote = "";
                                serviceList.salesPersonNote = "";
                            }
                        }
                        if (serviceList.typeOfStoneType2 == "Granite") {
                            if (serviceList.serviceIds[0].id == "Chip Repair") {
                                serviceList.informationNote = "INSERT CHIP REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE";
                            }
                            else if (serviceList.serviceIds[0].id == "Crack Repair") {
                                serviceList.informationNote = "INSERT CRACK REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE";
                            }
                            else if (serviceList.serviceIds[0].id == "Hard Water Removal") {
                                serviceList.informationNote = "INSERT BEFORE & AFTER IMAGE (HARD WATER)";
                            }
                            else if (serviceList.serviceIds[0].id == "Stain Removal") {
                                serviceList.salesPersonNote = "RECOMMEND this be on a separate invoice to be done BEFORE the rest of the project or AFTER the rest of the project in order to avoid having other work held hostage should the stain prove unremovable.";
                            }
                            else if (serviceList.serviceIds[0].id == "Seam Repair") {
                                serviceList.informationNote = "INSERT BEFORE & AFTER SEAM REPAIR";
                            }
                            else if (serviceList.serviceIds[0].id == "Honing") {
                                serviceList.salesPersonNote = "Rarely required on granite. If the granite looks dull it is more likely they are suffering a build-up of wax from use of an inappropriate cleaner such as a glass or all purpose cleaner which includes micro-waxes. Clean first with acetone to remove residual wax before consider applying diamonds. Work backwards starting with an 11000, 8000, 3000 in a test area. Never dip below 800.";
                            }
                            else if (serviceList.serviceIds[0].id == "Sealing") {
                                serviceList.salesPersonNote = "OFFER to schedule their ANNUAL Clean-and-Seal visit.";
                            }
                            else {
                                serviceList.informationNote = "";
                                serviceList.salesPersonNote = "";
                            }
                        }
                        if (serviceList.typeOfStoneType2 == "Corian") {
                            if (serviceList.serviceIds[0].id == "Corian-Chip Repair") {
                                serviceList.informationNote = "INSERT CHIP REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE (CORIAN)";
                            }
                            else if (serviceList.serviceIds[0].id == "Corian-Crack Repair") {
                                serviceList.salesPersonNote = "Explain that this is major surgery as a wedge needs to be cut from the counter and replaced. Customer should have additional material from initial install, else there will be a color difference due to age, oxidation and wear. This should run no less than 800 per area. If one fails one purchases the counter OR they understand they need to replace it.";
                                serviceList.informationNote = "INSERT CRACK REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE (CORIAN)";
                            }
                            else {
                                serviceList.informationNote = "";
                                serviceList.salesPersonNote = "";
                            }
                        }
                        if (serviceList.typeOfStoneType2 == "Engineered Stone") {
                            if (serviceList.serviceIds[0].id == "Engineered Stone - Polish") {
                                serviceList.salesPersonNote = "MARBLELIFE developed the techniques to restore original aligator skin Zodiac surface texturing.";
                                serviceList.informationNote = "INSERT BEFORE & AFTER EXAMPLE IMAGE (ENGINEERED STONE)";
                            }
                            else {
                                serviceList.informationNote = "";
                                serviceList.salesPersonNote = "";
                            }
                        }
                    }
                    else if (serviceList.serviceType == "CLEANSHIELD") {
                        if (serviceList.serviceIds[0].id == "Cleanshield") {
                            serviceList.informationNote = "INSERT BEFORE & AFTER IMAGE (CLEANSHIELD)";
                        }
                    }
                    else if (serviceList.serviceType == "MAINTAINANCE:OTHER" || serviceList.serviceType == "MAINTENANCE:BI-MONTHLY" || serviceList.serviceType == "MAINTENANCE:MONTHLY" || serviceList.serviceType == "MAINTENANCE:QUARTERLY") {
                        serviceList.salesPersonNote = "Typically need 1 man-day per 1500-2000 sqft of floor.  Quarterly service only makes sense for properties less than 700 sqft.  Otherwise quarterly is 5x monthly - due to scratch pattern development.";
                        serviceList.informationNote = "INSERT AFTER IMAGE OF PROPERTY MAINTAINED BY MARBLELIFE FOR OVER 15 YEARS";
                    }
                    else if (serviceList.serviceType == "GROUTLIFE/TILELOCK") {
                        if (serviceList.typeOfStoneType2 == "CERAMIC" || serviceList.typeOfStoneType2 == "PORCELAIN" || serviceList.typeOfStoneType2 == "CONCRETE" || serviceList.typeOfStoneType2 == "TERRAZZO") {
                            if (serviceList.serviceIds[0].id == "Tilelok") {
                                serviceList.informationNote = "INSERT BEFORE & AFTER IMAGES TILELOK";
                            }
                            else {
                                serviceList.salesPersonNote = "";
                                serviceList.informationNote = "";
                            }
                        }
                        else {
                            if (serviceList.serviceIds[0].id == "Stripping Carpet Adhesive" || serviceList.serviceIds[0].id == "Stripping-Silicone" || serviceList.serviceIds[0].id == "Stripping-Urethane" || serviceList.serviceIds[0].id == "Stripping-Wax" || serviceList.serviceIds[0].id == "Stripping-Unknown") {
                                serviceList.salesPersonNote = "Recommend this be on a separate invoice from rest of project in order to allow client to source independently, as many do not understand that unlike a janitorial wax stripping where they need to get most of it off, we need to get every corner and grout line.";
                            }
                            else if (serviceList.serviceIds[0].id == "Grout Replacement") {
                                serviceList.salesPersonNote = "Grout damage is generally caused by either (1) poorly mixed grout or (2) building settling, or (3) a flexing floor.  If the building is still settling grout could be expected to rebreak.  If the floor is flexing grout will rebreak unless they bring in someone to reinforce the floor.";
                            }
                            else if (serviceList.serviceIds[0].id == "Colorseal") {
                                serviceList.informationNote = "INSERT COLORSEAL BEFORE & AFTER";
                                serviceList.salesPersonNote = "Be Ready to discuss the root causes that lead to stained grout (failure of initial seal, and use of acidic cleaners that dissolve grout seals)";
                            }
                            else {
                                serviceList.salesPersonNote = "";
                                serviceList.informationNote = "";
                            }
                        }
                    }
                    else if (serviceList.serviceType == "CONCRETE-REPAIR") {
                        if (serviceList.typeOfStoneType2 == "CONCRETE" || serviceList.typeOfStoneType2 == "TERRAZZO") {
                            if (serviceList.serviceIds[0].id == "Concrete Chip Repair") {
                                serviceList.informationNote = "INSERT CHIP REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE";
                            }
                            else if (serviceList.serviceIds[0].id == "Concrete Crack Repair") {
                                serviceList.salesPersonNote = "Remember that while a chip and crack repair will prevent water and dirt from collecting in the open area, the resulting repair is still an eyesore and will need an opaque coating to restore a clean uniform finish to the surface";
                                serviceList.informationNote = "INSERT CRACK REPAIR BEFORE-AND-AFTER EXAMPLE IMAGE";
                            }
                            else if (serviceList.serviceIds[0].id == "Large Concrete Area Repairs") {
                                serviceList.salesPersonNote = "Concrete is used to establish a base to within 1/2\" of desired surface so as to reduce the amount of terrazzo needed AND to reduce chance of pebbles settling below the surface prior to grinding. Surface should be overpacked to prevent an overcream presence";
                            }
                            else if (serviceList.serviceIds[0].id == "Terrazzo Chip Repair" || serviceList.serviceIds[0].id == "Terrazzo Crack Repair" || serviceList.serviceIds[0].id == "Terrazzo Large Area Repair") {
                                serviceList.salesPersonNote = "Be advised, that even a small terrazzo repair will run $700, as the terrazzo suppliers will not supply chips in less than 25 Lb quantities and resin in less than 5-Gal pails. Reach out to MARBLELIFE R&D for guidance on how to make terrazzo samples for your client to sign-off on the matrix, chip sizes, chip colors and chip variations.";
                            }
                            else if (serviceList.serviceIds[0].id == "Stripping Carpet Adhesive" || serviceList.serviceIds[0].id == "Stripping-Silicone" || serviceList.serviceIds[0].id == "Stripping-Urethane" || serviceList.serviceIds[0].id == "Stripping-Wax" || serviceList.serviceIds[0].id == "Stripping-Unknown") {
                                serviceList.salesPersonNote = "Recommend this be on a separate invoice from rest of project in order to allow client to source independently, as many do not understand that unlike a janitorial wax stripping where they need to get most of it off, we need to get every corner and grout line.";
                            }
                            else {
                                serviceList.salesPersonNote = "";
                                serviceList.informationNote = "";
                            }
                        }
                    }
                    else if (serviceList.serviceType == "CONCRETE-COUNTERTOPS") {
                        if (serviceList.serviceIds[0].id == "Marblized Top Coat Installation") {
                            serviceList.salesPersonNote = "It is imperative the counter be protected from touching for 24 hours while it flows and settles.  Alert client that if the surface is touched by person or pet additional charges would apply as the surface must be removed and re-applied.";
                        }
                        else {
                            serviceList.salesPersonNote = "";
                            serviceList.informationNote = "";
                        }
                    }
                    else if (serviceList.serviceType == "CONCRETE-COATINGS") {
                        if (serviceList.serviceIds[0].id == "Moisture Vapor Barrier") {
                            serviceList.salesPersonNote = "This should be required in FL and TX and elsewhere where we have high water tables reported.  This is generally a greater need in vallies than on top of a hillside.  See the water table map insert to show your client as needed.";
                        }
                        else if (serviceList.serviceIds[0].id == "Epoxy Base") {
                            serviceList.informationNote = "INSERT COATING COLOR CHART";
                        }
                        else if (serviceList.serviceIds[0].id == "Polyspartic Base") {
                            serviceList.informationNote = "INSERT COATING COLOR CHART";
                        }
                        else if (serviceList.serviceIds[0].id == "Vinyl Chips") {
                            serviceList.informationNote = "INSERT CHIP COLOR CHART";
                        }
                        else if (serviceList.serviceIds[0].id == "Epoxy Top Coat") {
                            serviceList.informationNote = "INSERT CHIP COLOR CHART";
                        }
                        else if (serviceList.serviceIds[0].id == "Polyspartic Top Coat") {
                            serviceList.informationNote = "INSERT COATING COLOR CHART";
                        }
                        else if (serviceList.serviceIds[0].id == "Endurachip - Light") {
                            serviceList.salesPersonNote = "This is a light chip one coat system designed to be a low cost coating - often used in a FOR SALE situation when owner is seeking to cover cracks, pits and stains and clean-up appearance.  Fast ";
                        }
                        else if (serviceList.serviceIds[0].id == "Endurachip - Fast") {
                            serviceList.salesPersonNote = "This is a 1-2 day process designed to allow minimum loss of garage time.";
                        }
                        else if (serviceList.serviceIds[0].id == "Endurachip - Durable") {
                            serviceList.salesPersonNote = "This is a 2-3 day process designed to provide the best durability coating";
                        }
                        else if (serviceList.serviceIds[0].id == "Marblized Coating") {
                            serviceList.informationNote = "Link to web gallery";
                        }
                        else if (serviceList.serviceIds[0].id == "Anti-Spauling Driveway Sealing" || serviceList.serviceIds[0].id == "Anti-Spauling Sideway Sealing") {
                            serviceList.informationNote = "Insert images";
                        }
                        else {
                            serviceList.salesPersonNote = "";
                            serviceList.informationNote = "";
                        }
                    }
                    else if (serviceList.serviceType == "CLEANAIR") {
                        if (serviceList.serviceIds[0].id == "Clean Air Purification Treatments") {
                            serviceList.salesPersonNote = "ISSUE - Presently this technology works well, but we do NOT have a test that allows us to confirm its presence.  This service will go live once a test has been found.";
                        }
                        else {
                            serviceList.salesPersonNote = "";
                            serviceList.informationNote = "";
                        }
                    }
                    else if (serviceList.serviceType == "TILEINSTALL") {
                        if (serviceList.serviceIds[0].id == "Grouting") {
                            serviceList.salesPersonNote = "Recommend the client select a standard gray grout and then to seal with a colorizeablle Colorseal as this allows them to confirm everything is indeed seal, and expands the cleaners they can use without damaging their grout.";
                        }
                        else if (serviceList.serviceIds[0].id == "Grout Sealing - Clear") {
                            serviceList.salesPersonNote = "Ceramic and Porcelein do not need sealing.";
                        }
                        else {
                            serviceList.salesPersonNote = "";
                            serviceList.informationNote = "";
                        }
                    }
                    else if (serviceList.serviceType == "VINYLGUARD") {
                        if (serviceList.serviceIds[0].id == "Cleaning and Pre-Coat Preparations") {
                            serviceList.salesPersonNote = "This is where your time is as every corner and edge has to be removed of dirt, dust and hair. Not a bad idea to have team wear a hair net and booties during this process.";
                        }
                        else if (serviceList.serviceIds[0].id == "VinylGuard Installation") {
                            serviceList.salesPersonNote = "Vinylguard does not need to be stripped, and is durable enough that it can go a year without needed attention, unlike wax which scuffs, must be buffed, and periodically tripped. Keep in mind this is a labor savings, which is only realized if they can reduce staff or have the staff do more of something else.";
                        }
                        else {
                            serviceList.salesPersonNote = "";
                            serviceList.informationNote = "";
                        }
                    }
                    else if (serviceList.serviceType == "WOODLIFE") {
                        if (serviceList.serviceIds[0].id == "Cleaning") {
                            serviceList.salesPersonNote = "Wood cleaners contain waxes that build up over time and dull the surface, cleaning in this case means removing these residual waxes.";
                        }
                        else {
                            serviceList.salesPersonNote = "";
                            serviceList.informationNote = "";
                        }
                    }
                    else if (serviceList.serviceType == "SALESTAX") {
                        if (serviceList.serviceIds[0].id == "Service") {
                            serviceList.salesPersonNote = "Most states do not require tax on services.";
                        }
                        else {
                            serviceList.salesPersonNote = "";
                            serviceList.informationNote = "";
                        }
                    }
                    else {
                        serviceList.informationNote = "";
                        serviceList.salesPersonNote = "";
                    }
                }

                function calculateSumTotalArea(value, index) {
                    value.sumTotalArea = 0;
                    angular.forEach(value.measurements, function (value1, index1) {
                        if (value1.area != '' || value1.area != null) {
                            value.sumTotalArea += value1.area;
                        }
                    });
                }

                function calculatingDataByInvoiceNumber() {
                    vm.estimateInvoiceInfo.price = 0;

                    angular.forEach(vm.estimateInvoiceInfo.serviceList, function (value1) {
                        if (!value1.isCross) {

                            if (value1.option1 == '' || value1.option1 == undefined) {
                                value1.option1 = 0;
                            }

                            if (value1.option2 == '' || value1.option2 == undefined) {
                                value1.option2 = 0;
                            }

                            if (value1.option3 == '' || value1.option3 == undefined) {
                                value1.option3 = 0;
                            }

                            if (vm.isOption1Checked) {
                                vm.estimateInvoiceInfo.price += parseFloat(value1.option1);
                            }
                            else if (vm.isOption2Checked) {
                                vm.estimateInvoiceInfo.price += parseFloat(value1.option2);
                            }
                            else if (vm.isOption3Checked) {
                                vm.estimateInvoiceInfo.price += parseFloat(value1.option3);
                            }
                        }
                        angular.forEach(value1.subItem, function (value) {
                            if (!value.isCross) {
                                if (vm.isOption1Checked) {
                                    vm.estimateInvoiceInfo.price += parseFloat(value.option1);
                                }
                                else if (vm.isOption2Checked) {
                                    vm.estimateInvoiceInfo.price += parseFloat(value.option2);
                                }
                                else if (vm.isOption3Checked) {
                                    vm.estimateInvoiceInfo.price += parseFloat(value.option3);
                                }
                            }
                        });
                        if (isNaN(vm.estimateInvoiceInfo.price)) {
                            vm.estimateInvoiceInfo.price = 0;
                        }
                        vm.estimateInvoiceInfo.lessDeposit = (vm.estimateInvoiceInfo.lessDepositPercentage * vm.estimateInvoiceInfo.price) / 100;
                        vm.estimateInvoiceInfo.balance = vm.estimateInvoiceInfo.price - vm.estimateInvoiceInfo.lessDeposit;
                    });


                }

                function checkBoxClicked(option) {
                    if (option == 'option1') {
                        vm.isOption1Checked = true;
                        vm.isOption2Checked = false;
                        vm.isOption3Checked = false;

                        vm.estimateInvoiceInfo.option = 'option1';
                        var option1 = document.getElementById("option1");
                        if (option1 != null) {
                            option1.checked = true;
                        }
                        var option2 = document.getElementById("option2");
                        if (option2 != null) {
                            option2.checked = true;
                        }
                        var option3 = document.getElementById("option3");
                        if (option3 != null) {
                            option3.checked = false;
                        }
                    }
                    else if (option == 'option2') {
                        vm.isOption1Checked = false;
                        vm.isOption2Checked = true;
                        vm.isOption3Checked = false;

                        vm.estimateInvoiceInfo.option = 'option2';
                        var option1 = document.getElementById("option1");
                        if (option1 != null) {
                            option1.checked = false;
                        }
                        var option2 = document.getElementById("option2");
                        if (option2 != null) {
                            option2.checked = true;
                        }
                        var option3 = document.getElementById("option3");
                        if (option3 != null) {
                            option3.checked = false;
                        }
                    }
                    else if (option == 'option3') {

                        vm.isOption1Checked = false;
                        vm.isOption2Checked = false;
                        vm.isOption3Checked = true;

                        vm.estimateInvoiceInfo.option = 'option3';
                        var option1 = document.getElementById("option1");
                        if (option1 != null) {
                            option1.checked = true;
                        }
                        var option2 = document.getElementById("option2");
                        if (option2 != null) {
                            option2.checked = true;
                        }
                        var option3 = document.getElementById("option3");
                        if (option3 != null) {
                            option3.checked = true;
                        }
                    }
                    calculatingDataByInvoiceNumber();
                    optionWiseTotal();
                }

                function optionWiseTotal() {
                    vm.totalSumList = [];
                    vm.totalNumberOfInvoices = [];
                    for (var value = 1; value <= vm.estimateInvoiceInfo.invoiceCount; value++) {
                        vm.totalNumberOfInvoices.push(value);
                        var option1 = 0;
                        vm.lists = $filter('filter')(vm.estimateInvoiceInfo.serviceList, { invoiceNumber: value.toString() }, true);

                        angular.forEach(vm.lists, function (value1) {
                            if (!value1.isCross) {
                                if (vm.estimateInvoiceInfo.option == 'option1') {
                                    option1 = option1 + value1.option1;
                                }
                                else if (vm.estimateInvoiceInfo.option == 'option2') {
                                    option1 = option1 + value1.option2;
                                }
                                else if (vm.estimateInvoiceInfo.option == 'option3') {
                                    option1 = option1 + value1.option3;
                                }
                            }
                            angular.forEach(value1.subItem, function (value) {
                                if (!value.isCross) {
                                    if (vm.estimateInvoiceInfo.option == 'option1') {
                                        option1 = option1 + parseFloat(value.option1);
                                    }
                                    else if (vm.estimateInvoiceInfo.option == 'option2') {
                                        option1 = option1 + parseFloat(value.option2);
                                    }
                                    else if (vm.estimateInvoiceInfo.option == 'option3') {
                                        option1 = option1 + parseFloat(value.option3);
                                    }
                                }
                            });
                        });
                        vm.totalSumList.push(option1);
                    }
                }

                function totalSumForOptions() {
                    vm.option1 = 0;
                    vm.option2 = 0;
                    vm.option3 = 0;
                    angular.forEach(vm.estimateInvoiceInfo.serviceList, function (value1) {
                        if (!value1.isCross) {
                            if (value1.option1 != undefined && value1.option1 != '') {
                                vm.option1 = vm.option1 + parseFloat(value1.option1);
                            }
                            else {
                                vm.option1 = 0;
                            }
                            if (value1.option2 != undefined && value1.option2 != '') {
                                vm.option2 = vm.option2 + parseFloat(value1.option2);
                            }
                            else {
                                vm.option2 = 0;
                            }
                            if (value1.option3 != undefined && value1.option3 != '') {
                                vm.option3 = vm.option3 + parseFloat(value1.option3);
                            }
                            else {
                                vm.option3 = 0;
                            }

                        }
                        angular.forEach(value1.subItem, function (value) {
                            if (!value.isCross) {
                                if (value.option1 != undefined && value.option1 != '') {
                                    vm.option1 = vm.option1 + parseFloat(value.option1);
                                }

                                if (value.option2 != undefined && value.option2 != '') {
                                    vm.option2 = vm.option2 + parseFloat(value.option2);
                                }

                                if (value.option3 != undefined && value.option3 != '') {
                                    vm.option3 = vm.option3 + parseFloat(value.option3);
                                }
                            }
                        })
                    });

                    optionWiseTotal();

                }

                function addingNotes(serviceList, isEditable, from) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/modal.add.notes.client.view.html',
                        controller: 'ModalAddNotesController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Id: vm.estimateId,
                                    ServiceList: serviceList,
                                    IsEditable: false,
                                    From: from
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                    });
                }

                function changeInNumberOfInvoices() {
                    if (vm.estimateInvoiceInfo.invoiceCount < 1 || vm.estimateInvoiceInfo.invoiceCount == '' || vm.estimateInvoiceInfo.invoiceCount == undefined) {
                        //toaster.error('Invoice Count Cannot be less than 1');
                        return;
                    }
                    vm.invoiceNumberForInvoice = [];
                    for (var i = 1; i <= vm.estimateInvoiceInfo.invoiceCount; i++) {
                        vm.invoiceNumberForInvoice.push({ display: i.toString(), value: i.toString() })
                    }
                    angular.forEach(vm.estimateInvoiceInfo.serviceList, function (value1) {
                        if (value1.invoiceNumber == "" || value1.invoiceNumber == undefined) {
                            value1.invoiceNumber = "1";
                        }
                    });
                    var index = 1;
                    vm.currentInvoice = [];
                    angular.forEach(vm.invoiceNumberForInvoice, function (value1, index) {
                        var currentIndex = parseInt(value1.value);
                        if (vm.estimateInvoiceInfo.invoiceNotesList[index] != undefined) {
                            vm.estimateInvoiceInfo.invoiceNotesList[index].invoiceNumber = index + 1;
                            vm.currentInvoice.push(vm.estimateInvoiceInfo.invoiceNotesList[index]);
                        }
                        else {
                            vm.currentInvoice.push({ id: 0, notes: '', invoiceNumber: index + 1 });
                        }

                    });
                    vm.estimateInvoiceInfo.invoiceNotesList = vm.currentInvoice;
                }

                function openNotesModal(modalName, content) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/modal.notes.client.view.html',
                        controller: 'NotesModal',
                        controllerAs: 'vm',
                        size: 'm',
                        resolve: {
                            modalParam: function () {
                                return {
                                    ModalName: modalName,
                                    Content: content
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                    });
                }

                function sendMailToCustomer() {
                    vm.openSelectInvoicesModal("Send Email to Customer", "Send Mail", true, false);
                }

                function openSelectInvoicesModal(modalName, buttonName, isFromMailSend, isFromAvailability) {
                    vm.serviceListSignedInvoices = [];
                    angular.forEach(vm.jobInfo.allInvoiceNumbersSignedPre, function (signedInvocieNumber) {
                        var signedInvoicesService = $filter('filter')(vm.estimateInvoiceInfo.serviceList, { invoiceNumber: signedInvocieNumber.toString() }, true);

                        angular.forEach(signedInvoicesService, function (signedInvoices) {
                            vm.serviceListSignedInvoices.push(signedInvoices);
                        });
                    });
                    if (vm.jobInfo.allInvoiceNumbersSignedPre.length != 0) {
                        vm.estimateInvoiceInfo.serviceList = vm.serviceListSignedInvoices;
                    }

                    var personId = vm.jobInfo.jobAssigneeIds[0];
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/modal.selectinvoices.client.view.html',
                        controller: 'SelectInvoicesModal',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    CustomerName: vm.jobInfo.jobCustomer.customerName,
                                    SchedulerId: vm.jobInfo.estimateSchedulerId,
                                    Services: vm.estimateInvoiceInfo.serviceList,
                                    EstimateDate: vm.jobInfo.startDate,
                                    ButtonName: buttonName,
                                    ModalName: modalName,
                                    IsFromMailSend: isFromMailSend,
                                    MailBody: !isFromAvailability ? vm.jobInfo.mailBody : vm.jobInfo.mailBodyToAdmin,
                                    Title: vm.estimateInvoiceInfo.title,
                                    IsFromJob: true,
                                    ToEmail: vm.estimateInvoiceInfo.email,
                                    CcEmail: vm.estimateInvoiceInfo.ccEmail,
                                    TypeId: DataHelper.CustomerSignatureType.AFTERCOMPLETITION,
                                    IsFromAvailability: isFromAvailability,
                                    EstimateInvoiceId: vm.estimateInvoiceInfo.id,
                                    PersonId: personId,
                                    JobSchedulerId: vm.jobInfo.id,
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                        getEstimateInvoiceInfo();
                    });
                }
                function openSignatureModal() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/modal.esignature.client.view.html',
                        controller: 'ESignatureModal',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    CustomerName: vm.jobInfo.jobCustomer.customerName,
                                    SchedulerId: vm.jobInfo.estimateSchedulerId,
                                    EstimateInvoiceId: vm.estimateInvoiceInfo.id,
                                    CustomerId: vm.jobInfo.jobCustomer.customerId,
                                    JobSignature: angular.copy(vm.jobInfo.jobSignaturePost),
                                    AllInvoiceNumbersSigned: vm.jobInfo.allInvoiceNumbersSignedPost,
                                    Services: angular.copy(vm.estimateInvoiceInfo.serviceList),
                                    EstimateDate: vm.jobInfo.startDate,
                                    IsFromJob: true,
                                    JobOrginialSchedulerId: vm.rowId,
                                    InvoiceNumberSignedForEstimate: vm.jobInfo.allInvoiceNumbersSignedForEstimate,
                                    JobSchedulerId: vm.jobInfo.id,
                                    TypeId: 290
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                        //getJobInfo();
                        //getInvoiceInfo();
                        getEstimateInvoiceInfo();
                    });
                }

                function saveCustomerInvoice(isFromInvoice, isFromDownload) {
                    openSelectInvoicesModal("Download Customer Invoice(s)", "Download Customer Invoice(s)", false, false);
                }

                function changeAvailability(estimateInvoice) {
                    openSelectInvoicesModal("Send Email to Customer", "Send Mail To Customer", true, true);
                }
                function openPopUp(img, isFromBefore) {
                    vm.isPopUpOpened = true;
                    img.isSelected = !img.isSelected;
                    vm.imageModel = {};
                    if (isFromBefore) {
                        if (img.beforeImages.imagesInfo[0].isImageCropped) {
                            fileService.getFileStreamByUrl(img.beforeImages.imagesInfo[0].croppedImageUrl).then(function (result) {
                                vm.imageModel.url = fileService.getStreamUrl(result);
                                img.beforeImages.imagesInfo[0].url = vm.imageModel.url;
                            });
                        }
                        else {
                            if (img.beforeImages.imagesInfo[0].url != null) {
                                vm.imageModel.url = img.beforeImages.imagesInfo[0].url;
                            }
                            else {
                                fileService.getFileStreamByUrl(img.beforeImages.imagesInfo[0].imageUrl).then(function (result) {
                                    vm.imageModel.url = fileService.getStreamUrl(result);
                                    img.beforeImages.imagesInfo[0].url = vm.imageModel.url;
                                });
                            }
                        }
                    }
                    if (!isFromBefore) {
                        if (img.afterImages.imagesInfo[0].isImageCropped) {
                            fileService.getFileStreamByUrl(img.afterImages.imagesInfo[0].croppedImageUrl).then(function (result) {
                                vm.imageModel.url = fileService.getStreamUrl(result);
                                img.afterImages.imagesInfo[0].url = vm.imageModel.url;
                            });
                        }
                        else {
                            if (img.afterImages.imagesInfo[0].url != null) {
                                vm.imageModel.url = img.afterImages.imagesInfo[0].url;
                            }
                            else {
                                fileService.getFileStreamByUrl(img.afterImages.imagesInfo[0].imageUrl).then(function (result) {
                                    vm.imageModel.url = fileService.getStreamUrl(result);
                                    img.afterImages.imagesInfo[0].url = vm.imageModel.url;
                                });
                            }
                        }
                    }
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/pop-up-image.client.view.html',
                        controller: 'PopUpImageController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Image: vm.imageModel,
                                    isFromReview: false,
                                    text: 'Before After Images'
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                        //getJobInfo();
                    });
                }


                function cropImage(img, isFromBefore) {

                    vm.isPopUpOpened = true;
                    img.isSelected = !img.isSelected;
                    vm.imageModel = {};
                    if (isFromBefore) {
                        if (img.beforeImages.imagesInfo[0].s3BucketImageUrl != null) {
                            vm.imageModel.url = img.beforeImages.imagesInfo[0].s3BucketImageUrl;
                            vm.imageModel.beforeAfterId = img.beforeImages.imagesInfo[0].beforeAfterId;
                            vm.imageModel.fileId = img.beforeImages.imagesInfo[0].fileId;
                        }
                        else {
                            if (img.beforeImages.imagesInfo[0].url != null) {
                                vm.imageModel.url = img.beforeImages.imagesInfo[0].url;
                                vm.imageModel.beforeAfterId = img.beforeImages.imagesInfo[0].beforeAfterId;
                                vm.imageModel.fileId = img.beforeImages.imagesInfo[0].fileId;
                            }
                            else {
                                if (img.beforeImages.imagesInfo[0].s3BucketImageUrl != null) {
                                    vm.imageModel.url = img.beforeImages.imagesInfo[0].s3BucketImageUrl;
                                    vm.imageModel.beforeAfterId = img.beforeImages.imagesInfo[0].beforeAfterId;
                                    vm.imageModel.fileId = img.beforeImages.imagesInfo[0].fileId;
                                }
                                else {
                                    fileService.getFileStreamByUrl(img.beforeImages.imagesInfo[0].imageUrl).then(function (result) {
                                        vm.imageModel.url = fileService.getStreamUrl(result);
                                        vm.imageModel.beforeAfterId = img.beforeImages.imagesInfo[0].beforeAfterId;
                                        vm.imageModel.fileId = img.beforeImages.imagesInfo[0].fileId;
                                        img.beforeImages.imagesInfo[0].url = vm.imageModel.url;
                                    });
                                }
                            }
                        }
                    }
                    if (!isFromBefore) {
                        if (img.afterImages.imagesInfo[0].s3BucketImageUrl != null) {
                            vm.imageModel.url = img.afterImages.imagesInfo[0].s3BucketImageUrl;
                            vm.imageModel.beforeAfterId = img.afterImages.imagesInfo[0].beforeAfterId;
                            vm.imageModel.fileId = img.afterImages.imagesInfo[0].fileId;
                        }
                        else {
                            if (img.afterImages.imagesInfo[0].url != null) {
                                vm.imageModel.url = img.afterImages.imagesInfo[0].url;
                                vm.imageModel.beforeAfterId = img.afterImages.imagesInfo[0].beforeAfterId;
                                vm.imageModel.fileId = img.afterImages.imagesInfo[0].fileId;
                            }
                            else {
                                fileService.getFileStreamByUrl(img.afterImages.imagesInfo[0].imageUrl).then(function (result) {
                                    vm.imageModel.url = fileService.getStreamUrl(result);
                                    vm.imageModel.beforeAfterId = img.afterImages.imagesInfo[0].beforeAfterId;
                                    vm.imageModel.fileId = img.afterImages.imagesInfo[0].fileId;
                                    img.afterImages.imagesInfo[0].url = vm.imageModel.url;
                                });
                            }
                        }
                    }

                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/crop-image.client.view.html',
                        controller: 'CropImageController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Image: vm.imageModel,
                                    isFromReview: false,
                                    text: 'Crop Image'
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                        getJobInfo();
                    });
                }

                function getAllServicesCollection() {
                    return franchiseeService.getAllServicesListForDropdown().then(function (result) {
                        vm.serviceTypeWithOther = result.data.slice();
                        vm.serviceTypeWithoutOther = result.data.slice();
                        vm.serviceType = vm.serviceTypeWithOther;
                        vm.serviceTypeWithoutOther.pop(); // removes Other
                        vm.serviceTypeForInvoices = vm.serviceTypeWithoutOther;
                    });
                }

                $scope.$emit("update-title", "");

                $q.all([getListForBeforeAfterDropDown(), getJobInfo(), getJobStatus(), getmarketingClassCollection(), getServicesCollection(), getFollowUp(), getAllServicesCollection()]);
            }]);
}());