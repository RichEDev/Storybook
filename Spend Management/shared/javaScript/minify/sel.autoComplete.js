(function (SEL, $, $g, $f, $e) {
    var scriptName = "AutoComplete";

    function execute() {
        SEL.registerNamespace("SEL.AutoComplete");
        SEL.filterRules = [];
        SEL.AutoComplete = {
            Data: {
                Request: null
            },

            EnableSelectinator: false,
            Bind: function (cntl, maxRows, matchTableId, displayField, matchFieldIDs, autoCompleteFieldIDs, fieldFilters, msDelay, triggerFields, keyIsString, childFilterList, displayAutocompleteMultipleResultsFields, unsuccessfulAutomatchFn) {

                var triggerParameter = new SEL.AutoCompleteCombo.JsonObjects.AutoCompleteDropdownBindParameter();
                triggerParameter.controlname = cntl.toString().substring(cntl.indexOf("txt"), cntl.lastIndexOf("_")).replace(/[^0-9]/gi, '');
                triggerParameter.BindMatchTableId = matchTableId;
                triggerParameter.BindtriggerFields = triggerFields;
                SEL.AutoCompleteCombo.AutoCompleteDropdownBindParameterList.BindMatchParameterList.push(triggerParameter);
                SEL.AutoComplete.Bind.childFilterList = childFilterList;

                var applyFilterRulesForCostCode = false;

                if (cntl.includes("txtCostCode")) {
                    applyFilterRulesForCostCode = true;
                }

                if (applyFilterRulesForCostCode) {

                    //check if filter rule already exists in array
                    var removeIndex = SEL.filterRules.map(function (item) { return item.id; }).indexOf(cntl);

                    if (removeIndex > -1) {
                        // remove object if exists, so we can readd with new filter rules
                        SEL.filterRules.splice(removeIndex, 1);
                    }

                    var obj = {};
                    obj["id"] = cntl;
                    obj["filter"] = JSON.stringify(childFilterList);

                    SEL.filterRules.push(obj);
                }


                $(document).ready(function () {
                    $('[id$=' + cntl + ']')
                        // don't navigate away from the field on tab when selecting an item
                        .bind("keydown", function (event) {
                            if (event.keyCode === $.ui.keyCode.TAB && $(this).data("autocomplete").menu.active) {
                                event.preventDefault();
                            }
                            else if (event.keyCode !== $.ui.keyCode.ENTER
                                && event.keyCode !== $.ui.keyCode.TAB
                                && event.keyCode !== $.ui.keyCode.RIGHT
                                && event.keyCode !== $.ui.keyCode.LEFT
                                && event.ctrlKey === false
                                && event.altKey === false
                                && event.shiftKey === false
                                && $(this).first().val() !== '') {
                                $('[id$=' + cntl + '_ID]').val('-1');
                                if (triggerFields !== null) {
                                    SEL.AutoComplete.TriggerFields.Clear(triggerFields);
                                }
                            }
                        })
                        .bind("paste", function () {
                            $('[id$=' + cntl + '_ID]').val('-1');
                            if (triggerFields !== null) {
                                SEL.AutoComplete.TriggerFields.Clear(triggerFields);
                            }
                        })
                        .autocomplete(
                            {

                                source: function (request, response) {
                                    var paramList, serviceMethod;
                                    if (autoCompleteFieldIDs === "null" || autoCompleteFieldIDs === null) {
                                        displayAutocompleteMultipleResultsFields = "False";
                                    }

                                    if (applyFilterRulesForCostCode) {
                                        var item = SEL.filterRules.filter(x => x.id === this.element.context.id);
                                        SEL.AutoComplete.Bind.childFilterList = item["0"].filter;
                                    }

                                    else if (SEL.AutoComplete.Bind.childFilterList == undefined) {
                                        SEL.AutoComplete.Bind.childFilterList = null;
                                    }
                                    if (displayAutocompleteMultipleResultsFields === "False") {
                                        paramList = "{ maxRows: " +
                                            maxRows +
                                            ", matchTable: \"" +
                                            matchTableId +
                                            "\", displayField: \"" +
                                            displayField +
                                            "\", matchFields: \"" +
                                            matchFieldIDs.split(',') +
                                            "\", matchText: \"" +
                                            request.term +
                                            "\", useWildcards: true, filters: " +
                                            fieldFilters +
                                            ", keyIsString: '" +
                                            keyIsString +
                                            "',childFilterList: " +
                                            SEL.AutoComplete.Bind.childFilterList + " }";
                                        serviceMethod = "/shared/webServices/svcAutoComplete.asmx/getAutoCompleteOptions";
                                    } else {
                                        paramList = "{ maxRows: " +
                                            maxRows +
                                            ", matchTable: \"" +
                                            matchTableId +
                                            "\", displayField: \"" +
                                            displayField +
                                            "\", matchFields: \"" +
                                            matchFieldIDs.split(',') +
                                            "\",autoCompleteFields: \"" +
                                            autoCompleteFieldIDs.split(',') +
                                            "\", matchText: \"" +
                                            request.term +
                                            "\", useWildcards: true, filters: " +
                                            fieldFilters +
                                            ", keyIsString: '" +
                                            keyIsString +
                                            "',childFilterList: " +
                                            SEL.AutoComplete.Bind.childFilterList + "}";
                                        serviceMethod = "/shared/webServices/svcAutoComplete.asmx/GetGreenlightAutoCompleteOptions";
                                    }
                                    SEL.Data.AbortAjax(SEL.AutoComplete.Data.Request);
                                    SEL.AutoComplete.Data.Request = $.ajax({
                                        type: "POST",
                                        url: appPath + serviceMethod,
                                        dataType: "json",
                                        contentType: "application/json; charset=utf-8",
                                        data: paramList,
                                        success: function (data) {

                                            if (displayAutocompleteMultipleResultsFields === "False") {
                                                response($.map(data.d,
                                                    function (item) {
                                                        return {
                                                            label: item.label, value: item.value
                                                        };
                                                    }));
                                            } else {
                                                response($.map(data.d,
                                                    function (item) {
                                                        return {
                                                            label: item.label + "|" + item.formattedText, value: item.value
                                                        };
                                                    }));
                                            }
                                        },
                                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                                            if (textStatus !== "abort") {
                                                alert('An error occurred with service call.\n\n' + errorThrown);
                                            }
                                        }
                                    });
                                },
                                minLength: 3,
                                delay: msDelay,
                                select: function (event, ui) {
                                    $('[id$=' + cntl + '_ID]').val(ui.item.value);
                                    var formattedLabel = ui.item.label.split('|');
                                    $('[id$=' + cntl + ']').first().val(formattedLabel[0]).trigger("change");

                                    if (triggerFields !== null) {
                                        SEL.AutoComplete.TriggerFields.Update(matchTableId, ui.item.value, triggerFields);
                                    }

                                    var form = new RegExp("[?&]" + 'formid'.replace(/[\[\]]/g, "\\$&") + "(=([^&#]*)|&|#|$)").exec(window.location.href);
                                    if (form != null) {
                                        var parentControlId = cntl.toString().substring(cntl.indexOf("txt"), cntl.lastIndexOf("_")).replace(/[^0-9]/gi, '');
                                        var entity = new RegExp("[?&]" + 'entityid'.replace(/[\[\]]/g, "\\$&") + "(=([^&#]*)|&|#|$)").exec(window.location.href);
                                        SEL.AutoComplete.TriggerFields.UpdateChild(parentControlId, ui.item.value, decodeURIComponent(form[2].replace(/\+/g, " ")), decodeURIComponent(entity[2].replace(/\+/g, " ")));

                                    }

                                    return false;
                                },
                                focus: function (event, ui) {
                                    var formattedLabel = ui.item.label.split('|');
                                    $('[id$=' + cntl + ']').first().val(formattedLabel[0]);
                                    return false;
                                },
                                open: function (event, ui) {
                                    $('[id$=' + cntl + ']').autocomplete('widget').css("z-index", 1 + SEL.Common.GetHighestZIndexInt());
                                    return false;
                                },
                                create: function () {
                                    $(this).data('ui-autocomplete')._renderItem = function (ul, item) {
                                        if (displayAutocompleteMultipleResultsFields === null || displayAutocompleteMultipleResultsFields === "False") {
                                            return $("<li class='ui-menu-item'></li>")
                                                .data("item.autocomplete", item)
                                                .append("<a>" + item.label + "</a>")
                                                .appendTo(ul);

                                        } else {
                                            var formattedLabel = item.label.split('|');
                                            return $("<li class='ui-menu-item'></li>")
                                                .data("item.autocomplete", item)
                                                .append("<a><strong>" +
                                                    formattedLabel[0] +
                                                    "</strong><br><span class='autoCompleteSearchResults'>" +
                                                    formattedLabel[1] +
                                                    "</span></a>")
                                                .appendTo(ul);
                                        }
                                    }
                                }
                            });
                    // Setup blur event to ensure a valid entry has been given
                    $('[id$=' + cntl + ']').blur(function () {
                        if (document.activeElement.id !== 'ui-active-menuitem') {
                            var fieldValue = $(this).val(),
                                hiddenIdField = $('[id$=' + cntl + '_ID]');

                            if (fieldValue === '') {
                                hiddenIdField.val('');
                                $('[id$=' + cntl + ']').trigger("change");
                                if (triggerFields !== null) {
                                    SEL.AutoComplete.TriggerFields.Clear(triggerFields);
                                }
                            }
                            else {
                                if (hiddenIdField.val() === "-1") {
                                    if (SEL.AutoComplete.Bind.childFilterList == undefined) {
                                        SEL.AutoComplete.Bind.childFilterList = null;
                                    }
                                    SEL.Data.AbortAjax(SEL.AutoComplete.Data.Request);
                                    SEL.AutoComplete.Data.Request = $.ajax(
                                        {
                                            type: "POST",
                                            url: appPath + "/shared/webServices/svcAutoComplete.asmx/getAutoCompleteOptions",
                                            dataType: "json",
                                            contentType: "application/json; charset=utf-8",
                                            data: "{ maxRows: " + maxRows + ", matchTable: \"" + matchTableId + "\", displayField: \"" + displayField + "\", matchFields: \"" + matchFieldIDs.split(',') + "\", matchText: \"%" + fieldValue + "%\", useWildcards: false, filters: " + fieldFilters + ", keyIsString: '" + keyIsString + "',childFilterList: " + SEL.AutoComplete.Bind.childFilterList + "  }",
                                            success: function (data) {
                                                // If the field has lost focus and the current text is the only result, select it
                                                if (data.d.length === 1) {
                                                    hiddenIdField.val(data.d[0].value);
                                                    $('[id$=' + cntl + ']').trigger("change");

                                                    if (triggerFields !== null) {
                                                        SEL.AutoComplete.TriggerFields.Update(matchTableId, data.d[0].value, triggerFields);
                                                    }

                                                    // if there are no results and the unsuccessfulAutomatchFn function exists    
                                                } else if (data.d.length === 0 && $.type(unsuccessfulAutomatchFn) === "function") {
                                                    unsuccessfulAutomatchFn(cntl);
                                                }
                                            },
                                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                                if (textStatus !== "abort") {
                                                    alert('An error occurred with service call.\n\n' + errorThrown);
                                                }
                                            }
                                        });
                                }
                            }
                        }
                    });

                    $('input.costcode-autocomplete, input.costcodeDescription-autocomplete').focus(function () {

                        $(this).autocomplete("option", "minLength", 1);

                        if (!$(this).val()) {
                            $(this).autocomplete("search", "%%%");
                        }
                    });
                });
            },

            SetCostCodeAutoCompleteOptions: function (element) {

                $(element).autocomplete("option", "minLength", 1);

                if (!$(element).val()) {
                    $(element).autocomplete("search", "%%%");
                }
            },

            TriggerFields:
            {
                Clear: function (triggerFields) {
                    $(triggerFields).each(
                        function (i, o) {
                            $('[id$=' + o.ControlId + ']').html('&nbsp;');
                        }
                    );
                },

                Update: function (baseTableId, matchId, triggerFields) {
                    Spend_Management.svcAutoComplete.GetTriggerFieldValues(baseTableId, matchId, triggerFields,
                        function (data) {
                            if (triggerFields !== null) {
                                if (data.length !== triggerFields.length) {
                                    SEL.AutoComplete.TriggerFields.Clear(triggerFields);
                                }
                            }

                            $(data)
                                .each(
                                    function (i, o) {
                                        $('[id$=' + o.ControlId + ']')
                                            .html(o.DisplayValue === null || o.DisplayValue === ''
                                                ? '&nbsp;'
                                                : o.DisplayValue);
                                        if (o.DocumentGuid !== null) {
                                            $("[id$=" + o.ControlId + "]").attr("onclick", "javascript:viewFieldLevelAttachment('" + o.DocumentGuid + "', 0, 0, 0, '" + $("[id$=" + o.ControlId + "]").attr("id") + "', true);");
                                            $("[id$=" + o.ControlId + "]").attr("referencevalue", o.DocumentGuid);
                                            $("[id$=" + o.ControlId + "]").attr("class", "lookupdisplayvalue attachmentLookUpDisplayField");
                                        }
                                    }
                                );

                        },
                        SEL.Common.WebService.ErrorHandler
                    );

                },

                UpdateChild: function (parentControlId, matchId, formId, entityId) {
                    var children = [];
                    var parents = [];

                    function updateValues(parentClass, val) {
                        if (typeof val !== "undefined") {
                            var obj = {};
                            var parentId = parseInt(parentClass.replace('parenttxt', ''));
                            if (typeof parentId === 'number' && (parentId % 1) === 0) {
                                obj["Id"] = parentId;
                                obj["Value"] = val;
                                if (parents.map(function (o) { return o.Id; }).indexOf(parentId) === -1) {
                                    parents.push(obj);
                                }
                            }
                        }
                    }

                    // get all the children of the current parent control
                    var childrenElements = $('.parenttxt' + parentControlId);

                    var childParentCollection = {};
                    // for each child element get parents
                    $(childrenElements).each(function (i) {
                        childParentCollection[$(this).attr('id')] = $(this).attr('class').replace('fillspan', '').trim().split(' ');

                    });

                    for (var key in childParentCollection) {
                        var value = key.replace('_SelectinatorTextSelect', '');
                        children.push(value.substring(value.lastIndexOf('_') + 1).replace('txt', ''));
                        $(childParentCollection[key]).each(function (i) {
                            updateValues(this, $("input[id$='" + this.replace('parent', '') + "_SelectinatorText_ID']").val());
                        });
                    }

                    Spend_Management.svcAutoComplete.GetTriggerFieldParentValues(parents, children, formId, entityId,
                        function (data) {
                            if (data != null) {
                                $(data).each(function (i) {
                                    var childFieldToBuild = this.Id;
                                    var childFieldValues = this.ChildFieldValues;
                                    var currentChildControl = $('select[id*=txt' + childFieldToBuild + ']');
                                    if (!AutoCompleteSearches.InitailData.filter(function (e) { return e.childControl == childFieldToBuild; }).length > 0) {
                                        var obj = {
                                            parentControls: parents,
                                            childControl: childFieldToBuild,
                                            EnableSelectinator: $('input[id$=txt' + childFieldToBuild + '_SelectinatorText]').is(":visible")
                                        };
                                        AutoCompleteSearches.InitailData.push(obj);
                                    }
                                    if (childFieldValues.length === 1 && childFieldValues[0].Key === 0) {
                                        currentChildControl.find('option').remove();
                                        currentChildControl.append($('<option>', { value: 0, text: '[None]', title: "None" }));
                                        currentChildControl.show();
                                        currentChildControl.next(".AutoCombostyledSelect").show();
                                        currentChildControl.next(".AutoCombooptions").show();
                                        currentChildControl.parent(".AutoComboselect").show();
                                        currentChildControl.next(".AutoCombostyledSelect").text("[None]");
                                        $("label[id*=txt" + childFieldToBuild + "]").addClass("AutoComboMargin");
                                        currentChildControl.parent().find(".AutoCombooptions").html("<li value=\"0\"><span class=\"displayName\">[None]</span></li>");

                                        var reference = $('input[id$=txt' + childFieldToBuild + '_SelectinatorText]');
                                        reference.hide();
                                        $('img[id*=txt' + childFieldToBuild + ']').hide();

                                        var referenceId = reference.prop("id");
                                        var referenceHiddenFieldForDropdownId = $("#" + referenceId + "_ID");
                                        referenceHiddenFieldForDropdownId.val('');

                                        //Clear the trigger fields
                                        SEL.AutoCompleteCombo.AutoCompleteDropdownBindParameterList.BindMatchParameterList.filter(function (newArray) {
                                            if (newArray.controlname === childFieldToBuild) {
                                                SEL.AutoCompleteCombo.AutocompleteComboSelect(0, newArray.BindMatchTableId, newArray.BindtriggerFields);
                                            }
                                        });
                                    }
                                    else {
                                        if (childFieldValues.length > 0 && childFieldValues[0].FieldToBuild != undefined)
                                            currentChildControl.find('option').remove();
                                        currentChildControl.append($('<option>', {
                                            value: 0,
                                            text: '[None]',
                                            title: "None"
                                        }));
                                        $(childFieldValues).each(function (i, o) {
                                            if (o.FormattedText === null) {
                                                currentChildControl.append($('<option>', { value: o.Key, text: o.Value }));
                                            }
                                            else {
                                                var newOption = "<option title='displayField'  value='" + o.Key + "',1>" + o.Value + "</option>";
                                                var newOption2 = "<option title='formatted' value='" + o.Key + "',2>" + o.FormattedText + "</option>";
                                                currentChildControl.append(newOption);
                                                currentChildControl.append(newOption2);
                                            }
                                        });
                                        //Set the child control value back to None
                                        currentChildControl.val(0).change();
                                        //Clear the trigger fields                                   
                                        var controlId = $(currentChildControl).attr('id').toString().substring($(currentChildControl).attr('id').indexOf("txt"), $(currentChildControl).attr('id').lastIndexOf("_")).replace(/[^0-9]/gi, '');
                                        SEL.AutoCompleteCombo.AutoCompleteDropdownBindParameterList.BindMatchParameterList.filter(function (newArray) {
                                            if (newArray.controlname === controlId) {
                                                SEL.AutoCompleteCombo.AutocompleteComboSelect(currentChildControl.val(), newArray.BindMatchTableId, newArray.BindtriggerFields);
                                            }
                                        });

                                        if (childFieldValues.length > 25 && $('input[id*=txt' + childFieldToBuild + ']').length > 0) ///25 is the maximum list item we allow in dropdown
                                        {
                                            var parentControlValue = {
                                                parentControls: parents,
                                                childControl: children
                                            };
                                            SEL.AutoComplete.Bind.childFilterList = JSON.stringify(childFieldValues);

                                            AutoCompleteSearches.ParentChildFilterData =
                                                AutoCompleteSearches.ParentChildFilterData.filter(function (childFieldValues) {
                                                    return childFieldValues.parentControl !== parentControlValue.parentControl;
                                                });

                                            AutoCompleteSearches.ParentChildFilterData.push(parentControlValue);
                                            currentChildControl.hide();
                                            currentChildControl.next(".AutoCombostyledSelect").hide();
                                            $('input[id$=txt' + childFieldToBuild + '_SelectinatorText]').val('');
                                            currentChildControl.parent(".AutoComboselect").hide();
                                            $('input[id$=txt' + childFieldToBuild + '_SelectinatorText]').show();
                                            $("label[id*=txt" + childFieldToBuild + "]")
                                                .removeClass("AutoComboMargin");
                                            $('img[id*=txt' + childFieldToBuild + ']').show();
                                        } else {
                                            if (childFieldValues[0].FormattedText === null) {
                                                currentChildControl.on("change", function (event) {
                                                    window["txt" + childFieldToBuild + "selectinator"]["SelectChange"](this);
                                                });
                                                $("label[id*=txt" + childFieldToBuild + "]").removeClass("AutoComboMargin");
                                            } else {
                                                $("label[id*=txt" + childFieldToBuild + "]").addClass("AutoComboMargin");
                                                currentChildControl.next(".AutoCombostyledSelect").remove();
                                                currentChildControl.parent(".AutoComboselect").show();

                                                var referenceId = $('input[id$=txt' + childFieldToBuild + '_SelectinatorText]').prop("id");
                                                var referenceHiddenFieldForDropdownId = $("#" + referenceId + "_ID");
                                                referenceHiddenFieldForDropdownId.val('');

                                                var childControl = ($(currentChildControl).attr('id').toString().substring($(currentChildControl).attr('id').indexOf("txt"), $(currentChildControl).attr('id').lastIndexOf("_")).replace(/[^0-9]/gi, ''));
                                                SEL.AutoCompleteCombo.ulSelectFormat(currentChildControl, childControl);
                                            }
                                            currentChildControl.show();
                                            currentChildControl.val(0).change();
                                            $('input[id$=txt' + childFieldToBuild + '_SelectinatorText]').hide();
                                            $('img[id*=txt' + childFieldToBuild + ']').hide();
                                        }
                                    }
                                });
                            }
                        },
                        SEL.Common.WebService.ErrorHandler
                    );
                }
            }
        };
    }

    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }

}(SEL, jQuery, $g, $f, $e));

