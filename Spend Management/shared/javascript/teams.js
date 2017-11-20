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

function DeleteTeam(teamid)
{
    if (confirm('Are you sure you wish to delete the selected team?'))
    {
        Spend_Management.svcTeams.DeleteTeam(teamid, DeleteTeamComplete, errorMessage);
    }
    return;
}
function DeleteTeamComplete(data)
{
    switch (data)
    {
        case 1:
            SEL.MasterPopup.ShowMasterPopup("This team cannot currently be deleted as it is currently responsible for approving 1 or more claims.", "Delete Team");
            break;
        case 2:
            SEL.MasterPopup.ShowMasterPopup("This team cannot currently be deleted as it is assigned to one or more signoff stages.", "Delete Team");
            break;
        case 3:
            SEL.MasterPopup.ShowMasterPopup("This team cannot currently be deleted as it is assigned to one or more tasks.", "Delete Team");
            break;
        case 4:
            SEL.MasterPopup.ShowMasterPopup("This team cannot currently be deleted as it is assigned to one or more audiences.", "Delete Team");
            break;
        case 5:
            SEL.MasterPopup.ShowMasterPopup("This team cannot currently be deleted as it is the owner of one or more cost codes.", "Delete Team");
            break;
        case 6:
            SEL.MasterPopup.ShowMasterPopup("This team cannot currently be deleted as it is assigned to one or more approval matrices.", "Delete Team");
            break;
        case -10:
            SEL.MasterPopup.ShowMasterPopup("This team cannot currently be deleted as it is assigned to one or more GreenLights or user defined fields.", "Delete Team");
            break;
        default:
            SEL.Grid.refreshGrid('gridTeams', SEL.Grid.currentPageNum['gridTeams']);
            break;
    }
    return;
}


var teamMembersTeamIDGlobal;

function ShowTeamMembers(teamID, controlID)
{
    teamMembersTeamIDGlobal = teamID;
    Spend_Management.svcTeams.CreateTeamMembersGrid(teamID, CreateTeamMembersGridComplete, errorMessage);
    return;
}
function CreateTeamMembersGridComplete(data) {
    if ($e(pnlTeamMembersGridID) === true) {
        $g(pnlTeamMembersGridID).innerHTML = data[1];
        SEL.Grid.updateGrid(data[0]);
        
        $find(popTeamMembersGridID)._popupBehavior._parentElement = document.getElementById('tbl_gridTeams_' + teamMembersTeamIDGlobal).childNodes[2];
        $find(popTeamMembersGridID).showPopup();
    }
    return;
}

/*
** Add/Edit Team page
*/
var aeTeam_TeamEmpID;
var aeTeam_EmployeeID;

function SaveTeam()
{
    if (validateform('generalDetails') == false) { return; }

    Spend_Management.svcTeams.GetTeamMemberCount(TeamID, SaveTeamContinue, errorMessage);
    return;
}
function SaveTeamContinue(count) {
    if (count > 0) {
        var teamName = document.getElementById(txtTeamNameID).value;
        var teamDescription = document.getElementById(txtDescriptionID).value;
        var teamLeaderID = parseInt(document.getElementById(ddlTeamLeaderID).value);

        Spend_Management.svcTeams.SaveTeam(TeamID, teamName, teamDescription, teamLeaderID, SaveTeamComplete, errorMessage);
    }
    else {
        SEL.MasterPopup.ShowMasterPopup('The team has no members. Empty teams are prohibited. Please add at least one member.');
    }
}
function SaveTeamComplete(data) {
    if (data == -1) {
        errorMessage("A team with this name already exists");
    } else {
        window.location = 'adminteams.aspx';
    }
    
    return;
}
function CancelTeam() {
    Spend_Management.svcTeams.GetTeamMemberCount(TeamID, CancelTeamContinue, errorMessage);

    return;
}
function CancelTeamContinue(count) {
    if (count == 0 && TeamID > 0) {
        SEL.MasterPopup.ShowMasterPopup('The team has no members. Empty teams are prohibited. Please add at least one member.');
    }
    else {
        window.location = 'adminteams.aspx';
    }
}
function DeleteTeamEmployee(teamEmpID, employeeID) {
    aeTeam_EmployeeID = employeeID;
    aeTeam_TeamEmpID = teamEmpID;

    Spend_Management.svcTeams.GetTeamMemberCount(TeamID, DeleteTeamEmployeeContinue, errorMessage);

    return;
}

