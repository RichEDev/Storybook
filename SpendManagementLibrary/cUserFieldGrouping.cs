using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    public class cUserdefinedFieldGrouping
    {
        private int nUserdefinedGroupID;
        private string sGroupName;
        private int nOrder;
        private cTable clsAssociatedTable;
        private Dictionary<int, List<int>> dlFilterCategories;
        private DateTime dtCreatedOn;
        private int nCreatedBy;
        private DateTime? dtModifiedOn;
        private int? nModifiedBy;

        public cUserdefinedFieldGrouping(int userdefinedgroupid, string groupname, int order, cTable associatedtable, Dictionary<int,List<int>> filter_categories, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby)
        {
            nUserdefinedGroupID = userdefinedgroupid;
            sGroupName = groupname;
            nOrder = order;
            clsAssociatedTable = associatedtable;
            dlFilterCategories = filter_categories;
            dtCreatedOn = createdon;
            nCreatedBy = createdby;
            dtModifiedOn = modifiedon;
            nModifiedBy = modifiedby;
        }

        #region properties
        public int UserdefinedGroupID
        {
            get { return nUserdefinedGroupID; }
        }
        public string GroupName
        {
            get { return sGroupName; }
        }
        public int Order
        {
            get { return nOrder; }
            set { nOrder = value; }
        }
        public cTable AssociatedTable
        {
            get { return clsAssociatedTable; }
        }
        public Dictionary<int, List<int>> FilterCategories
        {
            get { return dlFilterCategories; }
        }
        public DateTime CreatedOn
        {
            get { return dtCreatedOn; }
        }
        public int CreatedBy
        {
            get { return nCreatedBy; }
        }
        public DateTime? ModifiedOn
        {
            get { return dtModifiedOn; }
        }
        public int? ModifiedBy
        {
            get { return nModifiedBy; }
        }
        #endregion
    }
}