(function (SEL, $, $g, $f, $e) {
    var scriptName = "AutoCompleteCombo";

    function execute() {
        SEL.registerNamespace("SEL.AutoCompleteCombo");

        SEL.AutoCompleteCombo = {
            JsonObjects: {
                AutoCompleteDropdownBindParameter: function () {
                    this.controlname = null;
                    this.BindMatchTableId = null;
                    this.BindtriggerFields = null;
                }
            },
            AutoCompleteDropdownBindParameterList:
            {
                BindMatchParameterList: []
            },

            Grid: function (elementObject) {
                var containerSelector = $(elementObject).closest(".autocompletecombo-container"),
                    type = containerSelector.data("type"),
                    searchPanel = containerSelector.data("panel"),
                    gridPanel = containerSelector.data("grid"), // this.selectors.grid;
                    modalId = containerSelector.data("modal"); // this.selectors.modal;

                $.ajax({
                    dataType: "json",
                    contentType: "application/json",
                    url: appPath + "/shared/webServices/svcAutoComplete.asmx/GetAutoCompleteComboSearchGrid",
                    method: "post",
                    data: JSON.stringify({ typeId: type }),
                    success: function (data, status, jqXhr) {
                        if (typeof data !== "undefined" && data !== null && typeof data.d !== "undefined" && data.d !== null) {
                            $("#" + gridPanel).html(data.d[1]);
                            SEL.Grid.updateGrid(data.d[0]);
                            $("#" + searchPanel).data("contextcontrolid", elementObject.id);
                            $("#" + gridPanel + " .autocompletecombo-selectlink").removeAttr("href");
                            $f(modalId).show();
                        }
                        else {
                            SEL.Common.WebService.ErrorHandler(data);
                        }
                    },
                    error: function (data, status, jqXhr) { SEL.Common.WebService.ErrorHandler(data); }
                });
            },

            Search: function (elementObject) {
                SEL.AutoCompleteCombo.Grid(elementObject);
            },

            SearchChoice: function (elementObject, value, text) {
                var searchContainer = $(elementObject).closest(".autocompletecombo-search"),
                    contextControl = null;

                if (searchContainer.length !== 1) {
                    return;
                }

                contextControl = $("#" + searchContainer.data("contextcontrolid"));

                if (contextControl.length !== 1) {
                    return;
                }

                if (typeof value !== "undefined" && value !== null && typeof text !== "undefined" && text !== null) {
                    contextControl.siblings(".autocompletecombo-text").val(text);
                    contextControl.siblings(".autocompletecombo-id").val(value);
                }

                $f(contextControl.closest(".autocompletecombo-container").data("modal")).hide();
            },

            SelectChange: function (elementObject) {
                var selectControl = $(elementObject),
                    idControl = selectControl.siblings(".autocompletecombo-id");
                if (selectControl.length !== 1 || idControl.length !== 1) {
                    return;
                }

                if (selectControl.val() === "" || selectControl.val() === "0") {
                    idControl.val("");
                }
                else {
                    idControl.val(selectControl.val());
                }
            },

            Bind: function (cntl,
                maxRows,
                matchTableId,
                displayField,
                matchFieldIDs,
                autoCompleteFieldIDs,
                fieldFilters,
                msDelay,
                triggerFields,
                keyIsString,
                childFilterList,
                displayAutocompleteMultipleResultsFields) {

                var paramList, serviceMethod;

                var triggerParameter = new SEL.AutoCompleteCombo.JsonObjects.AutoCompleteDropdownBindParameter();
                triggerParameter.controlname = cntl.toString().substring(cntl.indexOf("txt"), cntl.lastIndexOf("_")).replace(/[^0-9]/gi, '');
                triggerParameter.BindMatchTableId = matchTableId;
                triggerParameter.BindtriggerFields = triggerFields;
                SEL.AutoCompleteCombo.AutoCompleteDropdownBindParameterList.BindMatchParameterList.push(triggerParameter);

                if (autoCompleteFieldIDs === "null" || autoCompleteFieldIDs === null) {
                    displayAutocompleteMultipleResultsFields = "False";
                }
                if (SEL.AutoComplete.Bind.childFilterList == undefined) {
                    SEL.AutoComplete.Bind.childFilterList = null;
                }
                if (displayAutocompleteMultipleResultsFields === "False") {
                    paramList = "{ maxRows: " +
                        maxRows +
                        ", matchTable: \"" +
                        matchTableId +
                        "\", displayField: \"" +
                        displayField +
                        "\", matchFields: \"" +
                        matchFieldIDs.split(',') +
                        "\", matchText: \"" +
                        request.term +
                        "\", useWildcards: true, filters: " +
                        fieldFilters +
                        ", keyIsString: '" +
                        keyIsString +
                        "' ,childFilterList: " +
                        SEL.AutoComplete.Bind.childFilterList + " }";
                    serviceMethod = "/shared/webServices/svcAutoComplete.asmx/getAutoCompleteOptions";
                } else {
                    paramList = "{ maxRows: " +
                        maxRows +
                        ", matchTable: \"" +
                        matchTableId +
                        "\", displayField: \"" +
                        displayField +
                        "\", matchFields: \"" +
                        matchFieldIDs.split(',') +
                        "\",autoCompleteFields: \"" +
                        autoCompleteFieldIDs.split(',') +
                        "\", matchText: '', useWildcards: true, filters: " +
                        fieldFilters +
                        ", keyIsString: '" +
                        keyIsString +
                        "',childFilterList: " +
                        SEL.AutoComplete.Bind.childFilterList + " }";
                    serviceMethod = "/shared/webServices/svcAutoComplete.asmx/GetGreenlightAutoCompleteOptions";
                }
                $.ajax({
                    type: "POST",
                    url: appPath + serviceMethod,
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    data: paramList,
                    success: function (data) {
                        var noneOption = "<option title='None'  value='0'>[None]</option>";
                        $('#' + cntl).append(noneOption);
                        $.each(data.d,
                            function (key, value) {
                                var newOption = "<option title='displayField'  value='" +
                                    value.value +
                                    "',1>" +
                                    value.label +
                                    "</option>";
                                var newOption2 =
                                    "<option title='formatted' value='" +
                                        value.value +
                                        "',2>" +
                                        value.formattedText +
                                        "</option>";

                                $('#' + cntl).append(newOption);
                                $('#' + cntl).append(newOption2);

                            });
                        var controlid = cntl.toString().substring(cntl.indexOf("txt"), cntl.lastIndexOf("_")).replace(/[^0-9]/gi, '');
                        SEL.AutoCompleteCombo.ulSelectFormat($('#' + cntl), controlid);
                    },
                    error: function (data) {
                        alert("Error");
                    }
                });
            },

            AutocompleteComboSelect: function (selectedValue, matchtable, triggerFields) {
                if (triggerFields !== null) {
                    SEL.AutoComplete.TriggerFields.Update(matchtable, selectedValue, triggerFields);
                }
                return false;
            },
            ulSelectFormat: function ($select, controlid) {
                var items = '';
                $select
                    .each(function () {

                        // Cache the number of options
                        var $this = $(this);

                        // Hides the select element
                        $this.addClass('s-hidden');
                        var ifElementExists = false;
                        if ($select.parent().hasClass("AutoComboselect")) {
                            ifElementExists = true;
                        }
                        if (!ifElementExists) {
                            // Wrap the select element in a div
                            $this.wrap('<div class="AutoComboselect"></div>');
                        } else {
                            $select.parent().remove(".AutoCombostyledSelect");
                            $select.parent().remove(".AutoCombooptions");
                        }

                        // Insert a styled div to sit over the top of the hidden select element
                        $this.after('<div class="AutoCombostyledSelect"></div>');
                        // Cache the styled div
                        var $styledSelect = $this.next('div.AutoCombostyledSelect');
                        // Show the first select option in the styled div
                        $styledSelect.text($this.children('option').eq(0).text());
                        //Dynamically adjust the height of a custom dropdown parent div
                        var dynamicHeight = $(".twocolumn:not(:has(>div.AutoCombostyledSelect))").height() + 1;
                        if (dynamicHeight === 1) {
                            dynamicHeight = 25;
                        }
                        $select.parents(".twocolumn").height(dynamicHeight);
                        // Insert an unordered list after the styled div and also cache the list
                        var $list = $('<ul />',
                                {
                                    'class': 'AutoCombooptions'
                                })
                            .insertAfter($styledSelect);
                        var $opts = $select.find('option');
                        // Insert a list item into the unordered list for each select option

                        $opts.each(function () {
                            var optionType = $(this).attr('title');
                            if (optionType == 'None') {
                                items += "<li value='" + $(this).val() + "'>" +
                                    "<span class='displayName'>" +
                                    $(this).text();
                            }
                            else if (optionType == 'displayField') {
                                items += "<li  value='" + $(this).val() + "'>" +
                                    "<span class='displayName'><strong>" +
                                    $(this).text() +
                                    "</strong></span></>";
                            }
                            else if (optionType == 'formatted') {
                                items += "<br><span class='autoCompleteSearchResults' value='" + $(this).val() + "'>" +
                                    $(this).text() +
                                    "</span></li>";
                            }
                        });

                        $(items).appendTo($list);

                        var $listItems = $list.children('li');

                        // Show the unordered list when the styled div is clicked (also hides it if the div is clicked again)
                        $styledSelect.click(function (e) {
                            e.stopPropagation();
                            $('div.AutoCombostyledSelect.AutoComboactive').not(this).each(function () {
                                $(this).removeClass('AutoComboactive').next('ul.AutoCombooptions').hide();
                            });

                            $(this).toggleClass('AutoComboactive').next('ul.AutoCombooptions').toggle();

                        });

                        // Hides the unordered list when a list item is clicked and updates the styled div to show the selected list item
                        // Updates the select element to have the value of the equivalent option and updates the hidden fields required by the drop down for saving the data of an entity.
                        $listItems.click(function (e) {
                            e.stopPropagation();
                            var labelText = $(this).find('span.displayName').text();

                            $styledSelect.text(SEL.AutoCompleteCombo.Ellipsis(labelText, 25)).attr('title', labelText);

                            $styledSelect.text($(this).find('span.displayName').text());
                            $this.val($(this).attr("value"));
                            $list.hide();
                            var referenceId = $this.prop("id");
                            var referenceHiddenFieldForDropdownValue = $this.parent().next("input");
                            var referenceHiddenFieldForDropdownId = $("#" + referenceHiddenFieldForDropdownValue.prop("id") + "_ID");
                            referenceHiddenFieldForDropdownId.val($(this).attr("value"));
                            referenceHiddenFieldForDropdownValue.val($("#" + referenceId + " option:selected").html());

                            var selectedValue = $(this).attr('value');
                            SEL.AutoCompleteCombo.AutoCompleteDropdownBindParameterList.BindMatchParameterList
                                .filter(function (newArray) {
                                    if (newArray.controlname === controlid) {
                                        SEL.AutoCompleteCombo
                                            .AutocompleteComboSelect(selectedValue,
                                                newArray.BindMatchTableId,
                                                newArray.BindtriggerFields);
                                    }
                                });

                            var form = new RegExp("[?&]" + 'formid'.replace(/[\[\]]/g, "\\$&") + "(=([^&#]*)|&|#|$)").exec(window.location.href);
                            if (form != null) {
                                var entity = new RegExp("[?&]" + 'entityid'.replace(/[\[\]]/g, "\\$&") + "(=([^&#]*)|&|#|$)").exec(window.location.href);
                                SEL.AutoComplete.TriggerFields.UpdateChild(controlid, selectedValue, decodeURIComponent(form[2].replace(/\+/g, " ")), decodeURIComponent(entity[2].replace(/\+/g, " ")));
                            }

                        });

                        // Hides the unordered list when clicking outside of it
                        $(document)
                            .click(function () {
                                $styledSelect.removeClass('AutoComboactive');
                                $list.hide();
                            });

                    });
            },
            Ellipsis: function (str, len) {
                if (len === undefined || len === null) {
                    len = 32;
                }
                if (str.length > len) {
                    return str.substring(0, len - 4) + '...';
                }
                return str;
            }
        }
    }

    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }

}(SEL, jQuery, $g, $f, $e));

