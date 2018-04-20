var curUserdefinedID;
var curUserdefinedEncrypted;
var currentRowID;

function deleteUserdefined(userdefineid) {
    currentRowID = userdefineid;
    if (confirm('Are you sure you wish to delete the selected user defined field?')) {
        
        PageMethods.deleteUserDefined(userdefineid, deleteUserDefinedComplete);
    }
}
function deleteUserDefinedComplete(data)
{
    switch (data)
    {
        case -1:

            SEL.MasterPopup.ShowMasterPopup("Userdefined field cannot be deleted as it is currently in use on a GreenLight View.", 'Message from ' + moduleNameHTML);
            break;
        case -2:
            SEL.MasterPopup.ShowMasterPopup("Userdefined field cannot be deleted as it is currently in use on a GreenLight View Filter.", 'Message from ' + moduleNameHTML);
            break;
        case -3:
            SEL.MasterPopup.ShowMasterPopup("Userdefined field cannot be deleted as it is in use as a GreenLight display field or lookup display field.", 'Message from ' + moduleNameHTML);
            break;
        case -4:
            SEL.MasterPopup.ShowMasterPopup("Userdefined field cannot be deleted as it is in use as a search field on a GreenLight relationship.", 'Message from ' + moduleNameHTML);
            break;
        case -5:
            SEL.MasterPopup.ShowMasterPopup("Userdefined field cannot be deleted as it is in use as a display field on a user defined relationship.", 'Message from ' + moduleNameHTML);
            break;
        case -6:
            SEL.MasterPopup.ShowMasterPopup("Userdefined field cannot be deleted as it is in use as a search field on a user defined relationship.", 'Message from ' + moduleNameHTML);
            break;
        case -7:
            SEL.MasterPopup.ShowMasterPopup("Userdefined field cannot be deleted as it is in use as a search or display field.", 'Message from ' + moduleNameHTML);
            break;
        case -8:
            $.ajax({            
                url: appPath + '/shared/webServices/svcUserdefined.asmx/GetUDFReports',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: "{ userDefinedId: " + currentRowID + "}",
                success: function (data) {
                    var html = "<div>\n";
                    html += '<div class=\"sectiontitle\">\n';
                    html += 'Message from ' + moduleNameHTML;
                    html += '</div>\n';
                    html += '<br> Userdefined field cannot be deleted as it is in use as a report column.<br><br>\n';
                    html += 'Please remove the user defined field from the following report(s) if you wish to delete the user defined field.<br><br>\n';
                    html += data.d[1];
                    html += '</div><br>\n';
                    $g('divUDFReports').innerHTML = html;
                    var modal = $find(modformid);
                    modal.show();
                    SEL.Grid.updateGrid(data.d[0]);
                    $(document).keydown(function (e) {
                        if (e.keyCode === 27) // esc
                        {
                            HideModal();
                        }
                    });
                },
                error: function (xmlHttpRequest, textStatus, errorThrown) {
                    SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                        'Message from ' + moduleNameHTML);
                }
            });
            break;
        case 1:
            SEL.Grid.refreshGrid('gridFields', SEL.Grid.getCurrentPageNum('gridFields'));
            break;
        default:
            SEL.MasterPopup.ShowMasterPopup("Userdefined field cannot be deleted as it is currently in use.", 'Message from ' + moduleNameHTML);
            break;
    }
}
function HideModal() {
   var modal = $find(modformid);
    modal.hide(); 
}
function getAvailableGroupings() {
    var cmb = document.getElementById(cmbappliesto);
    var tableid = cmb.options[cmb.selectedIndex].value;
    Spend_Management.svcUserdefined.getAvailableGroupings(tableid, getAvailableGroupingsComplete);
}

function getAvailableGroupingsComplete(data) {
    var cmb = document.getElementById(cmbgroup);
    cmb.options.length = 0;
    for (var i = 0; i < data.length; i++) {
        var option = document.createElement("OPTION");
        option.text = data[i].Text;
        option.value = data[i].Value;
        cmb.options.add(option);
    }
}
function showListItemModal()
{
    var oList = document.getElementById(lstitems);

    if (oList != null)
    {
        for (var i = 0; i < oList.options.length; i++)
        {
            oList.options[i].selected = false;
        }

        oList.selectedIndex = -1;

        var modal = $f(modlistitem);
        modal.show();
        $('#' + txtlistitem).select();
        ValidatorEnable($g(reqListItemText), true);
    }
}
function hideListItemModal()
{
    var modal = $find(modlistitem);
    modal.hide();
    document.getElementById(txtlistitem).value = "";
    document.getElementById(chkarchiveditem).checked = false;
    
    ValidatorEnable(document.getElementById(reqListItemText), false);
}
function editListItem()
{
    var oList = document.getElementById(lstitems);

    if (oList != null && oList.selectedIndex != -1)
    {
        var listItem = JSON.parse(oList.options[oList.selectedIndex].value);
        document.getElementById(txtlistitem).value = listItem.elementText;
        document.getElementById(chkarchiveditem).checked = listItem.Archived;
        var modal = $find(modlistitem);
        modal.show();
        ValidatorEnable(document.getElementById(reqListItemText), true);
    }
}
function populateListItems()
{
    var oList = document.getElementById(lstitems);

    if (oList != null)
    {
        var oItems = document.getElementById('txtlistitems');

        for (var i = 0; i < oList.options.length; i++)
        {
            oItems.value += oList.options[i].text + '^{#^' + oList.options[i].value + '^#}^';
        }
    }
}


