namespace Spend_Management.expenses.webservices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Services;
    using System.Web.UI.WebControls;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators;
    using Spend_Management.expenses.code;
    using Spend_Management.shared.code.ApprovalMatrix;

    /// <summary>
    /// Summary description for SignoffGroups
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class SignoffGroups : WebService
    {
        /// <summary>
        /// Save a signoff group
        /// </summary>
        /// <param name="groupId">The id of the <see cref="cGroup"/></param>
        /// <param name="groupName">The name of the <see cref="cGroup"/></param>
        /// <param name="groupDescription">The description of the <see cref="cGroup"/></param>
        /// <param name="oneClickAuth">The state of the one click authorisation of the <see cref="cGroup"/></param>
        /// <param name="validate">Whether or not to validate the <see cref="cGroup"/></param>
        /// <returns>The id of the saved <see cref="cGroup"/></returns>
        [WebMethod]
        public SignoffGroupValidation SaveGroup(int groupId, string groupName, string groupDescription, bool oneClickAuth, bool validate)
        {
            var user = cMisc.GetCurrentUser();

            if (!user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.SignOffGroups, true) && 
                !user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.SignOffGroups, true))
            {
                return new SignoffGroupValidation($"", -2);
            }

            var groups = new cGroups(user.AccountID);
            cGroup currentGroup;

            if (groupId > 0)
            {
                currentGroup = groups.GetGroupById(groupId);
                groupId = groups.SaveGroup(groupId, groupName, groupDescription, oneClickAuth, user, 2,
                    currentGroup.NotifyClaimantWhenEnvelopeReceived, currentGroup.NotifyClaimantWhenEnvelopeNotReceived);
            }
            else
            {
                groupId = groups.SaveGroup(groupId, groupName, groupDescription, oneClickAuth, user, 0);
                currentGroup = groups.GetGroupById(groupId);
            }

            if (validate)
            {
                //validate the group's stages.
                var stageValidity = groups.ValidateGroupStages(currentGroup);

                if (!stageValidity.Result)
                {
                    return new SignoffGroupValidation($"{stageValidity.Messages.Aggregate((a, b) => a + "<br/>" + b)}", groupId);
                }
            }
            
            return new SignoffGroupValidation($"", groupId);
        }

        /// <summary>
        /// Delete a <see cref="cGroup"/>
        /// </summary>
        /// <param name="groupId">The id of the <see cref="cGroup"/></param>
        /// <returns>Return code from the delete</returns>
        [WebMethod]
        public int DeleteGroup(int groupId)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            if (!user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.SignOffGroups, true))
            {
                return -3;
            }

            cGroups groups = new cGroups(user.AccountID);

            return groups.DeleteGroup(groupId, user);
        }

        /// <summary>
        /// Get a <see cref="cGroup"/>
        /// </summary>
        /// <param name="groupId">The id of the <see cref="cGroup"/></param>
        /// <returns>Return a <see cref="cGroup"/></returns>
        [WebMethod]
        public cGroup GetGroup(int groupId)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            if (!user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SignOffGroups, true))
            {
                return null;
            }

            cGroups groups = new cGroups(user.AccountID);

            return groups.GetGroupById(groupId);
        }

        /// <summary>
        /// Delete a <see cref="cStage"/>
        /// </summary>
        /// <param name="groupId">Id of the <see cref="cGroup"/> the stage is in</param>
        /// <param name="stageId">Id of the <see cref="cStage"/></param>
        /// <returns>Return code from the delete</returns>
        [WebMethod]
        public int DeleteStage(int groupId, int stageId)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            if (!user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.SignOffGroups, true))
            {
                return -1;
            }

            cGroups groups = new cGroups(user.AccountID);
            cGroup group = groups.GetGroupById(groupId);
            return groups.deleteStage(group, stageId);
        }

        /// <summary>
        /// Get a <see cref="cStage"/>
        /// </summary>
        /// <param name="groupId">Id of the <see cref="cGroup"/> the stage is in</param>
        /// <param name="stageId">Id of the <see cref="cStage"/></param>
        /// <returns>Return a <see cref="cStage"/></returns>
        [WebMethod]
        public cStage GetStage(int groupId, int stageId)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            if (!user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SignOffGroups, true))
            {
                return null;
            }

            cGroups groups = new cGroups(user.AccountID);
            cGroup group = groups.GetGroupById(groupId);

            return group.stages[stageId];
        }

        /// <summary>
        /// Save a <see cref="cStage"/>
        /// </summary>
        /// <param name="groupId">The id of the <see cref="cGroup"/></param>
        /// <param name="stageId">The id of the <see cref="cStage"/></param>
        /// <param name="selectedSignoffType">The <see cref="SignoffType"/> for the <see cref="cStage"/></param>
        /// <param name="singleSignoff">The state of sindle signoff for the <see cref="cStage"/></param>
        /// <param name="sendClaimantEmail">Whether to notify the claimant when this <see cref="cStage"/> has been reached</param>
        /// <param name="sendApproverEmail">Whether to notify the approver when this <see cref="cStage"/> has been reached</param>
        /// <param name="envelopesAreReceived">Whether to notify the claimant when envelopes are received for scan and attach</param>
        /// <param name="envelopesAreNotReceived">Whether to notify the claimant when envelopes are not received for scan and attach</param>
        /// <param name="displayApproverDeclaration">Whether approver have to sign a declaration for this <see cref="cStage"/></param>
        /// <param name="approverJustificationRequired">Whether approver justification is required for this <see cref="cStage"/></param>
        /// <param name="aboveMyLevel">Only approvers above the user level can approve this <see cref="cStage"/></param>
        /// <param name="approverMatrixLevels">The approval limit for this <see cref="cStage"/></param>
        /// <param name="selectedSignoffValue">Selected sinoff type approver option</param>
        /// <param name="noCostCodeOwnerAction">What occurs if the claimant has no owner or default cost code</param>
        /// <param name="stageInclusionType">The stage inclusion type for this <see cref="cStage"/></param>
        /// <param name="stageInclusionValue">The value for this inclusion type</param>
        /// <param name="stageInclusionDropdownType">The selected dropdown value for the stage inclusion type</param>
        /// <param name="involvementType">The involvement for this <see cref="cStage"/></param>
        /// <param name="onHolidayType">The on holiday ytpe for this <see cref="cStage"/></param>
        /// <param name="holidayApproverType">The holiday approver type for this <see cref="cStage"/></param>
        /// <param name="holidayApproverValue">The holuday approver value for thi <see cref="cStage"/></param>
        /// <param name="claimPercentageToValidate">The percentage of items to validate per claim for this <see cref="cStage"/></param>
        /// <returns>Id of the saved <see cref="cStage"/></returns>
        [WebMethod]
        public int SaveStage(int groupId, int stageId, string selectedSignoffType, bool singleSignoff, bool sendClaimantEmail, bool sendApproverEmail, 
            bool envelopesAreReceived, bool envelopesAreNotReceived, bool displayApproverDeclaration, bool approverJustificationRequired, bool aboveMyLevel, 
            string approverMatrixLevels, string selectedSignoffValue, string noCostCodeOwnerAction, string stageInclusionType, string stageInclusionValue, 
            string stageInclusionDropdownType, string involvementType, string onHolidayType, string holidayApproverType, string holidayApproverValue, decimal claimPercentageToValidate) //TODO: Feature flag
        {
            var user = cMisc.GetCurrentUser();

            if (!user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.SignOffGroups, true) &&
                !user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.SignOffGroups, true))
            {
                return -1;
            }

            int extraLevels = 0;
            int relid = 0;
            var include = StageInclusionType.None;
            int notify = 0;
            int holidayid = 0;
            var holidaytype = SignoffType.None;
            int onholiday = 0;
            decimal amount = 0;
            int includeid = 0;
            bool approveHigherLevelsOnly = false;
            bool nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner = true;
            var signofftype = (SignoffType)Enum.Parse(typeof(SignoffType), selectedSignoffType);
            bool allocateForPayment = false;
            int? validationCorrectionThreshold = null;

            // should we only worry about this when the signoff type = costcodeowner? if so, move this inside the switch.
            nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner = noCostCodeOwnerAction == "AssignmentSupervisor";

            switch (signofftype)
            {
                case SignoffType.LineManager:
                case SignoffType.ClaimantSelectsOwnChecker:
                case SignoffType.CostCodeOwner:
                case SignoffType.SELScanAttach:
                case SignoffType.SELValidation:
                case SignoffType.AssignmentSignOffOwner:
                    break;
                case SignoffType.DeterminedByClaimantFromApprovalMatrix:
                    int.TryParse(approverMatrixLevels, out extraLevels);
                    approveHigherLevelsOnly = aboveMyLevel;
                    int.TryParse(selectedSignoffValue, out relid);
                    break;
                case SignoffType.ApprovalMatrix:
                    int.TryParse(selectedSignoffValue, out relid);
                    break;
                default:
                    int.TryParse(selectedSignoffValue, out relid);
                    break;
            }

            int stageInclusionTypeValue = 0;
            int.TryParse(stageInclusionType, out stageInclusionTypeValue);
            include = (StageInclusionType)stageInclusionTypeValue;

            switch (include)
            {
                case StageInclusionType.ClaimTotalBelow:
                case StageInclusionType.ClaimTotalExceeds:
                case StageInclusionType.OlderThanDays:
                    decimal.TryParse(stageInclusionValue, out amount);
                    break;
            }

            int.TryParse(involvementType, out notify);
            int.TryParse(onHolidayType, out onholiday);
            
            var groups = new cGroups(user.AccountID);
            var group = groups.GetGroupById(groupId);

            if (signofftype == SignoffType.SELScanAttach)
            {
                notify = 2;
                groups.SaveGroup(group.groupid, group.groupname, group.description, group.oneclickauthorisation,
                    cMisc.GetCurrentUser(), 1, envelopesAreReceived, envelopesAreNotReceived);
            }

            if (signofftype == SignoffType.SELValidation)
            {
                notify = 2;
            }

            if (stageInclusionDropdownType != string.Empty)
            {
                int.TryParse(stageInclusionDropdownType, out includeid);
            }

            if (onholiday == 3)
            {
                var holidayApproverTypeId = 0;
                int.TryParse(holidayApproverType, out holidayApproverTypeId);
                holidaytype = (SignoffType)holidayApproverTypeId;

                if (holidayApproverValue != null && (holidaytype != SignoffType.LineManager && holidaytype != SignoffType.CostCodeOwner && holidaytype != SignoffType.AssignmentSignOffOwner))
                {
                    int.TryParse(holidayApproverValue, out holidayid);
                }
                else
                {
                    holidayid = 0;
                }
            }

            if (stageId > 0)
            {
                groups.updateStage(stageId, signofftype, relid, (int)include, amount, notify, onholiday, holidaytype, holidayid, includeid, sendClaimantEmail, singleSignoff, sendApproverEmail, displayApproverDeclaration, user.AccountID, extraLevels, approveHigherLevelsOnly, approverJustificationRequired, nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner, allocateForPayment, validationCorrectionThreshold, claimPercentageToValidate); //TODO: Feature flag
            }
            else
            {
                stageId = groups.addStage(groupId, signofftype, relid, (int)include, amount, notify, onholiday, holidaytype, holidayid, includeid, sendClaimantEmail, singleSignoff, sendApproverEmail, displayApproverDeclaration, user.AccountID, 0, false, extraLevels, approveHigherLevelsOnly, approverJustificationRequired, nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner, allocateForPayment, false, validationCorrectionThreshold, claimPercentageToValidate); //TODO: Feature flag
            }

            return stageId;
        }

        /// <summary>
        /// Create the signoff stages grid
        /// </summary>
        /// <param name="groupId">The id of the <see cref="cGroup"/></param>
        /// <returns>The grid data</returns>
        [WebMethod]
        public string[] CreateStagesGrid(int groupId)
        {
            var user = cMisc.GetCurrentUser();
            var clsfields = new cFields(user.AccountID);

            var enableEditing = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.SignOffGroups, true, true) ||
                user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.SignOffGroups, true, true);

            var claimCount = new cGroups(user.AccountID).getCountOfClaimsInProcessByGroupID(groupId);

            if (claimCount > 0)
            {
                enableEditing = false;
            }

            var grid = new cGridNew(user.AccountID, user.EmployeeID, "SignoffStagesGrid", "SELECT signoffid, stage, signofftype, relid, include, notify from SignoffStagesView")
            {
                KeyField = "stage",
                enabledeleting = enableEditing,
                enableupdating = enableEditing,
                deletelink = "javascript:SEL.SignoffGroups.SignoffStage.Delete({signoffid});",
                editlink =
                    "javascript:SEL.SignoffGroups.DomIDs.SignoffGroup.ShowStageModal = true;SEL.SignoffGroups.DomIDs.SignoffStage.StageId = {signoffid};SEL.SignoffGroups.SignoffGroup.Save();",
                EmptyText = "There are currently no Signoff stages defined",
                EnableSorting = true
            };

            grid.addFilter(clsfields.GetFieldByID(new Guid("161F5786-187B-4BC9-8635-27780BC28321")), ConditionType.Equals, new object[] { groupId }, null, ConditionJoiner.None);
            grid.getColumnByName("signoffid").hidden = true;
            grid.getColumnByName("relid").HeaderText = "Approver";
            grid.getColumnByName("include").HeaderText = "When to Include";
            grid.getColumnByName("signofftype").HeaderText = "Signoff Type";

            return grid.generateGrid();
        }

        /// <summary>
        /// Populates the dropdown list for the signoff type values
        /// </summary>
        /// <param name="signoffId">Id of the <see cref="SignoffType"/></param>
        /// <returns>A list of <see cref="ListItem"/> to put in the dropdown</returns>
        [WebMethod]
        public List<ListItem> PopulateSignoffValueDropdown(string signoffId)
        {
            var user = cMisc.GetCurrentUser();
            var currentSignoffType = (SignoffType)Enum.Parse(typeof(SignoffType), signoffId);
            var returnValues = new List<ListItem>();
            switch (currentSignoffType)
            {
                case SignoffType.BudgetHolder:
                    returnValues = new cBudgetholders(user.AccountID).CreateDropDown();
                    break;
                case SignoffType.Employee:
                    returnValues = new cEmployees(user.AccountID).CreateCheckPayDropDown(0, user.AccountID).ToList();
                    break;
                case SignoffType.Team:
                    returnValues = new cTeams(user.AccountID).CreateDropDown(0).ToList();
                    break;
                case SignoffType.ApprovalMatrix:
                    returnValues = new ApprovalMatrices(user.AccountID).CreateDropDown(0, false);
                    break;

                case SignoffType.DeterminedByClaimantFromApprovalMatrix:
                    returnValues = new ApprovalMatrices(user.AccountID).CreateDropDown(0, false);
                    break;
                case SignoffType.LineManager:
                case SignoffType.ClaimantSelectsOwnChecker:
                case SignoffType.CostCodeOwner:
                case SignoffType.AssignmentSignOffOwner:
                case SignoffType.SELScanAttach:
                case SignoffType.SELValidation:
                    break;

            }
            return returnValues;
        }

        /// <summary>
        /// Populates the stage inclusion dropdown
        /// </summary>
        /// <param name="inclusionId">Id of the <see cref="StageInclusionType"/></param>
        /// <returns>A list of <see cref="ListItem"/> to put in the dropdown</returns>
        [WebMethod]
        public List<ListItem> PopulateIncludeDropdown(string inclusionId)
        {
            var user = cMisc.GetCurrentUser();
            var returnValues = new List<ListItem>();
            var inclusionType = (StageInclusionType)int.Parse(inclusionId);

            switch (inclusionType)
            {
                case StageInclusionType.ClaimTotalExceeds:
                case StageInclusionType.ClaimTotalBelow:
                case StageInclusionType.OlderThanDays:
                    break;

                case StageInclusionType.IncludesCostCode:
                    returnValues = new cCostcodes(user.AccountID).CreateDropDown(false);
                    break;

                case StageInclusionType.IncludesDepartment:
                    returnValues = new cDepartments(user.AccountID).CreateDropDown(true);
                    break;

                case StageInclusionType.IncludesExpenseItem:
                    returnValues = new cSubcats(user.AccountID).CreateDropDown();
                    break;
            }
            return returnValues;
        }

        /// <summary>
        /// Populates the signoff type dropdown
        /// </summary>
        /// <param name="groupId">Id of the <see cref="cGroup"/></param>
        /// <param name="stageId">Id of the <see cref="SignoffType"/></param>
        /// <returns>A list of <see cref="ListItem"/> to put in the dropdown</returns>
        [WebMethod]
        public List<ListItem> PopulateSignoffTypeDropdown(int groupId, int stageId)
        {
            var returnValues = new List<ListItem>();
            CurrentUser user = cMisc.GetCurrentUser();
            var groups = new cGroups(user.AccountID);
            cGroup group = groups.GetGroupById(groupId);
            
            if (group.stagecount == 0)
            {
                returnValues.Add(SignoffType.ClaimantSelectsOwnChecker.ToListItem());
                returnValues.Add(SignoffType.ApprovalMatrix.ToListItem());
                returnValues.Add(SignoffType.DeterminedByClaimantFromApprovalMatrix.ToListItem());
            }

            if (stageId > 0)
            {
                var reqstage = group.stages[stageId];

                if (reqstage.stage == 1)
                {
                    returnValues.Add(SignoffType.ClaimantSelectsOwnChecker.ToListItem());
                    returnValues.Add(SignoffType.ApprovalMatrix.ToListItem());
                    returnValues.Add(SignoffType.DeterminedByClaimantFromApprovalMatrix.ToListItem());
                }
            }

            return returnValues;
        }

        /// <summary>
        /// Populates the holiday approver dropdown
        /// </summary>
        /// <param name="holidayTypeId">Id of the <see cref="StageInclusionType"/></param>
        /// <returns>A list of <see cref="ListItem"/> to put in the dropdown</returns>
        [WebMethod]
        public List<ListItem> PopulateHolidayApproverDropdown(int holidayTypeId)
        {
            var user = cMisc.GetCurrentUser();
            var returnedValues = new List<ListItem>();

            switch ((SignoffType)holidayTypeId)
            {
                case SignoffType.BudgetHolder:
                    returnedValues = new cBudgetholders(user.AccountID).CreateDropDown();
                    break;

                case SignoffType.Employee:
                    returnedValues =  new cEmployees(user.AccountID).CreateCheckPayDropDown(holidayTypeId, user.AccountID).ToList();
                    break;

                case SignoffType.Team:
                    returnedValues = new cTeams(user.AccountID).CreateDropDown(holidayTypeId).ToList();
                    break;

                case SignoffType.LineManager:
                case SignoffType.CostCodeOwner:
                case SignoffType.AssignmentSignOffOwner:
                case SignoffType.None:
                case SignoffType.ClaimantSelectsOwnChecker:
                case SignoffType.ApprovalMatrix:
                case SignoffType.DeterminedByClaimantFromApprovalMatrix:
                case SignoffType.SELScanAttach:
                case SignoffType.SELValidation:
                    break;
            }

            return returnedValues;
        }

        /// <summary>
        /// Validate a <see cref="cGroup"/>
        /// </summary>
        /// <param name="groupId">The id of the <see cref="cGroup"/></param>
        /// <returns>Validation message</returns>
        [WebMethod]
        public string ValidateGroup(int groupId)
        {
            if (groupId == 0)
            {
                return "";
            }

            var user = cMisc.GetCurrentUser();

            if (!user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SignOffGroups, true))
            {
                return "-1";
            }

            var groups = new cGroups(user.AccountID);
            var group = groups.GetGroupById(groupId);
            
            //validate the group's stages.
            var stageValidity = groups.ValidateGroupStages(group);

            return !stageValidity.Result ? $"{stageValidity.Messages.Aggregate((a, b) => a + "<br/>" + b)}" : "";
        }
    }
}
