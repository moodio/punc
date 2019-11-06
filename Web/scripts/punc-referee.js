var moodio = moodio || {};
moodio.punc = moodio.punc || {};

moodio.punc.apiHost = "https://localhost:5001";
// moodio.punc.apiHost = "http://api.nowleave.com";
moodio.punc.refereeEndpoint = "/api/referee/timer";

(function(){

    function load(){
        //change current page
        //get the buttons

        //add click events
        
        //

        new PuncReferee();
    }

    function PuncReferee()
    {
        this.inflight = false;
        this.elements = {};

        this.elements["widget-container"] = document.getElementById('widget-container');
        this.elements["customer-name"] = document.querySelectorAll('[data-moodio-customer-name]');
        this.elements["destination"] = document.querySelectorAll('[data-moodio-destination]');
        this.elements["arrival-time"] = document.querySelectorAll('[data-moodio-arrivaltime]')
        this.elements["was-ontime"] = document.querySelectorAll('[data-moodio-wasontime]');
        this.elements["error-output"] = document.getElementById('error-output');


        //load ontime button
        var ontimeButton = document.getElementById('button-ontime');
        var lateButton = document.getElementById('button-late');

        var me = this;
        ontimeButton.addEventListener('click', (function(event){this.confirmationEvent(event, true);}).bind(me));
        lateButton.addEventListener('click', (function(event){this.confirmationEvent(event, false);}).bind(me));

        //check if should load timer
        var params = new URLSearchParams(window.location.search);
        if(params.has('code')){
            this.code = params.get('code');

            if(params.has('ontime'))
            {
                var pot = params.get('ontime');
                if(pot === 'true'){
                    this.presetOnTime = true;
                }else if (pot === 'false'){
                    this.presetOnTime = false;
                }
            }
            this.loadTimerDetails();
        }else{
            console.log("code not included");
            this.displayError('No code supplied. Please contact <a href="mailto:info@nowleave.com">info@nowleave.com</a> for support.')
        }
    }

    PuncReferee.prototype.loadTimerDetails = function()
    {
        var url = moodio.punc.apiHost + moodio.punc.refereeEndpoint + '/' + this.code;
        var me = this;

        var req = new XMLHttpRequest();
        req.onreadystatechange = function(){
            if(req.readyState === 4)
            {
                if(req.status === 200){
                    console.log("got the timer details");
                    this.loadTimerCallback(req);
                } 
                else if(req.status === 404)
                {
                    this.displayError('Invalid code supplied.');
                }
                else{
                    
                    console.error("failed to load timer with given id");
                    this.displayError('Failed to load timer. Please ensure given code is correct or contact <a href="mailto:info@nowleave.com">info@nowleave.com</a> for help.');
                }
            }
        }.bind(me);

        req.open("GET",url);
        req.setRequestHeader("content-type", "application/json");
        req.send();
    }
    
    PuncReferee.prototype.loadTimerCallback = function(req)
    {
        console.dir(req);
        this.timer = JSON.parse(req.response);

        if(this.presetOnTime != null){
            console.log("Preset ontime: " + this.presetOnTime.toString());
            this.submitOntime(this.presetOnTime);
        }else{
            this.updateState();
        }
        
    }

    PuncReferee.prototype.confirmationEvent = function(event, ontime)
    {
         //prevent the default event
         if(event){
            event.preventDefault();
        }

        this.submitOntime(ontime);

    }

    PuncReferee.prototype.submitOntime = function(ontime)
    {
        console.log("submitting ontime");

        if(this.inflight === true)
        {
            return;
        }
        this.inflight = true;

        var url = moodio.punc.apiHost + moodio.punc.refereeEndpoint + '/' + this.code + '?onTime=' + ontime.toString();
        var me = this;

        var req = new XMLHttpRequest();
        req.onreadystatechange = function(){
            if(req.readyState === 4)
            {
                if(req.status === 200){
                    console.log("submitted ontime");
                    this.displayConfirmation(ontime);
                } 
                else if(req.status === 404)
                {
                    this.displayError('Invalid code supplied.');
                }
                else{
                    this.displayError('Sorry, there was an error submitting your confirmation. Please try again later or contact <a href="mailto:info@nowleave.com">info@nowleave.com</a> for support.');
                }
            }
        }.bind(me);

        req.open("POST",url);
        req.setRequestHeader("content-type", "application/json");
        req.send();

    }

    PuncReferee.prototype.updateState = function()
    {
        console.log('Timer status: ' + this.timer.status);

        switch(this.timer.status){
            case "ConfirmedLate":
                this.displayError('Timer has already been confirmed as late.');
                break;
            case "OnTime":
                this.displayError('Timer has already been confirmed as on time.');
                break;
            case "Active":
            case "Enroute":
            case "TimeToLeave":
                this.displayError('Timer is still active. Please come back on or after the arrival time of ' + this.timer.arrivalTimeUtc);
                break;
            case "AwaitingConfirmation":
            case "UnconfirmedLate":
                this.populateTimerDetails();
                break;
            default:
                this.displayError('Sorry but the timer cannot be updated.');
                break;
        }

    }

    PuncReferee.prototype.populateTimerDetails = function()
    {
        for(var i = 0, len = this.elements["customer-name"].length; i<len; i++){
            this.elements["customer-name"][i].textContent = this.timer.customerName;
        }

        var formattedArrivalTime = formatTimeForDisplay(this.timer.arrivalTimeUtc);
        for(var i = 0, len = this.elements["arrival-time"].length; i<len; i++){
            this.elements["arrival-time"][i].textContent = formattedArrivalTime;
        }
        
        for(var i = 0, len = this.elements["destination"].length; i<len; i++){
            this.elements["destination"][i].textContent = this.timer.destination.toString();
        }

        this.setPage(2);
    }

    PuncReferee.prototype.setPage = function(num)
    {
        var newClassname = this.elements["widget-container"].className.replace(/page[0-9]/i,"");
        this.elements["widget-container"].className = newClassname + " page"+num;
    }

    PuncReferee.prototype.displayConfirmation = function(ontime)
    {

        var was = "";
        if(ontime){
            was = "was"
        }else{
            was = "was not";
        }
        console.log("displaying as " + was);
        console.log("was ontime length: " + this.elements["was-ontime"].length);

        for(var i = 0, len = this.elements["was-ontime"].length; i<len; i++){
            this.elements["was-ontime"][i].textContent = was;
        }
        
        this.setPage(3);
    }

    PuncReferee.prototype.displayError = function(error)
    {
        this.setPage(4);
        this.elements["error-output"].innerHTML = error;
    }

    function formatTimeForDisplay(time)
    {
        var output = "";
        
        
        console.log("utc arrival time: " + time);

        time = new Date(time);
        //offset utc
        var localArrivalTime = new Date(time.getTime() - time.getTimezoneOffset() * 60 * 1000);
        
        console.log("localArrivalTime: " + localArrivalTime);

        var curDate = new Date();

        var daysDif = (curDate.getDate() - localArrivalTime.getDate());
        if(daysDif==0){
            output += "Today"
        }else if (daysDif == 1){
            output += "Yesterday"
        }else if (daysDif < 8)
        {
            output += "Last " + localArrivalTime.toLocaleDateString('en-GB', { weekday: 'long' });
        }

        output += " at ";

        var period = "AM";
        var hour = localArrivalTime.getHours();

        if(hour > 11){
            period = "PM";
            hour = hour - 12;
        }

        if(hour == 0){
            hour = 12;
        }

        output += hour + ":" + localArrivalTime.getMinutes() + period;

        return output;



        return time.toString();
        //convert to utc

        //convert date

        //time

        //am/pm
    }

    window.addEventListener('load', load);
})();