function ValidateListItemText(source, arguments)
{
    if (arguments.Value.indexOf('^{#^') === -1 && arguments.Value.indexOf('^#}^') === -1)
    {
        arguments.IsValid = true;
    }
    else
    {
        arguments.IsValid = false;
    }
}

function EntityListItem() {
    this.elementValue = 0;
    this.elementText = '';
    this.elementOrder = 0;
    this.Archived = false;
}

function addListItem()
{
    if (validateform('vgAttributeListItem') === false) { return; }

    var lstitem = document.getElementById(txtlistitem).value;
    var archived = document.getElementById(chkarchiveditem).checked;
    var option = document.createElement("OPTION");
    var newListItem = new EntityListItem();
    var selectedIndex = document.getElementById(lstitems).selectedIndex;
    var existingMatches = 0;
    var i = 0;
    newListItem.Archived = archived;
    newListItem.elementText = lstitem;
    option.text = lstitem;
    option.value = JSON.stringify(newListItem);
    

    if (lstitem.replace(' ', '') === '') { return; }

    if (selectedIndex != -1) 
    {
        
        for (i = 0; i < document.getElementById(lstitems).options.length; i++)
        {
             if(document.getElementById(lstitems).options[i].text == lstitem && i !== selectedIndex)
            {
                existingMatches++;
            }
        }

        if (existingMatches === 0)
        {
            var currentListItem = JSON.parse(document.getElementById(lstitems).options[document.getElementById(lstitems).selectedIndex].value);
            currentListItem.elementText = lstitem;
            currentListItem.Archived = archived;
            if (currentListItem.Archived)
            {
                lstitem = lstitem + ' (Archived)';
            }
            
            document.getElementById(lstitems).options[document.getElementById(lstitems).selectedIndex].text = lstitem;
            document.getElementById(lstitems).options[document.getElementById(lstitems).selectedIndex].value = JSON.stringify(currentListItem);
        }
    }
    else 
    {
        for (i = 0; i < document.getElementById(lstitems).options.length; i++) 
        {
            if(document.getElementById(lstitems).options[i].text == lstitem)
            {
                existingMatches++;
            }
        }
        if (existingMatches === 0)
        {
            document.getElementById(lstitems).options.add(option);
        }
    }
    hideListItemModal();
    ValidatorEnable(document.getElementById(reqListItemText), false);
    recreateList(selectedIndex);
}

function recreateList(selectedIndex)
{
    var cmblst = $g(lstitems);
    var items = [];
    var listItemText = '';
    var listItem = null;
    for (i = 0; i < document.getElementById(lstitems).options.length; i++)
    {
        items.push(document.getElementById(lstitems).options[i].value);
    }

    cmblst.options.length = 0;
    for (var i = 0; i < items.length; i++)
    {
        var option = document.createElement("OPTION");
        listItem = JSON.parse(items[i]);
        listItemText = listItem.elementText;
        if (listItem.Archived) {
            listItemText = listItemText + ' (Archived)';
        }

        option.text = listItemText;
        option.value = items[i];
        cmblst.options.add(option);
    }
    
    if (selectedIndex !== -1) {
        cmblst.selectedIndex = selectedIndex;
    }
}

function removeListItem()
{
    var lstItems = $g(lstitems);
    if (lstItems != null && lstItems.selectedIndex >= 0)
    {
        var listItemID = JSON.parse(lstItems[lstItems.selectedIndex].value).elementValue;

        if (listItemID === 0)
        {
            removeListItemComplete(0);
        }
        else
        {
            // Call the web service to make sure that the list item isn't being used anywhere
            Spend_Management.svcUserdefined.CheckListItemIsNotUsedWithinFilter(curUserdefinedID, listItemID, removeListItemComplete);
        }
    }
}

function removeListItemComplete(data)
{
    if (data === 0)
    {
        var lstItems = $g(lstitems);
        if (lstItems != null && lstItems.selectedIndex >= 0)
        {
            lstItems.remove(lstItems.selectedIndex);
        }
    }
    else
    {
        SEL.MasterPopup.ShowMasterPopup('This list item cannot be removed as it is in use on a GreenLight View Filter.', 'Message from ' + moduleNameHTML);
    }

    recreateList(-1);
}

function moveUpListItem()
{
    var oList = document.getElementById(lstitems);
    if (oList !== null && oList.selectedIndex > 0)
    {
        var nSelected = oList.selectedIndex;
        // create a copy of the selected item (cloneNode won't work for this)
        var oOption = document.createElement('option');
        oOption.value = oList.options[nSelected].value;
        oOption.text = oList.options[nSelected].text;
        if (oOption !== null)
        {
            // remove the item from its existing position
            oList.remove(nSelected);
            // add the copy in before the item that used to be before it
            try
            {
                oList.add(oOption, oList.options[nSelected - 1]);
            }
            catch (ex)
            {
                oList.add(oOption, (nSelected - 1));
            }
            // make it easier to move multiple positions by reselecting the item that just moved
            oList.selectedIndex = nSelected - 1;
        }
    }

    recreateList(oList.selectedIndex);
}

