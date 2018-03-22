var currentAction;
var selectedViewID;
var listItemEditMode = false;
var shallowSavePerformed = false;
var attributeEncrypted = false;
var editMode = false;

function addModalControlFocusOnShowOrHideEvent(modalid, controltofocus, showing) {
    if (showing){
        $f(modalid).add_shown(function () {
            $g(controltofocus).focus();
        });
    }
    else
    {
        $f(modalid).add_hiding(function () {
            try
            {
                $g(controltofocus).focus();
            }
            catch(e){} // IE6 may not have a control present to go back to at this point, due to the AJAX library modalpopupextender show/hide hiding selects for IE6
        });
    }
}

function closeOpenModal()
{
    var ns = SEL.CustomEntityAdministration,
        domIds = ns.DomIDs;

    if ($g(pnlMasterPopupid).style.display == '') {
        SEL.MasterPopup.HideMasterPopup();
        return;
    }
    if ($('#ui-datepicker-div').length != 0 && $('#ui-datepicker-div').css('display') != 'none'){
        $('#ui-datepicker-div').fadeOut(100);
        return;
    }
    if ($('#ui-timepicker-div').length != 0 && $('#ui-timepicker-div').css('display') != 'none') {
        $('#ui-timepicker-div').fadeOut(100);
        return;
    }
    if ($g(SEL.Trees.DomIDs.Filters.Panel).style.display == '')
    {
        $.filterModal.Filters.FilterModal.Cancel();
        return;
    }
    if ($g(domIds.Attributes.Relationship.NtoOne.Modal.Fields.MatchFieldSelector.Modal.Panel).style.display == '')
    {
        ns.Attributes.Relationship.ManyToOne.FieldItem.Modal.Close();        
        return;
    }
    if ($g(pnllistitemid).style.display == '') {
        hideListItemModal();
        return;
    }
    if ($g(pnltabid).style.display == '') {
        ns.Forms.HideTabModal();
        return;
    }
    if ($g(pnlsectionid).style.display == '') {
        ns.Forms.HideSectionModal();
        return;
    }    
    if ($g(pnlfieldid).style.display == '')
    {
        ns.Forms.HideFieldLabelModal();
        return;
    }
    if ($g(domIds.Attributes.Relationship.NtoOne.Modal.Panel).style.display == '')
    {
        //SEL.CustomEntityAdministration.Attributes.Relationship.CloseRelationship();
        SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.Modal.Close();
        return;
    }
    if ($g(domIds.Attributes.Relationship.OnetoN.Modal.Panel).style.display == '')
    {
        //SEL.CustomEntityAdministration.Attributes.Relationship.CloseRelationship();
        SEL.CustomEntityAdministration.Attributes.Relationship.OneToMany.Modal.Close();
        return;
    }
    if ($g(domIds.Attributes.Summary.Modal.DisplayFieldModal.Panel).style.display == '')
    {
        ns.Attributes.Summary.DisplayField.Modal.Hide();
        return;
    }
    if ($g(domIds.Attributes.Summary.Modal.Panel).style.display == '')
    {
        ns.Attributes.Summary.Modal.Hide();
        return;
    }
    if ($g(pnladdattributeid).style.display == '') {
        closeAttributeModal();
        return;
    }
    if ($("#" + domIds.Views.Modal.General.FormSelectionMappings.Panel + ":visible").length) {
        SEL.CustomEntityAdministration.Views.General.FormSelectionMappings.Cancel();
        return;
    }
    if ($g(pnlviewid).style.display == '') {
        $f(modviewid).hide();
        return;
    }
    if ($g('pnlDefaultText').style.display == '') {
        ns.Forms.HideDefaultTextModal();
        return;
    }
    if ($g(pnlactualformID).style.display == '') {
        ns.Forms.HideFormModal();
        return;
    }
    if ($g(pnlcopyformID).style.display == '')
    {
        ns.Forms.HideCopyModal();
        return;
    }
}

function FireDropdownValidatorWhenNoneSelected(ddID, reqID) {
    var dd = $g(ddID);
    var req = $g(reqID);
    req.isvalid = true;
    if (dd.selectedIndex === 0) {            
        req.isvalid = false;
    } 
    ValidatorUpdateDisplay(req);
}

function WebServiceError(ಠ_ಠ) {  // USE - SEL.Common.WebService.ErrorHandler
    SEL.MasterPopup.ShowMasterPopup(
        'An error has occurred processing your request.<span style="display:none;">' +
            (ಠ_ಠ['_message'] ? ಠ_ಠ['_message'] : ಠ_ಠ) + '</span>',
        'Message from ' + moduleNameHTML
    );
}

// validate the existence of at least one item in the attributes listbox
function ValidateListBox(sender, args) {
    var options = document.getElementById(listbox_tovalidate).options;
    if (options.length > 0) {
        args.IsValid = true;
    }
    else {
        args.IsValid = false;
    }
}

//########################entity###################################################

function CancelEntity() {
    document.location = "custom_entities.aspx";
}

function setCurrencyListState() {
    if ($g(chkEnableCurrenciesID).checked) {
        $g(ddlDefaultCurrencyID).disabled = false;
    }
    else {
        $g(ddlDefaultCurrencyID).disabled = true;
    }
}

function setMonetaryState() {
    //$g('divMoneyRow').style.display = '';
    if ($e(chkEnableCurrenciesID) === true && $e(ddlDefaultCurrencyID) === true) {
        var enableCurrenciesCntl = $g(chkEnableCurrenciesID);
        var defaultCurrencyListCntl = $g(ddlDefaultCurrencyID)  ;
        if (enableCurrenciesCntl !== null && defaultCurrencyListCntl !== null) {
            enableCurrenciesCntl.disabled = false;
            if (enableCurrenciesCntl.checked) {
                enableCurrenciesCntl.disabled = true;
                defaultCurrencyListCntl.disabled = false;
            }
            else {
                if (entityid !== 0) {
                    //$g('divMoneyRow').style.display = 'none';
                    enableCurrenciesCntl.disabled = true;    
                }
                defaultCurrencyListCntl.disabled = true;
            }

        }
    }
}

