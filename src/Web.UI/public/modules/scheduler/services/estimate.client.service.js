(function () {
    'use strict';

    angular.module(SchedulerConfiguration.moduleName).service("EstimateService", ["HttpWrapper", "APP_CONFIG", function (httpWrapper, config) {

        var baseUrl = "/scheduler/estimate"

        function saveEstimate(estimate) {
            return httpWrapper.post({ url: baseUrl + "/SaveEstimate", data: estimate });
        }

        function getEstimateInfo(id) {
            return httpWrapper.get({ url: baseUrl + "/get/" + id + "/estimate" });
        }

        function deleteEstimate(id) {
            return httpWrapper.delete({ url: baseUrl + "/delete/" + id + "/estimate" });
        }

        function getVacationInfo(id) {
            return httpWrapper.get({ url: baseUrl + "/get/" + id + "/vacation" });
        }
        function getMeetingInfo(id) {
            return httpWrapper.get({ url: baseUrl + "/get/" + id + "/meeting" });
        }
        function saveVacation(vacation) {
            return httpWrapper.post({ url: baseUrl + "/SaveVacation", data: vacation });
        }
        function saveMeeting(vacation) {
            return httpWrapper.post({ url: baseUrl + "/SaveMeeting", data: vacation });
        }

        function deleteVacation(id) {
            return httpWrapper.delete({ url: baseUrl + "/delete/" + id + "/Vacation" });
        }
        function deleteMeeting(meeting) {
            //return httpWrapper.delete({ url: baseUrl + "/delete/" + id + "/Meeting", data: techId });
            return httpWrapper.post({ url: baseUrl + "/delete/Meeting", data: meeting });
        }
        function getOccurenceInfo(id) {
            return httpWrapper.get({ url: baseUrl + "/get/" + id + "/occurence" });
        }
        function getEstimateOccurenceInfo(id) {
            return httpWrapper.get({ url: baseUrl + "/get/" + id + "/Estimateoccurence" });
        }
        function saveSchedule(scheduleInfo) {
            return httpWrapper.post({ url: baseUrl + "/SaveSchedule", data: scheduleInfo });
        }

        function getRepeatFrequency() {
            return httpWrapper.get({ url: "/application/dropdown/GetRepeatFrequency" });
        }

        function repeatVacation(vacation) {
            return httpWrapper.post({ url: baseUrl + "/repeat/vacation", data: vacation });
        }
        function repeatMeeting(vacation) {
            return httpWrapper.post({ url: baseUrl + "/repeat/meeting", data: vacation });
        }
        function repeatEstimate(scheduleInfo) {
            return httpWrapper.post({ url: baseUrl + "/repeat", data: scheduleInfo });
        }
        return {
            saveEstimate: saveEstimate,
            getEstimateInfo: getEstimateInfo,
            deleteEstimate: deleteEstimate,
            getVacationInfo: getVacationInfo,
            saveVacation: saveVacation,
            deleteVacation: deleteVacation,
            getOccurenceInfo: getOccurenceInfo,
            saveSchedule: saveSchedule,
            getRepeatFrequency: getRepeatFrequency,
            repeatVacation: repeatVacation,
            saveMeeting: saveMeeting,
            getMeetingInfo: getMeetingInfo,
            repeatMeeting: repeatMeeting,
            deleteMeeting: deleteMeeting,
            getEstimateOccurenceInfo: getEstimateOccurenceInfo,
            repeatEstimate: repeatEstimate
        };
    }]);
})();