function moveDownListItem()
{
    var oList = document.getElementById(lstitems);
    if (oList !== null && oList.selectedIndex >= 0 && oList.selectedIndex < (oList.length - 1))
    {
        var nSelected = oList.selectedIndex;
        // create a copy of the selected item (cloneNode won't work for this)
        var oOption = document.createElement('option');
        oOption.value = oList.options[nSelected].value;
        oOption.text = oList.options[nSelected].text;
        if (oOption !== null)
        {
            // remove the item from its existing position
            oList.remove(nSelected);
            // check to see if the item two positions after the selected one is off the end of the list
            // we've removed one at this point so the list is now shorter
            if ((nSelected + 1) < oList.length)
            {
                try
                {
                    oList.add(oOption, oList.options[nSelected + 1]);
                }
                catch (ex)
                {
                    oList.add(oOption, (nSelected + 1));
                }
            }
            else // if it is, move the new one to the end
            {
                try
                {
                    oList.add(oOption, null);
                }
                catch (ex)
                {
                    oList.add(oOption);
                }
            }
            // make it easier to move multiple positions by reselecting the item that just moved
            oList.selectedIndex = nSelected + 1;
        }
    }

    recreateList(oList.selectedIndex);
}

function showFurtherAttributeOptions() {
    document.getElementById('divTextOptions').style.display = 'none';
    document.getElementById('divLargeTextOptions').style.display = 'none';
    document.getElementById('divDateOptions').style.display = 'none';
    document.getElementById('divTickboxOptions').style.display = 'none';
    document.getElementById('divListOptions').style.display = 'none';
    document.getElementById('divDecimalOptions').style.display = 'none';
    document.getElementById('divHyperlinkOptions').style.display = 'none';
    document.getElementById('divRelationshipTextBoxOptions').style.display = 'none';
    $('#divEncryptOptions').hide();
    var cmbtype = document.getElementById(cmbattributetype);
    var type = cmbtype.options[cmbtype.selectedIndex].value;

    switch (type) {
        case '1':
            document.getElementById('divTextOptions').style.display = '';
            $('#divEncryptOptions').show();
            ValidatorEnable(document.getElementById(reqHyperlinkText), false);
            ValidatorEnable(document.getElementById(reqHyperlinkPath), false);
            ValidatorEnable(document.getElementById(revHyperlinkPath), false);
            ValidatorEnable(document.getElementById(cvRelatedTable), false);
            ValidatorEnable(document.getElementById(compdefaultvalue), false);
            ValidatorEnable(document.getElementById(compmaxlength), true);
            ValidatorEnable(document.getElementById(compprecision), false);
            ValidatorEnable($g(custmtomatchfieldsID), false);
            ValidatorEnable($g(cmpDisplayFieldID), false);
            ValidatorEnable($g(cmpMaxRowsID), false);
            disableMandatoryCheck(false);
            break;
        case '2':
            ValidatorEnable(document.getElementById(reqHyperlinkText), false);
            ValidatorEnable(document.getElementById(reqHyperlinkPath), false);
            ValidatorEnable(document.getElementById(revHyperlinkPath), false);
            ValidatorEnable(document.getElementById(cvRelatedTable), false);
            ValidatorEnable(document.getElementById(compdefaultvalue), false);
            ValidatorEnable(document.getElementById(compmaxlength), false);
            ValidatorEnable(document.getElementById(compprecision), false);
            ValidatorEnable($g(custmtomatchfieldsID), false);
            ValidatorEnable($g(cmpDisplayFieldID), false);
            ValidatorEnable($g(cmpMaxRowsID), false);
            disableMandatoryCheck(false);
            break;
        case '3':
            document.getElementById('divDateOptions').style.display = '';
            ValidatorEnable(document.getElementById(reqHyperlinkText), false);
            ValidatorEnable(document.getElementById(reqHyperlinkPath), false);
            ValidatorEnable(document.getElementById(revHyperlinkPath), false);
            ValidatorEnable(document.getElementById(cvRelatedTable), false);
            ValidatorEnable(document.getElementById(compdefaultvalue), false);
            ValidatorEnable(document.getElementById(compmaxlength), false);
            ValidatorEnable(document.getElementById(compprecision), false);
            ValidatorEnable($g(custmtomatchfieldsID), false);
            ValidatorEnable($g(cmpDisplayFieldID), false);
            ValidatorEnable($g(cmpMaxRowsID), false);
            disableMandatoryCheck(false);
            break;
        case '4':
            document.getElementById('divListOptions').style.display = '';
            ValidatorEnable(document.getElementById(reqHyperlinkText), false);
            ValidatorEnable(document.getElementById(reqHyperlinkPath), false);
            ValidatorEnable(document.getElementById(revHyperlinkPath), false);
            ValidatorEnable(document.getElementById(cvRelatedTable), false);
            ValidatorEnable(document.getElementById(compdefaultvalue), false);
            ValidatorEnable(document.getElementById(compmaxlength), false);
            ValidatorEnable(document.getElementById(compprecision), false);
            ValidatorEnable($g(custmtomatchfieldsID), false);
            ValidatorEnable($g(cmpDisplayFieldID), false);
            ValidatorEnable($g(cmpMaxRowsID), false);
            disableMandatoryCheck(false);
            break;
        case '5':
            document.getElementById('divTickboxOptions').style.display = '';
            ValidatorEnable(document.getElementById(reqHyperlinkText), false);
            ValidatorEnable(document.getElementById(reqHyperlinkPath), false);
            ValidatorEnable(document.getElementById(revHyperlinkPath), false);
            ValidatorEnable(document.getElementById(cvRelatedTable), false);
            ValidatorEnable(document.getElementById(compdefaultvalue), true);
            ValidatorEnable(document.getElementById(compmaxlength), false);
            ValidatorEnable(document.getElementById(compprecision), false);
            ValidatorEnable($g(custmtomatchfieldsID), false);
            ValidatorEnable($g(cmpDisplayFieldID), false);
            ValidatorEnable($g(cmpMaxRowsID), false);
            disableMandatoryCheck(true);
            break;
        case '6':
            ValidatorEnable(document.getElementById(reqHyperlinkText), false);
            ValidatorEnable(document.getElementById(reqHyperlinkPath), false);
            ValidatorEnable(document.getElementById(revHyperlinkPath), false);
            ValidatorEnable(document.getElementById(cvRelatedTable), false);
            ValidatorEnable(document.getElementById(compdefaultvalue), false);
            ValidatorEnable(document.getElementById(compmaxlength), false);
            ValidatorEnable(document.getElementById(compprecision), false);
            ValidatorEnable($g(custmtomatchfieldsID), false);
            ValidatorEnable($g(cmpDisplayFieldID), false);
            ValidatorEnable($g(cmpMaxRowsID), false);
            disableMandatoryCheck(false);
            break;
        case '7':
            document.getElementById('divDecimalOptions').style.display = '';
            ValidatorEnable(document.getElementById(reqHyperlinkText), false);
            ValidatorEnable(document.getElementById(reqHyperlinkPath), false);
            ValidatorEnable(document.getElementById(revHyperlinkPath), false);
            ValidatorEnable(document.getElementById(cvRelatedTable), false);
            ValidatorEnable(document.getElementById(compdefaultvalue), false);
            ValidatorEnable(document.getElementById(compmaxlength), false);
            ValidatorEnable(document.getElementById(compprecision), true);
            ValidatorEnable($g(custmtomatchfieldsID), false);
            ValidatorEnable($g(cmpDisplayFieldID), false);
            ValidatorEnable($g(cmpMaxRowsID), false);
            disableMandatoryCheck(false);
            break;
        case '8':
            document.getElementById('divHyperlinkOptions').style.display = '';
            ValidatorEnable(document.getElementById(reqHyperlinkText), true);
            ValidatorEnable(document.getElementById(reqHyperlinkPath), true);
            ValidatorEnable(document.getElementById(revHyperlinkPath), true);
            ValidatorEnable(document.getElementById(cvRelatedTable), false);
            ValidatorEnable(document.getElementById(compdefaultvalue), false);
            ValidatorEnable(document.getElementById(compmaxlength), false);
            ValidatorEnable(document.getElementById(compprecision), false);
            ValidatorEnable($g(custmtomatchfieldsID), false);
            ValidatorEnable($g(cmpDisplayFieldID), false);
            ValidatorEnable($g(cmpMaxRowsID), false);
            disableMandatoryCheck(true);
            break;
        case '10':
            document.getElementById('divLargeTextOptions').style.display = '';
            $('#divEncryptOptions').show();
            ValidatorEnable(document.getElementById(reqHyperlinkText), false);
            ValidatorEnable(document.getElementById(reqHyperlinkPath), false);
            ValidatorEnable(document.getElementById(revHyperlinkPath), false);
            ValidatorEnable(document.getElementById(cvRelatedTable), false);
            ValidatorEnable(document.getElementById(compdefaultvalue), false);
            ValidatorEnable(document.getElementById(compmaxlength), false);
            ValidatorEnable(document.getElementById(compprecision), false);
            ValidatorEnable($g(custmtomatchfieldsID), false);
            ValidatorEnable($g(cmpDisplayFieldID), false);
            ValidatorEnable($g(cmpMaxRowsID), false);
            disableMandatoryCheck(false);
            break;
        case '9':
            document.getElementById('divRelationshipTextBoxOptions').style.display = '';
            ValidatorEnable(document.getElementById(reqHyperlinkText), false);
            ValidatorEnable(document.getElementById(reqHyperlinkPath), false);
            ValidatorEnable(document.getElementById(revHyperlinkPath), false);
            ValidatorEnable(document.getElementById(cvRelatedTable), true);
            ValidatorEnable(document.getElementById(compdefaultvalue), false);
            ValidatorEnable(document.getElementById(compmaxlength), false);
            ValidatorEnable(document.getElementById(compprecision), false);
            ValidatorEnable($g(custmtomatchfieldsID), true);
            ValidatorEnable($g(cmpDisplayFieldID), true);
            ValidatorEnable($g(cmpMaxRowsID), true);
            disableMandatoryCheck(false);
            break;
        case '16':
            ValidatorEnable(document.getElementById(reqHyperlinkText), false);
            ValidatorEnable(document.getElementById(reqHyperlinkPath), false);
            ValidatorEnable(document.getElementById(revHyperlinkPath), false);
            ValidatorEnable(document.getElementById(cvRelatedTable), false);
            ValidatorEnable(document.getElementById(compdefaultvalue), false);
            ValidatorEnable(document.getElementById(compmaxlength), false);
            ValidatorEnable(document.getElementById(compprecision), false);
            ValidatorEnable($g(custmtomatchfieldsID), false);
            ValidatorEnable($g(cmpDisplayFieldID), false);
            ValidatorEnable($g(cmpMaxRowsID), false);
            disableMandatoryCheck(false);
            break;
    }
}

