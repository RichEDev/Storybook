function deleteCurrency(currencyid) 
{
    currentRowID = currencyid;
    if (confirm('Are you sure you wish to delete the selected currency?')) 
    {
        PageMethods.deleteCurrency(currencyid, deleteCurrencyComplete, commandFail);
    }
}

function deleteCurrencyComplete(data)
{
    if (data == 1)
    {
        alert('The selected currency cannot be deleted as it is currently set as the primary currency for your company.');
        return;
    }
    if (data == 2)
    {
        alert('The selected currency cannot be deleted as it has been used on one or more expense claims.');
        return;
    }
    if (data == 3)
    {
        alert('The selected currency cannot be deleted as it has been used on one or more contracts.');
        return;
    }
    if (data == 4)
    {
        alert('The selected currency cannot be deleted as it has been used on one or more supplier records.');
        return;
    }
    if (data == 5)
    {
        alert('The selected currency cannot be deleted as it has been used on one or more contract product records.');
        return;
    }
    if (data == 6)
    {
        alert('The selected currency cannot be deleted as it has been used on one or more GreenLight monetary records.');
        return;
    }
    if (data == 7)
    {
        alert('The selected currency cannot be deleted as it has been used on one or more mobile expense items.');
        return;
    }
    if (data == 8)
    {
        alert('The selected currency cannot be deleted as an error has occurred.');
        return;
    }
    if (data == 9) {
        alert('The selected currency cannot be deleted as it has been used on one or more bank accounts.');
        return;
    }

    SEL.Grid.deleteGridRow('gridCurrencies', currentRowID);
}

function deleteMonthlyCurrency(currencyid, currencymonthid) 
{
    currentRowID = currencymonthid;
    if (confirm('Are you sure you wish to delete the selected currency month?')) 
    {
        SEL.Grid.deleteGridRow('gridMonthlyCurrencies', currentRowID);
        PageMethods.deleteCurrencyMonth(currencyid, currencymonthid, null, commandFail);
    }
}

function deleteRangedCurrency(currencyid, currencyrangeid) 
{
    currentRowID = currencyrangeid;
    if (confirm('Are you sure you wish to delete the selected currency range?')) 
    {
        SEL.Grid.deleteGridRow('gridRangedCurrencies', currentRowID);
        PageMethods.deleteCurrencyRange(currencyid, currencyrangeid, null, commandFail);
    }
}

function changeArchiveStatus(currencyid) 
{
    currentRowID = currencyid;
    PageMethods.changeStatus(currencyid, changeStatusComplete, commandFail);
}

function changeStatusComplete(data) 
{

    if (data == 1) 
    {
        alert('The selected currency cannot be archived as it is currently set as the primary currency for your company');
        return;
    }

    if (data == 2) {
        alert('The selected currency cannot be archived as it has been used on one or more contracts');
        return;
    }
    if (data == 3) {
        alert('The selected currency cannot be archived as it has been used on one or more supplier records');
        return;
    }
    if (data == 4)
    {
        alert('The selected currency cannot be archived as it has been used on one or more contract product records');
        return;
    }
    if (data == 9) {
        alert('The selected currency cannot be archived as it has been used on one or more bank accounts.');
        return;
    }

    var cell = SEL.Grid.getCellById('gridCurrencies', currentRowID, 'archiveStatus');
    if (cell.innerHTML.indexOf('Un-Archive') != -1) {
        cell.innerHTML = "<a href='javascript:changeArchiveStatus(" + currentRowID + ");'><img title='Archive' src='/shared/images/icons/folder_lock.png'></a>";
    }
    else 
    {
        cell.innerHTML = "<a href='javascript:changeArchiveStatus(" + currentRowID + ");'><img title='Un-Archive' src='/shared/images/icons/folder_into.gif'></a>";
    }
}

function saveCurrencyType(currType) 
{
    currencyType = currType;
    PageMethods.saveCurrencyType(currencyType);
}

function addCurrency() 
{
    window.location = "aecurrency.aspx?currencyType=" + currencyType;
}

function editCurrency(currencyid) 
{
    window.location = "aecurrency.aspx?currencyid=" + currencyid + "&currencyType=" + currencyType;
}

function getCurrencyDetails() 
{
    var cmbcurrency = document.getElementById(cmbcurrencyid);
    var globalcurrencyid = cmbcurrency.options[cmbcurrency.selectedIndex].value;

    PageMethods.getCurrencyDetails(globalcurrencyid, getCurrencyDetailsComplete, commandFail);
}

function getCurrencyDetailsComplete(data) 
{
    var txtalphacode = document.getElementById(txtalphacodeid);
    var txtnumericcode = document.getElementById(txtnumericcodeid);
    var txtsymbol = document.getElementById(txtsymbolid);
    
    txtalphacode.value = data[0];
    txtnumericcode.value = data[1];
    txtsymbol.value = data[2];
}

function getExchangeStatus(sender, args) 
{
    if (currencyExists) 
    {
        args.IsValid = false;
        return;
    }

    args.IsValid = true;
}

function checkExchange()
{
    switch (currencyType) 
    {
        case 2:
            var month = $get(cmbmonthid).value;
            var year = $get(txtyearid).value;

            PageMethods.checkMonthExchangeExists(currencyid, month, year, checkExchangeComplete, commandFail);
            break;

        case 3:
            var startdate = $get(txtstartdateid).value;
            var enddate = $get(txtenddateid).value;
            PageMethods.checkRangeExchangeExists(currencyid, startdate, enddate, checkExchangeComplete, commandFail);
            break;
    }
}

function checkExchangeComplete(val) 
{
    if (val == true) 
    {
        switch (currencyType) {
            case 2:
                currencyExists = true;
                //alert('This currency month already exists');
                return;
            case 3:
                currencyExists = true;
                //alert('This currency range already exists');
                return;
        }
    }
}

function saveCurrencyExchanges() 
{
    if (validateform(null) == false)
    {
        return;
    }
    
    var exchangeRates = new Array();
    var id;
    
    for (var i = 0; i < lstCurrencyIDs.length; i++)
    {
        id = lstCurrencyIDs[i];
        exchangeRates[i] = new Array();
        exchangeRates[i][0] = id;
        exchangeRates[i][1] = document.getElementById('exchangerate' + id).value;
    }
    
    switch (currencyType) 
    {
        case 2:
            var month = $get(cmbmonthid).value;
            var year = $get(txtyearid).value;

            PageMethods.saveCurrencyMonth(currencyid, currID, exchangeRates, month, year, saveCurrencyExchangesComplete, commandFail);
            return;
        case 3:
            var startdate = $get(txtstartdateid).value;
            var enddate = $get(txtenddateid).value;

            PageMethods.saveCurrencyRange(currencyid, currID, exchangeRates, startdate, enddate, saveCurrencyExchangesComplete, commandFail);
            return;
    }
}

function saveCurrencyExchangesComplete(val) 
{
    if (val == 1) 
    {
        switch (currencyType) {
            case 2:
                alert('This currency month already exists');
                return;
            case 3:
                alert('This currency range already exists');
                return;
        }
    }

    window.location = 'aecurrency.aspx?currencyid=' + currencyid + '&currencyType=' + currencyType;
}

function repopulateGrid()
{
    var cmbFilterVal = document.getElementById(cmbFilterID).options[document.getElementById(cmbFilterID).selectedIndex].value;
    
    PageMethods.CreateGrid(cmbFilterVal, repopulateGridComplete, commandFail);
}

function repopulateGridComplete(data)
{
    if($e(pnlGridID) === true)
    {
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
