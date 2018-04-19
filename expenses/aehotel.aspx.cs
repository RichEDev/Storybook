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
using SpendManagementLibrary;
using Spend_Management;

namespace expenses
{
	/// <summary>
	/// Summary description for aehotel.
	/// </summary>
	public partial class aehotel : System.Web.UI.Page
	{

        protected void Page_Load(object sender, System.EventArgs e)
		{

			if (IsPostBack == false)
			{
				int action = 0;
				int hotelid = 0;
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

				
				if (Request.QueryString["action"] != null)
				{
					action = int.Parse(Request.QueryString["action"]);
				}
				ViewState["action"] = action;

				if (Request.QueryString["hotelid"] != null)
				{
					hotelid = int.Parse(Request.QueryString["hotelid"]);
				}
				ViewState["hotelid"] = hotelid;
				
				if (action == 2)
				{

                    Hotel reqhotel = Hotel.Get(hotelid);

					txtname.Text = reqhotel.hotelname;
					txtaddress1.Text = reqhotel.address1;
					txtaddress2.Text = reqhotel.address2;
					txtcity.Text = reqhotel.city;
					txtcounty.Text = reqhotel.county;
					txtpostcode.Text = reqhotel.postcode;
					txtcountry.Text = reqhotel.country;
					txttelno.Text = reqhotel.telno;
					txtemail.Text = reqhotel.email;
					if (cmbrating.Items.FindByValue(reqhotel.rating.ToString()) != null)
					{
						cmbrating.Items.FindByValue(reqhotel.rating.ToString());
					}
				}

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
			this.cmdok.Click += new System.Web.UI.ImageClickEventHandler(this.cmdok_Click);

		}
		#endregion

		private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			Hotel hotel = new Hotel();
			string hotelname, address1, address2, city, county, postcode, country, telno, email;
			byte rating = 0;
			int hotelid = 0;
			hotelname = txtname.Text;
			address1 = txtaddress1.Text;
			address2 = txtaddress2.Text;
			city = txtcity.Text;
			county = txtcounty.Text;
			postcode = txtpostcode.Text;
			country = txtcountry.Text;
			telno = txttelno.Text;
			email = txtemail.Text;

			if (cmbrating.SelectedValue != "")
			{
				rating = byte.Parse(cmbrating.SelectedValue);
			}
			if ((int)ViewState["action"] == 2) //update
			{
				hotel.Update((int)ViewState["hotelid"],hotelname, address1, address2, city, county, postcode, country, rating, telno, email);
                hotelid = (int)ViewState["hotelid"];
			}
			else
			{
				hotelid = hotel.Add(hotelname, address1, address2, city, county, postcode, country, rating, telno, email, 0);
				if (hotelid == -1)
				{
					lblmsg.Text = "The hotel cannot be added as it already exists.";
					lblmsg.Visible = true;
					return;
				}
			}

			Response.Redirect("hotelsearch.aspx?hotelid=" + hotelid,true);
		}
	}
}
