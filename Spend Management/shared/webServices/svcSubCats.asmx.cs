using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;
using SpendManagementLibrary;
using SpendManagementLibrary.Expedite;

namespace Spend_Management.shared.webServices
{
    /// <summary>
    /// Summary description for svcSubCats
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class svcSubCats : WebService
    {
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public void SwitchSubAccount(int SubAccountID)
        {
            System.Web.HttpContext.Current.Session["SubAccountID"] = SubAccountID;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public cSubcatVatRate GetVatRange(int subCatID, int vatRangeID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cSubcats clssubcats = new cSubcats(user.AccountID);
            cSubcat subcat = clssubcats.GetSubcatById(subCatID);
            return subcat.getVatRateByID(vatRangeID);

        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] getSplitItems(string contextKey)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            int subcatid = 0;
            Int32.TryParse(contextKey, out subcatid);

            cSubcats clssubcats = new cSubcats(user.AccountID);
            cSubcat subcat = clssubcats.GetSubcatById(subcatid);


            cTables clstables = new cTables(user.AccountID);
            cFields clsfields = new cFields(user.AccountID);
            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("d4ed76bd-605c-45ce-b075-4c6018a50b08"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("44c2a53a-db33-45af-af66-d0055aad48ec"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("abfe0bb2-e6ac-40d0-88ce-c5f7b043924d"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("379b67dd-654e-43cd-b55d-b9b5262eeeee"))));

            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridModalSplit", clstables.GetTableByID(new Guid("401b44d7-d6d8-497b-8720-7ffcc07d635d")), columns);
            if (subcat != null)
            {
                object[] existingSubcats = new object[subcat.subcatsplit.Count];
                foreach (int i in subcat.subcatsplit)
                {
                    existingSubcats[subcat.subcatsplit.IndexOf(i)] = i;
                }
                if (existingSubcats.GetLength(0) > 0)
                {
                    clsgrid.addFilter(clsfields.GetFieldByID(new Guid("d4ed76bd-605c-45ce-b075-4c6018a50b08")), ConditionType.DoesNotEqual, existingSubcats, null, ConditionJoiner.None);
                }
            }
            if (subcatid > 0)
            {
                clsgrid.addFilter(clsfields.GetFieldByID(new Guid("d4ed76bd-605c-45ce-b075-4c6018a50b08")), ConditionType.DoesNotEqual, new object[] { subcatid }, null, ConditionJoiner.And);
            }
            clsgrid.getColumnByName("subcatid").hidden = true;
            clsgrid.EnableSelect = true;
            clsgrid.KeyField = "subcatid";

            List<string> retVals = new List<string>();
            retVals.Add(clsgrid.GridID);
            retVals.AddRange(clsgrid.generateGrid());
            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public void AddVatDateRange(int reqVatDateRangeID, int subcatid, DateTime? date1, DateTime? date2, DateRangeType datetype, double vatamount, byte vatpercent, bool vatreceipt, decimal vatlimitwithout, decimal vatlimitwith)
        {

            CurrentUser user = cMisc.GetCurrentUser();
            cSubcats clssubcats = new cSubcats(user.AccountID);
            cSubcatVatRate rate = new cSubcatVatRate(reqVatDateRangeID, subcatid, vatamount, vatreceipt, vatlimitwithout, vatlimitwith, vatpercent, datetype, date1, date2);
            clssubcats.SaveVatRate(rate);

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public bool deleteVatDateRange(int vatRateID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cSubcats clssubcats = new cSubcats(user.AccountID);
            clssubcats.DeleteVatRate(vatRateID);
            return true;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int saveSubcat(int subcatid, string subcat, int categoryid, string accountcode, string description, decimal allowanceamount, bool addasnet, bool mileageapp, bool staffapp, bool othersapp, bool attendeesapp, bool pmilesapp, bool bmilesapp, bool tipapp, bool eventinhomeapp, bool passengersapp, bool nopassengersapp, bool passengernamesapp, bool splitentertainment, int entertainmentid, int pdcatid, bool reimbursable, bool nonights, bool hotelapp, string comment, bool attendeesmand, bool nodirectorsapp, string alternateaccountcode, bool hotelmand, bool nopersonalguests, bool noremoteworkers, bool splitpersonal, bool splitremote, bool reasonapp, bool otherdetails, int personalid, int remoteid, bool noroomsapp, bool vatnumber, bool vatnumbermand, bool fromapp, bool toapp, bool companyapp, string shortsubcat, bool receipt, CalculationType calculation, object[] arrCountries, List<int> allowances, List<int> associatedudfs, bool enableHomeToLocationMileage, HomeToLocationType hometolocationtype, int? mileageCategory, bool isRelocationMileage, int? reimbursableSubcatID, bool allowHeavyBulkyMileage, List<object> udfs, bool homeToOfficeAsZero, float? homeToOfficeFixedMiles, int? publicTransportRate, string startDateString, string endDateString, bool validate, List<int> validationRequirementIds, List<string> validationRequirements,bool enableDoc, bool requireClass1BusinessInsurance, float? homeToOfficeMileageCap, bool enforceMileageCap)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cSubcats clssubcats = new cSubcats(user.AccountID);
            cSubcat clssubcat = null;
            List<cCountrySubcat> countries = new List<cCountrySubcat>();
            SortedList<int, object> userdefined = new SortedList<int, object>();

            DateTime tempStartDate, tempEndDate;

            DateTime? startDate = DateTime.TryParse(startDateString, out tempStartDate) ? (DateTime?)tempStartDate : null;

            DateTime? endDate = DateTime.TryParse(endDateString, out tempEndDate) ? (DateTime?)tempEndDate : null;

            foreach (object o in udfs)
            {
                userdefined.Add(Convert.ToInt32(((object[])o)[0]), ((object[])o)[1]);
            }
            for (int i = 0; i < arrCountries.GetLength(0); i++)
            {
                countries.Add(new cCountrySubcat(subcatid, Convert.ToInt32(((object[])arrCountries[i])[0]), Convert.ToString(((object[])arrCountries[i])[1])));
            }

            var criteria = new List<ExpenseValidationCriterion>();
            criteria.AddRange(validationRequirementIds.Select((t, i) => new ExpenseValidationCriterion { Id = t, Requirements = validationRequirements[i] }));

            if (subcatid > 0) //update
            {
                cSubcat oldsubcat = clssubcats.GetSubcatById(subcatid);
                clssubcat = new cSubcat(subcatid, categoryid, subcat, description, mileageapp, staffapp, othersapp, tipapp, pmilesapp, bmilesapp, allowanceamount, accountcode, attendeesapp, addasnet, pdcatid, eventinhomeapp, receipt, calculation, passengersapp, nopassengersapp, passengernamesapp, comment, splitentertainment, entertainmentid, reimbursable, nonights, attendeesmand, nodirectorsapp, hotelapp, noroomsapp, hotelmand, vatnumber, vatnumbermand, nopersonalguests, noremoteworkers, alternateaccountcode, splitpersonal, splitremote, personalid, remoteid, reasonapp, otherdetails, userdefined, oldsubcat.createdon, oldsubcat.createdby, DateTime.Now.ToUniversalTime(), user.EmployeeID, shortsubcat, fromapp, toapp, countries, allowances, associatedudfs, oldsubcat.subcatsplit, companyapp, oldsubcat.vatrates, enableHomeToLocationMileage, hometolocationtype, mileageCategory, isRelocationMileage, reimbursableSubcatID, allowHeavyBulkyMileage, homeToOfficeAsZero, homeToOfficeFixedMiles, publicTransportRate, startDate, endDate, validate, criteria,enableDoc,requireClass1BusinessInsurance, enforceMileageCap, homeToOfficeMileageCap);
            }
            else
            {
                clssubcat = new cSubcat(subcatid, categoryid, subcat, description, mileageapp, staffapp, othersapp, tipapp, pmilesapp, bmilesapp, allowanceamount, accountcode, attendeesapp, addasnet, pdcatid, eventinhomeapp, receipt, calculation, passengersapp, nopassengersapp, passengernamesapp, comment, splitentertainment, entertainmentid, reimbursable, nonights, attendeesmand, nodirectorsapp, hotelapp, noroomsapp, hotelmand, vatnumber, vatnumbermand, nopersonalguests, noremoteworkers, alternateaccountcode, splitpersonal, splitremote, personalid, remoteid, reasonapp, otherdetails, userdefined, DateTime.Now.ToUniversalTime(), user.EmployeeID, null, 0, shortsubcat, fromapp, toapp, countries, allowances, associatedudfs, new List<int>(), companyapp, new List<cSubcatVatRate>(), enableHomeToLocationMileage, hometolocationtype, mileageCategory, isRelocationMileage, reimbursableSubcatID, allowHeavyBulkyMileage, homeToOfficeAsZero, homeToOfficeFixedMiles, publicTransportRate, startDate, endDate, validate, criteria,enableDoc,requireClass1BusinessInsurance, enforceMileageCap, homeToOfficeMileageCap);
            }

            return clssubcats.SaveSubcat(clssubcat);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] saveSplitItems(int subcatid, List<int> splititems)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var clssubcats = new cSubcats(user.AccountID);
            return clssubcats.AddSplit(subcatid, splititems);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public bool deleteSplitItem(int subcatID, int subcatSplitID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cSubcats clsSubCats = new cSubcats(user.AccountID);
            clsSubCats.DeleteSplit(subcatID, subcatSplitID);
            return true;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public cRoleSubcat getRole(int subcatid, int roleid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cItemRoles clsroles = new cItemRoles(user.AccountID);
            cItemRole role = clsroles.getItemRoleById(roleid);
            cRoleSubcat rolesub = role.items[subcatid];
            return rolesub;

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public bool saveRole(int subcatid, int roleid, bool addtotemplate, decimal receiptmaximum, decimal maximum)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cItemRoles clsroles = new cItemRoles(user.AccountID);

            cRoleSubcat rolesub = new cRoleSubcat(0, roleid, subcatid, maximum, receiptmaximum, addtotemplate);
            clsroles.saveRoleSubcat(rolesub);
            return true;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public bool deleteRole(int subcatid, int roleid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cItemRoles clsroles = new cItemRoles(user.AccountID);
            clsroles.deleteRoleSubcat(subcatid, roleid);
            return true;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] createVATGrid(string contextKey)
        {
            int subcatid = 0;
            if (contextKey != "")
            {
                subcatid = Convert.ToInt32(contextKey);
            }
            CurrentUser user = cMisc.GetCurrentUser();
            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridVATRates", "select vatrateid, daterangetype, datevalue1, datevalue2, vatamount, vatpercent from subcat_vat_rates");
            clsgrid.KeyField = "vatrateid";
            clsgrid.enableupdating = true;
            clsgrid.enabledeleting = true;
            clsgrid.getColumnByName("vatrateid").hidden = true;
            clsgrid.editlink = "javascript:editVatRange({vatrateid})";
            clsgrid.deletelink = "javascript:deleteVatRange({vatrateid})";
            clsgrid.EmptyText = "There are no VAT Rates defined for this expense item";
            ((cFieldColumn)clsgrid.getColumnByName("daterangetype")).addValueListItem(1, "After or Equal To");
            ((cFieldColumn)clsgrid.getColumnByName("daterangetype")).addValueListItem(3, "Any");
            ((cFieldColumn)clsgrid.getColumnByName("daterangetype")).addValueListItem(0, "Before");
            ((cFieldColumn)clsgrid.getColumnByName("daterangetype")).addValueListItem(2, "Between");
            cFields clsfields = new cFields(user.AccountID);
            clsgrid.addFilter(clsfields.GetFieldByID(new Guid("c25ed78e-a8d3-4c74-9c37-f1f2c1e4bc7e")), ConditionType.Equals, new object[] { subcatid }, null, ConditionJoiner.None);

            List<string> retVals = new List<string>();
            retVals.Add(clsgrid.GridID);
            retVals.AddRange(clsgrid.generateGrid());
            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] createSplitItemGrid(string contextKey)
        {

            CurrentUser user = cMisc.GetCurrentUser();
            int subcatid = 0;
            if (contextKey != "")
            {
                subcatid = Convert.ToInt32(contextKey);
            }

            cTables clstables = new cTables(user.AccountID);
            cFields clsfields = new cFields(user.AccountID);
            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("d4ed76bd-605c-45ce-b075-4c6018a50b08"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("44c2a53a-db33-45af-af66-d0055aad48ec"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("abfe0bb2-e6ac-40d0-88ce-c5f7b043924d"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("379b67dd-654e-43cd-b55d-b9b5262eeeee"))));

            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridSplit", clstables.GetTableByID(new Guid("9fe1aa3f-8e0e-4ca6-a31e-86e4c5484a4a")), columns);
            clsgrid.addFilter(clsfields.GetFieldByID(new Guid("54d4f7c2-be2f-458a-a57c-c030f2197316")), ConditionType.Equals, new object[] { subcatid }, null, ConditionJoiner.None);
            clsgrid.getColumnByName("subcatid").hidden = true;
            clsgrid.enabledeleting = true;
            clsgrid.EmptyText = "There are currently no Split Expense Items defined for this item.";
            clsgrid.deletelink = "javascript:deleteSplitItem({subcatid})";
            clsgrid.KeyField = "subcatid";

            List<string> retVals = new List<string>();
            retVals.Add(clsgrid.GridID);
            retVals.AddRange(clsgrid.generateGrid());
            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] createRoleGrid(string contextKey)
        {
            int subcatid = 0;
            if (contextKey != "")
            {
                subcatid = Convert.ToInt32(contextKey);
            }
            CurrentUser user = cMisc.GetCurrentUser();
            cTables clstables = new cTables(user.AccountID);
            cFields clsfields = new cFields(user.AccountID);
            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("f3016e05-1832-49d1-9d33-79ed893b4366"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("54825039-9125-4705-b2d4-eb340d1d30de"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("a3ba1781-2c99-48dc-bc8c-126ef90ca55b"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("10a310fa-d34a-4568-b573-07a91f9aa765"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("d2702ae8-609a-45ab-bf01-c58210ef1720"))));
            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridRoles", clstables.GetTableByID(new Guid("db7d42fd-e1fa-4a42-84b4-e8b95c751bda")), columns);
            clsgrid.addFilter(clsfields.GetFieldByID(new Guid("6eb08c2f-6fb5-49d0-b2b1-eb4edaa586f3")), ConditionType.Equals, new object[] { subcatid }, null, ConditionJoiner.None);
            clsgrid.getColumnByName("itemroleid").hidden = true;
            clsgrid.EmptyText = "There are no item roles associated with this expense item";
            clsgrid.enabledeleting = true;
            clsgrid.enableupdating = true;
            clsgrid.editlink = "javascript:editRole({itemroleid});";
            clsgrid.deletelink = "javascript:deleteRole({itemroleid});";
            clsgrid.KeyField = "itemroleid";

            List<string> retVals = new List<string>();
            retVals.Add(clsgrid.GridID);
            retVals.AddRange(clsgrid.generateGrid());
            return retVals.ToArray();
        }

        /// <summary>
        ///  GetDoCGeneralOption method checks , if any of the DOC option is enabled for the account.  
        /// </summary>
        /// <returns>integer value based on doc setting . 
        /// 0-None of Doc option , 1-Any one of the Doc , 2-Insurance document expiry is enabled
        /// </returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int GetDoCGeneralOption(int accountid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            int setDoCClass1Options = 0;
            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(user.AccountID);
            cAccountProperties clsProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties;
            var isDocEnabled = clsProperties.BlockTaxExpiry ||clsProperties.BlockMOTExpiry|| clsProperties.BlockInsuranceExpiry || clsProperties.BlockDrivingLicence || clsProperties.BlockBreakdownCoverExpiry;
            setDoCClass1Options = Convert.ToInt32(isDocEnabled);
            if (isDocEnabled && clsProperties.BlockInsuranceExpiry)
            {
                setDoCClass1Options = 2;
            }
            return setDoCClass1Options;
        }
    }
}
