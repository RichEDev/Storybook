namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Web.Services;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using BusinessLogic;
    using SEL.FeatureFlags;
    using SpendManagementLibrary.Employees;
    using Spend_Management;
    using SpendManagementLibrary;
    using Spend_Management.expenses.webservices;
    using Spend_Management.shared.code.ApprovalMatrix;

    /// <summary> 
    /// Summary description for aegroup.
    /// </summary>
    public partial class aeSignoffGroup : Page
    {
        public bool featureFlag = false;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Title = "Add / Edit Signoff Group";
            Master.title = Title;
            Master.showdummymenu = true;
            Master.helpid = 1033;

            if (this.IsPostBack)
            {
                return;
            }

            this.Master.enablenavigation = false;
            CurrentUser user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SignOffGroups, true, true);
            var groups = new cGroups(user.AccountID);

            #region Populate dropdown boxes in modal

            if (user.Account.IsNHSCustomer)
            {
                this.cmbsignofftype.Items.Add(SignoffType.AssignmentSignOffOwner.ToListItem());
                this.DropDownLineManagerAssignmentSupervisor.Items.Add(
                    new ListItem("Assign claim to Assignment Supervisor", "AssignmentSupervisor"));
            }

            // check here for Expedite Receipts
            if (user.Account.ReceiptServiceEnabled)
            {
                this.cmbsignofftype.Items.Add(SignoffType.SELScanAttach.ToListItem());
            }

            // check here for Expedite Validation
            if (user.Account.ValidationServiceEnabled)
            {
                this.cmbincludelst.Items.Add(new ListItem("Only if an expense item fails validation twice", "9"));
                this.cmbsignofftype.Items.Add(SignoffType.SELValidation.ToListItem());
            }

            #endregion

            int groupId = 0;
            int.TryParse(this.Request.QueryString["groupid"], out groupId);

            if (groupId > 0)
            {
                if (groups.getCountOfClaimsInProcessByGroupID(groupId) > 0)
                {
                    this.lblclaimsinprocess.Visible = true;
                }

                cGroup reqgroup = groups.GetGroupById(groupId);
                if (reqgroup == null)
                {
                    this.Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }
                this.txtgroupname.Text = reqgroup.groupname;
                this.txtdescription.Text = reqgroup.description;
                this.chkAllowOneStepAuthorisation.Checked = reqgroup.oneclickauthorisation;
            }

            var signoffGroups = new SignoffGroups();
            var gridData = signoffGroups.CreateStagesGrid(groupId);
            this.litGrid.Text = gridData[1];
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "SignOffStagesVars", cGridNew.generateJS_init("SignOffStagesVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
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

        protected override void OnInitComplete(EventArgs e)
        {
            base.OnInitComplete(e);
        }

    }
}
