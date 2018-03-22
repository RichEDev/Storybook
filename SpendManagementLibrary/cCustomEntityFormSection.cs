namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;

    public class cCustomEntityFormSection
    {
        private int nSectionid;
        private int nFormid;
        private string sHeaderCaption;
        private byte nOrder;
        [NonSerialized()]
        private cCustomEntityFormTab clsTab;
        private List<cCustomEntityFormField> lstFields = new List<cCustomEntityFormField>();

        public cCustomEntityFormSection(int sectionid, int formid, string headercaption, byte order, cCustomEntityFormTab tab)
        {
            this.nSectionid = sectionid;
            this.nFormid = formid;
            this.sHeaderCaption = headercaption;
            this.nOrder = order;
            this.clsTab = tab;
        }

        public List<cCustomEntityFormField> getFieldsForSection()
        {
            return this.lstFields;
        }
        #region properties
        public int sectionid
        {
            get { return this.nSectionid; }
            set { this.nSectionid = value; }
        }
        public int formid
        {
            get { return this.nFormid; }
        }
        public string headercaption
        {
            get { return this.sHeaderCaption; }
        }
        public byte order
        {
            get { return this.nOrder; }
        }

        public cCustomEntityFormTab tab
        {
            get { return this.clsTab; }
        }
        public List<cCustomEntityFormField> fields
        {
            get { return this.lstFields; }
        }
        #endregion
    }
}