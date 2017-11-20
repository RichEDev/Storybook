using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Web.Script.Serialization;
using SpendManagementLibrary;
using Spend_Management;

namespace tempMobileUnitTests
{
    internal class WcfServiceObject
    {
        public static MobileAPI GetMobileAPI(ICurrentUser currentUser)
        {
            cAccountSubAccounts subcaccs = new cAccountSubAccounts(currentUser.AccountID);
            MobileAPI api = new MobileAPI { AccountID = currentUser.AccountID, SubAccountID = subcaccs.getFirstSubAccount().SubAccountID };

            return api;
        }

        public static string GeneratePairingKey(int employeeId)
        {
            MobileAPI api = new MobileAPI();
            cMobileDevices devices = new cMobileDevices(Moqs.CurrentUser(), GlobalVariables.MetabaseConnectionString);
            return devices.GeneratePairingKey(employeeId);
        }

        public static object CallMobileServiceMethod(string methodName, Dictionary<string, object> methodParameters, Type returnObjectType) 
        {
            WebClient client = new WebClient();
            client.Headers["Content-type"] = "application/json";
            string apiUrl = ConfigurationManager.AppSettings["MobileAPI"];

            byte[] responseArray = client.UploadData(apiUrl + "/" + methodName, "POST", EncodeParameters(methodParameters));

            string responseStr = Encoding.ASCII.GetString(responseArray);

            JavaScriptSerializer jsr = new JavaScriptSerializer();
            
            object returnObject = jsr.CleanAndDeserialize(responseStr, returnObjectType);

            return returnObject;
        }

        private static byte[] EncodeParameters(IEnumerable variables)
        {
            StringBuilder sb = new StringBuilder();
            string comma = "";

            sb.Append("{ ");

            foreach (KeyValuePair<string, object> kvp in variables)
            {
                sb.Append(comma);
                sb.Append("\"" + kvp.Key + "\":\"" + kvp.Value.ToString() + "\"");
                comma = ",";
            }

            sb.Append(" }");

            return Encoding.Default.GetBytes(sb.ToString());
        }
    }
}
