using System;
using System.Collections.Generic;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cCategory
    {
        private int nCategoryid;
        private string sCategory;
        private string sDescription;
        private DateTime dtCreatedon;
        private int nCreatedby;
        private DateTime dtModifiedon;
        private int nModifiedby;

        public cCategory(int categoryid, string category, string description, DateTime createdon, int createdby, DateTime modifiedon, int modifiedby)
        {
            nCategoryid = categoryid;
            sCategory = category;
            sDescription = description;
            dtCreatedon = createdon;
            nCreatedby = createdby;
            dtModifiedon = modifiedon;
            nModifiedby = modifiedby;
        }

        #region properties
        public int categoryid
        {
            get { return nCategoryid; }
        }
        public string category
        {
            get { return sCategory; }
        }
        public string description
        {
            get { return sDescription; }
        }
        public DateTime createdon
        {
            get { return dtCreatedon; }
        }
        public int createdby
        {
            get { return nCreatedby; }
        }
        public DateTime modifiedon
        {
            get { return dtModifiedon; }
        }
        public int modifiedby
        {
            get { return nModifiedby; }
        }
        #endregion
    }

    [Serializable()]
    public struct sOnlineCatInfo
    {
        public Dictionary<int, cCategory> lstonlinecats;
        public List<int> lstcatids;
    }
}
