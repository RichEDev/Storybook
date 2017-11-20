using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Spend_Management;
using System.Web.Services;
using SpendManagementLibrary;
using System.Text;
using System.Collections.Generic;

namespace Spend_Management
{
    /// <summary>
    /// Add or edit a workflow
    /// </summary>
    public partial class aeworkflow : System.Web.UI.Page
    {
        /// <summary>
        /// The workflowID currently being edited
        /// </summary>
        public int nWorkflowID;
        /// <summary>
        /// The number of steps currently on the workflow at the point of page load
        /// </summary>
        public int nStepsCounter;
        /// <summary>
        /// The current workflow being edited
        /// </summary>
        public cWorkflow reqWorkflow;
        CurrentUser user;

        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;
            }

            if (string.IsNullOrEmpty(Request.QueryString["workflowID"]) == false)
            {
                Title = "Edit Workflow";

                if (Request.QueryString["tab"] != null && Request.QueryString["tab"] == "steps")
                {
                    tcWorkflow.ActiveTabIndex = 1;
                }

                int workflowID = 0;

                int.TryParse(Request.QueryString["workflowID"], out workflowID);

                if (workflowID == 0)
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }

                cWorkflows clsWorkflows = new cWorkflows(user);
                reqWorkflow = clsWorkflows.GetWorkflowByID(workflowID);

