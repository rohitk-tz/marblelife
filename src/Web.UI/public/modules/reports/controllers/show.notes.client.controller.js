(function () {

    angular.module(OrganizationsConfiguration.moduleName).controller("ShowNotesController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "modalParam", "FranchiseeService", "MarketingLeadService", "APP_CONFIG",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, modalParam, franchiseeService, marketingLeadService, config) {

                var vm = this;
                vm.callerId = modalParam.CallerId;
                vm.callerNote = modalParam.CallerNote;
                vm.isCallerNoteHistory = modalParam.IsCallerNoteHistory;
                vm.close = function () {
                    $uibModalInstance.dismiss();
                };
                vm.count = 0;
                vm.getCallDetailNotes = getCallDetailNotes;

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
                    callerId: vm.callerId
                };

                function getCallDetailNotes() {
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

                $q.all([getCallDetailNotes()]);
            }
        ]
    );
}());