/*
 * TODO: 
 * Save id of timer in local storage
 * Add logic to retrieve timer on reload
 * add backend logic to email referee
 * Add email signup for updates on real muljin
 * add update gps location button in timer
 * Add sound notification on you're late
 * add time to leave, 5 minutes before actual leave time
 * Add privacy statement and T&C
*/


var moodio = moodio || {};
moodio.punc = moodio.punc || {};

moodio.punc.apiHost = "http://api.nowleave.com";
moodio.punc.capturePaymentEndpoint = "/api/expertmode/payments/authorize";
moodio.punc.startTimer = "/api/timers";
moodio.punc.recaptchaSiteId = "6LeGabIUAAAAAEThhGHi7_NZYL_cEg1A4fRBSeXH";
moodio.punc.stripeKey = "pk_test_kXIE0Ku9iDwdXCN5F1QvqK6k005nRrDLdB";

(function(){
    
    function load()
    {
        //get the form
        var form = document.querySelector("form[data-punc-widget]");

        if(form == null){
            console.error("Cannot find core form");
            return;
        }

        new PuncCore(form);
    }

    function PuncCore(formEl)
    {
        //dev mode
        this.developmentMode = true;

        // flag for when a request is inflight
        this.inflight = false;

        //page
        this.currentPage = 1;

        //elements
        this.elements = {};

        this.elements["form"] = formEl;
        this.elements["body"] = document.querySelector('body');

        //get elements
        this.elements["location"] = document.querySelector("[data-punc-addressbox='acl']")
        this.elements["destination"] = document.querySelector("[data-punc-addressbox='acd']")

        this.elements["arrival-hour"] = document.getElementById("arrival-hour");
        this.elements["arrival-minute"] = document.getElementById("arrival-minute");
        this.elements["arrival-period"] = document.getElementById("arrival-period");

        this.elements["transport-mode"] = document.getElementsByName("transport-mode");
        this.elements["expert-mode"] = document.getElementsByName("expert-mode");
        this.elements["payment-container"] = document.getElementById("payment-container");
        this.elements["customer-name"] = document.getElementById("customer-name");
        this.elements["customer-email"] = document.getElementById("customer-email");

        this.elements["confirmation-method"] = document.getElementsByName("confirmation-method");
        this.elements["referee-email"] = document.getElementById("referee-email");
        this.elements["referee-email-container"] = document.getElementById("referee-email-container");

        //error
        this.elements["error-container"] = document.getElementById("error-container");

        this.elements["submit"] = document.getElementById("start-button");

        this.elements["page-back"] = document.getElementById("form-pageback-button");

        //counter app
        this.elements["timer-container"] = document.getElementById("timer-time-container");
        this.elements["timer-title"] = document.getElementById("timer-time-title");
        this.elements["departure-time"] = document.getElementById("departure-time");
        this.elements["arrival-time"] = document.getElementById("arrival-time");
        this.elements["journey-duration"] = document.getElementById("journey-duration");
        this.elements["journey-origin"] = document.getElementById("journey-origin");
        this.elements["journey-destination"] = document.getElementById("journey-destination");


        //set the default state
        this.state = "active";

        //store for binding
        var me = this;

        //attach events to expert mode
        this.expertMode = false;
        for(var i = 0, len = this.elements["expert-mode"].length; i<len; i++){
            var el = this.elements["expert-mode"][i];
            el.addEventListener('change', this.expertModeChange.bind(me))

            //check if this is selected by default
            if(el.checked){
                this.setExpertMode(el.value == 'true');
            }
        }

        //attach events to confirmation methods
        for(var i = 0, len = this.elements["confirmation-method"].length; i<len; i++){
            var el = this.elements["confirmation-method"][i];
            el.addEventListener('change',this.confirmationMethodChange.bind(me));

            if(el.checked){
                this.setConfirmationMethod(el.value);
            }
        }
        
        //attach events to submit
        this.elements["submit"].addEventListener('click', this.submit.bind(me));

        this.elements["page-back"].addEventListener('click', function(){this.setPage(this.currentPage - 1)}.bind(me));

        //setup timer to 2 hours from now
        this.setDefaultTime();


        //check if should load timer
        var params = new URLSearchParams(window.location.search);
        if(params.has('tid')){
            var tid = params.get('tid');
            this.loadTimer(tid);
        }else{
            console.log("tid doesnt exist");
            //load stripe
            this.loadStripe();
        }

        
    }
    
    PuncCore.prototype.loadTimer = function(timerId)
    {
        var url = moodio.punc.apiHost + moodio.punc.startTimer + '/' + timerId;

        var me = this;

        var req = new XMLHttpRequest();
        req.onreadystatechange = function(){
            if(req.readyState === 4)
            {
                if(req.status === 200){
                    console.log("got the timer id");
                    this.submitRequestCallback(req);
                }else{
                    console.error("failed to load timer with given id");
                    this.loadStripe();
                }
            }
        }.bind(me);

        req.open("GET",url);
        req.setRequestHeader("content-type", "application/json");
        req.send();
    }

    // load the stripe element
    PuncCore.prototype.loadStripe = function(){
        //stripe
        this.stripe = Stripe(moodio.punc.stripeKey);

        // Create an instance of Elements.
        this.stripeElements = this.stripe.elements();

        var style = {
            base: {
              fontSize: '0.9rem',
              color: "#32325d",
              lineHeight:"32px"
            }
          };
          
          // Create an instance of the card Element.
          this.stripeCard = this.stripeElements.create('card', {style: style, hidePostalCode: true});
          
          // Add an instance of the card Element into the `card-element` <div>.
          this.stripeCard.mount('#stripe-container');

          //add card listener
          var me = this;

          this.stripeCard.addEventListener('change', function(event){
            if(event.error){
                this.displayError(event.error.message);
            }else{
                this.displayError('');
            }
          }.bind(me));
    }

    PuncCore.prototype.setDefaultTime = function()
    {
        //set the time to ~2.5 hours from now
        var setTime = new Date((new Date()).getTime() + (2.75 * 60 * 60 * 1000));
        var hour = setTime.getHours();
        

        var setPeriod = hour > 11 ? "PM" : "AM";
        var setHour = hour >= 12 ? hour - 12 : hour;
        // var setHour = setHour == 12 ? 0 : setHour;
        var setMinute = Math.floor(setTime.getMinutes()/15)*15;

        if(setMinute == 0){
            setMinute = "00";
        }

        this.elements["arrival-hour"].value = setHour;
        this.elements["arrival-minute"].value = setMinute;
        this.elements["arrival-period"].value = setPeriod;

        console.log("setting as " + setHour + ":" + setMinute + ":" + setPeriod);

    }

    // form is submitted
    PuncCore.prototype.submit = function(event)
    {
        //prevent the default event
        if(event){
            event.preventDefault();
        }

        if(this.inflight === true)
        {
            this.log("Cannot submit while in inflight");
            return;
        }

        if(this.expertMode && this.currentPage === 1)
        {
            if(this.validateForm(1)){
                this.setPage(2);
            }
            return;
        }

        //validate final form
        if(!this.validateForm()){
            return;
        }

        this.setStatus(true);
        this.setPage(3);
        this.displayError("");
        this.getRecaptchaCode(this.getFormattedStartRequest()); 
    }

    PuncCore.prototype.confirmationMethodChange = function(event){
        this.setConfirmationMethod(event.srcElement.value);
    }

    PuncCore.prototype.expertModeChange = function(event)
    {
        this.log("change expert mode");
        this.dir(this);
        var val = event.srcElement.value == 'true';
        this.setExpertMode(val);
    }

    PuncCore.prototype.setConfirmationMethod = function(mode)
    {
        this.log("Confirmation method changed to: " + mode);
        
        if(mode === 'Gps')
        {
            this.elements["referee-email-container"].className = this.elements["referee-email-container"].className + " hidden";
        }
        else if(mode === 'LinkConfirmation'){
            this.elements["referee-email-container"].className = this.elements["referee-email-container"].className.replace(" hidden", "");
        }

        this.confirmationMethod = mode;
    }

    PuncCore.prototype.setExpertMode = function(enabled)
    {
        if(this.inflight === true){
            return;
        }

        this.expertMode = enabled;
        if(enabled){
            this.elements["form"].className = this.elements["form"].className + " expert-mode";
            this.elements["submit"].textContent = "NEXT";
        }else{
            this.elements["form"].className = this.elements["form"].className.replace(" expert-mode", "");
            this.elements["submit"].textContent = "START";
        }
    }

    PuncCore.prototype.setPage = function(num)
    {
        this.log("attempting to set page as: " + num);

        if(this.inflight === true
            && num != 3){
            this.log("both inflight and page is not 3");
            return;
        }

        this.currentPage = num;

        if(num === 1){
            this.elements["form"].className = this.elements["form"].className.replace(" page2", "").replace(" page3","");
            this.elements["submit"].textContent = this.expertMode ? "NEXT" : "START";
        } else if(num === 2){
            this.elements["form"].className = this.elements["form"].className.replace(" page3", "") + " page2";
            this.elements["submit"].textContent = "START";
        } else if(num === 3){
            this.elements["form"].className = this.elements["form"].className.replace(" page2", "") + " page3";
        }
    }

    PuncCore.prototype.setStatus = function(inflight){
        
        if(this.inflight === inflight){
            return;
        }

        if(inflight)
        {
            this.elements["submit"].disabled = true;
            this.elements["submit"].textContent = "SUBMITING...";
            this.elements["form"].className = this.elements["form"].className + " submitting";
        }else{
            this.elements["submit"].disabled = false;
            this.elements["submit"].textContent = "START";
            this.elements["form"].className = this.elements["form"].className.replace(" submitting","");
        }

        this.inflight = inflight;
        this.log("set inflight as " + inflight);
    }

    // STEP 1 : Get the request formatted object
    //format the input into a json object
    PuncCore.prototype.getFormattedStartRequest = function()
    {
        //set status
        this.setStatus(true);

        //validate the form
        if(!this.validateForm())
        {
            this.displayError("Please correct the items above.");
            this.setStatus(false);
            return;
        }

        //return the results
        var res = {};

        res["location"] = this.elements["location"].value;
        res["destination"] = this.elements["destination"].value;

        
        //set up the time in the users local timezone then convert to UTC
        var arrivalOffset = (this.elements["arrival-period"].value === 'PM' ? 12 : 0);
        var arrivalHour = parseInt(this.elements["arrival-hour"].value) + arrivalOffset;
        var arrivalMinute = this.elements["arrival-minute"].value;

        var arrivalTime = new Date();
        arrivalTime.setMilliseconds(0);
        arrivalTime.setSeconds(0);
        arrivalTime.setMinutes(arrivalMinute);
        arrivalTime.setHours(arrivalHour);

        //if time already passed, set to tomorrow
        if(arrivalTime < (new Date()) ){
            arrivalTime.setDate( arrivalTime.getDate() + 1);
        }
        res["arrivalTimeUtc"] = arrivalTime.toUTCString();
        
        //get the transport method
        res["transportMode"] = this.getRadioValue("transport-mode");

        //get the expert mode
        res["expertMode"] = this.getRadioValue("expert-mode") == 'true';

        if(res.expertMode){
            //get the confirmation method
            res["confirmationMethod"] = this.getRadioValue("confirmation-method");
            
            //get the email if required
            if(res.confirmationMethod === 'LinkConfirmation')
            {
                res["refereeEmail"] = this.elements["referee-email"].value;
            }

            //get the customer name
            res["customerName"] = this.elements["customer-name"].value;
        }

        this.dir(res);
        return res;
    }

    // STEP 2: Get the recaptcha code
    PuncCore.prototype.getRecaptchaCode = function(startRequest){
        var me = this;
        grecaptcha.ready(
            function() {
                var me = this;
                grecaptcha.execute(moodio.punc.recaptchaSiteId , {action: 'starttimer'})
                .then(
                    function(token) {
                        startRequest["verifyToken"] = token;
                        this.recaptchaCallback(startRequest);
                    }.bind(me)
                );
        }.bind(me));
    }

    // STEP 3: Continue after call back.
    // If expert mode, then create the payment token
    // if not expert mode, submit the request to start directly
    PuncCore.prototype.recaptchaCallback = function(startRequest){
        //call the api with the request or get
        if(startRequest.expertMode)
        {
            this.createPaymentMethod(startRequest);
        }else{
            this.submitRequest(startRequest);            
        }
    }

    // STEP 4: Create the payment token
    PuncCore.prototype.createPaymentMethod = function(startRequest)
    {
        var options = {billing_details: {name: this.elements["customer-name"].value}};
        
        var me = this;
        this.stripe.createPaymentMethod('card', this.stripeCard, options).then( function(result){ this.createPaymentMethodCallback(result, startRequest);}.bind(me) );
    }

    // STEP 5: Stripe create payment method callback
    PuncCore.prototype.createPaymentMethodCallback = function(result, startRequest)
    {
        if(result.error){
            this.log("result error!");
            this.setStatus(false);
            this.displayError(result.error.message);
            this.setPage(2);
        }
        else{
            startRequest["paymentMethodId"] = result.paymentMethod.id;

            if(startRequest.expertMode){
                this.capturePayment(startRequest);
            }else{
                this.submitRequest(startRequest);
            } 
        }
    }

    // STEP 6 : Capture payment on backend
    PuncCore.prototype.capturePayment = function(startRequest)
    {
        var req = new XMLHttpRequest();
        var me = this;

        req.onreadystatechange = function(){
            if(req.readyState === 4 && req.status === 200){
                this.capturePaymentCallback(startRequest, JSON.parse(req.response));
            }else if(req.status >= 400){
                console.error("error calling backend! Status code:" + req.status);
                this.dir(req.response);
                this.setStatus(false);
                this.setPage(2);
            }
        }.bind(me);

        var body = JSON.stringify(startRequest.paymentMethodId);
        
        req.open("POST", moodio.punc.apiHost + moodio.punc.capturePaymentEndpoint);
        req.setRequestHeader("content-type", "application/json");
        req.send(body);
    }

    // STEP 7 : Callback for capturing payment,
    PuncCore.prototype.capturePaymentCallback = function(startRequest, response)
    {
        if(response.requiresClientAction)
        {
            this.handleCardAction(startRequest, response.clientSecret);
        } 
        else if (response.success)
        {
            this.log("straight payment success:");
            startRequest["paymentIntentId"] = response.paymentIntentId;
            this.submitRequest(startRequest);
        }else {
            console.error("failed to capture payment.");
            this.setStatus(false);
        }
    }

    // STEP 8: If card payment requires validation, validate
    PuncCore.prototype.handleCardAction = function(startRequest, clientSecret)
    {
        var me = this;
        this.stripe.handleCardAction(clientSecret)
            .then(function(result){this.handleCardActionCallback(startRequest, result)}.bind(me));
    }

    // STEP 9: Handle call back for card verification
    PuncCore.prototype.handleCardActionCallback = function(startRequest, result)
    {
        if(result.error){
            this.displayError("Error authorizing payment");
            this.setStatus(false);
        }else{
            startRequest["paymentIntentId"] = result.paymentIntent.id;
            this.submitRequest(startRequest);
        }
    }

    // STEP (10): Submit the request to the server, and get the response
    PuncCore.prototype.submitRequest = function(startRequest)
    {
        this.log("submitting request to server");
        this.dir(startRequest);

        var me = this;

        var req = new XMLHttpRequest();
        req.onreadystatechange = function(){
            if(req.readyState === 4){
                this.submitRequestCallback(req);
            }
        }.bind(me);

        req.open("POST",moodio.punc.apiHost + moodio.punc.startTimer);
        req.setRequestHeader("content-type", "application/json");
        req.send(JSON.stringify(startRequest));

    }

    // STEP 11: Get the results. Display error or start the timer!
    PuncCore.prototype.submitRequestCallback = function(req)
    {
        console.log("submit to server callback");
        var res = JSON.parse(req.response);

        if(req.status === 200)
        {
            console.log("req status is 200");
            console.dir(this);
            window.history.pushState(null, '', '?tid='+res.id);

            //set the timer
            this.timer = res;
            this.setTimerState();     
        }else{
            this.setStatus(false);
            this.setPage(1);
            this.displayError(res.error == null ? "Unknown error" : res.error);
        }
    }


    /// Start the timer
    PuncCore.prototype.startLeaveTimer = function(timer)
    {

        this.elements["form"].className = this.elements["form"].className + " timer-active";

        //set journey information
        this.elements["journey-duration"].textContent = Math.floor(this.timer.travelDuration/60) + "mins";
        this.elements["departure-time"].textContent = this.getTimeFromUnixEpoch(timer.departureTimeEpoch);
        this.elements["arrival-time"].textContent = this.getTimeFromUnixEpoch(timer.estimatedArrivalTimeEpoch);

        this.elements["journey-origin"].textContent = timer.location;
        this.elements["journey-destination"].textContent = timer.destination;
        
        //set interval to continue updating timer
        var me = this;
        this.updateTimer = window.setInterval(this.updateLeaveTimer.bind(me),1000);

        //TODO: If expert mode, reload timer at set intervals
    }

    PuncCore.prototype.updateLeaveTimer = function()
    {
        var secsRemaining = this.timer.departureTimeEpoch - Math.floor((new Date().getTime())/1000);

        if(secsRemaining <= 0 
            || (secsRemaining < 300 && this.timer.Status == "Active")){
            //clear timer
            window.clearInterval(this.updateTimer);
            //update the timer and status
            this.loadTimer(this.timer.id);
        }
        else{
            this.updateTimerWidget(secsRemaining);
        }
    }

    PuncCore.prototype.startJourneyTimer = function()
    {
        console.log("starting journey");

        var me = this;
        this.updateTimer = window.setInterval(this.updateTimer.bind(me),1000);
    }

    PuncCore.prototype.updateJourneyTimer = function()
    {
        var secsRemaining = this.timer.arrivalTimeEpoch - Math.floor((new Date().getTime())/1000);

        if(secsRemaining <= 0){
            //update final status
            window.clearInterval(this.updateTimer);
            this.loadTimer(this.timer.id);
        }else{
            this.updateTimerWidget(secsRemaining);
        }
    }

    PuncCore.prototype.startAwaitConfirmation = function()
    {
        var me = that;
        this.updateTimer = window.setInterval(function(){this.loadTimer(this.timer.id);}.bind(me),1000);
    }

    //update status of timer
    // possible states are:
    // Active
    // TimeToLeave
    // Enroute
    // AwaitingConfirmation
    // OnTime
    // Late
    // Cancelled
    // Failed

    PuncCore.prototype.setTimerStatus = function(status)
    {
        if(this.state === state){
            return;
        }

        //all state currently does is change the classname of body
        var classname = "";
        var title = "";
        switch(status){
            case "Active":
                className = "active";
                title = "Leave By";
                this.startLeaveTimer();
                break;
            case "TimeToLeave":
                classname = "almost";
                title = "Leave By";
                break;
            case "Enroute":
                classname = "enroute";
                title = "Arrive by";
                this.startJourneyTimer();
                break;
            case "AwaitingConfirmation":
                break;
                classname = "";
                title = "Leave Before";
                this.startAwaitConfirmation();
                break;
            case "OnTime":
                classname = "ontime";
                title = "You made it";
                break;
            case "Late":
                classname = "late";
                title = "Late by";
                break;
            default:
                break;
        }

        this.elements["body"].className = classname;
        this.elements["timer-title"].textContent = title;
        
    }

    PuncCore.prototype.updateTimerWidget = function(seconds){
        this.elements["timer-container"].textContent = formatTimeRemaingStopwatch(seconds);
    }

    // display an error to the user
    PuncCore.prototype.displayError = function(error)
    {
        this.elements["error-container"].textContent = error;
        if(error === ''){
            this.elements["error-container"].className = this.elements["error-container"].className.replace(" error", "");
        }else{
            this.elements["error-container"].className = this.elements["error-container"].className + " error";
        }
    }

    //get the value for a radiobutton group
    PuncCore.prototype.getRadioValue = function(elementsName)
    {
        for(var i = 0, len = this.elements[elementsName].length; i<len; i++){
            if(this.elements[elementsName][i].checked){
                return this.elements[elementsName][i].value;
            }
        }
    }

    PuncCore.prototype.getTimeFromUnixEpoch = function(epoch){
        var time = new Date(epoch*1000);
        
        var hours = time.getHours();
        var period = hours > 11 ? "PM" : "AM";
        hours = hours % 12;
        if(hours == 0){
            hours = 12;
        }
        var minutes = time.getMinutes();
        if(minutes<10){
            minutes = "0"+minutes;
        }
        return hours + ":" + minutes + period;
    }

    // validate the user input form
    PuncCore.prototype.validateForm = function(page)
    {
        var errors = [];
        if(typeof page === 'undefined' || page === 1){
            if(this.elements["location"].value.length < 10)
            {
                errors.push("Please enter a valid location.");
            }

            if(this.elements["destination"].value.length < 10)
            {
                errors.push("Please enter a valid destination.");
            }
        }

        if(typeof page === 'undefined' || page == 2)
        {
            if(this.confirmationMethod == "LinkConfirmation"){
                var email = this.elements["referee-email"].value;
                this.log("email");

                if(email.length < 4 || !email.includes('@')){
                    errors.push("Please enter a valid email");
                }
            } 
            if(this.confirmationMethod == "Gps"){
                //ensure geolcoation is supported
                if(!("geolocation" in navigator))
                {
                    errors.push("Geolocation is not available. Select a different confirmation method.");
                }
            }
            else{
                this.log("confirm method is " + this.confirmationMethod);
            }
        }


        if(errors.length > 0){
            this.displayError(errors);
            return false;
        } else{
            this.displayError('');
            return true;
        }

    }


    // format time remaining
    function formatTimeRemaingStopwatch(seconds)
    {
        var res = "";

        var hours = Math.floor(seconds/3600);
        var minutes = Math.floor((seconds - hours*3600)/60);
        var seconds = Math.floor( seconds - (hours*3600) - (minutes*60));

        if(hours<10){
            res += "0";
        }
        res += hours.toString() + ":";

        if(minutes<10){
            res += "0";
        }
        res += minutes.toString() + ":";

        if(seconds < 10){
            res += "0";
        }
        res += seconds.toString();

        return res;

    }

    //Logging
    PuncCore.prototype.log = function(text){
        if(this.developmentMode === true)
        {
            console.log(text);
        }
    }

    PuncCore.prototype.dir = function(object){
        if(this.developmentMode === true){
            console.dir(object);
        }
    }

    window.addEventListener('load', load);
})();