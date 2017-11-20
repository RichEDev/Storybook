/*<summary>Error Methods</summary>*/
(function ()
{
    var scriptName = "Errors";
    
    function execute()
    {
        SEL.registerNamespace("SEL.Errors");
        
        SEL.Errors =
        {
            /* <summary>Private method for displaying an error</summary> */
            _showError: function (title, body)
            {
                if (body === "" || title === "")
                {
                    alert("Invalid error message format");
                }
                else
                {
                    alert(title + "\r\n\r\n" + body);
                }
            },
            /* <summary>If any required objects are missing then use this error message</summary> */
            MissingObjects: function (message)
            {
                this._showError("Missing Objects", message);
            },
            /* <summary>Any mandatory fields that are not set, show this message</summary> */
            MandatoryFieldsMissing: function (message)
            {
                this._showError("Required Fields Not Set", message);
            },
            /* <summary>Any invalid arguments, show this message</summary> */
            InvalidArgument: function (message)
            {
                this._showError("Invalid Argument", message);
            },
            /* <summary>Any invalid operations, show this message</summary> */
            InvalidOperation: function (message)
            {
                this._showError("Invalid Operation", message);
            },
            /* <summary>Reports a JavaScript error via our webservice.</summary> */
            ReportClientError: function (errorMessage, pageUrl, lineNumber)
            {
                try
                {
                    alert("JavaScript error..\nmessage: " + errorMessage + "\nurl: " + pageUrl + "\nline: " + lineNumber);
                    Spend_Management.svcErrors.ClientJavaScriptError(errorMessage, pageUrl, lineNumber);
                }
                catch (e)
                {
                }

                return false;
            }
        };
    }

    if (window.Sys && window.Sys.loader)
    {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else
    {
        execute();
    }
}());