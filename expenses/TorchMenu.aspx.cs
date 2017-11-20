using System;
using Spend_Management;

namespace expenses
{
    public partial class TorchMenu : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack == false)
            {
                this.Page.Title = "Torch";
                this.Master.Title = "Torch";
                var user = cMisc.GetCurrentUser();
                this.ViewState["accountid"] = user.AccountID;
                this.ViewState["employeeid"] = user.EmployeeID;

                if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DocumentConfigurations, true))
                {
                    Master.AddMenuItem("document_into", 48, "Document Configurations",
                        "Define and configure report sources and grouping configurations for merging into a Microsoft Word document.",
                        "shared/admin/admindocmergeprojects.aspx");

                }
                
                if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DocumentTemplates, true))
                {
                    Master.AddMenuItem("document_pinned", 48, "Document Templates",
                        "Manage document templates for use with Torch functionality.",
                        "shared/admin/admindoctemplates.aspx");

                }

            }
        }
    }
}