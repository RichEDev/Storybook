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

namespace expenses
{
	/// <summary>
	/// Summary description for hotelsearch.
	/// </summary>
	public partial class hotelsearch : System.Web.UI.Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (IsPostBack == false)
			{
				string onload;
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                Hotels hotels = new Hotels();
				onload = "<script language=javascript>\n";
				onload += "function window_onload()\n";
				onload += "{\n";
				if (Request.QueryString["hotelid"] != null)
				{				
					Hotel reqhotel = Hotel.Get(int.Parse(Request.QueryString["hotelid"]));
					onload += "populateHotelDetails(" + reqhotel.hotelid + ",'" + reqhotel.hotelname + "');\n";			
					onload += "window.opener.getHotel();\n";
				}

				onload += "}\n";
				onload += "</script>";
				this.RegisterClientScriptBlock("onload",onload);
                cmbcounty.DataSource = hotels.GetCountyList();
				cmbcounty.DataTextField = "county";
				cmbcounty.DataBind();
				cmbcounty.Items.Insert(0,new System.Web.UI.WebControls.ListItem("{All Counties}",""));

                cColours clscolours = new cColours(user.AccountID, user.CurrentSubAccountId, user.CurrentActiveModule);

                System.Text.StringBuilder output = new System.Text.StringBuilder();
                output.Append("<style type\"text/css\">\n");
                if (clscolours.sectionHeadingUnderlineColour != clscolours.defaultSectionHeadingUnderlineColour)
                {
                    output.Append(".infobar\n");
                    output.Append("{\n");
                    output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                    output.Append("}\n");
                    output.Append(".datatbl th\n");
                    output.Append("{\n");
                    output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                    output.Append("}\n");
                    output.Append(".inputpaneltitle\n");
                    output.Append("{\n");
                    output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                    output.Append("border-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                    output.Append("}\n");
                    output.Append(".paneltitle\n");
                    output.Append("{\n");
                    output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                    output.Append("border-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                    output.Append("}\n");
                    output.Append(".homepaneltitle\n");
                    output.Append("{\n");
                    output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                    output.Append("border-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                    output.Append("}\n");
                }
                if (clscolours.rowBGColour != clscolours.defaultRowBGColour)
                {
                    output.Append(".datatbl .row1\n");
                    output.Append("{\n");
                    output.Append("background-color: " + clscolours.rowBGColour + ";\n");
                    output.Append("}\n");
                }
                if (clscolours.rowTxtColour != clscolours.defaultRowTxtColour)
                {
                    output.Append(".datatbl .row1\n");
                    output.Append("{\n");
                    output.Append("color: " + clscolours.rowTxtColour + ";\n");
                    output.Append("}\n");
                }
                if (clscolours.altRowBGColour != clscolours.defaultAltRowBGColour)
                {
                    output.Append(".datatbl .row2\n");
                    output.Append("{\n");
                    output.Append("background-color: " + clscolours.altRowBGColour + ";\n");
                    output.Append("}\n");
                }
                if (clscolours.altRowTxtColour != clscolours.defaultAltRowTxtColour)
                {
                    output.Append(".datatbl .row2\n");
                    output.Append("{\n");
                    output.Append("color: " + clscolours.altRowTxtColour + ";\n");
                    output.Append("}\n");
                }
                if (clscolours.fieldTxtColour != clscolours.defaultFieldTxt)
                {
                    output.Append(".labeltd\n");
                    output.Append("{\n");
                    output.Append("color: " + clscolours.fieldTxtColour + ";\n");
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

		protected void cmbcounty_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Hotels clshotels = new Hotels();
			string county = cmbcounty.SelectedValue;
			cmbcity.DataSource = clshotels.GetCityList(county);
			cmbcity.DataTextField = "city";
			cmbcity.DataBind();

			cmbcity.Items.Insert(0,new System.Web.UI.WebControls.ListItem("{All Cities}",""));
		}

		protected void cmdsearch_Click(object sender, System.EventArgs e)
		{
			Hotels clshotels = new Hotels();
			byte searchtype = byte.Parse(Request.Form["searchtype"]);
			string county, city;

			string hotelname;
			if (searchtype == 1) //search by name
			{
				hotelname = txthotelname.Text;
				litgrid.Text = populateHotelGrid(clshotels.SearchForHotels(hotelname));
			}
			else //search by county
			{
				county = cmbcounty.SelectedValue;
				city = cmbcity.SelectedValue;
				litgrid.Text = populateHotelGrid(clshotels.SearchForHotels(county,city));
			}

			
		}

		public string populateHotelGrid (System.Data.DataSet ds)
		{
			int i;
			string rowclass = "row1";
			System.Text.StringBuilder output = new System.Text.StringBuilder();
			output.Append("<table class=datatbl>");
			output.Append("<tr><th></th><th>Hotel Name</th><th>County</th><th>City</th></tr>");
			for (i = 0; i < ds.Tables[0].Rows.Count; i++)
			{
				output.Append("<tr>");
				output.Append("<td class=" + rowclass + "><input onclick=\"populateHotelDetails(" + ds.Tables[0].Rows[i]["hotelid"] + ",'" + ds.Tables[0].Rows[i]["hotelname"] + "');\" type=radio name=hotelids></td>");
				output.Append("<td class=" + rowclass + ">" + ds.Tables[0].Rows[i]["hotelname"] + "</td>");
				output.Append("<td class=" + rowclass + ">" + ds.Tables[0].Rows[i]["county"] + "</td>");
				output.Append("<td class=" + rowclass + ">" + ds.Tables[0].Rows[i]["city"] + "</td>");
				output.Append("</tr>");
				if (rowclass == "row1")
				{
					rowclass = "row2";
				}
				else
				{
					rowclass = "row1";
				}
			}
			output.Append("</table>");

			output.Append("<table align=center><tr><td>If the hotel you are looking for is not listed, <a href=\"aehotel.aspx\">Click Here</a></td></tr></table>");
			return output.ToString();
		}
	}
}
