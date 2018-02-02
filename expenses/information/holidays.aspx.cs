namespace expenses.information
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Services;
    using System.Web.UI;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Holidays;

    using Spend_Management;

    /// <summary>
	/// Summary description for holidays.
	/// </summary>
	public partial class holidays : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
			Title = "Holidays";
            Master.title = Title;
            Master.helpid = 1166;
			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

			    string[] gridData = createGrid(user.AccountID, user.EmployeeID);
			    litgrid.Text = gridData[1];

			    // set the sel.grid javascript variables
			    Page.ClientScript.RegisterStartupScript(this.GetType(), "HolidaysGridVars", cGridNew.generateJS_init("HolidaysGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
            }
		}

	    private string[] createGrid(int accountid, int employeeid)
	    {
	        cTables clstables = new cTables(accountid);
	        cFields clsfields = new cFields(accountid);
	        List<cNewGridColumn> columns = new List<cNewGridColumn>();
	        columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("35B90FF5-780A-43C2-8E54-932B1A7C8B6E"))));
	        columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("76E6DAEB-DD74-4B4F-B7D5-E69D47FAA0EA"))));
	        columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("F6A23A31-B705-4567-92AE-48FD95372941"))));
	        cGridNew clsgrid = new cGridNew(accountid, employeeid, "gridHolidays", clstables.GetTableByID(new Guid("A6FF86D3-808F-406F-9DD6-E21B7B9A8D67")), columns);
	        clsgrid.getColumnByName("holidayid").hidden = true;
	        clsgrid.KeyField = "holidayid";
	        clsgrid.enabledeleting = true;
	        clsgrid.deletelink = "javascript:deleteHoliday({holidayid});";
	        clsgrid.enableupdating = true;
	        clsgrid.editlink = "aeholiday.aspx?action=2&holidayid={holidayid}";
	        var employeeIdField = clsfields.GetFieldByID(Guid.Parse("10300FB9-53F1-471A-A65A-D77A6491BB8F")); // employeeID
	        clsgrid.addFilter(employeeIdField, ConditionType.Equals, new object[] { employeeid }, null, ConditionJoiner.None);
            return clsgrid.generateGrid();
	    }

        [WebMethod(EnableSession = true)]
        public static void deleteHoliday (int accountid, int holidayid)
        {
            var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid));
            Holidays holidays = new Holidays(connection);
            holidays.DeleteHoliday(holidayid);
        }

        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string sPreviousURL = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(sPreviousURL, true);
        }
	}
}
