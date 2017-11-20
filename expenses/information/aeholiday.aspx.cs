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
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Holidays;

    /// <summary>
    /// Summary description for aeholiday.
    /// </summary>
    public partial class aeholiday : Page
    {


        protected System.Web.UI.WebControls.ImageButton cmdhelp;

        private string action = "";

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Title = "Add / Edit Holiday";
            Master.title = Title;
            Master.showdummymenu = true;
            Master.helpid = 1166;


            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cEmployees clsemployees = new cEmployees(user.AccountID);

                int holidayid = 0;
                if (Request.QueryString["action"] != null)
                {
                    action = Request.QueryString["action"];
                }

                if (action == "2")
                {
                    var connection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID));

                    Holidays holidays = new Holidays(connection);
                    Holiday holiday = new Holiday();
                    txtaction.Text = "2";
                    holidayid = int.Parse(Request.QueryString["holidayid"]);
                    txtholidayid.Text = holidayid.ToString();
                    holiday = holidays.GetHolidayById(holidayid);
                    dtstart.Text = holiday.StartDate.ToShortDateString();
                    dtend.Text = holiday.EndDate.ToShortDateString();
                }
                else
                {
                    dtstart.Text = DateTime.Today.ToShortDateString();
                    dtend.Text = DateTime.Today.ToShortDateString();
                }
                litholiday.Text = clsemployees.getHolidayApprovers(user.EmployeeID);

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
            this.cmdok.Click += new System.Web.UI.ImageClickEventHandler(this.cmdok_Click);
            this.cmdcancel.Click += new System.Web.UI.ImageClickEventHandler(this.ImageButton2_Click);

        }

        #endregion

        private void ImageButton2_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Redirect("holidays.aspx", true);
        }

        private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var connection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID));
            Holidays holidays = new Holidays(connection);
            DateTime startDate;
            DateTime endDate;
            int holidayid = 0;

            startDate = DateTime.Parse(dtstart.Text);
            endDate = DateTime.Parse(dtend.Text);

            action = txtaction.Text;

            if (action == "2") //update
            {
                holidayid = int.Parse(txtholidayid.Text);
                holidays.UpdateHoliday(holidayid, startDate, endDate);
            }
            else
            {
                holidays.AddHoliday((int)ViewState["employeeid"], startDate, endDate);
            }

            Response.Redirect("holidays.aspx", true);

        }
    }
}