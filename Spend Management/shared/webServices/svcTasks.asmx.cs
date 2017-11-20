using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Data;
using SpendManagementLibrary;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcTasks
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcTasks : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<int> CompleteTasks(List<int> completedTaskIDs)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cTasks clsTasks = new cTasks(currentUser.AccountID, currentUser.CurrentSubAccountId);
            clsTasks.setTasksToComplete(completedTaskIDs, currentUser.EmployeeID);
            return completedTaskIDs;
        }

        /// <summary>
        /// Obtains a grid of tasks awaiting completion for a particular employee
        /// </summary>
        /// <param name="employeeid">Employee ID to obtain tasks for</param>
        /// <returns>HTML grid</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] getTasksToCompleteGrid()
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            

            cAccountSubAccounts subaccs = new cAccountSubAccounts(currentUser.AccountID);
            cAccountProperties properties = subaccs.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;

            cTables tables = new cTables(currentUser.AccountID);
            cFields clsfields = new cFields(currentUser.AccountID);
            cTable basetable = tables.GetTableByName("employeeTasks");
            
            
            
            bool canEditTasks = currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Tasks, true);

            
            
            cFields fields = new cFields(currentUser.AccountID);

            List<cNewGridColumn> columns = new List<cNewGridColumn>
                                           {
                                               new cFieldColumn(fields.GetBy(basetable.TableID, "taskId")),
                                               new cFieldColumn(fields.GetBy(basetable.TableID, "regardingArea")),
                                               new cFieldColumn(fields.GetBy(basetable.TableID, "subject")),
                                               new cFieldColumn(fields.GetBy(basetable.TableID, "description")),
                                               new cFieldColumn(fields.GetBy(basetable.TableID, "regardingId")),
                                               new cFieldColumn(fields.GetBy(basetable.TableID, "dueDate"))
                                           };


            cGridNew tasksGrid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "taskstocompletegrid", basetable, columns)
                                 {
                                     EmptyText = "There are currently no tasks awaiting completion",
                                     EnableSelect = canEditTasks,
                                     KeyField = "taskId",
                                     EnableSorting = false
                                 };

            tasksGrid.getColumnByName("taskId").hidden = true;
            tasksGrid.getColumnByName("regardingId").hidden = true;

            tasksGrid.WhereClause = "(((taskOwnerType = 1 and teamemployeeid = @teamemployeeid) or (taskOwnerType = 3 and taskOwnerid = @taskownerid)) or taskCreatorId = @taskCreatorId) and (statusId <> " + (int)TaskStatus.Cancelled + " and statusId <> " + (int)TaskStatus.Completed + ")";
            
            tasksGrid.addFilter(clsfields.GetBy(basetable.TableID, "teamemployeeid"), "@teamemployeeid", currentUser.EmployeeID);
            tasksGrid.addFilter(clsfields.GetBy(basetable.TableID, "taskOwnerId"), "@taskownerid",  currentUser.EmployeeID);
            tasksGrid.addFilter(clsfields.GetBy(basetable.TableID, "taskCreatorId"), "taskCreatorId", currentUser.EmployeeID);
            

            
            ((cFieldColumn)tasksGrid.getColumnByName("regardingArea")).addValueListItem((int)AppliesTo.CONTRACT_DETAILS, "Contract");
            ((cFieldColumn)tasksGrid.getColumnByName("regardingArea")).addValueListItem((int)AppliesTo.CONTRACT_GROUPING, "Contract");
            ((cFieldColumn)tasksGrid.getColumnByName("regardingArea")).addValueListItem((int)AppliesTo.CONPROD_GROUPING, "Contract Product");
            ((cFieldColumn)tasksGrid.getColumnByName("regardingArea")).addValueListItem((int)AppliesTo.CONTRACT_PRODUCTS, "Contract Product");
            ((cFieldColumn)tasksGrid.getColumnByName("regardingArea")).addValueListItem((int)AppliesTo.PRODUCT_DETAILS, "Product");
            ((cFieldColumn)tasksGrid.getColumnByName("regardingArea")).addValueListItem((int)AppliesTo.STAFF_DETAILS, "Employee");
            ((cFieldColumn)tasksGrid.getColumnByName("regardingArea")).addValueListItem((int)AppliesTo.Employee, "Employee");
            ((cFieldColumn)tasksGrid.getColumnByName("regardingArea")).addValueListItem((int)AppliesTo.VENDOR_DETAILS, properties.SupplierPrimaryTitle);
            ((cFieldColumn)tasksGrid.getColumnByName("regardingArea")).addValueListItem((int)AppliesTo.RECHARGE_GROUPING, "Recharge");
            ((cFieldColumn)tasksGrid.getColumnByName("regardingArea")).addValueListItem((int)AppliesTo.INVOICE_DETAILS, "Invoice Details");
            ((cFieldColumn)tasksGrid.getColumnByName("regardingArea")).addValueListItem((int)AppliesTo.INVOICE_FORECASTS, "Invoice Forecast");
            ((cFieldColumn)tasksGrid.getColumnByName("regardingArea")).addValueListItem((int)AppliesTo.Car, "Car");


            if (currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Tasks, true))
            {
                tasksGrid.addEventColumn("viewtask", "/shared/images/icons/edit.png", "javascript:goToViewTask({taskId},{regardingId},{regardingArea});", "Task details", "Go to view task details");
            }

            tasksGrid.addEventColumn("viewrecord", "/shared/images/icons/16/plain/zoom_in.png", "javascript:getTaskRegardingRecord({regardingArea},{regardingId});", "View", "Go to record");
            List<string> retVals = new List<string>
                                   {
                                       tasksGrid.GridID
                                   };
            retVals.AddRange(tasksGrid.generateGrid());

            return retVals.ToArray();
        }

    }


}

