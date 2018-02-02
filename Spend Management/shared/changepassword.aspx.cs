namespace Spend_Management
{
    using System;
    using System.Text;
    using System.Web.UI;
    using Common.Cryptography;
    using shared.code.Authentication;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;

    /// <summary>
    /// Summary description for changepassword.
    /// </summary>
    public partial class changepassword : Page
    {
        private ICurrentUser user;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.Title = "Change Password";
            this.Master.PageSubTitle = Title;
            
            this.Master.enablenavigation = false;

            this.user = cMisc.GetCurrentUser();
            

            switch (this.user.CurrentActiveModule)
            {
                case Modules.contracts:
                    this.Master.helpid = 1003;
                    break;
                default:
                    this.Master.helpid = 1164;
                    break;
            }

            if (this.IsPostBack == false)
            {
                EncryptorFactory.SetCurrent(new HashEncryptorFactory());

                int employeeid = 0;

                int returnto = 0;
                if (this.Request["b"] != null)
                {
                    var clssecure = new cSecureData();
                    int accountid = int.Parse(clssecure.Decrypt(this.Request["a"]));
                    employeeid = int.Parse(clssecure.Decrypt(this.Request["b"]));
                    System.Web.Security.FormsAuthentication.RedirectFromLoginPage(
                        accountid + "," + employeeid.ToString(), false);
                }

                this.ViewState["accountid"] = this.user.AccountID;
                this.ViewState["employeeid"] = this.user.EmployeeID;


                employeeid = int.Parse(this.Request["employeeid"]);
                returnto = !this.user.CheckUserHasAccesstoWebsite() ? 3 : int.Parse(this.Request["returnto"]);

                this.ViewState["returnto"] = returnto;
                this.ViewState["employeeid"] = employeeid;    

                if (returnto == 1 || returnto == 3)
                {
                    if (returnto == 1)
                    {
                        this.divTwoCol.Visible = false;
                        this.spanOldPassword.Visible = false;
                        this.txtold.Visible = false;
                        this.lblold.Visible = false;
                        this.txtold.Enabled = false;
                        this.reqold.Enabled = false;
                    }

                    if (this.Request["change"] != null)
                    {
                        this.lbltodo.Text = "Your password has expired and must be changed.";
                    }
                    else
                    {
                        this.txtold.Visible = false;
                        this.lblold.Visible = false;
                        this.txtold.Enabled = false;
                        this.reqold.Enabled = false;
                        this.divTwoCol.Visible = false;
                        this.spanOldPassword.Visible = false;
                        this.spanOldPassword.Style.Add(HtmlTextWriterStyle.Display, "none");
                        this.divTwoCol.Style.Add(HtmlTextWriterStyle.Display, "none");
                        this.lbltodo.Text = "Please enter a new password for the employee";
                    }
                }
                else
                {
                    this.lbltodo.Text = "Please enter your current password and then a new password in the boxes provided.";
                }

                this.litpolicy.Text = createPolicy();
            }
        }

        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdok.Click += new System.Web.UI.ImageClickEventHandler(this.cmdok_Click);
            this.cmdcancel.Click += new System.Web.UI.ImageClickEventHandler(this.cmdcancel_Click);

        }

        #endregion

        private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            int returnto = (int)ViewState["returnto"];
            switch (returnto)
            {
                case 1:
                    Response.Redirect(
                        "~/shared/admin/adminemployees.aspx?surname=" + Request.QueryString["surname"] + "&roleid="
                        + Request.QueryString["roleid"] + "&groupid=" + Request.QueryString["groupid"] + "&costcodeid="
                        + Request.QueryString["costcodeid"] + "&departmentid=" + Request.QueryString["departmentid"],
                        true);
                    break;
                case 2:
                    Response.Redirect(Session["PreviousPageUrl"].ToString(), true);
                    break;
                case 3:
                    Session.Remove("ChangePasswordUserId");
                    Response.Redirect("~/shared/logon.aspx", true);
                    break;
            }
        }

        private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            byte returncode = 0;
            cEmployees clsemployees = new cEmployees((int)ViewState["accountid"]);
            string password = this.txtnew.Text;


            int employeeid = (int)this.ViewState["employeeid"];

            int queryStringEmployeeID = -1;

            if (Request.QueryString["employeeid"] != null && Request.QueryString["employeeid"] != "")
            {
                queryStringEmployeeID = Convert.ToInt32(Request.QueryString["employeeid"]);
            }

            int returnto = (int)this.ViewState["returnto"];

            if (user.EmployeeID == employeeid || user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Employees, true) || (queryStringEmployeeID > -1 && queryStringEmployeeID == user.EmployeeID))
            {
                byte checkpwd;
                Employee employee = clsemployees.GetEmployeeById(employeeid);
                cAccountSubAccounts subaccs = new cAccountSubAccounts((int)ViewState["accountid"]);
                CurrentUser curUser = cMisc.GetCurrentUser();
                cAccountProperties clsproperties = (curUser.Employee.DefaultSubAccount >= 0 ? subaccs.getSubAccountById(curUser.Employee.DefaultSubAccount).SubAccountProperties : subaccs.getFirstSubAccount().SubAccountProperties);

                // returnto = 1 when you change password from employees in user management
                // returnto = 2 when you change password from mydetails
                // returnto = 3 when you change password from forgotten details
                // returnto = 4 when you change password from quick menu
                switch (returnto)
                {
                    case 1:
                    case 3:
                        checkpwd = clsemployees.checkpassword(password, (int)this.ViewState["accountid"], employeeid,EncryptorFactory.CreateEncryptor());
                        returncode = employee.ChangePassword(txtold.Text, txtnew.Text, false, checkpwd, clsproperties.PwdHistoryNum, curUser, EncryptorFactory.CreateEncryptor());
                        break;
                    case 2:
                        checkpwd = clsemployees.checkpassword(password, (int)this.ViewState["accountid"], employeeid,EncryptorFactory.CreateEncryptor());
                        returncode = employee.ChangePassword(txtold.Text, txtnew.Text, true, checkpwd, clsproperties.PwdHistoryNum, curUser, EncryptorFactory.CreateEncryptor());
                        break;
                }

                switch (returncode)
                {
                    case 1:
                        lblmsg.Text = "<br /><br />The password could not be changed because the old password you have entered is incorrect.";
                        lblmsg.Visible = true;
                        return;

                    case 2:
                        lblmsg.Text = "<br /><br />The password could not be changed as it does not conform to the password policy.";
                        lblmsg.Visible = true;
                        return;

                }

                if (!string.IsNullOrEmpty(Request.QueryString["resetKey"]))
                {
                    clsemployees.RemovePasswordKey(Request.QueryString["resetKey"]);
                }

                switch (returnto)
                {
                    case 1:
                        Session.Remove("ChangePasswordUserId");
                        Response.Redirect("~/shared/admin/selectemployee.aspx", true);
                        break;
                    case 2:
                        Response.Redirect(Session["PreviousPageUrl"].ToString(), true);
                        break;
                    case 3:
                        Session.Remove("ChangePasswordUserId");
                        var employeeCars = new cEmployeeCars((int)ViewState["accountid"], employee.EmployeeID);
                        if (Logon.CheckOdometerReadingsRequired(employeeCars, clsproperties.RecordOdometer, clsproperties.EnterOdometerOnSubmit, clsproperties.OdometerDay))
                        {
                            Session["OdometerReadingsOnLogon"] = true;
                            Response.Redirect("~/odometerreading.aspx?odotype=1", true);
                        }
                        else
                        {
                        Response.Redirect("~/home.aspx", true);
                        }

                        break;
                }
            }
            else
            {
                lblmsg.Text = "<br /><br />You do not have access to perform this update.";
                lblmsg.Visible = true;
                return;
            }

        }

        private string createPolicy()
        {
            cAccountSubAccounts subaccs = new cAccountSubAccounts((int)ViewState["accountid"]);
            CurrentUser curUser = cMisc.GetCurrentUser();

            var subaccountid = curUser.Employee.DefaultSubAccount >= 0
                                   ? curUser.Employee.DefaultSubAccount
                                   : subaccs.getFirstSubAccount().SubAccountID;

            var subAccounts = new cAccountSubAccounts(curUser.AccountID);
            var passwordPolicyTexts = subAccounts.getSubAccountById(subaccountid).PasswordPolicyText;

            var output = new StringBuilder();

            foreach (var policy in passwordPolicyTexts)
            {
                output.Append("<li>");
                output.Append(policy);
                output.Append("</li>");
            }

            if (output.Length <= 0)
            {
                return output.ToString();
            }

            output.Insert(0, "<ul>");
            output.Insert(0, "<div class=\"sectiontitle\">Password Policy</div>");

            output.Append("</ul>");
            output.Append("</div>");

            return output.ToString();
        }
    }
}