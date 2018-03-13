namespace SpendManagementApi.Repositories
{
    using System;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Logic_Classes.Fields;

    /// <summary>
    /// A class to return value objects from <see cref="cAttribute"/> instances
    /// </summary>
    internal static class AttributeValueFactory
    {
        /// <summary>
        /// Convert a given <see cref="object"/> value to another value where necessary.
        /// </summary>
        /// <param name="attribute">The instance of <see cref="cAttribute"/>that this value relates to</param>
        /// <param name="value">The value of the attribute</param>
        /// <param name="fields">An instance of <see cref="IFields"/></param>
        /// <returns>A corrected value based on the attribute type.</returns>
        public static object Convert(cAttribute attribute, object value, IFields fields)
        {
            if (attribute is cHyperlinkAttribute)
            {
                return value.ToString();    
            }

            if (attribute is cTextAttribute)
            {
                return value.ToString();   
            }

            if (attribute is cDateTimeAttribute)
            {
                return DateTime.ParseExact(value.ToString(), "dd/MM/yyyy", null);
            }

            if (attribute is cListAttribute && value is string)
            {
                var listAttribute = attribute as cListAttribute;
                var searchText = value.ToString().ToLower().Replace("-", "");
                foreach (cListAttributeElement element in listAttribute.items.Values)
                {
                    if (element.elementText.ToLower().Replace("-", "").Replace(" ", string.Empty) == searchText)
                    {
                        return element.elementValue;
                    }
                }
            }

            if (attribute is cManyToOneRelationship && value is string)
            {
                var relationShip = attribute as cManyToOneRelationship;
                
                return FindRelatedKeyValue(relationShip, value.ToString(), fields);
            }

            if (attribute is cTickboxAttribute)
            {
                return value.ToString().ToLower() == "true" || value.ToString().ToLower() == "1";
            }

            return value;
        }

        /// <summary>
        /// Create a query to get matching value from the text entered.
        /// </summary>
        /// <param name="relationShip">An instance of <see cref="cManyToOneRelationship"/></param>
        /// <param name="text">The string to search for</param>
        /// <param name="fields">An instance of <see cref="IFields"/></param>
        /// <returns></returns>
        public static int FindRelatedKeyValue(cManyToOneRelationship relationShip, string text, IFields fields)
        {
            var result = fields.SearchFieldByFieldID(relationShip.AutoCompleteDisplayField, ConditionType.Like, text, 10);
            if (result.Length >0)
            {
                return int.Parse(result[0]);
            }
            
            return 0;
        }
    }
}