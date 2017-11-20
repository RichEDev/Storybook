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
using SpendManagementLibrary.Employees;
using Spend_Management;
using System.Collections.Generic;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for aebudget.
    /// </summary>
    public partial class aebudget : Page
    {
        protected System.Web.UI.WebControls.ImageButton cmdhelp;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Master.PageSubTitle = "Budget Holders";
            Master.enablenavigation = false;
            Master.helpid = 1030;
            cmdok.Attributes.Add("onclick", "if (validateform(null) == false) {return;}");
            CurrentUser user = cMisc.GetCurrentUser();
            cEmployees clsemployees = new cEmployees(user.AccountID);
            cTables clsTables = new cTables(user.AccountID);

            //string varScript = clsemployees.createEmployeeControl(ref placeEmp, "budgetemployeeid", "budgetemployee", EmployeeAreaType.BudgetEmployee, false);
            //ClientScript.RegisterClientScriptBlock(this.GetType(), "script", varScript, true);

            // bind the jQuery auto complete to the txtUser field
            List<object> acParams = AutoComplete.getAutoCompleteQueryParams("employees");
            string acBindStr = AutoComplete.createAutoCompleteBindString("txtUser", 15, clsTables.GetTableByName("employees").TableID, (Guid)acParams[0], (List<Guid>)acParams[1], 500);

            ClientScriptManager scmgr = this.ClientScript;
            scmgr.RegisterStartupScript(this.GetType(), "autocompleteBind", AutoComplete.generateScriptRegisterBlock(new List<string>() { acBindStr }), true);

            if (IsPostBack == false)
            {
                Master.enablenavigation = false;

                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.BudgetHolders, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                int budgetholderid = 0;
                if (Request.QueryString["budgetholderid"] != null)
                {
                    budgetholderid = Convert.ToInt32(Request.QueryString["budgetholderid"]);
                }
                ViewState["budgetholderid"] = budgetholderid;

                if (budgetholderid > 0) //update
                {
                    cBudgetholders clsholders = new cBudgetholders(user.AccountID);
                    cBudgetHolder reqholder = clsholders.getBudgetHolderById(budgetholderid);
                    Employee reqemp = clsemployees.GetEmployeeById(reqholder.employeeid);
                    txtUser.Text = reqemp.FullName;
                    txtUser_ID.Text = reqemp.EmployeeID.ToString();

                    txtlabel.Text = reqholder.budgetholder;
                    txtdescription.Text = reqholder.description;
                    Master.title = "Budget Holder: " + reqholder.budgetholder;
                }
                else
                {
                    Master.title = "Budget Holder: New";
                    txtUser_ID.Text = "0";
                }
                Title = Master.title;
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

        private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string costcode;
            string description;
            int employeeid = 0;

            int budgetholderid = (int)ViewState["budgetholderid"];
            costcode = txtlabel.Text;
            description = txtdescription.Text;

            int.TryParse(txtUser_ID.Text, out employeeid);

            int returnvalue;

            cBudgetholders clsholders = new cBudgetholders((int)ViewState["accountid"]);

            if (description.Length > 3999)
            {
                description = description.Substring(0, 3999);
            }
            cBudgetHolder holder = new cBudgetHolder(budgetholderid, costcode, description, employeeid, null, null, null, null);
            returnvalue = clsholders.saveBudgetHolder(holder);

            if (returnvalue == -1)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "alert", "alert('The budget holder name you have entered already exists')", true);
            }
            else
            {
                Response.Redirect("adminbudget.aspx", true);
            }
        }

        private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Redirect("adminbudget.aspx", true);
        }
    }
}