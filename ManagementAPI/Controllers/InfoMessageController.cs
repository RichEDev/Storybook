namespace ManagementAPI.Controllers
{
    using System.Collections.Generic;
    using System.Web.Http;
    using ManagementAPI.Models;
    using ManagementAPI.Repositories.InfoMessage;
    using System.Net.Http;
    using System.Net;
    using ManagementAPI.Filters;

    [RoutePrefix("InfoMessage")]
    [ManagementToolExceptionFilter]
    public class InfoMessageController : ApiController
    {
        /// <summary>
        /// An instance of <see cref="IEmployeeRepository"/>.
        /// </summary>
        private readonly InfoMessageRepository _repository = new InfoMessageRepository();

        /// <summary>
        /// Returns a list of all information messages.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<InformationMessage> Get()
        {
            return this._repository.GetAll();
        }

        /// <summary>
        /// Returns an information message by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public InformationMessage Get(int id)
        {
            return this._repository.Get(id);
        }

        /// <summary>
        /// Saves the information message. If it does not already exist, creates a new one.
        /// </summary>
        /// <param name="infoMessage"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Save(InformationMessage infoMessage)
        {
            if (ModelState.IsValid == false)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                this._repository.Save(infoMessage);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Deletes an information message by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public bool Delete(int id)
        {
            return this._repository.Delete(id);
        }
    }
}