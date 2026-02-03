(function () {

	'use strict';

	angular.module(CoreConfiguration.moduleName).directive("keyStrokeAction", ["$timeout", function ($timeout) {

		return {
			restrict: 'A',
			scope: {
				change: "="
			},
			link: function ($scope, element, $attrs, controller) {

				var lastKeyDown = null;
				var keptTimeout = null;
				var hasValidChar = false;

				$(element).keyup(function (evt) {

					if (keptTimeout != null) {
						$timeout.cancel(keptTimeout);
						keptTimeout = null;
					}

					var code = (evt.keyCode ? evt.keyCode : evt.which);
					if (code == 13) { //Enter keycode
						$scope.change();
						hasValidChar = false;
						return;
					}

					if (code == 8 || code == 127 || code == 46 || (code > 47 && code < 58) || (code >= 65 && code < 91) || (code >= 97 && code < 123)) {
						hasValidChar = true;
					}

					lastKeyDown = moment();
					checkDiffAndFire(lastKeyDown);
				});

				function checkDiffAndFire(keyDown) {
					keptTimeout = $timeout(function () {
						if (keyDown == lastKeyDown && hasValidChar == true) {
							$scope.change();
							hasValidChar = false;
						}
					}, 500);
				}

			}
		};
	}]);

}());
