(function (angular) {
    'use strict';
    function printDirective() {

        function link(scope, element, attrs) {
            element.on('click', function () {
                var elemToPrint = document.getElementById(attrs.printElementId);
                if (elemToPrint) {
                    printElement(elemToPrint);
                }
            });
        }
        function printElement(elem) {
            
            var a = $('#printSection').clone();
            //var btnImg = $('#printSection').getElementById('btn_img');
            
            //btnImg.className += " hide-during-print"; 
            $('body *:visible').addClass("hide-during-print").hide();
            $('body').prepend(a);
            window.print();
                        
            $('body .hide-during-print').show().removeClass("hide-during-print");
            a.remove();
            
        }
        return {
            link: link,
            restrict: 'A'
        };
    }
    angular.module(CoreConfiguration.moduleName).directive('ngPrint', [printDirective]);
}(window.angular));