function setItemSpecificState() 
{
    var appliesto_cntl = document.getElementById(cmbappliesto);
    var itemspecific_cntl = document.getElementById(chkitemspecific);

    if (appliesto_cntl != null && itemspecific_cntl != null) 
    {
        if (appliesto_cntl.options[appliesto_cntl.selectedIndex].value == 'd70d9e5f-37e2-4025-9492-3bcf6aa746a8')
        {
            if (itemspecific_cntl.parentElement.tagName == 'SPAN' && itemspecific_cntl.parentElement.disabled == true)
            {
                itemspecific_cntl.parentElement.disabled = false;
                itemspecific_cntl.disabled = false;
            }
        }
        else
        {
            if (itemspecific_cntl.parentElement.tagName == 'SPAN' && itemspecific_cntl.parentElement.disabled == false)
            {
                itemspecific_cntl.parentElement.disabled = true;
                itemspecific_cntl.disabled = true;
                itemspecific_cntl.checked = false;
            }
        }
    }
}

function SetEmployeePopulationState()
{
    var appliesto_cntl = $g(cmbappliesto);
    var employeepopulate_cntl = $g('spanAllowClaimantPopulation');
    var employeepopulate_checkboxcntl = $g(chkallowclaimantpopulationID);

    if (appliesto_cntl !== null && employeepopulate_cntl !== null)
    {
        if (appliesto_cntl.options[appliesto_cntl.selectedIndex].value === 'a184192f-74b6-42f7-8fdb-6dcf04723cef') // cars table
        {
            employeepopulate_cntl.style.display = '';
        }
        else
        {
            employeepopulate_cntl.style.display = 'none';
            employeepopulate_checkboxcntl.checked = false;
        }
    }
}

