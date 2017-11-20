function deleteProjectcode(projectcodeid) {
    currentRowID = projectcodeid;
    if (confirm('Are you sure you wish to delete the selected project code?')) 
    {
        SEL.PublicApi.Call("DELETE", "ProjectCodes/" + projectcodeid, "", deleteProjectcodeComplete, commandFail);
    }
}
function deleteProjectcodeComplete(data) 
{
	var messageTopic = "Message from " + moduleNameHTML;

	switch (data) {
		case -2:
			SEL.MasterPopup.ShowMasterPopup('The Project Code cannot be deleted as it is currently assigned to one or more Employees.', messageTopic);
			break;
		case -4:
			SEL.MasterPopup.ShowMasterPopup('The Project Code cannot be deleted as it is currently assigned to one or more Expense Items.', messageTopic);
			break;
		default:
			SEL.Grid.deleteGridRow('gridProjectcodes', currentRowID);
			break;
	}
}

function changeArchiveStatus(projectcodeid) {
    SEL.PublicApi.Call("PUT", "ProjectCodes/Archive/" + projectcodeid, "", changeArchiveStatusComplete, commandFail);
    currentRowID = projectcodeid;
}

function changeArchiveStatusComplete() {
    var cell = SEL.Grid.getCellById('gridProjectcodes', currentRowID, 'archiveStatus');
    if (cell.innerHTML.indexOf('Un-Archive') !== -1) {
        cell.innerHTML = "<a href='javascript:changeArchiveStatus(" +
            currentRowID +
            ");'><img title='Archive' src='/shared/images/icons/folder_lock.png'></a>";
    }
    else {
        cell.innerHTML = "<a href='javascript:changeArchiveStatus(" +
            currentRowID +
            ");'><img title='Un-Archive' src='/shared/images/icons/folder_into.gif'></a>";
    }
}

function repopulateGrid() 
{
    var cmbFilterVal = document.getElementById(cmbFilterID).options[document.getElementById(cmbFilterID).selectedIndex].value;

    PageMethods.CreateGrid(cmbFilterVal, repopulateGridComplete, commandFail);
}

function repopulateGridComplete(data) 
{
    if ($e(pnlGridID) === true) {
        $g(pnlGridID).innerHTML = data[1];
        SEL.Grid.updateGrid(data[0]);
    }
}

function commandFail(error) 
{
    if (error["_message"] !== null) 
    {
        SEL.MasterPopup.ShowMasterPopup(error["_message"], "Web Service Message");
    }
    else
    {
        SEL.MasterPopup.ShowMasterPopup(error, "Web Service Message");
    }
    return;
}
