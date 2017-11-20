using System;
using System.Collections.Generic;
using System.Linq;

namespace SpendManagementApi.Utilities
{
	using System.Globalization;
	using System.Text.RegularExpressions;

	using SpendManagementApi.Interfaces;
	using SpendManagementApi.Models.Common;
	using SpendManagementApi.Models.Types;

	using SpendManagementLibrary;

	using Spend_Management;

	internal class UdfValidator
	{
	    /// <summary>
	    /// Validates that a specified list of udfs apply to the element in context and the values specified if a list type are valid values 
	    /// </summary>
	    /// <param name="udfValueList">Sorted list of udf values</param>
	    /// <param name="actionContext">Action Context</param>
	    /// <param name="tableName">Table name</param>
	    /// <param name="checkMandatoryFields">Specifies whether to ensure mandatory fields.</param>
	    /// <param name="expenseItemAssociatedUserDefinedIds">Used when the user defined field is part of an expense item. Contains a list of associated user defined fields for the associated subcat . Pass null of the the user defined field is not associated with an expense item.</param>
	    internal static List<UserDefinedFieldValue> Validate(List<UserDefinedFieldValue> udfValueList, IActionContext actionContext, string tableName, bool checkMandatoryFields = true, List<int> expenseItemAssociatedUserDefinedIds = null)
		{
			if (udfValueList == null)
			{
                udfValueList = new List<UserDefinedFieldValue>();
			}

			// Validate that all user defined ids specified are valid
			Validate(udfValueList.Select(u => u.Id).ToList(), actionContext, tableName, checkMandatoryFields, expenseItemAssociatedUserDefinedIds);

			var filteredUdfList = ValidateValuesAgainstFieldType(udfValueList, actionContext);

			ValidateListItems(filteredUdfList, actionContext);

			return filteredUdfList;
		}

        private static List<UserDefinedFieldValue> ValidateValuesAgainstFieldType(
            IReadOnlyList<UserDefinedFieldValue> valueList, IActionContext actionContext)
		{
            var filteredValueList = new List<UserDefinedFieldValue>();

			foreach (var udfId in valueList.Select(x => x.Id))
			{
				cUserDefinedField udf = actionContext.UserDefinedFields.GetUserDefinedById(udfId);
			    var userDefinedFieldValue = valueList.FirstOrDefault(target => target.Id == udfId);
			 
                if (userDefinedFieldValue != null)
			    {
			        var currentValue = userDefinedFieldValue.Value;

                    if (udf.mandatory == true && currentValue == null) 
			        {
                        throw new ApiException(ApiResources.InvalidUdf, ApiResources.InvalidNullValueForMandatory);
                    }
                   
                    if(udf.mandatory == true || currentValue != null)
                    {
			            string currentValueString = currentValue.ToString();
			            switch (udf.attribute.fieldtype)
			            {
			                case FieldType.Integer:
			                    try
			                    {
			                        int.Parse(currentValueString);
			                    }
			                    catch (Exception)
			                    {
			                        throw new ApiException(ApiResources.InvalidUdf, ApiResources.InvalidIntValueMessage);
			                    }

			                    break;
			                case FieldType.Currency:
			                    try
			                    {
			                        decimal.Parse(currentValueString);
			                    }
			                    catch (Exception)
			                    {
			                        throw new ApiException(
			                            ApiResources.InvalidUdf, ApiResources.InvalidCurrencyValueMessage);
			                    }

			                    break;
			                case FieldType.Number:
			                    try
			                    {
			                        decimal.Parse(currentValueString);
			                    }
			                    catch (Exception)
			                    {
			                        throw new ApiException(ApiResources.InvalidUdf, ApiResources.InvalidNumberValueMessage);
			                    }

			                    break;
			                case FieldType.DateTime:
			                    var v = (cDateTimeAttribute)udf.attribute;
			                    DateTime dateTime = DateTime.UtcNow;

			                    switch (v.format)
			                    {
			                        case AttributeFormat.DateOnly:
			                            if (
			                                !DateTime.TryParseExact(
			                                    currentValueString, "yyyy-MM-dd", null, DateTimeStyles.None, out dateTime))
			                            {
			                                throw new ApiException(
			                                    ApiResources.InvalidUdf, ApiResources.InvalidDateOnlyValueMessage);
			                            }

			                            break;
			                        case AttributeFormat.TimeOnly:
			                            if (
			                                !DateTime.TryParseExact(
			                                    currentValueString, "HH:mm:ss", null, DateTimeStyles.None, out dateTime))
			                            {
			                                throw new ApiException(
			                                    ApiResources.InvalidUdf, ApiResources.InvalidTimeOnlyValueMessage);
			                            }

			                            break;
			                        case AttributeFormat.DateTime:
			                            if (
			                                !DateTime.TryParseExact(
			                                    currentValueString,
			                                    "yyyy-MM-dd HH:mm:ss",
			                                    null,
			                                    DateTimeStyles.None,
			                                    out dateTime))
			                            {
			                                throw new ApiException(
			                                    ApiResources.InvalidUdf, ApiResources.InvalidDateTimeValueMessage);
			                            }

			                            break;
			                    }

			                    break;
			                case FieldType.DynamicHyperlink:
			                    const string RegexPattern = @"^(https?|ftps?)://(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?.*|^mailto:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
			                    if (!Regex.IsMatch(currentValueString, RegexPattern))
			                    {
			                        throw new ApiException(ApiResources.InvalidUdf, ApiResources.InvalidUdfMessage);
			                    }

			                    break;
			                case FieldType.List:
			                    int a;

			                    if (!int.TryParse(currentValueString, NumberStyles.Integer, null, out a))
			                    {
			                        throw new ApiException(
			                            ApiResources.InvalidUdf, ApiResources.InvalidListItemIdValueMessage);
			                    }

			                    break;
			                case FieldType.Relationship:
			                    var attrib = ((cManyToOneRelationship)udf.attribute);
			                    var vals = AutoComplete.GetAutoCompleteMatches(
			                        cMisc.GetCurrentUser(),
			                        0,
			                        attrib.relatedtable.TableID.ToString(),
			                        attrib.AutoCompleteDisplayField.ToString(),
			                        string.Join(",", attrib.AutoCompleteMatchFieldIDList),
			                        string.Empty,
			                        true,
			                        null);

			                    if (vals.All(val => val.value != currentValueString))
			                    {
			                        throw new ApiException(
			                            ApiResources.InvalidUdf, ApiResources.InvalidRelationshipValueMessage);
			                    }

			                    break;
			                case FieldType.TickBox:
			                    bool bVal;

			                    if (!bool.TryParse(currentValueString, out bVal))
			                    {
			                        throw new ApiException(ApiResources.InvalidUdf, ApiResources.InvalidTickboxValueMessage);
			                    }

			                    break;
			            }

			            filteredValueList.Add(new UserDefinedFieldValue(udfId, currentValue));                 
			        }
			    }
			}

			return filteredValueList;
		}

