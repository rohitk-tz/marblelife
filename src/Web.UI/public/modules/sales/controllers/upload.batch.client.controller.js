(function () {

    angular.module(SalesConfiguration.moduleName).controller("UploadBatchController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "FranchiseeService",
            "BatchService", "FileService", "Toaster", "Clock", "Notification", "APP_CONFIG",
    function ($scope, $rootScope, $state, $q, $uibModalInstance, franchiseeService,
        batchService, fileService, toaster, clock, notification, config) {

        var vm = this;
        vm.batch = {};
        vm.feeProfile = {};
        vm.applyDateValidation = config.applyDateValidation;
        vm.feeProfiles = DataHelper.FeeProfile;
        vm.currentYear = moment(clock.now(), "DD/MM/YYYY").year();
        vm.batch.periodEndDate = null;
        vm.batch.periodStartDate = null;
        vm.batch.annualYear = null;

        vm.yearCollection = [];
        vm.yearCollection.push({ alias: '2017', display: '2017', id: '0', value: '2017' });
        vm.yearCollection.push({ alias: '2018', display: '2018', id: '1', value: '2018' });
        vm.Roles = DataHelper.Role;
        vm.SalesDataUploadStatus = DataHelper.SalesDataUploadStatus;
        vm.isSuperAdmin = $rootScope.identity.roleId == vm.Roles.SuperAdmin; //To do replace with lookup
        var isValidUpload = true;
        if (!vm.isSuperAdmin)
            vm.batch.franchiseeId = $rootScope.identity.organizationId;

        vm.save = function () {

            if (vm.batch.isAnnualUpload && vm.batch.annualFile == null) {
                toaster.error("Please upload annual audit file");
                return;
            }

            if (vm.batch.file == null) {
                toaster.error("Please upload a file");
                return;
            }
            if (vm.batch.annualUploadStartDate != null && vm.batch.annualUploadEndDate != null) {
                vm.batch.annualUploadEndDate = moment(vm.batch.annualUploadEndDate).format("MM/DD/YYYY");
                vm.batch.annualUploadStartDate = moment(vm.batch.annualUploadStartDate).format("MM/DD/YYYY");
            }

            vm.batch.periodEndDate = moment(vm.batch.periodEndDate).format("MM/DD/YYYY");
            vm.batch.periodStartDate = moment(vm.batch.periodStartDate).format("MM/DD/YYYY");

            var periodenddate = moment(vm.batch.periodEndDate).format("MM/DD/YYYY");
            var currentDate = moment(clock.now()).format("MM/DD/YYYY");

            //var dateForValidation = moment(vm.testDate).format("MM/DD/YYYY");

            if (!vm.applyDateValidation) {
                vm.batch.statusId = vm.SalesDataUploadStatus.Uploaded;
                saveBatch();
            }
            else {
                var valid = moment(periodenddate).isBefore(currentDate);
                if (valid || moment(periodenddate).isSame(currentDate)) {
                    //if (!vm.isSuperAdmin)
                    //    vm.batch.franchiseeId = $rootScope.identity.organizationId;                   

                    vm.batch.statusId = vm.SalesDataUploadStatus.Uploaded; //71; //To do replace with with lookup

                    var result = validateDateRange();
                    if ((!result && result != undefined) || !isValidUpload) {
                        notification.showAlert("Invalid dates range.");
                        return;
                    }
                    else {
                        saveBatch();
                    }
                } else {
                    notification.showAlert("Can't upload Sales Data for future dates.");
                    return;
                }
            }
        };

        function validateSelectedDates(isWeekly) {
            var result = true;
            var startDate = vm.batch.periodStartDate;
            var endDate = vm.batch.periodEndDate;

            if (!isWeekly) {
                result = validateDatesAreAValidMonth(startDate, endDate);
            }
            else {
                //need to determine start of week
                result = validateDatesAreAValidWeek(startDate, endDate);
            }
            return result;
        }

        function validateDatesAreAValidMonth(startDate, endDate) {
            var selectedDay = clock.getDay(startDate);
            var selectedMonth = clock.getMonth(startDate);
            var selectedYear = clock.getYear(startDate);

            var month = clock.getMonthDateRange(selectedYear, Number(selectedMonth) - 1);
            var startDateToCheck = moment(startDate).format("MM/DD/YYYY");
            var endDateToCheck = moment(endDate).format("MM/DD/YYYY");

            var monthStartDate = moment(month.start).format("MM/DD/YYYY");
            var monthEndDate = moment(month.end).format("MM/DD/YYYY");

            return (moment(startDateToCheck).isSame(monthStartDate) && moment(endDateToCheck).isSame(monthEndDate));
        }

        function validateDatesAreAValidWeek(startDate, endDate) {
            var cr = moment.localeData();
            var msd = moment(startDate), med = moment(endDate);
            if (startDate == null || endDate == null) return false;

            if (cr.weekdays(msd) != "Monday") {
                isValidUpload = false;
                return false;
            }

            if (cr.weekdays(med) != "Sunday") {
                isValidUpload = false;
                return false;
            }

            if (clock.diffInDays(msd, med) + 1 != 7) {
                isValidUpload = false;
                return false;
            }
            isValidUpload = true;
            return true;
        }



        function validateDateRange() {
            var result = true;
            var periodenddate = moment(vm.batch.periodEndDate).format("MM/DD/YYYY");
            var periodstartdate = moment(vm.batch.periodStartDate).format("MM/DD/YYYY");
            var currentDate = moment(clock.now()).format("MM/DD/YYYY");

            if (moment(periodenddate).isBefore(periodstartdate)) return false;
            if (moment(currentDate).isBefore(periodenddate)) return false;

            if (vm.feeProfile.salesBasedRoyalty == true) {
                if (vm.feeProfile.paymentFrequencyId == vm.feeProfiles.Monthly) // to do : replace with lookup
                {
                    result = validateSelectedDates(false);
                }
                else {
                    result = validateSelectedDates(true);
                }
            }
            else {
                result = validateSelectedDates();
            }

        }

        function saveBatch() {
            if (vm.isAnnualUploaded != undefined && !vm.isAnnualUploaded && (vm.batch.franchiseeId != '70' && vm.batch.franchiseeId != '74'&& vm.batch.franchiseeId != '78')) {
                toaster.error("Please upload annual audit file For Year "+ vm.annualUploadYears);
                return;
            }
            return batchService.saveBatch(vm.batch).then(function (result) {
                if (result.data != null && result.data == false) {
                    toaster.error(result.message.message);
                }
                else {
                    toaster.show(result.message.message);
                    $uibModalInstance.close(result);
                }
            });
        }

        function getFranchiseeFeeProfile() {
            if (vm.batch.franchiseeId == null) return;
            vm.feeProfile = {};
            vm.batch.periodEndDate = null;
            vm.batch.periodStartDate = null;
            return franchiseeService.getFranchiseeFeeProfile(vm.batch.franchiseeId).then(function (result) {
                if (result != null && result.data != null)
                    vm.feeProfile = result.data;
                if (vm.feeProfile != null) {
                    getFranchiseePriviousSalesDataUploaded();
                }
            });
        }

        function getFranchiseePriviousSalesDataUploaded() {
            if (vm.batch.franchiseeId == null) return;

            return batchService.getLastBatchUploaded(vm.batch.franchiseeId).then(function (result) {
                if (result != null && result.data != null)
                    vm.nextUploadStartDate = result.data;
                if (vm.nextUploadStartDate != null) {
                    getNextUploadRange(vm.nextUploadStartDate);
                }
            })
        }

        function getNextUploadRange(nextUploadStartDate) {
            if (vm.feeProfile.paymentFrequencyId == vm.feeProfiles.Weekly) {
                var startDate = nextUploadStartDate;
                var endDate = clock.addDays(startDate, 6);

                var result = validateDatesAreAValidWeek(startDate, endDate);
                if (result) {
                    vm.batch.periodStartDate = new Date(startDate);
                    vm.batch.periodEndDate = new Date(endDate);
                }
            }
            else {
                var startDate = nextUploadStartDate;
                var startDateNextMonth = clock.addMonths(startDate, 1);
                var endDate = clock.addDays(startDateNextMonth, -1);

                var result = validateDatesAreAValidMonth(startDate, endDate);
                if (result) {
                    vm.batch.periodStartDate = new Date(startDate);
                    vm.batch.periodEndDate = new Date(endDate);
                }
            }
            getAnnualUploadInfo();
        }


        function getAnnualUploadInfo() {
            if (vm.batch.periodStartDate == null || vm.batch.periodEndDate == null) return;
            vm.annualModel = {};
            vm.annualModel.paymentFrequencyid = vm.feeProfile.paymentFrequencyId;
            vm.annualModel.periodStartDate = moment(vm.batch.periodStartDate).format("MM/DD/YYYY");
            vm.annualModel.periodEndDate = moment(vm.batch.periodEndDate).format("MM/DD/YYYY");
            vm.annualModel.franchiseeId = vm.batch.franchiseeId;
            return batchService.getAnnualUploadInfo(vm.annualModel).then(function (result) {
                if (result != null && result.data != null) {
                    vm.batch.isAnnualUpload = false;
                    vm.isAnnualUploaded = result.data.isAnnualUpload;
                    vm.annualUploadYears = result.data.annualUploadYears
                    //vm.batch.isAnnualUpload = result.data.isAnnualUpload;
                    vm.batch.annualUploadStartDate = result.data.uploadStartDate;
                    vm.batch.annualUploadEndDate = result.data.uploadEndDate;
                    var date = moment(vm.batch.annualUploadStartDate).format("MM/DD/YYYY");
                    vm.currentYear = moment(date, "MM/DD/YYYY").year();
                }
            });
        }

        vm.uploadFile = function (file) {
            if (file != null) {
                return fileService.upload(file).then(function (result) {
                    toaster.show("File uploaded.");
                    vm.batch.file = result.data;
                });
            }
        }

        vm.uploadAnnualFile = function (file) {
            if (file != null) {
                return fileService.upload(file).then(function (result) {
                    toaster.show("Annual File uploaded.");
                    vm.batch.annualFile = result.data;
                });
            }
        }

        vm.close = function () {
            $uibModalInstance.dismiss();
        };

        $scope.$watch('vm.batch.franchiseeId', function (nv, ov) {
            if (nv == ov) return;

            $q.all([getFranchiseeFeeProfile(), getFranchiseePriviousSalesDataUploaded()]);

        });

        function getFranchiseeCollection() {
            return franchiseeService.getActiveFranchiseeList().then(function (result) {
                vm.franchiseeCollection = result.data;
            });
        }
        $q.all([getFranchiseeFeeProfile(), getFranchiseePriviousSalesDataUploaded(), getFranchiseeCollection()]);

    }]);
}());