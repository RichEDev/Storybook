using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Web.Services;
using System.Text;

namespace Spend_Management
{
    public partial class aeexchangerate : System.Web.UI.Page
    {
        public int currID = 0;
        public int currencyid = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.enablenavigation = false;

            CurrentUser user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Currencies, true, true);
            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID;
            ViewState["subAccountID"] = user.CurrentSubAccountId;

            cMisc clsmisc = new cMisc(user.AccountID);

            cGlobalProperties clsproperties = clsmisc.GetGlobalProperties(user.AccountID);
            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();

            cCurrencies clsCurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);

            string action;
            CurrencyType currType = new CurrencyType();
            
            if (Request.QueryString["currencyid"] != null)
            {
                currencyid = Convert.ToInt32(Request.QueryString["currencyid"]);
            }
            ViewState["currencyid"] = currencyid;

            if (Request.QueryString["currencyType"] != null)
            {
                currType = (CurrencyType)byte.Parse(Request.QueryString["currencyType"].ToString());
            }
            ViewState["currencyType"] = currType;

            switch (currType)
            {
                case CurrencyType.Static:
                case CurrencyType.Monthly:
                    rangeCurrencies.Visible = false;
                    int currencymonthid = 0;

                    if (Request.QueryString["currencymonthid"] != null)
                    {
                        currencymonthid = Convert.ToInt32(Request.QueryString["currencymonthid"]);
                        currID = currencymonthid;
                    }
                    ViewState["currencymonthid"] = currencymonthid;

                    cMonthlyCurrencies clsMonthCurrencies = new cMonthlyCurrencies((int)ViewState["accountid"], (int)ViewState["subAccountID"]);

                    if (currencymonthid > 0)
                    {
                        action = "Edit ";
                        cMonthlyCurrency reqcurrency = clsMonthCurrencies.getCurrencyById(currencyid);

                        cCurrencyMonth reqmonth = reqcurrency.getCurrentMonthById(currencymonthid);

                        txtYear.Text = reqmonth.year.ToString();

                        cmbMonth.Items.FindByValue(reqmonth.month.ToString()).Selected = true;
                        cmbMonth.Enabled = false;
                    }
                    else
                    {
                        action = "Add ";
                    }

                    Master.PageSubTitle = action + "Currency Month";
                    litexchangerates.Text = clsCurrencies.CreateExchangeTable(currencyid, currencymonthid, CurrencyType.Monthly);
                    break;

                case CurrencyType.Range:

                    monthlyCurrencies.Visible = false;
                    int currencyrangeid = 0;

                    if (Request.QueryString["currencyrangeid"] != null)
                    {
                        currencyrangeid = Convert.ToInt32(Request.QueryString["currencyrangeid"]);
                        currID = currencyrangeid;
                    }
                    ViewState["currencyrangeid"] = currencyrangeid;

                    cRangeCurrencies clsRangeCurrencies = new cRangeCurrencies((int)ViewState["accountid"], (int)ViewState["subAccountID"]);

                    if (currencyrangeid > 0)
                    {
                        action = "Edit ";
                        cRangeCurrency reqcurrency = clsRangeCurrencies.getCurrencyById(currencyid);

                        cCurrencyRange reqrange = reqcurrency.getCurrencyRangeById(currencyrangeid);
                        txtStartDate.Text = reqrange.startdate.ToShortDateString();
                        txtEndDate.Text = reqrange.enddate.ToShortDateString();
                    }
                    else
                    {
                        action = "Add ";
                    }

                    Master.PageSubTitle = action + "Currency Range";
                    litexchangerates.Text = clsCurrencies.CreateExchangeTable(currencyid, currencyrangeid, CurrencyType.Range);
                    
                    break;
            }

            StringBuilder sbCurrencies = new StringBuilder("<script type=\"text/javascript\">\nvar lstCurrencyIDs = new Array()\n");

            int i = 0;
            foreach (int id in clsCurrencies.currencyList.Keys)
            {
                if (id != currencyid)
                {
                    sbCurrencies.Append("lstCurrencyIDs[" + i + "] = '" + id.ToString().Replace("'", "\'") + "';\n");
                    i++;
                }
            }

            sbCurrencies.Append("\n</script>");

            ClientScript.RegisterClientScriptBlock(this.GetType(), "variables", "var currencyType = " + (byte)currType + ";", true);
            ClientScript.RegisterClientScriptBlock(this.GetType(), "exists", "var currencyExists = false;", true);
            ClientScript.RegisterClientScriptBlock(this.GetType(), "lstCurrencies", sbCurrencies.ToString());
            
        }

        [WebMethod(EnableSession = true)]
        public static int saveCurrencyMonth(int currencyid, int currencymonthid, object[][] exchangerates, int month, int year)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            double exchangerate;
            int tocurrencyid;
            SortedList<int, double> lstExchangeRates = new SortedList<int, double>();

            cMonthlyCurrencies clsCurrencies = new cMonthlyCurrencies(user.AccountID, user.CurrentSubAccountId);

            if (clsCurrencies.checkCurrencyMonthExists(currencyid, currencymonthid, month, year))
            {
                return 1;
            }

            for (int i = 0; i < exchangerates.Length; i++)
            {
                tocurrencyid = int.Parse(exchangerates[i][0].ToString());

                if (double.TryParse(exchangerates[i][1].ToString(), out exchangerate))
                {
                    lstExchangeRates.Add(tocurrencyid, exchangerate);
                }
            }

            cCurrencyMonth monthcurrency = new cCurrencyMonth(user.AccountID, currencyid, currencymonthid, month, year, DateTime.UtcNow, user.EmployeeID, DateTime.UtcNow, user.EmployeeID, lstExchangeRates);

            clsCurrencies.saveCurrencyMonth(monthcurrency);
            
            return 0;

        }
        [WebMethod(EnableSession = true)]
        public static int saveCurrencyRange(int currencyid, int currencyrangeid, object[][] exchangerates, string strStartDate, string strEndDate)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            double exchangerate;
            int tocurrencyid;
            DateTime startdate = DateTime.Parse(strStartDate);
            DateTime enddate = DateTime.Parse(strEndDate);

            SortedList<int, double> lstExchangeRates = new SortedList<int, double>();

            cRangeCurrencies clsCurrencies = new cRangeCurrencies(user.AccountID, user.CurrentSubAccountId);

            if (clsCurrencies.checkCurrencyRangeExists(currencyid, currencyrangeid, startdate, enddate))
            {
                return 1;
            }

            for (int i = 0; i < exchangerates.Length; i++)
            {
                tocurrencyid = int.Parse(exchangerates[i][0].ToString());

                if (double.TryParse(exchangerates[i][1].ToString(), out exchangerate))
                {
                    lstExchangeRates.Add(tocurrencyid, exchangerate);
                }
            }

            cCurrencyRange rangecurrency = new cCurrencyRange(user.AccountID, currencyid, currencyrangeid, startdate, enddate, DateTime.UtcNow, user.EmployeeID, DateTime.UtcNow, user.EmployeeID, lstExchangeRates);

            clsCurrencies.saveCurrencyRange(rangecurrency);
            cCurrency.RemoveFromCache(clsCurrencies.AccountID, currencyid);

            return 0;
        }
            
        

        protected void cmdcancel_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("aecurrency.aspx?currencyid=" + (int)ViewState["currencyid"] + "&currencyType=" + (byte)(CurrencyType)ViewState["currencyType"]);
        }

        //[WebMethod(EnableSession = true)]
        //public static bool checkMonthExchangeExists(int currencyid, int month, int Year)
        //{
        //    if (HttpContext.Current.User.Identity.IsAuthenticated)
        //    {
        //        CurrentUser user = cMisc.GetCurrentUser();
        //        cCurrencies clsCurrencies = new cCurrencies(user.AccountID);
        //        bool exists = clsCurrencies.checkCurrencyMonthExists(currencyid, month, Year);
        //        return exists;
        //    }

        //    return false;
        //}

        //[WebMethod(EnableSession = true)]
        //public static bool checkRangeExchangeExists(int currencyid, DateTime startDate, DateTime endDate)
        //{
        //    if (HttpContext.Current.User.Identity.IsAuthenticated)
        //    {
        //        CurrentUser user = cMisc.GetCurrentUser();
        //        cCurrencies clsCurrencies = new cCurrencies(user.AccountID);
        //        bool exists = clsCurrencies.checkCurrencyRangeExists(currencyid, startDate, endDate);
        //        return exists;
        //    }

        //    return false;
        //}

    }
}
