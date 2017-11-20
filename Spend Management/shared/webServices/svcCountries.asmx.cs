using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using SpendManagementLibrary;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcCountries
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcCountries : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        public object[] getCountryItems(int countryID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCountries countries = new cCountries(currentUser.AccountID, currentUser.CurrentSubAccountId);
            List<ListItem> items = countries.CreateDropDown();
            if (countryID != 0)
            {
                cCountry country = countries.getCountryById(countryID);
                cGlobalCountries clsglobalcountries = new cGlobalCountries();
                if (country != null)
                    items.Add(new ListItem(clsglobalcountries.getGlobalCountryById(country.GlobalCountryId).Country, countryID.ToString()));
            }
            return items.ToArray();
        }
    }
}
