(function () {
    angular.module(SchedulerConfiguration.moduleName).controller("ESignatureModal",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "SchedulerService", "Toaster", "$filter", "FileService",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, schedulerService, toaster, $filter, fileService) {
                var vm = this;
                vm.data = {
                    invoiceNumbers: []
                };
                vm.save = save;
                vm.clear = clear;
                vm.alreadyUsedSign = false;
                vm.getAlreadySigned = getAlreadySigned;
                $scope.editMode = false;
                vm.data.customerId = modalParam.CustomerId;
                vm.data.estimateInvoiceId = modalParam.EstimateInvoiceId;
                vm.data.schedulerId = modalParam.SchedulerId;
                vm.customerName = modalParam.CustomerName;
                vm.data.name = modalParam.CustomerName;
                vm.data.jobSchedulerId = modalParam.JobSchedulerId;
                vm.jobSignature = modalParam.JobSignature;
                vm.allInvoiceNumbersSigned = modalParam.AllInvoiceNumbersSigned;
                vm.allInvoiceNumbersSignedForEstimate = modalParam.InvoiceNumberSignedForEstimate;
                vm.data.isFromJob = modalParam.IsFromJob;
                vm.data.jobSchedulerId = modalParam.JobSchedulerId;
                vm.data.jobOrginialSchedulerId = modalParam.JobOrginialSchedulerId;
                vm.data.typeId = modalParam.TypeId;

                if (vm.jobSignature != undefined) {
                    if (vm.jobSignature.length > 0) {
                        vm.isSigned = true;
                    }
                    else {
                        vm.isSigned = false;
                    }
                }

                vm.data.estimateInvoiceId = modalParam.EstimateInvoiceId;
                vm.services = modalParam.Services;
                vm.invoiceNumberList = [];
                var invoiceNumbers = [];
                vm.allCheck = true;
                vm.selectAllCheckboxes = selectAllCheckboxes;
                vm.allCheckUncheck = allCheckUncheck;
                vm.downloadInvoice = downloadInvoice;
                vm.estimateDate = modalParam.EstimateDate.substr(0, 10);
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
                function save() {
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
                    vm.data.signature = base64;
                    vm.signature = base64;
                    vm.data.invoiceNumbers = [];
                    angular.forEach(vm.invoiceNumberList, function (item) {
                        if (item.selected) {
                            vm.data.invoiceNumbers.push(item.invoiceNumber);
                        }
                    });
                    if (vm.data.invoiceNumbers.length == 0) {
                        toaster.error("Select atleast one invoice to sign!!!");
                        return;
                    }
                    vm.data.isFromUrl = false;

                    schedulerService.saveCustomerSignature(vm.data).then(function (result) {
                        toaster.show("Invoice has been Signed Successfully!!!");
                        //$uibModalInstance.dismiss();
                    });
                }
                function clear() {
                    var abc = document.getElementsByClassName("colors_sketch");
                    var context = abc[0].getContext('2d');
                    context.clearRect(0, 0, abc[0].width, abc[0].height);
                }
                function getAlreadySigned(invoiceNumber) {
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
                //getting the name of invoices
                vm.allCheckedSignature = true;
                angular.forEach(vm.services, function (value1) {
                    var index = invoiceNumbers.indexOf($filter('filter')(invoiceNumbers, value1.invoiceNumber, true)[0]);
                    if (index == -1) {
                        var customerSplittedName = vm.customerName.split(' ');
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
                        var indexForAllInvoices = -1;
                        var indexForEstimateInvoice = -1;
                        if (vm.allInvoiceNumbersSigned != undefined) {
                            indexForAllInvoices = vm.allInvoiceNumbersSigned.indexOf($filter('filter')(vm.allInvoiceNumbersSigned, parseInt(value1.invoiceNumber), true)[0]);
                        }
                        if (vm.allInvoiceNumbersSignedForEstimate != undefined) {
                            indexForEstimateInvoice = vm.allInvoiceNumbersSignedForEstimate.indexOf($filter('filter')(vm.allInvoiceNumbersSignedForEstimate, parseInt(value1.invoiceNumber), true)[0]);
                            if (indexForEstimateInvoice != -1) {
                                invoiceNumbers.push(value1.invoiceNumber);
                                vm.invoiceNumberList.push({ invoiceNumber: value1.invoiceNumber, selected: true, label: fileName, isSignedCheckBox: false, isSigned: true });
                            }
                        }
                        else {
                            if (index == -1) {
                                invoiceNumbers.push(value1.invoiceNumber);
                                if (indexForAllInvoices != -1) {
                                    vm.invoiceNumberList.push({ invoiceNumber: value1.invoiceNumber, selected: true, label: fileName, isSignedCheckBox: false, isSigned: true });
                                }
                                else {
                                    vm.invoiceNumberList.push({ invoiceNumber: value1.invoiceNumber, selected: true, label: fileName, isSignedCheckBox: false, isSigned: false });
                                }
                            }
                        }
                    }
                });

                function downloadInvoice(invoiceNumber) {
                    var model = {};
                    //model.schedulerId = vm.data.;
                    var invoices = [];
                    invoices.push(invoiceNumber)
                    model.serviceInvoice = invoices;
                    model.schedulerId = vm.data.schedulerId;
                    model.typeId = vm.data.typeId;
                    if (vm.data.schedulerId == undefined) {
                        model.schedulerId = vm.data.jobSchedulerId;
                    }
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
            }
        ]
    );
}());