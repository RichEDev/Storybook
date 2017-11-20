using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spend_Management;
using SpendManagementLibrary;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cTaskObject
    {
        /// <summary>
        /// Create a global static task variable wuth all property values set
        /// </summary>
        /// <returns></returns>
        public static cTask CreateTaskWithAllValuesSet()
        {
            cTasks clsTasks = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            int tempTaskID = clsTasks.AddTask(new cTask(0, cGlobalVariables.SubAccountID, SpendManagementLibrary.TaskCommand.ESR_RecordActivateOn, cGlobalVariables.EmployeeID, DateTime.UtcNow, null, cGlobalVariables.EmployeeID, SpendManagementLibrary.AppliesTo.Employee, "Unit Test Task", "Task for unit test", DateTime.Now.AddDays(-5), DateTime.Now, DateTime.Now.AddDays(2), SpendManagementLibrary.TaskStatus.InProgress, new cTaskOwner(cGlobalVariables.EmployeeID, sendType.employee, null), true, DateTime.Now), cGlobalVariables.EmployeeID);

            cGlobalVariables.TaskID = tempTaskID;

            cTask task = clsTasks.GetTaskById(tempTaskID);
            return task;
        }

        /// <summary>
        /// Create a global static task variable wuth all property values set to null or nothing that can be
        /// </summary>
        /// <returns></returns>
        public static cTask CreateTaskWithValuesSetToNullOrNothing()
        {
            cTasks clsTasks = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            int tempTaskID = clsTasks.AddTask(new cTask(0, cGlobalVariables.SubAccountID, SpendManagementLibrary.TaskCommand.ESR_RecordActivateOn, cGlobalVariables.EmployeeID, DateTime.Now, null, cGlobalVariables.EmployeeID, SpendManagementLibrary.AppliesTo.Employee, "", "", null, null, null, SpendManagementLibrary.TaskStatus.InProgress, new cTaskOwner(cGlobalVariables.EmployeeID, sendType.employee, null), false, null), cGlobalVariables.EmployeeID);

            cGlobalVariables.TaskID = tempTaskID;

            cTask task = clsTasks.GetTaskById(tempTaskID);
            return task;
        }


        /// <summary>
        /// Delete the global task from the database
        /// </summary>
        public static void DeleteTask()
        {
            cTasks clsTasks = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            clsTasks.DeleteTask(cGlobalVariables.TaskID, cGlobalVariables.EmployeeID);
        }

        public static readonly List<string> lstOmittedProperties;

        static cTaskObject()
        {
            lstOmittedProperties = new List<string>();
            lstOmittedProperties.Add("TaskCreatedDate");
        }
    }
}
