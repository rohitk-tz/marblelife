(function () {
    'use strict';

    angular.module(SchedulerConfiguration.moduleName).service("JobService", ["HttpWrapper", "APP_CONFIG", function (httpWrapper, config) {

        var baseUrl = "/Scheduler/job"

        function getJobInfo(id) {
            return httpWrapper.get({ url: baseUrl + "/" + id });
        }

        function save(model) {
            return httpWrapper.put({ url: baseUrl + "/" + model.id, data: model });
        }

        function deleteJob(id) {
            return httpWrapper.delete({ url: baseUrl + "/" + id });
        }

        return {
            getJobInfo: getJobInfo,
            save: save,
            deleteJob: deleteJob
        };
    }]);
})();