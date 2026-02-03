(function () {
    'use strict';

    angular.module(UsersConfiguration.moduleName).directive('validateUsername', ["UserService", function (UserService) {
        return {
            restrict: 'A',
            require: '?ngModel',
            scope: {
                userId: '='
            },
            link: function (scope, elem, attrs, ctrl) {

                function checkDuplicateUserName() {
                    if (elem.val() == null || elem.val() == "") { return; }
                    return UserService.isUniqueUserName(scope.userId, elem.val()).then(function (result) {
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
                    checkDuplicateUserName();
                });

            }
        }
    }]);
}());
