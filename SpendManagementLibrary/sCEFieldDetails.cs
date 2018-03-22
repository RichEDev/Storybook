namespace SpendManagementLibrary
{
    /// <summary>
    /// Stores the field details for the custom entity forms
    /// </summary>
    public struct sCEFieldDetails
    {
        public int AttributeID;
        public string ControlName;
        public string DisplayName;
        public string Description;
        public string Tooltip;
        public string LabelText;
        public bool Mandatory;
        public FieldType FieldType;
        public bool ReadOnly;
        public byte Row;
        public byte Column;
        public AttributeFormat Format;
        public string CommentText;
        public int RelationshipType;
        public int ColumnSpan;
        public string FieldValue;
        public string SortDisplayName;
        public string DefaultValue;
        public int MaxLength;
        public bool? MandatoryCheckOverride;
    }
}