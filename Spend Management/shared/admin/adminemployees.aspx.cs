using System;
using System.Web;
using System.Web.UI;
using System.Web.Services;
using SpendManagementLibrary;
using SpendManagementLibrary.Employees;

using System.Collections.Generic;

namespace Spend_Management
{
    using BusinessLogic.Modules;

    /// <summary>
	/// Summary description for adminemployees.
	/// </summary>
	public partial class adminemployees : Page
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";

            CurrentUser user = cMisc.GetCurrentUser();

            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID; 

			Title = "Employees";
            Master.title = Title;

            switch (user.CurrentActiveModule)
            {
                case Modules.Contracts:
                    Master.helpid = 1139;
                    break;
                default:
                    Master.helpid = 1031;
                    break;
            }

			if (IsPostBack == false)
			{
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Employees, true, true);
                pnlAddNewEmployee.Visible = user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Employees, true);

			    string surname = "";
                string username = "";
                int employeeidToFind = 0;
				int roleid = 0;
                int groupid = 0;
                int costcodeid = 0;
                int departmentid = 0;

                if (Request.QueryString["username"] != null)
                {
                    username = Request.QueryString["username"];
                }

                ViewState["username"] = username;

                if (Request.QueryString["employeeid"] != null)
                {
                    employeeidToFind = !String.IsNullOrEmpty(this.Request.QueryString["employeeid"]) ? int.Parse(this.Request.QueryString["employeeid"]) : 0;
                }

                ViewState["employeeid_to_find"] = employeeidToFind;

				if (Request.QueryString["surname"] != null)
				{
					surname = Request.QueryString["surname"];
				}

				ViewState["surname"] = surname;

                if (Request.QueryString["roleid"] != null)
                {
                    roleid = !String.IsNullOrEmpty(this.Request.QueryString["roleid"]) ? int.Parse(this.Request.QueryString["roleid"]) : 0;
                }

				ViewState["roleid"] = roleid;

                if (Request.QueryString["groupid"] != null)
                {
                    groupid = !String.IsNullOrEmpty(this.Request.QueryString["groupid"]) ? int.Parse(this.Request.QueryString["groupid"]) : 0;
                }

				ViewState["groupid"] = groupid;

                if (Request.QueryString["costcodeid"] != null)
                {
                    costcodeid = !String.IsNullOrEmpty(this.Request.QueryString["costcodeid"]) ? int.Parse(this.Request.QueryString["costcodeid"]) : 0;
                }

				ViewState["costcodeid"] = costcodeid;

                if (Request.QueryString["departmentid"] != null)
                {
                    departmentid = !String.IsNullOrEmpty(this.Request.QueryString["departmentid"]) ? int.Parse(this.Request.QueryString["departmentid"]) : 0;
                }
                
				ViewState["departmentid"] = departmentid;
				
                string[] gridData = createEmployeeGrid(employeeidToFind, username, surname, groupid, costcodeid, departmentid, roleid, cmbfilter.SelectedValue);
                litgrid.Text = gridData[2];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "EmpsGridVars", cGridNew.generateJS_init("EmpsGridVars", new List<string>() { gridData[1] }, user.CurrentActiveModule), true);

                if (username == "")
                {
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "username", "var username = '';", true);
                }
                else
                {
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "username", "var username = '" + username + "';", true);
                }

                if (surname == "")
                {
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "surname", "var surname = '';", true);
                }
                else
                {
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "surname", "var surname = '" + surname + "';", true);
                }
                
                ClientScript.RegisterClientScriptBlock(this.GetType(), "variables", "var groupid = " + groupid + "; var costcodeid = " + costcodeid + "; var departmentid = " + departmentid + ";var roleid = " + roleid + ";", true);
			
			}
		}

        [WebMethod(EnableSession=true)]
        public static string[] createEmployeeGrid(int employeeid, string username, string surname, int groupid, int costcodeid, int departmentid, int accessRoleID, string FilterVal)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var clsemployees = new cEmployees(user.AccountID);
            var clsfields = new cFields(user.AccountID);
            var grid = new cGridNew(user.AccountID, user.EmployeeID, "gridEmployees", clsemployees.createGrid());

            if (employeeid != 0)
            {
                grid.addFilter(((cFieldColumn)grid.getColumnByName("employeeid")).field, ConditionType.Equals, new object[] { employeeid }, null, ConditionJoiner.None);
            }

            if (FilterVal != "2")
            {
                int val = int.Parse(FilterVal);
                grid.addFilter(((cFieldColumn)grid.getColumnByName("archived")).field, ConditionType.Equals, new object[] { val }, null, ConditionJoiner.None);
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
                grid.addFilter(clsfields.GetFieldByID(new Guid("008C4487-9634-4280-9F45-772CDAA7EA4D")), ConditionType.Equals, new object[] { accessRoleID }, null, ConditionJoiner.And);
                grid.addFilter(clsfields.GetFieldByID(new Guid("FA7B6410-76BC-40A6-B618-8B231FAA7A23")), ConditionType.Equals, new object[] { user.CurrentSubAccountId }, null, ConditionJoiner.And);
            }

            grid.addFilter(clsfields.GetFieldByID(new Guid("1c45b860-ddaa-47da-9eec-981f59cce795")), ConditionType.NotLike, new object[] { "admin%" }, null, ConditionJoiner.And);
            grid.EmptyText = "No employees to display";
            grid.KeyField = "employeeid";
            grid.getColumnByName("employeeid").hidden = true;
            grid.getColumnByName("archived").hidden = true;
            grid.enabledeleting = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Employees, true, false);
            grid.enableupdating = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Employees, true, false);
            grid.editlink = "aeemployee.aspx?employeeid={employeeid}";
            grid.deletelink = "javascript:deleteEmployee({employeeid});";
            grid.enablearchiving = true;
            grid.ArchiveField = "archived";
            grid.archivelink = "javascript:changeArchiveStatus({employeeid});";
            grid.addTwoStateEventColumn("Active", (cFieldColumn)grid.getColumnByName("active"), false, true, "/static/icons/16/new-icons/stopwatch_stop.png", "aeemployee.aspx?employeeid={employeeid}", "Inactive Account", "Inactive Account", "", "", "Activate Account", "Activate Account");
            grid.addTwoStateEventColumn("Locked", (cFieldColumn)grid.getColumnByName("locked"), false, true, "", "", "Lock Account", "Lock Account", "/static/icons/16/new-icons/lock.png", "javascript:changeLockedStatus({employeeid});", "Unlock Account", "Unlock Account");
            grid.addEventColumn("changepwd", "../images/icons/replace2.gif", "/shared/changepassword.aspx?returnto=1&employeeid={employeeid}", "Change Password", "Change Password");
            grid.addEventColumn("resetpwd", "../images/icons/redo.png", "javascript:sendPasswordLink({employeeid});", "Reset Password", "Reset Password");

            switch (user.CurrentActiveModule)
            {
                case Modules.SpendManagement:
                case Modules.SmartDiligence:
                case Modules.Contracts:
                    grid.getColumnByName("groupname").hidden = true;
                    break;
            }

            var retVals = new List<string> { grid.GridID };
            retVals.AddRange(grid.generateGrid());
            return retVals.ToArray();
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
        public static int deleteEmployee(int accountid, int employeeid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var clsemployees = new cEmployees(accountid);
            Employee employee = clsemployees.GetEmployeeById(employeeid);
            return employee.Delete(user);
        }

        [WebMethod(EnableSession = true)]
        public static int changeStatus(int employeeid)
        {
            int result = 0;
            CurrentUser user = cMisc.GetCurrentUser();
            var clsemployees = new cEmployees(user.AccountID);
            Employee reqemp = clsemployees.GetEmployeeById(employeeid);
            result = reqemp.ChangeArchiveStatus(!reqemp.Archived, user);
            if (result > 0)
            {
                result= reqemp.Save(user);

            }
            return result;
        }

        [WebMethod(EnableSession=true)]
        public static bool ResetPassword(int employeeID)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Employees, true))
            {
                var clsEmployees = new cEmployees(user.AccountID);
                Employee reqEmp = clsEmployees.GetEmployeeById(employeeID);

                if (reqEmp.Archived || reqEmp.Active == false)
                {
                    return false;
                }

                clsEmployees.SendPasswordKey(employeeID, cEmployees.PasswordKeyType.AdminRequest, null, user.CurrentActiveModule);
                return true;
            }

            return false;
        }

       
        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (SiteMap.CurrentNode != null && SiteMap.CurrentNode.ParentNode != null && SiteMap.CurrentNode.ParentNode.ParentNode != null)
            {
                string previousUrl = (this.Request.QueryString["search"] == "1") ? SiteMap.CurrentNode.ParentNode.Url : SiteMap.CurrentNode.ParentNode.ParentNode.Url;
                this.Response.Redirect(previousUrl, true);
            }
            else
            {
                this.Response.Redirect(SiteMap.RootNode.Url, true);
            }
        }
	}
}
