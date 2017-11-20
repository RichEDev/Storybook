using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Spend_Management;
using SpendManagementLibrary;

namespace Expenses_Scheduler
{
    using System.Text;

    using SpendManagementLibrary.Employees;

    class cESRRoutines
    {
        private cAccount curAccount;
        private string sConnectionString;

        public cESRRoutines(cAccount account)
        {
            curAccount = account;
            sConnectionString = cAccounts.getConnectionString(curAccount.accountid);
        }

        /// <summary>
        /// Check for any employees that need to be archived. If the archive type is set to manual then a task is created for a user to action. 
        /// If the archive type is automatic then the archive is automatically processed 
        /// </summary>
        /// <returns></returns>
        public bool ArchiveCheck()
        {
            DBConnection db = new DBConnection(sConnectionString);

            cAccountSubAccounts subaccs = new cAccountSubAccounts(curAccount.accountid);
            //if (subaccs.getSubAccountsCollection().Count > 1)
            //{
            //throw new NotImplementedException("The support for multiple subaccounts has not been fully implemented in the scheduler");
            //}

            cAccountSubAccount subaccount = subaccs.getFirstSubAccount(); // only got a single subaccount until fully implemented
            int archivedCount = 0;
            int archiveFailCount = 0;

            if (subaccount.SubAccountProperties.AutoArchiveType != AutoArchiveType.DoNotArchive)
            {
                int gracePeriod = subaccount.SubAccountProperties.ArchiveGracePeriod;

                cEmployees emps = new cEmployees(curAccount.accountid);
                cClaims clsclaims = new cClaims(curAccount.accountid);
                cEmailNotifications notifications = new cEmailNotifications(curAccount.accountid);
                cTasks tasks = new cTasks(curAccount.accountid, subaccount.SubAccountID);

                foreach (KeyValuePair<int, DateTime?> kvp in emps.getPendingArchives(gracePeriod))
                {
                    bool archiveOK = true;
                    string taskBody = "";

                    #region Employee Checks before archiving

                    // if employee is a line manager, are there any outstanding claims to approve
                    if (emps.isCheckAndPayer(curAccount.accountid, kvp.Key))
                    {
                        const string sql = "select count([claimid]) from dbo.checkandpay where checkerid = @empId";
                        db.sqlexecute.Parameters.Clear();
                        db.sqlexecute.Parameters.AddWithValue("@empId", kvp.Key);
                        int claimsToCheckCount = db.getcount(sql);

                        if (claimsToCheckCount > 0)
                        {
                            taskBody = "Automatic notification has been received that indicates this employee requires archiving but they currently need to approve some outstanding claims. <br /><br />";
                            archiveOK = false;
                        }
                    }

                    // check that no outstanding claims (current or submitted exist) before allowing auto-archive
                    if (clsclaims.getCount(kvp.Key, ClaimStage.Current) > 0 || clsclaims.getCount(kvp.Key, ClaimStage.Submitted) > 0)
                    {
                        taskBody += "Automatic notification has been received that indicates this employee requires archiving but they currently have some outstanding claims.";
                        archiveOK = false;
                    }

                    #endregion

                    if (archiveOK)
                    {
                        taskBody = "Automatic notification has been received that indicates this employee requires archiving.\nChecks have been performed to ensure that no outstanding actions exist for the employee.";
                        //emps.archiveEmployee(employee.username, curAccount.accountid);
                    }

                    List<object[]> tellWho = notifications.GetNotificationSubscriptions(EmailNotificationType.ESRSummaryNotification);

                    int idOfNotify;
                    switch (subaccount.SubAccountProperties.AutoArchiveType)
                    {
                        case AutoArchiveType.ArchiveManual:
                            // create a task for subscribing audience

                            foreach (object[] o in tellWho)
                            {
                                idOfNotify = (int)o[1];
                                if ((sendType)o[0] == sendType.employee)
                                {
                                    cTaskOwner taskOwner = new cTaskOwner(idOfNotify, sendType.employee, null);

                                    cTask empTask = new cTask(0, null, TaskCommand.ESR_RecordArchiveManual, kvp.Key, DateTime.Now, null, kvp.Key, AppliesTo.Employee, "Employee Archival Due", taskBody, kvp.Value, DateTime.Now, null, TaskStatus.InProgress, taskOwner, false, null);
                                    if (!tasks.taskExists(kvp.Key, AppliesTo.Employee, TaskCommand.ESR_RecordArchiveManual, taskOwner.OwnerId, taskOwner.OwnerType, true))
                                    {
                                        tasks.AddTask(empTask, kvp.Key);

                                        if (archiveOK)
                                        {
                                            archivedCount++;
                                        }
                                        else
                                        {
                                            archiveFailCount++;
                                        }
                                    }
                                }
                                else if ((sendType)o[0] == sendType.team)
                                {
                                    cTeams clsTeams = new cTeams(curAccount.accountid);
                                    cTeam notifyTeam = clsTeams.GetTeamById(idOfNotify);

                                    cTaskOwner taskOwner = new cTaskOwner(notifyTeam.teamid, sendType.team, notifyTeam);
                                    cTask teamTask = new cTask(0, null, TaskCommand.ESR_RecordArchiveManual, kvp.Key, DateTime.Now, null, kvp.Key, AppliesTo.Employee, "Employee Archival Due", "Automatic notification has been received that indicates this employee requires archiving.\nChecks have been performed to ensure that no outstanding actions exist for the employee.", kvp.Value, DateTime.Now, null, TaskStatus.InProgress, taskOwner, false, null);

                                    if (!tasks.taskExists(kvp.Key, AppliesTo.Employee, TaskCommand.ESR_RecordArchiveManual, taskOwner.OwnerId, taskOwner.OwnerType, true))
                                    {
                                        tasks.AddTask(teamTask, kvp.Key);
                                        if (archiveOK)
                                        {
                                            archivedCount++;
                                        }
                                        else
                                        {
                                            archiveFailCount++;
                                        }
                                    }
                                }
                            }
                            break;

                        case AutoArchiveType.ArchiveAuto:

                            if (archiveOK)
                            {
                                emps.GetEmployeeById(kvp.Key).ChangeArchiveStatus(true, null);
                                archivedCount++;
                            }
                            else//If the auto archive cannot archive due to the employee having outstanding claims to submit or approve if a line manager then
                            //a task is created to inform a user of this.
                            {
                                cTeams clsTeams = new cTeams(curAccount.accountid);

                                foreach (object[] o in tellWho)
                                {
                                    idOfNotify = (int)o[1];
                                    if ((sendType)o[0] == sendType.employee)
                                    {

                                        cTaskOwner taskOwner = new cTaskOwner(idOfNotify, sendType.employee, null);
                                        cTask empTask = new cTask(0, null, TaskCommand.ESR_RecordArchiveManual, kvp.Key, DateTime.Now, null, kvp.Key, AppliesTo.Employee, "Automatic Employee Archival not complete", taskBody, kvp.Value, DateTime.Now, null, TaskStatus.InProgress, taskOwner, false, null);
                                        if (!tasks.taskExists(kvp.Key, AppliesTo.Employee, TaskCommand.ESR_RecordArchiveManual, taskOwner.OwnerId, taskOwner.OwnerType, true))
                                        {
                                            tasks.AddTask(empTask, kvp.Key);
                                            archiveFailCount++;
                                        }
                                    }
                                    else if ((sendType)o[0] == sendType.team)
                                    {
                                        cTeam notifyTeam = clsTeams.GetTeamById(idOfNotify);

                                        cTaskOwner taskOwner = new cTaskOwner(notifyTeam.teamid, sendType.team, notifyTeam);
                                        cTask teamTask = new cTask(0, null, TaskCommand.ESR_RecordArchiveManual, kvp.Key, DateTime.Now, null, kvp.Key, AppliesTo.Employee, "Automatic Employee Archival not complete", taskBody, kvp.Value, DateTime.Now, null, TaskStatus.InProgress, taskOwner, false, null);

                                        if (!tasks.taskExists(kvp.Key, AppliesTo.Employee, TaskCommand.ESR_RecordArchiveManual, taskOwner.OwnerId, taskOwner.OwnerType, true))
                                        {
                                            tasks.AddTask(teamTask, kvp.Key);
                                        }
                                    }
                                }
                            }

                            break;
                    }
                }

                if (archivedCount > 0 || archiveFailCount > 0)
                {
                    // log number of archive successes and failures in the autoArchiveLog
                    db.sqlexecute.Parameters.Clear();
                    db.sqlexecute.Parameters.AddWithValue("@archiveCount", archivedCount);
                    db.sqlexecute.Parameters.AddWithValue("@archiveFailCount", archiveFailCount);
                    db.ExecuteSQL("insert into dbo.autoActionLog (archiveCount, archiveFailCount) values (@archiveCount, @archiveFailCount)");

                    return true;
                }

            }
            return false;
        }

