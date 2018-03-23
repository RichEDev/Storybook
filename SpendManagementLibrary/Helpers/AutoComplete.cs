using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using SpendManagementLibrary.Helpers;

namespace SpendManagementLibrary
{
    using System.Runtime.Caching;

    using SpendManagementLibrary.Definitions.JoinVia;

    public class AutoComplete
    {

        public static List<object> getAutoCompleteQueryParams(string tableName)
        {
            // display name first, then match fields
            List<object> vals = new List<object>();
            List<Guid> tmpLst = new List<Guid>();

            switch (tableName)
            {
                case "employees":
                    vals.Add(new Guid("B5DD3E91-DA63-4E9B-B933-FEC64831D920")); // dbo.getEmployeeFullName
                    tmpLst = new List<Guid>() { new Guid("B5DD3E91-DA63-4E9B-B933-FEC64831D920"), new Guid("1C45B860-DDAA-47DA-9EEC-981F59CCE795"), new Guid("0F951C3E-29D1-49F0-AC13-4CFCABF21FDA") }; // displayfield and username, email
                    vals.Add(tmpLst);
                    break;
                case "supplier_details":
                    vals.Add(new Guid("3F834EE1-C272-4C42-902B-F37B1FC95DD9"));
                    tmpLst = new List<Guid>() { new Guid("3F834EE1-C272-4C42-902B-F37B1FC95DD9"), new Guid("6C5D6FB1-CA57-473D-A04A-8262557B70C6") }; // supplier name, web address
                    vals.Add(tmpLst);
                    break;
                case "budgetholders":
                    vals.Add(new Guid("28E88047-145A-45E5-AFEB-3F18A4683BA5"));
                    tmpLst = new List<Guid>() { new Guid("28E88047-145A-45E5-AFEB-3F18A4683BA5"), new Guid("7A470F7D-D081-4D64-B663-2A0AC2A7C221") }; // label, description
                    vals.Add(tmpLst);
                    break;
                case "departments":
                    vals.Add(new Guid("9617A83E-6621-4B73-B787-193110511C17"));
                    tmpLst = new List<Guid>() { new Guid("9617A83E-6621-4B73-B787-193110511C17"), new Guid("990FD383-14F8-4F50-A2E2-13A9D1F847B7") }; // department, description
                    vals.Add(tmpLst);
                    break;
                case "signoffentities":
                    vals.Add(new Guid("900EF8E0-1FE2-4849-A274-93C985AF91C8"));
                    tmpLst = new List<Guid> { new Guid("900EF8E0-1FE2-4849-A274-93C985AF91C8") }; // Name
                    vals.Add(tmpLst);
                    break;
                case "employeesWithUsername":
                    vals.Add(new Guid("142EA1B4-7E52-4085-BAAA-9C939F02EB77")); // dbo.GetEmployeeFirstnameSurnameUsernameById
                    tmpLst = new List<Guid>() { new Guid("142EA1B4-7E52-4085-BAAA-9C939F02EB77"), new Guid("0F951C3E-29D1-49F0-AC13-4CFCABF21FDA") }; // displayfield and username, email
                    vals.Add(tmpLst);
                    break;
                case "organisations":
                    vals.Add(new Guid("4D0F2409-0705-4F0F-9824-42057B25AEBE")); // organisation Name
                    tmpLst = new List<Guid>() { new Guid("4D0F2409-0705-4F0F-9824-42057B25AEBE"), new Guid("AC87C4C4-9107-4555-B2A3-27109B3EBFBB") }; // organisation name, organisation code
                    vals.Add(tmpLst);
                    break;
                case "EmployeeView":
                    vals.Add(new Guid("36131B2F-F85F-4B97-8412-C0139812AFAA"));
                    tmpLst = new List<Guid> { new Guid("36131B2F-F85F-4B97-8412-C0139812AFAA") }; // Name
                    vals.Add(tmpLst);
                    break;
                default:
                    break;
            }

            return vals;
        }
        /// <summary>
        /// Generate the AutoComplete jQuery bind() function call to link an autocomplete to a control
        /// </summary>
        /// <param name="cntl">
        /// ASP control name
        /// </param>
        /// <param name="maxRows">
        /// Number of rows to return in the autocomplete match list
        /// </param>
        /// <param name="lookupTableID">
        /// Table ID that records are to be matched against
        /// </param>
        /// <param name="displayFieldID">
        /// Field to display in the autocomplete match list
        /// </param>
        /// <param name="lookupMatchFields">
        /// Field list to perform wildcard matches against
        /// </param>
        /// <param name="msLookupDelay">
        /// Delay in milliseconds to wait after keystroke before making autocomplete request (Default: 0.5 second)
        /// </param>
        /// <param name="fieldFilters">
        /// Optional field list to perform filters against
        /// </param>
        /// <param name="triggerFields">
        /// The trigger Fields.
        /// </param>
        /// <param name="keyFieldIsString">
        /// The key Field Is String.
        /// </param>
        /// <param name="AutoCompleteDisplayFields">
        /// Field list to display in autocomplete search results
        /// </param>
        /// <param name="displayAutocompleteMultipleResultsFields">
        /// Display the mutliple fields in search results (Identify if this feature is available in the page with autocomplete text input.Set to true for greenlight n:1 attributes) 
        /// </param>
        /// <returns>
        /// JavaScript bind string
        /// </returns>
        public static string CreateAutoCompleteComboString(string cntl, int maxRows, Guid lookupTableID, Guid displayFieldID, List<Guid> lookupMatchFields, int msLookupDelay = 500, SortedList<int, FieldFilter> fieldFilters = null, List<AutoCompleteTriggerField> triggerFields = null, bool keyFieldIsString = false, List<Guid> AutoCompleteDisplayFields = null, bool displayAutocompleteMultipleResultsFields = false)
        {
            Dictionary<string, JSFieldFilter> filterDict = new Dictionary<string, JSFieldFilter>();

            if (fieldFilters != null && fieldFilters.Count > 0)
            {
                foreach (KeyValuePair<int, FieldFilter> filter in fieldFilters)
                {
                    filterDict.Add(filter.Key.ToString(), new JSFieldFilter
                    {
                        FieldID = filter.Value.Field.FieldID,
                        ConditionType = filter.Value.Conditiontype,
                        JoinViaID = filter.Value.JoinVia == null ? 0 : filter.Value.JoinVia.JoinViaID,
                        Order = filter.Value.Order,
                        ValueOne = filter.Value.ValueOne,
                        ValueTwo = filter.Value.ValueTwo,
                        formID = filter.Key
                    });
                }
            }

            var jsSerializer = new JavaScriptSerializer();
            string filters = jsSerializer.Serialize(filterDict);
            string triggers = "null";
            var autoCompleteDisplayFields = "null";
            if (triggerFields != null && triggerFields.Count > 0)
            {
                triggers = jsSerializer.Serialize(triggerFields);
            }

            if (AutoCompleteDisplayFields != null && AutoCompleteDisplayFields.Count > 0)
            {
                autoCompleteDisplayFields = string.Join(",", AutoCompleteDisplayFields.ConvertAll(x => x.ToString()));
            }

            return string.Format("SEL.AutoCompleteCombo.Bind('{0}',{1}, '{2}', '{3}', '{4}', '{5}', '{6}', {7}, {8}, '{9}', '{10}');", cntl, maxRows.ToString(), lookupTableID.ToString(), displayFieldID.ToString(), string.Join(",", lookupMatchFields.ConvertAll(x => x.ToString())), autoCompleteDisplayFields, filters, msLookupDelay.ToString(), triggers, keyFieldIsString, displayAutocompleteMultipleResultsFields);
        }

