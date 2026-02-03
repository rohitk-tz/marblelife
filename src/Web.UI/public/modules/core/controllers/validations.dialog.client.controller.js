
(function () {

    angular.module(CoreConfiguration.moduleName).controller("ValidationsController",
        ["data", "$uibModalInstance", function (data, $uibModalInstance) {
            var vm = this;
            vm.messages = [];


            function prepareValidationMessages() {
                var errors = data.validation.errors;

                for (var index = 0; index < errors.length; index++) {
                    var obj = errors[index];

                    if (obj.error.message != null) {
                        vm.messages.push(obj.error.message);
                    }
                    else {
                        //vm.messages.push(resolveMessage(obj.error.type, obj.name, obj.error));
                        vm.messages.push(obj.name + " - " + obj.error);
                    }
                }
            }

            function resolveMessage(type, name, error) {
                var message = "";

                switch (type) {
                    case data.ValidationType.NullOrEmpty:
                        message = "required";
                        break;

                    case data.ValidationType.Length.LessThanMin:
                    case data.ValidationType.Length.GreaterThanMax:
                        message = "length should be between " + error.min + " - " + error.max;
                        break;

                    case data.ValidationType.Length.NotEqualsTo:
                        message = "length should be equals to " + error.length;
                        break;

                    case data.ValidationType.InvalidEmail:
                    case data.ValidationType.InvalidPhone:
                    case data.ValidationType.InvalidUrl:
                    case data.ValidationType.Date.Invalid:
                        message = "invalid data";
                        break;

                    case data.ValidationType.NotUnique:
                        message = "not unique";
                        break;

                    case data.ValidationType.NaN:
                        message = "not a number";
                        break;

                    case data.ValidationType.Password.Invalid:
                        message = "should contain the required characters.";
                        break;

                    case data.ValidationType.Password.SameAsOld:
                        message = "same as old password.";
                        break;

                    case data.ValidationType.Password.NotMatch:
                        message = "confirm password does not match.";
                        break;

                    case data.ValidationType.InvalidScriptStatus:
                        message = "script status invalid.";
                        break;
                }

                return name + " - " + message;
            }


            //vm.hide = function(){
            //    $mdDialog.hide();
            //}

            vm.cancel = function () {
                $uibModalInstance.close();
            };

            prepareValidationMessages();
        }]);

}());