(function ()
{
    var scriptName = "JsonParse";
    
    function execute()
    {
        SEL.registerNamespace("SEL.JsonParse");

        SEL.jsonParse = {
            parse: function(data)
            {
                if (typeof data !== "string" || !data)
                {
                    return null;
                }

                if (/^[\],:{}\s]*$/.test(data.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, "@")
                    .replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, "]")
                    .replace(/(?:^|:|,)(?:\s*\[)+/g, "")))
                {
                    return window.JSON && window.JSON.parse ?
                        window.JSON.parse(data) :
                        (new Function("return " + data))();
                }
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