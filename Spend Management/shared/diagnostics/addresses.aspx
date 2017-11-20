<%@ Page Language="C#" %>
<%@ Import namespace="Spend_Management" %>
<%@ Import Namespace="SpendManagementLibrary" %>
<!DOCTYPE html>
<html>
	<head runat="server">
	    <title>Address Search Diagnostics Page</title>
    
        <script src="/static/js/jQuery/jquery-1.9.0.min.js"></script>
        <script src="/static/js/json2.min.js"></script>
        <script src="/shared/javaScript/minify/sel.main.js"></script>
        <script src="/shared/javaScript/minify/sel.common.js"></script>
        <script src="/shared/javaScript/minify/sel.data.js"></script>
        
        <style>
            #rawLog, #log, label { padding: 4px; }
        </style>

	</head>
	<body>
		<h1>Address Search Diagnostics Page</h1>
		
        <div id="main" style="width: 50%; float: left;">
            <div>
                <label>Search term</label><input type="text" name="searchTerm" value="LN63JY" />
                <input type="button" name="search" value="Go"/>
            </div>
        
            <div id="log"></div>
            
            <div>
                <input type="button" name="send" value="Send results to Sofware Europe"/>
                <span></span>
            </div>
        </div>
        
        <div id="rawLog" style="width: 49%; float: left; border-left: 1px solid #000;"></div>
        
        <script>

            var pcaKey = "<%=new cAccounts().GetAccountByID(cMisc.GetCurrentUser().AccountID).PostcodeAnywhereKey%>";

            $(document).ready(function() {

                var log = function(message) {
                    $("#log").html($("#log").html() + new Date().toTimeString().replace(/.*(\d{2}:\d{2}:\d{2}).*/, "$1") + " - " + message + "<br />");
                };

                var logRaw = function() {
                    $("#rawLog").html($("#rawLog").html() + JSON.stringify(arguments) + "<br /><br />");
                };

                $("input[type=button][name=search]").on("click", function() {

                    $("#log, #rawLog").html("");

                    var searchTerm = $("input[name=searchTerm]").val();

                    if (searchTerm.length) {

                        log("Searching \"" + searchTerm + "\"...");
                        $.ajax({
                            url: "//services.postcodeanywhere.co.uk/CapturePlus/Interactive/AutoComplete/v2.00/json3.ws?",
                            data: {
                                Key: pcaKey,
                                Country: "GBR",
                                searchTerm: searchTerm
                            },
                            dataType: "jsonp",
                            crossdomain: true,
                            timeout: 20000,
                            success: function(data, status, pcaResponse) {
                                logRaw(data, status, pcaResponse);
                                log(data.Items.length + " results retrieved.");

                                if (data.Items.length) {

                                    if (data.Items[0].IsRetrievable) {

                                        var item = data.Items[0];
                                        log("Automatically looking up first result (" + item.Id + ")...");

                                        SEL.Data.Ajax({
                                            serviceName: "svcAddresses",
                                            methodName: "GetByCapturePlusId",
                                            timeout: 20000,
                                            data: {
                                                capturePlusId: item.Id,
                                                labelId: 0
                                            },
                                            success: function(data, status, selResponse) {

                                                logRaw(data, status, selResponse);

                                                if ("d" in data) {
                                                    log("Successfully retrieved \"" + data.d.FriendlyName + "\"");
                                                }
                                            },
                                            error: function(xhr, status, error) {

                                                logRaw(xhr);
                                                log("Lookup failed (" + JSON.stringify(error) + ")");
                                            }
                                        });

                                    } else {

                                        log("First result wasn't a retrievable address, please use a more specific search term.");
                                    }
                                }
                            },
                            error: function(xhr, status, error) {

                                logRaw(xhr, status, error);
                                log("Search failed.");
                            }
                        });
                    }
                });

                $("input[type=button][name=send]").on("click", function () {
                    var confirmationPanel = $(this).siblings("span");
                    var message = $("#log").html() + $("#rawLog").html();

                    if (message.length) {
                        SEL.Data.Ajax({
                            serviceName: "svcErrors",
                            methodName: "ClientJavaScriptError",
                            data: {
                                errorMessage: message,
                                pageUrl: document.location.href,
                                lineNumber: 0
                            },
                            success: function () {
                                confirmationPanel.html("Information sent");
                            },
                            error: function () {
                                confirmationPanel.html("Failed to send");
                            }
                        });
                    }
                    
                });
            });
        
        </script>
	</body>
</html>