function disableMandatoryCheck(disableMand)
{
    var chkmand = document.getElementById(chkattributemandatory);
    if (disableMand == false)
    {
        chkmand.parentElement.disabled = false;
        chkmand.disabled = false;
    }
    else
    {
        chkmand.parentElement.disabled = true;
        chkmand.disabled = true;
        chkmand.checked = false;
    }
    return;
}

function ClearRelatedTable() 
{
    var oRelatedTable = document.getElementById(ddlRelatedTableID);
    oRelatedTable[0].selected = true;
}

function FilterRelatedTables()
{
    var oAppliesTo = document.getElementById(cmbappliesto);
    var oRelatedTable = document.getElementById(ddlRelatedTableID);
    var i;

    if (oAppliesTo !== null && oRelatedTable !== null && oRelatedTable.options.length > 0)
    {
        var oSelectedAT = oAppliesTo.options[oAppliesTo.selectedIndex];
        var oRelatedTables = oRelatedTable.options;

        for (i = 0; i < oRelatedTables.length; i++)
        {
            // If the selected value is the currently selected Related Table, refresh the options
            if (oRelatedTables[i].value === oSelectedAT.value && oRelatedTables[i].selected) 
            {
                if (oRelatedTables[i].value.toUpperCase() !== '618DB425-F430-4660-9525-EBAB444ED754') 
                {
                    oRelatedTable[0].selected = true;
                } else 
                {
                    getRelationshipLookupOptions();    
                }
                ValidatorEnable($g(cvRelatedTable), true);
                
                if ($e(cmbmtodisplayfieldID) === true)
                {
                    var displayCntl = $g(cmbmtodisplayfieldID);
                    if (displayCntl !== null)
                    {
                        displayCntl.options.length = 0;
                        var option = document.createElement("OPTION");
                        option.text = '[None]';
                        option.value = '0';
                        displayCntl.options.add(option);
                        ValidatorEnable($g(cmpDisplayFieldID), true);
                    }
                }

                if ($e(udfMatchFieldsID) === true)
                {
                    var matchCntl = $g(udfMatchFieldsID);
                    if (matchCntl !== null)
                    {
                        matchCntl.options.length = 0;
                        ValidatorEnable($g(custmtomatchfieldsID), true);                                                
                    }
                }
            }

            // if an option matches the Applies To selection, disable it to prevent circular references - except employees which permitted as special case
            oRelatedTables[i].disabled = (oSelectedAT.value.toUpperCase() !== '618DB425-F430-4660-9525-EBAB444ED754' && oRelatedTables[i].value === oSelectedAT.value) ? true : false;            
        }
    }

    return;
}

