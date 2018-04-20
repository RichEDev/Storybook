namespace Spend_Management
{
    using System;
    using System.Web;
    using System.Web.UI;
    using System.Web.Services;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using SpendManagementLibrary;

    using CurrencyType = BusinessLogic.GeneralOptions.Currencies.CurrencyType;

    /// <summary>
	/// Summary description for admincurrencies.
	/// </summary>
	public partial class admincurrencies : Page
	{
	    /// <summary>
	    /// An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/>
	    /// </summary>
        [Dependency]
	    public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory { get; set; }

		protected void Page_Load(object sender, System.EventArgs e)
		{
            CurrentUser user = cMisc.GetCurrentUser();
			Title = "Currencies";
            Master.title = Title;
            switch (user.CurrentActiveModule)
            {
                case Modules.contracts:
                    Master.helpid = 1165;
                    break;
                default:
                    Master.helpid = 1020;
                    break;
            }

            if (IsPostBack == false)
            {
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Currencies, true, true);

                if (!user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Currencies, true))
                {
                    lnkAddCurrency.Style.Add(HtmlTextWriterStyle.Display, "none");
                }

                rButStatic.Attributes.Add("onClick", "saveCurrencyType(1);");
                rButMonth.Attributes.Add("onClick", "saveCurrencyType(2);");
                rButRange.Attributes.Add("onClick", "saveCurrencyType(3);");
                
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                var generalOptions = this.GeneralOptionsFactory[user.CurrentSubAccountId].WithCurrency();

                if (generalOptions.Currency.EnableAutoUpdateOfExchangeRates)
                {
                    this.rButStatic.Enabled = false;
                    this.rButMonth.Enabled = false;
                    this.rButRange.Enabled = false;
                    this.exchangeRateComment.Style.Add("display", "block");
                }

                ViewState["currencyType"] = generalOptions.Currency.CurrencyType;

                switch (generalOptions.Currency.CurrencyType)
                {
                    case CurrencyType.Static:
                        rButStatic.Checked = true;
                        break;

                    case CurrencyType.Monthly:
                        rButMonth.Checked = true;
                        break;

                    case CurrencyType.Range:
                        rButRange.Checked = true;
                        break;
                }

                string[] gridData = CreateGrid(cmbfilter.SelectedValue);
                litgrid.Text = gridData[1];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CurrenciesGridVars", cGridNew.generateJS_init("CurrenciesGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, user.CurrentActiveModule), true);

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("var currencyType = " + (byte)generalOptions.Currency.CurrencyType + ";\n");
                
                switch (user.CurrentActiveModule)
                {
                    case Modules.SpendManagement:
                    case Modules.SmartDiligence:
                    case Modules.contracts:
                        sb.Append("if(document.getElementById('ERTypeDiv') != null) { document.getElementById('ERTypeDiv').style.display = 'none'; }\n");
                        break;
                    default:
                        break;
                }

                ClientScript.RegisterStartupScript(this.GetType(), "variables", sb.ToString(), true);
            }            
		}

        [WebMethod(EnableSession = true)]
        public static string[] CreateGrid(string FilterVal)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cGridNew newgrid = null;
            cCurrencies clsCurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);
            
            newgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridCurrencies", clsCurrencies.getGrid());
            newgrid.addFilter(((cFieldColumn)newgrid.getColumnByName("subAccountId")).field, ConditionType.Equals, new object[] { user.CurrentSubAccountId }, null, ConditionJoiner.None);
            
            if (FilterVal != "2")
            {
                int val = int.Parse(FilterVal);

                newgrid.addFilter(((cFieldColumn)newgrid.getColumnByName("archived")).field, ConditionType.Equals, new object[] { val }, null, ConditionJoiner.And);
            }
            newgrid.enablearchiving = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Currencies, true);
            newgrid.enabledeleting = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Currencies, true);
            newgrid.enableupdating = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Currencies, true);
            newgrid.editlink = "javascript:editCurrency({currencyid});"; //aecurrency.aspx?currencyid={currencyid}&currencyType=" + (byte)(CurrencyType)ViewState["currencyType"];
            newgrid.deletelink = "javascript:deleteCurrency({currencyid});";
            newgrid.archivelink = "javascript:changeArchiveStatus({currencyid});";
            newgrid.ArchiveField = "archived";
            newgrid.getColumnByName("currencyid").hidden = true;
            newgrid.getColumnByName("archived").hidden = true;
            newgrid.getColumnByName("subAccountId").hidden = true;
            newgrid.EmptyText = "No currencies to display";
            newgrid.KeyField = "currencyid";
            return newgrid.generateGrid();
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

		

        //protected void optstatic_CheckedChanged(object sender, System.EventArgs e)
        //{
        //    if (optstatic.Checked == true)
        //    {
        //        clscurrencies = new cCurrencies((int)ViewState["accountid"]);
        //        clscurrencies.changeCurrencyType(1);

        //        Response.Redirect("admincurrencies.aspx",true);
        //    }
					
        //}

        //protected void optmonthly_CheckedChanged(object sender, System.EventArgs e)
        //{
        //    if (optmonthly.Checked == true)
        //    {
        //        System.Data.DataSet rcdstcurrencies = new DataSet();
        //        System.Data.DataTable tblcurrencymonths = new System.Data.DataTable();

        //        clscurrencies = new cCurrencies((int)ViewState["accountid"]);
        //        clscurrencies.changeCurrencyType(2);




        //        Response.Redirect("admincurrencies.aspx",true);
        //    }
        //}

        //protected void optrange_CheckedChanged(object sender, System.EventArgs e)
        //{
        //    if (optrange.Checked == true)
        //    {
        //        clscurrencies = new cCurrencies((int)ViewState["accountid"]);
        //        clscurrencies.changeCurrencyType(3);

        //        Response.Redirect("admincurrencies.aspx",true);
        //    }
        //}

        [WebMethod(EnableSession = true)]
        public static int deleteCurrency(int currencyid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cCurrencies clsCurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);
            return clsCurrencies.deleteCurrency(currencyid);
        }

        [WebMethod(EnableSession = true)]
        public static int changeStatus(int currencyid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cCurrencies clsCurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);
            cCurrency currency = clsCurrencies.getCurrencyById(currencyid);

            return clsCurrencies.changeStatus(currencyid, !currency.archived);
        }

        [WebMethod(EnableSession = true)]
        public static void saveCurrencyType(CurrencyType currencyType)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                cCurrencies clsCurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);
                clsCurrencies.ChangeCurrencyType((SpendManagementLibrary.CurrencyType)currencyType, user.EmployeeID);
            }
        }

        //[WebMethod(EnableSession = true)]
        //public static string refreshCurrencyGrid(CurrencyType currencyType)
        //{
        //    cGridNew newgrid = null;

        //    if (HttpContext.Current.User.Identity.IsAuthenticated)
        //    {
        //        CurrentUser user = cMisc.GetCurrentUser();

        //        switch (currencyType)
        //        {
        //            case CurrencyType.Static:
        //                cStaticCurrencies clsStatCurrencies = new cStaticCurrencies(user.accountid);
        //                newgrid = new cGridNew(user.accountid, user.employeeid, "gridCurrencies", clsStatCurrencies.getGrid());
        //                break;

        //            case CurrencyType.Monthly:
        //                cMonthlyCurrencies clsMonthCurrencies = new cMonthlyCurrencies(user.accountid);
        //                newgrid = new cGridNew(user.accountid, user.employeeid, "gridCurrencies", clsMonthCurrencies.getGrid());
        //                break;

        //            case CurrencyType.Range:
        //                cRangeCurrencies clsRangeCurrencies = new cRangeCurrencies(user.accountid);
        //                newgrid = new cGridNew(user.accountid, user.employeeid, "gridCurrencies", clsRangeCurrencies.getGrid());
        //                break;
        //        }

        //        newgrid.enablearchiving = true;
        //        newgrid.enabledeleting = true;
        //        newgrid.enableupdating = true;
        //        newgrid.editlink = "aecurrency.aspx?currencyid={currencyid}&currencyType=" + (byte)currencyType;
        //        newgrid.deletelink = "javascript:deleteCurrency({currencyid});";
        //        newgrid.archivelink = "javascript:changeArchiveStatus({currencyid});";
        //        newgrid.ArchiveField = "archived";
        //        newgrid.getColumnByName("currencyid").hidden = true;
        //        newgrid.getColumnByName("archived").hidden = true;
        //        newgrid.KeyField = "currencyid";
        //    }

        //    return newgrid.generateGrid();
        //}


        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            switch (user.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                case Modules.SpendManagement:
                case Modules.contracts:
                    Response.Redirect("~/MenuMain.aspx?menusection=baseinfo", true);
                    break;
                default:
                    Response.Redirect("~/categorymenu.aspx", true);
                    break;
            }
        }

	}


}