        public bool ActivationCheck()
        {
            DBConnection db = new DBConnection(sConnectionString);

            cAccountSubAccounts subaccs = new cAccountSubAccounts(curAccount.accountid);
            cAccountSubAccount subaccount = subaccs.getFirstSubAccount();
            int activateCount = 0;

            if (subaccount.SubAccountProperties.AutoActivateType != AutoActivateType.DoNotActivate)
            {
                cEmailNotifications notifications = new cEmailNotifications(curAccount.accountid);
                cEmployees emps = new cEmployees(curAccount.accountid);
                cTasks tasks = new cTasks(curAccount.accountid, subaccount.SubAccountID);

                foreach (KeyValuePair<int, DateTime?> kvp in emps.getPendingActivations())
                {
                    switch (subaccount.SubAccountProperties.AutoActivateType)
                    {
                        case AutoActivateType.ActivateManual:
                            List<object[]> tellWho = notifications.GetNotificationSubscriptions(EmailNotificationType.ESRSummaryNotification);

                            foreach (object[] o in tellWho)
                            {
                                int idOfNotify = (int)o[1];
                                if ((sendType)o[0] == sendType.employee)
                                {
                                    cTaskOwner taskOwner = new cTaskOwner(idOfNotify, sendType.employee, null);
                                    cTask empTask = new cTask(0, null, TaskCommand.ESR_RecordActivateManual, kvp.Key, DateTime.Now, null, kvp.Key, AppliesTo.Employee, "Employee Activation Due", "Automatic notification has been received that indicates this employee requires activating.", DateTime.Now, kvp.Value, null, TaskStatus.InProgress, taskOwner, false, null);

                                    if (!tasks.taskExists(kvp.Key, AppliesTo.Employee, TaskCommand.ESR_RecordActivateManual, taskOwner.OwnerId, taskOwner.OwnerType, true))
                                    {
                                        tasks.AddTask(empTask, kvp.Key);
                                        activateCount++;
                                    }
                                }
                                else if ((sendType)o[0] == sendType.team)
                                {
                                    cTeams clsteams = new cTeams(curAccount.accountid);
                                    cTeam notifyTeam = clsteams.GetTeamById(idOfNotify);

                                    cTaskOwner taskOwner = new cTaskOwner(notifyTeam.teamid, sendType.team, notifyTeam);
                                    cTask teamTask = new cTask(0, null, TaskCommand.ESR_RecordActivateManual, kvp.Key, DateTime.Now, null, kvp.Key, AppliesTo.Employee, "Employee Archival Due", "Automatic notification has been received that indicates this employee requires activating.", DateTime.Now, kvp.Value, null, TaskStatus.InProgress, taskOwner, false, null);

                                    if (!tasks.taskExists(kvp.Key, AppliesTo.Employee, TaskCommand.ESR_RecordActivateManual, taskOwner.OwnerId, taskOwner.OwnerType, true))
                                    {
                                        tasks.AddTask(teamTask, kvp.Key);
                                        activateCount++;
                                    }
                                }
                            }
                            break;

                        case AutoActivateType.ActivateAuto:
                            emps.Activate(kvp.Key);
                            activateCount++;
                            break;
                    }
                }

                if (activateCount > 0)
                {
                    // log number of archives in the autoArchiveLog
                    db.sqlexecute.Parameters.Clear();
                    db.sqlexecute.Parameters.AddWithValue("@activateCount", activateCount);
                    db.ExecuteSQL("insert into dbo.autoActionLog (activationCount) values (@activateCount)");

                    return true;
                }
            }

            return false;
        }

