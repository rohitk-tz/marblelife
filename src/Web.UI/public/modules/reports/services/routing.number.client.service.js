(function () {
    'use strict';

    angular.module(ReportsConfiguration.moduleName).service("RoutingNumberService", ["HttpWrapper", function (httpWrapper) {
        var baseUrl = "/marketingLead";

        function getRoutingNumbers(query) {
            return httpWrapper.get({
                url: baseUrl + "/routingNumber?filter.text=" + query.text + "&filter.franchiseeId=" + query.franchiseeId
                             + "&filter.tagId=" + query.tagId
                             + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize + "&filter.sortingColumn="
                             + query.sort.propName + "&filter.sortingOrder=" + query.sort.order
            });
        }

        function updateFranchisee(id, franchiseeId) {
            return httpWrapper.post({ url: "/routingNumber/" + id + "/update", data: franchiseeId });
        }

        function updateTag(id, tagid) {
            return httpWrapper.post({ url: "/routingNumber/" + id + "/update/tag", data: tagid });
        }

        function downloadPhoneLabels(query) {
            return httpWrapper.getFileByPost({ url: "/routingNumber/download", data: query });
        }

        function getTagList() {
            return httpWrapper.get({ url: "/application/dropdown/GetTagList" });
        }

        return {
            getRoutingNumbers: getRoutingNumbers,
            updateFranchisee: updateFranchisee,
            downloadPhoneLabels: downloadPhoneLabels,
            updateTag: updateTag,
            getTagList: getTagList
        };
    }]);
})();
