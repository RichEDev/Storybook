namespace SpendManagementLibrary
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Web.UI.WebControls;

    using Definitions.JoinVia;

    using Employees;

    using Helpers;

    using Microsoft.SqlServer.Server;

    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// FieldFilters
    /// </summary>
    public class FieldFilters
    {
        #region Methods

        /// <summary>
        /// Converts a JSFieldFilter to a FieldFilter object
        /// </summary>
        /// <param name="jsFieldFilter"></param>
        /// <param name="oCurrentUser"></param>
        /// <returns></returns>
        public static FieldFilter JsToCs(JSFieldFilter jsFieldFilter, ICurrentUserBase oCurrentUser)
        {
            cFields fields = new cFields(oCurrentUser.AccountID);
            JoinVias joinVias = new JoinVias(oCurrentUser);

            cField field = fields.GetFieldByID(jsFieldFilter.FieldID);
            JoinVia joinVia = joinVias.GetJoinViaByID(jsFieldFilter.JoinViaID);

            if (field == null || (jsFieldFilter.JoinViaID > 0 && joinVia == null)) return null;

            return new FieldFilter(
                field,
                jsFieldFilter.ConditionType,
                jsFieldFilter.ValueOne,
                jsFieldFilter.ValueTwo,
                jsFieldFilter.Order,
                joinVia,
                isFilterOnEdit: jsFieldFilter.FilterOnEdit);
        }

        /// <summary>
        /// Converts a FieldFilter to a JSFieldFilter object
        /// </summary>
        /// <param name="fieldFilter">
        /// The filter to convert</param>
        /// <param name="oCurrentUser">The current user</param>
        /// <returns></returns>
        public static JSFieldFilter CsToJs(FieldFilter fieldFilter, ICurrentUserBase oCurrentUser)
        {
            var field = fieldFilter.Field;
            var joinVia = fieldFilter.JoinVia;

            if (field == null) return null;

            var result = new JSFieldFilter
            {
                ConditionType = fieldFilter.Conditiontype,
                FieldID = fieldFilter.Field.FieldID,
                JoinViaID = joinVia != null ? fieldFilter.JoinVia.JoinViaID : 0,
                Order = fieldFilter.Order,
                ValueOne = fieldFilter.ValueOne,
                ValueTwo = fieldFilter.ValueTwo,
                formID = fieldFilter.FormId,
                IsParentFilter = fieldFilter.IsParentFilter
            };

            return result;
        }

        /// <summary>
        /// Gets the filter values to be applied to the grid.
        /// </summary>
        /// <param name="values">
        /// An object of values</param>
        /// <param name="condition">
        /// The conditiontype being applied</param>
        /// <returns>An object of values</returns>
        public static object[] GetGridFilterWhereValues(object[] values, byte condition)
        {
            switch (condition)
            {
                case (int)ConditionType.AtMyHierarchy:
                case (int)ConditionType.AtMyClaimsHierarchy:
                    var employeeRecords = new List<SqlDataRecord>();
                    var whereValues = values[0];
                    SqlMetaData[] tvpItems = { new SqlMetaData("employeeid", SqlDbType.Int) };
                    IEnumerable enumerable = (IEnumerable)whereValues;

                    foreach (var employeeid in enumerable)
                    {
                        var row = new SqlDataRecord(tvpItems);
                        row.SetInt32(0, Convert.ToInt32(employeeid.ToString()));
                        employeeRecords.Add(row);
                    }
                    return new object[] { employeeRecords };
                default:
                    return values;
            }
        }

        /// <summary>
        /// Gets a QueryBuilder-friendly set of filters, with keywords and data types handled
        /// </summary>
        /// <param name="fieldFilter"></param>
        /// <param name="oCurrentUser"></param>
        /// <returns></returns>
        public static FieldFilterValues GetFilterValuesFromFieldFilter(FieldFilter fieldFilter, ICurrentUserBase oCurrentUser)
        {
            var filterValues = new FieldFilterValues();
            object[] value1 = { string.Empty }; 
            var value2 = new object[1] { ReplaceKeyWords(typeof(string), fieldFilter.ValueTwo, oCurrentUser) };
            ConditionType condition = fieldFilter.Conditiontype;

            // Find the values for @InMySelectedAccessRoles
            if (condition == ConditionType.WithAccessRoles)
            {
                string[] temp = fieldFilter.ValueOne.Split(',');
                List<int> accessRoles = temp[0] == string.Empty ? null : fieldFilter.ValueOne.Split(',').Select(int.Parse).ToList();
                List<SqlDataRecord> employeeIds = accessRoles == null ? null : GetMySelectedAccessRoleEmployeeIds(oCurrentUser, accessRoles);

                value1 = new object[] { employeeIds };
                fieldFilter.Conditiontype = ConditionType.AtMyHierarchy;

            }
            else if (condition == ConditionType.AtMyHierarchy || condition == ConditionType.AtMyClaimsHierarchy || (fieldFilter.ValueOne == "@MY_HIERARCHY" && condition == ConditionType.Equals))
            {
                // Find the values for @MY_HIERARCHY and Claims Hierarchy
                Guid lookupFieldId;
                if (fieldFilter.JoinVia != null &&
                    fieldFilter.JoinVia.JoinViaList.Last().Value.ViaIDType == JoinViaPart.IDType.Field)
                {
                    lookupFieldId = fieldFilter.JoinVia.JoinViaList.Last().Value.ViaID;
                }
                else
                {
                    // line manager field id should be passed in value 1
                    Guid.TryParse(fieldFilter.ValueOne, out lookupFieldId);
                }

                List<SqlDataRecord> employeeIds = condition == ConditionType.AtMyClaimsHierarchy
                    ? GetMyClaimHierarchyEmployeeIds(oCurrentUser, lookupFieldId, oCurrentUser.EmployeeID)
                    : GetMyHierarchyEmployeeIds(oCurrentUser, lookupFieldId, oCurrentUser.EmployeeID);

                value1 = new object[] { employeeIds };

                if (condition != ConditionType.AtMyClaimsHierarchy)
                {
                    condition = ConditionType.AtMyHierarchy;
                }
            }
            else if (condition == ConditionType.AtMyCostCodeHierarchy)
            {
                value1 = new object[] { GetEmployeeIdsForMyCostCodeHierarchy(oCurrentUser.EmployeeID, oCurrentUser) };
            }
            // Split out the values for ValueLists and CurrencyLists
            else if (fieldFilter.Field.ValueList || fieldFilter.Field.FieldType == "CL")
            {
                value1 = (from x in fieldFilter.ValueOne.Split(',') select (object)x).ToArray();
            }
            else if (fieldFilter.Field.GenList) // Split out and convert the values for GenLists
            {
                switch (fieldFilter.Conditiontype)
                {
                    case ConditionType.AtMe:
                        value1 = new object[1] { ReplaceKeyWords(typeof(string), fieldFilter.ValueOne, oCurrentUser) };
                        break;

                    case ConditionType.Like:
                    case ConditionType.NotLike:
                        value1 = fieldFilter.ValueOne == "admin%"
                                     ? new object[1] { fieldFilter.ValueOne }
                                     : new object[1] { "%" + ReplaceKeyWords(typeof(string), fieldFilter.ValueOne, oCurrentUser) + "%" };
                        break;

                    default:
                        List<string> genListIDs = fieldFilter.ValueOne.Split(',').ToList();

                        List<ListItem> itemsForField = GetGenListItemsForField(fieldFilter.Field.FieldID, oCurrentUser);

                        List<string> genListValues =
                            (from item in itemsForField
                             from o in genListIDs
                             where o == item.Value
                             select item.Text).ToList();

                        value1 = (from x in genListValues select (object)x).ToArray();
                        break;
                }
            }
            // Convert Date Times/Dates/Times to the correct format
            else if (fieldFilter.Field.FieldType == "D" || fieldFilter.Field.FieldType == "DT" || fieldFilter.Field.FieldType == "T")
            {
                switch (fieldFilter.Conditiontype)
                {
                    case ConditionType.Equals:
                    case ConditionType.DoesNotEqual:
                    case ConditionType.LessThan:
                    case ConditionType.LessThanEqualTo:
                    case ConditionType.GreaterThan:
                    case ConditionType.GreaterThanEqualTo:
                    case ConditionType.After:
                    case ConditionType.Before:
                    case ConditionType.On:
                    case ConditionType.OnOrAfter:
                    case ConditionType.OnOrBefore:
                    case ConditionType.NotOn:
                    case ConditionType.Between:
                        DateTime dtValueOne;
                        DateTime dtValueTwo;

                        if (fieldFilter.Field.FieldType == "T")
                        {
                            dtValueOne = Convert.ToDateTime("1900/01/01 " + fieldFilter.ValueOne);
                        }
                        else
                        {
                            dtValueOne = Convert.ToDateTime(fieldFilter.ValueOne);
                        }
                        value1 = new object[1] { dtValueOne };

                        if (fieldFilter.ValueTwo.Trim() != string.Empty)
                        {
                            if (fieldFilter.Field.FieldType == "T")
                            {
                                dtValueTwo = Convert.ToDateTime("1900/01/01 " + fieldFilter.ValueTwo);
                            }
                            else
                            {
                                dtValueTwo = Convert.ToDateTime(fieldFilter.ValueTwo);
                            }
                            value2 = new object[1] { dtValueTwo };
                        }

                        break;
                    case ConditionType.LastXDays:
                    case ConditionType.LastXMonths:
                    case ConditionType.LastXWeeks:
                    case ConditionType.LastXYears:
                    case ConditionType.NextXDays:
                    case ConditionType.NextXMonths:
                    case ConditionType.NextXWeeks:
                    case ConditionType.NextXYears:
                        value1 = new object[1] { int.Parse(fieldFilter.ValueOne) };

                        break;
                }
            }
            // Handle likes/not likes and replace keywords
            else
            {
                if (fieldFilter.Conditiontype == ConditionType.Like || fieldFilter.Conditiontype == ConditionType.NotLike)
                {
                    value1 = new object[1] { "%" + ReplaceKeyWords(typeof(string), fieldFilter.ValueOne, oCurrentUser) + "%" };
                }
                else
                {
                    value1 = new object[1] { ReplaceKeyWords(typeof(string), fieldFilter.ValueOne, oCurrentUser) };
                }
            }

            filterValues.valueOne = value1;
            filterValues.valueTwo = value2;
            filterValues.conditionType = condition;
            return filterValues;
        }

        /// <summary>
        /// Gets all of the genlist items for a given Field
        /// </summary>
        /// <param name="fieldid"></param>
        /// <param name="oCurrentUser"></param>
        /// <returns></returns>
        public static List<ListItem> GetGenListItemsForField(Guid fieldid, ICurrentUserBase oCurrentUser)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(oCurrentUser.AccountID));
            string strsql = "";
            SqlDataReader reader;
            cFields clsfields = new cFields(oCurrentUser.AccountID);
            cField field = clsfields.GetFieldByID(fieldid);
            cField primarykey;
            cField srcField;
            List<ListItem> returnList = new List<ListItem>();

            if (field.FieldType == "R")
            {
                primarykey = field.GetRelatedTable().GetPrimaryKey();
                srcField = field.GetRelatedTable().GetKeyField();
            }
            else
            {
                primarykey = field.GetParentTable().GetPrimaryKey();
                srcField = field;
            }

            strsql = "select " + primarykey.GetParentTable().TableName + "." + primarykey.FieldName + " as table_value, " +
                     srcField.GetParentTable().TableName + "." + srcField.FieldName + " as table_description from " +
                     srcField.GetParentTable().TableName;

            if (primarykey.GetParentTable().GetSubAccountIDField() != null && primarykey.GetParentTable().SubAccountIDFieldID != Guid.Empty)
            {
                // need to filter by subaccount
                cField subaccField = primarykey.GetParentTable().GetSubAccountIDField();
                strsql += " where " + subaccField.FieldName + " = @subAccId";
                expdata.sqlexecute.Parameters.AddWithValue("@subAccId", oCurrentUser.CurrentSubAccountId);
            }

            strsql += " order by " + srcField.FieldName;

            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    returnList.Add(new ListItem(reader[1].ToString(), reader[0].ToString()));
                }
                reader.Close();
            }
            return returnList;
        }

        /// <summary>
        /// Replaces any keywords used within a field filter
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="stringValue"></param>
        /// <param name="oCurrentUser"></param>
        /// <returns></returns>
        public static object ReplaceKeyWords(Type dataType, string stringValue, ICurrentUserBase oCurrentUser)
        {
            // Replace @ME_ID with the CurrentUser.EmployeeID
            if (stringValue.Contains("@ME_ID") == true)
            {
                stringValue = stringValue.Replace("@ME_ID", oCurrentUser.EmployeeID.ToString());
            }

            // Replace @ME with the CurrentUser.Employee.username
            if (stringValue.Contains("@ME") == true)
            {
                stringValue = stringValue.Replace("@ME", oCurrentUser.Employee.Username);
            }

            // Replace @ACTIVEMODULE_ID with CurrentUser.CurrentActiveModule (int value for the enumerator)
            if (stringValue.Contains("@ACTIVEMODULE_ID") == true)
            {
                stringValue = stringValue.Replace("@ACTIVEMODULE_ID", oCurrentUser.CurrentActiveModule.ToString());
            }

            // Replace @ACTIVEMODULE with CurrentUser.CurrentActiveModule (string value for the enumerator)
            if (stringValue.Contains("@ACTIVEMODULE") == true)
            {
                stringValue = stringValue.Replace("@ACTIVEMODULE", Convert.ToInt32(oCurrentUser.CurrentActiveModule).ToString());
            }

            // Replace @ACTIVESUBACCOUNT_ID with CurrentUser.CurrentActiveSubAccountId
            if (stringValue.Contains("@ACTIVESUBACCOUNT_ID") == true)
            {
                stringValue = stringValue.Replace("@ACTIVESUBACCOUNT_ID", oCurrentUser.CurrentSubAccountId.ToString());
            }

            if (dataType == typeof(int))
            {
                return Convert.ToInt32(stringValue);
            }

            if (dataType == typeof(decimal))
            {
                return Convert.ToDecimal(stringValue);
            }

            return stringValue;
        }

        /// <summary>
        /// Get the textual value for the given Filter's condition type
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public static string GetConditionTypeTextForFilter(FieldFilter filter, int accountID)
        {
            int conditionType = (int)filter.Conditiontype;
            cField field = filter.Field;
            string fieldType = field.FieldType;

            List<ListItem> operatorList =
                Enumerable.ToList<ListItem>(GetOperatorsListItemsByFilterFieldType(fieldType, CanUseMyHierarchy(filter.Field)).CriteriaList);

            return (operatorList.Where(x => x.Value == conditionType.ToString()).Select(x => x.Text)).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldtype"></param>
        /// <param name="showAtMeConditions"></param>
        /// <returns></returns>
        public static FilterCriteria GetOperatorsListItemsByFilterFieldType(string fieldtype, bool showAtMeConditions, bool showInOption = false)
        {
            List<ListItem> lstConditions = new List<ListItem>();

            switch (fieldtype)
            {
                case "LT":
                    lstConditions.Add(new ListItem("Contains Data", ((int)ConditionType.ContainsData).ToString()));
                    lstConditions.Add(new ListItem("Does Not Contain Data", ((int)ConditionType.DoesNotContainData).ToString()));
                    break;
                case "FS":
                case "S":
                case "CO":
                    lstConditions.Add(new ListItem("Equals", ((int)ConditionType.Equals).ToString()));
                    lstConditions.Add(new ListItem("Does Not Equal", ((int)ConditionType.DoesNotEqual).ToString()));
                    lstConditions.Add(new ListItem("Like", ((int)ConditionType.Like).ToString()));
                    lstConditions.Add(new ListItem("Not Like", ((int)ConditionType.NotLike).ToString()));
                    lstConditions.Add(new ListItem("Contains Data", ((int)ConditionType.ContainsData).ToString()));
                    lstConditions.Add(new ListItem("Does Not Contain Data", ((int)ConditionType.DoesNotContainData).ToString()));
                    if (showInOption)
                    {
                        lstConditions.Add(new ListItem("In", ((int)ConditionType.In).ToString()));
                    }
                    break;
                case "M":
                case "N":
                case "C":
                case "FD":
                case "FC":
                    lstConditions.Add(new ListItem("Equals", ((int)ConditionType.Equals).ToString()));
                    lstConditions.Add(new ListItem("Does Not Equal", ((int)ConditionType.DoesNotEqual).ToString()));
                    lstConditions.Add(new ListItem("Greater Than", ((int)ConditionType.GreaterThan).ToString()));
                    lstConditions.Add(new ListItem("Less Than", ((int)ConditionType.LessThan).ToString()));
                    lstConditions.Add(new ListItem("Greater Than or Equal To", ((int)ConditionType.GreaterThanEqualTo).ToString()));
                    lstConditions.Add(new ListItem("Less Than or Equal To", ((int)ConditionType.LessThanEqualTo).ToString()));
                    lstConditions.Add(new ListItem("Between", ((int)ConditionType.Between).ToString()));
                    lstConditions.Add(new ListItem("Contains Data", ((int)ConditionType.ContainsData).ToString()));
                    lstConditions.Add(new ListItem("Does Not Contain Data", ((int)ConditionType.DoesNotContainData).ToString()));
                    break;
                case "CL":
                case "GL":
                case "L":
                    lstConditions.Add(new ListItem("Equals", ((int)ConditionType.Equals).ToString()));
                    lstConditions.Add(new ListItem("Does Not Equal", ((int)ConditionType.DoesNotEqual).ToString()));
                    lstConditions.Add(new ListItem("Contains Data", ((int)ConditionType.ContainsData).ToString()));
                    lstConditions.Add(new ListItem("Does Not Contain Data", ((int)ConditionType.DoesNotContainData).ToString()));
                    break;
                case "X":
                    lstConditions.Add(new ListItem("Equals", ((int)ConditionType.Equals).ToString()));
                    lstConditions.Add(new ListItem("Does Not Equal", ((int)ConditionType.DoesNotEqual).ToString()));
                    break;
                case "T":
                    lstConditions.Add(new ListItem("Equals", ((int)ConditionType.Equals).ToString()));
                    lstConditions.Add(new ListItem("Does Not Equal", ((int)ConditionType.DoesNotEqual).ToString()));
                    lstConditions.Add(new ListItem("After", ((int)ConditionType.After).ToString()));
                    lstConditions.Add(new ListItem("Before", ((int)ConditionType.Before).ToString()));
                    lstConditions.Add(new ListItem("On or After", ((int)ConditionType.OnOrAfter).ToString()));
                    lstConditions.Add(new ListItem("On or Before", ((int)ConditionType.OnOrBefore).ToString()));
                    lstConditions.Add(new ListItem("Between", ((int)ConditionType.Between).ToString()));
                    lstConditions.Add(new ListItem("Contains Data", ((int)ConditionType.ContainsData).ToString()));
                    lstConditions.Add(new ListItem("Does Not Contain Data", ((int)ConditionType.DoesNotContainData).ToString()));
                    break;
                case "D":
                case "DT":
                    lstConditions.Add(new ListItem("On", ((int)ConditionType.On).ToString()));
                    lstConditions.Add(new ListItem("Not On", ((int)ConditionType.NotOn).ToString()));
                    lstConditions.Add(new ListItem("After", ((int)ConditionType.After).ToString()));
                    lstConditions.Add(new ListItem("Before", ((int)ConditionType.Before).ToString()));
                    lstConditions.Add(new ListItem("On or After", ((int)ConditionType.OnOrAfter).ToString()));
                    lstConditions.Add(new ListItem("On or Before", ((int)ConditionType.OnOrBefore).ToString()));
                    lstConditions.Add(new ListItem("Between", ((int)ConditionType.Between).ToString()));
                    lstConditions.Add(new ListItem("Yesterday", ((int)ConditionType.Yesterday).ToString()));
                    lstConditions.Add(new ListItem("Today", ((int)ConditionType.Today).ToString()));
                    lstConditions.Add(new ListItem("Tomorrow", ((int)ConditionType.Tomorrow).ToString()));
                    lstConditions.Add(new ListItem("Next 7 Days", ((int)ConditionType.Next7Days).ToString()));
                    lstConditions.Add(new ListItem("Last 7 Days", ((int)ConditionType.Last7Days).ToString()));
                    lstConditions.Add(new ListItem("Next Week", ((int)ConditionType.NextWeek).ToString()));
                    lstConditions.Add(new ListItem("Last Week", ((int)ConditionType.LastWeek).ToString()));
                    lstConditions.Add(new ListItem("This Week", ((int)ConditionType.ThisWeek).ToString()));
                    lstConditions.Add(new ListItem("Next Month", ((int)ConditionType.NextMonth).ToString()));
                    lstConditions.Add(new ListItem("Last Month", ((int)ConditionType.LastMonth).ToString()));
                    lstConditions.Add(new ListItem("This Month", ((int)ConditionType.ThisMonth).ToString()));
                    lstConditions.Add(new ListItem("Next Year", ((int)ConditionType.NextYear).ToString()));
                    lstConditions.Add(new ListItem("Last Year", ((int)ConditionType.LastYear).ToString()));
                    lstConditions.Add(new ListItem("This Year", ((int)ConditionType.ThisYear).ToString()));
                    lstConditions.Add(new ListItem("Next Tax Year", ((int)ConditionType.NextTaxYear).ToString()));
                    lstConditions.Add(new ListItem("Last Tax Year", ((int)ConditionType.LastTaxYear).ToString()));
                    lstConditions.Add(new ListItem("This Tax Year", ((int)ConditionType.ThisTaxYear).ToString()));
                    lstConditions.Add(new ListItem("Next Financial Year", ((int)ConditionType.NextFinancialYear).ToString()));
                    lstConditions.Add(new ListItem("Last Financial Year", ((int)ConditionType.LastFinancialYear).ToString()));
                    lstConditions.Add(new ListItem("This Financial Year", ((int)ConditionType.ThisFinancialYear).ToString()));
                    lstConditions.Add(new ListItem("Last X Days", ((int)ConditionType.LastXDays).ToString()));
                    lstConditions.Add(new ListItem("Next X Days", ((int)ConditionType.NextXDays).ToString()));
                    lstConditions.Add(new ListItem("Last X Weeks", ((int)ConditionType.LastXWeeks).ToString()));
                    lstConditions.Add(new ListItem("Next X Weeks", ((int)ConditionType.NextXWeeks).ToString()));
                    lstConditions.Add(new ListItem("Last X Months", ((int)ConditionType.LastXMonths).ToString()));
                    lstConditions.Add(new ListItem("Next X Months", ((int)ConditionType.NextXMonths).ToString()));
                    lstConditions.Add(new ListItem("Last X Years", ((int)ConditionType.LastXYears).ToString()));
                    lstConditions.Add(new ListItem("Next X Years", ((int)ConditionType.NextXYears).ToString()));
                    // lstConditions.Add(new ListItem("Any Time", "36"));
                    lstConditions.Add(new ListItem("Contains Data", ((int)ConditionType.ContainsData).ToString()));
                    lstConditions.Add(new ListItem("Does Not Contain Data", ((int)ConditionType.DoesNotContainData).ToString()));
                    lstConditions.Add(new ListItem("On or After Today", ((int)ConditionType.OnOrAfterToday).ToString()));
                    lstConditions.Add(new ListItem("On or Before Today", ((int)ConditionType.OnOrBeforeToday).ToString()));
                    break;
                case "AT":
                    lstConditions.Add(new ListItem("Contains Data", ((int)ConditionType.ContainsData).ToString()));
                    lstConditions.Add(new ListItem("Does Not Contain Data", ((int)ConditionType.DoesNotContainData).ToString()));
                    break;
            }
            
            if (fieldtype == "GL" && showAtMeConditions)
            {
                lstConditions.Add(new ListItem("@ME", ((int)ConditionType.AtMe).ToString()));
                lstConditions.Add(new ListItem("@MY_HIERARCHY", ((int)ConditionType.AtMyHierarchy).ToString()));
                lstConditions.Add(new ListItem("@MY_COST_CODE_HIERARCHY", ((int)ConditionType.AtMyCostCodeHierarchy).ToString()));
            }

            FilterCriteria fc = new FilterCriteria(fieldtype, lstConditions);
            return fc;
        }

        /// <summary>
        /// The get my hierarchy employee ids.
        /// </summary>
        /// <param name="currentUser">
        /// The current User.
        /// </param>
        /// <param name="lookupfieldId">
        /// The lookup field id.
        /// </param>
        /// <param name="lineManagerEmployeeId">
        /// The line manager employee id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<SqlDataRecord> GetMyHierarchyEmployeeIds(ICurrentUserBase currentUser, Guid lookupfieldId, int lineManagerEmployeeId)
        {
            var result = new List<SqlDataRecord>();

            SqlMetaData[] tvpItems = { new SqlMetaData("employeeid", SqlDbType.Int) };

            foreach (var employeeid in GetHeirachyList(lookupfieldId, lineManagerEmployeeId, currentUser))
            {
                var row = new SqlDataRecord(tvpItems);
                row.SetInt32(0, employeeid);
                result.Add(row);
            }

            return result.Count > 0 ? result : null;
        }

        /// <summary>
        /// The get employee ids for my cost code hierarchy (Ids of employees that the cost code owner is responsible for).
        /// </summary>
        /// <param name="costCodeOwnerEmployeeId">
        /// The cost code owner employee id.
        /// </param>
        /// <param name="currentUser">
        /// The current User.
        /// </param>
        /// <returns>
        /// The <see cref="List"/> of <see cref="SqlDataRecord"/> with the employees the cost code owner is responsible for.
        /// </returns>
        public static List<SqlDataRecord> GetEmployeeIdsForMyCostCodeHierarchy(int costCodeOwnerEmployeeId, ICurrentUserBase currentUser)
        {
            var result = new List<SqlDataRecord>();

            SqlMetaData[] tvpItems = { new SqlMetaData("employeeid", SqlDbType.Int) };

            foreach (var employeeid in GetCostCodeHierarchyList(costCodeOwnerEmployeeId, currentUser))
            {
                var row = new SqlDataRecord(tvpItems);
                row.SetInt32(0, employeeid);
                result.Add(row);
            }

            return result.Count > 0 ? result : null ;
        }

        /// <summary>
        /// Gets employee ids that have my selected access roles.
        /// </summary>
        /// <param name="user">
        /// The current user.
        /// </param>
        /// <param name="accessRoleIds">
        /// The access Role Ids.
        /// </param>
        /// <returns>
        /// A list of employee ids.
        /// </returns>
        public static List<SqlDataRecord> GetMySelectedAccessRoleEmployeeIds(ICurrentUserBase user, List<int> accessRoleIds)
        {
            var result = new List<SqlDataRecord>();

            SqlMetaData[] tvpItems = { new SqlMetaData("c1", SqlDbType.Int) };

            foreach (var employeeid in Employee.GetEmployeesWithAccessRoles(user.AccountID, accessRoleIds))
            {
                var row = new SqlDataRecord(tvpItems);
                row.SetInt32(0, employeeid);
                result.Add(row);
            }

            return result.Count > 0 ? result : null;
        }

        /// <summary>
        /// Gets the hierarchy of employee ids that the user can see the claims of.
        /// </summary>
        /// <param name="currentUser">
        /// The current User.
        /// </param>
        /// <param name="lookupfieldId">
        /// The lookup field id.
        /// </param>
        /// <param name="lineManagerEmployeeId">
        /// The line manager employee id.
        /// </param>
        /// <returns>
        /// A list of user defined table table (SqlDataRecord) employee id's
        /// </returns>
        public static List<SqlDataRecord> GetMyClaimHierarchyEmployeeIds(ICurrentUserBase currentUser, Guid lookupfieldId, int lineManagerEmployeeId)
        {
            var result = new List<SqlDataRecord>();
            SqlMetaData[] tvpItems = { new SqlMetaData("employeeid", SqlDbType.Int) };
            IEnumerable<int> claimHierarchyList = GetClaimHeirachyList(lookupfieldId, lineManagerEmployeeId, currentUser);
            IEnumerable<int> extendedclaimHierarchyList = GetClaimHeirachyList(lookupfieldId, lineManagerEmployeeId, currentUser, true);
            IEnumerable<int> completeHierarchy = claimHierarchyList.Concat(extendedclaimHierarchyList).Distinct().ToList();

            foreach (var employeeid in completeHierarchy)
            {
                var row = new SqlDataRecord(tvpItems);
                row.SetInt32(0, employeeid);
                result.Add(row);
            }

            return result.Count > 0 ? result : null;
        }

        /// <summary>
        /// The get heirachy list.
        /// </summary>
        /// <param name="lookupfieldId">
        /// The lookup field id.
        /// </param>
        /// <param name="lineManagerEmployeeId">
        /// The line manager employee id.
        /// </param>
        /// <param name="oCurrentUser">
        /// The current user.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private static IEnumerable<int> GetHeirachyList(Guid lookupfieldId, int lineManagerEmployeeId, ICurrentUserBase oCurrentUser)
        {
            var returnList = new List<int>();
            var expdata = new DBConnection(cAccounts.getConnectionString(oCurrentUser.AccountID));         
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", lineManagerEmployeeId);
            expdata.sqlexecute.Parameters.AddWithValue("@matchfieldID", lookupfieldId);

            using (var reader = expdata.GetProcReader("GetHeirachy"))
            {
                while (reader.Read())
                {
                    returnList.Add(reader.GetInt32(0));
                }

                expdata.sqlexecute.Parameters.Clear();
                reader.Close();
            }

            return returnList;
        }

        /// <summary>
        /// The get cost code hierarchy list.
        /// </summary>
        /// <param name="costCodeOwnerEmployeeId">
        /// The cost code owner employee id.
        /// </param>
        /// <param name="currentUser">
        /// The current User.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        private static IEnumerable<int> GetCostCodeHierarchyList(int costCodeOwnerEmployeeId, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            var returnList = new List<int>();

            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", costCodeOwnerEmployeeId);
                expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", currentUser.CurrentSubAccountId);

                using (var reader = expdata.GetReader("GetCostCodeHierarchy", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        returnList.Add(reader.GetInt32(0));
                    }

                    expdata.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }

            return returnList;
        }

        /// <summary>
        /// Gets the claim hierarchy.
        /// </summary>
        /// <param name="lookupfieldId">
        /// The lookup field id.
        /// </param>
        /// <param name="lineManagerEmployeeId">
        /// The line manager employee id.
        /// </param>
        /// <param name="user">
        /// The current user.
        /// </param>
        /// <returns>
        /// The list of integers representing the claim hierarchy.
        /// </returns>
        public static IEnumerable<int> GetClaimHeirachyList(Guid lookupfieldId, int lineManagerEmployeeId, ICurrentUserBase user, bool GetExtendedHierarchy= false)
        {
            string procedureName = GetExtendedHierarchy ? "GetExtendedClaimHierarchy" : "GetClaimHierarchy";
            var returnList = new List<int>();
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                connection.sqlexecute.Parameters.Clear();
                connection.sqlexecute.Parameters.AddWithValue("@employeeid", lineManagerEmployeeId);
                connection.sqlexecute.Parameters.AddWithValue("@matchfieldID", lookupfieldId);

                using (var reader = connection.GetReader(procedureName, CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        returnList.Add(reader.GetInt32(0));
                    }

                    connection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }

            return returnList;
        }

        public static void GetDependentIds(int accountId, Guid relatedTableId, Guid lookupFieldId, int recordId, ref List<int> currentIds)
        {
            cTables clsTables = new cTables(accountId);

            cFields clsFields = new cFields(accountId);
            cField lookupField = clsFields.GetFieldByID(lookupFieldId);
            cTable baseTable = clsTables.GetTableByID(lookupField.TableID);
            cField pkField = baseTable.GetPrimaryKey();

            if (!currentIds.Contains(recordId))
            {
                currentIds.Add(recordId);
            }

            cQueryBuilder qb = new cQueryBuilder(accountId, cAccounts.getConnectionString(accountId), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, baseTable, clsTables, clsFields);

            qb.addColumn(pkField);
            qb.addFilter(lookupField, ConditionType.Equals, new object[] { recordId }, null, ConditionJoiner.None, null);

            using (SqlDataReader reader = qb.getReader())
            {
                while (reader.Read())
                {
                    int recId = reader.GetInt32(0);
                    if (!currentIds.Contains(recId))
                    {
                        currentIds.Add(recId);

                        GetDependentIds(accountId, relatedTableId, lookupFieldId, recId, ref currentIds);
                    }
                }
                reader.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static bool CanUseMyHierarchy(cField field)
        {
            bool showAtMeConditions = false;
            switch (field.TableID.ToString().ToUpper())
            {
                case "618DB425-F430-4660-9525-EBAB444ED754": // employees
                case "972AC42D-6646-4EFC-9323-35C2C9F95B62": // userdefined_employees
                    showAtMeConditions = true;
                    break;
            }
            return showAtMeConditions;
        }
        #endregion Methods

        /// <summary>
        /// Stores the values used on a Field Filter
        /// </summary>
        public struct FieldFilterValues
        {
            public object[] valueOne;
            public object[] valueTwo;
            public ConditionType conditionType;
        }
    }

    public class FilterCriteria
    {
        public string DataType { get; set; }
        public List<ListItem> CriteriaList { get; set; }
        public FilterCriteria(string type, List<ListItem> options)
        {
            DataType = type;
            CriteriaList = options;
        }
    }
}
