namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;
    using SpendManagementApi.Models;

    /// <summary>
    /// Manages operations on <see cref="BankAccount">BankAccount</see>.
    /// </summary>
    [RoutePrefix("BankAccounts")]
    [Version(1)]
    public class BankAccountsV1Controller : BaseApiController<BankAccount>
    {

        /// <summary>
        /// Gets all of the available end points from the <see cref="BankAccount">BankAccount</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions, Route("")]
        public List<Link> Options()
        {
            return Links();
        }

        /// <summary>
        /// Saves a <see cref="BankAccount">BankAccount</see>. 
        /// </summary>
        /// <param name="request">The<see cref="BankAccount">BankAccount</see>details to be saved</param>
        /// <returns>An <see cref="BankAccountResponse">BankAccountResponse</see> containing the newly saved <see cref="BankAccount">BankAccount</see></returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public BankAccountResponse Post([FromBody] BankAccount request)
        {
            return this.Post<BankAccountResponse>(request);
        }

        /// <summary>
        /// Gets a list of <see cref="BankAccount">BankAccount</see> for current user.
        /// </summary>
        /// <returns>
        /// The <see cref="BankAccountsResponse">BankAccountsResponse</see>
        /// </returns>
        [HttpGet, Route("GetBankAccountsForCurrentUser")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public BankAccountsResponse GetBankAccountsForCurrentUser()
        {
            var response = this.InitialiseResponse<BankAccountsResponse>();
            response.List = ((BankAccountRepository)this.Repository).GetBankAccountsForCurrentUser();
            return response;
        }

        /// <summary>
        /// Gets the <see cref="BankAccountMasterData">BankAccountMasterData</see> for current user. 
        /// Used to help build up the Add/Edit bank account form. 
        /// </summary>
        /// <returns>
        /// The <see cref="BankAccountMasterDataResponse">BankAccountMasterDataResponse</see>
        /// </returns>
        [HttpGet, Route("GetBankAccountMasterData")]
        [AuthAudit(SpendManagementElement.BankAccounts, AccessRoleType.View)]
        public BankAccountMasterDataResponse GetBankAccountMasterData()
        {          
            var response = this.InitialiseResponse<BankAccountMasterDataResponse>();
            response.Item = ((BankAccountRepository)this.Repository).GetBankAccountMasterData();
            return response;
        }

        /// <summary>
        /// Deletes a <see cref="BankAccount">BankAccount</see> by its Id 
        /// </summary>
        /// <param name="id">The Id of the <see cref="BankAccount">BankAccount</see> to delete</param>
        /// The <see cref="StringResponse">BankAccountMasterDataResponse</see> with the outcome of the action
        [HttpDelete, Route("DeleteBankAccount")]
        [AuthAudit(SpendManagementElement.BankAccounts, AccessRoleType.View)]
        public StringResponse DeleteBankAccount([FromUri] int id)
        {
            var response = this.InitialiseResponse<StringResponse>();
            response.Value = ((BankAccountRepository)this.Repository).DeleteBankAccount(id);
            return response;
        }

        /// <summary>
        /// Changes the status of a <see cref="BankAccount">BankAccount</see> to archived, or un-archived
        /// </summary>
        /// <param name="id">The Id of the <see cref="BankAccount">BankAccount</see> to update the status of</param>
        /// <param name="archive">Whether the <see cref="BankAccount">BankAccount</see> should be archived or not </param>
        /// The <see cref="StringResponse">BankAccountMasterDataResponse</see> with the outcome of the action
        [HttpPut, Route("ChangeBankAccountStatus")]
        [AuthAudit(SpendManagementElement.BankAccounts, AccessRoleType.View)]
        public StringResponse ChangeBankAccountStatus([FromUri] int id, [FromUri] bool archive)
        {
            var response = this.InitialiseResponse<StringResponse>();
            response.Value = ((BankAccountRepository)this.Repository).ChangeBankAccountStatus(id, archive);
            return response;
        }
    }
}
