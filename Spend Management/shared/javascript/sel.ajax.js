/* <summary>Reports Methods</summary> */
(function()
{
    var scriptName = "Axaj";

    function execute()
    {
        SEL.registerNamespace("SEL.Axaj");
        SEL.Ajax = {
            Service: function(path, method, params, sucessFunction, failFunction, async)
            {
                if (!path.endsWith('/'))
                {
                    path = path + '/';
                }
                if (async === undefined || async === null) {
                    async = true;
                }

                $.ajax({
                    async: async,
                    url: appPath + path + method,
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    data: JSON.stringify(params),
                    success: sucessFunction,
                    error: failFunction
                });
            },

            // Top tip: $("#control").datepicker("setDate", SEL.Ajax.ParseDate(date_from_server))
            ParseDate: function(aspAjaxDate)
            {
                var timestamp = aspAjaxDate.match(/^\/Date\((-?\d+)\)\/$/i);
                if (timestamp != null && timestamp.length > 1)
                {
                    return new Date(parseInt(timestamp[1]));
                }
                return null;
            }
        };
    }
if (window.Sys && Sys.loader) {
        Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
})();
