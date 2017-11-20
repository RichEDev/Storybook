namespace SpendManagementLibrary.Logic_Classes
{
    using System;
    using Helpers;

    /// <summary>
    /// Populate the Exchange Rate Date Ranges from the current date ranges when turning on <see cref="cAccountProperties.EnableAutoUpdateOfExchangeRates"/>
    /// </summary>

    internal class DateRangeExchangeRateModifier : ExchangeRateModifier
    {
        public DateRangeExchangeRateModifier(int accountId) : base(accountId)
        {
        }

        /// <summary>
        /// Populate the Exchange Rate Date Range info based on Current Date Range info.
        /// </summary>
        /// <param name="activatedDate">The date that the <see cref="cAccountProperties.EnableAutoUpdateOfExchangeRates"/> was previously activated (if any)</param>
        public override void PopulateRanges(DateTime? activatedDate)
        {
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                expdata.ExecuteProc("dbo.PopulateDateRanges");
            }
        }
    }
}