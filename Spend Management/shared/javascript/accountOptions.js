var currentTabContainerId;
var currentActiveTabId;

function cFieldToDisplay()
{
    this.Fieldid = null;
    this.Code = null;
    this.Description = null;
    this.Display = null;
    this.Mandatory = null;
    this.Individual = null;
    this.Displaycc = null;
    this.Mandatorycc = null;
    this.Displaypc = null;
    this.Mandatorypc = null;
}

function showAddScreen(codeName)
{
    Spend_Management.svcAccountOptions.GetFieldToDisplay(codeName, showAddScreenCallBack);
    return;
}

function showAddScreenCallBack(data)
{
    $find(addScreenModalPopupID).show();

    var fieldToDisplay = new cFieldToDisplay();

    fieldToDisplay.Fieldid = data.fieldid;
    fieldToDisplay.Code = data.code;
    fieldToDisplay.Description = data.description;
    fieldToDisplay.Display = data.display;
    fieldToDisplay.Mandatory = data.mandatory;
    fieldToDisplay.Individual = data.individual;
    fieldToDisplay.Displaycc = data.displaycc;
    fieldToDisplay.Mandatorycc = data.mandatorycc;
    fieldToDisplay.Displaypc = data.displaypc;
    fieldToDisplay.Mandatorypc = data.mandatorypc;

    document.getElementById(addScreenFieldName).innerHTML = fieldToDisplay.Description;
    document.getElementById(addScreenDisplayAs).value = fieldToDisplay.Description;
    document.getElementById(addScreenDisplayFieldOnCash).checked = fieldToDisplay.Display;
    document.getElementById(addScreenDisplayOnItem).checked = fieldToDisplay.Individual;
    document.getElementById(addScreenMandatoryOnCash).checked = fieldToDisplay.Mandatory;
    document.getElementById(addScreenDisplayOnCC).checked = fieldToDisplay.Displaycc;
    document.getElementById(addScreenMandatoryOnCC).checked = fieldToDisplay.Mandatorycc;
    document.getElementById(addScreenDisplayOnPC).checked = fieldToDisplay.Displaypc;
    document.getElementById(addScreenMandatoryOnPC).checked = fieldToDisplay.Mandatorypc;
    document.getElementById(addScreenHiddenField).value = fieldToDisplay.Code;
    document.getElementById(addScreenHiddenFieldID).value = fieldToDisplay.Fieldid;
    return;
}

function saveAddScreen()
{
    var fieldToDisplay = new cFieldToDisplay();

    fieldToDisplay.Description = document.getElementById(addScreenDisplayAs).value;
    fieldToDisplay.Display = document.getElementById(addScreenDisplayFieldOnCash).checked;
    fieldToDisplay.Individual = document.getElementById(addScreenDisplayOnItem).checked;
    fieldToDisplay.Mandatory = document.getElementById(addScreenMandatoryOnCash).checked;
    fieldToDisplay.Displaycc = document.getElementById(addScreenDisplayOnCC).checked;
    fieldToDisplay.Mandatorycc = document.getElementById(addScreenMandatoryOnCC).checked;
    fieldToDisplay.Displaypc = document.getElementById(addScreenDisplayOnPC).checked;
    fieldToDisplay.Mandatorypc = document.getElementById(addScreenMandatoryOnPC).checked;
    fieldToDisplay.Code = document.getElementById(addScreenHiddenField).value;
    fieldToDisplay.Fieldid = document.getElementById(addScreenHiddenFieldID).value;

    Spend_Management.svcAccountOptions.SaveFieldToDisplay(fieldToDisplay, null, ajaxError);

    closeAddScreen();
    SEL.Grid.refreshGrid("addScreen", 0);
}

function ajaxError(data)
{
}

function closeAddScreen()
{
    $find(addScreenModalPopupID).hide();
}

function activateCarsOnAdd()
{
    var allowMilage = document.getElementById(allowMileage);
    var activateCar = document.getElementById(ActivateCarOnUserAdd);

    if (activateCar.checked == true)
    {
        allowMilage.checked = true
        allowMilage.disabled = true;
        document.getElementById(hdnAllowMileage).value = true;
    }
    else
    {
        allowMilage.disabled = false;
        allowMilage.checked = false;
        document.getElementById(hdnAllowMileage).value = false;
    }
}

