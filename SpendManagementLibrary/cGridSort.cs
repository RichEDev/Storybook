using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable]
    public class cNewGridSort
    {
        private string sGridID;
        private Guid gSortedColumn;
        SpendManagementLibrary.SortDirection eSortDirection;
        private int nJoinViaID;

        public cNewGridSort(string gridid, Guid sortedcolumn, SpendManagementLibrary.SortDirection sortdirection, int joinViaID)
        {
            sGridID = gridid;
            gSortedColumn = sortedcolumn;
            eSortDirection = sortdirection;
            nJoinViaID = joinViaID;
        }

        #region properties
        public string GridID
        {
            get { return sGridID; }
        }
        public Guid SortedColumn
        {
            get { return gSortedColumn; }
        }
        public SpendManagementLibrary.SortDirection SortDirection
        {
            get { return eSortDirection; }
        }
        public int JoinViaID
        {
            get { return nJoinViaID; }
        }
        #endregion
    }
}
