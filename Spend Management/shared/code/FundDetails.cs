
namespace Spend_Management.shared.code
{
    using System;
    using System.Collections.Generic;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Expedite;

    /// <summary>
    /// FundDetails contains basic method for fund details 
    /// </summary>
    public class FundDetails
    {

        private CurrentUser currentUser;
        /// <summary>
        /// Constructor to assign current user in variable
        /// </summary>
        public FundDetails()
        {
            currentUser = cMisc.GetCurrentUser();
        }

        /// <summary>
        /// Returns fund details as html
        /// </summary>
        /// <param name="transactionStartDate">start date</param>
        /// <param name="transactionEndDate">end date</param>
        /// <param name="transactionType">transaction type</param>
        /// <returns>grid with fund details</returns>
        public string[] FillGrid(string transactionStartDate, string transactionEndDate, int transactionType)
        {
            var clsfields = new cFields(currentUser.AccountID);
            const string SqlQuery = "SELECT TransactionId,TransactionType,TransactionDate,Amount,AvailableFund FROM FundTransaction";

            var gridFunds = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridFunds", SqlQuery)
            {
                KeyField = "TransactionId",
                EmptyText = "No Results Found."
            };

            gridFunds.getColumnByName("TransactionId").hidden = true;
            ((cFieldColumn)gridFunds.getColumnByName("TransactionType")).addValueListItem((byte)1, "Credit");
            ((cFieldColumn)gridFunds.getColumnByName("TransactionType")).addValueListItem((byte)2, "Debit");
            gridFunds.enabledeleting = false;
            gridFunds.enableupdating = false;

            if (transactionType > 0 && transactionType != 0)
            {
                gridFunds.addFilter(clsfields.GetFieldByID(new Guid("47999C5A-6A6B-411F-A55B-8C306913FDC5")),
                    ConditionType.Equals, new object[] { transactionType }, null, ConditionJoiner.And);
            }
            
            if (!string.IsNullOrEmpty(transactionStartDate) && transactionEndDate != "")
            {
                var startDate = DateTime.Parse(transactionStartDate);
                var endDate = DateTime.Parse(transactionEndDate);

                gridFunds.addFilter(clsfields.GetFieldByID(new Guid("4D6E6A51-7D6A-48DE-8C51-A750EF496FE4")),
                    ConditionType.Between, new object[] {startDate.ToString("yyyy/MM/dd") + " 00:00:00"},
                    new object[] {endDate.ToString("yyyy/MM/dd") + " 23:59:59"}, ConditionJoiner.And);
            }

            else if (!string.IsNullOrEmpty(transactionStartDate))
            {
                var startDate = DateTime.Parse(transactionStartDate);

                gridFunds.addFilter(clsfields.GetFieldByID(new Guid("4D6E6A51-7D6A-48DE-8C51-A750EF496FE4")),
                    ConditionType.GreaterThanEqualTo,new object[] { startDate.ToString("yyyy/MM/dd") + " 00:00:00" },null, ConditionJoiner.And);
            }

            else if (!string.IsNullOrEmpty(transactionEndDate))
            {
                var endDate = DateTime.Parse(transactionEndDate);

                gridFunds.addFilter(clsfields.GetFieldByID(new Guid("4D6E6A51-7D6A-48DE-8C51-A750EF496FE4")),
                   ConditionType.LessThanEqualTo, new object[] { endDate.ToString("yyyy/MM/dd") + " 00:00:00" }, null, ConditionJoiner.And);
            }


            var retVals = new List<string> { gridFunds.GridID };
            retVals.AddRange(gridFunds.generateGrid());
            return retVals.ToArray();
        }

        /// <summary>
        /// Returns availabe fund
        /// </summary>
        /// <returns>Available fund</returns>
        public decimal GetAvailableFunds()
        {
            var funds = new Funds(currentUser.AccountID);
            var fundDetails = funds.GetFundAvailable(currentUser.AccountID);
            return fundDetails.AvailableFund;
        }
    }
}