
function deleteCostcode(costcodeid) {

    currentRowID = costcodeid;
    if (confirm('Are you sure you wish to delete the selected cost code?')) {
        PageMethods.deleteCostcode(accountid, costcodeid, deleteCostcodeComplete, commandFail);

    }
}

function deleteCostcodeComplete(data) {
    switch (data) {
        case -1:
            SEL.MasterPopup.ShowMasterPopup('This Cost Code cannot be deleted as it is assigned to one or more Signoff Stages.', 'Message from ' + moduleNameHTML);
            break;
        case -2:
            SEL.MasterPopup.ShowMasterPopup('This Cost Code cannot be deleted as it is assigned to one or more Employees.', 'Message from ' + moduleNameHTML);
            break;
        case -4:
            SEL.MasterPopup.ShowMasterPopup('This Cost Code cannot be deleted as it is assigned to one or more Expense Items.', 'Message from ' + moduleNameHTML);
            break;
        case -10:
            SEL.MasterPopup.ShowMasterPopup('This Cost Code cannot be deleted as it is assigned to one or more GreenLights or User Defined Fields.', 'Message from ' + moduleNameHTML);
            break;
        default:
            SEL.Grid.deleteGridRow('gridCostcodes', currentRowID);
            break;
    }
}
function changeArchiveStatus(costcodeid) {
    currentRowID = costcodeid;
    PageMethods.changeStatus(accountid, costcodeid, changeStatusComplete, commandFail)
}

function changeStatusComplete(data) 
{
    if (data == -1) 
    {
        SEL.MasterPopup.ShowMasterPopup('This costcode cannot be archived as it is assigned to one or more signoff stages.', 'Message from ' + moduleNameHTML);
    }
    else
     {
        var cell = SEL.Grid.getCellById('gridCostcodes', currentRowID, 'archiveStatus');
        if (cell.innerHTML.indexOf('Un-Archive') != -1) {
            cell.innerHTML = "<a href='javascript:changeArchiveStatus(" + currentRowID + ");'><img title='Archive' src='/shared/images/icons/folder_lock.png'></a>";
        }
        else {
            cell.innerHTML = "<a href='javascript:changeArchiveStatus(" + currentRowID + ");'><img title='Un-Archive' src='/shared/images/icons/folder_into.gif'></a>";
        }
        
    }
}

function repopulateGrid()
{
    var cmbFilterVal = document.getElementById(cmbFilterID).options[document.getElementById(cmbFilterID).selectedIndex].value;

    PageMethods.CreateGrid(cmbFilterVal, repopulateGridComplete, commandFail);
}

function repopulateGridComplete(data)
{
    var pnlGrid = $g(pnlGridID);

    if (pnlGrid != undefined)
    {
        pnlGrid.innerHTML = data[1];
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
