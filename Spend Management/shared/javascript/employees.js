var corporatecardid;
var worklocationid = 0;
var addNewWorkLocation = false;
var addNewHomeLocation = false;
var homelocationid = 0;
var esrassignmentid = 0;

// new Object (= {}) allows the use of "for (x in y)" when used as an associative array
// new Array (= []) would cause iteration over the Array methods (this is bad m'kay)
var WebMethodValidatorErrors = {};
// should be moved to a sel.xxx namespace if expanded
var WebMethodValidatorError = { 'ID': '', 'ControlToValidate': '', 'ValidatorErrorMessage': '', 'ServerError': '' };

function saveEmployee(ESRAssignCheck, SendEmailCheck)
{
    if (validateform('vgMain') === false)
        return;

    //The save method passes in a boolean value that allows the check to see if there are any ESR assignments on the employee before saving
    if (ESRAssignCheck)
    {
        if (CheckESRAssignments)
        {
            if (validateform('vgAssign') === false)
                return;
        }
    }

    var username = $g(txtusername).value;
    var title = $g(txttitle).value;
    var firstname = $g(txtfirstname).value;
    var middlenames = $g(txtmiddlenames).value;
    var surname = $g(txtsurname).value;
    var maidenname = $g(txtmaidenname).value;
    var preferredname = $g(txtpreferredname).value;
    var email = $g(txtemail).value;
    var extension = $g(txtextension).value;
    var mobileno = $g(txtmobileno).value;
    var pagerno = $g(txtpagerno).value;
    var emailhome = $g(txtemailhome).value;
    var telno = $g(txttelno).value;
    var faxno = $g(txtfaxno).value;
    var creditaccount = $g(txtcreditaccount).value;
    var payroll = $g(txtpayroll).value;
    var position = $g(txtposition).value;
    var ninumber = $g(txtninumber).value;
    var employeenumber = $g(txtemployeenumber).value;
    var nhsuniqueid = $g(txtnhsuniqueid).value;
    var hiredate = null;
    var excessMileage = $g(txtExcessMileage).value == "" ? 0 : $g(txtExcessMileage).value;

    if ($g(txthiredate).value !== "") {
        hiredate = $g(txthiredate).value.substring(6, 10) + "/" + $g(txthiredate).value.substring(3, 5) + "/" + $g(txthiredate).value.substring(0, 2);
    }
    var leavedate = null;
    if ($g(txtleavedate).value !== "") {
        leavedate = $g(txtleavedate).value.substring(6, 10) + "/" + $g(txtleavedate).value.substring(3, 5) + "/" + $g(txtleavedate).value.substring(0, 2);
    }

    var homecountry = new Number($g(cmbcountry).options[$g(cmbcountry).selectedIndex].value);
    var currency = new Number($g(cmbcurrency).options[$g(cmbcurrency).selectedIndex].value);
    var lineManagerId = parseInt($(lineManagerCombo.selectors.id).val(), 10);
    var linemanager = (isFinite(lineManagerId) && lineManagerId >= 0) ? lineManagerId : 0;
    var signoff = new Number($g(cmbsignoffs).options[$g(cmbsignoffs).selectedIndex].value);
    var signoffcc = new Number($g(cmbsignoffcc).options[$g(cmbsignoffcc).selectedIndex].value);
    var signoffpc = new Number($g(cmbsignoffpc).options[$g(cmbsignoffpc).selectedIndex].value);
    var advancesgroup = new Number($g(cmbadvancesgroup).options[$g(cmbadvancesgroup).selectedIndex].value);
    var accountholder = '';
    var accountnumber = '';
    var accounttype = '';
    var sortcode = '';
    var reference = '';
    var gender = $g(cmbgender).options[$g(cmbgender).selectedIndex].value;
    var dateofbirth = null;
    if ($g(txtdateofbirth).value !== "") {
        dateofbirth = $g(txtdateofbirth).value.substring(6, 10) + "/" + $g(txtdateofbirth).value.substring(3, 5) + "/" + $g(txtdateofbirth).value.substring(0, 2);
    }
    var startmiles = 0;
    if ($g(txtstartmiles).value !== '') {
        startmiles = new Number($g(txtstartmiles).value);
    }
    var startmilesdate = null;
    if ($g(txtstartmilesdate).value !== "") {
        startmilesdate = $g(txtstartmilesdate).value.substring(6, 10) + "/" + $g(txtstartmilesdate).value.substring(3, 5) + "/" + $g(txtstartmilesdate).value.substring(0, 2);
    }

    var localeID = null;
    if ($g(ddlstLocale).options[$g(ddlstLocale).selectedIndex].value !== '0')
    {
        localeID = new Number($g(ddlstLocale).options[$g(ddlstLocale).selectedIndex].value);
    }

    var NHSTrustID = null;
    if ($g(NHSTrustIDClientID).options.length > 0 && $g(NHSTrustIDClientID).options[$g(NHSTrustIDClientID).selectedIndex].value !== '0')
    {
        NHSTrustID = new Number($g(NHSTrustIDClientID).options[$g(NHSTrustIDClientID).selectedIndex].value);
    }

    var emailNotifications = new Array();
    var emailNotificationPanel = $g(emailNotificationsClientID);
    var emailNotificationObjects = emailNotificationPanel.getElementsByTagName("input");
    var i;

    for (i = 0; i < emailNotificationObjects.length; i++) {
        if (emailNotificationObjects[i].checked === true) {
                emailNotifications.push(new Number(emailNotificationObjects[i].id.substr(52 + ("emailNotification_".length))));
            }
        }

    if ($e(emailNotificationsNHSClientID))
    {
        var emailNotificationNHSObjects;
        var emailNotificationNHSPanel = $g(emailNotificationsNHSClientID);
        if (emailNotificationNHSPanel !== null)
        {
            emailNotificationNHSObjects = emailNotificationNHSPanel.getElementsByTagName("input");
        }

        if (emailNotificationNHSObjects !== null)
        {
            for (i = 0; i < emailNotificationNHSObjects.length; i++)
            {
                if (emailNotificationNHSObjects[i].checked === true)
                {
                    emailNotifications.push(new Number(emailNotificationNHSObjects[i].id.substr(52 + ("emailNotification_".length))));
                }
            }
        }
    }
    var ccbSaveSucceeded = ccbSaveBreakdown();
    if (ccbSaveSucceeded == false) { return; }
    // if "Items should be assigned to" are all false, ccbItemArray will be undefined
    //if (ccbItemArray == undefined) { var ccbItemArray = new Array(); }

    var sendPasswordKey = $g(chkSendPasswordKey).checked;
    var sendWelcomeEmail = $g(chkWelcomeEmail).checked;
    //var defaultSubAccount = 0;
    //if ($g(cmbDefaultSubAccount).selectedIndex > 0) {
    var defaultSubAccount = $g(cmbDefaultSubAccount).options[$g(cmbDefaultSubAccount).selectedIndex].value;
    //}

    var userdefined = getItemsFromPanel('vgMain');

    var wmerrors = [];
    var key;
    for (key in WebMethodValidatorErrors)
    {
        if (WebMethodValidatorErrors.hasOwnProperty(key))
        {
            wmerrors.push(WebMethodValidatorErrors[key]);
        }
    }
    var levelValue = null;
    var hdnAuthoriserLevelValue = $get(hdnAuthoriserLevel);
    if (hdnAuthoriserLevelValue != null && hdnAuthoriserLevelValue.value != '')
    {
        levelValue = hdnAuthoriserLevelValue.value;
    }
    var hdnDefaultAuthoriserLevelIdValue = $get(hdnDefaultAuthoriserLevelId);
    if (hdnDefaultAuthoriserLevelIdValue != null && hdnDefaultAuthoriserLevelIdValue.value != '')
    {
        levelValue = hdnDefaultAuthoriserLevelIdValue.value;
    }
    else
    {
        var authoriserLevel = $get(cmbAuthoriserLevel);
        if (authoriserLevel) {
            levelValue = authoriserLevel.value;
        }
    }

    if (levelValue === 0 || levelValue === "0")
    {
        levelValue = null;
    }

    PageMethods.saveEmployee(nEmployeeid, username, title, firstname, middlenames, surname, maidenname, email, extension, pagerno, mobileno, emailhome, telno, faxno, creditaccount, payroll, position, ninumber, hiredate, leavedate, homecountry, currency, linemanager, signoff, signoffcc, signoffpc, advancesgroup, accountholder, accountnumber, accounttype, sortcode, reference, gender, dateofbirth, startmiles, startmilesdate, localeID, NHSTrustID, employeenumber, preferredname, nhsuniqueid, emailNotifications, ccbItemArray, userdefined, sendPasswordKey, sendWelcomeEmail, defaultSubAccount, SendEmailCheck, NewEmployee, wmerrors, (currentAction === "activate"), levelValue, excessMileage, saveEmployeeComplete);
}

