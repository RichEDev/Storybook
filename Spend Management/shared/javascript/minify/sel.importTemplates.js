(function()
{
    var scriptName = "ImportTemplates";

    function execute()
    {
        SEL.registerNamespace("SEL.ImportTemplates");
        SEL.ImportTemplates =
        {
            CurrentTemplateID: null,
            TemplateNameDomID: null,
            ApplicationTypeDomID: null,
            TrustDomID: null,
            TabsDomID: null,
            CurrentFieldsDropdownDomID: null,
            CurrentTableSelectionID: null,
            CurrentDefaultTableName: null,
            CurrentInitialMappingFieldID: null,
            CurrentDefaultMappingFieldID: null,

            SaveImportTemplateMappings: function()
            {
                if (SEL.Common.ValidateForm() === false) { return; }
    
                var templateName = $g(SEL.ImportTemplates.TemplateNameDomID).value;
                var applicationType = parseInt($g(SEL.ImportTemplates.ApplicationTypeDomID).value, 10);
                var automated = 1; //(document.getElementById(IsAutomatedID).value == "on") ? 1 : 0;
                var trust = parseInt($g(SEL.ImportTemplates.TrustDomID).value, 10);
                var tabs = $('#' + SEL.ImportTemplates.TabsDomID).find('table'); //.getElementsByTagName('table');
                var maps = Array();
                var duplicatesMapped = false;

                tabs.each(function(index, tabObj) {
                    var elementname = tabObj.id;
                    var tmpArr = elementname.split("_");
                    elementname = tmpArr[tmpArr.length - 1];

                    var mapsGroup = [];

                    for (var i = 1; i < tabObj.rows.length; i++)
                    {
                        var row = tabObj.rows[i];

                        if ($(row).hasClass("reverseMapping"))
                        {
                            continue;
                        }

                        var mapsFields = [];
                        var cell0 = row.cells[0];
                        var cell2 = row.cells[2];
                        var cell3 = row.cells[3];

                        var columnName = $(cell0).text();
                        var colRef = parseInt(cell0.childNodes[1].value, 10);
                        var fieldGuid = cell2.childNodes[0].value;
                        var selectedIndex = cell2.childNodes[0].selectedIndex;
                        var mand = cell0.childNodes[2].value;
                        var dtype = parseInt(cell0.childNodes[3].value, 10);
                        var lkuptableid = cell0.childNodes[4].value;
                        var mfieldid = cell0.childNodes[5].value;
                        var opk = cell0.childNodes[6].value;
                        var populated = cell0.childNodes[7].value;
                        var allowdynamicmapping = cell0.childNodes[8].value;
                        var importfield = cell3.childNodes[0].childNodes[0].checked;

                        if (!SEL.ImportTemplates.CheckForDuplicateFields(mapsGroup, fieldGuid))
                        {
                            SEL.MasterPopup.ShowMasterPopup('Database field "' + cell2.childNodes[0][selectedIndex].text + '" on ' + elementname + ' Tab has been mapped more than once.', 'Message from ' + moduleNameHTML);
                            duplicatesMapped = true;
                            return;
                        }

                        mapsFields.push(elementname);
                        mapsFields.push(colRef);
                        mapsFields.push(columnName);
                        mapsFields.push(fieldGuid);
                        mapsFields.push(mand);
                        mapsFields.push(dtype);
                        mapsFields.push(lkuptableid);
                        mapsFields.push(mfieldid);
                        mapsFields.push(opk);
                        mapsFields.push(populated);
                        mapsFields.push(allowdynamicmapping);
                        mapsFields.push(importfield);

                        if (fieldGuid !== "0")
                        {
                            mapsGroup.push(mapsFields);
                        }
                    }
                    maps.push(mapsGroup);
                });

                if (!duplicatesMapped)
                {
                    Spend_Management.svcImportTemplates.SaveImportTemplateMappings(SEL.ImportTemplates.CurrentTemplateID, templateName, applicationType, trust, automated, $(".ddlAssignmentSignOffOwner").val(), $(".ddlEmployeeLineManager").val(), maps, SEL.ImportTemplates.SaveImportTemplateMappingsComplete, SEL.ImportTemplates.errorMessage);
                }
                return;
            },

            SaveImportTemplateMappingsComplete: function(data)
            {
                if (data <= 0)
                {
                    SEL.MasterPopup.ShowMasterPopup("There was an error adding your template mapping.");
                }
                else
                {
                    window.location = appPath + "/shared/ImportsExports/importTemplates.aspx";
                }
            },

            CancelImportTemplateMapping: function()
            {
                window.location = appPath + "/shared/ImportsExports/importTemplates.aspx";
            },

            
            errorMessage: function(data)
            {
                if (typeof (data._message) !== 'undefined')
                {
                    SEL.MasterPopup.ShowMasterPopup(data._message);
                }
                else
                {
                    SEL.MasterPopup.ShowMasterPopup(data);
                }
            },

            ToggleFieldImportFlags: function(object, tabIdx)
            {
                if (typeof(object) === 'undefined' || object === null)
                {
                    SEL.MasterPopup.ShowMasterPopup("Invalid Object Reference");
                    return;
                }

                var status = object.checked;

                $(object).parentsUntil('table').find('input[type=checkbox]').each(function(index, o) {
                    o.checked = status;
                });
            },

            LoadFields: function(object, dataType, currentDefaultTableId, initialMapping, defaultMapping)
            {
                SEL.ImportTemplates.CurrentDefaultTableID = currentDefaultTableId;
                SEL.ImportTemplates.CurrentInitialMappingFieldID = initialMapping;
                SEL.ImportTemplates.CurrentDefaultMappingFieldID = defaultMapping;

                if (typeof (object) === 'undefined' || object === null) {
                    SEL.MasterPopup.ShowMasterPopup("Invalid Object Reference");
                    return;
                }

                var fieldsDropDown = $(object).parentsUntil('table').contents('td').eq(2).find('select');
                
                SEL.ImportTemplates.CurrentFieldsDropdownDomID = fieldsDropDown.attr('id');
                SEL.ImportTemplates.CurrentTableSelectionID = object.options[object.selectedIndex].value;
                
                if (object.value === "0")
                {
                    fieldsDropDown.empty();
                    fieldsDropDown.append($('<option></option>').attr("value", "0").text("[None]"));
                    fieldsDropDown.attr('disabled', 'disabled');
                    return;
                }

                var tableGuid = object.options[object.selectedIndex].value;
                Spend_Management.svcImportTemplates.GetTableFields(tableGuid, dataType, true, true, SEL.ImportTemplates.LoadFieldsComplete, SEL.ImportTemplates.errorMessage);
            },

            LoadFieldsComplete: function(result)
            {
                var dropdown = $('#' + SEL.ImportTemplates.CurrentFieldsDropdownDomID);
                
                dropdown.empty();
                dropdown.append($('<option></option>').attr("value", "0").text("[None]"));
                $.each(result, function () {
                    dropdown.append($('<option></option>').attr("value", this.FieldID).text(this.Description));
                });

                if (SEL.ImportTemplates.CurrentTableSelectionID == SEL.ImportTemplates.CurrentDefaultTableID)
                {
                    dropdown.attr('disabled', 'disabled');
                }
                else
                {
                    dropdown.removeAttr('disabled');
                }
                SEL.ImportTemplates.SetSelectedItem(dropdown[0], SEL.ImportTemplates.CurrentInitialMappingFieldID, SEL.ImportTemplates.CurrentDefaultMappingFieldID);
            },

            CheckForDuplicateFields: function(mapsGroup, newFieldGuid)
            {
                var result = true;
                for (var i = 0; i < mapsGroup.length; i++)
                {
                    if (mapsGroup[i][3] === newFieldGuid)
                    {
                        result = false;
                        break;
                    }
                }

                return result;
            },

            SetSelectedItem: function(object, selectedId, defaultMappingId)
            {
                var selectedIdx = -1;
                var defaultIdx = 0;

                for (var ddlIdx = 0; ddlIdx < object.options.length; ddlIdx++)
                {
                    if (object.options[ddlIdx].value === selectedId)
                    {
                        selectedIdx = ddlIdx;
                    }

                    if (object.options[ddlIdx].value == defaultMappingId)
                    {
                        defaultIdx = ddlIdx;
                    }
                }

                object.options.selectedIndex = selectedIdx === -1 ? defaultIdx : selectedIdx;
            }            
        };
    }

    if (window.Sys && Sys.loader) {
        Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }

})();


    