function EnableUsersAddCar()
{
    var enabled = document.getElementById(allowUsersToAddCars);
    var allowMilage = document.getElementById(allowMileage);
    var activateCar = document.getElementById(ActivateCarOnUserAdd);
    if (enabled.checked == true)
    {
        allowMilage.disabled = false;
        activateCar.disabled = false;
        activateCar.checked = false;
        allowMilage.checked = false;
        document.getElementById(hdnAllowMileage).value = false;
    } else
    {
        allowMilage.disabled = true;
        activateCar.disabled = true;
        activateCar.checked = false;
        allowMilage.checked = false;
        document.getElementById(hdnAllowMileage).value = false;
    }
}

function EnableUserCanSpecifyVehicleStartDate() {
    
    var canSpecifyStartDate = document.getElementById(EmployeeSpecifyCarStartDate);
    var startDateMandatory = document.getElementById(EmployeeSpecifyCarStartDateMandatory);
   
    if (canSpecifyStartDate.checked == true) {
        startDateMandatory.disabled = false;
    }
    else {
        startDateMandatory.disabled = true;
        startDateMandatory.checked = false;
    }
}

function changeHiddenAllowMileageValue()
{
    var allowMilage = document.getElementById(allowMileage);

    if (allowMilage.checked)
    {
        document.getElementById(hdnAllowMileage).value = true;
    }
    else
    {
        document.getElementById(hdnAllowMileage).value = false;
    }
}

function SetupPasswordFields()
{
    var plength = document.getElementById(pConstraint).options[document.getElementById(pConstraint).selectedIndex].value;
    var span1 = document.getElementById(pLength1);
    var span2 = document.getElementById(pLength2);
    var label1 = document.getElementById(pLenLbl1);
    var label2 = document.getElementById(pLenLbl2);
    var reqValLength1 = document.getElementById(reqLength1);
    var compValLength1 = document.getElementById(compLength1);
    var reqValLength2 = document.getElementById(reqLength2);
    var compValLength2 = document.getElementById(compLength2);
    
    switch (parseInt(plength))
    {
        case 1:
            span1.style.display = "none";
            span2.style.display = "none";
            ValidatorEnable(document.getElementById(reqLength1), false);
            ValidatorEnable(document.getElementById(compLength1), false);
            ValidatorEnable(document.getElementById(compLength1Greater), false);
            ValidatorEnable(document.getElementById(compLength1LessThan), false);
            ValidatorEnable(document.getElementById(reqLength2), false);
            ValidatorEnable(document.getElementById(compLength2), false);
            ValidatorEnable(document.getElementById(compLength2Greater), false);
            ValidatorEnable(document.getElementById(compLength2LessThan), false);
            ValidatorEnable(document.getElementById(compMinLess), false);
            ValidatorEnable(document.getElementById(compMaxGreater), false);
            break;
        case 2:
            label1.innerHTML = 'Length';
            span1.style.display = "";
            ValidatorEnable(document.getElementById(reqLength1), true);
            ValidatorEnable(document.getElementById(compLength1), true);
            ValidatorEnable(document.getElementById(compLength1Greater), true);
            ValidatorEnable(document.getElementById(compLength1LessThan), true);
            span2.style.display = "none";
            ValidatorEnable(document.getElementById(reqLength2), false);
            ValidatorEnable(document.getElementById(compLength2), false);
            ValidatorEnable(document.getElementById(compLength2Greater), false);
            ValidatorEnable(document.getElementById(compLength2LessThan), false);
            ValidatorEnable(document.getElementById(compMinLess), false);
            ValidatorEnable(document.getElementById(compMaxGreater), false);
            break;
        case 3:
            label1.innerHTML = 'Length';
            span1.style.display = "";
            ValidatorEnable(document.getElementById(reqLength1), true);
            ValidatorEnable(document.getElementById(compLength1), true);
            ValidatorEnable(document.getElementById(compLength1Greater), true);
            ValidatorEnable(document.getElementById(compLength1LessThan), true);
            span2.style.display = "none";
            ValidatorEnable(document.getElementById(reqLength2), false);
            ValidatorEnable(document.getElementById(compLength2), false);
            ValidatorEnable(document.getElementById(compLength2Greater), false);
            ValidatorEnable(document.getElementById(compLength2LessThan), false);
            ValidatorEnable(document.getElementById(compMinLess), false);
            ValidatorEnable(document.getElementById(compMaxGreater), false);
            break;
        case 4:
            label1.innerHTML = 'Length';
            span1.style.display = "";
            ValidatorEnable(document.getElementById(reqLength1), true);
            ValidatorEnable(document.getElementById(compLength1), true);
            ValidatorEnable(document.getElementById(compLength1Greater), true);
            ValidatorEnable(document.getElementById(compLength1LessThan), true);
            span2.style.display = "none";
            ValidatorEnable(document.getElementById(reqLength2), false);
            ValidatorEnable(document.getElementById(compLength2), false);
            ValidatorEnable(document.getElementById(compLength2Greater), false);
            ValidatorEnable(document.getElementById(compLength2LessThan), false);
            ValidatorEnable(document.getElementById(compMinLess), false);
            ValidatorEnable(document.getElementById(compMaxGreater), false);
            break;
        case 5:
            label1.innerHTML = 'Minimum length';
            span1.style.display = "";
            span2.style.display = "";
            ValidatorEnable(document.getElementById(reqLength1), true);
            ValidatorEnable(document.getElementById(compLength1), true);
            ValidatorEnable(document.getElementById(compLength1Greater), true);
            ValidatorEnable(document.getElementById(compLength1LessThan), true);
            ValidatorEnable(document.getElementById(reqLength2), true);
            ValidatorEnable(document.getElementById(compLength2), true);
            ValidatorEnable(document.getElementById(compLength2Greater), true);
            ValidatorEnable(document.getElementById(compLength2LessThan), true);
            ValidatorEnable(document.getElementById(compMinLess), true);
            ValidatorEnable(document.getElementById(compMaxGreater), true);
            break;
    }
}

