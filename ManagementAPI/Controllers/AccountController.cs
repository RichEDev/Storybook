namespace ManagementAPI.Controllers
{
    using ManagementAPI.Filters;
    using ManagementAPI.Models;
    using ManagementAPI.Repositories.Accounts;
    using ManagementAPI.ViewModels;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
        
    [RoutePrefix("Account")]
    [ManagementToolExceptionFilter]
    public class AccountController : ApiController
    {
        /// <summary>
        /// An instance of <see cref="IAccountRepository"/>.
        /// </summary>
        private readonly AccountRepository _repository = new AccountRepository();

        /// <summary>
        /// Returns a list of all database servers.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<DatabaseServer> GetDatabaseServers()
        {
            return this._repository.GetDatabaseServers();
        }

        #region Accounts

        /// <summary>
        /// Returns a list of all accounts.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Account> GetAccounts()
        {
            return this._repository.GetAll();
        }

        /// <summary>
        /// Returns a single account by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public Account GetAccount(int id)
        {
            return this._repository.Get(id);
        }

        /// <summary>
        /// Saves an account to the database.
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SaveAccount(AccountViewModel vm)
        {
            if (ModelState.IsValid == false)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                this._repository.Save(vm);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Delete an account from the database by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public bool DeleteAccount(int id)
        {
            return this._repository.Delete(id);
        }

        #endregion

        #region Hostnames

        /// <summary>
        /// Returns a list of all hostnames.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Hostname> GetHostnames()
        {
            return this._repository.GetHostnames();
        }

        /// <summary>
        /// Returns a list of all hostnames associated with an account by the account's ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public List<int> GetHostnamesByAccountId(int id)
        {
            return this._repository.GetHostnamesByAccountId(id);
        }

        /// <summary>
        /// Associates a hostname with an account.
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SaveHostnameToAccount(HostnameLicensedElementBundle bundle)
        {
            if (ModelState.IsValid == false)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                this._repository.SaveHostnameToAccount(bundle);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Dissociates a hostname from an account.
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        [HttpDelete]
        public HttpResponseMessage RemoveHostnameFromAccount(HostnameLicensedElementBundle bundle)
        {
            if (ModelState.IsValid == false)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                this._repository.RemoveHostnameFromAccount(bundle);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        }

        #endregion

        #region Licensed Elements

        /// <summary>
        /// Returns a list of all licensed elements.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<LicensedElement> GetLicensedElements()
        {
            return this._repository.GetLicensedElements();
        }

        /// <summary>
        /// Returns a list of all licensed elements associated with an account by the account's ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public List<int> GetLicensedElementsByAccountId(int id)
        {
            return this._repository.GetLicensedElementsByAccountId(id);
        }

        /// <summary>
        /// Associates a licensed element with an account.
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SaveLicensedElementToAccount(HostnameLicensedElementBundle bundle)
        {
            if (ModelState.IsValid == false)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                this._repository.SaveLicensedElementToAccount(bundle);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Dissociates a licensed element from an account.
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        [HttpDelete]
        public HttpResponseMessage RemoveLicensedElementFromAccount(HostnameLicensedElementBundle bundle)
        {
            if (ModelState.IsValid == false)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                this._repository.RemoveLicensedElementFromAccount(bundle);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        }

        #endregion
    }
}