// Validate that a currency has been selected if the monetory record tickbox is ticked
function updateCurrenciesValidator(isFirstRun) {
    var currencyLabel = $('#' + lblDefaultCurrencyID);

    if ($g(chkEnableCurrenciesID).checked)
    {
        // Change the font to be emboldened and add an asterix
        currencyLabel.css('font-weight', 'bold');        
        currencyLabel.html(currencyLabel.html() + '*');
    }
    else
    {
        // Change the font from being emboldened and remove the asterisk
        currencyLabel.css('font-weight', '');
        if (isFirstRun === false) { currencyLabel.html(currencyLabel.html().substring(0, currencyLabel.html().length - 1)); }
        
        // Disable validation on Default currency field
        ValidatorEnable(document.getElementById(cvDefaultCurrencyID), false);
    }
}

//########################attributes###############################################

function ValidateAttributeDisplayName(sender, args) {
    // trim end and lower
    if ($g(txtattributenameid).value.replace(/^\s+/, '').toLowerCase().replace(' ', '') == 'greenlightcurrency')
    {
        args.IsValid = false;
    }
    else {
        args.IsValid = true;
    }
}

function showAttributeModal() 
{
    if (entityid == 0) 
    {
        currentAction = 'addAttribute';
        SEL.CustomEntityAdministration.Base.Save();
        shallowSavePerformed = true;
        return;
    }
    SEL.Common.ShowModal(modattributeid);
    $('#' + txtattributenameid).select();
}
function saveAttribute()
{
    EnableAttributeValidators('normal');
    if (validateform('vgAttribute') == false)
    {
        return;
    }
    var maxlength;
    var attibutename = $g(txtattributenameid).value;
    var description = $g(txtattributedescriptionid).value;
    var tooltip = $g(txtattributetooltipid).value;
    var mandatory = $g(chkattributemandatoryid).checked;
    var showInMobile = $g(chkDisplayInMobile).checked;
    var cmbtype = $g(cmbattributetypeid);
    var type = cmbtype.options[cmbtype.selectedIndex].value;
    var format = 1;
    var precision = 0;
    var defaultvalue = '';
    var listItems = null;
    var workflowid = 0;
    var CommentText = '';
    var auditidentifier = $g(auditidentifierid).checked;
    var isunique = $g(isuniqueid).checked;
    var encrypted = $g(encrypt).checked;
    var cmbDisplayWidth = $g(cmbDisplayWidthID);
    var display_width = cmbDisplayWidth.options[cmbDisplayWidth.selectedIndex].value;
    var boolAttribute = false;
    var builtIn = $("#" + chkattributebuiltinid).is(":checked");
    maxlength = null;

    var populateDefaults = false;

    switch (type)
    {
        case '1':
            if ($g(txtmaxlengthid).value != '')
            {
                maxlength = $g(txtmaxlengthid).value;
            }
        case '16':
            if ($g(txtmaxlengthlargeid).value != '')
            {
                maxlength = $g(txtmaxlengthlargeid).value;
            }
            format = $g(cmbtextformatid).options[$g(cmbtextformatid).selectedIndex].value;
            if (type == '1' && display_width == '2')
            {
                //single line wide
                format = '7';
            }
            break;
        case '4':
        case '17':
            var oList = $g(lstitemsid);
            var valueArray = $('#' + lstitemsid).data(lstitemsid);
            if (oList != null && $e('txtlistitems'))
            {
                var oItems = $g('txtlistitems');

                oItems.value = '';
                listItems = [];
                for (var i = 0; i < valueArray.length; i++)
                {
                    listItems[i] = JSON.stringify(valueArray[i]);
                }
            }

            if (type == '4')
            {
                if (display_width == '1')
                {
                    //list standard
                    format = '8';
                }
                else
                {
                    //list wide
                    format = '9';
                }
            }
            break;
        case '5':
            // tickbox
            defaultvalue = $g(cmbdefaultvalueid).options[$g(cmbdefaultvalueid).selectedIndex].value;

            if (shallowSavePerformed && attributeid === 0)
            {
                populateDefaults = true;
            }
            else
            {
                populateDefaults = (attributeid === 0 && isunique === false && confirm('Would you like to populate any existing records for this GreenLight with the default value of "' + defaultvalue + '" for this new attribute?'));
            }
            break;
        case '3':
// datetime
            format = $g(cmbdateformatid).options[$g(cmbdateformatid).selectedIndex].value;
            break;
        case '7':
            precision = $g(txtprecisionid).value;
            break;
        case '10':
            if ($g(txtmaxlengthlargeid).value != '') {
                maxlength = $g(txtmaxlengthlargeid).value;
            } else {
                boolAttribute = $('#' + chkRemoveFont)[0].checked;
            }
            format = $g(cmbtextformatlargeid).options[$g(cmbtextformatlargeid).selectedIndex].value;
            break;
        case '11':
            workflowid = $g(cmbattributeworkflowid).options[$g(cmbattributeworkflowid).selectedIndex].value;
            break;
        case '19':
            CommentText = $g(txtAdviceText).value;
            break;
        case '22':
            format = '1';
            boolAttribute = $('#chkImageLibrary')[0].checked;
            break;
        case '23':
            format = $g(cmbcontactformatid).options[$g(cmbcontactformatid).selectedIndex].value;
            break;
    }

    var saveFunction = function () {Spend_Management.svcCustomEntities.saveAttribute(CurrentUserInfo.AccountID, employeeid, entityid, attributeid, attibutename, description, tooltip, mandatory, type, maxlength, format, defaultvalue, precision, listItems, workflowid, CommentText, auditidentifier, isunique, populateDefaults, showInMobile, boolAttribute, builtIn, encrypted,
        function (data)
        {
            if (data[0] === -1)
            {
                SEL.MasterPopup.ShowMasterPopup('An attribute or relationship with this Display name already exists.', 'Message from ' + moduleNameHTML);
            }
            else
            {
                if (data[1] === '1')
                {
                    document.getElementById(hdnAuditAttributeID).value = data[0];
                    document.getElementById(hdnAuditAttributeDisplayNameID).value = data[2];
                }

                closeAttributeModal();
                Spend_Management.svcCustomEntities.getAttributesGrid(entityid, RefreshAttributeGridComplete);

                if ((type === "1" && (format === "1" || format === "7")) || type === "4") // text & single line or single line wide, or list
                {
                    SEL.CustomEntityAdministration.Base.AddFormSelectionAttribute(attibutename, data[0]);
                }

                // if the attribute is built-in/system the GreenLight will be too (if it wasn't already, it will be now), so update the checkbox for the GreenLight automatically.
                if (builtIn && !$("#ctl00_contentmain_chkBuiltIn").is(":checked")) {
                    $("#ctl00_contentmain_chkBuiltIn").prop("checked", true).prop("disabled", true);
                }
            }
        },
        WebServiceError
    );
    }

    if ( editMode && !attributeEncrypted && encrypted) {
        SEL.MasterPopup.ShowMasterConfirm("The 'Encrypt' option is irreversible. Once activated, all data stored for this attribute will be encrypted. Are you sure you want to continue?", 'Message from ' + moduleNameHTML, saveFunction, function () { });
    } else {
        saveFunction();
    }
}


