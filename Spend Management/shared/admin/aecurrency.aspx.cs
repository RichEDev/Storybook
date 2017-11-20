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
    public partial class aecurrency : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                Master.enablenavigation = false;
                cmbCurrency.Attributes.Add("onchange", "getCurrencyDetails();");
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Currencies, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["subAccountID"] = user.CurrentSubAccountId;
                ViewState["employeeid"] = user.EmployeeID;

                switch (user.CurrentActiveModule)
                {
                    case Modules.contracts:
                        Master.helpid = 1165;
                        break;
                    default:
                        Master.helpid = 1020;
                        break;
                }

                cMisc clsmisc = new cMisc(user.AccountID);

                cGlobalProperties clsproperties = clsmisc.GetGlobalProperties(user.AccountID);
                cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();

                cCurrencies clsCurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);

                int currencyid = 0;
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

                cmbCurrency.Items.AddRange(clsglobalcurrencies.CreateDropDown().ToArray());
                cmbPositiveFormat.Items.AddRange(clsCurrencies.CreatePositiveFormatDropDown().ToArray());
                cmbNegativeFormat.Items.AddRange(clsCurrencies.CreateNegativeFormatDropDown().ToArray());

                if (currencyid > 0)
                {
                    user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Currencies, true, true);
                    action = "Edit ";
                    
                    cCurrency reqcurrency;

                    currencyid = int.Parse(Request.QueryString["currencyid"].ToString());

                    reqcurrency = clsCurrencies.getCurrencyById(currencyid);

                    cGlobalCurrency globalcurrency = clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid);

                    if (cmbCurrency.Items.FindByValue(reqcurrency.globalcurrencyid.ToString()) != null)
                    {
                        cmbCurrency.Items.FindByValue(reqcurrency.globalcurrencyid.ToString()).Selected = true;
                    }

                    if (cmbPositiveFormat.Items.FindByValue(reqcurrency.positiveFormat.ToString()) != null)
                    {
                        cmbPositiveFormat.Items.FindByValue(reqcurrency.positiveFormat.ToString()).Selected = true;
                    }

                    if (cmbNegativeFormat.Items.FindByValue(reqcurrency.negativeFormat.ToString()) != null)
                    {
                        cmbNegativeFormat.Items.FindByValue(reqcurrency.negativeFormat.ToString()).Selected = true;
                    }

                    txtAlphaCode.Text = globalcurrency.alphacode;
                    txtSymbol.Text = globalcurrency.symbol;
                    txtNumericCode.Text = globalcurrency.numericcode;

                    cmbCurrency.Enabled = false;
                }
                else
                {
                    user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Currencies, true, true);
                    action = "Add ";
                    foreach (cCurrency currency in clsCurrencies.currencyList.Values)
                    {
                        cmbCurrency.Items.RemoveAt(cmbCurrency.Items.IndexOf(cmbCurrency.Items.FindByValue(currency.globalcurrencyid.ToString())));
                    }
                    cGlobalCurrency global = clsglobalcurrencies.getGlobalCurrencyById(int.Parse(cmbCurrency.SelectedValue));
                    txtSymbol.Text = global.symbol;
                    txtNumericCode.Text = global.numericcode;
                    txtAlphaCode.Text = global.alphacode;
                }

                switch (user.CurrentActiveModule)
                {
                    case Modules.SpendManagement:
                    case Modules.SmartDiligence:
                    case Modules.contracts:
                        Master.title = action + " Currency";
                        cmdAddExchange.Visible = false;
                        staticCurrencies.Visible = true;

                        litexchangerates.Text = clsCurrencies.CreateExchangeTable(currencyid, 0, CurrencyType.Static);
                        break;
                    default:
                        switch (currType)
                        {
                            case CurrencyType.Static:
                                Master.title = action + "Static Currency";
                                cmdAddExchange.Visible = false;
                                staticCurrencies.Visible = true;

                                litexchangerates.Text = clsCurrencies.CreateExchangeTable(currencyid, 0, CurrencyType.Static);
                                break;

                            case CurrencyType.Monthly:
                                Master.title = action + "Monthly Currency";
                                CreateGrid();
                                break;

                            case CurrencyType.Range:
                                Master.title = action + "Ranged Currency";
                                CreateGrid();
                                break;
                        }
                        break;
                }

                ClientScript.RegisterClientScriptBlock(this.GetType(), "variables", "var currencyType = " + (byte)currType + ";", true);
                Master.PageSubTitle = "Currency Details";
            }
        }

        private void CreateGrid()
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            cGridNew newgrid = null;
            cCurrencies clsCurrencies = new cCurrencies((int)ViewState["accountid"], (int)ViewState["subAccountID"]);

            switch ((CurrencyType)ViewState["currencyType"])
            {
                case CurrencyType.Static:
                    break;

                case CurrencyType.Monthly:
                    cMonthlyCurrencies clsMonthCurrencies = new cMonthlyCurrencies((int)ViewState["accountid"], (int)ViewState["subAccountID"]);
                    newgrid = new cGridNew((int)ViewState["accountid"], (int)ViewState["employeeid"], "gridMonthlyCurrencies", clsMonthCurrencies.getMonthGrid());
                    newgrid.editlink = "aeexchangerate.aspx?currencyid=" + (int)ViewState["currencyid"] + "&currencymonthid={currencymonthid}&currencyType=" + (byte)(CurrencyType)ViewState["currencyType"];
                    newgrid.deletelink = "javascript:deleteMonthlyCurrency(" + (int)ViewState["currencyid"] + ",{currencymonthid});";
                    newgrid.getColumnByName("currencymonthid").hidden = true;
                    newgrid.EmptyText = "No Monthly Currencies Exist";
                    newgrid.KeyField = "currencymonthid";

                    #region Get Month Names

                    ((cFieldColumn)newgrid.getColumnByName("Month")).addValueListItem(1, "January");
                    ((cFieldColumn)newgrid.getColumnByName("Month")).addValueListItem(2, "February");
                    ((cFieldColumn)newgrid.getColumnByName("Month")).addValueListItem(3, "March");
                    ((cFieldColumn)newgrid.getColumnByName("Month")).addValueListItem(4, "April");
                    ((cFieldColumn)newgrid.getColumnByName("Month")).addValueListItem(5, "May");
                    ((cFieldColumn)newgrid.getColumnByName("Month")).addValueListItem(6, "June");
                    ((cFieldColumn)newgrid.getColumnByName("Month")).addValueListItem(7, "July");
                    ((cFieldColumn)newgrid.getColumnByName("Month")).addValueListItem(8, "August");
                    ((cFieldColumn)newgrid.getColumnByName("Month")).addValueListItem(9, "September");
                    ((cFieldColumn)newgrid.getColumnByName("Month")).addValueListItem(10, "October");
                    ((cFieldColumn)newgrid.getColumnByName("Month")).addValueListItem(11, "November");
                    ((cFieldColumn)newgrid.getColumnByName("Month")).addValueListItem(12, "December");

                    #endregion

                    break;

                case CurrencyType.Range:
                    cRangeCurrencies clsRangeCurrencies = new cRangeCurrencies((int)ViewState["accountid"], (int)ViewState["subAccountID"]);
                    newgrid = new cGridNew((int)ViewState["accountid"], (int)ViewState["employeeid"], "gridRangedCurrencies", clsRangeCurrencies.getRangeGrid());
                    newgrid.editlink = "aeexchangerate.aspx?currencyid=" + (int)ViewState["currencyid"] + "&currencyrangeid={currencyrangeid}&currencyType=" + (byte)(CurrencyType)ViewState["currencyType"];
                    newgrid.deletelink = "javascript:deleteRangedCurrency(" + (int)ViewState["currencyid"] + ",{currencyrangeid});";
                    newgrid.getColumnByName("currencyrangeid").hidden = true;
                    newgrid.EmptyText = "No Ranged Currencies Exist";
                    newgrid.KeyField = "currencyrangeid";
                    break;
            }

            var clsFields = new cFields((int)ViewState["accountid"]);
            var values = new object[] { (int)ViewState["currencyid"] };

            cField currencyId = clsFields.GetFieldByID(new Guid("AA4D627C-03C2-4B1D-9FD8-D517E6A693DD"));
            newgrid.addFilter(currencyId, ConditionType.Equals, values, null, ConditionJoiner.None);
            newgrid.enabledeleting = true;
            newgrid.enableupdating = true;
            
            string[] gridData = newgrid.generateGrid();
            litgrid.Text = gridData[1];

            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "aecurrencyGridVars", cGridNew.generateJS_init("aecurrencyGridVars", new List<string>() { gridData[0] }, curUser.CurrentActiveModule), true);
        }

        [WebMethod(EnableSession = true)]
        public static object[] getCurrencyDetails(int globalCurrencyID)
        {
            cGlobalCurrencies clsGlobalCurrencies = new cGlobalCurrencies();
            cGlobalCurrency currency = clsGlobalCurrencies.getGlobalCurrencyById(globalCurrencyID);
            List<string> details = new List<string>();
            details.Add(currency.alphacode);
            details.Add(currency.numericcode);
            details.Add(currency.symbol);
            return details.ToArray();
        }

        /// <summary>
        /// Web Method to delete the currency month from the page grid and the database
        /// </summary>
        /// <param name="currencyID">ID of the currency</param>
        /// <param name="currencymonthID">ID of the currency month</param>
        [WebMethod(EnableSession = true)]
        public static void deleteCurrencyMonth(int currencyID, int currencymonthID)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                cMonthlyCurrencies clsCurrencies = new cMonthlyCurrencies(user.AccountID, user.CurrentSubAccountId);
                clsCurrencies.deleteCurrencyMonth(currencyID, currencymonthID);
            }
        }

        /// <summary>
        /// Web Method to delete the currency range from the page grid and the database
        /// </summary>
        /// <param name="currencyID">ID of the currency</param>
        /// <param name="currencyrangeID">ID of the currency range</param>
        [WebMethod(EnableSession = true)]
        public static void deleteCurrencyRange(int currencyID, int currencyrangeID)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                cRangeCurrencies clsCurrencies = new cRangeCurrencies(user.AccountID, user.CurrentSubAccountId);
                clsCurrencies.deleteCurrencyRange(currencyID, currencyrangeID);
            }
        }

        /// <summary>
        /// Add all Static Exchange rates to the database by reading through the exchange rate input boxes on the page
        /// </summary>
        /// <param name="currencyID">ID of the currency</param>
        private void saveStaticExchangeRates(int currencyID)
        {
            double exchangerate;
            SortedList<int, double> exchangerates = new SortedList<int, double>();
            cCurrencies clsCurrencies = new cCurrencies((int)ViewState["accountid"], (int)ViewState["subAccountID"]);

            foreach(int currID in clsCurrencies.currencyList.Keys)
            {
                if (Request.Form["exchangerate" + currID] != null)
                {
                    if (Request.Form["exchangerate" + currID] != "")
                    {
                        exchangerate = double.Parse(Request.Form["exchangerate" + currID]);
                        exchangerates.Add(currID, exchangerate);

                    }
                }
            }
            clsCurrencies.addExchangeRates(currencyID, CurrencyType.Static, exchangerates, DateTime.UtcNow, (int)ViewState["employeeid"]);
            cCurrency.RemoveFromCache(clsCurrencies.AccountID, currencyID);
        }

        /// <summary>
        /// Save the curency to the database, getting the values for the currency object from the page input controls
        /// </summary>
        private int saveCurrency()
        {
            cCurrencies clsCurrencies = new cCurrencies((int)ViewState["accountid"], (int)ViewState["subAccountID"]);
            int globalCurrencyID = int.Parse(cmbCurrency.SelectedValue);
            byte bPositiveFormat = byte.Parse(cmbPositiveFormat.SelectedValue);
            byte bNegativeFormat = byte.Parse(cmbNegativeFormat.SelectedValue);

            cCurrency currency = new cCurrency((int)ViewState["accountid"], (int)ViewState["currencyid"], globalCurrencyID, bPositiveFormat, bNegativeFormat, false, DateTime.UtcNow, (int)ViewState["employeeid"], DateTime.UtcNow, (int)ViewState["employeeid"]);
            int currencyid = clsCurrencies.saveCurrency(currency);

            if ((CurrencyType)ViewState["currencyType"] == CurrencyType.Static)
            {
                saveStaticExchangeRates(currencyid);
            }

            return currencyid;
        }

        protected void cmdok_Click(object sender, ImageClickEventArgs e)
        {
            saveCurrency();
            Response.Redirect("admincurrencies.aspx", true);
        }

        protected void cmdAddExchange_Click(object sender, EventArgs e)
        {
            int currencyid = saveCurrency();
            Response.Redirect("aeexchangerate.aspx?currencyid=" + currencyid + "&currencyType=" + (byte)(CurrencyType)ViewState["currencyType"], true);
        }

        protected void cmdcancel_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("admincurrencies.aspx", true);
        }
    }
}