function saveEmployeeComplete(data)
{
    if (data < 0) {
        switch (data) {
            case -1:
                SEL.MasterPopup.ShowMasterPopup('The username you have entered already exists. Please enter another username.', 'Message from ' + moduleNameHTML);
                return;
            case -2:
                SEL.MasterPopup.ShowMasterPopup('The signoff group of this user cannot currently be changed as they have 1 or more claims in the approval process.', 'Message from ' + moduleNameHTML);
                break;
            case -3:
                SEL.MasterPopup.ShowMasterPopup('The selected Signoff Group cannot be used as the employee would approve one of their own stages.', 'Message from ' + moduleNameHTML);
                break;
            case -4:
                SEL.MasterPopup.ShowMasterPopup('The selected Signoff Group (Credit Cards) cannot be used as the employee would approve one of their own stages.', 'Message from ' + moduleNameHTML);
                break;
            case -5:
                SEL.MasterPopup.ShowMasterPopup('The selected Signoff Group (Purchase Cards) cannot be used as the employee would approve one of their own stages.', 'Message from ' + moduleNameHTML);
                break;
        }
    }
    else
    {
        nEmployeeid = data;

        SEL.MobileDevices.updateGridFilterEmployeeId(nEmployeeid);
        SEL.BankAccounts.updateGridFilterEmployeeId(nEmployeeid);
    }

    if (currentAction != undefined) {
        switch (currentAction) {
            case 'saveCard':
                showCorporateCardModal(false);
                break;
            case 'saveWorkLocation':
                showWorkLocationModal(false);
                break;
            case 'saveCar':
                showCarModal(false);
                break;
            case 'saveHomeLocation':
                showHomeLocationModal(false);
                break;
            case 'saveESRAssignment':
                showESRModal(false);
                break;
            case 'OK':
            case 'activate':
                document.location = "selectemployee.aspx";
                break;
            case 'saveMobileDevice':
                showMobileDevice(false);
                break;
            case 'saveBankAccount':
                showBankAccount(false);
                break;
                // MW: THIS FIX FOR SHALLOW SAVE CHECKED IN ERRONEOUSLY AND IS SUBJECT TO A STACK RANKED BUG, SO COMMENTED OUT FOR NOW
                //        case 'saveAccessRole':
                //            showNewAccessRoleModal(false);
                //            break;
            default:
                break;
        }
    }

}

function ActivateEmployee()
{
    var warningMessage = "Warning: This employee does not have an email address stored. Click Cancel to go back to the employee record and enter a valid email before retrying activation.\\n\\nClicking OK will continue with activation, but the employee will need contacting directly with their logon details.";
    var email = $g(txtemail).value;

    if ((typeof email === "undefined" || email === null || $.trim(email) === "" || email.indexOf("@") === -1) && !confirm(warningMessage))
    {
        return false;
    }

    currentAction = 'activate';
    saveEmployee(true, true);
}

function sendPasswordLink(employeeid)
{
    if (confirm("Are you sure you wish to reset this employees password?"))
    {
        PageMethods.ResetPassword(employeeid, sendPasswordLinkComplete, sendPasswordLinkFailed);
    }
}

function sendPasswordLinkComplete(data) {
    if (data == false) {
        SEL.MasterPopup.ShowMasterPopup("This employee is currently archived or not active.", 'Message from ' + moduleNameHTML);
    } else {
        SEL.MasterPopup.ShowMasterPopup("Password reset email sent.", 'Message from ' + moduleNameHTML);
    }
}

function sendPasswordLinkFailed(data)
{

}

function deleteCar(carid)
{
    if (confirm('Are you sure you wish to delete the selected vehicle?'))
    {
        Spend_Management.svcCars.deleteCar(nEmployeeid, carid, deleteCarComplete);
    }
}

function deleteCarComplete(retVal)
{
    switch (retVal)
    {
        case 0:
            PageMethods.getCarGrid(nEmployeeid, getCardGridComplete);
            break;
        case 1:
            SEL.MasterPopup.ShowMasterPopup("Cannot delete vehicle as it is associated to a current, submitted or previous expense item.", 'Message from ' + moduleNameHTML);
            break;
        case 2:
            SEL.MasterPopup.ShowMasterPopup("Cannot delete vehicle as it is associated to a duty of care record.", 'Message from ' + moduleNameHTML);
            break;
        case -10:
            SEL.MasterPopup.ShowMasterPopup("Cannot delete vehicle as it is associated to a GreenLight or user defined field record.", 'Message from ' + moduleNameHTML);
            break;
        default:
            break;
    }
}

function getCardGridComplete(data) {
    if ($e(pnlCarsGrid) === true) {
        $g(pnlCarsGrid).innerHTML = data[2];
        SEL.Grid.updateGrid(data[1]);
    }
}

function showCorporateCardModal(save) {
    currentAction = 'saveCard';
    if (save)
    {
        saveEmployee(true, false);
        return;
    }
    if (corporatecardid == 0) {
        clearCorporateCardModal();
    }
    var modal = $find(modcorporatecard);
    modal.show();
}

function hideCorporateCardModal() {
    var modal = $find(modcorporatecard);
    modal.hide();
}

function clearCorporateCardModal() {
    document.getElementById(cmbcardprovider).selectedIndex = 0;
    document.getElementById(txtcardnumber).value = '';
    document.getElementById(chkCardActive).checked = false;
}

function saveCorporateCard() {
    if (validateform('vgCorporateCard') == false)
        return;


    var cardprovider = new Number(document.getElementById(cmbcardprovider).options[document.getElementById(cmbcardprovider).selectedIndex].value);
    var cardnumber = document.getElementById(txtcardnumber).value;
    var active = document.getElementById(chkCardActive).checked;

    PageMethods.saveCorporateCard(nEmployeeid, corporatecardid, cardprovider, cardnumber, active, saveCorporateCardComplete);
}

function saveCorporateCardComplete(data) {
    if (data == -1) {
        SEL.MasterPopup.ShowMasterPopup('The card number provided already exists on the system and cannot be duplicated', 'Save Card Failed');
    }
    else {
        populateCorporateCardGrid();
        hideCorporateCardModal();
    }
}

