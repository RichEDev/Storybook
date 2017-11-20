var currentAction;
var rateID;
var allowanceID;
function popupRateModal(save) {
    
    if (save) {
        saveAllowance();
        return;
    }
    $find(modRate).show();
    return;
}

function hideRateModal() {
    $find(modRate).hide();
}

function saveAllowance() {
    if (validateform('vgMain') == false) {
        return;
    }
    var name = document.getElementById(txtname).value;
    var description = document.getElementById(txtdescription).value;
    var nighthours = 0;
    if (document.getElementById(txtnighthours).value != '') {
        nighthours = new Number(document.getElementById(txtnighthours).value);
    }
    var nightrate = 0;
    if (document.getElementById(txtnightrate).value != '') {
        nightrate = new Number(document.getElementById(txtnightrate).value);
    }
    var currencyID = 0;
    var currencies
    if (document.getElementById(cmbcurrencies).selectedIndex != -1) {
        currencyID = new Number(document.getElementById(cmbcurrencies).options[document.getElementById(cmbcurrencies).selectedIndex].value);
    }
    
    PageMethods.saveAllowance(allowanceID, name, description, nighthours, nightrate, currencyID, saveAllowanceComplete, errorMessage);
}
function saveAllowanceComplete(data) {
    if (data == -1) {
        alert('This allowance cannot be updated as the allowance name you have entered already exists.');
        return;
    }
    allowanceID = data;
    switch (currentAction) {
        case 'OK':
            document.location = 'adminallowances.aspx';
            break;
        case 'addRate':
            popupRateModal(false);
            break;
    }
}

function saveRate() {
    if (validateform('vgRate') == false) {
        return;
    }
    var rate = new Number(document.getElementById(txtrate).value);
    var hours = new Number(document.getElementById(txthours).value);
    PageMethods.saveRate(rateID, allowanceID, hours, rate, saveRateComplete);
}

function saveRateComplete(data) {
    populateRates();
    hideRateModal();
}
function populateRates() {
    PageMethods.createRatesGrid(allowanceID, populateRatesComplete);
}
function populateRatesComplete(data) {
    if ($e(ratesGrid) == true) {
        $g(ratesGrid).innerHTML = data[1];
        SEL.Grid.updateGrid(data[0]);
    }
}

function editRate(id) {
    rateID = id;
    PageMethods.getRate(id,allowanceID,getRateComplete)
}

function getRateComplete(data) {
    document.getElementById(txtrate).value = data.rate;
    document.getElementById(txthours).value = data.hours;
    popupRateModal(false);
}

function deleteRate(id) {
    rateID = id;
    if (confirm('Are you sure you wish to delete the selected rate?')) {
        PageMethods.deleteRate(id,deleteRateComplete);
    }
}

function deleteRateComplete(data) {
    SEL.Grid.deleteGridRow('gridRates', rateID);
}

function deleteAllowance(allowanceid) {
    allowanceID = allowanceid;
    if (confirm('Are you sure you wish to delete the selected allowance?')) {
        PageMethods.deleteAllowance(allowanceid, deleteAllowanceComplete);

    }
}

function deleteAllowanceComplete(data) {
    if (data == -1) {
        alert('The selected allowance cannot be deleted as it has been claimed for by one or more employees.');
    }
    else if (data == -2) {
        alert('The selected allowance cannot be deleted as it has been claimed for by one or more employees on a mobile expense item.');
    }
    else {
        SEL.Grid.deleteGridRow('gridAllowances', allowanceID);
    }
}

function errorMessage(data)
{
    if (data["_message"] != null)
    {
        SEL.MasterPopup.ShowMasterPopup(data["_message"], "Web Service Message");
    }
    else
    {
        SEL.MasterPopup.ShowMasterPopup(data, "Web Service Message");
    }
    return;
}