        private static void ValidateListItems(List<UserDefinedFieldValue> udfValueList, IActionContext actionContext)
		{
			// Get udfs that are list attributes
			List<UserDefinedFieldValue> udfListAttributeIds = udfValueList.Where(udfId => actionContext.UserDefinedFields.GetUserDefinedById(udfId.Id).attribute.fieldtype == FieldType.List).ToList();

			if (!udfListAttributeIds.Any())
			{
				return;
			}

			// Get values provided for all udfs that are lists
			Dictionary<int, object> values = udfValueList.Where(udfListAttributeIds.Contains).ToDictionary(k => k.Id, v => v.Value);

			if (!values.Any())
			{
				return;
			}

			// Check that the values provided are valid
			foreach (var key in values.Keys)
			{
				cUserDefinedField udf = actionContext.UserDefinedFields.GetUserDefinedById(key);
				int val;

				try
				{
					val = int.Parse(values[key].ToString());
				}
				catch
				{
					throw new ApiException(ApiResources.InvalidUdf, ApiResources.InvalidUdfListItemMessage);
				}
				if (!udf.items.ContainsKey(val) || (udf.items.ContainsKey(val) && udf.items[val] == null))
				{
					throw new ApiException(ApiResources.InvalidUdf, ApiResources.InvalidUdfListItemMessage);
				}

				if (udf.items.ContainsKey(val) && udf.items[val].Archived)
				{
					throw new ApiException(ApiResources.InvalidUdf, ApiResources.ArchivedUdfListItemMessage);
				}
			}
		}

		/// <summary>
		/// Validates that the specified list of udfs apply to the elements they're being applied against 
		/// </summary>
		/// <param name="udfIds">List of UDF ids</param>
		/// <param name="actionContext">Action Context</param>
		/// <param name="tableName">Element table name</param>
		/// <param name="checkMandatoryFields">Specifies whether to enforce mandatory fields</param>
        /// /// <param name="expenseItemAssociatedUserDefinedIds">Used when the user defined field is part of an expense item. Contains a list of associated user defined fields for the associated subcat . Pass null of the the user defined field is not associated with an expense item.</param>
		internal static void Validate(List<int> udfIds, IActionContext actionContext, string tableName, bool checkMandatoryFields = true, List<int> expenseItemAssociatedUserDefinedIds = null)
		{
			udfIds = udfIds ?? new List<int>();

			if (actionContext == null)
			{
				throw new ApiException("No action context", "No action context");    
			}

			List<cUserDefinedField> udfs = actionContext.UserDefinedFields.GetFieldsByTable(actionContext.Tables.GetTableByName(tableName));
			
			if (!udfIds.All(udfId => (udfs.Select(udf => udf.userdefineid).Contains(udfId))))
			{
				throw new ApiException(ApiResources.InvalidUdf, ApiResources.InvalidUdfMessage);
			}

			if (checkMandatoryFields)
			{
				List<int> mandatoryUdfIds = udfs.Where(udf => udf.attribute.mandatory & !udf.Archived).Select(udf => udf.userdefineid).ToList();

                // if we are dealing with expense item udf's, then mandatory fields may only actually be a subset of the udf id's specified in "additional fields via administration".
                if (tableName == "userdefinedExpenses" && expenseItemAssociatedUserDefinedIds != null)
                {
                    mandatoryUdfIds = mandatoryUdfIds.Intersect(expenseItemAssociatedUserDefinedIds).ToList();
                }

			    if (mandatoryUdfIds.Any(udfId => !udfIds.Contains(udfId)))
			    {
			        throw new ApiException(ApiResources.InvalidUdf, ApiResources.MandatoryUdfMissingMessage);
			    }
			}
		}
	}
}