var AutoCompleteSearches = {
    New: function (controlName, controlId, modalId, panelId) {
        var newSearch = null;
        if (!AutoCompleteSearches.hasOwnProperty(controlName)) {
            newSearch = new AutoCompleteSearches.Class(controlName, controlId, modalId, panelId);
            AutoCompleteSearches[controlName] = newSearch;
        }
        return newSearch;
    },
    Class: function (controlName, controlId, modalId, panelId) {
        this.name = controlName;
        this.selectors = {
            textbox: "#" + controlId,
            id: "#" + controlId + "_ID",
            select: "#" + controlId + "Select",
            icon: "#" + controlId + "SearchIcon",
            grid: "#" + panelId + ">div.searchgrid",
            panel: "#" + panelId,
            modal: modalId
        };
    }
};

AutoCompleteSearches.Class.prototype.Grid = function () {
    var containerSelector = this.selectors.grid,
        modalSelector = this.selectors.modal,
        panelSelector = this.selectors.panel,
        _this = this;

    $.ajax({
        dataType: "json",
        contentType: "application/json",
        url: appPath + "/shared/webServices/svcAutoComplete.asmx/Get" + this.name.replace(/[^a-zA-Z]/gi, "") + "SearchGrid",
        method: "post",
        success: function (data, status, jqXhr) {
            if (typeof data !== "undefined" && data !== null && typeof data.d !== "undefined" && data.d !== null) {
                $(containerSelector).html(data.d[1] + "<script>SEL.Grid.updateGrid('" + data.d[0] + "');</script>");
                $f(modalSelector).show();

                $(document).on("keydown.autocompletesearch", function (e) {
                    if (e.keyCode === $.ui.keyCode.ESCAPE) {
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

AutoCompleteSearches.Class.prototype.Search = function () {
    this.Grid();
};

AutoCompleteSearches.Class.prototype.SearchChoice = function (value, text) {
    if (typeof value !== "undefined" && value !== null && typeof text !== "undefined" && text !== null) {
        $(this.selectors.textbox).val(text);
        $(this.selectors.id).val(value);
    }
    this.CloseSearch();
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
};

AutoCompleteSearches.Class.prototype.CloseSearch = function () {
    $f(this.selectors.modal).hide();
    $(document).off("keydown.autocompletesearch");
};
