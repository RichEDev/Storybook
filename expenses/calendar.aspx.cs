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
using Spend_Management;

namespace expenses
{
	/// <summary>
	/// Summary description for calendar.
	/// </summary>
	public partial class calendar : System.Web.UI.Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

				
				System.Text.StringBuilder output = new System.Text.StringBuilder();
                cColours clscolours = new cColours(user.AccountID);
				output.Append("<style type\"text/css\">\n");
				if (clscolours.titlebarBGColour != clscolours.defaultTitlebarBGColour)
				{
					output.Append(".infobar\n");
					output.Append("{\n");
					output.Append("background-color: " + clscolours.titlebarBGColour + ";\n");
					output.Append("}\n");
					output.Append(".datatbl th\n");
					output.Append("{\n");
					output.Append("background-color: " + clscolours.titlebarBGColour + ";\n");
					output.Append("}\n");
					output.Append(".calendar th\n");
					output.Append("{\n");
					output.Append("background-color: " + clscolours.titlebarBGColour + ";\n");
					output.Append("}\n");
					output.Append(".inputpaneltitle\n");
					output.Append("{\n");
					output.Append("background-color: " + clscolours.titlebarBGColour + ";\n");
					output.Append("border-color: " + clscolours.titlebarBGColour + ";\n");
					output.Append("}\n");
					output.Append(".paneltitle\n");
					output.Append("{\n");
					output.Append("background-color: " + clscolours.titlebarBGColour + ";\n");
					output.Append("border-color: " + clscolours.titlebarBGColour + ";\n");
					output.Append("}\n");
					output.Append(".homepaneltitle\n");
					output.Append("{\n");
					output.Append("background-color: " + clscolours.titlebarBGColour + ";\n");
					output.Append("border-color: " + clscolours.titlebarBGColour + ";\n");
					output.Append("}\n");
				}
				if (clscolours.rowColour != clscolours.defaultRowColour)
				{
					output.Append(".datatbl .row1\n");
					output.Append("{\n");
					output.Append("background-color: " + clscolours.rowColour + ";\n");
					output.Append("}\n");
					output.Append(".calendar .row1\n");
					output.Append("{\n");
					output.Append("background-color: " + clscolours.rowColour + ";\n");
					output.Append("}\n");
				}
				if (clscolours.altRowColour != clscolours.defaultAltRowColour)
				{
					output.Append(".datatbl .row2\n");
					output.Append("{\n");
					output.Append("background-color: " + clscolours.altRowColour + ";\n");
					output.Append("}\n");
					output.Append(".calendar .row2\n");
					output.Append("{\n");
					output.Append("background-color:" + clscolours.altRowColour + ";\n");
					output.Append("}\n");
				}
				if (clscolours.fieldBG != clscolours.defaultFieldBg)
				{
					output.Append(".labeltd\n");
					output.Append("{\n");
					output.Append("background-color: " + clscolours.fieldBG + ";\n");
					output.Append("}\n");
				}
				if (clscolours.fieldFG != clscolours.defaultFieldFG)
				{
					output.Append(".labeltd\n");
					output.Append("{\n");
					output.Append("color: " + clscolours.fieldFG + ";\n");
					output.Append("}\n");
				}
				output.Append("</style>");
				litstyles.Text = output.ToString();
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
	}
}
