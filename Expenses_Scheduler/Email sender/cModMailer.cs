using System;
using System.Collections.Generic;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;
using System.Net.Mail;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using System.Data;
using System.Collections;


namespace Expenses_Scheduler
{
    using System.Globalization;

    public enum csvDB_Fields
    {
        RowStatus = 0,
        FriendlyName = 1,
        TableName = 2,
        FieldName = 3,
        JoinsReq = 4,
        FieldAlias = 5,
    }
    public class cModMailer
    {
        private int _currentRunLocationId;
        cEmailLog _clslog;
        private cAccount cAccount;

        public cModMailer(cAccount account, ref cEmailLog log)
        {
            cAccount = account;
            _clslog = log;
        }
        public cAccount Account
        {
            get { return cAccount; }
        }
        
        public int RunLocationId
        {
            get { return _currentRunLocationId; }
            set
            {
                _currentRunLocationId = value;
            }
        }
        
        public void DespatchEmail(string sendTo, string msgBody, string msgSubject)
        {
            cAccounts accs = new cAccounts();
            cAccountSubAccounts subaccs = new cAccountSubAccounts(Account.accountid);
            cAccountProperties properties = subaccs.getSubAccountById(RunLocationId).SubAccountProperties;

            if (string.IsNullOrEmpty(sendTo))
            {
                sendTo = properties.EmailAdministrator;
            }

            _clslog.AddToLog("Emailing '" + sendTo + "'", Account);

            string emailSig = "Framework Contract Management" + "\r\n" + "Copyright Selenity Ltd 2003 - "+ DateTime.Now.Year + "\r\n" + "support@selenity.com" + "\r\n" + "Tel: 01522 881300 || Fax: 01522 881355";
            MailMessage msg = new MailMessage(properties.EmailServerFromAddress, sendTo, msgSubject.Replace("\r\n", ""), msgBody + "\r\n\r\n" + emailSig);

            SmtpClient msender = new SmtpClient(properties.EmailServerAddress);

            msender.Send(msg);
            _clslog.AddToLog("Send request completed.", Account);

            msg.Dispose();
            msender = null;
            msg = null;
        }

        private string LoadMailTemplate(int tplId, string tplName)
        {
            try
            {
                _clslog.AddToLog("Loading Mail Template '" + tplName + "'", Account);
                string tPath = ConfigurationManager.AppSettings["templatePath"];
                tplName = tplName.Replace("./", "").Replace("/", "\\");
                EmailTemplates y = new EmailTemplates();
                string combinedPath = Path.Combine(tPath, tplName);
                NameValueCollection res = y.ReadTemplate(combinedPath);

                string templateBody = res["templateBody"];

                _clslog.AddToLog("Template loaded.", Account);

                return templateBody;
            }
            catch (Exception ex)
            {
                _clslog.AddToLog("Template " + tplName + " failed to load", Account);
                _clslog.AddToLog(ex.Message, Account);

                return "";
            }
        }

        private List<string> GetFieldsFromTemplate(string tmpCont)
        {
            _clslog.AddToLog("Parsing Template (Fields)...", Account);
            bool stopStack = false;

            List<string> strOut = new List<string>();
            try
            {
                while (stopStack == false)
                {
                    int intStart = tmpCont.IndexOf("[*");

                    if (intStart > -1)
                    {
                        tmpCont = tmpCont.Substring(intStart, tmpCont.Length - intStart);
                        intStart = tmpCont.IndexOf("*]");

                        strOut.Add(tmpCont.Substring(0, intStart + 2));
                        tmpCont = tmpCont.Substring(intStart + 2, tmpCont.Length - intStart - 2);
                    }
                    else
                    {
                        stopStack = true;
                    }
                }
                _clslog.AddToLog("Template Parsed, " + strOut.Count + " fields.", Account);
            }
            catch (Exception ex)
            {
                _clslog.AddToLog("FAIL - Failed to get fields from templates. " + ex.Message, Account);
                strOut[0] = "";
            }

            return strOut;
        }

