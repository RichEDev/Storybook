namespace SpendManagementLibrary
{
    using System;

    /// <summary>
    /// Structure that contains information relevant to custom fields
    /// </summary>
    [Serializable()]
    public struct sCustomField
    {
        public string FieldID;
        public string FieldName;
        public string Description;
        public string DataType;
        public string TableID;
        public FieldCategory FieldCat;
        public string RelatedFieldID;
    }
}