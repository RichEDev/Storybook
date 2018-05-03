namespace SpendManagementApi.Controllers.Expedite.V1
{
    using SpendManagementApi.Attributes;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses.Expedite;
    using SpendManagementApi.Models.Types.Expedite;
    using SpendManagementApi.Repositories.Expedite;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Description;
    using SM = Spend_Management.Expedite;
   
    /// <summary>
    /// Contains Expedite clients Payment process actions
    /// </summary>
    [RoutePrefix("Expedite/Payment")]
    [Version(1)]
    [InternalSelenityMethod, NoAuthorisationRequired, ApiExplorerSettings(IgnoreApi = true)]
    public class PaymentManagerV1Controller : BaseApiController<PaymentService>
    {
        /// <summary>
        /// Gets ALL of the available end points from the API.
        /// </summary>
        /// <returns>A list of available Links</returns>
        [HttpOptions]
        [Route("~/Options")]
        public List<Link> Options()
        {
            return this.Links("Options");
        }
        
        #region Controller Actions
       
        /// <summary>
        /// Gets the finance export  data for payment service enable customer
        /// </summary>
        /// <param name="id">The Id of expedite client.</param>
        /// <returns>Gets the finance export for the expedite client</returns>
        [HttpGet, Route("~/Expedite/Payment/GetFinanceExportForDownload/{id:int}")]      
        public PaymentServiceResponse GetFinanceExportForDownload(int id)
        {
            var response = this.InitialiseResponse<PaymentServiceResponse>();
            response.List = ((PaymentServiceRepository)this.Repository).GetFinanceExportForDownload(id);
            return response;
        }      

        /// <summary>
        ///Get a list of finance exports which physically paid for selected expedite customers.
        /// </summary>
        /// <param name="id">The Id of expedite client.</param>
        /// <returns>Gets the finance export for the expedite client</returns>
        [HttpGet, Route("~/Expedite/Payment/GetFinanceExportForMarkAsExecuted/{id:int}")]       
        public PaymentServiceResponse GetFinanceExportForMarkAsExecuted(int id)
        {
            var response = this.InitialiseResponse<PaymentServiceResponse>();
            response.List = ((PaymentServiceRepository)this.Repository).GetFinanceExportsForMarkAsExecuted(id);
            return response;
        }

        /// <summary>
        /// update a list finance exports which are donwloaded by expedite admin  as downloaded(status 0 -> 1)
        /// </summary>
        /// <param name="request">List of records to updated</param>
        /// <returns>Respose contain updated status</returns>
        [Route("~/Expedite/Payment/Download")]       
        public PaymentProcessStatusResponse MarkAsDownload(List<PaymentService> request)
        {
            var response = this.InitialiseResponse<PaymentProcessStatusResponse>();
            response.isProcessed = ((PaymentServiceRepository)this.Repository).MarkAsDownload(request);
            return response;
        }

        /// <summary>
        /// update a list finance exports which are marked as executed by expedite admin (status 1 -> 2)
        /// </summary>
        /// <param name="request">List of records to updated</param>
        /// <returns>Respose contain updated status</returns>
        [Route("~/Expedite/Payment/Executed")]
        public PaymentProcessStatusResponse MarkAsExecuted(List<PaymentService> request)
        {
            var response = this.InitialiseResponse<PaymentProcessStatusResponse>();
            response.isProcessed = ((PaymentServiceRepository)this.Repository).MarkAsExecuted(request);
            return response;
        }

        /// <summary>
        /// function meant to send email regarding the top up required to carry out pending reimbursements(for expedite).
        /// </summary>
        /// <param name="fundInfo">FundManager object which has accountId and FundTopUp properties set</param>
        /// <returns>Returns a response object which has the status flag and error message</returns>
        [Route("~/Expedite/Payment/SendMailForTopUp")]
        public EmailSenderResponse SendMailForTopUp([FromBody] FundManager fundInfo)
        {
            var response = this.InitialiseResponse<EmailSenderResponse>();
            var expediteMail = new SM.ExpediteEmail();
            try
            {
                response.isSendingSuccessful = expediteMail.SendTopUpEmailNotification(fundInfo.AccountId, fundInfo.FundTopup);
                
                if (response.isSendingSuccessful == false)
                {
                    response.errorMessage ="Error while sending email";
                }
            }
            catch (Exception ex)
            {              
                response.isSendingSuccessful = false;
                response.errorMessage = ex.InnerException.Message;
            }

            return response;
        }

        /// <summary>
        /// Extract report data for the financial export id for the expedite client
        /// This method calls the report service and generates report data and other financial export details like amount payable export type etc
        /// </summary>
        /// <param name="accountId">The accountId of expedite client.</param>
        /// <param name="financialExportId">The financialExportId of export</param>
        /// <returns>Returns a response object with payment details for the financial export</returns>
        [HttpGet, Route("~/Expedite/Payment/ExtractFinancialExportFromReportService/{id:int}/{financialExportId:int}")]
        public PaymentServiceResponse ExtractFinancialExportFromReportService(int id, int financialExportId)
        {
            var response = this.InitialiseResponse<PaymentServiceResponse>();
            response.List = ((PaymentServiceRepository)this.Repository).ExtractFinancialExportFromReportService(id, financialExportId);
            return response;
        }

        #endregion Controller Actions
    }
}