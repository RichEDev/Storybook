(function () {
    var scriptName = "CorporateCardLogs";

    function execute() {
        SEL.registerNamespace("SEL.CorporateCardLogs");

        SEL.CorporateCardLogs =
            {
                openModal: function (importDate, logMessage) {
                    $('#divViewLogsModal').dialog({
                        resizable: false,
                        modal: true,
                        width: 800,
                        title: "Corporate Card Log: " + importDate.substring(0, importDate.length - 3), //trimming off :ss off date string
                        value: logMessage,
                        dialogClass: "ui-no-close-button",
                        buttons: [{
                            text: "close",
                            "class": "jQueryUIButton",
                            click: function () {
                                $(this).dialog("close");
                            }
                        }],
                        open: function () {
                            $("#txtAreaViewLogs").val(logMessage);
                        }
                    });
                },

                getLogs: function(importId, importDate) {
                    var params = { "importId": importId };
                    SEL.Ajax.Service("/shared/webServices/svcCorporateCardsLogs.asmx",
                        "GetCardLogs",
                        params,
                        function(data) {
                            if (data.d.CardProviderId === -1) {
                                SEL.MasterPopup.ShowMasterPopup("An error has occured processng your request.");
                            } else {
                                SEL.CorporateCardLogs.openModal(importDate, data.d.LogMessage);
                            }
                        });
                },

                showViewLogsModal: function (importId, importDate) {
                    SEL.CorporateCardLogs.getLogs(importId, importDate);
                }
        }
    }

    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}());