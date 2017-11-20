namespace SpendManagementLibrary.Interfaces.Expedite
{
    using System.Collections.Generic;
    using SpendManagementLibrary.Expedite;
    using Enumerators.Expedite;
    /// <summary>
    /// The Interface for concrete implementaions of Expedite Payment Services .
    /// </summary>
    public interface IManagePayment
    {
        /// <summary>
        /// Get Financial Exports for download / mark as executed based on expeditePaymentStatus parameter
        /// <param name="accountId">expedite Client Account id</param>
        /// <param name="expeditePaymentStatus">Expedite Payment Status : Download :0 ,Execute:1</param>
        /// <returns>List of payment includes </returns>
        /// <summary>     
        List<Payment> GetFinanceExportForExpedite(int accountId, int expeditePaymentStatus);
        /// <summary>
        /// Update ExpeditePaymentProcessStatus as Executed for the list of financial exports        
        /// <param name="financialExport">List of executed financila exports</param>
        /// <param name="accountId">expedite Client Account id</param>
        /// <param name="fundAmount">Fund Amount available for the customer</param>
        /// <returns>Processed status</returns>
        ///  </summary>
        ///  
        int UpdateFinanceExportProcessExecutedStatus(List<FinancialExport> financialExport, int accountId, decimal fundAmount);
        /// <summary>
        /// Update ExpeditePaymentProcessStatus as Downloaded for the list of financial exports      
        /// <param name="financialExport">List of executed financila exports</param>
        /// <param name="accountId">expedite Client Account id</param>
        /// <returns>Processed status</returns>
        /// </summary>    
        int UpdateFinanceExportProcessDownloadStatus(List<FinancialExport> financialExport, int accountId);
        /// <summary>
        /// Get Claim uner each financila exports . This is used for update claim history 
        /// </summary>
        /// <param name="financialExportId"></param>
        /// <param name="accountId"></param>
        /// <returns>List of claim ids</returns>
        /// </summary>
        List<int> GetClaimByFinancialExportIds(int financialExportId,int accountId);
    }
}
