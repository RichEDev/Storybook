
namespace SpendManagementApi.Repositories
{
    using Spend_Management;
    using SpendManagementApi.Models.Responses;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Mileage;
    using System.Collections.Generic;

    /// <summary>
    /// FuelReceiptToVATCalculationRepository manages data access for FuelReceiptToVAT Calculation.
    /// </summary>
    public class FuelReceiptToVATCalculationRepository
    {        
        /// <summary>
        /// Method process for FuelReceipt To Mileage Allocation calculation
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public FuelReceiptToVATCalculationProcessResponse ProcessFuelReceiptToMileageAllocations(int accountId)
        {
            FuelReceiptToVATCalculationProcessResponse vatCalculationResponse = new FuelReceiptToVATCalculationProcessResponse();
            FuelReceiptToVATCalculations vatCalculation = new FuelReceiptToVATCalculations();
            vatCalculationResponse.isProcessed = vatCalculation.ProcessFuelReceiptToMileageAllocations(accountId);
            CurrentUser user = cMisc.GetCurrentUser(); 
            string errorMessage = vatCalculationResponse.isProcessed == 0 ? "Automated schedule to allocate fuel receipt VAT  failed" : "Automated schedule to allocate fuel receipt VAT completed";
            vatCalculation.AddAuditLog(accountId, user.EmployeeID, SpendManagementElement.FuelReceiptToVATCalculation, errorMessage, user.CurrentSubAccountId, cAccounts.getConnectionString(accountId));
            return vatCalculationResponse;            
        }

        /// <summary>
        /// GetAccountsWithFuelReceiptToVATCalculationEnabled method get all the accounts with FuelReceiptToVATCalculation Enabled 
        /// </summary>
        /// <returns>GetAccountVatCalculationEnabledResponses</returns>
        public GetAccountVatCalculationEnabledResponses GetAccountsWithFuelReceiptToVATCalculationEnabled()
        {
            List<int> accountIds= new List<int>();
            GetAccountVatCalculationEnabledResponses accountList = new GetAccountVatCalculationEnabledResponses();
            var accounts = new cAccounts().GetAllAccounts();
            if (accounts != null && accounts.Count != 0)
            {
                foreach (var account in accounts)
                {
                    cAccountSubAccounts subAccounts = new cAccountSubAccounts(account.accountid);
                    cAccountSubAccount reqSubAccount = subAccounts.getFirstSubAccount();
                    if (reqSubAccount.SubAccountProperties.EnableCalculationsForAllocatingFuelReceiptVatToMileage)
                    {
                        accountIds.Add(account.accountid);
                    }
                }
            }
            accountList.List = accountIds;
            return accountList;
        }

        
    }
}