function closeAttributeModal() {
    SEL.CustomEntityAdministration.Base.RemoveAllShortCuts();
    SEL.Common.HideModal(modattributeid);
    $('#divtooltip').css('display', '');
}

function RefreshAttributeGridComplete(data) {
    if ($e(pnlAttributeID) === true) {
        $g(pnlAttributeID).innerHTML = data[1];
        SEL.Grid.updateGrid(data[0]);
    }
    return;
}

function ConfirmAuditIdentifierChange() {
    var hdnAI = $g(hdnAuditAttributeID);
    var hdnAIDN = $g(hdnAuditAttributeDisplayNameID);
    var chkAudit = $g(auditidentifierid);

    if (chkAudit.checked) {
        if (hdnAI.value != attributeid && hdnAIDN.value != "id") {
            var answer = confirm('Are you sure you want to change the audit identifier for this GreenLight? The current identifier is: ' + hdnAIDN.value);
            if (answer) {
                chkAudit.disabled = true;
                return true;
            }
            else {
                return false;
            }
        }
    }
    return true;
}

function deleteAttribute(attributeid, fieldtype) {
    var auditid = $g(hdnAuditAttributeID).value;
    var auditname = $g(hdnAuditAttributeDisplayNameID).value;
    var message = "Are you sure you wish to delete the selected attribute?";
    // output additional information when fieldtype is NOT summary
    if(fieldtype !== 15) {
        message += "\n\nPlease note once this attribute has been deleted, ALL DATA entered by users for this attribute will be permanently deleted and cannot be retrieved.\n\nThis action is NOT reversible.";
    }
    if (attributeid == auditid) {
        message += " This attribute is currently the audit identifier, deleting this attribute will reset the identifier to the id attribute.";
    }
    if (confirm(message)) {
        Spend_Management.svcCustomEntities.deleteAttribute(attributeid, entityid, CurrentUserInfo.AccountID, deleteAttributeComplete, WebServiceError);
    }
}

function deleteAttributeComplete(data) {
    switch (data[2]) {
        case -1:
            SEL.MasterPopup.ShowMasterPopup('The attribute cannot be deleted as it is used as a display field or lookup display field in a n:1 relationship attribute.', 'Message from ' + moduleNameHTML);
            break;
        case -2:
            SEL.MasterPopup.ShowMasterPopup('The attribute cannot be deleted as it is used as a look-up field in a n:1 relationship attribute.', 'Message from ' + moduleNameHTML);
            break;
        case -4:
            SEL.MasterPopup.ShowMasterPopup('The attribute cannot be deleted as it is used as a display field in a user defined relationship.', 'Message from ' + moduleNameHTML);
            break;
        case -5:
            SEL.MasterPopup.ShowMasterPopup('The attribute cannot be deleted as it is used as a search field in a user defined relationship.', 'Message from ' + moduleNameHTML);
            break;
        case -6:
            SEL.MasterPopup.ShowMasterPopup('The attribute cannot be deleted as it is used as a search or display field.', 'Message from ' + moduleNameHTML);
            break;
        case -7:
            SEL.MasterPopup.ShowMasterPopup('The attribute cannot be deleted as it is used in a report as a column or filter.', 'Message from ' + moduleNameHTML);
            break;
        case -8:
            SEL.MasterPopup.ShowMasterPopup("The attribute cannot be deleted as it is used in a view's form selection mappings.", 'Message from ' + moduleNameHTML);
            break;
        case -9:
            SEL.MasterPopup.ShowMasterPopup('The attribute cannot be deleted as it is a system attribute.', 'Message from ' + moduleNameHTML);
            break;
        default:
            var auditcontrol = $g(hdnAuditAttributeID);
            var auditnamecontrol = $g(hdnAuditAttributeDisplayNameID);
            if (data[0] == auditcontrol.value) {
                auditcontrol.value = data[1];
                auditnamecontrol.value = "id";
            }

            SEL.CustomEntityAdministration.Base.DeleteFormSelectionAttribute(data[0]);
            
            Spend_Management.svcCustomEntities.getAttributesGrid(entityid, RefreshAttributeGridComplete, WebServiceError);
            break;
    }

    return;
}

function NewAttribute() {
    $g('divAttributeSectionHeader').innerHTML = "New Attribute";
    attributeid = 0;
    SEL.CustomEntityAdministration.IDs.Attribute = 0;
    clearAttributeForm();
    showAttributeModal();
    editMode = false;
    attributeEncrypted = false;
}

