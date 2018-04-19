namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web.Http;
    using System.Web.Http.Description;

    using BusinessLogic.FilePath;

    using Attributes;
    using Models.Common;
    using Utilities;
    using Spend_Management;

    using CompanyPolicyResult = SpendManagementLibrary.Mobile.CompanyPolicyResult;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Definitions;

    /// <summary>
    /// The mobile controller dealing with company policies.
    /// </summary> 
    [Version(1)]
    [RoutePrefix("CompanyPolicy")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CompanyPolicyV1Controller : BaseApiController
    {
        /// <summary>
        /// Gets the Company Policy for the users account.
        /// </summary>
        /// <returns>The company policy contained in a <see cref="CompanyPolicyResult"></see></returns>
        [HttpGet]
        [Route("")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public CompanyPolicyResult GetCompanyPolicy()
        {
            var result = new CompanyPolicyResult();

            try
            {
                var accountId = this.CurrentUser.AccountID;
                var subAccounts = new cAccountSubAccounts(accountId);
                cAccountProperties properties = subAccounts.getFirstSubAccount().SubAccountProperties;

                cAccounts accounts;
                cAccount reqAccount;

                switch (properties.PolicyType)
                {
                    case 1:
                        result.isHTML = false;
                        result.CompanyPolicy = properties.CompanyPolicy;

                        break;
                    case 2:
               
                        result.isHTML = true;

                        accounts = new cAccounts();
                        reqAccount = accounts.GetAccountByID(accountId);
                        string policyPath = accounts.GetFilePaths(accountId, FilePathType.PolicyFile);                              
                        string fullPath = Path.Combine(policyPath, reqAccount.companyid + ".htm");

                        try
                        {
                            StringBuilder sb = new StringBuilder();
                            Uri fileUrl = new Uri(fullPath);

                            WebRequest myFileWebRequest = WebRequest.CreateDefault(fileUrl);
                            WebResponse myFileWebResponse = myFileWebRequest.GetResponse();

                            Stream receiveStream = myFileWebResponse.GetResponseStream();
                            Encoding encode = Encoding.GetEncoding("utf-8");

                            if (receiveStream != null)
                            {
                                StreamReader readStream = new StreamReader(receiveStream, encode);

                                Char[] read = new Char[256];
                                // Read 256 characters at a time.    
                                int count = readStream.Read(read, 0, 256);

                                while (count > 0)
                                {
                                    // Dump the 256 characters on a string and display the string onto the console.
                                    var str = new String(read, 0, count);
                                    sb.Append(str);
                                    count = readStream.Read(read, 0, 256);
                                }

                                // Release resources of stream object.
                                readStream.Close();
                            }

                            // Release resources of response object.
                            myFileWebResponse.Close();

                            result.CompanyPolicy = sb.ToString();
                        }
                        catch (Exception)
                        {
                            throw new InvalidDataException(String.Format(ApiResources.ApiErrorCompanyPolicyNotFound));
                        }

                        break;

                    case 3:

                        result.isHTML = false;
                        accounts = new cAccounts();
                        reqAccount = accounts.GetAccountByID(accountId);
                        Host host = HostManager.GetHost(reqAccount.HostnameIds.FirstOrDefault());

                        var urlPath = $"{this.Request.RequestUri.Scheme}://{host.HostnameDescription}";
                        result.PdfPolicyUrlPath = $"{urlPath}/policies/{reqAccount.companyid}.pdf";

                        break;
                }
            }
            catch (Exception)
            {

                throw new ApiException(ApiResources.ApiErrorGetCompanyPolicyUnSucessful, ApiResources.ApiErrorGetCompanyPolicyMessage);
            }

            return result;
        }

        protected override void Init()
        {
        
        }
    }
}

