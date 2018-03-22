namespace SpendManagementLibrary
{
    using System;

    [Serializable()]
    public struct sSummaryColumn
    {
        public int columnid;
        public int attributeid;
        public int columnAttributeID;
        public string alt_header;
        public int width;
        public int order;
        public bool default_sort;
        public string filterVal;
        public bool ismtoattribute;
        public string displayFieldId;
        public string displayFieldName;
        public int JoinViaID;
    }
}