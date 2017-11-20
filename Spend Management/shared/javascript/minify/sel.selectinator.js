var AutoCompleteSearches = {
    New: function(controlName, controlId, modalId, panelId, triggerFields, matchTableId) {
        var newSearch = null;
        if (!AutoCompleteSearches.hasOwnProperty(controlName)) {
            newSearch = new AutoCompleteSearches.Class(controlName,
                controlId,
                modalId,
                panelId,
                triggerFields,
                matchTableId);
            AutoCompleteSearches[controlName] = newSearch;
        }
        return newSearch;
    },
    Class: function(controlName, controlId, modalId, panelId, triggerFields, matchTableId) {
        this.name = controlName;
        this.selectors = {
            textbox: "#" + controlId,
            id: "#" + controlId + "_ID",
            select: "#" + controlId + "Select",
            icon: "#" + controlId + "SearchIcon",
            grid: "#" + panelId + ">div.searchgrid",
            panel: "#" + panelId,
            modal: modalId,
            triggerFields: triggerFields,
            matchTableId: matchTableId
        };
    },
    Modal: function (myPopupDiv, searchType, controlId, selectinator) {

        if (AutoCompleteSearches.ParentChildFilterData == undefined) {
            AutoCompleteSearches.ParentChildFilterData = null;
        }

        $('.selectinatorDiv')
            .each(function(index) {
                if (index > 0) {
                    $(this).attr('class', 'unusedDiv' + index);
                }
            });
        $('.selectinatorGrid')
            .each(function(index) {
                if (index > 0) {
                    $(this).attr('class', 'unusedGrid' + index);
                }
            });
        var title = searchType + ' Search';

        var highestZIndex = 1 + SEL.Common.GetHighestZIndexInt();
        $('.selectinatorDiv')
            .dialog({
                title: title,
                zIndex: highestZIndex,
                autoOpen: false,
                modal: true,
                width: 890,
                height: "auto",
                closeOnEscape: true,
                draggable: false,
                resizable: false
            });

        $.ajax({
            dataType: "json",
            contentType: "application/json",
            url: appPath + "/shared/webServices/svcAutoComplete.asmx/GetSelectinatorComboSearchGrid",
            data: "{ 'type': '" +
                searchType +
                "' , 'controlId': '" +
                controlId +
                "' , 'selectinatorString': '" +
                selectinator +
                "',childFilterList:" +
                JSON.stringify(AutoCompleteSearches.ParentChildFilterData) + " }",
            method: "post",
            success: function (data, status, jqXhr) {
                if (typeof data !== "undefined" && data !== null && typeof data.d !== "undefined" && data.d !== null) {
                    var modal = $('.selectinatorDiv');
                    modal.dialog("option", "dialogClass", "modalpanel");
                    modal.dialog("option", "dialogClass", "formpanel");
                    $(".ui-dialog-titlebar").hide();
                    $(".selectinatorTitle").text(title);
                    $('.selectinatorGrid').html(data.d[1]);
                    SEL.Grid.updateGrid(data.d[0]);
                    modal.dialog('open');
                }
                else {
                    SEL.Common.WebService.ErrorHandler(data);
                }
            },
            error: function (data, status, jqXhr) { SEL.Common.WebService.ErrorHandler(data); }
        });


    },
    HideModal: function(myPopupDiv) {
        $('.selectinatorDiv').dialog('destroy');
    },

    InitailData : [],
   ParentChildFilterData: []
};

AutoCompleteSearches.Class.prototype.Grid = function (conditionType) {
    var containerSelector = this.selectors.grid,
        modalSelector = this.selectors.modal,
        panelSelector = this.selectors.panel,
        _this = this,
        conditionTypeNumber = null;
    
    if (conditionType !== undefined && conditionType !== null) {
        switch (conditionType) {
            case 'All':
                conditionTypeNumber = 251;
                break;
            case 'Roles':
                conditionTypeNumber = 252;
                break;
            case 'Hierarchy':
                conditionTypeNumber = 253;
                break;
        default:
        }
    }
    $.ajax({
        dataType: "json",
        contentType: "application/json",
        url: appPath + "/shared/webServices/svcAutoComplete.asmx/Get" + this.name.replace(/[^a-zA-Z]/gi, "") + "SearchGrid",
        data: "{ 'conditionType': '" + conditionTypeNumber + "' }",
        method: "post",
        success: function (data, status, jqXhr) {
            if (typeof data !== "undefined" && data !== null && typeof data.d !== "undefined" && data.d !== null) {
                $(containerSelector).html(data.d[1] + "<script>SEL.Grid.updateGrid('" + data.d[0] + "');</script>");
                $f(modalSelector).show();

                $(document).on("keydown.selectinator", function (e)
                {
                    if (e.keyCode === $.ui.keyCode.ESCAPE)
                    {
                        e.preventDefault();
                        _this.CloseSearch();
                    }
                });
            }
            else {
                SEL.Common.WebService.ErrorHandler(data);
            }
        },
        error: function (data, status, jqXhr) { SEL.Common.WebService.ErrorHandler(data); }
    });
};