function showOdoDay()
{
    var chkOdo = document.getElementById(chkRecordOdometer);
    var optOdoLogon = document.getElementById(optRecordOdoOnLogon);

    if (chkOdo.checked && optOdoLogon.checked) 
    {
        document.getElementById(spanOdoDay).style.display = '';
        document.getElementById(txtOdoDay).style.display = '';
        ValidatorEnable(document.getElementById(compOdoDayGreaterThan), true);
        ValidatorEnable(document.getElementById(compOdoDayLessThan), true);
        ValidatorEnable(document.getElementById(reqOdoDay), true);
    }
    else
    {
        document.getElementById(spanOdoDay).style.display = 'none';
        document.getElementById(txtOdoDay).style.display = 'none';
        ValidatorEnable(document.getElementById(compOdoDayGreaterThan), false);
        ValidatorEnable(document.getElementById(compOdoDayLessThan), false);
        ValidatorEnable(document.getElementById(reqOdoDay), false);
    }
}

function ValidateForm()
{
    if (validateform('vgMain') == false)
        return;
}

function hidePageOptions(activeModule) {
    switch (activeModule)
    {
        case 9:
        case 7:
        case 2: // expenses
            if (document.getElementById('divContractEmailFrom') != null) {
                document.getElementById('divContractEmailFrom').style.display = 'none';
            }
            break;
        case 5:
        case 3: // contracts
            if (document.getElementById('lnkESROptions') != null) {
                document.getElementById('lnkESROptions').style.display = 'none';
            }
            if (document.getElementById('divExpensesEmailFrom') != null) {
                document.getElementById('divExpensesEmailFrom').style.display = 'none';
            }
            if (document.getElementById('divContractEmailFrom') != null) {
                document.getElementById('divContractEmailFrom').style.display = '';
            }
            break;
        default:
            break;
    }
}

function setActiveTab() {
    setActiveAjaxTab(currentTabContainerId, currentActiveTabId);
}


