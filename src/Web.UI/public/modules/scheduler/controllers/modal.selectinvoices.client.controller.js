(function () {
    angular.module(SchedulerConfiguration.moduleName).controller("SelectInvoicesModal",
        ["$scope", "$rootScope", "$state", "$uibModalInstance", "modalParam", "SchedulerService", "FileService", "Toaster", "$filter", "FileService",
            function ($scope, $rootScope, $state, $uibModalInstance, modalParam, schedulerService, fileService, toaster, $filter, fileServicese) {
                var vm = this;
                vm.sendMailToCustomer = sendMailToCustomer;
                vm.MediaType = DataHelper.ScheduleType;
                vm.removeFile = removeFile;
                vm.data = {
                    serviceInvoice: [],
                    fileModel: []
                };
                vm.info = {
                    fileModel: []
                };
                vm.invoiceNumberList = [];
                var invoiceNumbers = [];
                vm.allCheck = true;
                vm.selectAllCheckboxes = selectAllCheckboxes;
                vm.sendMail = sendMail;
                vm.downloadInvoices = downloadInvoices;
                vm.downloadCustomerInvoices = downloadCustomerInvoices;
                vm.allCheckUncheck = allCheckUncheck;
                vm.data.schedulerId = modalParam.SchedulerId;
                vm.data.typeId = modalParam.TypeId;
                vm.services = modalParam.Services;
                vm.isFromAvailability = modalParam.IsFromAvailability;
                vm.customerName = modalParam.CustomerName;
                vm.customerId = modalParam.CustomerId;
                vm.modalName = modalParam.ModalName;
                vm.isFromMailSend = modalParam.IsFromMailSend;
                vm.buttonName = modalParam.ButtonName;
                vm.data.jobSchedulerId = modalParam.JobSchedulerId;
                vm.toEmail = modalParam.ToEmail;
                vm.toEmailId = modalParam.ToEmailId;
                vm.ccEmail = modalParam.CcEmail;
                vm.data.body = modalParam.MailBody;
                vm.isFromJob = modalParam.IsFromJob;
                vm.data.estimateInvoiceId = modalParam.EstimateInvoiceId;
                vm.data.userId = modalParam.PersonId;
                vm.estimateDate = modalParam.EstimateDate.substr(0, 10);
                
                if (vm.buttonName == "Send Mail") {
                    vm.isSendMail = true;
                    vm.isDownloadInvoices = false;
                    vm.isDownloadCustomerInvoices = false;
                    vm.isCustomerInvoice = true;
                    vm.message = "Please check the checkboxes for the invoices which you want to attach in the email.";
                }
                else if (vm.buttonName == "Send Mail To Customer") {
                    vm.isSendMail = false;
                    vm.isDownloadInvoices = false;
                    vm.isDownloadCustomerInvoices = false;
                    vm.isCustomerInvoice = true;
                    vm.isSendMailToCustomer = true;
                    vm.message = "Please check the checkboxes for the invoices which you want to attach in the email.";
                }
                else if (vm.buttonName == "Download Internal Invoice(s)") {
                    vm.isDownloadInvoices = true;
                    vm.isSendMail = false;
                    vm.isDownloadCustomerInvoices = false;
                    vm.isCustomerInvoice = false;
                    vm.message = "Please check the checkboxes for the invoices which you want to download.";
                }
                else if (vm.buttonName == "Download Customer Invoice(s)") {
                    vm.isDownloadCustomerInvoices = true;
                    vm.isDownloadInvoices = false;
                    vm.isSendMail = false;
                    vm.isCustomerInvoice = true;
                    vm.message = "Please check the checkboxes for the invoices which you want to download.";
                }
                //getting the name of invoices
                angular.forEach(vm.services, function (value1) {
                    var index = invoiceNumbers.indexOf($filter('filter')(invoiceNumbers, value1.invoiceNumber, true)[0]);
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
                    if (index == -1) {
                        invoiceNumbers.push(value1.invoiceNumber);
                        vm.invoiceNumberList.push({ invoiceNumber: value1.invoiceNumber, selected: true, label: fileName });
                    }
                });
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
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

                function sendMail() {
                    if (vm.toEmail == null || vm.toEmail == "" || vm.toEmail == undefined) {
                        toaster.error("No Email ID is available");
                        return;
                    }
                    else if (vm.ccEmail == null || vm.ccEmail == "" || vm.toEmail == undefined) {
                        toaster.error("No Email ID is available");
                        return;
                    }
                    var toEmailInLowerCase = vm.toEmail.toLowerCase();
                    var emailTo = toEmailInLowerCase.includes("@marblelife.com");
                    if (emailTo) {
                        toaster.error("@MarbleLife.com email ID is not valid email ID, please enter valid email ID else contact Admin");
                        return;
                    }
                    angular.forEach(vm.invoiceNumberList, function (item) {
                        if (item.selected) {
                            vm.data.serviceInvoice.push(item.invoiceNumber);
                        }
                    });
                    if (vm.data.serviceInvoice.length == 0 && vm.data.typeId == DataHelper.CustomerSignatureType.BEFORECOMPLETITION) {
                        toaster.error("Select atleast one invoice to send the mail!!!");
                        return;
                    }
                    vm.data.isDynamic = true;
                    vm.data.IsFromJob = vm.isFromJob;
                    vm.data.email = vm.toEmail;
                    vm.data.toEmailId = vm.toEmailId;
                    vm.data.cCEmail = vm.ccEmail;
                    vm.data.customerId = vm.customerId;
                    schedulerService.sendInvoiceToCustomer(vm.data).then(function (result) {
                        if (result != null && result.data != null && result.data == 1) {
                            toaster.show(result.message.message);
                        }
                        else if (result != null && result.data != null && result.data == -1) {
                            toaster.error(result.message.message);
                        }
                        else {
                            toaster.error(result.message.message);
                        }
                        $uibModalInstance.dismiss();
                    });
                }
                function downloadInvoices() {
                    angular.forEach(vm.invoiceNumberList, function (item) {
                        if (item.selected) {
                            vm.data.serviceInvoice.push(item.invoiceNumber);
                        }
                    });
                    if (vm.data.serviceInvoice.length == 0) {
                        toaster.error("Select atleast one invoice to download!!!");
                        return;
                    }
                    if (vm.data.schedulerId == undefined) {
                        vm.data.schedulerId = vm.data.jobSchedulerId;
                    }
                    return schedulerService.uploadInvoicesZipFile(vm.data).then(function (result) {
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
                            toaster.show("Invoices downloaded successfully. Please check the Download folder to view all the invoices.");
                            $uibModalInstance.dismiss();
                        });
                    });
                }
                function downloadCustomerInvoices() {
                    angular.forEach(vm.invoiceNumberList, function (item) {
                        if (item.selected) {
                            vm.data.serviceInvoice.push(item.invoiceNumber);
                        }
                    });
                    if (vm.data.serviceInvoice.length == 0) {
                        toaster.error("Select atleast one invoice to download!!!");
                        return;
                    }
                    //notification.showConfirm("Kindly make sure that you have saved the Invoice with the latest changes, unsaved items will not be downloaded. Do you want to  Download the Invoice?", "Download Invoices", function () {
                    return schedulerService.uploadCustomerInvoicesZipFile(vm.data).then(function (result) {
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
                            toaster.show("Invoices downloaded successfully. Please check the Download folder to view all the invoices.");
                            $uibModalInstance.dismiss();
                        });
                    });
                    //});
                }

                function removeFile(index) {
                    vm.info.fileModel.splice(index, 1);
                    //vm.data.fileModel.splice(index, 1);
                }

                function sendMailToCustomer() {
                    return schedulerService.changeCustomerAvailability(vm.data).then(function (result) {
                        toaster.show("Mail Send successfully.");
                        $uibModalInstance.dismiss();
                    });
                }

                vm.uploadFile = function (file) {
                    if (file != null) {
                        return fileService.uploadDyamicFile(file).then(function (result) {
                            toaster.show("File uploaded.");
                            vm.info.fileModel.push(result.data);
                            vm.data.fileModel = (vm.info.fileModel);
                        });
                    }
                }
            }
        ]
    );
}());