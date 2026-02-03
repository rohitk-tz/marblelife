
(function () {
    'use strict';

    angular.module(CoreConfiguration.moduleName).directive("googleMap", [function () {
        return {
            restrict: "EA",
            template: '<div style="width:100%;height:800px" id="gmaps"></div>',
            replace: true,
            scope: {
                counter: '@' // Accept two ways binding
            },
            link: function (scope, element, attrs) {

                var geocoder = new google.maps.Geocoder();
                var toAddressLat = "";
                var toAddressLng = "";
                var desLanLng = "";
                var curLanLng = "";
                var currentLocation = "";
                var directionsService = new google.maps.DirectionsService();
                var addDirectionsRendererOptions = { draggable: false, hideRouteList: true, suppressMarkers: true, preserveViewport: false, polylineOptions: { strokeColor: "black", strokeWeight: 6 } };
                var drawDirectionsRendererOptions = { draggable: false, hideRouteList: true, suppressMarkers: true, preserveViewport: false, polylineOptions: { strokeColor: "black", strokeWeight: 6 } };
                var addDirectionsRenderer = new google.maps.DirectionsRenderer(addDirectionsRendererOptions);
                var routeMarkers = [];
                var map;

                function initMap() {
                    if (geocoder) {
                        if (navigator.geolocation) {
                            navigator.geolocation.getCurrentPosition(function (p) {
                                
                                var latlng = new google.maps.LatLng(p.coords.latitude, p.coords.longitude);
                                var latlng = {
                                    lat: parseFloat(p.coords.latitude),
                                    lng: parseFloat(p.coords.longitude)
                                }
                                geocoder.geocode({ 'location': latlng }, function (results, status) {
                                    if (status == google.maps.GeocoderStatus.OK) {
                                        if (results[0]) {
                                            currentLocation = (results[0].formatted_address);
                                            directionService(currentLocation, scope.counter, latlng);
                                        } else {
                                            console.log('Location not found');
                                        }
                                    } else {
                                        console.log('Geocoder failed due to: ' + status);
                                    }
                                });
                            }, function error(msg) {
                                alert('Please enable your GPS .');

                            }, { maximumAge: 600, timeout: 5000, enableHighAccuracy: true });
                        } else {
                            alert("There is Some Problem on your current browser to get Geo Location!");
                        }
                    };

                }
                initMap();

                //start

                function directionService(currentaddress, destinationaddress, curLanLng) {
                    var mapOptions = {
                        center: curLanLng,
                        zoom: 900,
                        scrollwheel: true,
                    };
                    //console.log(destination);
                    map = new google.maps.Map(element[0], mapOptions);
                    var directionsRequest =
                       {
                           destination: destinationaddress,
                           //origin: currentaddress,
                           origin: "1209 W 5th St #300, Austin, TX 78703, USA",
                           travelMode: google.maps.TravelMode.DRIVING,
                       };
                    directionsService.route(directionsRequest,
                                         function (result, status) {
                                             if (status === google.maps.DirectionsStatus.OK) {
                                                 addDirectionsRenderer.setDirections(result);
                                                 addDirectionsRenderer.setMap(map);
                                                 if (routeMarkers.length > 1) {
                                                     scope.$emit("Show-Route-Button");

                                                 }
                                                 var leg = result.routes[0].legs[0];
                                                 makeMarker(leg.start_location, icons.start, 'Your Current Location' + "( " + "1209 W 5th St #300, Austin, TX 78703, USA" + ")", map);
                                                 makeMarker(leg.end_location, icons.end,'Your Destination' + "( " + destinationaddress + ")" , map);
                                             } else {
                                                 if (status === google.maps.DirectionsStatus.ZERO_RESULTS) {
                                                     console.log("No Route Found.");
                                                 }
                                                 else if (status.toString() == "MAX_WAYPOINTS_EXCEEDED") {
                                                     console.log("You can add maximum .... way points.");
                                                 }
                                                 else {
                                                     console.log(status);
                                                 }
                                                 routeMarkers.pop();
                                             }
                                         });
                }

                function makeMarker(position, icon, title, map) {
                    new google.maps.Marker({
                        position: position,
                        map: map,
                        icon: icon,
                        title: title
                    });
                }

                //end

                var icons = {
                    start: new google.maps.MarkerImage(
                    // URL
                    '/Content/images/green_map.png',
                    // (width,height)
                    new google.maps.Size(84, 32),
                    // The origin point (x,y)
                    new google.maps.Point(0, 0),
                    // The anchor point (x,y)
                    new google.maps.Point(22, 32)),
                    end: new google.maps.MarkerImage(
                    // URL
                    '/Content/images/red_map.png',
                    // (width,height)
                    new google.maps.Size(44, 32),
                    // The origin point (x,y)
                    new google.maps.Point(0, 0),
                    // The anchor point (x,y)
                    new google.maps.Point(22, 32))
                };
            }
        };

    }]);
})();

