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
            },  
            ShowMasterConfirm: function (body, topic, onConfirmFunction, onCancelFunction) {
                $('#divMasterPopup').html("<div style='float:left;margin-top:14px;'><img title='Question' src='/static/icons/48/new/question.png' ></div><div class=\"errorModalBody\" style='width:75%;float:right;margin-top:10px;'>" + body + "</div>");
                if ($('#btnMasterPopupConfirm').length === 0) {
                    $('#divMasterPopup').append('<div class="formpanel formbuttons " style="position:relative;left:-19px;height:30px;z-index:0;float:left;padding-top:10px;padding-bottom:0px;"><span class="buttonContainer"><input type="button" id="btnMasterPopupConfirm" value="yes" class="buttonInner" style="padding-bottom:0;padding-top:0;"/></span><span class="buttonContainer"><input type="button" value="no"  id="btnMasterPopupDecline" class="buttonInner" style="padding-bottom:0;padding-top:0;"/></span></div>');
                }

                $('#btnMasterPopupConfirm').click(function() {
                    onConfirmFunction();
                    $('#divMasterPopup').dialog('close');
                });

                $('#btnMasterPopupDeny').click(function() {
                    onCancelFunction();
                    $('#divMasterPopup').dialog('close');
                });



                $('#divMasterPopup').dialog(
                    {
                        title: topic,
                        autoOpen: true,
                        resizable: false,
                        modal: true,
                        close: function() {
                            $('#btnMasterPopupConfirm').remove();
                            $('#btnMasterPopupDeny').remove();
                            $('divMasterPopup').dialog('destroy');
                        },
                        open: function() {
                            var zIndex = SEL.Common.GetHighestZIndex();
                            $('.ui-widget-overlay:last').css('zIndex', zIndex + 1);
                            $('#divMasterPopup').parent().css('zIndex', zIndex + 2);
                        }
                    });

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