function deleteCorporateCard(corporatecardid) {
    if (confirm('Are you sure you wish to delete the selected corporate card?')) {
        PageMethods.deleteCorporateCard(corporatecardid, deleteCorporateCardComplete);
    }
}
function deleteCorporateCardComplete(data) {
    populateCorporateCardGrid();
}
function populateCorporateCardGrid() {
    PageMethods.getCorporateCardGrid(nEmployeeid, populateCorporateCardGridComplete);
}

function populateCorporateCardGridComplete(data) {
    if ($e(pnlCorporateCardsGrid) === true) {
        $g(pnlCorporateCardsGrid).innerHTML = data[2];
        SEL.Grid.updateGrid(data[1]);
    }
}

function editCorporateCard(cardid)
{
    corporatecardid = cardid;
    PageMethods.getCorporateCard(nEmployeeid, corporatecardid, getCorporateCardComplete)
}

function getCorporateCardComplete(data)
{
    document.getElementById(txtcardnumber).value = data.cardnumber;
    document.getElementById(chkCardActive).checked = data.active;
    var cardprovider = document.getElementById(cmbcardprovider);

    for (var i = 0; i < cardprovider.options.length; i++)
    {
        if (cardprovider.options[i].value == data.cardprovider.cardproviderid)
        {
            cardprovider.selectedIndex = i;
            break;
        }
    }
    showCorporateCardModal();
}

function showWorkLocationModal(save)
{
    currentAction = 'saveWorkLocation';

    if (addNewWorkLocation)
    {
        clearWorkLocationModal();
    }

    if (save)
    {
        saveEmployee(true, false);
        return;
    }
    var modal = $find(modworklocation);
    modal.show();
}

function hideWorkLocationModal() {
    var modal = $find(modworklocation);
    modal.hide();
}

function saveWorkLocation() {
    if (validateform('vgWorkLocation') == false) {
        return;
    }

    var locationid = new Number(document.getElementById(txtworklocationid).value);
    var worklocationstart = null;
    if (document.getElementById(txtworklocationstart).value != '') {
        worklocationstart = document.getElementById(txtworklocationstart).value.substring(6, 10) + "/" + document.getElementById(txtworklocationstart).value.substring(3, 5) + "/" + document.getElementById(txtworklocationstart).value.substring(0, 2);
    }
    var worklocationend = null;
    if (document.getElementById(txtworklocationend).value != '') {
        worklocationend = document.getElementById(txtworklocationend).value.substring(6, 10) + "/" + document.getElementById(txtworklocationend).value.substring(3, 5) + "/" + document.getElementById(txtworklocationend).value.substring(0, 2);
    }
    worklocationactive = true;
    worklocationtemporary = document.getElementById(chkworklocationtemporary).checked;

    var worklocationprimaryrotational = $('#' + chkworklocationprimaryrotational).prop('checked');

    PageMethods.saveWorkLocation(worklocationid, nEmployeeid, locationid, worklocationstart, worklocationend, worklocationactive, worklocationtemporary, worklocationprimaryrotational, saveWorkLocationComplete);
}

function saveWorkLocationComplete(data) {
    hideWorkLocationModal();
    populateWorkLocationsGrid();
}

function editWorkLocation(id) {
    worklocationid = id;
    PageMethods.getWorkLocation(nEmployeeid, id, getWorkLocationComplete);
}

function getWorkLocationComplete(data)
{
    //var locationid = new Number(document.getElementById(txtworklocationid).value);

    document.getElementById(txtworklocation).value = data[2];
    document.getElementById(txtworklocationid).value = data[1];

    if (data[0].StartDate !== null)
    {
        document.getElementById(txtworklocationstart).value = data[0].StartDate.format('dd/MM/yyyy');
    }
    else
    {
        document.getElementById(txtworklocationstart).value = "";
    }

    if (data[0].EndDate !== null)
    {
        document.getElementById(txtworklocationend).value = data[0].EndDate.format('dd/MM/yyyy');
    }
    else
    {
        document.getElementById(txtworklocationend).value = "";
    }

    document.getElementById(chkworklocationtemporary).checked = data[0].Temporary;

    $('#' + chkworklocationprimaryrotational).prop('checked', data[0].PrimaryRotational);

    disableWorkLocationModal($("#deletegridWorkLocations_" + worklocationid + " img").length == 0);

    // reset the address widget
    $("#" + txtworklocation).address("reset");

    showWorkLocationModal();
}
function populateWorkLocationsGrid() {
    PageMethods.createWorkLocationGrid(nEmployeeid, populateWorkLocationsGridComplete);
}

function populateWorkLocationsGridComplete(data) {
    if ($e(pnlWorkLocationsGrid) == true) {
        $g(pnlWorkLocationsGrid).innerHTML = data[2];
        SEL.Grid.updateGrid(data[1]);
    }
}

function deleteWorkLocation(id)
{
    if (confirm('Are you sure you wish to delete the selected work address?'))
    {
        PageMethods.deleteWorkLocation(id, nEmployeeid, deleteWorkLocationComplete);
    }
}
function deleteWorkLocationComplete(data) {
    populateWorkLocationsGrid();
}

function clearWorkLocationModal()
{
    document.getElementById(txtworklocation).value = "";
    document.getElementById(txtworklocationid).value = "";
    document.getElementById(txtworklocationstart).value = "";
    document.getElementById(txtworklocationend).value = "";
    document.getElementById(chkworklocationtemporary).checked = false;
    $('#' + chkworklocationprimaryrotational).prop('checked', false);
    disableWorkLocationModal(false);

    // reset the address widget
    $("#" + txtworklocation).address("reset");

    return;
}

function disableWorkLocationModal(disabled) {
    $("#ctl00_contentmain_pnlWorkLocation .twocolumn input").prop("disabled", disabled);
    $("#ctl00_contentmain_pnlWorkLocation .twocolumn img, #ctl00_contentmain_pnlWorkLocation .formbuttons a").toggle(!disabled);
}

function showCarModal(save, addNew) {
    currentAction = 'saveCar';

    if (addNew)
    {
        // the "does this replace..." part may have been hidden by an edit so show it again
        $(".replaceCar").closest(".formpanel").show();

        document.getElementById(cmdSaveID).src = appPath + "/shared/images/buttons/btn_save.png";
        clearCarModal();
            $("#ctl00_contentmain_aeCar_cmdSave").click(function () {
                vehicleDocuments("The vehicle has been successfully added.",true)
        });
       
    }

    if (save)
    {
        saveEmployee(true, false);
        return;
    }
    var modal = $find(modcar);
    modal.show();
}

function hideCarModal() {
    var modal = $find(modcar);
    modal.hide();
    $("#ctl00_contentmain_aeCar_cmdSave").unbind('click');
}

function deletePoolCar(carid)
{
    if (confirm('Are you you wish to remove the selected pool vehicle from this employee?')) {
        PageMethods.deletePoolCar(nEmployeeid, carid, deletePoolCarComplete);
    }
}

function deletePoolCarComplete(retVal)
{
    populatePoolCarGrid();
}

function showPoolCarModal()
{
    var modal = $find(modpoolcar);
    var poolCarSelect = $(poolCarCombo.selectors.select).length === 1 ? $(poolCarCombo.selectors.select)[0] : null;

    if (poolCarSelect !== null && poolCarSelect.options.length > 0)
    {
        $(poolCarCombo.selectors.id).val(poolCarSelect.options[poolCarSelect.selectedIndex].value);
    }
    else
    {
        $(poolCarCombo.selectors.id).val("");
        $(poolCarCombo.selectors.textbox).val("");
    }

    modal.show();
}

function hidePoolCarModal()
{
    var modal = $find(modpoolcar);
    modal.hide();
}

function populatePoolCarGrid() {
    PageMethods.getPoolCarGrid(nEmployeeid, populatePoolCarGridComplete);
}

