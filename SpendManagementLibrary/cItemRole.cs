using System;
using System.Collections.Generic;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cItemRole
    {
        private int nItemroleid;
        private string sRolename;
        private string sDescription;
        private Dictionary<int, cRoleSubcat> lstsubcats;
        private DateTime dtCreatedon;
        private int nCreatedby;
        private DateTime dtModifiedon;
        private int nModifiedby;

        public cItemRole(int itemroleid, string rolename, string description, Dictionary<int, cRoleSubcat> subcats, DateTime createdon, int createdby, DateTime modifiedon, int modifiedby)
        {
            nItemroleid = itemroleid;
            sRolename = rolename;
            sDescription = description;
            lstsubcats = subcats;
            dtCreatedon = createdon;
            nCreatedby = createdby;
            dtModifiedon = modifiedon;
            nModifiedby = modifiedby;
        }

        public void deleteSubcat(int subcatid)
        {
            lstsubcats.Remove(subcatid);

        }
        #region properties
        public int itemroleid
        {
            get { return nItemroleid; }
        }
        public string rolename
        {
            get { return sRolename; }
        }
        public string description
        {
            get { return sDescription; }
        }
        public Dictionary<int, cRoleSubcat> items
        {
            get { return lstsubcats; }
            set { lstsubcats = value; }
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
    public class cRoleSubcat
    {
        private int nRolesubcatid;
        private int nRoleid;
        private decimal dMaximum;
        private decimal dReceiptmaximum;
        private bool bIsadditem;

        public cRoleSubcat(int rolesubcatid, int roleid, int subCatId, decimal maximum, decimal receiptmaximum, bool isadditem)
        {
            nRolesubcatid = rolesubcatid;
            nRoleid = roleid;
            this.SubcatId = subCatId;
            dMaximum = maximum;
            dReceiptmaximum = receiptmaximum;
            bIsadditem = isadditem;
        }

        #region properties
        public int rolesubcatid
        {
            get { return nRolesubcatid; }
        }
        public int roleid
        {
            get { return nRoleid; }
        }

        public int SubcatId { get; private set; }

        public decimal maximum
        {
            get { return dMaximum; }
            set
            {
                dMaximum = value;
            }
        }
        public decimal receiptmaximum
        {
            get { return dReceiptmaximum; }
            set
            {
                dReceiptmaximum = value;
            }
        }
        public bool isadditem
        {
            get { return bIsadditem; }
        }
        #endregion
    }

    [Serializable()]
    public struct sOnlineItemRoleInfo
    {
        public Dictionary<int, cItemRole> lstonlineitemroles;
        public List<int> lstitemroleids;
    }
}
