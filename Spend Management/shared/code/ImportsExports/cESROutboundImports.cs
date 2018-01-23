namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.ESRTransferServiceClasses;
    using SpendManagementLibrary.Employees;
    using Spend_Management.shared.code;

    /// <summary>
    /// Class to import the outbound ESR file into expenses so that all employee and asdsignment records are saved.
    /// </summary>
    [Serializable()]
    public class cESRImport
    {
        private int headercount = 0;

        private int footercount = 0;

        private int recordcount = 0;

        private int expectedrecordcount = 0;

        private int processedrecordcount = 0;

        private readonly ICurrentUser _user;

        private int nTemplateID;

        private int nAccountID;

        private int logID;

        private bool validationError;

        private DateTime importStartDate;

        private DateTime importEndDate;

        private byte[] fileData;

        private List<string> lstUsernameFormatFieldMatches;

        private List<string> lstHomeAddressFormatFieldMatches;

        private cField empField;

        private cField modifiedOnField;

        private cField usernamefield;

        private cField activeField;

        private cField nhstrustfield;

        private cField primarycurrencyfield;

        private cField primarycountryfield;

        private cField creationmethodfield;

        private cField createdonField;

        private cField defaultSubaccountIDField;

        private cField passwordField;

        private cField lineManagerField;

        private cTable emptable;

        private cLogging clsLogging;

        private cGlobalCountries clsGlobalCountries = new cGlobalCountries();

        private readonly Dictionary<string, int> lstWorkAddresses = new Dictionary<string, int>();

        private readonly Dictionary<string, int> lstHomeAddresses = new Dictionary<string, int>();

        private string sInsertSQLForPersonInsert;

        private string sUpdateSQLForPersonInsert;

        private string sInsertSQLForAssignmentInsert;

        private string sUpdateSQLForAssignmentInsert;

        private string sUpdateSQLForAssignmentEmployeeInsert;

        private bool bHasEmployeeAssignmentField = false;

        private cElement personElement;

        private cElement assignmentElement;

        private bool hasLineManagerField;

        /// <summary>
        /// Constructor for the cESRImport
        /// </summary>
        /// <param name="user"></param>
        /// <param name="TemplateID">Id of the import template for the import</param>
        /// <param name="data">byte[] of the data from the imported file</param>
        public cESRImport(ICurrentUser user, int TemplateID, byte[] arrFileData)
        {
            this.nAccountID = user.AccountID;
            this._user = user;
            this.nTemplateID = TemplateID;
            this.fileData = arrFileData;
            this.clsLogging = new cLogging(user.AccountID);
        }

        private void prepareFieldVariables()
        {
            var clsFields = new cFields(AccountID);

            empField = clsFields.GetFieldByID(new Guid("EDA990E3-6B7E-4C26-8D38-AD1D77FB2FBF"));
            modifiedOnField = clsFields.GetFieldByID(new Guid("E27C6957-0435-4177-B1A6-B56459466C40"));
            usernamefield = clsFields.GetFieldByID(new Guid("1C45B860-DDAA-47DA-9EEC-981F59CCE795"));
            activeField = clsFields.GetFieldByID(new Guid("B8D81E67-51C2-483A-8015-606A3DBDA0A4"));
            nhstrustfield = clsFields.GetFieldByID(new Guid("9573ED0B-814B-4CB0-916A-8CE25893617D"));
            primarycurrencyfield = clsFields.GetFieldByID(new Guid("026CC190-20D8-427E-9AE2-200747F45670"));
            primarycountryfield = clsFields.GetFieldByID(new Guid("031CF9C5-FFFF-4A7A-AB98-54617A91766A"));
            creationmethodfield = clsFields.GetFieldByID(new Guid("473BF9A3-4D7D-4993-AAB4-096FEE8002B4"));
            createdonField = clsFields.GetFieldByID(new Guid("F3F2E4BF-F594-4125-A924-FD6703DB2F2D"));
            defaultSubaccountIDField = clsFields.GetFieldByID(new Guid("25960ABD-3D30-4EB3-9F51-B553F6CDABC3"));
            passwordField = clsFields.GetFieldByID(new Guid("669236FD-CBA3-4E80-B58D-68A52C45032B"));
            lineManagerField = clsFields.GetFieldByID(new Guid("96F11C6D-7615-4ABD-94EC-0E4D34E187A0"));
            nhstrustfield = clsFields.GetFieldByID(new Guid("9573ED0B-814B-4CB0-916A-8CE25893617D"));

            var clstables = new cTables(AccountID);
            emptable = clstables.GetTableByName("employees");
        }

        #region Properties

        /// <summary>
        /// ID of the account
        /// </summary>
        public int AccountID
        {
            get
            {
                return nAccountID;
            }
        }

        /// <summary>
        /// ID of the import template for this outbound import
        /// </summary>
        public int TemplateID
        {
            get
            {
                return nTemplateID;
            }
        }

        #endregion

        public string setInsertSQLForPersonTemplate(ref cImportTemplate template, ref cTable table)
        {
            cFields clsFields = new cFields(this.AccountID);

            cField tempField;
            cUpdateQuery updateQry = new cUpdateQuery(this.AccountID, cAccounts.getConnectionString(this.AccountID), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, table, new cTables(this.AccountID), clsFields);

            foreach (cImportTemplateMapping mapping in template.Mappings)
            {
                if (mapping.ElementType == ImportElementType.Employee && mapping.Field != null)
                {
                    tempField = mapping.Field;
                    cField overrideField = this.UDFOverrideField(mapping, ref clsFields);

                    if (tempField.TableID == table.TableID || overrideField != null)
                    {
                        if (overrideField == null)
                        {
                            updateQry.addColumn(tempField, DBNull.Value);
                        }
                        else
                        {
                            updateQry.addColumn(overrideField, DBNull.Value);
                        }
                    }
                }
            }

            updateQry.addColumn(this.usernamefield, DBNull.Value);
            updateQry.addColumn(this.activeField, false);
            updateQry.addColumn(this.nhstrustfield, DBNull.Value);
            updateQry.addColumn(this.primarycurrencyfield, DBNull.Value);
            updateQry.addColumn(this.primarycountryfield, DBNull.Value);
            updateQry.addColumn(this.creationmethodfield, DBNull.Value);
            updateQry.addColumn(this.createdonField, DateTime.UtcNow);
            updateQry.addColumn(this.defaultSubaccountIDField, DBNull.Value);
            updateQry.addColumn(this.passwordField, DBNull.Value);

            return updateQry.insertSQL;
        }

        private string setUpdateSQLForPersonTemplate(ref cImportTemplate template, ref cTable table)
        {
            cFields clsFields = new cFields(this.AccountID);
            cField tempField;
            cUpdateQuery updateQry = new cUpdateQuery(this.AccountID, cAccounts.getConnectionString(this.AccountID), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, table, new cTables(this.AccountID), clsFields);

            foreach (cImportTemplateMapping mapping in template.Mappings)
            {
                if (mapping.ElementType == ImportElementType.Employee)
                {
                    tempField = mapping.Field;
                    cField overrideField = this.UDFOverrideField(mapping, ref clsFields);

                    if (tempField != null)
                    {
                        if (tempField.TableID == table.TableID || overrideField != null)
                        {
                            if (overrideField == null)
                            {
                                updateQry.addColumn(tempField, DBNull.Value);
                            }
                            else
                            {
                                updateQry.addColumn(overrideField, DBNull.Value);
                            }
                        } 
                    }
                }
            }

            updateQry.addColumn(this.modifiedOnField, DBNull.Value);
            updateQry.addFilter(this.empField, ConditionType.Equals, new object[] { 0 }, null, ConditionJoiner.None, null); // null as no reports/esr joinvia integration yet? !!!!!!!
            return updateQry.updateSQL;
        }

        /// <summary>
        /// Generates the insert sql for assignment employee data in readiness for applying parameter values
        /// </summary>
        /// <param name="template">Import template object</param>
        /// <param name="table">Table object being imported into</param>
        /// <returns>Insert SQL string</returns>
        public string SetInsertSqlForAssignmentTemplate(ref cImportTemplate template, ref cTable table)
        {
            cFields clsFields = new cFields(this.AccountID);

            cField tempField;
            cUpdateQuery updateQry = new cUpdateQuery(this.AccountID, cAccounts.getConnectionString(this.AccountID), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, table, new cTables(this.AccountID), clsFields);

            foreach (cImportTemplateMapping mapping in template.Mappings)
            {
                if (mapping.ElementType == ImportElementType.Assignment)
                {
                    tempField = mapping.Field;

                    if (tempField != null && tempField.TableID == table.TableID)
                    {
                        updateQry.addColumn(tempField, DBNull.Value);
                    }
                }
            }

            updateQry.addColumn(clsFields.GetFieldByID(new Guid("0CFDCDCD-1F51-4578-BE89-90610D6D7F7D")), DBNull.Value);
            updateQry.addColumn(clsFields.GetFieldByID(new Guid("E4BD2B1E-28C1-4550-B780-9084520F92D0")), DBNull.Value);
            
            return updateQry.insertSQL;
        }

        /// <summary>
        /// Generates the update sql for assignment employee data in readiness for applying parameter values
        /// </summary>
        /// <param name="template">Import template object</param>
        /// <param name="table">Table object being imported into</param>
        /// <returns>Update SQL string</returns>
        public string SetUpdateSqlForAssignmentEmployeeData(ref cImportTemplate template, ref cTable table)
        {
            cField tempField;
            cUpdateQuery updateEmpQry = new cUpdateQuery(this.AccountID, cAccounts.getConnectionString(this.AccountID), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, table, new cTables(this.AccountID), new cFields(this.AccountID));

            foreach (cImportTemplateMapping mapping in template.Mappings)
            {
                if (mapping.ElementType == ImportElementType.Assignment)
                {
                    tempField = mapping.Field;
                    if (tempField != null && (tempField.TableID == table.TableID || tempField.TableID == table.UserDefinedTableID))
                    {
                        if (mapping.DestinationField != "Supervisor Employee Number" && mapping.DestinationField != "Supervisor Assignment Number")
                        {
                            if (tempField.TableID == table.TableID)
                            {
                                updateEmpQry.addColumn(tempField, DBNull.Value);
                                this.bHasEmployeeAssignmentField = true;
                            }
                        }
                    }
                }
            }

            updateEmpQry.addFilter(this.empField, ConditionType.Equals, new object[] { 0 }, null, ConditionJoiner.None, null); // null as no reports/esr joinvia integration yet? !!!!!!!
            return updateEmpQry.updateSQL;
        }

        /// <summary>
        /// Generates the update sql for assignment record data in readiness for applying parameter values
        /// </summary>
        /// <param name="template">Import template object</param>
        /// <param name="table">Table object being imported into</param>
        /// <returns>Update SQL string</returns>
        public string SetUpdateSqlForAssignmentTemplate(ref cImportTemplate template, ref cTable table)
        {
            cFields clsFields = new cFields(this.AccountID);

            cField tempField;
            cUpdateQuery updateQry = new cUpdateQuery(this.AccountID, cAccounts.getConnectionString(this.AccountID), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, table, new cTables(this.AccountID), clsFields);
            foreach (cImportTemplateMapping mapping in template.Mappings)
            {
                if (mapping.ElementType == ImportElementType.Assignment)
                {
                    tempField = mapping.Field;

                    if (tempField != null && tempField.TableID == table.TableID)
                    {
                        updateQry.addColumn(tempField, DBNull.Value);
                    }
                }
            }

            updateQry.addColumn(clsFields.GetFieldByID(new Guid("12539FEB-9766-4E2F-B786-D48EB247614F")), DateTime.UtcNow);

            updateQry.addFilter(clsFields.GetFieldByID(new Guid("0CFDCDCD-1F51-4578-BE89-90610D6D7F7D")), ConditionType.Equals, new object[] { 0 }, null, ConditionJoiner.And, null);
            
            // null as no reports/esr joinvia integration yet? !!!!!!!
            updateQry.addFilter(clsFields.GetFieldByID(new Guid("C23858B8-7730-440E-B481-C43FE8A1DBEF")), ConditionType.Equals, new object[] { 0 }, null, ConditionJoiner.And, null);
            
            // null as no reports/esr joinvia integration yet? !!!!!!!
            return updateQry.updateSQL;
        }

        /// <summary>
        /// Method that loops round every row of the outbound import file and saves each record type
        /// </summary>
        public OutboundStatus importOutboundData(int dataID)
        {
            OutboundStatus status = OutboundStatus.None;

            try
            {
                string sRecordType;

                string currentline;
                string[] data;
                int employeeID = 0;

                MemoryStream stream = new MemoryStream(this.fileData);

                this.recordcount = 1;
                this.prepareFieldVariables();

                Employee emp = null;
                cEmployees clsEmployees = new cEmployees(this.AccountID);
                cImportTemplates clsImportTemps = new cImportTemplates(this.AccountID);
                cImportTemplate template = clsImportTemps.getImportTemplateByID(this.TemplateID);
                SortedList<int, string> lstLineManagersToUpdate = new SortedList<int, string>();
                Dictionary<int, Dictionary<int,string>> lstUserdefinedLineManagersToUpdate = new Dictionary<int, Dictionary<int, string>>();
                List<int> lstLineManagersToCheck = new List<int>();
                
                cElements clsElements = new cElements();
                cElement element = null;
                cESRTrusts clsTrusts = new cESRTrusts(this.AccountID);
                cESRTrust trust = clsTrusts.GetESRTrustByID(template.NHSTrustID);

                // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.
                cAccountSubAccounts subaccs = new cAccountSubAccounts(this.AccountID);
                cAccountSubAccount subacc = subaccs.getFirstSubAccount();

                cTables clsTables = new cTables(this.AccountID);
                cTable employeeTable = clsTables.GetTableByName("employees");
                cTable assignmentTable = clsTables.GetTableByName("esr_assignments");
                this.sInsertSQLForPersonInsert = this.setInsertSQLForPersonTemplate(ref template, ref employeeTable);
                this.sUpdateSQLForPersonInsert = this.setUpdateSQLForPersonTemplate(ref template, ref employeeTable);
                this.sInsertSQLForAssignmentInsert = this.SetInsertSqlForAssignmentTemplate(ref template, ref assignmentTable);
                this.sUpdateSQLForAssignmentInsert = this.SetUpdateSqlForAssignmentTemplate(ref template, ref assignmentTable);
                this.sUpdateSQLForAssignmentEmployeeInsert = this.SetUpdateSqlForAssignmentEmployeeData(ref template, ref employeeTable);

                this.personElement = clsElements.GetElementByName("Employees");
                this.assignmentElement = clsElements.GetElementByName("ESRAssignment");

                this.logID = this.clsLogging.saveLog(0, LogType.ESROutboundImport, 0, 0, 0, 0, 0);

                // Made up ICurrentUser for use by relationship lookup
                ICurrentUserBase curUser = new CurrentUser(this.AccountID, 0, -1, Modules.ESR, subacc.SubAccountID);

                using (StreamReader reader = new StreamReader(stream))
                {
                    while ((currentline = reader.ReadLine()) != null)
                    {
                        Regex r = new Regex(trust.DelimiterCharacter + "(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                        data = r.Split(currentline);
                        sRecordType = data[0];

                        switch (sRecordType)
                        {
                            case "PER":
                                employeeID = this.savePerson(data, ref template, ref subacc, ref employeeTable);
                                this.processedrecordcount++;
                                break;
                            case "ASG":
                                this.saveAssignment(data, ref template, employeeID, ref lstLineManagersToUpdate, ref lstLineManagersToCheck, ref lstUserdefinedLineManagersToUpdate, ref subacc, ref assignmentTable, curUser);
                                this.processedrecordcount++;
                                break;
                        }

                        this.recordcount++;
                    }

                    reader.Close();
                }

                // If any line managers need to be assigned to a user but the line manager employee record did not exist at the time then assign the line manager here
                #region Line Managers

                int LineManagerID = 0;
                element = clsElements.GetElementByName("EmployeeLineManager");

                foreach (KeyValuePair<int, string> kvp in lstLineManagersToUpdate)
                {
                    LineManagerID = clsEmployees.getEmployeeidByAssignment(this.AccountID, kvp.Value);
                    emp = clsEmployees.GetEmployeeById(kvp.Key);

                    if (LineManagerID > 0)
                    {
                        emp.LineManager = LineManagerID;
                        emp.Save(null);

                        if (!lstLineManagersToCheck.Contains(LineManagerID))
                        {
                            lstLineManagersToCheck.Add(LineManagerID);
                        }

                        if (emp != null)
                        {
                            this.clsLogging.addLogItem(LogReasonType.SuccessAdd, element, "Line manager assigned for employee " + emp.Forename + " " + emp.Surname + " with assignment number " + emp.PayrollNumber + " assigned Line Manager assignment number " + kvp.Value);
                        }
                    }
                    else
                    {
                        if (emp != null)
                        {
                            this.clsLogging.addLogItem(LogReasonType.Warning, element, "Line manager not assigned for employee " + emp.Forename + " " + emp.Surname + " with assignment number " + emp.PayrollNumber + ". Could not find Line Manager assignment number " + kvp.Value);
                        }
                    }
                }

                cUserdefinedFields userdefined = new cUserdefinedFields(this.AccountID);
                cFields clsFields = new cFields(this.AccountID);

                foreach (int userFieldId in lstUserdefinedLineManagersToUpdate.Keys)
                {
                    cUserDefinedField userField = userdefined.GetUserDefinedById(userFieldId);

                    foreach (KeyValuePair<int, string> kvp in lstUserdefinedLineManagersToUpdate[userFieldId])
                    {
                        LineManagerID = clsEmployees.getEmployeeidByAssignment(this.AccountID, kvp.Value);
                        emp = clsEmployees.GetEmployeeById(kvp.Key);

                        if (LineManagerID > 0)
                        {
                            if (!lstLineManagersToCheck.Contains(LineManagerID))
                            {
                                lstLineManagersToCheck.Add(LineManagerID);
                            }

                            SortedList<int, object> udfValues = new SortedList<int, object> { { userFieldId, LineManagerID } };
                            userdefined.SaveValues(userField.table, emp.EmployeeID, udfValues, clsTables, clsFields, (cCurrentUserBase)curUser, skipFieldsNotOnPage: true);

                            if (emp != null)
                            {
                                this.clsLogging.addLogItem(LogReasonType.SuccessAdd, element, "Line manager assigned for employee " + emp.Forename + " " + emp.Surname + " with assignment number " + emp.PayrollNumber + " assigned Line Manager assignment number " + kvp.Value);
                            }
                        }
                        else
                        {
                            if (emp != null)
                            {
                                this.clsLogging.addLogItem(LogReasonType.Warning, element, "Line manager not assigned for employee " + emp.Forename + " " + emp.Surname + " with assignment number " + emp.PayrollNumber + ". Could not find Line Manager assignment number " + kvp.Value);
                            }
                        }
                    }
                }

                #endregion

                #region Check and email list of Line Managers who don't have a check and pay Access role

                cAccessRoles clsAccessRoles = new cAccessRoles(this.AccountID, cAccounts.getConnectionString(this.AccountID));
                cAccessRole role = null;

                List<Employee> lstLineManagersToModify = new List<Employee>();
                bool addLineManager = false;

                foreach (int id in lstLineManagersToCheck)
                {
                    emp = clsEmployees.GetEmployeeById(id);

                    if (emp != null)
                    {
                        addLineManager = false;
                        List<int> lstAccessRoles = emp.GetAccessRoles().GetBy(subacc.SubAccountID);

                        foreach (int roleID in lstAccessRoles)
                        {
                            role = clsAccessRoles.GetAccessRoleByID(roleID);

                            if (role != null)
                            {
                                if (role.ElementAccess.ContainsKey(SpendManagementElement.CheckAndPay))
                                {
                                    if (role.ElementAccess[SpendManagementElement.CheckAndPay].CanView)
                                    {
                                        addLineManager = true;
                                    }
                                }
                            }
                        }

                        if (!addLineManager)
                        {
                            if (!lstLineManagersToModify.Contains(emp))
                            {
                                lstLineManagersToModify.Add(emp);
                            }
                        }
                    }
                }

                StringBuilder strBody = new StringBuilder();

                foreach (Employee reqEmp in lstLineManagersToModify)
                {
                    strBody.Append("Username: " + reqEmp.Username + " with name " + reqEmp.Forename + " " + reqEmp.Surname + "<br />");
                }

             

                #endregion

                // Set the end date of the import
                this.importEndDate = DateTime.Now;
                this.clsLogging.addLogItem(LogReasonType.None, null, "Process finish time = " + this.importEndDate);

                // Finish the log so summary line counters can be added
                this.clsLogging.saveLog(this.logID, LogType.ESROutboundImport, this.expectedrecordcount, this.processedrecordcount, 0, 0, 0);

                #region Get the import status of the run template from the log line processing

                cLog log = this.clsLogging.getLogFromDatabase(this.logID, LogReasonType.None);
                ImportHistoryStatus importStatus = new ImportHistoryStatus();

                if (log.numFailedUpdates > 0 && log.numSuccessfulUpdates > 0)
                {
                    importStatus = ImportHistoryStatus.Success_With_Errors;
                }
                else if (log.numFailedUpdates > 0 && log.numSuccessfulUpdates == 0)
                {
                    importStatus = ImportHistoryStatus.Failure;
                }
                else if (log.numFailedUpdates == 0 && log.numSuccessfulUpdates > 0 && log.numWarningUpdates > 0)
                {
                    importStatus = ImportHistoryStatus.Success_With_Warnings;
                }
                else if (log.numSuccessfulUpdates > 0 && log.numWarningUpdates == 0)
                {
                    importStatus = ImportHistoryStatus.Success;
                }

                #endregion

                status = OutboundStatus.Complete;

                // Save the import history
                cImportHistory clsHistory = new cImportHistory(this.AccountID);
                clsHistory.saveHistory(new cImportHistoryItem(0, this.TemplateID, this.logID, this.importStartDate, importStatus, ApplicationType.ESROutboundImport, dataID, DateTime.UtcNow, null));
            }
            catch (Exception ex)
            {
                status = OutboundStatus.Failed;
                cEventlog.LogEntry(ex.Message + " " + ex.StackTrace);

                // Send email of the error
                ErrorHandlerWeb clsessors = new ErrorHandlerWeb();
                clsessors.SendAutomatedProcessError(ex);
                this.clsLogging.addLogItem(LogReasonType.Error, null, "An error occurred in the import. Error is " + ex.Message + " " + ex.StackTrace);
                cImportHistory clsHistory = new cImportHistory(this.AccountID);
                clsHistory.saveHistory(new cImportHistoryItem(0, this.TemplateID, this.logID, this.importStartDate, ImportHistoryStatus.Failure, ApplicationType.ESROutboundImport, dataID, DateTime.UtcNow, null));
            }
            finally
            {
                this.clsLogging.saveAllLogItems(this.logID);
            }

            return status;
        }


        /// <summary>
        /// Validate the import before processing the outbound file. If it is invalid then no data will be imported.
        /// </summary>
        /// <returns>A boolean value to see if the validation was successful</returns>
        public bool validateImport(int dataID)
        {
            cImportTemplates clsImportTemps = new cImportTemplates(AccountID);
            cImportTemplate template = clsImportTemps.getImportTemplateByID(TemplateID);

            cESRTrusts clsTrusts = new cESRTrusts(AccountID);
            cESRTrust trust = clsTrusts.GetESRTrustByID(template.NHSTrustID);
            //Set the start date of the import
            importStartDate = DateTime.Now;

            clsLogging.addLogItem(LogReasonType.None, null, "Process start time = " + importStartDate);

            MemoryStream stream = new MemoryStream(fileData);
            StreamReader reader = new StreamReader(stream);
            string sRecordType;
            string currentline;
            string[] data;
            bool valid = true;


            while ((currentline = reader.ReadLine()) != null)
            {
                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(trust.DelimiterCharacter + "(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                data = r.Split(currentline);
                sRecordType = data[0];

                switch (sRecordType)
                {
                    case "HDR":
                        headercount++;
                        break;
                    case "FTR":
                        footercount++;
                        expectedrecordcount = int.Parse(data[2]);
                        break;
                    case "PER":
                        recordcount++;
                        break;
                    case "ASG":
                        recordcount++;
                        break;
                    default:
                        clsLogging.addLogItem(LogReasonType.Error, null, "Unknown record type encountered in validateImport(). Record Type: { " + sRecordType + " }");
                        break;
                }
            }

            //check file before processing
            if (headercount != 1)
            {
                clsLogging.addLogItem(LogReasonType.Error, null, "This file is invalid as it does not have a header record.");
                valid = false;
            }
            if (footercount != 1)
            {
                clsLogging.addLogItem(LogReasonType.Error, null, "This file is invalid as it does not have a footer record.");
                valid = false;
            }
            if (recordcount != expectedrecordcount)
            {
                clsLogging.addLogItem(LogReasonType.Error, null, "This file is invalid as it should contain " + expectedrecordcount + " records however only " + recordcount + " were found.");
                valid = false;
            }

            if (!valid)
            {
                //Set the end date of the import
                importEndDate = DateTime.Now;

                clsLogging.addLogItem(LogReasonType.None, null, "Process finish time = " + importEndDate);

                //Finish the log so summary lines can be added
                clsLogging.saveLog(logID, LogType.ESROutboundImport, expectedrecordcount, 0, 0, 0, 0);

                //Save the import history
                cImportHistory clsHistory = new cImportHistory(AccountID);
                clsHistory.saveHistory(new cImportHistoryItem(0, TemplateID, logID, importStartDate, ImportHistoryStatus.Failure, ApplicationType.ESROutboundImport, dataID, DateTime.UtcNow, DateTime.UtcNow));
            }

            return valid;

        }

        /// <summary>
        /// Save the employee to the database by dynamically generating the SQL based on the fields from the import template mapping
        /// </summary>
        /// <param name="data">string array of the data line</param>
        /// <param name="template">The import template to use</param>
        /// <param name="reqSubAccount">Subaccount being imported to</param>
        /// <param name="table">Table class object </param>
        /// <returns>The employeeID</returns>
        private int savePerson(string[] data, ref cImportTemplate template, ref cAccountSubAccount reqSubAccount, ref cTable table)
        {
            int employeeID = 0;
            cField field;
            cField tempField;
            object fieldValue;
            string dataVal;
            string employeeNum = "";
            string firstname = "";
            string lastname = "";
            string address1 = "";
            string address2 = "";
            string town = "";
            string county = "";
            string postcode = "";
            string homeCountry = "";

            // Used to replace the referenced ESR file column with the value in the username and home location formats
            string formatValue = "";

            // Used to store the username formatted value
            string usernameVal = reqSubAccount.SubAccountProperties.ImportUsernameFormat;

            // Used to store the home address formatted value
            string homeAddressVal = reqSubAccount.SubAccountProperties.ImportHomeAddressFormat;

            this.validationError = false;
            bool saveEmployeeRec = false;
            bool overrideUDFContinue;

            // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.
            int subAccountID = reqSubAccount.SubAccountID;

            // Made up ICurrentUser for use by relationship lookup
            ICurrentUserBase curUser = new CurrentUser(this.AccountID, 0, -1, Modules.ESR, subAccountID);
            cUserdefinedFields userdefined = new cUserdefinedFields(this.AccountID);
            SortedList<int, object> udfValues = new SortedList<int, object>();

            int fieldcount = 1;
            DBConnection updateconn = new DBConnection(cAccounts.getConnectionString(this.AccountID));

            // Get data for address1 address2 county postocde and country. -- Down and dirty.

            address1 = data[14].TrimStart('"').TrimEnd('"');
            address2 = data[15].TrimStart('"').TrimEnd('"');
            town = data[16].TrimStart('"').TrimEnd('"');
            county = data[17].TrimStart('"').TrimEnd('"');
            homeCountry = data[19].TrimStart('"').TrimEnd('"');
            postcode = data[18].TrimStart('"').TrimEnd('"');

            foreach (cImportTemplateMapping mapping in template.Mappings)
            {
                if (mapping.ElementType == ImportElementType.Employee)
                {
                    overrideUDFContinue = false;
                    tempField = mapping.Field;
                    if (tempField != null && (tempField.TableID == table.TableID || tempField.TableID == table.UserDefinedTableID))
                    {
                        dataVal = data[mapping.ColRef].TrimStart('"').TrimEnd('"');

                        // Need to get the employee number to check if they already exist
                        if (mapping.DestinationField == "Employee Number")
                        {
                            employeeNum = dataVal;

                            if (employeeNum == string.Empty)
                            {
                                this.clsLogging.addLogItem(LogReasonType.Error, this.personElement, "Line " + this.recordcount + ": The employee number is blank and will not be imported");
                                this.validationError = true;
                                break;
                            }
                        }

                        switch (mapping.DestinationField)
                        {
                            case "First Name":  //TODO:  Change the mapping for address?!
                                // Need the first name for username creation
                                firstname = dataVal;
                                overrideUDFContinue = true;
                                break;
                            case "Last Name":
                                // Need the last name for username creation
                                lastname = dataVal;
                                overrideUDFContinue = true;
                                break;
                            case "Employee's Address 1st line":
                                address1 = dataVal;
                                formatValue = dataVal;
                                break;
                            case "Employees Address Postcode":
                                postcode = dataVal;
                                formatValue = dataVal;
                                break;
                            case "Employees Address Country":
                                homeCountry = dataVal;
                                break;
                        }

                        if (mapping.DestinationField != "Employee's Address 1st line" || mapping.DestinationField != "Employees Address Postcode")
                        {
                            fieldValue = this.parseDataTypeValue(dataVal, mapping, ref this.validationError);

                            if (mapping.DestinationField == "Title")
                            {
                                if (fieldValue.ToString() == string.Empty)
                                {
                                    overrideUDFContinue = true;
                                    fieldValue = ".";
                                }
                            }

                            field = mapping.Field;

                            if (field.FieldSource == cField.FieldSourceType.Userdefined)
                            {
                                int udfFieldId = 0;
                                
                                // if is a relationship field, then need to lookup ID for value being saved
                                if (!this.GetRelationshipIdByValue(field, ref fieldValue, ref curUser, out udfFieldId))
                                {
                                    break;
                                }

                                if (field.FieldType == "S" && field.Length > 0)
                                {
                                    fieldValue = this.CheckFieldValueLength(field, fieldValue.ToString());
                                }

                                if (!udfValues.ContainsKey(udfFieldId))
                                {
                                    udfValues.Add(udfFieldId, fieldValue);

                                    saveEmployeeRec = true;
                                }

                                if (!overrideUDFContinue)
                                {
                                    // if not also saving a mandatory field, go straight to proceed with next field
                                    continue;
                                }
                            }

                            if (fieldValue.ToString() != string.Empty)
                            {
                                formatValue = fieldValue.ToString();

                                // Trim the value if it exceeds the field length 
                                if (field.FieldType == "S")
                                {
                                    if (field.Length > 0)
                                    {
                                        fieldValue = this.CheckFieldValueLength(field, fieldValue.ToString());
                                    }

                                    updateconn.AddWithValue("@" + fieldcount, fieldValue, field.Length);
                                }
                                else
                                {
                                    updateconn.sqlexecute.Parameters.AddWithValue("@" + fieldcount, fieldValue);
                                }
                            }
                            else
                            {
                                updateconn.AddWithValue("@" + fieldcount, DBNull.Value, field.Length);
                            }
                        }


                        #region Set the matching ESR employee file field value for the username format

                        if (reqSubAccount.SubAccountProperties.ImportUsernameFormat != string.Empty)
                        {
                            usernameVal = this.SetStringFormatValues(usernameVal, mapping.DestinationField, formatValue);
                        }

                        #endregion

                        #region Set the matching ESR employee file field value for the home address format

                        if (reqSubAccount.SubAccountProperties.ImportHomeAddressFormat != string.Empty)
                        {
                            homeAddressVal = this.SetStringFormatValues(homeAddressVal, mapping.DestinationField, formatValue);
                            formatValue = string.Empty;
                        }

                        #endregion

                        saveEmployeeRec = true;
                        fieldcount++;
                    }
                }
            }
            if (reqSubAccount.SubAccountProperties.ImportHomeAddressFormat != string.Empty)
            {
                homeAddressVal = this.SetStringFormatValues(homeAddressVal, "Employee's Address 1st line", address1);
                homeAddressVal = this.SetStringFormatValues(homeAddressVal, "Employee's Address 2nd line", address2);
                homeAddressVal = this.SetStringFormatValues(homeAddressVal, "Employee's Address Town", town);
                homeAddressVal = this.SetStringFormatValues(homeAddressVal, "Employees Address County", county);
                homeAddressVal = this.SetStringFormatValues(homeAddressVal, "Employees Address Postcode", postcode);
                homeAddressVal = this.SetStringFormatValues(homeAddressVal, "Employees Address Country", homeCountry);
            }

            // No employee fields mapped
            if (!saveEmployeeRec)
            {
                this.clsLogging.addLogItem(
                    LogReasonType.Warning, this.personElement, "Line " + this.recordcount + ": The employee record will not be imported as there are no employee fields mapped on the import template");
            }

            if (!this.validationError && saveEmployeeRec)
            {
                #region Check existence of employee

                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
                string strsql = "getEsrAssignmentsCount";
                expdata.sqlexecute.Parameters.AddWithValue("@employeeNum", employeeNum);
                int recCount = expdata.getcount(strsql, CommandType.StoredProcedure);
                expdata.sqlexecute.Parameters.Clear();

                #endregion

                cEmployees clsEmployees = new cEmployees(this.AccountID);

                if (recCount > 0)
                {
                    // Employee exists update
                    strsql = "getEsrAssignmentsEmployeeId";
                    expdata.sqlexecute.Parameters.AddWithValue("@employeeNum", employeeNum);
                    using (System.Data.SqlClient.SqlDataReader reader = expdata.GetStoredProcReader(strsql))
                    {
                        expdata.sqlexecute.Parameters.Clear();
                        employeeID = 0;

                        while (reader.Read())
                        {
                            employeeID = reader.GetInt32(0);
                        }

                        reader.Close();
                    }

                    updateconn.sqlexecute.Parameters.AddWithValue("@" + fieldcount, DateTime.Now);
                    fieldcount++;
                    updateconn.sqlexecute.Parameters.AddWithValue("@filter1_0_0", employeeID);

                    try
                    {
                        if (employeeID > 0)
                        {
                            updateconn.ExecuteSQL(this.sUpdateSQLForPersonInsert);
                            updateconn.sqlexecute.Parameters.Clear();

                            // save any user defined data
                            if (udfValues.Count > 0)
                            {
                                userdefined.SaveValues(table.GetUserdefinedTable(), employeeID, udfValues, new cTables(this.AccountID), new cFields(this.AccountID), (cCurrentUserBase)curUser, skipFieldsNotOnPage: true);
                            }

                            User.CacheRemove(employeeID, this.AccountID);

                            this.clsLogging.addLogItem(LogReasonType.SuccessUpdate, this.personElement, "Line " + this.recordcount + ": Updated employee " + firstname + " " + lastname + " with assignment number " + employeeNum);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.clsLogging.addLogItem(LogReasonType.SQLError, this.personElement, "Line " + this.recordcount + ": SQL Error {" + ex.Message + "}");

                        // Send email of the error
                        ErrorHandlerWeb clsessors = new ErrorHandlerWeb();
                        clsessors.SendAutomatedProcessError(ex);
                    }
                }
                else
                {
                    // Add employee
                    #region Create a Username

                    string username;
                    int usernameCount = 0;

                    // If the username has a format then run the calculate method
                    if (usernameVal != string.Empty)
                    {
                        cCalculationField clsCalField = new cCalculationField(this.AccountID);
                        username = clsCalField.CalculatedColumn(usernameVal, table, employeeID, new cFields(this.AccountID), cAccounts.getConnectionString(this.AccountID),  new cTables(this.AccountID));
                    }
                    else
                    {
                        // If no format set then set below as the default
                        username = firstname + "." + lastname;
                    }

                    // Check for duplicates and if there is append the username with a numeric value
                    strsql = "select count(*) from employees where username = @username";
                    expdata.sqlexecute.Parameters.AddWithValue("@username", username);
                    int usernameRecCount = expdata.getcount(strsql);
                    expdata.sqlexecute.Parameters.Clear();

                    string newUsername = string.Empty;

                    if (usernameRecCount == 0)
                    {
                        newUsername = username;
                    }
                    else
                    {
                        while (usernameRecCount > 0)
                        {
                            newUsername = username + usernameCount;
                            strsql = "select count(*) from employees where username = @username";
                            expdata.sqlexecute.Parameters.AddWithValue("@username", newUsername);
                            usernameRecCount = expdata.getcount(strsql);
                            expdata.sqlexecute.Parameters.Clear();
                            usernameCount++;
                        }
                    }

                    #endregion

                    updateconn.sqlexecute.Parameters.AddWithValue(string.Format("@{0}", fieldcount++), newUsername);
                    updateconn.sqlexecute.Parameters.AddWithValue(string.Format("@{0}", fieldcount++), false);
                    updateconn.sqlexecute.Parameters.AddWithValue(string.Format("@{0}", fieldcount++), template.NHSTrustID);
                    updateconn.sqlexecute.Parameters.AddWithValue(string.Format("@{0}", fieldcount++), reqSubAccount.SubAccountProperties.BaseCurrency);
                    updateconn.sqlexecute.Parameters.AddWithValue(string.Format("@{0}", fieldcount++), reqSubAccount.SubAccountProperties.HomeCountry);
                    updateconn.sqlexecute.Parameters.AddWithValue(string.Format("@{0}", fieldcount++), (int)CreationMethod.ESROutbound);
                    updateconn.sqlexecute.Parameters.AddWithValue(string.Format("@{0}", fieldcount++), DateTime.UtcNow);
                    updateconn.sqlexecute.Parameters.AddWithValue(string.Format("@{0}", fieldcount++), subAccountID);
                    
                    cSecureData secureData = new cSecureData();
                    string NewPasswordEncryped = secureData.Encrypt(Guid.NewGuid().ToString());
                    updateconn.sqlexecute.Parameters.AddWithValue("@" + fieldcount, NewPasswordEncryped);
                    updateconn.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                    updateconn.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;

                    try
                    {
                        updateconn.ExecuteSQL(this.sInsertSQLForPersonInsert);
                        employeeID = (int)updateconn.sqlexecute.Parameters["@identity"].Value;
                        updateconn.sqlexecute.Parameters.Clear();

                        if (employeeID > 0)
                        {
                            // save any user defined data
                            if (udfValues.Count > 0)
                            {
                                userdefined.SaveValues(table.GetUserdefinedTable(), employeeID, udfValues, new cTables(this.AccountID), new cFields(this.AccountID), (cCurrentUserBase)curUser, skipFieldsNotOnPage: true);
                            }

                            this.clsLogging.addLogItem(
                                LogReasonType.SuccessAdd, this.personElement, "Line " + this.recordcount + ": Added employee " + firstname + " " + lastname + " with assignment number " + employeeNum);
                        }
                        else
                        {
                            this.clsLogging.addLogItem(LogReasonType.Error, this.personElement, "Line " + this.recordcount + ": Failed to add employee");
                        }
                    }
                    catch (Exception ex)
                    {
                        this.clsLogging.addLogItem(LogReasonType.SQLError, this.personElement, "Line " + this.recordcount + ": SQL Error {" + ex.Message + "}");

                        // Send email of the error
                        ErrorHandlerWeb clsessors = new ErrorHandlerWeb();
                        clsessors.SendAutomatedProcessError(ex);
                    }

                    if (employeeID > 0)
                    {
                        Employee employee = clsEmployees.GetEmployeeById(employeeID);


                    if (reqSubAccount.SubAccountProperties.DefaultItemRole.HasValue)
                    {
                        if (reqSubAccount.SubAccountProperties.DefaultItemRole.Value != 0)
                        {
                            List<EmployeeItemRole> lstItemRoles = new List<EmployeeItemRole>();
                            lstItemRoles.Add(new EmployeeItemRole((int)reqSubAccount.SubAccountProperties.DefaultItemRole));
                                employee.GetItemRoles().Add(lstItemRoles, null);
                        }
                    }


                    if (reqSubAccount.SubAccountProperties.DefaultRole.HasValue)
                    {
                        if (reqSubAccount.SubAccountProperties.DefaultRole.Value != 0)
                        {
                            List<int> lstAccessRoles = new List<int>();
                            lstAccessRoles.Add((int)reqSubAccount.SubAccountProperties.DefaultRole);
                                employee.GetAccessRoles().Add(lstAccessRoles, subAccountID, null);
                            }
                        }
                    }

                    #region Set Employee Item Role



                    #endregion

                    #region Set Employee Access Role


                    #endregion

                    #region Insert employee password key

                    if (employeeID > 0)
                    {
                        clsEmployees.AddPasswordKey(employeeID, cEmployees.PasswordKeyType.AdminRequest, DateTime.Now);
                    }

                    #endregion
                }

                #region Add/Assign Employee Home Address

                if (address1 != string.Empty && postcode != string.Empty)
                {
                    if (employeeID > 0)
                    {
                        int newAddressID;
                        this.lstHomeAddresses.TryGetValue(address1 + "," + postcode, out newAddressID);

                        // Get the home location to see if it already exists in expenses
                        if (newAddressID == 0)
                        {
                            newAddressID = Address.FindForEsr(nAccountID, address1, postcode);
                            
                            if (newAddressID > 0)
                            {
                                this.lstHomeAddresses.Add(address1 + "," + postcode, newAddressID);
                            }
                        }

                        if (newAddressID == 0)
                        {
                            cGlobalCountry globCountry = this.clsGlobalCountries.getCountryByName(homeCountry) ?? this.clsGlobalCountries.getGlobalCountryByAlphaCode(homeCountry);
                            int globalCountryId = (globCountry != null) ? globCountry.GlobalCountryId : 0;
                            
                            Address newAddress;

                            // Inform a subscribed user about postcodes that are invalid
                            if (!clsGlobalCountries.ValidatePostcode(globalCountryId, postcode))
                            {
                                this.CreateInvalidAddressTask(employeeID, true, postcode, firstname, lastname, employeeNum);
                            }

                            // todo - are we trying to match against postcodeanywhere?
                            string[] addressDetails = new string[0]; //this.clsComps.getAddress(homeCountry, postcode, countryID);

                            if (addressDetails.Length > 0)
                            {
                                newAddress = new Address
                                {
                                    Identifier = 0,
                                    Line1 = address1,
                                    Line2 = addressDetails[1],
                                    Line3 = string.Empty,
                                    City = addressDetails[2],
                                    County = addressDetails[3],
                                    Postcode = postcode,
                                    Country = globalCountryId,
                                    CreationMethod = Address.AddressCreationMethod.EsrOutbound,
                                    Longitude = string.Empty,
                                    Latitude = string.Empty,
                                    GlobalIdentifier = string.Empty,
                                    AccountWideFavourite = false
                                };
                            }
                            else
                            {
                                newAddress = new Address
                                {
                                    Identifier = 0,
                                    Line1 = address1,
                                    Line2 = string.Empty,
                                    Line3 = string.Empty,
                                    City = string.Empty,
                                    County = string.Empty,
                                    Postcode = postcode,
                                    Country = globalCountryId,
                                    CreationMethod = Address.AddressCreationMethod.EsrOutbound,
                                    Longitude = string.Empty,
                                    Latitude = string.Empty,
                                    GlobalIdentifier = string.Empty,
                                    AccountWideFavourite = false
                                };
                            }

                            newAddressID = newAddress.Save(new CurrentUser(nAccountID,employeeID, 0, Modules.ESR, subAccountID));

                            if (newAddressID > 0)
                            {
                                this.lstHomeAddresses.Add(address1 + "," + postcode, newAddressID);
                            }
                        }

                        if (newAddressID > 0)
                        {
                            DBConnection datacon = new DBConnection(cAccounts.getConnectionString(AccountID));
                            datacon.sqlexecute.Parameters.AddWithValue("@employeeID", employeeID);
                            datacon.sqlexecute.Parameters.AddWithValue("@addressID", newAddressID);
                            datacon.ExecuteProc("SaveHomeAddressFromEsrOutbound");
                            datacon.sqlexecute.Parameters.Clear();
                            new NotificationHelper(Employee.Get(employeeID, this.AccountID)).ExcessMileage();
                        }
                    }
                }
            }

            #endregion

            User.CacheRemove(employeeID, this.AccountID);
            Employee.CacheRemove(employeeID, this.AccountID);
            cESRAssignments.ResetCache(AccountID, employeeID);
            return employeeID;
        }

        /// <summary>
        /// Save the assignment record to the database
        /// </summary>
        /// <param name="data">string array of the data line</param>
        /// <param name="template">Import template object to use</param>
        /// <param name="employeeID">ID of the employee to associate the assignment to</param>
        /// <param name="lstLineManagersToUpdate"></param>
        /// <param name="lstLineManagersToCheck"></param>
        /// <param name="lstUserdefinedLineManagersToUpdate"></param>
        /// <param name="reqSubAccount">Subaccount being imported into</param>
        /// <param name="table">Table class object of table being imported into</param>
        /// <param name="curUser"></param>
        private void saveAssignment(string[] data, ref cImportTemplate template, int employeeID, ref SortedList<int, string> lstLineManagersToUpdate, ref List<int> lstLineManagersToCheck, ref Dictionary<int, Dictionary<int, string>> lstUserdefinedLineManagersToUpdate, ref cAccountSubAccount reqSubAccount, ref cTable table, ICurrentUserBase curUser)
        {
            if (employeeID == 0)
            {
                this.clsLogging.addLogItem(LogReasonType.Error, this.assignmentElement, "Line " + this.recordcount + ": Cannot save assignment record as an associated employee record could not be added");
                return;
            }

            cField field;
            cField tempField;
            object fieldValue;
            string dataVal;
            string AssignmentNum = string.Empty;
            string address1 = string.Empty;
            string postcode = string.Empty;
            string homeCountry = string.Empty;
            string assignmentLocation = string.Empty;
            bool primaryAssignment = false;
            int AssignmentID = 0;
            validationError = false;
            bool saveAssignmentRec = false;

            cUserdefinedFields userdefined = new cUserdefinedFields(this.AccountID);
            SortedList<int, object> udfValues = new SortedList<int, object>();

            DBConnection updateconn = new DBConnection(cAccounts.getConnectionString(this.AccountID));

            #region Assignment element Record with the assignment as the basetable

            int fieldcount = 1;

            // Update all assignment element records mapped to the assignment table
            foreach (cImportTemplateMapping mapping in template.Mappings)
            {
                if (mapping.ElementType != ImportElementType.Assignment)
                {
                    continue;
                }

                tempField = mapping.Field;
                if (tempField != null && (tempField.TableID == table.TableID || tempField.TableID == table.UserDefinedTableID))
                {
                    dataVal = data[mapping.ColRef].TrimStart('"').TrimEnd('"');

                    // Need to get the Assignment number to check if the assignment record already exists
                    if (mapping.DestinationField == "Assignment Number")
                    {
                        AssignmentNum = dataVal;

                        if (AssignmentNum == string.Empty)
                        {
                            this.clsLogging.addLogItem(LogReasonType.Error, this.assignmentElement, "Line " + this.recordcount + ": The Assignment Number is blank and will not be imported");
                            this.validationError = true;
                            break;
                        }
                    }

                    #region Fields for adding the work location

                    if (mapping.DestinationField == "Assignment Address 1st Line")
                    {
                        address1 = dataVal;
                    }

                    if (mapping.DestinationField == "Assignment Address Postcode")
                    {
                        postcode = dataVal;
                    }

                    if (mapping.DestinationField == "Assignment Address Country")
                    {
                        homeCountry = dataVal;
                    }

                    if (mapping.DestinationField == "Primary Assignment")
                    {
                        primaryAssignment = dataVal == "1";
                    }

                    if (mapping.DestinationField == "Assignment Location")
                    {
                        assignmentLocation = dataVal;
                    }

                    #endregion

                    fieldValue = this.parseDataTypeValue(dataVal, mapping, ref this.validationError);
                    field = mapping.Field;

                    if (field.FieldSource == cField.FieldSourceType.Userdefined)
                    {
                        int udfFieldId = 0;
                        if (!this.GetRelationshipIdByValue(field, ref fieldValue, ref curUser, out udfFieldId))
                        {
                            break;
                        }

                        if (field.FieldType == "S" && field.Length > 0)
                        {
                            fieldValue = this.CheckFieldValueLength(field, fieldValue.ToString());
                        }

                        if (!udfValues.ContainsKey(udfFieldId))
                        {
                            udfValues.Add(udfFieldId, fieldValue);
                            saveAssignmentRec = true;
                        }
                        continue;
                    }

                    if (fieldValue.ToString() != string.Empty)
                    {
                        // Trim the value if it exceeds the field length 
                        if (field.FieldType == "S")
                        {
                            if (field.Length > 0)
                            {
                                fieldValue = this.CheckFieldValueLength(field, fieldValue.ToString());
                            }

                            updateconn.AddWithValue("@" + fieldcount, fieldValue, field.Length);
                        }
                        else
                        {
                            updateconn.sqlexecute.Parameters.AddWithValue("@" + fieldcount, fieldValue);
                        }
                    }
                    else
                    {
                        updateconn.sqlexecute.Parameters.AddWithValue("@" + fieldcount, DBNull.Value);
                    }

                    saveAssignmentRec = true;
                    fieldcount++;
                }
            }

            // No Mappings for Assignment
            if (!saveAssignmentRec)
            {
                this.clsLogging.addLogItem(
                    LogReasonType.Warning, this.assignmentElement, "Line " + this.recordcount + ": The assignment record will not be imported as there are no assignment fields mapped on the import template");
            }

            if (!this.validationError && saveAssignmentRec)
            {
                #region Check existence of assignment record
                
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
                const string strsql = "getEsrAssignmentsCount";
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeID);
                expdata.sqlexecute.Parameters.AddWithValue("@assignmentnumber", AssignmentNum);
                int recCount = expdata.getcount(strsql, CommandType.StoredProcedure);
                expdata.sqlexecute.Parameters.Clear();

                #endregion

                // Update Assignment
                if (recCount > 0)
                {
                    updateconn.sqlexecute.Parameters.AddWithValue("@" + fieldcount, DateTime.UtcNow);
                    updateconn.sqlexecute.Parameters.AddWithValue("@filter1_0_0", employeeID);
                    updateconn.sqlexecute.Parameters.AddWithValue("@filter1_1_0", AssignmentNum);

                    if (employeeID > 0)
                    {
                        try
                        {
                            updateconn.ExecuteSQL(this.sUpdateSQLForAssignmentInsert);
                            updateconn.sqlexecute.Parameters.Clear();

                            // save any user defined data
                            if (udfValues.Count > 0)
                            {
                                userdefined.SaveValues(table.GetUserdefinedTable(), employeeID, udfValues, new cTables(this.AccountID), new cFields(this.AccountID), (cCurrentUserBase)curUser, skipFieldsNotOnPage: true);
                            }

                            User.CacheRemove(employeeID, this.AccountID);

                            this.clsLogging.addLogItem(
                                LogReasonType.SuccessUpdate, this.assignmentElement, "Line " + this.recordcount + ": Updated assignment record for employee with Assignment Number " + AssignmentNum);
                        }
                        catch (Exception ex)
                        {
                            this.clsLogging.addLogItem(LogReasonType.SQLError, this.assignmentElement, "Line " + this.recordcount + ": SQL Error {" + ex.Message + "}");
                            ErrorHandlerWeb clsessors = new ErrorHandlerWeb();
                            clsessors.SendAutomatedProcessError(ex);
                        }
                    }
                }
                else
                {
                    // Add assignment
                    try
                    {
                        updateconn.sqlexecute.Parameters.AddWithValue("@" + fieldcount, employeeID);
                        fieldcount++;
                        updateconn.sqlexecute.Parameters.AddWithValue("@" + fieldcount, DateTime.UtcNow);

                        updateconn.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                        updateconn.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;
                        updateconn.ExecuteSQL(this.sInsertSQLForAssignmentInsert);

                        AssignmentID = (int)updateconn.sqlexecute.Parameters["@identity"].Value;
                        updateconn.sqlexecute.Parameters.Clear();

                        if (AssignmentID > 0)
                        {
                            // save any user defined data
                            if (udfValues.Count > 0)
                            {
                                userdefined.SaveValues(table.GetUserdefinedTable(), employeeID, udfValues, new cTables(this.AccountID), new cFields(this.AccountID), (cCurrentUserBase)curUser, skipFieldsNotOnPage: true);
                            }

                            this.clsLogging.addLogItem(LogReasonType.SuccessAdd, this.assignmentElement, "Line " + this.recordcount + ": Added assignment record for employee with Assignment Number " + AssignmentNum);
                        }
                        else
                        {
                            this.clsLogging.addLogItem(LogReasonType.Error, this.assignmentElement, "Line " + this.recordcount + ": Failed to add assignment record");
                        }
                    }
                    catch (Exception ex)
                    {
                        this.clsLogging.addLogItem(LogReasonType.SQLError, this.assignmentElement, "Line " + this.recordcount + ": SQL Error {" + ex.Message + "}");

                        // Send email of the error
                        ErrorHandlerWeb clsessors = new ErrorHandlerWeb();
                        clsessors.SendAutomatedProcessError(ex);
                    }
                }

                #region Add/Assign Employee Office Address

                if (primaryAssignment)
                {
                    if (address1 != string.Empty && postcode != string.Empty)
                    {
                        if (employeeID > 0)
                        {
                            int newAddressID;
                            this.lstWorkAddresses.TryGetValue(address1 + "," + postcode, out newAddressID);

                            if (newAddressID == 0)
                            {
                                cGlobalCountry globCountry = this.clsGlobalCountries.getCountryByName(homeCountry) ?? this.clsGlobalCountries.getGlobalCountryByAlphaCode(homeCountry);
                                int globalCountryId = (globCountry != null) ? globCountry.GlobalCountryId : 0;

                                // Get the office location to see if it already exists in expenses
                                newAddressID = Address.FindForEsr(nAccountID, address1, postcode);

                                if (newAddressID == 0)
                                {
                                    // Inform a subscribed user about postcodes that are invalid
                                    if (!clsGlobalCountries.ValidatePostcode(globalCountryId, postcode))
                                    {
                                        this.CreateInvalidAddressTask(employeeID, false, postcode, string.Empty, string.Empty, AssignmentNum.ToString());
                                    }
                                    
                                    // todo - are we trying to match against postcodeanywhere?
                                    string[] addressDetails = new string[0]; // this.clsComps.getAddress(homeCountry, postcode, countryID);

                                    Address newAddress;

                                    if (addressDetails.Length > 0)
                                    {
                                        newAddress = new Address
                                        {
                                            Identifier = 0,
                                            Line1 = address1,
                                            Line2 = addressDetails[1],
                                            Line3 = string.Empty,
                                            City = addressDetails[2],
                                            County = addressDetails[3],
                                            Postcode = postcode,
                                            Country = globalCountryId,
                                            CreationMethod = Address.AddressCreationMethod.EsrOutbound,
                                            Longitude = string.Empty,
                                            Latitude = string.Empty,
                                            GlobalIdentifier = string.Empty,
                                            AccountWideFavourite = false
                                        };
                                    }
                                    else
                                    {
                                        newAddress = new Address
                                        {
                                            Identifier = 0,
                                            Line1 = address1,
                                            Line2 = string.Empty,
                                            Line3 = string.Empty,
                                            City = string.Empty,
                                            County = string.Empty,
                                            Postcode = postcode,
                                            Country = globalCountryId,
                                            CreationMethod = Address.AddressCreationMethod.EsrOutbound,
                                            Longitude = string.Empty,
                                            Latitude = string.Empty,
                                            GlobalIdentifier = string.Empty,
                                            AccountWideFavourite = false
                                        };
                                    }

                                    newAddressID = newAddress.Save(new CurrentUser(nAccountID, employeeID, 0, Modules.ESR, reqSubAccount.SubAccountID));
                                }

                                this.lstWorkAddresses.Add(address1 + "," + postcode, newAddressID);
                            }

                            if (newAddressID > 0)
                            {
                                DBConnection datacon = new DBConnection(cAccounts.getConnectionString(this.AccountID));
                                datacon.sqlexecute.Parameters.AddWithValue("@employeeID", employeeID);
                                datacon.sqlexecute.Parameters.AddWithValue("@addressID", newAddressID);
                                datacon.ExecuteProc("SaveWorkAddressFromEsrOutbound");
                                datacon.sqlexecute.Parameters.Clear();
                                new NotificationHelper(Employee.Get(employeeID, this.AccountID)).ExcessMileage();
                            }
                        }
                    }
                }

                #endregion
            }

            #endregion

            #region Assignment element record with employees as the base table

            // This is so that any assignment record fields that are being mapped to the employees table update the employee record
            cTables clsTables = new cTables(this.AccountID);

            this.validationError = false;
            bool updateEmployee = false;
            string supervisorAssignment = string.Empty;
            udfValues.Clear();

            fieldcount = 1;
            foreach (cImportTemplateMapping mapping in template.Mappings)
            {
                if (mapping.ElementType != ImportElementType.Assignment)
                {
                    continue;
                }

                tempField = mapping.Field;
                if (tempField != null && (tempField.TableID != this.emptable.TableID && tempField.TableID != this.emptable.UserDefinedTableID))
                {
                    continue;
                }

                dataVal = data[mapping.ColRef].TrimStart('"').TrimEnd('"');

                fieldValue = this.parseDataTypeValue(dataVal, mapping, ref this.validationError);

                if (mapping.DestinationField != "Supervisor Employee Number" && mapping.DestinationField != "Supervisor Assignment Number")
                {
                    if (tempField != null)
                    {
                        if (tempField.FieldSource == cField.FieldSourceType.Userdefined)
                        {
                            int udfFieldId = 0;
                            if (!this.GetRelationshipIdByValue(tempField, ref fieldValue, ref curUser, out udfFieldId))
                            {
                                break;
                            }

                            if (tempField.FieldType == "S" && tempField.Length > 0)
                            {
                                fieldValue = this.CheckFieldValueLength(tempField, fieldValue.ToString());
                            }

                            if (!udfValues.ContainsKey(udfFieldId))
                            {
                                udfValues.Add(udfFieldId, fieldValue);
                                updateEmployee = true;
                            }

                            continue;
                        }

                        if (fieldValue.ToString() != string.Empty)
                        {
                            // Trim the value if it exceeds the field length 
                            if (tempField.FieldType == "S" && tempField.Length > 0)
                            {
                                fieldValue = this.CheckFieldValueLength(tempField, fieldValue.ToString());
                            }

                            updateconn.sqlexecute.Parameters.AddWithValue("@" + fieldcount, fieldValue);
                        }
                        else
                        {
                            updateconn.sqlexecute.Parameters.AddWithValue("@" + fieldcount, DBNull.Value);
                        }

                        fieldcount++;
                    }
                }

                updateEmployee = true;
            }

            if (!this.validationError && updateEmployee)
            {
                ////cEmployees clsEmployees = new cEmployees(AccountID);
                ////cEmployee emp = clsEmployees.GetEmployeeById(employeeID);

                if (employeeID > 0)
                {
                    try
                    {
                        if (this.bHasEmployeeAssignmentField)
                        {
                            updateconn.sqlexecute.Parameters.AddWithValue("@filter1_0_0", employeeID);
                            updateconn.ExecuteSQL(this.sUpdateSQLForAssignmentEmployeeInsert);
                            updateconn.sqlexecute.Parameters.Clear();
                        }

                        // save any user defined data
                        if (udfValues.Count > 0)
                        {
                            userdefined.SaveValues(this.emptable.GetUserdefinedTable(), employeeID, udfValues, clsTables, new cFields(this.AccountID), (cCurrentUserBase)curUser, skipFieldsNotOnPage: true);
                        }

                        this.clsLogging.addLogItem(
                            LogReasonType.SuccessUpdate, this.assignmentElement, "Line " + this.recordcount + ": Updated employee from assignment element values with assignment number " + AssignmentNum);
                    }
                    catch (Exception ex)
                    {
                        this.clsLogging.addLogItem(LogReasonType.SQLError, this.assignmentElement, "Line " + this.recordcount + ": SQL Error {" + ex.Message + "}");

                        // Send email of the error
                        ErrorHandlerWeb clsessors = new ErrorHandlerWeb();
                        clsessors.SendAutomatedProcessError(ex);
                    }
                }
            }

            int LineManagerID = 0;
            bool IsUserdefinedLineManager = false;
            fieldValue = string.Empty;
            udfValues.Clear();

            foreach (cImportTemplateMapping mapping in template.Mappings)
            {
                if (mapping.DestinationField != "Supervisor Employee Number" && mapping.DestinationField != "Supervisor Assignment Number")
                {
                    continue;
                }

                if (!primaryAssignment)
                {
                    continue;
                }

                if (mapping.Field == null)
                {
                    continue;
                }

                IsUserdefinedLineManager = false;
                dataVal = data[mapping.ColRef].TrimStart('"').TrimEnd('"');

                fieldValue = this.parseDataTypeValue(dataVal, mapping, ref this.validationError);
                supervisorAssignment = fieldValue.ToString();

                if (supervisorAssignment == string.Empty)
                {
                    continue;
                }

                field = mapping.Field;

                cEmployees clsEmployees = new cEmployees(this.AccountID);
                LineManagerID = clsEmployees.getEmployeeidByAssignment(this.AccountID, supervisorAssignment);
                cUserDefinedField udfField = null;

                if (field.FieldSource == cField.FieldSourceType.Userdefined)
                {
                    IsUserdefinedLineManager = true;
                    udfField = userdefined.GetUserdefinedFieldByFieldID(field.FieldID);

                    if (udfField != null)
                    {
                        if (udfField.fieldtype == FieldType.Relationship && ((cManyToOneRelationship)udfField.attribute).relatedtable.TableID == this.emptable.TableID)
                        {
                            udfValues.Add(udfField.userdefineid, LineManagerID);
                            userdefined.SaveValues(udfField.table, employeeID, udfValues, clsTables, new cFields(this.AccountID), (cCurrentUserBase)curUser, skipFieldsNotOnPage: true);
                        }
                        else
                        {
                            this.clsLogging.addLogItem(
                                LogReasonType.Error,
                                this.assignmentElement,
                                "Line " + this.recordcount + ": Supervisor field is mapped to a userdefined field that is either not a relationship type field or is not related to employees.");
                        }
                    }
                    else
                    {
                        this.clsLogging.addLogItem(LogReasonType.Error, this.assignmentElement, "Line " + this.recordcount + ": Supervisor mapping is mapped to an unknown userdefined field");
                    }
                }

                if (LineManagerID > 0)
                {
                    fieldValue = LineManagerID;

                    if (!lstLineManagersToCheck.Contains(LineManagerID))
                    {
                        lstLineManagersToCheck.Add(LineManagerID);
                    }
                }
                else
                {
                    if (!IsUserdefinedLineManager)
                    {
                        if (!lstLineManagersToUpdate.ContainsKey(employeeID))
                        {
                            lstLineManagersToUpdate.Add(employeeID, supervisorAssignment);
                        }
                    }
                    else
                    {
                        if (udfField != null)
                        {
                            if (!lstUserdefinedLineManagersToUpdate.ContainsKey(udfField.userdefineid))
                            {
                                lstUserdefinedLineManagersToUpdate.Add(udfField.userdefineid, new Dictionary<int, string>());
                            }

                            if (!lstUserdefinedLineManagersToUpdate[udfField.userdefineid].ContainsKey(employeeID))
                            {
                                lstUserdefinedLineManagersToUpdate[udfField.userdefineid].Add(employeeID, supervisorAssignment);
                            }
                        }
                    }
                }
            }

            if (this.lineManagerField != null && LineManagerID > 0 && !IsUserdefinedLineManager)
            {
                string sql = "update employees set " + this.lineManagerField.FieldName + " = @linemanager where employeeid = @employeeid";
                updateconn.sqlexecute.Parameters.AddWithValue("@linemanager", fieldValue);
                updateconn.sqlexecute.Parameters.AddWithValue("@employeeid", employeeID);
                updateconn.ExecuteSQL(sql);
                updateconn.sqlexecute.Parameters.Clear();
            }

            #endregion
        }

        /// <summary>
        /// Method to parse each field value extracted from the ESR outbound file to the correct data type 
        /// </summary>
        /// <param name="dataVal">The value of the data to parse</param>
        /// <param name="mapping">The import template field mapping</param>
        /// <param name="validationError">Boolean value to reference if a validation error occurs</param>
        /// <returns>The data object value for the sql query</returns>
        private object parseDataTypeValue(string dataVal, cImportTemplateMapping mapping, ref bool validationError)
        {
            object fieldValue = "";

            if (dataVal != "")
            {

                switch (mapping.dataType)
                {
                    case DataType.stringVal:
                        fieldValue = dataVal;
                        break;

                    case DataType.decimalVal:
                        decimal dVal = 0;

                        if (decimal.TryParse(dataVal, out dVal))
                        {
                            fieldValue = dVal;
                        }
                        else
                        {
                            validationError = true;
                            clsLogging.addLogItem(
                                LogReasonType.Error,
                                null,
                                "Line " + recordcount + ": Cannot parse the decimal value for field " + mapping.DestinationField + " record type " + mapping.ElementType.ToString() + " Line not Imported.");
                        }
                        break;

                    case DataType.intVal:
                        int nVal = 0;

                        if (int.TryParse(dataVal, out nVal))
                        {
                            fieldValue = nVal;
                        }
                        else
                        {
                            validationError = true;
                            clsLogging.addLogItem(
                                LogReasonType.Error,
                                null,
                                "Line " + recordcount + ": Cannot parse the int value for field " + mapping.DestinationField + " record type " + mapping.ElementType.ToString() + " Line not Imported.");
                        }
                        break;

                    case DataType.doubleVal:
                        double dbVal = 0;

                        if (double.TryParse(dataVal, out dbVal))
                        {
                            fieldValue = dbVal;
                        }
                        else
                        {
                            validationError = true;
                            clsLogging.addLogItem(
                                LogReasonType.Error,
                                null,
                                "Line " + recordcount + ": Cannot parse the double value for field " + mapping.DestinationField + " record type " + mapping.ElementType.ToString() + " Line not Imported.");
                        }
                        break;

                    case DataType.floatVal:
                        float fVal = 0;

                        if (float.TryParse(dataVal, out fVal))
                        {
                            fieldValue = fVal;
                        }
                        else
                        {
                            validationError = true;
                            clsLogging.addLogItem(
                                LogReasonType.Error,
                                null,
                                "Line " + recordcount + ": Cannot parse the float value for field " + mapping.DestinationField + " record type " + mapping.ElementType.ToString() + " Line not Imported.");
                        }
                        break;

                    case DataType.dateVal:
                        DateTime dtVal = new DateTime(9999, 01, 01);

                        if (DateTime.TryParse(dataVal.Substring(0, 4) + "/" + dataVal.Substring(4, 2) + "/" + dataVal.Substring(6, 2), out dtVal))
                        {
                            fieldValue = dtVal;
                        }
                        else
                        {
                            validationError = true;
                            clsLogging.addLogItem(
                                LogReasonType.Error,
                                null,
                                "Line " + recordcount + ": Cannot parse the date value for field " + mapping.DestinationField + " record type " + mapping.ElementType.ToString() + " Line not Imported.");
                        }
                        break;

                    case DataType.booleanVal:
                        bool bVal = false;

                        if (dataVal == "1")
                        {
                            bVal = true;
                            fieldValue = bVal;
                            break;
                        }
                        else if (dataVal == "0")
                        {
                            bVal = false;
                            fieldValue = bVal;
                            break;
                        }

                        if (bool.TryParse(dataVal, out bVal))
                        {
                            fieldValue = bVal;
                        }
                        else
                        {
                            validationError = true;
                            clsLogging.addLogItem(
                                LogReasonType.Error,
                                null,
                                "Line " + recordcount + ": Cannot parse the boolean value for field " + mapping.DestinationField + " record type " + mapping.ElementType.ToString() + " Line not Imported.");
                        }
                        break;
                }
            }

            return fieldValue;
        }

        /// <summary>
        /// Parse the dynamicVal string to see if any of the columns referenced within it match the current column for the employee.
        /// If there is a match replace the column reference with the actual value and return the parsed string value 
        /// </summary>
        /// <param name="dynamicVal">Formatted string value</param>
        /// <param name="destinationField">The string value of the destination field</param>
        /// <param name="fieldValue">The value of the field to replace</param>
        /// <returns></returns>
        private string SetStringFormatValues(string dynamicVal, string destinationField, string fieldValue)
        {
            Regex regex = new Regex(@"\[(?<myMatch>[^\]]*)\]", RegexOptions.Compiled | RegexOptions.IgnoreCase); //@"\[(?<myMatch>([^\]]*))\]"
            MatchCollection matches = regex.Matches(dynamicVal);

            List<cField> lstMatchedFields = new List<cField>();
            string matchVal = "";
            string newDynamicValue = dynamicVal;

            foreach (Match match in matches)
            {
                matchVal = match.Groups["myMatch"].Value;
                if (destinationField == matchVal)
                {
                    newDynamicValue = newDynamicValue.Replace("[" + matchVal + "]", fieldValue);
                    break;
                }
            }


            return newDynamicValue;
        }

        /// <summary>
        /// Check that the field value does not exceed the length allowed for the field in the database. 
        /// If the value does exceed then it is trimmed to the specific length
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        private string CheckFieldValueLength(cField field, string fieldValue)
        {
            string val = fieldValue;

            if (fieldValue.ToString().Length > field.Length)
            {
                val = fieldValue.Substring(0, field.Length);
            }

            return val;
        }

        /// <summary>
        /// Create a task to inform a subscribed user that the home address could not be added due to an invalid postcode
        /// </summary>
        /// <param name="reqEmp"></param>
        /// <param name="isHome"></param>
        /// <param name="postcode"></param>
        private void CreateInvalidAddressTask(int employeeID, bool isHome, string postcode, string firstname, string surname, string assignment)
        {
            cAccountSubAccounts subaccs = new cAccountSubAccounts(AccountID);
            cAccountSubAccount subaccount = subaccs.getFirstSubAccount(); // only got a single subaccount until fully implemented
            Notifications notifications = new Notifications(AccountID);
            cTasks tasks = new cTasks(AccountID, subaccount.SubAccountID);

            List<object[]> tellWho = notifications.GetNotificationSubscriptions(EmailNotificationType.ESROutboundInvalidPostcodes);
            TaskCommand tskCmd = TaskCommand.Standard;
            string taskSubject = string.Empty;
            string taskText = string.Empty;

            CurrentUser currentUser = cMisc.GetCurrentUser();
            Modules currentModule = (currentUser != null) ? currentUser.CurrentActiveModule : Modules.expenses;
            cModules clsModules = new cModules();
            cModule clsModule = clsModules.GetModuleByID((int)currentModule);
            string brandName = (clsModule != null) ? clsModule.BrandNamePlainText : "Expenses";

            if (isHome)
            {
                taskSubject = "ESR Outbound Employee Home Postcode Invalid";
                tskCmd = TaskCommand.ESR_HomeLocationPostcodeInvalid;
                taskText = "The 'Home' address postcode '" + postcode + "', for employee " + firstname + " " + surname + " with assignment number " + assignment
                           + " is not in a valid format in ESR and could not be added to " + brandName;
            }
            else
            {
                taskSubject = "ESR Outbound Employee Work Postcode Invalid";
                tskCmd = TaskCommand.ESR_WorkLocationPostcodeInvalid;
                taskText = "The 'Work' address postcode '" + postcode + "', for employee " + firstname + " " + surname + " with assignment number " + assignment
                           + " is not in a valid format in ESR and could not be added to " + brandName;
            }

            int idOfNotify;
            foreach (object[] o in tellWho)
            {
                idOfNotify = (int)o[1];
                if ((sendType)o[0] == sendType.employee)
                {
                    cTaskOwner taskOwner = new cTaskOwner(idOfNotify, sendType.employee, null);
                    cTask empTask = new cTask(
                        0, null, tskCmd, employeeID, DateTime.Now, null, employeeID, AppliesTo.Employee, taskSubject, taskText, DateTime.Now, null, null, TaskStatus.InProgress, taskOwner, false, null);

                    if (!tasks.taskExists(employeeID, AppliesTo.Employee, tskCmd, taskOwner.OwnerId, taskOwner.OwnerType, true))
                    {
                        tasks.AddTask(empTask, employeeID);
                    }
                }
                else if ((sendType)o[0] == sendType.team)
                {
                    cTeams clsTeams = new cTeams(AccountID);
                    cTeam notifyTeam = clsTeams.GetTeamById(idOfNotify);
                    cTaskOwner taskOwner = new cTaskOwner(idOfNotify, sendType.team, notifyTeam);
                    cTask teamTask = new cTask(
                        0,
                        null,
                        TaskCommand.ESR_RecordActivateManual,
                        employeeID,
                        DateTime.Now,
                        null,
                        employeeID,
                        AppliesTo.Employee,
                        taskSubject,
                        taskText,
                        DateTime.Now,
                        null,
                        null,
                        TaskStatus.InProgress,
                        taskOwner,
                        false,
                        null);

                    if (!tasks.taskExists(employeeID, AppliesTo.Employee, tskCmd, taskOwner.OwnerId, taskOwner.OwnerType, true))
                    {
                        tasks.AddTask(teamTask, employeeID);
                    }
                }
            }
        }

        /// <summary>
        /// Performs a lookup of an ID for a field value and adds it to the udfValue collection
        /// </summary>
        /// <param name="field">Mapping field</param>
        /// <param name="fieldValue">Value from the import file to look up and find the ID</param>
        /// <param name="curUser">ICurrentUserBase object</param>
        /// <param name="udfId">User defined ID of field</param>
        /// <returns></returns>
        private bool GetRelationshipIdByValue(cField field, ref object fieldValue, ref ICurrentUserBase curUser, out int udfId)
        {
            cUserdefinedFields userdefined = new cUserdefinedFields(this.AccountID);

            cUserDefinedField udfField = userdefined.GetUserdefinedFieldByFieldID(field.FieldID);
            if (udfField == null)
            {
                udfId = 0;
                return false;
            }

            udfId = udfField.userdefineid;

            if (field.IsForeignKey && field.GetRelatedTable() != null)
            {
                // if is a relationship field, then need to lookup ID for value being saved
                cManyToOneRelationship relField = (cManyToOneRelationship)udfField.attribute;

                List<sAutoCompleteResult> val = AutoComplete.GetAutoCompleteMatches(
                    curUser,
                    2,
                    field.RelatedTableID.ToString(),
                    relField.AutoCompleteDisplayField.ToString(),
                    string.Join(",", Array.ConvertAll(relField.AutoCompleteMatchFieldIDList.ToArray(), x => x.ToString())),
                    fieldValue.ToString(),
                    false,
                    null);

                // Lookup must be unique, so should only return a single value. If two values returned, can't save
                if (val.Count == 1)
                {
                    fieldValue = val[0].value;
                }
                else
                {
                    this.clsLogging.addLogItem(
                        LogReasonType.Error, this.personElement, "Line " + this.recordcount + ": Relationship field lookup failed for value " + fieldValue + " in table [" + field.GetRelatedTable().TableName + "]");
                    return false;
                }
            }

            return true;
        }

        private cField UDFOverrideField(cImportTemplateMapping mapping, ref cFields clsFields)
        {
            cField retField = null;

            switch (mapping.DestinationField)
            {
                case "First Name":
                    retField = clsFields.GetFieldByID(new Guid("6614ACAD-0A43-4E30-90EC-84DE0792B1D6"));
                    break;
                case "Last Name":
                    retField = clsFields.GetFieldByID(new Guid("9D70D151-5905-4A67-944F-1AD6D22CD931"));
                    break;
            }

            return retField;
        }
    }

    //[Serializable()]
    //public class cESRPerson
    //{
    //    private string sRecordType;
    //    private int nPersonID;
    //    private string sEmployeeNumber;
    //    private string sTitle;
    //    private string sLastName;
    //    private string sFirstName;
    //    private string sMiddleNames;
    //    private string sMaidenName;
    //    private string sGender;
    //    private string sDateOfBirth;
    //    private string sNationalInsuranceNumber;
    //    private string sHireDate;
    //    private string sTerminationDate;
    //    private string sCurrentEmployeeStatusFlag;
    //    private string sAddressLine1;
    //    private string sAddressLine2;
    //    private string sAddressTown;
    //    private string sAddressCounty;
    //    private string sAddressPostcode;
    //    private string sAddressCountry;
    //    private string sPersonalEmail;
    //    private string sHomeTelephoneNumber;
    //    private string sHomeFaxNumber;
    //    private string sMobileNumber;
    //    private string sOfficeEmail;
    //    private string sPagerNumber;
    //    private string sWorkNumber;
    //    private string sWTROptOut;
    //    private string sApplicantNumber;
    //    private string sApplicantActiveStatusFlag;
    //    private List<cESRAssignment> lstAssignments = new List<cESRAssignment>();

    //    public cESRPerson(string recordtype, int personid, string employeenumber, string title, string lastname, string firstname, string middlenames, string maidenname, string gender, string dateofbirth, string nationalinsurancenumber, string hiredate, string terminationdate, string currentemployeestatusflag, string addressline1, string addressline2, string addresstown, string addresscounty, string addresspostcode, string addresscountry, string personalemail, string hometelephonenumber, string homefaxnumber, string mobilenumber, string officeemail, string pagernumber, string worknumber, string wtroptout, string applicantnumber, string applicantactivestatusflag)
    //    {
    //        sRecordType = recordtype;
    //        nPersonID = personid;
    //        sEmployeeNumber = employeenumber;
    //        sTitle = title;
    //        sLastName = lastname;
    //        sFirstName = firstname;
    //        sMiddleNames = middlenames;
    //        sMaidenName = maidenname;
    //        sGender = gender;
    //        sDateOfBirth = dateofbirth;
    //        sNationalInsuranceNumber = nationalinsurancenumber;
    //        sHireDate = hiredate;
    //        sTerminationDate = terminationdate;
    //        sCurrentEmployeeStatusFlag = currentemployeestatusflag;
    //        sAddressLine1 = addressline1;
    //        sAddressLine2 = addressline2;
    //        sAddressTown = addresstown;
    //        sAddressCounty = addresscountry;
    //        sAddressPostcode = addresspostcode;
    //        sAddressCountry = addresscountry;
    //        sPersonalEmail = personalemail;
    //        sHomeTelephoneNumber = hometelephonenumber;
    //        sHomeFaxNumber = homefaxnumber;
    //        sMobileNumber = mobilenumber;
    //        sOfficeEmail = officeemail;
    //        sPagerNumber = pagernumber;
    //        sWorkNumber = worknumber;
    //        sWTROptOut = wtroptout;
    //        sApplicantNumber = applicantnumber;
    //        sApplicantActiveStatusFlag = applicantactivestatusflag;
    //    }

    //    public void addAssignment(cESRAssignment assignment)
    //    {
    //        lstAssignments.Add(assignment);
    //    }

    //    #region properties
    //    public List<cESRAssignment> assignments
    //    {
    //        get { return lstAssignments; }
    //    }
    //    public string recordtype
    //    {
    //        get { return sRecordType; }
    //    }
    //    public int personid
    //    {
    //        get { return nPersonID; }
    //    }
    //    public string employeenumber
    //    {
    //        get { return sEmployeeNumber; }
    //    }
    //    public string title
    //    {
    //        get { return sTitle; }
    //    }
    //    public string firstname
    //    {
    //        get { return sFirstName; }
    //    }
    //    public string lastname
    //    {
    //        get { return sLastName; }
    //    }
    //    public string middlenames
    //    {
    //        get { return sMiddleNames; }
    //    }
    //    public string maidenname
    //    {
    //        get { return sMaidenName; }
    //    }
    //    public string gender
    //    {
    //        get { return sGender; }
    //    }
    //    public string dateofbirth
    //    {
    //        get { return sDateOfBirth; }
    //    }
    //    public string nationalinsurancenumber
    //    {
    //        get { return sNationalInsuranceNumber; }
    //    }
    //    public string hiredate
    //    {
    //        get { return sHireDate; }
    //    }
    //    public string terminationdate
    //    {
    //        get { return sTerminationDate; }
    //    }
    //    public string currentemployeestatusflag
    //    {
    //        get { return sCurrentEmployeeStatusFlag; }
    //    }
    //    public string addressline1
    //    {
    //        get { return sAddressLine1; }
    //    }
    //    public string addressline2
    //    {
    //        get { return sAddressLine2; }
    //    }
    //    public string addresstown
    //    {
    //        get { return sAddressTown; }
    //    }
    //    public string addresscounty
    //    {
    //        get { return sAddressCounty; }
    //    }
    //    public string addresspostcode
    //    {
    //        get { return sAddressPostcode; }
    //    }
    //    public string addresscountry
    //    {
    //        get { return sAddressCountry; }
    //    }
    //    public string personalemail
    //    {
    //        get { return sPersonalEmail; }
    //    }
    //    public string hometelephonenumber
    //    {
    //        get { return sHomeTelephoneNumber; }
    //    }
    //    public string homefaxnumber
    //    {
    //        get { return sHomeFaxNumber; }
    //    }
    //    public string mobilenumber
    //    {
    //        get { return sMobileNumber; }
    //    }
    //    public string officeemail
    //    {
    //        get { return sOfficeEmail; }
    //    }
    //    public string pagernumber
    //    {
    //        get { return sPagerNumber; }
    //    }
    //    public string worknumber
    //    {
    //        get { return sWorkNumber; }
    //    }
    //    public string wtroptout
    //    {
    //        get { return sWTROptOut; }
    //    }
    //    public string applicantnumber
    //    {
    //        get { return sApplicantNumber; }
    //    }
    //    public string applicantactivestatusflag
    //    {
    //        get { return sApplicantActiveStatusFlag; }
    //    }
    //    #endregion
    //}

    //[Serializable()]
    //public class cESRAssignment
    //{
    //    private string sRecordtype;
    //    private int nAssignmentID;
    //    private string sAssignmentNumber;
    //    private string sEarliestAssignmentStartDate;
    //    private string sFinalAssigmentEndDate;
    //    private string sAssignmentStatus;
    //    private string sPayrollPayType;
    //    private string sPayrollName;
    //    private string sPayrollPeriodType;
    //    private string sAssignmentAddressLine1;
    //    private string sAssignmentAddressLine2;
    //    private string sAssignmentAddressTown;
    //    private string sAssignmentAddressCounty;
    //    private string sAssignmentAddressPostcode;
    //    private string sAssignmentAdressCountry;
    //    private string sSupervisorFlag;
    //    private string sSupervisorAssignmentNumber;
    //    private string sSupervisorEmployeeNumber;
    //    private string sSupervisorFullName;
    //    private string sAccrualPlan;
    //    private string sEmployeeCategory;
    //    private string sAssignmentCategory;
    //    private bool bPrimaryAssignment;
    //    private decimal dNormalHours;
    //    private string sNormalHoursFrequency;
    //    private decimal dGradeContractHours;
    //    private decimal dNoOfSessions;
    //    private string sSessionsFrequency;
    //    private string sWorkPatternDetails;
    //    private string sWorkPatternStartDay;
    //    private string sFlexibleWorkingPattern;
    //    private string sAvailabilitySchedule;
    //    private string sOrganisation;
    //    private string sLegalEntity;
    //    private string sPositionName;
    //    private string sJobRole;
    //    private string sOccupationCode;
    //    private string sAssignmentLocation;
    //    private string sGrade;
    //    private string sJobName;
    //    private string sGroup;
    //    private string sTAndAFlag;
    //    private string sNightWorkerOptOut;
    //    private string sProjectedHireDate;
    //    private decimal dVacancyID;

    //    public cESRAssignment(string recordtype, int assignmentid, string assignmentnumber, string earliestassignementstartdate, string finalassignmentenddate, string assignmentstatus, string payrollpaytype, string payrollname, string payrollperiodtype, string assignmentaddressline1, string assignmentaddressline2, string assignmentaddresstown, string assignmentaddresscounty, string assignmentaddresspostcode, string assignmentaddresscountry, string supervisorflag, string supervisorassignmentnumber, string supervisoremployeenumber, string supervisorfullname, string accrualplan, string employeecategory, string assignmentcategory, bool primaryassignment, decimal normalhours, string normalhoursfrequency, decimal gradecontracthours, decimal numberofsessions, string sessionsfrequency, string workpatterndetails, string workpatterstartday, string flexibleworkingpattern, string availabilityschedule, string organisation, string legalentity, string positionname, string jobrole, string occupationcode, string assignmentlocation, string grade, string jobname, string group, string tandaflag, string nightworkeroptout, string projectedhiredate, decimal vacancyid)
    //    {
    //        sRecordtype = recordtype;
    //        nAssignmentID = assignmentid;
    //        sAssignmentNumber = assignmentnumber;
    //        sEarliestAssignmentStartDate = earliestassignementstartdate;
    //        sFinalAssigmentEndDate = finalassignmentenddate;
    //        sAssignmentStatus = assignmentstatus;
    //        sPayrollPayType = payrollpaytype;
    //        sPayrollName = payrollname;
    //        sPayrollPeriodType = payrollperiodtype;
    //        sAssignmentAddressLine1 = assignmentaddressline1;
    //        sAssignmentAddressLine2 = assignmentaddressline2;
    //        sAssignmentAddressTown = assignmentaddresstown;
    //        sAssignmentAddressCounty = assignmentaddresscounty;
    //        sAssignmentAddressPostcode = assignmentaddresspostcode;
    //        sAssignmentAdressCountry = assignmentaddresscountry;
    //        sSupervisorFlag = supervisorflag;
    //        sSupervisorAssignmentNumber = supervisorassignmentnumber;
    //        sSupervisorEmployeeNumber = supervisoremployeenumber;
    //        sSupervisorFullName = supervisorfullname;
    //        sAccrualPlan = accrualplan;
    //        sEmployeeCategory = employeecategory;
    //        sAssignmentCategory = assignmentcategory;
    //        bPrimaryAssignment = primaryassignment;
    //        dNormalHours = normalhours;
    //        sNormalHoursFrequency = normalhoursfrequency;
    //        dGradeContractHours = gradecontracthours;
    //        dNoOfSessions = numberofsessions;
    //        sSessionsFrequency = sessionsfrequency;
    //        sWorkPatternDetails = workpatterndetails;
    //        sWorkPatternStartDay = workpatterstartday;
    //        sFlexibleWorkingPattern = flexibleworkingpattern;
    //        sAvailabilitySchedule = availabilityschedule;
    //        sOrganisation = organisation;
    //        sLegalEntity = legalentity;
    //        sPositionName = positionname;
    //        sJobRole = jobrole;
    //        sOccupationCode = occupationcode;
    //        sAssignmentLocation = assignmentlocation;
    //        sGrade = grade;
    //        sJobName = jobname;
    //        sGroup = group;
    //        sTAndAFlag = tandaflag;
    //        sNightWorkerOptOut = nightworkeroptout;
    //        sProjectedHireDate = projectedhiredate;
    //        dVacancyID = vacancyid;
    //    }

    //    #region properties
    //    public string recordtype
    //    {
    //        get { return sRecordtype; }
    //    }
    //    public int assignmentid
    //    {
    //        get { return nAssignmentID; }
    //    }
    //    public string assignmentnumber
    //    {
    //        get { return sAssignmentNumber; }
    //    }
    //    public string earliestassignmentstartdate
    //    {
    //        get { return sEarliestAssignmentStartDate; }
    //    }
    //    public string finalassignmentenddate
    //    {
    //        get { return sFinalAssigmentEndDate; }
    //    }
    //    public string assignmentstatus
    //    {
    //        get { return sAssignmentStatus; }
    //    }
    //    public string payrollpaytype
    //    {
    //        get { return sPayrollPayType; }
    //    }
    //    public string payrollname
    //    {
    //        get { return sPayrollName; }
    //    }
    //    public string payrollperiodtype
    //    {
    //        get { return sPayrollPeriodType; }
    //    }
    //    public string assignmentaddress1
    //    {
    //        get { return sAssignmentAddressLine1; }
    //    }
    //    public string assignmentaddress2
    //    {
    //        get { return sAssignmentAddressLine2; }
    //    }
    //    public string assignmentaddresstown
    //    {
    //        get { return sAssignmentAddressTown; }
    //    }
    //    public string assignmentaddresscounty
    //    {
    //        get { return sAssignmentAddressCounty; }
    //    }
    //    public string assignmentaddresspostcode
    //    {
    //        get { return sAssignmentAddressPostcode; }
    //    }
    //    public string assignmentaddresscountry
    //    {
    //        get { return sAssignmentAdressCountry; }
    //    }
    //    public string supervisorflag
    //    {
    //        get { return sSupervisorFlag; }
    //    }
    //    public string supervisorassignmentnumber
    //    {
    //        get { return sSupervisorAssignmentNumber; }
    //    }
    //    public string supervisoremployementnumber
    //    {
    //        get { return sSupervisorEmployeeNumber; }
    //    }
    //    public string supervisorfullname
    //    {
    //        get { return sSupervisorFullName; }
    //    }
    //    public string accrualplan
    //    {
    //        get { return sAccrualPlan; }
    //    }
    //    public string employeecategory
    //    {
    //        get { return sEmployeeCategory; }
    //    }
    //    public string assignmentcategory
    //    {
    //        get { return sAssignmentCategory; }
    //    }
    //    public bool primaryassignment
    //    {
    //        get { return bPrimaryAssignment; }
    //    }
    //    public decimal normalhours
    //    {
    //        get { return dNormalHours; }
    //    }
    //    public string normalhoursfrequency
    //    {
    //        get { return sNormalHoursFrequency; }
    //    }
    //    public decimal gradecontracthours
    //    {
    //        get { return dGradeContractHours; }
    //    }
    //    public decimal noofsessions
    //    {
    //        get { return dNoOfSessions; }
    //    }
    //    public string sessionsfrequency
    //    {
    //        get { return sSessionsFrequency; }
    //    }
    //    public string workpatterndetails
    //    {
    //        get { return sWorkPatternDetails; }
    //    }
    //    public string workpatternstartday
    //    {
    //        get { return sWorkPatternStartDay; }
    //    }
    //    public string flexibleworkingpattern
    //    {
    //        get { return sFlexibleWorkingPattern; }
    //    }
    //    public string availabilityschedule
    //    {
    //        get { return sAvailabilitySchedule; }
    //    }
    //    public string organisation
    //    {
    //        get { return sOrganisation; }
    //    }
    //    public string legalentity
    //    {
    //        get { return sLegalEntity; }
    //    }
    //    public string positionname
    //    {
    //        get { return sPositionName; }
    //    }
    //    public string jobrole
    //    {
    //        get { return sJobRole; }
    //    }
    //    public string occupationcode
    //    {
    //        get { return sOccupationCode; }
    //    }
    //    public string assignmentlocation
    //    {
    //        get { return sAssignmentLocation; }
    //    }
    //    public string grade
    //    {
    //        get { return sGrade; }
    //    }
    //    public string jobname
    //    {
    //        get { return sJobName; }
    //    }
    //    public string group
    //    {
    //        get { return sGroup; }
    //    }
    //    public string tandaflag
    //    {
    //        get { return sTAndAFlag; }
    //    }
    //    public string nightworkeroptout
    //    {
    //        get { return sNightWorkerOptOut; }
    //    }
    //    public string projectedhiredate
    //    {
    //        get { return sProjectedHireDate; }
    //    }
    //    public decimal vacancyid
    //    {
    //        get { return dVacancyID; }
    //    }
    //    #endregion
    //}
}