function populatePoolCarGridComplete(data) {
    if ($e(pnlPoolCarsGrid) === true) {
        $g(pnlPoolCarsGrid).innerHTML = data[2];
        SEL.Grid.updateGrid(data[1]);
    }
}

function savePoolCar()
{
    var poolCar = $(poolCarCombo.selectors.id).val();
    PageMethods.savePoolCar(nEmployeeid, poolCar, savePoolCarComplete);
}

function savePoolCarComplete() {
    populatePoolCarGrid();
    hidePoolCarModal();
}

function deleteEmployee(employeeid) {
    nEmployeeid = employeeid;
    if (confirm('Are you sure you wish to delete the selected employee?')) {
        PageMethods.deleteEmployee(accountid, employeeid, deleteEmployeeComplete);
    }
}

function deleteEmployeeComplete(data) {
    switch (data) {
        case 1:
            SEL.MasterPopup.ShowMasterPopup('This employee cannot be deleted as they are assigned to one or more Signoff Groups.', 'Message from ' + moduleNameHTML);
            break;
        case 2:
            SEL.MasterPopup.ShowMasterPopup('This employee cannot be deleted as they have one or more advances allocated to them.', 'Message from ' + moduleNameHTML);
            break;
        case 3:
            SEL.MasterPopup.ShowMasterPopup('This employee is currently set as a budget holder.', 'Message from ' + moduleNameHTML);
            break;
        case 4:
            SEL.MasterPopup.ShowMasterPopup('You must archive an employee before it can be deleted.', 'Message from ' + moduleNameHTML);
            break;
        case 5:
            SEL.MasterPopup.ShowMasterPopup('This employee cannot be deleted as they are the owner of one or more contracts.', 'Message from ' + moduleNameHTML);
            break;
        case 6:
            SEL.MasterPopup.ShowMasterPopup('This employee cannot be deleted as they are on one or more contract audiences as an individual.', 'Message from ' + moduleNameHTML);
            break;
        case 7:
            SEL.MasterPopup.ShowMasterPopup('This employee cannot be deleted as they are on one or more attachment audiences as an individual.', 'Message from ' + moduleNameHTML);
            break;
        case 8:
            SEL.MasterPopup.ShowMasterPopup('This employee cannot be deleted as they are on one or more contract notification lists.', 'Message from ' + moduleNameHTML);
            break;
        case 9:
            SEL.MasterPopup.ShowMasterPopup('This employee cannot be deleted as they are the leader of one or more teams.', 'Message from ' + moduleNameHTML);
            break;
        case 10:
            SEL.MasterPopup.ShowMasterPopup('This employee cannot be deleted as it would leave one or more empty teams.', 'Message from ' + moduleNameHTML);
            break;
        case 11:
            SEL.MasterPopup.ShowMasterPopup('This employee cannot be deleted as they are associated to one or more contracts history.', 'Message from ' + moduleNameHTML);
            break;
        case 12:
            SEL.MasterPopup.ShowMasterPopup('This employee cannot be deleted as they are associated to one or more report folders.', 'Message from ' + moduleNameHTML);
            break;
        case 13:
            SEL.MasterPopup.ShowMasterPopup('This employee cannot be deleted as they are associated to one or more audiences.', 'Message from ' + moduleNameHTML);
            break;
        case 14:
            SEL.MasterPopup.ShowMasterPopup('This employee cannot be deleted as they are the owner of one or more cost codes.', 'Message from ' + moduleNameHTML);
            break;
        case 15:
            SEL.MasterPopup.ShowMasterPopup('This employee cannot be deleted as they are assigned to one or more approval matrices.', 'Message from ' + moduleNameHTML);
            break;
        case 16:
            SEL.MasterPopup.ShowMasterPopup('This employee cannot be deleted as they are the Owner or Support Contact for one or more Greenlights.', 'Message from ' + moduleNameHTML);
            break;
        case 17:
            SEL.MasterPopup.ShowMasterPopup('This employee cannot be deleted as they are currently assigned as the Default authoriser.', 'Message from ' + moduleNameHTML);
            break;
        case -10:
            SEL.MasterPopup.ShowMasterPopup('This employee cannot be deleted as they are referenced in either a GreenLight or by a user defined field.', 'Message from ' + moduleNameHTML);
            break;
        default:
            SEL.Grid.deleteGridRow('gridEmployees', nEmployeeid);
            break;
    }
}

function changeArchiveStatus(employeeid) {
    nEmployeeid = employeeid;
    PageMethods.changeStatus(employeeid, changeArchiveStatusComplete);

}

function changeArchiveStatusComplete(data)
{
    if (data==-1) {
        SEL.MasterPopup.ShowMasterPopup('This employee cannot be archived as they are currently assigned as the Default authoriser.', 'Message from ' +moduleNameHTML);
        return false;
    }
    var cell = SEL.Grid.getCellById('gridEmployees', nEmployeeid, 'archiveStatus');
    if (cell.innerHTML.indexOf('Un-Archive') != -1)
    {
        cell.innerHTML = "<a href='javascript:changeArchiveStatus(" + nEmployeeid + ");'><img title='Archive' src='/shared/images/icons/folder_lock.png'></a>";
    }
    else
    {
        cell.innerHTML = "<a href='javascript:changeArchiveStatus(" + nEmployeeid + ");'><img title='Un-Archive' src='/shared/images/icons/folder_into.gif'></a>";
    }
}

function populateNewAccessRoles() {
    PageMethods.createNewAccessRoleGrid(nEmployeeid, populateNewAccessRolesComplete);
}

function populateNewAccessRolesComplete(data) {
    if ($e(pnlNewAccessRolesGrid) === true) {
        var grid = $g(pnlNewAccessRolesGrid);
        grid.innerHTML = data[2];
        SEL.Grid.updateGrid(data[1]);
    }
}

function showNewAccessRoleModal(save) {
    currentAction = 'saveAccessRole';
    if (save) {
        saveEmployee(true, false);
        if (nEmployeeid == 0) {
            return;
        }

    }
    var modal = $find(modNewAccessRoles);
    modal.show();
}

function hideNewAccessRoleModal() {
    var modal = $find(modNewAccessRoles);
    modal.hide();
}

function saveAccessRoles() {
    if (validateform('vgAccessRoles') == false) {
        return;
    }

    var lst = document.getElementById(cmbAccessRoleSubAccount);
    var subacc = null;
    if (lst != null) {
        subacc = lst[lst.selectedIndex].value;
    }
    var roles = SEL.Grid.getSelectedItemsFromGrid('gridNewAccessRoles');
    PageMethods.saveAccessRoles(nEmployeeid, roles, subacc, saveAccessRolesComplete);
}

function saveAccessRolesComplete(data) {
    populateAccessRoles();
    hideNewAccessRoleModal();
}

function deleteAccessRole(roleID, subAccountID) {
    if (confirm('Are you sure you wish to remove the selected role? The employee will lose any privileges associated with the role.'))
    {
        PageMethods.deleteAccessRole(nEmployeeid, roleID, subAccountID, deleteAccessRoleComplete);
    }
}
function deleteAccessRoleComplete(data) {
    if (data == -1) {
        SEL.MasterPopup.ShowMasterPopup('The role of this user cannot currently be removed as the user is responsible for authorising expense claims.', 'Message from ' + moduleNameHTML);
        return;
    }
    populateAccessRoles();
}
function populateAccessRoles() {
    PageMethods.CreateAccessRoleGrid(nEmployeeid, populateAccessRolesComplete);
}

function populateAccessRolesComplete(data) {
    if ($e(pnlAccessRolesGrid) === true) {
        $g(pnlAccessRolesGrid).innerHTML = data[2];
        SEL.Grid.updateGrid(data[1]);
    }
}

