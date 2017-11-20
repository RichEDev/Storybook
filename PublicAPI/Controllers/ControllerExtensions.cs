using System.Collections.Generic;
using System.Web.Http.Results;

namespace PublicAPI.Controllers
{
    using System.Net;
    using System.Web.Http;

    public static class ControllerExtensions
    {
        //TODO - this is a work in progress still
        public static IHttpActionResult Result<T>(this ApiController controller, T content)
        {
            if (content == null || EqualityComparer<T>.Default.Equals(content, default(T)))
            {
                return new HttpActionResult(HttpStatusCode.NotFound, "Does not exist or cannot be found.");
            }

            return new HttpActionResult(HttpStatusCode.OK, "");
        }
    }
}