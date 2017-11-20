var currentRowID;

function deleteReason(reasonid) {
    currentRowID = reasonid;
    if (confirm('Are you sure you wish to delete the selected reason?')) {
        PageMethods.deleteReason(accountid, reasonid,deleteReasonComplete);
    }
}

function deleteReasonComplete(data) {
    switch (data) {
        case -10:
            SEL.MasterPopup.ShowMasterPopup('The reason cannot be deleted as it currently assigned to a GreenLight or user defined field record.', 'Message from ' + moduleNameHTML);
            break;
        case 1:
            SEL.MasterPopup.ShowMasterPopup('The reason cannot be deleted as it is currently assigned to one or more expense items.', 'Message from ' + moduleNameHTML);
            break;
        default:
            SEL.Grid.deleteGridRow('gridReasons', currentRowID);
            break;
    }
}




function SaveReason()
{
    if (validateform(null) == false) { return; }
    
    var reasonName = document.getElementById(reasonNameID).value;
    var description = document.getElementById(descriptionID).value;
    var codeWithVAT = document.getElementById(codeWithVATID).value;
    var codeWithoutVAT = document.getElementById(codeWithoutVATID).value;
    var isArchived = false;
    Spend_Management.svcReasons.SaveReason(reasonID, reasonName, description, codeWithVAT, codeWithoutVAT, isArchived, SaveReasonComplete, errorMessage);
    return;
}
function SaveReasonComplete(data)
{
    if (data == -1)
    {
        SEL.MasterPopup.ShowMasterPopup('This reason cannot be saved as the reason name you have entered already exists', 'Validation Message');
    }
    else
    {
        window.location = "adminreasons.aspx";
    }
    return;
}

function CancelReason()
{
    window.location = "adminreasons.aspx";
    return;
}

function errorMessage()
{
    if (data["_message"] != null)
    {
        SEL.MasterPopup.ShowMasterPopup(data["_message"], 'WebService Message');
    }
    else
    {
        SEL.MasterPopup.ShowMasterPopup(data, 'WebService Message');
    }
    return;
}

function changeArchiveStatus(reasonid) {
    currentRowID = reasonid;
    PageMethods.ChangeStatus(accountid, reasonid);
    var cell = SEL.Grid.getCellById('gridReasons', currentRowID, 'archiveStatus');
    if (cell.innerHTML.indexOf('Un-Archive') != -1) {
        cell.innerHTML = "<a href='javascript:changeArchiveStatus(" + currentRowID + ");'><img title='Archive' src='/shared/images/icons/folder_lock.png'></a>";
    }
    else {
        cell.innerHTML = "<a href='javascript:changeArchiveStatus(" + currentRowID + ");'><img title='Un-Archive' src='/shared/images/icons/folder_into.gif'></a>";
    }
}
