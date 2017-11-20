using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable]
    public class cLocale
    {
        private int nLocaleID;
        private string sLocaleName;
        private string sLocaleCode;
        private bool bActive;

        public cLocale(int localeid, string localename, string localecode, bool active)
        {
            nLocaleID = localeid;
            sLocaleName = localename;
            sLocaleCode = localecode;
            bActive = active;
        }

        #region properties
        /// <summary>
        /// Gets the unique identifer of the locale
        /// </summary>
        public int LocaleID
        {
            get { return nLocaleID; }
        }
        /// <summary>
        /// The readable name of the locale
        /// </summary>
        public string LocaleName
        {
            get { return sLocaleName; }
        }
        /// <summary>
        /// The code of the locale. This will be used to set the users browser locale
        /// </summary>
        public string LocaleCode
        {
            get { return sLocaleCode; }
        }
        /// <summary>
        /// Gets whether the locale is active. Only active locales can be selected for use
        /// </summary>
        public bool Active
        {
            get { return bActive; }
        }
        #endregion
    }
}
