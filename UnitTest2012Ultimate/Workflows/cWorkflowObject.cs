using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace UnitTest2012Ultimate
{
    internal class cWorkflowObject
    {
        public static cWorkflow New(cWorkflow entity = null)
        {
            entity = (entity == null) ? Template() : entity;

            try
            {
                cWorkflows clsWorkflows = new cWorkflows(Moqs.CurrentUser());
                int entityID = clsWorkflows.SaveWorkFlow(entity.workflowid, entity.workflowtype, entity.workflowname, entity.description, entity.BaseTable.TableID, entity.canbechildworkflow, entity.runoncreation, entity.runonchange, entity.runondelete, Moqs.CurrentUser().EmployeeID, new object[,] { });
                clsWorkflows = new cWorkflows(Moqs.CurrentUser());
                entity = clsWorkflows.GetWorkflowByID(entityID);
            }
            catch (Exception e)
            {
                throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cWorkflowObject).ToString() + ">", e);
            }

            return entity;
        }

        public static bool Delete(int entityID)
        {
            if (entityID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cWorkflows clsWorkflows = new cWorkflows(currentUser);
                    clsWorkflows.DeleteWorkflowByID(entityID);

                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return false;
        }

        public static cWorkflow Template(int workflowID = 0, string workflowName = "UT WF <DateTime.UtcNow.Ticks>", string description = "A unit test workflow object", DateTime createdOn = new DateTime(), int createdBy = 21423, DateTime? modifiedOn = null, int? modifiedBy = null, WorkflowType workflowType = WorkflowType.CustomTable, bool canBeChildWorkflow = false, bool runOnCreation = false, bool runOnChange = false, bool runOnDelete = false, cTable baseTable = null)
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            workflowName = (workflowName == "UT WF <DateTime.UtcNow.Ticks>") ? "UT WF " + dt : workflowName;
            createdBy = (createdBy == 21423) ? GlobalTestVariables.EmployeeId : createdBy;
            createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;

            return new cWorkflow(
                workflowid: workflowID,
                workflowname: workflowName,
                description: description,
                createdon: createdOn,
                createdby: createdBy,
                modifiedon: modifiedOn,
                modifiedby: modifiedBy,
                workflowtype: workflowType,
                canbechildworkflow: canBeChildWorkflow,
                runoncreation: runOnCreation,
                runonchange: runOnChange,
                runondelete: runOnDelete,
                baseTable: baseTable
            );
        }
    }
}
