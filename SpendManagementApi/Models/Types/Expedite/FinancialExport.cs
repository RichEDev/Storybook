
namespace SpendManagementApi.Models.Types.Expedite
{
    using Interfaces;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Expedite;
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// This FinancialExport Class represent reports which are accessed from the Expedite 
    /// </summary>
    public class FinancialExport : IApiFrontForDbObject<SpendManagementLibrary.Expedite.FinancialExport, FinancialExport>
    {
        /// <summary>
        /// The Unique Id for FinanceExport
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
        public List<byte> ReportData { get; set; }
        /// <summary>
        /// Total expenses amount contain in finance export
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Export Type of financial Export file
        /// </summary>
        public string ExportType { get; set; }
        
        /// <summary>
        /// Id of the Export History 
        /// </summary>
        public int ExportHistoryId { get; set; }

        public FinancialExport From(SpendManagementLibrary.Expedite.FinancialExport dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }
            Id = dbType.Id;
            ReportName = dbType.ReportName;
           
            if (dbType.ReportData != null)
            {
                ReportData = new List<byte>(dbType.ReportData);
            }
            else
            {
                ReportData = null;
            }
            ExpeditePaymentStatus = dbType.ExpeditePaymentStatus;
            Amount = dbType.Amount;
            ExportType = ((ExportType)dbType.ExportType).ToString();
            ExportHistoryId = dbType.ExportHistoryId;
            return this;
        }
        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Expedite.FinancialExport To(IActionContext actionContext)
        {
            byte[] reportData = null;
            
            if (ReportData != null)
            {
                reportData = ReportData.ToArray();
            }
            return new SpendManagementLibrary.Expedite.FinancialExport {Id=Id, ReportName = ReportName, ReportData = reportData, ExpeditePaymentStatus = ExpeditePaymentStatus, Amount = Amount, ExportType  = (ExportType)Enum.Parse(typeof(ExportType), ExportType), ExportHistoryId = this.ExportHistoryId };
        }
    }
}
