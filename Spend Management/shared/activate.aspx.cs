namespace Spend_Management
{
    using System;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;

    public partial class activate : System.Web.UI.Page
    {
        /// <summary>
        /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IProductModule"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IProductModule, Modules> ProductModuleFactory { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                int employeeid = int.Parse(Request.QueryString["employeeid"]);
                int accountid = int.Parse(Request.QueryString["accountid"]);
                cEmployees clsemployees = new cEmployees(accountid);

                var module = this.ProductModuleFactory[Modules.Contracts];
                string brandName = (module != null) ? module.BrandName : "Expenses";

                Employee reqemp = clsemployees.GetEmployeeById(employeeid);
                clsemployees.Activate(employeeid);

                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                sb.Append("Thank you for activating " + reqemp.Forename + " " + reqemp.Surname + ".");
                sb.Append("<br /><br />");
                sb.Append("An email will be sent advising " + reqemp.Forename + " that they can start using " + brandName + ".");
                sb.Append("<br /><br />");
                sb.Append("<a href=\"" + cMisc.Path + "/shared/logon.aspx\"><img src=\"" + cMisc.Path + "/shared/images/buttons/btn_close.png\" alt=\"Close\" /></a>");

                litMessage.Text = sb.ToString();
            }
        }
    }
}