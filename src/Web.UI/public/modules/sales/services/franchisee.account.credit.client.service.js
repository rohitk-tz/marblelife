(function () {
    'use strict';

    angular.module(SalesConfiguration.moduleName).service("FranchiseAccountCreditService", ["HttpWrapper", "APP_CONFIG", function (httpWrapper, config) {

        var baseUrl = "/payment/franchiseeAccountCredit"

        function getAccountCredit(franchiseeId, pageNumber, pageSize) {
            return httpWrapper.get({ url: baseUrl + "/" + franchiseeId + "/account/credit?pageNumber=" + pageNumber + "&pageSize=" + pageSize });
        }

        function saveAccountCredit(accountCredit, franchiseeId) {
            return httpWrapper.post({ url: baseUrl + "/" + franchiseeId + "/save/", data: accountCredit });
        }

        function deleteAccountCredit(accountCreditId) {
            return httpWrapper.get({ url: baseUrl + "/" + accountCreditId + "/delete" });
        }

        function removeCredit(accountCreditId) {
            return httpWrapper.get({ url: baseUrl + "/" + accountCreditId + "/remove" });
        }

        function getCreditType() {
            return httpWrapper.get({ url: "/application/dropdown/GetCreditType" });
        }

        function getAccountCreditforInvoice(franchiseeId, invoiceId) {
            return httpWrapper.get({ url: baseUrl + "/" + franchiseeId + "/get/" + invoiceId });
        }

        return {
            getAccountCredit: getAccountCredit,
            saveAccountCredit: saveAccountCredit,
            deleteAccountCredit: deleteAccountCredit,
            removeCredit: removeCredit,
            getCreditType: getCreditType,
            getAccountCreditforInvoice: getAccountCreditforInvoice
        };
    }]);
})();
