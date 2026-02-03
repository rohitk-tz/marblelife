(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("BeforeAfterBestMarkController",
        ["$scope", "$rootScope", "$state", "$q", "EstimateService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "AddressService", "SchedulerService", "$filter", "CustomerService",
            "APP_CONFIG", "GeoCodeService", "$uibModal", "BeforeAfterService", "FileService", "$window", "$timeout", "$location",
            function ($scope, $rootScope, $state, $q, estimateService, franchiseeService,
                notification, clock, toaster, addressService, schedulerService, $filter, customerService, config, geoCodeService,
                $uibModal, beforeAfterService, fileService, $window, $timeout, $location) {
                var vm = this;
                vm.getServicesCollection = getServicesCollection;
                vm.openPopUp = openPopUp;
                vm.isPopUpOpened = false;
                vm.isFromLoad = true;
                vm.isFromLoadForNext = false;
                vm.isFromPrevious = false;
                vm.resetSearch = resetSearch;
                vm.reviewMarked = reviewMarked;
                vm.refresh = refresh;
                vm.isTodayDisable = true;
                vm.currentWeek = currentWeek;
                vm.prev = prev;
                vm.next = next;
                vm.changeClass = changeClass;
                vm.isLoaded = false;
                //vm.addToSlide = addToSlide;
                vm.count = 0;
                var lastScrollTop = 0;
                vm.searchOptions = [];
                vm.isFranchiseeAdmin = false;
                vm.bestPictureSelected = bestPictureSelected;
                vm.addToLocalSiteGallery = addToLocalSiteGallery;
                vm.naviage = naviage;
                vm.getDateFilterReviewReview = getDateFilterReviewReview;
                vm.resetSeachOption = resetSeachOption;
                vm.getUserList = getUserList;

                vm.surfaceMaterial = [{ display: "Marble", value: "Stonelife:marble" }, { display: "Granite", value: "stonelife:granite" }, { display: "Ceramic (tile and grout)", value: "groutlife:ceramic (tile and grout)" },
                { display: "Travertine", value: "stonelife:travertine" }, { display: "Terrazzo", value: "Enduracrete:terrazzo" }, { display: "Porcelain", value: "groutlife:porcelain" },
                { display: "Vinyl", value: "vinylguard:vinyl" }, { display: "Wood", value: "wood" }, { display: "Enduracrete:concrete", value: "Enduracrete:concrete" },
                { display: "Slate", value: "Slate" }, { display: "Other", value: "Other" }, { display: "Metal", value: "Metal" },
                { display: "Carpet", value: "Carpet" }, { display: "Glass", value: "Glass" }, { display: "Limestone", value: "Limestone" },
                { display: "Quartz", value: "Quartz" }
                ];

                vm.surfaceType = [{ display: "Floor", value: "Floor" }, { display: "Counter (Kitchen)", value: "Counter (Kitchen)" }, { display: "Wall", value: "Wall" },
                { display: "Vanity (Bathroom)", value: "Vanity (Bathroom)" }, { display: "Sign", value: "Sign" }, { display: "Building Exterior", value: "Building Exterior" },
                { display: "Pool Deck", value: "Pool Deck" }, { display: "Patio", value: "Patio" }, { display: "Walkway", value: "Walkway" },
                { display: "Other", value: "Other" }, { display: "Fireplace", value: "Fireplace" }, { display: "Concrete", value: "Concrete" },
                { display: "Shower Wall", value: "Shower Wall" }, { display: "Tub Deck", value: "Tub Deck" }, { display: "Threshold", value: "Threshold" }
                ];

                vm.buildingLocation = [{ display: "Lobby", value: "Lobby" }, { display: "Garage", value: "Garage" }, { display: "Basement", value: "Basement" },
                { display: "Foyer", value: "Foyer" }, { display: "Bathroom", value: "Bathroom" }, { display: "Kitchen", value: "Kitchen" },
                { display: "Bedroom", value: "Bedroom" }, { display: "Living", value: "Living" }, { display: "Dining", value: "Dining" },
                { display: "Hall", value: "Hall" }, { display: "Elevator", value: "Elevator" }, { display: "Floor", value: "Floor" },
                { display: "Exterior Feature", value: "Exterior Feature" }, { display: "Baseboards", value: "Baseboards" }, { display: "Laundry", value: "Laundry" }
                ];

                vm.surfaceColor = [{ display: "White", value: "White" }, { display: "Green", value: "Green" }, { display: "Black", value: "Black" },
                { display: "Brown", value: "Brown" }, { display: "Tan", value: "Tan" }, { display: "Red ", value: "Red " },
                { display: "Other", value: "Other" }
                ];

                vm.finishMaterial = [{ display: "Matte", value: "Matte" }, { display: "Satin/Semi-Gloss", value: "Satin/Semi-Gloss" },
                { display: "Gloss", value: "Gloss" }, { display: "Ultimate Finish", value: "Ultimate Finish" }
                ];

                vm.query = {
                    userId: null,
                    franchiseeId: null,
                    startDate: null,
                    endDate: null,
                    surfaceType: '',
                    surfaceMaterial: '',
                    surfaceColor: '',
                    finishMaterial: '',
                    buildingLocation: '',
                    surfaceTypeId: '',
                    serviceTypeId: '',
                    marketingClassId: 0,
                    isImagePairReviewed: 0,
                    //managementCompany: '',
                    //maidService: '',
                    isDateFilter: false,
                    isFilter: false,
                    //buildingType: '',
                    //statusId: '',
                    //pageNumber: 1,
                    //text: '',
                    //pageSize: 20
                    pendingToAddInLocalSiteGallery: true
                };

                vm.saveQuery = {
                    beforeServiceId: '',
                    afterServiceId: '',
                    beforeAfterBestPairType: 229
                };

                vm.reviewMarkedQuery = {
                    startDate: '',
                    endate: '',
                    isReview: false,

                };

                vm.saveImagesBestPair = saveImagesBestPair;
                vm.scrollClick = 1;
                vm.startReview = startReview;
                vm.addToSlider = addToSlider;
                vm.getReviewMarketingImagesList = getReviewMarketingImagesList;
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
                $scope.addSlide = function (value) {
                    var newWidth = 600 + slides.length + 1;
                    slides.push({
                        image: '//unsplash.it/' + newWidth + '/300',
                        text: ['Nice image', 'Awesome photograph', 'That is so cool', 'I love that'][slides.length % 4],
                        id: currIndex++,
                        relactiveLocationBeforeImageUrl: value.relactiveLocationBeforeImageUrl,
                        relactiveLocationAfterImageUrl: value.relactiveLocationAfterImageUrl,
                        afterServiceId: value.afterServiceId,
                        beforeServiceId: value.beforeServiceId,
                        isSelected: value.isSelected,
                        franchiseeName: value.frachiseeName,
                        selectedBy: value.selectedBy,
                        modifiedOn: value.modifiedDate,
                        index: value.index
                    });
                };



                function addToSlider() {
                    vm.index = 0;
                    angular.forEach(vm.beforeAfterReviewImages, function (value1, index) {
                        if (value1 != null) {
                            vm.index += 1;
                            value1.relactiveLocationBeforeImageUrl = value1.relactiveLocationBeforeImage;
                            value1.image = value1.relactiveLocationBeforeImage;
                            value1.relactiveLocationAfterImageUrl = value1.relactiveLocationAfterImage;
                            value1.image = value1.relactiveLocationAfterImage;
                            addToSlide();
                        }
                    })
                }

                function changeVariabe() {
                    vm.index = vm.index + 1;
                }

                //function getReviewMarketingImagesList() {
                //    vm.isLoaded = false;
                //    vm.query.scrollClick = vm.scrollClick;
                //    vm.index = 0;
                //    return beforeAfterService.getReviewImages(vm.query).then(function (result) {
                //        vm.beforeAfterReviewImages = result.data.beforeAfterViewModel;
                //        if (vm.isFromLoad || vm.isFromPrevious) {
                //            vm.isFromLoad = false;
                //            vm.currentStartDate = result.data.startDate;
                //            vm.currentEndDate = result.data.endate;
                //        }
                //        vm.count = result.data.totalCount;
                //        vm.startDate = result.data.startDate;
                //        vm.endDate = result.data.endate;
                //        vm.isReviewed = result.data.isReview;
                //        $scope.isReviewed = result.data.isReview;
                //        $scope.isReviewed = result.data.isReview;
                //        vm.reviewMarkedQuery.isReview = result.data.isReview;

                //        addToSlider(vm.beforeAfterReviewImages);
                //        vm.isLoaded = true;
                //    });
                //}

                function getReviewMarketingImagesList() {
                    vm.isLoaded = false;
                    //vm.query.scrollClick = vm.scrollClick;
                    //vm.index = 0;
                    return beforeAfterService.getReviewImages(vm.query).then(function (result) {
                        if (result.data != null) {
                            vm.marketingReviewStartDate = result.data.startDate;
                            vm.marketingReviewEndtDate = result.data.endDate;
                            vm.isFranchiseeAdmin = result.data.isFranchiseeAdmin;
                            if (result.data.reviewMarketingFranchiseeViewModels != null) {
                                vm.reviewMarketingImageList = result.data.reviewMarketingFranchiseeViewModels;
                                getUserList();
                                //blob
                                angular.forEach(vm.reviewMarketingImageList, function (value1, index) {
                                    if (value1.personCount > 0) {
                                        angular.forEach(value1.reviewMarketingPersonViewModel, function (value2, index) {
                                            if (value2.schedulerCount > 0) {
                                                angular.forEach(value2.reviewMarketingSchedulerViewModel, function (value3, index) {
                                                    if (value3.count > 0 || value3.reviewMarketingBeforeAfterImageViewModel.length > 0) {
                                                        angular.forEach(value3.reviewMarketingBeforeAfterImageViewModel, function (value4, index) {
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
                    });
                }
                function addToSlide() {
                    if (vm.index != vm.beforeAfterReviewImages.length) {
                        return;
                    }
                    angular.forEach(vm.beforeAfterReviewImages, function (value1, index) {
                        $scope.addSlide(value1);
                    });
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
                    vm.searchOptions.push({ display: 'Surface Type', value: '1' },
                        { display: 'Surface Material', value: '2' },
                        { display: 'Service Type', value: '3' },
                        { display: 'Surface Color', value: '4' },
                        { display: 'Building Location', value: '5' },
                        { display: 'Management Company', value: '7' },
                        { display: 'Maid Service', value: '8' },
                        { display: 'Finish Material', value: '9' },
                        { display: 'Marketing Class', value: '10' },
                        { display: 'Pending To Add In Local Site Gallery', value: '11' },
                        { display: 'User List', value: '12' });
                }


                function getServicesCollection() {
                    //return franchiseeService.getServiceTypeCollection().then(function (result) {
                    //    vm.serviceType = result.data;
                    //});
                    return franchiseeService.getServiceTypeCollectionForInvoiceNew().then(function (result) {
                        vm.serviceType = result.data;
                    });
                }

                function resetSearch() {
                    vm.searchOption = '';
                    vm.query = {
                        userId: null,
                        franchiseeId: null,
                        startDate: null,
                        endDate: null,
                        surfaceType: '',
                        surfaceMaterial: '',
                        surfaceColor: '',
                        finishMaterial: '',
                        buildingLocation: '',
                        surfaceTypeId: '',
                        serviceTypeId: '',
                        marketingClassId: 0,
                        isImagePairReviewed: 0,
                        //managementCompany: '',
                        //maidService: '',
                        isDateFilter: false,
                        isFilter: false
                    //buildingType: '',
                    //statusId: '',
                    //pageNumber: 1,
                    //text: '',
                    //pageSize: 20
                    };
                    $scope.$broadcast("reset-dates");
                    refresh();
                }

                function refresh() {
                    slides = $scope.slides = [];
                    getReviewMarketingImagesList();
                    getFranchiseeCollection();
                    prepareSearchOptions();
                    getServicesCollection();
                    getUserList();
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                //function prev() {
                //    vm.isFromLoadForNext = true;
                //    slides = $scope.slides = [];
                //    vm.isTodayDisable = false;
                //    var startDate = new Date(vm.startDate);
                //    vm.query.startDate = new Date(startDate.setDate(startDate.getDate() - 6));
                //    vm.isFromPrevious = true;
                //    var endDate = new Date(vm.endDate);
                //    vm.query.endDate = new Date(endDate.setDate(endDate.getDate() - 6));
                //    getReviewMarketingImagesList();
                //}

                //function next() {
                //    slides = $scope.slides = [];
                //    vm.isTodayDisable = false;
                //    var startDate = new Date(vm.startDate);
                //    vm.query.startDate = new Date(startDate.setDate(startDate.getDate() + 7));
                //    vm.isFromPrevious = false;
                //    var endDate = new Date(vm.endDate);
                //    vm.query.endDate = new Date(endDate.setDate(endDate.getDate() + 7));

                //    getReviewMarketingImagesList();
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
                    getReviewMarketingImagesList();
                }
                function getmarketingClassCollection() {
                    return customerService.getmarketingClassCollection().then(function (result) {
                        vm.marketingClass = result.data;
                    });
                }

                function prev() {
                    var startDate = new Date(vm.marketingReviewStartDate);
                    vm.query.startDate = new Date(startDate.setDate(startDate.getDate() - 7));

                    var endDate = new Date(vm.marketingReviewEndDate);
                    vm.query.endDate = new Date(endDate.setDate(endDate.getDate() - 7));

                    getReviewMarketingImagesList();
                }

                function next() {
                    var startDate = new Date(vm.marketingReviewStartDate);
                    vm.query.startDate = new Date(startDate.setDate(startDate.getDate() + 7));

                    var endDate = new Date(vm.marketingReviewEndDate);
                    vm.query.endDate = new Date(endDate.setDate(endDate.getDate() + 7));

                    getReviewMarketingImagesList();
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

                function getDateFilterReviewReview() {
                    vm.query.isDateFilter = true;
                    getReviewMarketingImagesList();
                }

                function resetSeachOption() {
                    if (vm.searchOption == "1") {
                        vm.query.surfaceMaterial = '';
                        vm.query.serviceTypeId = '';
                        vm.query.surfaceColor = '';
                        vm.query.buildingLocation = '';
                        vm.query.finishMaterial = '';
                        vm.query.marketingClassId = 0;
                        vm.query.pendingToAddInLocalSiteGallery = true;
                        vm.query.userId = null;
                    }
                    else if (vm.searchOption == "2") {
                        vm.query.surfaceType = '';
                        vm.query.serviceTypeId = '';
                        vm.query.surfaceColor = '';
                        vm.query.buildingLocation = '';
                        vm.query.finishMaterial = '';
                        vm.query.marketingClassId = 0;
                        vm.query.pendingToAddInLocalSiteGallery = true;
                        vm.query.userId = null;
                    }
                    else if (vm.searchOption == "3") {
                        vm.query.surfaceType = '';
                        vm.query.surfaceMaterial = '';
                        vm.query.surfaceColor = '';
                        vm.query.buildingLocation = '';
                        vm.query.finishMaterial = '';
                        vm.query.marketingClassId = 0;
                        vm.query.pendingToAddInLocalSiteGallery = true;
                        vm.query.userId = null;
                    }
                    else if (vm.searchOption == "4") {
                        vm.query.surfaceType = '';
                        vm.query.surfaceMaterial = '';
                        vm.query.serviceTypeId = '';
                        vm.query.buildingLocation = '';
                        vm.query.finishMaterial = '';
                        vm.query.marketingClassId = 0;
                        vm.query.pendingToAddInLocalSiteGallery = true;
                        vm.query.userId = null;
                    }
                    else if (vm.searchOption == "5") {
                        vm.query.surfaceType = '';
                        vm.query.surfaceMaterial = '';
                        vm.query.serviceTypeId = '';
                        vm.query.surfaceColor = '';
                        vm.query.finishMaterial = '';
                        vm.query.marketingClassId = 0;
                        vm.query.pendingToAddInLocalSiteGallery = true;
                        vm.query.userId = null;
                    }
                    else if (vm.searchOption == "7") {
                        vm.query.surfaceType = '';
                        vm.query.surfaceMaterial = '';
                        vm.query.serviceTypeId = '';
                        vm.query.surfaceColor = '';
                        vm.query.buildingLocation = '';
                        vm.query.finishMaterial = '';
                        vm.query.marketingClassId = 0;
                        vm.query.pendingToAddInLocalSiteGallery = true;
                        vm.query.userId = null;
                    }
                    else if (vm.searchOption == "8") {
                        vm.query.surfaceType = '';
                        vm.query.surfaceMaterial = '';
                        vm.query.serviceTypeId = '';
                        vm.query.surfaceColor = '';
                        vm.query.buildingLocation = '';
                        vm.query.finishMaterial = '';
                        vm.query.marketingClassId = 0;
                        vm.query.pendingToAddInLocalSiteGallery = true;
                        vm.query.userId = null;
                    }
                    else if (vm.searchOption == "9") {
                        vm.query.surfaceType = '';
                        vm.query.surfaceMaterial = '';
                        vm.query.serviceTypeId = '';
                        vm.query.surfaceColor = '';
                        vm.query.buildingLocation = '';
                        vm.query.marketingClassId = 0;
                        vm.query.pendingToAddInLocalSiteGallery = true;
                        vm.query.userId = null;
                    }
                    else if (vm.searchOption == "10") {
                        vm.query.surfaceType = '';
                        vm.query.surfaceMaterial = '';
                        vm.query.serviceTypeId = '';
                        vm.query.surfaceColor = '';
                        vm.query.buildingLocation = '';
                        vm.query.finishMaterial = '';
                        vm.query.pendingToAddInLocalSiteGallery = true;
                        vm.query.userId = null;
                    }
                    else if (vm.searchOption == "11") {
                        vm.query.surfaceType = '';
                        vm.query.surfaceMaterial = '';
                        vm.query.serviceTypeId = '';
                        vm.query.surfaceColor = '';
                        vm.query.buildingLocation = '';
                        vm.query.finishMaterial = '';
                        vm.query.marketingClassId = 0;
                        vm.query.pendingToAddInLocalSiteGallery = false;
                        vm.query.userId = null;
                        getReviewMarketingImagesList();
                    }
                    else if (vm.searchOption == "12") {
                        vm.query.surfaceType = '';
                        vm.query.surfaceMaterial = '';
                        vm.query.serviceTypeId = '';
                        vm.query.surfaceColor = '';
                        vm.query.buildingLocation = '';
                        vm.query.finishMaterial = '';
                        vm.query.marketingClassId = 0;
                        vm.query.pendingToAddInLocalSiteGallery = false;
                    }
                }

                function getUserList() {
                    return beforeAfterService.getSalesRepTechnicianList(vm.query.franchiseeId).then(function (result) {
                        vm.techList = result.data;
                    });
                }

                $scope.$emit("update-title", "");
                $q.all([getReviewMarketingImagesList(), prepareSearchOptions(), getServicesCollection(), getFranchiseeCollection(), getmarketingClassCollection()]);
            }]);
}());