        public bool AssignmentCheck()
        {
            DBConnection db = new DBConnection(sConnectionString);
            cAccountSubAccounts subaccs = new cAccountSubAccounts(curAccount.accountid);

            //if (subaccs.getSubAccountsCollection().Count > 1)
            //{
            //throw new NotImplementedException("The support for multiple subaccounts has not been fully implemented in the scheduler");
            //}

            int assignmentArchiveFailCount = 0;

            cEmailNotifications notifications = new cEmailNotifications(curAccount.accountid);
            cTasks tasks = new cTasks(curAccount.accountid, subaccs.getFirstSubAccount().SubAccountID);

            //Dictionary<int, int> itemsToArchive = cESRAssignment.getPendingArchives(curAccount.accountid, grace_period, cAccounts.getConnectionString(curAccount.accountid));
            List<object[]> tellWho = notifications.GetNotificationSubscriptions(EmailNotificationType.ESRSummaryNotification);

            db.sqlexecute.Parameters.Add("@assignmentCount", System.Data.SqlDbType.Int);
            db.sqlexecute.Parameters["@assignmentCount"].Direction = System.Data.ParameterDirection.ReturnValue;

            db.ExecuteProc("getAssignmentArchiveCount");
            int assignmentArchiveCount = (int)db.sqlexecute.Parameters["@assignmentCount"].Value;
            db.sqlexecute.Parameters.Clear();

            using (SqlDataReader reader = db.GetStoredProcReader("ESRAssignmentCheck"))
            {
                while (reader.Read())
                {
                    //int esrAssignID = reader.GetInt32(0);
                    int employeeID = reader.GetInt32(1);
                    DateTime finalAssignmentEndDate = reader.GetDateTime(2);

                    foreach (object[] o in tellWho)
                    {
                        int idOfNotify = (int)o[1];
                        if ((sendType)o[0] == sendType.employee)
                        {
                            cTaskOwner taskOwner = new cTaskOwner(idOfNotify, sendType.employee, null);
                            cTask empTask = new cTask(0, null, TaskCommand.ESR_RecordAssignment, employeeID, DateTime.Now, null, employeeID, AppliesTo.Employee, "Employee Assignment Record Archival Due", "Automatic notification has been received that indicates this employee has an assignment record that requires archiving but the assignment record is currently being used in a current or submitted claim.", DateTime.Now, finalAssignmentEndDate, null, TaskStatus.InProgress, taskOwner, false, null);

                            if (!tasks.taskExists(employeeID, AppliesTo.Employee, TaskCommand.ESR_RecordAssignment, taskOwner.OwnerId, taskOwner.OwnerType, true))
                            {
                                tasks.AddTask(empTask, employeeID);
                                assignmentArchiveFailCount++;
                            }
                        }
                        else if ((sendType)o[0] == sendType.team)
                        {
                            cTeams clsteams = new cTeams(curAccount.accountid);
                            cTeam notifyTeam = clsteams.GetTeamById(idOfNotify);
                            cTaskOwner taskOwner = new cTaskOwner(idOfNotify, sendType.team, notifyTeam);
                            cTask teamTask = new cTask(0, null, TaskCommand.ESR_RecordAssignment, employeeID, DateTime.Now, null, employeeID, AppliesTo.Employee, "Employee Assignment Record Archival Due", "Automatic notification has been received that indicates this employee has an assignment record that requires archiving but the assignment record is currently being used in a current or submitted claim.", DateTime.Now, finalAssignmentEndDate, null, TaskStatus.InProgress, taskOwner, false, null);

                            if (!tasks.taskExists(employeeID, AppliesTo.Employee, TaskCommand.ESR_RecordAssignment, taskOwner.OwnerId, taskOwner.OwnerType, true))
                            {
                                tasks.AddTask(teamTask, employeeID);
                                assignmentArchiveFailCount++;
                            }
                        }
                    }
                }
            }

            if (assignmentArchiveCount > 0)
            {
                // log number of archive successes and failures in the autoArchiveLog
                db.sqlexecute.Parameters.Clear();
                db.sqlexecute.Parameters.AddWithValue("@assignmentCount", assignmentArchiveCount);
                db.sqlexecute.Parameters.AddWithValue("@assignmentFailCount", assignmentArchiveFailCount);
                db.ExecuteSQL("insert into dbo.autoActionLog (assignmentCount, assignmentFailCount) values (@assignmentCount, @assignmentFailCount)");

                return true;
            }
            return false;
        }

