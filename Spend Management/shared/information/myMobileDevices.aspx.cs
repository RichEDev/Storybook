using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using SpendManagementHelpers;
using System.Text;
using SpendManagementLibrary;

namespace Spend_Management
{
    /// <summary>
    /// myMobileDevices
    /// </summary>
    public partial class myMobileDevices : System.Web.UI.Page
    {
        /// <summary>
        /// Main page load method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "My Mobile Devices";
            Master.PageSubTitle = "My Mobile Devices";
            Master.helpid = 1093;

            Master.UseDynamicCSS = true;

            if (!this.IsPostBack)
            {
                // check access role permits page access
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.MobileDevices, true, true);

                // ensure it uses current user
                usrMobileDevices.DevicesEmployeeID = -1;

                cAccountSubAccounts subaccs = new cAccountSubAccounts(user.AccountID);
                cAccountProperties properties = subaccs.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;

                if(!properties.UseMobileDevices)
                {
                    Response.Redirect("~/shared/restricted.aspx?reason=Current%20access%20role%20does%20not%20permit%20you%20to%20view%20this%20page.", true);
                }
            }
        }
        
        /// <summary>
        /// Handles the click of the Close button and navigates to be parent breadcrumb in the web sitemap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClose_Click(object sender, EventArgs e)
        {
            Response.Redirect((SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url, true);
        }
    }
}
