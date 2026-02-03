(function () {
    'use strict';

    angular.module(OrganizationsConfiguration.moduleName).directive('validateBusinessid', ["FranchiseeService", function (franchiseeService) {
        return {
            restrict: 'A',
            require: '?ngModel',
            scope: {
                franchiseeId: '=',
                feedbackEnabled: '='
            },
            link: function (scope, elem, attrs, ctrl) {

                function checkDuplicateBusinessId() {
                    if (!scope.feedbackEnabled) {
                        ctrl.$setValidity("alreadyExist", true);
                    }
                    if (scope.feedbackEnabled && (elem.val() == null || elem.val() == "")) {
                        ctrl.$setValidity("alreadyExist", true);
                    }
                        return franchiseeService.isUniqueBusinessId(scope.franchiseeId, elem.val()).then(function (result) {
                            setValidity(result);
                        });
                }

                function setValidity(result) {
                    if (result.data == false) {
                        ctrl.$setValidity("alreadyExist", false);
                    }
                    else {
                        ctrl.$setValidity("alreadyExist", true);
                    }
                }

                $(elem).change(function () {
                    checkDuplicateBusinessId();
                });

            }
        }
    }]);
}());
