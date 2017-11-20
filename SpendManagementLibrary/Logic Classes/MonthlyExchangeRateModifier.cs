namespace SpendManagementLibrary.Logic_Classes
{
    using System;
    using Helpers;

    /// <summary>
    /// Populate the Exchange Rate Date Ranges from the current Monthly Exchange Rate info when turning on <see cref="cAccountProperties.EnableAutoUpdateOfExchangeRates"/>
    /// </summary>

    internal class MonthlyExchangeRateModifier : ExchangeRateModifier
    {
        public MonthlyExchangeRateModifier(int accountId) : base(accountId)
        {
        }

        /// <summary>
        /// Populate the Exchange Rate Date Range info based on the current Monthy rate info.
        /// </summary>
        /// <param name="activatedDate">The date that the <see cref="cAccountProperties.EnableAutoUpdateOfExchangeRates"/> was previously activated (if any)</param>
        public override void PopulateRanges(DateTime? activatedDate)
        {
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                if (activatedDate.HasValue)
                {
                    expdata.AddWithValue("@date", activatedDate.Value);
                }
                else
                {
                    expdata.AddWithValue("@date", DBNull.Value);
                }

                expdata.ExecuteProc("dbo.PopulateMonthlyRanges");
            }
        }
    }
}