                if (reqWorkflow == null)
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }

                    nWorkflowID = reqWorkflow.workflowid; // Printed in the client side script block

                    imgAddStep.ImageUrl = "~/shared/images/buttons/btn_updatestep.gif";

                    if (reqWorkflow.Steps == null)
                    {
                        nStepsCounter = 0;
                    }
                    else
                    {
                        nStepsCounter = reqWorkflow.Steps.Count - 1; // 0 Indexed javascript counter for the number of steps added


                        StringBuilder sbExistingStepsArray = new StringBuilder();

                        cApprovalStep approvalStep;
                        cDecisionStep decisionStep;
                        cMovetoStepStep moveToStepStep;
                        cRunSubworkflowStep runSubWorkflowStep;
                        cSendEmailStep sendEmailStep;
                        cChangeValueStep changeValueStep;
                        cCheckConditionStep checkConditionStep;
                        cCreateDynamicValue createDynamicStep;
                        cShowMessageStep showMessageStep;
                        cChangeFormStep changeFormStep;

                        int stepID = 0;
                        
                        foreach (KeyValuePair<int, cWorkflowStep> step in reqWorkflow.Steps)
                        {
                            switch (step.Value.Action)
                            {
                                case WorkFlowStepAction.Decision:
                                case WorkFlowStepAction.DecisionFalse:
                                    decisionStep = (cDecisionStep)step.Value;
                                    sbExistingStepsArray.Append(AddStepArray(stepID, (int)decisionStep.Action, 0, decisionStep.Description, decisionStep.Question, decisionStep.TrueOption, decisionStep.FalseOption, new List<cWorkflowCriteria>(), 0, false, false, decisionStep.ParentStepID, decisionStep.RelatedStepID, null, string.Empty, null));
                                    stepID++;
                                    break;
                                case WorkFlowStepAction.MoveToStep:
                                    moveToStepStep = (cMovetoStepStep)step.Value;
                                    int movetoStepID = reqWorkflow.Steps.IndexOfKey(moveToStepStep.StepID);
                                    sbExistingStepsArray.Append(AddStepArray(stepID, (int)moveToStepStep.Action, movetoStepID, moveToStepStep.Description, string.Empty, string.Empty, string.Empty, new List<cWorkflowCriteria>(), 0, false, false, moveToStepStep.ParentStepID, moveToStepStep.RelatedStepID, null, string.Empty, null));
                                    stepID++;
                                    break;
                                case WorkFlowStepAction.RunSubWorkflow:
                                    runSubWorkflowStep = (cRunSubworkflowStep)step.Value;
                                    sbExistingStepsArray.Append(AddStepArray(stepID, (int)runSubWorkflowStep.Action, runSubWorkflowStep.SubWorkflowID, runSubWorkflowStep.Description, string.Empty, string.Empty, string.Empty, new List<cWorkflowCriteria>(), 0, false, false, runSubWorkflowStep.ParentStepID, runSubWorkflowStep.RelatedStepID, null, string.Empty, null));
                                    stepID++;
                                    break;
                                case WorkFlowStepAction.SendEmail:
                                    sendEmailStep = (cSendEmailStep)step.Value;
                                    sbExistingStepsArray.Append(AddStepArray(stepID, (int)sendEmailStep.Action, sendEmailStep.EmailTemplateID, sendEmailStep.Description, string.Empty, string.Empty, string.Empty, new List<cWorkflowCriteria>(), 0, false, false, sendEmailStep.ParentStepID, sendEmailStep.RelatedStepID, null, string.Empty, null));
                                    stepID++;
                                    break;
                                case WorkFlowStepAction.Approval: // need to add the filtereditems
                                    approvalStep = (cApprovalStep)step.Value;
                                    sbExistingStepsArray.Append(AddStepArray(stepID, (int)approvalStep.Action, approvalStep.ApproverID, approvalStep.Description, approvalStep.Question, approvalStep.TrueOption, approvalStep.FalseOption, approvalStep.FilteredItems, (int)approvalStep.ApproverType, approvalStep.OneClickSignOff, approvalStep.ShowDeclaration, approvalStep.ParentStepID, approvalStep.RelatedStepID, approvalStep.EmailTemplateID, string.Empty, null));
                                    stepID++;
                                    break;
                                case WorkFlowStepAction.ChangeValue:
                                    changeValueStep = (cChangeValueStep)step.Value;
                                    sbExistingStepsArray.Append(AddStepArray(stepID, (int)changeValueStep.Action, 0, changeValueStep.Description, string.Empty, string.Empty, string.Empty, changeValueStep.Criteria, 0, false, false, changeValueStep.ParentStepID, changeValueStep.RelatedStepID, null, string.Empty, null));
                                    stepID++;
                                    break;
                                case WorkFlowStepAction.CheckCondition:
                                case WorkFlowStepAction.ElseCondition:
                                case WorkFlowStepAction.ElseOtherwise:
                                    checkConditionStep = (cCheckConditionStep)step.Value;
                                    sbExistingStepsArray.Append(AddStepArray(stepID, (int)checkConditionStep.Action, 0, checkConditionStep.Description, string.Empty, string.Empty, string.Empty, checkConditionStep.Criteria, 0, false, false, checkConditionStep.ParentStepID, checkConditionStep.RelatedStepID, null, string.Empty, null));
                                    stepID++;
                                    break;
                                case WorkFlowStepAction.CreateDynamicValue:
                                    createDynamicStep = (cCreateDynamicValue)step.Value;
                                    sbExistingStepsArray.Append(AddStepArray(stepID, (int)createDynamicStep.Action, 0, createDynamicStep.Description, string.Empty, string.Empty, string.Empty, new List<cWorkflowCriteria>(), 0, false, false, createDynamicStep.ParentStepID, createDynamicStep.RelatedStepID, null, string.Empty, null));
                                    stepID++;
                                    break;
                                case WorkFlowStepAction.FinishWorkflow:
                                    sbExistingStepsArray.Append(AddStepArray(stepID, (int)step.Value.Action, 0, step.Value.Description, string.Empty, string.Empty, string.Empty, new List<cWorkflowCriteria>(), 0, false, false, step.Value.ParentStepID, step.Value.RelatedStepID, null, string.Empty, null));
                                    stepID++;
                                    break;
                                case WorkFlowStepAction.ShowMessage:
                                    showMessageStep = (cShowMessageStep)step.Value;
                                    sbExistingStepsArray.Append(AddStepArray(stepID, (int)showMessageStep.Action, 0, step.Value.Description, string.Empty, string.Empty, string.Empty, new List<cWorkflowCriteria>(), 0, false, false, step.Value.ParentStepID, step.Value.RelatedStepID, null, showMessageStep.Message, null));
                                    stepID++;
                                    break;
                                case WorkFlowStepAction.ChangeCustomEntityForm:
                                    changeFormStep = (cChangeFormStep)step.Value;
                                    sbExistingStepsArray.Append(AddStepArray(stepID, (int)changeFormStep.Action, 0, changeFormStep.Description, string.Empty, string.Empty, string.Empty, new List<cWorkflowCriteria>(), 0, false, false, changeFormStep.ParentStepID, null, null, string.Empty, changeFormStep.FormID));
                                    stepID++;
                                    break;
                                default:
                                    throw new Exception("Error loading all values into the existing workflow steps array.");
                            }
                        }

                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "scriptArray", sbExistingStepsArray.ToString(), true);
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "scriptRun", "window.onload = function() { addStepsToStepsInEdit() };", true);
                    }
            }
            else
            {
                Title = "Add Workflow";
                nWorkflowID = 0; // Printed in the client side script block
                nStepsCounter = 0; // 0 Indexed javascript counter for the number of steps added
            }

            Master.title = Title;
            Master.PageSubTitle = "Workflow Details";

            if (IsPostBack == false)
            {
                Master.enablenavigation = false;

                cTables clstables = new cTables((int)ViewState["accountid"]);

                cmbworkflowtype.Items.AddRange(clstables.CreateDropDown(cTables.DropDownType.AllowedWorkflow).ToArray());

                if (reqWorkflow != null)
                {
                    txtdescription.Text = reqWorkflow.description;
                    txtworkflowname.Text = reqWorkflow.workflowname;

                    if (cmbworkflowtype.Items.FindByValue(reqWorkflow.BaseTable.TableID.ToString()) != null)
                    {
                        cmbworkflowtype.Items.FindByValue(reqWorkflow.BaseTable.TableID.ToString()).Selected = true;
                    }

                    chkrunoncreation.Checked = reqWorkflow.runoncreation;
                    chkrunoncreation.InputAttributes.Add("onclick", "SetWorkflowChanged();");
                    chkrunonchange.Checked = reqWorkflow.runonchange;
                    chkrunonchange.InputAttributes.Add("onclick", "SetWorkflowChanged();");
                    chkrunondelete.Checked = reqWorkflow.runondelete;
                    chkrunondelete.InputAttributes.Add("onclick", "SetWorkflowChanged();");
                    chkchildworkflow.Checked = reqWorkflow.canbechildworkflow;
                    chkchildworkflow.InputAttributes.Add("onclick", "SetWorkflowChanged();");
                }
            }
        }

        /// <summary>
        /// Add an existing step to a javascript array for editing on aeworkflow.aspx
        /// </summary>
        /// <param name="stepID"></param>
        /// <param name="action"></param>
        /// <param name="actionID"></param>
        /// <param name="description"></param>
        /// <param name="question"></param>
        /// <param name="trueAnswer"></param>
        /// <param name="falseAnswer"></param>
        /// <param name="lstCriteria"></param>
        /// <param name="approverType"></param>
        /// <param name="oneClickSignOff"></param>
        /// <param name="showDeclaration"></param>
        /// <param name="parentStepID"></param>
        /// <param name="relatedStepID"></param>
        /// <param name="approvalEmailTemplateID"></param>
        /// <returns></returns>
        public string AddStepArray(int stepID, int action, int actionID, string description, string question, string trueAnswer, string falseAnswer, List<cWorkflowCriteria> lstCriteria, int approverType, bool oneClickSignOff, bool showDeclaration, int parentStepID, int? relatedStepID, int? approvalEmailTemplateID, string message, int? formID)
        {
            StringBuilder sbStep = new StringBuilder();
            sbStep.Append("currentSteps[" + stepID + "] = new Array();\n");
            sbStep.Append("currentSteps[" + stepID + "][\"action\"] = " + action + ";\n");
            sbStep.Append("currentSteps[" + stepID + "][\"description\"] = \"" + description.Replace("'", "\'") + "\";\n");
            sbStep.Append("currentSteps[" + stepID + "][\"question\"] = \"" + question.Replace("'", "\'") + "\";\n");
            sbStep.Append("currentSteps[" + stepID + "][\"trueAnswer\"] = \"" + trueAnswer.Replace("'", "\'") + "\";\n");
            sbStep.Append("currentSteps[" + stepID + "][\"falseAnswer\"] = \"" + falseAnswer.Replace("'", "\'") + "\";\n");

            sbStep.Append("currentSteps[" + stepID + "][\"conditions\"] = new Array();\n");

            cWorkflowCriteria tmpCriteria;
            if (lstCriteria != null)
            {
                for (int i = 0; i < lstCriteria.Count; i++)
                {
                    tmpCriteria = lstCriteria[i];
                    sbStep.Append("currentSteps[" + stepID + "][\"conditions\"][" + i + "] = new Array();\n");
                    if (tmpCriteria.Field != null)
                    {
                        sbStep.Append("currentSteps[" + stepID + "][\"conditions\"][" + i + "][0] = '" + tmpCriteria.Field.FieldID.ToString() + "';\n");
                    }
                    else
                    {
                        sbStep.Append("currentSteps[" + stepID + "][\"conditions\"][" + i + "][0] = '" + new Guid().ToString() + "';\n");
                    }
                    sbStep.Append("currentSteps[" + stepID + "][\"conditions\"][" + i + "][1] = '" + Convert.ToString((int)tmpCriteria.Condition) + "';\n");
                    sbStep.Append("currentSteps[" + stepID + "][\"conditions\"][" + i + "][2] = " + tmpCriteria.IsUpdateCriteria.ToString().ToLower() + ";\n");
                    sbStep.Append("currentSteps[" + stepID + "][\"conditions\"][" + i + "][3] = " + tmpCriteria.Runtime.ToString().ToLower() + ";\n");
                    sbStep.Append("currentSteps[" + stepID + "][\"conditions\"][" + i + "][4] = '" + tmpCriteria.Value + "';\n");
                    sbStep.Append("currentSteps[" + stepID + "][\"conditions\"][" + i + "][5] = '" + tmpCriteria.ValueTwo + "';\n");
                }
            }

            sbStep.Append("currentSteps[" + stepID + "][\"approverType\"] = " + approverType + ";\n");
            sbStep.Append("currentSteps[" + stepID + "][\"actionID\"] = " + actionID + ";\n");
            sbStep.Append("currentSteps[" + stepID + "][\"oneclicksignoff\"] = " + oneClickSignOff.ToString().ToLower() + ";\n");
            sbStep.Append("currentSteps[" + stepID + "][\"showdeclaration\"] = " + showDeclaration.ToString().ToLower() + ";\n");

            if (parentStepID > -1)
            {
                sbStep.Append("currentSteps[" + stepID + "][\"parentStepID\"] = " + reqWorkflow.Steps.IndexOfKey(parentStepID) + ";\n");
            }
            else
            {
                sbStep.Append("currentSteps[" + stepID + "][\"parentStepID\"] = " + parentStepID + ";\n");
            }
            if (relatedStepID.HasValue == true)
            {
                sbStep.Append("currentSteps[" + stepID + "][\"relatedStepID\"] = " + reqWorkflow.Steps.IndexOfKey(relatedStepID.Value) + ";\n");
            }
            else
            {
                sbStep.Append("currentSteps[" + stepID + "][\"relatedStepID\"] = null;\n");
            }

            sbStep.Append("currentSteps[" + stepID + "][\"divID\"] = \"step_" + stepID + "\";\n");

            if (approvalEmailTemplateID.HasValue)
            {
                sbStep.Append("currentSteps[" + stepID + "][\"approvalEmailTemplateID\"] = " + approvalEmailTemplateID.Value + ";\n");
            }
            else
            {
                sbStep.Append("currentSteps[" + stepID + "][\"approvalEmailTemplateID\"] = null;\n");
            }

            sbStep.Append("currentSteps[" + stepID + "][\"message\"] = '" + message.Replace("'", "\'") + "';\n");

            if (formID.HasValue == true)
            {
                sbStep.Append("currentSteps[" + stepID + "][\"formID\"] = " + formID.Value + ";\n");
            }
            else
            {
                sbStep.Append("currentSteps[" + stepID + "][\"formID\"] = '';\n");
            }

            sbStep.Append("\n\n");

            return sbStep.ToString();
        }
    }


}