function getItemsFromPanel(ugroupid)
{
    var userdefinedValues = [];
    var userdefinedField;
    var txtbox;
    var ddlst;

    if (window.lstUserdefined === undefined)
    {
        window.lstUserdefined = [];
    }
    
    for (var i = 0; i < lstUserdefined.length; i++) {
        if (lstUserdefined[i][3] == ugroupid)
        {
            switch (lstUserdefined[i][1]) {
                case 'Text':
                case 'LargeText':
                case 'DynamicHyperlink':
                case 'RelationshipTextbox':
                    txtbox = document.getElementById(lstUserdefined[i][2]);

                    userdefinedField = [];
                    userdefinedField.push(lstUserdefined[i][0]);

                    var htmlEditorControl = $get(txtbox.id + "_ctl02_ctl00");

                    if (htmlEditorControl !== null) //(txtbox.firstChild != null && txtbox.firstChild.className == "ajax__htmleditor_editor_container") <-- MW: this method doesn't work in Firefox
                    {
                        var editorDesignPanel = htmlEditorControl.contentWindow.document.body;

                        userdefinedField.push(editorDesignPanel.innerHTML);
                    }
                    else
                    {
                        userdefinedField.push(txtbox.value);
                    }
                    userdefinedValues.push(userdefinedField);
                    break;
                case 'Relationship':
                    txtbox = document.getElementById(lstUserdefined[i][2] + "_ID");
                    userdefinedField = [];
                    userdefinedField.push(lstUserdefined[i][0]);
                    userdefinedField.push(txtbox.value);
                    userdefinedValues.push(userdefinedField);
                    break;
                case 'Currency':
                case 'Number':
                case 'Integer':
                    txtbox = document.getElementById(lstUserdefined[i][2]);
                    //if (txtbox.value != '') {
                        userdefinedField = [];
                        userdefinedField.push(lstUserdefined[i][0]);
                        userdefinedField.push(new Number(txtbox.value));
                        userdefinedValues.push(userdefinedField);
                    //}
                    break;
                case 'DateTime':
                    txtbox = document.getElementById(lstUserdefined[i][2]);
                    //if (txtbox.value != '') {
                    userdefinedField = [];
                    userdefinedField.push(lstUserdefined[i][0]);
                    switch (lstUserdefined[i][4]) {
                        case "DateOnly":
                            if (txtbox.value == "") {
                                userdefinedField.push("");
                            } else {
                                userdefinedField.push(txtbox.value.substring(6, 10) + "/" + txtbox.value.substring(3, 5) + "/" + txtbox.value.substring(0, 2));
                            }
                            break;
                        case "TimeOnly":
                            if (txtbox.value == "") {
                                userdefinedField.push("");
                            } else {
                                userdefinedField.push("1900/01/01 " + txtbox.value);
                            }
                            break;
                        case "DateTime":
                            if (txtbox.value == "") {
                                userdefinedField.push("");
                            } else {
                                userdefinedField.push(txtbox.value.substring(6, 10) + "/" + txtbox.value.substring(3, 5) + "/" + txtbox.value.substring(0, 2) + " " + txtbox.value.substring(11, 16));
                            }
                            break;
                        default:
                            break;
                    }
                    userdefinedValues.push(userdefinedField);
                    //}
                    break;
                case 'List':
                case 'TickBox':
                    ddlst = document.getElementById(lstUserdefined[i][2]);
                    //if (ddlst.selectedIndex != -1) {
                        userdefinedField = [];
                        userdefinedField.push(lstUserdefined[i][0]);
                        userdefinedField.push(ddlst.options[ddlst.selectedIndex].value);
                        userdefinedValues.push(userdefinedField);
                    //}
                    break;
            }
        }
    }
    return userdefinedValues;
}

function checkUDFExistence()
{
    
}

