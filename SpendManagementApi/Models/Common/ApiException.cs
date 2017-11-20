using System;
using System.Web;

namespace SpendManagementApi.Models.Common
{
    /// <summary>
    /// ApiException is different to ApiError, in that the ApiError indicates issues within the API and not http.
    /// Api Exception is a way of adding more detail and convenience to HttpException.
    /// </summary>
    public class ApiException : HttpException
    {
        private readonly string _code ;

        public ApiException(string code, string message) : base(message)
        {
            _code = code;
        }




        public string Code
        {
            get { return _code; }
        }
    }
}