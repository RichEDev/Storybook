namespace SpendManagementLibrary
{
    public enum FieldType
    {
        NotSet = 0,
        Text = 1,
        Integer = 2,
        DateTime = 3,
        List = 4,
        TickBox = 5,
        Currency = 6,
        Number = 7,
        Hyperlink = 8,
        Relationship = 9,
        LargeText = 10,
        RunWorkflow = 11,
        RelationshipTextbox = 12,
        AutoCompleteTextbox = 13,
        /*Message = 14, */
        OTMSummary = 15,
        DynamicHyperlink = 16,
        CurrencyList = 17,
        Comment = 19,
        Spacer = 20,    
        /// <summary>
        /// A readonly display field that shows data from a related Many to One
        /// </summary>
        LookupDisplayField = 21,
        Attachment = 22,

        /// <summary>
        /// A field that shows contact information
        /// </summary>
        Contact = 23
    }
}