function DeleteTeamEmployeeContinue(count) {
    if (count > 1) {
        Spend_Management.svcTeams.DeleteTeamEmployee(aeTeam_TeamEmpID, DeleteTeamEmployeeComplete, errorMessage);
        /// Remove member from the ddl
        var ddlTL = document.getElementById(ddlTeamLeaderID);
        var i;
        for (i = 0; i < ddlTL.length; i++) {
            if (parseInt(ddlTL.options[i].value) === aeTeam_EmployeeID) {
                ddlTL.remove(i); break;
            }
        }
    }
    else {
        SEL.MasterPopup.ShowMasterPopup('There must be at least one team member present. Deletion of the last member is prohibited.', 'Delete Team Member');
    }
}

function DeleteTeamEmployeeComplete(data)
{
    SEL.Grid.refreshGrid('gridTeamMembers', SEL.Grid.currentPageNum['gridTeamMembers']);

    return;
}

function AddTeamMembers() {
    if (validateform('generalDetails') == false) { return; }

    SEL.Grid.clearSelectAllOnGrid('gridTeamEmployees');
    //    if (TeamID == 0) {
    //        var teamName = document.getElementById(txtTeamNameID).value;
    //        var teamDescription = document.getElementById(txtDescriptionID).value;
    //        var teamLeaderID = parseInt(document.getElementById(ddlTeamLeaderID).value);

    //        Spend_Management.svcTeams.SaveTeam(TeamID, teamName, teamDescription, teamLeaderID, ShallowSaveTeamComplete, errorMessage);
    //    }
    //    else {
    SEL.Grid.clearFilter('gridTeamEmployees');
    SEL.Grid.filterGrid('gridTeamEmployees');
    $find(mdlEmployeesGridID).show();
    //}
    return;
}
function ShallowSaveTeamComplete(data) {
    if (data != -1) {
//        var gridDetails = SEL.Grid.getGridById('gridTeamEmployees');
//        if (gridDetails !== null) {
//            gridDetails.filters[0].values1[0] = data;
        TeamID = data;

        var employeeIDs = Array();
        employeeIDs = SEL.Grid.getSelectedItemsFromGrid('gridTeamEmployees');

        Spend_Management.svcTeams.SaveTeamEmps(TeamID, employeeIDs, SaveEmployeesModalComplete, errorMessage);
//        $find(mdlEmployeesGridID).show();
    } else {
        errorMessage("A team with this name already exists");
    }
    return;
}

function SaveEmployeesModal() {
    var employeeIDs = Array();
    employeeIDs = SEL.Grid.getSelectedItemsFromGrid('gridTeamEmployees');

    if (TeamID == 0 && employeeIDs.length == 0) {
        SEL.MasterPopup.ShowMasterPopup('The team has no members. Empty teams are prohibited. Please add at least one member.');
        return;
    }

    if (TeamID == 0) {
        var teamName = document.getElementById(txtTeamNameID).value;
        var teamDescription = document.getElementById(txtDescriptionID).value;
        var teamLeaderID = parseInt(document.getElementById(ddlTeamLeaderID).value);

        Spend_Management.svcTeams.SaveTeam(TeamID, teamName, teamDescription, teamLeaderID, ShallowSaveTeamComplete, errorMessage);
    }
    else {
        Spend_Management.svcTeams.SaveTeamEmps(TeamID, employeeIDs, SaveEmployeesModalComplete, errorMessage);
    }

    return;
}
function SaveEmployeesModalComplete(data)
{
    var x = document.getElementById(ddlTeamLeaderID);
    for (var i = 0; i < data.length; i++) {
        var exists = false;
        var str = data[i].split(';');

        for (var optIdx = 0; optIdx < x.options.length; optIdx++) {
            var tmpOpt = x.options[optIdx];
            if (tmpOpt.value == str[1]) {
                exists = true;
                break;
            }
        }

        if (!exists) {
            var option = document.createElement('option');
            option.text = str[0];
            option.value = str[1];
            try {
                x.add(option, null); // standards compliant
            }
            catch (ex) {
                x.add(option); // IE only
            }
        }
    }

    Spend_Management.svcTeams.CreateTeamEmpsGrid(TeamID, CreateTeamEmployeesGridComplete, errorMessage);
//    refreshGrid("gridTeamMembers", 0);
//    $find(mdlEmployeesGridID).hide();
    return;
}
function CreateTeamEmployeesGridComplete(data) {
    if ($e(pnlTeamMembersGridID) === true) {
        $g(pnlTeamMembersGridID).innerHTML = data[1];
        SEL.Grid.updateGrid(data[0]);
    }
    $find(mdlEmployeesGridID).hide();
    return;
}

function CancelEmployeesModal()
{
    $find(mdlEmployeesGridID).hide();
    return;
}
