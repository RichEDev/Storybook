/*global SEL:false, $:false */
/*
* Data
*/
(function (SEL, $) {
    var scriptName = "Data";
    function execute() {
        SEL.registerNamespace("SEL.Data");
        SEL.getServiceUrl = function(options) {

        };
        SEL.Data = {
            /*
            * Aborts a pending jQuery Ajax request
            * 
            * The xmlHttpRequest object is the return value of SEL.Data.Ajax() / $.ajax()
            *
            * var xhr = SEL.Data.Ajax(stuff...);
            * SEL.Data.AbortAjax(xhr);
            */
            AbortAjax: function (xmlHttpRequest) {
                if (xmlHttpRequest && xmlHttpRequest.readyState != 4) {
                    xmlHttpRequest.abort();
                }
            },

            GetServiceUrl: function(options) {
                return "/shared/webServices/" + options.serviceName + ".asmx/" + options.methodName;
            },
            

            /*
            * Common Ajax function for communicating with SEL web services.
            * 
            * Usage is the same as Jquery's $.ajax(), any of the options can be used. 
            * The options "type", "dataType", "contentType" and "error" are already defined for 
            * communicating with a SEL webservice so can be omitted.
            * 
            * There are two new options; "serviceName" and "methodName", if both are used then the "url" option can be omitted.
            * 
            * The "data" option can be an object or a pre-stringified object.
            */
            Ajax: function (options) {

                // if options "serviceName" and "methodName" are defined then build the "url" option from their values
                if (options.serviceName && options.methodName) {
                    $.extend(options, { url: this.GetServiceUrl(options) });
                }

                // convert the data to a string if necessary
                if ($.type(options.data) === 'object') {
                    options.data = JSON.stringify(options.data);
                }

                // call jQuery $.ajax, combining the defaults with any options that were passed in
                var ajaxOptions = $.extend({
                    type: 'POST',
                    dataType: 'json',
                    contentType: "application/json; charset=utf-8",
                    error: function (xmlHttpRequest, textStatus, errorThrown) {
                        // error handling for expired sessions
                        if (xmlHttpRequest.status === 401)
                        {
                            SEL.MasterPopup.ShowMasterPopup("You have been automatically logged out due to a period of inactivity, you must <a href='javascript:location.reload()'>login again</a> before continuing.", "Session Expired");
                        }
                        else if (textStatus != "abort")
                        {
                            SEL.Common.WebService.ErrorHandler(errorThrown);
                        }
                    }
                }, options);
                return $.ajax(ajaxOptions);
            }
        };
    }

    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}
)(SEL, $);