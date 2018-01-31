using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Web.Script.Services;
using SpendManagementLibrary;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace Spend_Management
{
    /// <summary>
    /// Workflow specific methods.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class svcWorkflows : System.Web.Services.WebService
    {
        private readonly Regex regGuid = new Regex("[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Saves the workflow to the database
        /// </summary>
        /// <param name="workflowID">0 for a new workflow or the workflowID for an update</param>
        /// <param name="workflowName">Name of the workflow</param>
        /// <param name="description">Description of the workflow</param>
        /// <param name="canBeChild">If it can be run as a child workflow</param>
        /// <param name="runOnCreation">If it can be run when a record is created</param>
        /// <param name="runOnChange">If it can be run when a record is changed</param>
        /// <param name="runOnDelete">If it can be run when a record is deleted</param>
        /// <param name="steps">Steps object</param>
        /// <param name="workflowBaseTable">The base table which this workflow is for (guid from tables)</param>
        /// <returns>ID of the workflow.</returns>
        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public int SaveWorkflow(int workflowID, string workflowName, string description, string workflowBaseTable, bool canBeChild, bool runOnCreation, bool runOnChange, bool runOnDelete, object[][] steps)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Workflows, true) == false)
            {
                return -3;
            }

            cWorkflows clsWorkflows = new cWorkflows(currentUser);

            Guid gBaseTableID = new Guid(workflowBaseTable);

            WorkflowType workflowType;
            
            if (workflowBaseTable == "d70d9e5f-37e2-4025-9492-3bcf6aa746a8")
            {
                workflowType = WorkflowType.ClaimApproval;
            }
            else if (workflowBaseTable == "618DB425-F430-4660-9525-EBAB444ED754")
            {
                workflowType = WorkflowType.SelfRegistration;
            }
            else
            {
                workflowType = WorkflowType.CustomTable;
            }

            object[,] convertedSteps;

            #region convert javascript array into c# one
            if (steps != null)
            {
                convertedSteps = new object[steps.Length,16];
                for(int i = 0; i < steps.Length; i++) 
                {
                    convertedSteps[i, 0] = steps[i][0];
                    convertedSteps[i, 1] = steps[i][1];
                    convertedSteps[i, 2] = steps[i][2];
                    convertedSteps[i, 3] = steps[i][3];
                    convertedSteps[i, 4] = steps[i][4];
                    convertedSteps[i, 5] = steps[i][5];
                    convertedSteps[i, 6] = steps[i][6];
                    convertedSteps[i, 7] = steps[i][7];
                    convertedSteps[i, 8] = steps[i][8];
                    convertedSteps[i, 9] = steps[i][9];
                    convertedSteps[i, 10] = steps[i][10];
                    convertedSteps[i, 11] = steps[i][11];
                    convertedSteps[i, 12] = steps[i][12];
                    convertedSteps[i, 13] = steps[i][13];
                    convertedSteps[i, 14] = steps[i][14];
                    convertedSteps[i, 15] = steps[i][15];
                }
            }
            else
            {
                convertedSteps = null;
            }
            #endregion convert javascript array into c# one

            return clsWorkflows.SaveWorkFlow(workflowID, workflowType, workflowName, description, gBaseTableID, canBeChild, runOnCreation, runOnChange, runOnDelete, currentUser.EmployeeID, convertedSteps);
        }

        /// <summary>
        /// Deletes a workflow from the database
        /// </summary>
        /// <param name="workflowID">WorkflowID</param>
        /// <returns></returns>
        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public bool DeleteWorkflow(int workflowID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            cWorkflows clsWorkflows = new cWorkflows(currentUser);
            return clsWorkflows.DeleteWorkflowByID(workflowID);
        }
   
        /// <summary>
        /// Delete all workflow steps from a workflow.
        /// </summary>
        /// <param name="workflowID">The workflowID that you want to delete all the steps for</param>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public void DeleteWorkflowSteps(int workflowID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cWorkflows clsWorkflows = new cWorkflows(currentUser);
            clsWorkflows.DeleteWorkflowSteps(workflowID);
        }

        /// <summary>
        /// Returns a list of tables that you can join your base table on to.
        /// </summary>
        /// <param name="baseTableID">Your base tableID as a string</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<sTableBasics> GetAllowedTables(string baseTableID)
        {
            if (string.IsNullOrEmpty(baseTableID) == true || regGuid.IsMatch(baseTableID) == false)
            {
                throw new FormatException("baseTableID can not be converted to a Guid");
            }

            Guid gBaseTableID = new Guid(baseTableID);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            List<sTableBasics> lstTables = new List<sTableBasics>();
            cTables clsTables = new cTables(currentUser.AccountID);
            lstTables = clsTables.getAllowedTables(gBaseTableID, cAccounts.getConnectionString(currentUser.AccountID));
            return lstTables;
        }

        /// <summary>
        /// Returns a list of fields that are related to tableID and can be updated or searched on dependin on your update parameter
        /// </summary>
        /// <param name="tableID"></param>
        /// <param name="update">If the fields are to be updated set true, else enter false</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<sFieldBasics> GetTableFields(string tableID, bool update)
        {
            if (string.IsNullOrEmpty(tableID) == true || regGuid.IsMatch(tableID) == false)
            {
                throw new FormatException("tableID can not be converted to a Guid");
            }

            Guid gTableID = new Guid(tableID);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            List<sFieldBasics> lstFields = new List<sFieldBasics>();
            cFields clsFields = new cFields(currentUser.AccountID);
            lstFields = clsFields.GetFieldBasicsByTableID(gTableID, update);
            return lstFields;
        }

        /// <summary>
        /// Returns a list of fields that are related to tableID and can be updated or searched on dependin on your update parameter
        /// </summary>
        /// <param name="tableID"></param>
        /// <param name="update">If the fields are to be updated set true, else enter false</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<sFieldBasics> GetTableFieldsByDatatype(string tableID, bool update, DataType dataType)
        {
            if (string.IsNullOrEmpty(tableID) == true || regGuid.IsMatch(tableID) == false)
            {
                throw new FormatException("tableID can not be converted to a Guid");
            }

            Guid gTableID = new Guid(tableID);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            List<sFieldBasics> lstFields = new List<sFieldBasics>();
            cFields clsFields = new cFields(currentUser.AccountID);
            lstFields = clsFields.GetFieldBasicsByTableIDAndDatatype(gTableID, update, dataType);
            return lstFields;
        }

        /// <summary>
        /// Returns a list of approvers for approverType
        /// </summary>
        /// <param name="appoverType"></param>
        /// <param name="entityID"></param>
        /// <param name="approverID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<System.Web.UI.WebControls.ListItem> GetApproverList(int appoverType, int entityID, int? approverID)
        {
            List<System.Web.UI.WebControls.ListItem> lstApprovers = new List<System.Web.UI.WebControls.ListItem>();
            CurrentUser user = cMisc.GetCurrentUser();

            switch (appoverType)
            {
                case 1://Budget Holders
                    cBudgetholders clsBudgetHolders = new cBudgetholders(user.AccountID);
                    lstApprovers.AddRange(clsBudgetHolders.CreateDropDown().ToArray());
                    break;
                case 2:// Employees
                    cEmployees clsEmployees = new cEmployees(user.AccountID);
                    lstApprovers.AddRange(clsEmployees.CreateCheckPayDropDown(entityID, user.AccountID).ToArray());
                    break;
                case 3://Team
                    cTeams clsTeams = new cTeams(user.AccountID);
                    lstApprovers.AddRange(clsTeams.CreateDropDown(entityID).ToArray());
                    break;
                case 4: break;//Line Manager // return list with 0 items
                case 5: break;//Determined by claimant // return list with 0 items                        
            }

            if (approverID.HasValue)
            {
                foreach (ListItem lstItem in lstApprovers)
                {
                    if (lstItem.Value == approverID.Value.ToString())
                    {
                        lstItem.Selected = true;
                    }
                }
            }

            return lstApprovers;
        }

        /// <summary>
        /// Returns a string with all of the relevant DOM objects for a specific workflow action/type
        /// </summary>
        /// <param name="stepAction">The step action (int)WorkflowStepAction</param>
        /// <param name="workflowType">workflows base table tableID</param>
        /// <param name="editMode">If this step is currently in edit or not</param>
        /// <param name="workflowID">WorkflowID</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<string> GetStepActionFields(int stepAction, string workflowType, int workflowID, bool editMode) {

            List<string> returnValues = new List<string>();

            returnValues.Add(stepAction.ToString());
            returnValues.Add(workflowType.ToString());
            returnValues.Add(editMode.ToString());

            CurrentUser currentUser = cMisc.GetCurrentUser();
            Guid gWorkflowType = new Guid(workflowType);
            cWorkflows clsWorkflows = new cWorkflows(currentUser);
            WorkFlowStepAction eAction = (WorkFlowStepAction)stepAction;
            NotificationTemplates notifications;
            List<ListItem> lstEmailTemplates;
            StringBuilder sbReturnString = new StringBuilder();

            // Every step has a description input box
            sbReturnString.Append("<div class=\"onecolumnsmall\"><label for=\"txtStepDescription\">Description</label><span class=\"inputs\"><input type=\"text\" id=\"txtStepDescription\" style=\"width: 80%;\" title=\"Step Description\" /></span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\"></span></div>");

            switch (eAction)
            {
                case WorkFlowStepAction.Approval:
                    #region Approval
                    sbReturnString.Append("<div class=\"twocolumn\">");
                    sbReturnString.Append("<label for=\"approvalType\">Approver Type</label><span class=\"inputs\">");
                    sbReturnString.Append("<select id=\"approvalType\" onchange=\"populateApproverSelect();\" title=\"Approver Type\">");
                    sbReturnString.Append("<option value=\"0\">Select Approver Type</option>");
                    sbReturnString.Append("<option value=\"1\">" + SignoffType.BudgetHolder.GetDisplayValue() + "</option>");
                    sbReturnString.Append("<option value=\"2\">" + SignoffType.Employee.GetDisplayValue() + "</option>");
                    sbReturnString.Append("<option value=\"3\">" + SignoffType.Team.GetDisplayValue() + "</option>");
                    sbReturnString.Append("<option value=\"4\">" + SignoffType.LineManager.GetDisplayValue() + "</option>");
                    sbReturnString.Append("<option value=\"5\">" + SignoffType.ClaimantSelectsOwnChecker.GetDisplayValue() + "</option>");
                    sbReturnString.Append("</select>");
                    sbReturnString.Append("</span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\"></span>");
                        
                    sbReturnString.Append("<label for=\"approverSelect\">Approver</label><span class=\"inputs\"><select id=\"approverSelect\" style=\"display: none;\" title=\"Selected Approver\"></select></span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\"></span>");
                    sbReturnString.Append("</div>");


                    //sbReturnString.Append("<tr><td class=\"labeltd\">Involvement:</td><td class=\"inputtd\">");
                    //sbReturnString.Append(".!.</td></tr>");

                    notifications = new NotificationTemplates(currentUser);
                    lstEmailTemplates = notifications.CreateDropDown(gWorkflowType);

                    if (lstEmailTemplates.Count > 0)
                    {
                        sbReturnString.Append("<div class=\"twocolumn\">");
                            sbReturnString.Append("<label for=\"emailTemplate\">Notification Template</label><span class=\"inputs\"><select id=\"emailTemplate\">");
                            sbReturnString.Append("<option value=\"0\">[None]</option>");

                            foreach (ListItem lstItem in lstEmailTemplates)
                            {
                                sbReturnString.Append("<option value=\"" + lstItem.Value + "\">" + lstItem.Text + "</option>");
                            }

                            sbReturnString.Append("</select></span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\"></span></div>");
                    }
                    else
                    {
                        sbReturnString.Append("<div class=\"onecolumnsmall\"><label>Notification Template</label><span class=\"inputs\">No valid notification templates are currently setup</span></div>");
                    }

                    if (workflowType == "d70d9e5f-37e2-4025-9492-3bcf6aa746a8")
                    {
                        sbReturnString.Append("<div class=\"twocolumn\">");
                        sbReturnString.Append("<label for=\"oneclicksignoff\">One Click Signoff</label><span class=\"inputs\"><input type=\"checkbox\" id=\"oneclicksignoff\" title=\"One Click Signoff\" /></span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\"></span>");
                        sbReturnString.Append("<label for=\"approvalShowDeclaration\">Show Declaration</label><span class=\"inputs\"><input type=\"checkbox\" onclick=\"showapprovaldeclaration();\" id=\"approvalShowDeclaration\" title=\"Show Declaration\" /></span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\"></span>");
                        sbReturnString.Append("</div>");
                        sbReturnString.Append("<span id=\"approvalDeclaration\" style=\"display: none;\"><div class=\"onecolumnsmall\"><label for=\"txtQuestion\">Question</label><span class=\"inputs\"><input type=\"text\" id=\"txtQuestion\" style=\"width: 80%;\" title=\"Question\" /></span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\"></span></div>");
                        sbReturnString.Append("<div class=\"onecolumnsmall\"><label for=\"txtTrueButton\">Yes/ Button Text</label><span class=\"inputs\"><input type=\"text\" id=\"txtTrueButton\" style=\"width: 80%;\" title=\"Yes/True Button Text\" /></span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\"></span></div>");
                        sbReturnString.Append("<div class=\"onecolumnsmall\"><label for=\"txtFalseButton\">No/False Button Text</label><span class=\"inputs\"><input type=\"text\" id=\"txtFalseButton\" style=\"width: 80%;\" title=\"No/False Button Text\" /></span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\"></span></div></span>");
                    }
                    break;
                    #endregion
                case WorkFlowStepAction.Decision:
                case WorkFlowStepAction.DecisionFalse:
                    #region descision question txt + true/false buttons 
                        sbReturnString.Append("<div class=\"onecolumnsmall\"><label for=\"txtQuestion\">Question</label><span class=\"inputs\"><input type=\"text\" id=\"txtQuestion\" style=\"width: 80%;\" title=\"Question\" /></span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\"></span></div>");
                        sbReturnString.Append("<div class=\"onecolumnsmall\"><label for=\"txtTrueButton\">Yes/ Button Text</label><span class=\"inputs\"><input type=\"text\" id=\"txtTrueButton\" style=\"width: 80%;\" title=\"Yes/True Button Text\"  /></span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\"></span></div>");
                        sbReturnString.Append("<div class=\"onecolumnsmall\"><label for=\"txtFalseButton\">No/False Button Text</label><span class=\"inputs\"><input type=\"text\" id=\"txtFalseButton\" style=\"width: 80%;\" title=\"No/False Button Text\"  /></span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\"></span></div>");
                    break;
                    #endregion
                case WorkFlowStepAction.ChangeValue:
                case WorkFlowStepAction.CheckCondition:
                case WorkFlowStepAction.ElseCondition:
                    break;
                case WorkFlowStepAction.ElseOtherwise:
                    sbReturnString.Append("No additional information is required for this step");
                    break;
                case WorkFlowStepAction.FinishWorkflow:
                    sbReturnString.Append("No additional information is required for this step");
                    break;
                case WorkFlowStepAction.MoveToStep:
                    #region movetostep - ddl to select the step to move to.
                    sbReturnString.Append("<div class=\"twocolumn\"><label for=\"ddlSelectedMoveToStep\">Step</label><span class=\"inputs\"><select id=\"ddlSelectedMoveToStep\" style=\"width: 80%;\" title=\"Move to step\"></select></span>");
                    break;
                    #endregion                    
                case WorkFlowStepAction.RunSubWorkflow:
                    #region runsubworkflow - ddl for subworkflows
                    List<cWorkflow> lstSubWorkflows = clsWorkflows.GetSelectableSubWorkflows(gWorkflowType, workflowID);

                    if (lstSubWorkflows.Count > 0)
                    {
                        sbReturnString.Append("<div class=\"twocolumn\">");
                        sbReturnString.Append("<label for=\"ddlSelectedRunSubWorkflows\">Workflow</label><span class=\"inputs\"><select id=\"ddlSelectedRunSubWorkflows\">");
                        sbReturnString.Append("<option value=\"0\">[None]</option>");

                        cWorkflow tmpWorkflow;
                        for (int i = 0; i < lstSubWorkflows.Count; i++)
                        {
                            tmpWorkflow = lstSubWorkflows[i];
                            sbReturnString.Append("<option value=\"" + tmpWorkflow.workflowid + "\">" + tmpWorkflow.workflowname + "</option>");
                        }


                        sbReturnString.Append("</select></span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\"></span></div>");
                    }
                    else
                    {
                        sbReturnString.Append("<div class=\"onecolumnsmall\"><label>Workflow</label><span class=\"inputs\">There are currently no workflows allowed to be run as sub-workflows</span></div>");
                    }

                    break;
                    #endregion
                case WorkFlowStepAction.SendEmail:
                    #region send emails
                        notifications = new NotificationTemplates(currentUser);
                        lstEmailTemplates = notifications.CreateDropDown(gWorkflowType);

                        if (lstEmailTemplates.Count > 0)
                        {
                            sbReturnString.Append("<div class=\"twocolumn\">");
                            sbReturnString.Append("<label for=\"emailTemplate\">Notification Template</label><span class=\"inputs\"><select id=\"emailTemplate\">");
                            sbReturnString.Append("<option value=\"0\">[None]</option>");

                            foreach (ListItem lstItem in lstEmailTemplates)
                            {
                                sbReturnString.Append("<option value=\"" + lstItem.Value + "\">" + lstItem.Text + "</option>");
                            }

                            sbReturnString.Append("</select></span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\"></span></div>");
                        }
                        else
                        {
                            sbReturnString.Append("<div class=\"onecolumnsmall\"><label for=\"emailTemplate\">Notification Template</label><span class=\"inputs\">No valid Notificatioin Templates are currently setup</span></div>");
                        }
                        break;
                    #endregion
                case WorkFlowStepAction.CreateDynamicValue:
                    sbReturnString.Append("No additional information is required for this step");
                    break;
                case WorkFlowStepAction.ShowMessage:
                    sbReturnString.Append("<div class=\"onecolumnsmall\"><label for=\"txtShowMessage\">Message</label><span class=\"inputs\"><textarea id=\"txtMessage\" style=\"width: 80%;\" title=\"Message\"></textarea></span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\"></span></div>");
                    break;
                case WorkFlowStepAction.ChangeCustomEntityForm:
                    #region change custom entity form
                    cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                    cCustomEntity reqCustomEntity = clsCustomEntities.getEntityByTableId(gWorkflowType);
                        sbReturnString.Append("<div class=\"twocolumn\">");
                        sbReturnString.Append("<label for=\"ddlFormID\">Form</label><span class=\"inputs\"><select id=\"ddlFormID\">");
                        sbReturnString.Append("<option value=\"0\">[None]</option>");

                        List<ListItem> lstForms = reqCustomEntity.CreateFormDropDown();

                        foreach (ListItem listItem in lstForms)
                        {
                            sbReturnString.Append("<option value=\"" + listItem.Value + "\">" + listItem.Text + "</option>");
                        }

                        sbReturnString.Append("</select></span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\"></span></div>");
                    break;
                    #endregion change custom entity form
                default:
                    sbReturnString.Append("Invalid step action");
                    break;
            }


            returnValues.Add(sbReturnString.ToString());

            return returnValues;
        }

        /// <summary>
        /// Returns the selectable fields for the selected fieldID
        /// </summary>
        /// <param name="fieldID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] SelectableConditions(string fieldID)
        {
            if (string.IsNullOrEmpty(fieldID) == true || regGuid.IsMatch(fieldID) == false)
            {
                throw new FormatException("fieldID can not be converted to a Guid");
            }

            Guid gFieldID = new Guid(fieldID);

            object[] returnContainer = new object[4];

            List<System.Web.UI.WebControls.ListItem> lstConditions = new List<System.Web.UI.WebControls.ListItem>();
            List<System.Web.UI.WebControls.ListItem> lstValues = new List<System.Web.UI.WebControls.ListItem>();

            returnContainer[0] = lstConditions;
            returnContainer[1] = lstValues;
            returnContainer[2] = "";
            returnContainer[3] = fieldID;

            CurrentUser user = cMisc.GetCurrentUser();

            cFields clsFields = new cFields(user.AccountID);

            cField reqField = clsFields.GetFieldByID(gFieldID);

            returnContainer[2] = reqField.FieldType;


            switch (reqField.FieldType)
            {
                case "FS":
                case "S":
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Equals", "1"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Does Not Equal", "2"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Contains Data", "9"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Does Not Contain Data", "10"));
                    if (reqField.GenList == false)
                    {
                        lstConditions.Add(new System.Web.UI.WebControls.ListItem("Like (%)", "7"));
                    }
                    break;
                case "M":
                case "N":
                case "C":
                case "FD":
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Equals", "1"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Does Not Equal", "2"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Greater Than", "3"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Less Than", "4"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Greater Than or Equal To", "5"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Less Than or Equal To", "6"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Between", "8"));
                    break;
                case "X":
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Equals", "1"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Does Not Equal", "2"));
                    break;
                case "D": // date
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("On", "37"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Not On", "38"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("After", "39"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Before", "40"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("On or After", "41"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("On or Before", "42"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Between", "8"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Yesterday", "11"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Today", "12"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Tomorrow", "12"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Next 7 Days", "14"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Last 7 Days", "15"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Next Week", "16"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Last Week", "17"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("This Week", "18"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Next Month", "19"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Last Month", "20"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("This Month", "21"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Next Year", "22"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Last Year", "23"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("This Year", "24"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Next Tax Year", "43"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Last Tax Year", "44"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("This Tax Year", "45"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Next Financial Year", "25"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Last Financial Year", "26"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("This Financial Year", "27"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Last X Days", "28"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Next X Days", "29"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Last X Weeks", "30"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Next X Weeks", "31"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Last X Months", "32"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Next X Months", "33"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Last X Years", "34"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Next X Years", "35"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Any Time", "36"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Contains Data", "9"));
                    lstConditions.Add(new System.Web.UI.WebControls.ListItem("Does Not Contain Data", "10"));
                    break;
                default:
                    break;
            }


            if (reqField.GenList == true)
            {
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(user.AccountID));
                //string strSQL = "SELECT [" + reqField.table.primarykeyfield.field + "] AS primaryKey, [" + reqField.table.stringkeyfield.field + "] AS keyField FROM [" + reqField.table.tablename + "] ORDER BY [" + reqField.table.stringkeyfield.field + "];";

                string strSQL = "SELECT [" + reqField.GetParentTable().GetPrimaryKey().FieldName + "] AS primaryKey, [" + reqField.GetParentTable().GetKeyField().FieldName + "] AS keyField FROM [" + reqField.GetParentTable().TableName + "] ORDER BY [" + reqField.GetParentTable().GetKeyField().FieldName + "];";

                int primaryKey;
                string keyField;

                using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strSQL))
                {
                    while (reader.Read())
                    {
                        primaryKey = reader.GetInt32(reader.GetOrdinal("primaryKey"));
                        keyField = reader.GetString(reader.GetOrdinal("keyField"));
                        lstValues.Add(new System.Web.UI.WebControls.ListItem(keyField, primaryKey.ToString()));
                    }

                    reader.Close();
                }
            }
            else if (reqField.ValueList == true)
            {
                foreach (string tmpItem in reqField.ListItems.Values)
                {
                    lstValues.Add(new System.Web.UI.WebControls.ListItem(tmpItem, reqField.ListItems.IndexOfValue(tmpItem).ToString()));
                }
            }

            return returnContainer;
        }

        /// <summary>
        /// Builds the fields_selector table from scratch when editing a step, then returns the string and an counter, allowing javascript to continue adding rows
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="update"></param>
        /// <param name="baseTableID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<string> RebuildFieldSelectorForEdit(object[][] criteria, bool update, string baseTableID)
        {
            
            CurrentUser currentUser = cMisc.GetCurrentUser();
            int rowIndex = 0;
            StringBuilder sbTable = new StringBuilder();
            List<string> lstReturnStrings = new List<string>();
            Guid fieldID;
            Int16 conditionType;
            CriteriaMode criteriaMode;
            bool runtime;
            string valueOne;
            string valueTwo;
            cField reqField;
            cFields clsFields = new cFields(currentUser.AccountID);
            
            List<sTableBasics> lstTables = this.GetAllowedTables(baseTableID);

            sbTable.Append("<table id=\"tbl\" class=\"datatbl\" style=\"width: 710px;\" border=\"1\">");
            sbTable.Append("<thead><tr>");
            sbTable.Append("<th width=\"20\"><img src=\"/shared/images/icons/delete2.gif\" height=\"16\" width=\"16\" alt=\"X\" title=\"X\" /></th>");
            sbTable.Append("<th style=\"width: 150px;\">Element</th>");
            sbTable.Append("<th style=\"width: 150px;\">Field</th>");
            sbTable.Append("<th style=\"width: 150px;\">Operator</th>");
            sbTable.Append("<th style=\"width: 290px;\">Value 1</th>");
            sbTable.Append("<th style=\"width: 145px;");
            if (update == true)
            {
                sbTable.Append(" display:none;");
            }
            sbTable.Append("\">Value 2</th>");
            sbTable.Append("<th style=\"width: 40px;\"");
            
            if (update == false)
            {
                sbTable.Append(" style=\"display: none;\"");
            }

            sbTable.Append(">Runtime</th></tr></thead>");
            sbTable.Append("<tbody>");

            for (rowIndex = 0; rowIndex < criteria.Length; rowIndex++)
            {
                fieldID = new Guid((string)criteria[rowIndex][0]);

                reqField = clsFields.GetFieldByID(fieldID);

                conditionType = Convert.ToInt16(criteria[rowIndex][1]);
                criteriaMode = (CriteriaMode)Convert.ToInt32(criteria[rowIndex][2]);
                runtime = Convert.ToBoolean(criteria[rowIndex][3]);
                valueOne = Convert.ToString(criteria[rowIndex][4]);
                valueTwo = Convert.ToString(criteria[rowIndex][5]);

                sbTable.Append("<tr id=\"" + rowIndex + "\">\n");
                sbTable.Append("<td classname=\"fieldstbl\"><span classname=\"visableclass\" class=\"visableclass\" id=\"" + rowIndex + "_1\"><img src=\"/shared/images/icons/");
                
                if(rowIndex == 0) 
                {
                    sbTable.Append("delete_disabled.png\"");
                } 
                else 
                {
                    sbTable.Append("delete2.gif\" onclick=\"javascript:deleteRow(" + rowIndex + ");\" ");
                }
                
                sbTable.Append(" alt=\"Delete criteria\" title=\"Delete criteria\" align=\"absmiddle\" border=\"0\"></span></td>\n");
                sbTable.Append("<td id=\"" + rowIndex + "_0\"><span classname=\"hiddenclass\" class=\"hiddenclass\" id=\"" + rowIndex + "_2\"><a href=\"javascript:void(0);\">Element</a></span><span classname=\"visableclass\" class=\"visableclass\" id=\"" + rowIndex + "_3\"><select id=\"" + rowIndex + "_ddlTable\" onchange=\"selectTable(" + rowIndex + ");\">");
                
                foreach(sTableBasics table in lstTables) 
                {
                    sbTable.Append("<option value=\"" + table.TableID.ToString() + "\"");
                    
                    if(table.TableID == reqField.TableID) 
                    {
                            sbTable.Append(" selected");
                    }

                    sbTable.Append(">" + table.Description + "</option>");
                }
                
                List<sFieldBasics> lstFields = this.GetTableFields(reqField.TableID.ToString(), update);

                sbTable.Append("</select></span></td>\n");
                sbTable.Append("<td><span classname=\"visableclass\" class=\"visableclass\" id=\"" + rowIndex + "_4\"><select id=\"" + rowIndex + "_ddlField\" onchange=\"selectField(" + rowIndex + ");\">");

                foreach(sFieldBasics field in lstFields) 
                {
                    sbTable.Append("<option value=\"" + field.FieldID.ToString() + "\"");

                    if (field.FieldID == reqField.FieldID)
                    {
                        sbTable.Append(" selected");
                    }

                    sbTable.Append(">" + field.Description + "</option>");
                }

                
                
                sbTable.Append("</select></span></td>\n");

                object[] conditions = SelectableConditions(reqField.FieldID.ToString());

                #region Operator
                sbTable.Append("<td><span classname=\"visableclass\" class=\"visableclass\" id=\"" + rowIndex + "_5\"><select id=\"" + rowIndex + "_ddlCondition\" onchange=\"selectedOperatorChanged(" + rowIndex + ");\">");

                for (int i = 0; i < ((List<ListItem>)conditions[0]).Count; i++)
                {
                    sbTable.Append("<option value=\"" + ((ListItem)((List<ListItem>)conditions[0])[i]).Value + "\"");

                    if (conditionType.ToString() == ((ListItem)((List<ListItem>)conditions[0])[i]).Value)
                    {
                        sbTable.Append(" selected");
                    }
                    
                    sbTable.Append(">" + ((ListItem)((List<ListItem>)conditions[0])[i]).Text + "</option>");
                }

                sbTable.Append("</select></span></td>\n");
                #endregion Operator

                #region Value One

                sbTable.Append("<td><span classname=\"visableclass\" class=\"visableclass\" id=\"" + rowIndex + "_6\">");

                if (((List<ListItem>)conditions[1]).Count == 0)
                {
                    if (reqField.FieldType == "X")
                    {
                        sbTable.Append("<select id=\"" + rowIndex + "_value\">");

                        sbTable.Append("<option value=\"1\""); 
                        
                        if(valueOne.ToString() == "1") {
                            sbTable.Append(" selected");
                        }

                        sbTable.Append(">Yes</option>");

                        sbTable.Append("<option value=\"0\"");

                        if(valueOne.ToString() == "0") {
                            sbTable.Append(" selected");
                        }

                        sbTable.Append(">No</option>");


                        sbTable.Append("</select>");
                    }
                    else
                    {
                        sbTable.Append("<input id=\"" + rowIndex + "_value\" size=\"10\" type=\"text\" value=\"" + valueOne.ToString() + "\">");
                    }
                }
                else
                {
                    sbTable.Append("<select id=\"" + rowIndex + "_value\">");

                    for (int i = 0; i < ((List<ListItem>)conditions[1]).Count; i++)
                    {
                        sbTable.Append("<option value=\"" + ((ListItem)((List<ListItem>)conditions[1])[i]).Value + "\">" + ((ListItem)((List<ListItem>)conditions[1])[i]).Text + "</option>");
                    }

                    sbTable.Append("</select>");
                }
                
                sbTable.Append("</span></td>\n");
                #endregion Value One

                #region Value Two

                sbTable.Append("<td");

                if (update == true)
                {
                    sbTable.Append(" style=\"display: none;\"");
                }

                sbTable.Append("><span style=\"\" id=\"" + rowIndex + "_7\"");
                
                sbTable.Append(">");
                
                sbTable.Append("<input id=\"" + rowIndex + "_value2\" size=\"10\" type=\"text\"");

                if (conditionType != 8)
                {
                    sbTable.Append(" style=\"display:none;\"");
                }
                else
                {
                    sbTable.Append(" value=\"" + valueTwo.ToString() + "\"");
                }
                
                sbTable.Append(">");
                
                sbTable.Append("</span></td>\n");

                #endregion Value Two

                sbTable.Append("<td align=\"center\"");

                if (update == false)
                {
                    sbTable.Append(" style=\"display: none;\"");
                }

                sbTable.Append("><span><input value=\"true\" id=\"" + rowIndex + "_runtime\" type=\"checkbox\"");

                if (runtime == true)
                {
                    sbTable.Append(" checked=\"checked\"");
                }

                sbTable.Append("></span></td>\n");
                sbTable.Append("</tr>\n");
            }

            sbTable.Append("</tbody></table>");


            lstReturnStrings.Add(sbTable.ToString());
            lstReturnStrings.Add(rowIndex.ToString());

            return lstReturnStrings;
        }

        /// <summary>
        /// Completes a decision step
        /// </summary>
        /// <param name="entityID">Entity ID</param>
        /// <param name="workflowID">Workflow ID</param>
        /// <param name="responseToDecision">True if the true/yes options is selected, false otherwise</param>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public cWorkflowNextStep CompleteDecisionStep(int entityID, int workflowID, bool responseToDecision)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cWorkflows clsWorkflows = new cWorkflows(currentUser);
            cWorkflowNextStep reqNextStep = null;
            cWorkflowEntityDetails reqEntityDetails = clsWorkflows.GetCurrentEntityStatus(entityID, workflowID);

            if (reqEntityDetails != null)
            {
                cWorkflow reqWorkflow = clsWorkflows.GetWorkflowByID(workflowID);

                if (reqWorkflow != null)
                {
                    if (reqWorkflow.Steps[reqEntityDetails.StepNumber].Action == WorkFlowStepAction.Decision)
                    {
                        reqNextStep = clsWorkflows.UpdateDecisionStep(entityID, workflowID, responseToDecision);
                    }
                    else
                    {
                        reqNextStep = new cWorkflowNextStep(clsWorkflows.GetWorkflowStatusByStepType(reqWorkflow.Steps[reqEntityDetails.StepNumber].Action), reqWorkflow.Steps[reqEntityDetails.StepNumber]);
                    }
                }
            }
            return reqNextStep;
        }

        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public cWorkflowNextStep CompleteApprovalStep(int entityID, int workflowID, bool responseToApproval)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cWorkflows clsWorkflows = new cWorkflows(currentUser);
            cWorkflowNextStep reqNextStep = null;
            cWorkflowEntityDetails reqEntityDetails = clsWorkflows.GetCurrentEntityStatus(entityID, workflowID);
            cWorkflow reqWorkflow = clsWorkflows.GetWorkflowByID(workflowID);

            if (reqEntityDetails != null && reqWorkflow != null)
            {
                if (reqWorkflow.Steps[reqEntityDetails.StepNumber].Action == WorkFlowStepAction.Approval)
                {
                    reqNextStep = clsWorkflows.UpdateApprovalStep(entityID, workflowID, responseToApproval, string.Empty);
                }
                else
                {
                    reqNextStep = new cWorkflowNextStep(clsWorkflows.GetWorkflowStatusByStepType(reqWorkflow.Steps[reqEntityDetails.StepNumber].Action), reqWorkflow.Steps[reqEntityDetails.StepNumber]);
                }
            }

            return reqNextStep;
        }
    }
}