AutoCompleteSearches.Class.prototype.Search = function (conditionType) {
    this.Grid(conditionType);
};

AutoCompleteSearches.Class.prototype.SearchChoice = function (value, text) {
    if (typeof value !== "undefined" && value !== null && typeof text !== "undefined" && text !== null) {
        $(this.selectors.textbox).val(text);
        $(this.selectors.id).val(value);
    }
    this.CloseSearch();
};

AutoCompleteSearches.Class.prototype.SelectinatorChoice = function (value, text) {
    if (typeof value !== "undefined" && value !== null && typeof text !== "undefined" && text !== null) {
        $(this.selectors.textbox).val(text);
        $(this.selectors.id).val(value);
    }
    
    if (this.selectors.triggerFields !== null) {
        SEL.AutoComplete.TriggerFields.Update(this.selectors.matchTableId, value, this.selectors.triggerFields);
    }
    var form = new RegExp("[?&]" + 'formid'.replace(/[\[\]]/g, "\\$&") + "(=([^&#]*)|&|#|$)").exec(window.location.href);
    if (form != null) {
        var parentControlId = this.selectors.id.toString().substring(this.selectors.id.indexOf("txt"), this.selectors.id.lastIndexOf("_")).replace(/[^0-9]/gi, '');
        var entity = new RegExp("[?&]" + 'entityid'.replace(/[\[\]]/g, "\\$&") + "(=([^&#]*)|&|#|$)").exec(window.location.href);
        SEL.AutoComplete.TriggerFields.UpdateChild(parentControlId, value, decodeURIComponent(form[2].replace(/\+/g, " ")), decodeURIComponent(entity[2].replace(/\+/g, " ")));

    }

    $(".ui-dialog-content").dialog().dialog("destroy");
};


AutoCompleteSearches.Class.prototype.SelectChange = function () {
    var obj = $(this.selectors.select)[0];
    var idControl = $(this.selectors.id)[0];


    if (typeof obj === "undefined" || typeof idControl === "undefined" || obj === null || idControl === null) {
        return;
    }

    var hiddenIdField = $(this.selectors.id),
        selectedValue = (obj.nodeName === "SELECT" && obj.options.length > 0) ? obj.options[obj.selectedIndex].value : "0";

    if (selectedValue === "" || selectedValue === "0") {
        if (hiddenIdField.length === 1) {
            hiddenIdField.val("");
        }
    }
    else {
        if (hiddenIdField.length === 1) {
            hiddenIdField.val(selectedValue);
        }
    }
    if (this.selectors.triggerFields !== null) {
        SEL.AutoComplete.TriggerFields.Update(this.selectors.matchTableId, selectedValue, this.selectors.triggerFields);
    }

    var form = new RegExp("[?&]" + 'formid'.replace(/[\[\]]/g, "\\$&") + "(=([^&#]*)|&|#|$)").exec(window.location.href);
    if (form != null) {
        var parentControlId = obj.id.toString().substring(obj.id.indexOf("txt"), obj.id.lastIndexOf("_")).replace(/[^0-9]/gi, '');
        var entity = new RegExp("[?&]" + 'entityid'.replace(/[\[\]]/g, "\\$&") + "(=([^&#]*)|&|#|$)").exec(window.location.href);
         
        SEL.AutoComplete.TriggerFields.UpdateChild(parentControlId, selectedValue, decodeURIComponent(form[2].replace(/\+/g, " ")),decodeURIComponent(entity[2].replace(/\+/g, " ")));
    }


};

AutoCompleteSearches.Class.prototype.CloseSearch = function ()
{
    $f(this.selectors.modal).hide();
    $(document).off("keydown.selectinator");
};

AutoCompleteSearches.Class.prototype.CloseSearch = function ()
{
    $f(this.selectors.modal).hide();
    $(document).off("keydown.selectinator");
};