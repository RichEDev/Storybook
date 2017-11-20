using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Spend_Management.shared.code;
using SpendManagementLibrary.Enumerators;
using SpendManagementLibrary;

namespace Spend_Management.shared.admin
{
    /// <summary>
    /// This class contains methods for fund availabilty and transaction
    /// </summary>
    public partial class ViewFundsDetails : System.Web.UI.Page
    {
        
        private readonly Utilities.DistributedCaching.Cache cache = new Utilities.DistributedCaching.Cache();
        private const string CacheKey = "CacheCurrencySymbol";
        CurrentUser reqCurrentUser = cMisc.GetCurrentUser();
       

        /// <summary>
        /// Called when page is loaded
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = @"Fund Details";
            Master.PageSubTitle = "View Funds";
            Master.UseDynamicCSS = true;
            if (!Page.IsPostBack)
            {
                //if the company has not subscribed for expedite payment service, do not load display the page.
                var accountId = reqCurrentUser.AccountID;
                cAccount account = new cAccounts().GetAccountsWithPaymentServiceEnabled().Find(x => x.accountid == accountId);

                if (account == null)
                {
                    return;
                }

                var subAccounts = new cAccountSubAccounts(reqCurrentUser.AccountID);
                var properties = subAccounts.getSubAccountById(reqCurrentUser.CurrentSubAccountId).SubAccountProperties;
                var currencies = new cCurrencies(reqCurrentUser.AccountID, reqCurrentUser.CurrentSubAccountId);
                var globalCurrencies = new cGlobalCurrencies();

                if (properties.BaseCurrency.HasValue && properties.BaseCurrency.Value != 0)
                {
                    int baseCurrency = properties.BaseCurrency.Value;
                    var currencyList = currencies.CreateDropDown(baseCurrency);

                    foreach (var item in currencyList.Where(item => item.Selected))
                    {
                      var currencySymbol = globalCurrencies.getGlobalCurrencyByLabel(item.Text).symbol;
                      SetCurrencySymbolToCache(reqCurrentUser.AccountID, currencySymbol);
                      lblFundDetails.Text = @"Fund Details (All Amounts in " + currencySymbol + @")";
                      break;
                    }
                }

                if (!properties.AllowViewFundDetails || cMisc.IsDelegate || !reqCurrentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFunds, true))
                {
                    Response.Redirect("~/shared/restricted.aspx?reason=Current%20access%20role%20does%20not%20permit%20you%20to%20view%20this%20page.", true);
                }
                else
                {
                    GetAvailabelFunds();    
                }
                GenerateFundDetailsGrid();
            }
        }
        /// <summary>
        /// Sets currency symbol in cache
        /// </summary>
        /// <param name="accountId">accountId</param>
        /// <param name="symbol">currency symbol</param>
        private void SetCurrencySymbolToCache(int accountId,string symbol)
        {
            cache.Add(accountId, string.Empty, CacheKey, symbol);
        }

        /// <summary>
        /// Gets currency symbol from cache
        /// </summary>
        /// <param name="accountId">accountId</param>
        /// <returns>currency symbol</returns>
        private string GetCurrencySymbolFromCache(int accountId)
        {
            return cache.Get(accountId, string.Empty, CacheKey) as string;
        }


        /// <summary>
        /// Called when click on button cancel. It cancels all the operations.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">EVent arguments</param>
        protected void cmdCancel_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/adminmenu.aspx");
        }

        /// <summary>
        /// Creates transaction grid and shows available fund.
        /// </summary>
        private void GenerateFundDetailsGrid()
        {
            var cFunds = new FundDetails();
            TransactionType fundType = (TransactionType)Enum.Parse(typeof(TransactionType), ddlTranscationType.Text);
            string[] gridData = cFunds.FillGrid(txtTransactionStartDate.Text, txtTransactionEndDate.Text, (int)fundType);
            this.litFundDetails.Text = gridData[2];

            Page.ClientScript.RegisterStartupScript(this.GetType(), "FundDetailsVars",  cGridNew.generateJS_init("FundDetailsGridVars", new List<string> { gridData[1] }, reqCurrentUser.CurrentActiveModule), true);

            GetAvailabelFunds();
        }

        /// <summary>
        /// Get available fund for the account
        /// </summary>
        private void GetAvailabelFunds()
        {
            var cFunds = new FundDetails();
            var availabeFund = new StringBuilder(GetCurrencySymbolFromCache(reqCurrentUser.AccountID));
            availabeFund.Append(cFunds.GetAvailableFunds().ToString("###,###,##0.00"));
            lblAvailableFund.Text = availabeFund.ToString();
        }

        /// <summary>
        /// Called when search button clicked and filter records.
        /// </summary>
        /// <param name="sender">Sender of the event </param>
        /// <param name="e">Event arguments</param>
        protected void cmdSearch_OnClick(object sender, EventArgs e)
        {
            GenerateFundDetailsGrid();
        }
    }
}