namespace SpendManagementApi.Controllers.V1
{
    using Spend_Management;
    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Repositories;
    using SpendManagementLibrary;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    /// <summary>
    /// Contains End points for FuelReceiptToVATCalculation console
    /// </summary>
    [RoutePrefix("FuelReceiptToVATCalculation")]
    [Version(1)]
    public class FuelReceiptToVATCalculationV1Controller : BaseApiController
    {
        private readonly cAccounts _accounts;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FuelReceiptToVATCalculationV1Controller()
        {
            this._accounts = new cAccounts();
        }

        /// <summary>
        /// Gets a list of Ids of Accounts that FuelReceipt To VAT Calculation Enabled.
        /// </summary>
        /// <returns>GetAccountVatCalculationEnabledResponses</returns>
        [HttpGet, Route("GetAccountsWithFuelReceiptToVATCalculationEnabled")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.View)]
        public GetAccountVatCalculationEnabledResponses GetAccountsWithFuelReceiptToVATCalculationEnabled()
        {
            var response = this.InitialiseResponse<GetAccountVatCalculationEnabledResponses>();
            response = new FuelReceiptToVATCalculationRepository().GetAccountsWithFuelReceiptToVATCalculationEnabled();
            return response;
        }

        /// <summary>
        /// Gets a list of Ids of Accounts that have the Validation Service enabled.
        /// </summary>
        /// <returns>FuelReceiptToVATCalculationProcessResponse</returns>       
        [HttpPost,Route("Process")]
        [AuthAudit(SpendManagementElement.Api, AccessRoleType.Edit)]
        public FuelReceiptToVATCalculationProcessResponse ProcessFuelReceiptToMileageAllocations([FromBody]Account Account)
        {           
            var response = this.InitialiseResponse<FuelReceiptToVATCalculationProcessResponse>();
            response = new FuelReceiptToVATCalculationRepository().ProcessFuelReceiptToMileageAllocations(Account.AccountId);           
            return response;
        }

        /// <summary>
        ///  Override Init 
        /// </summary>
        protected override void Init() { }
    }
}