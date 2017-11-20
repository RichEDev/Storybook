using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spend_Management;
using SpendManagementLibrary;

namespace SpendManagementUnitTests.Global_Objects
{
    class cWorkflowObject
    {

        private static object[,] addWorkflowStep(object[,] steps, WorkFlowStepAction action, string description, string question, string trueAnswer, string falseAnswer, object[][] criteria, WorkflowEntityType approverType, int actionID, bool oneClickSignOff, bool showDeclaration, int? parentStepID, int? relatedStepID, int? approverEmailTemplateID, string message, int? formID)
        {

            object[,] newSteps = null;
            if (steps == null)
            {
                newSteps = new object[1, 14];
            }
            else
            {
                newSteps = new object[steps.GetLongLength(0) + 1, 14];
            }


            int i;
            int x;

            if (steps != null)
            {
                for (i = 0; i < steps.GetLongLength(0); i++)
                {
                    for (x = 0; x < steps.GetLongLength(1) - 2; x++)
                    {
                        newSteps[i, x] = steps[i, x];
                    }
                }
            }

            i = Convert.ToInt32(newSteps.GetLongLength(0)) - 1;
            newSteps[i, 0] = (int)action;
            newSteps[i, 1] = description;
            newSteps[i, 2] = question;
            newSteps[i, 3] = trueAnswer;
            newSteps[i, 4] = falseAnswer;
            newSteps[i, 5] = criteria;
            newSteps[i, 6] = approverType;
            newSteps[i, 7] = actionID;
            newSteps[i, 8] = oneClickSignOff;
            newSteps[i, 9] = showDeclaration;
            newSteps[i, 10] = parentStepID;
            newSteps[i, 11] = relatedStepID;
            newSteps[i, 13] = approverEmailTemplateID;
            newSteps[i, 14] = message;
            newSteps[i, 15] = formID;
            steps = newSteps;
            return newSteps;
        }

        /// <summary>
        /// Add a show message step
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="description"></param>
        /// <param name="parentStepID"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static object[,] addShowMessageStep(object[,] steps, string description, int? parentStepID, string message)
        {
            return addWorkflowStep(steps, WorkFlowStepAction.ShowMessage, description, string.Empty, string.Empty, string.Empty, null, WorkflowEntityType.None, 0, false, false, parentStepID, null, null, message, null);
        }

        /// <summary>
        /// Custom entity change form step
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="description"></param>
        /// <param name="parentStepID"></param>
        /// <param name="formID"></param>
        /// <returns></returns>
        public static object[,] addChangeFormStep(object[,] steps, string description, int? parentStepID, int formID)
        {
            return addWorkflowStep(steps, WorkFlowStepAction.ChangeCustomEntityForm, description, string.Empty, string.Empty, string.Empty, null, WorkflowEntityType.None, 0, false, false, null, null, null, string.Empty, formID);
        }

        /// <summary>
        /// Add actionID step
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="wfAction"></param>
        /// <param name="description"></param>
        /// <param name="parentStepID"></param>
        /// <param name="relatedStepID"></param>
        /// <param name="actionID"></param>
        /// <returns></returns>
        public static object[,] addActionIDStep(object[,] steps, WorkFlowStepAction wfAction, string description, int? parentStepID, int? relatedStepID, int actionID)
        {
            return addWorkflowStep(steps, wfAction, description, string.Empty, string.Empty, string.Empty, null, WorkflowEntityType.None, actionID, false, false, parentStepID, relatedStepID, null, string.Empty, null);
        }

        /// <summary>
        /// Adds a crteria step
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="wfAction"></param>
        /// <param name="description"></param>
        /// <param name="parentStepID"></param>
        /// <param name="relatedStepID"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static object[,] addCriteriaStep(object[,] steps, WorkFlowStepAction wfAction, string description, int? parentStepID, int? relatedStepID, object[][] criteria)
        {
            return addWorkflowStep(steps, wfAction, description, string.Empty, string.Empty, string.Empty, criteria, WorkflowEntityType.None, 0, false, false, parentStepID, relatedStepID, null, string.Empty, null);
        }

