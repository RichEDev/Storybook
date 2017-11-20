
namespace SpendManagementApi.Controllers.V1
{
    using Newtonsoft.Json;
    using Spend_Management;
    using SpendManagementApi.Attributes;
    using SpendManagementApi.Common;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Models.Types.Employees;
    using SpendManagementApi.Repositories;
    using SpendManagementApi.Utilities;
    using SpendManagementLibrary.Random;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Http;
    using System.Web.Http.Description;

    /// <summary>
    /// Manages operations on <see cref="CorporateCard">CorporateCards</see>
    /// </summary>
    [RoutePrefix("CorporateCards")]
    [Version(1)]
    public class CorporateCardsV1Controller : BaseApiController<CorporateCard>
    {
        #region Api Methods

        /// <summary>Gets all of the available end points from the <see cref="CorporateCard">CorporateCards</see> part of the API.</summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return base.Links();
        }

        /// <summary>Gets all <see cref="CorporateCard">CorporateCards</see> in the system.</summary>
        /// <returns>A GetCorporateCardsResponse containing a list of all Cards.</returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.View)]
        public GetCorporateCardsResponse GetAll()
        {
            return this.GetAll<GetCorporateCardsResponse>();
        }

        /// <summary>Gets a single <see cref="CorporateCard">CorporateCard</see>, by its Id.</summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>A CorporateCardResponse, containing the <see cref="CorporateCard">CorporateCard</see> if found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.View)]
        public CorporateCardResponse Get([FromUri] int id)
        {
            return this.Get<CorporateCardResponse>(id);
        }