function editAttribute(attid, fieldtype, isParentFilter)
{
    SEL.CustomEntityAdministration.ParentFilter.IsParentFilter = isParentFilter;
    attributeid = attid;
    SEL.CustomEntityAdministration.IDs.Attribute = attid;
    
    switch (fieldtype) {
        case 4:
            if (SEL.CustomEntityAdministration.Attributes.IsFormSelectionAttribute(attid) && !confirm("This Attribute is currently assigned as a Form Selection Attribute.\n\nPlease note that any Form Selection Attribute mappings will be automatically deleted if you delete a related list item.")) {
                return;
            };
            getAttribute(attid);
            break;
        case 9:
            $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.General.RelationshipEntity).disabled = true;
            $g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.OnetoN.Modal.General.RelationshipEntity).disabled = true;
            SEL.CustomEntityAdministration.Attributes.Relationship.Get(attid, sessionStorage.getItem('formIdforN1'));
            break;
        case 15:
            SEL.CustomEntityAdministration.Attributes.Summary.Edit(attid);
            break;
        case 19:
            $('#divtooltip').css('display', 'none');
            getAttribute(attid);
            break;
        default:
            getAttribute(attid);
            break;
    }
}

function clearAttributeForm() {
    SEL.Common.Page_ClientValidateReset();
    clearBasicAttributeDetails();
    clearAdvancedAttributeDetails();
    showFurtherAttributeOptions();
    ToggleMaxLengthDropDown();
    ShowDisplayWidthOptions('text', false);
}

function clearBasicAttributeDetails() {
    $g(txtattributenameid).value = '';
    $g(txtattributedescriptionid).value = '';
    $g(txtattributetooltipid).value = '';
    $g(chkattributemandatoryid).checked = false;
    $g(chkDisplayInMobile).checked = false;
    $g(chkattributebuiltinid).checked = false;
    $g(chkattributebuiltinid).disabled = !CurrentUserInfo.AdminOverride;
    $g(cmbattributetypeid).disabled = false;
    var chkauditid = $g(auditidentifierid);
    chkauditid.checked = false;
    chkauditid.disabled = false;
    var chkunique = $g(isuniqueid);
    chkunique.checked = false;
    chkunique.disabled = false;
    $g(cmbattributetypeid).selectedIndex = 0;
    $g(chkattributemandatoryid).disabled = false;
    $('#' + txtmaxlengthid).css('display', '');
    $('#' + encrypt).prop('checked', false);
}

function clearAdvancedAttributeDetails() {
    var txtmaxlength = $g(txtmaxlengthid);
    txtmaxlength.value = '';
    txtmaxlength.disabled = false;
    var txtmaxlengthlarge = $g(txtmaxlengthlargeid);
    txtmaxlengthlarge.value = '';
    txtmaxlengthlarge.disabled = false;
    var txtprecision = $g(txtprecisionid);
    txtprecision.value = '';
    txtprecision.disabled = false;
    $g(cmbtextformatid).selectedIndex = 0;
    $g(cmbtextformatid).disabled = false;
    $g(cmbdefaultvalueid).selectedIndex = 0;
    $g(lstitemsid).options.length = 0;
    $g(txtAdviceText).value = '';
    $g(cmbDisplayWidthID).selectedIndex = 0;
    $g(txtAdviceText).value = '';
    var cmbdateformat = $g(cmbdateformatid);
    cmbdateformat.selectedIndex = 0;
    cmbdateformat.disabled = false;
    var cmblargetextformat = $g(cmbtextformatlargeid);
    cmblargetextformat.selectedIndex = 0;
    cmblargetextformat.disabled = false;
    var cmbcontactformat = $g(cmbcontactformatid);
    cmbcontactformat.selectedIndex = 0;
    cmbcontactformat.disabled = false;
    $('#chkImageLibrary')[0].checked = false;
    $('#' + chkRemoveFont)[0].checked = false;
}