        /// <summary>
        /// Add decision step
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="description"></param>
        /// <param name="parentStepID"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static object[,] addDecisionStep(object[,] steps, string description, int? parentStepID, string question, string confirmButton, string cancelButton)
        {
            return addWorkflowStep(steps, WorkFlowStepAction.Decision, description, question, confirmButton, cancelButton, null, WorkflowEntityType.None, 0, false, false, parentStepID, null, null, string.Empty, null);
        }

        /// <summary>
        /// Add decision false step
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="description"></param>
        /// <param name="parentStepID"></param>
        /// <param name="relatedStepID"></param>
        /// <param name="question"></param>
        /// <param name="confirmButton"></param>
        /// <param name="cancelButton"></param>
        /// <returns></returns>
        public static object[,] addDecisionFalseStep(object[,] steps, string description, int? parentStepID, int relatedStepID, string question, string confirmButton, string cancelButton)
        {
            return addWorkflowStep(steps, WorkFlowStepAction.DecisionFalse, description, question, confirmButton, cancelButton, null, WorkflowEntityType.None, 0, false, false, parentStepID, relatedStepID, null, string.Empty, null);
        }

        /// <summary>
        /// Approval step
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="description"></param>
        /// <param name="parentStepID"></param>
        /// <param name="approverType"></param>
        /// <param name="approverID"></param>
        /// <param name="approvalEmailTemplateID"></param>
        /// <param name="oneClickSignOff"></param>
        /// <param name="showDeclaration"></param>
        /// <param name="question"></param>
        /// <param name="accept"></param>
        /// <param name="decline"></param>
        /// <returns></returns>
        public static object[,] addApprovalStep(object[,] steps, string description, int? parentStepID, WorkflowEntityType approverType, int? approverID, int? approvalEmailTemplateID, bool oneClickSignOff, bool showDeclaration, string question, string accept, string decline)
        {
            if (approverID.HasValue)
            {
                return addWorkflowStep(steps, WorkFlowStepAction.Approval, description, question, accept, decline, null, approverType, approverID.Value, oneClickSignOff, showDeclaration, parentStepID, null, approvalEmailTemplateID, string.Empty, null);
            }
            else
            {
                return addWorkflowStep(steps, WorkFlowStepAction.Approval, description, question, accept, decline, null, approverType, 0, oneClickSignOff, showDeclaration, parentStepID, null, approvalEmailTemplateID, string.Empty, null);
            }
        }

        public static object[,] addOtherWiseConditionStep(object[,] steps, string description, int? parentStepID, int relatedStepID)
        {
            return addWorkflowStep(steps, WorkFlowStepAction.ElseOtherwise, description, string.Empty, string.Empty, string.Empty, null, WorkflowEntityType.None, 0, false, false, parentStepID, relatedStepID, null, string.Empty, null);
        }

        //steps = cWorkflowObject.addWorkflowStep(steps, WorkFlowStepAction.Approval, "Approval", string.Empty, string.Empty, string.Empty, null, WorkflowEntityType.Employee, 517, false, false, null, null, 18, string.Empty, null);

