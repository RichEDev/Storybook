/// <summary>
/// Audience Methods
/// </summary>    
(function () {
    var scriptName = "Audience";
    function execute() {
        SEL.registerNamespace("SEL.Audience");
        SEL.Audience =
        {
            DeleteRecordID: 0,
            AudienceID: 0,
            CanEdit: true,
            CanDelete: true,
            DeleteAudience: function (audienceGridID) {
                if (confirm('This will permanently delete the audience, are you sure?')) {
                    Spend_Management.svcAudiences.DeleteAudience(audienceGridID, SEL.Audience.DeleteAudienceComplete, SEL.Audience.errorMessage);
                }
                return;
            },
            DeleteAudienceComplete: function (result) {
                if (result > -1)
                    SEL.Grid.refreshGrid('gridAudiences', SEL.Grid.currentPageNum['gridAudiences']);
                else
                    SEL.Audience.errorMessage('This audience is already in use and cannot be deleted.');
                return;
            },
            onCreateAudiencesGridComplete: function (gridData) {
                if ($e(pnlAudienceGrid) === true) {
                    $g(pnlAudienceGrid).innerHTML = gridData[1];
                    SEL.Grid.updateGrid(gridData[0]);
                }
            },
            SaveAudience: function () {
                if (validateform('generalDetails') == false) { return; }

                audienceName = document.getElementById(audienceNameID).value;
                audienceDescription = document.getElementById(audienceDescriptionID).value;
                Spend_Management.svcAudiences.GetAudienceMemberCount(SEL.Audience.AudienceID, SEL.Audience.SaveAudienceContinue, SEL.Audience.errorMessage);
            },
            SaveAudienceContinue: function (count) {
                if (count > 0) {
                    Spend_Management.svcAudiences.SaveAudience(SEL.Audience.AudienceID, audienceName, audienceDescription, SEL.Audience.SaveAudienceComplete, SEL.Audience.errorMessage);
                }
                else {
                    SEL.MasterPopup.ShowMasterPopup('Audiences cannot be left empty, please ensure at least one member present');
                }
                return;
            },
            SaveAudienceComplete: function (data) {
                switch (data) {
                    case -1:
                        SEL.MasterPopup.ShowMasterPopup('The audience name specified already exists.', 'Save Audience');
                        break;
                    case -2:
                        SEL.MasterPopup.ShowMasterPopup('Audiences cannot be left empty, please ensure at least one member present');
                        break;
                    default:
                        window.location = 'adminAudiences.aspx';
                        break;
                }
                return;
            },
            CancelAudience: function () {
                Spend_Management.svcAudiences.GetAudienceMemberCount(SEL.Audience.AudienceID, SEL.Audience.CancelAudienceComplete, SEL.Audience.errorMessage);
            },
            CancelAudienceComplete: function (data) {
                if (SEL.Audience.AudienceID > 0 && data == 0) {
                    SEL.MasterPopup.ShowMasterPopup('Audiences cannot be left empty, please ensure at least one member present');
                }
                else {
                    window.location = 'adminAudiences.aspx';
                }
                return;
            },
            AddEmployeeToAudience: function () {
                //                if (audienceID == 0) {
                //                    if (confirm('This will save the audience details, are you sure?')) {
                //                        if (validateform('generalDetails') == false) { return; }
                //                        SEL.Audience.ShallowSaveAudienceForEmployees();
                //                    }
                //                }
                document.getElementById(employeesFilter).value = '';

                Spend_Management.svcAudiences.CreateEmployeesModalGrid(SEL.Audience.AudienceID, '', SEL.Audience.CreateEmployeesModalGridComplete);
                SEL.Audience.ShowEmployeeModal();
                return;
            },
            ShallowSaveAudienceForEmployees: function () {
                audienceName = document.getElementById(audienceNameID).value;
                audienceDescription = document.getElementById(audienceDescriptionID).value;

                Spend_Management.svcAudiences.SaveAudience(SEL.Audience.AudienceID, audienceName, audienceDescription, SEL.Audience.ShallowSaveAudienceForEmployeesComplete, SEL.Audience.errorMessage);
                return;
            },
            CreateEmployeesModalGridComplete: function (data) {
                if ($e(employeesModalGridID) === true) {
                    $g(employeesModalGridID).innerHTML = data[1];
                    SEL.Grid.updateGrid(data[0]);
                }
                SEL.Audience.ShowEmployeeModal(SEL.Audience.AudienceID);
            },
            ShallowSaveAudienceForEmployeesComplete: function (data) {
                if (data != null) {
                    if (data > 0) {
                        PageMethods.updateAudienceID(data);
                        SEL.Audience.AudienceID = data;
                        SEL.Audience.SaveAudienceEmployees();
                    }
                    else {
                        SEL.MasterPopup.ShowMasterPopup('The audience name specified already exists. Please amend before attempting to add members', 'Save Audience');
                    }
                }
            },
            ShowEmployeeModal: function (data) {
                if (data != null && data > 0) {
                    PageMethods.updateAudienceID(data);
                    SEL.Audience.AudienceID = data;
                }
                $find(employeesModalID).show();
                return;
            },
            FilterEmployeesGrid: function () {
                var filter = document.getElementById(employeesFilter).value;
                if (filter == null) {
                    filter = '';
                }
                Spend_Management.svcAudiences.CreateEmployeesModalGrid(SEL.Audience.AudienceID, filter, SEL.Audience.CreateEmployeesModalGridComplete);
                return;
            },
            SaveAudienceEmployees: function () {
                if (SEL.Audience.AudienceID == 0) {
                    //if (confirm('This will save the audience details, are you sure?')) {
                    if (validateform('generalDetails') == false) { return; }
                    SEL.Audience.ShallowSaveAudienceForEmployees();
                    return;
                    //}
                }
                var employeeIDs = Array();
                employeeIDs = SEL.Grid.getSelectedItemsFromGrid('gridEmployeesSearch');
                if (employeeIDs.length == 0)
                    SEL.MasterPopup.ShowMasterPopup('An employee selection is mandatory.', 'Page Validation Failed');
                else
                    Spend_Management.svcAudiences.SaveAudienceEmployees(SEL.Audience.AudienceID, employeeIDs, SEL.Audience.SaveAudienceEmployeesComplete, SEL.Audience.errorMessage);
                return;
            },
            SaveAudienceEmployeesComplete: function (data) {
                if ($e(pnlEmployees) === true) {
                    $g(pnlEmployees).innerHTML = data[2];
                    SEL.Grid.updateGrid(data[1]);
                    PageMethods.updateAudienceID(data[0]);
                    SEL.Audience.AudienceID = data[0];
                }
                $find(employeesModalID).hide();
                return;
            },
            CancelAudienceEmployees: function () {
                $find(employeesModalID).hide();
                return;
            },
            DeleteAudienceEmployee: function (audienceID, audienceEmployeeID) {
                SEL.Audience.DeleteRecordID = audienceEmployeeID;
                SEL.Audience.AudienceID = audienceID;
                Spend_Management.svcAudiences.GetAudienceMemberCount(audienceID, SEL.Audience.DeleteAudienceEmployeeContinue, SEL.Audience.errorMessage);
            },
            DeleteAudienceEmployeeContinue: function (count) {
                if (count > 1) {
                    if (confirm('Click OK to confirm deletion of employee from this audience')) {
                        Spend_Management.svcAudiences.DeleteAudienceEmployee(SEL.Audience.AudienceID, SEL.Audience.DeleteRecordID, SEL.Audience.onDeleteAudienceEmpComplete, SEL.Audience.errorMessage);
                    }
                }
                else {
                    SEL.MasterPopup.ShowMasterPopup('This is the last audience member. Audiences cannot be left without members. Delete request refused.');
                }
                return;
            },
            onDeleteAudienceEmpComplete: function (data) {
                SEL.Grid.refreshGrid('gridEmployees', SEL.Grid.currentPageNum['gridEmployees']);
            },
            AddBudgetHolderToAudience: function () {
                document.getElementById(budgetHoldersFilter).value = '';
                Spend_Management.svcAudiences.CreateBudgetHoldersModalGrid(SEL.Audience.AudienceID, '', SEL.Audience.AddBudgetHolderToAudienceComplete);

                return;
            },
            AddBudgetHolderToAudienceComplete: function (data) {
                if ($e(budgetHoldersModalGridID) === true) {
                    $g(budgetHoldersModalGridID).innerHTML = data[1];
                    SEL.Grid.updateGrid(data[0]);
                }
                SEL.Audience.ShowBudgetHolderModal(SEL.Audience.AudienceID);
            },
            ShallowSaveAudienceForBudgetHolders: function () {
                audienceName = document.getElementById(audienceNameID).value;
                audienceDescription = document.getElementById(audienceDescriptionID).value;
                Spend_Management.svcAudiences.SaveAudience(SEL.Audience.AudienceID, audienceName, audienceDescription, SEL.Audience.ShallowSaveAudienceForBudgetHoldersComplete, SEL.Audience.errorMessage);
                return;
            },
            ShallowSaveAudienceForBudgetHoldersComplete: function (data) {
                if (data != null) {
                    if (data > 0) {
                        PageMethods.updateAudienceID(data);
                        SEL.Audience.AudienceID = data;
                        SEL.Audience.SaveAudienceBudgetHolders();
                    }
                    else {
                        SEL.MasterPopup.ShowMasterPopup('The audience name specified already exists. Please amend before attempting to add members', 'Save Audience');
                    }
                }
            },
            ShowBudgetHolderModal: function (data) {
                if (data != null && data > 0) {
                    PageMethods.updateAudienceID(data);
                    SEL.Audience.AudienceID = data;
                }
                $find(budgetHoldersModalID).show();
                return;
            },
            FilterBudgetHoldersGrid: function () {
                var filter = document.getElementById(budgetHoldersFilter).value;
                if (filter == null) {
                    filter = '';
                }
                Spend_Management.svcAudiences.CreateBudgetHoldersModalGrid(SEL.Audience.AudienceID, filter, SEL.Audience.AddBudgetHolderToAudienceComplete);
                return;
            },
            SaveAudienceBudgetHolders: function () {
                if (SEL.Audience.AudienceID == 0) {
                    if (validateform('generalDetails') == false) { return; }
                    SEL.Audience.ShallowSaveAudienceForBudgetHolders();
                    return;
                }
                var budgetHolderIDs = new Array();
                budgetHolderIDs = SEL.Grid.getSelectedItemsFromGrid('gridBudgetHoldersSearch');
                if (budgetHolderIDs.length == 0)
                    SEL.MasterPopup.ShowMasterPopup('A budget holder selection is mandatory.', 'Page Validation Failed');
                else
                    Spend_Management.svcAudiences.SaveAudienceBudgetHolders(SEL.Audience.AudienceID, budgetHolderIDs, SEL.Audience.SaveAudienceBudgetHoldersComplete, SEL.Audience.errorMessage);
                return;
            },
            SaveAudienceBudgetHoldersComplete: function (data) {
                if ($e(pnlBudgetHolders) === true) {
                    $g(pnlBudgetHolders).innerHTML = data[2];
                    SEL.Grid.updateGrid(data[1]);
                    PageMethods.updateAudienceID(data[0]);
                    SEL.Audience.AudienceID = data[0];
                }
                $find(budgetHoldersModalID).hide();
                return;
            },
            CancelAudienceBudgetHolders: function () {
                $find(budgetHoldersModalID).hide();
                return;
            },
            DeleteAudienceBudgetHolder: function (audienceID, audienceBudgetHolderID) {
                SEL.Audience.AudienceID = audienceID;
                SEL.Audience.DeleteRecordID = audienceBudgetHolderID;
                Spend_Management.svcAudiences.GetAudienceMemberCount(SEL.Audience.AudienceID, SEL.Audience.DeleteAudienceBudgetHolderContinue, SEL.Audience.errorMessage);
            },
            DeleteAudienceBudgetHolderContinue: function (count) {
                if (count > 1) {
                    if (confirm('Click OK to confirm deletion of budget holder from this audience')) {
                        Spend_Management.svcAudiences.DeleteAudienceBudgetHolder(SEL.Audience.AudienceID, SEL.Audience.DeleteRecordID, SEL.Audience.DeleteAudienceBudgetHolderComplete, SEL.Audience.errorMessage);
                    }
                }
                else {
                    SEL.MasterPopup.ShowMasterPopup('This is the last audience member. Audiences cannot be left without members. Delete request refused.');
                }
                return;
            },
            DeleteAudienceBudgetHolderComplete: function (data) {
                SEL.Grid.refreshGrid('gridBudgetHolders', SEL.Grid.currentPageNum['gridBudgetHolders']);
                return;
            },
            AddTeamToAudience: function () {
                document.getElementById(teamsFilter).value = '';
                Spend_Management.svcAudiences.CreateTeamsModalGrid(SEL.Audience.AudienceID, '', SEL.Audience.AddTeamToAudienceComplete, SEL.Audience.errorMessage);
                return;
            },
            AddTeamToAudienceComplete: function (data) {
                if ($e(teamsModalGridID) === true) {
                    $g(teamsModalGridID).innerHTML = data[1];
                    SEL.Grid.updateGrid(data[0]);
                }
                SEL.Audience.ShowTeamModal(SEL.Audience.AudienceID);
            },
            ShallowSaveAudienceForTeams: function () {
                audienceName = document.getElementById(audienceNameID).value;
                audienceDescription = document.getElementById(audienceDescriptionID).value;
                Spend_Management.svcAudiences.SaveAudience(SEL.Audience.AudienceID, audienceName, audienceDescription, SEL.Audience.ShallowSaveAudienceForTeamsComplete, SEL.Audience.errorMessage);
                return;
            },
            ShallowSaveAudienceForTeamsComplete: function (data) {
                if (data != null) {
                    if (data > 0) {
                        PageMethods.updateAudienceID(data);
                        SEL.Audience.AudienceID = data;
                        SEL.Audience.SaveAudienceTeams();
                    }
                    else {
                        SEL.MasterPopup.ShowMasterPopup('The audience name specified already exists. Please amend before attempting to add members', 'Save Audience');
                    }
                }
            },
            ShowTeamModal: function (data) {
                if (data != null && data > 0) {
                    PageMethods.updateAudienceID(data);
                    SEL.Audience.AudienceID = data;
                }
                $find(teamsModalID).show();
                return;
            },
            FilterTeamsGrid: function () {
                var filter = document.getElementById(teamsFilter).value;
                if (filter == null) {
                    filter = '';
                }
                Spend_Management.svcAudiences.CreateTeamsModalGrid(SEL.Audience.AudienceID, filter, SEL.Audience.AddTeamToAudienceComplete);
                return;
            },
            SaveAudienceTeams: function () {
                if (SEL.Audience.AudienceID == 0) {
                    if (validateform('generalDetails') == false) { return; }
                    SEL.Audience.ShallowSaveAudienceForTeams();
                    return;
                }
                var teamIDs = new Array();
                teamIDs = SEL.Grid.getSelectedItemsFromGrid('gridTeamsSearch');
                if (teamIDs.length == 0)
                    SEL.MasterPopup.ShowMasterPopup('A team selection is mandatory.', 'Page Validation Failed');
                else
                    Spend_Management.svcAudiences.SaveAudienceTeams(SEL.Audience.AudienceID, teamIDs, SEL.Audience.SaveAudienceTeamsComplete, SEL.Audience.errorMessage);
                return;
            },
            SaveAudienceTeamsComplete: function (data) {
                if ($e(pnlTeams) === true) {
                    $g(pnlTeams).innerHTML = data[2];
                    SEL.Grid.updateGrid(data[1]);
                }
                $find(teamsModalID).hide();
                return;
            },
            CancelAudienceTeams: function () {
                $find(teamsModalID).hide();
                return;
            },
            DeleteAudienceTeam: function (audienceID, audienceTeamID) {
                SEL.Audience.AudienceID = audienceID;
                SEL.Audience.DeleteRecordID = audienceTeamID;
                Spend_Management.svcAudiences.GetAudienceMemberCount(SEL.Audience.AudienceID, SEL.Audience.DeleteAudienceTeamContinue, SEL.Audience.errorMessage);
            },
            DeleteAudienceTeamContinue: function (count) {
                if (count > 1) {
                    if (confirm('Click OK to confirm deletion of the team from this audience')) {
                        Spend_Management.svcAudiences.DeleteAudienceTeam(SEL.Audience.AudienceID, SEL.Audience.DeleteRecordID, SEL.Audience.DeleteAudienceTeamComplete, SEL.Audience.errorMessage);
                    }
                }
                else {
                    SEL.MasterPopup.ShowMasterPopup('This is the last audience member. Audiences cannot be left without members. Delete request refused.');
                }
                return;
            },
            DeleteAudienceTeamComplete: function (data) {
                SEL.Grid.refreshGrid('gridTeams', SEL.Grid.currentPageNum['gridTeams']);
            },
            errorMessage: function (data) {
                if (data["_message"] != null) {
                    SEL.MasterPopup.ShowMasterPopup(data["_message"], "Web Service Message");
                }
                else {
                    SEL.MasterPopup.ShowMasterPopup(data, "Web Service Message");
                }
            },
            CancelAudienceUC: function () {
                $find(ucAudienceModalID).hide();
            },
            SaveAudienceRecord: function () {
                var tableKey = 'audienceSelectionGrid';
                var selections = new Array();
                if (audId == 0)
                    selections = SEL.Grid.getSelectedItemsFromGrid(tableKey);
                else
                    selections.push(audId);
                if (selections.length == 0)
                    SEL.MasterPopup.ShowMasterPopup('Selection of an audience from the list is mandatory.', 'Page Validation Failed');
                else {
                    $find(ucAudienceModalID).hide();
                    Spend_Management.svcAudiences.saveAudienceRecord(entityIdentifier, parentRecordId, baseTableId, selections, $g('chkCanView').checked, $g('chkCanEdit').checked, $g('chkCanDelete').checked, SEL.Audience.CanEdit, SEL.Audience.CanDelete, SEL.Audience.AudienceRecordSuccess, SEL.Audience.AudienceRecordFail);
                }
                return;
            },
            AudienceRecordSuccess: function (results) {
                if (results[2] == -1) {
                    SEL.MasterPopup.ShowMasterPopup('View, edit and delete permissions cannot be revoked as at least one audience must have full permissions. Audience(s) not saved.', 'Save Audience');
                }

                if ($e(pnlAudienceGrid) === true) {
                    $g(pnlAudienceGrid).innerHTML = results[1];
                    SEL.Grid.updateGrid(results[0]);
                }
            },
            AudienceRecordFail: function (error) {
                SEL.Audience.errorMessage('An error occurred attempting to perform action for the audience record');
            },
            addAudience: function (entityId, basetableid, parentRecId) {
                audId = 0;
                entityIdentifier = entityId;
                baseTableId = basetableid;
                parentRecordId = parentRecId;
                $g('divAudience').innerHTML = 'Add Audience';
                $g('chkCanView').checked = true;
                $g('chkCanView').disabled = true;
                $g('chkCanEdit').checked = true;
                $g('chkCanDelete').checked = true;

                Spend_Management.svcAudiences.CreateAudienceSearchGridUC(baseTableId, parentRecordId, -1, SEL.Audience.searchPanelSuccess, SEL.Audience.searchPanelFail);
                return;
            },
            searchPanelSuccess: function (results) {
                var searchPanel = $get(pnlAudienceSearchGrid);
                if (searchPanel != null) {
                    searchPanel.innerHTML = results[1];
                    SEL.Grid.updateGrid(results[0]);
                }
                $find(ucAudienceModalID).show();
            },
            searchPanelFail: function (error) {
                SEL.Audience.errorMessage('A problem occurred attempting to create the search panel');
                return;
            },
            DeleteAudienceRecord: function (recId, parentRecordId, baseTableId, entityId) {
                if (confirm('Click OK to confirm removal of this audience')) {
                    Spend_Management.svcAudiences.deleteAudienceRecord(recId, parentRecordId, baseTableId, entityId, SEL.Audience.onDeleteAudienceRecordSuccess, SEL.Audience.errorMessage);
                }
            },
            onDeleteAudienceRecordSuccess: function (data) {
                if (data > 0) {
                    SEL.Grid.refreshGrid('gridAudienceUC', SEL.Grid.currentPageNum['gridAudienceUC']);
                }
                else {
                    SEL.MasterPopup.ShowMasterPopup('Other audiences do not have sufficient view, edit and delete rights for this audience to be deleted.', 'Delete Audience');
                }
            },
            EditAudienceRecord: function (recId, parentRecId, audienceID, basetableid, entityId) {
                audId = audienceID;
                baseTableId = basetableid;
                entityIdentifier = entityId;
                parentRecordId = parentRecId;
                $g('divAudience').innerHTML = 'Edit Audience';
                Spend_Management.svcAudiences.getAudienceRecord(recId, baseTableId, SEL.Audience.getAudienceRecordSuccess, SEL.Audience.getAudienceRecordFail);
            },
            getAudienceRecordSuccess: function (recStatus) {
                $g('chkCanView').checked = recStatus.CanView;
                $g('chkCanEdit').checked = recStatus.CanEdit;
                $g('chkCanDelete').checked = recStatus.CanDelete;

                SEL.Audience.CheckPermissionState();
                Spend_Management.svcAudiences.CreateAudienceSearchGridUC(baseTableId, parentRecordId, recStatus.AudienceID, SEL.Audience.searchPanelSuccess, SEL.Audience.searchPanelFail);
                return;
            },
            getAudienceRecordFail: function (error) {
                SEL.MasterPopup.ShowMasterPopup('Attempt to retrieve audience record failed', 'Web Service Message');
            },
            CheckPermissionState: function () {
                if ($g('chkCanEdit').checked || $g('chkCanDelete').checked) { // || $g('chkCanAdd').checked 
                    $g('chkCanView').checked = true;
                    $g('chkCanView').disabled = true;
                }
                else {
                    $g('chkCanView').disabled = false;
                }
            }
        };
    }

    if (window.Sys && Sys.loader) {
        Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
})();