function deleteDepartment(departmentid) {
    currentRowID = departmentid;
    if (confirm('Are you sure you wish to delete the selected department?')) {
        PageMethods.deleteDepartment(accountid, departmentid, deleteDepartmentComplete, commandFail);
    }
}

function deleteDepartmentComplete(data) {
	var messageTopic = "Message from " + moduleNameHTML;
	
	switch (data) {
	    case -1:
	        SEL.MasterPopup.ShowMasterPopup('The Department cannot be deleted as it is currently assigned to one or more Signoff Stages.', messageTopic);
	        break;
		case -2:
			SEL.MasterPopup.ShowMasterPopup('The Department cannot be deleted as it is currently assigned to one or more Employees.', messageTopic);
			break;
		case -4:
			SEL.MasterPopup.ShowMasterPopup('The Department cannot be deleted as it is currently assigned to one or more Expense Items.', messageTopic);
			break;
        case -6:
            SEL.MasterPopup.ShowMasterPopup('The Department cannot be deleted as it is currently assigned to one or more attribute field filters.', messageTopic);
            break;
        case -10:
        	SEL.MasterPopup.ShowMasterPopup('The Department cannot be deleted as it currently assigned to one or more GreenLights or user defined field records.', messageTopic);
            break;
        default:
            SEL.Grid.deleteGridRow('gridDepartments', currentRowID);
            break;
    }
}
function changeArchiveStatus(departmentid) {
    currentRowID = departmentid;
    PageMethods.changeStatus(accountid, departmentid);
var cell = SEL.Grid.getCellById('gridDepartments', currentRowID, 'archiveStatus');
    if (cell.innerHTML.indexOf('Un-Archive') != -1) {
        cell.innerHTML = "<a href='javascript:changeArchiveStatus(" + currentRowID + ");'><img title='Archive' src='/shared/images/icons/folder_lock.png'></a>";
    }
    else {
        cell.innerHTML = "<a href='javascript:changeArchiveStatus(" + currentRowID + ");'><img title='Un-Archive' src='/shared/images/icons/folder_into.gif'></a>";
    }
}

function changeArchiveStatusComplete() {

}

function repopulateGrid() 
{
    var cmbFilterVal = document.getElementById(cmbFilterID).options[document.getElementById(cmbFilterID).selectedIndex].value;

    PageMethods.createGrid(cmbFilterVal, repopulateGridComplete, commandFail);
}

function repopulateGridComplete(data) 
{
    if ($e(pnlGridID) === true) 
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
