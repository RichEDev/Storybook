namespace Spend_Management.shared
{
    using System;
    using System.Collections.Generic;
    using Spend_Management.shared.code;
    using Spend_Management.shared.code.GreenLight;
    using Spend_Management.shared.code.EasyTree;
    using System.Text;
    using System.Linq;

    using BusinessLogic.Modules;

    using SpendManagementLibrary.Employees.DutyOfCare;
    using SpendManagementLibrary;

    /// <summary>
    /// This page displays the Custom menu Icons configured using the Custom Menu tree .
    /// Displays Views under the custom menu as configured on GreenLight Views
    /// </summary>
    public partial class ViewCustomMenu : System.Web.UI.Page
    {
        #region declaration
        /// <summary>
        /// Store Current User 
        /// </summary>
        private CurrentUser user;

        /// <summary>
        /// Store greenlight entities
        /// </summary>
        private cCustomEntities entities = new cCustomEntities();

        /// <summary>
        /// Store single Custom Menu
        /// </summary>
        private CustomMenuStructure customMenu;

        /// <summary>
        /// Store single Custom Node
        /// </summary>
        private CustomMenuNodes customMenuNode=new CustomMenuNodes();

        /// <summary>
        /// General option account Properties
        /// </summary>
        private cAccountProperties accountProperties;
        #endregion

        #region Page Events
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {                
                int menuid = 0;

                if (Request.QueryString["menuid"] != null && !int.TryParse(Request.QueryString["menuid"], out menuid))
                {
                  Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);                   
                }

                this.user = cMisc.GetCurrentUser();
                this.customMenu = new CustomMenuStructure(this.user.AccountID);
                List<CustomMenuStructureItem> menuItems = this.customMenu.GetCustomMenusByParentId(menuid);

                if (menuItems == null)
                {
                    this.Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }                                         
                var customMenuItem = this.customMenu.GetCustomMenuById(menuid);
                this.Title = customMenuItem.CustomMenuName;
                this.Master.title = this.Title;
                this.Master.PageSubTitle = customMenuItem.CustomMenuName;                                         
                this.Master.AddCustomEntityViewMenuIcons(this.user, menuid);
                this.GenerateBreadcrumb(customMenuItem);

                //Display DVLA menu icon                
                var subAccounts = new cAccountSubAccounts(this.user.AccountID);
                var subAccount = subAccounts.getFirstSubAccount();
                this.accountProperties = subAccount.SubAccountProperties.Clone();
                var entityview = new DutyOfCareDocumentsInformation().GetDocEntityAndViewIdByGuid("223018FE-EDAE-408E-8851-C09ABA09DF81", "F95C0754-82FA-493A-97B4-E1ED42B3C337", this.user.AccountID);
                if (customMenuItem.CustomMenuName == "My Duty of Care Documents")
                {                    
                    if (this.user.CurrentActiveModule == Modules.Expenses && !this.user.isDelegate && (this.accountProperties.EnableAutomaticDrivingLicenceLookup && this.user.Account.HasDvlaLookupKeyAndDvlaConnectLicenceElement(SpendManagementElement.DvlaConnect)) && this.user.CheckAccessRole(
                       AccessRoleType.View, CustomEntityElementType.View, Convert.ToInt32(entityview.Split(',')[0]), Convert.ToInt32(entityview.Split(',')[1]), false))
                    {
                        this.Master.addMenuItem(
                            "check",
                            48,
                            "DVLA Check Consent",
                            "Provide your consent for DVLA checks.",
                            "~/shared/information/DrivingLicenceLookupConsent.aspx");
                    }
                }
            }
        }

        #endregion

        #region private methods
        /// <summary>
        /// This method create Breadcrumb for the Views/CustomMenu 
        /// </summary>
        /// <param name="menu"></param>
        private void GenerateBreadcrumb(CustomMenuStructureItem menu)
        {
            StringBuilder breadCrumb = new StringBuilder();
            int breadcrumbLength = 0;
            bool isTruncated = false;
            breadCrumb.Append("<ol class='breadcrumb'>");
            breadCrumb.Append("<li><a title='Home page' href='");           
            breadCrumb.Append("/Home.aspx'> <i><img src='/static/images/expense/menu-icons/bradcrums-dashboard-icon.png'></i>Home</a></li> ");
            breadCrumb.Append(customMenuNode.GenerateCustomMenuBreadcrumb(menu, ref breadcrumbLength, ref isTruncated));
            breadCrumb.Append("<li><label class='breadcrumb_arrow'>/</label>");
            breadCrumb.Append(menu.CustomMenuName);
            breadCrumb.Append("</li></ol>");
            Master.enablenavigation = false;
            Master.LitTitle = breadCrumb.ToString();           
        }        
      
        #endregion
    }
}