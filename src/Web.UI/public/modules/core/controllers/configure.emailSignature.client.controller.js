(function () {
    'use strict';
    angular.module(CoreConfiguration.moduleName).controller("EmailSignatureController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "UserService", "Toaster", 
        function ($scope, $rootScope, $state, $q, $uibModalInstance, userService, toaster) {
            var vm = this;
            vm.getEmailSignaturesCollection = getEmailSignaturesCollection;
            vm.addNewSignature = addNewSignature;
            vm.removeSignature = removeSignature;
            vm.saveSignatures = saveSignatures;
            vm.checkDefault = checkDefault;
            vm.close = function () {
                $uibModalInstance.dismiss();
            };

            function getEmailSignaturesCollection() {
                return userService.getEmailSignaturesCollection().then(function (result) {
                    vm.signatureList = result.data.userSignatureEditModel;
                    if (vm.signatureList.length == 0) {
                        vm.signatureList.push(
                            {
                                signature: '',
                                signatureName: '',
                                isDefault: true,
                                isActive: true
                            }
                        );
                    }
                });
            }

            function addNewSignature()
            {
                vm.signatureList.push(
                    {
                        signature: '',
                        signatureName: '',
                        isDefault: false,
                        isActive: true
                    }
                );
            }

            function removeSignature(index) {
                vm.signatureList.splice(index, 1);
            }

            function saveSignatures() {
                var countSignature = 0;
                var countSignatureName = 0;
                var defaultCount = 0;
                if (vm.signatureList.length > 0) {
                    angular.forEach(vm.signatureList, function (value, index) {
                        if (value.signature == "" || value.signature == null){
                            countSignature = countSignature + 1;
                        }
                        if (value.signatureName == "" || value.signatureName == null) {
                            countSignatureName = countSignatureName + 1;
                        }
                        if (value.isDefault) {
                            defaultCount = defaultCount + 1;
                        }
                    });
                    if (countSignatureName > 0 && countSignature > 0 && defaultCount != 1) {
                        toaster.error("Signature Name and Signature cannot be blank!");
                        return;
                    }
                    else if (countSignatureName > 0) {
                        toaster.error("Signature Name cannot be blank!");
                        return;
                    }
                    else if (countSignature > 0) {
                        toaster.error("Signature cannot be blank!");
                        return;
                    }
                    else if (defaultCount != 1)
                    {
                        toaster.error("It is mandatory to mark one Signature as Default!");
                        return;
                    }
                    else {
                        var signatures = {};
                        signatures.userSignatureSaveModel = vm.signatureList;
                        return userService.saveEmailSignaturesCollection(signatures).then(function (result)
                        {
                            if (result.data) {
                                toaster.show("Email Signatures Saved Successfully!!");
                                $uibModalInstance.dismiss();
                            }
                        });
                    }
                }
            }

            function checkDefault(index) {
                angular.forEach(vm.signatureList, function (value, indexLoop) {
                    if (index == indexLoop) {
                        value.isDefault = true;
                    }
                    else {
                        value.isDefault = false;
                    }
                })

            }

            $q.all([getEmailSignaturesCollection()]);
        }]);
}());