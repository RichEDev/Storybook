(function() {
    var scriptName = "PublicApi";
    function execute() {
        SEL.registerNamespace("SEL.PublicApi");

        SEL.PublicApi =
        {
            Call: function (type, call, data, onSuccess, onError) {
                var url = publicApiUrl + "/" + call;
                var token = "Bearer " + publicApiToken;
                
                $.ajax({
                    type: type,
                    url: url,
                    data: JSON.stringify(data),
                    headers: { "Authorization": token },
                    success: onSuccess,
                    error: onError
                });
            },
            Url: null
        }
    }
    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}());