function showHomeLocationModal(save)
{
    currentAction = 'saveHomeLocation';

    if (addNewHomeLocation)
    {
        clearHomeLocationModal();
    }

    if (save)
    {
        saveEmployee(true, false);
        return;
    }
    var modal = $find(modhomelocation);
    modal.show();
}

function hideHomeLocationModal() {
    var modal = $find(modhomelocation);
    modal.hide();
}

function saveHomeLocation() {
    if (validateform('vgHomeLocation') == false) {
        return;
    }

    var locationid = new Number(document.getElementById(txthomelocationid).value);
    var homelocationstart = null;
    if (document.getElementById(txthomelocationstart).value != '') {
        homelocationstart = document.getElementById(txthomelocationstart).value.substring(6, 10) + "/" + document.getElementById(txthomelocationstart).value.substring(3, 5) + "/" + document.getElementById(txthomelocationstart).value.substring(0, 2);
    }
    var homelocationend = null;
    if (document.getElementById(txthomelocationend).value != '') {
        homelocationend = document.getElementById(txthomelocationend).value.substring(6, 10) + "/" + document.getElementById(txthomelocationend).value.substring(3, 5) + "/" + document.getElementById(txthomelocationend).value.substring(0, 2);
    }
    PageMethods.saveHomeLocation(homelocationid, nEmployeeid, locationid, homelocationstart, homelocationend, saveHomeLocationComplete);
}

function saveHomeLocationComplete(data)
{
    hideHomeLocationModal();
    populateHomeLocationsGrid();
}

function editHomeLocation(id, EsrId)
{
    homelocationid = id;
    PageMethods.getHomeLocation(nEmployeeid, id, EsrId === '' ? 0 : EsrId, getHomeLocationComplete);
}

function getHomeLocationComplete(data)
{
    //var locationid = new Number(document.getElementById(txthomelocationid).value);

    document.getElementById(txthomelocation).value = data[2];
    document.getElementById(txthomelocationid).value = data[1];

    if (data[0].StartDate !== null)
    {
        document.getElementById(txthomelocationstart).value = data[0].StartDate.format('dd/MM/yyyy');
    }
    else
    {
        document.getElementById(txthomelocationstart).value = "";
    }

    if (data[0].EndDate !== null)
    {
        document.getElementById(txthomelocationend).value = data[0].EndDate.format('dd/MM/yyyy');
    }
    else
    {
        document.getElementById(txthomelocationend).value = "";
    }

    if (data[0].EsrAddressId === undefined || data[0].EsrAddressId === null || data[0].EsrAddressId === 0) {
        $('#' + txthomelocation + ', #' + txthomelocationstart + ', #' + txthomelocationend).removeAttr("disabled");
        $('[id$="pnlHomeLocation"]').find('.formbuttons a').first().css('display', '');
        $('.esrEditWarning').hide();
    } else {
        $('#' + txthomelocation + ', #' + txthomelocationstart + ', #' + txthomelocationend).attr('disabled', 'disabled');
        $('[id$="pnlHomeLocation"]').find('.formbuttons a').first().css('display', 'none');;
        $('.esrEditWarning').show();
    }

    // reset the address widget
    $("#" + txthomelocation).address("reset");

    showHomeLocationModal(false);
}

function clearHomeLocationModal()
{
    document.getElementById(txthomelocation).value = "";
    document.getElementById(txthomelocationid).value = "";
    document.getElementById(txthomelocationstart).value = "";
    document.getElementById(txthomelocationend).value = "";
    $('#' + txthomelocation + ', #' + txthomelocationstart + ', #' + txthomelocationend).removeAttr("disabled");
    $('[id$="pnlHomeLocation"]').find('.formbuttons a').first().css('display', '');
    $('.esrEditWarning').hide();
    // reset the address widget
    $("#" + txthomelocation).address("reset");

    return;
}

function populateHomeLocationsGrid() {
    PageMethods.createHomeLocationGrid(nEmployeeid, populateHomeLocationsGridComplete);
}
function populateHomeLocationsGridComplete(data) {
    if ($e(pnlHomeLocationsGrid) === true) {
        $g(pnlHomeLocationsGrid).innerHTML = data[2];
        SEL.Grid.updateGrid(data[1]);
    }
}
function deleteHomeLocation(id, EsrId)
{
    if (EsrId !== '') {
        SEL.MasterPopup.ShowMasterPopup('This address was imported from ESR and cannot be deleted.', 'Message from ' + moduleNameHTML);
        return;
    }
    if (confirm('Are you sure you wish to delete the selected home address?')) {
        PageMethods.deleteHomeLocation(id, nEmployeeid, deleteHomeLocationComplete);
    }
}
function deleteHomeLocationComplete(data) {
    populateHomeLocationsGrid();
}

function ValidDates(startDate, endDate) {
    if (startDate === '' && endDate === '') return true;
    var bits = startDate.split('/');
    var startdate = new Date(bits[2], bits[1] - 1, bits[0]);
    var isStartDateValid = startDate === '' || (startdate && (startdate.getMonth() + 1) == bits[1]);
    bits = endDate.split('/');
    var enddate = new Date(bits[2], bits[1] - 1, bits[0]);
    var isEndDateValid = endDate === '' || (enddate && (enddate.getMonth() + 1) == bits[1]);
    if (!isStartDateValid) {
        SEL.MasterPopup.ShowMasterPopup("Please enter a valid start date");
        return false;
    }
    if (!isEndDateValid) {
        SEL.MasterPopup.ShowMasterPopup("Please enter a valid end date");
        return false;
    }
    if (isEndDateValid && isStartDateValid && enddate < startdate) {
        SEL.MasterPopup.ShowMasterPopup("The end date must be greater than or equal to the start date");
        return false;
    }
    return true;
}

function toDateFromJson(src) {
    return new Date(parseInt(src.substr(6))).format("dd/MM/yyyy");
}

function FormatDate(date) {
    var formattedDate = null;
    if (date !== '') {
        formattedDate = date.substring(6, 10) + "/" + date.substring(3, 5) + "/" + date.substring(0, 2);
    }
    return formattedDate;
}

function InitialiseDatePicker() {
    $("#" + txtItemRoleStartDate).datepicker({
        changeMonth: true,
        changeYear: true,
        minDate: '-116Y',
        dateFormat: 'dd/mm/yy',
        onSelect: function (selected) {
            $("#" + txtItemRoleEndDate).datepicker("option", "minDate", selected);
        }
    }).attr("maxlength", 10);

    $("#" + txtItemRoleEndDate).datepicker({
        changeMonth: true,
        changeYear: true,
        minDate: '-116Y',
        dateFormat: 'dd/mm/yy',
        OnSelect: function (selected) {
            $("#" + txtItemRoleStartDate).datepicker("option", "maxDate", selected);
        }
    }).attr("maxlength", 10);

    InitialiseDatePickerImage();
}

function InitialiseStartAndEndDatePickers(id) {
    $("#itemRoleStartDate_" + id).datepicker({
        changeMonth: true,
        changeYear: true,
        minDate: '-116Y',
        dateFormat: 'dd/mm/yy',
        onSelect: function (selected) {
            $("#itemRoleEndDate_" + id).datepicker("option", "minDate", selected);
        }
    }).attr("maxlength", 10);

    $("#itemRoleEndDate_" + id).datepicker({
        changeMonth: true,
        changeYear: true,
        minDate: '-116Y',
        dateFormat: 'dd/mm/yy',
        OnSelect: function (selected) {
            $("#itemRoleStartDate_" + id).datepicker("option", "maxDate", selected);
        }
    }).attr("maxlength", 10);
}

