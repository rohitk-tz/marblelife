(function (angular) {
    'use strict';

    function printDirective() {
        function link(scope, element, attrs) {
            element.on('click', function () {
                var elemToPrint = document.getElementById(attrs.printElementId);
                if (elemToPrint) {
                    var content = elemToPrint.innerHTML; // rendered DOM
                    printElement(content);
                }
            });
        }

        function printElement(content) {
            var printWindow = window.open('', '_blank', 'width=1000,height=800');

            printWindow.document.open();
            printWindow.document.write(
                '<html>' +
                '<head>' +
                '<title>MarbleLife</title>' +
                '<link rel="stylesheet" href="/css/bootstrap.min.css">' +
                '<link rel="stylesheet" href="/css/site.css">' +
                '<style>' +
                'body { font-size: 11px; line-height: 1.4; margin:0; padding:0; }' +
                '@page { size: A4; margin: 10mm 8mm; }' +
                'table { border-collapse: collapse; width: 100%; }' +
                'th, td { border: 1px solid #000; padding: 4px; text-align:left; }' +
                'thead { display: table-header-group; }' +
                'tr { page-break-inside: avoid; }' +
                '.fa { display: none !important; }' +
                '</style>' +
                '</head>' +
                '<body>' +
                content +
                '</body>' +
                '</html>'
            );

            printWindow.document.close();

            printWindow.onload = function () {
                printWindow.focus();
                printWindow.print();
                printWindow.close();
            };
        }

        return {
            link: link,
            restrict: 'A'
        };
    }

    angular.module(CoreConfiguration.moduleName).directive('ngPrintAmchart', [printDirective]);
}(window.angular));
