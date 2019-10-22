var moodio = moodio || {};
moodio.punc = moodio.punc || {};

moodio.punc.reverseLookupEndpoint = "/api/geocoding/reverselookup";
//load
(function(){

    function load(){
        
        // console.log("loading punc gps locators");

        var gpsButtons = document.querySelectorAll("[data-punc-gpslocator]")
        for(var i = 0, len = gpsButtons.length; i<len; i++){
            new GpsLocator(gpsButtons[i]);
        }

        //get the users location
    }

    function GpsLocator(inputElem)
    {
        //elements
        this.elem = inputElem;
        this.targetElem = document.getElementById(inputElem.dataset.puncGpslocator);

        this.apiUri = moodio.punc.apiHost + moodio.punc.reverseLookupEndpoint;
            
        this.lastCoords = {};
        this.lastAddress = null;

        //store the original classname for when changing the classname back from loading to avoid having to run a replace on a string
        this.originalClassName = this.elem.className;

        //bind to click events
        var me = this;
        inputElem.addEventListener('click', ((e) =>{this.getLocation();}).bind(me));
    }

    GpsLocator.prototype.getLocation = function()
    {
        // console.log("getlocation");
        var me = this;
        navigator.geolocation.getCurrentPosition(this.locationCallback.bind(me), this.failedLocationCallback.bind(me));

        //set as loading
        this.setLoadingStatus(true);
    }

    GpsLocator.prototype.locationCallback = function(position)
    {
        // console.log("locationCallback");

        //if coords have not changed, dont bother
        if(this.lastCoords.latitude == position.coords.latitude 
            && this.lastCoords.longitude == position.coords.longitude
            && this.lastAddress != null)
        {
            this.addressCallback( { "formattedAddress": this.lastAddress});
        }
        else{
            this.lastCoords = position.coords;
            this.getAddressFromCoordinates(position.coords.latitude, position.coords.longitude);
        }
    }

    GpsLocator.prototype.failedLocationCallback = function(event)
    {
        // console.dir(event);
        this.setLoadingStatus(false);
    }

    GpsLocator.prototype.getAddressFromCoordinates = function(lat, long)
    {
        // console.log("get address from coords");
        var req = new XMLHttpRequest();

        //bind events to when call is completed
        var me = this;
        req.onreadystatechange = function()
        {
            if(req.readyState === 4 && req.status === 200){
                this.addressCallback(JSON.parse(req.response));
            }
        }.bind(me);

        req.open("GET", this.apiUri + "?lat=" + lat + "&lng="+long);
        req.setRequestHeader("Content-Type","application/json");
        req.send();
    }

    GpsLocator.prototype.addressCallback = function(res)
    {
        // console.log("called back address with address of " + res.formattedAddress);
        this.lastAddress = res.formattedAddress;
        this.targetElem.value = res.formattedAddress;
        this.setLoadingStatus(false);
    }

    GpsLocator.prototype.setLoadingStatus = function(status){
        if(status === true){
            this.elem.className = this.elem.className + " loading";
        }else{
            this.elem.className = this.originalClassName;
        }
    }
    
    window.addEventListener('load', load);
})();

