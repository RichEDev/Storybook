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
	/// Summary description for reviewdetails.
	/// </summary>
	public partial class reviewdetails : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Title = "Hotel Details";
            Master.title = Title;
			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

				int hotelid = int.Parse(Request.QueryString["hotelid"]);
				ViewState["hotelid"] = hotelid;
				string address = "";

                Hotel reqhotel = Hotel.Get(hotelid);
        
				lblname.Text = reqhotel.hotelname;

                if (reqhotel.address1 != "")
                {
                    address += reqhotel.address1 + "<br />";
                }
				if (reqhotel.address2 != "")
				{
                    address += reqhotel.address2 + "<br />";
				}
				if (reqhotel.city != "")
				{
                    address += reqhotel.city + "<br />";
				}
				if (reqhotel.county != "")
				{
                    address += reqhotel.county + "<br />";
				}
				if (reqhotel.postcode != "")
				{
                    address += reqhotel.postcode + "<br />";
				}
				if (reqhotel.country != "")
				{
                    address += reqhotel.country + "<br />";
				}
				lbladdress.Text = address;
				lbltelno.Text = reqhotel.telno;
				if (reqhotel.email != "")
				{
					lblemail.Text = "<a href=\"mailto:" + reqhotel.email + "\">" + reqhotel.email + "</a>";
				}
				
				cReviews clsreviews = new cReviews();

                litreviews.Text = clsreviews.getReviewsGrid(user.AccountID, hotelid);
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

		protected void cmdreview_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("addreview.aspx?hotelid=" + ViewState["hotelid"],true);
		}
	}
}
