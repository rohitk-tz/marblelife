(function () {
    'use strict';

    angular.module(SalesConfiguration.moduleName).service("CustomerService", ["HttpWrapper", "APP_CONFIG", function (httpWrapper, config) {

        var baseUrl = "/sales/customer"

        function getCustomers(query) {
            return httpWrapper.get({
                url: baseUrl + "?filter.text=" + query.text + "&filter.dateCreated=" + query.dateCreated + "&filter.dateModified=" + query.dateModified
                    + "&filter.fromDate=" + query.fromDate + "&filter.toDate=" + query.toDate + "&filter.franchiseeId=" + query.franchiseeId
                    + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize + "&filter.receiveNotification=" + query.receiveNotification
            + "&filter.sortingColumn=" + query.sort.propName + "&filter.sortingOrder=" + query.sort.order + "&filter.advancedText=" + query.advancedText + "&filter.advancedSearchBy=" + query.advancedSearchBy
            })
        }

        function downloadCustomerList(query) {
            return httpWrapper.getFileByPost({ url: "/customer/download", data: query });
        }

        function saveCustomerFile(model) {
            return httpWrapper.post({ url: baseUrl + "/upload", data: model });
        }

        function getmarketingClassCollection() {
            return httpWrapper.get({ url: "/application/dropdown/GetMarketingClass" });
        }

        function getCustomerById(id) {
            return httpWrapper.get({ url: baseUrl + "/" + id });
        }

        function saveCustomer(customer) {
            return httpWrapper.post({ url: "/customer/save", data: customer });
        }

        function updateMarketingClass(id, classTypeId) {
            return httpWrapper.post({ url: "/customer/" + id + "/update", data: classTypeId });
        }


        return {
            getCustomers: getCustomers,
            downloadCustomerList: downloadCustomerList,
            saveCustomerFile: saveCustomerFile,
            getCustomerById: getCustomerById,
            saveCustomer: saveCustomer,
            getmarketingClassCollection: getmarketingClassCollection,
            updateMarketingClass: updateMarketingClass
        };
    }]);
})();