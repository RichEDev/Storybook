/* <summary>Master Popup Modal Methods</summary> */
(function ()
{
    var scriptName = "MasterPopup";
    function execute()
    {
        SEL.registerNamespace("SEL.MasterPopup");
        SEL.MasterPopup =
        {
            // <summary> The master popup modal id </summary>
            ModalDOMID: null,

            /* <summary> Shows the Master Popup Modal for error messages </summary> */
            ShowMasterPopup: function (body, topic)
            {
                SEL.MasterPopup.SetPopupText(body, topic);
                SEL.Common.ShowModal(this.ModalDOMID);
                return;
            },

            //do not delete, use to set text if showing modal from server side
            SetPopupText: function (body, topic)
            {
                var popupDiv = document.getElementById("divMasterPopup");

                if (typeof topic !== "undefined" && topic !== null && topic !== "")
                {
                    popupDiv.innerHTML = "<a href=\"javascript:function() {return false};\" id=\"hrefMasterPopup\"></a><div class=\"errorModalSubject\">" + topic + "</div><br /><div class=\"errorModalBody\">" + body + "</div>";
                }
                else
                {
                    popupDiv.innerHTML = "<a href=\"javascript:function() {return false};\" id=\"hrefMasterPopup\"></a><div class=\"errorModalSubject\">Message from " + moduleNameHTML + "</div><br /><div class=\"errorModalBody\">" + body + "</div>";
                }
            },
            /* <summary> Hides the master popup modal </summary> */
            HideMasterPopup: function ()
            {
                SEL.Common.HideModal(this.ModalDOMID);
                var popupDiv = $g("divMasterPopup");
                popupDiv.innerHTML = "";
                return;
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