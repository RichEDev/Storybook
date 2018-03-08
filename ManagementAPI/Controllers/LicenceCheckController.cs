namespace ManagementAPI.Controllers
{
    using ManagementAPI.LicenceCheckAPI;
    using ManagementAPI.Models;
    using ManagementAPI.Repositories.LicenceCheck;
    using System.Web.Http;

    public class LicenceCheckController : ApiController
    {
        private readonly LicenceCheckRepository _repository = new LicenceCheckRepository();

        /// <summary>
        /// Creates a company on the Licence Check API.
        /// </summary>
        /// <param name="licenceCheckForm"></param>
        /// <returns></returns>
        [HttpPost]
        public CompanyResponse CreateCompany(LicenceCheckForm licenceCheckForm)
        {
            return _repository.Setup(licenceCheckForm);
        }
    }
}