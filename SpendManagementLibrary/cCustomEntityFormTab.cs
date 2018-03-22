namespace SpendManagementLibrary
{
    using System.Collections.Generic;
    using System.Linq;

    public class cCustomEntityFormTab
    {
        private int nTabid;
        private int nFormid;
        private string sHeaderCaption;
        private byte nOrder;
        private List<cCustomEntityFormSection> lstSections = new List<cCustomEntityFormSection>();

        public cCustomEntityFormTab(int tabid, int formid, string headercaption, byte order)
        {
            this.nTabid = tabid;
            this.nFormid = formid;
            this.sHeaderCaption = headercaption;
            this.nOrder = order;
        }

        public SortedList<byte, cCustomEntityFormSection> getSectionsForTab()
        {
            return this.sortSections();
        }

        private SortedList<byte, cCustomEntityFormSection> sortSections()
        {
            SortedList<byte, cCustomEntityFormSection> sections = new SortedList<byte, cCustomEntityFormSection>((from x in this.lstSections
                select x).ToDictionary(a => a.order));
            //foreach (cCustomEntityFormSection section in lstSections)
            //{
            //    sections.Add(section.order, section);
            //}
            return sections;
        }
        #region properties
        public int tabid
        {
            get { return this.nTabid; }
            set { this.nTabid = value; }
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
        public List<cCustomEntityFormSection> sections
        {
            get { return this.lstSections; }
        }
        #endregion
    }
}