var moodio = moodio || {};
moodio.punc = moodio.punc || {};

moodio.punc.getSessionkeyEndpoint = "/api/autocomplete/sessionkey";
moodio.punc.searchEndpoint = "/api/autocomplete/search";


(function(){

    function load(){
        
        console.log("loading punc autocomplete");
        
        //get all the locationbox inputs
        var locInputs = document.querySelectorAll("input[data-punc-addressbox]");
        for(var i = 0, len = locInputs.length; i<len; i++)
        {
            new PuncLocationInput(locInputs[i]);
        }
    }

    function PuncLocationInput(inputElem)
    {

        //elements

        //get the autocomplete element
        this.uid = inputElem.dataset.puncAddressbox;
        console.dir("Creating punc autocomplete address box for " + this.uid);
        
        //store/get all the elements we'll need
        this.el = inputElem;
        this.dropdownEl = document.querySelector("[data-punc-addressbox-ac='" + this.uid + "']");
        if(this.dropdownEl!=null){
            this.dropdownElUl = this.dropdownEl.getElementsByTagName("UL")[0];
        }
        
        //setup uris
        this.sessionKeyApiUri = moodio.punc.apiHost + moodio.punc.getSessionkeyEndpoint;
        this.searchApiUri = moodio.punc.apiHost + moodio.punc.searchEndpoint;

        //properties
        this.ready = false;
        this.sessionKey = null;

        //ints for referencing the call number and ensuring only newer calls override old ones
        this.callNumber = 0; 
        this.lastReturnedCallNumber = 0;

        //attach a listeneder
        var me = this;
        inputElem.addEventListener('input', this.inputChange.bind(me));
        inputElem.parentElement.addEventListener('blur', this.closeDropdown.bind(me) );

        //load a new session key
        this.loadSessionKey();

        //attach on change event
    }

    PuncLocationInput.prototype.inputChange = function(e)
    {
        if(!this.ready){
            console.dir("not yet ready");
            return;
        }

        var req = new XMLHttpRequest();

        var me = this;

        req.onreadystatechange = function(){
            if(req.readyState === 4 && req.status === 200){
                this.updateResults(JSON.parse(req.response));
            }
        }.bind(me);

        var searchParams = "?key=" + this.sessionKey + "&q=" + this.el.value + "&callNumber=" + ++this.callNumber;
        // console.log("search params: " + searchParams);
        req.open("GET", this.searchApiUri + searchParams);
        req.send();
    }

    PuncLocationInput.prototype.loadSessionKey = function()
    {
        var req = new XMLHttpRequest();

        //bind events
        var me = this;
        req.onreadystatechange = function(){
            if(req.readyState === 4 && req.status === 200){
                this.sessionKey = req.response;
                // console.log("session key: " + this.sessionKey);
                this.ready = true;
            }
        }.bind(me);

        req.open("GET", this.sessionKeyApiUri);
        req.send();

    }

    PuncLocationInput.prototype.updateResults = function(e)
    {
        if(this.lastReturnedCallNumber >= e.callNumber){
            return;
        }

        this.lastReturnedCallNumber = e.callNumber;

        while(this.dropdownElUl.firstChild){
            this.dropdownElUl.removeChild(this.dropdownElUl.firstChild);
        }

        for(var i = 0, len = e.predictions.length; i<len; i++)
        {
            //too keep scope unique in each loop
            var me = this;
            (function(me){
                var newEl = document.createElement("li");
                var selection = e.predictions[i];
                newEl.innerHTML = selection;

                newEl.addEventListener('click', (ev) => {me.updateSelection(selection);});
                me.dropdownElUl.appendChild(newEl);
            })(me);
        }
    }

    PuncLocationInput.prototype.updateSelection = function(selection){
        this.el.value = selection;
        this.closeDropdown();
    }

    PuncLocationInput.prototype.closeDropdown = function(){
        // console.log("closing..");
        var emptyRes = {};
        emptyRes.callNumber = ++this.callNumber;
        emptyRes.predictions = [];
        this.updateResults(emptyRes);
    }

    window.addEventListener('load', load);
})();