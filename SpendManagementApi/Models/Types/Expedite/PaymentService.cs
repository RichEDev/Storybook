
namespace SpendManagementApi.Models.Types.Expedite
{
    using Interfaces;
    using SpendManagementLibrary.Expedite;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents payments which are ready for the Physical payment .
    /// This include finance exports list for payment service enabled customers.
    /// </summary>
    public class PaymentService : BaseExternalType, IApiFrontForDbObject<Payment, PaymentService>
    {
        /// <summary>
        /// The AccountId of the account  this Payment belongs to
        /// </summary>
        public int AccountId { get; set; }
        /// <summary>
        /// The list of Finance Export for AccountId
        /// </summary>
        public IList<FinancialExport> FinancialExport { get; set; }
        /// <summary>
        /// The account has Payment Service enabled
        /// </summary>
        public bool PaymentServiceEnabled { get; set; }
        /// <summary>
        /// Fund Amount available for the account
        /// </summary>
        public decimal FundAmount { get; set; }
        /// <summary>
        /// Converts to a API type from a base class of Payment
        /// </summary>
        /// <param name="dbType">An Payment.</param>
        /// <param name="actionContext">The actionContext which contains the DAL classes.</param>
        /// <returns></returns>
        public PaymentService From(Payment dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }
            AccountId = dbType.AccountId;
            FinancialExport = dbType.FinancialExport.Select(item => new FinancialExport().From(item, actionContext))
                            .ToList();
            PaymentServiceEnabled = dbType.PaymentServiceEnabled;
            FundAmount = dbType.FundAmount;
            return this;
        }
        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <returns>A data access layer Type</returns>
        public Payment To(IActionContext actionContext)
        {

            var financeExport = new List<SpendManagementLibrary.Expedite.FinancialExport>();
            foreach (var items in FinancialExport)
            {
                financeExport.Add(items.To(actionContext));
            }
            return new Payment
            {
               AccountId = AccountId,
               FinancialExport = financeExport,
               PaymentServiceEnabled = PaymentServiceEnabled,
               FundAmount=FundAmount
            };
        }
    }
}