function getAttribute(attid) {
    clearAttributeForm();
    Spend_Management.svcCustomEntities.getAttribute(CurrentUserInfo.AccountID, entityid, attid, getAttributeComplete);
}
function getAttributeComplete(attribute) {
    editMode = true;
    $g('divAttributeSectionHeader').innerHTML = 'Attribute: ' + attribute.displayname;
    $g(txtattributenameid).value = attribute.displayname;
    $g(txtattributedescriptionid).value = attribute.description;
    $g(txtattributetooltipid).value = attribute.tooltip;
    $g(chkattributemandatoryid).checked = attribute.mandatory;
    $g(chkDisplayInMobile).checked = attribute.DisplayInMobile;
    $('#' + chkattributebuiltinid).prop('checked', attribute.BuiltIn);
    $('#' + encrypt).prop('checked', attribute.Encrypted);

    $('#' + encrypt).attr('disabled', false);
    attributeEncrypted = attribute.Encrypted;

    if (attributeEncrypted) {
        
        $('#' + encrypt).attr('disabled', true);
    }

    if (CurrentUserInfo.AdminOverride) {
        $('#' + chkattributebuiltinid).prop('disabled', false);
    }

    if (attribute.BuiltIn) {
        $('#' + chkattributebuiltinid).prop('disabled', true);
    }

    var chkauditid = $g(auditidentifierid);
    chkauditid.checked = attribute.isauditidentifer;
    if (chkauditid.checked) {
        chkauditid.disabled = true;
    }
    else {
        chkauditid.disabled = false;
    }
    $g(isuniqueid).checked = attribute.isunique;

    var cmbtype = $g(cmbattributetypeid);
    cmbtype.disabled = true;

    for (var i = 0; i < cmbtype.options.length; i++) {
        if (cmbtype.options[i].value == attribute.fieldtype) {
            cmbtype.selectedIndex = i;
            showFurtherAttributeOptions();
            break;
        }
    }    

    switch (attribute.fieldtype) {
        case 1: //standard text
            var cmbtextformat = $g(cmbtextformatid);
            var cmbDisplayWidth = $g(cmbDisplayWidthID);
            if (attribute.format === SEL.CustomEntityAdministration.Forms.Format.MultiLine)
            {
                cmbtextformat.selectedIndex = 2;
            }
            else
            {
                cmbtextformat.selectedIndex = 1;
                if (attribute.format == SEL.CustomEntityAdministration.Forms.Format.SingleLineWide)
                {
                    cmbDisplayWidth.selectedIndex = 2;
                }
                else
                {
                    cmbDisplayWidth.selectedIndex = 1;
                }
            }
            ShowDisplayWidthOptions('text', false);
            if (attribute.maxlength != null)
            {
                $g(txtmaxlengthid).value = attribute.maxlength;
            }
            $g(txtmaxlengthid).disabled = true;
            cmbtextformat.disabled = true;
            break;
        case 3: // datetime
            var cmbdateformat = $g(cmbdateformatid);
            for (var i = 0; i < cmbdateformat.options.length; i++) {
                if (cmbdateformat.options[i].value == attribute.format) {
                    cmbdateformat.selectedIndex = i;
                    break;
                }
            }
            cmbdateformat.disabled = true;
            break;
        case 4: //list
        case 17: //list Wide
            if (attribute.fieldtype == 4) {
                var cmbDisplayWidth = $g(cmbDisplayWidthID);
                if (attribute.format == SEL.CustomEntityAdministration.Forms.Format.ListWide)
                {
                    cmbDisplayWidth.selectedIndex = 2;
                }
                else {
                    cmbDisplayWidth.selectedIndex = 1;
                }
            }
            Spend_Management.svcCustomEntities.getListItems(entityid, attribute.attributeid, getListItemsComplete, WebServiceError);
            break;
        case 5: //tickbox
            var cmbdefaultvalue = $g(cmbdefaultvalueid);
            for (var i = 0; i < cmbdefaultvalue.options.length; i++) {
                if (cmbdefaultvalue.options[i].value == attribute.defaultvalue) {
                    cmbdefaultvalue.selectedIndex = i;
                    break;
                }
            }
            break;
        case 7: //decimal
            var txtprecision = $g(txtprecisionid);
            if (attribute.precision != null) {
                txtprecision.value = attribute.precision;
            }
            txtprecision.disabled = true;
            ValidatorEnable($g(reqPrecision), false);
            break;
        case 10: //large text
            var cmblargetextformat = $g(cmbtextformatlargeid);
            for (var i = 0; i < cmblargetextformat.options.length; i++)
            {
                if (cmblargetextformat.options[i].value == attribute.format)
                {
                    cmblargetextformat.selectedIndex = i;
                    cmblargetextformat.disabled = true;
                    break;
                }
            }
            if (attribute.maxlength != null)
            {
                $g(txtmaxlengthlargeid).value = attribute.maxlength;
            }
            $g(txtmaxlengthlargeid).disabled = true;
            $('#' + chkRemoveFont)[0].checked = attribute.BoolAttribute;
            break;
        case 11: //workflow
            break;
        case 19:
            if (attribute.commentText !== '') {
                $g(txtAdviceText).value = attribute.commentText;
            }
            break;
        case 22:
            $('#chkImageLibrary')[0].checked = attribute.BoolAttribute;
            break;
        case 23:
            $("#" + cmbcontactformatid).val(attribute.format).prop("disabled", true);
            break;
    }

    ToggleMaxLengthDropDown(true);

    if (attribute.fieldtype != 19) {
        Spend_Management.svcCustomEntities.IsCustomEntityAttributeUniqueInInstances(entityid, attributeid, ConfirmAttributeInstancesUniqueComplete, UniqueError);
    }
    else {
        ConfirmAttributeInstancesUniqueComplete(false); 
    }
    
}

function ConfirmAttributeInstancesUniqueComplete(data) {
    var chkUnique = $g(isuniqueid);
    if (data == true) {
        chkUnique.disabled = false;
    }
    else {
        chkUnique.disabled = true;
    }
    showAttributeModal();
}

function UniqueError() {
    SEL.MasterPopup.ShowMasterPopup('An error has been encountered evaluating the uniqueness of this attribute in existing instances.', 'Message from ' + moduleNameHTML);
}

function getListItemsComplete(data) {
    var listItem = null;
    var valueArray = [];
    for (var i = 0; i < data.length; i = i + 1) {
        listItem = JSON.parse(data[i]);
        valueArray[i] = listItem;
    }

    $('#' + lstitemsid).data(lstitemsid, valueArray);
    showListItems(-1);
    ShowDisplayWidthOptions('list', false);
}

function showListItems(selectedIndex)
{
    var valueArray = $('#' + lstitemsid).data(lstitemsid);
    var listItemText = '';
    var listItem = null;
    var cmblst = $g(lstitemsid);
    cmblst.options.length = 0;

    for (var i = 0; i < valueArray.length; i++)
    {
        var option = document.createElement("OPTION");
        listItem = valueArray[i];
        listItemText = listItem.elementText;
        if (listItem.Archived) {
            listItemText = listItemText + ' (Archived)';
        }

        option.text = listItemText;
        option.value = listItem.elementValue;
        cmblst.options.add(option);
    }

    if (selectedIndex !== -1)
    {
        cmblst.selectedIndex = selectedIndex;
}
}

function showFurtherAttributeOptions(onChange) 
{
    SEL.CustomEntityAdministration.Base.RemoveAllShortCuts();
    
    if (onChange === undefined)
    {
        $g('divTextOptions').style.display = 'none';
        $g('divLargeTextOptions').style.display = 'none';
        $g('divDateOptions').style.display = 'none';
        $g('divTickboxOptions').style.display = 'none';
        $g('divListOptions').style.display = 'none';
        $g('divDecimalOptions').style.display = 'none';
        $g('divWorkflowOptions').style.display = 'none';
        $g('divAdviceText').style.display = 'none';
        $g('divDisplayWidthOptions').style.display = 'none';
        $g('audit_row').style.display = '';
        $g(chkattributemandatoryid).disabled = false;
        $g('divImageLibrary').style.display = 'none';
    }
        
    showGeneralFieldsOnAttributeModal();
    clearAdvancedAttributeDetails();
    DisableAttributeValidators();

}

