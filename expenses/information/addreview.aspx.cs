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
	/// Summary description for addreview.
	/// </summary>
	public partial class addreview : Page
	{
		protected System.Web.UI.WebControls.DropDownList cmbrating;
		protected System.Web.UI.WebControls.RadioButton Radiobutton6;
		protected System.Web.UI.WebControls.RadioButton Radiobutton7;
		protected System.Web.UI.WebControls.RadioButton Radiobutton8;
		protected System.Web.UI.WebControls.RadioButton Radiobutton9;
		protected System.Web.UI.WebControls.RadioButton Radiobutton10;
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Master.showdummymenu = true;
			if (IsPostBack == false)
			{
                Title = "Review This Hotel";
                Master.title = Title;
                Master.enablenavigation = false;
				int hotelid = int.Parse(Request.QueryString["hotelid"]);
				ViewState["hotelid"] = hotelid;

				if (Request.QueryString["close"] != null)
				{
					ViewState["close"] = 1;
				}
                Hotel reqhotel = Hotel.Get(hotelid);
				lblhotel.Text = reqhotel.hotelname;
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
			Response.Redirect("reviewdetails.aspx?hotelid=" + ViewState["hotelid"],true);
		}

		private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            CurrentUser user = cMisc.GetCurrentUser();
            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID;

			cReviews clsreviews = new cReviews();
			byte rating, standardrooms, hotelfacilities, valuemoney, performancestaff, location;
			string review;
			byte displaytype = 1;
			decimal amountpaid = 0;

			rating = byte.Parse(optrating.SelectedValue);
			review = txtreview.Text;
			if (optdispname.Checked == true)
			{
				displaytype = 1;
			}
			if (optdispnamecomp.Checked == true)
			{
				displaytype = 2;
			}
			if (optdispanon.Checked == true)
			{
				displaytype = 3;
			}
			if (txtpaid.Text != "")
			{
				amountpaid = decimal.Parse(txtpaid.Text);
			}

			standardrooms = byte.Parse(optrooms.SelectedValue);
			hotelfacilities = byte.Parse(optfacilities.SelectedValue);
			valuemoney = byte.Parse(optvalue.SelectedValue);
			performancestaff = byte.Parse(optperformance.SelectedValue);
			location = byte.Parse(optlocation.SelectedValue);


            clsreviews.addReview((int)ViewState["hotelid"], rating, review, displaytype, (int)ViewState["employeeid"], amountpaid, standardrooms, hotelfacilities, valuemoney, performancestaff, location);

			if (ViewState["close"] != null)
			{
				Response.Redirect("review_thanks.aspx?hotelid=" + ViewState["hotelid"],true);
			}
			else
			{
				Response.Redirect("reviewdetails.aspx?hotelid=" + ViewState["hotelid"],true);
			}
			
		}
	}
}
