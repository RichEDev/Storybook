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
using System.Web.Services;

using SpendManagementLibrary;
using System.Text;


namespace Spend_Management
{
    /// <summary>
    /// Summary description for checkpaylist.
    /// </summary>
    public partial class checkpaylist : Page
    {
        

        protected void Page_Load(object sender, System.EventArgs e)
        {
            
            Response.Expires = 60;
            Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
            Response.AddHeader("pragma", "no-cache");
            Response.AddHeader("cache-control", "private");
            Response.CacheControl = "no-cache";

            Title = "Check & Pay Expenses";
            Master.title = Title;
            Master.helpid = 1050;
            if (IsPostBack == false)
            {
                
                
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CheckAndPay, true, true);
                
                cClaims clsclaims  = new cClaims(user.AccountID);



                string[] gridData = clsclaims.generateClaimsToCheckGrid(user.EmployeeID, "", 0, user.isDelegate ? user.Delegate.EmployeeID : (int?)null);
                litClaimsGrid.Text = gridData[1];
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ClaimGridVars", cGridNew.generateJS_init("ClaimGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, user.CurrentActiveModule), true);



                gridData = clsclaims.generateUnallocatedClaimsGrid(user.EmployeeID, "", 0, user.isDelegate ? user.Delegate.EmployeeID : (int?)null);
                litUnallocatedGrid.Text = gridData[1];
                Page.ClientScript.RegisterStartupScript(this.GetType(), "UnallocatedClaimGridVars", cGridNew.generateJS_init("UnallocatedClaimGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, user.CurrentActiveModule), true);

                StringBuilder js = new StringBuilder();
                js.Append("SEL.Claims.IDs.txtSurnameId = '" + txtsurname.ClientID + "';\n");
                js.Append("SEL.Claims.IDs.cmbFilterId = '" + cmbfilter.ClientID + "';\n");
                ClientScript.RegisterStartupScript(this.GetType(), "js", js.ToString(), true);
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

        }

        #endregion



        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string sPreviousURL = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(sPreviousURL, true);
        }

        
    }
}

