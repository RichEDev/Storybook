//Last position
var lastPos = new Number();
var offsetPos = 0;

//Detect browser version
var BrowserDetect = {
	init: function () {
		this.browser = this.searchString(this.dataBrowser) || "An unknown browser";
		this.version = this.searchVersion(navigator.userAgent)
			|| this.searchVersion(navigator.appVersion)
			|| "an unknown version";
		this.OS = this.searchString(this.dataOS) || "an unknown OS";
	},
	searchString: function (data) {
		for (var i=0;i<data.length;i++)	{
			var dataString = data[i].string;
			var dataProp = data[i].prop;
			this.versionSearchString = data[i].versionSearch || data[i].identity;
			if (dataString) {
				if (dataString.indexOf(data[i].subString) != -1)
					return data[i].identity;
			}
			else if (dataProp)
				return data[i].identity;
		}
	},
	searchVersion: function (dataString) {
		var index = dataString.indexOf(this.versionSearchString);
		if (index == -1) return;
		return parseFloat(dataString.substring(index+this.versionSearchString.length+1));
	},
	dataBrowser: [
		{   //Apple Safari
			string: navigator.vendor,
			subString: "Apple",
			identity: "Safari"
		},
		{   //Opera
			prop: window.opera,
			identity: "Opera"
		},
		{   //Firefox
			string: navigator.userAgent,
			subString: "Firefox",
			identity: "Firefox"
		},
		{	//for newer Netscapes (6+)
			string: navigator.userAgent,
			subString: "Netscape",
			identity: "Netscape"
		},
		{   //Mosaic (MSIE)
			string: navigator.userAgent,
			subString: "MSIE",
			identity: "Explorer",
			versionSearch: "MSIE"
		},
		{   //Gecko / Mozilla
			string: navigator.userAgent,
			subString: "Gecko",
			identity: "Mozilla",
			versionSearch: "rv"
		},
		{ 	//for older Netscapes (4-)
			string: navigator.userAgent,
			subString: "Mozilla",
			identity: "Netscape",
			versionSearch: "Mozilla"
		}
	],
	dataOS : [
		{   //Microsoft Windows x86
			string: navigator.platform,
			subString: "Win32",
			identity: "Windows x86/i386/i686"
		},
		{   //Microsft Windows x64 
			string: navigator.platform,
			subString: "Win64",
			identity: "Windows x64/IA64/AMD64"
		},
		{   //Apple Mac
			string: navigator.platform,
			subString: "Mac",
			identity: "Mac"
		},
		{   //*nix (Linux)
			string: navigator.platform,
			subString: "Linux",
			identity: "Linux"
		}
	]

};

function insertAtCaret(myField, myValue) {
    //MSIE Support
    if (BrowserDetect.browser == 'Explorer') {
        myField.focus();
        setSelectedTextRange(myField, lastPos, lastPos);
        sel = document.selection.createRange();
        sel.text = myValue;
    }
    //Gecko, Mozilla, Firefox, Netscape & Opera Support
    else if (BrowserDetect.browser == 'Mozilla' || BrowserDetect.browser == 'Opera' || BrowserDetect.browser == 'Netscape' || BrowserDetect.browser == 'Firefox') {
        var startPos = myField.selectionStart;
        var endPos = myField.selectionEnd;
        myField.value = myField.value.substring(0, startPos) + myValue + myField.value.substring(endPos, myField.value.length);
    //Safari and other unrecognised browsers
    } else {
        myField.value += myValue;
    }
}

function insertValue(oField, sValue){
    insertAtCaret(oField, sValue);
}

function GetCursorPosition(elm) {
    var pos = new Number();
    if (typeof elm.selectionStart != "undefined")
    {
        pos = 0;
    }
    else if (document.selection)
    {
        elm.focus();
        pos =  Math.abs(document.selection.createRange().moveStart("character",-1000000));
    }
    lastPos = pos - offsetPos;
    return pos;
    
}

function setSelectedTextRange(elm, selectionStart, selectionEnd) {
    if (elm.setSelectionRange) {
        elm.focus();
        elm.setSelectionRange(selectionStart, selectionEnd);
    }
    else if (elm.createTextRange) 
    {
        var range = elm.createTextRange();
        range.collapse(true);
        range.moveEnd('character', selectionEnd);
        range.moveStart('character', selectionStart);
        range.select();
    }
}




//Get the browser version
BrowserDetect.init();


//Node Object
function nodeObject(){
    var ID;
    var FunctionName;
    var Description;
    var Remarks;
    var Example;
    var Syntax;
}