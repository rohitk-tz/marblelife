(function () {
    'use strict';

    angular.module(SchedulerConfiguration.moduleName).service("ToDoService", ["HttpWrapper", "APP_CONFIG", function (httpWrapper, config) {

        var baseUrl = "/scheduler/toDo";
        function saveToDo(todoList) {
            return httpWrapper.post({ url: baseUrl + "/SaveToDo", data: todoList });
        }
        function getToDoList(todoList) {
            return httpWrapper.post({ url: baseUrl + "/GetToDoList", data: todoList });
        }
        function getToDoListById(id) {
            return httpWrapper.get({ url: baseUrl + "/GetToDoById/" + id });
        }

        function customerList(text) {
            return httpWrapper.get({ url: baseUrl + "/GetCustomerList/" + text , skipFullPageLoader: true });
        }
        function customerInfo(text) {
            return httpWrapper.get({ url: baseUrl + "/GetCustomerInfo/" + text, skipFullPageLoader: true });
        }
        function saveCommentToDo(commentModel) {
            return httpWrapper.post({ url: baseUrl + "/SaveCommentToDo", data: commentModel });
        }
        function getCommentToDo(id) {
            return httpWrapper.get({ url: baseUrl + "/GetCommentToDo/" + id });
        }
        function getCommentToDoForScheduler(schedulerId) {
            return httpWrapper.get({ url: baseUrl + "/GetCommentToDoForScheduler/" + schedulerId });
        }
        function getToDoListForScheduler(schedulerIds) {
            return httpWrapper.post({ url: baseUrl + "/GetToDoListForScheduler", data: schedulerIds });
        }
        function saveCommentToDoByStatus(commentModel) {
            return httpWrapper.post({ url: baseUrl + "/SaveToDoByStatus", data: commentModel });
        }
        function deleteToDo(id) {
            return httpWrapper.post({ url: baseUrl + "/DeleteToDo/" + id });
        }
        function toDoInfo(text) {
            return httpWrapper.get({ url: baseUrl + "/GetToDoInfo/" + text, skipFullPageLoader: true });
        }
        function toDoList(text) {
            return httpWrapper.get({ url: baseUrl + "/toDoList/" + text, skipFullPageLoader: true });
        }
        function getSchedulerListForToDo(model) {
            return httpWrapper.post({ url: baseUrl + "/getSchedulerListForToDo", data: model });
        }
        return {
            saveToDo: saveToDo,
            getToDoList: getToDoList,
            getToDoListById: getToDoListById,
            customerList: customerList,
            customerInfo: customerInfo,
            saveCommentToDo: saveCommentToDo,
            getCommentToDo: getCommentToDo,
            getCommentToDoForScheduler: getCommentToDoForScheduler,
            getToDoListForScheduler: getToDoListForScheduler,
            saveCommentToDoByStatus: saveCommentToDoByStatus,
            deleteToDo: deleteToDo,
            toDoInfo: toDoInfo,
            toDoList: toDoList,
            getSchedulerListForToDo: getSchedulerListForToDo
        }
    }]);
})();