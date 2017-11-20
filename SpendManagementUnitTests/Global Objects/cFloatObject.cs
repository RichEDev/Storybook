using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    class cFloatObject
    {
        public static cFloat CreateObject()
        {
            cFloats advances = new cFloats(cGlobalVariables.AccountID);
            int approver = cEmployeeObject.CreateUTEmployee();
            cCurrency currency = cCurrencyObject.CreateCurrency();
            string name = "UnitTestFloat" + DateTime.UtcNow.ToString() + " " + DateTime.UtcNow.TimeOfDay.ToString();

            advances.requestFloat(cGlobalVariables.EmployeeID, name, "Unit Testing", (decimal)2.0, currency.currencyid, DateTime.Now.Date.ToShortDateString(), currency.currencyid);

            cFloat advance = advances.getFloatByName(cGlobalVariables.EmployeeID,name);
            
            return advance;
        }
    }
}
