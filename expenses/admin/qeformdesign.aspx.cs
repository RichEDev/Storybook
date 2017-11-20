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
using expenses.Old_App_Code;

namespace expenses.admin
{
	/// <summary>
	/// Summary description for qeformdesign.
	/// </summary>
	public partial class qeformdesign : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Title = "Form Design";
            Master.title = Title;
			Master.showdummymenu = true;
			Master.onloadfunc = "window_onload();";
            Master.helpid = 1045;
			if (IsPostBack == false)
			{
				int quickentryid;
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

				int.TryParse(Request.QueryString["quickentryid"], out quickentryid);

                cQeForms clsforms = new cQeForms(user.AccountID);
				cQeForm reqform = clsforms.getFormById(quickentryid);
				
				ViewState["quickentryid"] = quickentryid;

				System.Collections.ArrayList fields = clsforms.getAvailableFields(quickentryid);
				populateAvailableFields(fields);

				object[,] availsubcats = clsforms.getAvailableSubcats(quickentryid);
				populateAvailableSubcats(availsubcats);

				createJavaColumns(reqform.columns);
			}
		}

		private void populateAvailableSubcats(object[,] subcats)
		{
			int i;
			System.Text.StringBuilder output = new System.Text.StringBuilder();

			output.Append("<select name=availsubcats id=availsubcats size=10>\n");
			for (i = 0; i < subcats.GetLength(0); i++)
			{
				output.Append("<option value=\"" + subcats[i,0] + "\">" + subcats[i,1] + "</option>");
			}
			output.Append("</select>");
			litsubcats.Text = output.ToString();
		}
		private void populateAvailableFields(System.Collections.ArrayList fields)
		{
			int i;
			System.Text.StringBuilder output = new System.Text.StringBuilder();
			cUserdefinedFields clsuserdefined = new cUserdefinedFields((int)ViewState["accountid"]);
			cMisc clsmisc = new cMisc((int)ViewState["accountid"]);
			System.Collections.SortedList sortedlst = new SortedList();
			for (i = 0; i < fields.Count; i++)
			{
				switch (fields[i].ToString().ToUpper())
				{
					case "7CF61909-8D25-4230-84A9-F5701268F94B":
                        sortedlst.Add(clsmisc.GetGeneralFieldByCode("otherdetails").description, (Guid)fields[i]);
						break;
					case "1EE53AE2-2CDF-41B4-9081-1789ADF03459":
                        sortedlst.Add(clsmisc.GetGeneralFieldByCode("currency").description, (Guid)fields[i]);
						break;
					case "EC527561-DFEE-48C7-A126-0910F8E031B0":
                        sortedlst.Add(clsmisc.GetGeneralFieldByCode("country").description, (Guid)fields[i]);
						break;
					case "AF839FE7-8A52-4BD1-962C-8A87F22D4A10":
                        sortedlst.Add(clsmisc.GetGeneralFieldByCode("reason").description, (Guid)fields[i]);
						break;
					default:
                        sortedlst.Add(clsuserdefined.GetUserdefinedFieldByFieldID((Guid)fields[i]).description, (Guid)fields[i]);
						break;  
				}
			}

			output.Append("<select name=availfields id=availfields size=10>\n");
			for (i = 0; i < sortedlst.Count; i++)
			{
				output.Append("<option value=\"" + sortedlst.GetByIndex(i) + "\">" + sortedlst.GetKey(i) + "</option>");
			}
			output.Append("</select>");
			litfields.Text = output.ToString();
		}


		private void createJavaColumns(cQeColumn[] columns)
		{
			cQeFieldColumn fieldcol;
			cQeSubcatColumn subcatcol;
			System.Text.StringBuilder output = new System.Text.StringBuilder();
			output.Append("<script language=javascript>\n");
			output.Append("function window_onload()\n");
			output.Append("{\n");
			if (columns.Length == 0) //new form, add the date
			{
                output.Append("columns[0] = new Array (1,1,'a52b4423-c766-47bb-8bf3-489400946b4c','Date');\n");
			}
			else
			{
				int i;
				for (i = 0; i < columns.Length; i++)
				{
					output.Append("columns[" + i + "] = new Array(" + (i+1) + ",");
					if (columns[i].GetType().ToString() == "expenses.cQeFieldColumn")
					{
						fieldcol = (cQeFieldColumn)columns[i];
                        output.Append("1,'" + fieldcol.field.FieldID + "','" + fieldcol.field.Description + "'");
					}
					else
					{
						subcatcol = (cQeSubcatColumn)columns[i];
						output.Append("2," + subcatcol.subcat.subcatid + ",'" + subcatcol.subcat.subcat + "'");
					}
					output.Append(");\n");
				}
			}
			output.Append("createTable();\n");
			output.Append("}\n");
			output.Append("</script>");
			this.RegisterClientScriptBlock("windowonload",output.ToString());
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
			cQeForms clsforms = new cQeForms((int)ViewState["accountid"]);
			cQeForm reqform = clsforms.getFormById((int)ViewState["quickentryid"]);

			string colid = "";
			string coltype = "";
			string[] arrcolid;
			string[] arrcoltype;
			int i;
			if (Request.Form["colid"] != null)
			{
				colid = Request.Form["colid"];
				coltype = Request.Form["coltype"];
			}

			if (colid == "")
			{
				return;
			}

			arrcolid = colid.Split(',');
			arrcoltype = coltype.Split(',');

			object[,] columns = new object[arrcolid.Length,2];

			for (i = 0; i < arrcolid.Length; i++)
			{
                columns[i, 0] = int.Parse(arrcoltype[i]);
                columns[i, 1] = arrcolid[i];
			}

			reqform.updateColumns(columns);
			Response.Redirect("aeqeform.aspx?action=2&quickentryid=" + reqform.quickentryid,true);
		}

		private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			Response.Redirect("aeqeform.aspx?action=2&quickentryid=" + ViewState["quickentryid"],true);
		}
	}
}
