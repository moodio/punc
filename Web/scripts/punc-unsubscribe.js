var moodio = moodio || {};
moodio.punc = moodio.punc || {};

// moodio.punc.apiHost = "https://localhost:5001";
moodio.punc.apiHost = "http://api.nowleave.com";
moodio.punc.subscribeEnpoint = "/api/subscriptions/subscribe";
moodio.punc.unsubscribeEnpoint = "/api/subscriptions/unsubscribe";

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
        this.elements["error-output"] = document.getElementById('error-output');

        //load buttons
        var unsubButton = document.getElementById('button-unsubscribe');
        var subButton = document.getElementById('button-subscribe');

        var me = this;
        subButton.addEventListener('click', this.subscribe.bind(me));
        unsubButton.addEventListener('click', this.unsubscribe.bind(me));

        //check if should load timer
        var params = new URLSearchParams(window.location.search);
        if(params.has('email')){
            this.email = params.get('email');
            this.unsubscribe();
        }else{
            this.displayError('Sorry but it appears there is something wrong with the link you were supplied. Please email <a href="mailto:info@nowleave.com">info@nowleave.com</a> with the subject unsubscribe and we\'ll manually process your unsubscription right away.');
        }
    }

    PuncReferee.prototype.unsubscribe = function(event)
    {
        if(event){
            event.preventDefault();
        }

        var url = moodio.punc.apiHost + moodio.punc.unsubscribeEnpoint + '?email=' + this.email;
        var me = this;

        var req = new XMLHttpRequest();
        req.onreadystatechange = function(){
            if(req.readyState === 4)
            {
                if(req.status === 200){
                    this.displayConfirmation();
                } 
                else
                {
                    this.displayError('Sorry but it appears there is something wrong with the link you were supplied. Please email <a href="mailto:info@nowleave.com">info@nowleave.com</a> with the subject unsubscribe and we\'ll manually process your unsubscription right away.');
                }
            }
        }.bind(me);

        req.open("POST",url);
        req.setRequestHeader("content-type", "application/json");
        req.send();
    }

    PuncReferee.prototype.subscribe = function(event)
    {
        if(event){
            event.preventDefault();
        }

        var url = moodio.punc.apiHost + moodio.punc.subscribeEnpoint + '?email=' + this.email;
        var me = this;

        var req = new XMLHttpRequest();
        req.onreadystatechange = function(){
            if(req.readyState === 4)
            {
                if(req.status === 200){
                    this.displayResub();
                } 
                else
                {
                    this.displayError('Sorry but we couldn\'t resubscribe you. Please email <a href="mailto:info@nowleave.com">info@nowleave.com</a> with the subject subscribe and we\'ll manually process your request.');
                }
            }
        }.bind(me);

        req.open("POST",url);
        req.setRequestHeader("content-type", "application/json");
        req.send();
    }

    PuncReferee.prototype.setPage = function(num)
    {
        var newClassname = this.elements["widget-container"].className.replace(/page[0-9]/i,"");
        this.elements["widget-container"].className = newClassname + " page"+num;
    }

    PuncReferee.prototype.displayConfirmation = function()
    {
        this.setPage(2);
    }

    PuncReferee.prototype.displayResub = function(){
        this.setPage(3);
    }

    PuncReferee.prototype.displayError = function(error)
    {
        this.setPage(4);
        this.elements["error-output"].innerHTML = error;
    }

    

    window.addEventListener('load', load);
})();
