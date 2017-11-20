namespace SpendManagementApi.Controllers.Mobile.V1
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Attributes.Mobile;

    using SpendManagementLibrary;

    using Spend_Management;

    using CompanyPolicyResult = SpendManagementLibrary.Mobile.CompanyPolicyResult;

    /// <summary>
    /// The mobile controller dealing with company policies.
    /// </summary>
    [Version(1)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MobileCompanyPolicyV1Controller : BaseMobileApiController
    {
        /// <summary>
        /// Gets the Company Policy for the mobile users account.
        /// </summary>
        /// <returns>The company policy contained in a <see cref="CompanyPolicyResult"/></returns>
        [HttpGet]
        [MobileAuth]
        [Route("mobile/companypolicy")]
        public CompanyPolicyResult GetCompanyPolicy()
        {
            CompanyPolicyResult result = new CompanyPolicyResult { FunctionName = "GetCompanyPolicy", ReturnCode = this.ServiceResultMessage.ReturnCode };

            if (this.ServiceResultMessage.ReturnCode == MobileReturnCode.Success)
            {
                try
                {
                    cAccountSubAccounts subaccs = new cAccountSubAccounts(this.PairingKeySerialKey.PairingKey.AccountID);
                    cAccountProperties clsproperties = subaccs.getFirstSubAccount().SubAccountProperties;

                    switch (clsproperties.PolicyType)
                    {
                        case 1:
                            result.isHTML = false;
                            result.CompanyPolicy = clsproperties.CompanyPolicy;

                            break;
                        case 2:
                            result.isHTML = true;

                            cAccounts clsaccounts = new cAccounts();
                            cAccount reqaccount = clsaccounts.GetAccountByID(this.PairingKeySerialKey.PairingKey.AccountID);
                            string policyPath = clsaccounts.GetFilePaths(this.PairingKeySerialKey.PairingKey.AccountID, FilePathType.PolicyFile);
                            string fullPath = Path.Combine(policyPath, reqaccount.companyid + ".htm");

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
                                        String str = new String(read, 0, count);
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
                            catch (Exception ex)
                            {
                                result.CompanyPolicy = "Company Policy Not Found.";
                                result.Message = "file not found";

                                // ReSharper disable PossibleIntendedRethrow
                                throw ex;
                                // ReSharper restore PossibleIntendedRethrow
                            }

                            break;
                    }
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry("MobileAPI.GetCompanyPolicy():Error:{ Pairingkey: " + this.PairingKeySerialKey.PairingKey + "\nSerialKey: " + this.PairingKeySerialKey.SerialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                    // ReSharper disable PossibleIntendedRethrow
                    throw ex;
                    // ReSharper restore PossibleIntendedRethrow
                }
            }

            return result;
        }
    }
}