        private string[] GetConditionsFromTemplate(string tmpCont)
        {
            _clslog.AddToLog("Parsing Template (Conditions)...", Account);
            bool stopStack = false;
            string[] strOut = new string[0];

            while (!stopStack)
            {
                int intStart = tmpCont.IndexOf("[-");
                if (intStart > -1)
                {
                    tmpCont = tmpCont.Substring(intStart, tmpCont.Length - 1);
                    intStart = tmpCont.IndexOf("-]");

                    strOut[strOut.Length - 1] = tmpCont.Substring(1, intStart + 1);

                    tmpCont = tmpCont.Substring(intStart + 2, tmpCont.Length - 1 - intStart - 2);
                }
                else
                {
                    stopStack = true;
                }
            }
            _clslog.AddToLog("Template Parsed, " + strOut.Length + " conditions.", Account);
            return strOut;
        }

        public void OverdueInvoiceMail(DataRow cDetails, DataSet recplist, string template)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(Account.accountid));
            cAccountSubAccounts subaccs = new cAccountSubAccounts(Account.accountid);
            cAccountProperties properties = subaccs.getSubAccountById(RunLocationId).SubAccountProperties;
            const string brandName = "Framework";

            _clslog.AddToLog("Handling overdue invoice.", Account);

            string tpl = LoadMailTemplate((int)emailType.OverdueInvoice, template);

            List<string> fields = GetFieldsFromTemplate(tpl);

            cModBuildSQL modBuildSQL = new cModBuildSQL(ref _clslog);
            string sql = modBuildSQL.ConstructQuery(fields, "contract_forecastdetails", "ContractForecastId", cDetails["contractForecastId"].ToString(), "[contract_details].[contractId],");

            DataSet ds = db.GetDataSet(sql);

            tpl = PopulateFields(tpl, fields, ds.Tables[0].Rows[0], "");
            if(tpl == "") return;

