using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System;
using System.Collections.Generic;
using SpendManagementLibrary;
using SpendManagementUnitTests.Global_Objects;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cTasksTest and is intended
    ///to contain all cTasksTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cTasksTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            cTaskObject.DeleteTask();

            System.Web.HttpContext.Current.Session["myid"] = null;
            cEmployeeObject.DeleteDelegateUTEmployee();
        }
        
        #endregion


        /// <summary>
        ///A test for UserHasTasks is true
        ///</summary>
        [TestMethod()]
        public void UserHasTasksIsTrueTest()
        {
            cTask task = cTaskObject.CreateTaskWithAllValuesSet();
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            bool actual = target.UserHasTasks(cGlobalVariables.EmployeeID);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        ///A test for UserHasTasks is false
        ///</summary>
        [TestMethod()]
        public void UserHasTasksIsFalseTest()
        {
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            bool actual = target.UserHasTasks(cGlobalVariables.EmployeeID);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for taskExists with the ignore closed tasks set to true
        ///</summary>
        [TestMethod()]
        public void TaskExistsWithIgnoreTrueTest()
        {
            cTask task = cTaskObject.CreateTaskWithAllValuesSet();
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            bool actual = target.taskExists(task.RegardingId, task.RegardingArea, task.TaskCommandType, task.TaskOwner.OwnerId, task.TaskOwner.OwnerType, true);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        ///A test for taskExists with the ignore closed tasks set to false
        ///</summary>
        [TestMethod()]
        public void TaskExistsWithIgnoreFalseTest()
        {
            cTask task = cTaskObject.CreateTaskWithAllValuesSet();
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            bool actual = target.taskExists(task.RegardingId, task.RegardingArea, task.TaskCommandType, task.TaskOwner.OwnerId, task.TaskOwner.OwnerType, false);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        ///A test for setTasksToComplete
        ///</summary>
        [TestMethod()]
        public void SetTasksToCompleteTest()
        {
            cTaskObject.CreateTaskWithAllValuesSet();
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            List<int> lstTasks = new List<int>();
            lstTasks.Add(cGlobalVariables.TaskID);
            target.setTasksToComplete(lstTasks, cGlobalVariables.EmployeeID);
            cTask actual = target.GetTaskById(cGlobalVariables.TaskID);
            Assert.IsNotNull(actual);
            Assert.AreEqual(TaskStatus.Completed, actual.StatusId);           
        }
       
        /// <summary>
        ///A test for GetTaskSummary
        ///</summary>
        [TestMethod()]
        public void GetTaskSummaryTest()
        {
            cTask expected = cTaskObject.CreateTaskWithAllValuesSet();
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            Dictionary<int, cTask> actual = target.GetTaskSummary(AppliesTo.Employee, cGlobalVariables.EmployeeID, TaskStatus.InProgress);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);
            cCompareAssert.AreEqual(expected, actual[cGlobalVariables.TaskID], cTaskObject.lstOmittedProperties);
        }

        /// <summary>
        ///A test for GetTasksForUserId
        ///</summary>
        [TestMethod()]
        public void GetTasksForUserIdTest()
        {
            cTask expected = cTaskObject.CreateTaskWithAllValuesSet();
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            Dictionary<int, cTask> actual = target.GetTasksForUserId(cGlobalVariables.EmployeeID);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);
            cCompareAssert.AreEqual(expected, actual[cGlobalVariables.TaskID], cTaskObject.lstOmittedProperties);
        }

        /// <summary>
        ///A test for GetTasksByStatus
        ///</summary>
        [TestMethod()]
        public void GetTasksByStatusTest()
        {
            cTask expected = cTaskObject.CreateTaskWithAllValuesSet();
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            Dictionary<int, cTask> actual = target.GetTasksByStatus(TaskStatus.InProgress, cGlobalVariables.EmployeeID);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);
            cCompareAssert.AreEqual(expected, actual[cGlobalVariables.TaskID], cTaskObject.lstOmittedProperties);
        }

        /// <summary>
        ///A test for GetTasksByStatus for all users
        ///</summary>
        [TestMethod()]
        public void GetTasksByStatusForAllUsersTest()
        {
            cTask expected = cTaskObject.CreateTaskWithAllValuesSet();
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            Dictionary<int, cTask> actual = target.GetTasksByStatus(TaskStatus.InProgress, -1);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);
            cCompareAssert.AreEqual(expected, actual[cGlobalVariables.TaskID], cTaskObject.lstOmittedProperties);
        }

        /// <summary>
        ///A test to get a task by a valid ID
        ///</summary>
        [TestMethod()]
        public void GetTaskByValidIdTest()
        {
            cTask expected = cTaskObject.CreateTaskWithAllValuesSet();
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
          
            cTask actual = target.GetTaskById(cGlobalVariables.TaskID);
            Assert.IsNotNull(actual);
            
            cCompareAssert.AreEqual(expected, actual, cTaskObject.lstOmittedProperties);
        }

        /// <summary>
        ///A test to get a task by an invalid ID
        ///</summary>
        [TestMethod()]
        public void GetTaskByInvalidIdTest()
        {
            cTask expected = cTaskObject.CreateTaskWithAllValuesSet();
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            cTask actual = target.GetTaskById(0);
            Assert.IsNull(actual);
        }

        /// <summary>
        ///A test for GetTaskByCreatorId
        ///</summary>
        [TestMethod()]
        public void GetTaskByCreatorIdTest()
        {
            cTask expected = cTaskObject.CreateTaskWithAllValuesSet();
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            Dictionary<int, cTask> actual = target.GetTaskByCreatorId(cGlobalVariables.EmployeeID);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);
            cCompareAssert.AreEqual(expected, actual[cGlobalVariables.TaskID], cTaskObject.lstOmittedProperties);
        }

        /// <summary>
        ///A test for deleting a task with a valid ID
        ///</summary>
        [TestMethod()]
        public void DeleteTaskWithValidIDTest()
        {
            cTaskObject.CreateTaskWithAllValuesSet();
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            
            target.DeleteTask(cGlobalVariables.TaskID, cGlobalVariables.EmployeeID);
            Assert.IsNull(target.GetTaskById(cGlobalVariables.TaskID));
        }

        /// <summary>
        ///A test for deleting a task with an invalid ID
        ///</summary>
        [TestMethod()]
        public void DeleteTaskWithAnInvalidIDTest()
        {
            cTaskObject.CreateTaskWithAllValuesSet();
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            
            target.DeleteTask(0, cGlobalVariables.EmployeeID);
            Assert.IsNotNull(target.GetTaskById(cGlobalVariables.TaskID));
        }

        /// <summary>
        ///A test for deleting a task with a valid ID as a delegate
        ///</summary>
        [TestMethod()]
        public void DeleteTaskWithValidIDAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            cTaskObject.CreateTaskWithAllValuesSet();
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            target.DeleteTask(cGlobalVariables.TaskID, cGlobalVariables.EmployeeID);
            Assert.IsNull(target.GetTaskById(cGlobalVariables.TaskID));
        }


        /// <summary>
        ///A test for caching tasks with all values set
        ///</summary>
        [TestMethod()]
        public void CacheTasksWithAllValuesSetTest()
        {
            int AccountID = cGlobalVariables.AccountID;
            System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
            Cache.Remove("tasks_" + AccountID.ToString() + "_" + cGlobalVariables.SubAccountID.ToString());

            cTaskObject.CreateTaskWithAllValuesSet();

            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            
            SortedList<int, cTask> expected = (SortedList<int, cTask>)Cache["tasks_" + AccountID.ToString() + "_" + cGlobalVariables.SubAccountID.ToString()];

            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.Count > 0);
            Cache.Remove("tasks_" + AccountID.ToString() + "_" + cGlobalVariables.SubAccountID.ToString());
        }

        // <summary>
        ///A test for caching tasks with all values set to null or nothing that can be
        ///</summary>
        [TestMethod()]
        public void CacheTasksWithAllValuesSetToNullOrNothingTest()
        {
            int AccountID = cGlobalVariables.AccountID;
            System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
            Cache.Remove("tasks_" + AccountID.ToString() + "_" + cGlobalVariables.SubAccountID.ToString());

            cTaskObject.CreateTaskWithValuesSetToNullOrNothing();

            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            SortedList<int, cTask> expected = (SortedList<int, cTask>)Cache["tasks_" + AccountID.ToString() + "_" + cGlobalVariables.SubAccountID.ToString()];

            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.Count > 0);
            Cache.Remove("tasks_" + AccountID.ToString() + "_" + cGlobalVariables.SubAccountID.ToString());
        }

        /// <summary>
        ///A test for adding a task with all values set
        ///</summary>
        [TestMethod()]
        public void AddTaskWithAllValuesSetTest()
        {
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID); // TODO: Initialize to an appropriate value
            cTask newTask = new cTask(0, cGlobalVariables.SubAccountID, SpendManagementLibrary.TaskCommand.ESR_RecordActivateOn, cGlobalVariables.EmployeeID, DateTime.Now, null, cGlobalVariables.EmployeeID, SpendManagementLibrary.AppliesTo.Employee, "Unit Test Task", "Task for unit test", DateTime.Now.AddDays(-5), DateTime.Now, DateTime.Now.AddDays(2), SpendManagementLibrary.TaskStatus.InProgress, new cTaskOwner(cGlobalVariables.EmployeeID, sendType.employee, null), true, DateTime.Now);

            
            int ID = target.AddTask(newTask, cGlobalVariables.EmployeeID);

            Assert.IsTrue(ID > 0);

            cGlobalVariables.TaskID = ID;

            cTask actual = target.GetTaskById(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(newTask.TaskCommandType, actual.TaskCommandType);
            Assert.AreEqual(newTask.TaskCreator, actual.TaskCreator);
            Assert.AreEqual(newTask.TaskCreatedDate, actual.TaskCreatedDate);
            Assert.AreEqual(newTask.TaskType, actual.TaskType);
            Assert.AreEqual(newTask.RegardingId, actual.RegardingId);
            Assert.AreEqual(newTask.RegardingArea, actual.RegardingArea);
            Assert.AreEqual(newTask.Subject, actual.Subject);
            Assert.AreEqual(newTask.Description, actual.Description);
            Assert.AreEqual(newTask.StartDate, actual.StartDate);
            Assert.AreEqual(newTask.DueDate, actual.DueDate);
            Assert.AreEqual(newTask.EndDate, actual.EndDate);
            Assert.AreEqual(newTask.StatusId, actual.StatusId);
            Assert.AreEqual(newTask.TaskOwner, actual.TaskOwner);
            Assert.AreEqual(newTask.TaskEscalated, actual.TaskEscalated);
            Assert.AreEqual(newTask.TaskEscalatedDate, actual.TaskEscalatedDate);
        }

        /// <summary>
        ///A test for adding a task with all values set to null or nothing that can be
        ///</summary>
        [TestMethod()]
        public void AddTaskWithValuesSetToNullOrNothingThatCanBeTest()
        {
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID); // TODO: Initialize to an appropriate value
            cTask newTask = new cTask(0, cGlobalVariables.SubAccountID, SpendManagementLibrary.TaskCommand.ESR_RecordActivateOn, cGlobalVariables.EmployeeID, DateTime.Now, null, cGlobalVariables.EmployeeID, SpendManagementLibrary.AppliesTo.Employee, "", "", null, null, null, SpendManagementLibrary.TaskStatus.InProgress, new cTaskOwner(cGlobalVariables.EmployeeID, sendType.employee, null), false, null);

            int ID = target.AddTask(newTask, cGlobalVariables.EmployeeID);

            Assert.IsTrue(ID > 0);

            cGlobalVariables.TaskID = ID;

            cTask actual = target.GetTaskById(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(newTask.TaskCommandType, actual.TaskCommandType);
            Assert.AreEqual(newTask.TaskCreator, actual.TaskCreator);
            Assert.AreEqual(newTask.TaskCreatedDate, actual.TaskCreatedDate);
            Assert.AreEqual(newTask.TaskType, actual.TaskType);
            Assert.AreEqual(newTask.RegardingId, actual.RegardingId);
            Assert.AreEqual(newTask.RegardingArea, actual.RegardingArea);
            Assert.AreEqual(newTask.Subject, actual.Subject);
            Assert.AreEqual(newTask.Description, actual.Description);
            Assert.AreEqual(newTask.StartDate, actual.StartDate);
            Assert.AreEqual(newTask.DueDate, actual.DueDate);
            Assert.AreEqual(newTask.EndDate, actual.EndDate);
            Assert.AreEqual(newTask.StatusId, actual.StatusId);
            Assert.AreEqual(newTask.TaskOwner, actual.TaskOwner);
            Assert.AreEqual(newTask.TaskEscalated, actual.TaskEscalated);
            Assert.AreEqual(newTask.TaskEscalatedDate, actual.TaskEscalatedDate);
        }

        /// <summary>
        ///A test for updating a task with all values set
        ///</summary>
        [TestMethod()]
        public void EditTaskWithAllValuesSetTest()
        {
            cTask newTask = cTaskObject.CreateTaskWithAllValuesSet();
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            target.UpdateTask(new cTask(newTask.TaskId, newTask.SubAccountID, newTask.TaskCommandType, newTask.TaskCreator, newTask.TaskCreatedDate, newTask.TaskType, newTask.RegardingId, newTask.RegardingArea, "Unit Test Task Updated", "Task for unit test updated", newTask.StartDate, newTask.DueDate, newTask.EndDate, newTask.StatusId, newTask.TaskOwner, newTask.TaskEscalated, newTask.TaskEscalatedDate), cGlobalVariables.EmployeeID);

            cTask actual = target.GetTaskById(cGlobalVariables.TaskID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(newTask.TaskCommandType, actual.TaskCommandType);
            Assert.AreEqual(newTask.TaskCreator, actual.TaskCreator);
            Assert.AreEqual(newTask.TaskCreatedDate, actual.TaskCreatedDate);
            Assert.AreEqual(newTask.TaskType, actual.TaskType);
            Assert.AreEqual(newTask.RegardingId, actual.RegardingId);
            Assert.AreEqual(newTask.RegardingArea, actual.RegardingArea);
            Assert.AreEqual("Unit Test Task Updated", actual.Subject);
            Assert.AreEqual("Task for unit test updated", actual.Description);
            Assert.AreEqual(newTask.StartDate, actual.StartDate);
            Assert.AreEqual(newTask.DueDate, actual.DueDate);
            Assert.AreEqual(newTask.EndDate, actual.EndDate);
            Assert.AreEqual(newTask.StatusId, actual.StatusId);
            Assert.AreEqual(newTask.TaskOwner, actual.TaskOwner);
            Assert.AreEqual(newTask.TaskEscalated, actual.TaskEscalated);
            Assert.AreEqual(newTask.TaskEscalatedDate, actual.TaskEscalatedDate);
        }

        /// <summary>
        ///A test for updating a task with all values set to null or nothing that can be
        ///</summary>
        [TestMethod()]
        public void EditTaskWithValuesSetToNullOrNothingThatCanBeTest()
        {
            cTask newTask = cTaskObject.CreateTaskWithValuesSetToNullOrNothing();
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            target.UpdateTask(new cTask(newTask.TaskId, newTask.SubAccountID, newTask.TaskCommandType, newTask.TaskCreator, newTask.TaskCreatedDate, newTask.TaskType, newTask.RegardingId, newTask.RegardingArea, "Unit Test Task Updated", "Task for unit test updated", newTask.StartDate, newTask.DueDate, newTask.EndDate, newTask.StatusId, newTask.TaskOwner, newTask.TaskEscalated, newTask.TaskEscalatedDate), cGlobalVariables.EmployeeID);

            cTask actual = target.GetTaskById(cGlobalVariables.TaskID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(newTask.TaskCommandType, actual.TaskCommandType);
            Assert.AreEqual(newTask.TaskCreator, actual.TaskCreator);
            Assert.AreEqual(newTask.TaskCreatedDate, actual.TaskCreatedDate);
            Assert.AreEqual(newTask.TaskType, actual.TaskType);
            Assert.AreEqual(newTask.RegardingId, actual.RegardingId);
            Assert.AreEqual(newTask.RegardingArea, actual.RegardingArea);
            Assert.AreEqual("Unit Test Task Updated", actual.Subject);
            Assert.AreEqual("Task for unit test updated", actual.Description);
            Assert.AreEqual(newTask.StartDate, actual.StartDate);
            Assert.AreEqual(newTask.DueDate, actual.DueDate);
            Assert.AreEqual(newTask.EndDate, actual.EndDate);
            Assert.AreEqual(newTask.StatusId, actual.StatusId);
            Assert.AreEqual(newTask.TaskOwner, actual.TaskOwner);
            Assert.AreEqual(newTask.TaskEscalated, actual.TaskEscalated);
            Assert.AreEqual(newTask.TaskEscalatedDate, actual.TaskEscalatedDate);
        }

        /// <summary>
        ///A test for adding a task with all values set as a delegate
        ///</summary>
        [TestMethod()]
        public void AddTaskWithAllValuesSetAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cTask newTask = new cTask(0, cGlobalVariables.SubAccountID, SpendManagementLibrary.TaskCommand.ESR_RecordActivateOn, cGlobalVariables.EmployeeID, DateTime.Now, null, cGlobalVariables.EmployeeID, SpendManagementLibrary.AppliesTo.Employee, "Unit Test Task", "Task for unit test", DateTime.Now.AddDays(-5), DateTime.Now, DateTime.Now.AddDays(2), SpendManagementLibrary.TaskStatus.InProgress, new cTaskOwner(cGlobalVariables.EmployeeID, sendType.employee, null), true, DateTime.Now);


            int ID = target.AddTask(newTask, cGlobalVariables.EmployeeID);

            Assert.IsTrue(ID > 0);

            cGlobalVariables.TaskID = ID;

            cTask actual = target.GetTaskById(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(newTask.TaskCommandType, actual.TaskCommandType);
            Assert.AreEqual(newTask.TaskCreator, actual.TaskCreator);
            Assert.AreEqual(newTask.TaskCreatedDate, actual.TaskCreatedDate);
            Assert.AreEqual(newTask.TaskType, actual.TaskType);
            Assert.AreEqual(newTask.RegardingId, actual.RegardingId);
            Assert.AreEqual(newTask.RegardingArea, actual.RegardingArea);
            Assert.AreEqual(newTask.Subject, actual.Subject);
            Assert.AreEqual(newTask.Description, actual.Description);
            Assert.AreEqual(newTask.StartDate, actual.StartDate);
            Assert.AreEqual(newTask.DueDate, actual.DueDate);
            Assert.AreEqual(newTask.EndDate, actual.EndDate);
            Assert.AreEqual(newTask.StatusId, actual.StatusId);
            Assert.AreEqual(newTask.TaskOwner, actual.TaskOwner);
            Assert.AreEqual(newTask.TaskEscalated, actual.TaskEscalated);
            Assert.AreEqual(newTask.TaskEscalatedDate, actual.TaskEscalatedDate);
        }

        /// <summary>
        ///A test for updating a task with all values set as a delegate
        ///</summary>
        [TestMethod()]
        public void EditTaskWithAllValuesSetAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            cTask newTask = cTaskObject.CreateTaskWithAllValuesSet();
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            target.UpdateTask(new cTask(newTask.TaskId, newTask.SubAccountID, newTask.TaskCommandType, newTask.TaskCreator, newTask.TaskCreatedDate, newTask.TaskType, newTask.RegardingId, newTask.RegardingArea, "Unit Test Task Updated", "Task for unit test updated", newTask.StartDate, newTask.DueDate, newTask.EndDate, newTask.StatusId, newTask.TaskOwner, newTask.TaskEscalated, newTask.TaskEscalatedDate), cGlobalVariables.EmployeeID);

            cTask actual = target.GetTaskById(cGlobalVariables.TaskID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(newTask.TaskCommandType, actual.TaskCommandType);
            Assert.AreEqual(newTask.TaskCreator, actual.TaskCreator);
            Assert.AreEqual(newTask.TaskCreatedDate, actual.TaskCreatedDate);
            Assert.AreEqual(newTask.TaskType, actual.TaskType);
            Assert.AreEqual(newTask.RegardingId, actual.RegardingId);
            Assert.AreEqual(newTask.RegardingArea, actual.RegardingArea);
            Assert.AreEqual("Unit Test Task Updated", actual.Subject);
            Assert.AreEqual("Task for unit test updated", actual.Description);
            Assert.AreEqual(newTask.StartDate, actual.StartDate);
            Assert.AreEqual(newTask.DueDate, actual.DueDate);
            Assert.AreEqual(newTask.EndDate, actual.EndDate);
            Assert.AreEqual(newTask.StatusId, actual.StatusId);
            Assert.AreEqual(newTask.TaskOwner, actual.TaskOwner);
            Assert.AreEqual(newTask.TaskEscalated, actual.TaskEscalated);
            Assert.AreEqual(newTask.TaskEscalatedDate, actual.TaskEscalatedDate);
        }

        /// <summary>
        ///A test for cTasks Constructor
        ///</summary>
        [TestMethod()]
        public void cTasksConstructorTest()
        {
            cTasks target = new cTasks(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            Assert.IsNotNull(target);
        }
    }
}