        /// <summary>
        /// Generate the AutoComplete jQuery bind() function call to link an autocomplete to a control
        /// </summary>
        /// <param name="cntl">
        /// ASP control name
        /// </param>
        /// <param name="maxRows">
        /// Number of rows to return in the autocomplete match list
        /// </param>
        /// <param name="lookupTableID">
        /// Table ID that records are to be matched against
        /// </param>
        /// <param name="displayFieldID">
        /// Field to display in the autocomplete match list
        /// </param>
        /// <param name="lookupMatchFields">
        /// Field list to perform wildcard matches against
        /// </param>
        /// <param name="msLookupDelay">
        /// Delay in milliseconds to wait after keystroke before making autocomplete request (Default: 0.5 second)
        /// </param>
        /// <param name="fieldFilters">
        /// Optional field list to perform filters against
        /// </param>
        /// <param name="triggerFields">
        /// The trigger Fields.
        /// </param>
        /// <param name="keyFieldIsString">
        /// The key Field Is String.
        /// </param>
        /// <param name="AutoCompleteDisplayFields">
        /// Field list to display in autocomplete search results
        /// </param>
        /// <param name="displayAutocompleteMultipleResultsFields">
        /// Display the mutliple fields in search results (Identify if this feature is available in the page with autocomplete text input.Set to true for greenlight n:1 attributes) 
        /// </param>
        /// <returns>
        /// JavaScript bind string
        /// </returns>
        public static string createAutoCompleteBindString(string cntl, int maxRows, Guid lookupTableID, Guid displayFieldID, List<Guid> lookupMatchFields, int msLookupDelay = 500, SortedList<int, FieldFilter> fieldFilters = null, List<AutoCompleteTriggerField> triggerFields = null, bool keyFieldIsString = false, List<Guid> AutoCompleteDisplayFields=null,bool displayAutocompleteMultipleResultsFields=false)
        {
            Dictionary<string, JSFieldFilter> filterDict = new Dictionary<string, JSFieldFilter>();

            if (fieldFilters != null && fieldFilters.Count > 0)
            {
                foreach (KeyValuePair<int, FieldFilter> filter in fieldFilters)
                {
                    filterDict.Add(filter.Key.ToString(), new JSFieldFilter
                    {
                        FieldID = filter.Value.Field.FieldID,
                        ConditionType = filter.Value.Conditiontype,
                        JoinViaID = filter.Value.JoinVia == null ? 0 : filter.Value.JoinVia.JoinViaID,
                        Order = filter.Value.Order,
                        ValueOne = filter.Value.ValueOne,
                        ValueTwo = filter.Value.ValueTwo,
                        formID = filter.Key,
                        FilterOnEdit = filter.Value.IsFilterOnEdit
                    });
                }
            }

            var jsSerializer = new JavaScriptSerializer();
            string filters = jsSerializer.Serialize(filterDict);
            string triggers = "null";
            var autoCompleteDisplayFields = "null";
            if (triggerFields != null && triggerFields.Count > 0)
            {
                triggers = jsSerializer.Serialize(triggerFields);
            }
            if (AutoCompleteDisplayFields != null && AutoCompleteDisplayFields.Count > 0)
            {
                autoCompleteDisplayFields = string.Join(",", AutoCompleteDisplayFields.ConvertAll(x => x.ToString()));
            }
            return "SEL.AutoComplete.Bind('" + cntl + "'," + maxRows.ToString() + ", '" + lookupTableID.ToString() + "', '"
                   + displayFieldID.ToString() + "', '"
                   + string.Join(",", lookupMatchFields.ConvertAll(x => x.ToString())) + "', '"
                    + autoCompleteDisplayFields + "', '" 
                    + filters + "', "
                   + msLookupDelay.ToString() + ", " + triggers + ", '" + keyFieldIsString + "', '" + "null" + "', '"
                    + displayAutocompleteMultipleResultsFields + "');";
        }

