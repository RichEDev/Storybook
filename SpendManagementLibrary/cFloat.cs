

namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;

    [Serializable()]
    public class cFloat
    {
        private int nAccountid;
        private int nFloatid;
        private int nEmployeeid;
        private int nCurrencyid;
        private string sName;
        private string sReason;
        private DateTime dtRequiredby;
        private bool bApproved;
        private int nApprover;
        private decimal dFloatamount;
        private decimal dForeignAmount;
        private double dExchangerate;
        private byte bStage;
        private bool bRejected;
        private string sRejectreason;
        private bool bDisputed;
        private string sDispute;
        private bool bPaid;
        private DateTime dtDatepaid;
        private int nIssuenum;
        private int nBasecurrency;
        private decimal dFloatUsed;
        private bool bSettled;
        private SortedList<int, decimal> lstAllocations;
        private DateTime dtCreatedon;
        private int nCreatedby;
        private DateTime dtModifiedon;
        private int nModifiedby;


        public cFloat(int accountid, int floatid, int employeeid, int currencyid, string name, string reason, DateTime requiredby, bool approved, int approver, decimal floatamount, double exchangerate, byte stage, bool rejected, string rejectreason, bool disputed, string dispute, bool paid, DateTime datepaid, int issuenum, int basecurrency, bool settled, SortedList<int, decimal> allocations, decimal floatused, DateTime createdon, int createdby, DateTime modifiedon, int modifiedby, decimal foreignAmount)
        {
            nAccountid = accountid;
            nFloatid = floatid;
            nEmployeeid = employeeid;
            nCurrencyid = currencyid;
            sName = name;
            sReason = reason;
            dtRequiredby = requiredby;
            bApproved = approved;
            nApprover = approver;
            dFloatamount = floatamount;
            dForeignAmount = foreignAmount;
            dExchangerate = exchangerate;
            bStage = stage;
            bRejected = rejected;
            sRejectreason = rejectreason;
            bDisputed = disputed;
            sDispute = dispute;
            bPaid = paid;
            dtDatepaid = datepaid;
            dFloatUsed = floatused;
            nIssuenum = issuenum;
            nBasecurrency = basecurrency;
            bSettled = settled;
            lstAllocations = allocations;
            dtCreatedon = createdon;
            nCreatedby = createdby;
            dtModifiedon = modifiedon;
            nModifiedby = modifiedby;
        }

        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        public int floatid
        {
            get { return nFloatid; }
        }
        public int employeeid
        {
            get { return nEmployeeid; }
        }
        public int currencyid
        {
            get { return nCurrencyid; }
        }
        public string name
        {
            get { return sName; }
        }
        public string reason
        {
            get { return sReason; }
        }
        public DateTime requiredby
        {
            get { return dtRequiredby; }
        }
        public bool approved
        {
            get { return bApproved; }
        }
        public int approver
        {
            get { return nApprover; }
            set { nApprover = value; }
        }
        public decimal floatamount
        {
            get { return dFloatamount; }
        }

        public decimal foreignAmount
        {
            get
            {
                return dForeignAmount;
            }
        }

        public double exchangerate
        {
            get { return dExchangerate; }
        }
        public byte stage
        {
            get { return bStage; }
            set { bStage = value; }
        }

        public decimal floatused
        {
            get { return dFloatUsed; }
        }
        public decimal floatavailable
        {
            get { return dFloatamount - dFloatUsed; }
        }
        public bool disputed
        {
            get { return bDisputed; }
        }
        public string dispute
        {
            get { return sDispute; }
        }
        public bool rejected
        {
            get { return bRejected; }
            set { bRejected = value; }
        }
        public string rejectreason
        {
            get { return sRejectreason; }
        }
        public bool paid
        {
            get { return bPaid; }
        }
        public DateTime datepaid
        {
            get { return dtDatepaid; }
        }
        public int issuenum
        {
            get { return nIssuenum; }
        }
        public int basecurrency
        {
            get { return nBasecurrency; }
        }
        public bool settled
        {
            get { return bSettled; }
        }
        public SortedList<int, decimal> allocations
        {
            get { return lstAllocations; }
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


        public decimal calculateFloatValue(int expenseid, decimal total, decimal allocatedamount)
        {


            decimal allocation;

            //deleteAllocation(expenseid);

            if ((floatavailable + allocatedamount) >= total) //can claim whole amount on a float
            {
                allocation = total;
            }
            else
            {
                allocation = floatavailable + allocatedamount;
            }


            return total - allocation;

        }

        public void payAdvance()
        {
            bPaid = true;
            dtDatepaid = DateTime.Now;
        }

        public void settleAdvance()
        {
            bSettled = true;
        }

        public void approveAdvance()
        {
            bApproved = true;
        }
        public void rejectAdvance()
        {
            bRejected = true;
        }

        public void updateFloat(decimal floatUsed)
        {
            dFloatUsed = floatUsed;
        }

    }

    [Serializable()]
    public struct sFloatInfo
    {
        public Dictionary<int, cFloat> lstfloats;
        public List<int> lstfloatids;
    }
}