        public static int CreateNewWorkflow()
        {
            cWorkflows clsWorkflows = new cWorkflows(new Moqs().CurrentUser());
            cTables clsTables = new cTables(cGlobalVariables.AccountID);
            cWorkflow tmpWorkflow = new cWorkflow(0, "Parking Permit " + DateTime.Now.ToString(), "Description", new DateTime(), cGlobalVariables.EmployeeID, null, null, WorkflowType.CustomTable, false, false, false, false, clsTables.GetTableByID(new Guid("DF886D56-4A64-406E-9A91-4F747D21C4A1")));

            object[,] steps = null;
            object[][] criteria;

            //CHECK CONDITION (disabled) (0)
            criteria = new object[][] { new object[] { "51182C0F-AD12-4FAF-8848-30BD2F9E5EC9", "0", false, false, "3", string.Empty } };
            cWorkflowObject.addCriteriaStep(steps, WorkFlowStepAction.CheckCondition, "Check if is disabled", null, null, criteria);
                //Show message - no permit required (1)
                cWorkflowObject.addShowMessageStep(steps, "Show Message", 0, "No parking permit required - please use the blue.");
            //ELSE (regular user) (2)
                criteria = new object[][] { new object[] { "51182C0F-AD12-4FAF-8848-30BD2F9E5EC9", "1", false, false, "3", string.Empty } };
                cWorkflowObject.addCriteriaStep(steps, WorkFlowStepAction.ElseCondition, "Check if regular user", null, 0, criteria);
                //CHECK CONDITION (is regular user on esr) (3)
                criteria = new object[][] { new object[] { "759bc1c3-a809-408c-8744-fa46c9c5fcb8", "1", false, false, "3", string.Empty } };
                cWorkflowObject.addCriteriaStep(steps, WorkFlowStepAction.CheckCondition, "Check if is regular user on esr", 2, null, criteria);
                    //Subworkflow Grant Permit (4)
                    cWorkflowObject.addActionIDStep(steps, WorkFlowStepAction.RunSubWorkflow, "Create PP", 3, null, 0);
                //OTHERWISE (5)
                cWorkflowObject.addOtherWiseConditionStep(steps, "Otherwise", 2, 3);
                    //Show message - decline - appeal process (6)
                    cWorkflowObject.addShowMessageStep(steps, "Show message", 5, "Request defined - this how you appeal");
            //ELSE (workbase) (7)
            criteria = new object[][] { new object[] { "51182C0F-AD12-4FAF-8848-30BD2F9E5EC9", "2", false, false, "3", string.Empty } };
            cWorkflowObject.addCriteriaStep(steps, WorkFlowStepAction.ElseCondition, "Check if is workbase", null, 0, criteria);
               //change custom entity form - allow post code entry/selection (8)
                cWorkflowObject.addChangeFormStep(steps, "Change to allow postcode entry", 7, 0);
                //CHECK CONDITION (postcode is in predefined list) (9)
                criteria = new object[][] { new object[] { "759bc1c3-a809-408c-8744-fa46c9c5fcb8", "1", false, false, "3", string.Empty } };
                cWorkflowObject.addCriteriaStep(steps, WorkFlowStepAction.CheckCondition, "postcode is in predefined list", 8, null, criteria);
                    //decision step – T&C (10)
                    cWorkflowObject.addDecisionStep(steps, "show terms and conditions", 9, "show t&c", "yes", "no");
                        //Subworkflow Grant Permit (11)
                        cWorkflowObject.addActionIDStep(steps, WorkFlowStepAction.RunSubWorkflow, "Create PP", 10, null, 0);
                    //decision false (12)
                    cWorkflowObject.addDecisionFalseStep(steps, "show terms and conditions - false", 9, 10, "show t&c", "yes", "no");
                        //Show message - permit request cancelled (13)
                        cWorkflowObject.addShowMessageStep(steps, "Show message", 12, "Permit request cancelled");
                //OTHERWISE (14)
                        cWorkflowObject.addOtherWiseConditionStep(steps, "Otherwise", 8, 9);
                    //Show message – no permit required for this trust (15)
                    cWorkflowObject.addShowMessageStep(steps, "Show Message", 14, "No parking permit required for this trust.");
            //ELSE (rotational) (16)
            criteria = new object[][] { new object[] { "51182C0F-AD12-4FAF-8848-30BD2F9E5EC9", "3", false, false, "3", string.Empty } };
            cWorkflowObject.addCriteriaStep(steps, WorkFlowStepAction.ElseCondition, "Check if is rotational", null, 0, criteria);
                //Show message - send for approval by line manager (17)
                cWorkflowObject.addShowMessageStep(steps, "Show message", null, "Permit request send to line manager for approval");
                //approval – line manager (18)
                cWorkflowObject.addApprovalStep(steps, "Approve permit request", 16, WorkflowEntityType.LineManager, null, null, false, false, string.Empty, string.Empty, string.Empty);
                    //decision step – T&C (19)
                    cWorkflowObject.addDecisionStep(steps, "show terms and conditions", 18, "show t&c", "yes", "no");
                        //Subworkflow Grant Permit (20)
                        cWorkflowObject.addActionIDStep(steps, WorkFlowStepAction.RunSubWorkflow, "Create PP", 19, null, 0);
                    //decision false (21)
                    cWorkflowObject.addDecisionFalseStep(steps, "show terms and conditions - false", 18, 19, "show t&c", "yes", "no");                    
                        //Show message - permit request cancelled (22)
                        cWorkflowObject.addShowMessageStep(steps, "Show message", 21, "Permit request cancelled");
                //approval false (23)

                    //Send email - permit request rejected (24)
                    cWorkflowObject.addActionIDStep(steps, WorkFlowStepAction.SendEmail, "Send email about rejected approval", 23, null, 0);
            //ELSE (regular staff) (25)
            criteria = new object[][] { new object[] { "51182C0F-AD12-4FAF-8848-30BD2F9E5EC9", "4", false, false, "3", string.Empty } };
            cWorkflowObject.addCriteriaStep(steps, WorkFlowStepAction.ElseCondition, "Check if is regular staff", null, 0, criteria);
                //CHECK CONDITION (is postcode in excluded list) (26)
                criteria = new object[][] { new object[] { "759bc1c3-a809-408c-8744-fa46c9c5fcb8", "1", false, false, "3", string.Empty } };
                cWorkflowObject.addCriteriaStep(steps, WorkFlowStepAction.CheckCondition, "Check if postcode is in excluded list", 25, null, criteria);
                    //Show message - decline - appeal process (27)
                    cWorkflowObject.addShowMessageStep(steps, "Show message", 26, "Request defined - this how you appeal");
                //OTHERWISE (28)
                    cWorkflowObject.addOtherWiseConditionStep(steps, "Otherwise", 25, 26);
                    //decision step – T&C (29)
                    cWorkflowObject.addDecisionStep(steps, "show terms and conditions", 28, "show t&c", "yes", "no");
                        //Subworkflow Grant Permit (30)
                        cWorkflowObject.addActionIDStep(steps, WorkFlowStepAction.RunSubWorkflow, "Create PP", 29, null, 0);
                    //decision false (31)
                    cWorkflowObject.addDecisionFalseStep(steps, "show terms and conditions - false", 28, 29, "show t&c", "yes", "no");
                        //Show message - permit request cancelled (32)
                        cWorkflowObject.addShowMessageStep(steps, "Show message", 31, "Permit request cancelled");
             



                        #region old tests

                        //// 0
            //steps = cWorkflowObject.addWorkflowStep(steps, WorkFlowStepAction.Approval, "Approval", string.Empty, string.Empty, string.Empty, null, WorkflowEntityType.Employee, 517, false, false, null, null, 18, string.Empty, null);

            //// 1
            //criteria = new object[][] { new object[] { "759bc1c3-a809-408c-8744-fa46c9c5fcb8", "1", false, false, "3", string.Empty } };
            //steps = cWorkflowObject.addWorkflowStep(steps, WorkFlowStepAction.ChangeValue, "Change Value", "", "", "", criteria, WorkflowEntityType.None, 0, false, false, null, null, null, string.Empty, null);

            //// 2
            //criteria = new object[][] { new object[] { "759bc1c3-a809-408c-8744-fa46c9c5fcb8", "1", false, false, "3", string.Empty } };
            //steps = cWorkflowObject.addWorkflowStep(steps, WorkFlowStepAction.CheckCondition, "Check Condition", string.Empty, string.Empty, string.Empty, criteria, WorkflowEntityType.None, 0, false, false, null, null, null, string.Empty, null);

            //// 3
            //criteria = new object[][] { new object[] { "759bc1c3-a809-408c-8744-fa46c9c5fcb8", "1", false, false, "2", string.Empty } };
            //steps = cWorkflowObject.addWorkflowStep(steps, WorkFlowStepAction.ElseCondition, "Else Condition", string.Empty, string.Empty, string.Empty, criteria, WorkflowEntityType.None, 0, false, false, null, 2, null, string.Empty, null);

            //// 4
            //steps = cWorkflowObject.addWorkflowStep(steps, WorkFlowStepAction.SendEmail, "Send Email", string.Empty, string.Empty, string.Empty, null, WorkflowEntityType.None, 16, false, false, 3, null, null, string.Empty, null);

            //// 5
            //criteria = new object[][] { new object[] { "759bc1c3-a809-408c-8744-fa46c9c5fcb8", "1", false, false, "2", string.Empty } };
            //steps = cWorkflowObject.addWorkflowStep(steps, WorkFlowStepAction.CheckCondition, "Check Condition Nested", string.Empty, string.Empty, string.Empty, criteria, WorkflowEntityType.None, 0, false, false, 3, null, null, string.Empty, null);
                

            //// 6
            //criteria = new object[][] { new object[] { "759bc1c3-a809-408c-8744-fa46c9c5fcb8", "1", false, false, "1", string.Empty } };
            //steps = cWorkflowObject.addWorkflowStep(steps, WorkFlowStepAction.ElseOtherwise, "Otherwise Condition", string.Empty, string.Empty, string.Empty, criteria, WorkflowEntityType.None, 0, false, false, null, 2, null, string.Empty, null);

            //// 7
            //steps = cWorkflowObject.addWorkflowStep(steps, WorkFlowStepAction.Decision, "Desc", "Q", "1", "0", null, WorkflowEntityType.None, 0, false, false, null, null, null, string.Empty, null);

            //// 8
            //steps = cWorkflowObject.addWorkflowStep(steps, WorkFlowStepAction.SendEmail, "Send Email", string.Empty, string.Empty, string.Empty, null, WorkflowEntityType.None, 16, false, false, 7, null, null, string.Empty, null);

            //// 9
            //steps = cWorkflowObject.addWorkflowStep(steps, WorkFlowStepAction.DecisionFalse, "Desc", string.Empty, string.Empty, string.Empty, null, WorkflowEntityType.None, 0, false, false, null, 7, null, string.Empty, null);

            //// 10 
            //steps = cWorkflowObject.addWorkflowStep(steps, WorkFlowStepAction.MoveToStep, "Move to step", string.Empty, string.Empty, string.Empty, null, WorkflowEntityType.None, 1, false, false, 9, null, null, string.Empty, null);

            //// 11
            //steps = cWorkflowObject.addWorkflowStep(steps, WorkFlowStepAction.SendEmail, "Send Email", string.Empty, string.Empty, string.Empty, null, WorkflowEntityType.None, 16, false, false, null, null, null, string.Empty, null);

            //// 12
            //criteria = new object[][] { new object[] { "ea8773a0-c19f-4424-b768-8b3d3cca1a68", "1", false, false, "UNIT TEST \"[78D1F684-0DF4-4836-9AD6-FAB4A142AD77]\"-\"[639E17EA-C85F-4AF0-A58D-7C022ECA0FBE]\"", string.Empty } };
            //steps = cWorkflowObject.addWorkflowStep(steps, WorkFlowStepAction.CreateDynamicValue, "Create Dynamic Value", string.Empty, string.Empty, string.Empty, criteria, (WorkflowEntityType)0, 1, false, false, null, null, null, string.Empty, null);

            ////RunSubWorkflow = 6,

            //// 13
                        //steps = cWorkflowObject.addWorkflowStep(steps, WorkFlowStepAction.FinishWorkflow, "Finish Workflow", string.Empty, string.Empty, string.Empty, null, (WorkflowEntityType)0, 0, false, false, null, null, null, string.Empty, null);
                        #endregion
                        int workflowID = clsWorkflows.SaveWorkFlow(tmpWorkflow.workflowid, tmpWorkflow.workflowtype, tmpWorkflow.workflowname, tmpWorkflow.description, tmpWorkflow.BaseTable.TableID, tmpWorkflow.canbechildworkflow, tmpWorkflow.runoncreation, tmpWorkflow.runonchange, tmpWorkflow.runondelete, cGlobalVariables.EmployeeID, steps);
            return workflowID;
        }
    }
}
