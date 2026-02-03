(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).factory("FileService", ["HttpWrapper", function (httpWrapper) {
        var baseUrl = "/application/file"

        var upload = function (file) {
            return httpWrapper.upload({ url: baseUrl + "/upload" }, file);

        };
        var uploadZip = function (file) {
            return httpWrapper.upload({ url: baseUrl + "/uploadZip" }, file);

        };
        var uploadFile = function (file) {
            return httpWrapper.upload({ url: baseUrl + "/uploadfile" }, file);
        };
        var getFile = function (fileId) {
            return httpWrapper.get({ url: baseUrl + "/GetById/" + fileId });
        };

        var getExcel = function (fileId) {
            return httpWrapper.getFile({ url: baseUrl + "/GetExcelFileById/" + fileId });
        };

        var getExcelByName = function () {
            return httpWrapper.getFile({ url: baseUrl + "/GetExcelFileByName/" });
        }

        var getFileForDownload = function (fileId) {
            return httpWrapper.getFile({ url: baseUrl + "/GetFile/" + fileId });
        };
        var getFileForDownloadForBeforeAfter = function (fileId) {
            return httpWrapper.getFile({ url: baseUrl + "/GetFileForBeforeAfter/" + fileId });
        };

        var downloadBeforeAfterZipFile = function (fileId) {
            return httpWrapper.getFile({ url: baseUrl + "/DownloadZipFile?fileId=" + fileId });
        };

        var uploadBeforeAfterZipFile = function (fileModels) {
            return httpWrapper.post({ url: baseUrl + "/UploadBeforeAfterZipFile/", data: fileModels });
        };
        var downloadCustomerInvoice = function (fileName) {
            return httpWrapper.getFile({ url: baseUrl + "/DownloadCustomerInvoice?fileName=" + fileName  });
        };

        var downloadFile = function (data, fileName) {

            var file = new Blob([data], {
                type: 'application/octet-binary'
            });
            var fileUrl = URL.createObjectURL(file);

            // for IE 10+
            if (window.navigator.msSaveOrOpenBlob) {
                window.navigator.msSaveOrOpenBlob(file, fileName);
            } else {
                var element = document.createElement('a');
                element.href = fileUrl;
                element.setAttribute('download', fileName);
                element.setAttribute('target', '_blank');
                document.body.appendChild(element); //Append the element to work in firefox
                element.click();
            }
        };

        var getFileStreamByUrl = function (fileUrl) {
            //var url = "Application/File/Get?url=" + fileUrl;
            return httpWrapper.getFile({ url: baseUrl + "/Get?url=" + fileUrl });
        };


        var getStreamUrl = function (result) {
            var file = new Blob([result.data], {
                type: 'application/octet-binary'
            });
            var fileUrl = URL.createObjectURL(file);
            return fileUrl;
        };
     var  readLocalFile = function (input, imageId) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {
                    $('#' + imageId)
                      .attr('src', e.target.result);
                };
                reader.readAsDataURL(input.files[0]);
            }
     };

     var uploadedExceltoImage = function (file) {
         return httpWrapper.upload({ url: baseUrl + "/uploadedExceltoImage" }, file);

     };
        var uploadForBeforeAfter = function (file) {
            return httpWrapper.upload({ url: baseUrl + "/uploadForBeforeAfter" }, file);

        };
        var downloadFileImage = function (data, fileName) {

            var file = new Blob([data], {
                type: 'image/png'
            });
            var fileUrl = URL.createObjectURL(file);

            // for IE 10+
            if (window.navigator.msSaveOrOpenBlob) {
                window.navigator.msSaveOrOpenBlob(file, fileName);
            } else {
                var element = document.createElement('a');
                element.href = fileUrl;
                element.setAttribute('download', fileName);
                element.setAttribute('target', '_blank');
                document.body.appendChild(element); //Append the element to work in firefox
                element.click();
            }
        };

        var getBeforeAfter = function (fileId) {
            return httpWrapper.getFile({ url: baseUrl + "/getBeforeAfter/" + fileId});

        };

        var uploadnvoiceFile = function (file) {
            return httpWrapper.upload({ url: baseUrl + "/UploadnvoiceFile" }, file);

        };
        var uploadDyamicFile = function (file) {
            return httpWrapper.upload({ url: baseUrl + "/uploadDyamicFile" }, file);

        };
        return {
            upload: upload,
            getFile: getFile,
            getExcel: getExcel,
            downloadFile: downloadFile,
            getExcelByName: getExcelByName,
            getFileStreamByUrl: getFileStreamByUrl,
            getStreamUrl: getStreamUrl,
            readLocalFile: readLocalFile,
            uploadFile: uploadFile,
            uploadZip: uploadZip,
            uploadedExceltoImage: uploadedExceltoImage,
            getFileForDownload: getFileForDownload,
            downloadFileImage: downloadFileImage,
            uploadForBeforeAfter: uploadForBeforeAfter,
            getFileForDownloadForBeforeAfter: getFileForDownloadForBeforeAfter,
            downloadBeforeAfterZipFile: downloadBeforeAfterZipFile,
            uploadBeforeAfterZipFile: uploadBeforeAfterZipFile,
            getBeforeAfter: getBeforeAfter,
            downloadCustomerInvoice: downloadCustomerInvoice,
            uploadnvoiceFile: uploadnvoiceFile,
            uploadDyamicFile: uploadDyamicFile
        };
    }]);
}());