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
	/// <summary>
	/// Summary description for disputeadvance.
	/// </summary>
	public partial class disputeadvance : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Title = "Dispute Advance";
            Master.title = Title;
            Master.helpid = 1162;
			
			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

				

				int floatid = int.Parse(Request.QueryString["floatid"]);
				ViewState["floatid"] = floatid;

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

		private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			Response.Redirect("myadvances.aspx",true);
		}

		private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			string dispute = txtreason.Text;

			cFloats clsfloats = new cFloats((int)ViewState["accountid"]);
			cFloat reqfloat = clsfloats.GetFloatById((int)ViewState["floatid"]);
			clsfloats.disputeAdvance((int)ViewState["accountid"],dispute, reqfloat.floatid);

			Response.Redirect("myadvances.aspx",true);
		}
	}
}
