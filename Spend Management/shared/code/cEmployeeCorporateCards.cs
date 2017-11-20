namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using SpendManagementLibrary;
    using System.Web.Caching;

    public class cEmployeeCorporateCards : cEmployeeCorporateCardsBase
    {
        Cache cache = (Cache)System.Web.HttpRuntime.Cache;

        /// <summary>
        /// Constructor for SpendManagementLibrary
        /// </summary>
        /// <param name="accountID">Account ID</param>
        /// <param name="connectionString">Connection string from cAccounts</param>
        public cEmployeeCorporateCards(int accountID)
        {
            nAccountID = accountID;
            sConnectionString = cAccounts.getConnectionString(accountID);
            clsFields = new cFields(accountID);
            clsCardProviders = new CardProviders();
            //clsCurrentUser = cMisc.GetCurrentUser();

            InitialiseData();
        }

        private void InitialiseData()
        {
            lstCorporateCards = (SortedList<int, SortedList<int, cEmployeeCorporateCard>>)cache[CacheKey];
            if (lstCorporateCards == null)
            {
                CacheList();
            }
        }

        private void CacheList()
        {
            lstCorporateCards = GetCollection();

            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                DBConnection data = new DBConnection(sConnectionString);

                if (lstCorporateCards != null)
                {
                    SqlCacheDependency cardDep =
                        data.CreateSQLCacheDependency(
                            sSQL + " WHERE " + nAccountID.ToString() + " = " + nAccountID.ToString(),
                            new SortedList<string, object>());
                    cache.Insert(CacheKey, lstCorporateCards, cardDep, Cache.NoAbsoluteExpiration,
                        TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Medium), CacheItemPriority.Default, null);
                }
            }
        }

        private void ResetCache()
        {
            lstCorporateCards = null;
            cache.Remove(CacheKey);
            CacheList();
        }

        private string CacheKey
        {
            get { return "employeeCorporateCards_" + nAccountID.ToString(); }
        }

        /// <summary>
        /// Saves a corporate card for an employee
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public int SaveCorporateCard(cEmployeeCorporateCard card)
        {
            CurrentUser clsCurrentUser = cMisc.GetCurrentUser();

            int retCode = base.SaveCorporateCardBase(clsCurrentUser, card);
            ResetCache();

            return retCode;
        }

        /// <summary>
        /// Delete a corporate card association from an employee
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        public int DeleteCorporateCard(int cardID)
        {
            CurrentUser clsCurrentUser = cMisc.GetCurrentUser();

            int retCode = base.DeleteCorporateCardBase(clsCurrentUser, cardID);
            ResetCache();

            return retCode;
        }


        /// <summary>
        /// Gets all card statements for Employee
        /// </summary>
        /// <param name="employeeId">Id of the employee </param>
        /// <returns>List of <see cref="CardStatement"/>CardStatement</returns>
        public List<CardStatement> GetCardStatementsByEmployee(int employeeId)
        {
            return this.GetCardStatementsByEmployeeBase(employeeId);
        }

        /// <summary>
        /// Gets additional transaction information for given Transaction Id
        /// </summary>
        /// <param name="transactionId">Transaction Id of the statement</param>
        /// <returns>A HTML string containg all additional information</returns>
        public string GetAdditionalTransactionInfo(int transactionId)
        {
            cCardStatements templates = new cCardStatements(this.nAccountID);
            return templates.getTransactionDetails(transactionId);
        }
    }
}