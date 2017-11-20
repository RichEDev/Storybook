using System;
using System.Collections.Generic;
using System.Text;
using SpendManagementLibrary;

namespace ExpensesLibrary
{
    public class cAutomatedImport
    {
        protected int nImportid;
        protected int nAccountid;
        protected cTable clsProductArea;

        #region properties
        public int importid
        {
            get { return nImportid; }
        }
        public cTable productarea
        {
            get { return clsProductArea; }
        }
        public int accountid
        {
            get { return nAccountid; }
        }
        #endregion

        public void setImportId(int id)
        {
            nImportid = id;
        }
    }

    public class cProviderAutomatedImport : cAutomatedImport
    {
        int nProviderid;

        public cProviderAutomatedImport (int importid, int accountid, cTable productarea, int providerid)
        {
            nImportid = importid;
            nAccountid = accountid;
            clsProductArea = productarea;
            nProviderid = providerid;
        }

        #region properties
        public int providerid
        {
            get { return nProviderid; }
        }
        #endregion

    }
}
