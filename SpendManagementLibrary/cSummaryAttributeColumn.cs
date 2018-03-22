namespace SpendManagementLibrary
{
    using System;
    using SpendManagementLibrary.Definitions.JoinVia;

    /// <summary>
    /// Used to identify a column to be displayed from the related entity in a cSummaryAttribute
    /// </summary>
    public class cSummaryAttributeColumn
    {
        private int nColumnId;
        private int nColumnAttributeID;
        private string sAlternateHeader;
        private int nWidth;
        private int nOrder;
        private bool bDefaultSort;
        private string sFilterValue;
        private bool bIsManyToOne;
        private Guid gDisplayFieldId;
        private JoinVia oJoinVia;

        #region properties
        public int ColumnAttributeID
        {
            get { return this.nColumnAttributeID; }
        }
        public int ColumnId
        {
            get { return this.nColumnId; }
        }
        public string AlternateHeader
        {
            get { return this.sAlternateHeader; }
        }
        public int Width
        {
            get { return this.nWidth; }
        }
        public int Order
        {
            get { return this.nOrder; }
        }
        public bool DefaultSort
        {
            get { return this.bDefaultSort; }
        }
        public string FilterValue
        {
            get { return this.sFilterValue; }
        }

        public bool IsMTOField
        {
            get { return this.bIsManyToOne; }
        }

        public Guid DisplayFieldId
        {
            get { return this.gDisplayFieldId; }
        }

        public JoinVia JoinViaObj
        {
            get { return this.oJoinVia; }
            set { this.oJoinVia = value; }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnid"></param>
        /// <param name="columnattributeid"></param>
        /// <param name="alternate_header"></param>
        /// <param name="width"></param>
        /// <param name="order"></param>
        /// <param name="sortColumn"></param>
        /// <param name="filter_value"></param>
        /// <param name="ismanytooneattribute"></param>
        /// <param name="displayfieldid"></param>
        /// <param name="joinviaObj"></param>
        public cSummaryAttributeColumn(int columnid, int columnattributeid, string alternate_header, int width, int order, bool sortColumn, string filter_value, bool ismanytooneattribute, Guid displayfieldid, JoinVia joinviaObj)
        {
            this.nColumnAttributeID = columnattributeid;
            this.nColumnId = columnid;
            this.sAlternateHeader = alternate_header;
            this.nWidth = width;
            this.nOrder = order;
            this.bDefaultSort = sortColumn;
            this.sFilterValue = filter_value;
            this.bIsManyToOne = ismanytooneattribute;
            this.gDisplayFieldId = displayfieldid;
            this.oJoinVia = joinviaObj;
        }
    }
}