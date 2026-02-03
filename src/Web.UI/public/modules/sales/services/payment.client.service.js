(function () {
    'use strict';

    angular.module(SalesConfiguration.moduleName).service("PaymentService", ["HttpWrapper", "APP_CONFIG", function (httpWrapper, config) {

        var baseUrl = "/payment"

        function getInstrumentList(franchiseeId, paymentTypeId) {
            return httpWrapper.get({ url: baseUrl + "/" + franchiseeId + "/" + paymentTypeId + "/instrument/list" });
        }

        function saveChargeCard(chargeCardDetails, franchiseeId) {
            return httpWrapper.post({ url: baseUrl + "/" + franchiseeId + "/chargecard", data: chargeCardDetails });
        }
        function saveECheck(echeckDetails, franchiseeId) {
            return httpWrapper.post({ url: baseUrl + "/" + franchiseeId + "/echeck", data: echeckDetails });
        }
        function makePayment(paymentDetails, invoiceId, franchiseeId) {
            return httpWrapper.post({ url: baseUrl + "/" + franchiseeId + "/" + invoiceId + "/chargecard/new", data: paymentDetails });
        }
        function MakePaymentByECheck(paymentDetails, invoiceId, franchiseeId) {
            return httpWrapper.post({ url: baseUrl + "/" + franchiseeId + "/" + invoiceId + "/echeck", data: paymentDetails });
        }
        function makePaymentByOnFileCard(paymentDetails, invoiceId, franchiseeId) {
            return httpWrapper.post({ url: baseUrl + "/" + franchiseeId + "/" + invoiceId + "/chargecard/file", data: paymentDetails });
        }
        function getYears() {
            return httpWrapper.get({ url: "/application/dropdown/GetYears" });
        };

        function getMonths() {
            return httpWrapper.get({ url: "/application/dropdown/GetMonths" });
        };

        function getCardType() {
            return httpWrapper.get({ url: "/application/dropdown/GetCardType" });
        };
        function getInstrumentType() {
            return httpWrapper.get({ url: "/application/dropdown/GetInstrumentType" });
        };

        function getAccountType() {
            return httpWrapper.get({ url: "/application/dropdown/GetAccountType" });
        };
        function manageCard(instrumentIds, isActive) {
            return httpWrapper.post({ url: baseUrl + "/" + instrumentIds + "/manage/instrument", data: isActive });
        }

        function deleteCard(instrumentIds) {
            return httpWrapper.get({ url: baseUrl + "/" + instrumentIds + "/delete/instrument" });
        }

        function setPrimary(instrumentIds, franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/" + instrumentIds + "/" + franchiseeId + "/setPrimary" });
        }

        function checkExpiry(month, year) {
            return httpWrapper.get({ url: baseUrl + "/CheckExpiry/" + month + "/" + year });
        }

        function saveCheck(checkDetails, invoiceId, franchiseeId) {
            return httpWrapper.post({ url: baseUrl + "/" + franchiseeId + "/" + invoiceId + "/check/record", data: checkDetails });
        }
        function getFranchiseeInstrumentList(franchiseeId) {
            return httpWrapper.get({ url: baseUrl + "/" + franchiseeId + "/franchiseeinstrument/list" });
        }

        function adjustAccountCredit(franchiseeId, invoiceId) {
            return httpWrapper.get({ url: baseUrl + "/" + franchiseeId + "/" + invoiceId + "/adjust/account/Credit" });
        }

        function getInvoicePaymentInfo(invoiceId) {
            return httpWrapper.get({ url: baseUrl + "/payment?invoiceId=" + invoiceId });
        }

        return {
            saveChargeCard: saveChargeCard,
            getInstrumentList: getInstrumentList,
            getYears: getYears,
            getMonths: getMonths,
            getCardType: getCardType,
            manageCard: manageCard,
            deleteCard: deleteCard,
            setPrimary: setPrimary,
            checkExpiry: checkExpiry,
            makePayment: makePayment,
            saveCheck: saveCheck,
            getAccountType: getAccountType,
            makePaymentByOnFileCard: makePaymentByOnFileCard,
            MakePaymentByECheck: MakePaymentByECheck,
            getInstrumentType: getInstrumentType,
            saveECheck: saveECheck,
            getFranchiseeInstrumentList: getFranchiseeInstrumentList,
            adjustAccountCredit: adjustAccountCredit,
            getInvoicePaymentInfo: getInvoicePaymentInfo
        };
    }]);
})();