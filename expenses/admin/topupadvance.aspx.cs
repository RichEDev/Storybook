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

namespace expenses.admin
{
	/// <summary>
	/// Summary description for topupadvance.
	/// </summary>
	public partial class topupadvance : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{

			Title = "Top-Up Advance";
            Master.title = Title;
            Master.helpid = 1055;
			

			
			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Advances, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

				int floatid = int.Parse(Request.QueryString["floatid"]);
				ViewState["floatid"] = floatid;

                cFloats clsFloats = new cFloats(user.AccountID);
                cFloat reqFloat = clsFloats.GetFloatById(floatid);
                cCurrencies clsCurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);
                cCurrency reqCurrency = clsCurrencies.getCurrencyById(reqFloat.currencyid);
                cGlobalCurrencies clsGlobalCurrencies = new cGlobalCurrencies();
                cGlobalCurrency reqGlobalCurrency = clsGlobalCurrencies.getGlobalCurrencyById(reqCurrency.globalcurrencyid);
                lblForeignCurrencyType.Text = reqGlobalCurrency.label;
                txtForeignAmount.Text = "0";
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
			Response.Redirect("adminfloats.aspx",true);
		}

		private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			cFloats clsfloats = new cFloats((int)ViewState["accountid"]);
			int floatid = (int)ViewState["floatid"];
            decimal foreignAmount = decimal.Parse(txtForeignAmount.Text);

            if (clsfloats.topUpAdvance(floatid, foreignAmount) == true)
            {
                Response.Redirect("adminfloats.aspx", true);
            }
            else
            {
                litUpdateResponse.Text = "Advance not updated as this would place the advance into a negative amount.";
            }
		}
	}
}
