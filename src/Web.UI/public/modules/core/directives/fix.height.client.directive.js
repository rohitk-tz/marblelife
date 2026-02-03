(function () {


    angular.module(CoreConfiguration.moduleName).directive('fixHeight', [function () {

        return {
            restrict: 'A',
            link: function($scope, $element){

                var el = $($element);

                setTimeout(function () {
                    var candHeight = $(window).height() - 20 - el.offset().top;
                    el.css({ "height": candHeight + "px" });
                }, 2000);
            }
        };


    }]);

}());