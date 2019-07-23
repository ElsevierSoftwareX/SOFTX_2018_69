var map = L.map('mapid',{
     	crs: L.CRS.Simple
 	});
  var imageBounds = [[0,0], [700,700]];
  var image = L.imageOverlay('img/maze_top.jpg', imageBounds).addTo(map);
  map.fitBounds(imageBounds);

  var yx = L.latLng;

  var xy = function(x, y) {
      if (L.Util.isArray(x)) {    // If arguments are an array
          return yx(x[1], x[0]);
      }
      return yx(y, x);  // If arguments are a pair
  };

  var xOffSet = 8.139; // If squared map, use only one offset variable
  var zOffSet = 8.139;
  var xOriginUnity = -43; // Origin point in Unity3D
  var yOriginUnity = -42; // Origin point in Unity3D
  var totalUnity = 86; // Total Unity environment length (Final - Origin)

  var xyOffset = function(x,y){
    return yx(((yOriginUnity - parseFloat(y))*xOffSet*-1),(xOriginUnity - parseFloat(x))*xOffSet*-1);
  };

 var addressPoints = [];

//xml unwrap

$.ajax({
    type: "GET",
    url: "data/BaseSceneTP.5.xml", //sample file with user data
    dataType: "xml",
    success: function (xml) {
        console.log(xml);
        var i = 0;


        // Parse the xml file and get data
        var xmlDoc = $.parseXML(xml),
            $xml = $(xmlDoc);
            $(xml).find('startPosition').each(function () {
              //console.log("Movement " + i);
              i++;

              var xStartPosition = $(this).find('x').text();
              var zStartPosition = $(this).find('z').text();
              //console.log(xStartPosition,zStartPosition);
              var point = xyOffset(xStartPosition,zStartPosition);
              //console.log("point = " + point);
              var zOK = $(zStartPosition) + $(zOffSet);
              var xAux = (xOriginUnity - parseFloat(xStartPosition))*xOffSet*-1;
              var yAux = (yOriginUnity - parseFloat(zStartPosition))*xOffSet*-1;
              //console.log("x = " + xAux + " / y = " + yAux);

              addressPoints.push([ yAux,xAux ,1]);

                });

    },
    error: function() {
    console.log("ERROR");
    alert("XML file loading was unsuccessful");
}
});

//console.log(addressPoints);

var heat = L.heatLayer(addressPoints, {
  minOpacity:0.5,
  max:1.0,
  radius: 20,
  gradient:{
    0.4: 'blue',
    0.65: 'lime',
    1: 'red'
  }
}).addTo(map);
