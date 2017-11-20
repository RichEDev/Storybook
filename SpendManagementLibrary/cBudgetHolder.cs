using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    using SpendManagementLibrary.Interfaces;

    [Serializable]
    public class cBudgetHolder : IOwnership
    {
        private int nBudgetHolderID;
        private string sBudgetHolder;
        private string sDescription;
        private int nEmployeeid;
        private int? nCreatedBy;
        private DateTime? dtCreatedOn;
        private int? nModifiedBy;
        private DateTime? dtModifiedOn;

        public cBudgetHolder(int budgetholderid, string budgetholder, string description, int employeeid, int? createdby, DateTime? createdon, int? modifiedby, DateTime? modifiedon)
        {
            nBudgetHolderID = budgetholderid;
            sBudgetHolder = budgetholder;
            sDescription = description;
            nEmployeeid = employeeid;
            nCreatedBy = createdby;
            dtCreatedOn = createdon;
            nModifiedBy = modifiedby;
            dtModifiedOn = modifiedon;
        }

        #region properties
        public int budgetholderid
        {
            get { return nBudgetHolderID; }
        }
        public string budgetholder
        {
            get { return sBudgetHolder; }
        }
        public string description
        {
            get { return sDescription; }
        }
        public int employeeid
        {
            get { return nEmployeeid; }
        }
        public int? createdby
        {
            get { return nCreatedBy; }
        }
        public DateTime? createdon
        {
            get { return dtCreatedOn; }
        }
        public DateTime? modifiedon
        {
            get { return dtModifiedOn; }
        }
        public int? modifiedby
        {
            get { return nModifiedBy; }
        }
        #endregion

        public virtual int ItemPrimaryID()
        {
            return nBudgetHolderID;
        }

        public virtual string ItemDefinition()
        {
            return string.Format("{0} (Budget Holder)", sBudgetHolder);
        }

        public virtual SpendManagementElement ElementType()
        {
            return SpendManagementElement.BudgetHolders;
        }

        public virtual int? OwnerId()
        {
            return budgetholderid;
        }

        public virtual string OwnerDefinition()
        {
            return string.Empty;
        }

        public virtual SpendManagementElement OwnerElementType()
        {
            return SpendManagementElement.BudgetHolders;
        }

        public string CombinedItemKey
        {
            get
            {
                return string.Format("{0},{1}", (int)this.ElementType(), this.ItemPrimaryID());
            }
        }

        public string CombinedOwnerKey
        {
            get
            {
                return string.Format("{0},{1}", (int)this.OwnerElementType(), this.OwnerId());
            }
        }
    }
}
