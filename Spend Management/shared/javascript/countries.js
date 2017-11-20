var currentAction;
var cmbsubcat;
var countrysubcat = 0;

function saveCountry() 
{
    if (validateform('vgMain') == false) 
    {
        return false;
    }
    
    var cmbcountry = document.getElementById(cmbcountryid);
    var globalcountryid = cmbcountry.options[cmbcountry.selectedIndex].value;

    PageMethods.saveCountry(countryid, globalcountryid, saveCountryComplete, commandFail);
}

function saveCountryComplete(data) 
{
    if (data == -1) 
    {
        alert('The country you have provided already exists.');
        return;
    }

    countryid = data;
    
    switch (currentAction) {
        case 'addSubcat':
            showSubcatModal();
            break;
    }
}

function saveCountrySubcat() 
{
    if (validateform('vgSubcat') == false) 
    {
        return;
    }

    cmbsubcat = document.getElementById(cmbsubcatid);
    var subcatid = cmbsubcat.options[cmbsubcat.selectedIndex].value;
    cmbsubcat.remove(cmbsubcat.selectedIndex);
    cmbsubcat.enabled = true;
    var vatrate = document.getElementById(txtvatrateid).value;
    var claimablepercentage = document.getElementById(txtclaimablepercentageid).value;

    PageMethods.saveCountrySubcat(countryid, countrysubcat, subcatid, vatrate, claimablepercentage, saveCountrySubcatComplete, commandFail);
}

function saveCountrySubcatComplete() 
{

    hideSubcatModal();
    populateVATRates()
}

function showSubcatModal() 
{
    if (countryid == 0) {
        currentAction = 'addSubcat';
        saveCountry();
        return;
    }
    cmbsubcat = document.getElementById(cmbsubcatid);
    cmbsubcat.disabled = false;
    cmbsubcat.selectedIndex = 0;
    document.getElementById(txtvatrateid).value = '';
    document.getElementById(txtclaimablepercentageid).value = '';
    
    var modal = $find(modsubcatid);
    modal.show();

}

function hideSubcatModal() 
{
    $find(modsubcatid).hide();
}

function showEditSubcatModal(countrysubcatid, subcat, subcatid, vat, vatpercent) 
{
    countrysubcat = countrysubcatid;
    cmbsubcat = document.getElementById(cmbsubcatid);
    
    var optn;
    optn = document.createElement("OPTION");
    optn.text = subcat;
    optn.value = subcatid;
    cmbsubcat.options.add(optn);
    
    for (var i = 0; i < cmbsubcat.options.length; i++) 
    {
        if (cmbsubcat.options[i].value == subcatid) 
        {
            cmbsubcat.selectedIndex = i;
        }
    }
    cmbsubcat.disabled = true;

    document.getElementById(txtvatrateid).value = vat;
    document.getElementById(txtclaimablepercentageid).value = vatpercent;

    var modal = $find(modsubcatid);
    modal.show();
}

function deleteCountry(countryid) 
{
    currentRowID = countryid;
    if (confirm('Are you sure you wish to delete the selected country?')) 
    {
        PageMethods.deleteCountry(countryid, deleteCountryComplete, commandFail);
    }
}

function deleteCountryComplete(data) 
{
    if (data == 1) 
    {
        alert('The selected country cannot be deleted as it is currently set as the primary country for one or more employees');
        
        return;
    }
    if (data == 2) 
    {
        alert('The selected country cannot be deleted as it has been used on one or more expense claims');
        return;
    }
    if (data == 3) {
        alert('The selected country cannot be deleted as it has been used on one or more suppliers');
        return;
    }
    SEL.Grid.deleteGridRow('gridCountries', currentRowID);
}

function deleteCountrySubcat(countrysubcatid) 
{
    currentRowID = countrysubcatid;
    PageMethods.deleteCountrySubcat(countrysubcatid, deleteCountrySubcatComplete, commandFail);
}

