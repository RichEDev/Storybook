using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using SpendManagementLibrary;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcCreditCards
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcCreditCards : System.Web.Services.WebService
    {
        /// <summary>
        /// Save the card companies to the database
        /// </summary>
        /// <param name="lstCardCompanies"></param>
        /// <returns></returns>
        [WebMethod(EnableSession=true)]
        public void SaveCardCompanies(List<cCardCompany> lstCardCompanies)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cCardCompanies clsCardComps = new cCardCompanies(user.AccountID);
            cCardCompany cachedComp = null;

            foreach (cCardCompany comp in lstCardCompanies)
            {
                cachedComp = clsCardComps.GetCardCompanyByNumber(comp.companyNumber);

                if (cachedComp != null)
                {
                    //If editing and the item used for import status is still the same then dont save
                    if (cachedComp.usedForImport == comp.usedForImport)
                    {
                        continue;
                    }
                }

                clsCardComps.SaveCardCompany(comp);
            }
        }
    }
}
