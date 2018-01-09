namespace Spend_Management
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web.Script.Services;
    using System.Web.Services;
    using System.Web.UI.WebControls;
    using System.Text.RegularExpressions;
    using System.Web;
    using Spend_Management.shared.webServices;
    using System.Runtime.InteropServices;
    using System.Web.Script.Serialization;
    using SpendManagementLibrary.Hotels;
    using SpendManagementLibrary.Selectinator;
    using AjaxControlToolkit;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Definitions.JoinVia;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Helpers;
    using shared.code;

    /// <summary>
    /// Summary description for auto_complete
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class svcAutoComplete : WebService
    {
        private string strsql;

        /// <summary>
        /// Returns an object array with elementIDs and a string array of search matches.
        /// </summary>
        /// <param name="searchText">The search text you want to find</param>
        /// <param name="searchFieldID">The GUID of the field you want to search</param>
        /// <param name="searchTextElementID">The elementID of the text field the user is typing in</param>
        /// <param name="popupPanelElementID">The elementID of the popupPanel being used to display results</param>
        /// <param name="searchType">The type of search you want to perform</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] AutoComplete(string searchText, Guid searchFieldID, string searchTextElementID, string popupPanelElementID, ConditionType searchType)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            object[] returnObjects = new object[4];
            returnObjects[0] = searchTextElementID;
            returnObjects[1] = popupPanelElementID;
            returnObjects[3] = searchFieldID;

            cFields clsFields = new cFields(currentUser.AccountID);

            returnObjects[2] = clsFields.SearchFieldByFieldID(searchFieldID, searchType, searchText, 10);

            return returnObjects;
        }

        [WebMethod(EnableSession = true)]
        public List<string> getCountryList(string prefixText, int count)
        {
            DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            List<string> items = new List<string>();
            string strsql = "select top 10 country from global_countries where country like @country order by country";
            expdata.sqlexecute.Parameters.AddWithValue("@country", prefixText + "%");

            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    items.Add(reader.GetString(0));
                }

                reader.Close();
            }

            return items;
        }

        [WebMethod(EnableSession = true)]
        public List<string> getEmployeeUsername(string prefixText, int count)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(user.AccountID));
            List<string> items = new List<string>();
            strsql = "select top 10 username from employees where username like @username order by username";
            expdata.sqlexecute.Parameters.AddWithValue("@username", prefixText + "%");

            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    items.Add(reader.GetString(0));
                }

                reader.Close();
            }

            return items;

        }

        [WebMethod(EnableSession = true)]
        public List<string> getEmployee(string prefixText, int count)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(user.AccountID));
            List<string> items = new List<string>();
            strsql = "select top 10 surname, firstname from employees where surname like @surname order by surname";
            expdata.sqlexecute.Parameters.AddWithValue("@surname", prefixText + "%");

            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    items.Add(reader.GetString(0) + ", " + reader.GetString(1));
                }

                reader.Close();
            }

            return items;

        }

        private string CapitalizeFirstLetterOfWords(string s)
        {
            return Regex.Replace(s, @"\b[a-z]", m => m.Value.ToUpper());
        }

        /// <summary>
        /// Searches employees based on the supplied crieria
        /// </summary>
        /// <param name="q">
        /// The search string
        /// </param>
        /// <returns>
        /// A list of <see cref="TokenInputResult">TokenInputResult</see>
        /// </returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<TokenInputResult> searchEmployees(string q)
        {
            var filters = new Dictionary<string, JSFieldFilter>
            {
                {
                    "0",
                    new JSFieldFilter
                    {
                        ConditionType = ConditionType.NotLike,
                        FieldID = new Guid("1C45B860-DDAA-47DA-9EEC-981F59CCE795"),
                        ValueOne = "admin%"
                    }
                }
            };

            var user = cMisc.GetCurrentUser();
            var employees = new cEmployees(user.AccountID);

            var tokenInputResults = employees.SearchEmployees(filters, q, user);

            return tokenInputResults;
        }

        /// <summary>
        /// Returns the employee email address and name based on their username or full name
        /// </summary>
        /// <param name="q">The JSON object that contains the search data</param>
        /// <returns>A list of <see cref="TokenInputResult"/></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<TokenInputResult> SearchEmployeeEmailByNameAndUsername(string q)
        {
            List<TokenInputResult> tokenInputResults = new List<TokenInputResult>();
            if (User.Identity.IsAuthenticated)
            {
                CurrentUser user = cMisc.GetCurrentUser();

                using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
                {
                    const string sql = "select top 10 employeeid, surname, firstname, username, email from employees where username not like 'admin%' and (firstname + ' ' + surname like @prefixText or surname + ' ' + firstname like @prefixText or email like @prefixtext or username like @prefixtext) and ISNULL(email, '') > '' order by surname";
                    expdata.sqlexecute.Parameters.AddWithValue("@prefixText", "%" + q + "%");

                    using (var reader = expdata.GetReader(sql))
                    {
                        expdata.sqlexecute.Parameters.Clear();

                        var surnameOrdinal = reader.GetOrdinal("surname");
                        var forenameOrdinal = reader.GetOrdinal("firstname");
                        var usernameOrdinal = reader.GetOrdinal("username");
                        var emailOrdinal = reader.GetOrdinal("email");

                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(emailOrdinal))
                            {
                                var email = reader.GetString(emailOrdinal);
                                var firstName = reader.GetString(forenameOrdinal);
                                var surname = reader.GetString(surnameOrdinal);
                                var username = reader.GetString(usernameOrdinal);
                                tokenInputResults.Add(new TokenInputResult
                                {
                                    id = email,
                                    name = string.Format("{0} {1} [{2}]", firstName, surname, username),
                                    searchDisplay = string.Format("{0} {1} [{2}]", firstName, surname, username)
                                });
                            }
                        }

                        reader.Close();
                    }
                }
            }

            if (Regex.IsMatch(q, @"^(?:(?:[\w`~!#$%^&*\-=+;:{}'|,?\/]+(?:(?:\.(?:""(?:\\?[\w`~!#$%^&*\-=+;:{}'|,?\/\.()<>\[\] @]|\\""|\\\\)*""|[\w`~!#$%^&*\-=+;:{}'|,?\/]+))*\.[\w`~!#$%^&*\-=+;:{}'|,?\/]+)?)|(?:""(?:\\?[\w`~!#$%^&*\-=+;:{}'|,?\/\.()<>\[\] @]|\\""|\\\\)+""))@(?:[a-zA-Z\d\-]+(?:\.[a-zA-Z\d\-]+)*|\[\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\])$"))
            {
                const string cantFindText = "Add this email address?";
                tokenInputResults.Add(new TokenInputResult
                {
                    id = "-1",
                    name = cantFindText,
                    searchDisplay = cantFindText
                });
            }

            return tokenInputResults;
        }

        [WebMethod(EnableSession = true)]
        public List<string> getEmployeeNameAndUsername(string prefixText, int count)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(user.AccountID));
            List<string> items = new List<string>();
            strsql = "select top 10 surname, firstname, username from employees where username not like 'admin%' and (firstname + ' ' + surname like @prefixText or surname + ' ' + firstname like @prefixText) order by surname";
            expdata.sqlexecute.Parameters.AddWithValue("@prefixText", "%" + prefixText + "%");

            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    items.Add(reader.GetString(reader.GetOrdinal("surname")) + ", " + reader.GetString(reader.GetOrdinal("firstname")) + " [" + reader.GetString(reader.GetOrdinal("username")) + "]");
                }

                reader.Close();
            }

            return items;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] checkEmployee(string strEmpVal, byte empAreaType)
        {
            object[] arrEmpDet = new object[2];
            CurrentUser user = cMisc.GetCurrentUser();
            cEmployees clsEmps = new cEmployees(user.AccountID);

            int startIndex = strEmpVal.IndexOf('[');
            int endIndex = strEmpVal.IndexOf(']');
            int strLength = endIndex - startIndex;

            if (startIndex != -1)
            {
                string username = strEmpVal.Substring(startIndex + 1, strLength - 1);
                arrEmpDet[0] = clsEmps.getEmployeeidByUsername(user.AccountID, username);
            }
            else
            {
                arrEmpDet[0] = 0;
            }

            arrEmpDet[1] = empAreaType;

            return arrEmpDet;
        }

        [WebMethod(EnableSession = true)]
        public List<string> getParentCompanyList(string prefixText, int count)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(user.AccountID));
            List<string> items = new List<string>();

            strsql = "select top 10 company from companies where archived = 0 and isCompany = 1 and company like @company order by company";
            expdata.sqlexecute.Parameters.AddWithValue("@company", prefixText + "%");

            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    if ((reader.GetString(0) == "Home" && items.Contains("Home")) || (reader.GetString(0) == "Office" && items.Contains("Office")))
                    {

                    }
                    else
                    {
                        items.Add(reader.GetString(0));
                    }
                }

                reader.Close();
            }

            return items;
        }

        [WebMethod(EnableSession = true)]
        public List<string> getDepartmentList(string prefixText, int count)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(user.AccountID));
            List<string> items = new List<string>();
            strsql = "select top 10 department from departments where department like @department AND archived = 0 order by department";
            expdata.sqlexecute.Parameters.AddWithValue("@department", prefixText + "%");

            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    items.Add(reader.GetString(0));
                }

                reader.Close();
            }

            return items;
        }

        [WebMethod(EnableSession = true)]
        public List<string> getDepartmentListByDescription(string prefixText, int count)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(user.AccountID));
            List<string> items = new List<string>();
            strsql = "select top 10 description from departments where description like @department order by description";
            expdata.sqlexecute.Parameters.AddWithValue("@department", prefixText + "%");

            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    items.Add(reader.GetString(0));
                }

                reader.Close();
            }

            return items;
        }

        [WebMethod(EnableSession = true)]
        public List<string> getCostcodeList(string prefixText, int count)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(user.AccountID));
            List<string> items = new List<string>();
            strsql = "select top 10 costcode from costcodes where costcode like @costcode AND archived = 0 order by costcode";
            expdata.sqlexecute.Parameters.AddWithValue("@costcode", prefixText + "%");

            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    items.Add(reader.GetString(0));
                }

                reader.Close();
            }

            return items;
        }

        [WebMethod(EnableSession = true)]
        public List<string> getCostcodeListByDescription(string prefixText, int count)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(user.AccountID));
            List<string> items = new List<string>();
            strsql = "select top 10 description from costcodes where description like @costcode order by description";
            expdata.sqlexecute.Parameters.AddWithValue("@costcode", prefixText + "%");

            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    items.Add(reader.GetString(0));
                }

                reader.Close();
            }

            return items;
        }

        [WebMethod(EnableSession = true)]
        public List<string> getProjectcodeList(string prefixText, int count)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(user.AccountID));
            List<string> items = new List<string>();
            strsql = "select top 10 projectcode from project_codes where projectcode like @projectcode AND archived = 0 order by projectcode";
            expdata.sqlexecute.Parameters.AddWithValue("@projectcode", prefixText + "%");

            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    items.Add(reader.GetString(0));
                }

                reader.Close();
            }

            return items;
        }

        [WebMethod(EnableSession = true)]
        public List<string> getReasonList(string prefixText, int count)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(user.AccountID));
            List<string> items = new List<string>();
            strsql = "select top 10 reason from reasons where reason like @reason order by reason";

            expdata.sqlexecute.Parameters.AddWithValue("@reason", prefixText + "%");

            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    items.Add(reader.GetString(0));
                }

                reader.Close();
            }

            return items;
        }

        [WebMethod(EnableSession = true)]
        public List<string> getProjectcodeListByDescription(string prefixText, int count)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(user.AccountID));
            List<string> items = new List<string>();
            strsql = "select top 10 description from project_codes where description like @projectcode order by description";
            expdata.sqlexecute.Parameters.AddWithValue("@projectcode", prefixText + "%");

            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    items.Add(reader.GetString(0));
                }

                reader.Close();
            }

            return items;
        }

        /// <summary>
        /// A method to get hotel names for the hotel lookup control
        /// </summary>
        /// <param name="prefixText">The Hotel Name</param>
        /// <returns>A list of hotel names</returns>
        [WebMethod(EnableSession = true)]
        public List<string> GetHotelList(string prefixText)
        {
            var hotels = new Hotels();
            IEnumerable<Hotel> hotelSearchResults = hotels.GetHotelsByName(prefixText);
            List<string> items = new List<string>();

            foreach (var hotel in hotelSearchResults)
            {
                items.Add(hotel.hotelname);
            }

            return items;
        }

        private List<CascadingDropDownNameValue> getCategoryList(IEnumerable<SubcatItemRoleBasic> roleitems)
        {
            var cats = new List<CascadingDropDownNameValue>();
            CurrentUser user = cMisc.GetCurrentUser();
            var clscategories = new cCategories(user.AccountID);
            foreach (SubcatItemRoleBasic rolesub in roleitems)
            {
                cCategory category = clscategories.FindById(rolesub.CategoryId);
                if (cats.Contains(new CascadingDropDownNameValue(category.category, category.categoryid.ToString(CultureInfo.InvariantCulture))) == false)
                {
                    cats.Add(new CascadingDropDownNameValue(category.category, category.categoryid.ToString(CultureInfo.InvariantCulture)));
                }
            }

            return cats;
        }

        private List<CascadingDropDownNameValue> getSubcatList(int categoryid, IEnumerable<SubcatItemRoleBasic> roleitems)
        {
            var sorted = roleitems.Where(rolesub => rolesub.CategoryId == categoryid).ToList();

            return sorted.OrderBy(x => x.Subcat).Select(rolesub => new CascadingDropDownNameValue(rolesub.Subcat, rolesub.SubcatId.ToString())).ToList();
            }

        [WebMethod(EnableSession = true)]
        public SelectOptionValue[] getMileageCategoriesByCarId(int carid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cEmployeeCars employeeCars = new cEmployeeCars(user.AccountID, user.EmployeeID);
            cMileagecats mileageCategories = new cMileagecats(user.AccountID);

            List<cMileageCat> mileageCategorgies = mileageCategories.GetByVehicleId(carid, user, employeeCars);
            List<SelectOptionValue> selectOptionValues = new List<SelectOptionValue>();

            foreach (var mileageCategory in mileageCategorgies)
            {
                selectOptionValues.Add(new SelectOptionValue { id = mileageCategory.mileageid, name = mileageCategory.carsize });
            }

            SelectOptionValue[] selectedOptions = selectOptionValues.ToArray();

            return selectedOptions;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] getContacts(string prefixText, int count, string contextKey)
        {
            List<string> retStr = new List<string>();

            CurrentUser curUser = cMisc.GetCurrentUser();
            DBConnection db = new DBConnection(cAccounts.getConnectionString(curUser.AccountID));

            string strsql = "SELECT contactid, contactname FROM dbo.supplier_contacts WHERE contactname LIKE @name AND supplierid = @supplierid ORDER BY contactname";
            db.sqlexecute.CommandText = strsql;
            db.sqlexecute.Parameters.AddWithValue("@supplierid", int.Parse(contextKey));
            db.sqlexecute.Parameters.AddWithValue("@name", "%" + prefixText + "%");

            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strsql))
            {
                while (reader.Read())
                {
                    retStr.Add(reader.GetString(reader.GetOrdinal("contactname")));
                }

                reader.Close();
            }

            return retStr.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] GetStringMatches(string prefixText, int count, string contextKey)
        {
            List<string> lstMatches = new List<string>();

            CurrentUser user = cMisc.GetCurrentUser();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(user.AccountID));
            cTables clsTables = new cTables(user.AccountID);
            Guid tableID = new Guid(contextKey);
            cTable reqTable = clsTables.GetTableByID(tableID);
            cField reqKeyField = reqTable.GetKeyField();


            expdata.sqlexecute.Parameters.AddWithValue("@likeParam", "%" + prefixText + "%");

            string strSQL = "SELECT [" + reqKeyField.FieldName + "] FROM [" + reqTable.TableName + "] WHERE [" + reqKeyField.FieldName + "] LIKE @likeParam";
            string stringkeyfield;

            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strSQL))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                    {
                        stringkeyfield = reader.GetString(0);
                        lstMatches.Add(stringkeyfield);
                    }
                }

                reader.Close();
            }

            return lstMatches.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] getSuppliers(string prefixText, int count)
        {
            List<string> retStr = new List<string>();

            CurrentUser curUser = cMisc.GetCurrentUser();
            DBConnection db = new DBConnection(cAccounts.getConnectionString(curUser.AccountID));

            string strsql = "SELECT supplierid, suppliername FROM dbo.supplier_details WHERE suppliername LIKE @supplier ORDER BY suppliername";
            db.sqlexecute.CommandText = strsql;
            db.sqlexecute.Parameters.AddWithValue("@supplier", "%" + prefixText + "%");

            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strsql))
            {
                while (reader.Read())
                {
                    retStr.Add(reader.GetString(reader.GetOrdinal("suppliername")));
                }

                reader.Close();
            }

            return retStr.ToArray();
        }

        /// <summary>
        /// Returns ID and display for for auto complete extender (jQuery) to for dynamic data sources
        /// </summary>
        /// <param name="maxRows">
        /// Number of records to return
        /// </param>
        /// <param name="matchTable">
        /// Table ID (Guid) to perform the look up on
        /// </param>
        /// <param name="displayField">
        /// Text field to display in the returned list
        /// </param>
        /// <param name="matchFields">
        /// Comma separated list of Field IDs (Guids) to perform wildcard match on 
        /// </param>
        /// <param name="matchText">
        /// Text to match
        /// </param>
        /// <param name="useWildcards">
        /// Specify if the search should use wildcards or return specific values
        /// </param>
        /// <param name="filters">
        /// Specifiy the field filters to use with the Auto Complete
        /// </param>
        /// <param name="keyIsString">
        /// The key Is String.
        /// </param>
        /// <param name="childFilterList">
        /// The child Filter List.
        /// </param>
        /// <returns>
        /// Serializable structure of type sAutoComplete
        /// </returns>
        [WebMethod(EnableSession = true)]
        public List<sAutoCompleteResult> getAutoCompleteOptions(int maxRows, string matchTable, string displayField, string matchFields, string matchText, bool useWildcards, Dictionary<string, JSFieldFilter> filters, bool keyIsString, List<AutoCompleteChildFieldValues> childFilterList)
        {
            return SpendManagementLibrary.AutoComplete.GetAutoCompleteMatches(cMisc.GetCurrentUser(), maxRows, matchTable, displayField, matchFields, matchText, useWildcards, filters, keyIsString, childFilterList);
        }


        /// <summary>
        /// Returns ID and display for for auto complete extender (jQuery) to for dynamic data sources
        /// </summary>
        /// <param name="maxRows">
        /// Number of records to return
        /// </param>
        /// <param name="matchTable">
        /// Table ID (Guid) to perform the look up on
        /// </param>
        /// <param name="displayField">
        /// Text field to display in the returned list
        /// </param>
        /// <param name="matchFields">
        /// Comma separated list of Field IDs (Guids) to perform wildcard match on 
        /// </param>
        /// <param name="autoCompleteFields">
        /// Comma separated list of Field IDs (Guids) to display on autocimplete search results
        /// </param>
        /// <param name="matchText">
        /// Text to match
        /// </param>
        /// <param name="useWildcards">
        /// Specify if the search should use wildcards or return specific values
        /// </param>
        /// <param name="filters">
        /// Specifiy the field filters to use with the Auto Complete
        /// </param>
        /// <param name="keyIsString">
        /// The key Is String.
        /// </param>
        /// <param name="childFilterList">
        /// The child Filter List.
        /// </param>
        /// <returns>
        /// Serializable structure of type sAutoComplete
        /// </returns>
        [WebMethod(EnableSession = true)]
        public List<CustomEntityAutoCompleteResult> GetGreenlightAutoCompleteOptions(int maxRows, string matchTable, string displayField, string matchFields, string autoCompleteFields, string matchText, bool useWildcards, Dictionary<string, JSFieldFilter> filters, bool keyIsString, List<AutoCompleteChildFieldValues> childFilterList)
        {
            return SpendManagementLibrary.AutoComplete.GetAutoCompleteMatches(cMisc.GetCurrentUser(), maxRows, matchTable, displayField, matchFields, autoCompleteFields, matchText, useWildcards, filters, keyIsString, childFilterList);
        }


        /// <summary>
        /// Gets properties for population on the CE and UDF admin screens when adding a N:1 relationship field type
        /// </summary>
        /// <param name="relatedTable_ID">The TableID (Guid) of the foreign table</param>
        /// <param name="excludeList">IDs of any Tables that are to be excluded from the list</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public List<ListItem> getRelationshipLookupOptions(string relatedTable_ID, string[] excludeList)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities entities = new cCustomEntities(currentUser);
            List<ListItem> items = new List<ListItem>();
            Guid relTableID = Guid.Empty;
            List<Guid> exclusions = (from x in excludeList select new Guid(x)).ToList();

            if (Guid.TryParseExact(relatedTable_ID, "D", out relTableID))
            {
                items = entities.GetTablesFieldListItemsForManyToOneRelationship(relTableID, exclusions);
            }

            return items;
        }

        /// <summary>
        /// Get list of fields from the n:1 attribute table that can be selected in to set as display fields
        /// </summary>
        /// <param name="relatedTable_ID">The TableID (Guid) of the foreign table</param>
        /// <param name="excludeList">IDs of any Tables that are to be excluded from the list</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public List<ListItem> GetAutocompleteSearchResultsFields(string relatedTable_ID, string[] excludeList)
        {
            var currentUser = cMisc.GetCurrentUser();
            var entities = new cCustomEntities(currentUser);
            var items = new List<ListItem>();
            var relTableID = Guid.Empty;
            var fields = new cFields(currentUser.AccountID);
            excludeList = excludeList.Where(fieldName => !string.IsNullOrEmpty(fieldName)).ToArray();
            if (Guid.TryParseExact(relatedTable_ID, "D", out relTableID))
            {
                var exclusions = (from autocompleteDisplay in excludeList select fields.GetFieldByTableAndDescription(relTableID, autocompleteDisplay) into autolookupDisplayField where autolookupDisplayField != null select autolookupDisplayField.FieldID).ToList();
                items = entities.GetTablesFieldListItemsForAutoCompleteDisplay(relTableID, exclusions);
            }

            return items;
        }

        /// <summary>
        /// Populates the values for a list of trigger fields
        /// </summary>
        /// <param name="matchTableId">
        /// The match Table guid
        /// </param>
        /// <param name="matchId">
        /// The match Id.
        /// </param>
        /// <param name="triggerFields">
        /// The list of trigger fields related to the autocomplete field
        /// </param>
        /// <returns>
        /// The list of trigger fields with their values populated for the selected match in the autocomplete
        /// </returns>
        [WebMethod(EnableSession = true)]
        public List<JsAutoCompleteTriggerField> GetTriggerFieldValues(string matchTableId, int matchId, List<JsAutoCompleteTriggerField> triggerFields)
        {
            Guid matchTableGuid;
            return Guid.TryParseExact(matchTableId, "D", out matchTableGuid) ? SpendManagementLibrary.AutoComplete.GetTriggerFieldValues(cMisc.GetCurrentUser(), matchTableGuid, matchId, triggerFields) : new List<JsAutoCompleteTriggerField>();
        }

        /// <summary>
        /// This get the Readonly lookup field values associated with the child , based on parent control id. On loading greenlight form, the readonly lookup field should be fetched for parent and associated child
        /// </summary>
        /// <param name="parentControlId">
        /// The parent control id for which the child field associated.
        /// </param>
        /// <param name="parentControls">
        /// The list of all the parent control Ids with their values.
        /// </param>
        /// <param name="childrenControls">
        /// The list of all the child control Ids.
        /// </param>
        /// <param name="formId">
        /// Id of the form where the parent and child fields exists.
        /// </param>
        /// <param name="entityId">
        /// Id of the greenlight for which the has parent child filter set.
        /// </param>
        /// <returns>
        /// List of readonly trigger field values associated with child.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public List<AutoCompleteChildFieldValues> GetTriggerFieldParentValues(string parentControlId, List<SelectinatorParentFilter> parentControls, List<string> childrenControls, int formId, int entityId)
        {
            var childElementData = new List<AutoCompleteChildFieldValues>();
            var currentUser = cMisc.GetCurrentUser();
            var clsentities = new cCustomEntities(currentUser);
            var entity = clsentities.getEntityById(entityId);
            var attributesToFilterBy = new List<cManyToOneRelationship>();

            foreach (var parent in parentControls)
            {
                attributesToFilterBy.Add((cManyToOneRelationship)entity.getAttributeById(Convert.ToInt32(parent.Id)));
            }

            foreach (var child in childrenControls)
            {
                var attribute = (cManyToOneRelationship)entity.getAttributeById(Convert.ToInt32(child));

                var filters = attribute.filters.Where(e => e.Value.IsParentFilter == false);
                Dictionary<string, JSFieldFilter> filterDictionary =
                    filters.ToDictionary(
                        x => x.Key.ToString(CultureInfo.InvariantCulture),
                        x => new JSFieldFilter
                        {
                            ConditionType = x.Value.Conditiontype,
                            FieldID = x.Value.Field.FieldID,
                            Order = x.Value.Order,
                            ValueOne = x.Value.ValueOne.ToString(CultureInfo.InvariantCulture)
                        });

                var index = 0;

                var parentFilters = attribute.filters.Where(e => e.Value.IsParentFilter == true);

                foreach (var parentFilter in parentFilters)
                {
                    var filterBy = attributesToFilterBy.FirstOrDefault(a => a.attributeid.ToString() == parentFilter.Value.ValueOne);
                    filterDictionary.Add(index.ToString(), new JSFieldFilter { ConditionType = ConditionType.Equals, FieldID = filterBy.relatedtable.PrimaryKeyID, ValueOne = parentControls.FirstOrDefault(a => a.Id == filterBy.attributeid.ToString(CultureInfo.InvariantCulture)).Value, Joiner = ConditionJoiner.And });
                    index++;
                }

                if (attribute.AutoCompleteDisplayFieldIDList.Count > 0)
                {
                    var autoDisplayResultList = SpendManagementLibrary.AutoComplete.GetAutoCompleteMatches(currentUser, 26, attribute.relatedtable.TableID.ToString(), attribute.AutoCompleteDisplayField.ToString(), string.Join(",", attribute.AutoCompleteMatchFieldIDList), string.Join(",", attribute.AutoCompleteDisplayFieldIDList), string.Empty, true, filterDictionary, false, null);
                    childElementData.AddRange(autoDisplayResultList.Select(result => new AutoCompleteChildFieldValues { FieldToBuild = child, Key = Convert.ToInt32(result.value), Value = result.label, FormattedText = result.formattedText }));
                }
                else
                {
                    var resultList = SpendManagementLibrary.AutoComplete.GetAutoCompleteMatches(currentUser, 26, attribute.relatedtable.TableID.ToString(), attribute.AutoCompleteDisplayField.ToString(), string.Join(",", attribute.AutoCompleteMatchFieldIDList), string.Empty, true, filterDictionary);
                    childElementData.AddRange(resultList.Select(result => new AutoCompleteChildFieldValues { FieldToBuild = child, Key = Convert.ToInt32(result.value), Value = result.label }));
                }

                if (childElementData.Count == 0)
                    childElementData.Add(
                        new AutoCompleteChildFieldValues { FieldToBuild = child, Key = 0, Value = null });
            }

            return childElementData.Count > 0 ? childElementData : null;
        }

        /// <summary>
        /// Line Manager search grid, should be refactored when a combined autocomplete/select control is made
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string[] GetLineManagerSearchGrid()
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Employees, true))
            {
                return this.GetAutoCompleteSearchGrid(currentUser, "gridLineManagerSearch");
            }

            return new string[0];
        }

        /// <summary>
        /// Pool Car search grid, should be refactored when a combined autocomplete/select control is made
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string[] GetPoolCarSearchGrid()
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Employees, true))
            {
                return this.GetAutoCompleteSearchGrid(currentUser, "gridPoolCarSearch");
            }

            return new string[0];
        }

        /// <summary>
        /// Claimant search grid, should be refactored when a combined autocomplete/select control is made
        /// </summary>
        /// <param name="conditionType">
        /// The condition Type AtMyClaimsHierarchy / WithAccessRoles or CompleteHierarchy
        /// </param>
        /// <returns>
        /// cGridNew object data.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public string[] GetClaimantSearchGrid(ConditionType conditionType = ConditionType.CompleteHierarchy)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ClaimViewer, true))
            {
                return this.GetAutoCompleteSearchGrid(currentUser, "gridClaimantSearch", conditionType);
            }

            return new string[0];
        }

        /// <summary>
        /// Organisation search grid, should be refactored when a combined autocomplete/select control is made
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string[] GetOrganisationSearchGrid()
        {
            return this.GetAutoCompleteSearchGrid(cMisc.GetCurrentUser(), "gridOrganisationSearch");
        }

        /// <summary>
        /// Department search grid, should be refactored when a combined autocomplete/select control is made
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string[] GetAutoCompleteComboSearchGrid(string typeId)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            switch (typeId)
            {
                case "87D021DA-EAB8-40F7-8C70-CF5ADCE9486C":
                    return this.GetAutoCompleteSearchGrid(currentUser, "gridCostCentreDepartmentSearch");
                case "BC95890F-47D4-4FEC-A6AF-BBEEC4470497":
                    return this.GetAutoCompleteSearchGrid(currentUser, "gridCostCentreDepartmentDescriptionSearch");
                case "E1F85384-D8E7-4BDC-BA27-339B59BEDB85":
                    return this.GetAutoCompleteSearchGrid(currentUser, "gridCostCentreCostCodeSearch");
                case "D3F54727-45FB-4D82-B400-C37BFD8E1E73":
                    return this.GetAutoCompleteSearchGrid(currentUser, "gridCostCentreCostCodeDescriptionSearch");
                case "C944F362-E745-4A31-B626-FB360DE9B908":
                    return this.GetAutoCompleteSearchGrid(currentUser, "gridCostCentreProjectCodeSearch");
                case "F4FE7871-8043-4020-9150-B21BDE238F94":
                    return this.GetAutoCompleteSearchGrid(currentUser, "gridCostCentreProjectCodeDescriptionSearch");
                default:
                    return new string[0];
            }
        }

        // this is here because the stupid autocomplete class is in the stupid library and the stupid cGridStupidNew isn't

        /// <summary>
        /// Returns the grid for the appropriate search type related to an autocomplete
        /// </summary>
        /// <param name="currentUser">
        /// The currentuser object
        /// </param>
        /// <param name="gridName">
        /// The cGridNew name
        /// </param>
        /// <param name="conditionType">
        /// The condition Type. Useful for claimant grid to specify the resultset. AtMyClaimsHeirarchy, WithAccessRoles or None for all access.
        /// </param>
        /// <returns>
        /// Grid array
        /// </returns>
        public string[] GetAutoCompleteSearchGrid(ICurrentUser currentUser, string gridName, ConditionType conditionType = ConditionType.CompleteHierarchy)
        {
            var fields = new cFields(currentUser.AccountID);
            cGridNew grid;
            string selectIconPath = GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/nav_undo_green.png";

            switch (gridName)
            {
                case "gridOrganisationSearch":
                    cFieldToDisplay company = new cMisc(currentUser.AccountID).GetGeneralFieldByCode("organisation");
                    cTable table = new cTables(currentUser.AccountID).GetTableByID(new Guid("7BDAF84E-A373-4008-83D1-9E18AAA47F8E"));
                    JoinVia joinVia = new JoinVia(0, "Organisation Primary Address", Guid.NewGuid(), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(new Guid("B74065BB-6D13-464F-92E8-D6A49AE2F65E"), JoinViaPart.IDType.Field) } });

                    List<cNewGridColumn> columns = new List<cNewGridColumn>
                    {
                        new cFieldColumn(fields.GetFieldByID(new Guid("9325D7AB-70DE-4CF8-A6B5-B1AC46481169"))),            // OrganisationID
                        new cFieldColumn(fields.GetFieldByID(new Guid("4D0F2409-0705-4F0F-9824-42057B25AEBE"))),            // OrganisationName
                        new cFieldColumn(fields.GetFieldByID(new Guid("9BB72DFE-CAFD-459C-AF10-97FFCFA1B06F"))),            // ParentOrganisationID
                        new cFieldColumn(fields.GetFieldByID(new Guid("0D08813C-00E3-45EB-BDCC-0EA512615ADE"))),            // Comment
                        new cFieldColumn(fields.GetFieldByID(new Guid("AC87C4C4-9107-4555-B2A3-27109B3EBFBB"))),            // Code
                        new cFieldColumn(fields.GetFieldByID(new Guid("25597581-F3C5-40F0-B7BC-53FB67AC1A0E")), joinVia),   // Primary Address Line1
                        new cFieldColumn(fields.GetFieldByID(new Guid("B90920FE-BEA1-4948-A26B-CB997136C9F4")), joinVia),   // Primary Address City
                        new cFieldColumn(fields.GetFieldByID(new Guid("5A999C9F-440E-4D65-8B3C-187BA69E0234")), joinVia)    // Primary Address Postcode
                    };

                    grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridOrganisationSearch", table, columns)
                    {
                        KeyField = "OrganisationID",
                        EmptyText = "There are no " + company.description + " search results to display."
                    };

                    grid.getColumnByName("OrganisationID").hidden = true;
                    grid.getColumnByName("ParentOrganisationID").hidden = true;

                    grid.addEventColumn("selectOrganisation", selectIconPath, "javascript:OrganisationSearch.SearchChoice({OrganisationID}, '{OrganisationName}')", "Select this " + company.description, "Select this " + company.description);

                    grid.addFilter(fields.GetBy(table.TableID, "IsArchived"), ConditionType.Equals, new object[] { 0 }, null, ConditionJoiner.And);

                    grid.getColumnByID(new Guid("4D0F2409-0705-4F0F-9824-42057B25AEBE")).HeaderText = company.description;

                    break;
                case "gridClaimantSearch":
                    var employeeTableGuid = new Guid("618DB425-F430-4660-9525-EBAB444ED754");
                    const string LineManagerGuid = "96F11C6D-7615-4ABD-94EC-0E4D34E187A0";
                    var employeeIdGuid = new Guid("EDA990E3-6B7E-4C26-8D38-AD1D77FB2FBF");
                    List<Guid> columnsToDisplay = new List<Guid>();
                    columnsToDisplay.Add(new Guid("1C45B860-DDAA-47DA-9EEC-981F59CCE795"));
                    columnsToDisplay.Add(new Guid("28471060-247D-461C-ABF6-234BCB4698AA"));
                    columnsToDisplay.Add(new Guid("6614ACAD-0A43-4E30-90EC-84DE0792B1D6"));
                    columnsToDisplay.Add(new Guid("9D70D151-5905-4A67-944F-1AD6D22CD931"));
                    columnsToDisplay.Add(new Guid("0F951C3E-29D1-49F0-AC13-4CFCABF21FDA"));
                    var getEmployeeFirstnameSurnameByIdFunctionField = new Guid("142EA1B4-7E52-4085-BAAA-9C939F02EB77");
                    var employeeEmailField = new Guid("0F951C3E-29D1-49F0-AC13-4CFCABF21FDA");
                    var matchFields = string.Format("{0},{1}", getEmployeeFirstnameSurnameByIdFunctionField, employeeEmailField);
                    var javascriptFilters = FieldFilterGenerator.GetHierarchyJsFieldFilters(currentUser);
                    DataSet employeeDetails = SpendManagementLibrary.AutoComplete.GetAutoCompleteMatches(currentUser, employeeTableGuid.ToString(), columnsToDisplay, matchFields, string.Empty, true, javascriptFilters);
                    grid = new cGridNew(currentUser, employeeDetails, gridName, employeeTableGuid)
                    {
                        KeyField = "employeeid"
                    };
                    grid.getColumnByIndex(0).hidden = true;
                    grid.addEventColumn("selectEmployee", GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/nav_undo_green.png", "javascript:SEL.ClaimSelector.ClaimantCombo.SearchChoice({employeeid}, '{firstname} {surname} ({username})')", "Select this claimant", "Select this claimant");
                    grid.SortedColumn = grid.getColumnByName("firstname");
                    grid.EmptyText = "There are no claimants to display.";
                    break;
                case "gridLineManagerSearch":
                    grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, gridName, "SELECT employeeid, username, title, firstname, surname, email FROM dbo.employees")
                    {
                        KeyField = "employeeid"
                    };
                    grid.getColumnByIndex(0).hidden = true;
                    grid.addEventColumn("selectEmployee", GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/nav_undo_green.png", "javascript:lineManagerCombo.SearchChoice({employeeid}, '{firstname} {surname} ({username})')", "Select this employee", "Select this employee");
                    grid.addFilter(fields.GetBy(new Guid("618DB425-F430-4660-9525-EBAB444ED754"), "username"), ConditionType.NotLike, new object[] { "admin%" }, null, ConditionJoiner.And);
                    break;
                case "gridPoolCarSearch":
                    grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, gridName, "SELECT carid, make, model, registration FROM dbo.cars")
                    {
                        KeyField = "carid"
                    };
                    grid.getColumnByIndex(0).hidden = true;
                    grid.addEventColumn("selectPoolCar", selectIconPath, "javascript:poolCarCombo.SearchChoice({carid}, '{make} {model} ({registration})')", "Select this pool car", "Select this pool car");
                    grid.addFilter(fields.GetBy(new Guid("A184192F-74B6-42F7-8FDB-6DCF04723CEF"), "employeeid"), ConditionType.DoesNotContainData, null, null, ConditionJoiner.And);
                    break;
                case "gridCostCentreDepartmentSearch":
                    grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, gridName, "SELECT departmentid, department, description FROM dbo.departments")
                    {
                        KeyField = "departmentid"
                    };
                    grid.getColumnByIndex(0).hidden = true;
                    grid.addEventColumn("selectDepartment", selectIconPath, "#\" class=\"btn autocompletecombo-selectlink\" onclick=\"SEL.AutoCompleteCombo.SearchChoice(this, {departmentid}, '{department}')", "Select this department", "Select this department");
                    grid.addFilter(fields.GetBy(new Guid("A0F31CB0-16BB-4ACE-AAEA-69A7189D9599"), "archived"), ConditionType.Equals, new object[] { 0 }, null, ConditionJoiner.And);
                    break;
                case "gridCostCentreDepartmentDescriptionSearch":
                    grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, gridName, "SELECT departmentid, department, description FROM dbo.departments")
                    {
                        KeyField = "departmentid"
                    };
                    grid.getColumnByIndex(0).hidden = true;
                    grid.addEventColumn("selectDepartment", selectIconPath, "#\" class=\"btn autocompletecombo-selectlink\" onclick=\"SEL.AutoCompleteCombo.SearchChoice(this, {departmentid}, '{description}')", "Select this department", "Select this department");
                    grid.addFilter(fields.GetBy(new Guid("A0F31CB0-16BB-4ACE-AAEA-69A7189D9599"), "archived"), ConditionType.Equals, new object[] { 0 }, null, ConditionJoiner.And);
                    break;
                case "gridCostCentreCostCodeSearch":
                    grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, gridName, "SELECT costcodeid, costcode, description FROM dbo.costcodes")
                    {
                        KeyField = "costcodeid"
                    };
                    grid.getColumnByIndex(0).hidden = true;
                    grid.addEventColumn("selectCostCode", selectIconPath, "#\" class=\"btn autocompletecombo-selectlink\" onclick=\"SEL.AutoCompleteCombo.SearchChoice(this, {costcodeid}, '{costcode}')", "Select this cost code", "Select this cost code");
                    grid.addFilter(fields.GetBy(new Guid("02009E21-AA1D-4E0D-908A-4E9D73DDFBDF"), "archived"), ConditionType.Equals, new object[] { 0 }, null, ConditionJoiner.And);
                    break;
                case "gridCostCentreCostCodeDescriptionSearch":
                    grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, gridName, "SELECT costcodeid, costcode, description FROM dbo.costcodes")
                    {
                        KeyField = "costcodeid"
                    };
                    grid.getColumnByIndex(0).hidden = true;
                    grid.addEventColumn("selectCostCode", selectIconPath, "#\" class=\"btn autocompletecombo-selectlink\" onclick=\"SEL.AutoCompleteCombo.SearchChoice(this, {costcodeid}, '{description}')", "Select this cost code", "Select this cost code");
                    grid.addFilter(fields.GetBy(new Guid("02009E21-AA1D-4E0D-908A-4E9D73DDFBDF"), "archived"), ConditionType.Equals, new object[] { 0 }, null, ConditionJoiner.And);
                    break;
                case "gridCostCentreProjectCodeSearch":
                    grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, gridName, "SELECT projectcodeid, projectcode, description FROM dbo.project_codes")
                    {
                        KeyField = "projectcodeid"
                    };
                    grid.getColumnByIndex(0).hidden = true;
                    grid.addEventColumn("selectProjectCode", selectIconPath, "#\" class=\"btn autocompletecombo-selectlink\" onclick=\"SEL.AutoCompleteCombo.SearchChoice(this, {projectcodeid}, '{projectcode}')", "Select this project code", "Select this project code");
                    grid.addFilter(fields.GetBy(new Guid("E1EF483C-7870-42CE-BE54-ECC5C1D5FB34"), "archived"), ConditionType.Equals, new object[] { 0 }, null, ConditionJoiner.And);
                    break;
                case "gridCostCentreProjectCodeDescriptionSearch":
                    grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, gridName, "SELECT projectcodeid, projectcode, description FROM dbo.project_codes")
                    {
                        KeyField = "projectcodeid"
                    };
                    grid.getColumnByIndex(0).hidden = true;
                    grid.addEventColumn("selectProjectCode", selectIconPath, "#\" class=\"btn autocompletecombo-selectlink\" onclick=\"SEL.AutoCompleteCombo.SearchChoice(this, {projectcodeid}, '{Description}')", "Select this project code", "Select this project code");
                    grid.addFilter(fields.GetBy(new Guid("E1EF483C-7870-42CE-BE54-ECC5C1D5FB34"), "archived"), ConditionType.Equals, new object[] { 0 }, null, ConditionJoiner.And);
                    break;
                default:
                    grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, gridName, "")
                    {
                        KeyField = ""
                    };
                    break;
            }

            grid.pagesize = GlobalVariables.DefaultModalGridPageSize;

            return grid.generateGrid();
        }

        [WebMethod(EnableSession = true)]
        public string getJourneyDetails(string contextKey)
        {
            string[] temp = contextKey.Split(',');
            int claimid = Convert.ToInt32(temp[0].Trim());
            int expenseid = Convert.ToInt32(temp[1].Trim());
            string rowclass = "row1";
            CurrentUser user = cMisc.GetCurrentUser();
            cClaims clsclaims = new cClaims(user.AccountID);
            cClaim reqclaim = clsclaims.getClaimById(claimid);
            cExpenseItems expenseItems = new cExpenseItems(user.AccountID);
            cExpenseItem item = clsclaims.getExpenseItemById(expenseid);
            item.journeysteps = expenseItems.GetJourneySteps(expenseid);
            cEmployees clsEmployees = new cEmployees(user.AccountID);
            Employee reqclaimemp = clsEmployees.GetEmployeeById(reqclaim.employeeid);
            var subAccounts = new Lazy<cAccountSubAccounts>(() => new cAccountSubAccounts(user.AccountID));
            var subAccount = new Lazy<cAccountSubAccount>(() => subAccounts.Value.getSubAccountById(user.CurrentSubAccountId));
            StringBuilder output = new StringBuilder("<div style=\"padding: 4px; border: 1px solid #000000; background-color: #ffffff;\">");
            cSubcats subcats = new cSubcats(user.AccountID);
            cSubcat subcat = subcats.GetSubcatById(item.subcatid);
            var addresses = new Addresses(user.AccountID);

            if (subcat.EnableHomeToLocationMileage)
            {
                string comment = "";
                if (subcat.reimbursable && (item.mileageid != 0 || subcat.MileageCategory != null))
                {
                    cMileagecats clsmileagecats = new cMileagecats(user.AccountID);
                    cMileageCat reqmileage = clsmileagecats.GetMileageCatById(item.mileageid == 0 ? subcat.MileageCategory.Value : item.mileageid);
                    string sUom = reqmileage.mileUom.ToString();
                    cEmployeeCars clsEmpCars = new cEmployeeCars(user.AccountID, reqclaim.employeeid);

                    cCar reqcar = clsEmpCars.GetCarByID(item.carid); // user.Employee.getCarById(item.carid);
                    decimal hometooffice = 0;
                    decimal officetohome = 0;
                    string pluralUomHtO = "";
                    string pluralUomOtH = "";

                    if (reqcar != null && reqcar.ExemptFromHomeToOffice)
                    {
                        comment += "This car is exempt from any Home to Office Mileage Deductions";
                    }
                    else
                    {
                        comment += "Home to Office Mileage Deductions are enforced by your company policy.";

                        #region Get home to office distance if a home to office deduction is required

                        if (subcat.HomeToLocationType != HomeToLocationType.CalculateHomeAndOfficeToLocationDiff && subcat.HomeToLocationType != HomeToLocationType.FlagHomeAndOfficeToLocationDiff && subcat.HomeToLocationType != HomeToLocationType.None)
                        {
                            var workAddress = reqclaimemp.GetWorkAddresses().GetBy(user, item.date, (int)item.ESRAssignmentId);
                            var homeAddress = reqclaimemp.GetHomeAddresses().GetBy(item.date);

                            int workAddressId = workAddress != null ? workAddress.LocationID : 0;
                            int homeAddressId = homeAddress != null ? homeAddress.LocationID : 0;

                            var homeAddressRef = addresses.GetAddressById(homeAddressId);
                            var workAddressRef = addresses.GetAddressById(workAddressId);
                            officetohome = AddressDistance.GetRecommendedOrCustomDistance(workAddressRef, homeAddressRef, user.AccountID, subAccount.Value, user) ?? 0m;
                            hometooffice = AddressDistance.GetRecommendedOrCustomDistance(homeAddressRef, workAddressRef, user.AccountID, subAccount.Value, user) ?? 0m;

                            if (reqmileage.mileUom == MileageUOM.KM)
                            {
                                hometooffice = clsmileagecats.convertMilesToKM(hometooffice);
                                officetohome = clsmileagecats.convertMilesToKM(officetohome);
                            }

                            if (hometooffice > 1)
                            {
                                pluralUomHtO = "s";
                            }
                            if (officetohome > 1)
                            {
                                pluralUomOtH = "s";
                            }
                        }

                        #endregion

                        comment += cSubcat.GetMileageText(subcat, hometooffice, sUom, pluralUomHtO, officetohome, pluralUomOtH);
                    }

                    if (comment != "")
                    {
                        output.Append("<div class=\"comment\" style=\"padding: 4px; margin-bottom: 5px; width: 600px\">");
                        output.Append(comment);
                        output.Append("</div>");
                    }
                }
            }

            const string mapPlaceholder = "<!-- Recommended route map -->";
            output.Append(mapPlaceholder);
            bool includesHome = false, auditHomeAddress = true;

            output.Append("<table class=\"datatbl\">");
            output.Append("<tr><th>Start Address</th><th>End Address</th><th>Number Miles</th><th>Number Passengers</th><th>Recommended Distance</th><th>Reimbursable Distance</th><th>Heavy/Bulky Equipment</tr>");
            if (item.journeysteps != null)
            {
                foreach (cJourneyStep step in item.journeysteps.Values)
                {
                    bool fromLabelOverride = false;
                    bool toLabelOverride = false;
                    var employeeHomeLocation = reqclaimemp.GetHomeAddresses().GetBy(item.date);
                    var properties =
                        new cAccountSubAccounts(reqclaimemp.AccountID).getSubAccountById(reqclaimemp.DefaultSubAccount)
                            .SubAccountProperties;

                    if (employeeHomeLocation != null && (!properties.ShowFullHomeAddressOnClaims || user.isDelegate))
                    {
                        Address startAddress = Address.Get(user.AccountID, step.startlocation.Identifier);
                        Address endAddress = Address.Get(user.AccountID, step.endlocation.Identifier);
                        Address homeAddress = Address.Get(user.AccountID, employeeHomeLocation.LocationID);
                        string homePostCode = homeAddress.Postcode.ToLower().Replace(" ", string.Empty);
               
                        if (homePostCode == startAddress.Postcode.ToLower().Replace(" ", string.Empty))
                        {
                            fromLabelOverride = true;
                        }
                        if (homePostCode == endAddress.Postcode.ToLower().Replace(" ", string.Empty))
                        {
                            toLabelOverride = true;
                        }

                        auditHomeAddress = false;
                    }

                    output.Append("<tr>");
                    output.Append("<td class=\"" + rowclass + "\">");

                    if (step.startlocation != null)
                    {
                        string startLocationText = step.startlocation.FriendlyName;
                        //if not specified in options, don't show home address (data protection):

                        if (fromLabelOverride || ((!properties.ShowFullHomeAddressOnClaims || user.isDelegate) &&
                            ((employeeHomeLocation != null) &&
                            employeeHomeLocation.LocationID == step.startlocation.Identifier)))
                        {
                            includesHome = true;
                            startLocationText = "Home";
                        }
                        output.Append(startLocationText);
                    }
                    output.Append("</td>");
                    output.Append("<td class=\"" + rowclass + "\">");

                    if (step.endlocation != null)
                    {
                        string endLocationText = step.endlocation.FriendlyName;
                        if (toLabelOverride || ((!properties.ShowFullHomeAddressOnClaims || user.isDelegate) &&
                            ((employeeHomeLocation != null) &&
                            employeeHomeLocation.LocationID == step.endlocation.Identifier)))
                        {
                            includesHome = true;
                            endLocationText = "Home";
                        }
                        output.Append(endLocationText);
                    }

                    output.Append("</td>");
                    output.Append("<td align=\"right\" class=\"" + rowclass + "\">" + step.NumActualMiles + "</td>");
                    string passengersHtml = "";
                    if (step.passengers.Any())
                    {
                        passengersHtml =
                            "<span class='passengersinfoicon'></span>" +
                            "<div class='passengersinfocomment commenttooltip'>" +
                                string.Join("", step.passengers.Select(p => "<div>" + HttpUtility.HtmlEncode(p.Name) + "</div>")) +
                            "</div>";


                    }
                    output.Append("<td align=\"right\" class=\"" + rowclass + "\">" + passengersHtml + "<span>" + step.numpassengers + "</span>" + "</td>");

                    decimal? recommendedMiles = null;
                    if (step.startlocation != null && step.endlocation != null)
                    {
                        recommendedMiles = AddressDistance.GetRecommendedOrCustomDistance(step.startlocation, step.endlocation, user.AccountID, subAccount.Value, user);
                    }
                    output.Append("<td align=\"right\" class=\"" + rowclass + "\">" + recommendedMiles ?? 0 + "</td>");
                    output.Append("<td align=\"right\" class=\"" + rowclass + "\">" + step.nummiles + "</td>");
                    var hbe = "<input type='checkbox' " + (step.heavyBulkyEquipment ? "checked=\"checked\"" : "") + " disabled/>";
                    output.Append("<td align=\"center\" class=\"" + rowclass + "\">" + hbe + "</td>");
                    output.Append("</tr>");
                    if (rowclass == "row1")
                    {
                        rowclass = "row2";
                    }
                    else
                    {
                        rowclass = "row1";
                    }
                }
            }

            // audit if home address is viewed by approver
            if (auditHomeAddress == true && (user.EmployeeID != reqclaimemp.EmployeeID && !user.isDelegate))
            {
                cAuditLog auditLog = new cAuditLog();
                auditLog.ViewRecord(SpendManagementElement.Claims, "Home Address of " + reqclaimemp.Username + " viewed by employee.");
            }

            output.Append("</table>");
            output.Append("<div style=\"height: 30px; padding-top: 10px; padding-bottom: 4px;\"><img src=\"/shared/images/buttons/btn_close.png\" id=\"btnJourney_details\" alt=\"OK\" onclick=\"hideModal();\"style=\"cursor: pointer;\"/></div>");

            output.Append("</div>");

            bool preventMap = includesHome && (user.isDelegate || user.EmployeeID != reqclaimemp.EmployeeID);
            StringBuilder mapOutput = new StringBuilder();
            if (user.Account.MapsEnabled && !preventMap)
            {
                mapOutput.Append("<table>");
                mapOutput.Append("<tr><td class=\"labeltd\">Map &amp; Route:</td><td><a href=\"javascript:SEL.AddressesAndTravel.GetRouteByClaimAndExpense(" + expenseid + ");\">View the recommended route</a></td></tr>");
                mapOutput.Append("</table>");
            }

            return output.Replace(mapPlaceholder, mapOutput.ToString()).ToString();
        }

        /// <summary>
        /// Search grid, should be refactored when a combined autocomplete/select control is made
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="controlId">
        /// The control Id.
        /// </param>
        /// <param name="selectinatorString">
        /// The selectinator String.
        /// </param>
        /// <param name="childFilterList">
        /// The child Filter List.
        /// </param>
        /// <returns>
        /// </returns>
        [WebMethod(EnableSession = true)]
        public string[] GetSelectinatorComboSearchGrid(string type, string controlId, string selectinatorString, List<ParentAndChildElementValues> childFilterList)
        {
            var currentUser = cMisc.GetCurrentUser();
            var serializer = new JavaScriptSerializer();
            var selectinator = serializer.Deserialize<JsSelectinator>(selectinatorString);

            if (childFilterList != null && childFilterList.Count > 0)
            {
                var connection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID));

                foreach (var dynamicFilter in childFilterList)
                {
                    if (controlId != $"txt{dynamicFilter.ChildControl}selectinator") continue;

                    var fieldValues = cCustomFields.GetFilterAttributeForChildField(
                        connection,
                        dynamicFilter.ParentControl,
                        dynamicFilter.ChildControl);
                    selectinator.Filters.RemoveAll(filter => filter.FilterOnEdit);
                    if (fieldValues != Guid.Empty)
                        selectinator.Filters.Add(
                            new JSFieldFilter
                                {
                                    ConditionType = ConditionType.Equals,
                                    FieldID = fieldValues,
                                    ValueOne = dynamicFilter.ParentValue,
                                    Joiner = ConditionJoiner.And
                                });
                    break;
                }
            }
            return this.GetSelectinatorSearchGrid(currentUser, type, controlId, selectinator.TableGuid, selectinator.DisplayField, selectinator.MatchFields, selectinator.Filters);
        }

        internal string[] GetSelectinatorSearchGrid(ICurrentUser user, string type, string controlId, Guid tableId, Guid displayFieldId, List<Guid> matchFields, List<JSFieldFilter> filters)
        {
            var fields = new cFields(user.AccountID);
            var gridName = "grd" + type;
            var tables = new cTables(user.AccountID);
            var baseTable = tables.GetTableByID(tableId);
            var selectIconPath = GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/nav_undo_green.png";
            var fieldList = new List<cNewGridColumn>();
            var keyField = fields.GetFieldByID(baseTable.PrimaryKeyID);
            fieldList.Add(new cFieldColumn(keyField));
            fieldList.Add(new cFieldColumn(fields.GetFieldByID(displayFieldId)));
            if (matchFields.Contains(displayFieldId))
            {
                matchFields.Remove(displayFieldId);
            }

            if (matchFields.Contains(keyField.FieldID))
            {
                matchFields.Remove(keyField.FieldID);
            }

            foreach (Guid matchField in matchFields)
            {
                fieldList.Add(new cFieldColumn(fields.GetFieldByID(matchField)));
            }

            var grid = new cGridNew(user.AccountID, user.EmployeeID, gridName, baseTable, fieldList)
            {
                KeyField = keyField.FieldName,
                pagesize = GlobalVariables.DefaultModalGridPageSize,
            };

            grid.getColumnByIndex(0).hidden = true;
            var message = string.Format("Select this {0}", type);
            var columnString = "{" + keyField.FieldName + "}, '{" + fields.GetFieldByID(displayFieldId).FieldName + "}'";
            grid.addEventColumn(string.Format("select{0}", type), selectIconPath, string.Format("javascript:{0}.SelectinatorChoice({1})", controlId, columnString), message, message);

            foreach (JSFieldFilter jsFieldFilter in filters)
            {
                if (jsFieldFilter.IsParentFilter)
                {
                    continue;
                }

                CurrentUser currentUser = cMisc.GetCurrentUser();
                FieldFilter fieldFilter = FieldFilters.JsToCs(jsFieldFilter, currentUser);

                if (fieldFilter == null)
                {
                    continue;
                }

                FieldFilters.FieldFilterValues filterValues = FieldFilters.GetFilterValuesFromFieldFilter(fieldFilter, currentUser);

                grid.addFilter(fields.GetFieldByID(jsFieldFilter.FieldID), jsFieldFilter.ConditionType, new[] { filterValues.valueOne[0] },
                    new[] { filterValues.valueTwo[0] }, ConditionJoiner.And, fieldFilter.Conditiontype == ConditionType.AtMe ? fieldFilter.JoinVia : null);
            }

            return grid.generateGrid();
        }
    }
}
