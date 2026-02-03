(function () {
    'use strict';
    angular.module(UsersConfiguration.moduleName).service("UserService", ["HttpWrapper", "URLAuthenticationServiceForEncryption"
        , function (httpWrapper, uRLAuthenticationServiceForEncryption) {
            var baseUrl = "/users";
            function getUsers(query) {
                return httpWrapper.get({
                    url: baseUrl + "?filter.text=" + query.text + "&pageNumber=" + query.pageNumber + "&pageSize=" + query.pageSize
                        + "&filter.franchiseeId=" + query.franchiseeId + "&filter.sortingColumn=" + query.sort.propName
                        + "&filter.sortingOrder=" + query.sort.order + "&filter.roleId=" + query.roleId + "&filter.name=" + query.name + "&filter.email=" + query.email
                        + "&filter.userName=" + query.userName + "&filter.statusId=" + query.statusId
                })
            }
            function getUserById(id, franchiseeId) {
                id = uRLAuthenticationServiceForEncryption.decrypt(id);
                franchiseeId = uRLAuthenticationServiceForEncryption.decrypt(franchiseeId);
                if (id == "") {
                    id = 0;
                }
                if(franchiseeId == ""){
                    franchiseeId = 0;
                }
                return httpWrapper.get({ url: baseUrl + "/" + id + "/" + franchiseeId });
                                
            }
            function saveUser(user, isEdit) {
                if (isEdit == true) {
                    return httpWrapper.put({ url: baseUrl + "/" + user.id, data: user });
                }
                else {
                    return httpWrapper.post({ url: baseUrl, data: user });
                }
            }
            function getRoles() {
                return httpWrapper.get({ url: "/application/dropdown/GetRoles" });
            }
            function lock(id, isLocked) {
                return httpWrapper.post({ url: baseUrl + "/" + id + "/Lock", data: isLocked });
            }
            function isUniqueEmail(id, email) {
                if (id == undefined) {
                    id = 0;
                }
                return httpWrapper.get({ url: baseUrl + "/" + id + "/email/" + email + "/verify" });
            }
            function isUniqueUserName(id, userName) {
                if (id == undefined) {
                    id = 0;
                }
                return httpWrapper.get({ url: baseUrl + "/" + id + "/userName/" + userName + "/verify" });
            }
            function manageAccount(userId, franchiseeIds) {
                return httpWrapper.post({ url: "/manage/" + userId + "/account", data: franchiseeIds });
            }
            function getImageUrl() {
                return httpWrapper.get({ url: baseUrl + "/User/getImageUrl" });
            }
            function saveUserForEquipment(user, isEdit) {
                if (isEdit == true) {
                    return httpWrapper.post({ url: baseUrl + "/User/userEdit/" + user.id, data: user });
                }
                else {
                    return httpWrapper.post({ url: baseUrl + "/User/postForEquipment", data: user });
                }
            }
            function saveCalendarDefaultView(deafaultView) {
                return httpWrapper.post({ url: baseUrl + "/User/saveDefaultView/" + deafaultView + "/save" });
            }
            function getDefaultView() {
                return httpWrapper.post({ url: baseUrl + "/User/getDefaultView" });
            }
            function getUserListByFranchisee() {
                return httpWrapper.post({ url: baseUrl + "/User/getUserList" });
            }
            function getEmailSignaturesCollection() {
                return httpWrapper.post({ url: baseUrl + "/User/getUserSignature" });
            }
            function saveEmailSignaturesCollection(query) {
                return httpWrapper.post({ url: baseUrl + "/User/saveUserSignature", data: query });
            }

            return {
                getUsers: getUsers,
                getUserById: getUserById,
                saveUser: saveUser,
                getRoles: getRoles,
                lock: lock,
                isUniqueEmail: isUniqueEmail,
                isUniqueUserName: isUniqueUserName,
                manageAccount: manageAccount,
                getImageUrl: getImageUrl,
                saveUserForEquipment: saveUserForEquipment,
                saveCalendarDefaultView: saveCalendarDefaultView,
                getDefaultView: getDefaultView,
                getUserListByFranchisee: getUserListByFranchisee,
                getEmailSignaturesCollection: getEmailSignaturesCollection,
                saveEmailSignaturesCollection: saveEmailSignaturesCollection
            };
        }]);
})();