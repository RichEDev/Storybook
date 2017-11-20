using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using ExpensesLibrary;

namespace expenses
{
    public partial class aeCarPage : System.Web.UI.Page
    {
        CurrentUser user;

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.title = "Add Car";
            Master.enablenavigation = false;
            user = cMisc.getCurrentUser(User.Identity.Name);

            cEmployees clsEmployees = new cEmployees(user.accountid);
            cEmployee reqEmployee = clsEmployees.GetEmployeeById(user.employeeid);

            cRoles clsroles = new cRoles(user.accountid);
            cRole role = clsroles.getRoleById(reqEmployee.roleid);
            cMisc clsmisc = new cMisc(user.accountid);

            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(user.accountid);

            if ((!Master.isDelegate && role.employeeadmin == false) || (Master.isDelegate && !clsproperties.delemployeeadmin))
            {
                aeCar.EmployeeAdmin = false;
            }
            else
            {
                aeCar.EmployeeAdmin = true;
            }

            aeCar.AccountID = user.accountid;
            aeCar.EmployeeID = user.employeeid;

            aeCar.ReturnURL = "mydetailsmenu.aspx";

            if (Request.QueryString["action"] == "2" && (Request.QueryString["carID"] != null && Request.QueryString["carID"] != ""))
            {
                aeCar.Action = aeCarPageAction.Edit;
                aeCar.CarID = int.Parse(Request.QueryString["carID"]);
            }
            else
            {
                aeCar.Action = aeCarPageAction.Add;
            }





        }
    }
}
