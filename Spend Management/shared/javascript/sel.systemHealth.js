/* <summary>System Health Methods</summary> */
(function (SEL)
{
    var scriptName = "SystemHealth";
    function execute()
    {
        SEL.registerNamespace("SEL.SystemHealth");
        SEL.SystemHealth =
        {
            MessageGenericError: "An error occurred while processing your request.",
            CurrentReport:null,
            ShowBasicHealthInfo: function()
            {
                $.ajax({
                    type: "POST",
                    url: window.appPath + "/shared/webServices/svcCache.asmx/NumberOfCacheEntries",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",                    
                    success: function(data)
                    {                        
                        var infoSpan = $('<span></span>').addClass('healthInfoNode').text('Number of objects in cache: ' + data.d);

                        $('#generalInformation').append(infoSpan);
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown)
                    {
                        SEL.Common.WebService.ErrorHandler(errorThrown);
                    }
                });

                $('#cacheKeys').on('click', '.healthInfoNode', function ()
                {
                    var cacheKey = $(this).text().replace('Cachekey: ', '');

                    $.ajax({
                        type: 'POST',
                        url: window.appPath + '/shared/webServices/svcCache.asmx/CacheItems',
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({ 'cacheKey': cacheKey }),
                        success: function (data)
                        {
                            data = data.d;

                            var cacheEntryKey = '';

                            $('#dialogInfo').html('');

                            $(data).each(function (x, cacheEntry)
                            {
                                cacheEntryKey = cacheEntry.Key;

                                var valueType = $('<div>').text('Value Type: ').append($('<div>').text(cacheEntry.ValueType));

                                var valueInfo = $('<div>').text('Value: ');

                                $(cacheEntry.Value).each(function(i, entry)
                                {
                                    valueInfo.append($('<div>').text(entry));
                                });                                                             

                                $('#dialogInfo').append(valueType).append(valueInfo);
                            });

                            $('#healthDialog').dialog({
                                height: 'auto',
                                width: 'auto',
                                position: { my: "center", at: "center", of: window },
                                title: cacheEntryKey,
                                buttons: [
                                    {
                                        text: 'Close',
                                        class: "jQueryUIButton",
                                        click: function() {
                                             $(this).dialog('close');
                                        }
                                    }]
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown)
                        {
                            SEL.Common.WebService.ErrorHandler(errorThrown);
                        }
                    });
                });
                
                $('#distributedCache').on('click', '.healthInfoNode', function() {
                    var cacheKey = $(this).text().replace('Cachekey: ', '');
                    if (confirm("Clear cache entry?"))
                    {
                        $.ajax({
                            type: 'POST',
                            url: window.appPath + '/shared/webServices/svcCache.asmx/ClearDistributedCacheItem',
                            dataType: 'json',
                            contentType: 'application/json; charset=utf-8',
                            data: JSON.stringify({ 'cacheKey': cacheKey }),
                            success: function(data)
                            {
                                SEL.SystemHealth.ShowDistributedCacheInfo();
                            },
                            error: function(XMLHttpRequest, textStatus, errorThrown)
                            {
                                SEL.Common.WebService.ErrorHandler(errorThrown);
                            }
                        });
                }
                });

                SEL.SystemHealth.SetupShowHideButton();
                SEL.SystemHealth.SetupShowEventLogInfo();
            },

            ShowAccountCacheInfo: function ()
            {
                $("#accountCacheKeys").html($("<span></span>").addClass("healthInfoNode").html($g("bowlG").cloneNode(true)));

                $.ajax({
                    type: "POST",
                    url: window.appPath + "/shared/webServices/svcCache.asmx/AccountCacheKeys",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (data)
                    {
                        var infoContainer = $('#accountCacheKeys');

                        if (typeof data === "undefined" || data === null || !data.hasOwnProperty("d"))
                        {
                            infoContainer.html($("<span></span>").addClass("healthInfoNode").text("No information was returned from the log reader service."));
                            return;
                        }

                        if (data.d.length === 0)
                        {
                            infoContainer.html($("<span></span>").addClass("healthInfoNode").text("There are no event log entries."));
                            return;
                        }

                        data = data.d;

                        infoContainer.find("*").remove();

                        $(data).each(function (x, dataInfo)
                        {
                            var infoSpan = $('<span></span>').addClass('healthInfoNode').text('Account Identity: ' + dataInfo);

                            infoContainer.append(infoSpan);
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown)
                    {
                        SEL.Common.WebService.ErrorHandler(errorThrown);
                    }
                });

            },

            UpdateAccountCacheInfo: function ()
            {
                if (confirm("Update accounts cache entries?"))
                {
                    $.ajax({
                        type: 'POST',
                        url: window.appPath + '/shared/webServices/svcCache.asmx/ResetAccountCache',
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8',
                        success: function (data)
                        {
                            if (data.d === true)
                            {
                                SEL.MasterPopup.ShowMasterPopup("Account Cache is now consistant with the registeredusers database entries.");
                            }
                            else
                            {
                                SEL.MasterPopup.ShowMasterPopup("Account Cache does not seem to be consistant with the registeredusers database entries.");
                            }

                            SEL.SystemHealth.ShowAccountCacheInfo();
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown)
                        {
                            SEL.Common.WebService.ErrorHandler(errorThrown);
                        }
                    });
                }
            },
            
            ShowDistributedCacheInfo: function () {
                
                $("#distributedCache").html($("<span></span>").addClass("healthInfoNode").html($g("bowlG").cloneNode(true)));

                $.ajax({
                    type: "POST",
                    url: window.appPath + "/shared/webServices/svcCache.asmx/DistributedCacheKeys",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        data = data.d;

                        var infoContainer = $('#distributedCache');

                        infoContainer.find("*").remove();

                        $(data).each(function (x, dataInfo) {
                            var infoSpan = $('<span></span>').addClass('healthInfoNode').text('Cachekey: ' + dataInfo);

                            infoContainer.append(infoSpan);
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        SEL.Common.WebService.ErrorHandler(errorThrown);
                    }
                });
                
            },
            ClearDistributedCache: function()
            {
                $("#distributedCache").html($("<span></span>").addClass("healthInfoNode").html($g("bowlG").cloneNode(true)));
                if (confirm("Clear all cache entries for the current account?"))
                {
                    $.ajax({
                        type: "POST",
                        url: window.appPath + "/shared/webServices/svcCache.asmx/ClearDistributedCacheItems",
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        success: function (data)
                        {
                            if (data.d !== true)
                            {
                               SEL.MasterPopup.ShowMasterPopup("Operation completed but not all keys were removed from the cache", "Message from Expenses");
                            }
                            var infoContainer = $('#distributedCache');
                            infoContainer.find("*").remove();
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown)
                        {
                            SEL.Common.WebService.ErrorHandler(errorThrown);
                        }
                    });
                }
            },

            ShowDistributedCacheStatisticsInfo: function () {

                $("#distributedCacheStatistics").html($("<span></span>").addClass("healthInfoNode").html($g("bowlG").cloneNode(true)));

                $.ajax({
                    type: "POST",
                    url: window.appPath + "/shared/webServices/svcCache.asmx/DistributedCacheStatistics",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        data = data.d;

                        var infoContainer = $('#distributedCacheStatistics');

                        infoContainer.find("*").remove();
                        $('#dialogInfo').html('');
                        $(data).each(function (x, dataInfo) {
                            var infoSpan = $('<span></span>').addClass('healthInfoNode').text(dataInfo);

                            //infoContainer.append(infoSpan);
                            
                            $('#dialogInfo').append($('<div>').text(dataInfo));
                            

                        });
                        
                        $('#healthDialog').dialog({
                            height: 'auto',
                            width: 'auto',
                            position: { my: "center", at: "center", of: window },
                            title: 'Distributed Cache Statistics',
                            buttons: [
                                {
                                    text: 'Close',
                                    "class": "jQueryUIButton",
                                    click: function() {
                                         $(this).dialog('close');
                                    }
                                }]
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        SEL.Common.WebService.ErrorHandler(errorThrown);
                    }
                });

            },

            SetupShowHideButton: function()
            {
                var iconPath = $('#iconPath').text();

                $('#healthPage .btn').one('click', function()
                {
                    $(this).next('.showHideButton').fadeIn(300);
                });

                // Note that 'navigate_open' is an UPWARDS arrow. IconExperience logic.
                $('#healthPage .showHideButton').attr('src', iconPath + 'navigate_open.png').click(function()
                {
                    var button = $(this);

                    if (button.attr('src') === iconPath + 'navigate_open.png')
                    {
                        button.nextAll('div:first').slideUp(400, function ()
                        {
                            button.attr('src', iconPath + 'navigate_close.png');
                        });
                    }
                    else
                    {
                        button.nextAll('div:first').slideDown(400, function()
                        {
                            button.attr('src', iconPath + 'navigate_open.png');
                        });
                    }
                });
            },

            ShowExtendedHealthInfo: function()
            {
                $("#cacheKeys").html($("<span></span>").addClass("healthInfoNode").html($g("bowlG").cloneNode(true)));
                
                $.ajax({
                    type: "POST",
                    url: window.appPath + "/shared/webServices/svcCache.asmx/CacheKeys",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function(data)
                    {
                        data = data.d;

                        var infoContainer = $('#cacheKeys');

                        infoContainer.find("*").remove();

                        $(data).each(function(x, dataInfo)
                        {
                            var infoSpan = $('<span></span>').addClass('healthInfoNode').text('Cachekey: ' + dataInfo);

                            infoContainer.append(infoSpan);
                        });                        
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown)
                    {
                        SEL.Common.WebService.ErrorHandler(errorThrown);
                    }
                });
            },

            ShowRunningReportsInfo: function ()
            {
                $("#runningReportRequests").html($("<span></span>").addClass("healthInfoNode").html($g("bowlG").cloneNode(true)));
                $("#runningReportThreads").html($("<span></span>").addClass("healthInfoNode").html($g("bowlG").cloneNode(true)));
                
                $.ajax({
                    type: "POST",
                    url: window.appPath + "/shared/webServices/svcSystemHealth.asmx/GetRunningReportsInformation",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (data)
                    {
                        if (typeof data === "undefined" || data === null || !data.hasOwnProperty("d"))
                        {
                            $("#runningReportRequests").html($("<span></span>").addClass("healthInfoNode").text("No information was returned from the reports service."));
                            return;
                        }
                        
                        if (data.d.length === 0)
                        {
                            $("#runningReportRequests").html($("<span></span>").addClass("healthInfoNode").text("There are no report requests being processed."));
                            return;
                        }
                        
                        $("#runningReportRequests").html("");
                        
                        var statuses = ["BeingProcessed", "Complete", "Failed", "Queued"];
                        var reportTypes = ["", "Item", "Summary", "None"];
                        var exportTypes = ["", "Viewer", "Excel", "CSV", "FlatFile", "Pivot"];
                        var runFroms = ["PrimaryServer", "ReportsServer"];
                        
                        for (var i = 0; i < data.d.length; i++)
                        {
                            if (data.d[i] === null || !data.d[i].hasOwnProperty("Status"))
                            {
                                continue;
                            }

                            var r = data.d[i];
                            
                            var infoSpan = $("<span></span>")
                                            .addClass("healthInfoNode")
                                            .html(
                                                "Status: " + r.Status + " (<span class=\"infonode-highlightproperty\">" + statuses[r.Status] + "</span>)"
                                                    + "<br/>AccountId: " + r.AccountId
                                                    + "<br/>SubAccountId: " + r.SubAccountId
                                                    + "<br/>Report Name: " + r.ReportName
                                                    + "<br/>Report Type: " + r.ReportType + " (<span class=\"infonode-highlightproperty\">" + reportTypes[r.ReportType] + "</span>)"
                                                    + "<br/>Export Type: " + r.ExportType + " (<span class=\"infonode-highlightproperty\">" + exportTypes[r.ExportType] + "</span>)"
                                                    + "<br/>Scheduler Request: " + r.SchedulerRequest
                                                    + "<br/>Processed Rows: " + r.ProcessedRows
                                                    + "<br/>Max Row Limit: " + r.ReportMaxRowLimit
                                                    + "<br/>Row Count: " + r.RowCount
                                                    + "<br/>Completion Time: " + new Date(parseInt(r.CompletionTime.replace('/Date(', ''), 10))
                                                    + "<br/>Report Run From: " + r.ReportRunFrom + " (<span class=\"infonode-highlightproperty\">" + runFroms[r.ReportRunFrom] + "</span>)"
                                                    + "<br/>Report Static Sql: " + r.ReportStaticSql
                                                    + "<br/>Report Join Sql: " + r.ReportJoinSql
                                                    + "<br/>");

                            $("#runningReportRequests").append(infoSpan);
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown)
                    {
                        SEL.Common.WebService.ErrorHandler(errorThrown);
                    }
                });
                

                $.ajax({
                    type: "POST",
                    url: window.appPath + "/shared/webServices/svcSystemHealth.asmx/GetRunningThreadsInformation",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (data)
                    {
                        if (typeof data === "undefined" || data === null || !data.hasOwnProperty("d"))
                        {
                            $("#runningReportThreads").html($("<span></span>").addClass("healthInfoNode").text("No information was returned from the reports service."));
                            return;
                        }

                        if (data.d.length === 0)
                        {
                            $("#runningReportThreads").html($("<span></span>").addClass("healthInfoNode").text("There are no threads processing reports."));
                            return;
                        }

                        $("#runningReportThreads").html("");

                        var states = { "0": "Running", "1": "StopRequested", "2": "SuspendRequested", "4": "Background", "8": "Unstarted", "16": "Stopped", "32": "WaitSleepJoin", "64": "Suspended", "128": "AbortRequested", "256": "Aborted" };
                        var priorities = ["Lowest", "BelowNormal", "Normal", "AboveNormal", "Highest"];
                        
                        for (var i = 0; i < data.d.length; i++)
                        {
                            if (data.d[i] === null || !data.d[i].hasOwnProperty("ThreadKey"))
                            {
                                continue;
                            }

                            var t = data.d[i];
                            
                            var infoSpan = $("<span></span>")
                                            .addClass("healthInfoNode")
                                            .html(
                                                "Key: " + t.ThreadKey
                                                    + "<br/>Name: " + t.Name
                                                    + "<br/>State: " + t.State + " (<span class=\"infonode-highlightproperty\">" + states[t.State.toString(10)] + "</span>)"
                                                    + "<br/>Priority: " + t.Priority + " (<span class=\"infonode-highlightproperty\">" + priorities[t.Priority] + "</span>)"
                                                    + "<br/>Managed Id: " + t.ManagedId
                                                    + "<br/>");
                            
                            $("#runningReportThreads").append(infoSpan);
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown)
                    {
                        SEL.Common.WebService.ErrorHandler(errorThrown);
                    }
                });
            },
            
            ShowReportsEventLogInfo: function ()
            {
                $("#reportsEventLogs").html($("<span></span>").addClass("healthInfoNode").html($g("bowlG").cloneNode(true)));

                $.ajax({
                    type: "POST",
                    url: window.appPath + "/shared/webServices/svcSystemHealth.asmx/GetEventLogEntries",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    data: "{ \"machineName\": \".\", \"logType\": 2 }",
                    success: function (data)
                    {
                        if (typeof data === "undefined" || data === null || !data.hasOwnProperty("d"))
                        {
                            $("#reportsEventLogs").html($("<span></span>").addClass("healthInfoNode").text("No information was returned from the log reader service."));
                            return;
                        }

                        if (data.d.length === 0)
                        {
                            $("#reportsEventLogs").html($("<span></span>").addClass("healthInfoNode").text("There are no event log entries."));
                            return;
                        }

                        $("#reportsEventLogs").html("");
                        
                        var entryTypes = { "1": "Error", "2": "Warning", "4": "Information", "8": "SuccessAudit", "16": "FailureAudit" };

                        for (var i = 0; i < data.d.length; i++)
                        {
                            if (data.d[i] === null || !data.d[i].hasOwnProperty("EntryType"))
                            {
                                continue;
                            }

                            var e = data.d[i];

                            var infoSpan = $("<span></span>")
                                            .addClass("healthInfoNode")
                                            .html(
                                                "Index: " + e.Index
                                                    + "<br/>Entry Type: " + e.EntryType + " (<span class=\"infonode-highlightproperty\">" + entryTypes[e.EntryType.toString(10)] + "</span>)"
                                                    + "<br/>Source: " + e.Source
                                                    + "<br/>Message: " + e.Message
                                                    + "<br/>Time Generated: " + new Date(parseInt(e.TimeGenerated.replace('/Date(', ''), 10))
                                                    + "<br/>");

                            $("#reportsEventLogs").append(infoSpan);
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown)
                    {
                        SEL.Common.WebService.ErrorHandler(errorThrown);
                    }
                });
            },

            SetupShowEventLogInfo: function ()
            {
                $(".healthpage-eventloader").click(function ()
                {
                    var displayDiv = $(this).nextAll("div.healthpage-events:first");
                    if (!displayDiv)
                    {
                        return;
                    }

                    displayDiv.html($("<span></span>").addClass("healthInfoNode").html($g("bowlG").cloneNode(true)));
                    
                    var applicationType = $(this).data("eventlog");
                    if (typeof applicationType !== "number" || applicationType < 0 || applicationType > 8)
                    {
                        applicationType = 0;
                    }

                    var machineName = $(this).nextAll("input.healthpage-input:first").val();
                    var numberOfEvents = $(this).nextAll("input.healthpage-input").eq(1).val();

                    if (typeof machineName !== "string" || machineName === "")
                    {
                        machineName = ".";
                    }

                    numberOfEvents = isNaN(parseInt(numberOfEvents, 10)) ? 10 : parseInt(numberOfEvents, 10);
                    numberOfEvents = numberOfEvents === 0 ? 10 : numberOfEvents;

                    $.ajax({
                        type: "POST",
                        url: window.appPath + "/shared/webServices/svcSystemHealth.asmx/GetEventLogEntries",
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        data: "{ \"machineName\": \"" + machineName + "\", \"logType\": " + applicationType + ", \"numberOfEvents\": " + numberOfEvents + " }",
                        success: function(data)
                        {
                            if (typeof data === "undefined" || data === null || !data.hasOwnProperty("d"))
                            {
                                displayDiv.html($("<span></span>").addClass("healthInfoNode").text("No information was returned from the log reader service."));
                                return;
                            }

                            if (data.d.length === 0)
                            {
                                displayDiv.html($("<span></span>").addClass("healthInfoNode").text("There are no event log entries."));
                                return;
                            }

                            displayDiv.html("");

                            var entryTypes = { "1": "Error", "2": "Warning", "4": "Information", "8": "SuccessAudit", "16": "FailureAudit" };

                            for (var i = 0; i < data.d.length; i++)
                            {
                                if (data.d[i] === null || !data.d[i].hasOwnProperty("EntryType"))
                                {
                                    continue;
                                }

                                var e = data.d[i];

                                var infoSpan = $("<span></span>")
                                    .addClass("healthInfoNode")
                                    .html(
                                        "Index: " + e.Index
                                            + "<br/>Entry Type: " + e.EntryType + " (<span class=\"infonode-highlightproperty\">" + entryTypes[e.EntryType.toString(10)] + "</span>)"
                                            + "<br/>Source: " + e.Source
                                            + "<br/>Message: " + e.Message
                                            + "<br/>Time Generated: " + new Date(parseInt(e.TimeGenerated.replace('/Date(', ''), 10))
                                            + "<br/>");

                                infoSpan.appendTo(displayDiv).slideDown(300);
                            }
                        },
                        error: function(XMLHttpRequest, textStatus, errorThrown)
                        {
                            SEL.Common.WebService.ErrorHandler(errorThrown);
                        }
                    });
                });
            },
            TestFinancialReports: function() {
                $("#financialReportsResults").html($("<span></span>").addClass("healthInfoNode").html($g("bowlG").cloneNode(true)));

                $.ajax({
                    type: "POST",
                    url: window.appPath + "/shared/webServices/svcSystemHealth.asmx/GetAccountsWithFinancialExports",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    data: "",
                    success: function (data) {
                        if (typeof data === "undefined" || data === null || !data.hasOwnProperty("d")) {
                            $("#financialReportsResults").html($("<span></span>").addClass("healthInfoNode").text("No information was returned from the log reader service."));
                            return;
                        }

                        if (data.d.length === 0) {
                            $("#financialReportsResults").html($("<span></span>").addClass("healthInfoNode").text("There are no event log entries."));
                            return;
                        }

                        $("#financialReportsResults").html("");

                        for (var i = 0; i < data.d.length; i++) {
                            var e = data.d[i];
                            var infoSpan = '<div>' + e + '</div>';

                            $("#financialReportsResults").append(infoSpan);
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        SEL.Common.WebService.ErrorHandler(errorThrown);
                    }
                });
            },
            TestFinancialExports: function(accountId) {
                $("#fedetail" + accountId).html($("<span></span>").addClass("healthInfoNode").html($g("bowlG").cloneNode(true)));

                $.ajax({
                    type: "POST",
                    url: window.appPath + "/shared/webServices/svcSystemHealth.asmx/TestFinancialExports",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    data: "{accountId: "+accountId+"}",
                    success: function (data) {
                        if (typeof data === "undefined" || data === null || !data.hasOwnProperty("d")) {
                            $("#fedetail" + accountId).html($("<span></span>").addClass("healthInfoNode").text("No information was returned from the log reader service."));
                            return;
                        }

                        if (data.d.length === 0) {
                            $("#fedetail" + accountId).html($("<span></span>").addClass("healthInfoNode").text("There are no event log entries."));
                            return;
                        }

                        $("#fedetail" + accountId).html("");

                        for (var i = 0; i < data.d.length; i++) {
                            var e = data.d[i];
                            var infoSpan = '<div>' + e + '</div>';

                            $("#fedetail" + accountId).append(infoSpan);
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        SEL.Common.WebService.ErrorHandler(errorThrown);
                    }
                });
            },
            TestReports: function() {
                $("#reportsResults").html($("<span></span>").addClass("healthInfoNode").html($g("bowlG").cloneNode(true)));
                $.ajax({
                    type: "POST",
                    url: window.appPath + "/shared/webServices/svcSystemHealth.asmx/TestReports",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (data)
                    {
                        if (typeof data === "undefined" || data === null || !data.hasOwnProperty("d")) {
                            $("#reportsResults").html($("<span></span>").addClass("healthInfoNode").text("No information was returned from the log reader service."));
                            return;
                        }

                        if (data.d.length === 0) {
                            $("#reportsResults").html($("<span></span>").addClass("healthInfoNode").text("There are no event log entries."));
                            return;
                        }

                        $("#reportsResults").html("");

                        for (var i = 0; i < data.d.length; i++) {
                            var e = data.d[i];
                            var infoSpan = '<div id="'+e.Id+'" class="pendingReport">' + e.Name + '</div>';

                            $("#reportsResults").append(infoSpan);
                        }

                        SEL.SystemHealth.RunReport();

                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown)
                    {
                        SEL.Common.WebService.ErrorHandler(errorThrown);
                    }
                });
            },
            RunReport: function() {
                var pending = $('.pendingReport').first();
                $(pending).stop().css("background-color", "#19a2e6").animate({ backgroundColor: "#FFFFFF" }, 1500).removeClass('pendingReport');
                if (pending.length > 0) {
                    SEL.SystemHealth.CurrentReport = pending.attr('id');
                    $.ajax({
                        type: "POST",
                        url: window.appPath + "/shared/webServices/svcSystemHealth.asmx/TestReport",
                        dataType: "json",
                        data: "{reportId: '" + SEL.SystemHealth.CurrentReport + "'}",
                        contentType: "application/json; charset=utf-8",
                        success: function (data)
                        {
                            for (var i = 0; i < data.d.length; i++) {
                                var e = data.d[i];
                                var infoSpan = " => " +e;

                                $('#' + SEL.SystemHealth.CurrentReport).append(infoSpan);
                                
                            }
                            
                            $(document).scrollTop($('#' + SEL.SystemHealth.CurrentReport).position().top - 250);
                            SEL.SystemHealth.CurrentReport = '';
                            SEL.SystemHealth.RunReport();
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown)
                        {
                            SEL.Common.WebService.ErrorHandler(errorThrown);
                            $('#' + SEL.SystemHealth.CurrentReport).removeClass('pendingReport');
                            $(document).scrollTop($('#' + SEL.SystemHealth.CurrentReport).position().top - 250);
                            SEL.SystemHealth.CurrentReport = '';
                            SEL.SystemHealth.RunReport();
                        }
                    });





                    
                }
            }
        };
    }

    $(document).ready(function()
    {
        SEL.SystemHealth.ShowBasicHealthInfo();
    });

    if (window.Sys && window.Sys.loader)
    {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else
    {
        execute();
    }
})(SEL);
