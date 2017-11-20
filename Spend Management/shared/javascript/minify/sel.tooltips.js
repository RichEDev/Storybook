(function ()
{
    var scriptName = "Tooltip";
    
    function execute()
    {
        SEL.registerNamespace("SEL.Tooltip");
        
        SEL.Tooltip =
        {
            activeTooltipControl: null,
            tooltipPopupExtenderID: null,
            tooltipPopupContent: null,
            /// <summary>
            /// This should be attached to controls with tooltips and will fetch the tooltip content and attach a 'close' event, the first parameter should be either the database help_text helpid or a custom string to display.
            /// The last parameter is the enum value of the position 0 = right, 5 = top-left.
            /// </summary>     
            Show: function (tooltipValue, tooltipArea, popupControl, position)
            {
                if (position) {
                    $f(SEL.Tooltip.tooltipPopupExtenderID)._popupBehavior._positioningMode = position;
                } else {
                    $f(SEL.Tooltip.tooltipPopupExtenderID)._popupBehavior._positioningMode = 0;
                }
                
                this.activeTooltipControl = popupControl;
                if (this.activeTooltipControl === null || this.tooltipPopupContent === null || this.tooltipPopupExtenderID === null)
                {
                    SEL.Errors.InvalidArgument("Required properties/variables are not set.");
                }
                else
                {
                    this.hide(false);
                    popupControl.onmouseout = function ()
                    {
                        SEL.Tooltip.hide(true);
                    };
                    
                    if (/^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/.test(tooltipValue))
                    {
                        Spend_Management.svcTooltip.GetToolTipContents(tooltipValue, tooltipArea, this.onComplete, this.onError);
                    }
                    else
                    {
                        SEL.Tooltip._show(tooltipValue);
                    }
                }
            },
            /// <summary>
            /// Performs the actual showing and setting content of the tooltip
            /// </summary>     
            _show: function (contents)
            {
                if (SEL.Tooltip.activeTooltipControl !== null)
                {
                    this.setContents(contents);
                    SEL.Common.ShowPopup(this.tooltipPopupExtenderID, this.activeTooltipControl);
                    this.activeTooltipControl = null;
                }
                return;
            },
            /// <summary>
            /// Called when a tooltip is fetched by web service
            /// </summary>     
            onComplete: function (tooltipContents)
            {
                SEL.Tooltip._show(tooltipContents);
                return;
            },
            /// <summary>
            /// If an error occurs trying to retrieve the tooltip contents this method is called and a friendly error returned
            /// </summary>                
            onError: function (tooltipError)
            {
                SEL.Tooltip._show("Unable to display tooltip content");
                return;
            },
            /// <summary>
            /// Hides the tooltip
            /// </summary>     
            hide: function (nullifyActiveTooltip)
            {
                SEL.Common.HidePopup(this.tooltipPopupExtenderID);
                if (nullifyActiveTooltip === true)
                {
                    this.activeTooltipControl = null;
                }
                return;
            },
            /// <summary>
            /// Sets the content of the tooltip
            /// </summary>
            setContents: function (contents)
            {
                $g(this.tooltipPopupContent).innerHTML = contents;
                return;
            },
            /// <summary>
            /// Sets the content of the of a user defined tooltip
            /// </summary>
            UDTooltip: function showUserDefinedTooltip(popupControl, helpID, accountID)
            {

                var contents;
                this.activeTooltipControl = popupControl;

                if (this.popupControl === null || this.tooltipPopupContent === null || this.tooltipPopupExtenderID === null)
                {
                    SEL.Errors.InvalidArgument("Required properties/variables are not set.");
                }
                else
                {
                    this.hide(false);
                    popupControl.onmouseout = function ()
                    {
                        SEL.Tooltip.hide(true);
                    };

                    contents = helpID + ',' + accountID

                   Spend_Management.svcTooltip.getUserDefinedTooltip(contents, this.onComplete, this.onError);
                }
            },
            /// <summary>
            /// Sets the content of the of a Custom defined tooltip
            /// </summary>
            customToolTip: function showCustomTooltip(popupControl, tooltipText)
            {
                if (typeof event !== 'undefined')
                {
                    event.cancelBubble = true;
                }
                var tooltip;
                this.activeTooltipControl = popupControl;

                tooltip = "<div class=\"tooltipcontainer\">";
                tooltip += "<div class=\"tooltipcontent\">" + tooltipText + "</div>";
                tooltip += "<!--[if lte IE 6.5]><iframe class=\"iefix\" src=\"/blank.htm\"></iframe><![endif]--></div>";
                tooltip += "</div>";

                document.getElementById(tooltipPanel).innerHTML = tooltip;

                $find(tooltipPopupControlExtender)._popupBehavior._parentElement = document.getElementById(popupControl);
                $find(tooltipPopupControlExtender).showPopup();
                $find(tooltipPopupControlExtender)._popupElement.style.zIndex = GetHighestZIndex();
            },
            /// <summary>
            /// Sets the content of the of a Custom defined tooltip
            /// </summary>
            hideTooltip: function hideCustomTooltip()
            {
                $find(tooltipPopupControlExtender).hidePopup();
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