

namespace expenses
{
	using System;
	using System.Security.Authentication;
	using System.Text;
	using System.Web;
	using System.Collections.Generic;
	using System.Web.UI;
    using SpendManagementLibrary.Definitions;
	using SpendManagementLibrary.Employees;
	using Spend_Management;
	using SpendManagementLibrary;	

	/// <summary>
	/// Summary description for employeesearch.
	/// </summary>
	public partial class employeesearch : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
            this.Master.UseDynamicCSS = true;
			Master.showdummymenu = true;
            Title = "Employee Search";
            Master.title = Title;
            Master.enablenavigation = false;

			if (IsPostBack == false)
			{
				int i;
				cGridColumn newcol;
				cGridRow reqrow;
			    CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cEmployees clsemployees = new cEmployees(user.AccountID);

				if (Request.QueryString["action"] != null)
				{
				    var action = new EmployeeProxyAction(HttpUtility.UrlDecode(this.Request.QueryString["action"]));
				    if (action.SessionId != this.Session.SessionID)
				    {
				        throw new InvalidCredentialException();
				    }

				    switch (action.Action)
					{
						case 1: //assign
                            Employee newProxy = clsemployees.GetEmployeeById(action.EmployeeId);
                            if (!newProxy.Archived)
                            {
                                clsemployees.assignProxy(user.EmployeeID, action.EmployeeId);
                            }
							break;
						case 3://delete
                            clsemployees.removeProxy(user.EmployeeID, action.EmployeeId);
							break;
					}
				}
                cGrid clsgrid = new cGrid(user.AccountID, clsemployees.getProxies(user.EmployeeID), true, false, Grid.Delegates);
				clsgrid.tblclass = "datatbl";
                clsgrid.emptytext = "There are no delegates currently assigned";
				clsgrid.getColumn("employeeid").hidden = true;
				clsgrid.getColumn("username").description = "Username";
				clsgrid.getColumn("empname").description = "Employee Name";
				newcol = new cGridColumn("delete","<img alt=\"Delete\" src=\"../icons/delete2_blue.gif\">","S","",false, true);
				clsgrid.gridcolumns.Insert(0,newcol);
				clsgrid.tableid = "empprox";
				clsgrid.getData();
				for (i = 0; i < clsgrid.gridrows.Count; i++)
				{
					reqrow = (cGridRow)clsgrid.gridrows[i];

				    var employeeProxyAction =  new EmployeeProxyAction(3, (int)reqrow.getCellByName("employeeid").thevalue, this.Session.SessionID);  
                    reqrow.getCellByName("delete").thevalue =
                        $"<a href=\"employeeproxy.aspx?action={HttpUtility.UrlEncode(employeeProxyAction.ToString())}\"><img src=\"../icons/delete2.gif\" alt=\"Delete\"></a>";
				}
				litallocated.Text = clsgrid.CreateGrid();
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
			this.cmdsearch.Click += new System.Web.UI.ImageClickEventHandler(this.cmdsearch_Click);

		}
		#endregion

		private void cmdsearch_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{			
			string surname = txtsurname.Text;
		    CurrentUser user = cMisc.GetCurrentUser();
            cEmployees clsemployees = new cEmployees(user.AccountID);
		    cFields clsFields = new cFields(user.AccountID);
            cGridNew clsGrid = new cGridNew(user.AccountID, user.EmployeeID, "gridEmployees", clsemployees.createGrid());

            if (surname != "")
            {
                clsGrid.addFilter(clsFields.GetFieldByID(new Guid("9d70d151-5905-4a67-944f-1ad6d22cd931")), ConditionType.Like, new object[] { surname + "%" }, null, ConditionJoiner.And);
            }
            clsGrid.addFilter(clsFields.GetFieldByID(new Guid("1c45b860-ddaa-47da-9eec-981f59cce795")), ConditionType.NotLike, new object[] { "admin%" }, null, ConditionJoiner.And);

            clsGrid.EmptyText = "No employees to display";
            clsGrid.KeyField = "employeeid";
            clsGrid.getColumnByName("employeeid").hidden = true;
            clsGrid.getColumnByName("archived").hidden = true;
                        
		    clsGrid.showheaders = true;
		    clsGrid.enablearchiving = false;
		    clsGrid.enabledeleting = false;
		    clsGrid.enableupdating = false;
		    clsGrid.enablepaging = false;
            clsGrid.addEventColumn("select","Assign",$"employeeproxy.aspx?action=1,{this.Session.SessionID},{{employeeid}} ","");
		    clsGrid.SortedColumn = clsGrid.getColumnByName("empoyeeid");
		    clsGrid.InitialiseRow += this.ClsGridOnInitialiseRow;
            switch (user.CurrentActiveModule)
            {
                case Modules.SpendManagement:
                case Modules.SmartDiligence:
                case Modules.contracts:
				case Modules.Greenlight:
                case Modules.GreenlightWorkforce:
                    clsGrid.getColumnByName("groupname").hidden = true;
                    break;                
            }
			
		    string[] result = clsGrid.generateGrid();
            
		    litgrid.Text = result[1];
            Page.ClientScript.RegisterStartupScript(this.GetType(), "gridEmployeeVars", cGridNew.generateJS_init("gridEmployeeVars", new List<string>() { result[0] }, user.CurrentActiveModule), true);
            
		}

	    private void ClsGridOnInitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridInfo)
	    {
	        var value = row.getCellByID("select").Value.ToString().Split('"');
	        var newValue = new StringBuilder();
	        foreach (var element in value)
	        {
	            if (element.Contains("action="))
	            {
	                var position = element.IndexOf("action=") + 7;
	                
	                var partial = element.Substring(position).Replace("{employeeid}", row.getCellByID("employeeid").Value.ToString()).Split(',');
	                var proxy = new EmployeeProxyAction(partial);
	                newValue.Append(element.Substring(0, position) + HttpUtility.UrlEncode(proxy.ToString()));
	            }
	            else
	            {
	                newValue.Append(element);
	            }

	            newValue.Append("\"");
	        }

	        row.getCellByID("select").Value = newValue.ToString().TrimEnd('\"');
	    }

	    protected void cmdok_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("delegates.aspx", true);
        }
}
}
