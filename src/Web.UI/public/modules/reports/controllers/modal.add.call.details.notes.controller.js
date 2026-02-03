(function () {
    angular.module(SchedulerConfiguration.moduleName).controller("AddCallDetailsNotes",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "modalParam", "Toaster", "MarketingLeadService", "APP_CONFIG", "GeoCodeService", "$filter", "$interval", "$uibModal", "FileService", "$timeout",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, modalParam, toaster, marketingLeadService, config, geoCodeService, $filter, $interval, $uibModal, fileService, $timeout) {
                var vm = this;
                vm.isViewHistory = modalParam.IsViewHistory;
                vm.openValidationModel = openValidationModel;
                vm.isCallNoteSaved = false;
                vm.downloadCallNotesHistory = downloadCallNotesHistory;
                vm.close = function () {
                    if (vm.isViewHistory) {
                        $uibModalInstance.dismiss();
                        return;
                    }

                    // List of fields to check for null or undefined
                    var fieldsToCheck = [
                        'callerId',
                        'zipCode',
                        'preferredContactNumber',
                        'email',
                        'firstName',
                        'lastName',
                        'company',
                        'office',
                        'resultingAction',
                        'houseNumber',
                        'street',
                        'city',
                        'state',
                        'country',
                        'callDetailsNotes'
                    ];

                    // Check if all fields are null, undefined, or empty string
                    var isAllFieldsEmpty = fieldsToCheck.every(function (field) {
                        var value = vm[field];
                        return value === null || value === undefined || value === "";
                    });

                    if (isAllFieldsEmpty) {
                        $uibModalInstance.dismiss();
                    } else {
                        openValidationModel();

                        var unbind = $rootScope.$on("IsCallNoteSaved", function (evt, data) {
                            if (data === true) {
                                vm.isCallNoteSaved = false;
                                $uibModalInstance.dismiss();
                                unbind(); // Clean up the listener
                            }
                        });
                    }
                };

                vm.query = {
                    zipCode: '',
                    countryId: 1,
                    userId: '',
                    pageNumber: 1,
                    startDate: null,
                    endDate: null,
                    franchiseeId: null,
                    tagId: null,
                    pageSize: config.defaultPageSize,
                    text: '',
                    sort: { order: 0, propName: '' },
                    callerId: null,
                    office: null,
                    resultingAction: null
                };
                vm.saveCallDetailsReportNote = {
                    callNote: '',
                    callerId: '',
                    preferredContactNumber: '',
                    firstName: '',
                    lastName: '',
                    company: '',
                    office: '',
                    zipCode: '',
                    resultingAction: '',
                    houseNumber: '',
                    street: '',
                    city: '',
                    state: '',
                    country: '',
                    email: ''
                }
                vm.editCallDetailsReportNote = {
                    id: null,
                    callNote: '',
                    callerId: '',
                    preferredContactNumber: '',
                    firstName: '',
                    lastName: '',
                    company: '',
                    office: '',
                    zipCode: '',
                    resultingAction: '',
                    houseNumber: '',
                    street: '',
                    city: '',
                    state: '',
                    country: '',
                    email: ''
                }

                vm.rowDetailQuery = {
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
                    marketingLeadId: null
                };

                vm.resultingActionList = [
                    { display: "Sent Information To Franchise", value: "Sent Information To Franchise" },
                    { display: "Scheduled Estimate - (date and time) is needed so the franchise can find the estimate in the calendar", value: "Scheduled Estimate - (date and time) is needed so the franchise can find the estimate in the calendar" },
                    { display: "Answered Their Questions", value: "Answered Their Questions" },
                    { display: "Placed MLD Order", value: "Placed MLD Order" },
                    { display: "Placed MLFS Order", value: "Placed MLFS Order" },
                    { display: "No Answer", value: "No Answer" },
                    { display: "Open Market", value: "Open Market" },
                    { display: "Other ", value: "Other " },
                ]
                vm.zeroPrefixed = [
                    { display: "0-INC", value: "0-INC", IsActive: true },
                    { display: "0-MLFS", value: "0-MLFS", IsActive: true },
                    { display: "Unassigned", value: "Unassigned", IsActive: true },
                    { display: "X- Open Territory", value: "X- Open Territory", IsActive: true }
                ]

                vm.count = 0;
                vm.save = save;
                vm.searchZipCode = searchZipCode;
                vm.pageChange = pageChange;
                vm.rowDetail = rowDetail;
                vm.expanded = false;
                vm.pagingOptions = config.pagingOptions;
                vm.getCallDetailNotes = getCallDetailNotes;
                vm.searchOptions = [];
                vm.searchOption = '';
                vm.refresh = refresh;
                vm.resetSearch = resetSearch;
                vm.resetSeachOption = resetSeachOption;
                vm.collapseRow = collapseRow;
                vm.clearPhoneCallData = clearPhoneCallData;
                vm.closedOffice = [];
                vm.getClosedOffice = getClosedOffice;
                vm.isOfficeClosed = false;
                vm.editCallNote = editCallNote;
                vm.isEditing = false;
                vm.edit = edit;
                vm.cancel = cancel;
                vm.debouncedSearchZipCode = debouncedSearchZipCode;
                var debounceTimeout = null;

                function prepareSearchOptions() {
                    vm.searchOptions.push({ display: 'Caller ID', value: '1' });
                    //vm.searchOptions.push({ display: 'Others', value: '2' });
                    vm.searchOptions.push({ display: 'Office', value: '3' });
                }


                function save() {
                    vm.saveCallDetailsReportNote.timestamp = $filter('date')(vm.timestamp, 'MM/dd/yyyy h:mm:ssa');
                    if ((vm.callerId == undefined || vm.callerId == "") && (vm.callDetailsNotes == undefined || vm.callDetailsNotes == "")) {
                        toaster.error("Please Enter Caller ID and Notes!!");
                        return;
                    }
                    else if (vm.callerId == undefined || vm.callerId == "") {
                        toaster.error("Please Enter Caller ID!!");
                        return;
                    }
                    else if (vm.callDetailsNotes == undefined || vm.callDetailsNotes == "") {
                        toaster.error("Please Enter Notes!!");
                        return;
                    }
                    else {
                        if (vm.callerId.split("_").length > 1) {
                            toaster.error("Please enter valid Caller ID!!");
                            return;
                        }
                        vm.saveCallDetailsReportNote.callNote = vm.callDetailsNotes;
                        vm.saveCallDetailsReportNote.callerId = vm.callerId;
                        vm.saveCallDetailsReportNote.preferredContactNumber = vm.preferredContactNumber;
                        vm.saveCallDetailsReportNote.firstName = vm.firstName;
                        vm.saveCallDetailsReportNote.lastName = vm.lastName;
                        vm.saveCallDetailsReportNote.company = vm.company;
                        vm.saveCallDetailsReportNote.office = vm.office;
                        vm.saveCallDetailsReportNote.zipCode = vm.zipCode;
                        vm.saveCallDetailsReportNote.resultingAction = vm.resultingAction;
                        vm.saveCallDetailsReportNote.houseNumber = vm.houseNumber;
                        vm.saveCallDetailsReportNote.street = vm.street;
                        vm.saveCallDetailsReportNote.city = vm.city;
                        vm.saveCallDetailsReportNote.state = vm.state;
                        vm.saveCallDetailsReportNote.country = vm.country;
                        vm.saveCallDetailsReportNote.email = vm.email;

                        return marketingLeadService.saveCallDetailsReportNotes(vm.saveCallDetailsReportNote).then(function (result) {
                            if (result != null && result.data != null) {
                                if (result.data) {
                                    toaster.show("Notes saved successfully!!");
                                    //$uibModalInstance.dismiss();
                                    clearPhoneCallData();
                                }
                                else {
                                    toaster.error("Error in Saving Notes!!");
                                }
                            }
                        });
                    }
                }

                function getCallDetailNotes() {
                    if (vm.isViewHistory) {
                        vm.query.zipCode = null;
                        vm.query.countryId = null;
                        vm.query.userId = null;
                        marketingLeadService.getCallDetailsReportNotes(vm.query).then(function (result) {
                            if (result != null && result.data != null) {
                                vm.notesList = result.data.collection.callDetailsReportNotesHistory;
                                vm.count = result.data.pagingModel.totalRecords;
                            }
                            else {
                                vm.notesList = [];
                            }
                        });
                    }
                }

                vm.removeArray = [{ display: "0-MLFS", value: "0-MLFS", IsActive: true }, { display: "FL-TAMPA", value: "FL-TAMPA", IsActive: false },
                    { display: "MI-GRAND RAPIDS", value: "MI-GRAND RAPIDS", IsActive: false }, { display: "MO-STLOUIS", value: "MO-STLOUIS", IsActive: false },
                    { display: "X- Open Territory", value: "X- Open Territory", IsActive: true }]
                function getOfficeCollection() {
                    return marketingLeadService.getOfficeCollection().then(function (result) {
                        if (result != null && result.data != null) {
                            vm.officeCollection = result.data;
                            vm.officeCollection = vm.zeroPrefixed.concat(vm.officeCollection);
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
                                            vm.closedOffice.push(value.display);
                                            vm.officeCollection.push(value);
                                        }
                                    });
                                }
                                for (var i = 0; i < vm.officeCollection.length; i++) {
                                    for (var j = 0; j < vm.removeArray.length; j++) {
                                        if (vm.officeCollection[i].display == vm.removeArray[j].display) {
                                            vm.officeCollection.splice(i, 1); // removes the matched element
                                        }
                                    }
                                }
                            });
                        }
                    });
                }

                function debouncedSearchZipCode(stateCollection) {
                    if (debounceTimeout) {
                        $timeout.cancel(debounceTimeout);
                    }

                    debounceTimeout = $timeout(function () {
                        searchZipCode(stateCollection);
                    }, 3000); // 3000 milliseconds = 3 seconds
                }

                function searchZipCode() {
                    if (vm.zipCode != "" && vm.zipCode != undefined) {
                        vm.query.zipCode = vm.zipCode;
                        //vm.query.userId = 1;
                        return geoCodeService.getGeoCode(vm.query).then(function (result) {
                            if (result != null && result.data != null && result.data.collection != null && result.data.collection.length != 0) {
                                vm.zipCodeData = result.data.collection;
                                vm.city = vm.zipCodeData[0].city;
                                vm.state = vm.zipCodeData[0].state;
                                vm.country = vm.zipCodeData[0].country;
                                vm.office = vm.zipCodeData[0].franchiseeName.toUpperCase();
                                getClosedOffice();
                            }
                            else {
                                vm.zipCodeData = null;
                                vm.city = null;
                                vm.state = null;
                                vm.country = null;
                                vm.office = null;
                                getClosedOffice();
                            }
                        });
                    }
                }

                function pageChange() {
                    getCallDetailNotes();
                };

                function refresh() {
                    vm.query.callerId = null;
                    getCallDetailNotes();
                }

                function resetSeachOption() {
                    if (vm.searchOption == '1') {
                        vm.query.franchiseeId = 0,
                            vm.query.tagId = 0,
                            vm.query.convertedLead = null,
                            vm.query.mappedFranchisee = null,
                            vm.query.callTypeId = 0,
                            vm.query.text = '',
                            vm.query.idList = [],
                            vm.query.categoryIds = [],
                            vm.query.columnList = [],
                            vm.query.downloadColumnList = [],
                            vm.query.callerId = null,
                            vm.query.office = null,
                            vm.query.resultingAction = null
                    }
                    else if (vm.searchOption == '2') {
                        vm.query.franchiseeId = 0,
                            vm.query.tagId = 0,
                            vm.query.convertedLead = null,
                            vm.query.mappedFranchisee = null,
                            vm.query.callTypeId = 0,
                            vm.query.text = '',
                            vm.query.idList = [],
                            vm.query.categoryIds = [],
                            vm.query.columnList = [],
                            vm.query.downloadColumnList = [],
                            vm.query.callerId = null,
                            vm.query.office = null,
                            vm.query.resultingAction = null
                    }
                    else if (vm.searchOption == '3') {
                        vm.query.franchiseeId = 0,
                            vm.query.tagId = 0,
                            vm.query.convertedLead = null,
                            vm.query.mappedFranchisee = null,
                            vm.query.callTypeId = 0,
                            vm.query.text = '',
                            vm.query.idList = [],
                            vm.query.categoryIds = [],
                            vm.query.columnList = [],
                            vm.query.downloadColumnList = [],
                            vm.query.callerId = null
                    }
                    else {
                        vm.query.franchiseeId = 0,
                            vm.query.tagId = 0,
                            vm.query.convertedLead = null,
                            vm.query.mappedFranchisee = null,
                            vm.query.callTypeId = 0,
                            vm.query.text = '',
                            vm.query.idList = [],
                            vm.query.categoryIds = [],
                            vm.query.columnList = [],
                            vm.query.downloadColumnList = [],
                            vm.query.callerId = null
                    }
                }

                function resetSearch() {
                    vm.query.pageNumber = 1,
                        vm.query.startDate = null,
                        vm.query.endDate = null,
                        vm.query.franchiseeId = 0,
                        vm.query.tagId = 0,
                        vm.query.convertedLead = null,
                        vm.query.mappedFranchisee = null,
                        vm.query.callTypeId = 0,
                        vm.query.pageSize = config.defaultPageSize,
                        vm.query.text = '',
                        vm.query.sort = { order: 0, propName: '' },
                        vm.query.idList = [],
                        vm.query.categoryIds = [],
                        vm.query.columnList = [],
                        vm.query.downloadColumnList = [],
                        vm.query.callerId = null,
                        vm.query.office = null,
                        vm.query.resultingAction = null,
                        vm.searchOption = '';
                    $scope.$broadcast("reset-dates");
                    getCallDetailNotes();
                }

                function rowDetail(marketingLeadId, item) {
                    item.isExpend = !item.isExpend;
                    if (marketingLeadId != null && marketingLeadId != undefined && marketingLeadId != "") {
                        vm.rowDetailQuery.marketingLeadId = marketingLeadId;
                    }
                    else {
                        vm.rowDetailQuery.marketingLeadId = null;
                    }
                    return marketingLeadService.getCallDetails(vm.rowDetailQuery).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.callDetailList = result.data.collection;
                        }
                    });
                };
                function collapseRow(item) {
                    item.isExpend = false;
                }

                var tick = function () {
                    vm.timestamp = Date.now();
                    vm.timestamp = $filter('date')(vm.timestamp, 'MM/dd/yyyy h:mm:ssa');
                }
                tick();
                $interval(tick, 1000);

                function clearPhoneCallData() {
                    vm.callDetailsNotes = null;
                    vm.callerId = null;
                    vm.preferredContactNumber = null;
                    vm.firstName = null;
                    vm.lastName = null;
                    vm.company = null;
                    vm.office = null;
                    vm.zipCode = null;
                    vm.resultingAction = null;
                    vm.houseNumber = null;
                    vm.street = null;
                    vm.city = null;
                    vm.state = null;
                    vm.country = null;
                    vm.email = null;
                }

                function getClosedOffice() {
                    for (var i = 0, len = vm.closedOffice.length; i < len; i++) {
                        if (vm.closedOffice[i] === vm.office) {
                            vm.isOfficeClosed = true;
                            break;
                        }
                        else {
                            vm.isOfficeClosed = false;
                        }
                    }
                }

                function openValidationModel() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/reports/views/modal.validate.call.details.notes.view.html',
                        controller: 'ModalValidationCallNotesController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        //size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    IsCallNoteSaved: false
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {

                    }, function () {

                    });
                }

                function downloadCallNotesHistory() {
                    vm.downloading = true;
                    return marketingLeadService.downloadCallNotesHistoryDetails(vm.query).then(function (result) {
                        var fileName = "CallNotesHistory.xlsx";
                        fileService.downloadFile(result.data, fileName);
                        vm.downloading = false;
                    }, function () {
                        vm.downloading = false;
                    });
                }

                function editCallNote(note) {
                    vm.editCallNoteId = note.id;
                    vm.callerId = note.callerId;
                    vm.zipCode = Number(note.zipCode);
                    vm.preferredContactNumber = note.preferredContactNumber;
                    vm.email = note.email;
                    vm.firstName = note.firstName;
                    vm.lastName = note.lastName;
                    vm.company = note.company;
                    vm.office = note.office;
                    vm.resultingAction = note.resultingAction;
                    vm.houseNumber = note.houseNumber;
                    vm.street = note.street;
                    vm.city = note.city;
                    vm.state = note.state;
                    vm.country = note.country;
                    vm.callDetailsNotes = note.callNote;
                    vm.isEditing = true;
                    vm.isViewHistory = false;
                }

                function edit() {
                    vm.editCallDetailsReportNote.timestamp = $filter('date')(vm.timestamp, 'MM/dd/yyyy h:mm:ssa');
                    if ((vm.callerId == undefined || vm.callerId == "") && (vm.callDetailsNotes == undefined || vm.callDetailsNotes == "")) {
                        toaster.error("Please Enter Caller ID and Notes!!");
                        return;
                    }
                    else if (vm.callerId == undefined || vm.callerId == "") {
                        toaster.error("Please Enter Caller ID!!");
                        return;
                    }
                    else if (vm.callDetailsNotes == undefined || vm.callDetailsNotes == "") {
                        toaster.error("Please Enter Notes!!");
                        return;
                    }
                    else {
                        if (vm.callerId.split("_").length > 1) {
                            toaster.error("Please enter valid Caller ID!!");
                            return;
                        }
                        vm.editCallDetailsReportNote.id = vm.editCallNoteId;
                        vm.editCallDetailsReportNote.callNote = vm.callDetailsNotes;
                        vm.editCallDetailsReportNote.callerId = vm.callerId;
                        vm.editCallDetailsReportNote.preferredContactNumber = vm.preferredContactNumber;
                        vm.editCallDetailsReportNote.firstName = vm.firstName;
                        vm.editCallDetailsReportNote.lastName = vm.lastName;
                        vm.editCallDetailsReportNote.company = vm.company;
                        vm.editCallDetailsReportNote.office = vm.office;
                        vm.editCallDetailsReportNote.zipCode = vm.zipCode;
                        vm.editCallDetailsReportNote.resultingAction = vm.resultingAction;
                        vm.editCallDetailsReportNote.houseNumber = vm.houseNumber;
                        vm.editCallDetailsReportNote.street = vm.street;
                        vm.editCallDetailsReportNote.city = vm.city;
                        vm.editCallDetailsReportNote.state = vm.state;
                        vm.editCallDetailsReportNote.country = vm.country;
                        vm.editCallDetailsReportNote.email = vm.email;

                        return marketingLeadService.editCallDetailsReportNotes(vm.editCallDetailsReportNote).then(function (result) {
                            if (result != null && result.data != null) {
                                if (result.data) {
                                    toaster.show("Notes Edited successfully!!");
                                    //$uibModalInstance.dismiss();
                                    vm.isEditing = false;
                                    vm.isViewHistory = true;
                                    vm.editCallNoteId = null;
                                    clearPhoneCallData();
                                    getCallDetailNotes();
                                }
                                else {
                                    toaster.error("Error in Editing Notes!!");
                                }
                            }
                        });
                    }
                }

                function cancel() {
                    vm.isViewHistory = true;
                }

                $q.all([getOfficeCollection(), getCallDetailNotes(), prepareSearchOptions()]);
            }
        ]
    );
}());