        /// <summary>
        /// Generates any jQuery cmds inside a $(document).ready block for using with RegisterStartupScript
        /// </summary>
        /// <param name="jsScriptCmds">List of string cmds to be included in the jQuery block</param>
        /// <returns>javascript code block</returns>
        public static string generateScriptRegisterBlock(List<string> jsScriptCmds)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("$(document).ready(function() { ");
            foreach (string bindStr in jsScriptCmds)
            {
                sb.Append(bindStr + "\n");
            }
            sb.Append(" });"); // close the jQuery .ready() function block
            return sb.ToString();
        }

        /// <summary>
        /// Gets collection of controls needed for autocomplete text control
        /// </summary>
        /// <param name="cntlID"></param>
        /// <param name="validationGroup"></param>
        /// <returns>TextBox, Hidden ID field and Validator for catching invalid data entry</returns>
        public static List<WebControl> createAutoCompleteControls(string cntlID, string fieldLabel, string cssClass)
        {
            List<WebControl> controls = new List<WebControl>();
            WebControl oControl = null;

            oControl = new TextBox();
            oControl.ID = cntlID;
            oControl.CssClass = cssClass;

            controls.Add(oControl);

            oControl = new TextBox();
            oControl.Attributes.Add("style", "display: none;");
            oControl.ID = cntlID + "_ID";
            controls.Add(oControl);

            return controls;
        }

        public static CompareValidator getAutoCompleteInvalidEntryValidator(string cntlID, string fieldLabel, string validationGroup)
        {
            // add compare validator to detect invalid entry
            CompareValidator oControl = new CompareValidator
            {
                ID = "cmp" + cntlID,
                ValidationGroup = validationGroup,
                ControlToValidate = cntlID + "_ID",
                Text = "*",
                ErrorMessage =
                                                    "Invalid entry provided for " + fieldLabel +
                                                    " - Type three or more characters and select a valid entry from the available options.",
                Operator = ValidationCompareOperator.NotEqual,
                ValueToCompare = "-1",
                Display = ValidatorDisplay.Dynamic,
                Type = ValidationDataType.Integer
            };

            return oControl;
        }

        public static RequiredFieldValidator getAutoCompleteMandatoryValidator(string cntlID, string fieldLabel, string validationGroup)
        {
            // add required field validator
            RequiredFieldValidator oControl = new RequiredFieldValidator
            {
                ID = "req" + cntlID,
                ValidationGroup = validationGroup,
                ControlToValidate = cntlID,
                Text = "*",
                ErrorMessage =
                                                          fieldLabel + " is a mandatory field. Please enter a value in the box provided.",
                Display = ValidatorDisplay.Dynamic
            };
            return oControl;
        }

