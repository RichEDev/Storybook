//===========================================================================
// This file was modified as part of an ASP.NET 2.0 Web project conversion.
// The class name was changed and the class modified to inherit from the abstract base class 
// in file 'App_Code\Migrated\Stub_process_aspx_cs.cs'.
// During runtime, this allows other classes in your web application to bind and access 
// the code-behind page using the abstract base class.
// The associated content page 'process.aspx' was also modified to refer to the new class name.
// For more information on this code pattern, please refer to http://go.microsoft.com/fwlink/?LinkId=46995 
//===========================================================================
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using SpendManagementLibrary;
using System.Web.Security;

namespace Spend_Management
{
    using BusinessLogic.Modules;

    /// <summary>
    /// Summary description for process.
    /// </summary>
    public partial class process : Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (IsPostBack == false)
            {
                int claimid = 0;
                int process = 0;
                int employeeid;
                int expenseid = 0;
                int returnto = 0;
                process = int.Parse(Request.QueryString["process"]);
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;


                cClaims clsclaims;
                cClaim reqclaim;
                switch (process)
                {
                    case 1: //logoff
                        cAuditLog clsaudit = new cAuditLog(user.AccountID, user.EmployeeID);
                        clsaudit.recordLogout();

                        switch (user.CurrentActiveModule)
                        {
                            case Modules.Contracts:
                            case Modules.SpendManagement:
                            case Modules.SmartDiligence:
                                // remove concurrent user manage
                                HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                                Guid manageID = Guid.Empty;
                                Guid.TryParse(authTicket.UserData, out manageID);
                                cConcurrentUsers.LogoffUser(manageID, user.AccountID);
                                break;
                        }

                        Session.Abandon();
                        FormsAuthentication.SignOut();

                        Response.Cookies["myid"].Value = null;

                        var sso = SingleSignOn.Get(user);

                        if (Request.Cookies["SSO"] != null && Request.Cookies["SSO"].Value == "1" && sso != null && !String.IsNullOrEmpty(sso.ExitUrl))
                        {
                            Response.Redirect(sso.ExitUrl, true);
                        }
                        else
                        {
                            Response.Redirect("~/shared/logon.aspx", true);
                        }
                        break;

                    case 5: //log of emp account
                        if (Session["myid"] != null)
                        {
                            cAuditLog log = new cAuditLog();
                            log.recordLogout();

                            Session.Abandon();
                            employeeid = (int)Session["myid"];

                            FormsAuthentication.RedirectFromLoginPage(user.AccountID + "," + employeeid.ToString(), false);

                            this.Session.Remove("myid");
                            this.Session.Remove("delegatetype");
                        }
                        else
                        {
                            Response.Redirect("~/home.aspx", true);
                        }
                        Response.Redirect("~/shared/admin/emplogon.aspx", true);
                        break;
                    case 6:
                        System.Text.StringBuilder output = new System.Text.StringBuilder();
                        cColours clscolours = new cColours(user.AccountID, user.CurrentSubAccountId, user.CurrentActiveModule);
                        output.Append("<style type\"text/css\">\n");
                        if (clscolours.headerBGColour != clscolours.defaultHeaderBGColour)
                        {
                            output.Append(".infobar\n");
                            output.Append("{\n");
                            output.Append("background-color: " + clscolours.headerBGColour + ";\n");
                            output.Append("}\n");
                            output.Append(".datatbl th\n");
                            output.Append("{\n");
                            output.Append("background-color: " + clscolours.headerBGColour + ";\n");
                            output.Append("}\n");
                            output.Append(".inputpaneltitle\n");
                            output.Append("{\n");
                            output.Append("background-color: " + clscolours.headerBGColour + ";\n");
                            output.Append("border-color: " + clscolours.headerBGColour + ";\n");
                            output.Append("}\n");
                            output.Append(".paneltitle\n");
                            output.Append("{\n");
                            output.Append("background-color: " + clscolours.headerBGColour + ";\n");
                            output.Append("border-color: " + clscolours.headerBGColour + ";\n");
                            output.Append("}\n");
                            output.Append(".homepaneltitle\n");
                            output.Append("{\n");
                            output.Append("background-color: " + clscolours.headerBGColour + ";\n");
                            output.Append("border-color: " + clscolours.headerBGColour + ";\n");
                            output.Append("}\n");
                        }
                        if (clscolours.rowBGColour != clscolours.defaultRowBGColour)
                        {
                            output.Append(".datatbl .row1\n");
                            output.Append("{\n");
                            output.Append("background-color: " + clscolours.rowBGColour + ";\n");
                            output.Append("}\n");
                            output.Append(".calendar .row1\n");
                            output.Append("{\n");
                            output.Append("background-color: " + clscolours.rowBGColour + ";\n");
                            output.Append("}\n");
                        }
                        if (clscolours.rowTxtColour != clscolours.defaultRowTxtColour)
                        {
                            output.Append(".datatbl .row1\n");
                            output.Append("{\n");
                            output.Append("color: " + clscolours.rowTxtColour + ";\n");
                            output.Append("}\n");
                            output.Append(".calendar .row1\n");
                            output.Append("{\n");
                            output.Append("color: " + clscolours.rowTxtColour + ";\n");
                            output.Append("}\n");
                        }
                        if (clscolours.altRowBGColour != clscolours.defaultAltRowBGColour)
                        {
                            output.Append(".datatbl .row2\n");
                            output.Append("{\n");
                            output.Append("background-color: " + clscolours.altRowBGColour + ";\n");
                            output.Append("}\n");
                            output.Append(".calendar .row2\n");
                            output.Append("{\n");
                            output.Append("background-color:" + clscolours.altRowBGColour + ";\n");
                            output.Append("}\n");
                        }
                        if (clscolours.altRowTxtColour != clscolours.defaultAltRowTxtColour)
                        {
                            output.Append(".datatbl .row2\n");
                            output.Append("{\n");
                            output.Append("color: " + clscolours.altRowTxtColour + ";\n");
                            output.Append("}\n");
                            output.Append(".calendar .row2\n");
                            output.Append("{\n");
                            output.Append("color:" + clscolours.altRowTxtColour + ";\n");
                            output.Append("}\n");
                        }
                        if (clscolours.fieldTxtColour != clscolours.defaultFieldTxt)
                        {
                            output.Append(".labeltd\n");
                            output.Append("{\n");
                            output.Append("color: " + clscolours.fieldTxtColour + ";\n");
                            output.Append("}\n");
                        }
                        output.Append("</style>");
                        Response.Write(output.ToString());
                        Response.Flush();
                        Response.End();
                        break;


                }
            }

        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
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

        }
        #endregion
    }
}
