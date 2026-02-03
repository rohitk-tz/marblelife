(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).service("LocalStorageService", [function () {
        var locaStoragequery = null;
        var locaStorageToDoFranchiseeId = null;
        var locaStorageIsFromFranchiseeLevel = null;
        var locaStorageToDoFranchiseeName = null;
        var locaCustomerStoragequery = null;
        var localFranchiseeStoragequery = null;
        var localSchedulerStoragequery = null;
        var setStorageValue = function (query) {
            locaStoragequery = "";
            locaStoragequery = query
            return true;
        };

        var getStorageValue = function () {
            return locaStoragequery;
        };

        var setCustomerStorageValue = function (query) {
            locaCustomerStoragequery = "";
            locaCustomerStoragequery = query
            return true;
        };
        var setSchedulerStorageValue = function (query) {
            localSchedulerStoragequery = "";
            localSchedulerStoragequery = query
            return true;
        };

        var setFranchiseeStorageValue = function (query) {
            localFranchiseeStoragequery = "";
            localFranchiseeStoragequery = query
            return true;
        };

        var getFranchiseeStorageValue = function () {
            return localFranchiseeStoragequery;
        };

        var getCustomerStorageValue = function () {
            return locaCustomerStoragequery;
        };

        var getSchedulerStorageValue = function () {
            return localSchedulerStoragequery;
        };


        var setToDoListFranchiseeId = function (query) {
            locaStorageToDoFranchiseeId = "";
            locaStorageToDoFranchiseeId = query
            return true;
        };

        var getToDoListFranchiseeId = function () {
        return locaStorageToDoFranchiseeId;
        };

        var setToDoListFranchiseeName = function (query) {
            locaStorageToDoFranchiseeName = "";
            locaStorageToDoFranchiseeName = query;
            return true;
        };

        var getToDoListFranchiseeName = function () {
            return locaStorageToDoFranchiseeName;
        };

        

        var setToDoListIsFromFranchiseeLevel = function (query) {
            locaStorageIsFromFranchiseeLevel = "";
            locaStorageIsFromFranchiseeLevel = query;
            return true;
        };

        var getToDoListFranchiseeId = function () {
            return locaStorageToDoFranchiseeId;
        };
        var getToDoListIsFromFranchiseeLevel = function () {
            return locaStorageIsFromFranchiseeLevel;
        };
        return {
            setStorageValue: setStorageValue,
            getStorageValue: getStorageValue,
            setCustomerStorageValue: setCustomerStorageValue,
            getCustomerStorageValue: getCustomerStorageValue,
            setSchedulerStorageValue: setSchedulerStorageValue,
            getSchedulerStorageValue: getSchedulerStorageValue,
            getFranchiseeStorageValue: getFranchiseeStorageValue,
            setFranchiseeStorageValue: setFranchiseeStorageValue,
            setToDoListFranchiseeId: setToDoListFranchiseeId,
            setToDoListFranchiseeName: setToDoListFranchiseeName,
            setToDoListIsFromFranchiseeLevel: setToDoListIsFromFranchiseeLevel,
            getToDoListFranchiseeName: getToDoListFranchiseeName,
            getToDoListFranchiseeId: getToDoListFranchiseeId,
            getToDoListIsFromFranchiseeLevel: getToDoListIsFromFranchiseeLevel
        };

    }]);
}());