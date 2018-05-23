using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using SpendManagementLibrary;
using Spend_Management;
using System.Collections.Generic;
using System.Web.Services;
using System.Text;

namespace Spend_Management.shared.admin
{
    using BusinessLogic.Modules;

    /// <summary>
	/// Add/Edit countries and their associated VAT rates for expense items.
	/// </summary>
	public partial class aecountry : Page
	{
		protected System.Web.UI.WebControls.ImageButton cmdhelp;
        int countryid;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            CurrentUser user = cMisc.GetCurrentUser();
            switch (user.CurrentActiveModule)
            {
                case Modules.Contracts:
                    Master.helpid = 0;
                    break;
                default:
                    Master.helpid = 1019;
                    break;
            }

            countryid = 0;

			if (IsPostBack == false)
			{
                cmdok.Attributes.Add("onclick", "if (validateform('vgMain') == false) {return;}");
                cmbcountry.Attributes.Add("onchange", "getCountryCode();");
                Master.enablenavigation = false;
                
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Countries, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;
                ViewState["subAccountID"] = user.CurrentSubAccountId;

                cCountries clsCountries = new cCountries(user.AccountID, user.CurrentSubAccountId);

                cGlobalCountries clsglobalcountries = new cGlobalCountries();
                cmbcountry.Items.AddRange(clsglobalcountries.CreateDropDown().ToArray());

                cSubcats clssubcats = new cSubcats((int)ViewState["accountid"]);
                cmbSubcat.Items.AddRange(clssubcats.CreateDropDown().ToArray());
                
                if (Request.QueryString["countryid"] != null)
                {
                    countryid = int.Parse(Request.QueryString["countryid"]);
                }
                ViewState["countryid"] = countryid;

                //if (reqcountry == null)
                //{
                //    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                //}
                cGlobalCountry global;
                string[] gridData = null;

				if (countryid > 0)
				{
                    var reqcountry = clsCountries.getCountryById(countryid);
                    if (reqcountry == null)
                    {
                        Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                    }

                    user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Countries, true, true);

                    global = clsglobalcountries.getGlobalCountryById(reqcountry.GlobalCountryId);
                    cmbcountry.Enabled = false;
                    if (cmbcountry.Items.FindByValue(global.GlobalCountryId.ToString()) != null)
                    {
                        cmbcountry.Items.FindByValue(global.GlobalCountryId.ToString()).Selected = true;
                    }
                    txtCountryCode.Text = global.CountryCode;

                    //Remove any subcats from the subcat dropdown that are already associated to this country.
                    foreach (var existingItem in reqcountry.VatRates.Values.Select(vatRate => cmbSubcat.Items.FindByValue(vatRate.SubcatId.ToString())))
                    {
                        cmbSubcat.Items.Remove(existingItem);
                    }
                    gridData = CreateGrid(countryid.ToString());
                    
					Master.title = "Country: " + cmbcountry.SelectedItem.Text;                    
				}
				else
				{
                    user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Countries, true, true);

                    global = clsglobalcountries.getGlobalCountryById(int.Parse(cmbcountry.SelectedValue));
                    gridData = CreateGrid("");
                    
                    Master.title = "Country: New";
				    //remove already assigned
                    foreach (var existingItem in clsCountries.list.Values.Select(country => cmbcountry.Items.FindByValue(country.GlobalCountryId.ToString())))
                    {
                        cmbcountry.Items.Remove(existingItem);
                    }
				}

