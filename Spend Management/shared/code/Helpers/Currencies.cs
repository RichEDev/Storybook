using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Spend_Management.shared.code.Helpers
{
    /// <summary>
    /// Help class for currencies
    /// </summary>
    public static class Currencies
    {
        /// <summary>
        /// Return a list of ListItems which contain currencies and no item is selected.
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="subaccountid"></param>
        /// <returns></returns>
        public static List<ListItem> GetCurrencyList(int accountid, int subaccountid)
        {
            var clsCurrencies = new cCurrencies(accountid, subaccountid);

            List<ListItem> list = clsCurrencies.CreateDropDown();
            foreach (var item in list.Where(item => item.Selected))
            {
                item.Selected = false;
            }

            return list;
        }

    }
}