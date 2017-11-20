using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using SpendManagementLibrary;
using Spend_Management;

namespace expenses.information
{
    using SpendManagementLibrary.Employees;

    /// <summary>
    /// Summary description for direntry.
    /// </summary>
    public partial class direntry : Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Title = "Directory Entry";
            Master.title = Title;
            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cEmployees clsemployees = new cEmployees(user.AccountID);
                int employeeid = int.Parse(this.Request.QueryString["employeeid"]);
                Employee employee = clsemployees.GetEmployeeById(employeeid);

                txtname.Text = employee.Title + " " + employee.Forename + " " + employee.Surname + "(" + employee.Username + ")";
                txtextension.Text = employee.TelephoneExtensionNumber;
                txtpagerno.Text = employee.PagerNumber;
                txtmobileno.Text = employee.MobileTelephoneNumber;
                txtemail.Text = employee.EmailAddress;

                txttelno.Text = employee.TelephoneNumber;
                txtfaxno.Text = employee.FaxNumber;
                txtemailhome.Text = employee.HomeEmailAddress;
            }
        }

        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }

        #endregion
    }
}
