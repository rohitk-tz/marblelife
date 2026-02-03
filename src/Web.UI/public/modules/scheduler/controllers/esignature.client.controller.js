(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("eSignatureController",
        ["$scope", "$rootScope", "$state", "$q", "EstimateService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "AddressService", "FileService", "$filter", "CustomerService", "SchedulerService",
            function ($scope, $rootScope, $state, $q, estimateService, franchiseeService,
                notification, clock, toaster, addressService, fileService, $filter, customerService, schedulerService) {
                var vm = this;
                vm.signauturePad = signauturePad;
                vm.downloadInvoice = downloadInvoice;
                vm.data = {
                    invoiceNumbers: []
                };
                vm.data.name = $rootScope.identity.customerName;

                vm.alreadyUsedSign = false;
                vm.saveSignature = saveSignature;
                vm.clear = clear;

                vm.data.customerId = $rootScope.identity.customerId;
                vm.data.estimateInvoiceId = $rootScope.identity.estimateInvoiceId;
                vm.data.estimateCustomerId = $rootScope.identity.estimateCustomerId;
                vm.data.schedulerId = $rootScope.identity.schedulerId;
                vm.data.typeId = $rootScope.identity.typeId;
                //vm.data.jobOrginialSchedulerId = $rootScope.identity.jobOrginialSchedulerId;


                vm.invoiceNumberList = [];
                var invoiceNumbers = [];
                vm.allCheck = true;
                vm.selectAllCheckboxes = selectAllCheckboxes;
                vm.allCheckUncheck = allCheckUncheck;
                vm.downloadInvoice = downloadInvoice;
                vm.getAlreadySigned = getAlreadySigned;

                function saveSignature() {
                    var abc = document.getElementsByClassName("colors_sketch");
                    const context = abc[0].getContext('2d');
                    const pixelBuffer = new Uint32Array(
                        context.getImageData(0, 0, abc[0].width, abc[0].height).data.buffer
                    );
                    //var a = !pixelBuffer.some(color => color !== 0);
                    var value = false;
                    angular.forEach(pixelBuffer, function (value1) {

                        if (value1 != 0) {
                            value = true;
                        }
                    });
                    if (!value) {
                        toaster.error("Empty Signature Cannot be Save!!!");
                        return;
                    }
                    var base64 = abc[0].toDataURL();
                    $("input[name=base64String]").val(base64);
                    vm.data.invoiceNumbers = [];
                    vm.data.signature = base64;
                    vm.signature = base64;
                    vm.data.customerId = $rootScope.identity.customerId;
                    vm.data.estimateInvoiceId = $rootScope.identity.estimateInvoiceId;
                    vm.data.estimateCustomerId = $rootScope.identity.estimateCustomerId;
                    vm.data.schedulerId = $rootScope.identity.schedulerId;
                    vm.data.isPostSignature = $rootScope.identity.isPostSignature != null ? $rootScope.identity.isPostSignature : false;
                    vm.data.typeId = $rootScope.identity.isPostSignature ? DataHelper.CustomerSignatureType.AFTERCOMPLETITION : DataHelper.CustomerSignatureType.BEFORECOMPLETITION;
                    vm.data.isFromUrl = true;
                    vm.data.isFromJob = true;
                    angular.forEach(vm.invoiceNumberList, function (item) {
                        if (item.selected) {
                            vm.data.invoiceNumbers.push(item.invoiceNumber);
                        }
                    });
                    if (vm.data.invoiceNumbers.length == 0) {
                        toaster.error("Select atleast one invoice to sign!!!");
                        return;
                    }
                    vm.data.typeId = $rootScope.identity.typeId;
                    schedulerService.saveCustomerSignature(vm.data).then(function (result) {
                        toaster.show("Invoice has been Signed Successfully!!!");
                        getInvoiceDetails();
                    });
                    
                }

                function clear() {
                    var root = $rootScope;
                    var abc = document.getElementsByClassName("colors_sketch");
                    var context = abc[0].getContext('2d');
                    context.clearRect(0, 0, abc[0].width, abc[0].height);
                }

                function signauturePad() {
                    //alert('hello');
                }

                function selectAllCheckboxes() {
                    if (vm.allCheck) {
                        angular.forEach(vm.invoiceNumberList, function (item) {
                            item.selected = true;
                        });
                    }
                    else {
                        angular.forEach(vm.invoiceNumberList, function (item) {
                            item.selected = false;
                        });
                    }
                }

                function allCheckUncheck() {
                    angular.forEach(vm.invoiceNumberList, function (item) {
                        if (!item.selected) {
                            vm.allCheck = item.selected;
                        }
                    });
                }

                function downloadInvoice(invoiceNumber) {
                    var model = {};
                    model.schedulerId = vm.data.schedulerId;
                    var invoices = [];
                    invoices.push(invoiceNumber)
                    model.serviceInvoice = invoices;
                    return schedulerService.uploadCustomerInvoicesZipFile(model).then(function (result) {
                        vm.fileName = result.data;
                        if (vm.fileName == "") {
                            toaster.error("Please Save the Invoice to Download.");
                            return;
                        }
                        return fileService.downloadCustomerInvoice(result.data).then(function (result) {
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
                            toaster.show("Invoices downloaded successfully. Please check the Download folder to view all the images.");
                        });
                    });
                }

                function getInvoiceDetails() {
                    vm.estimateModel = {};
                    vm.estimateModel.id = vm.data.estimateInvoiceId;
                    vm.estimateModel.typeId = vm.data.typeId;
                    return schedulerService.getInvoiceInfo(vm.estimateModel).then(function (result) {
                        if (result != null && result.data != null) {
                            vm.estimateInvoiceInfo = result.data;

                            if (vm.estimateInvoiceInfo.invoiceNotesList.length == 0) {
                                vm.estimateInvoiceInfo.invoiceNotesList.push({ id: 0, notes: '', invoiceNumber: 1 });
                            }
                            vm.services = vm.estimateInvoiceInfo.serviceList;
                            getEstimateInfo();
                            //getting the name of invoices

                        }
                    });
                }

                function getEstimateInfo() {
                    return estimateService.getEstimateInfo(vm.estimateInvoiceInfo.estimateId).then(function (result) {
                        vm.estimateInfo = result.data;
                        vm.estimateInvoiceInfo.address = vm.estimateInfo.jobCustomer.address.addressLine1;
                        vm.estimateInvoiceInfo.city = vm.estimateInfo.jobCustomer.address.city + ', ' + vm.estimateInfo.jobCustomer.address.state
                            + ',' + vm.estimateInfo.jobCustomer.address.zipCode;
                        vm.estimateInvoiceInfo.lessDepositPercentage = vm.estimateInfo.lessDeposit;
                        vm.estimateInvoiceInfo.phoneNumber1 = vm.estimateInfo.jobCustomer.phoneNumber;
                        vm.estimateInvoiceInfo.customerName = vm.estimateInfo.jobCustomer.customerName;
                        vm.estimateInvoiceInfo.email = vm.estimateInfo.jobCustomer.email;
                        vm.estimateInvoiceId = vm.estimateInfo.estimateInvoiceId;
                        vm.jobSignature = vm.estimateInfo.jobSignature;
                        vm.allInvoiceNumbersSigned = vm.estimateInfo.allInvoiceNumbersSigned;

                        vm.jobSignaturePost = vm.estimateInfo.jobSignaturePost;
                        vm.allInvoiceNumbersSignedPost = vm.estimateInfo.allInvoiceNumbersSignedPost;

                        vm.estimateDate = moment((vm.estimateInfo.startDate)).format("MM/DD/YYYY");
                        if (vm.jobSignature.length > 0) {
                            vm.isSigned = true;
                        }
                        else {
                            vm.isSigned = false;
                        }
                        vm.allCheckedSignature = true;
                        invoiceNumbers = [];
                        vm.invoiceNumberList = [];
                        angular.forEach(vm.services, function (value1) {
                            var index = invoiceNumbers.indexOf($filter('filter')(invoiceNumbers, value1.invoiceNumber, true)[0]);
                            if (index == -1) {
                                var customerSplittedName = vm.estimateInvoiceInfo.customerName.split(' ');
                                var locationJoined = "";
                                var fileName = "";
                                angular.forEach(customerSplittedName.reverse(), function (custName) {
                                    fileName += custName + "_";
                                });
                                if (value1.serviceType == "CONCRETE-COATINGS" || value1.serviceType == "ENDURACRETE") {
                                    if (vm.isCustomerInvoice)
                                        fileName = fileName + "_CustomerConcreteOrder";
                                    else
                                        fileName = fileName + "_InternalConcreteOrder";
                                }
                                else {
                                    if (vm.isCustomerInvoice)
                                        fileName = fileName + "_CustomerOrder";
                                    else
                                        fileName = fileName + "_InternalOrder";
                                }
                                if (value1.locationIds.length > 0) {
                                    var locationsName = value1.locationIds;
                                    if (locationsName.length >= 2) {
                                        locationJoined = "_" + locationsName[0].id + "_" + locationsName[1].id;
                                    }
                                    else {
                                        locationJoined = "_" + locationsName[0].id;
                                    }
                                    fileName = fileName + locationJoined;
                                }
                                fileName = fileName + " (Invoice " + value1.invoiceNumber + "_" + vm.estimateDate + ")";
                                if (vm.data.typeId == 289) {
                                    var indexForAllInvoices = vm.allInvoiceNumbersSigned.indexOf($filter('filter')(vm.allInvoiceNumbersSigned, parseInt(value1.invoiceNumber), true)[0]);
                                    /*if (index == -1) {*/
                                    invoiceNumbers.push(value1.invoiceNumber);
                                    if (indexForAllInvoices != -1) {
                                        vm.invoiceNumberList.push({ invoiceNumber: value1.invoiceNumber, selected: true, label: fileName, isSignedCheckBox: false, isSigned: true });
                                    }
                                    else {
                                        vm.invoiceNumberList.push({ invoiceNumber: value1.invoiceNumber, selected: true, label: fileName, isSignedCheckBox: false, isSigned: false });
                                    }
                                    //}
                                }
                                else {
                                    var indexForAllInvoices = vm.allInvoiceNumbersSignedPost.indexOf($filter('filter')(vm.allInvoiceNumbersSignedPost, parseInt(value1.invoiceNumber), true)[0]);
                                    /*if (index == -1) {*/
                                    invoiceNumbers.push(value1.invoiceNumber);
                                    if (indexForAllInvoices != -1) {
                                        vm.invoiceNumberList.push({ invoiceNumber: value1.invoiceNumber, selected: true, label: fileName, isSignedCheckBox: false, isSigned: true });
                                    }
                                    else {
                                        vm.invoiceNumberList.push({ invoiceNumber: value1.invoiceNumber, selected: true, label: fileName, isSignedCheckBox: false, isSigned: false });
                                    }
                                    //}
                                }
                            }
                        });
                    });
                }

                function getAlreadySigned(invoiceNumber) {

                    if (vm.data.typeId == 290) {
                        getAlreadySignedPost(invoiceNumber)
                        return;
                    }
                    angular.forEach(vm.invoiceNumberList, function (item, index) {
                        item.isSignedCheckBox = false;
                    });
                    vm.alreadyUsedSign = true;
                    if (invoiceNumber == undefined) {
                        vm.jobSignatureSelected = vm.jobSignature[0];
                    }
                    else {
                        angular.forEach(vm.jobSignature, function (item, index) {
                            var indexInvoiceNumber = item.invoiceNumber.indexOf($filter('filter')(item.invoiceNumber, parseInt(invoiceNumber), true)[0]);
                            if (indexInvoiceNumber != -1) {
                                vm.jobSignatureSelected = vm.jobSignature[index];
                            }
                        });
                    }
                    angular.forEach(vm.jobSignatureSelected.invoiceNumber, function (item, index) {
                        var indexInvoiceNumber = vm.invoiceNumberList.indexOf($filter('filter')(vm.invoiceNumberList, { invoiceNumber: item.toString() }, true)[0]);
                        if (indexInvoiceNumber != -1) {
                            vm.invoiceNumberList[indexInvoiceNumber].isSignedCheckBox = vm.jobSignatureSelected.isSigned;
                        }
                    });
                    vm.signature = vm.jobSignatureSelected.signature;
                }

                function getAlreadySignedPost(invoiceNumber) {

                    angular.forEach(vm.invoiceNumberList, function (item, index) {
                        item.isSignedCheckBox = false;
                    });
                    vm.alreadyUsedSign = true;
                    if (invoiceNumber == undefined) {
                        vm.jobSignatureSelected = vm.jobSignaturePost[0];
                    }
                    else {
                        angular.forEach(vm.jobSignaturePost, function (item, index) {
                            var indexInvoiceNumber = item.invoiceNumber.indexOf($filter('filter')(item.invoiceNumber, parseInt(invoiceNumber), true)[0]);
                            if (indexInvoiceNumber != -1) {
                                vm.jobSignatureSelected = vm.jobSignaturePost[index];
                            }
                        });
                    }
                    angular.forEach(vm.jobSignatureSelected.invoiceNumber, function (item, index) {
                        var indexInvoiceNumber = vm.invoiceNumberList.indexOf($filter('filter')(vm.invoiceNumberList, { invoiceNumber: item.toString() }, true)[0]);
                        if (indexInvoiceNumber != -1) {
                            vm.invoiceNumberList[indexInvoiceNumber].isSignedCheckBox = vm.jobSignatureSelected.isSigned;
                        }
                    });
                    vm.signature = vm.jobSignatureSelected.signature;
                }

                $q.all([getInvoiceDetails()]);
            }]);
}());
