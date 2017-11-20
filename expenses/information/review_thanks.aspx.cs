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
using SpendManagementLibrary.Hotels;
using Spend_Management;

namespace expenses.information
{
	/// <summary>
	/// Summary description for review_thanks.
	/// </summary>
	public partial class review_thanks : System.Web.UI.Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (IsPostBack == false)
			{
                Hotel reqhotel = Hotel.Get(int.Parse(Request.QueryString["hotelid"]));
				lblmsg.Text = "Many thanks for taking part in the expenses hotel ratings service.\n\n";
				lblmsg.Text += "Your review of " + reqhotel.hotelname + " has been saved and will be available shortly.";
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