function ToggleMaxLengthDropDown(onEdit)
{
    var cmbFormat = $g(cmbtextformatlargeid);
    var format = cmbFormat.options[cmbFormat.selectedIndex].text;

    if (format === "Formatted Text Box")
    {
        if (onEdit === true)
        {
            $('#maxLengthArea').css('display', 'none');
            $('.stripFont').fadeIn(200);
        }
        else
        {
            $('#maxLengthArea').fadeOut(200);
            $('.stripFont').fadeIn(200);
        }        
    }
    else
    {
        $('#maxLengthArea').fadeIn(200);
        $('.stripFont').css('display', 'none');
    }
}

function showGeneralFieldsOnAttributeModal()
{
    $g(chkattributemandatoryid).disabled = false;

    $("[attributeelement=slideDown]").each(function ()
    {
        if ($(this).css('display') === 'none')
        {
            $(this).slideDown(200);
        }
    });

    $("[attributeelement=slideUp]").each(function ()
    {
        if ($(this).css('display') !== 'none')
        {
            $(this).slideUp(200);
        }
    });

    if ($('div:animated').length !== 0)
    {
        setTimeout('showSpecificFieldsOnAttributeModal()', 400);
    }
    else
    {
        showSpecificFieldsOnAttributeModal();
    }
}

function showSpecificFieldsOnAttributeModal()
{
    var cmbtype = $g(cmbattributetypeid);
    var type = cmbtype.options[cmbtype.selectedIndex].value;

    if (type !== '19')
    {
        $('#divAdviceText').slideUp(200);
    }

    $('.encrypt').hide();

    switch (type)
    {
        case '':
            break;
        case '1':
            $('#divTextOptions').slideDown(200, function ()
            {
                ShowDisplayWidthOptions('text', false);
                $('.encrypt').show();
            });            
            break;
        case '4':
        case '17':
            $('#divListOptions').slideDown(200, function ()
            {
                ShowDisplayWidthOptions('list', false);
                SEL.CustomEntityAdministration.Base.AddShortCuts('listAttribute');
                $('#' + encrypt).attr('checked', false);
            });
            var valueArray = [];
            $('#' + lstitemsid).data(lstitemsid, valueArray);
            break;
        case '5':
            $('#divDisplayWidthOptions').slideUp(200);
            $('#divTickboxOptions').slideDown(200);
            $('#' + encrypt).attr('checked', false);
            break;
        case '3':
            $('#divDisplayWidthOptions').slideUp(200);
            $('#divDateOptions').slideDown(200);
            $('#' + encrypt).attr('checked', false);
            break;
        case '7':
            $('#divDecimalOptions').slideDown(200);
            $('#' + encrypt).attr('checked', false);
            break;
        case '10':                                   
            $('#divLargeTextOptions').slideDown(200);
            $('.encrypt').show();
            break;
        case '11':
            $('#divWorkflowOptions').slideDown(200);
            $('#' + encrypt).attr('checked', false);
            break;
        case '19':
            $('#divtooltip').css('position', 'inherit').slideUp(200);
            $('#audit_row').css('display', 'none');
            $('#divAdviceText').slideDown(200, function ()
            {
                var mandatory = $g(chkattributemandatoryid);
                mandatory.checked = false;
                mandatory.disabled = true;
                $g(auditidentifierid).checked = false;
                $g(auditidentifierid).disabled = false;
                $g(isuniqueid).checked = false;
                $('#' + encrypt).attr('checked', false);
            });
            break;
        case '22':
            $('#divtooltip').css('position', 'inherit').slideUp(200);
            $('#audit_row').css('display', 'none');     
            $g(auditidentifierid).checked = false;             
            $g(isuniqueid).checked = false;          
            $('#divImageLibrary').slideDown(200, function () {
                $('#' + encrypt).attr('checked', false);
            });
            break;
        case '23':
            $("#divContactOptions").css("position", "inherit").slideDown(200);
            $('.encrypt').show();
            break;
        default:
            break;
    }
}

function DisableAttributeValidators() {
    ValidatorEnable($g(cmpSingleLineMaxLength), false);
    ValidatorEnable($g(reqPrecision), false);
    ValidatorEnable($g(cmpPrecision1), false);
    ValidatorEnable($g(cmpPrecision2), false);
    ValidatorEnable($g(custAttListItem), false);
    //ValidatorEnable($g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFieldsReqValidator), false);
    ValidatorEnable($g(reqTextFormat), false);
    ValidatorEnable($g(reqDefaultYesNo), false);
    ValidatorEnable($g(reqDisplayWidth), false);
    ValidatorEnable($g(reqLargeTextFormat), false);
    ValidatorEnable($g(reqDateFormat), false);
    ValidatorEnable($g(reqComment), false);
}

function EnableAttributeValidators(mode) {
    if (mode == 'normal') {
        var cmbtype = $g(cmbattributetypeid);
        var type = cmbtype.options[cmbtype.selectedIndex].value;
        switch (type) {
            case '1':
                ValidatorEnable($g(reqTextFormat), true);
                var cmbtextformat = $g(cmbtextformatid);
                if (cmbtextformat.selectedIndex == 1) {
                    ValidatorEnable($g(reqDisplayWidth), true);
                    ValidatorEnable($g(cmpSingleLineMaxLength), true);
                }
                else {
                    ValidatorEnable($g(reqDisplayWidth), false);
                    ValidatorEnable($g(cmpSingleLineMaxLength), false);
                }
                break;
            case '5':
                ValidatorEnable($g(reqDefaultYesNo), true);
                break;
            case '4':
            case '17':
                ValidatorEnable($g(custAttListItem), true);
                ValidatorEnable($g(reqDisplayWidth));
                break;
            case '3':
                $g('divDateOptions').style.display = '';
                ValidatorEnable($g(reqDateFormat));
                break;
            case '7':
                ValidatorEnable($g(reqPrecision), true);
                ValidatorEnable($g(cmpPrecision1), true);
                ValidatorEnable($g(cmpPrecision2), true);
                break;
            case '10':
                ValidatorEnable($g(reqLargeTextFormat), true);
                break;
            case '19':
                ValidatorEnable($g(reqComment), true);
                break;
            case '23':
                ValidatorEnable($g(reqContactFormat), true);
                break;
            default:
                break;
        }
    }
//    else {
//        if (SEL.CustomEntityAdministration.IDs.Relationship.RelationshipType === SEL.CustomEntityAdministration.Forms.RelationshipType.OneToMany)
//        {
//            ValidatorEnable($g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFieldsReqValidator), false);
//            ValidatorEnable($g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MaxRowsCmpValidator), false);
//            ValidatorEnable($g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.DisplayFieldReqValidator), false);
//        }
//        else {
//            ValidatorEnable($g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.Modal.Fields.MatchFieldsReqValidator), true);
//            ValidatorEnable($g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.Modal.Fields.MaxRowsCmpValidator), true);
//            ValidatorEnable($g(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.Modal.Fields.DisplayFieldReqValidator), true);
//        }
//    }
}

