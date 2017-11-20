namespace SpendManagementLibrary
{
    using System;

    /// <summary>
    /// The custom entity auto complete result.
    /// </summary>
    [Serializable()]
    public struct CustomEntityAutoCompleteResult
    {
        /// <summary>
        /// The label to display search result.Field value of the display field for n:1 attribute.
        /// </summary>
        public string label;

        /// <summary>
        /// The Id of the result data 
        /// </summary>
        public string value;

        /// <summary>
        /// The formatted text with the concatenated fields values set at autolookup display fields for custom entity
        /// </summary>
        public string formattedText;
    }
}
