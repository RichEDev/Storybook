namespace Spend_Management
{
    using System;
    using System.ComponentModel;
    using System.Web.Services;
    using System.Web.Script.Services;

    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;

    /// <summary>
    /// Summary description for svcExchangeRates
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcExchangeRates : System.Web.Services.WebService
    {
        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public static object[] GetExchangeRate(int currencyID, DateTime date)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCurrencies clsCurrencies = new cCurrencies(currentUser.AccountID, currentUser.CurrentSubAccountId);
            cEmployees clsEmployees = new cEmployees(currentUser.AccountID);
            Employee reqemp = clsEmployees.GetEmployeeById(currentUser.EmployeeID);

            double exchangerate = 0;

            int basecurrency;
            if (reqemp.PrimaryCurrency != 0)
            {
                basecurrency = reqemp.PrimaryCurrency;
            }
            else
            {
                basecurrency = (int)FunkyInjector.Container.GetInstance<IDataFactory<IGeneralOptions, int>>()[currentUser.CurrentSubAccountId].WithCurrency().Currency.BaseCurrency;
            }

            exchangerate = clsCurrencies.getCurrencyById(basecurrency).getExchangeRate(currencyID, date);
            if (exchangerate == 0)
            {
                exchangerate = 1;
            }

            object[] data = new object[2];
            if (currencyID == basecurrency) // don't display
            {
                data[0] = false;
            }
            else
            {
                data[0] = true;
                data[1] = exchangerate;
            }
            return data;
        }
    }
}
