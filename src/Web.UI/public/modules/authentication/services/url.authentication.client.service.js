(function () {
    'use strict';
    var base64Key = CryptoJS.enc.Base64.parse("2b7e151738aed2a6abf7158809cf4f3c");
    var iv = CryptoJS.enc.Base64.parse("3ad77bb90d7a3770a89ecaf32466ef97");

    angular.module(AuthenticationConfiguration.moduleName).service('URLAuthenticationServiceForEncryption', function () {

        function encrypt(data) {
            var encrypted = CryptoJS.AES.encrypt(
                data,
                base64Key,
                { iv: iv });
            var encryptedCipherText = encrypted.ciphertext.toString(CryptoJS.enc.Base64);
            return encryptedCipherText;
        }

        function decrypt(data) {
            var cipherParams = CryptoJS.lib.CipherParams.create({
                ciphertext: CryptoJS.enc.Base64.parse(data)
            });
            var decrypted = CryptoJS.AES.decrypt(
                cipherParams,
                base64Key,
                { iv: iv });
            return decrypted.toString(CryptoJS.enc.Utf8);
        }

        return {
            encrypt: encrypt,
            decrypt: decrypt
        };
    });
})();