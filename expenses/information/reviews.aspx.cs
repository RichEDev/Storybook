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

namespace expenses.information
{
	/// <summary>
	/// Summary description for reviews.
	/// </summary>
	public partial class reviews : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Title = "Hotel Reviews";
            Master.title = Title;
			if (IsPostBack == false)
			{
				cReviews clsreviews = new cReviews();
				string county = "";
				string city = "";

				if (Request.QueryString["county"] != null)
				{
					county = Request.QueryString["county"];
				}
				if (Request.QueryString["city"] != null)
				{
					city = Request.QueryString["city"];
				}

				if (county == "" && city == "")
				{
					litgrid.Text = clsreviews.getCountyGrid();
				}
				else if (county != "" && city == "")
				{
					litgrid.Text = clsreviews.getCityGrid(county);
				}
				else if (county != "" && city != "")
				{
					litgrid.Text = clsreviews.getHotelGrid(county,city);
				}

			}
		}

        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string sPreviousURL = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(sPreviousURL, true);
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
