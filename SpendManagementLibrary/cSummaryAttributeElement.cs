namespace SpendManagementLibrary
{
    /// <summary>
    /// Used to identify the individual one to many attributes to be summarised in a cSummaryAttribute
    /// </summary>
    public class cSummaryAttributeElement
    {
        private int nSummaryAttributeId;
        private int nAttributeId;
        private int nOTMAttributeId;
        private int nOrder;

        #region properties
        public int SummaryAttributeId
        {
            get { return this.nSummaryAttributeId; }
        }
        public int AttributeId
        {
            get { return this.nAttributeId; }
        }
        public int OTM_AttributeId
        {
            get { return this.nOTMAttributeId; }
        }
        public int Order
        {
            get { return this.nOrder; }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="summaryattributeid"></param>
        /// <param name="attributeid"></param>
        /// <param name="otm_attributeid"></param>
        /// <param name="order"></param>
        public cSummaryAttributeElement(int summaryattributeid, int attributeid, int otm_attributeid, int order)
        {
            this.nSummaryAttributeId = summaryattributeid;
            this.nAttributeId = attributeid;
            this.nOTMAttributeId = otm_attributeid;
            this.nOrder = order;
        }
    }
}