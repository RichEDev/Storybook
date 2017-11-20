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
using System.Web.Services;
using System.Collections.Generic;
using System.Text;


namespace Spend_Management
{
	/// <summary>
	/// Summary description for aeallowance.
	/// </summary>
	public partial class aeallowance : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
			Title = "Add/Edit Allowance";
            Master.title = Title;
            Master.PageSubTitle = "Allowance Details";
            Master.helpid = 1003;

			Response.Expires = -1;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";
			Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
			
			
			if (IsPostBack == false)
			{
                Master.enablenavigation = false;
				
				System.Text.StringBuilder output = new System.Text.StringBuilder();
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Allowances, true, true);
				
				cCurrencies clscurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);

                int allowanceid = 0;
                if (Request.QueryString["allowanceid"] != null)
                {
                    int.TryParse(Request.QueryString["allowanceid"], out allowanceid);
                }

                string[] gridData;
				if (allowanceid > 0) //update
				{
                    cAllowances clsallowances = new cAllowances(user.AccountID);
					cAllowance reqallowance = clsallowances.getAllowanceById(allowanceid);

					txtname.Text = reqallowance.allowance;
					txtdescription.Text = reqallowance.description;
					txtnightrate.Text = reqallowance.nightrate.ToString("######0.00");
					txtnighthours.Text = reqallowance.nighthours.ToString();
					cmbcurrencies.Items.AddRange(clscurrencies.CreateDropDown(reqallowance.currencyid));

                    gridData = createRatesGrid(allowanceid.ToString());
                    Master.title = "Allowance: " + reqallowance.allowance;
				}
				else
				{
					cmbcurrencies.Items.AddRange(clscurrencies.CreateDropDown(0));
                    gridData = createRatesGrid("");
                    Master.title = "Allowance: New";
				}

                litrates.Text = gridData[1];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "allowanceGridVars", cGridNew.generateJS_init("allowanceGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);

                this.ClientScript.RegisterStartupScript(this.GetType(), "startup", "var allowanceID = " + allowanceid + ";", true);
			}
		}

        [WebMethod(EnableSession = true)]
        public static int saveAllowance(int allowanceID, string name, string description, int numhours, decimal rate, int currencyid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cAllowances clsallowances = new cAllowances(user.AccountID);
            cAllowance allowance;
            if (allowanceID > 0)
            {
                cAllowance oldallowance = clsallowances.getAllowanceById(allowanceID);
                allowance = new cAllowance(user.AccountID, allowanceID, name, description, currencyid, numhours, rate, oldallowance.createdon, oldallowance.createdby, DateTime.Now, user.EmployeeID, oldallowance.breakdown);
            }
            else
            {
                allowance = new cAllowance(user.AccountID, allowanceID, name, description, currencyid, numhours, rate, DateTime.Now, user.EmployeeID, null, null, new List<cAllowanceBreakdown>());
            }
            return clsallowances.saveAllowance(allowance);
        }

        [WebMethod(EnableSession = true)]
        public static int saveRate(int rateID, int allowanceID, int hours, decimal rate)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cAllowances clsallowances = new cAllowances(user.AccountID);
            cAllowanceBreakdown breakdown = new cAllowanceBreakdown(rateID, allowanceID, hours, rate);
            return clsallowances.saveRate(breakdown);
        }
        [WebMethod(EnableSession = true)]
        public static cAllowanceBreakdown getRate(int rateID, int allowanceID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cAllowances clsallowances = new cAllowances(user.AccountID);
            cAllowance allowance = clsallowances.getAllowanceById(allowanceID);
            return allowance.getAllowanceBreakdownByID(rateID);
        }
        [WebMethod(EnableSession = true)]
        public static bool deleteRate(int rateID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cAllowances clsallowances = new cAllowances(user.AccountID);
            clsallowances.deleteAllowanceBreakdown(rateID);
            return true;
        }
        [WebMethod(EnableSession = true)]
        public static string[] createRatesGrid(string contextKey)
        {
            int allowanceid = 0;
            if (contextKey != "")
            {
                int.TryParse(contextKey,out allowanceid);
            }
            CurrentUser user = cMisc.GetCurrentUser();
            cAllowances clsallowances = new cAllowances(user.AccountID);
            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridRates", clsallowances.getBreakdown());
            cFields clsfields = new cFields(user.AccountID);
            clsgrid.addFilter(clsfields.GetFieldByID(new Guid("e23baff1-0e47-4cdf-97b5-5cb9d05f1ecb")), ConditionType.Equals, new object[] { allowanceid }, null, ConditionJoiner.None);
            clsgrid.getColumnByName("breakdownid").hidden = true;
            clsgrid.KeyField = "breakdownid";
            clsgrid.enableupdating = true;
            clsgrid.editlink = "javascript:editRate({breakdownid});";
            clsgrid.enabledeleting = true;
            clsgrid.deletelink = "javascript:deleteRate({breakdownid});";
            clsgrid.EmptyText = "There are no rates defined for this allowance.";
            return clsgrid.generateGrid();
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