        /// <summary>
        /// The vehicle activation check.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CarActivationCheck()
        {
            var subaccs = new cAccountSubAccounts(this.curAccount.accountid);
            cAccountSubAccount subaccount = subaccs.getFirstSubAccount();
            if (subaccount.SubAccountProperties.EsrAutoActivateCar)
            {
                int idOfNotify = subaccount.SubAccountProperties.MainAdministrator;

                var tasks = new cTasks(this.curAccount.accountid, subaccount.SubAccountID);
                var employees = new cEmployees(this.curAccount.accountid);

                foreach (KeyValuePair<int, cCar> keyValuePair in employees.GetCarsToActivate())
                {
                    var employeeCars = new cEmployeeCars(this.curAccount.accountid, keyValuePair.Value.employeeid);
                    var currentCar = employeeCars.GetCarByID(keyValuePair.Key);

                    bool carValid = true;
                    var taskBody = new StringBuilder();
                    var employee = employees.GetEmployeeById(currentCar.employeeid);

                    taskBody.AppendFormat("The vehicle ({0} {1} id:{2}) assigned to employee {3}, could not be activated.\n", currentCar.make, currentCar.model, currentCar.carid, employee.FullName);
                    if (currentCar.EngineSize == 0)
                    {
                        carValid = false;
                        taskBody.Append("The 'Engine Size' field is zero.\n");
                    }

                    if (string.IsNullOrEmpty(currentCar.make))
                    {
                        carValid = false;
                        taskBody.Append("The 'Make' field is blank.\n");
                    }

                    if (string.IsNullOrEmpty(currentCar.model))
                    {
                        carValid = false;
                        taskBody.Append("The 'Model' field is blank.\n");
                    }

                    if (currentCar.VehicleEngineTypeId <= 0)
                    {
                        carValid = false;
                        taskBody.Append("The 'Engine Type' field is blank.\n");
                    }

                    if (currentCar.mileagecats.Count == 0)
                    {
                        carValid = false;
                        taskBody.Append("A 'Vehicle Journey Rate' is not set.\n");
                    }

                    if (string.IsNullOrEmpty(currentCar.registration.Trim()))
                    {
                        carValid = false;
                        taskBody.Append("The 'Registration' is blank.\n");
                    }

                    if (carValid)
                    {
                        currentCar.active = true;
                        currentCar.Approved = true;
                        if (currentCar.modifiedby == null || currentCar.modifiedby == 0)
                        {
                            currentCar.modifiedby = idOfNotify;
                        }

                        employeeCars.SaveCar(currentCar);
                    }
                    else
                    {
                        var taskOwner = new cTaskOwner(idOfNotify, sendType.employee, null);

                        var empTask = new cTask(0, subaccount.SubAccountID, TaskCommand.ESR_CarAutoActivate, idOfNotify, DateTime.Now, null, currentCar.carid, AppliesTo.Car, "Vehicle Activation Failed", taskBody.ToString(), keyValuePair.Value.startdate, DateTime.Now, null, TaskStatus.InProgress, taskOwner, false, null);
                        if (!tasks.taskExists(currentCar.carid, AppliesTo.Car, TaskCommand.ESR_CarAutoActivate, taskOwner.OwnerId, taskOwner.OwnerType, true))
                        {
                            tasks.AddTask(empTask, idOfNotify);
                        }
                    }
                }
            }

            return false;
        }
    }
}
