/* <summary>Common Methods</summary> */
(function ()
{
    var scriptName = "Common";
    function execute()
        {
        SEL.registerNamespace("SEL.Common");
        
        SEL.Common = {
            MessageGenericError: "An error occurred while processing your request.",

            /* <summary>Returns the highest visible z-index found on all divs on the current document (different frames/iframes are different documents)</summary> */
            GetHighestZIndex: function ()
            {
                var documentDivs = document.getElementsByTagName("DIV"),
                    highestZIndex = 0,
                    i;
                
                for (i = 0; i < documentDivs.length; i++)
                {
                    if (documentDivs[i].style.display === "")
                    {
                        if (documentDivs[i].style.zIndex > highestZIndex)
                        {
                            highestZIndex = documentDivs[i].style.zIndex;
                        }
                    }
                }
                
                return highestZIndex;
            },
            /* <summary>Returns the highest visible z-index found on all divs on the current document (different frames/iframes are different documents) and returns it as an int</summary> */
            GetHighestZIndexInt: function ()
            {
                var documentDivs = document.getElementsByTagName("DIV"),
                    highestZIndex = 0,
                    i,
                    currentZIndex;
                
                for (i = 0; i < documentDivs.length; i++)
                {
                    if (documentDivs[i].style.display !== "none")
                    {
                        currentZIndex = parseInt(documentDivs[i].style.zIndex, 10);
                        highestZIndex = (currentZIndex > highestZIndex) ? currentZIndex : highestZIndex;
                    }
                }
                
                return highestZIndex;
            },
            /* <summary>Returns the highest visible z-index found on all divs on the current document (different frames/iframes are different documents) and returns it as an int</summary>*/
            // todo: testify the stuff out of this
            GetHighestZIndex: function ()
            {
                var highestZIndex = 0;
                $("div").filter(":visible").each(function(i, o) { highestZIndex = (parseInt(o.style.zIndex, 10) > highestZIndex) ? parseInt(o.style.zIndex, 10) : highestZIndex; });
                return highestZIndex;
            },
            /* <summary>Shows a ModalPopupExtender</summary> */
            ShowModal: function (modalPopupID) {
                $f(modalPopupID).show();
                $f(modalPopupID)._backgroundElement.style.zIndex = this.GetHighestZIndexInt() + 1;
                $f(modalPopupID)._popupElement.style.zIndex = this.GetHighestZIndexInt() + 2;
            },
            /* <summary>Hides a ModalPopupExtender</summary> */
            HideModal: function (modalPopupId)
            {
                $f(modalPopupId).hide();
            },
            /* <summary>Shows a PopupControlExtender and positions it on the focusOnControl</summary>*/
            ShowPopup: function (popupId, focusOnControl)
            {
                $f(popupId)._popupBehavior._parentElement = focusOnControl;
                $f(popupId).showPopup();
                $f(popupId)._popupElement.style.zIndex = this.GetHighestZIndex() + 1;
            },
            /* <summary>Hides a PopupControlExtender</summary> */
            HidePopup: function (popupId)
            {
                $f(popupId).hidePopup();
            },
            /* <summary>Stops the default propagation (IE)/bubble (W3C)</summary> */
            stopPropagation: function (evt)
            {
                var e = (evt) ? evt : window.event;
                if (window.event)
                {
                    e.cancelBubble = true;
                    e.returnValue = false;
                }
                else
                {
                    e.stopPropagation();
                    e.preventDefault();
                }
                return true;
            },
            /* <summary>Change the document's favicon</summary> */
            SetFavicon: function (url)
            {
                var oDocHead = document.getElementsByTagName('head')[0];
                var oLinks = oDocHead.getElementsByTagName('link');
                var oLink;
                var i = 0;

                for (i = 0; i < oLinks.length; i++)
                {
                    oLink = oLinks[i];
                    if (oLink.type == 'image/x-icon' && oLink.rel == 'shortcut icon')
                    {
                        if (oLink.href === url)
                        {
                            return false;
                        }
                        oDocHead.removeChild(oLink);
                    }
                }

                oLink = document.createElement('link');
                oLink.rel = 'shortcut icon';
                oLink.href = url;
                oLink.type = 'image/x-icon';
                oDocHead.appendChild(oLink);

                return true;
            },
            /* <summary>Replaces tokens in a message: ReplaceTokens("I like %tokenOne% and %tokenTwo%.", {"%tokenOne%": "JavaScript", "%tokenTwo": "CSS"});</summary> */
            ReplaceTokens: function (message, replaceTokenArray)
            {
                var newMessage = message;

                for (var tokenName in replaceTokenArray)
                {
                    if (replaceTokenArray.hasOwnProperty(tokenName))
                    {
                        if (replaceTokenArray[tokenName] !== null)
                        {
                            newMessage = newMessage.replace(tokenName, replaceTokenArray[tokenName]);
                        }
                    }
                }
                return newMessage;
            },
            /* <summary>Checks is the validators are set to valid for a validation group or for all validators on the page if one is not passed</summary> */
            // optionally accepts an element ID, if present the message will be displayed there instead of in a modal
            ValidateForm: function (validationGroup, errorMessageElement) {
                if (typeof Page_Validators === "undefined") { return true; }

                var i,
                    validationErrorMessage = "";

                if (validationGroup !== undefined && validationGroup !== null && validationGroup.length > 0)
                {
                    Page_ClientValidate(validationGroup);
                }
                else
                {
                    Page_ClientValidate();
                }

                if (Page_IsValid === false)
                {
                    validationErrorMessage = "<ul style=\"margin:0; padding: 0;list-style-type: none;\">";

                    for (i = 0; i < Page_Validators.length; i++)
                    {
                        if (Page_Validators[i].isvalid == false && typeof (Page_Validators[i].errormessage) == "string" && (validationGroup == null || Page_Validators[i].validationGroup == validationGroup))
                        {
                            validationErrorMessage += "<li>" + Page_Validators[i].errormessage + "</li>";
                        }
                    }

                    validationErrorMessage += "</ul>";
                    
                    if (typeof(errorMessageElement) === "undefined") {
                        if (SEL.MasterPopup != null)
                        {
                            if (typeof SEL.MasterPopup.ShowMasterPopup === "function")
                            {
                                SEL.MasterPopup.ShowMasterPopup(validationErrorMessage, 'Message from ' + moduleNameHTML);
                            }
                            } else {
                            validationErrorMessage = "";
                                for (i = 0; i < Page_Validators.length; i++) {
                                    if (Page_Validators[i].isvalid == false && typeof(Page_Validators[i].errormessage) == "string" && (validationGroup == null || Page_Validators[i].validationGroup == validationGroup)) {
                                    validationErrorMessage += "-" + Page_Validators[i].errormessage + "\n";
                                }
                            }
                        
                            if (validationErrorMessage.length > 0)
                            {
                                alert(validationErrorMessage);
                            }
                        }

                    } else {
                        $("#" + errorMessageElement).html(validationErrorMessage);

                    }
                    return false;
                }
                else
                {
                    return true;
                }
            },

            Page_ClientValidateReset: function ()
            {
                if (typeof (Page_Validators) !== "undefined")
                {
                    for (var i = 0; i < Page_Validators.length; i++)
                    {
                        var validator = Page_Validators[i];
                        validator.isvalid = true;
                        ValidatorUpdateDisplay(validator);
                    }
                }
            },

            // Generic web service callback method for errors
            WebService: {
                ErrorHandler: function (data) {
                    var errorMessage = (data.responseText ? data.responseText : (data._message ? data._message : data));
                    SEL.MasterPopup.ShowMasterPopup(
                    'An error has occurred processing your request.<span style="display:none;">' + errorMessage + '</span>',
                    'Message from ' + moduleNameHTML);
                }
            },

            // Use this function to bind the Enter key to a specific element. Currently, this must be done using the element's ID.
            BindEnterKeyForElement: function (elementId, functionToCall)
            {
                $('#' + elementId).bind('keypress.checkForEnter', function (e)
                {
                    if (e.which == 13) //Enter keycode
                    {
                        // Prevent the Enter key from performing any default action
                        e.preventDefault();

                        // Unbind the Enter key from the element, to avoid multiple callbacks
                        $('#' + elementId).unbind('keypress.checkForEnter');

                        // Call the desired function
                        //window[functionToCall]();
                        functionToCall.apply($g(elementId));

                        // Setup enter key again for next press
                        SEL.Common.BindEnterKeyForElement(elementId, functionToCall);
                    }
                });
            },

            // Use this function to bind the Enter key to a set of elements using a jQuery selector. 
            // Specify an excludeSelector for any elements in the set you do not wish to bind.
            BindEnterKeyForSelector: function (selector, functionToCall, excludeSelector)
            {
                if (typeof excludeSelector === "undefined")
                {
                    excludeSelector = "";
                }

                $(selector).not(excludeSelector).bind('keypress.checkForEnter', function (e)
                {
                    if (e.which === 13) //Enter keycode
                    {
                        // Prevent the Enter key from performing any default action
                        e.preventDefault();

                        // Unbind the Enter key from the element, to avoid multiple callbacks
                        $(selector).unbind('keypress.checkForEnter');

                        // Call the desired function
                        functionToCall.apply(($(this)[0]));

                        // Setup enter key again for next press
                        SEL.Common.BindEnterKeyForSelector(selector, functionToCall, excludeSelector);
                    }
                });
            },

            // Get all textareas that have a "textareamaxlength" property. 
            SetTextAreaMaxLength: function ()
            {
                $('textarea[textareamaxlength]').on('keyup blur', function ()
                {
                    var control = $(this),
                    // Store the maxlength and value of the field. 
                        maxlength = control.attr('textareamaxlength'),
                        val = control.val();
                    
                    // Trim the field if it has content over the maxlength. 
                    if (val.length > maxlength)
                    {
                        control.val(val.slice(0, maxlength));
                    }
                });
            },

            GetBroadcastMessages: function (interval, includeNotes, page)
            {
                if (interval === undefined || isNaN(interval))
                {
                    interval = 1000;
                }

                var stackTopleft = { "dir1": "down", "dir2": "right", "push": "bottom" };

                $.pnotify.defaults.styling = "jqueryui";

                $.ajax({
                    type: "POST",
                    url: window.appPath + "/shared/webServices/SvcBroadcastMessage.asmx/GetMessages",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    data: "{ includeNotes: " + includeNotes + ", page: \"" + page + "\" }",
                    success: function (data)
                    {
                        var broadcastMessages = data.d;
                        if (broadcastMessages.length)
                        {
                            var index = 0;
                            var showMessage = function ()
                            {
                                if (broadcastMessages[index].broadcastid === 0)
                                {
                                    $.pnotify({
                                        title: broadcastMessages[index].title,
                                        text: broadcastMessages[index].message,
                                        hide: false,
                                        type: 'info',
                                        styling: 'jqueryui',
                                        closer_hover: false,
                                        sticker_hover: false,
                                        addclass: "stack-topleft",
                                        stack: stackTopleft,
                                        animation:
                                            {
                                                effect_in: 'show',
                                                effect_out: 'slide'
                                            },
                                        width: '400px'
                                    });
                                }
                                else
                                {
                                    $.pnotify({
                                        title: broadcastMessages[index].title,
                                        text: broadcastMessages[index].message,
                                        styling: 'jqueryui',
                                        addclass: "stack-topleft",
                                        stack: stackTopleft,
                                        animation:
                                            {
                                                effect_in: 'show',
                                                effect_out: 'slide'
                                            },
                                        width: '500px',
                                        closer_hover: false,
                                        sticker_hover: false
                                    });
                                }

                                if (index === broadcastMessages.length - 1)
                                {
                                    clearInterval(timer);
                                }

                                index++;
                            };

                            //set up a timer for the specified interval, and display the first message immediately
                            var timer = setInterval(showMessage, interval);
                            setTimeout(showMessage, 300);
                        }
                    },
                    error: function (xmlHttpRequest, textStatus, errorThrown)
                    {
                        SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                        'Message from ' + moduleNameHTML);
                    }
                });
            },
            /* Create a Title property to show the original text of an element */
            EllidedTextToTitle: function ()
            {
                /*
                not yet tested as unused on original creation 
                delete this comment when tested somewhere
                */
                $('.ellide').each(function (i, o)
                {
                    if (o.title === null || o.title === '')
                    {
                        o.title = $(o).text();
                    }
                });
            },

            BuildJqueryDialogue: function(selector, width, height) {
                
                // set up the assigment modal
                var highestZIndex = 1 + SEL.Common.GetHighestZIndexInt();
                var modal = $(selector);
                modal.dialog({
                    modal: true,
                    autoOpen: false,
                    zIndex: highestZIndex,
                    width: width,
                    height: height,
                    closeOnEscape: true,
                    draggable: false,
                    resizable: false,
                    open: function () {
                        //replace button classes
                        $(".ui-button").unbind(); //prevent the jQuery standard focus effects
                        $(".ui-button").click(function () {
                            modal.dialog('destroy');
                        }); // replace the click event
                        $(".ui-button").keyup(function (event) {
                            if (event.keyCode == 13) {
                                modal.dialog('destroy');
                            } // also allow ENTER to close modal
                        });
                        $('.ui-dialog-buttonpane.ui-widget-content.ui-helper-clearfix').removeClass('ui-dialog-buttonpane ui-widget-content ui-helper-clearfix').addClass('formbuttons').addClass('formpanel');
                        $('.ui-dialog-buttonset').removeClass('ui-dialog-buttonset').addClass('buttonContainer');
                        $('.ui-button.ui-widget.ui-corner-all.ui-button-text-only').removeClass('ui-button ui-widget ui-corner-all ui-button-text-only').addClass('buttonInner');

                        $('.ui-button-text').removeClass('ui-button-text');
                        $('.ui-state-default').removeClass('ui-state-default');
                        $('.ui-state-hover').removeClass('ui-state-hover');
                        $('.ui-state-focus').removeClass('ui-state-focus');
                    }
                });
                modal.dialog("option", "dialogClass", "modalpanel").css('padding', '0');
                modal.dialog("option", "dialogClass", "formpanel");
                $(".ui-dialog-titlebar").hide();
                modal.hide();

                return modal;
            },

            Sign: function (num)
            {
	            // IE does not support method sign here
                if (typeof Math.sign === 'undefined')
                {
	                if (num > 0) {
	                    return 1;
	                }
	                if (num < 0) {
	                    return -1;
	                }
	                return 0;
	            }

                return Math.sign(num);
            },
 
            PreciseRound: function (num, decimals)
            {
                num = num || "";
                decimals = decimals || 0;

                if (typeof num == 'string')
                {
                    var o = num;
                    if (num.substr(0, 1) == '.')
                        num = '0' + num;
                    num = parseFloat(num);
                    if (isNaN(num))
                        return o;
                }

                var t = Math.pow(10, decimals);
                return (Math.round((num * t) + (decimals > 0 ? 1 : 0) * (SEL.Common.Sign(num) * (10 / Math.pow(100, decimals)))) / t).toFixed(decimals);
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