function launchURL(textBoxID)
{
    if (textBoxID !== undefined && textBoxID !== null)
    {
        // check format of link and make sure not linking to our domains
        var localRegex = /sel\-expenses\.com|sel\-framework\.com/i;
        var linkFormatRegex = /^(https?|ftps?):\/\/(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?.*|^mailto:[a-z0-9!#$%&'*+\/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+\/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/i;

        var oTextBox = document.getElementById(textBoxID);
        if (oTextBox !== null)
        {
            // the link should not contain our domain - currently set via account.hostname in udf constructor
            // the link should match one of the standard http/https/ftp/ftps/mailto formats to be valid
            if (localRegex.test(oTextBox.value) || linkFormatRegex.test(oTextBox.value) === false)
            {
                SEL.MasterPopup.ShowMasterPopup("The link you attempted to visit is either not valid, or disallowed", "Invalid Link");
                return false;
            }

            // check they are aware of risk of launching link
            if (confirm("The link you are about to open is not part of this application and can not be controlled by this software. Please be sure the link is genuine before opening it.\n\nWould you like to continue to the linked resource?"))
            {
                // launch in new window
                window.open(oTextBox.value, "_blank");
            }
        }
    }
}

function ddlstgroup_OnChange() {
    var groupid = document.getElementById(cmbgroup).options[document.getElementById(cmbgroup).selectedIndex].value;
    document.getElementById(txtgroupid).value = groupid;
}

// ################### Relationship Display and Match Field functions #############
function getRelationshipLookupOptions() {
    if ($e(ddlRelatedTableID) === true) {
        var cmbrelationshiplist = $g(ddlRelatedTableID);

        Spend_Management.svcAutoComplete.getRelationshipLookupOptions(cmbrelationshiplist.options[cmbrelationshiplist.selectedIndex].value, [], getRelationshipLookupOptionsComplete, WebServiceError);
    }
}

function showMatchFieldModal() {
    var arrSelections = [];

    if ($e(udfMatchFieldsID) === true && $e(ddlRelatedTableID) === true) {
        var fielditemcntl = $g(udfMatchFieldsID);
        var cmbrelationshiplist = $g(ddlRelatedTableID);

        for (var idx = 0; idx < fielditemcntl.length; idx++) {
            arrSelections.push(fielditemcntl.options[idx].value);
        }
        Spend_Management.svcAutoComplete.getRelationshipLookupOptions(cmbrelationshiplist.options[cmbrelationshiplist.selectedIndex].value, arrSelections, showFieldItemModalComplete, WebServiceError);
    }
}

function showFieldItemModalComplete(data) {
    if ($e(cmbfielditemlistID) === true) {
        var fielditemlist = $g(cmbfielditemlistID);
        fielditemlist.options.length = 0;

        option = document.createElement("OPTION");
        option.text = '[None]';
        option.value = '0';
        fielditemlist.options.add(option);

        for (var idx = 0; idx < data.length; idx++) {
            option = document.createElement("OPTION");
            option.text = data[idx].Text;
            option.value = data[idx].Value;
            fielditemlist.options.add(option);
        }
    }
    SEL.Common.ShowModal(modmatchfieldID);
    return;
}

function removeFieldItem()
{
    if ($e(udfMatchFieldsID) === true)
    {
        var fielditemlist = $g(udfMatchFieldsID);

        if (fielditemlist.selectedIndex != -1)
        {
            fielditemlist.remove(fielditemlist.selectedIndex);

            if (fielditemlist.options.length === 0)
            {
                ValidatorEnable($g(custmtomatchfieldsID), true);
            }
        }
        else
        {
            SEL.MasterPopup.ShowMasterPopup('Please select a field item to remove.', 'Remove Field Item');
        }
    }
    return;
}

function closeFieldItemListModal()
{
    var matchCntl = $g(udfMatchFieldsID);
    if (matchCntl !== null && matchCntl.options.length > 0)
    {
        ValidatorEnable($g(custmtomatchfieldsID), false);
    }
    
    SEL.Common.HideModal(modmatchfieldID);
}

function saveFieldItem() {
    if (validateform('vgFieldItemList') === false)
        return false;

    var selection = '';

    if ($e(cmbfielditemlistID) === true) {
        var fielditemlist = $g(cmbfielditemlistID);
        selection = fielditemlist.options[fielditemlist.selectedIndex];
    }

    if ($e(udfMatchFieldsID) === true) {
        var matchCntl = $g(udfMatchFieldsID);
        if (matchCntl !== null) {
            var option = document.createElement('OPTION');
            option.text = selection.text;
            option.value = selection.value;
            matchCntl.options.add(option);
        }
    }

    closeFieldItemListModal();
    return;
}

function getRelationshipLookupOptionsComplete(data) {
    if (data === null)
        return;

    var idx;
    var option;

    if ($e(cmbmtodisplayfieldID) === true) {
        var displayCntl = $g(cmbmtodisplayfieldID);
        if (displayCntl !== null) {
            displayCntl.options.length = 0;

            option = document.createElement("OPTION");
            option.text = '[None]';
            option.value = '0';
            displayCntl.options.add(option);
            ValidatorEnable($g(cmpDisplayFieldID), true);

            for (idx = 0; idx < data.length; idx++) {
                option = document.createElement("OPTION");
                option.text = data[idx].Text;
                option.value = data[idx].Value;
                if (data[idx].Value == selectedDisplayField)
                    option.selected = true;

                displayCntl.options.add(option);
            }
        }
    }

    if ($e(udfMatchFieldsID) === true)
    {
        var matchCntl = $g(udfMatchFieldsID);
        if (matchCntl !== null)
        {
            matchCntl.options.length = 0;
            ValidatorEnable($g(custmtomatchfieldsID), true);
        }
    }

    return;
}

function getRelationshipMatchSelectionsComplete(data) {
    if ($e(udfMatchFieldsID) === true) {
        var matchCntl = $g(udfMatchFieldsID);
        if (matchCntl !== null) {
            matchCntl.options.length = 0;

            for (idx = 0; idx < data.length; idx++) {
                option = document.createElement("OPTION");
                option.text = data[idx].Text;
                option.value = data[idx].Value;
                matchCntl.options.add(option);
            }
        }
    }
}

function saveUserDefined() {
    if (validateform('vgAttribute') === false)
        return false;

    var cntl;
    var idx;
    var udfName = '';
    var appliesTo = null;
    var udfDesc = '';
    var udfOrder = 0;
    var udfItemSpecific = false;
    var udfGroup = 0;
    var udfTooltip = '';
    var udfType = 1;
    var udfRelatedTableID = null;
    var udfDisplayField = null;
    var udfMandatory = false;
    var udfAllowSearch = false;
    var udfMatchFields = null;
    var udfMaxLength = 0;
    var udfPrecision = 0;
    var udfListItems = [];
    var udfDefault = null;
    var udfFormat = 0;
    var udfHypLink = null;
    var udfHypPath = null;
    var udfMaxRows = 15;
    var udfAllowClaimantPopulation = false;

    if ($e(udfNameID) === true) {
        udfName = $g(udfNameID).value;
    }
    if ($e(cmbappliesto) === true) {
        var cntl = $g(cmbappliesto);
        appliesTo = cntl.options[cntl.selectedIndex].value;
    }
    if ($e(udfDescID) === true) {
        udfDesc = $g(udfDescID).value;
    }
    if ($e(udfOrderID) === true) {
        udfOrder = new Number($g(udfOrderID).value);
    }
    if ($e(chkitemspecific) === true) {
        udfItemSpecific = $g(chkitemspecific).checked;
    }
    if ($e(cmbgroup) === true) {
        cntl = $g(cmbgroup);
        udfGroup = new Number(cntl.options[cntl.selectedIndex].value);
    }
    if ($e(udfTooltipID) === true) {
        udfTooltip = $g(udfTooltipID).value;
    }
    if ($e(cmbattributetype) === true) {
        cntl = $g(cmbattributetype);
        udfType = cntl.options[cntl.selectedIndex].value;
    }
    if ($e(ddlRelatedTableID) === true) {
        cntl = $g(ddlRelatedTableID);
        udfRelatedTableID = cntl.options[cntl.selectedIndex].value;
    }
    if ($e(chkattributemandatory) === true) {
        udfMandatory = $g(chkattributemandatory).checked;
    }
    if ($e(udfAllowSearchID) === true) {
        udfAllowSearch = $g(udfAllowSearchID).checked;
    }
    if ($e(udfDefaultValueID) === true) {
        cntl = $g(udfDefaultValueID);
        udfDefault = cntl.options[cntl.selectedIndex].text;
    }
    if ($e(chkallowclaimantpopulationID) === true)
    {
        udfAllowClaimantPopulation = $g(chkallowclaimantpopulationID).checked;
    }

    switch (udfType) {
        case '1': // text normal
            if ($e(udfMaxLengthID) === true) {
                udfMaxLength = new Number($g(udfMaxLengthID).value);
            }
            if ($e(udfTextFormatID) == true) {
                cntl = $g(udfTextFormatID);
                udfFormat = new Number(cntl.options[cntl.selectedIndex].value);
            }
            break;
        case '3': // Date
            if ($e(udfDateFormatID) == true) {
                cntl = $g(udfDateFormatID);
                udfFormat = new Number(cntl.options[cntl.selectedIndex].value);
            }
            break;
        case '4': // list item
            var options = $g(lstitems).options;
            for (var i = 0; i < options.length; i++)
            {
                udfListItems.push(options[i].value);
            }

            break;
        case '7': // number
            if ($e(udfPrecisionID) === true) {
                udfPrecision = new Number($g(udfPrecisionID).value);
            }
            break;
        case '8': // hyperlink
            if ($e(udfHypTextID) === true) {
                udfHypLink = $g(udfHypTextID).value;
            }
            if ($e(udfHypPathID) === true) {
                udfHypPath = $g(udfHypPathID).value;
            }
            break;
        case '9': // relationship
            if ($e(udfDisplayFieldID) === true) {
                cntl = $g(udfDisplayFieldID);
                udfDisplayField = cntl.options[cntl.selectedIndex].value;
            }
            if ($e(udfMatchFieldsID) === true) {
                udfMatchFields = [];
                cntl = $g(udfMatchFieldsID);
                for (idx = 0; idx < cntl.options.length; idx++) {
                    udfMatchFields.push(cntl.options[idx].value);
                }
            }
            if ($e(udfmtomaxrowsID) === true) {
                cntl = $g(udfmtomaxrowsID);
                if (cntl.value == '' || cntl.value == '0') {
                    udfMaxRows = 15;
                }
                else {
                    udfMaxRows = new Number(cntl.value);
                }
            }
            break;
        case '10': // text large
            if ($e(udfMaxLengthLargeID) === true) {
                udfMaxLength = new Number($g(udfMaxLengthLargeID).value);
            }
            if ($e(udfTextLargeFormatID) == true) {
                cntl = $g(udfTextLargeFormatID);
                udfFormat = new Number(cntl.options[cntl.selectedIndex].value);
            }
            break;
        default:
            break;
    }
    
    var encrypted = $g(encrypt).checked;
    var saveFunction = function() {
        Spend_Management.svcUserdefined.saveUserDefinedField(curUserdefinedID,
            udfName,
            udfDesc,
            udfTooltip,
            new Number(udfType),
            udfGroup,
            udfOrder,
            udfFormat,
            udfItemSpecific,
            udfAllowSearch,
            appliesTo,
            udfHypLink,
            udfHypPath,
            udfRelatedTableID,
            udfMandatory,
            udfDisplayField,
            udfMatchFields,
            udfMaxLength,
            udfPrecision,
            udfListItems,
            udfDefault,
            udfMaxRows,
            udfAllowClaimantPopulation,
            encrypted,
            saveUDFComplete,
            WebServiceError);
    };

    if (curUserdefinedID > 0 && !curUserdefinedEncrypted && encrypted) {
        SEL.MasterPopup.ShowMasterConfirm("The 'Encrypt' option is irreversible. Once activated, all data stored for this attribute will be encrypted. Are you sure you want to continue?", 'Message from ' + moduleNameHTML, saveFunction, function () { }, 'encrypt');
    } else {
        saveFunction();
    }
}

function saveUDFComplete(data)
{
    if (data === -1) {
        // already exists
        SEL.MasterPopup.ShowMasterPopup('Userdefined field already exists', 'Save Field');
        showFurtherAttributeOptions();    
    }
    else {
        window.location.href = 'adminuserdefined.aspx';
    }
}

function checkMatchFields(sender, args)
{
    if ($e(udfMatchFieldsID) === true)
    {
        var options = $g(udfMatchFieldsID).options;
        if (options.length > 0)
        {
            args.IsValid = true;
        }
        else
        {
            args.IsValid = false;
        }
    }
    else
    {
        args.IsValid = true;
    }
}

function FilterFieldTypes()
{
    // if applies to is Expense Items, Claims or Self Registration, don't permit relationship type as these use old addItemsToPage method which doesn't support auto complete
    var oAppliesTo = document.getElementById(cmbappliesto);
    var oFieldType = document.getElementById(cmbattributetype);
    var oRelationshipTypeOption = null;
    for (var idx = 0; idx < oFieldType.options.length; idx++)
    {
        if (oFieldType.options[idx].value === '9')
        {
            oRelationshipTypeOption = oFieldType.options[idx];
            break;
        }
    }

    switch (oAppliesTo.value.toUpperCase())
    {
    case '0EFA50B5-DA7B-49C7-A9AA-1017D5F741D0': // claims
    case 'D70D9E5F-37E2-4025-9492-3BCF6AA746A8': // savedexpenses
    
        if (oRelationshipTypeOption !== null)
        {
            // remove Relationship option
            oFieldType.options.remove(idx);
        }
        break;
    default:
        if (oRelationshipTypeOption === null)
        {
            var option = document.createElement('OPTION');
            option.text = 'Relationship';
            option.value = '9';
            oFieldType.options.add(option);
        }
        break;
    }
}