/// <summary>
/// Main namespace and namespace register method
/// </summary>
var SEL = {
    _loadedNamespaces: ["SEL"],
    registerNamespace: function(nameSpace)
    {
        var i,
            nameSpaceParts = nameSpace.split("."),
            root = window,
            typeOf,
            currentFullNs,
            currentNs;

        for (i = 0; i < nameSpaceParts.length; i++)
        {
            currentFullNs = nameSpace.substr(0, nameSpace.indexOf(nameSpaceParts[i]) + nameSpaceParts[i].length);
            currentNs = nameSpaceParts[i];
            
            if (currentNs !== "SEL")
            {
                typeOf = typeof (root[currentNs]);
                
                if (typeOf === "undefined")
                {
                    root[nameSpaceParts[i]] = {};
                    this._loadedNamespaces.push(currentFullNs);
                }
                else
                {
                    if (typeOf === "function")
                    {
                        SEL.Errors.InvalidOperation(currentFullNs + " already contains a class definition.");
                    }
                    
                    if (typeOf === "object" || currentNs instanceof Array)
                    {
                        SEL.Errors.InvalidOperation(currentFullNs + " already exists as an array.");
                    }
                }
            }
            root = root[currentNs];
        }
    }
};

/// <summary>
/// Returns a document.getElementById on controlID and returns the element
/// </summary>
var $g = function (controlId)
{
    if (controlId !== "" && document.getElementById(controlId) !== null)
    {
        return document.getElementById(controlId);
    }
    else
    {
        SEL.Errors.MissingObjects("Missing or invalid controlID on $g");
    }
};

/// <summary>
/// Returns an AjaxControlToolkit $find on controlID and returns the element - used for extended controls
/// </summary>
var $f = function (controlId)
{
    if (controlId !== "")
    {
        return $find(controlId);
    }
    else
    {
        SEL.Errors.MissingObjects("Missing controlID on $f");
    }
};

/// <summary>
/// Returns true if a control of the ID provided exists on the page
/// </summary>
var $e = function(controlId)
{
    var bRetVal = false;

    if (controlId !== "")
    {
        if (document.getElementById(controlId) !== null)
        {
            bRetVal = true;
        }
    }
    return bRetVal;
};

var $ddlValue = function (controlId)
{
    if (controlId !== "")
    {
        return $g(controlId).options[$g(controlId).selectedIndex].value;
    }
    else
    {
        SEL.Errors.MissingObjects("Missing controlID on $ddl");
    }
};

var $ddlText = function (controlId)
{
    if (controlId !== "")
    {
        return $g(controlId).options[$g(controlId).selectedIndex].value;
    }
    else
    {
        SEL.Errors.MissingObjects("Missing controlID on $ddl");
    }
};

var $ddlSetSelected = function(controlId, index, value)
{
    var i;
    if (controlId !== "")
    {
        for (i = 0; i < $g(controlId).options.length; i++)
        {
            if (index !== null)
            {
                if (i !== index)
                {
                    $g(controlId).options[i].selected = false;
                }
                else
                {
                    $g(controlId).options[i].selected = true;
                }
            }
            else if (value !== undefined && value !== null)
            {
                if (value !== parseInt($g(controlId).options[i].value, 10))
                {
                    $g(controlId).options[i].selected = false;
                }
                else
                {
                    $g(controlId).options[i].selected = true;
                }
            }
        }
    }
    else
    {
        SEL.Errors.MissingObjects("Missing controlID on $ddl");
    }
};

var $ddlPopulate = function (controlId, items, selectedValue)
{
    if ($e(controlId))
    {
        var i = 0,
            hasSelected = 0,
            oSelect = $g(controlId),
            oOption = document.createElement('option');

        oSelect.options.length = 0;

        oOption.value = '0';
        oOption.text = '[None]';
        oSelect.options[0] = oOption;

        if (typeof items !== "undefined" && items !== null)
        {
            for (i = 0; i < items.length; i++)
            {
                oOption = document.createElement("option");
                oOption.value = items[i].Value;
                oOption.text = items[i].Text;
                
                if (hasSelected === 0 && ((selectedValue !== undefined && selectedValue !== null && selectedValue === items[i].Value) || items[i].Selected === 'selected' || items[i].Selected === true))
                {
                    oOption.selected = true;
                    hasSelected = 1;
                }

                try
                {
                    // for IE earlier than version 8
                    oSelect.add(oOption, oSelect.options[null]);
                }
                catch (e)
                {
                    oSelect.add(oOption, null);
                }
            }
        }
        return true;
    }
    else
    {
        SEL.Errors.MissingObjects("Missing controlID on $ddlPopulate");
    }
};

String.prototype.format = function ()
{
    var args = arguments;
    return this.replace(/\{(\d+)\}/g, function (match, number)
    {
        return typeof args[number] !== "undefined"
            ? args[number]
            : match;
    });
};

/*
Replace HTML special characters within the string
< - \u003c
> - \u003e
" - \u0022
' - \u0027
\ - \u005c
& - \u0026
*/
String.prototype.htmlEncode = function()
{
    var htmlSpecialCharactersToReplace = {
        "\"": "&#34;",
        "&": "&#38;",
        "'": "&#39;",
        "<": "&#60;",
        ">": "&#62;",
        "\\": "&#92;"
    };

    return this.replace(/[<>"'\\&]/g, function (match) { return htmlSpecialCharactersToReplace[match] || match; });
};

if (!Date.prototype.toISOString) {
    (function () {

        function pad(number) {
            var r = String(number);
            if (r.length === 1) {
                r = '0' + r;
            }
            return r;
        }

        Date.prototype.toISOString = function () {
            return this.getUTCFullYear()
              + '-' + pad(this.getUTCMonth() + 1)
              + '-' + pad(this.getUTCDate())
              + 'T' + pad(this.getUTCHours())
              + ':' + pad(this.getUTCMinutes())
              + ':' + pad(this.getUTCSeconds())
              + '.' + String((this.getUTCMilliseconds() / 1000).toFixed(3)).slice(2, 5)
              + 'Z';
        };

    }());
};

$(document).ready(function () {
    
    // IE10+ have the new cross 'X' in every textbox, when users click it the field is cleared.
    // Some SEL code relies on keypresses, so when the field is cleared in this way unexpected behaviour can occur.
    // Unfortunately we can't target via CSS and hide the cross because this has no effect when using compat mode 
    // (see http://connect.microsoft.com/IE/feedback/details/783743/disable-ie10-clear-field-button-when-rendering-in-compatibility-mode)
    // So we hack, catch the mouseup event, if 10ms later the field is empty then we can assume the cross was clicked so we fake a keydown event
    if ($("html.ie")) {
        $("body").on("mouseup", "input[type=text]", function () {
            var input = $(this);
            if (input.val() == "") return;

            setTimeout(function () {
                if (input.val() == "") {

                    // the keydown event is used by jQuery autocompletes and the address widget 
                    input.trigger("keydown");
                }
            }, 10);
        });
    }
    
});
