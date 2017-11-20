namespace Spend_Management
{
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using System.Xml.Linq;

    using SpendManagementLibrary;

    public partial class mycars : System.Web.UI.Page
    {
        /// <summary>
        /// The user.
        /// </summary>
        CurrentUser user;

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
            this.Master.title = string.Empty;
            this.Master.PageSubTitle = @"Add Vehicle";
            this.Master.ShowSubMenus = true;
            this.Master.helpid = 1088;
            user = cMisc.GetCurrentUser(User.Identity.Name);

            var subAccounts = new cAccountSubAccounts(user.AccountID);
            cAccountProperties clsproperties = subAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;

            if (clsproperties.AllowUsersToAddCars == false)
            {
                Response.Redirect("~/mydetailsmenu.aspx", true);
            }

            if (IsPostBack == false)
            {
                Master.enablenavigation = false;
                aeCar.ReturnURL = cMisc.Path + "/mydetailsmenu.aspx";
                aeCar.EmployeeAdmin = false;
                aeCar.Action = aeCarPageAction.Add;
                aeCar.SendEmailWhenNewCarAdded = true;
                aeCar.EmployeeID = user.EmployeeID;
                aeCar.AccountID = user.AccountID;
                aeCar.isAeExpenses = false;
                aeCar.ShowStartDateOnly = clsproperties.AllowEmpToSpecifyCarStartDateOnAdd;
                aeCar.ShowDutyOfCare = clsproperties.AllowEmpToSpecifyCarDOCOnAdd;

                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                // load the user's currently active cars so they can choose to replace old with new
                var employeeCars = new cEmployeeCars(user.AccountID, user.EmployeeID);
                aeCar.cmbPreviousCar.Items.AddRange(employeeCars.CreateCurrentValidCarDropDown(DateTime.UtcNow, "You currently have no vehicle to replace").ToArray());
            }

            ClientScript.RegisterClientScriptBlock(this.GetType(), "variables", "var employeeid = " + user.EmployeeID + ";", true);
            ClientScript.RegisterClientScriptBlock(this.GetType(), "variables2", "var nEmployeeid = employeeid;", true);
        }
    }
}
