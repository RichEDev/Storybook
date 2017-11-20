using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;

namespace Spend_Management
{
    using System.Net;

    public partial class helpInformation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            Master.title = "Help &amp; Support Information";
            Title = "Help &amp; Support Information";
            Master.PageSubTitle = Title;

            Master.enablenavigation = false;

            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(currentUser.AccountID);
            cAccountProperties reqProperties = clsSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;

            cModules clsModules = new cModules();

            if (IsPostBack == false)
            {
                litModuleSectionTitle.Text = clsModules.GetModuleByID((int)currentUser.CurrentActiveModule).BrandNamePlainText;

                #region help information
                txtCustomerHelpContactName.Text = reqProperties.CustomerHelpContactName;
                txtCustomerHelpContactTelephone.Text = reqProperties.CustomerHelpContactTelephone;
                txtCustomerHelpContactFax.Text = reqProperties.CustomerHelpContactFax;
                txtCustomerHelpContactAddress.Text = reqProperties.CustomerHelpContactAddress;
                txtCustomerHelpContactEmailAddress.Text = reqProperties.CustomerHelpContactEmailAddress;
                this.txtCustomerHelpInformation.Text = reqProperties.CustomerHelpInformation;
                #endregion help information
                this.txtCustomerHelpInformation.BasePath = GlobalVariables.StaticContentLibrary + "/ckeditor";
                CKEditorExtensions.Configure(this.txtCustomerHelpInformation, EditorMode.Short);
                //no helpinformation element defined yet
                //currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement, true, true);
            }
        }

        /// <summary>
        /// Save Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, ImageClickEventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(currentUser.AccountID);
            cAccountProperties reqProperties = clsSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties.Clone();

            reqProperties.CustomerHelpContactName = txtCustomerHelpContactName.Text;
            reqProperties.CustomerHelpContactTelephone = txtCustomerHelpContactTelephone.Text;
            reqProperties.CustomerHelpContactFax = txtCustomerHelpContactFax.Text;
            reqProperties.CustomerHelpContactAddress = txtCustomerHelpContactAddress.Text;
            reqProperties.CustomerHelpContactEmailAddress = txtCustomerHelpContactEmailAddress.Text;
            reqProperties.CustomerHelpInformation = WebUtility.HtmlDecode(this.txtCustomerHelpInformation.Text);

            clsSubAccounts.SaveAccountProperties(reqProperties, currentUser.EmployeeID, currentUser.isDelegate ? currentUser.Delegate.EmployeeID : (int?)null);

            this.btnCancel_Click(sender, e);
        }

        /// <summary>
        /// Cancel Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancel_Click(object sender, ImageClickEventArgs e)
        {
            string previousUrl = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(previousUrl, true);
        }
    }
}
