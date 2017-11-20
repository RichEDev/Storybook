using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cAllowance
    {
        private int nAccountid;
        private int nAllowanceid;
        private string sAllowance;
        private string sDescription;
        private int nCurrencyid;
        private int nNighthours;
        private decimal dNightrate;
        private List<cAllowanceBreakdown>lstBreakdown;
        private DateTime dtCreatedon;
        private int nCreatedby;
        private DateTime? dtModifiedon;
        private int? nModifiedby;


        public cAllowance(int accountid, int allowanceid, string allowance, string description, int currencyid, int nighthours, decimal nightrate, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, List<cAllowanceBreakdown> breakdown)
        {
            nAccountid = accountid;
            nAllowanceid = allowanceid;
            sAllowance = allowance;
            sDescription = description;
            nCurrencyid = currencyid;
            nNighthours = nighthours;
            dNightrate = nightrate;
            dtCreatedon = createdon;
            nCreatedby = createdby;
            dtModifiedon = modifiedon;
            nModifiedby = modifiedby;
            lstBreakdown = breakdown;
        }

        public cAllowanceBreakdown getAllowanceBreakdownByID(int id)
        {
            foreach (cAllowanceBreakdown breakdown in lstBreakdown)
            {
                if (breakdown.breakdownid == id)
                {
                    return breakdown;
                }
            }
            return null;
        }
        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        public int allowanceid
        {
            get { return nAllowanceid; }
        }
        public string allowance
        {
            get { return sAllowance; }
        }
        public string description
        {
            get { return sDescription; }
        }
        public int currencyid
        {
            get { return nCurrencyid; }
        }
        public int nighthours
        {
            get { return nNighthours; }
        }
        public decimal nightrate
        {
            get { return dNightrate; }
        }
        public DateTime createdon
        {
            get { return dtCreatedon; }
        }
        public int createdby
        {
            get { return nCreatedby; }
        }
        public DateTime? modifiedon
        {
            get { return dtModifiedon; }
        }
        public int? modifiedby
        {
            get { return nModifiedby; }
        }
        public List<cAllowanceBreakdown> breakdown
        {
            get { return lstBreakdown; }
        }
        #endregion
    }

    [Serializable()]
    public class cAllowanceBreakdown
    {
        private int nBreakdownid;
        private int nAllowanceid;
        private int nHours;
        private decimal dRate;

        public cAllowanceBreakdown(int breakdownid, int allowanceid, int hours, decimal rate)
        {
            nBreakdownid = breakdownid;
            nAllowanceid = allowanceid;
            nHours = hours;
            dRate = rate;
        }

        #region properties
        public int breakdownid
        {
            get { return nBreakdownid; }
        }
        public int allowanceid
        {
            get { return nAllowanceid; }
        }
        public int hours
        {
            get { return nHours; }
        }
        public decimal rate
        {
            get { return dRate; }
        }
        #endregion
    }

    [Serializable()]
    public struct sOnlineAllowInfo
    {
        public Dictionary<int, cAllowance> lstonlineallowances;
        public List<int> lstallowanceids;
    }
}
