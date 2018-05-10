namespace expenses.webServices
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Web.Services;
    using System.Linq;
    using System.Web.Script.Services;
    using System.Web.UI.WebControls;

    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Hotels;

    using Spend_Management;
    using Spend_Management.shared.code.Helpers;

    /// <summary>
    /// This webservice is for the Add Edit Expenses page and contains a collection of function that are called by AJAX
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class svcAddEditExpenses : System.Web.Services.WebService
    {
        /// <summary>
        /// An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/>
        /// </summary>
        public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory = expenses.Global.container.GetInstance<IDataFactory<IGeneralOptions, int>>();

        [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object[] getExchangeRate(int accountid, int employeeid, int currencyid, DateTime date)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cCurrencies clscurrencies = new cCurrencies(accountid, user.CurrentSubAccountId);
            double exchangerate = 0;
            employeeid = user.Employee.EmployeeID;

            var generalOptions = this.GeneralOptionsFactory[user.CurrentSubAccountId].WithCurrency();

            cEmployees clsemployees = new cEmployees(accountid);
            Employee reqemp = clsemployees.GetEmployeeById(employeeid);
            int basecurrency = reqemp.PrimaryCurrency != 0 ? reqemp.PrimaryCurrency : (int)generalOptions.Currency.BaseCurrency;

            cCurrency currency = clscurrencies.getCurrencyById(basecurrency);
            exchangerate = currency.getExchangeRate(currencyid, date);
            if (exchangerate == 0)
            {
                exchangerate = 1;
            }

            object[] data = new object[2];
            if (currencyid == basecurrency) // don't display
            {
                data[0] = false;
            }
            else
            {
                var exRate = exchangerate.ToString();

                data[0] = true;
                data[1] = exRate.Length > 12 ? exRate.Substring(0, 12) : exRate;
            }

            return data;
        }

        [WebMethod(EnableSession = false), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object[] getHotelDetails(string id, string name)
        {
            Hotel hotel = Hotel.Get(name);

            object[] data = new object[8];

            data[0] = id;
            if (hotel != null)
            {
                data[1] = hotel.hotelid;
                data[2] = hotel.address1;
                data[3] = hotel.address2;
                data[4] = hotel.city;
                data[5] = hotel.county;
                data[6] = hotel.postcode;
                data[7] = hotel.country;
            }
            else
            {
                data[1] = 0;
            }
            return data;
        }

        [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object[] getAdvances(int accountid, int employeeid, int currencyid)
        {
            cFloats clsfloats = new cFloats(accountid);
            int i = 0;
            employeeid = cMisc.GetCurrentUser().Employee.EmployeeID;
            List<ListItem> items = clsfloats.CreateDropDown(employeeid, currencyid, 0);

            object[] data = new object[items.Count];
            foreach (ListItem item in items)
            {
                data[i] = new object[] { item.Value, item.Text };
                i++;
            }
            return data;
        }

        [WebMethod(EnableSession = false), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string[] getMileageComment(string id, int accountid, int employeeid, int carid, int mileageid, DateTime date, int subcatid, long esrAssignmentId)
        {
            var mileageCat = new cMileagecats(accountid);
            return mileageCat.GetComment(id, accountid, employeeid, carid, mileageid, date, subcatid, esrAssignmentId);
        }

        [WebMethod(EnableSession = false), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object[] popChildControls(string sType, string sFilterid, string id, string parCtlIndex, int accountid)
        {
            ArrayList objlst = new ArrayList();
            int parid = 0;
            if (id != "")
            {
                parid = int.Parse(id);
            }
            int filterid = int.Parse(sFilterid);
            int type = int.Parse(sType);
            cFilterRules clsfilterrules = new cFilterRules(accountid);
            cFilterRule rule = clsfilterrules.GetFilterRuleById(filterid);

            var generalOptions = this.GeneralOptionsFactory[cMisc.GetCurrentUser().CurrentSubAccountId]
                .WithCodeAllocation();

            Dictionary<int, cFilterRuleValue> lstRuleVals = rule.rulevals;
            object[] child;
            FilterType filtertype = (FilterType)type;
         
            if (filtertype == FilterType.Userdefined)
            {
                #region userdefined
                cUserdefinedFields clsudf = new cUserdefinedFields(accountid);
                int childuserdefineid = 0;
                string udfctl = "";
                string area = "";


                childuserdefineid = rule.childuserdefineid;

                cUserDefinedField field = clsudf.GetUserDefinedById(childuserdefineid);

                udfctl = "cmbudf" + field.userdefineid;

                if (field.Specific == false)
                {
                    area = "general";
                }
                else
                {
                    area = "individual";
                }

                foreach (cFilterRuleValue val in lstRuleVals.Values)
                {
                    if (val.parentid == parid)
                    {
                        foreach (KeyValuePair<int, cListAttributeElement> kvp in field.items)
                        {
                            if (val.childid == kvp.Key)
                            {
                                objlst.Add(new object[] { kvp.Value.elementText, val.childid.ToString(), });
                            }
                        }
                    }
                }

                if (objlst.Count == 0)
                {
                    foreach (KeyValuePair<int, cListAttributeElement> kvp in field.items)
                    {
                        objlst.Add(new object[] { kvp.Value.elementText, kvp.Key.ToString(), });
                    }
                }

                child = new object[] { udfctl, objlst.ToArray(), area, parCtlIndex };

                #endregion
            }
            else
            {

                string temp = clsfilterrules.getChildTargetControl(filtertype);
                string[] ctl = temp.Split(';');
                List<ListItem> lstItems = new List<ListItem>();

                var breakdown = new CostCodeBreakDownFactory().Initialize(filtertype, accountid);

                foreach (cFilterRuleValue val in lstRuleVals.Values)
                {
                    if (val.parentid == parid)
                    {
                        string item;
                        switch (filtertype)
                        {
                            case FilterType.Costcode:
                                // No need to get the cost code item details as these are populated in the cost code type ahead functionality.
                                item = string.Empty;
                                break;
                            case FilterType.Department:
                                item = clsfilterrules.GetParentOrChildItem(filtertype, val.childid, false, generalOptions.CodeAllocation.UseDepartmentDescription, breakdown);
                                break;
                            case FilterType.Projectcode:
                                item = clsfilterrules.GetParentOrChildItem(filtertype, val.childid, false, generalOptions.CodeAllocation.UseProjectCodeDesc, breakdown);
                                break;
                            default:
                                item = clsfilterrules.GetParentOrChildItem(filtertype, val.childid, false, false, breakdown);
                                break;
                        }

                        lstItems.Add(new ListItem(item, val.childid.ToString()));
                    }
                }

                if (lstItems.Count == 0)
                {

                    if (filtertype == FilterType.Costcode)
                    {
                        lstItems = clsfilterrules.GetItems(filtertype, generalOptions.CodeAllocation.UseCostCodeDescription);
                    }
                    else if (filtertype == FilterType.Department)
                    {
                        lstItems = clsfilterrules.GetItems(filtertype, generalOptions.CodeAllocation.UseDepartmentDescription);
                    }
                    else if (filtertype == FilterType.Projectcode)
                    {
                        lstItems = clsfilterrules.GetItems(filtertype, generalOptions.CodeAllocation.UseProjectCodeDesc);
                    }
                    else
                    {
                        lstItems = clsfilterrules.GetItems(filtertype, false);
                    }
                }

                lstItems = lstItems.OrderBy(o => o.Text).ToList();

                foreach (ListItem lstitem in lstItems)
                {
                    objlst.Add(new object[] { lstitem.Text, lstitem.Value });
                }

                child = new object[] { ctl[0], objlst.ToArray(), ctl[1], parCtlIndex };
            }
            return child;
        }

        [WebMethod, ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object[] getJourneyCats(int uom, int accountid)
        {
            cMileagecats clsmileage = new cMileagecats(accountid);
            ArrayList objlst = new ArrayList();


            foreach (ListItem item in clsmileage.CreateMileageCatsDropdown(0, uom))
            {
                objlst.Add(new object[] { item.Text, item.Value });
            }

            return objlst.ToArray();
        }

        [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ListItem[] GetAssignmentListItems(string claimDate)
        {
            var currentuser = cMisc.GetCurrentUser();
            var assignments = new cESRAssignments(currentuser.AccountID, currentuser.EmployeeID);
            DateTime date;
            if (!DateTime.TryParse(claimDate, out date))
            {
                date = DateTime.Now;
            }

            return assignments.GetAvailableAssignmentListItems(false, date);
        }


        /// <summary>
        /// Gets the defaultalpha3code via the country id. Null if not a valid PCA country
        /// </summary>
        /// <param name="countryid">Unique id for the selected country</param>
        /// <param name="countryname">The name of the selected country</param>
        [WebMethod(EnableSession = true), ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetPCAValidAlpha3CodeByCountryId(int accountid, int countryid, string countryname)
        {
            string alpha3Code = string.Empty;
            CurrentUser user = cMisc.GetCurrentUser();
            cCountries countries = new cCountries(accountid, user.CurrentSubAccountId);
            var filteredcountries = countries.GetPostcodeAnywhereEnabledCountries();
            var selectedcountry = countries.getCountryById(countryid);
            foreach (cGlobalCountry filteredCountry in filteredcountries)
            {
                if (filteredCountry.GlobalCountryId == selectedcountry.GlobalCountryId)
                {
                   alpha3Code = new cGlobalCountries().getGlobalCountryById(selectedcountry.GlobalCountryId).Alpha3CountryCode;
                }
            }                                       
            return alpha3Code;
        }
    }
}
