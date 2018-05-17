
namespace SpendManagementApi.Repositories.Expedite
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Interfaces;
    using Models.Types.Expedite;
    using SM = Spend_Management.Expedite;
    using SpendManagementLibrary.Interfaces.Expedite;
    using Spend_Management;
    using System;

    using Expenses_Reports;

    using SpendManagementLibrary;

    /// <summary>
    /// PaymentServiceRepository manages data access for PaymentServices.
    /// </summary>
    internal class PaymentServiceRepository : BaseRepository<PaymentService>, ISupportsActionContext
    {
        /// <summary>
        /// An instance of <see cref="IManagePayment"/>.
        /// </summary>
        private readonly IManagePayment _data;

        /// <summary>
        /// An instance of <see cref="IActionContext"/>.
        /// </summary>
        private readonly IActionContext _actionContext = null;

        /// <summary>
        /// Creates a new PaymentServiceRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public PaymentServiceRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.AccountId, x => x.AccountId.ToString(CultureInfo.InvariantCulture))
        {
            this._data = ActionContext.Payment;
            this._actionContext = ActionContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentServiceRepository"/> class.
        /// </summary>
        public PaymentServiceRepository()
        {
            this._data = new SpendManagementLibrary.Expedite.PaymentService();
        }

        /// <summary>
        /// Get all of T
        /// </summary>
        /// <returns></returns>
        public override IList<PaymentService> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get all of T
        /// </summary>
        /// <returns></returns>
        public override PaymentService Get(int id)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets all finance Exports which are ready for payment for selected payment service enabled customer
        /// </summary>
        /// <param name="accountId">The accountId of expedite client.</param>
        /// <returns>A list of payments with financial exports.</returns>
        public List<PaymentService> GetFinanceExportForDownload(int accountId)
        {
            int expeditePaymentStatus = 0;
            var item = this._data.GetFinanceExportForExpedite(accountId, expeditePaymentStatus).Select(e => new PaymentService().From(e, this._actionContext)).ToList();
            return item;
        }

        /// <summary>
        /// Gets all finance Exports which are physically paid and ready to mark as executed
        /// </summary>  
        /// <param name="accountId">The accountId of expedite client.</param>
        /// <returns>List of PaymentService data</returns>       
        public List<PaymentService> GetFinanceExportsForMarkAsExecuted(int accountId)
        {
            int expeditePaymentStatus = 1;
            var item = this._data.GetFinanceExportForExpedite(accountId, expeditePaymentStatus).Select(e => new PaymentService().From(e, this._actionContext)).ToList();
            return item;
        }

        /// <summary>
        /// MarkAsDownload update the Payment Downloaded status
        /// </summary>
        /// <param name="items">List of exports for which the status to be updated as executed</param>
        /// <returns>processed status</returns>
        public int MarkAsDownload(List<PaymentService> items)
        {
            int isProcessed = 0;
            List<KeyValuePair<int, int>> accountClaimIds = new List<KeyValuePair<int, int>>();
            List<int> claimIds = new List<int>();
            List<int> accountIds = new List<int>();
            List<int> distinctclaimIds = new List<int>();
            foreach (PaymentService paymentServiceAccount in items)
            {
                isProcessed = this._data.UpdateFinanceExportProcessDownloadStatus(paymentServiceAccount.FinancialExport.Select(i => i.To(this._actionContext)).ToList(), paymentServiceAccount.AccountId);
                bool idIsPresent = false;
                if (isProcessed == 1)
                {
                    foreach (FinancialExport financialExport in paymentServiceAccount.FinancialExport)
                    {
                        claimIds = this._data.GetClaimByFinancialExportIds(financialExport.Id, paymentServiceAccount.AccountId);
                    }

                    if (accountIds.IndexOf(paymentServiceAccount.AccountId) == -1)
                    {
                        accountIds.Add(paymentServiceAccount.AccountId);
                        foreach (int id in claimIds)
                        {
                            distinctclaimIds.Add(id);
                            accountClaimIds.Add(new KeyValuePair<int, int>(paymentServiceAccount.AccountId, id));
                        }
                    }
                    else
                    {
                        foreach (int id in claimIds)
                        {
                            foreach (int existingId in distinctclaimIds)
                            {
                                if (existingId == id)
                                {
                                    idIsPresent = true;
                                }
                            }
                            if (!idIsPresent)
                            {
                                distinctclaimIds.Add(id);
                                accountClaimIds.Add(new KeyValuePair<int, int>(paymentServiceAccount.AccountId, id));
                            }
                        }
                    }
                }
            }
            foreach (KeyValuePair<int,int> accountClaimId in accountClaimIds)
            {
               
                cClaims claims = new cClaims(accountClaimId.Key);
                SpendManagementLibrary.cClaim claim = claims.getClaimById(accountClaimId.Value);
                if (claim != null)
                {
                    var history = string.Format("Claim {0} is sent for Payment Process.", claim.claimno);
                    claims.UpdateClaimHistory(claim, history);
                }
            }
            return isProcessed;
        }

        /// <summary>
        /// MarkAsExecuted update the Payment Executed status
        /// </summary>
        /// <param name="items">List of exports for which the status to be updated as executed</param>
        /// <returns>processed status</returns>
        public int MarkAsExecuted(List<PaymentService> items)
        {
            int isProcessed = 0;
            List<int> claimIds = new List<int>();

            foreach (PaymentService paymentServiceAccount in items)
            {
                foreach (FinancialExport financialExport in paymentServiceAccount.FinancialExport)
                {
                    claimIds = this._data.GetClaimByFinancialExportIds(financialExport.Id, paymentServiceAccount.AccountId);
                }
                isProcessed =
                    this._data.UpdateFinanceExportProcessExecutedStatus(
                        paymentServiceAccount.FinancialExport.Select(i => i.To(this._actionContext)).ToList(),
                        paymentServiceAccount.AccountId, paymentServiceAccount.FundAmount);

                if (isProcessed == 1)
                {
                    var employees = new cEmployees(paymentServiceAccount.AccountId);               
                    var claims = new cClaims(paymentServiceAccount.AccountId);
                    var emails = new NotificationTemplates(paymentServiceAccount.AccountId);

                    foreach (int id in claimIds)
                    {
                        var claim = claims.getClaimById(id);

                        if (claim != null)
                        {
                            var employee = employees.GetEmployeeById(claim.employeeid);

                            // update claim history.
                            var history = string.Format("Claim {0} has reimbursed.", claim.claimno);
                            claims.UpdateClaimHistory(claim, history);

                            //Send an email to each claimant                                
                            notifications.SendMessage(
                                new Guid("67E99E3E-E431-4C9C-A131-5D647D64FF05"),
                                0,
                                new[] { employee.EmployeeID });
                        }
                    }
                }
            }
            return isProcessed;
        }

        /// <summary>
        /// Extract report data for the financial export id for the expedite client
        /// This method calls the report service and generates report data and other financial export details like amount payable export type etc
        /// </summary>
        /// <param name="accountId">The accountId of expedite client.</param>
        /// <param name="financialExportId">The financialExportId of export</param>
        /// <returns>A list of payments</returns>
        public List<PaymentService> ExtractFinancialExportFromReportService(int accountId, int financialExportId)
        {
            SM.PaymentService paymentService = new SM.PaymentService();
            IReports reports = new cReportsSvc();
            var item = paymentService.ExtractFinancialExportFromReportService(accountId, financialExportId, reports).Select(e => new PaymentService().From(e, this._actionContext)).ToList();
            return item;
        }
    }
}