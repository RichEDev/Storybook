using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using SpendManagementLibrary;
using Spend_Management;

namespace expenses.admin
{
    /// <summary>
    /// Summary description for companydetails.
    /// </summary>
    public partial class companydetails : Page
    {
        protected System.Web.UI.WebControls.ImageButton ImageButton3;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Title = "Company Details";
            Master.Page.Title = Title;
            Master.PageSubTitle = Title;
            Master.UseDynamicCSS = true;
            Master.helpid = 1038;

            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CompanyDetails, true, true);
                this.ScriptManagerProxy1.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/shortcut.js"));
                this.ScriptManagerProxy1.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery-ui.datepicker-en-gb.js"));
                this.ScriptManagerProxy1.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.ui.timepicker-0.3.2.js"));
                this.ScriptManagerProxy1.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.ui.multiselect.js"));
                this.ScriptManagerProxy1.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/json2.min.js"));
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cMisc clsmisc = new cMisc(user.AccountID);

                sCompanyDetails reqcomp = clsmisc.GetCompanyDetails();
                txtcompanyname.Text = reqcomp.companyname;
                txtaddress1.Text = reqcomp.address1;
                txtaddress2.Text = reqcomp.address2;
                txtcity.Text = reqcomp.city;
                txtcounty.Text = reqcomp.county;
                txtpostcode.Text = reqcomp.postcode;
                txttelno.Text = reqcomp.telno;
                txtfaxno.Text = reqcomp.faxno;
                txtemail.Text = reqcomp.email;
                txtbankref.Text = reqcomp.bankref;
                txtaccountno.Text = reqcomp.accoutno;
                txtaccounttype.Text = reqcomp.accounttype;
                txtsortcode.Text = reqcomp.sortcode;
                txtcompanynumber.Text = reqcomp.companynumber;

                string[] grid = generateGrid(user);
                litFinancialYearGrid.Text = grid[1];
                Page.ClientScript.RegisterStartupScript(this.GetType(), "gridFinancialYearsVars", cGridNew.generateJS_init("gridFinancialYearsVars", new List<string> { grid[0] }, user.CurrentActiveModule), true);

            }
        }

        private string[] generateGrid(ICurrentUser user)
        {
            cGridNew grid = new cGridNew(user.AccountID, user.EmployeeID, "gridFinancialYears", "select FinancialYearID, [Description], YearStart, yearend, Active, Primary from FinancialYears");
            grid.KeyField = "FinancialYearID";
            grid.getColumnByName("FinancialYearID").hidden = true;
            grid.getColumnByName("YearStart").CustomDateFormat = "dd/MM";
            grid.getColumnByName("YearEnd").CustomDateFormat = "dd/MM";
            grid.enableupdating = true;
            grid.enabledeleting = true;
            grid.editlink = "javascript:SEL.CompanyDetails.Year.Edit({FinancialYearID});";
            grid.deletelink = "javascript:SEL.CompanyDetails.Year.Delete({FinancialYearID});";
            return grid.generateGrid();
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
            this.btnok.Click += new EventHandler(this.cmdok_Click);
            this.btncancel.Click += new EventHandler(this.cmdcancel_Click);
        }

        #endregion

        private void cmdok_Click(object sender, EventArgs e)
        {

            cMisc clsmisc = new cMisc((int)ViewState["accountid"]);
            string companyname, address1, address2, city, county, postcode, telno, faxno, email;
            string bankref, accountno, accounttype, sortcode;
            string companynumber = txtcompanynumber.Text;
            companyname = txtcompanyname.Text;
            address1 = txtaddress1.Text;
            address2 = txtaddress2.Text;
            city = txtcity.Text;
            county = txtcounty.Text;
            postcode = txtpostcode.Text;
            telno = txttelno.Text;
            faxno = txtfaxno.Text;
            email = txtemail.Text;
            bankref = txtbankref.Text;
            accountno = txtaccountno.Text;
            accounttype = txtaccounttype.Text;
            sortcode = txtsortcode.Text;

            clsmisc.UpdateCompanyDetails(companyname, address1, address2, city, county, postcode, telno, faxno, email, bankref, accountno, accounttype, sortcode, companynumber);
            lblmsg.Text = "The Company Details have been updated successfully.";
            lblmsg.Visible = true;

            string sPreviousURL = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;
            Response.Redirect(sPreviousURL, true);
        }

        private void cmdcancel_Click(object sender, EventArgs e)
        {
            string sPreviousURL = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;
            Response.Redirect(sPreviousURL, true);
        }
    }
}
