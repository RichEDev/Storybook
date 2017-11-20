(function () {
    var scriptName = "CorporateCardProviders";

    function execute() {
        SEL.registerNamespace("SEL.CorporateCardProviders");

        SEL.CorporateCardProviders =
        {
            DeleteCard: function(cardid)
            {
                if (confirm("Are you sure you wish to delete the selected corporate card?")) {
                    var params = { "cardid": cardid }
                    SEL.Ajax.Service("/shared/webServices/svcCorporateCardsProviders.asmx", "DeleteCard", params,
                        function (data) {
                            var $response = data.d;
                            if ($response) {
                                SEL.Grid.refreshGrid("gridCorporateCardPorviders", SEL.Grid.getCurrentPageNum("gridCorporateCardPorviders"));
                            }
                            else {
                                SEL.MasterPopup.ShowMasterPopup(
                                    "The selected card cannot be deleted as one or more statements of this type have already been imported.");
                            }
                        });
                }
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