// function to check whether the self registration checkbox has been selected or deselected.
// if deselected, we need to confirm with the user before deselecting all other controls in this tab
function CheckSelfRegistrationStatus(regcheckbox) {
    if (!regcheckbox.checked) {
        var agree = confirm('Are you sure you want to disable self registration? Doing so will reset all options in this tab.');
        if (agree) {
            registrationdisablecontrol(chkselfregempconact, true, true);
            registrationdisablecontrol(chkselfreghomaddr, true, true);
            registrationdisablecontrol(chkselfregempinfo, true, true);
            registrationdisablecontrol(chkselfregrole, true, true);
            registrationdisablecontrol(chkselfregitemrole, true, true);
            registrationdisablecontrol(cmbdefaultrole, true, false);
            registrationdisablecontrol(cmbdefaultitemrole, true, false);
            registrationdisablecontrol(chkselfregsignoff, true, true);
            registrationdisablecontrol(chkselfregadvancessignoff, true, true);
            registrationdisablecontrol(chkselfregdepcostcode, true, true);
            registrationdisablecontrol(chkselfregbankdetails, true, true);
            registrationdisablecontrol(chkselfregcardetails, true, true);
            registrationdisablecontrol(chkselfregudf, true, true);
            return true;
        }
        else {
            regcheckbox.checked = true;
            registrationdisablecontrol(chkselfregempconact, false, true);
            registrationdisablecontrol(chkselfreghomaddr, false, true);
            registrationdisablecontrol(chkselfregempinfo, false, true);
            registrationdisablecontrol(chkselfregrole, false, true);
            registrationdisablecontrol(chkselfregitemrole, false, true);
            registrationdisablecontrol(cmbdefaultrole, false, false);
            registrationdisablecontrol(cmbdefaultitemrole, false, false);
            registrationdisablecontrol(chkselfregsignoff, false, true);
            registrationdisablecontrol(chkselfregadvancessignoff, false, true);
            registrationdisablecontrol(chkselfregdepcostcode, false, true);
            registrationdisablecontrol(chkselfregbankdetails, false, true);
            registrationdisablecontrol(chkselfregcardetails, false, true);
            registrationdisablecontrol(chkselfregudf, false, true);
            return false;
        }
    }
    else {
        regcheckbox.checked = true;
        registrationdisablecontrol(chkselfregempconact, false, true);
        registrationdisablecontrol(chkselfreghomaddr, false, true);
        registrationdisablecontrol(chkselfregempinfo, false, true);
        registrationdisablecontrol(chkselfregrole, false, true);
        registrationdisablecontrol(chkselfregitemrole, false, true);
        registrationdisablecontrol(cmbdefaultrole, false, false);
        registrationdisablecontrol(cmbdefaultitemrole, false, false);
        registrationdisablecontrol(chkselfregsignoff, false, true);
        registrationdisablecontrol(chkselfregadvancessignoff, false, true);
        registrationdisablecontrol(chkselfregdepcostcode, false, true);
        registrationdisablecontrol(chkselfregbankdetails, false, true);
        registrationdisablecontrol(chkselfregcardetails, false, true);
        registrationdisablecontrol(chkselfregudf, false, true);
        return true;
    }
}

function registrationdisablecontrol(controlid, disable, isCheckBox) {
    var objcontrol = document.getElementById(controlid);
    if (disable) {
        if (isCheckBox) {
            objcontrol.checked = false;
        }
        else {
            objcontrol.options[0].selected = true;
        }
    }
    objcontrol.disabled = disable;
}

function checkMobileDeviceReceiptCompatibility(fromOption) {

    switch (fromOption) {
        case 'mobileDevices':
            
            break;
        case 'attachReceipts':
            if ($g(chkattachreceiptID).checked === false && $g(chkmobiledevicesID).checked) {
                // warn if attach receipts is unchecked and mobile devices is activated
                SEL.MasterPopup.ShowMasterPopup("Disabling this option will prevent mobile device users from being able to upload receipts from their device.");
            }

            break;
        default:
            break;
    }

    if ($g(chkattachreceiptID).checked && $g(chkmobiledevicesID).checked) {
        // check that PNG file types are permitted to be uploaded, if not confirm to user that they will be activated automatically.
        Spend_Management.svcAccountOptions.checkForPNGAttachmentType(PNGOptionCheckComplete);
    }
}

function PNGOptionCheckComplete(hasPNG)
{
    if (!hasPNG)
    {
        SEL.MasterPopup.ShowMasterPopup("Enabling this option will automatically permit .PNG file type attachments for use by mobile device receipts.<br /><br />This is not currently one of the permitted attachment types.");
    }
}

function EmpNotifyOfChangesClick() {
    if ($e(chkemployeedetailschanged)) {
        if ($g(chkemployeedetailschanged).checked) {
            if (!confirm('\n\nSwitching this functionality on will allow users to send change of detail requests to their administrator in an email.\nPlease be aware that messages sent over the internet via email may not be secure.\n\n')) {
                return false;
            }
        }
    }
    return true;
}

function ValidateAddressKeyword(source, args) {

    if (args.Value.match(/[^A-Za-z0-9]/) || ($("input[id$=txtHomeAddressKeyword]").val() === $("input[id$=txtWorkAddressKeyword]").val() && $("input[id$=txtHomeAddressKeyword]").val().length > 0)) {
        args.IsValid = false;
    } else {
        SEL.Data.Ajax({
            url: "accountOptions.aspx/ValidateAddressKeyword",
            async: false,
            data: {
                keyword: args.Value
            },
            success: function (r) {
                args.IsValid = r.d;
            }
        });
        
    }

}

