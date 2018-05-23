namespace expenses
{
    using System;

    using Spend_Management;

    /// <summary>
    /// The GreenLight admin menu.
    /// </summary>
    public partial class GreenLightAdminMenu : System.Web.UI.Page
    {
        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack == false)
            {
                this.Title = "GreenLight Management";
                this.Master.Title = this.Title;
                
                CurrentUser user = cMisc.GetCurrentUser();
                this.ViewState["accountid"] = user.AccountID;
                this.ViewState["employeeid"] = user.EmployeeID;

                if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CustomEntities, true))
                {
                    this.Master.AddMenuItem(
                        "trafficlight_green",
                        48,
                        "GreenLights",
                        "Create and configure GreenLights for all products.",
                        "~/shared/admin/custom_entities.aspx");
                }

                if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.GreenLightMenu, true))
                {
                    this.Master.AddMenuItem(
                        "elements_tree",
                        48,
                        "Custom Menu Management",
                        "Organise GreenLight forms by creating a custom menu structure.",
                        "~/shared/admin/CustomMenu.aspx");
                }
            }
        }
    }
}