            if (recplist == null || recplist.Tables[0].Rows.Count == 0)
            {
                DespatchEmail(properties.EmailAdministrator, tpl, brandName + ": Invoice Overdue (Missing notification employee)");
            }
            else
            {
                foreach (DataRow dRow in recplist.Tables[0].Rows)
                {
                    DespatchEmail(((string)dRow["Email"]).Trim(), tpl, brandName + ": Invoice Overdue");
                }
            }
        }

        public void LicenceExpiryMail(DataRow pDetails, string template)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(Account.accountid));
            cEmployees emps = new cEmployees(Account.accountid);
            cTeams teams = new cTeams(Account.accountid, RunLocationId);

            const string brandName = "Framework";

            _clslog.AddToLog("Handling Licence Expiry.", Account);

            string tpl = LoadMailTemplate((int)emailType.LicenceExpiry, template.Replace("/", "\\"));
            if (tpl != "")
            {
                List<string> fields = GetFieldsFromTemplate(tpl);

                cModBuildSQL modBuildSQL = new cModBuildSQL(ref _clslog);
                string sql = modBuildSQL.ConstructQuery(fields, "productLicences", "LicenceId", pDetails["LicenceId"].ToString(), "[productDetails].[subAccountId],productLicences.[NotifyType] AS [NotifyType],productLicences.[NotifyId] AS [Recipient], ");


                DataSet ds = db.GetDataSet(sql);
                if (ds != null && ds.Tables.Count > 0)
                {
                    tpl = PopulateFields(tpl, fields, ds.Tables[0].Rows[0], "");
                    string emailAddress = "";

                    if (ds.Tables[0].Rows.Count > 0 && (ds.Tables[0].Rows[0]["NotifyType"] != null && ds.Tables[0].Rows[0]["Recipient"] != null) && (int)ds.Tables[0].Rows[0]["Recipient"] != 0)
                    {
                        switch ((AudienceType)ds.Tables[0].Rows[0]["NotifyType"])
                        {
                            case AudienceType.Individual:
                                if (emps.GetEmployeeById((int)ds.Tables[0].Rows[0]["Recipient"]) != null)
                                {
                                    emailAddress = emps.GetEmployeeById((int)ds.Tables[0].Rows[0]["Recipient"]).EmailAddress.Trim();
                                }
                                break;
                            case AudienceType.Team:
                                cTeam team = teams.GetTeamById((int)ds.Tables[0].Rows[0]["Recipient"]);
                                string separator = "";
                                foreach (int empId in team.teammembers)
                                {
                                    string emailAddr = emps.GetEmployeeById(empId).EmailAddress;
                                    if (!string.IsNullOrEmpty(emailAddr.Trim()))
                                    {
                                        emailAddress = separator + emailAddr;
                                        separator = ";";
                                    }
                                }
                                break;
                        }

                        DespatchEmail(emailAddress, tpl, brandName + ": Licence Expiry");
                    }
                    else
                    {
                        DespatchEmail("", tpl, brandName + ": Licence Expiry (Missing notification employee(s)");
                    }
                }
            }
        }

        public void ContractReviewMail(DataRow cDetails, DataSet recplist, string template, ref SortedList invalidRecipients, ref int numBlankNotifies)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(Account.accountid));
            cAccountSubAccounts subaccs = new cAccountSubAccounts(Account.accountid);
            cAccountProperties properties = subaccs.getSubAccountById(RunLocationId).SubAccountProperties;
            const string brandName = "Framework";

            string tpl = LoadMailTemplate((int)emailType.ContractReview, template);

            if(tpl == "") return;

            List<string> fields = GetFieldsFromTemplate(tpl);

            int cid = (int)cDetails["ContractId"];

            MaintParams mParams = SMRoutines.GetMaintParams(ref db, cid);
            string strNYM = GetProducts(mParams, cid);
            cModBuildSQL modBuildSQL = new cModBuildSQL(ref _clslog);
            string sql = modBuildSQL.ConstructQuery(fields, "contract_details", "ContractId", cid.ToString(), "");

            DataSet ds = db.GetDataSet(sql);

            string toGo = PopulateFields(tpl, fields, ds.Tables[0].Rows[0], strNYM);

            ArrayList curEmailList = new ArrayList();

            if (recplist.Tables[0].Rows.Count == 0)
            {
                // no staff specified for contract notification
                numBlankNotifies++;
                _clslog.AddToLog("No recipients specified for notification. Informing Email administrator [" + properties.EmailAdministrator + "]", Account);
                try
                {
                    MailMessage errMailMsg = new MailMessage {From = new MailAddress(properties.EmailServerFromAddress)};

                    string[] recipients = properties.EmailAdministrator.Replace(",", ";").Split(';');

                    foreach (string email in recipients)
                    {
                        _clslog.AddToLog("Setting recipient [" + email + "]", Account);
                        errMailMsg.To.Add(email);
                    }
                    errMailMsg.Subject = brandName + " Contract Notification Failure";

                    StringBuilder msg = new StringBuilder();

                    msg.Append("An error has occurred trying to send automatic email notifications from " + brandName + "\r\n\r\n");
                    msg.Append("The contract detailed below does not have any employees nominated for notification" + "\r\n\r\n");
                    msg.Append("Contract Key         : " + cDetails["contractKey"] + "\r\n");
                    msg.Append("Contract Description : " + cDetails["contractDescription"] + "\r\n");
                    msg.Append("Supplier             : " + cDetails["supplierName"] + "\r\n\r\n");
                    msg.Append("Click the link below to access the contract" + "\r\n\r\n");
                    msg.AppendFormat("http://{0}/ContractSummary.aspx?tab=0&loc={1}&id={2}", HostManager.GetHostName(this.Account.HostnameIds, Modules.contracts, this.Account.companyid), this.RunLocationId.ToString(CultureInfo.InvariantCulture).Trim(), cid);
                    msg.Append("\r\n\r\n");
                    msg.Append("** NOTE ** This is an automated email. Do not reply.");

                    errMailMsg.Body = msg.ToString();

                    SmtpClient sender = new SmtpClient(properties.EmailServerAddress);

                    sender.Send(errMailMsg);

                    sender = null;
                }
                catch (Exception ex)
                {
                    _clslog.AddToLog("Error report to Email Administrator failed to send", Account);
                    _clslog.AddToLog(ex.Message, Account);
                }
            }

            foreach (DataRow rRow in recplist.Tables[0].Rows)
            {
                // if email address already in the curEmailList, then don't bother to send email > once! Probably named in >1 team
                // or as an individual and a team member
                bool isValidEmail = true;

                if (rRow["Email"] == DBNull.Value)
                {
                    isValidEmail = false;
                }
                else
                {
                    if ((string)rRow["Email"] == string.Empty)
                    {
                        isValidEmail = false;
                    }
                }

                if (!isValidEmail)
                {
                    // cannot issue an email as the recipient's email address is invalid.
                    // Notify the email admin
                    _clslog.AddToLog("An email recipient specified for notification does not have an email address specified. Informing Email administrator [" + properties.EmailAdministrator + "]", Account);

                    if (invalidRecipients.ContainsKey((int)rRow["employeeId"]) == false)
                    {
                        // store that message has been sent to ensure multiple messages for the same employee are not sent
                        invalidRecipients.Add(rRow["employeeId"], rRow["memberName"]);

                        try
                        {
                            MailMessage errMailMsg = new MailMessage {From = new System.Net.Mail.MailAddress(properties.EmailServerFromAddress)};

                            string[] recipients = properties.EmailAdministrator.Replace(",", ";").Split(';');
                            
                            foreach (string email in recipients)
                            {
                                _clslog.AddToLog("Setting recipient [" + email + "]", Account);
                                errMailMsg.To.Add(email);
                            }
                            errMailMsg.Subject = brandName + " Contract Notification Failure";

                            StringBuilder msg = new StringBuilder();

                            msg.Append("An error has occurred trying to send automatic email notifications from " + brandName + "\r\n\r\n");
                            msg.Append("The employee named below was nominated to receive an email notification but does not have an email address associated with their details." + "\r\n\r\n");
                            msg.Append("Employee : " + rRow["memberName"] + "\r\n");
                            msg.Append("\r\n\r\n");
                            msg.Append("Click the link below to access the employee record" + "\r\n\r\n");
                            msg.AppendFormat("http://{0}/shared/admin/aeemployee.aspx?employeeid={1}", HostManager.GetHostName(this.Account.HostnameIds, Modules.contracts, this.Account.companyid), rRow["EmployeeId"].ToString().Trim());
                            msg.Append("\r\n\r\n");
                            msg.Append("** NOTE ** This is an automated email. Do not reply.");

                            errMailMsg.Body = msg.ToString();

                            SmtpClient sender = new SmtpClient(properties.EmailServerAddress);

                            sender.Send(errMailMsg);

                            sender = null;
                        }
                        catch (Exception ex)
                        {
                            _clslog.AddToLog("Error report to Email Administrator failed to send", Account);
                            _clslog.AddToLog(ex.Message, Account);
                        }
                    }
                    else
                    {
                        _clslog.AddToLog("Administrator already been informed by email. Skipping duplicate notification", Account);
                    }
                }
                else
                {
                    if (curEmailList.Contains(rRow["Email"]) == false)
                    {
                        string rcpList = (string)rRow["Email"];

                        curEmailList.Add(rRow["Email"]);

                        _clslog.AddToLog("Emailing " + rcpList, Account);

                        try
                        {
                            DespatchEmail(rcpList.Trim(), toGo, "Contract Up For Review: " + cDetails["contractDescription"]);
                        }
                        catch (Exception ex)
                        {
                            _clslog.AddToLog("Emailing failed. " + ex.Message, Account);
                        }
                    }
                    else
                    {

                        _clslog.AddToLog("Email duplicated for current contract : " + rRow["Email"] + " - skipped", Account);
                    }
                }

            }
        }

        public string PopulateFields(string tpl, List<string> fields, DataRow resRow, string nym)
        {
            _clslog.AddToLog("Populating fields with database values.", Account);

            cCSV csvIn = new cCSV();
            
            DataSet ds = csvIn.CSVToDataset(System.AppDomain.CurrentDomain.BaseDirectory + "email sender\\database.csv");

            tpl = ParseConditions(tpl, resRow, ds);

            foreach (string field in fields)
            {
                foreach (DataRow dRow in ds.Tables[0].Rows)
                {
                    if (field == "[*" + dRow[(int)csvDB_Fields.FriendlyName] + "*]")
                    {
                        _clslog.AddToLog("Database field relation found, replacing template value.", Account);

                        bool isNull = false;
                        string dataType;
                        if ((string)dRow[(int)csvDB_Fields.TableName] == "dbo")
                        {
                            if (resRow[(string)dRow[(int)csvDB_Fields.FieldAlias]] == DBNull.Value)
                            {
                                isNull = true;
                            }
                            
                            dataType = resRow[(string)dRow[(int)csvDB_Fields.FieldAlias]].GetType().ToString();
                        }
                        else
                        {
                            if (resRow[(string)dRow[(int)csvDB_Fields.FieldName]] == DBNull.Value)
                            {
                                isNull = true;
                            }
                           
                            dataType = resRow[(string)dRow[(int)csvDB_Fields.FieldName]].GetType().ToString();
                        }

                        if (isNull == false)
                        {
                            if (dataType == "System.DateTime")
                            {
                                tpl = tpl.Replace("[*" + dRow[(int)csvDB_Fields.FriendlyName] + "*]", ((DateTime)resRow[(string)dRow[(int)csvDB_Fields.FieldName]]).ToShortDateString());
                            }
                            else
                            {
                                if ((string)dRow[(int)csvDB_Fields.TableName] == "dbo")
                                {
                                    tpl = tpl.Replace("[*" + dRow[(int)csvDB_Fields.FriendlyName] + "*]", resRow[(string)dRow[(int)csvDB_Fields.FieldAlias]].ToString());
                                }
                                else
                                {
                                    tpl = tpl.Replace("[*" + dRow[(int)csvDB_Fields.FriendlyName] + "*]", resRow[(string)dRow[(int)csvDB_Fields.FieldName]].ToString());
                                }
                            }
                        }
                        else
                        {
                            tpl = tpl.Replace("[*" + dRow[(int)csvDB_Fields.FriendlyName] + "*]", "");
                        }
                        _clslog.AddToLog("Field added to email.", Account);
                    }
                    else if (field == "[*Contract Link*]")
                    {
                        string hostName = HostManager.GetHostName(this.Account.HostnameIds, Modules.contracts, this.Account.companyid);
                        tpl = hostName.Substring(hostName.Length - 1, 1) == "/" 
                            ? tpl.Replace(field, string.Format("http://{0}ContractSummary.aspx?tab=0&loc={1}&id={2}", hostName, this.RunLocationId.ToString(CultureInfo.InvariantCulture).Trim(), resRow["contractId"])) 
                            : tpl.Replace(field, string.Format("http://{0}/ContractSummary.aspx?tab=0&loc={1}&id={2}", hostName, this.RunLocationId.ToString(CultureInfo.InvariantCulture).Trim(), resRow["contractId"]));
                    }
                    else if (field == "[*NYM Info*]")
                    {
                        tpl = tpl.Replace(field, nym);

                    }
                }
            }
            return tpl;
        }

        private string ParseConditions(string tpl, DataRow resRow, DataSet ds)
        {
            string[] conditions = GetConditionsFromTemplate(tpl);

            for (int i = 0; i < conditions.Length - 1; i++)
            {
                foreach (DataRow dRow in ds.Tables[0].Rows)
                {
                    bool isBlank = false;
                    if(conditions[i] != "[-" + dRow[(int) csvDB_Fields.FriendlyName] + "-]") continue;

                    string entireTrue = ComputeEntireTrueCondition(tpl, conditions[i]);
                    string entireFalse = ComputeEntireFalseCondition(tpl, conditions[i]);
                    string entire = ComputeEntireCondition(tpl, conditions[i]);

                    if (resRow[(int)dRow[(int)csvDB_Fields.FieldName]] == DBNull.Value)
                    {
                        isBlank = true;
                    }

                    if (isBlank == false)
                    {
                        if (resRow[(int)dRow[(int)csvDB_Fields.FieldName]].ToString() == "")
                        {
                            isBlank = true;
                        }
                    }

                    string Out;
                    if (isBlank == false)
                    {
                        Out = entireTrue.Replace("[-" + dRow[(int)csvDB_Fields.FriendlyName] + "-]{", "");
                        Out = Out.Replace("}", "");
                        Out = Out.Replace(Out, "\r\n\r\n\r\n");
                        tpl = tpl.Replace(entire, Out);
                    }
                    else
                    {
                        Out = entireFalse.Replace("}else{", "");
                        Out = Out.Replace("}", "");
                        Out = Out.Replace(Out, "\r\n\r\n\r\n");
                        tpl = tpl.Replace(entire, Out);
                    }
                }
            }

            return tpl;
        }

        private static string ComputeEntireTrueCondition(string tpl, string condition)
        {
            int intStart = tpl.IndexOf(condition + "{");
            if (intStart > -1)
            {
                int intEnd = tpl.IndexOf("}", intStart);
                tpl = tpl.Substring(intStart, intEnd - intStart + 1);
                return tpl;
            }
            return "";
        }

        private static string ComputeEntireFalseCondition(string tpl, string condition)
        {
            int intStart = tpl.IndexOf(condition + "{");
            if (intStart > -1)
            {
                int intEnd = tpl.IndexOf("}else{", intStart + 1);
                if (intEnd > -1)
                {
                    int intVeryEnd = tpl.IndexOf("}", intEnd + 1);
                    tpl = tpl.Substring(intEnd, intVeryEnd - intEnd + 1);
                    return tpl;
                }
            }
            return "";
        }

        private static string ComputeEntireCondition(string tpl, string condition)
        {
            int intStart = tpl.IndexOf(condition + "{");
            if (intStart > -1)
            {
                int intEnd = tpl.IndexOf("}else{", intStart + 1);
                int intVeryEnd;
                if (intEnd > -1)
                {
                    intVeryEnd = tpl.IndexOf("}", intEnd + 1);
                    tpl = tpl.Substring(intStart, intVeryEnd - intStart + 1);
                    return tpl;
                }
                
                intVeryEnd = tpl.IndexOf("}", intStart + 1);
                tpl = tpl.Substring(intStart, intVeryEnd - intStart + 1);
                return tpl;
            }
            return "";
        }

        public string GetProducts(MaintParams mParams, long contractID)
        {
            StringBuilder strOut = new StringBuilder();
            DBConnection db = new DBConnection(cAccounts.getConnectionString(Account.accountid));
            const string sql = "SELECT [productDetails].[ProductName],[MaintenanceValue],[ProductValue],[MaintenancePercent] FROM [contract_productdetails] INNER JOIN [productDetails] ON [productDetails].[ProductId] = [contract_productdetails].[ProductId] WHERE [ContractId] = @conId";
            db.sqlexecute.Parameters.AddWithValue("@conId", contractID);
            DataSet ds = db.GetDataSet(sql);

            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dRow in ds.Tables[0].Rows)
                {
                    mParams.CurMaintVal = (double)dRow["MaintenanceValue"];

                    mParams.ListPrice = (double)dRow["ProductValue"];

                    mParams.PctOfLP = (double)dRow["MaintenancePercent"];

                    NYMResult nymRes = SMRoutines.CalcNYM(mParams, 0);

                    strOut.Append(dRow["ProductName"] + " - Current annual cost (£" + dRow["MaintenanceValue"] + ") increase is capped not to exceed £" + nymRes.NYMValue + " next year." + "\r\n");
                }
                return "Product Annual Cost: " + "\r\n" + strOut.ToString();
            }
            return "";
        }
    }
}