function loadDutyOfCareFunctions() {
    $('input[type=radio][name$=selectApprover]').on('change', function () {
        switch ($(this).val()) {
            case 'lineManagerAsApprover':
                $('.dutyOfCareApproverTeamSection').fadeOut('fast');
                break;
            case 'teamAsApprover':
                $('.dutyOfCareApproverTeamSection').fadeIn();
                break;
        }
    });
}
function toggleValidator(chk) {
    var validator;
    var reminderFrequencySpan;
    var reminderFrequencyTxt;
    if (chk.id === "chkEnableCurrentClaimsReminder")
    {
        validator = document.getElementById("currentClaimsFrequencyValidator");
        reminderFrequencySpan = $("#currentClaimReminderFrequency");
        reminderFrequencyTxt = $("#ddlCurrentClaimReminderFrequency");
    }
    else {
        validator = document.getElementById('frequencyValidator');
        reminderFrequencySpan = $('#claimReminderFrequency');
        reminderFrequencyTxt = $('#ddlClaimApprovalReminderFrequency');
    }

    if (!chk.checked) {
        reminderFrequencySpan.hide();
        ValidatorEnable(validator, false);
    }
    else {
        if (parseInt(reminderFrequencyTxt.val()) === 0) {
            reminderFrequencyTxt.val("1");
        }
        reminderFrequencySpan.show();
        ValidatorEnable(validator, true);
        
    }
    
}

function hideAutomaticDrivingLicenceLookup(chk) {
    if (chk !== undefined && chk !== null) {
        if (chk.checked) {
            $('#automaticDrivingLicenceLookupFrequency').show();
            $("#automaticAutoRevokeOfConsentLookupFrequency").show();
        } else {
            $('#automaticDrivingLicenceLookupFrequency').hide();
            $("#automaticAutoRevokeOfConsentLookupFrequency").hide();
        }
    }
}

function hideDrivingLicenceFrequencyPanel(reviewCheck, drivingLicenceCheck) {
    if (reviewCheck != null) {
        if (reviewCheck.checked) {
            $(".DrivingLicenceReviewFrequency").show();
        } else {
            $(".DrivingLicenceReviewFrequency").hide();
        }
    }
    if (drivingLicenceCheck != null) {
        if (drivingLicenceCheck.checked) {
            $("#DrivingLicenceReviewWrapper").show();
        } else {
            $("#DrivingLicenceReviewWrapper").hide();
        }
    }
}

function hideReminderFrequencyIfChecked() {
    if (! $('#chkEnableClaimApprovalReminder:checked').length >0) {
        $('#claimReminderFrequency').hide();
    }
}

function frequencyDaysValidation(sender, args) {
    args.IsValid = true;
    var checkboxId = "chkEnableCurrentClaimsReminder";
    if (sender.controltovalidate === "ddlClaimApprovalReminderFrequency") {
        checkboxId = "chkEnableClaimApprovalReminder";
    }
    if ($('#' + checkboxId + ':checked').length > 0) {
        var days = parseInt($('#' + sender.controltovalidate).val());
        if (isNaN(days) || days <= 0 || days > 99) {
            args.IsValid = false;
        }
    }
}

function hideProvidersIfChecked() {
    if ($('#chkEnableAutoUpdateOfExchangeRates:checked').length > 0) {
        $('#exchangeRateProvider').show();
    } else {
        $('#exchangeRateProvider').hide();
    }
}

function hideDrivingLicenceReviewReminderDaysPanel(reminderCheck) {
    if (reminderCheck != null) {
        if (reminderCheck.checked) {
            $(".DrivingLicenceReviewReminderDays").show();
        } else {
            $(".DrivingLicenceReviewReminderDays").hide();
        }
    }
}

function hideAutomaticVehicleDocumentLookup(blockCheck) {
    if (blockCheck != null) {
        if ($("#" + chkblocktaxexpiry + ":checked").length > 0 || $("#" + chkblockmotexpiry + ":checked").length > 0) {
            $("#" + spanVehicleDocumentLookups).show();
        } else {
            $("#" + spanVehicleDocumentLookups).hide();
            $("#" + chkVehicleDocumentLookup).prop('checked', false);
        }
    }
}