function InitialiseDatePickerImage() {
    $(".dateCalImg").click(function () {
        var inputControl = $(this).parent().prev().children().first();
        if ($(this).hasClass("newItemRole")) {
            inputControl = $(this).parent().prev();
        }

        if (inputControl.is(":disabled")) return false;

        if (inputControl.hasClass("hasCalControl")) {
            var pickerDiv = $(this).hasClass("dateCalImg") ? "#ui-datepicker-div" : "#ui-timepicker-div";

            if ($(pickerDiv).css("display") === "none") {
                inputControl.focus();
            }
            else {
                if ($(pickerDiv).is(":animated") === false) {
                    $(pickerDiv).fadeOut(100);
                }
            }
        }
        else {
            inputControl.focus();
        }
    });
}

function AddStartAndEndDateColumns() {
    var gridHeader = $("#gridNewItemRoles tr:first-child");
    gridHeader.append("<th>Start Date</th><th>End Date</th>");

    var gridBody = $("#gridNewItemRoles tr:not(:first-child)");
    var startDatefield = "<td><input type='text' class='itemRoleStartDate fillspan hasCalControl'><span class='inputicon'><img class='dateCalImg newItemRole' src='/shared/images/icons/cal.gif' style='border-width:0px;'></span></td>";
    var endDatefield = "<td><input type='text' class='itemRoleEndDate fillspan hasCalControl'><span class='inputicon'><img class='dateCalImg newItemRole' src='/shared/images/icons/cal.gif' style='border-width:0px;'></span></td>";

    gridBody.append(startDatefield + endDatefield);
    gridBody.each(function () {
        var id = $(this).find("input[name=selectgridNewItemRoles]").val();
        $(this).find(".itemRoleStartDate").attr("id", "itemRoleStartDate_" + id).parent().addClass("itemRoleWidth");
        $(this).find(".itemRoleEndDate").attr("id", "itemRoleEndDate_" + id).parent().addClass("itemRoleWidth");
        InitialiseStartAndEndDatePickers(id);
    });

    InitialiseDatePickerImage();
}

function RemoveSelectedItemRoles() {
    SEL.Ajax.Service('/shared/webServices/svcEmployees.asmx/',
        'GetEmployeeItemRoles',
        { employeeId: nEmployeeid },
        function (data) {
            $(data.d).each(function () {
                $("#tbl_gridNewItemRoles_" + this.ItemRoleId).remove();
            });

            var rows = $('#gridNewItemRoles tr:not(:first-child)');
            rows.find('td').removeClass('row1').removeClass('row2');
            rows.filter(':even').find('td').addClass("row1");
            rows.filter(':odd').find('td').addClass("row2");
        });
}

function populateNewItemRoles() {
    PageMethods.createNewItemRoleGrid(nEmployeeid, populateNewItemRolesComplete);
}

function populateNewItemRolesComplete(data) {

    if ($e(pnlItemRoleListGrid) === true) {

        $("#" + pnlItemRoleListGrid).hide();
        $g(pnlItemRoleListGrid).innerHTML = data[2];

        AddStartAndEndDateColumns();
        RemoveSelectedItemRoles();

        SEL.Grid.updateGrid(data[1]);
        $("#" + pnlItemRoleListGrid).show();

        var modal = $('#ctl00_contentmain_pnlNewItemRoles');
        modal.css("left", ($(window).width() - modal.width()) / 2 + "px");
    }
}

function showNewItemRoleModal(save) {
    currentAction = 'saveItemRole';
    if (save) {
        saveEmployee(true, false);
        if (nEmployeeid == 0) {
            return;
        }

    }
    var modal = $find(modNewItemRoles);
    modal.show();
}

function editItemRole(itemRoleId, employeeId) {
    var params = { itemRoleId: itemRoleId, employeeId: employeeId };
    SEL.Ajax.Service('/shared/webServices/svcEmployees.asmx/', 'GetEmployeeItemRole', params, function(data) {
        var modal = $find(modEditItemRole);
        $("#" + itemRoleValue).val(data.d.ItemRoleId).attr("empId", employeeId).html(data.d.ItemRoleName);
        if (data.d.StartDate === null) {
            $("#" + txtItemRoleStartDate).val('');
        } else {
            $("#" + txtItemRoleStartDate).val(toDateFromJson(data.d.StartDate));
            $("#" + txtItemRoleEndDate).datepicker("option", "minDate", toDateFromJson(data.d.StartDate));
        }
        if (data.d.EndDate === null) {
            $("#" + txtItemRoleEndDate).val('');
        } else {
            $("#" + txtItemRoleEndDate).val(toDateFromJson(data.d.EndDate));
        }
        modal.show(); 
    }, function() {
        SEL.MasterPopup.ShowMasterPopup("An error occured retrieving the item role.");
        
    });  
}

function updateItemRole() {
    var emp = $("#" + itemRoleValue).attr("empId");
    var startDate = $("#" + txtItemRoleStartDate).val();
    var endDate = $("#" + txtItemRoleEndDate).val();

    if (ValidDates(startDate, endDate)) {
        var params = {
            employeeId: emp,
            itemRoleId: $("#" + itemRoleValue).val(),
            startDate: startDate,
            endDate: endDate
        };

        SEL.Ajax.Service('/shared/webServices/svcEmployees.asmx/',
            'UpdateEmployeeItemRole',
            params,
            UpdateItemRoleComplete,
            function() {
                SEL.MasterPopup.ShowMasterPopup("An error occured when updating the item role.");
            });
    }

}

function UpdateItemRoleComplete() {
    var modal = $find(modEditItemRole);
    modal.hide();
    populateItemRoles();
}

function hideItemRole() {
    var modal = $find(modEditItemRole);
    modal.hide();
}

function hideNewItemRoleModal() {
    var modal = $find(modNewItemRoles);
    modal.hide();
}

function saveItemRoles() {
    var selectedRoles = $('#gridNewItemRoles td input[name=selectgridNewItemRoles]:checked');
    var roles = [];
    selectedRoles.each(function () {
        var txtStartDate = $(this).parent().parent().find(".itemRoleStartDate").first().val();
        var txtEndDate = $(this).parent().parent().find(".itemRoleEndDate").last().val();
        if (ValidDates(txtStartDate, txtEndDate)) {
            var startdate = FormatDate(txtStartDate);
            var enddate = FormatDate(txtEndDate);
            var itemroleid = $(this).val();
            roles.push({
                StartDate: startdate,
                EndDate: enddate,
                ItemRoleId: itemroleid

            });
        } else {
            return;
        }
    });
    if (selectedRoles.length === roles.length) {
        PageMethods.saveItemRoles(nEmployeeid, roles, saveItemRolesComplete);
    }

}

function saveItemRolesComplete() {
    populateItemRoles();
    hideNewItemRoleModal();
}

function deleteItemRole(roleId) {
    if (confirm('Are you sure you wish to remove the selected role?')) {
        PageMethods.deleteItemRole(nEmployeeid, roleId, deleteItemRoleComplete);

    }
}

function deleteItemRoleComplete() {
    populateItemRoles();  
}

function populateItemRoles() {
    PageMethods.CreateItemRoleGrid(nEmployeeid, populateItemRolesComplete);
}

function populateItemRolesComplete(data) {
    if ($e(pnlItemRolesGrid) == true) {
        $g(pnlItemRolesGrid).innerHTML = data[2];
        SEL.Grid.updateGrid(data[1]);
    }
}

function addEsrAssignment()
{
    ClearESRModal();
    esrassignmentid = 0;

    showESRModal(true);
}

