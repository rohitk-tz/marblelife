(function () {
    'use strict';

    var SortColumns = {
        ID: 'ID',
        DialedNumber: 'DialedNumber',
        CallerId: 'CallerId',
        PhoneLabel: 'PhoneLabel',
        TransferTo: 'TransferTo',
        TransferType: 'TransferType',
        Date: 'Date',
        CallType: 'CallType',
        CallDuration: 'CallDuration',
        ValidCall: 'ValidCall',
        Franchisee: 'Franchisee',
        CalledFranchisee: 'CalledFranchisee',
        InvoiceId: 'InvoiceId',
        Tag: 'Tag'
    };

    var ShowColumnss = {
        CallflowSetName: 'CALLFLOWSETNAME',

    };


    var SortColumnsLeadFlow = {
        ID: 'ID',
        Destination: 'Destination',
        TransferTo: 'TransferTo',
        PhoneLabel: 'PhoneLabel',
        CallStatus: 'CallStatus',
        Date: 'Date',
        ZipCode: 'ZipCode',
        CallRoute: 'CallRoute',
        CallerId: 'CallerId',
        CallDuration: 'CallDuration',
        ValidCall: 'ValidCall',
        Franchisee: 'Franchisee',
        CalledFranchisee: 'CalledFranchisee',
    };

    angular.module(ReportsConfiguration.moduleName).controller("MarketingLeadCallDetailController",
        ["$state", "$stateParams", "$q", "$scope", "MarketingLeadService", "APP_CONFIG", "$rootScope", "FileService", "FranchiseeService", "$uibModal", "Clock", "$filter",
            "RoutingNumberService",
            function ($state, $stateParams, $q, $scope, marketingLeadService, config, $rootScope, fileService, franchiseeService, $uibModal, clock, $filter,
                routingNumberService) {
                var vm = this;
                vm.sortingForLead = sortingForLead;
                vm.pageChangeForLeadFlow = pageChangeForLeadFlow;
                vm.pageChangeForLeadFlow = pageChangeForLeadFlow;
                vm.getLeadFlowDetails = getLeadFlowDetails;
                vm.isCallDetailsTab = true;
                vm.isLeadFlowTab = false;
                vm.changeTab = changeTab;
                vm.selectedColumns = [];
                vm.showColumns = [
                    { label: "Keyword Groups", id: "KeywordGroups_CallAnalytics" },
                    { label: "Keyword Spotting Complete", id: "KeywordSpottingComplete_CallAnalytics" },
                    { label: "Campaign", id: "Campaign_VisitorData" },
                    { label: "Campaign Id", id: "CampaignId_PaidSearch" },
                    { label: "Ad Group", id: "Adgroup_PaidSearch" },
                    { label: "Ad Group Id", id: "AdgroupId_PaidSearch" },
                    { label: "Ads", id: "Ads_PaidSearch" },
                    { label: "Ad Id", id: "AdId_PaidSearch" },
                    { label: "Keywords", id: "Keywords_PaidSearch" },
                    { label: "Keyword Id", id: "KeywordId_PaidSearch" },
                    { label: "Click Id", id: "ClickId_PaidSearch" },
                    { label: "Keyword Match Type", id: "KeyWordMatchType_PaidSearch" },
                    { label: "CallInly Flag", id: "CallInlyFlag_PaidSearch" },
                    { label: "Valid Call", id: "Valid_Call" },
                    { label: "Call Type", id: "Call_Type" },
                    { label: "Tag", id: "Tag" },
                    { label: "Call Flow Set Name", id: "CallFlowSetName" },
                    { label: "Data From New API", id: "DataFromNewAPI" },
                    { label: "Call Flow Set Id", id: "CallFlowSetId" },
                    { label: "Call Flow Destination", id: "CallFlowDestination" },
                    { label: "Call Flow Destination Id", id: "CallFlowDestinationId" },
                    { label: "Call Flow Reroute", id: "CallFlowReroute" },
                    { label: "Call Transfer Type", id: "CallTransferType" },
                    { label: "Invoice#", id: "Invoice" },
                    { label: "Call Flow Source", id: "CallFlowSource" },
                    { label: "Call Flow Source Id", id: "CallFlowSourceId" },
                    { label: "Call Flow Source Qualified", id: "CallFlowSourceQualified" },
                    { label: "Call Flow Repeat Source Caller", id: "CallFlowRepeatSourceCaller" },
                    { label: "Call Flow Source Cap", id: "CallFlowSourceCap" },
                    { label: "Call Flow Source Route", id: "CallFlowSourceRoute" },
                    { label: "Call Flow Source Route Id", id: "CallFlowSourceRouteId" },
                    { label: 'Call Flow Source Route Qualified Id', id: 'CallFlowSourceRouteQualified' },
                    { label: 'Call Flow State', id: 'CallFlowState' },
                    { label: 'Transfer Type', id: "TransferType_CallFlow" },
                    { label: 'Call Transfer Status', id: "CallTransferStatus_CallFlow" },
                    { label: "Dialog Analytics", id: "DialogAnalytics_Recording" },
                    { label: 'GeoLookup Attempt', id: 'GeoLookupAttempt_ReverseLookUp' },
                    { label: "GeoLookup Result", id: "GeoLookupResult_ReverseLookUp" },
                    { label: "Call Activities", id: "CallActivities" },
                    { label: "Channel", id: "Channel_Attribution" },
                    { label: "Status", id: "Status_Attribution" },
                    { label: "Rank", id: "Rank_Attribution" },
                    { label: "Pid", id: "Pid_Attribution" },
                    { label: "Bid", id: "Bid_Attribution" },
                    { label: "First Touch Document Title", id: "DocumentTitle_FirstTouch" },
                    { label: "First Touch Document Url", id: "DocumentUrl_FirstTouch" },
                    { label: "First Touch Document Path", id: "DocumentPath_FirstTouch" },
                    { label: "First Touch Document TimeStamp", id: "DocumentTimeStamp_FirstTouch" },
                    { label: "Last Touch Document Title", id: "DocumentTitle_LastTouch" },
                    { label: "Last Touch Document Url", id: "DocumentUrl_LastTouch" },
                    { label: "Last Touch Document Path", id: "DocumentPath_LastTouch" },
                    { label: "Last Touch Document TimeStamp", id: "DocumentTimeStamp_LastTouch" },
                    { label: "IPAddress", id: "IPAddress_VisitorData" },
                    { label: "Device", id: "Device_VisitorData" },
                    { label: "Browser", id: "Browser_VisitorData" },
                    { label: "BrowserVersion", id: "BrowserVersion_VisitorData" },
                    { label: "Os", id: "Os_VisitorData" },
                    { label: "OsVersion", id: "OsVersion_VisitorData" },
                    { label: "SearchTerm", id: "SearchTerm_VisitorData" },
                    { label: "ActivityValue", id: "ActivityValue_VisitorData" },
                    { label: "ActivityTypeId", id: "ActivityTypeId_VisitorData" },
                    { label: "ActivityKeyword", id: "ActivityKeyword_VisitorData" },
                    { label: "ActivityTag", id: "ActivityTag_VisitorData" },
                    { label: "Platform", id: "Platform_VisitorData" },
                    { label: "SourceGuard", id: "SourceGuard_VisitorData" },
                    { label: "VisitorLogUrl", id: "VisitorLogUrl_VisitorData" },
                    { label: "GoogleUaClientId", id: "GoogleUaClientId_VisitorData" },
                    { label: "GClid", id: "GClid_VisitorData" },
                    { label: "UtmSource", id: "UtmSource_DefaultUtmParameters" },
                    { label: "UtmMedium", id: "UtmMedium_DefaultUtmParameters" },
                    { label: "UtmCampaign", id: "UtmCampaign_DefaultUtmParameters" },
                    { label: "UtmTerm", id: "UtmTerm_DefaultUtmParameters" },
                    { label: "UtmContent", id: "UtmContent_DefaultUtmParameters" },
                    { label: "VtKeyword", id: "VtKeyword_ValueTrackParameters" },
                    { label: "VtMatchType", id: "VtMatchType_ValueTrackParameters" },
                    { label: "VtNetwork", id: "VtNetwork_ValueTrackParameters" },
                    { label: "VtDevice", id: "VtDevice_ValueTrackParameters" },
                    { label: "VtDeviceModel", id: "VtDeviceModel_ValueTrackParameters" },
                    { label: "VtCreative", id: "VtCreative_ValueTrackParameters" },
                    { label: "VtPlacement", id: "VtPlacement_ValueTrackParameters" },
                    { label: "VtTarget", id: "VtTarget_ValueTrackParameters" },
                    { label: "VtParam1", id: "VtParam1_ValueTrackParameters" },
                    { label: "VtParam2", id: "VtParam2_ValueTrackParameters" },
                    { label: "VtRandom", id: "VtRandom_ValueTrackParameters" },
                    { label: "VtAceid", id: "VtAceid_ValueTrackParameters" },
                    { label: "VtAdposition", id: "VtAdposition_ValueTrackParameters" },
                    { label: "VtProductTargetId", id: "VtProductTargetId_ValueTrackParameters" },
                    { label: "VtAdType", id: "VtAdType_ValueTrackParameters" },
                    { label: "DomainSetName", id: "DomainSetName_SourceIqData" },
                    { label: "DomainSetId", id: "DomainSetId_SourceIqData" },
                    { label: "PoolName", id: "PoolName_SourceIqData" },
                    { label: "LocationName", id: "LocationName_SourceIqData" },
                    { label: "CustomValue", id: "CustomValue_SourceIqData" },
                    { label: "CustomId", id: "CustomId_SourceIqData" },                    
                    { label: "Type", id: "Type_PaidSearch" },
                    { label: "Call Recording", id: "RecordingUrl_Recording" },
                    { label: "Home Owner", id: "HomeOwner" },
                    { label: "Home Market", id: "HomeMarket" }
                ];

                vm.queryLeadFlow = {
                    pageNumber: 1,
                    startDate: null,
                    endDate: null,
                    franchiseeId: 0,
                    callTypeId: 0,
                    pageSize: config.defaultPageSize,
                    text: '',
                    sortingOrder: 0,
                    sortingColumn: '',

                };

                vm.query = {
                    pageNumber: 1,
                    startDate: null,
                    endDate: null,
                    franchiseeId: 0,
                    tagId: 0,
                    convertedLead: null,
                    mappedFranchisee: null,
                    callTypeId: 0,
                    pageSize: config.defaultPageSize,
                    text: '',
                    sort: { order: 0, propName: '' },
                    idList: [],
                    categoryIds: [],
                    columnList: [],
                    downloadColumnList: [],
                    callerId: null,
                    transferToNumber: null,
                    office: null
                };

                vm.query.downloadColumnList.push("Id");
                vm.query.downloadColumnList.push("Date/Time Of Call");
                vm.query.downloadColumnList.push("Dialed Number (dnis)");
                vm.query.downloadColumnList.push("Caller ID (ani)");
                vm.query.downloadColumnList.push("Transfer To Number");
                vm.query.downloadColumnList.push("Phone Label");
                vm.query.downloadColumnList.push("Ring Seconds");
                vm.query.downloadColumnList.push("Ring Count");
                vm.query.downloadColumnList.push("Recorded Seconds");
                vm.query.downloadColumnList.push("Call Duration");
                vm.query.downloadColumnList.push("Call Route(Mapped By ZipCode)");
                vm.query.downloadColumnList.push("Franchisee(Invoca API)");
                vm.query.downloadColumnList.push("Call Flow Entered Zip");
                vm.query.downloadColumnList.push("Preferred Contact Number");
                vm.query.downloadColumnList.push("Email");
                vm.query.downloadColumnList.push("First Name");
                vm.query.downloadColumnList.push("Last Name");
                vm.query.downloadColumnList.push("Company");
                vm.query.downloadColumnList.push("Office");
                vm.query.downloadColumnList.push("Zip Code");
                vm.query.downloadColumnList.push("Resulting Action");
                vm.query.downloadColumnList.push("Number");
                vm.query.downloadColumnList.push("Street");
                vm.query.downloadColumnList.push("City");
                vm.query.downloadColumnList.push("State");
                vm.query.downloadColumnList.push("Country");
                vm.query.downloadColumnList.push("Call Note");
                vm.query.downloadColumnList.push("Transcription Status");
                vm.query.downloadColumnList.push("Missed Call");
                vm.query.downloadColumnList.push("Find Me List");


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
                    buttonDefaultText: "Select Group(s)",
                    dynamicButtonTextSuffix: 'Selected'
                };
                $scope.translationTextsForColumn = {
                    checkAll: 'Select All',
                    uncheckAll: 'Deselect All',
                    selectGroup: 'Select All',
                    buttonDefaultText: "Select Column(s)",
                    dynamicButtonTextSuffix: 'Selected'
                };

                vm.getCallDetails = getCallDetails;
                vm.pagingOptions = config.pagingOptions;
                vm.currentPage = vm.query.pageNumber;
                vm.count = 0;
                vm.sorting = sorting;
                vm.SortColumns = SortColumns;
                vm.SortColumnsLeadFlow = SortColumnsLeadFlow;
                vm.pageChange = pageChange;
                vm.searchOptions = [];
                vm.searchOption = '';
                vm.searchOptionsForLeadFlow = [];
                vm.searchOptionForLeadFlow = '';
                vm.viewInvoice = viewInvoice;
                vm.resetSeachOption = resetSeachOption;
                vm.Roles = DataHelper.Role;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin;
                vm.currentRole = $rootScope.identity.roleId;
                vm.resetSearch = resetSearch;
                vm.refresh = refresh;
                vm.managePhoneLabels = managePhoneLabels;
                vm.downloadCallDetails = downloadCallDetails;
                vm.downloadLeadFlowDetails = downloadLeadFlowDetails;
                vm.addCallDetailsNotes = addCallDetailsNotes;
                vm.getCallNotesHistory = getCallNotesHistory;
                vm.ids = [];
                vm.leadOptions = [];
                vm.mappingOptions = [];
                vm.getNotes = getNotes;
                vm.getNotesHistory = getNotesHistory;


                vm.zeroPrefixedForOfficeDropdown = [
                    { display: "0-INC", value: "0-INC", IsActive: true },
                    { display: "0-MLFS", value: "0-MLFS", IsActive: true },
                    { display: "Unassigned", value: "Unassigned", IsActive: true },
                    { display: "X- Open Territory", value: "X- Open Territory", IsActive: true }
                ]

                function prepareLeadOptions() {
                    vm.leadOptions.push({ display: 'Yes', value: 1 }),
                        vm.leadOptions.push({ display: 'No', value: 0 });
                };

                function prepareMappingOptions() {
                    vm.mappingOptions.push({ display: 'Yes', value: 1 }),
                        vm.mappingOptions.push({ display: 'No', value: 0 });
                };


                function downloadCallDetails() {
                    vm.query.categoryIds = [];
                    if (vm.query.idList != null && vm.query.idList.length > 0) {
                        vm.query.categoryIds = vm.query.idList.map(function (elem) {
                            return elem.id;
                        }).join(",");
                    }
                    if (vm.query.franchiseeId != "" && vm.query.franchiseeId != null && vm.query.franchiseeId != 0) {
                        angular.forEach(vm.franchiseeCollectionForSearch, function (value, key) {
                            if (vm.query.franchiseeId == value.display) {
                                vm.query.franchiseeId = value.id;
                                vm.franchiseeForDisplay = value.display;
                            }
                        })
                    }
                    if (vm.query.categoryIds.length <= 0)
                        vm.query.categoryIds = null;
                    vm.downloading = true;
                    if (vm.query.franchiseeId == undefined || vm.query.franchiseeId == null) {
                        vm.query.franchiseeId = 0;
                    }
                    return marketingLeadService.downloadCallDetails(vm.query).then(function (result) {
                        var fileName = "marketingLeads.xlsx";
                        fileService.downloadFile(result.data, fileName);
                        vm.downloading = false;
                        vm.query.franchiseeId = vm.franchiseeForDisplay;
                    },
                        function () { vm.downloading = false; });
                }

                function managePhoneLabels() {
                    $state.go('core.layout.report.phoneLabel');
                }

                function viewInvoice(invoiceId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/invoice-detail.client.view.html',
                        controller: 'InvoiceDetailController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    InvoiceId: invoiceId
                                };
                            }
                        }
                    });

                    modalInstance.result.then(function () {
                    }, function () {
                    });
                }


                function prepareSearchOptions() {
                    if (vm.currentRole == vm.Roles.SuperAdmin) {
                        vm.searchOptions.push({ display: 'Franchisee', value: '1' });
                        vm.searchOptions.push({ display: 'Tag', value: '6' });
                    }

                    vm.searchOptions.push({ display: 'CallType', value: '2' });
                    vm.searchOptions.push({ display: 'Caller ID', value: '7' });
                    vm.searchOptions.push({ display: 'Converted Leads', value: '4' });
                    vm.searchOptions.push({ display: 'Mapped Franchisee', value: '5' });
                    vm.searchOptions.push({ display: 'Others', value: '3' });
                    vm.searchOptions.push({ display: 'Transfer To Number', value: '8' });
                    vm.searchOptions.push({ display: 'Office', value: '9' });
                }
                function refresh() {
                    getCallDetails();
                    getLeadFlowDetails();
                }

                function resetSeachOption() {
                    if (vm.searchOption == '1') {
                        vm.query.callTypeId = 0;
                        vm.query.text = '';
                        vm.query.tagId = 0;
                        //vm.query.pageNumber = 1;
                        vm.query.convertedLead = null;
                        vm.query.mappedFranchisee = null;
                        vm.query.callerId = null;
                        vm.query.transferToNumber = null;
                    }
                    else if (vm.searchOption == '2') {
                        vm.query.franchiseeId = 0;
                        vm.query.tagId = 0;
                        vm.query.text = '';
                        vm.query.convertedLead = null;
                        vm.query.mappedFranchisee = null;
                        //vm.query.pageNumber = 1;
                        vm.query.callerId = null;
                        vm.query.transferToNumber = null;
                    }
                    else if (vm.searchOption == '3') {
                        vm.query.franchiseeId = 0;
                        vm.query.tagId = 0;
                        vm.query.text = '';
                        vm.query.convertedLead = null;
                        vm.query.mappedFranchisee = null;
                        vm.query.pageNumber = 1;
                        vm.query.callerId = null;
                        vm.query.transferToNumber = null;
                    }
                    else if (vm.searchOption == '4') {
                        vm.query.franchiseeId = 0;
                        vm.query.tagId = 0;
                        vm.query.callTypeId = 0;
                        vm.query.text = '';
                        vm.query.mappedFranchisee = null;
                        //vm.query.pageNumber = 1;
                        vm.query.callerId = null;
                        vm.query.transferToNumber = null;
                    }
                    else if (vm.searchOption == '5') {
                        vm.query.franchiseeId = 0;
                        vm.query.tagId = 0;
                        vm.query.callTypeId = 0;
                        vm.query.text = '';
                        vm.query.convertedLead = null;
                        //vm.query.pageNumber = 1;
                        vm.query.callerId = null;
                        vm.query.transferToNumber = null;
                    }
                    else if (vm.searchOption == '6') {
                        vm.query.franchiseeId = 0;
                        vm.query.callTypeId = 0;
                        vm.query.text = '';
                        vm.query.mappedFranchisee = null;
                        vm.query.convertedLead = null;
                        //vm.query.pageNumber = 1;
                        vm.query.callerId = null;
                        vm.query.transferToNumber = null;
                    }
                    else if (vm.searchOption == '7') {
                        vm.query.franchiseeId = 0;
                        vm.query.callTypeId = 0;
                        vm.query.text = '';
                        vm.query.mappedFranchisee = null;
                        vm.query.convertedLead = null;
                        //vm.query.pageNumber = 1;
                        vm.query.callerId = null;
                        vm.query.transferToNumber = null;
                    }
                    else if (vm.searchOption == '8') {
                        vm.query.franchiseeId = 0;
                        vm.query.callTypeId = 0;
                        vm.query.text = '';
                        vm.query.mappedFranchisee = null;
                        vm.query.convertedLead = null;
                        //vm.query.pageNumber = 1;
                        vm.query.callerId = null;
                        vm.query.transferToNumber = null;
                    }
                    else {
                        vm.query.callTypeId = 0;
                        vm.query.tagId = 0;
                        vm.query.franchiseeId = 0;
                        vm.query.convertedLead = null;
                        vm.query.mappedFranchisee = null;
                        //vm.query.pageNumber = 1;
                        vm.query.callerId = null;
                        vm.query.transferToNumber = null;
                    }

                    vm.queryLeadFlow = {
                        pageNumber: 1,
                        startDate: null,
                        endDate: null,
                        franchiseeId: 0,
                        callTypeId: 0,
                        pageSize: config.defaultPageSize,
                        text: '',
                        sort: { order: 0, propName: '' },
                    };
                }

                function resetSearch() {
                    vm.query.text = '';
                    vm.query.callTypeId = 0;
                    vm.query.franchiseeId = 0;
                    vm.query.startDate = null;
                    vm.query.convertedLead = null;
                    vm.query.tagId = 0;
                    vm.query.mappedFranchisee = null;
                    vm.query.endDate = null;
                    vm.searchOption = '';
                    vm.searchOptionForLeadFlow = '';
                    vm.query.idList = [];
                    vm.query.categoryIds = [];
                    vm.query.downloadColumnList = [];
                    vm.query.callerId = null;
                    vm.query.transferToNumber = null;

                    vm.query.downloadColumnList.push("Id");
                    vm.query.downloadColumnList.push("Date/Time Of Call");
                    vm.query.downloadColumnList.push("Dialed Number (dnis)");
                    vm.query.downloadColumnList.push("Caller ID (ani)");
                    vm.query.downloadColumnList.push("Transfer To Number");
                    vm.query.downloadColumnList.push("Phone Label");
                    vm.query.downloadColumnList.push("Ring Seconds");
                    vm.query.downloadColumnList.push("Ring Count");
                    vm.query.downloadColumnList.push("Recorded Seconds");
                    vm.query.downloadColumnList.push("Call Duration");
                    vm.query.downloadColumnList.push("Call Route");
                    vm.query.downloadColumnList.push("Call Flow Entered Zip");
                    vm.query.downloadColumnList.push("Call Note");
                    vm.query.downloadColumnList.push("Transcription Status");
                    vm.query.downloadColumnList.push("Missed Call");
                    vm.query.downloadColumnList.push("Find Me List");
                    vm.query.pageNumber = 1;

                    vm.queryLeadFlow = {
                        pageNumber: 1,
                        startDate: null,
                        endDate: null,
                        franchiseeId: 0,
                        callTypeId: 0,
                        pageSize: config.defaultPageSize,
                        text: '',
                        sort: { order: 0, propName: '' },
                        mappedFranchisee: null
                    };
                    $scope.$broadcast("reset-dates");
                }

                $scope.$on('clearDates', function (event) {
                    vm.query.startDate = null;
                    vm.query.endDate = null;
                    if (vm.isCallDetailsTab) {
                        getCallDetails();
                    }
                    else {
                        getLeadFlowDetails();
                    }
                });

                function getCallDetails() {
                    vm.query.categoryIds = [];
                    if (vm.query.idList.length > 0) {
                        angular.forEach(vm.query.idList, function (value) {
                            vm.query.categoryIds.push(value.id);
                        });
                    }
                    if (vm.query.callerId <= 0 || vm.query.callerId == null) {
                        vm.query.callerId = null;
                    }
                    if (vm.query.franchiseeId != "" && vm.query.franchiseeId != null && vm.query.franchiseeId != 0) {
                        angular.forEach(vm.franchiseeCollectionForSearch, function (value, key) {
                            if (vm.query.franchiseeId == value.display) {
                                vm.query.franchiseeId = value.id;
                                vm.franchiseeForDisplay = value.display;
                            }
                        })
                    }
                    if (vm.query.franchiseeId == undefined || vm.query.franchiseeId == null) {
                        vm.query.franchiseeId = 0;
                    }
                    return marketingLeadService.getCallDetails(vm.query).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.ids = [];
                            vm.callDetailList = result.data.collection;
                            vm.count = result.data.pagingModel.totalRecords;
                            vm.query.sort.order = result.data.filter.sortingOrder;
                            addResultToList(vm.callDetailList);
                            vm.query.franchiseeId = vm.franchiseeForDisplay;
                        }
                    });
                }

                function addResultToList() {
                    angular.forEach(vm.callDetailList, function (value, key) {
                        vm.ids.push(value.id);
                    })
                }

                function sorting(propName) {
                    vm.query.sort.propName = propName;
                    vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                    getCallDetails();
                };

                function pageChange() {
                    getCallDetails();
                };

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                        vm.franchiseeCollectionForSearch = result.data;
                    });
                }

                function getCallTypes() {
                    return marketingLeadService.getCallTypes().then(function (result) {
                        vm.callTypeCollection = result.data;
                    });
                }

                function getCategoryList() {
                    return marketingLeadService.getCategoryList().then(function (result) {
                        vm.categoryList = result.data;
                    });
                }

                function getTagList() {
                    return routingNumberService.getTagList().then(function (result) {
                        vm.tagList = result.data;
                    });
                }
                function changeTab(tabNo) {
                    if (tabNo == 1) {
                        $scope.$emit("update-title", "Call Details Report");
                        vm.isCallDetailsTab = true;
                        vm.isLeadFlowTab = false;
                        getCallDetails();
                        vm.selectedColumns = [];
                        vm.query.columnList = [];
                    }
                    else {
                        $scope.$emit("update-title", "Lead Flow Report");
                        vm.isCallDetailsTab = false;
                        vm.isLeadFlowTab = true;
                        prepareSearchOptionsForLead();
                        getLeadFlowDetails();
                    }
                }

                function getLeadFlowDetails() {

                    return marketingLeadService.getLeadFlowDetails(vm.queryLeadFlow).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.leadFlowList = result.data.collection;
                            vm.count = result.data.pagingModel.totalRecords;
                            vm.queryLeadFlow.sort.order = result.data.filter.sortingOrder;
                        }
                    });
                }
                function pageChangeForLeadFlow() {
                    getLeadFlowDetails();
                }

                function prepareSearchOptionsForLead() {
                    if (vm.currentRole == vm.Roles.SuperAdmin) {
                        vm.searchOptionsForLeadFlow.push({ display: 'Franchisee', value: '1' });
                    }
                    vm.searchOptionsForLeadFlow.push({ display: 'Mapped Franchisee', value: '5' });
                    vm.searchOptionsForLeadFlow.push({ display: 'Others', value: '3' });
                }

                function downloadLeadFlowDetails() {
                    return marketingLeadService.downloadLeadFlow(vm.queryLeadFlow).then(function (result) {
                        var fileName = "LeadFlow.xlsx";
                        fileService.downloadFile(result.data, fileName);
                        vm.downloading = false;
                    }, function () { vm.downloading = false; });
                }

                function sortingForLead(propName) {
                    vm.queryLeadFlow.sortingColumn = propName;
                    vm.queryLeadFlow.sortingOrder = (vm.queryLeadFlow.sortingOrder == 0) ? 1 : 0;
                    getLeadFlowDetails();
                };

                $scope.selectEventsColumn = {
                    onItemSelect: function (item) {
                        vm.query.downloadColumnList.push(item.id);
                        vm.selectedColumns.push(item.id);
                    },
                    onItemDeselect: function (item) {
                        var index = vm.selectedColumns.indexOf($filter('filter')(vm.selectedColumns, item.id, true)[0]);
                        vm.selectedColumns.splice(index, 1);
                        var index2 = vm.query.downloadColumnList.indexOf($filter('filter')(vm.query.downloadColumnList, item.id, true)[0]);
                        vm.query.downloadColumnList.splice(index2, 1);
                    },
                    onSelectAll: function (item) {
                        angular.forEach(vm.showColumns, function (value) {
                            vm.selectedColumns.push(value.id);
                            vm.query.downloadColumnList.push(value.label);
                        });
                    },
                    onDeselectAll: function (item) {
                        vm.selectedColumns = [];
                        vm.query.downloadColumnList = [];
                        vm.query.downloadColumnList.push("Id");
                        vm.query.downloadColumnList.push("Date/Time Of Call");
                        vm.query.downloadColumnList.push("Dialed Number (dnis)");
                        vm.query.downloadColumnList.push("Caller ID (ani)");
                        vm.query.downloadColumnList.push("Transfer To Number");
                        vm.query.downloadColumnList.push("Phone Label");
                        vm.query.downloadColumnList.push("Ring Seconds");
                        vm.query.downloadColumnList.push("Ring Count");
                        vm.query.downloadColumnList.push("Recorded Seconds");
                        vm.query.downloadColumnList.push("Call Duration");
                        vm.query.downloadColumnList.push("Call Route");
                        vm.query.downloadColumnList.push("Call Flow Entered Zip");
                        vm.query.downloadColumnList.push("Call Note");
                        vm.query.downloadColumnList.push("Transcription Status");
                        vm.query.downloadColumnList.push("Missed Call");
                        vm.query.downloadColumnList.push("Find Me List");
                    }
                }

                function addCallDetailsNotes() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/modal.add.call.details.notes.client.view.html',
                        controller: 'AddCallDetailsNotes',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    IsViewHistory: false
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                        getCallDetails();
                    });
                }

                function getCallNotesHistory() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/modal.add.call.details.notes.client.view.html',
                        controller: 'AddCallDetailsNotes',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    IsViewHistory: true
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                        getCallDetails();
                    });

                }

                function getNotes(callNote, callId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/show-notes.client.view.html',
                        controller: 'ShowNotesController',
                        controllerAs: 'vm',
                        size: 'lg',
                        backdrop: 'static',
                        resolve: {
                            modalParam: function () {
                                return {
                                    CallerId: callId,
                                    CallerNote: callNote,
                                    IsCallerNoteHistory: false
                                };
                            }
                        }
                    });
                }

                function getNotesHistory(callNote, callId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/show-notes.client.view.html',
                        controller: 'ShowNotesController',
                        controllerAs: 'vm',
                        size: 'lg',
                        backdrop: 'static',
                        resolve: {
                            modalParam: function () {
                                return {
                                    CallerId: callId,
                                    CallerNote: callNote,
                                    IsCallerNoteHistory: true
                                };
                            }
                        }
                    });
                }

                function getOfficeCollection() {
                    return marketingLeadService.getOfficeCollection().then(function (result) {
                        if (result != null && result.data != null) {
                            vm.officeCollection = result.data;
                            vm.officeCollection = vm.zeroPrefixedForOfficeDropdown.concat(vm.officeCollection);
                            return marketingLeadService.getFranchiseeNameValuePairCollection().then(function (result) {
                                if (result != null && result.data != null) {
                                    vm.franchiseeCollection = result.data;

                                    angular.forEach(vm.franchiseeCollection, function (value) {
                                        vm.officeCount = 0;
                                        angular.forEach(vm.officeCollection, function (value1) {
                                            if (value.display.toUpperCase() === value1.display.toUpperCase()) {
                                                vm.officeCollection.splice(vm.officeCount, 1);
                                            }
                                            vm.officeCount++;
                                        });

                                        if (value.isActive == true) {
                                            value.display = value.display.toUpperCase();
                                            vm.officeCollection.push(value);
                                        }
                                        else {
                                            value.display = "X- " + value.display.toUpperCase();
                                            //vm.closedOffice.push(value.display);
                                            vm.officeCollection.push(value);
                                        }
                                    });
                                }
                            });
                        }
                    });
                }

                $scope.$emit("update-title", "Call Details Report");
                $q.all([getTagList(), getCallDetails(), prepareSearchOptions(), getFranchiseeCollection(), getCallTypes(), prepareLeadOptions(),
                    prepareMappingOptions(), getCategoryList(), getOfficeCollection()]);

            }
        ]
    );
}());