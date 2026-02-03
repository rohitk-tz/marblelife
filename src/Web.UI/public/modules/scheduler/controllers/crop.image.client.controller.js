(function () {
    'use strict';
    angular.module(SchedulerConfiguration.moduleName).controller("CropImageController",
        ["$scope", "$rootScope", "$state", "$q", "$uibModalInstance", "SchedulerService", "FranchiseeService",
            "Notification", "Clock", "Toaster", "modalParam",
            function ($scope, $rootScope, $state, $q, $uibModalInstance, schedulerService, franchiseeService,
                notification, clock, toaster, modalParam) {
                var vm = this;
                vm.close = close;
                vm.save = save;
                vm.image = modalParam.Image;
                vm.isFromReview = modalParam.isFromReview;
                vm.text = modalParam.text;
                vm.magnify = magnify;
                $scope.myCroppedImage = '';
                vm.query = {
                    BeforeAfterId: null,
                    FileId: null,
                    Base64: "",
                    Type: "",
                    Size: null
                }

                function magnify(imgID, zoom) {
                    var img, glass, w, h, bw;
                    img = document.getElementById(imgID);
                    /*create magnifier glass:*/
                    glass = document.createElement("DIV");
                    glass.setAttribute("class", "img-magnifier-glass");
                    /*insert magnifier glass:*/
                    img.parentElement.insertBefore(glass, img);
                    /*set background properties for the magnifier glass:*/
                    glass.style.backgroundImage = "url('" + img.src + "')";
                    glass.style.backgroundRepeat = "no-repeat";
                    glass.style.backgroundSize = (img.width * zoom) + "px " + (img.height * zoom) + "px";
                    bw = 3;
                    w = glass.offsetWidth / 2;
                    h = glass.offsetHeight / 2;
                    /*execute a function when someone moves the magnifier glass over the image:*/
                    glass.addEventListener("mousemove", moveMagnifier);
                    img.addEventListener("mousemove", moveMagnifier);
                    /*and also for touch screens:*/
                    glass.addEventListener("touchmove", moveMagnifier);
                    img.addEventListener("touchmove", moveMagnifier);
                    function moveMagnifier(e) {
                        var pos, x, y;
                        /*prevent any other actions that may occur when moving over the image*/
                        e.preventDefault();
                        /*get the cursor's x and y positions:*/
                        pos = getCursorPos(e);
                        x = pos.x;
                        y = pos.y;
                        /*prevent the magnifier glass from being positioned outside the image:*/
                        if (x > img.width - (w / zoom)) { x = img.width - (w / zoom); }
                        if (x < w / zoom) { x = w / zoom; }
                        if (y > img.height - (h / zoom)) { y = img.height - (h / zoom); }
                        if (y < h / zoom) { y = h / zoom; }
                        /*set the position of the magnifier glass:*/
                        glass.style.left = (x - w) + "px";
                        glass.style.top = (y - h) + "px";
                        /*display what the magnifier glass "sees":*/
                        glass.style.backgroundPosition = "-" + ((x * zoom) - w + bw) + "px -" + ((y * zoom) - h + bw) + "px";
                    }
                    function getCursorPos(e) {
                        var a, x = 0, y = 0;
                        e = e || window.event;
                        /*get the x and y positions of the image:*/
                        a = img.getBoundingClientRect();
                        /*calculate the cursor's x and y coordinates, relative to the image:*/
                        x = e.pageX - a.left;
                        y = e.pageY - a.top;
                        /*consider any page scrolling:*/
                        x = x - window.pageXOffset;
                        y = y - window.pageYOffset;
                        return { x: x, y: y };
                    }
                }

                vm.close = function () {
                    $uibModalInstance.dismiss();
                };

                function save(str) {
                    // extract content type and base64 payload from original string
                    var pos = str.indexOf(';base64,');
                    var type = str.substring(5, pos);
                    var b64 = str.substr(pos + 8);
                    // decode base64
                    var imageContent = atob(b64);
                    // create an ArrayBuffer and a view (as unsigned 8-bit)
                    var buffer = new ArrayBuffer(imageContent.length);
                    var view = new Uint8Array(buffer);
                    // fill the view, using the decoded base64
                    for (var n = 0; n < imageContent.length; n++) {
                        view[n] = imageContent.charCodeAt(n);
                    }
                    // convert ArrayBuffer to Blob
                    var blob = new Blob([buffer], { type: type });

                    vm.query.Base64 = b64;
                    vm.query.BeforeAfterId = vm.image.beforeAfterId;
                    vm.query.FileId = vm.image.fileId;
                    vm.query.Type = type;
                    vm.query.Size = blob.size;
                    schedulerService.saveCroppedImage(vm.query).then(function (result) {
                        if (result.data != null) {
                            if (result.data) {
                                toaster.show(result.message.message);
                            }
                        }
                        vm.close();
                    });
                }
                //function save(myCroppedImage) {
                //    var imageBase64 = myCroppedImage;
                //    var blob = new Blob([imageBase64], { type: 'image/png' });
                //    var file = new File([blob], 'imageFileName.png');
                //}

                $q.all([]);
            }]);
}());