                litgrid.Text = gridData[2];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "countryGridVars", cGridNew.generateJS_init("countryGridVars", new List<string>() { gridData[1] }, user.CurrentActiveModule), true);

                txtCountryCode.Text = global.CountryCode;
                txtAlpha3.Text = global.Alpha3CountryCode;
                if (global.Numeric3CountryCode < 10)
                {
                    txtNumeric3.Text = "00" + global.Numeric3CountryCode.ToString();
                }
                else if (global.Numeric3CountryCode < 100)
                {
                    txtNumeric3.Text = "0" + global.Numeric3CountryCode.ToString();
                }
                else
                {
                    txtNumeric3.Text = global.Numeric3CountryCode.ToString();
                }

			    this.chkPostcodeAnywhereEnabled.Checked = global.PostcodeAnywhereEnabled;

                Master.PageSubTitle = "Country Details";
                                
                ClientScript.RegisterClientScriptBlock(this.GetType(), "variables", "var countryid = " + countryid + ";", true);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "accvariables", "var accountid = " + user.AccountID + ";", true);

                switch (user.CurrentActiveModule)
                {
                    case Modules.SpendManagement:
                    case Modules.SmartDiligence:
                    case Modules.Contracts:
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.Append("if(document.getElementById('lnkNewVatRate') != null) { document.getElementById('lnkNewVatRate').style.display = 'none'; }\n");
                        sb.Append("if(document.getElementById('" + pnlrates.ClientID + "') != null) { document.getElementById('" + pnlrates.ClientID + "').style.display = 'none'; }\n");
                        ClientScript.RegisterStartupScript(this.GetType(), "hideVat", sb.ToString(), true);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Creates the grid for the country's VAT rates
        /// </summary>
        /// <param name="contextKey">
        /// The context Key.
        /// </param>
        [WebMethod(EnableSession = true)]
        public static string[] CreateGrid(string contextKey)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            int countryid = 0;
            if (!string.IsNullOrEmpty(contextKey))
            {
                int.TryParse(contextKey, out countryid);
            }

            var clsCountries = new cCountries(user.AccountID, user.CurrentSubAccountId);
            var newgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridCountrySubcats", clsCountries.getForeignVATGrid());

            var clsFields = new cFields(user.AccountID);
            var values = new object[] { countryid };

            cField countryId = clsFields.GetFieldByID(new Guid("33603D8A-C2F8-4662-A815-C59C1E282CE8"));
            newgrid.addFilter(countryId, ConditionType.Equals, values, null, ConditionJoiner.None);
            newgrid.enablearchiving = false;
            newgrid.enabledeleting = true;
            newgrid.enableupdating = true;
            newgrid.EmptyText = "There are currently no VAT rates set up";
            newgrid.editlink = "javascript:showEditSubcatModal({countrysubcatid}, '{subcat}', {subcatid}, {vat}, {vatpercent});";
            newgrid.deletelink = "javascript:deleteCountrySubcat({countrysubcatid});";
            newgrid.getColumnByName("countrysubcatid").hidden = true;
            newgrid.getColumnByName("subcatid").hidden = true;
            newgrid.KeyField = "countrysubcatid";

            var retVals = new List<string> { newgrid.GridID };
            retVals.AddRange(newgrid.generateGrid());
            return retVals.ToArray();
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
			Response.Redirect("admincountries.aspx",true);
		}

		private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            cCountries clsCountries = new cCountries((int)ViewState["accountid"], (int)ViewState["subAccountID"]);
			
            int globalcountryid = int.Parse(cmbcountry.SelectedValue);

            cCountry newCountry = new cCountry(countryid, globalcountryid, false, new Dictionary<int,ForeignVatRate>(), DateTime.UtcNow, (int)ViewState["employeeid"]);

            countryid = clsCountries.saveCountry(newCountry);
            string suffix = "";

		    Response.Redirect("admincountries.aspx", true);
            Response.Redirect("admincountries.aspx" + suffix, true);
		}

        //protected void cmbcountry_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    int countryid = int.Parse(cmbcountry.SelectedValue);
        //    cGlobalCountries clscountries = new cGlobalCountries();
        //    cGlobalCountry reqGlobalCountry =  clscountries.getGlobalCountryById(countryid);
        //    txtCountryCode.Text = reqGlobalCountry.countrycode;
        //    txtNumeric3.Text = reqGlobalCountry.Numeric3CountryCode.ToString();
        //    txtAlpha3.Text = reqGlobalCountry.Alpha3CountryCode;
        //}

        [WebMethod(EnableSession = true)]
        public static int saveCountry(int countryid, int globalcountryid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cCountries clsCountries = new cCountries(user.AccountID, user.CurrentSubAccountId);

            cCountry newCountry = new cCountry(countryid, globalcountryid, false, new Dictionary<int, ForeignVatRate>(), DateTime.UtcNow, user.EmployeeID);

            countryid = clsCountries.saveCountry(newCountry);

            return countryid;
        }

        [WebMethod(EnableSession = true)]
        public static void saveCountrySubcat(int countryid, int countrysubcatid, int subcatid, double vatRate, double claimablePercentage)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cCountries clsCountries = new cCountries(user.AccountID, user.CurrentSubAccountId);
            ForeignVatRate forVatRate = new ForeignVatRate();
            forVatRate.CountryId = countryid;
            forVatRate.SubcatId = subcatid;
            forVatRate.Vat = vatRate;
            forVatRate.VatPercent = claimablePercentage;
            clsCountries.saveVatRate(forVatRate, countrysubcatid, user.EmployeeID);
        }

        [WebMethod(EnableSession = true)]
        public static void deleteCountrySubcat(int countrysubcatid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cCountries clsCountries = new cCountries(user.AccountID, user.CurrentSubAccountId);
            clsCountries.deleteVatRate(countrysubcatid);
        }

        [WebMethod(EnableSession=true)]
        public static object[] refreshSubcatDropdown(int countryid, int subcatid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cSubcats clssubcats = new cSubcats(user.AccountID);
            cCountries clsCountries = new cCountries(user.AccountID, user.CurrentSubAccountId);
            cCountry country = clsCountries.getCountryById(countryid);

            ArrayList objList = new ArrayList();
            bool isUsed;

            List<ListItem> lstItems = clssubcats.CreateDropDown();

            foreach (ListItem lstitem in lstItems)
            {
                isUsed = false;
                foreach (ForeignVatRate rate in country.VatRates.Values)
                {
                    if (lstitem.Value != subcatid.ToString())
                    {
                        if (lstitem.Value == rate.SubcatId.ToString())
                        {
                            isUsed = true;
                            break;
                        }
                    }
                }

                if (!isUsed)
                {
                    objList.Add(new object[] { lstitem.Text, lstitem.Value });
                }
            }

            return objList.ToArray();
        }

        [WebMethod(EnableSession = true)]
        public static List<string> getCountryCode(int globalCountryID)
        {
            cGlobalCountries clsGlobalCountries = new cGlobalCountries();
            cGlobalCountry country = clsGlobalCountries.getGlobalCountryById(globalCountryID);

            List<string> lstCodes = new List<string>();
            lstCodes.Add(country.CountryCode);
            lstCodes.Add(country.Alpha3CountryCode);
            lstCodes.Add(country.Numeric3CountryCode.ToString());
            lstCodes.Add(country.PostcodeAnywhereEnabled.ToString());
            return lstCodes;
        }

	}
}
