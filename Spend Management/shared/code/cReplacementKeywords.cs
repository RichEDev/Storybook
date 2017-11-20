using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementLibrary;

namespace Spend_Management
{
    /// <summary>
    /// Class that allows replacement of keywords
    /// </summary>
    [Obsolete]
    public class cReplacementKeywords
    {
        private ICurrentUser oCurrentUser;

        public cReplacementKeywords(ICurrentUser currentUser)
        {
            oCurrentUser = currentUser;
        }

        public object Replace(Type dataType, string stringValue)
        {
            // Replace @ME_ID with the CurrentUser.EmployeeID
            if(stringValue.Contains("@ME_ID") == true)
            {
                stringValue = stringValue.Replace("@ME_ID", oCurrentUser.EmployeeID.ToString());
            }

            // Replace @ME with the CurrentUser.Employee.username
            if(stringValue.Contains("@ME") == true)
            {
                stringValue = stringValue.Replace("@ME", oCurrentUser.Employee.Username);
            }

            // Replace @ACTIVEMODULE_ID with CurrentUser.CurrentActiveModule (int value for the enumerator)
            if(stringValue.Contains("@ACTIVEMODULE_ID") == true)
            {
                stringValue = stringValue.Replace("@ACTIVEMODULE_ID", oCurrentUser.CurrentActiveModule.ToString());
            }

            // Replace @ACTIVEMODULE with CurrentUser.CurrentActiveModule (string value for the enumerator)
            if(stringValue.Contains("@ACTIVEMODULE") == true)
            {
                stringValue = stringValue.Replace("@ACTIVEMODULE", Convert.ToInt32(oCurrentUser.CurrentActiveModule).ToString());
            }

            // Replace @ACTIVESUBACCOUNT_ID with CurrentUser.CurrentActiveSubAccountId
            if(stringValue.Contains("@ACTIVESUBACCOUNT_ID") == true)
            {
                stringValue = stringValue.Replace("@ACTIVESUBACCOUNT_ID", oCurrentUser.CurrentSubAccountId.ToString());
            }

            if(dataType == typeof(int))
            {
                return Convert.ToInt32(stringValue);
            }
            else if(dataType == typeof(decimal))
            {
                return Convert.ToDecimal(stringValue);
            }
            else
            {
                return stringValue;
            }
        }
    }
}
