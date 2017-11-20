
namespace SpendManagementLibrary.Expedite
{
    using System.Collections.Generic;
    /// <summary>
    /// FinanceExport for customers
    /// </summary>
    public class FinancialExport
    {
        /// <summary>
        ///Initialises a new instance of the <see cref="FinanceExport"/> class
        /// </summary>       
        public FinancialExport()
        {
           
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="FinancialExport"/> class accepting the id,reportName and expeditePaymentStatus
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reportName"></param>
        /// <param name="expeditePaymentStatus"></param>
        /// <param name="exportHistoryId"></param>
        public FinancialExport(int id, string reportName, int expeditePaymentStatus,decimal amount, int exportHistoryId=0)
        {
            this.Id = id;
            this.ReportName = reportName;
            this.ExpeditePaymentStatus = expeditePaymentStatus;
            this.Amount = amount;
            this.ExportHistoryId = exportHistoryId;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="FinancialExport"/> class accepting the id,reportName , expeditePaymentStatus ,reportData
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reportName"></param>
        /// <param name="expeditePaymentStatus"></param>
        /// <param name="reportData"></param>
        /// <param name="exportHistoryId"></param>
        public FinancialExport(int id, string reportName, int expeditePaymentStatus, byte[] reportData, decimal amount, int exportHistoryId = 0)
        {
            this.Id = id;
            this.ReportName = reportName;
            this.ExpeditePaymentStatus = expeditePaymentStatus;
            this.ReportData = reportData;
            this.Amount = amount;
            this.ExportHistoryId = exportHistoryId;
        }
        /// <summary>
        /// The unqiue primary key of this FinanceExport.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The ReportName of the finance Export that this        
        /// </summary>
        public string ReportName { get; set; }
        /// <summary>
        /// The Expedite Payment Status     
        /// </summary>
        public int? ExpeditePaymentStatus { get; set; }
        /// <summary>
        /// The Report Data     
        /// </summary>
        public byte[] ReportData { get; set; }
        /// <summary>
        /// Total amount that a financial Export file contain
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Export Type of financial Export file
        /// </summary>
        public ExportType ExportType { get; set; }
        
        /// <summary>
        /// Export History ID
        /// </summary>
        public int ExportHistoryId { get; set; }

    }
}
