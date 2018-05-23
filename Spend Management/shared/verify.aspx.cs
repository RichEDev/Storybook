namespace Spend_Management
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    using System.Web;
    using System.Web.UI;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Enumerators;

    #endregion

    /// <summary>
    /// The verify.
    /// </summary>
    public partial class verify : Page
    {
        /// <summary>
        /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IGeneralOptions"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory { get; set; }

        /// <summary>
        /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IProductModule"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IProductModule, Modules> ProductModuleFactory { get; set; }

        #region Public Methods and Operators

        /// <summary>
        /// The create advances signoff summary.
        /// </summary>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <param name="reqemp">
        /// The reqemp.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CreateAdvancesSignoffSummary(int accountid, Employee reqemp)
        {
            var output = new StringBuilder();
            output.Append("<div>\n");
            output.Append("<div class=\"inputpaneltitle\">Advances Signoff Group</div>");
            output.Append("<table>");

            output.Append("<tr><td class=\"labeltd\">Advances Signoff Group:</td><td class=\"inputtd\">");
            if (reqemp.AdvancesSignOffGroup != 0)
            {
                var clsgroups = new cGroups(accountid);
                cGroup group = clsgroups.GetGroupById(reqemp.AdvancesSignOffGroup);
                if (group != null)
                {
                    output.Append(group.groupname);
                }
            }

            output.Append("</td></tr>");
            output.Append("</table>");
            output.Append("</div>\n");
            return output.ToString();
        }

        /// <summary>
        /// The create car summary.
        /// </summary>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <param name="reqemp">
        /// The reqemp.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CreateCarSummary(int accountId, Employee employee, IProductModule module)
        {
            var clsEmpCars = new cEmployeeCars(accountId, employee.EmployeeID);

            var output = new StringBuilder();
            output.Append("<div>\n");
            output.Append("<div class=\"inputpaneltitle\">Car Details</div>");
            output.Append("<table>");
            if (clsEmpCars.GetCarArray(false).Length != 0)
            {
                var generalOptions = this.GeneralOptionsFactory[cMisc.GetCurrentUser().CurrentSubAccountId].WithDutyOfCare();

                cCar car = clsEmpCars.GetCarByID(
                    clsEmpCars.GetDefaultCarID(
                        generalOptions.DutyOfCare.BlockTaxExpiry, 
                        generalOptions.DutyOfCare.BlockMOTExpiry, 
                        generalOptions.DutyOfCare.BlockInsuranceExpiry,
                        generalOptions.DutyOfCare.BlockBreakdownCoverExpiry,
                        false,null));

                if (car == null && clsEmpCars.Cars.Count > 0)
                {
                    car = clsEmpCars.Cars.FirstOrDefault();
                }

                if (car != null)
                {
                    output.Append("<tr><td class=\"labeltd\">Make:</td><td class=\"inputtd\">" + car.make + "</td></tr>");
                    output.Append("<tr><td class=\"labeltd\">Model:</td><td class=\"inputtd\">" + car.model + "</td></tr>");
                    output.Append(
                        "<tr><td class=\"labeltd\">Registration Number:</td><td class=\"inputtd\">" + car.registration
                        + "</td></tr>");
                    output.Append("<tr><td class=\"labeltd\">Engine Type:</td><td class=\"inputtd\">");

                    var engine = VehicleEngineType.Get(new CurrentUser(accountId, employee.EmployeeID, 0, (Modules)module.Id, 0), car.VehicleEngineTypeId);
                    output.Append(engine == null ? "None" : engine.Name);

                    output.Append("</td></tr>");
                    output.Append("<tr><td class=\"labeltd\">Vehicle Journey Rate Categories:</td><td class=\"inputtd\">");
                    if (car.mileagecats.Count != 0)
                    {
                        var mileageCats = new cMileagecats(accountId);
                        foreach (int i in car.mileagecats)
                        {
                            cMileageCat mileage = mileageCats.GetMileageCatById(i);
                            if (mileage != null)
                            {
                                output.Append(mileage.carsize + ", ");
                            }
                        }

                        output.Remove(output.Length - 2, 2);
                    }

                    output.Append("</td></tr>");
                }
            }
            else
            {
                output.Append("<tr><td class=\"labeltd\">Make:</td><td class=\"inputtd\">-</td></tr>");
                output.Append("<tr><td class=\"labeltd\">Model:</td><td class=\"inputtd\">-</td></tr>");
                output.Append("<tr><td class=\"labeltd\">Registration Number:</td><td class=\"inputtd\">-</td></tr>");
                output.Append("<tr><td class=\"labeltd\">Engine Type:</td><td class=\"inputtd\">");
                output.Append("-");
                output.Append("</td></tr>");
                output.Append("<tr><td class=\"labeltd\">Vehicle Journey Rate Category:</td><td class=\"inputtd\">");
                output.Append("-</td></tr>");
            }

            output.Append("</table>");
            output.Append("</div>\n");
            return output.ToString();
        }

        /// <summary>
        /// The create employee contact summary.
        /// </summary>
        /// <param name="reqemp">
        /// The reqemp.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CreateEmployeeContactSummary(Employee reqemp)
        {
            var output = new StringBuilder();
            output.Append("<div>\n");
            output.Append("<div class=\"inputpaneltitle\">Employee Contact Information</div>");
            output.Append("<table>");

            output.Append(
                "<tr><td class=\"labeltd\">Mobile Number:</td><td class=\"inputtd\">" + reqemp.MobileTelephoneNumber + "</td></tr>");
            output.Append("<tr><td class=\"labeltd\">Pager:</td><td class=\"inputtd\">" + reqemp.PagerNumber + "</td></tr>");
            output.Append(
                "<tr><td class=\"labeltd\">Extension Number:</td><td class=\"inputtd\">" + reqemp.TelephoneExtensionNumber
                + "</td></tr>");
            output.Append("</table>");
            output.Append("</div>");
            return output.ToString();
        }

        /// <summary>
        /// The create employement info summary.
        /// </summary>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <param name="reqemp">
        /// The reqemp.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CreateEmployementInfoSummary(int accountid, Employee reqemp)
        {
            var clsemployees = new cEmployees((int)this.ViewState["accountid"]);

            // WHEN MULTIPLE SUB-ACCOUNTS SUPPORTED, THIS WILL NEED PULLING FROM THE IMPORT SPREADSHEET
            int subAccountId = reqemp.DefaultSubAccount;

            var output = new StringBuilder();
            output.Append("<div>\n");
            output.Append("<div class=\"inputpaneltitle\">Employment Information</div>");
            output.Append("<table>");

            output.Append(
                "<tr><td class=\"labeltd\">Purchase Ledger Number:</td><td class=\"inputtd\">" + reqemp.Creditor
                + "</td></tr>");
            output.Append(
                "<tr><td class=\"labeltd\">Position:</td><td class=\"inputtd\">" + reqemp.Position + "</td></tr>");
            output.Append(
                "<tr><td class=\"labeltd\">Payroll Number:</td><td class=\"inputtd\">" + reqemp.PayrollNumber + "</td></tr>");

            output.Append("<tr><td class=\"labeltd\">Line Manager:</td><td class=\"inputtd\">");
            if (reqemp.LineManager != 0)
            {
                Employee linemanager = clsemployees.GetEmployeeById(reqemp.LineManager);
                if (linemanager != null)
                {
                    output.Append(linemanager.Title + " " + linemanager.Forename + " " + linemanager.Surname);
                }
            }

            output.Append("</td></tr>");
            output.Append("<tr><td class=\"labeltd\">Primary Country:</td><td class=\"inputtd\">");

            var clscountries = new cCountries(accountid, subAccountId);
            var clsglobalcountries = new cGlobalCountries();
            cCountry country = clscountries.getCountryById(reqemp.PrimaryCountry);
            if (country != null)
            {
                output.Append(clsglobalcountries.getGlobalCountryById(country.GlobalCountryId).Country);
            }

            output.Append("</td></tr>");
            output.Append("<tr><td class=\"labeltd\">Primary Currency:</td><td class=\"inputtd\">");

            var clsglobalcurrencies = new cGlobalCurrencies();
            var clscurrencies = new cCurrencies(accountid, subAccountId);
            cCurrency currency = clscurrencies.getCurrencyById(reqemp.PrimaryCurrency);
            if (currency != null)
            {
                output.Append(clsglobalcurrencies.getGlobalCurrencyById(currency.globalcurrencyid).label);
            }

            output.Append("</td></tr>");
            output.Append("<tr><td class=\"labeltd\">Department:</td><td class=\"inputtd\">");
            cDepCostItem[] lstCostCodes = reqemp.GetCostBreakdown().ToArray();
            if (lstCostCodes.Length != 0)
            {
                if (lstCostCodes[0].departmentid != 0)
                {
                    var clsdepartments = new cDepartments(accountid);
                    cDepartment department = clsdepartments.GetDepartmentById(lstCostCodes[0].departmentid);
                    if (department != null)
                    {
                        output.Append(department.Department);
                    }
                }
            }

            output.Append("</td></tr>");
            output.Append("<tr><td class=\"labeltd\">Cost Code:</td><td class=\"inputtd\">");
            if (lstCostCodes.Length != 0)
            {
                if (lstCostCodes[0].costcodeid != 0)
                {
                    var clscostcodes = new cCostcodes(accountid);
                    cCostCode costcode = clscostcodes.GetCostcodeById(lstCostCodes[0].costcodeid);
                    if (costcode != null)
                    {
                        output.Append(costcode.Costcode);
                    }
                }
            }

            output.Append("</td></tr>");
            output.Append("<tr><td class=\"labeltd\">Project Code:</td><td class=\"inputtd\">");
            if (lstCostCodes.Length != 0)
            {
                if (lstCostCodes[0].projectcodeid != 0)
                {
                    var clsprojectcodes = new cProjectCodes(accountid);
                    cProjectCode projectcode = clsprojectcodes.getProjectCodeById(lstCostCodes[0].projectcodeid);
                    if (projectcode != null)
                    {
                        output.Append(projectcode.projectcode);
                    }
                }
            }

            output.Append("</td></tr>");
            output.Append("</table>");
            output.Append("</div>\n");
            return output.ToString();
        }

        /// <summary>
        /// The create home address summary.
        /// </summary>
        /// <param name="reqemp">
        /// The reqemp.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CreateHomeAddressSummary(Employee reqemp)
        {
            var output = new StringBuilder();
            output.Append("<div>\n");
            output.Append("<div class=\"inputpaneltitle\">Home Address / Contact Information</div>");
            output.Append("<table>");

            output.Append("<tr><td class=\"labeltd\">Home Telephone:</td><td class=\"inputtd\">" + reqemp.TelephoneNumber + "</td></tr>");
            output.Append("<tr><td class=\"labeltd\">Home Fax:</td><td class=\"inputtd\">" + reqemp.FaxNumber + "</td></tr>");
            output.Append("<tr><td class=\"labeltd\">Home E-mail Address:</td><td class=\"inputtd\">" + reqemp.HomeEmailAddress + "</td></tr>");
            output.Append("</table>");
            output.Append("</div>\n");
            return output.ToString();
        }

        /// <summary>
        /// The create role summary.
        /// </summary>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <param name="reqemp">
        /// The reqemp.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CreateRoleSummary(int accountid, Employee reqemp)
        {
            // TEMPORARY UNTIL EXP USES MULTIPLE SUB-ACCOUNTS
            int subaccid = reqemp.DefaultSubAccount;
            List<int> lstAccessRoles = reqemp.GetAccessRoles().GetBy(subaccid);

            var output = new StringBuilder();
            output.Append("<div>\n");
            output.Append("<div class=\"inputpaneltitle\">Role</div>");
            output.Append("<table>");

            output.Append("<tr><td class=\"labeltd\">Role:</td><td class=\"inputtd\">");
            if (lstAccessRoles.Count > 0)
            {
                var clsAccessRoles = new cAccessRoles(accountid, cAccounts.getConnectionString(accountid));
                foreach (cAccessRole accessRole in lstAccessRoles.Select(clsAccessRoles.GetAccessRoleByID))
                {
                    output.Append(accessRole.RoleName + "<br />");
                }
            }

            output.Append("</td></tr>");
            output.Append("</table>");

            output.Append("</div>\n");
            return output.ToString();
        }

        /// <summary>
        /// The create signoff summary.
        /// </summary>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <param name="reqemp">
        /// The reqemp.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CreateSignoffSummary(int accountid, Employee reqemp)
        {
            var output = new StringBuilder();
            output.Append("<div>\n");
            output.Append("<div class=\"inputpaneltitle\">Signoff Group</div>");
            output.Append("<table>");

            output.Append("<tr><td class=\"labeltd\">Signoff Group:</td><td class=\"inputtd\">");
            if (reqemp.SignOffGroupID != 0)
            {
                var clsgroups = new cGroups(accountid);
                cGroup group = clsgroups.GetGroupById(reqemp.SignOffGroupID);
                if (group != null)
                {
                    output.Append(group.groupname);
                }
            }

            output.Append("</td></tr>");
            output.Append("</table>");
            output.Append("</div>\n");
            return output.ToString();
        }

        /// <summary>
        /// The send activate email.
        /// </summary>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <param name="employeeid">
        /// The employeeid.
        /// </param>
        public void SendActivateEmail(int accountId, Employee employee, IProductModule module)
        {
            var subaccs = new cAccountSubAccounts(accountId);
            cAccountProperties properties = subaccs.getSubAccountById(employee.DefaultSubAccount).SubAccountProperties;

            string subject = "New Employee";
            var output = new StringBuilder();

            output.Append("<html>\n");
            output.Append("<head>\n");
            output.Append("<style>\n");
            output.Append("*\n");
            output.Append("{\n");
            output.Append("font: 12px Arial, Helvetica, sans-serif;\n");
            output.Append("}\n");
            output.Append(".labeltd\n");
            output.Append("{\n");
            output.Append("padding: 2px 3px 2px 7px;\n");
            output.Append("width: 130px;\n");
            output.Append("background-color: #cae8f4;\n");
            output.Append("border: 1px solid #fff;\n");
            output.Append("color: #000;\n");
            output.Append("}\n");
            output.Append(".inputtd\n");
            output.Append("{\n");

            output.Append("padding: 1px 3px 1px 7px;\n");

            output.Append("background-color: #ffffff;\n");
            output.Append("}\n");
            output.Append(".inputpaneltitle\n");
            output.Append("{\n");
            output.Append("border: 1px solid white;\n");
            output.Append("font-size: 1.2em;\n");
            output.Append("font-weight: bold;\n");
            output.Append("background-color: #6280a7;\n");
            output.Append("color: white;\n");
            output.Append("padding: 1px 2px 2px 5px;\n");
            output.Append("}\n");
            output.Append("</style>\n");
            output.Append("</head>\n");
            output.Append("<body>\n");
            output.Append("<div class=\"inputpanel\">\n");
            output.Append("<div class=\"inputpaneltitle\">Name &amp; Logon Details</div>");
            output.Append("<table>\n");
            output.Append("<tr><td class=\"labeltd\">Name:</td><td class=\"inputtd\">" + employee.Title + " " + employee.Forename + " " + employee.Surname + "</td></tr>");
            output.Append("<tr><td class=\"labeltd\">E-mail Address:</td><td class=\"inputtd\">" + employee.EmailAddress + "</td></tr>\n");
            output.Append("<tr><td class=\"labeltd\">Username:</td><td class=\"inputtd\">" + employee.Username + "</td></tr>");
            output.Append("</table>\n");
            output.Append("</div>");
            if (properties.AllowSelfRegEmployeeContact)
            {
                output.Append(this.CreateEmployeeContactSummary(employee));
            }

            if (properties.AllowSelfRegHomeAddress)
            {
                output.Append(this.CreateHomeAddressSummary(employee));
            }

            if (properties.AllowSelfRegEmployeeInfo)
            {
                output.Append(this.CreateEmployementInfoSummary(accountId, employee));
            }

            if (properties.AllowSelfRegRole)
            {
                output.Append(this.CreateRoleSummary(accountId, employee));
            }

            if (properties.AllowSelfRegSignOff)
            {
                output.Append(this.CreateSignoffSummary(accountId, employee));
            }

            if (properties.AllowSelfRegAdvancesSignOff)
            {
                output.Append(this.CreateAdvancesSignoffSummary(accountId, employee));
            }

            if (properties.AllowSelfRegCarDetails)
            {
                output.Append(this.CreateCarSummary(accountId, employee, module));
            }

            output.Append("<table cellspacing=\"5\">");
            output.Append("<tr>");
            output.AppendFormat(
                "<td><a href=\"http{0}://{1}{2}/shared/activate.aspx?accountid={3}&employeeid={4}\">Activate</a></td>", (HttpContext.Current.Request.IsSecureConnection ? "s" : string.Empty), HttpContext.Current.Request.Url.Host, this.Request.ApplicationPath, accountId, employee.EmployeeID);
            output.AppendFormat(
                "<td><a href=\"http{0}://{1}{2}/shared/admin/aeemployee.aspx?employeeid={3}\">Change Details</a></td>", (HttpContext.Current.Request.IsSecureConnection ? "s" : string.Empty), HttpContext.Current.Request.Url.Host, this.Request.ApplicationPath, employee.EmployeeID);
            output.Append("</tr>");
            output.Append("</table>");
            output.Append("</body>");
            output.Append("</html>");
            var employees = new cEmployees(accountId);
            Employee administrator = employees.GetEmployeeById(properties.MainAdministrator);

			string fromAddress;

			if (properties.SourceAddress == 1 && properties.EmailAdministrator != string.Empty)
			{
				fromAddress = properties.EmailAdministrator;
			}
			else
			{
                fromAddress = employee.EmailAddress;
			}
	        
            if (administrator.EmployeeID != 0)
            {
                if (administrator.EmailAddress != string.Empty)
                {
                    var msg = new MailMessage(fromAddress, administrator.EmailAddress, subject, output.ToString());
                    var sender = new EmailSender(properties.EmailServerAddress);

                    msg.IsBodyHtml = true;
                    try
                    {
                        sender.SendEmail(msg);
                        cEventlog.LogEntry(
                            "Email sent to " + administrator.EmailAddress + " regarding verification of " + employee.Username);
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry(
                            "Failed to send verify email to " + administrator.EmailAddress + "\n" + ex.Message + "\n\n"
                            + ex.InnerException);
                    }
                }
                else
                {
                    var clsaccounts = new cAccounts();
                    cAccount reqaccount = clsaccounts.GetAccountByID(accountId);

                    subject = "Main administrator Email address";
                    var strBody = new StringBuilder();

                    strBody.Append("**User Information**\n");
                    strBody.Append("Company ID: " + reqaccount.companyid + "\n");
                    strBody.Append("Account ID: " + reqaccount.accountid + "\n");
                    strBody.Append("Employeeid: " + properties.MainAdministrator + "\n");
                    strBody.Append("Employee Name: " + administrator.Title + " " + administrator.Forename + " " + administrator.Surname + "\n\n");
                    strBody.Append("The main administrator does not have an email address set and will not get notification to activate any new employee added via self registration. \n\n");
                    strBody.Append("The following employee requires activation:\n\n");
                    strBody.Append("Name: " + employee.Title + " " + employee.Forename + " " + employee.Surname + "\n");
                    strBody.Append("E-mail Address: " + employee.EmailAddress + "\n");
                    strBody.Append("Username: " + employee.Username + "\n");

                    var msg = new MailMessage(
						fromAddress, 
                        ConfigurationManager.AppSettings["SupportEmailAddress"], 
                        subject, 
                        strBody.ToString());

                    // System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(properties.EmailServerAddress);
                    var sender = new EmailSender(properties.EmailServerAddress);
                    if (sender.SendEmail(msg) == false)
                    {
                        cEventlog.LogEntry("Failed to send Main administrator Email address email to support\n");
                    }
                }
            }
            else
            {
                // Notify if there is no main administrator set on the company
                var clsaccounts = new cAccounts();
                cAccount reqaccount = clsaccounts.GetAccountByID(accountId);

                subject = "Main administrator not set";
                var strBody = new StringBuilder();

                strBody.Append("**Account Information**\n");
                strBody.Append("Company ID: " + reqaccount.companyid + "\n");
                strBody.Append("Account ID: " + reqaccount.accountid + "\n\n");
                strBody.Append("The main administrator is not set for the company, any notification to activate a new employee added via self registration will not be processed. \n\n");
                strBody.Append("The following employee requires activation:\n\n");
                strBody.Append("Name: " + employee.Title + " " + employee.Forename + " " + employee.Surname + "\n");
                strBody.Append("E-mail Address: " + employee.EmailAddress + "\n");
                strBody.Append("Username: " + employee.Username + "\n");

                var msg = new MailMessage(
					fromAddress, ConfigurationManager.AppSettings["SupportEmailAddress"], subject, strBody.ToString());

                var sender = new EmailSender(properties.EmailServerAddress);
                if (sender.SendEmail(msg) == false)
                {
                    cEventlog.LogEntry("Failed to send Main administrator not set email to support\n");
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack == false)
            {
                if (this.Request.QueryString["employeeid"] == null)
                {
                    return;
                }

                if (this.Request.QueryString["accountid"] == null)
                {
                    return;
                }

                var currentUser = cMisc.GetCurrentUser();

                int accountid = int.Parse(this.Request.QueryString["accountid"]);
                int employeeid = int.Parse(this.Request.QueryString["employeeid"]);
                cEmployees clsemployees = new cEmployees(accountid);

                this.ViewState["accountid"] = accountid;

                Employee employee = clsemployees.GetEmployeeById(employeeid);
                employee.Verified = true;
                employee.Save(null);

                Modules activeModule = HostManager.GetModule(this.Request.Url.Host);
                var module = this.ProductModuleFactory[activeModule];
                this.SendActivateEmail(accountid, employee, module);

                this.litMsgBrand.Text = (module != null) ? module.BrandNameHtml : "Expenses";
                this.litMsgBrand2.Text = this.litMsgBrand.Text;
            }
        }

        #endregion
    }
}