function ShowDisplayWidthOptions(type, eventdriven) 
{
    if (eventdriven) 
    {
        FireDropdownValidatorWhenNoneSelected(cmbtextformatid, reqTextFormat);
    }

    $('#divDisplayWidthOptions').stop(true, true);

    switch (type) 
    {
        case 'text':
            var cmbtype = $g(cmbtextformatid, reqTextFormat);
            if (cmbtype.options[cmbtype.selectedIndex].value == '1')
            {
                $('#divDisplayWidthOptions').slideDown(200);
            }
            else if ($('#divDisplayWidthOptions').css('display') !== 'none')
            {
                $('#divDisplayWidthOptions').slideUp(200);
            }
            break;
        case 'list':
            $('#divDisplayWidthOptions').slideDown(200);
            break;
        default:
            break;
    }
    return;
}

function showListItemModal(bEditMode) {
    listItemEditMode = bEditMode;
    if (bEditMode) {
        SEL.Common.Page_ClientValidateReset();
    }
    else {
        $g('divListItem').innerHTML = 'New List Item';
        $g(txtlistitemid).value = '';
        $g(chkarchiveitemid).checked = false;
    }
    SEL.Common.ShowModal(modlistitemid);
    $('#' + txtlistitemid).select();
}

function hideListItemModal() 
{
    SEL.Common.Page_ClientValidateReset();
    SEL.Common.HideModal(modlistitemid);
}

function editListItem() {
    var lstItems = $g(lstitemsid);
    var valueArray = $('#' + lstitemsid).data(lstitemsid);
    
    if (lstItems.selectedIndex != -1) {
        var listItem = valueArray[lstItems.selectedIndex];
        $g(txtlistitemid).value = listItem.elementText;
        $g(chkarchiveitemid).checked = listItem.Archived;
        $g('divListItem').innerHTML = 'List Item: ' + listItem.elementText;
        showListItemModal(true);
    }
    else {
        var msg = 'Please select an item in the list to edit.';
        if (lstItems.length == 0) {
            msg = 'There are no items in the list to edit.';
        }
        SEL.MasterPopup.ShowMasterPopup(msg, 'Message from ' + moduleNameHTML);
    }
}

function EntityListItem ()
{
    this.elementValue = 0;
    this.elementText = '';
    this.elementOrder = 0;
    this.Archived = false;
}

function addListItem() {
    if (validateform('vgAttributeListItem') == false) {
        return false;
    }

    var valueArray = $('#' + lstitemsid).data(lstitemsid);

    if (typeof(valueArray) === 'undefined')
    {
        valueArray = [];
    }

    var lstitem = $g(txtlistitemid).value;
    var archived = $g(chkarchiveitemid).checked;
    var selectedIndex = $g(lstitemsid).selectedIndex;
    //check option does not already exist.
    var bExists = false;
    var options = $g(lstitemsid).options;
    for (i = 0; i < options.length; i++) {
        var currentListItem = valueArray[i]; 
        if (currentListItem.elementText === lstitem) {
            bExists = true;
            break;
        }
    }
    if (listItemEditMode) {
        var currentItem = valueArray[selectedIndex];
        if (bExists && lstitem != currentItem.elementText) {
            SEL.MasterPopup.ShowMasterPopup('This value is already in the list.', 'Message from ' + moduleNameHTML);
            return;
        } else {
            currentItem.elementText = lstitem;
            currentItem.Archived = archived;
            if (archived)
        {
                lstitem = lstitem + ' (Archived)';
            }
            
            valueArray[selectedIndex] = currentItem;
            $('#' + lstitemsid).data(lstitemsid, valueArray);
            var req = $g(custAttListItem);
            req.isvalid = true;
            ValidatorUpdateDisplay(req);
            hideListItemModal();
            showListItems(selectedIndex);
        }
    }
    else {
        if (!bExists) {
            var listItem = new EntityListItem();
            listItem.elementText = lstitem;
            listItem.Archived = archived;
            listItem.elementValue = 0;
            valueArray.push(listItem);
            $('#' + lstitemsid).data(lstitemsid, valueArray);
            var req = $g(custAttListItem);
            req.isvalid = true;
            ValidatorUpdateDisplay(req);          
            hideListItemModal();
            showListItems(-1);
        } else {
            SEL.MasterPopup.ShowMasterPopup('This value is already in the list.', 'Message from ' + moduleNameHTML);
        }
    }
    return;
}

function removeListItem() {
    var lstItems = $g(lstitemsid);
    if (lstItems.selectedIndex != -1) {
        var valueArray = $('#' + lstitemsid).data(lstitemsid);
        var listItemID = valueArray[lstItems.selectedIndex].elementValue;

        if (listItemID === 0) {
            removeListItemComplete(0);
        }
        else {
            // Call the web service to make sure that the list item isn't being used anywhere
            Spend_Management.svcCustomEntities.CheckListItemIsNotUsedWithinFilter(entityid, attributeid, listItemID, removeListItemComplete);
        }
    }
    else {
        var msg = 'Please select an item in the list to remove.';
        if (lstItems.length == 0) {
            msg = 'There are no items in the list to remove.';
        }
        SEL.MasterPopup.ShowMasterPopup(msg, 'Message from ' + moduleNameHTML);
    }
}