function deleteCountrySubcatComplete() 
{
    SEL.Grid.deleteGridRow('gridCountrySubcats', currentRowID);
    cmbsubcat = document.getElementById(cmbsubcatid);
    var subcatid = cmbsubcat.options[cmbsubcat.selectedIndex].value;
    refreshSubcats(subcatid);
}

function changeArchiveStatus(countryid) 
{
    currentRowID = countryid;
    PageMethods.changeStatus(countryid, changeStatusComplete, commandFail)
}

function changeStatusComplete(data) 
{
    switch (data) {
        case 1:
            alert('The selected country cannot be archived as it is currently set as the primary country for one or more employees');
            break;
        case 2:
            alert('The selected country cannot be archived as it has been used on one or more expense claims');
            break;
        case 3:
            alert('The selected country cannot be archived as it has been used on one or more suppliers');
            break;
        default:
            var cell = SEL.Grid.getCellById('gridCountries', currentRowID, 'archiveStatus');
            if (cell.innerHTML.indexOf('Un-Archive') != -1) {
                cell.innerHTML = "<a href='javascript:changeArchiveStatus(" + currentRowID + ");'><img title='Archive' src='/shared/images/icons/folder_lock.png'></a>"
            }
            else {
                cell.innerHTML = "<a href='javascript:changeArchiveStatus(" + currentRowID + ");'><img title='Un-Archive' src='/shared/images/icons/folder_into.gif'></a>"
            }
            break;
    }
    return;
}

function refreshSubcats(subcatid) 
{
    PageMethods.refreshSubcatDropdown(countryid, subcatid, refreshSubcatsComplete, commandFail)
}

function refreshSubcatsComplete(data) 
{
    cmbsubcat = document.getElementById(cmbsubcatid);

    if (cmbsubcat == null) 
    {
        return;
    }
    else 
    {
        cmbsubcat.options.length = 0;

        for (var j = 0; j < data.length; j++) 
        {
            cmbsubcat.options[j] = new Option(data[j][0], data[j][1]);
        }
    }
}

function getCountryCode() 
{
    var cmbcountry = document.getElementById(cmbcountryid);
    var globalcountryid = cmbcountry.options[cmbcountry.selectedIndex].value;
    
    PageMethods.getCountryCode(globalcountryid, getCountryCodeComplete, commandFail)
}

function getCountryCodeComplete(data) 
{
    var txtcountrycode = document.getElementById(txtcountrycodeid);
    var txtNumeric3Object = document.getElementById(txtNumeric3);
    var txtAlpha3Object = document.getElementById(txtAlpha3Country);
    var chkPostcodeAnywhereEnabledObject = document.getElementById(chkPostcodeAnywhereEnabled);

    txtcountrycode.value = data[0];
    txtAlpha3Object.value = data[1];
    if (data[2] < 10) 
    {
        data[2] = "00" + data[2];
    } 
    else if(data[2] < 100) 
    {
        data[2] = "0" + data[2];
    }
    txtNumeric3Object.value = data[2];
    chkPostcodeAnywhereEnabledObject.checked = (data[3] === "True");
}

function populateVATRates() {
    PageMethods.CreateGrid(countryid, populateVATRatesComplete);
}

function populateVATRatesComplete(data) {
    if ($e(ratesGrid) == true) {
        $g(ratesGrid).innerHTML = data[2];
        SEL.Grid.updateGrid(data[1]);
    }
}

function repopulateGrid()
{
    var cmbFilterVal = document.getElementById(cmbFilterID).options[document.getElementById(cmbFilterID).selectedIndex].value;

    PageMethods.CreateGrid(cmbFilterVal, repopulateGridComplete, commandFail);
}

function repopulateGridComplete(data) {
    if ($e(pnlGridID) === true) {
        $g(pnlGridID).innerHTML = data[1];
        SEL.Grid.updateGrid(data[0]);
    }
}

function commandFail(error)
{
    if (error["_message"] != null)
    {
        SEL.MasterPopup.ShowMasterPopup(error["_message"], "Web Service Message");
    }
    else
    {
        SEL.MasterPopup.ShowMasterPopup(error, "Web Service Message");
    }
    return;
}
