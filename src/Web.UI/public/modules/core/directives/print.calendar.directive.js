(function (angular) {
    'use strict';
    function printDirective() {

        function link(scope, element, attrs) {
            element.on('click', function () {
                var elemToPrint = document.getElementById(attrs.printElementId);
                if (elemToPrint) {
                    printElement(elemToPrint, attrs.printElementId);
                }
            });
        }
        function printElement(elem, id) {
            $('#sizer').hide();
            var isVisible = true;
            var contentLength = [];
            var height = '';
            var overflowY = '';
            //$('#' + id).css('width', '50%');
            var a = $('#' + id).clone();
            var btnImg = document.getElementById("btn_img");
            var scrollPanelList = document.getElementsByClassName("fc-scroller fc-time-grid-container");
            if (scrollPanelList.length > 0) {
                var scrollPanel = scrollPanelList[0];
                height = scrollPanel.style.height;
                scrollPanelList[0].style.height = "auto";
            }
            if (btnImg != null) {
                btnImg.className += " hide-during-print";
            }
            if (a[0].attributes[2] != undefined && a[0].attributes[2].value != null) {
                a[0].attributes[2].value = "display:block";
                isVisible = false;
            }

            $('body *:visible').addClass("hide-during-print").hide();
            $('body').append('<style> .fc-row.fc-week.fc-widget-content.page-break-after { page-break-after : always} .fc-row.fc-week.fc-widget-content.page-break-before { page-break-after : always}.fc-scroller.fc-time-grid-container { height:auto !important } #printSection {font-size:1px important} #btn_img {display:none !important; } #myFirstChart1 { margin-top: -24px;} .print-header {margin-top:0px !important;margin-botton:0px !important; } #legenddiv1 { top:-30px !important} div#sizer {height:0px;} @page{size:landscape; margin: 0;height: 9%;zoom:50%} ::-moz-page{size:landscape; margin: 0;height: 9%} @media print { md-content {overflow: visible;margin:none;page-break-inside: always;}.print-header {display:block !important} }</style><html><head><title></title></head>');
            $('body').prepend(a);
            $('body').append('</html>');
            window.print();
            if (!isVisible) { 
                a[0].attributes[2].value = "display:none";
            }
            $('body .hide-during-print').show().removeClass("hide-during-print");
            $('body').append('<style> #btn_img {display:block !important;.fc-scroller.fc-time-grid-container { height:' + height + '!important;  }');
            a.remove();

        }
        return {
            link: link,
            restrict: 'A'
        };
    }
    angular.module(CoreConfiguration.moduleName).directive('ngPrintCalendar', [printDirective]);
}(window.angular));

