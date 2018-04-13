namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Models;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;

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
        /// Gets all the <see cref="BankAccount">BankAccount</see>s in the system.
        /// </summary>
        /// <returns>A <see cref="BankAccountsResponse">BankAccountsResponse</see> containing a list of <see cref="BankAccount">BankAccounts</see></returns>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.EmployeeBankAccounts, AccessRoleType.View)]
        public BankAccountsResponse GetAll()
        {
            return this.GetAll<BankAccountsResponse>();
        }

        /// <summary>
        /// Gets the <see cref="BankAccount">BankAccount</see> matching the specified id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="BankAccount">BankAccount</see> to get.</param>
        /// <returns>A <see cref="BankAccountResponse">BankAccountResponse</see> containing the matching <see cref="BankAccount">BankAccount</see>.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.View)]
        public BankAccountResponse Get([FromUri] int id)
        {
            return this.Get<BankAccountResponse>(id);
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
        /// Updates a <see cref="BankAccount">BankAccount</see>. 
        /// </summary>
        /// <param name="request">The <see cref="BankAccount">BankAccount</see>details to be updated</param>
        /// <returns>A <see cref="BankAccountResponse">BankAccountResponse</see> containing the updated <see cref="BankAccount">BankAccount</see></returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public BankAccountResponse Put([FromBody] BankAccount request)
        {
            return this.Put<BankAccountResponse>(request);
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
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet, Route("GetBankAccountMasterData")]
        [AuthAudit(SpendManagementElement.BankAccounts, AccessRoleType.View)]
        public BankAccountMasterDataResponse GetBankAccountMasterData()
        {          
            var response = this.InitialiseResponse<BankAccountMasterDataResponse>();
            response.Item = ((BankAccountRepository)this.Repository).GetBankAccountMasterData();
            return response;
        }

        /// <summary>
        /// Deletes the current user's <see cref="BankAccount">BankAccount</see> by its Id 
        /// </summary>
        /// <param name="id">
        /// The Id of the <see cref="BankAccount">BankAccount</see> to delete
        /// </param>
        /// The <see cref="StringResponse">BankAccountMasterDataResponse</see> with the outcome of the action
        /// <returns>
        /// The <see cref="StringResponse">StringResponse</see> with the outcome of the delete action
        /// </returns>
        [HttpDelete, Route("DeleteBankAccount")]
        [AuthAudit(SpendManagementElement.BankAccounts, AccessRoleType.View)]
        public StringResponse DeleteBankAccount([FromUri] int id)
        {
            var response = this.InitialiseResponse<StringResponse>();
            response.Value = ((BankAccountRepository)this.Repository).DeleteBankAccount(id);
            return response;
        }

        /// <summary>
        /// Deletes a <see cref="BankAccount">BankAccount</see>.
        /// </summary>
        /// <param name="id"><see cref="BankAccount">BankAccount</see> Id</param>
        /// <returns>The <see cref="StringResponse">StringResponse</see> with the outcome of the action</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.EmployeeBankAccounts, AccessRoleType.Delete)]
        public StringResponse Delete(int id)
        {
            var response = this.InitialiseResponse<StringResponse>();
            response.Value = ((BankAccountRepository)this.Repository).Delete(id);
            return response;
        }

        /// <summary>
        /// Archives or un-archives a <see cref="BankAccount">BankAccount</see> as the current user
        /// </summary>
        /// <param name="id">
        /// The Id of the <see cref="BankAccount">BankAccount</see> to update the status of
        /// </param>
        /// <param name="archive">
        /// Whether the <see cref="BankAccount">BankAccount</see> should be archived or not 
        /// </param>
        /// <returns>
        /// The <see cref="StringResponse">StringResponse</see> with the outcome of the action
        /// </returns>
        [HttpPut, Route("ChangeBankAccountStatus")]
        [AuthAudit(SpendManagementElement.BankAccounts, AccessRoleType.View)]
        public StringResponse ChangeBankAccountStatus([FromUri] int id, [FromUri] bool archive)
        {
            var response = this.InitialiseResponse<StringResponse>();
            response.Value = ((BankAccountRepository)this.Repository).ChangeBankAccountStatus(id, archive);
            return response;
        }

        /// <summary>
        /// Archives or un-archives a <see cref="BankAccount">BankAccount</see> as an administrator
        /// </summary>
        /// <param name="id">
        /// The id of the Bank Account to be archived/un-archived.
        /// </param>
        /// <param name="archive">
        /// Whether to archive or un-archive this <see cref="BankAccount">BankAccount</see>.
        /// </param>
        /// <returns>
        /// The <see cref="StringResponse">StringResponse</see> with the outcome of the action
        /// </returns>
        [HttpPatch, Route("{id:int}/Archive/{archive:bool}")]
        [AuthAudit(SpendManagementElement.EmployeeBankAccounts, AccessRoleType.Edit)]
        public StringResponse Archive(int id, bool archive)
        {
            var response = this.InitialiseResponse<StringResponse>();
            response.Value = ((BankAccountRepository)this.Repository).Archive(id, archive);
            return response;
        }
    }
}
