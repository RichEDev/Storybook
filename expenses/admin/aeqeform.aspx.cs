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

namespace expenses
{
	/// <summary>
	/// Summary description for aeqeform.
	/// </summary>
	public partial class aeqeform : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Title = "Add / Edit Quick Entry Form";
            Master.title = Title;
			Master.showdummymenu = true;
            Master.helpid = 1045;
			if (IsPostBack == false)
			{
                Master.enablenavigation = false;
				int action = 0;
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.QuickEntryForms, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cAccounts clsAccounts = new cAccounts();
                cAccount reqAccount = clsAccounts.GetAccountByID(user.AccountID);

                if (reqAccount.QuickEntryFormsEnabled == false)
                {
                    Response.Redirect("../home.aspx?", true);
                }
                
				if (Request.QueryString["action"] != null)
				{
					action = int.Parse(Request.QueryString["action"]);
				}
				ViewState["action"] = action;

				if (action == 2)
				{
					int quickentryid = int.Parse(Request.QueryString["quickentryid"]);
					ViewState["quickentryid"] = quickentryid;
                    cQeForms clsforms = new cQeForms(user.AccountID);
					cQeForm reqform = clsforms.getFormById((int)ViewState["quickentryid"]);
					txtname.Text = reqform.name;
					txtdescription.Text = reqform.description;
					optgenmonth.Checked = reqform.genmonth;
					optnumrows.Checked = !reqform.genmonth;
					txtnumrows.Text = reqform.numrows.ToString();
					cGrid clsgrid = new cGrid(reqform.getColumnGrid(),true,false);
					clsgrid.tblclass = "datatbl";
					clsgrid.getColumn("order").description = "Column No";
					clsgrid.getColumn("columnname").description = "Column Name";

					litform.Text = clsgrid.CreateGrid();
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

		protected void cmdformdesign_Click(object sender, System.EventArgs e)
		{
			if (updateDetails() == false)
			{
				return;
			}
			Response.Redirect("qeformdesign.aspx?quickentryid=" + ViewState["quickentryid"],true);
		}

		private bool updateDetails()
		{
			string name, description;
			bool genmonth;
			int numrows = 0;
			int quickentryid;
			name = txtname.Text;
			description = txtdescription.Text;

			genmonth = optgenmonth.Checked;
			if (genmonth == false)
			{
				int.TryParse(txtnumrows.Text, out numrows);
			}
			cQeForms clsforms = new cQeForms((int)ViewState["accountid"]);

            if (description.Length >= 1000)
            {
                description = description.Substring(0, 999);
            }

			if ((int)ViewState["action"] == 2)
			{
				if (clsforms.updateForm((int)ViewState["quickentryid"],name,description, genmonth, numrows) == -1)
				{
					lblmsg.Text = "The form name you have entered already exists.";
					lblmsg.Visible = true;
					return false;
				}
			}
			else
			{
				quickentryid = clsforms.addForm(name,description, genmonth, numrows);
				if (quickentryid == -1)
				{
					lblmsg.Text = "The form name you have entered already exists.";
					lblmsg.Visible = true;
					return false;
				}
				ViewState["quickentryid"] = quickentryid;
			}

			return true;


		}

		private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if (updateDetails() == false)
			{
				return;
			}
			Response.Redirect("adminqeforms.aspx",true);
		}

		protected void cmdprintout_Click(object sender, System.EventArgs e)
		{
			if (updateDetails() == false)
			{
				return;
			}
			Response.Redirect("qepage.aspx?quickentryid=" + ViewState["quickentryid"],true);
		}
	}
}
