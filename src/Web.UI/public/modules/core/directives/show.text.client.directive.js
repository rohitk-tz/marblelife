(function () {

    angular.module(CoreConfiguration.moduleName).directive("showText", [
        function () {

            var updateElem = function (element) {
                return function (displayFile) {
                    element.empty();

                    var objectElem = {};

                    if (displayFile && displayFile !== "") {
                        objectElem = angular.element(document.createElement("object"));

                        var jqObjElement = $(objectElem);

                        jqObjElement.css({ 'width': '100%', 'height': '100%', 'position': 'absolute', 'top': '0', 'left': '0' });
                        jqObjElement.attr("data", displayFile);
                        jqObjElement.attr("type", "text/plain");
                        jqObjElement.title = "Logs"
                    }
                    element.append(jqObjElement);
                };
            };

            return {
                restrict: "EA",
                scope: {
                    showText: "="
                },
                link: function (scope, element) {
                    scope.$watch("showText", function (newValue, oldValue) {
                        if (newValue != null && newValue.length > 0) updateElem(element)(newValue);
                    });
                }
            };

        }]);

}());
