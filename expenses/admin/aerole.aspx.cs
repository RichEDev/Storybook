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
using ExpensesLibrary;
using SpendManagementLibrary;

namespace expenses
{
	/// <summary>
	/// Summary description for aerole.
	/// </summary>
	public partial class aerole : Page
	{
		
		
		string action;
		int roleid;
		
		protected System.Web.UI.WebControls.ImageButton cmdhelp;
		cRoles clsroles;
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Title = "Add/Edit Role";
            Master.title = Title;
			Master.showdummymenu = true;
            Master.helpid = 1034;
			
			if (IsPostBack == false)
			{
                Master.enablenavigation = false;
                CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
                ViewState["accountid"] = user.accountid;
                ViewState["employeeid"] = user.employeeid;

				cEmployees clsemployees = new cEmployees(user.accountid);
				cEmployee reqemp;
				reqemp = clsemployees.GetEmployeeById(user.employeeid);
				
				
					clsroles = new  cRoles(reqemp.accountid);

                    cRole role = clsroles.getRoleById(reqemp.roleid);
                    cMisc clsmisc = new cMisc(reqemp.accountid);

                    cGlobalProperties clsproperties = clsmisc.getGlobalProperties(reqemp.accountid);
                    if ((!Master.isDelegate && role.employeeadmin == false) || (Master.isDelegate && !clsproperties.delemployeeadmin))
                    {
                        Response.Redirect("../restricted.aspx?", true);
                    }

                    cAccounts clsAccounts = new cAccounts();
                    cAccount reqAccount = clsAccounts.getAccountById(reqemp.accountid);

                    

				System.Data.DataSet DSsubcats = new DataSet();
				action = Request.QueryString["action"];
				
				
				
				



				
				
				

				if (action == "2")
				{
					txtaction.Text = "2";
					roleid = int.Parse(Request.QueryString["roleid"]);
					txtroleid.Text = roleid.ToString();
					cRole reqrole;
					reqrole = clsroles.getRoleById(roleid);
					txtrolename.Text = reqrole.rolename;;
					txtdescription.Text = reqrole.description;
					chksetup.Checked = reqrole.setup;
					chkusersetup.Checked = reqrole.employeeadmin;
					chkemplogon.Checked = reqrole.employeeaccounts;
					chkreports.Checked = reqrole.reports;
					chkcheckpay.Checked = reqrole.checkandpay;
					chkqedesign.Checked = reqrole.qedesign;

                    if (reqAccount.CorporateCardsEnabled == true)
                    {
                        chkcreditcard.Checked = reqrole.creditcard;
                    }
                    else
                    {
                        chkcreditcard.Checked = false;
                        chkcreditcard.Enabled = false;
                    }

                    if (reqAccount.AdvancesEnabled == true)
                    {
                        chkapproval.Checked = reqrole.approvals;
                    }
                    else
                    {
                        chkapproval.Checked = false;
                        chkapproval.Enabled = false;
                    }


					chkexports.Checked = reqrole.exports;
					chkcostcodesreadonly.Checked = reqrole.costcodesreadonly;
					chkdepartmentsreadonly.Checked = reqrole.departmentsreadonly;
					chkprojectcodesreadonly.Checked = reqrole.projectcodesreadonly;
					chkreportsreadonly.Checked = reqrole.reportsreadonly;
                    chkauditlog.Checked = reqrole.auditlog;
                    
					txtminclaim.Text = reqrole.minclaim.ToString("########0.00");
					txtmaxclaim.Text = reqrole.maxclaim.ToString("########0.00");
					if (reqrole.accesstype == 1)	
					{
						optall.Checked = true;
					}
					else if (reqrole.accesstype == 2)
					{
						optselected.Checked = true;

					}
					else
					{
						optgroups.Checked = true;
					}
					cmbmasters.Items.AddRange(clsroles.CreateMasterRolesDropDown(reqrole.masterrole));


					
					chkroles.DataSource = clsroles.getRoles(roleid);
					chkroles.DataTextField = "rolename";
					chkroles.DataValueField = "roleid";
					chkroles.DataBind();

					clsroles.tickRoles(ref chkroles,roleid);

					
					
					
				}
				else
				{
					chkroles.DataSource = clsroles.getRoles(0);
					chkroles.DataTextField = "rolename";
					chkroles.DataValueField = "roleid";
					chkroles.DataBind();
					cmbmasters.Items.AddRange(clsroles.CreateMasterRolesDropDown(0));
					
					
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
			this.cmdok.Click += new System.Web.UI.ImageClickEventHandler(this.cmdok_Click);
			this.cmdcancel.Click += new System.Web.UI.ImageClickEventHandler(this.cmdcancel_Click);

		}
		#endregion


		private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			string rolename;
			string description;
			bool setup = false;
			bool employeeaccounts = false;
			int masterrole = 0;
			bool employeeadmin = false;
			bool reports = false;
			bool checkandpay = false;
			bool qedesign = false;
			int accesstype = 0;
			bool creditcard = false;
			
			bool approvals = false;
			bool exports = false;

			bool costcodesreadonly = chkcostcodesreadonly.Checked;
			bool departmentsreadonly = chkdepartmentsreadonly.Checked;
			bool projectcodesreadonly = chkprojectcodesreadonly.Checked;
			bool reportsreadonly = chkreportsreadonly.Checked;
            bool auditlog = chkauditlog.Checked;
            
			int roleaccesscount = 0;
			int rolesubcatcount = 0;
			decimal minclaim = 0;
			decimal maxclaim = 0;
int i = 0;
			sRoleAccess[] roleaccess = new sRoleAccess[0];
			rolename = txtrolename.Text;
			description = txtdescription.Text;
			masterrole = int.Parse(cmbmasters.SelectedValue);
			
				setup = chksetup.Checked;
			
				employeeadmin = chkusersetup.Checked;
				qedesign = chkqedesign.Checked;
				employeeaccounts = chkemplogon.Checked;
			
		
				reports = chkreports.Checked;
		
			
				checkandpay = chkcheckpay.Checked;
				approvals = chkapproval.Checked;
				exports = chkexports.Checked;
			if (txtminclaim.Text != "")
			{
				minclaim = decimal.Parse(txtminclaim.Text);
			}
			if (txtmaxclaim.Text != "")
			{
				maxclaim = decimal.Parse(txtmaxclaim.Text);
			}
			if (optall.Checked == true)
			{
				accesstype = 1;
			}
			else if (optselected.Checked == true)
			{
				accesstype = 2;
			}
			else
			{
				accesstype = 3;
			}
			
				creditcard = chkcreditcard.Checked;
				
			
			if (accesstype == 2)
			{
				for (i = 0; i < chkroles.Items.Count; i++)
				{
					if (chkroles.Items[i].Selected == true)
					{
						roleaccesscount++;
					}
				}

				roleaccess = new sRoleAccess[roleaccesscount];
				roleaccesscount = 0;
				for (i = 0; i < chkroles.Items.Count; i++)
				{
					if(chkroles.Items[i].Selected == true)
					{
						roleaccess[roleaccesscount] = new sRoleAccess();
						roleaccess[roleaccesscount].accessid = int.Parse(chkroles.Items[i].Value.ToString());
						roleaccesscount++;
					}
				}
			}


			clsroles = new  cRoles((int)ViewState["accountid"]);
			
			action = txtaction.Text;

			if (action == "2") //update
			{
				roleid = int.Parse(txtroleid.Text);
				if (clsroles.updateRole(roleid,rolename,description,masterrole,setup,employeeadmin,employeeaccounts,reports,checkandpay,qedesign,accesstype,creditcard,roleaccess, approvals, exports, costcodesreadonly, departmentsreadonly, projectcodesreadonly, minclaim, maxclaim, reportsreadonly, auditlog, (int)ViewState["employeeid"]) == 1)
				{
					lblmsg.Text = "This role cannot be updated as the role name entered already exists.";
					lblmsg.Visible = true;
					return;
				}
			}
			else
			{
                if (clsroles.addRole(rolename, description, masterrole, setup, employeeadmin, employeeaccounts, reports, checkandpay, qedesign, accesstype, creditcard, roleaccess, approvals, exports, costcodesreadonly, departmentsreadonly, projectcodesreadonly, minclaim, maxclaim, reportsreadonly, auditlog, (int)ViewState["employeeid"]) == 1)
				{
					lblmsg.Text = "This role cannot be added as the role name entered already exists.";
					lblmsg.Visible = true;
					return;
				}
			}

			
			Response.Redirect("adminroles.aspx",true);
		}

		

		private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			Response.Redirect("adminroles.aspx",true);
		}

		protected void cmdrole_Click(object sender, System.EventArgs e)
		{
			chksetup.Checked = true;
			chkusersetup.Checked = true;
			chkemplogon.Checked = true;
			chkreports.Checked = true;
			chkcheckpay.Checked = true;
			chkqedesign.Checked = true;
			chkcreditcard.Checked = true;
			
			chkapproval.Checked = true;
			chkexports.Checked = true;
            chkreportsreadonly.Checked = true;
            chkauditlog.Checked = true;
            
		}

		
		protected void cmdroledesel_Click(object sender, System.EventArgs e)
		{
			chksetup.Checked = false;
			chkusersetup.Checked = false;
			chkemplogon.Checked = false;
			chkreports.Checked = false;
			chkcheckpay.Checked = false;
			chkqedesign.Checked = false;
			chkcreditcard.Checked = false;
			
			chkapproval.Checked = false;
			chkexports.Checked = false;
            chkreportsreadonly.Checked = false;
            chkauditlog.Checked = false;
            
		}

		
	}
}