function showESRModal(save)
{
    currentAction = 'saveESRAssignment';
    if (save) {
        saveEmployee(false, false);
        return;

    }
    var modal = $find(modesrassignment);
    modal.show();
}

function hideESRAssignmentModal() {
    var modal = $find(modesrassignment);
    modal.hide();
}

function populateESRAssignments() {
    PageMethods.createESRAssignmentGrid(nEmployeeid, populateESRAssignmentsComplete);
}

function populateESRAssignmentsComplete(data) {
    if ($e(pnlESRAssignmentsGrid) === true) {
        var grid = $g(pnlESRAssignmentsGrid);
        grid.innerHTML = data[2];
        SEL.Grid.updateGrid(data[1]);
    }
}

function saveESRAssignment() {
    if (validateform('vgESR') == false)
        return;

    var esractive = document.getElementById(chkesractive).checked;
    var esrprimary = document.getElementById(chkesrprimary).checked;
    var assignmentnumber = document.getElementById(txtassignmentnumber).value;
    var esrstartdate = null;
    if (document.getElementById(txtesrstartdate).value != '') {
        esrstartdate = document.getElementById(txtesrstartdate).value.substring(6, 10) + "/" + document.getElementById(txtesrstartdate).value.substring(3, 5) + "/" + document.getElementById(txtesrstartdate).value.substring(0, 2);
    }
    var esrenddate = null;
    if (document.getElementById(txtesrenddate).value != '') {
        esrenddate = document.getElementById(txtesrenddate).value.substring(6, 10) + "/" + document.getElementById(txtesrenddate).value.substring(3, 5) + "/" + document.getElementById(txtesrenddate).value.substring(0, 2);
    }
    PageMethods.saveESRAssignment(esrassignmentid, nEmployeeid, assignmentnumber, esractive, esrprimary, esrstartdate, esrenddate, $("#" +txtSignOffOwner + "_ID").val(), saveESRAssignmentComplete);
}

function saveESRAssignmentComplete(data) {
    switch(data)
    {
        case -1:
            SEL.MasterPopup.ShowMasterPopup("The Assignment Number you have entered already exists.");
            return;
        case -2:
            SEL.MasterPopup.ShowMasterPopup("The Assignment Supervisor you have selected could not be found.");
            return;
        case -3:
            SEL.MasterPopup.ShowMasterPopup("The Assignment Supervisor you have selected does not have an Assignment Number.");
            return;
    }

    populateESRAssignments();
    hideESRAssignmentModal();
}

function deleteESRAssignment(id) {
    if (confirm('Are you sure you wish to delete the selected ESR Assignment?')) {
        PageMethods.deleteESRAssignment(nEmployeeid, id, deleteESRAssignmentComplete);
    }
}

function deleteESRAssignmentComplete(data) {
    populateESRAssignments();
}

function editESRAssignment(id) {
    ClearESRModal();

    esrassignmentid = id;

    if (esrassignmentid > 0)
    {
        PageMethods.getESRAssignment(nEmployeeid, esrassignmentid, getESRAssignmentComplete);
    }
}

function getESRAssignmentComplete(data)
{
    document.getElementById(chkesractive).checked = data.active;
    esrprimary = document.getElementById(chkesrprimary).checked = data.primaryassignment;
    document.getElementById(txtassignmentnumber).value = data.assignmentnumber;

    if (data.earliestassignmentstartdate != null) {
        document.getElementById(txtesrstartdate).value = data.earliestassignmentstartdate.format('dd/MM/yyyy');
    }
    if (data.finalassignmentenddate != null) {
        document.getElementById(txtesrenddate).value = data.finalassignmentenddate.format('dd/MM/yyyy');
    }

    if (data.SignOffOwner != null && data.SignOffOwner.CombinedItemKey != null) {
        var combinedItemKey = data.SignOffOwner.CombinedItemKey.split(',');
        var itemDefinition = null;
        switch (parseInt(combinedItemKey[0]))
        {
            case 11: // BudgetHolder
                itemDefinition = data.SignOffOwner.budgetholder + " (Budget Holder)";
                break;
            case 25: // Employee
                itemDefinition = data.SignOffOwner.FullName + " (" + data.SignOffOwner.Username + ") (Employee)";
                break;
            case 49: // Team
                itemDefinition = data.SignOffOwner.teamname + " (Team)";
                break;
        }

        if(itemDefinition != null)
        {
            $("#" + txtSignOffOwner).val(itemDefinition);
            $("#" + txtSignOffOwner + "_ID").val(data.SignOffOwner.CombinedItemKey);
        }
    }

    showESRModal(false);
}

function ClearESRModal()
{
    document.getElementById(txtassignmentnumber).value = "";
    document.getElementById(chkesractive).checked = false;
    document.getElementById(chkesrprimary).checked = false;
    document.getElementById(txtesrstartdate).value = "";
    document.getElementById(txtesrenddate).value = "";
    $("#" +txtSignOffOwner).val("");
    $("#" +txtSignOffOwner + "_ID").val("");

    return;
}


function showEsrDetailsModal(esrObjectType, esrId)
{
    var detailLabel = document.getElementById(lblEsrDetailsTitleID);
    var panelHeight = 610;
    if (detailLabel != null)
    {
        var esrObjectName = '';
        switch (esrObjectType)
        {
            case 1:
                esrObjectName = 'ESR Person Record';
                break;
            case 2:
                esrObjectName = 'ESR Assignment Record';
                break;
            case 3:
                esrObjectName = 'ESR Address Record';
                panelHeight = 410;
                break;
            case 4:
                esrObjectName = 'ESR Location Record';
                panelHeight = 530;
                break;
            case 5:
                esrObjectName = 'ESR Vehicle Record';
                panelHeight = 410;
                break;
            default:
        }
        detailLabel.innerText = esrObjectName + ' Details';
        var detailDiv = document.getElementById(window.divEsrDetailsID);
        if (detailDiv != null) {
            detailDiv.innerHTML = '';
        }
        $find(modEsrDetailsID).show();
        $.ajax({
            url: appPath + '/shared/webServices/svcEmployees.asmx/GetEsrDetails',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: '{ "esrObjectType":' + esrObjectType + ', "esrId": ' + esrId + '}',
            success: function (r)
            {
                var returnValue = r.d;
                var detailDiv = document.getElementById(window.divEsrDetailsID);
                if (detailDiv != null)
                {
                    detailDiv.innerHTML = returnValue;
                }

                $('#' + window.pnlEsrDetailsID).css({ 'height': panelHeight + 'px' });
                $('#' + window.divEsrDetailsID).css({ 'height': panelHeight - 80 + 'px'});


            },
            error: function (r) {
                var error;
                if (r.responseJSON) {
                    error = WebMethodValidatorError;
                    error.ID = source.id;
                    error.ControlToValidate = source.controltovalidate;
                    error.ValidatorErrorMessage = source.errormessage;
                    error.ServerError = r.responseJSON.Message;
                    WebMethodValidatorErrors[error.ID] = error;
                }
                else {
                    error = WebMethodValidatorError;
                    error.ID = source.id;
                    error.ControlToValidate = source.controltovalidate;
                    error.ValidatorErrorMessage = source.errormessage;
                    var responseJSON = $.parseJSON(r.responseText);
                    error.ServerError = (typeof responseJSON === 'object' && responseJSON.hasOwnProperty('Message')) ? responseJSON.Message : 'An error occurred, but no message was returned';
                    WebMethodValidatorErrors[error.ID] = error;
                }
                args.IsValid = true;
            }
        });
    }



}

function hideEsrDetailsModal() {
    $find(modEsrDetailsID).hide();
}