        /// <summary>
        /// Gets all the <see cref="CorporateCard">Corporate Cards</see> for the supplied <see cref="Employee">Employee</see>.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Employee">Employee</see>.</param>
        /// <returns>A GetCorporateCardsResponse containing all <see cref="CorporateCard">CorporateCard</see> for the Employee.</returns>
        [HttpGet, Route("ByEmployee/{id:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.View)]
        public GetCorporateCardsResponse ByEmployee(int id)
        {
            var response = this.InitialiseResponse<GetCorporateCardsResponse>();
            response.List = ((CorporateCardRepository)this.Repository).GetAllCardsByEmployee(id);
            return response;
        }

        /// <summary>
        /// Finds all <see cref="CorporateCard">CorporateCard</see> matching specified criteria.<br/>
        /// Currently available querystring parameters: <br/>
        /// EmployeeId<br/>
        /// CardProviderId<br/>
        /// IsActive<br/>
        /// Use SearchOperator=0 to specify an AND query or SearchOperator=1 for an OR query
        /// </summary>
        /// <param name="criteria">Find query</param>
        /// <returns>A GetCorporateCardsRolesResponse containing matching <see cref="CorporateCard">CorporateCards</see>.</returns>
        [HttpGet, Route("Find")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.View)]
        public GetCorporateCardsResponse Find([FromUri] FindCorporateCardsRequest criteria)
        {
            var response = this.InitialiseResponse<GetCorporateCardsResponse>();
            var conditions = new List<Expression<Func<CorporateCard, bool>>>();

            if (criteria == null)
            {
                throw new ArgumentException(ApiResources.ApiErrorMissingCritera);
            }

            if (criteria.EmployeeId != null)
            {
                conditions.Add(b => b.EmployeeId == criteria.EmployeeId);
            }

            if (criteria.CardProviderId != null)
            {
                conditions.Add(b => b.CardProviderId == criteria.CardProviderId);
            }

            if (criteria.IsActive != null)
            {
                conditions.Add(b => b.IsActive == criteria.IsActive);
            }

            response.List = this.RunFindQuery(this.Repository.GetAll().AsQueryable(), criteria, conditions);
            return response;
        }

        /// <summary>
        /// Adds a <see cref="CorporateCard">CorporateCard</see>.
        /// </summary>
        /// <param name="request">The <see cref="CorporateCard">CorporateCard</see> to add.
        /// When adding a new <see cref="CorporateCard">CorporateCard</see> through the API, the following properties are required: 
        /// Id: Must be set to 0, or the add will throw an error.
        /// Label: Must be set to something meaningful, or the add will throw an error.
        /// EmployeeId: The Id of the Employee to link as the budget holder.
        /// </param>
        /// <returns>A CorporateCardResponse.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Add)]
        public CorporateCardResponse Post([FromBody] CorporateCard request)
        {
            return this.Post<CorporateCardResponse>(request);
        }

        /// <summary>
        /// Edits a <see cref="CorporateCard">CorporateCard</see>.
        /// </summary>
        /// <param name="id">The Id of the Item to edit.</param>
        /// <param name="request">The Item to edit.</param>
        /// <returns>The edited Budget Holder</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public CorporateCardResponse Put([FromUri] int id, [FromBody] CorporateCard request)
        {
            request.Id = id;
            return this.Put<CorporateCardResponse>(request);
        }

        /// <summary>
        /// Deletes a <see cref="CorporateCard">CorporateCard</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="CorporateCard">CorporateCard</see> to be deleted.</param>
        /// <returns>A CorporateCardResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Delete)]
        public CorporateCardResponse Delete(int id)
        {
            return this.Delete<CorporateCardResponse>(id);
        }

        /// <summary>
        /// Unmatch a transaction from expense item.
        /// </summary>
        /// <param name="expenseId">Expense id</param>
        /// <returns>Numeric Response</returns>
        [HttpPut, Route("UnmatchTransaction")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public NumericResponse UnmatchTransaction([FromUri] int expenseId)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((CorporateCardRepository) Repository).UnmatchTransaction(expenseId);
            return response;
        }

        /// <summary>
        /// Match a transaction to an expense item
        /// </summary>
        /// <param name="transactionId">Transaction id</param>
        /// <param name="expenseId">Expense id</param>
        /// <returns>Numeric Response</returns>
        [HttpPut, Route("MatchTransaction")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public NumericResponse MatchTransaction([FromUri] int transactionId, [FromUri] int expenseId)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((CorporateCardRepository)Repository).MatchTransaction(transactionId, expenseId, CheckIfItsMobileRequest());
            return response;
        }

        /// <summary>
        /// Gets all the <see cref="CardStatement"/>CardStatements for the current user
        /// </summary>
        /// <returns>A CardStatementsResponse containing all <see cref="CardStatement">CardStatement</see> for the current user.</returns>
        [HttpGet, Route("CardStatementsForCurrentUser")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public CardStatementsResponse CardStatementsForCurrentUser()
        {
            var response = this.InitialiseResponse<CardStatementsResponse>();
            response.List = ((CorporateCardRepository)this.Repository).GetCardStatementsForCurrentUser();
            return response;
        }

        /// <summary>
        /// Gets additional transaction information for supplied transaction id.
        /// </summary>
        /// <param name="transactionId">The transaction id.</param>
        /// <returns>A HTML string containg all additional information</returns>
        [HttpGet, Route("AdditionalTransactionInfo/{transactionId:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public CorporateCardTransactionDetailsResponse AdditionalTransactionInfo(int transactionId)
        {
            var response = this.InitialiseResponse< CorporateCardTransactionDetailsResponse> ();
            response.TransactionDetails = ((CorporateCardRepository)this.Repository).GetAdditionalTransactionInfo(transactionId);
            return response;
        }

        /// <summary>
        /// Gets additional transaction information for the supplied transaction Id as an approver.
        /// </summary>
        /// <param name="transactionId">
        /// The transaction id.
        /// </param>
        /// <param name="expenseId">
        /// The expense id, used to check the current user is in the expense item's claim hierarchy.
        /// </param>
        /// <returns>The <see cref="CorporateCardTransactionDetailsResponse">CorporateCardTransactionDetailsResponse</see></returns>
        [HttpGet, Route("AdditionalTransactionInfoAsApprover/{transactionId:int}/{expenseId:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public CorporateCardTransactionDetailsResponse AdditionalTransactionInfoAsApprover(int transactionId, int expenseId)
        {
            var response = this.InitialiseResponse<CorporateCardTransactionDetailsResponse>();
            response.TransactionDetails = ((CorporateCardRepository)this.Repository).GetAdditionalTransactionInfoAsApprover(transactionId, expenseId);
            return response;
        }

        /// <summary>
        /// Gets the transactions for the supplied statementId id.
        /// </summary>
        /// <param name="statementId">The statementId id.</param>
        /// <returns>The <see cref="CreditCardTransactions">CreditCardTransactions</see> response</returns>
        [HttpGet, Route("CreditCardTransactions")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public CreditCardTransactions GetTransactionsForStatementId([FromUri] int statementId)
        {
            var response = this.InitialiseResponse<CreditCardTransactions>();
            response.List = ((CorporateCardRepository)this.Repository).GetTransactionsForStatementId(statementId);
            return response;
        }

        /// <summary>
        /// Allocates a corporate card to an employee.
        /// </summary>
        /// <param name="request">
        /// The <see cref="AllocateCardToEmployeeRequest">AllocateCardToEmployeeRequest</see>
        /// </param>
        /// <returns>
        /// A <see cref="NumericResponse">NumericResponse</see> 1 if card allocation successful, else 0
        /// </returns>
        [HttpPut, Route("AllocateCardToEmployee")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public NumericResponse AllocateCardToEmployee(AllocateCardToEmployeeRequest request)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((CorporateCardRepository)this.Repository).AllocateCardToEmployee(request.StatementId, request.EmployeeId, request.CardNumber);
            return response;
        }


        /// <summary>
        /// Imports credit card transaction data.
        /// </summary>
        /// <param name="request">
        /// The <see cref="ImportTransactionRequest">ImportTransactionRequest</see> which contains details of the transactions to import.
        /// </param>
        /// <returns>
        /// The <see cref="NumericResponse">NumericResponse </see> of the statment Id the imported transactions belong to.
        /// </returns>
        [HttpPost, Route("ImportTransactions")]
        [AuthAudit(SpendManagementElement.CorporateCards, AccessRoleType.View)]
        public NumericResponse ImportTransactions(ImportTransactionRequest request)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((CorporateCardRepository)this.Repository).ImportCreditCardTransactions(request.CardProviderName, request.TransactionData, request.StatementDate);
            return response;
        }


        /// <summary>
        /// Deletes a statement by its Id.
        /// </summary>
        /// <param name="statementId">
        /// The statement Id.
        /// </param>
        /// <returns>
        /// The see <see cref="NumericResponse">NumericResponse</see> of the action, whereby success is 1.
        /// </returns>
        [HttpDelete, Route("DeleteStatementById")]
        [AuthAudit(SpendManagementElement.CorporateCards, AccessRoleType.View)]
        public NumericResponse DeleteStatementById([FromUri] int statementId)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((CorporateCardRepository)this.Repository).DeleteStatementById(statementId);
            return response;
        }


        /// <summary>
        /// Import a card file automatically.
        /// The File content should be Base 64 encoded and added to the request body. </summary>
        /// <param name="providerName">The provider name to use to imoprt the file</param>
        /// <returns>an instance of <see cref="NumericResponse"></see> where the value =;
        /// Positive integer = sucess
        /// 0 = Failed to validate the file
        /// -1= Failed to find the customer ID
        /// -100 = Customer ID is not unique</returns>
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost, Route("AutoImportCardFile/{providerName}")]
        public NumericResponse CorporateCardAutoImport(string providerName )
        {
            var response = this.InitialiseResponse<NumericResponse>();
            using (var item = this.Request.Content.ReadAsStringAsync())
            {
                var result = JsonConvert.DeserializeObject<FileDataRequest>(item.Result);
                var corporateCards = new cCardStatements();

                response.Item = corporateCards.AutoImportFile(result, providerName);
            }

            return response;
        }

        /// <summary>
        /// Determines if the request was from a mobile device.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CheckIfItsMobileRequest()
        {
            return Helper.IsMobileRequest(this.Request.Headers.UserAgent.ToString());
        }

        #endregion Api Methods
    }
}
