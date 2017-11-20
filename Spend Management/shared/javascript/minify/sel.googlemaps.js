(function (SEL, $g) {
    var scriptName = "GoogleMaps";
    function execute() {
        SEL.registerNamespace("SEL.GoogleMaps");
        SEL.GoogleMaps = {
            ScriptSource: null,
            CanvasDomId: null,
            LineSteps: null,
            Licensed: false,
            Create: function (canvasDomId, steps) {
                SEL.GoogleMaps.CanvasDomId = canvasDomId;
                SEL.GoogleMaps.LineSteps = steps;

                //if google maps api is loaded (google.maps exists) just load the map
                if (("google" in window) && ("maps" in google)) {
                    SEL.GoogleMaps.Initialise();
                }
                //otherwise inject a script tag
                else {
                    var script = document.createElement("script");
                    script.type = "text/javascript";
                    script.src = SEL.GoogleMaps.ScriptSource;
                    //as soon as the script is loaded SEL.GoogleMaps.Initialise is called
                    document.body.appendChild(script);
                }

            },
            Initialise: function () {
                /// <summary>Draws a Google map with the options specified in SEL.GoogleMaps.Create</summary>
                var i,
                    wayPoint,
                    latLong,
                    prevLatLong,
                    markerLatLong,
                    bounds = new google.maps.LatLngBounds(),
                    map = new google.maps.Map($g(SEL.GoogleMaps.CanvasDomId), {
                        mapTypeId: google.maps.MapTypeId.ROADMAP,
                        maxZoom: 16
                    });

                // add markers to the map for each location
                var steps = SEL.GoogleMaps.LineSteps;

                for (i = 0; i < steps.length; i += 1) {
                    wayPoint = steps[i];
                    latLong = SEL.GoogleMaps.ExtractPostcodeAnywhereLatLong(steps[i], false);

                    if (wayPoint.Action === "A" && i > 0) {
                        prevLatLong = SEL.GoogleMaps.ExtractPostcodeAnywhereLatLong(steps[i - 1], true);
                    } else {
                        prevLatLong = null;
                    }

                    // add a marker for each arrival point, one for the first departure too
                    if (wayPoint.Action === "A" || (wayPoint.Action === "D" && i === 0)) {
                        markerLatLong = (latLong === null ? prevLatLong : latLong);
                        SEL.GoogleMaps.AddMarker(map, markerLatLong, wayPoint.Description.replace("<b>", "").replace("</b>", ""));

                        bounds.extend(markerLatLong);
                    }
                }

                // fit the map to the markers
                map.fitBounds(bounds);

                // draw the route line on the map
                SEL.GoogleMaps.DrawRoute(map, steps);
            },
            ExtractPostcodeAnywhereLatLong: function (step, getLastLongLat) {
                var index, latLong = $.parseJSON(step.Line);

                if (getLastLongLat === true) {
                    index = latLong.length - 1;
                } else {
                    index = 0;
                }

                if (latLong[index] !== undefined) {
                    latLong = new google.maps.LatLng(latLong[index][0], latLong[index][1]);
                } else {
                    latLong = null;
                }

                return latLong;
            },
            AddMarker: function (map, latLng, title) {
                new google.maps.Marker({
                    map: map,
                    position: latLng,
                    title: title
                });
            },
            DrawRoute: function (map, steps)
            {
                /// <summary>Draws a route on a Google map</summary>
                /// <param name="map" type="google.maps.Map">The map on which to draw the route</param>
                /// <param name="steps" type="Array">Array of objects containing postcode anywhere directions data</param>
                var i,
                    j,
                    lines,
                    line,
                    linePath = [],
                    polyLine = new google.maps.Polyline({
                        clickable: false,
                        map: map,
                        strokeColor: "#414AED"
                    });

                // build an array of Google LatLong from all Postcode Anywhere route coordinates
                for (i = 0; i < steps.length; i += 1) {
                    lines = $.parseJSON(steps[i].Line);
                    for (j = 0; j < lines.length; j += 1) {
                        line = lines[j];
                        linePath.push(new google.maps.LatLng(line[0], line[1]));
                    }
                }
                
                // draw the line
                polyLine.setPath(linePath);
            }
        };
    }
    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}(SEL, $g));
