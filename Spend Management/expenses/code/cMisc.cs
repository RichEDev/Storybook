namespace Spend_Management
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Caching;
    using System.Web.UI.WebControls;

    using BusinessLogic.AccountProperties;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Enums;
    using BusinessLogic.GeneralOptions;

    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary;

    #endregion

    /// <summary>
    /// The c misc.
    /// </summary>
    public class cMisc
    {
        #region Fields

        /// <summary>
        ///   The accountid.
        /// </summary>
        private readonly int accountid;

        /// <summary>
        ///   The strsql.
        /// </summary>
        private string strsql;

        /// <summary>
        /// Cache key for General Fields within this class
        /// </summary>
        public const string GeneralFieldsCacheKey = "GeneralFields";

        /// <summary>
        /// The cache object for General Fields
        /// </summary>
        private readonly Utilities.DistributedCaching.Cache _generalFieldsCache = new Utilities.DistributedCaching.Cache();

        private readonly Lazy<IDataFactory<IAccountProperty, AccountPropertyCacheKey>> _accountPropertiesFactory = new Lazy<IDataFactory<IAccountProperty, AccountPropertyCacheKey>>(() => FunkyInjector.Container.GetInstance<IDataFactory<IAccountProperty, AccountPropertyCacheKey>>());
        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="cMisc"/> class.
        /// </summary>
        /// <param name="intaccountid">
        /// The intaccountid. 
        /// </param>
        public cMisc(int intaccountid)
        {
            this.accountid = intaccountid;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the path.
        /// </summary>
        public static string Path
        {
            get
            {
                return HttpRuntime.AppDomainAppVirtualPath == "/" ? string.Empty : HttpRuntime.AppDomainAppVirtualPath;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dotonate string.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="maxlength">
        /// The maxlength.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string DotonateString(string target, int maxlength)
        {
            if (target.Length > maxlength)
            {
                return target.Substring(0, maxlength) + "...";
            }

            return target;
        }

        /// <summary>
        /// Escape any new-line characters to prevent them being interpreted as literal new-line characters in output (markup)
        /// </summary>
        /// <param name="target">
        /// The string to be escaped 
        /// </param>
        /// <returns>
        /// The escaped string 
        /// </returns>
        public static string EscapeLinebreaks(string target)
        {
            // ignore the entire string if any linebreaks have already been escaped
            // ReSharper disable StringIndexOfIsCultureSpecific.1
            if (target.IndexOf("\\n") == -1 && target.IndexOf("\\r") == -1)
            // ReSharper restore StringIndexOfIsCultureSpecific.1
            {
                target = target.Replace("\n", "\\n").Replace("\r", "\\r");
            }

            return target;
        }

        /// <summary>
        /// The match password key.
        /// </summary>
        /// <param name="uniqueKey">
        /// The unique key.
        /// </param>
        /// <returns>
        /// The <see cref="cEmployee"/>.
        /// </returns>
        public static Tuple<int, Employee> MatchPasswordKey(string uniqueKey)
        {
            Employee reqEmployee = null;

            int accountId;

            if (uniqueKey.Length < 21)
            {
                return null;
            }

            int.TryParse(uniqueKey.Substring(0, 1) + uniqueKey.Substring(21), out accountId);

            if (accountId == 0)
            {
                return null;
            }

            var clsAccounts = new cAccounts();
            cAccount reqAccount = clsAccounts.GetAccountByID(accountId);

            if (reqAccount != null)
            {
                var data = new DBConnection(cAccounts.getConnectionString(accountId));
                data.sqlexecute.Parameters.AddWithValue("@uniqueKey", uniqueKey);
                const string SQL = "SELECT employeeID FROM employeePasswordKeys WHERE uniqueKey=@uniqueKey";
                int employeeId = 0;

                using (SqlDataReader reader = data.GetReader(SQL))
                {
                    while (reader.Read())
                    {
                        employeeId = reader.GetInt32(0);
                    }

                    reader.Close();
                }

                if (employeeId == 0)
                {
                    return null;
                }

                var clsEmployees = new cEmployees(accountId);
                reqEmployee = clsEmployees.GetEmployeeById(employeeId);

                if (reqEmployee.Archived || (!reqEmployee.Active))
                {
                    return null;
                }
            }

            return new Tuple<int, Employee>(accountId, reqEmployee);
        }

        /// <summary>
        /// The get current user.
        /// </summary>
        /// <param name="identity">
        /// The identity.
        /// </param>
        /// <param name="fromScheduler">Are we coming from the scheduler. Defaults to false</param>
        /// <returns>
        /// The <see cref="CurrentUser"/> . 
        /// </returns>
        public static CurrentUser GetCurrentUser(string identity = "", bool fromScheduler = false)
        {
            bool unitTestMode = false;
            int accountid;
            int employeeid;
            int subaccountid = -1;
            int delegateId = 0;

            if (ConfigurationManager.AppSettings["UnitTestSettings"] != null)
            {
                unitTestMode = SetAccountAndEmployeeForUnitTestMode(out delegateId, out accountid, out employeeid);
            }
            else
            {
                if (!SetAccountAndEmployeeStandardMode(identity, out accountid, out employeeid))
                {
                    return null;
                }
            }

            var activeModule = Modules.SpendManagement;

            if (!unitTestMode)
            {
                SetUserValuesFromContext(identity, out subaccountid, out delegateId, out activeModule);
            }

            var user = new CurrentUser(accountid, employeeid, delegateId, activeModule, subaccountid, fromScheduler);

            // refresh lastActivityDate for concurrent user management
            if (activeModule == Modules.contracts)
            {
                SetLastActivity(accountid, employeeid);
            }

            return user;
        }

        public static bool IsDelegate
        {
            get
            {
                if (HttpContext.Current.Session["myid"] != null)
                {
                    return (int)HttpContext.Current.Session["myid"] > 0 ? true:false;
                }
                return false;
            }
        }

        /// <summary>
        /// The set user values from context.
        /// </summary>
        /// <param name="identity">
        /// The identity.
        /// </param>
        /// <param name="subaccountid">
        /// The sub account id.
        /// </param>
        /// <param name="delegateId">
        /// returns current delegate id.
        /// </param>
        /// <param name="activeModule">
        /// The active module.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool SetUserValuesFromContext(
            string identity, out int subaccountid, out int delegateId, out Modules activeModule)
        {
            subaccountid = -1;
            activeModule = Modules.SpendManagement;
            delegateId = 0;

            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                if (HttpContext.Current.Session["SubAccountID"] != null)
                {
                    subaccountid = (int)HttpContext.Current.Session["SubAccountID"];
                }

                if (HttpContext.Current.Session["myid"] != null)
                {
                    delegateId = (int)HttpContext.Current.Session["myid"];
                }

                activeModule = HostManager.GetModule(HttpContext.Current.Request.Url.Host);

                return true;
            }

            // module will be set from a config file if not part of the web application. e.g. service.
            if (identity != string.Empty && ConfigurationManager.AppSettings.AllKeys.Contains("defaultModule"))
            {
                activeModule = (Modules)Convert.ToInt32(ConfigurationManager.AppSettings["defaultModule"]);
                return true;
            }

            // if there is HTTP context but no session (e.g. API request), set the module according to the hostname
            if (HttpContext.Current != null)
            {
                activeModule = HostManager.GetModule(HttpContext.Current.Request.Url.Host);
            }

            return false;
        }

        /// <summary>
        /// The set account and employee standard mode.
        /// </summary>
        /// <param name="identity">
        /// The identity.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool SetAccountAndEmployeeStandardMode(string identity, out int accountId, out int employeeId)
        {
            accountId = 0;
            employeeId = 0;

            if (identity == string.Empty && (HttpContext.Current == null || HttpContext.Current.User.Identity.Name.Split(',').Length != 2))
            {
                return false;
            }

            string[] stemp = identity == string.Empty
                                 ? HttpContext.Current.User.Identity.Name.Split(',')
                                 : identity.Split(',');
            accountId = int.Parse(stemp[0]);
            employeeId = string.IsNullOrEmpty(stemp[1]) ? 0 : int.Parse(stemp[1]);
            
            return true;
        }

        /// <summary>
        /// The set account and employee for unit test mode.
        /// </summary>
        /// <param name="delegateId">
        /// The delegate id.
        /// </param>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <param name="employeeid">
        /// The employeeid.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool SetAccountAndEmployeeForUnitTestMode(out int delegateId, out int accountid, out int employeeid)
        {
            var cache = new Caching();

            delegateId = 0;
            if (cache.Cache["UnitTestDelegate"] != null)
            {
                delegateId = int.Parse(cache.Cache["UnitTestDelegate"].ToString());
            }

            accountid = Convert.ToInt32(ConfigurationManager.AppSettings["AccountID"].ToString(CultureInfo.InvariantCulture));
            employeeid = Convert.ToInt32(ConfigurationManager.AppSettings["EmployeeID"].ToString(CultureInfo.InvariantCulture));
            return true;
        }

        /// <summary>
        /// The get add screen fields.
        /// </summary>
        /// <returns>
        /// The screen fields in a sorted list.
        /// </returns>
        public SortedList<string, cFieldToDisplay> GetAddScreenFields()
        {
            var lstFieldsToDisplay = (SortedList<string, cFieldToDisplay>)this._generalFieldsCache.Get(this.accountid, GeneralFieldsCacheKey, string.Empty) ?? this.GetGeneralFieldsToDisplay();
            return lstFieldsToDisplay;
        }

        /// <summary>
        /// The add role defaults to template.
        /// </summary>
        /// <param name="employeeid">
        /// The employeeid.
        /// </param>
        public void AddRoleDefaultsToTemplate(int employeeid)
        {
            var expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            var clsemployees = new cEmployees(this.accountid);
            Employee employee = clsemployees.GetEmployeeById(employeeid);
            List<EmployeeItemRole> lstItemRoles = employee.GetItemRoles().ItemRoles;

            this.strsql = "select subcatid from rolesubcats where roleid in (";
            foreach (EmployeeItemRole tmpItemRoleId in lstItemRoles)
            {
                this.strsql += tmpItemRoleId.ItemRoleId + ",";
            }

            if (lstItemRoles.Count > 0)
            {
                this.strsql = this.strsql.Remove(this.strsql.Length - 1, 1);
            }

            this.strsql += ") and isadditem = 1";

            using (SqlDataReader reader = expdata.GetReader(this.strsql))
            {
                this.strsql = string.Empty;
                while (reader.Read())
                {
                    this.strsql += "insert into additems (employeeid, subcatid) values (" + employeeid + "," + reader.GetInt32(0) + ");";
                }

                reader.Close();
            }

            if (this.strsql != string.Empty)
            {
                expdata.ExecuteSQL(this.strsql);
            }
        }

        /// <summary>
        /// The change policy type.
        /// </summary>
        /// <param name="policytype">
        /// The policytype.
        /// </param>
        public void ChangePolicyType(byte policytype)
        {
            CurrentUser currentUser = GetCurrentUser();

            this._accountPropertiesFactory.Value.Save(new AccountProperty(AccountPropertyKeys.PolicyType.GetDescription(), policytype.ToString(), currentUser.CurrentSubAccountId));
            var accountBase = new cAccountSubAccountsBase(currentUser.AccountID);
            accountBase.InvalidateCache(currentUser.CurrentSubAccountId);
        }

        /// <summary>
        /// The generate print out fields.
        /// </summary>
        /// <param name="fields">
        ///     The fields.
        /// </param>
        /// <param name="claimid">
        ///     The claimid.
        /// </param>
        /// <param name="allowmultipledestinations"></param>
        public void GeneratePrintOutFields(ref Panel fields, int claimid, bool allowmultipledestinations)
        {
            var expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            var row = new TableRow();

            int cellcount = 0;
            string fieldsql = this.CreatePrintHeaderSql(allowmultipledestinations);

            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimid);
            if (fieldsql == string.Empty)
            {
                return;
            }

            fieldsql += " where claims.claimid = @claimid;";

            using (SqlDataReader reader = expdata.GetReader(fieldsql))
            {
                while (reader.Read())
                {
                    int i;
                    for (i = 0; i < reader.FieldCount; i++)
                    {
                        if (cellcount == 0)
                        {
                            row = new TableRow();
                        }

                        var cell = new TableCell { Text = reader.GetName(i) + ":", CssClass = "labeltd" };
                        row.Cells.Add(cell);
                        cell = new TableCell { Text = reader.GetValue(i).ToString(), CssClass = "inputtd" };
                        row.Cells.Add(cell);
                        cellcount++;
                        if (cellcount == 2)
                        {
                            fields.Controls.Add(row);
                            cellcount = 0;
                    }
                    }

                    fields.Controls.Add(row);
                }

                reader.Close();
            }

            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// The get company details.
        /// </summary>
        /// <returns>
        /// The <see cref="sCompanyDetails"/>.
        /// </returns>
        public sCompanyDetails GetCompanyDetails()
        {
            var expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            var temp = new sCompanyDetails();

            this.strsql = "select * from companydetails";
            SqlDataReader compreader;
            using (compreader = expdata.GetReader(this.strsql))
            {
                while (compreader.Read())
                {
                    if (compreader.IsDBNull(compreader.GetOrdinal("companyname")) == false)
                    {
                        temp.companyname = compreader.GetString(compreader.GetOrdinal("companyname"));
                    }

                    if (compreader.IsDBNull(compreader.GetOrdinal("address1")) == false)
                    {
                        temp.address1 = compreader.GetString(compreader.GetOrdinal("address1"));
                    }

                    if (compreader.IsDBNull(compreader.GetOrdinal("address2")) == false)
                    {
                        temp.address2 = compreader.GetString(compreader.GetOrdinal("address2"));
                    }

                    if (compreader.IsDBNull(compreader.GetOrdinal("city")) == false)
                    {
                        temp.city = compreader.GetString(compreader.GetOrdinal("city"));
                    }

                    if (compreader.IsDBNull(compreader.GetOrdinal("county")) == false)
                    {
                        temp.county = compreader.GetString(compreader.GetOrdinal("county"));
                    }

                    if (compreader.IsDBNull(compreader.GetOrdinal("postcode")) == false)
                    {
                        temp.postcode = compreader.GetString(compreader.GetOrdinal("postcode"));
                    }

                    if (compreader.IsDBNull(compreader.GetOrdinal("telno")) == false)
                    {
                        temp.telno = compreader.GetString(compreader.GetOrdinal("telno"));
                    }

                    if (compreader.IsDBNull(compreader.GetOrdinal("faxno")) == false)
                    {
                        temp.faxno = compreader.GetString(compreader.GetOrdinal("faxno"));
                    }

                    if (compreader.IsDBNull(compreader.GetOrdinal("email")) == false)
                    {
                        temp.email = compreader.GetString(compreader.GetOrdinal("email"));
                    }

                    if (compreader.IsDBNull(compreader.GetOrdinal("companynumber")) == false)
                    {
                        temp.companynumber = compreader.GetString(compreader.GetOrdinal("companynumber"));
                }
                }

                compreader.Close();
            }

            this.strsql = "select * from [company_bankdetails]";

            using (compreader = expdata.GetReader(this.strsql))
            {
                while (compreader.Read())
                {
                    if (compreader.IsDBNull(compreader.GetOrdinal("bankreference")) == false)
                    {
                        temp.bankref = compreader.GetString(compreader.GetOrdinal("bankreference"));
                    }

                    if (compreader.IsDBNull(compreader.GetOrdinal("accountnumber")) == false)
                    {
                        temp.accoutno = compreader.GetString(compreader.GetOrdinal("accountnumber"));
                    }

                    if (compreader.IsDBNull(compreader.GetOrdinal("accounttype")) == false)
                    {
                        temp.accounttype = compreader.GetString(compreader.GetOrdinal("accounttype"));
                    }

                    if (compreader.IsDBNull(compreader.GetOrdinal("sortcode")) == false)
                    {
                        temp.sortcode = compreader.GetString(compreader.GetOrdinal("sortcode"));
                }
                }

                compreader.Close();
            }
            expdata.sqlexecute.Parameters.Clear();
            return temp;
        }

        /// <summary>
        /// The get general field by code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="cFieldToDisplay"/>.
        /// </returns>
        public cFieldToDisplay GetGeneralFieldByCode(string code)
        {
            return this.GetAddScreenFields()[code];
        }

        /// <summary>
        /// The get printout fields.
        /// </summary>
        /// <returns>
        /// The list of printout fields.
        /// </returns>
        public List<Guid> GetPrintoutFields()
        {
            var expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            var fields = new List<Guid>();
            this.strsql = "select fieldid from printout";
            using (SqlDataReader reader = expdata.GetReader(this.strsql))
            {
                while (reader.Read())
                {
                    fields.Add(reader.GetGuid(0));
                }

                reader.Close();
            }
            return fields;
        }

        /// <summary>
        /// read a document (e.g. Policy Document) from the server and return contents of the file
        /// </summary>
        /// <param name="documentPath">
        /// the full path of the document, including name and extension. 
        /// </param>
        /// <returns>
        /// The contents of the file specified. null if not found 
        /// </returns>
        public string ReadServerDocument(string documentPath)
        {
            string result;
            if (File.Exists(documentPath))
            {
                var fs = new FileStream(documentPath, FileMode.Open);
                var sr = new StreamReader(fs);
                result = sr.ReadToEnd();
                sr.Close();
                fs.Close();
            }
            else
            {
                result = null;
            }

            return StripTagsCharArray(result);
        }

        /// <summary>
        /// The update company details.
        /// </summary>
        /// <param name="companyname">
        /// The companyname.
        /// </param>
        /// <param name="address1">
        /// The address 1.
        /// </param>
        /// <param name="address2">
        /// The address 2.
        /// </param>
        /// <param name="city">
        /// The city.
        /// </param>
        /// <param name="county">
        /// The county.
        /// </param>
        /// <param name="postcode">
        /// The postcode.
        /// </param>
        /// <param name="telno">
        /// The telno.
        /// </param>
        /// <param name="faxno">
        /// The faxno.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="bankref">
        /// The bankref.
        /// </param>
        /// <param name="accountno">
        /// The accountno.
        /// </param>
        /// <param name="accounttype">
        /// The accounttype.
        /// </param>
        /// <param name="sortcode">
        /// The sortcode.
        /// </param>
        /// <param name="companynumber">
        /// The companynumber.
        /// </param>
        public void UpdateCompanyDetails(
            string companyname,
            string address1,
            string address2,
            string city,
            string county,
            string postcode,
            string telno,
            string faxno,
            string email,
            string bankref,
            string accountno,
            string accounttype,
            string sortcode,
            string companynumber)
        {
            var expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));

            expdata.sqlexecute.Parameters.AddWithValue("@companyname", companyname);
            expdata.sqlexecute.Parameters.AddWithValue("@address1", address1);
            expdata.sqlexecute.Parameters.AddWithValue("@address2", address2);
            expdata.sqlexecute.Parameters.AddWithValue("@city", city);
            expdata.sqlexecute.Parameters.AddWithValue("@county", county);
            expdata.sqlexecute.Parameters.AddWithValue("@postcode", postcode);
            expdata.sqlexecute.Parameters.AddWithValue("@telno", telno);
            expdata.sqlexecute.Parameters.AddWithValue("@faxno", faxno);
            expdata.sqlexecute.Parameters.AddWithValue("@email", email);
            expdata.sqlexecute.Parameters.AddWithValue("@bankref", bankref);
            expdata.sqlexecute.Parameters.AddWithValue("@accountno", accountno);
            expdata.sqlexecute.Parameters.AddWithValue("@accounttype", accounttype);
            expdata.sqlexecute.Parameters.AddWithValue("@sortcode", sortcode);
            expdata.sqlexecute.Parameters.AddWithValue("@companynumber", companynumber);

            this.strsql = "select count(*) from companydetails";
            int count = expdata.getcount(this.strsql);
            if (count == 0)
            {
                this.strsql =
                    "insert into companydetails (companyname, address1, address2, city, county, postcode, telno, faxno, email, companynumber) "
                    +
                    "values (@companyname,@address1,@address2,@city,@county,@postcode,@telno,@faxno,@email, @companynumber)";
            }
            else
            {
                this.strsql =
                    "update companydetails set companyname = @companyname, address1 = @address1, address2 = @address2, city = @city, county = @county, postcode = @postcode, telno = @telno, faxno = @faxno, email = @email, companynumber = @companynumber";
            }

            expdata.ExecuteSQL(this.strsql);
            this.strsql = "select count(*) from [company_bankdetails]";
            count = expdata.getcount(this.strsql);
            if (count == 0)
            {
                this.strsql = "insert into [company_bankdetails] (bankreference, accountnumber, accounttype, sortcode) "
                              + "values (@bankref,@accountno,@accounttype,@sortcode)";
            }
            else
            {
                this.strsql =
                    "update [company_bankdetails] set bankreference = @bankref, accountnumber = @accountno, accounttype = @accounttype, sortcode = @sortcode";
            }

            expdata.ExecuteSQL(this.strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// The update fields to display.
        /// </summary>
        /// <param name="fields">
        /// The fields.
        /// </param>
        public void UpdateFieldsToDisplay(List<cFieldToDisplay> fields)
        {
            var expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            int i = 0;

            foreach (cFieldToDisplay field in fields)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@display" + i, Convert.ToByte(field.display));
                expdata.sqlexecute.Parameters.AddWithValue("@mandatory" + i, Convert.ToByte(field.mandatory));
                expdata.sqlexecute.Parameters.AddWithValue("@individual" + i, Convert.ToByte(field.individual));
                expdata.sqlexecute.Parameters.AddWithValue("@fieldid" + i, field.fieldid);
                if (fields[i].description == string.Empty)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@description" + i, DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@description" + i, field.description);
                }

                expdata.sqlexecute.Parameters.AddWithValue("@displaycc" + i, Convert.ToByte(fields[i].displaycc));
                expdata.sqlexecute.Parameters.AddWithValue("@displaypc" + i, Convert.ToByte(fields[i].displaypc));
                expdata.sqlexecute.Parameters.AddWithValue("@mandatorycc" + i, Convert.ToByte(fields[i].mandatorycc));
                expdata.sqlexecute.Parameters.AddWithValue("@mandatorypc" + i, Convert.ToByte(fields[i].mandatorypc));
                expdata.sqlexecute.Parameters.AddWithValue("@modifiedon" + i, DateTime.Now.ToUniversalTime());
                this.strsql = "update addscreen set description = @description" + i + ", display = @display" + i
                              + ", mandatory = @mandatory" + i + ", individual = @individual" + i
                              + ", displaycc = @displaycc" + i + ", displaypc = @displaypc" + i
                              + ", mandatorycc = @mandatorycc" + i + ", mandatorypc = @mandatorypc" + i
                              + ", modifiedon = @modifiedon" + i + " where fieldid = @fieldid" + i;
                expdata.ExecuteSQL(this.strsql);
                i++;
            }

            expdata.sqlexecute.Parameters.Clear();

            this._generalFieldsCache.Delete(this.accountid, GeneralFieldsCacheKey, string.Empty);
        }

        /// <summary>
        /// The update policy.
        /// </summary>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <param name="policytype">
        /// The policytype.
        /// </param>
        public void UpdatePolicy(string policy, byte policytype)
        {
            CurrentUser currentUser = GetCurrentUser();

            this._accountPropertiesFactory.Value.Save(new AccountProperty(AccountPropertyKeys.PolicyType.GetDescription(), policytype.ToString(), currentUser.CurrentSubAccountId));

            this._accountPropertiesFactory.Value.Save(new AccountProperty(AccountPropertyKeys.CompanyPolicy.GetDescription(), policy, currentUser.CurrentSubAccountId));

            var accountBase = new cAccountSubAccountsBase(currentUser.AccountID);
            accountBase.InvalidateCache(currentUser.CurrentSubAccountId);
        }

        /// <summary>
        /// The update print out.
        /// </summary>
        /// <param name="fieldids">
        /// The fieldids.
        /// </param>
        public void UpdatePrintOut(Guid[] fieldids)
        {
            var expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            int i;

            this.strsql = "delete from printout";
            expdata.ExecuteSQL(this.strsql);

            this.strsql = string.Empty;

            for (i = 0; i < fieldids.Length; i++)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@fieldid" + i, fieldids[i]);
                this.strsql += "insert into printout (fieldid) " + "values (@fieldid" + i + ")";
            }

            if (this.strsql != string.Empty)
            {
                expdata.ExecuteSQL(this.strsql);
            }

            expdata.sqlexecute.Parameters.Clear();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The set last activity.
        /// </summary>
        /// <param name="accId">
        /// The account id.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        private static void SetLastActivity(int accId, int employeeId)
        {
            // don't run in unit test mode
            if (ConfigurationManager.AppSettings["UnitTestSettings"] == null)
            {
                // refresh lastActivityDate for concurrent user management
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    DateTime? previousLastActivity = null;

                    if (HttpContext.Current.Session["lastActivity"] != null)
                    {
                        previousLastActivity = Convert.ToDateTime(HttpContext.Current.Session["lastActivity"]);
                    }

                    // refresh lastActivityDate for concurrent user management
                    if (cConcurrentUsers.CUActivityHit(accId, employeeId, previousLastActivity))
                    {
                        // update the last visited hit to now
                        HttpContext.Current.Session.Add("lastActivity", DateTime.Now);
                    }
                }
            }
        }

        /// <summary>
        /// Remove HTML tags from string using char array.
        /// </summary>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <returns>
        /// The <see cref="string"/> . 
        /// </returns>
        private static string StripTagsCharArray(string source)
        {
            var array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            foreach (char @let in source)
            {
                if (@let == '<')
                {
                    inside = true;
                    continue;
                }

                if (@let == '>')
                {
                    inside = false;
                    continue;
                }

                if (!inside)
                {
                    array[arrayIndex] = @let;
                    arrayIndex++;
                }
            }

            return new string(array, 0, arrayIndex);
        }

        /// <summary>
        /// The create print header sql.
        /// </summary>
        /// <param name="allowmultipledestinations">Are multiple journey stepsn allowed from account properties</param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string CreatePrintHeaderSql(bool allowmultipledestinations)
        {
            var expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            string sql = string.Empty;
            var clsfields = new cFields(this.accountid);
            var fields = new SortedList<Guid, cField>();

            var basetableid = new Guid("0efa50b5-da7b-49c7-a9aa-1017d5f741d0");

            // get the list of fields required
            this.strsql = "select fieldid from printout";
            using (SqlDataReader reader = expdata.GetReader(this.strsql))
            {
                while (reader.Read())
                {
                    Guid fieldId = reader.GetGuid(0);
                    if (allowmultipledestinations
                        && (fieldId == new Guid("c75064ec-be87-4dd3-8299-d0d81ea3f819")
                            || fieldId == new Guid("3d8c699e-9e0e-4484-b821-b49b5cb4c098")))
                {
                        // don't include to and from for multiple journey steps
                        continue;
                    }

                    fields.Add(Guid.NewGuid(), clsfields.GetFieldByID(fieldId));
                }

                reader.Close();
            }

            sql += "select distinct ";
            if (fields.Count == 0)
            {
                return string.Empty;
            }

            foreach (cField field in fields.Values)
            {
                if (field.ValueList)
                {
                    sql += " [" + field.Description + "] = CASE";

                    sql = field.ListItems.Aggregate(sql, (current, kvp) => current + (" WHEN [" + field.GetParentTable().TableName + "].[" + field.FieldName + "] = " + kvp.Key + " THEN '" + kvp.Value.Replace("'", "''") + "'"));

                    sql += " END,";
                }
                else
                {
                    sql = sql + "[" + field.GetParentTable().TableName + "].[" + field.FieldName + "] as ["
                          + field.Description + "],";
                }
            }

            sql = sql.Remove(sql.Length - 1, 1);
            sql = sql + " from claims";

            var clsjoins = new cJoins(this.accountid);
            sql += " " + clsjoins.createJoinSQL(fields, basetableid, null);

            expdata.sqlexecute.Parameters.Clear();
            return sql;
        }

        /// <summary>
        /// The get general fields to display.
        /// </summary>
        /// <returns>
        /// The general fields to display.
        /// </returns>
        private SortedList<string, cFieldToDisplay> GetGeneralFieldsToDisplay()
        {
            var expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
            var list = new SortedList<string, cFieldToDisplay>();
            this.strsql = "select addscreen.fieldid, display, mandatory, code, description, individual, displaycc, displaypc, mandatorycc, mandatorypc, createdon, createdby, modifiedon, modifiedby from dbo.addscreen";
            expdata.sqlexecute.CommandText = this.strsql;

            var clsfields = new cFields(this.accountid);
            using (SqlDataReader reader = expdata.GetReader(this.strsql))
            {
                while (reader.Read())
                {
                    Guid fieldid = reader.GetGuid(reader.GetOrdinal("fieldid"));
                    bool display = reader.GetBoolean(reader.GetOrdinal("display"));
                    bool mandatory = reader.GetBoolean(reader.GetOrdinal("mandatory"));
                    string code = reader.GetString(reader.GetOrdinal("code"));
                    string description;
                    if (reader.IsDBNull(reader.GetOrdinal("description")) == false)
                    {
                        description = reader.GetString(reader.GetOrdinal("description"));
                    }
                    else
                {
                        cField reqfield = clsfields.GetFieldByID(fieldid);
                        description = reqfield.Description;
                    }

                    bool individual = reader.GetBoolean(reader.GetOrdinal("individual"));
                    bool displaycc = reader.GetBoolean(reader.GetOrdinal("displaycc"));
                    bool displaypc = reader.GetBoolean(reader.GetOrdinal("displaypc"));
                    bool mandatorycc = reader.GetBoolean(reader.GetOrdinal("mandatorycc"));
                    bool mandatorypc = reader.GetBoolean(reader.GetOrdinal("mandatorypc"));
                    DateTime createdon = reader.IsDBNull(reader.GetOrdinal("createdon")) ? new DateTime(1900, 01, 01) : reader.GetDateTime(reader.GetOrdinal("createdon"));

                    int createdby = reader.IsDBNull(reader.GetOrdinal("createdby")) ? 0 : reader.GetInt32(reader.GetOrdinal("createdby"));

                    DateTime modifiedon = reader.IsDBNull(reader.GetOrdinal("modifiedon")) ? new DateTime(1900, 01, 01) : reader.GetDateTime(reader.GetOrdinal("modifiedon"));

                    int modifiedby = reader.IsDBNull(reader.GetOrdinal("modifiedby")) ? 0 : reader.GetInt32(reader.GetOrdinal("modifiedby"));

                    var field = new cFieldToDisplay(fieldid, code, description, display, mandatory, individual, displaycc, mandatorycc, displaypc, mandatorypc, createdon, createdby, modifiedon, modifiedby);
                    list.Add(code, field);
                }

                reader.Close();
            }

            expdata.sqlexecute.Parameters.Clear();

            this._generalFieldsCache.Add(this.accountid, GeneralFieldsCacheKey, string.Empty, list);

            return list;
        }

        #endregion
    }

    /// <summary>
    ///   Action enumeration
    /// </summary>
    public enum Action
    {
        /// <summary>
        ///   Add enum
        /// </summary>
        Add = 1,

        /// <summary>
        ///   Edit enum
        /// </summary>
        Edit,

        /// <summary>
        ///   Delete enum
        /// </summary>
        Delete,

        /// <summary>
        /// Copy a specific expense item
        /// </summary>
        Copy
    }

    /// <summary>
    ///   wizardStep class
    /// </summary>
    public class wizardStep
    {
        #region Fields

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="wizardStep"/> class. 
        /// wizardStep constructor
        /// </summary>
        /// <param name="actualstep">
        /// The actual step number
        /// </param>
        /// <param name="logicalstep">
        /// The logical step number
        /// </param>
        /// <param name="label">
        /// The label
        /// </param>
        public wizardStep(int actualstep, int logicalstep, string label)
        {
            this.Actualstep = actualstep;
            this.Logicalstep = logicalstep;
            this.Label = label;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the actualstep.
        /// </summary>
        public int Actualstep { get; private set; }

        /// <summary>
        /// Gets the label.
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the logicalstep.
        /// </summary>
        public int Logicalstep { get; private set; }

        #endregion
    }
}
