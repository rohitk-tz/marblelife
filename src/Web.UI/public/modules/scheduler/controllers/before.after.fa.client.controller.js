(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("BeforeAfterForFAController",
        ["$scope", "$rootScope", "$state", "$q", "EstimateService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "AddressService", "SchedulerService", "$filter", "CustomerService",
            "APP_CONFIG", "GeoCodeService", "$uibModal", "BeforeAfterService", "FileService", "$window", "$timeout", "$location",
            function ($scope, $rootScope, $state, $q, estimateService, franchiseeService,
                notification, clock, toaster, addressService, schedulerService, $filter, customerService, config, geoCodeService,
                $uibModal, beforeAfterService, fileService, $window, $timeout, $location) {
                var vm = this;
                vm.isDataPresent = true;
                $scope.isOpen = false;
                vm.Roles = DataHelper.Role;
                vm.franchiseeChange = franchiseeChange;
                vm.isTech = $rootScope.identity.roleId == vm.Roles.Technician;
                vm.isSalesRep = $rootScope.identity.roleId == vm.Roles.SalesRep;
                vm.isOpsMgr = $rootScope.identity.roleId == vm.Roles.OperationsManager;
                vm.franchiseeName = null;
                vm.close = close;
                vm.naviage = naviage;
                vm.imagesrc = "/Content/images/no_image_thumb.gif";
                vm.isSurfaceType = false;
                vm.isSurfaceMaterial = false;
                vm.isServiceType = false;
                vm.isSurfaceColor = false;
                vm.isBuildingLocation = false;
                vm.isBuildingType = false;
                vm.isManagementCompany = false;
                vm.isMaidService = false;
                vm.isFinishMaterial = false;
                vm.isMarketingClassId = false;
                vm.getServicesCollection = getServicesCollection;
                vm.openPopUp = openPopUp;
                vm.isPopUpOpened = false;
                vm.isFromLoad = true;
                vm.resetSearch = resetSearch;
                vm.reviewMarked = reviewMarked;
                vm.refresh = refresh;
                vm.getUserList = getUserList;
                vm.isTodayDisable = true;
                vm.currentWeek = currentWeek;
                vm.prev = prev;
                vm.next = next;
                vm.bestPictureSelected = bestPictureSelected;
                vm.getRelativeLocation = getRelativeLocation;
                vm.selectChange = selectChange;
                vm.changeClass = changeClass;
                vm.isLoaded = false;
                vm.count = 0;
                vm.localMarketingReviewList = [];
                var lastScrollTop = 0;
                vm.searchOptions = [];
                vm.beforeAfterReviewImages = [];

                vm.getLocalMarketingReview = getLocalMarketingReview;
                vm.localMarketingReviewList = [];
                vm.marketingReviewStartDate = "";
                vm.marketingReviewEndtDate = "";
                vm.isFranchiseeAdmin = false;
                vm.getDateFilterLocalMarketingReview = getDateFilterLocalMarketingReview;
                vm.resetSearchOption = resetSearchOption;
                vm.resetInnerSearchOption = resetInnerSearchOption;
                vm.getFilteredLocalMarketingReview = getFilteredLocalMarketingReview;
                vm.markImageAsReviwed = markImageAsReviwed;
                vm.addToLocalSiteGallery = addToLocalSiteGallery;

                vm.surfaceMaterial = [{ display: "Stonelife:marble", value: "Stonelife:marble" }, { display: "stonelife:granite", value: "stonelife:granite" }, { display: "groutlife:ceramic (tile and grout)", value: "groutlife:ceramic (tile and grout)" },
                { display: "stonelife:travertine", value: "stonelife:travertine" }, { display: "Enduracrete:terrazzo", value: "Enduracrete:terrazzo" }, { display: "groutlife:porcelain", value: "groutlife:porcelain" }
                    , { display: "vinylguard:vinyl", value: "vinylguard:vinyl" }, { display: "wood", value: "wood" }, { display: "Enduracrete:concrete", value: "Enduracrete:concrete" }, { display: "Other", value: "Other" }];

                vm.surfaceType = [{ display: "Floor", value: "Floor" }, { display: "Counter (Kitchen)", value: "Counter (Kitchen)" }, { display: "Wall", value: "Wall" }, { display: "Vanity (Bathroom)", value: "Vanity (Bathroom)" }
                    , { display: "Sign", value: "Sign" }, { display: "Building Exterior", value: "Building Exterior" }, { display: "Pool Deck", value: "Pool Deck" },
                { display: "Patio", value: "Patio" }, { display: "Walkway", value: "Walkway" }, { display: "Other", value: "Other" }, { display: "Fireplace", value: "Fireplace" }];

                vm.buildingLocation = [{ display: "Lobby", value: "Lobby" }, { display: "Garage", value: "Garage" },
                                       { display: "Basement", value: "Basement" }, { display: "Foyer", value: "Foyer" }, 
                                       { display: "Bathroom", value: "Bathroom" }, { display: "Kitchen", value: "Kitchen" },
                                       { display: "Bedroom", value: "Bedroom" }, { display: "Living", value: "Living" }, 
                                       { display: "Dining", value: "Dining" }, { display: "Hall", value: "Hall" }, 
                                       { display: "Elevator", value: "Elevator" }, { display: "Floor", value: "Floor" },
                                       { display: "Exterior Feature", value: "Exterior Feature" }, { display: "Baseboards", value: "Baseboards" },
                                       { display: "Laundry", value: "Laundry" }];


                vm.surfaceColor = [{ display: "White (Carrerra)", value: "White (Carrerra)" }, { display: "Green (Verde)", value: "Green" }, { display: "Black", value: "Black" }, { display: "Brown", value: "Brown" }
                    , { display: "Tan", value: "Tan" }, { display: "Red ", value: "Red " }, { display: "Other", value: "Other" }];

                vm.finishMaterial = [{ display: "Matte", value: "Matte" }, { display: "Satin", value: "Satin" }, { display: "Semi-Gloss", value: "Semi-Gloss" }, { display: "Gloss", value: "Gloss" }, { display: "Ultimate Finish", value: "Ultimate Finish" }];

                vm.reviewImageList = [{ display: "Reviewed", value: 1 }, { display: "Not Reviewed", value: "2" }];
                vm.query = {
                    userId: 0,
                    statusId: '',
                    startDate: null,
                    endDate: null,
                    pageNumber: 1,
                    text: '',
                    pageSize: 3,
                    ServiceTypeId: 0,
                    surfaceType: '',
                    surfaceMaterial: '',
                    surfaceColor: '',
                    finishType: '',
                    buildingType: '',
                    managementCompany: '',
                    maidService: '',
                    finishMaterial: '',
                    franchiseeId: 0,
                    buildingLocation: '',
                    marketingClassId: 0
                };
                vm.LocalMarketingQuery = {
                    userId: null,
                    startDate: null,
                    endDate: null,
                    franchiseeId: null,
                    surfaceType: '',
                    surfaceMaterial: '',
                    surfaceColor: '',
                    finishMaterial: '',
                    buildingLocation: '',
                    surfaceTypeId: '',
                    serviceTypeId: '',
                    isImagePairReviewed: 0,
                    isDateFilter: false,
                    isFilter: false,
                    marketingClassId: 0
                    //pageNumber: 1,
                    //text: '',
                    //pageSize: 3
                };

                vm.saveQuery = {
                    beforeServiceId: '',
                    afterServiceId: '',
                    isAddToGalary: false,
                    isSelected: false
                };

                vm.reviewMarkedQuery = {
                    startDate: '',
                    endate: '',
                    isReview: false
                };

                vm.saveImagesBestPair = saveImagesBestPair;
                vm.scrollClick = 1;
                vm.startReview = startReview;
                vm.getImagesList = getImagesList;
                vm.isPreviewSlide = false;
                $scope.myInterval = 50000;
                $scope.active = 0;
                var slides = $scope.slides = [];
                var currIndex = 0;
                vm.sliderList = [];
                $scope.noWrapSlides = false;

                function startReview() {
                    vm.isPreviewSlide = !vm.isPreviewSlide;
                }



                function getImagesList() {
                    //vm.franchiseeName = null;
                    //vm.isLoaded = false;
                    //vm.query.scrollClick = vm.scrollClick;
                    //vm.query.pageNumber = 1;
                    //return beforeAfterService.getReviewImagesForFA(vm.query).then(function (result) {
                    //    vm.beforeAfterReviewImages = result.data.beforeAfterPersonViewModel;
                    //    vm.customerImageViewModel = result.data.customerImageViewModel;
                    //    vm.franchiseeName = result.data.frachiseeName;
                    //    vm.query.franchiseeId = result.data.frachiseeId.toString();
                    //    getUserList();
                    //    vm.count = result.data.totalCount;
                    //    vm.startDate = result.data.startDate;
                    //    vm.endDate = result.data.endDate;

                    //    angular.forEach(vm.beforeAfterReviewImages, function (value1, index) {
                    //        angular.forEach(value1.residentalCollection, function (value, index) {

                    //            angular.forEach(value1.customerImageViewModel, function (value3, index) {
                    //                angular.forEach(value3.customerForImageModel, function (value2, index) {

                    //                    if (value2.s3BucketAfterImageUrlThumb != null) {
                    //                        value2.afterThumbUrl = value2.s3BucketAfterImageUrlThumb;
                    //                    }
                    //                    else {
                    //                        if (value2.relactiveLocationAfterImageUrlThumb != "" && value2.relactiveLocationAfterImageUrlThumb != null && value2.relactiveLocationAfterImageUrlThumb != "/Content/images/no_image_thumb.gif") {
                    //                            fileService.getFileStreamByUrl(value2.relactiveLocationAfterImageUrlThumb).then(function (result) {
                    //                                value2.afterThumbUrl = fileService.getStreamUrl(result);
                    //                            });
                    //                        }
                    //                    }

                    //                    if (value2.s3BucketBeforeImageUrlThumb != null) {
                    //                        value2.beforeThumbUrl = value2.s3BucketBeforeImageUrlThumb;
                    //                    }
                    //                    else {

                    //                        if (value2.relactiveLocationBeforeImageUrlThumb != "" && value2.relactiveLocationBeforeImageUrlThumb != null && value2.relactiveLocationBeforeImageUrlThumb != "/Content/images/no_image_thumb.gif") {
                    //                            fileService.getFileStreamByUrl(value2.relactiveLocationBeforeImageUrlThumb).then(function (result) {
                    //                                value2.beforeThumbUrl = fileService.getStreamUrl(result);
                    //                            });
                    //                        }
                    //                    }

                    //                    if (value2.relactiveLocationAfterImageUrlThumb == "/Content/images/no_image_thumb.gif") {
                    //                        value2.afterThumbUrl = "/Content/images/no_image_thumb.gif";
                    //                    }
                    //                    if (value2.relactiveLocationBeforeImageUrlThumb == "/Content/images/no_image_thumb.gif") {
                    //                        value2.beforeThumbUrl = "/Content/images/no_image_thumb.gif";
                    //                    }
                    //                })
                    //            })

                    //        });



                    //        angular.forEach(value1.nonResidentalCollection, function (value, index) {
                    //            angular.forEach(value.beforeAfterViewModel, function (value2, index) {
                    //                if (value2.s3BucketAfterImageUrlThumb != null) {
                    //                    value2.afterThumbUrl = value2.s3BucketAfterImageUrlThumb;
                    //                }
                    //                else {
                    //                    if (value2.relactiveLocationAfterImageUrlThumb != "" && value2.relactiveLocationAfterImageUrlThumb != null && value2.relactiveLocationAfterImageUrlThumb != "/Content/images/no_image_thumb.gif") {
                    //                        fileService.getFileStreamByUrl(value2.relactiveLocationAfterImageUrlThumb).then(function (result) {
                    //                            value2.afterThumbUrl = fileService.getStreamUrl(result);
                    //                        });
                    //                    }
                    //                }
                    //                if (value2.relactiveLocationAfterImageUrlThumb != "/Content/images/no_image_thumb.gif") {
                    //                    value2.afterThumbUrl = "/Content/images/no_image_thumb.gif";
                    //                }

                    //                if (value2.s3BucketBeforeImageUrlThumb != null) {
                    //                    value2.beforeThumbUrl = value2.s3BucketBeforeImageUrlThumb;
                    //                }
                    //                else {
                    //                    if (value2.relactiveLocationBeforeImageUrlThumb == "" && value2.relactiveLocationBeforeImageUrlThumb == null) {
                    //                        fileService.getFileStreamByUrl(value2.relactiveLocationBeforeImageUrlThumb).then(function (result) {
                    //                            value2.beforeThumbUrl = fileService.getStreamUrl(result);
                    //                        });
                    //                    }

                    //                }
                    //                if (value2.relactiveLocationBeforeImageUrlThumb != "/Content/images/no_image_thumb.gif") {
                    //                    value2.afterThumbUrl = "/Content/images/no_image_thumb.gif";
                    //                }

                    //                if (value2.s3BucketBeforeImageUrlThumb != null) {
                    //                    value2.relactiveLocationExteriorImageUrl = value2.s3BucketExteriorImageUrlThumb;
                    //                }
                    //                else {
                    //                    if (value2.relactiveLocationExteriorImageUrl != "" && value2.relactiveLocationExteriorImageUrl != "/Content/images/no_image_thumb.gif") {
                    //                        fileService.getBeforeAfter(value2.exteriorImageFileId).then(function (result) {
                    //                            value2.relactiveLocationExteriorImage = fileService.getStreamUrl(result);
                    //                            value2.relactiveLocationExteriorImageUrl = value2.relactiveLocationExteriorImage;
                    //                            value.nonResidentalImageUrl = value2.relactiveLocationExteriorImage;
                    //                        });
                    //                    }
                    //                    else {
                    //                        value.nonResidentalImageUrl = '/Content/images/no_image_thumb.gif';
                    //                    }
                    //                }
                                    
                    //            });
                    //        });
                    //    });

                    //    $scope.$emit("update-title", "");

                    //});
                }

                function openPopUp(img) {
                    if (img.relactiveLocationBeforeImage == "") {
                        img.relactiveLocationBeforeImageUrl = img.emptyImage;
                    }
                    else {
                        fileService.getFileStreamByUrl(img.relactiveLocationBeforeImage).then(function (result) {
                            img.relactiveLocationBeforeImageUrl = fileService.getStreamUrl(result);
                        });
                    }

                    if (img.relactiveLocationAfterImage == "") {
                        img.relactiveLocationAfterImageUrl = img.emptyImage;
                    }
                    else {
                        fileService.getFileStreamByUrl(img.relactiveLocationAfterImage).then(function (result) {
                            img.relactiveLocationAfterImageUrl = fileService.getStreamUrl(result);
                        });
                    }

                    vm.isPopUpOpened = true;
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/scheduler/views/pop-up-image.client.view.html',
                        controller: 'PopUpImageController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    Image: img,
                                    isFromReview: true,
                                    text: 'Review Images'
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                        //getJobInfo();
                    });
                }


                function saveImagesBestPair(pairImg) {
                    if (vm.isPopUpOpened) {
                        pairImg.isSelected = !pairImg.isSelected;
                        vm.isPopUpOpened = false;
                        return;
                    }
                    pairImg.isSelected = !pairImg.isSelected;
                    vm.saveQuery.afterServiceId = pairImg.afterServiceId;
                    vm.saveQuery.beforeServiceId = pairImg.beforeServiceId;
                    return beforeAfterService.saveImagesBestPair(vm.saveQuery).then(function (result) {
                        if (result.data) {
                            if (pairImg.isSelected) {
                                toaster.show("Pair Selected for Local Gallery Images");
                            }
                            else {
                                toaster.show("Pair Deselected for Local Gallery Images");
                            }
                        }
                    });
                }

                function prepareSearchOptions() {
                    vm.searchOptions = []
                    vm.searchOptions.push(
                        { display: 'Marketing Class', value: '10' },
                        { display: 'User List', value: '11' })
                }


                function getServicesCollection() {
                    return franchiseeService.getServiceTypeCollection().then(function (result) {
                        vm.serviceType = result.data;

                    });
                }

                function resetSearch() {
                    vm.searchOption = '';
                    vm.query = {
                        userId: 0,
                        statusId: '',
                        startDate: null,
                        endDate: null,
                        pageNumber: 1,
                        text: '',
                        pageSize: 3,
                        ServiceTypeId: 0,
                        surfaceType: '',
                        surfaceMaterial: '',
                        surfaceColor: '',
                        finishType: '',
                        buildingType: '',
                        managementCompany: '',
                        maidService: '',
                        finishMaterial: '',
                        franchiseeId: 0,
                        buildingLocation: '',
                        marketingClassId: 0
                    };
                    vm.isSurfaceType = false;
                    vm.isSurfaceMaterial = false;
                    vm.isServiceType = false;
                    vm.isSurfaceColor = false;
                    vm.isBuildingLocation = false;
                    vm.isBuildingType = false;
                    vm.isManagementCompany = false;
                    vm.isMaidService = false;
                    vm.isFinishMaterial = false;
                    vm.isMarketingClassId = false;
                    $scope.$broadcast("reset-dates");
                    refresh();
                }

                function refresh() {
                    slides = $scope.slides = [];
                    getImagesList();
                    getFranchiseeCollectionForFA();
                    prepareSearchOptions();
                    getServicesCollection();
                    getUserList();
                }

                function getFranchiseeCollectionForFA() {
                    return franchiseeService.getFranchiseeNameValuePairByRoleForFA().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                //function prev() {
                //    slides = $scope.slides = [];
                //    vm.isTodayDisable = false;
                //    var startDate = new Date(vm.startDate);
                //    vm.query.startDate = new Date(startDate.setDate(startDate.getDate() - 7));

                //    var endDate = new Date(vm.endDate);
                //    vm.query.endDate = new Date(endDate.setDate(endDate.getDate() - 7));
                //    vm.isDataPresent = true;
                //    getImagesList();
                //}

                //function next() {
                //    slides = $scope.slides = [];
                //    vm.isTodayDisable = false;
                //    var startDate = new Date(vm.startDate);
                //    vm.query.startDate = new Date(startDate.setDate(startDate.getDate() + 7));

                //    var endDate = new Date(vm.endDate);
                //    vm.query.endDate = new Date(endDate.setDate(endDate.getDate() + 7));
                //    vm.isDataPresent = true;
                //    getImagesList();
                //}


                function changeClass() {
                    vm.reviewMarkedQuery.startDate = vm.startDate;
                    vm.reviewMarkedQuery.endDate = vm.endDate;
                    vm.reviewMarkedQuery.isReview = !vm.reviewMarkedQuery.isReview;
                    $scope.isReviewed = vm.reviewMarkedQuery.isReview;
                    reviewMarked();
                }
                function reviewMarked() {

                    return beforeAfterService.saveReviewMark(vm.reviewMarkedQuery).then(function (result) {
                        if (result.data) {
                            if (vm.reviewMarkedQuery.isReview) {
                                toaster.show("Review completed successfully");
                            }
                            else {
                                toaster.show("Review completed  not successfully");
                            }
                        }
                    });
                }

                function currentWeek() {
                    vm.query.startDate = vm.currentStartDate;
                    vm.query.endDate = vm.currentEndDate;
                    getImagesList();
                }
                function getmarketingClassCollection() {
                    return customerService.getmarketingClassCollection().then(function (result) {
                        vm.marketingClass = result.data;
                    });
                }

                //function bestPictureSelected(isAddToGalary, slide) {

                //    if (slide.beforeImageFileId == null || slide.afterImageFileId == null) {
                //        toaster.error("You can mark only if both images are uploaded!");
                //        slide.isBestPicture = false;
                //        slide.isAddToLocalGallery = false;
                //        return;
                //    }
                //    else {
                //        if (isAddToGalary) {
                //            vm.saveQuery.isAddToGalary = true;
                //            vm.saveQuery.beforeServiceId = slide.beforeServiceId;
                //            vm.saveQuery.afterServiceId = slide.afterServiceId;
                //            vm.saveQuery.isSelected = slide.isAddToLocalGallery;
                //        }
                //        else {
                //            vm.saveQuery.isAddToGalary = false;
                //            vm.saveQuery.beforeServiceId = slide.beforeServiceId;
                //            vm.saveQuery.afterServiceId = slide.afterServiceId;
                //            vm.saveQuery.isSelected = slide.isBestPicture;
                //        }
                //        return beforeAfterService.saveReviewImagesForFA(vm.saveQuery).then(function (result) {

                //        });
                //    }

                //}
                function getRelativeLocation(imageUrl, model) {

                    fileService.getFileStreamByUrl(imageUrl).then(function (result) {
                        $scope.afterImageUrl = fileService.getStreamUrl(result);
                        model.$scope.afterImageUrl;
                    });
                }
                //function getUserList() {
                //    return schedulerService.getUserListForFA(vm.LocalMarketingQuery.franchiseeId).then(function (result) {
                //        vm.techList = result.data;
                //    });
                //}
                function getUserList() {
                    return beforeAfterService.getSalesRepTechnicianList(vm.LocalMarketingQuery.franchiseeId).then(function (result) {
                        vm.techList = result.data;
                    });
                }


                function selectChange(optionId) {

                    if (optionId == '1') {
                        if (vm.isSurfaceType) {
                            vm.isSurfaceType = false;
                            vm.query.surfaceType = '';
                            getImagesList();
                            vm.searchOption = '';
                        }
                        else {
                            vm.isSurfaceType = true;

                        }
                    }
                    else if (optionId == '2') {
                        if (vm.isSurfaceMaterial) {
                            vm.isSurfaceMaterial = false;
                            vm.query.surfaceMaterial = '';
                            getImagesList();
                            vm.searchOption = '';
                        }
                        else {
                            vm.isSurfaceMaterial = true;
                        }
                    }
                    else if (optionId == '3') {
                        if (vm.isServiceType) {
                            vm.isServiceType = false;
                            vm.query.serviceType = '';
                            getImagesList();
                            vm.searchOption = '';
                        }
                        else {
                            vm.isServiceType = true;
                        }
                    }
                    else if (optionId == '4') {
                        if (vm.isSurfaceColor) {
                            vm.isSurfaceColor = false;
                            vm.query.surfaceColor = '';
                            getImagesList();
                            vm.searchOption = '';
                        }
                        else {
                            vm.isSurfaceColor = true;
                        }
                    }
                    else if (optionId == '5') {
                        if (vm.isBuildingLocation) {
                            vm.isBuildingLocation = false;
                            vm.query.buildingLocation = '';
                            getImagesList();
                            vm.searchOption = '';
                        }
                        else {
                            vm.isBuildingLocation = true;
                        }
                    }
                    else if (optionId == '7') {
                        if (vm.isManagementCompany) {
                            vm.isManagementCompany = false;
                            vm.query.managementCompany = '';
                            getImagesList();
                            vm.searchOption = '';
                        }
                        else {
                            vm.isManagementCompany = true;
                        }
                    }
                    else if (optionId == '8') {
                        if (vm.isMaidService) {
                            vm.isMaidService = false;
                            vm.query.maidService = '';
                            getImagesList();
                            vm.searchOption = '';
                        }
                        else {
                            vm.isMaidService = true;
                        }
                    }
                    else if (optionId == '9') {
                        if (vm.isFinishMaterial) {
                            vm.isFinishMaterial = false;
                            vm.query.finishMaterial = '';
                            getImagesList();
                            vm.searchOption = '';
                        }
                        else {
                            vm.isFinishMaterial = true;
                        }
                    }
                }

                $scope.status = {
                    isopen: false
                };

                $scope.toggled = function (open) {
                    $log.log('Dropdown is now: ', open);
                };

                $scope.toggleDropdown = function ($event) {
                    $event.preventDefault();
                    $event.stopPropagation();
                    $scope.status.isopen = !$scope.status.isopen;
                };

                $scope.appendToEl = angular.element(document.querySelector('#dropdown-long-content'));

                function franchiseeChange() {
                    slides = $scope.slides = [];
                    vm.query.pageNumber = 1;
                    vm.query.pageSize = 3;
                    getImagesList();
                    getFranchiseeCollectionForFA();
                    prepareSearchOptions();
                    getServicesCollection();
                    getUserList();
                }
                function close(event) {
                    $scope.isOpen = false;
                    console.log(event);
                }
                function naviage(url) {
                    $window.sessionStorage.setItem("IsBeforeAfterTabAtive", true);
                    window.open(url, '_blank');
                } $location
                angular.element($window).bind("scroll", function () {
                    var location = $location.absUrl();
                    var array = location.split('/');
                    var lastElement = array[array.length - 2];
                    if (lastElement != "beforeAfter") {
                        return;
                    }
                    var windowHeight = "innerHeight" in window ? window.innerHeight : document.documentElement.offsetHeight;
                    var body = document.body, html = document.documentElement;
                    var docHeight = Math.max(body.scrollHeight, body.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight);
                    var windowBottom = windowHeight + window.pageYOffset;
                    if (windowBottom >= docHeight - 5 && vm.isDataPresent) {
                        getImagesListAfterScrollEnd();
                    }
                });

                function getImagesListAfterScrollEnd() {
                    //vm.isDataPresent = false;
                    //vm.franchiseeName = null;
                    //vm.isLoaded = false;
                    //vm.query.pageNumber = vm.query.pageNumber + 1;
                    //if (vm.count == vm.beforeAfterReviewImages.length) {
                    //    return;
                    //}
                    //return beforeAfterService.getReviewImagesForFA(vm.query).then(function (result) {
                    //    vm.beforeAfterReviewImagesScroll = result.data.beforeAfterPersonViewModel;
                    //    vm.customerImageViewModel = result.data.customerImageViewModel;
                    //    vm.franchiseeName = result.data.frachiseeName;
                    //    vm.query.franchiseeId = result.data.frachiseeId.toString();
                    //    getUserList();
                    //    vm.count = result.data.totalCount;
                    //    vm.startDate = result.data.startDate;
                    //    vm.endDate = result.data.endDate;
                    //    angular.forEach(vm.beforeAfterReviewImagesScroll, function (value1, index1) {
                    //        angular.forEach(value1.residentalCollection, function (value, index) {

                    //            angular.forEach(value1.customerImageViewModel, function (value3, index) {
                    //                angular.forEach(value3.customerForImageModel, function (value2, index) {
                    //                    if (value2.relactiveLocationAfterImageUrl == "") {
                    //                        fileService.getBeforeAfter(value2.afterImageFileId).then(function (result) {
                    //                            value2.relactiveLocationAfterImage = fileService.getStreamUrl(result);
                    //                            value2.relactiveLocationAfterImageUrl = value2.relactiveLocationAfterImage;
                    //                        });

                    //                    }
                    //                    if (value2.relactiveLocationBeforeImageUrl == "" || value2.relactiveLocationBeforeImageUrl == null) {
                    //                        fileService.getBeforeAfter(value2.beforeImageFileId).then(function (result) {
                    //                            value2.relactiveLocationBeforeImage = fileService.getStreamUrl(result);
                    //                            value2.relactiveLocationBeforeImageUrl = value2.relactiveLocationBeforeImage;
                    //                        });

                    //                    }
                    //                })
                    //            })
                    //            if (value.relactiveLocationAfterImageUrl == "") {
                    //                fileService.getBeforeAfter(value.afterImageFileId).then(function (result) {
                    //                    value.relactiveLocationAfterImage = fileService.getStreamUrl(result);
                    //                    value.relactiveLocationAfterImageUrl = value.relactiveLocationAfterImage;
                    //                });

                    //            }
                    //            if (value.relactiveLocationBeforeImageUrl == "") {
                    //                fileService.getBeforeAfter(value.beforeImageFileId).then(function (result) {
                    //                    value.relactiveLocationBeforeImage = fileService.getStreamUrl(result);
                    //                    value.relactiveLocationBeforeImageUrl = value.relactiveLocationBeforeImage;
                    //                });

                    //            }

                    //        });
                    //        angular.forEach(value1.nonResidentalCollection, function (value, index) {
                    //            angular.forEach(value.beforeAfterViewModel, function (value2, index) {
                    //                if (value2.relactiveLocationAfterImageUrl == "") {
                    //                    fileService.getBeforeAfter(value2.afterImageFileId).then(function (result) {
                    //                        value2.relactiveLocationAfterImage = fileService.getStreamUrl(result);
                    //                        value2.relactiveLocationAfterImageUrl = value2.relactiveLocationAfterImage;
                    //                    });

                    //                }
                    //                if (value2.relactiveLocationBeforeImageUrl == "") {
                    //                    fileService.getBeforeAfter(value2.beforeImageFileId).then(function (result) {
                    //                        value2.relactiveLocationBeforeImage = fileService.getStreamUrl(result);
                    //                        value2.relactiveLocationBeforeImageUrl = value2.relactiveLocationBeforeImage;
                    //                    });

                    //                }
                    //                if (value2.relactiveLocationExteriorImageUrl == "") {
                    //                    fileService.getBeforeAfter(value2.exteriorImageFileId).then(function (result) {
                    //                        value2.relactiveLocationExteriorImage = fileService.getStreamUrl(result);
                    //                        value2.relactiveLocationExteriorImageUrl = value2.relactiveLocationExteriorImage;
                    //                        value.nonResidentalImageUrl = value2.relactiveLocationExteriorImage;
                    //                    });
                    //                }
                    //                else {
                    //                    value.nonResidentalImageUrl = '/Content/images/no_image_thumb.gif';
                    //                }


                    //            });
                    //        });
                    //        vm.beforeAfterReviewImages.push(value1);
                    //        if (index1 == vm.beforeAfterReviewImagesScroll.length - 1)
                    //            vm.isDataPresent = true;
                    //    });



                    //    $scope.$emit("update-title", "");
                    //    //getUserList();
                    //});
                }

                function resetSearchOption() {
                    vm.LocalMarketingQuery.franchiseeId = null;
                    vm.LocalMarketingQuery.startDate = null;
                    vm.LocalMarketingQuery.endDate = null;
                    vm.LocalMarketingQuery.isDateFilter = false;
                    $scope.$broadcast("reset-dates");
                    getLocalMarketingReview();
                }

                function resetInnerSearchOption() {
                    vm.LocalMarketingQuery.userId = null;
                    vm.LocalMarketingQuery.surfaceMaterial = '';
                    vm.LocalMarketingQuery.surfaceColor = '';
                    vm.LocalMarketingQuery.finishMaterial = '';
                    vm.LocalMarketingQuery.buildingLocation = '';
                    vm.LocalMarketingQuery.surfaceTypeId = '';
                    vm.LocalMarketingQuery.serviceTypeId = '';
                    vm.LocalMarketingQuery.isImagePairReviewed = 0;
                    vm.LocalMarketingQuery.marketingClassId = 0;
                    vm.LocalMarketingQuery.isFilter = false;
                    getLocalMarketingReview();
                }

                function getFilteredLocalMarketingReview() {
                    vm.LocalMarketingQuery.isFilter = true;
                    getLocalMarketingReview();
                }

                function getLocalMarketingReview() {
                    return beforeAfterService.getLocalMarketingReview(vm.LocalMarketingQuery).then(function (result) {
                        if (result.data != null) {
                            vm.marketingReviewStartDate = result.data.startDate;
                            vm.marketingReviewEndtDate = result.data.endDate;
                            vm.isFranchiseeAdmin = result.data.isFranchiseeAdmin;
                            if (result.data.franchiseeListLocalMarketingModel != null) {
                                vm.localMarketingReviewList = result.data.franchiseeListLocalMarketingModel;
                                getUserList();
                                //blob
                                angular.forEach(vm.localMarketingReviewList, function (value1, index) {
                                    if (value1.personCount > 0) {
                                        angular.forEach(value1.personListLocalMarketingModel, function (value2, index) {
                                            if (value2.schedulerCount > 0) {
                                                angular.forEach(value2.schedulerListLocalMarketingModel, function (value3, index) {
                                                    if (value3.count > 0 || value3.beforeAfterImagesLocalMarketingModel.length > 0) {
                                                        angular.forEach(value3.beforeAfterImagesLocalMarketingModel, function (value4, index) {
                                                            if (value4.s3BucketBeforeImageUrlThumb == "" || value4.s3BucketBeforeImageUrlThumb == null) {
                                                                if (value4.relactiveLocationBeforeImageUrlThumb == "") {
                                                                    if (value4.relactiveLocationBeforeImage != "") {
                                                                        fileService.getFileStreamByUrl(value4.relactiveLocationBeforeImage).then(function (result) {
                                                                            value4.s3BucketBeforeImageUrlThumb = fileService.getStreamUrl(result);
                                                                        });
                                                                    }
                                                                    else {
                                                                        value4.s3BucketBeforeImageUrlThumb = value4.emptyImage;
                                                                    }
                                                                }
                                                                else {
                                                                    fileService.getFileStreamByUrl(value4.relactiveLocationBeforeImageUrlThumb).then(function (result) {
                                                                        value4.s3BucketBeforeImageUrlThumb = fileService.getStreamUrl(result);
                                                                    });
                                                                }
                                                            }
                                                            if (value4.s3BucketAfterImageUrlThumb == "" || value4.s3BucketAfterImageUrlThumb == null) {
                                                                if (value4.relactiveLocationAfterImageUrlThumb == "") {
                                                                    if (value4.relactiveLocationAfterImage != "") {
                                                                        fileService.getFileStreamByUrl(value4.relactiveLocationAfterImage).then(function (result) {
                                                                            value4.s3BucketAfterImageUrlThumb = fileService.getStreamUrl(result);
                                                                        });
                                                                    }
                                                                    else {
                                                                        value4.s3BucketAfterImageUrlThumb = value4.emptyImage;
                                                                    }
                                                                }
                                                                else {
                                                                    fileService.getFileStreamByUrl(value4.relactiveLocationAfterImageUrlThumb).then(function (result) {
                                                                        value4.s3BucketAfterImageUrlThumb = fileService.getStreamUrl(result);
                                                                    });
                                                                }
                                                            }
                                                        })
                                                    }
                                                })
                                            }
                                        })
                                    }                                
                                })
                            }
                        }
                    })
                }

                function prev() {
                    var startDate = new Date(vm.marketingReviewStartDate);
                    vm.LocalMarketingQuery.startDate = new Date(startDate.setDate(startDate.getDate() - 7));

                    var endDate = new Date(vm.marketingReviewEndtDate);
                    vm.LocalMarketingQuery.endDate = new Date(endDate.setDate(endDate.getDate() - 7));

                    getLocalMarketingReview();
                }

                function next() {
                    var startDate = new Date(vm.marketingReviewStartDate);
                    vm.LocalMarketingQuery.startDate = new Date(startDate.setDate(startDate.getDate() + 7));

                    var endDate = new Date(vm.marketingReviewEndtDate);
                    vm.LocalMarketingQuery.endDate = new Date(endDate.setDate(endDate.getDate() + 7));

                    getLocalMarketingReview();
                }

                function getDateFilterLocalMarketingReview() {
                    vm.LocalMarketingQuery.isDateFilter = true;
                    getLocalMarketingReview();
                }

                function markImageAsReviwed(beforeAfterImage) {
                    beforeAfterImage.isImagePairReviewed = beforeAfterImage.isImagePairReviewed;
                    return beforeAfterService.markImageAsReviwed(beforeAfterImage).then(function (result) {
                        if (result.data != null) {
                            if (beforeAfterImage.isImagePairReviewed) {
                                toaster.show("Image Pair Marked As Reviewed Successfully!");
                            }
                            else if (!beforeAfterImage.isImagePairReviewed) {
                                toaster.show("Image Pair Unmarked As Reviewed Successfully!");
                            }
                            else {
                                toaster.show(result.message.message);
                            }
                        }
                    });
                }

                function bestPictureSelected(beforeAfterImage) {

                    if (beforeAfterImage.beforeImageFileId == null || beforeAfterImage.afterImageFileId == null) {
                        toaster.error("You can mark only if both images are uploaded!");
                        beforeAfterImage.isBestPicture = false;
                        beforeAfterImage.isAddToLocalGallery = false;
                        return;
                    }
                    else {
                        return beforeAfterService.markImageAsBestPair(beforeAfterImage).then(function (result) {
                            if (result.data != null) {
                                if (beforeAfterImage.isBestPicture) {
                                    toaster.show("Image Pair Marked As Best Pair Successfully!");
                                }
                                else if (!beforeAfterImage.isBestPicture) {
                                    toaster.show("Image Pair Unmarked As Best Pair Successfully!");
                                }
                                else {
                                    toaster.show(result.message.message);
                                }
                            }
                            else {
                                toaster.error("Error in mark image as Best Pair");
                            }
                        });
                    }

                }

                function addToLocalSiteGallery(beforeAfterImage) {
                    if (beforeAfterImage.beforeImageFileId == null || beforeAfterImage.afterImageFileId == null) {
                        toaster.error("You can mark only if both images are uploaded!");
                        beforeAfterImage.isBestPicture = false;
                        beforeAfterImage.isAddToLocalGallery = false;
                        return;
                    }
                    else {        
                        return beforeAfterService.markImageAsAddToLocalGallery(beforeAfterImage).then(function (result) {
                            if (result.data != null) {
                                if (beforeAfterImage.isAddToLocalGallery) {
                                    toaster.show("Image Pair Marked As Add To Local Site Gallery Successfully!");
                                }
                                else if (!beforeAfterImage.isAddToLocalGallery) {
                                    toaster.show("Image Pair Unmarked As Add To Local Site Gallery Successfully!");
                                }
                                else {
                                    toaster.show(result.message.message);
                                }
                            }
                            else {
                                toaster.error("Error in mark image as Add To LocalSite Gallery");
                            }
                        });
                    }
                }

                $scope.$emit("update-title", "");
                $q.all([getFranchiseeCollectionForFA(), getImagesList(), prepareSearchOptions(), getServicesCollection(), getmarketingClassCollection(), getLocalMarketingReview()]);
            }]);
}());