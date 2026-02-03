(function () {
    'use strict';

    angular.module(UsersConfiguration.moduleName).directive('validateEmail', ["UserService", function (UserService) {
        return {
            restrict: 'A',
            require: '?ngModel',
            scope:{
                userId: '='
            },
            link: function (scope, elem, attrs, ctrl) {

                function checkDuplicateEmail() {
                    if (elem.val() == null || elem.val() == "") { return; }
                    return UserService.isUniqueEmail(scope.userId, elem.val()).then(function (result) {
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
                    checkDuplicateEmail();
                });

            }
        }
    }]);
}());
