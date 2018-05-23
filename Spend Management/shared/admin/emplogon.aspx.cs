namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Web;
    using System.Web.Services;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using BusinessLogic.Modules;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators;

    /// <summary>
	/// Summary description for emplogon.
	/// </summary>
	public partial class emplogon : Page
	{
		protected System.Web.UI.WebControls.ImageButton ImageButton3;
	    private CurrentUser _user;

	    protected void Page_Load(object sender, System.EventArgs e)
		{			
			Title = "Delegate Logon";
            Master.title = Title;
            Master.helpid = 1092;
            byte delegatetype;
            this._user = cMisc.GetCurrentUser();
            cEmployees clsemployees = new cEmployees(_user.AccountID);
            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(_user.AccountID);

            cAccountProperties clsproperties = clsSubAccounts.getSubAccountById(_user.CurrentSubAccountId).SubAccountProperties; // clsSubAccounts.getFirstSubAccount().SubAccountProperties;

            if ((!this.isDelegate && clsemployees.isProxy(_user.EmployeeID) == true && _user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.EmployeeExpenses, false) == false)        
               || (this.isDelegate && !clsproperties.DelEmployeeAccounts && _user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.EmployeeExpenses, false) == false))
            {
                delegatetype = 1; //proxy
            }
            else
            {
                if (Session["myid"] != null)
                {
                    delegatetype = 0;
                    Response.Redirect("~/home.aspx", true);
                }
                else
                {
                    delegatetype = 2; //full admin
                }
            }

            ClientScript.RegisterClientScriptBlock(this.GetType(), "script", "var delegateType = " + delegatetype + ";\n", true);

			if (IsPostBack == false)
			{
                if (clsemployees.getCount(_user.AccountID) > 50 && delegatetype == 2)
                {
					if (_user.CurrentActiveModule == Modules.Expenses)
					{
						cGroups clsgroups = new cGroups(_user.AccountID);
						this.cmbgroups.Items.AddRange(clsgroups.CreateDropDown(0));
					}
					else
					{
						this.signoffDiv.Style.Add(HtmlTextWriterStyle.Display, "none");
					}

                    this.mview.ActiveViewIndex = 0;                    
                    
                    var clsdepartments = new cDepartments(_user.AccountID);
                    var clscostcodes = new cCostcodes(_user.AccountID);
                    var clsAccessRoles = new cAccessRoles(_user.AccountID, cAccounts.getConnectionString(_user.AccountID));

                    cmbroles.Items.AddRange(clsAccessRoles.CreateDropDown(0, true).ToArray());                    
                    cmbdepartments.Items.AddRange(clsdepartments.CreateDropDown(false).ToArray());
                    cmbdepartments.Items.Insert(0, new ListItem("[None]", "0"));
                    cmbcostcodes.Items.AddRange(clscostcodes.CreateDropDown(false).ToArray());
                    cmbcostcodes.Items.Insert(0, new ListItem("[None]", "0"));
                    ViewState["accountid"] = _user.AccountID;
                    ViewState["employeeid"] = _user.EmployeeID;
                    string[] gridData = createEmployeeGrid(-1, "", "", 0, 0, 0, 0, "0");
                    litresults.Text = gridData[2];

                    // set the sel.grid javascript variables
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "EmpGridVars", cGridNew.generateJS_init("EmpGridVars", new List<string>() { gridData[1] }, _user.CurrentActiveModule), true);
                }
                else
                {
                    mview.ActiveViewIndex = 1;
                    if (delegatetype == 1)
                    {
                        cmbemployee.Items.AddRange(clsemployees.createProxyDropDown(_user.EmployeeID));
                    }
                    else
                    {
                        cmbemployee.Items.AddRange(clsemployees.CreateDropDown(0, true));
                    }
                }
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
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


        [WebMethod(EnableSession = true)]
        public static bool logon(int employeeid, int delegateType)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cEmployees clsemployees = new cEmployees(user.AccountID);
            bool userIsAuthorised = clsemployees.createProxyDropDown(user.EmployeeID).Any(v => v.Value == employeeid.ToString());
            if (!userIsAuthorised && delegateType != 2)
            {
                throw new SecurityException("Unauthorised Access");
            }

            if (HttpContext.Current.Session != null && HttpContext.Current.Session["myid"] == null)
            {
                HttpContext.Current.Session.Add("myid", user.EmployeeID);
                HttpContext.Current.Session.Add("delegatetype", delegateType);
                System.Web.Security.FormsAuthentication.SetAuthCookie(user.AccountID + "," + employeeid.ToString(), false);

                DBConnection data = new DBConnection(cAccounts.getConnectionString(user.AccountID));
                data.sqlexecute.Parameters.AddWithValue("@employeeID", employeeid);
                data.sqlexecute.Parameters.AddWithValue("@delegateID", user.EmployeeID);
                data.sqlexecute.Parameters.AddWithValue("@delegateType", delegateType);
                data.sqlexecute.Parameters.AddWithValue("@subAccountID", user.CurrentSubAccountId);

                data.ExecuteProc("RecordDelegateLogon");
                data.sqlexecute.Parameters.Clear();

                return true;
            }

            return false;
        }

        public bool isDelegate
        {
            get
            {
                if (Session["myid"] != null)
                {
                    if ((int)Session["delegatetype"] == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        protected void cmdcancel_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/home.aspx", true);
        }

        protected void cmdsearch_Click(object sender, ImageClickEventArgs e)
        {
            string surname;
            string username;
            int roleid, costcodeid, departmentid, groupid;
            surname = txtsurname.Text;
            username = txtusername.Text.Trim();


            roleid = Convert.ToInt32(cmbroles.SelectedValue);
            costcodeid = Convert.ToInt32(cmbcostcodes.SelectedValue);
            if (cmbgroups.SelectedValue != "")
            {
                groupid = Convert.ToInt32(cmbgroups.SelectedValue);
            }
            else
            {
                groupid = 0;
            }

            departmentid = Convert.ToInt32(cmbdepartments.SelectedValue);
            string[] gridData = createEmployeeGrid(0, username, surname, groupid, costcodeid, departmentid, roleid, "0");
            litresults.Text = gridData[2];

            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "EmpSearchGridVars", cGridNew.generateJS_init("EmpSearchGridVars", new List<string>() { gridData[1] }, _user.CurrentActiveModule), true);
        }

        public string[] createEmployeeGrid(int employeeid, string username, string surname, int groupid, int costcodeid, int departmentid, int accessRoleID, string FilterVal)
        {
            var clsemployees = new cEmployees(this._user.AccountID);
            var clsfields = new cFields(this._user.AccountID);
            var grid = new cGridNew(this._user.AccountID, this._user.EmployeeID, "gridEmployees", clsemployees.createGrid());

            if (employeeid != 0)
            {
                grid.addFilter(((cFieldColumn)grid.getColumnByName("employeeid")).field, ConditionType.Equals, new object[] { employeeid }, null, ConditionJoiner.And);
            }

            if (FilterVal != "2")
            {
                int val = int.Parse(FilterVal);
                grid.addFilter(((cFieldColumn)grid.getColumnByName("archived")).field, ConditionType.Equals, new object[] { val }, null, ConditionJoiner.And);
            }

            if (username != "")
            {
                grid.addFilter(clsfields.GetFieldByID(new Guid("1c45b860-ddaa-47da-9eec-981f59cce795")), ConditionType.Like, new object[] { username + "%" }, null, ConditionJoiner.And);
            }
            if (surname != "")
            {
                grid.addFilter(clsfields.GetFieldByID(new Guid("9d70d151-5905-4a67-944f-1ad6d22cd931")), ConditionType.Like, new object[] { surname + "%" }, null, ConditionJoiner.And);
            }
            if (groupid != 0)
            {
                grid.addFilter(clsfields.GetFieldByID(new Guid("4f615406-8d1f-47b3-821b-88bade48205e")), ConditionType.Equals, new object[] { groupid }, null, ConditionJoiner.And);
            }
            if (departmentid != 0)
            {
                grid.addFilter(clsfields.GetFieldByID(new Guid("CAC12A35-EFC7-47E0-B870-B58803307DA8")), ConditionType.Equals, new object[] { departmentid }, null, ConditionJoiner.And);
            }
            if (costcodeid != 0)
            {
                grid.addFilter(clsfields.GetFieldByID(new Guid("2FA3AD65-B3F2-4658-94D7-08394B6EB43E")), ConditionType.Equals, new object[] { costcodeid }, null, ConditionJoiner.And);
            }
            if (accessRoleID != 0)
            {
                grid.addFilter(clsfields.GetFieldByID(new Guid("BDCE49C3-EAF8-4070-91C6-959DA3827DB6")), ConditionType.Equals, new object[] { accessRoleID }, null, ConditionJoiner.And);
            }

            grid.addFilter(clsfields.GetFieldByID(new Guid("1c45b860-ddaa-47da-9eec-981f59cce795")), ConditionType.NotLike, new object[] { "admin%" }, null, ConditionJoiner.And);
            grid.EmptyText = "Please enter criteria above to search for an employee";
            grid.KeyField = "employeeid";
            grid.getColumnByName("employeeid").hidden = true;
            grid.getColumnByName("archived").hidden = true;

			if (_user.CurrentActiveModule != Modules.Expenses)
			{
				grid.getColumnByName("groupname").hidden = true;
			}

            grid.EnableSelect = true;
            grid.GridSelectType = GridSelectType.RadioButton;
            grid.InitialiseRow += InitialiseRow;
            var retVals = new List<string> {grid.GridID};
            retVals.AddRange(grid.generateGrid());
            return retVals.ToArray();
        }

        private void InitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridInfo)
        {
            var employeeIdCell = row.getCellByID("employeeID");
            if (employeeIdCell != null)
            {
                var delegateUser = new CurrentUser(this._user.AccountID, (int) employeeIdCell.Value, 0,
                    this._user.CurrentActiveModule, this._user.CurrentSubAccountId);
                if (!delegateUser.CheckAccessRoleApi(SpendManagementElement.None, AccessRoleType.View,
                    AccessRequestType.Website))
                {
                    row.hidden = true;
                }
            }
        }
	}

}
