using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cCardCompanyObject
    {
        /// <summary>
        /// Get the global card company object
        /// </summary>
        /// <returns></returns>
        private static cCardCompany GetCardCompany()
        {
            cCardCompany comp = new cCardCompany(0, "TestCreditCardComp", "1234", true, null, null, null, null);
            return comp;
        }

        /// <summary>
        /// Create the global card company object and save in the database
        /// </summary>
        /// <returns></returns>
        public static cCardCompany CreateCardCompany()
        {
            cCardCompanies clsCardComps = new cCardCompanies(cGlobalVariables.AccountID);
            int compID = clsCardComps.SaveCardCompany(GetCardCompany());
            cCardCompany comp = clsCardComps.GetCardCompanyByID(compID);
            return comp;
        }

        /// <summary>
        /// Delete the card company from the database
        /// </summary>
        /// <param name="ID"></param>
        public static void DeleteCardCompany(int ID)
        {
            cCardCompanies clsCardComps = new cCardCompanies(cGlobalVariables.AccountID);
            clsCardComps.DeleteCardCompany(ID);
        }
    }
}
