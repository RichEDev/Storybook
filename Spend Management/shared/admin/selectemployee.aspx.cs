using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Spend_Management
{
    using BusinessLogic.Modules;

    /// <summary>
	/// Summary description for selectemployee.
	/// </summary>
	public partial class selectemployee : Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Title = "Employees";
            Master.PageSubTitle = Title;
            Master.title = "";		

			if (IsPostBack == false)
			{
                var user = cMisc.GetCurrentUser();
                pnlNewEmployee.Visible = user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Employees, false);
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Employees, true, true);

                switch (user.CurrentActiveModule)
                {
                    case Modules.Contracts:
                        Master.helpid = 1139;
                        break;
                    default:
                        Master.helpid = 1031;
                        break;
                }

                var clsemployees = new cEmployees(user.AccountID);

				if (clsemployees.getCount(user.AccountID) < 50)
				{
					Response.Redirect("adminemployees.aspx",true);
				}

                var clsgroups = new cGroups(user.AccountID);
                var clsdepartments = new cDepartments(user.AccountID);
                var clscostcodes = new cCostcodes(user.AccountID);

				cmbroles.Items.AddRange(user.AccessRoles.CreateDropDown(0, true).ToArray());
				cmbgroups.Items.AddRange(clsgroups.CreateDropDown(0));
				
                cmbdepartments.Items.AddRange(clsdepartments.CreateDropDown(false).ToArray());
                cmbdepartments.Items.Insert(0, new ListItem("", "0"));
                cmbcostcodes.Items.AddRange(clscostcodes.CreateDropDown(false).ToArray());
                cmbcostcodes.Items.Insert(0, new ListItem("", "0"));

                switch (user.CurrentActiveModule)
                {
                    case Modules.SpendManagement:
                    case Modules.SmartDiligence:
                    case Modules.Contracts:
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "hidePanel", "if(document.getElementById('expcriteria') != null) { document.getElementById('expcriteria').style.display = 'none'; }", true);
                        break;
                    case Modules.Greenlight:
                    case Modules.GreenlightWorkforce:
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "hidePanel", "if(document.getElementById('expsignoffgroup') != null) { document.getElementById('expsignoffgroup').style.display = 'none'; }", true);
                        break;
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

		}
		#endregion

		private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
		    string surname = this.txtsurname.Text;
            string username = this.txtusername.Text.Trim();
            string roleid = this.cmbroles.SelectedValue;
            string costcodeid = this.cmbcostcodes.SelectedValue;
            string groupid = this.cmbgroups.SelectedValue;
            string departmentid = this.cmbdepartments.SelectedValue;
            Response.Redirect("adminemployees.aspx?surname=" + surname + "&roleid=" + roleid + "&groupid=" + groupid + "&costcodeid=" + costcodeid + "&departmentid=" + departmentid + "&username=" + username + "&search=1", true);
		}

        protected void cmdClose_Click(object sender, ImageClickEventArgs e)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();

            switch (curUser.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                case Modules.SpendManagement:
                case Modules.Contracts:
                    Response.Redirect("~/MenuMain.aspx?menusection=employee", true);
                    break;
                default:
                    Response.Redirect("~/usermanagementmenu.aspx", true);
                    break;
            }
        }
	}
}
