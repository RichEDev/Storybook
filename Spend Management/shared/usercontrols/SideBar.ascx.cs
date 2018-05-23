namespace Spend_Management.shared.usercontrols
{
    using System;
    using System.Text;
    using System.Web.UI.WebControls;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;
    using BusinessLogic.Modules;

    using SpendManagementLibrary;
    using Spend_Management.shared.code;

    public partial class SideBar : System.Web.UI.UserControl
    {
        /// <summary>
        /// An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory { get; set; }

        /// <summary>
        /// Gets & Sets the user Name.
        /// </summary>
        public string Lituser
        {
            get { return this.lituser.Text; }
            set { this.lituser.Text = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
               if (Request.UrlReferrer != null)
                {
                    Session["PreviousPageUrl"] = Request.UrlReferrer.ToString();
                }

                this.LoadMenuItems();
            }
        }

        /// <summary>
        /// Load menu items on pageload.
        /// </summary>
        private void LoadMenuItems()
        {
            var menuItems = new StringBuilder();
            var user = cMisc.GetCurrentUser();
            var usingExpenses = user.CurrentActiveModule == Modules.Expenses;
            var usingFramework = user.CurrentActiveModule == Modules.Contracts;
            
            var accountSubAccounts = new cAccountSubAccounts(user.Account.accountid);
            cAccountProperties accountProperties = accountSubAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;

            menuItems.Append(this.CreateMenuItems("/home.aspx", "dashboard-icon.png", "Home"));
            if (usingExpenses)
            {
                menuItems.Append(this.CreateMenuItems("/aeexpense.aspx", "expenses-icon.png", "Add New Expenses"));              
            }

            if (usingFramework)
            {
                userIcon.Src = "/icons/framework_user.png";
                if (AccessRoleCheck.FWCanAccessAdminSettings(user, accountProperties))
                {
                    menuItems.Append(this.CreateMenuItems("/MenuMain.aspx?menusection=admin", "Administration-icon.png", "Administrative Settings"));
                }

                if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Employees, true))
                {
                    menuItems.Append(this.CreateMenuItems("/shared/admin/selectemployee.aspx", "employees.png", "Employees"));
                }

                ListItem[] subAccounts = accountSubAccounts.CreateFilteredDropDown(user.Employee, user.CurrentSubAccountId);
                if (subAccounts.Length > 1 || (subAccounts.Length == 1 && subAccounts[0].Value != user.CurrentSubAccountId.ToString()))
                {
                    menuItems.Append(this.CreateMenuItems("javascript:ShowSubAccountModal();", "switch_accounts.png", "Switch Sub Account"));
                }

                menuItems.Append(this.CreateMenuItems("/About.aspx?contractId=" + this.ViewState["ActiveContract"], "about_fw.png", "About"));
            }
            else
            {
                userIcon.Src = GlobalVariables.StaticContentLibrary + "/images/expense/menu-icons/user2-160x160.jpg";
                if (AccessRoleCheck.CanAccessAdminSettings(user, this.GeneralOptionsFactory))
                {
                    menuItems.Append(this.CreateMenuItems("/adminmenu.aspx", "Administration-icon.png", "Administrative Settings"));
                }

                if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Employees, true))
                {
                    menuItems.Append(this.CreateMenuItems("/shared/admin/selectemployee.aspx", "employees.png", "Employees"));
                }

                if (user.CurrentActiveModule != Modules.CorporateDiligence)
                {
                    menuItems.Append(this.CreateMenuItems("javascript:showpolicy('/policy.aspx')", "Copy-Policy-icon.png", "Company Policy"));
                }
            }

            menuItems.Append(this.CreateMenuItems("/shared/helpAndSupport.aspx", "Help-&-Support-icon.png", "Help & Support"));
            menuItems.Append(this.CreateMenuItems(string.Format("/shared/changepassword.aspx?returnto=2&employeeid={0}", user.EmployeeID), "edit_profile.png", "Change Password"));
            menuItems.Append(this.CreateMenuItems("javascript:exit();", "logout_small.png", "Log Out"));

            this.ltMenuItems.Text = menuItems.ToString();
        }

        /// <summary>
        /// Create menu items with standard format.
        /// </summary>
        private StringBuilder CreateMenuItems(string href, string iconName, string itemName)
        {
            StringBuilder itemBuilder = new StringBuilder();
            itemBuilder.AppendFormat("<li><a href=\"{0}\"><img src=\"" + GlobalVariables.StaticContentLibrary + "/images/expense/menu-icons/{1}\"  alt=\"\" /><p>{2}</p></a></li>", href, iconName, itemName);
            return itemBuilder;
        }
    }
}