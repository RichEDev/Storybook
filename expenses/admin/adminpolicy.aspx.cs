namespace expenses.admin
{
    using System;
    using System.Web.UI;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using SpendManagementLibrary;

    using Spend_Management;

    /// <summary>
    /// Summary description for adminpolicy.
    /// </summary>
    public partial class adminpolicy : Page
    {
        protected System.Web.UI.WebControls.ImageButton cmdhelp;

        /// <summary>
        /// An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory { get; set; }

        protected void Page_Load(object sender, System.EventArgs e)
        {

            Title = "Company Policy";
            Master.title = Title;

            Master.helpid = 1027;

            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CompanyPolicy, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                var generalOptions = this.GeneralOptionsFactory[user.CurrentSubAccountId].WithCompanyPolicy();

                switch (generalOptions.CompanyPolicy.PolicyType)
                {
                    case 1:
                        this.optfreeformat.Checked = true;
                        this.SetUpPolicyBox(1);

                        this.txtpolicy.Text = generalOptions.CompanyPolicy.CompanyPolicy;

                        this.reqhtml.Enabled = false;
                        break;
                    case 2:
                        this.SetUpPolicyBox(2);
                        this.reqhtml.Enabled = true;
                        this.opthtml.Checked = true;
                        this.reqpdf.Enabled = false;
                        this.optPdf.Checked = false;
                        break;
                    case 3:
                        this.SetUpPolicyBox(3);
                        this.reqhtml.Enabled = false;
                        this.opthtml.Checked = false;
                        this.reqpdf.Enabled = true;
                        this.optPdf.Checked = true;
                        break;
                }


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
            this.cmdcancel.Click += new ImageClickEventHandler(cmdcancel_Click);
        }


        #endregion

        protected void optfreeformat_CheckedChanged(object sender, System.EventArgs e)
        {
            int accountid = (int)ViewState["accountid"];
            cMisc clsmisc = new cMisc(accountid);

            CurrentUser user = cMisc.GetCurrentUser();
            var generalOptions = this.GeneralOptionsFactory[user.CurrentSubAccountId].WithCompanyPolicy();

            clsmisc.ChangePolicyType(1);
            this.SetUpPolicyBox(1);

            this.txtpolicy.Text = generalOptions.CompanyPolicy.CompanyPolicy;

            this.reqhtml.Enabled = false;
            this.reqpdf.Enabled = false;
        }

        private void SetUpPolicyBox(int policytype)
        {
            switch (policytype)
            {
                case 1:
                    this.optfreeformat.Checked = true;
                    this.lblenter.Visible = false;
                    this.txtpolicy.TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine;
                    this.txtpolicy.Height = System.Web.UI.WebControls.Unit.Pixel(300);
                    this.txtpolicy.Width = System.Web.UI.WebControls.Unit.Pixel(800);
                    this.polfile.Visible = false;
                    this.pdffile.Visible = false;
                    this.txtpolicy.Visible = true;
                    break;
                case 2:
                    this.lblenter.Visible = true;
                    this.txtpolicy.Visible = false;
                    this.polfile.Visible = true;
                    this.pdffile.Visible = false;
                    this.FormatLabel("HTML");
                    break;
                case 3:
                    this.lblenter.Visible = true;
                    this.txtpolicy.Visible = false;
                    this.polfile.Visible = false;
                    this.pdffile.Visible = true;
                    this.FormatLabel("PDF");
                    break;
            }
        }

        protected void opthtml_CheckedChanged(object sender, System.EventArgs e)
        {
            int accountid = (int)ViewState["accountid"];
            cMisc clsmisc = new cMisc(accountid);
            clsmisc.ChangePolicyType(2);
            this.txtpolicy.Text = "";
            this.reqhtml.Enabled = true;
            this.FormatLabel("HTML");
            this.SetUpPolicyBox(2);
        }

        protected void optpdf_CheckedChanged(object sender, EventArgs e)
        {
            int accountid = (int)ViewState["accountid"];
            cMisc clsmisc = new cMisc(accountid);
            clsmisc.ChangePolicyType(3);
            this.txtpolicy.Text = "";
            this.reqpdf.Enabled = true;
            this.FormatLabel("PDF");
            this.SetUpPolicyBox(3);
        }

        private void FormatLabel(string fileType)
        {
            this.lblenter.Text = string.Format("Please enter the location of the {0} file", fileType);
        }

        private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

            int accountid = (int)ViewState["accountid"];
            string uploadfile = "";
            cMisc clsmisc = new cMisc(accountid);


            byte policytype = 0;
            string policy;

            if (optfreeformat.Checked == true)
            {
                policytype = 1;
                policy = txtpolicy.Text;
                clsmisc.UpdatePolicy(policy, policytype);
                lblmsg.Text = "The policy has been updated successfully.";


            }
            else
            {
                cAccounts clsaccounts = new cAccounts();
                cAccount reqaccount = clsaccounts.GetAccountByID(accountid);
                if (this.optPdf.Checked)
                {
                    uploadfile = this.Server.MapPath("../policies/" + reqaccount.companyid + ".pdf");
                    policytype = 3;
                    policy = string.Empty;
                    this.pdffile.PostedFile.SaveAs(uploadfile);
                }
                else
                {
                    uploadfile = this.Server.MapPath("../policies/" + reqaccount.companyid + ".htm");
                    policytype = 2;
                    this.polfile.PostedFile.SaveAs(uploadfile);
                    policy = clsmisc.ReadServerDocument(uploadfile).Replace("\r\n", "");
                    
                }

                clsmisc.UpdatePolicy(policy, policytype);
            }
            
            this.Response.Redirect("../policymenu.aspx", true);
        }

        private void cmdcancel_Click(object sender, ImageClickEventArgs e)
        {
            this.Response.Redirect("../policymenu.aspx", true);
        }
    }
}
