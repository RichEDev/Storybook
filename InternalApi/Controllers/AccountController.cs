namespace InternalApi.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;
    using BusinessLogic.Identity;

    using InternalApi.Models;

    using SpendManagementLibrary;

    using Spend_Management;

    [RoutePrefix("Account")]
    public class AccountController : ApiController
    {
        /// <summary>
        /// Gets the list of <see cref="GeneralOptionEnabledAccount"/> which have Approver reminders or claimant reminders enabled 
        /// </summary>
        /// <returns>A list of accounts with claim reminders enabled</returns>
        [HttpGet, Route("GetAccountsWithClaimRemindersEnabled")]
        public IHttpActionResult GetAccountsWithClaimRemindersEnabled()
        {
            List<GeneralOptionEnabledAccount> accountList = new List<GeneralOptionEnabledAccount>();

            var accounts = new cAccounts().GetAllAccounts().Where(a => !a.archived).ToList();

            if (accounts == null || accounts.Count == 0)
            {
                return this.Json(accountList);
            }

            foreach (var account in accounts)
            {
                this.RequestContext.Principal = new WebPrincipal(new UserIdentity(account.accountid, 0));

                var subAccounts = new cAccountSubAccounts(account.accountid);
                var reqSubAccount = subAccounts.getFirstSubAccount();
                var generalOptionsFactory = FunkyInjector.Container.GetInstance<IDataFactory<IGeneralOptions, int>>();
                var generalOptions = generalOptionsFactory[reqSubAccount.SubAccountID].WithReminders();
                if (generalOptions.Reminders.EnableClaimApprovalReminders || generalOptions.Reminders.EnableCurrentClaimsReminders)
                {
                    accountList.Add(new GeneralOptionEnabledAccount(account.accountid, account.companyid));
                }
            }

            return this.Json(accountList);
        }

        /// <summary>
        /// Get accounts with auto populate of exchange rates general option enabled 
        /// </summary>
        /// <returns> Account details</returns>
        [HttpGet, Route("GetAccountsWithExchangeRatesUpdateEnabled")]
        public IHttpActionResult GetAccountsWithExchangeRatesUpdateEnabled()
        {
            List<GeneralOptionEnabledAccount> accountList = new List<GeneralOptionEnabledAccount>();

            var accounts = new cAccounts().GetAllAccounts().Where(a => !a.archived).ToList();

            if (accounts == null || accounts.Count == 0)
            {
                return this.Json(accountList);
            }

            foreach (var account in accounts)
            {
                this.RequestContext.Principal = new WebPrincipal(new UserIdentity(account.accountid, 0));

                var subAccounts = new cAccountSubAccounts(account.accountid);

                var generalOptionsFactory = FunkyInjector.Container.GetInstance<IDataFactory<IGeneralOptions, int>>();

                var generalOptions = generalOptionsFactory[subAccounts.getFirstSubAccount().SubAccountID].WithCurrency();
                if (generalOptions.Currency.EnableAutoUpdateOfExchangeRates)
                {
                    accountList.Add(new GeneralOptionEnabledAccount(account.accountid, account.companyid, generalOptions.Currency.ExchangeRateProvider));
                }
            }

            return this.Json(accountList);
        }
    }
}