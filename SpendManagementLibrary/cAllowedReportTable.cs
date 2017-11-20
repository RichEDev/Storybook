using System;
using System.Collections.Generic;
using System.Text;

namespace SpendManagementLibrary
{
    public class cAllowedReportTable
    {
        private int nBaseTableid;
        private List<int> lstTables;

        public cAllowedReportTable(int basetable, List<int> tables)
        {
            nBaseTableid = basetable;
            lstTables = tables;
        }

        #region properties
        public int basetableid
        {
            get {return nBaseTableid;}
        }
        public List<int> tables
        {
            get { return lstTables; }
        }
        #endregion

        public bool AllowsTable(int id)
        {
            return lstTables.Contains(id);
        }
    }
}