function checkSendPasswordEmail() {
    var chkSendPasswordKeyObj = document.getElementById(chkSendPasswordKey);
    var chkWelcomeEmailObj = document.getElementById(chkWelcomeEmail);
    if (chkSendPasswordKeyObj.checked == false) {
        chkWelcomeEmailObj.disabled = true;
        chkWelcomeEmailObj.checked = false;
    } else {
        chkWelcomeEmailObj.disabled = false;
        chkWelcomeEmailObj.checked = true;
    }

}
function repopulateGrid() {
    var cmbFilterVal = new Number(document.getElementById(cmbFilterID).options[document.getElementById(cmbFilterID).selectedIndex].value);

    PageMethods.createEmployeeGrid(0,username, surname, groupid, costcodeid, departmentid, roleid, cmbFilterVal, repopulateGridComplete, commandFail);
}

function repopulateGridComplete(data) {
    if ($e(pnlGridID) === true) {
        $g(pnlGridID).innerHTML = data[2];
        SEL.Grid.updateGrid(data[1]);
    }
}

function commandFail(error) {
    if (error._message != null) {
        SEL.MasterPopup.ShowMasterPopup(error._message, "Web Service Message");
    }
    else {
        SEL.MasterPopup.ShowMasterPopup(error, "Web Service Message");
    }
    return;
}

function checkEmployee(empAreaType)
{
    //Employee Licence checker
    var txtbox;
    var employee;

    switch (empAreaType)
    {
        //Budget Employee
        case 5:
            txtbox = document.getElementById(txtbudgetemployee);

            if (txtbox != undefined)
            {
                employee = txtbox.value;

                if (employee != "")
                {
                    Spend_Management.svcAutoComplete.checkEmployee(employee, 5, checkEmployeeComplete, commandFail);
                    return;
                }
            }
            break;

            //Float Employee
        case 6:
            txtbox = document.getElementById(txtfloatemployee);

            if (txtbox != undefined)
            {
                employee = txtbox.value;

                if (employee != "")
                {
                    Spend_Management.svcAutoComplete.checkEmployee(employee, 6, checkEmployeeComplete, commandFail);
                    return;
                }
            }
            break;

            //Employee Login

        case 7:
            txtbox = document.getElementById(txtemploginemployee);

            if (txtbox != undefined)
            {
                employee = txtbox.value;

                if (employee != "")
                {
                    Spend_Management.svcAutoComplete.checkEmployee(employee, 7, checkEmployeeComplete, commandFail);
                    return;
                }
            }
            break;
    }
}

function checkEmployeeComplete(data) {
    switch (data[1]) {
        case 5:
            txtbox = document.getElementById(txtbudgetemployeeid);
            setEmployeeIDValue(txtbox, data[0]);
            break;
        case 6:
            txtbox = document.getElementById(txtfloatemployeeid);
            setEmployeeIDValue(txtbox, data[0]);
            break;
        case 7:
            txtbox = document.getElementById(txtemploginemployeeid);
            setEmployeeIDValue(txtbox, data[0]);
            break;
    }
}

function setEmployeeIDValue(txtbox, data)
{
    if (txtbox != undefined)
    {
        txtbox.value = data;
    }
}

function CheckEmployeeHasAnESRAssignment(sender, args) {
    if (nEmployeeid === 0) {
        args.IsValid = false;
    }
    else {
        var esrGrid = SEL.Grid.getTableById('ESRAssignmentGrid');

        if (esrGrid !== null) {
            if (esrGrid.rows.length > 2) {
                args.IsValid = true;
            }
            else {
                if (esrGrid.rows[1].innerText == "There are not currently any assignments associated") {
                    args.IsValid = false;
                }
            }

        }
    }
    return;
}

function hideTab(TabId) {
    var tabControlId = TabId + '_tab';

    var tab = document.getElementById(tabControlId);
    if (tab != null) {
        tab.style.display = 'none';
    }
}

// Used in the custom validator of signoff groups
function ValidateSignoff(source, args)
{
    if (bAllowSelfInSignoffStages)
    {
        args.IsValid = true;
    }
    else
    {
        $.ajax({
            url: 'aeemployee.aspx/ValidateSignoff',
            async: false,
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: '{ "groupID":' + args.Value + ', "employeeID": ' + nEmployeeid + '}',
            success: function (r)
            {
                args.IsValid = r.d;
            },
            error: function (r)
            {
                var error;
                if (r.responseJSON)
                {
                    error = WebMethodValidatorError;
                    error.ID = source.id;
                    error.ControlToValidate = source.controltovalidate;
                    error.ValidatorErrorMessage = source.errormessage;
                    error.ServerError = r.responseJSON.Message;
                    WebMethodValidatorErrors[error.ID] = error;
                }
                else
                {
                    error = WebMethodValidatorError;
                    error.ID = source.id;
                    error.ControlToValidate = source.controltovalidate;
                    error.ValidatorErrorMessage = source.errormessage;
                    var responseJSON = $.parseJSON(r.responseText);
                    error.ServerError = (typeof responseJSON === 'object' && responseJSON.hasOwnProperty('Message')) ? responseJSON.Message : 'An error occurred, but no message was returned';
                    WebMethodValidatorErrors[error.ID] = error;
                }
                args.IsValid = true;
            }
        });
    }
}

function showMobileDevice(save) {
    currentAction = 'saveMobileDevice';
    if (save)
    {
        saveEmployee(true, false);
        return;
    }

    SEL.MobileDevices.LoadMobileDeviceModal(SEL.MobileDevices.LoadType.New, null);
}

function showBankAccount(save) {
    currentAction = 'saveBankAccount';
    if (save) {
        saveEmployee(true, false);
        return;
    }

    // Force the value of the currency dropdown list as the default currency of the bank account in case the user changes it before clicking new bank account
    $ddlSetSelected(SEL.BankAccounts.ModalWindowddlCurrency, null, parseInt($g(cmbcurrency).options[$g(cmbcurrency).selectedIndex].value));
    SEL.BankAccounts.LoadBankAccountModal(SEL.BankAccounts.LoadType.New, null);
}

function changeLockedStatus(employeeid) {
    nEmployeeid = employeeid;
    Spend_Management.svcEmployees.changeLockStatus(employeeid, changeLockedStatusComplete, commandFail);

}

function changeLockedStatusComplete(data) {
    var cell = SEL.Grid.getCellById('gridEmployees', nEmployeeid, 'Locked');
    if (cell.innerHTML.indexOf('Unlock') != -1) {
        cell.innerHTML = "<a ></a>";
    }
    else {
        cell.innerHTML = "<a href='javascript:changeLockedStatus(" + nEmployeeid + ");'><img title='Unlock Account' src='/static/icons/16/new-icons/lock.png'></a>";
    }
}

// ==================================================================================================================================================================
// This section should no longer be used. We need to refactor existing code to reference sel.selectinator.js until a control is created.
// ==================================================================================================================================================================
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
    var containerSelector = this.selectors.grid;
    var modalSelector = this.selectors.modal;

    $.ajax({
        dataType: "json",
        contentType: "application/json",
        url: appPath + "/shared/webServices/svcAutoComplete.asmx/Get" + this.name.replace(/[^a-zA-Z]/gi, "") + "SearchGrid",
        method: "post",
        success: function (data, status, jqXhr) {
            if (typeof data !== "undefined" && data !== null && typeof data.d !== "undefined" && data.d !== null) {
                $(containerSelector).html(data.d[1] + "<script>SEL.Grid.updateGrid('" + data.d[0] + "');</script>");
                $f(modalSelector).show();
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
    $f(this.selectors.modal).hide();
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
