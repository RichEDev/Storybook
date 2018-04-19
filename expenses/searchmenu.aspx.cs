using System;

using BusinessLogic;
using BusinessLogic.DataConnections;
using BusinessLogic.GeneralOptions;

using Spend_Management;

using SpendManagementLibrary;

public partial class searchmenu : System.Web.UI.Page
{
    /// <summary>
    /// An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/>
    /// </summary>
    [Dependency]
    public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.IsPostBack == false)
        {
            var user = cMisc.GetCurrentUser();
            this.ViewState["accountid"] = user.AccountID;
            this.ViewState["employeeid"] = user.EmployeeID;

            this.Title = "Search";
            this.Master.Title = this.Title;

            var generalOptions = this.GeneralOptionsFactory[user.CurrentSubAccountId].WithEmployee().WithHotel();

            var clsModules = new cModules();
            var module = clsModules.GetModuleByID((int)user.CurrentActiveModule);
            var brandName = (module != null) ? module.BrandNameHTML : "Expenses";

			var usingExpenses = user.CurrentActiveModule == Modules.expenses;

            if (generalOptions.Employee.SearchEmployees)
            {
                this.Master.AddMenuItem("user_find", 48, "Employee Search", "Search for the details of other employees that use " + brandName, "information/directory.aspx");
            }

			if (usingExpenses && generalOptions.Hotel.ShowReviews)
            {
                this.Master.AddMenuItem("houses", 48, "Hotel Search", "Search for a hotel and read reviews. Review a hotel you have visited.", "information/reviews.aspx");
            }
        }
    }
}