        /// <summary>
        /// Returns ID and display for for auto complete extender (jQuery) to for dynamic data sources
        /// </summary>
        /// <param name="currentUser">A current user object</param>
        /// <param name="maxRows">Number of records to return</param>
        /// <param name="matchTable">Table ID (Guid) to perform the look up on</param>
        /// <param name="displayField">Text field to display in the returned list</param>
        /// <param name="matchFields">Comma separated list of Field IDs (Guids) to perform wildcard match on </param>
        /// <param name="matchText">Text to match</param>
        /// <param name="useWildcards">Specify if the search should use wildcards or return specific values</param>
        /// <param name="filters">Specifiy the field filters to use with the Auto Complete</param>
        /// <param name="keyIsString">True if the key field is a string</param>
        /// <param name="childFilterList">Child control filtered value</param>
        /// <returns>Serializable structure of type sAutoComplete</returns>
        public static List<sAutoCompleteResult> GetAutoCompleteMatches(ICurrentUserBase currentUser, int maxRows, string matchTable, string displayField, string matchFields, string matchText, bool useWildcards, Dictionary<string, JSFieldFilter> filters, bool keyIsString = false, List<AutoCompleteChildFieldValues> childFilterList=null)
        {
            var retVals = new List<sAutoCompleteResult>();
            

            Guid displayFieldId;
            cFields fields = new cFields();

            if (Guid.TryParseExact(displayField, "D", out displayFieldId))
            {
                //Is this for an auto complete request and is the display field costcode description or cost code ?
                if (maxRows > 0 && (displayFieldId == new Guid("AF80D035-6093-4721-8AFC-061424D2AB72") || displayFieldId == new Guid("359DFAC9-74E6-4BE5-949F-3FB224B1CBFC")))
                {
                    retVals = GetCostCodeAutoCompleteResults(matchText, displayFieldId, currentUser, useWildcards, childFilterList);
                }
                else
                {
                    cTables tables = new cTables();
                    cTable basetable = new cTable();
                    List<Guid> matchFieldIDs;

                    if (InitialiseMatchingData(currentUser, matchTable, matchFields, ref matchText, useWildcards, out matchFieldIDs, ref tables, ref fields, ref basetable))
                    {
                        if (basetable != null)
                        {
                            cQueryBuilder qb = new cQueryBuilder(currentUser.AccountID, cAccounts.getConnectionString(currentUser.AccountID),
                                                                 ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, basetable,
                                                                 tables, fields, childFilterList != null && childFilterList.Count > 0 ? 0 : maxRows);
                            cField fieldToDisplay = fields.GetFieldByID(displayFieldId);

                            if (fieldToDisplay != null)
                            {
                                qb.addColumn(basetable.GetPrimaryKey());
                                qb.addColumn(fieldToDisplay);
                                qb.addSortableColumn(fieldToDisplay, SortDirection.Ascending);

                                List<cQueryFilter> matchFieldFilters = new List<cQueryFilter>();

                                // Add each of the Match Fields
                                if (matchFieldIDs.Count > 0)
                                {
                                    foreach (Guid id in matchFieldIDs)
                                    {
                                        cField curMatchField = fields.GetFieldByID(id);

                                        matchFieldFilters.Add(new cQueryFilter(curMatchField, ConditionType.Like, new List<object> { matchText }, null, ConditionJoiner.Or, null));
                                    }

                                    qb.addFilterGroup(new cQueryFilterGroup(matchFieldFilters, ConditionJoiner.And));
                                }

                                // Add each of the filters
                                if (filters != null)
                                {
                                    foreach (JSFieldFilter curFilter in filters.Values)
                                    {
                                        if (curFilter.IsParentFilter)
                                        {
                                            continue;
                                        }

                                        FieldFilter fieldFilter = FieldFilters.JsToCs(curFilter, currentUser);

                                        if (fieldFilter == null)
                                        {
                                            continue;
                                        }

                                        FieldFilters.FieldFilterValues filterValues = FieldFilters.GetFilterValuesFromFieldFilter(fieldFilter, currentUser);

                                        qb.addFilter(fieldFilter.Field, fieldFilter.Conditiontype, filterValues.valueOne, filterValues.valueTwo, ConditionJoiner.And, fieldFilter.JoinVia);

                                    }
                                }

                                if (basetable.GetSubAccountIDField() != null)
                                {
                                    qb.addFilter(basetable.GetSubAccountIDField(), ConditionType.Equals, new object[] { currentUser.CurrentSubAccountId }, null, ConditionJoiner.And, null);
                                }

                                // make sure the match fields are present, or all records will be shown
                                if (matchFieldFilters.Count > 0)
                                {
                                    using (SqlDataReader reader = qb.getReader())
                                    {
                                        while (reader.Read())
                                        {
                                            sAutoCompleteResult entry;
                                            if (!keyIsString)
                                            {
                                                if (reader.GetFieldType(0) == typeof(long) && reader.GetFieldType(1) == typeof(string))
                                                {
                                                    entry.value = reader.GetInt64(0).ToString(CultureInfo.InvariantCulture);
                                                    entry.label = Convert.ToString(reader.GetValue(1));
                                                }
                                                else if (reader.GetFieldType(0) == typeof(long) && reader.GetFieldType(1) == typeof(long))
                                                {
                                                    entry.value = reader.GetInt64(0).ToString(CultureInfo.InvariantCulture);
                                                    entry.label = reader.GetInt64(1).ToString(CultureInfo.InvariantCulture);
                                                }
                                                else if (reader.GetFieldType(0) == typeof(int) && reader.GetFieldType(1) == typeof(string))
                                                {
                                                    entry.value = reader.GetInt32(0).ToString(CultureInfo.InvariantCulture);
                                                    entry.label = Convert.ToString(reader.GetValue(1));
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                if (reader.GetFieldType(0) != typeof(string) || reader.GetFieldType(1) != typeof(string))
                                                {
                                                    continue;
                                                }

                                                entry.value = Convert.ToString(reader.GetValue(0));
                                                entry.label = Convert.ToString(reader.GetValue(1));
                                            }

                                            retVals.Add(entry);
                                        }

                                        reader.Close();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var sortedResults = retVals.OrderBy(x => x.label).ToList(); 

            if (childFilterList == null)
            {
                return sortedResults;
            }
  
            if (childFilterList.Count > 0 && childFilterList[0].FieldToBuild != string.Empty)
            {
                var requiredAttachmentField = fields.GetFieldByName($"att{childFilterList[0].FieldToBuild}", true);

                if (requiredAttachmentField == null || requiredAttachmentField.LookupFieldID.ToString() != displayField)
                {
                    return sortedResults.Count > maxRows ? sortedResults.GetRange(0, maxRows) : retVals;
                }           
            }
    
            return sortedResults.Count > maxRows ? sortedResults.GetRange(0, maxRows) : sortedResults;           
        }

        /// <summary>
        /// Returns ID and display for for auto complete extender (jQuery) to for dynamic data sources
        /// </summary>
        /// <param name="currentUser">A current user object</param>
        /// <param name="maxRows">Number of records to return</param>
        /// <param name="matchTable">Table ID (Guid) to perform the look up on</param>
        /// <param name="displayField">Text field to display in the returned list</param>
        /// <param name="matchFields">Comma separated list of Field IDs (Guids) to perform wildcard match on </param>
        /// <param name="autocompleteDisplayResult">Comma separated list of Field IDs (Guids) to display in search results </param> 
        /// <param name="matchText">Text to match</param>
        /// <param name="useWildcards">Specify if the search should use wildcards or return specific values</param>
        /// <param name="filters">Specifiy the field filters to use with the Auto Complete</param>
        /// <param name="keyIsString">True if the key field is a string</param>
        /// <returns>Serializable structure of type CustomEntityAutoCompleteResult</returns>
        public static List<CustomEntityAutoCompleteResult> GetAutoCompleteMatches(ICurrentUserBase currentUser, int maxRows, string matchTable, string displayField, string matchFields, string autocompleteDisplayResult, string matchText, bool useWildcards, Dictionary<string, JSFieldFilter> filters, bool keyIsString, List<AutoCompleteChildFieldValues> childFilterList)
        {
            var retVals = new List<CustomEntityAutoCompleteResult>();
            Guid displayFieldId;

            if (!Guid.TryParseExact(displayField, "D", out displayFieldId))
            {
                return retVals;
            }

            var tables = new cTables();
            var fields = new cFields();
            var basetable = new cTable();
            List<Guid> matchFieldIDs;

            if (
                !InitialiseMatchingData(
                    currentUser,
                    matchTable,
                    matchFields,
                    ref matchText,
                    useWildcards,
                    out matchFieldIDs,
                    ref tables,
                    ref fields,
                    ref basetable))
            {
                return retVals;
            }

            if (basetable == null)
            {
                return retVals;
                
            }

            var qb = new cQueryBuilder(currentUser.AccountID,cAccounts.getConnectionString(currentUser.AccountID),ConfigurationManager.ConnectionStrings["metabase"].ConnectionString,basetable,tables, fields, childFilterList != null && childFilterList.Count > 0 ? 0 : maxRows);
                
            var fieldToDisplay = fields.GetFieldByID(displayFieldId);

            if (fieldToDisplay == null)
            {
                return retVals;
            }

            qb.addColumn(basetable.GetPrimaryKey());
            qb.addColumn(fieldToDisplay);

            Guid tmpGuid;
            var autocompleteDisplayFieldIds = new List<Guid>(from x in autocompleteDisplayResult.Split(',')
                                                             where Guid.TryParseExact(x, "D", out tmpGuid)
                                                             select new Guid(x));

            if (autocompleteDisplayFieldIds.Count > 0)
            {
                foreach (var formattedFieldlds in autocompleteDisplayFieldIds)
                {
                    var formattedFieldld = fields.GetFieldByID(formattedFieldlds);
                    qb.addColumn(formattedFieldld, formattedFieldld.Description);
                   }
            }

            var matchFieldFilters = new List<cQueryFilter>();

            // Add each of the Match Fields
            if (matchFieldIDs.Count > 0)
            {
                matchFieldFilters.AddRange(matchFieldIDs.Select(id => fields.GetFieldByID(id)).Select(curMatchField => new cQueryFilter(curMatchField, ConditionType.Like, new List<object> { matchText }, null, ConditionJoiner.Or, null)));
                qb.addFilterGroup(new cQueryFilterGroup(matchFieldFilters, ConditionJoiner.And));
            }

            // Add each of the filters
            if (filters != null)
            {
                foreach (var curFilter in filters.Values)
                {
                    if (curFilter.FilterOnEdit && childFilterList != null) continue;
                    var fieldFilter = FieldFilters.JsToCs(curFilter, currentUser);

                    if (fieldFilter == null)
                    {
                        continue;
                    }

                    var filterValues = FieldFilters.GetFilterValuesFromFieldFilter(fieldFilter, currentUser);

                    qb.addFilter(fieldFilter.Field, fieldFilter.Conditiontype, filterValues.valueOne, filterValues.valueTwo, ConditionJoiner.And, fieldFilter.JoinVia);

                }
            }

            if (basetable.GetSubAccountIDField() != null)
            {
                qb.addFilter(basetable.GetSubAccountIDField(), ConditionType.Equals, new object[] { currentUser.CurrentSubAccountId }, null, ConditionJoiner.And, null);
            }

            // make sure the match fields are present, or all records will be shown
            if (matchFieldFilters.Count <= 0)
            {
                return retVals;
            }

            using (var reader = qb.getReader())
            {
                while (reader.Read())
                {
                    CustomEntityAutoCompleteResult entry;
                    var formattedString = string.Empty;
                    if (!keyIsString)
                    {
                        if (reader.GetFieldType(0) == typeof(long) && reader.GetFieldType(1) == typeof(string))
                        {
                            entry.value = reader.GetInt64(0).ToString(CultureInfo.InvariantCulture);
                            entry.label = Convert.ToString(reader.GetValue(1));
                        }
                        else if (reader.GetFieldType(0) == typeof(long) && reader.GetFieldType(1) == typeof(long))
                        {
                            entry.value = reader.GetInt64(0).ToString(CultureInfo.InvariantCulture);
                            entry.label = reader.GetInt64(1).ToString(CultureInfo.InvariantCulture);
                        }
                        else if (reader.GetFieldType(0) == typeof(int) && reader.GetFieldType(1) == typeof(string))
                        {
                            entry.value = reader.GetInt32(0).ToString(CultureInfo.InvariantCulture);
                            entry.label = Convert.ToString(reader.GetValue(1));
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (reader.GetFieldType(0) != typeof(string) || reader.GetFieldType(1) != typeof(string))
                        {
                            continue;
                        }

                        entry.value = Convert.ToString(reader.GetValue(0));
                        entry.label = Convert.ToString(reader.GetValue(1));
                    }
                    formattedString = autocompleteDisplayFieldIds.Select(autocompleteDisplayField => fields.GetFieldByID(autocompleteDisplayField)).Where(formattedFields => formattedFields != null).Aggregate(formattedString, (current, formattedFields) => current + "," + (reader.IsDBNull(reader.GetOrdinal(formattedFields.Description)) == false ? reader[reader.GetOrdinal(formattedFields.Description)] : string.Empty));
                    entry.formattedText = string.Join(
                        ", ",
                        formattedString.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());
                   
                        retVals.Add(entry);
                }

                reader.Close();
            }

            if (childFilterList == null) return retVals;
            var requiredField = fields.GetFieldByName($"att{childFilterList[0].FieldToBuild}", true);

            if (requiredField == null || requiredField.LookupFieldID.ToString() != displayField) return retVals.Count > maxRows ? retVals.GetRange(0, maxRows) : retVals;
            retVals = retVals.Where(val => childFilterList.Any(child => child.Key.ToString() == val.value)).ToList();

            return retVals.Count > maxRows ? retVals.GetRange(0, maxRows) : retVals; 
        }



        /// <summary>
        /// Get the values for the autocomplete and related fields
        /// </summary>
        /// <param name="currentUser">
        /// The current User.
        /// </param>
        /// <param name="matchTableId">
        /// The match Table Id.
        /// </param>
        /// <param name="matchedId">
        /// The matched Id.
        /// </param>
        /// <param name="displayField">The display cField object</param>
        /// <param name="triggerFields">
        /// The trigger fields
        /// </param>
        /// <returns>
        /// The field string values
        /// </returns>
        public static List<Tuple<string, string>> GetDisplayAndTriggerFieldValues(ICurrentUserBase currentUser, Guid matchTableId, string matchedId, cField displayField, List<JsAutoCompleteTriggerField> triggerFields)
        {
            return GetDisplayOrTriggerFieldStringValues(currentUser, matchTableId, matchedId, triggerFields, displayField);
        }

        /// <summary>
        /// Get the values for trigger fields
        /// </summary>
        /// <param name="currentUser">
        /// The current User.
        /// </param>
        /// <param name="matchTableId">
        /// The match Table Id.
        /// </param>
        /// <param name="matchedId">
        /// The matched Id.
        /// </param>
        /// <param name="triggerFields">
        /// The trigger fields
        /// </param>
        /// <returns>
        /// The filters with their values correctly populated
        /// </returns>
        public static List<JsAutoCompleteTriggerField> GetTriggerFieldValues(ICurrentUserBase currentUser, Guid matchTableId, int matchedId, List<JsAutoCompleteTriggerField> triggerFields)
        {
            var values = GetDisplayOrTriggerFieldStringValues(currentUser, matchTableId, matchedId.ToString(), triggerFields);
            if (triggerFields != null)
            {
                if (values.Count == triggerFields.Count)
                {
                    for (var i = 0; i < triggerFields.Count; i++)
                    {
                        triggerFields[i].DisplayValue = values[i].Item1;
                        triggerFields[i].DocumentGuid = values[i].Item2;
                    }

                    return triggerFields;
                }
            }

            return new List<JsAutoCompleteTriggerField>();
        }

        /// <summary>
        /// Get the string values for the autocomplete and related fields
        /// </summary>
        /// <param name="currentUser">
        /// The current User.
        /// </param>
        /// <param name="matchTableId">
        /// The match Table Id.
        /// </param>
        /// <param name="matchedId">
        /// The matched Id.
        /// </param>
        /// <param name="triggerFields">
        /// The trigger fields
        /// </param>
        /// <param name="displayField">The display cField object</param>
        /// <returns>A list of string values</returns>
        private static List<Tuple<string, string>> GetDisplayOrTriggerFieldStringValues(ICurrentUserBase currentUser, Guid matchTableId, string matchedId, List<JsAutoCompleteTriggerField> triggerFields, cField displayField = null)
        {
            cTables tables = new cTables(currentUser.AccountID);
            cTable matchTable = tables.GetTableByID(matchTableId);
            var customEntityImageData = new CustomEntityImageData(currentUser.AccountID);
            if (matchTable == null || triggerFields == null)
            {
                return new List<Tuple<string, string>>();
            }

            cFields fields = new cFields(currentUser.AccountID);
            JoinVias joinVias = new JoinVias(currentUser);
            cQueryBuilder query = new cQueryBuilder(currentUser.AccountID, cAccounts.getConnectionString(currentUser.AccountID), GlobalVariables.MetabaseConnectionString, matchTable, tables, fields, 1);
            List<Tuple<cField, JoinVia>> fieldInstances = new List<Tuple<cField, JoinVia>>();

            if (displayField != null)
            {
                fieldInstances.Add(new Tuple<cField, JoinVia>(displayField, null));
            }

            foreach (AutoCompleteTriggerField triggerField in triggerFields.Select(javascriptTriggerField => javascriptTriggerField.ToC()))
            {
                cField field = fields.GetFieldByID(triggerField.DisplayFieldId);
                JoinVia joinVia = joinVias.GetJoinViaByID(triggerField.JoinViaId);

                if (field == null)
                {
                    break;
                }

                fieldInstances.Add(new Tuple<cField, JoinVia>(field, joinVia));
            }

            foreach (Tuple<cField, JoinVia> t in fieldInstances)
            {
                if (t.Item2 == null)
                {
                    query.addColumn(t.Item1);
                }
                else
                {
                    query.addColumn(t.Item1, t.Item2);
                }
            }

            // add the primary key with the filtering id
            query.addFilter(matchTable.GetPrimaryKey(), ConditionType.Equals, new object[] { matchedId }, null, ConditionJoiner.None, null);

            List<string> results = new List<string>();
            var lookUpFields = new List<Tuple<string, string>>();

            using (SqlDataReader reader = query.getReader())
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount && i < fieldInstances.Count; i++)
                    {
                        if (reader.IsDBNull(i))
                        {
                            AddLookUpDisplayFields(ref lookUpFields, string.Empty);
                            continue;
                        }

                        if (fieldInstances[i].Item1.ValueList && fieldInstances[i].Item1.ListItems.Count > 0)
                        {
                            AddLookUpDisplayFields(ref lookUpFields, reader.GetString(reader.GetOrdinal(fieldInstances[i].Item1.FieldName + (fieldInstances[i].Item2 != null ? "_" + fieldInstances[i].Item2.JoinViaID : string.Empty) + "_text")));
                            
                            continue;
                        }

                        switch (fieldInstances[i].Item1.FieldType)
                        {
                            case "D":
                                AddLookUpDisplayFields(ref lookUpFields, reader.GetDateTime(i).ToShortDateString());
                                break;
                            case "T":
                                AddLookUpDisplayFields(ref lookUpFields, reader.GetDateTime(i).ToString("HH:mm:ss"));
                                break;
                            case "DT":
                                AddLookUpDisplayFields(ref lookUpFields, reader.GetDateTime(i).ToString(CultureInfo.InvariantCulture));
                                break;
                            case "N":
                            case "FI":
                                AddLookUpDisplayFields(ref lookUpFields, reader.GetValue(i).ToString());
                                break;
                            case "C":
                            case "FD":
                            case "M":
                                AddLookUpDisplayFields(ref lookUpFields, reader.GetDecimal(i).ToString(CultureInfo.InvariantCulture));
                                break;
                            case "X":
                                AddLookUpDisplayFields(ref lookUpFields, reader.GetBoolean(i) ? "Yes" : "No");
                                break;
                            case "F":
                                AddLookUpDisplayFields(ref lookUpFields, reader.GetDouble(i).ToString(CultureInfo.InvariantCulture));
                                break;
                            case "AT":
                                var guid = reader.GetGuid(i);
                                var fileOutput = customEntityImageData.GetHtmlImageData(guid.ToString());
                                if (fileOutput != null)
                                {
                                    AddLookUpDisplayFields(ref lookUpFields, fileOutput.FileName, guid.ToString());
                                }
                                break;
                            case "B":
                                AddLookUpDisplayFields(ref lookUpFields, reader.GetInt64(i).ToString(CultureInfo.InvariantCulture));
                                break;
                            default:
                                AddLookUpDisplayFields(ref lookUpFields, reader.GetString(i));
                                break;
                        }
                    }
                }

                reader.Close();
            }

            return lookUpFields;
        }

        /// <summary>
        /// Add lookup display fields to the list for n:1 entity attribute.
        /// </summary>
        /// <param name="lookUpFields">
        /// The look up fields to be added.
        /// </param>
        /// <param name="fieldName">
        /// The field name of the attribute.
        /// </param>
        /// <param name="documentValue">
        /// The document value of the attribute.
        /// </param>
        private static void AddLookUpDisplayFields(ref List<Tuple<string, string>> lookUpFields, string fieldName, string documentValue = null)
        {
            lookUpFields.Add(new Tuple<string, string>(fieldName, documentValue));
        }

        /// <summary>
        /// Returns ID and display for for auto complete extender (jQuery) to for dynamic data sources
        /// </summary>
        /// <param name="currentUser">A current user object</param>
        /// <param name="matchTable">Table ID (Guid) to perform the look up on</param>
        /// <param name="displayFields">Text fields to display in the returned list</param>
        /// <param name="matchFields">Comma separated list of Field IDs (Guids) to perform wildcard match on </param>
        /// <param name="matchText">Text to match</param>
        /// <param name="useWildcards">Specify if the search should use wildcards or return specific values</param>
        /// <param name="filters">Specifiy the field filters to use with the Auto Complete</param>
        /// <param name="keyIsString">True if the key field is a string</param>
        /// <returns>Serializable structure of type sAutoComplete</returns>
        public static DataSet GetAutoCompleteMatches(ICurrentUserBase currentUser, string matchTable, List<Guid> displayFields, string matchFields, string matchText, bool useWildcards, Dictionary<string, JSFieldFilter> filters, bool keyIsString = false)
        {
           var returnValue = new DataSet();

            cTables tables = new cTables();
            cFields fields = new cFields();
            cTable basetable = new cTable();
            List<Guid> matchFieldIDs;

            if (InitialiseMatchingData(currentUser, matchTable, matchFields, ref matchText, useWildcards, out matchFieldIDs, ref tables, ref fields, ref basetable ))
            {
                if (basetable != null)
                {
                    cQueryBuilder qb = new cQueryBuilder(currentUser.AccountID,
                        cAccounts.getConnectionString(currentUser.AccountID),
                        ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, basetable,
                        tables, fields, 0);
                    qb.addColumn(basetable.GetPrimaryKey());
                    foreach (var displayField in displayFields)
                    {
                        cField fieldToDisplay = fields.GetFieldByID(displayField);
                        if (fieldToDisplay != null)
                        {
                            qb.addColumn(fieldToDisplay);
                        }
                    }
                    if ((qb.lstColumns != null) && (qb.lstColumns.Count > 0))
                    {
                        List<cQueryFilter> matchFieldFilters = new List<cQueryFilter>();

                        // Add each of the Match Fields
                        if (matchFieldIDs.Count > 0)
                        {
                            foreach (Guid id in matchFieldIDs)
                            {
                                cField curMatchField = fields.GetFieldByID(id);

                                matchFieldFilters.Add(new cQueryFilter(curMatchField, ConditionType.Like,
                                    new List<object> { matchText }, null, ConditionJoiner.Or, null));
                            }

                            qb.addFilterGroup(new cQueryFilterGroup(matchFieldFilters, ConditionJoiner.And));
                        }

                        // Add each of the filters
                        if (filters != null)
                        {
                            foreach (JSFieldFilter curFilter in filters.Values)
                            {
                                FieldFilter fieldFilter = FieldFilters.JsToCs(curFilter, currentUser);

                                if (fieldFilter == null)
                                {
                                    continue;
                                }

                                FieldFilters.FieldFilterValues filterValues =
                                    FieldFilters.GetFilterValuesFromFieldFilter(fieldFilter, currentUser);

                                qb.addFilter(fieldFilter.Field, fieldFilter.Conditiontype, filterValues.valueOne,
                                    filterValues.valueTwo, ConditionJoiner.And, fieldFilter.JoinVia);

                            }
                        }

                        if (basetable.GetSubAccountIDField() != null)
                        {
                            qb.addFilter(basetable.GetSubAccountIDField(), ConditionType.Equals,
                                new object[] { currentUser.CurrentSubAccountId }, null, ConditionJoiner.And, null);
                        }

                        // make sure the match fields are present, or all records will be shown
                        if (matchFieldFilters.Count > 0)
                        {
                            returnValue = qb.getDataset();

                        }

                    }
                }
            }

            return returnValue;
        }

        private static bool InitialiseMatchingData(ICurrentUserBase currentUser, string matchTable, string matchFields, ref string matchText, bool useWildcards, out List<Guid> matchFieldIDs, ref cTables tables, ref cFields fields, ref cTable basetable)
        {

            Guid tmpGuid, matchTableId;
            matchFieldIDs = new List<Guid>(from x in matchFields.Split(',')
                                           where Guid.TryParseExact(x, "D", out tmpGuid)
                                           select new Guid(x));

            matchText = useWildcards ? ("%" + matchText + "%") : matchText;

            if (Guid.TryParseExact(matchTable, "D", out matchTableId))
            {
                tables = new cTables(currentUser.AccountID);
                fields = new cFields(currentUser.AccountID);
                basetable = tables.GetTableByID(matchTableId);
                return true;
            }
            return false;
        }

        /// <summary>
        /// A method for getting the cost code auto complete results
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="displayFieldId">
        /// The display field id.
        /// </param>
        /// <param name="currentUser">
        /// The current user.
        /// </param>
        /// <param name="useWildCard">
        /// Whether to use a wildcard search. Will be false when the on blur event validates the entered costcode
        /// </param>
        /// <param name="filterRules">A list of <see cref="AutoCompleteChildFieldValues"/> containing the filter rules to apply</param>
        /// <returns>
        /// The a list of <see cref="sAutoCompleteResult"/>.
        /// </returns>
        private static List<sAutoCompleteResult> GetCostCodeAutoCompleteResults(string searchTerm, Guid displayFieldId, ICurrentUserBase currentUser, bool useWildCard, List<AutoCompleteChildFieldValues> filterRules)
        {
            List<int> costCodeFilterRuleIds = new List<int>();
            string inClause = string.Empty;

            if (filterRules != null && filterRules.Count > 0)
            {
                foreach (var rule in filterRules)
                {
                    costCodeFilterRuleIds.Add(rule.Key);
                }

                inClause = string.Join(",", costCodeFilterRuleIds.Select(x => x.ToString()));
            }

            var autoCompleteResults = new List<sAutoCompleteResult>();
            cFields fields = new cFields();
            cField fieldToDisplay = fields.GetFieldByID(displayFieldId);     
            string field = fieldToDisplay.FieldName;

            var sqlSelect = $"SELECT TOP 25 CostCodeId, {field} FROM CostCodes WHERE {field}";         
            var sqlNoWildCardWhereClause = " = @SearchTerm";
            var sqlUnarchived = " AND Archived = 0";
            var sqlUnion = " UNION ";
            var sqlLikeWithWildCard = " LIKE + @SearchTermWildCard";
            var sqlOrderBy = $" ORDER BY {field}";
            var sqlAndCostCodeIdIn = " AND CostCodeId IN (" + inClause + ")";

            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {        
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.AddWithValue("@SearchTerm", searchTerm.Replace("%", string.Empty));
                databaseConnection.AddWithValue("@SearchTermWildCard", "%" + searchTerm + "%");

                //determine which sql where clause to build up. 
                var sb = new StringBuilder();

                bool hasCostCodeFilterRuleIds = costCodeFilterRuleIds.Count > 0;

                if (!useWildCard)
                {
                    sb.Append($"{sqlSelect}{sqlNoWildCardWhereClause}{sqlUnarchived}");

                    if (hasCostCodeFilterRuleIds)
                    {
                        sb.Append(sqlAndCostCodeIdIn);
                    }         
                }
                else
                {
                    sb.Append($"{sqlSelect}{sqlNoWildCardWhereClause}{sqlUnarchived}");

                    if (hasCostCodeFilterRuleIds)
                    {
                        sb.Append(sqlAndCostCodeIdIn);
                    }

                    sb.Append($"{sqlUnion}{sqlSelect}{sqlLikeWithWildCard}{sqlUnarchived}");

                    if (hasCostCodeFilterRuleIds)
                    {
                        sb.Append(sqlAndCostCodeIdIn);
                    }

                    sb.Append(sqlOrderBy);
                }
   
                using (IDataReader reader = databaseConnection.GetReader(sb.ToString()))
                {
                    while (reader.Read())
                    {
                        var entry = new sAutoCompleteResult
                                        {
                                            value = reader.GetInt32(reader.GetOrdinal("costcodeid")).ToString(),
                                            label = reader.GetString(reader.GetOrdinal(field))
                                        };

                        autoCompleteResults.Add(entry);
                    }

                    reader.Close();
                }
            }

            return autoCompleteResults;
        }
    }
}
