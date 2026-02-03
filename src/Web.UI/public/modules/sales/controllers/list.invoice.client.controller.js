(function () {
    'use strict';

    var SortColumns = {
        Id: 'Id',
        Name: 'Name',
        StartDate: 'StartDate',
        EndDate: 'EndDate',
        DueDate: 'DueDate',
        UploadedOn: 'UploadedOn',
        TotalSales: 'TotalSales',
        PaidAmount: 'PaidAmount',
        AccruedAmount: 'AccruedAmount',
        PayableAmount: 'PayableAmount',
        LastInvoiceId: 'LastInvoiceId',
        Status: 'Status',
        IsDownloaded: 'IsDownloaded',
        PaymentDate: 'PaymentDate',
        SEOCost: 'SEOCost',
        //Accounting: 'Accounting',
        LoanAndLoanInt: 'LoanAndLoanInt',
        ISQFT: 'ISQFT',
        WebSEO: 'WebSEO',
        BackUpPhone: 'BackUpPhone',
        AdFundOrRoyalty: 'AdFundOrRoyalty'
    };

    angular.module(SalesConfiguration.moduleName).controller("ListInvoiceController",
        ["$state", "$stateParams", "$q", "$scope", "$rootScope", "APP_CONFIG", "InvoiceService", "FranchiseeService",
            "$uibModal", "FileService", "LateFeeReportService", "FranchiseAccountCreditService",
            "URLAuthenticationServiceForEncryption", "Toaster", "$filter", "Notification",
            function ($state, $stateParams, $q, $scope, $rootScope, config, invoiceService, franchiseeService,
                $uibModal, fileService, lateFeeReportService, franchiseAccountCreditService, URLAuthenticationServiceForEncryption,
                toaster, $filter, notification) {
                var vm = this;
                vm.isFranchiseeChange = false;
                vm.InvoiceList = [];
                vm.query = {
                    text: '',
                    pageNumber: 1,
                    salesDataUploadId: 0,
                    franchiseeId: 0,
                    statusId: 0,
                    lateFeeTypeId: 0,
                    periodStartDate: null,
                    periodEndDate: null,
                    dueDateStart: null,
                    dueDateEnd: null,
                    paymentDateStart: null,
                    paymentDateEnd: null,
                    undownloadedInvoice: null,
                    //accounting: null,
                    fixedAccountingCharges: null,
                    variableAccountingCharges: null,
                    loanAndLoanInt: null,
                    iSQFT: null,
                    webSEO: null,
                    backUpCharges: null,
                    oneTimeCharges: null,
                    recruitingFee: null,
                    payrollProcessing: null,
                    isAdfund: null,
                    isRoyality: null,
                    pageSize: config.defaultPageSize,
                    sort: { order: 0, propName: '' }
                };
                vm.changeFranchisee = changeFranchisee;
                vm.payableForTotal = 0;
                vm.adfundForTotal = 0;
                vm.royalityForTotal = 0;
                vm.minAmountForTotal = 0;
                vm.query.salesDataUploadId = $stateParams.salesDataUploadId == null ? 0 : $stateParams.salesDataUploadId;
                vm.currentPage = vm.query.pageNumber;
                vm.count = 0;
                vm.pagingOptions = config.pagingOptions;
                vm.invoiceIds = [];
                vm.invoiceIdsWithStatus = [];
                vm.selectAll = false;
                vm.Roles = DataHelper.Role;
                vm.getInvoiceList = getInvoiceList;
                vm.pageChange = pageChange;
                vm.viewInvoice = viewInvoice;
                vm.sorting = sorting;
                vm.SortColumns = SortColumns;
                vm.resetSearch = resetSearch;
                vm.addInvoiceToList = addInvoiceToList;
                vm.downloadAdfundInvoice = downloadAdfundInvoice;
                vm.downloadRoyalityInvoice = downloadRoyalityInvoice;
                vm.downloadInvoiceList = downloadInvoiceList;
                vm.downloadAllInvoice = downloadAllInvoice;
                vm.downloadAllInvoiceList = downloadAllInvoiceList;
                vm.currencyCode = $rootScope.identity.currencyCode;
                vm.makePayment = makePayment;
                vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin; //To do replace with lookup
                vm.refresh = refresh;
                vm.statusId = DataHelper.InvoiceStatus.Paid; // 81 // To Do : Replace with lookup value.
                vm.searchOptions = []
                vm.searchOption = '';
                vm.resetSeachOption = resetSeachOption;
                vm.getDownloadedInvoice = getDownloadedInvoice;
                vm.currentRole = $rootScope.identity.roleId;
                vm.downloadAll = true;
                vm.selectedDownload = false;
                vm.query.isDownloaded = false;
                vm.downloadInvoiceIds = [];
                vm.downloadQuickBooks = downloadQuickBooks;
                vm.setFilters = setFilters;
                vm.editFranchisee = editFranchisee;
                vm.addViewNotes = addViewNotes;
                vm.addViewReconciliationNotes = addViewReconciliationNotes;

                function editFranchisee(franchiseeId) {
                    franchiseeId = URLAuthenticationServiceForEncryption.encrypt(franchiseeId.toString());
                    $state.go("core.layout.franchisee.edit", { id: franchiseeId });
                }
                if (!vm.isSuperAdmin) {
                    vm.isFranchiseeChange = true;
                    getAccountCredit();
                }
                function prepareSearchOptions() {
                    if (vm.currentRole == vm.Roles.SuperAdmin)
                        vm.searchOptions.push({ display: 'Invoice Id', value: '8' }, { display: 'Franchisee', value: '1' }, { display: 'Payment Date', value: '7' })

                    vm.searchOptions.push({ display: 'Status', value: '2' }, { display: 'End Date', value: '3' },
                        { display: 'Due Date', value: '4' }, { display: 'Late Fee Type', value: '5' },
                        { display: 'Undownloaded invoices', value: '9' },
                        { display: 'Loan & Loan Int', value: '11' }, { display: 'ISQFT', value: '12' },
                        { display: 'WebSEO', value: '13' }, { display: 'Back Up Charges', value: '14' },
                        { display: 'Adfund Invoices', value: '16' }, { display: 'Royalty Invoices', value: '17' },
                        { display: 'Others', value: '6' });
                }

                function refresh() {
                    vm.payableForTotal = 0;
                    vm.adfundForTotal = 0;
                    vm.royalityForTotal = 0;
                    vm.minAmountForTotal = 0;
                    //vm.query.accounting = null;
                    vm.query.loanAndLoanInt = null;
                    vm.query.iSQFT = null;
                    vm.query.webSEO = null;
                    vm.query.backUpCharges = null;
                    getInvoiceList();
                }
                function resetSeachOption() {
                    
                    if (vm.searchOption == '1') {
                        vm.isFranchiseeChange = false;
                        vm.query.statusId = 0;
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.paymentDateStart = null;
                        vm.query.paymentDateEnd = null;
                        vm.query.lateFeeTypeId = 0;
                        vm.payableForTotal = 0;
                        vm.adfundForTotal = 0;
                        vm.royalityForTotal = 0;
                        vm.minAmountForTotal = 0;
                        vm.query.undownloadedInvoice = null;
                        //vm.query.accounting = null;
                        vm.query.loanAndLoanInt = null;
                        vm.query.iSQFT = null;
                        vm.query.webSEO = null;
                        vm.query.backUpCharges = null;
                        vm.query.isAdfund = false;
                        vm.query.isRoyality = false;
                    }
                    else if (vm.searchOption == '2') {
                        vm.isFranchiseeChange = false;
                        vm.query.text = '';
                        vm.query.franchiseeId = 0;
                        vm.query.pageNumber = 1;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.paymentDateStart = null;
                        vm.query.paymentDateEnd = null;
                        vm.query.lateFeeTypeId = 0;
                        vm.payableForTotal = 0;
                        vm.adfundForTotal = 0;
                        vm.royalityForTotal = 0;
                        vm.minAmountForTotal = 0;
                        vm.query.undownloadedInvoice = null;
                        //vm.query.accounting = null;
                        vm.query.loanAndLoanInt = null;
                        vm.query.iSQFT = null;
                        vm.query.webSEO = null;
                        vm.query.backUpCharges = null;
                        vm.query.isAdfund = false;
                        vm.query.isRoyality = false;
                    }
                    else if (vm.searchOption == '3') {
                        vm.isFranchiseeChange = false;
                        vm.query.statusId = 0;
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.paymentDateStart = null;
                        vm.query.paymentDateEnd = null;
                        vm.query.franchiseeId = 0;
                        vm.query.lateFeeTypeId = 0;
                        vm.payableForTotal = 0;
                        vm.adfundForTotal = 0;
                        vm.royalityForTotal = 0;
                        vm.minAmountForTotal = 0;
                        vm.query.undownloadedInvoice = null;
                        //vm.query.accounting = null;
                        vm.query.loanAndLoanInt = null;
                        vm.query.iSQFT = null;
                        vm.query.webSEO = null;
                        vm.query.backUpCharges = null;
                        vm.query.isAdfund = false;
                        vm.query.isRoyality = false;
                    }
                    else if (vm.searchOption == '4') {
                        vm.isFranchiseeChange = false;
                        vm.query.statusId = 0;
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.periodStartDate = null;
                        vm.query.periodEndDate = null;
                        vm.query.paymentDateStart = null;
                        vm.query.paymentDateEnd = null;
                        vm.query.franchiseeId = 0;
                        vm.query.lateFeeTypeId = 0;
                        vm.payableForTotal = 0;
                        vm.adfundForTotal = 0;
                        vm.royalityForTotal = 0;
                        vm.minAmountForTotal = 0;
                        vm.query.undownloadedInvoice = null;
                        //vm.query.accounting = null;
                        vm.query.loanAndLoanInt = null;
                        vm.query.iSQFT = null;
                        vm.query.webSEO = null;
                        vm.query.backUpCharges = null;
                        vm.query.isAdfund = false;
                        vm.query.isRoyality = false;
                    }
                    else if (vm.searchOption == '5') {
                        vm.isFranchiseeChange = false;
                        vm.query.statusId = 0;
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.paymentDateStart = null;
                        vm.query.paymentDateEnd = null;
                        vm.query.franchiseeId = 0;
                        vm.payableForTotal = 0;
                        vm.adfundForTotal = 0;
                        vm.royalityForTotal = 0;
                        vm.minAmountForTotal = 0;
                        vm.query.undownloadedInvoice = null;
                        //vm.query.accounting = null;
                        vm.query.loanAndLoanInt = null;
                        vm.query.iSQFT = null;
                        vm.query.webSEO = null;
                        vm.query.backUpCharges = null;
                        vm.query.isAdfund = false;
                        vm.query.isRoyality = false;
                    }
                    else if (vm.searchOption == '7') {
                        vm.isFranchiseeChange = false;
                        vm.query.statusId = 0;
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.paymentDateStart = null;
                        vm.query.paymentDateEnd = null;
                        vm.query.franchiseeId = 0;
                        vm.query.lateFeeTypeId = 0;
                        vm.payableForTotal = 0;
                        vm.adfundForTotal = 0;
                        vm.royalityForTotal = 0;
                        vm.minAmountForTotal = 0;
                        vm.query.undownloadedInvoice = null;
                        //vm.query.accounting = null;
                        vm.query.loanAndLoanInt = null;
                        vm.query.iSQFT = null;
                        vm.query.webSEO = null;
                        vm.query.backUpCharges = null;
                        vm.query.isAdfund = false;
                        vm.query.isRoyality = false;
                    }
                    else if (vm.searchOption == '8') {
                        vm.isFranchiseeChange = false;
                        vm.query.statusId = 0;
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.paymentDateStart = null;
                        vm.query.paymentDateEnd = null;
                        vm.query.franchiseeId = 0;
                        vm.query.lateFeeTypeId = 0;
                        vm.payableForTotal = 0;
                        vm.adfundForTotal = 0;
                        vm.royalityForTotal = 0;
                        vm.minAmountForTotal = 0;
                        vm.query.undownloadedInvoice = null;
                        //vm.query.accounting = null;
                        vm.query.loanAndLoanInt = null;
                        vm.query.iSQFT = null;
                        vm.query.webSEO = null;
                        vm.query.backUpCharges = null;
                        vm.query.isAdfund = false;
                        vm.query.isRoyality = false;
                    }
                    else if (vm.searchOption == '9') {
                        vm.isFranchiseeChange = false;
                        vm.query.statusId = 0;
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.paymentDateStart = null;
                        vm.query.paymentDateEnd = null;
                        vm.query.franchiseeId = 0;
                        vm.query.lateFeeTypeId = 0;
                        vm.payableForTotal = 0;
                        vm.adfundForTotal = 0;
                        vm.royalityForTotal = 0;
                        vm.minAmountForTotal = 0;
                        //vm.query.accounting = null;
                        vm.query.loanAndLoanInt = null;
                        vm.query.iSQFT = null;
                        vm.query.webSEO = null;
                        vm.query.backUpCharges = null;
                        vm.query.isAdfund = false;
                        vm.query.isRoyality = false;
                    }
                    else if (vm.searchOption == '10') {
                        vm.isFranchiseeChange = false;
                        vm.query.statusId = 0;
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.paymentDateStart = null;
                        vm.query.paymentDateEnd = null;
                        vm.query.franchiseeId = 0;
                        vm.query.lateFeeTypeId = 0;
                        vm.payableForTotal = 0;
                        vm.adfundForTotal = 0;
                        vm.royalityForTotal = 0;
                        vm.minAmountForTotal = 0;
                        vm.query.undownloadedInvoice = null;
                        vm.query.loanAndLoanInt = null;
                        vm.query.iSQFT = null;
                        vm.query.webSEO = null;
                        vm.query.backUpCharges = null;
                        vm.query.isAdfund = false;
                        vm.query.isRoyality = false;
                    }
                    else if (vm.searchOption == '11') {
                        vm.isFranchiseeChange = false;
                        vm.query.statusId = 0;
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.paymentDateStart = null;
                        vm.query.paymentDateEnd = null;
                        vm.query.franchiseeId = 0;
                        vm.query.lateFeeTypeId = 0;
                        vm.payableForTotal = 0;
                        vm.adfundForTotal = 0;
                        vm.royalityForTotal = 0;
                        vm.minAmountForTotal = 0;
                        vm.query.undownloadedInvoice = null;
                        //vm.query.accounting = null;
                        vm.query.iSQFT = null;
                        vm.query.webSEO = null;
                        vm.query.backUpCharges = null;
                        vm.query.isAdfund = false;
                        vm.query.isRoyality = false;
                    }
                    else if (vm.searchOption == '12') {
                        vm.isFranchiseeChange = false;
                        vm.query.statusId = 0;
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.paymentDateStart = null;
                        vm.query.paymentDateEnd = null;
                        vm.query.franchiseeId = 0;
                        vm.query.lateFeeTypeId = 0;
                        vm.payableForTotal = 0;
                        vm.adfundForTotal = 0;
                        vm.royalityForTotal = 0;
                        vm.minAmountForTotal = 0;
                        vm.query.undownloadedInvoice = null;
                        //vm.query.accounting = null;
                        vm.query.loanAndLoanInt = null;
                        vm.query.webSEO = null;
                        vm.query.backUpCharges = null;
                        vm.query.isAdfund = false;
                        vm.query.isRoyality = false;
                    }
                    else if (vm.searchOption == '13') {
                        vm.isFranchiseeChange = false;
                        vm.query.statusId = 0;
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.paymentDateStart = null;
                        vm.query.paymentDateEnd = null;
                        vm.query.franchiseeId = 0;
                        vm.query.lateFeeTypeId = 0;
                        vm.payableForTotal = 0;
                        vm.adfundForTotal = 0;
                        vm.royalityForTotal = 0;
                        vm.minAmountForTotal = 0;
                        vm.query.undownloadedInvoice = null;
                        //vm.query.accounting = null;
                        vm.query.loanAndLoanInt = null;
                        vm.query.iSQFT = null;
                        vm.query.backUpCharges = null;
                        vm.query.isAdfund = false;
                        vm.query.isRoyality = false;
                    }
                    else if (vm.searchOption == '14') {
                        vm.isFranchiseeChange = false;
                        vm.query.statusId = 0;
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.paymentDateStart = null;
                        vm.query.paymentDateEnd = null;
                        vm.query.franchiseeId = 0;
                        vm.query.lateFeeTypeId = 0;
                        vm.payableForTotal = 0;
                        vm.adfundForTotal = 0;
                        vm.royalityForTotal = 0;
                        vm.minAmountForTotal = 0;
                        vm.query.undownloadedInvoice = null;
                        //vm.query.accounting = null;
                        vm.query.loanAndLoanInt = null;
                        vm.query.iSQFT = null;
                        vm.query.webSEO = null;
                        vm.query.isAdfund = false;
                        vm.query.isRoyality = false;
                    }
                    else if (vm.searchOption == '16') {
                        vm.isFranchiseeChange = false;
                        vm.query.statusId = 0;
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.paymentDateStart = null;
                        vm.query.paymentDateEnd = null;
                        vm.query.franchiseeId = 0;
                        vm.query.lateFeeTypeId = 0;
                        vm.payableForTotal = 0;
                        vm.adfundForTotal = 0;
                        vm.royalityForTotal = 0;
                        vm.minAmountForTotal = 0;
                        vm.query.undownloadedInvoice = null;
                        vm.query.accounting = null;
                        vm.query.loanAndLoanInt = null;
                        vm.query.iSQFT = null;
                        vm.query.webSEO = null;
                        vm.query.isAdfund = true;
                        vm.query.isRoyality = false;
                        setFilters();
                    }
                    else if (vm.searchOption == '17') {
                        vm.isFranchiseeChange = false;
                        vm.query.statusId = 0;
                        vm.query.text = '';
                        vm.query.pageNumber = 1;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.paymentDateStart = null;
                        vm.query.paymentDateEnd = null;
                        vm.query.franchiseeId = 0;
                        vm.query.lateFeeTypeId = 0;
                        vm.payableForTotal = 0;
                        vm.adfundForTotal = 0;
                        vm.royalityForTotal = 0;
                        vm.minAmountForTotal = 0;
                        vm.query.undownloadedInvoice = null;
                        vm.query.accounting = null;
                        vm.query.loanAndLoanInt = null;
                        vm.query.iSQFT = null;
                        vm.query.webSEO = null;
                        vm.query.isAdfund = false;
                        vm.query.isRoyality = true;
                        setFilters();
                    }
                    else {
                        vm.isFranchiseeChange = false;
                        vm.query.statusId = 0;
                        vm.query.franchiseeId = 0;
                        vm.query.pageNumber = 1;
                        vm.query.lateFeeTypeId = 0;
                        vm.query.dueDateStart = null;
                        vm.query.dueDateEnd = null;
                        vm.query.paymentDateStart = null;
                        vm.query.paymentDateEnd = null;
                        vm.query.isAdfund = false;
                        vm.query.isRoyality = false;
                    }
                }



                function getInvoiceStatus() {
                    return invoiceService.getInvoiceStatus().then(function (result) {
                        vm.invoiceStatus = result.data;
                    });
                }

                function getFranchiseeCollection() {
                    return franchiseeService.getFranchiseeNameValuePair().then(function (result) {
                        vm.franchiseeCollection = result.data;
                    });
                }

                function addInvoiceToList(item) {
                    vm.downloadAll = false;
                    vm.selectedDownload = true;
                    var index = vm.invoiceIds.indexOf(item.invoiceId);
                    if (index >= 0) {
                        item.selected = false;
                        vm.invoiceIds.splice(index, 1);
                        vm.invoiceIdsWithStatus.splice(index, 1);
                    }
                    else {
                        item.selected = true
                        vm.invoiceIds.push(item.invoiceId);
                        vm.invoiceIdsWithStatus.push({ Id: item.invoiceId, Status: item.status });
                    }
                    if (vm.invoiceIds.length <= 0) {
                        vm.downloadAll = true;
                        vm.selectedDownload = false;
                    }
                }

                function downloadQuickBooks() {
                    var countUnpaid = 0;
                    var unpaidIds = [];
                    if (vm.invoiceIds.length > 0) {
                        //angular.forEach(vm.invoiceIds, function (value, index) {
                        //    var invoice = $filter('filter')(vm.InvoiceList, { invoiceId: value }, true)[0];
                        //    if (invoice.status != "Paid") {
                        //        countUnpaid = countUnpaid + 1;
                        //        unpaidIds.push(invoice.invoiceId);
                        //    }
                        //});
                        angular.forEach(vm.invoiceIdsWithStatus, function (value, index) {
                            //var invoice = $filter('filter')(vm.InvoiceList, { invoiceId: value }, true)[0];
                            if (value.Status != "Paid" && value.Status != "$0-DUE") {
                                countUnpaid = countUnpaid + 1;
                                unpaidIds.push(value.Id);
                            }
                        });
                        if (countUnpaid > 0) {
                            var ids = unpaidIds.join(', ');
                            if (countUnpaid == 1)
                                notification.showAlert(ids + " is an Unpaid Invoice, Please uncheck the selected unpaid invoice to download CSV.");
                            else
                                notification.showAlert(ids + " are Unpaid Invoices, Please uncheck the selected unpaid invoices to download CSV.");
                            return;
                        }
                        downloadAdfundInvoice();
                        downloadRoyalityInvoice();
                        vm.downloading = false;
                        vm.selectedDownload = false;
                        vm.downloadAll = true;
                        ShowDownloadedInvoices();
                    }
                    else {
                        toaster.error("Please select Invoices to download!!");
                        return;
                    }
                }

                function downloadAdfundInvoice() {
                    vm.downloading = true;
                    return invoiceService.downloadAdfundInvoice(vm.invoiceIds).then(function (result) {
                        var fileName = "Adfund.csv";
                        if(result.data!=null)   
                            fileService.downloadFile(result.data, fileName);
                    }, function () {
                        vm.downloading = false; vm.selectedDownload = true;
                    });
                }

                function downloadRoyalityInvoice() {
                    vm.downloading = true;
                    return invoiceService.downloadRoyalityInvoice(vm.invoiceIds).then(function (result) {
                        var fileName = "Royalty.csv";
                        if (result.data != null)
                            fileService.downloadFile(result.data, fileName);
                    }, function () {
                        vm.downloading = false; vm.selectedDownload = true;
                    });
                }

                function downloadAllInvoice() {
                    vm.downloading = true;
                    return invoiceService.downloadAllInvoice(vm.query).then(function (result) {
                        var fileName = "QB_Invoice_Payment.xlsx";
                        fileService.downloadFile(result.data, fileName);
                        vm.downloading = false;
                        vm.selectedDownload = false;
                        vm.downloadAll = true;
                        ShowDownloadedInvoices();
                    }, function () { vm.downloading = false; });
                }

                function ShowDownloadedInvoices() {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/Downloaded-invoice-detail.client.view.html',
                        controller: 'DownloadedInvoiceDetailController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    InvoiceIds: vm.invoiceIds,
                                    Query: vm.query
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        vm.downloadAll = true;
                        vm.selectedDownload = false;
                        vm.query.isDownloaded = false;
                        getInvoiceList();
                    }, function () {
                        vm.downloadAll = true;
                        vm.query.isDownloaded = false;
                        vm.selectedDownload = false;
                        getInvoiceList();
                    });
                }

                //Download list

                function downloadAllInvoiceList() {
                    vm.downloading = true;
                    //vm.query.accounting = 0;
                    if (vm.query.undownloadedInvoice == "undownloadedInvoice") {
                        vm.query.undownloadedInvoice = "undownloadedInvoice"
                    }
                    else {
                        vm.query.undownloadedInvoice = 0;
                    }
                    vm.query.loanAndLoanInt = 0;
                    vm.query.iSQFT = 0;
                    vm.query.webSEO = 0;
                    vm.query.backUpCharges = 0;
                    vm.query.fixedAccountingCharges = 0;
                    vm.query.variableAccountingCharges = 0;
                    vm.query.oneTimeCharges = 0;
                    vm.query.recruitingFee = 0;
                    vm.query.payrollProcessing = 0;
                    return invoiceService.downloadAllInvoiceList(vm.query).then(function (result) {
                        var fileName = "invoice.xlsx";
                        fileService.downloadFile(result.data, fileName);
                        vm.downloading = false;
                    }, function () { vm.downloading = false; }).catch(function (err) {
                        vm.downloading = false; vm.selectedDownload = true;
                    });
                }

                function downloadInvoiceList() {
                    vm.downloading = true;
                    return invoiceService.downloadInvoiceList(vm.invoiceIds).then(function (result) {
                        var fileName = "invoice.xlsx";
                        fileService.downloadFile(result.data, fileName);
                        vm.downloading = false;
                        vm.selectedDownload = true;
                    }, function () {
                        vm.downloading = false; vm.selectedDownload = true;
                    }).catch(function (err) {
                        vm.downloading = false; vm.selectedDownload = true;
                    });
                }


                function getLateFeeItemType() {
                    return lateFeeReportService.getLateFeeItemType().then(function (result) {
                        vm.lateFeeType = result.data;
                    });
                }
                function resetSearch() {
                    vm.isFranchiseeChange = false;
                    vm.query.text = '';
                    vm.payableForTotal = 0;
                    vm.adfundForTotal = 0;
                    vm.royalityForTotal = 0;
                    vm.minAmountForTotal = 0;
                    vm.query.statusId = 0;
                    vm.query.franchiseeId = 0;
                    vm.query.periodStartDate = null;
                    vm.query.periodEndDate = null;
                    vm.query.paymentDateStart = null;
                    vm.query.paymentDateEnd = null;
                    vm.searchOption = '';
                    vm.query.lateFeeTypeId = 0;
                    vm.downloadAll = true;
                    vm.selectedDownload = false;
                    vm.invoiceIds = [];
                    clearSelection();
                    vm.query.pageNumber = 1;
                    vm.query.dueDateStart = null;
                    vm.query.dueDateEnd = null;
                    vm.query.undownloadedInvoice = null;
                    vm.query.accounting = null;
                    vm.query.loanAndLoanInt = null;
                    vm.query.iSQFT = null;
                    vm.query.webSEO = null;
                    vm.query.backUpCharges = null;
                    vm.query.isAdfund = null;
                    vm.query.isRoyality = null;
                    $scope.$broadcast("reset-dates");
                    setFilters();
                }

                $scope.$on('clearDates', function (event) {
                    vm.query.periodStartDate = null;
                    vm.query.periodEndDate = null;
                    vm.query.dueDateStart = null;
                    vm.query.dueDateEnd = null;
                    vm.query.paymentDateStart = null;
                    vm.query.paymentDateEnd = null;
                    setFilters();
                });

                function setFilters() {
                    if (!vm.isSuperAdmin) {
                        vm.isFranchiseeChange = true;
                        //getAccountCredit();
                    }
                    else if (!vm.isFranchiseeChange) {
                        vm.isFranchiseeChange = false;
                    }
                    vm.invoiceIds = [];
                    clearSelection();
                    getInvoiceList();
                }

                function addViewNotes(note) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/modal.list.invoice.notes.client.html',
                        controller: 'ModalListInvoiceNotesController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    ItemNote: note
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                    });
                }

                function addViewReconciliationNotes(note, id) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/modal.list.invoice.reconciliation-notes.client.html',
                        controller: 'ModalListInvoiceReconciliationNotesController',
                        controllerAs: 'vm',
                        size: 'lg',
                        resolve: {
                            modalParam: function () {
                                return {
                                    ReconciliationNotes: note,
                                    InvoiceId: id
                                };
                            }
                        },
                        backdrop: 'static',
                    });
                    modalInstance.result.then(function () {
                    }, function () {
                        getInvoiceList();
                    });
                }

                function getDownloadedInvoice() {
                    vm.query.undownloadedInvoice = "undownloadedInvoice";
                    getInvoiceList();
                }

                function getInvoiceList() {
                    return invoiceService.getInvoiceList(vm.query).then(function (result) {
                        vm.payableForTotal = 0;
                        vm.adfundForTotal = 0;
                        vm.royalityForTotal = 0;
                        vm.minAmountForTotal = 0;
                        if (result != null && result.data != null) {
                            vm.downloadInvoiceIds = [];
                            vm.downloadAll = true;
                            vm.selectedDownload = false;
                            vm.InvoiceList = result.data.collection;
                            vm.totolUnpaidAmount = result.data.totalUnPaidAmount;
                            angular.forEach(vm.InvoiceList, function (value, key) {
                                if (vm.invoiceIds.indexOf(value.invoiceId) >= 0) {
                                    value.selected = true;
                                }
                            });
                            vm.count = result.data.pagingModel.totalRecords;
                            vm.query.sort.order = result.data.filter.sortingOrder;
                            addInvoicesToList(vm.InvoiceList);
                        }
                    });
                }

                function clearSelection() {
                    angular.forEach(vm.InvoiceList, function (value, key) {
                        value.selected = false;
                    });
                }

                function addInvoicesToList() {
                    angular.forEach(vm.InvoiceList, function (value, key) {
                        vm.downloadInvoiceIds.push(value.invoiceId);
                    })
                }

                function viewInvoice(invoiceId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/franchisee-invoice-detail.client.view.html',
                        controller: 'FranchiseeInvoiceDetailController',
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
                        getInvoiceList();
                    }, function () {

                    });
                }

                function makePayment(franchiseeId, invoiceId, currencyRate, accountTypeId) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'modules/sales/views/payment.client.view.html',
                        controller: 'PaymentController',
                        controllerAs: 'vm',
                        backdrop: 'static',
                        size: 'md',
                        resolve: {
                            modalParam: function () {
                                return {
                                    FranchiseeId: franchiseeId,
                                    InvoiceId: invoiceId,
                                    CurrencyRate: currencyRate,
                                    AccountTypeId: accountTypeId,
                                };
                            }
                        }
                    });

                    modalInstance.result.then(function () {
                        getInvoiceList();
                    }, function () {

                    });
                }

                function sorting(propName) {
                    vm.query.sort.propName = propName;
                    vm.query.sort.order = (vm.query.sort.order == 0) ? 1 : 0;
                    getInvoiceList();
                };

                function pageChange() {
                    getInvoiceList();
                };

                $scope.$emit("update-title", "Invoice List");
                $q.all([getInvoiceList(), prepareSearchOptions(), getFranchiseeCollection(), getInvoiceStatus(), getLateFeeItemType()]);

                if ($(window).width() < 767) {
                    $scope.showFilterBox = true;
                }
                else {
                    $scope.showFilterBox = false;
                }

                function changeFranchisee() {
                    vm.isFranchiseeChange = true;
                    setFilters();
                    getAccountCredit();
                }
                function getAccountCredit() {
                    return franchiseAccountCreditService.getAccountCredit(parseInt(vm.query.franchiseeId), 1, 10).then(function (result) {
                        vm.list = result.data;
                        if (vm.list.collection.length > 0) {
                            vm.currencyRate = vm.list.collection[0].currencyRate;
                        }
                        vm.totalAmountByCategory = result.data.sumByCategory;
                        vm.count = result.data.pagingModel.totalRecords;
                        if (vm.list.collection.length > 0) {
                            vm.currencyRate = vm.list.collection[0].currencyRate;
                        }
                    })
                }
                $scope.showfilterBox = function () {
                    $scope.showFilterBox = !$scope.showFilterBox
                };

            }]);
}());