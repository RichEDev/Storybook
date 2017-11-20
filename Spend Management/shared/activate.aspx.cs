using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SpendManagementLibrary;
using SpendManagementLibrary.Employees;

namespace Spend_Management
{
    public partial class activate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                int employeeid = int.Parse(Request.QueryString["employeeid"]);
                int accountid = int.Parse(Request.QueryString["accountid"]);
                cEmployees clsemployees = new cEmployees(accountid);
                cModules clsModules = new cModules();
                cModule clsModule = clsModules.GetModuleByID((int)Modules.contracts);
                string brandName = (clsModule != null) ? clsModule.BrandNamePlainText : "Expenses";


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