function removeListItemComplete(data)
{
    if (data === 0)
    {
        var valueArray = $('#' + lstitemsid).data(lstitemsid);
        var lstItems = $g(lstitemsid);

        var newArray = valueArray.splice(lstItems.selectedIndex, 1);

        $('#' + lstitemsid).data(lstitemsid, valueArray);
        showListItems(-1);
    }
    else
    {
        SEL.MasterPopup.ShowMasterPopup('This list item cannot be removed as it is in use on a GreenLight View Filter.', 'Message from ' + moduleNameHTML);
    }
}


function SwitchValidator    (validatorid, isvalid) {
    validator = $g(validatorid);
    validator.isvalid = isvalid;
    ValidatorUpdateDisplay(validator);
    return;
}

function ValidateButtonText(sender, args) {
    var txt1 = $g(txtformsavebuttontextid);
    var txt2 = $g(txtformsaveandduplicatebuttontextid);
    var txt3 = $g(txtformsaveandstaybuttontextid);
    var txt4 = $g(txtformcancelbuttontextid);
    var txt5 = $g(txtformsaveandnewbuttontextid);

    args.IsValid = (txt1.value.replace(' ', '') != "") || (txt2.value.replace(' ', '') != "") || (txt3.value.replace(' ', '') != "") || (txt4.value.replace(' ', '') != "") || (txt5.value.replace(' ', '') != "");

    var lblsavebuttontext = $g(lblsavebuttontextid);
    var lblsaveandduplicatebuttontext = $g(lblsaveandduplicatebuttontextid);
    var lblsaveandstaybuttontext = $g(lblsaveandstaybuttontextid);
    var lblsaveandnewbuttontext = $g(lblsaveandnewbuttontextid);
    var lblcancelbuttontext = $g(lblcancelbuttontextid);
    
    if (args.IsValid) {
        lblsavebuttontext.className = '';
        lblsaveandduplicatebuttontext.className = '';
        lblsaveandstaybuttontext.className = '';
        lblsaveandnewbuttontext.className = '';
        lblcancelbuttontext.className = '';
        lblsavebuttontext.innerHTML = lblsavebuttontext.innerHTML.replace('*', '');
        lblsaveandduplicatebuttontext.innerHTML = lblsaveandduplicatebuttontext.innerHTML.replace('*', '');
        lblsaveandstaybuttontext.innerHTML = lblsaveandstaybuttontext.innerHTML.replace('*', '');
        lblsaveandnewbuttontext.innerHTML = lblsaveandnewbuttontext.innerHTML.replace('*', '');
        lblcancelbuttontext.innerHTML = lblcancelbuttontext.innerHTML.replace('*', '');
        
        SwitchValidator(cvTextSaveID, true);
        SwitchValidator(cvTextSaveAndDuplicateID, true);
        SwitchValidator(cvTextSaveAndStayID, true);
        SwitchValidator(cvTextCancelID, true);       
        SwitchValidator(cvTextSaveAndNewID, true);       
    } else {
        lblsavebuttontext.className = 'mandatory';
        lblsaveandduplicatebuttontext.className = 'mandatory';
        lblsaveandstaybuttontext.className = 'mandatory';
        lblsaveandnewbuttontext.className = 'mandatory';
        lblcancelbuttontext.className = 'mandatory';
        if (lblsavebuttontext.innerHTML.indexOf('*') <= 0) 
        {
            lblsavebuttontext.innerHTML = lblsavebuttontext.innerHTML + '*';
            lblsaveandduplicatebuttontext.innerHTML = lblsaveandduplicatebuttontext.innerHTML + '*';
            lblsaveandstaybuttontext.innerHTML = lblsaveandstaybuttontext.innerHTML + '*';
            lblsaveandnewbuttontext.innerHTML = lblsaveandnewbuttontext.innerHTML + '*';
            lblcancelbuttontext.innerHTML = lblcancelbuttontext.innerHTML + '*';
        }
        SwitchValidator(cvTextSaveID, false);
        SwitchValidator(cvTextSaveAndDuplicateID, false);
        SwitchValidator(cvTextSaveAndStayID, false);
        SwitchValidator(cvTextCancelID, false);      
        SwitchValidator(cvTextSaveAndNewID, false);      
    }
}

//######################### summary ###############################/


function deleteEntity(entityid) {
    currentRowID = entityid;
    if (confirm('Are you sure you wish to delete the selected GreenLight?\n\nPlease note once this GreenLight has been deleted, ALL RECORDS entered by users for this GreenLight will be permanently deleted and cannot be retrieved.\n\nThis action is NOT reversible.'))
    {
        Spend_Management.svcCustomEntities.DeleteEntity(CurrentUserInfo.AccountID, entityid,
            function (data) {
                if (data == 0) {
                    SEL.Grid.refreshGrid('gridEntities', SEL.Grid.getCurrentPageNum('gridEntities')); // deleteGridRow('gridEntities', currentRowID);
                }
                else if (data === -1) {
                    SEL.MasterPopup.ShowMasterPopup('The delete request was denied as a 1:n relationship to this GreenLight exists.', 'Message from ' + moduleNameHTML);
                }
                else if (data === -2) {
                    SEL.MasterPopup.ShowMasterPopup('The delete request was aborted as the GreenLight could not be found.', 'Message from ' + moduleNameHTML);
                }
                else if (data === -3) {
                    SEL.MasterPopup.ShowMasterPopup('You do not have permission to delete GreenLights.', 'Message from ' + moduleNameHTML);
                }
                else if (data === -4) {
                    SEL.MasterPopup.ShowMasterPopup('The delete request was denied as a n:1 relationship references this GreenLight.', 'Message from ' + moduleNameHTML);
                }
                else if (data === -5) {
                    SEL.MasterPopup.ShowMasterPopup('The delete request was denied as an attribute is used by a n:1 relationship, report or UDF.', 'Message from ' + moduleNameHTML);
                }
                else if (data === -6) {
                    SEL.MasterPopup.ShowMasterPopup('The delete request was denied as this GreenLight is a System GreenLight.', 'Message from ' + moduleNameHTML);
                }
            }, WebServiceError);

    }
    return;
}
