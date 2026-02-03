(function () {

    angular.module(OrganizationsConfiguration.moduleName).directive("franchiseeServices", ["FranchiseeService", function (franchiseeService) {

        return {
            restrict: 'E',
            templateUrl: 'modules/organizations/views/franchisee-services.client.view.html',
            scope: {
                services: '=',
                isEdit: '='
            },
            link: function (scope, element) {

                //function getServiceTypeCollection()
                //{
                //    return franchiseeService.getServiceTypeCollection().then(function (result) {
                //        scope.serviceTypes = result.data;
                //        updateServiceTypeName();
                //    });
                //}

                //scope.selectedValue = function (selectedService, form) {
                //    if (selectedService != null && scope.services.length == 0)
                //        form.$setValidity("empty", false);
                //}
                //function checkExistingServices(serviceCollection, selectedService) {
                //    var result = false;
                //    serviceCollection.forEach(function (service, index) {
                //        if (service.name == selectedService) {
                //            result = true;
                //        }
                //    });
                //    return result;
                //}

                //scope.addService = function (form) {
                //    //check if service exist
                //    var isExist = checkExistingServices(scope.services, scope.selectedService.display);

                //    if (!isExist)
                //        scope.services.push({ name: scope.selectedService.display, serviceTypeId: scope.selectedService.value, calculateRoyalty: true });

                //    form.$setValidity("empty", true);
                //};

                //scope.remove = function (form, index) {
                //    scope.services.splice(index, 1);

                //    if (scope.services.length == 0)
                //        form.$setValidity("empty", false);
                //    else
                //        form.$setValidity("empty", true);
                //};

                //function updateServiceTypeName() {
                //    if (scope.isEdit == true && (scope.services == null || scope.services.length < 1)) {
                //        scope.serviceForm.$setValidity("empty", false);
                //        return;
                //    }

                //    angular.forEach(scope.services, function (service, index) {
                //        angular.forEach(scope.serviceTypes, function (type) {
                //            if (service.serviceTypeId == type.value) {
                //                scope.services[index].name = type.display;
                //            }
                //        });
                //    });
                //}

                //getServiceTypeCollection();
            }
        };
    }
    ]);

}());