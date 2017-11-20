using System;
using System.Collections.Generic;
using System.Text;

namespace SpendManagementLibrary
{
    public class cProvider
    {
        private int nProviderid;
        private string sName;

        public cProvider(int providerid, string name)
        {
            nProviderid = providerid;
            sName = name;
        }

        #region properties
        public int providerid
        {
            get { return nProviderid; }
        }
        public string name
        {
            get { return sName; }
        }
        #endregion
    }
}
