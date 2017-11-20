using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Collections;
using SpendManagementLibrary.Helpers;
using SpendManagementLibrary.Interfaces;
using Utilities.DistributedCaching;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cCurrency
    {
        protected int nAccountid;
        protected int nCurrencyid;
        protected int nGlobalCurrencyID;
        protected byte PositiveFormat;
        protected byte NegativeFormat;
        private bool bArchived;
        protected DateTime dtCreatedon;
        protected int? nCreatedby;
        protected DateTime? dtModifiedon;
        protected int? nModifiedby;

        public static Dictionary<int, string> lstPositiveFormats;
        public static Dictionary<int, string> lstNegativeFormats;

        public cCurrency()
        {
        }
        public cCurrency(int accountid, int currencyid, int globalcurrencyid, byte positiveFormat, byte negativeFormat, bool archived, DateTime createdon, int? createdby, DateTime? modifiedon, int? modifiedby)
        {
            nAccountid = accountid;
            nCurrencyid = currencyid;
            nGlobalCurrencyID = globalcurrencyid;
            PositiveFormat = positiveFormat;
            NegativeFormat = negativeFormat;
            bArchived = archived;
            dtCreatedon = createdon;
            nCreatedby = createdby;
            dtModifiedon = modifiedon;
            nModifiedby = modifiedby;

        }

        public virtual double getExchangeRate(int toCurrencyId, DateTime dateTime)
        {
            throw new InvalidOperationException("This should only be called on a derivative of cCurrency.");
        }

        public void AddToCache()
        {
            var cache = new Cache();
            string cacheKey = currencyid.ToString(CultureInfo.InvariantCulture);
            cache.Add(accountid, "currency", cacheKey, this);
        }

        public static cCurrency GetFromCache(int accountid, int currencyid)
        {
            var cache = new Cache();
            string cacheKey = currencyid.ToString(CultureInfo.InvariantCulture);
            return (cCurrency) cache.Get(accountid, "currency", cacheKey);
        }

        public static void RemoveFromCache(int accountid, int currencyid)
        {
            var cache = new Cache();
            string cacheKey = currencyid.ToString(CultureInfo.InvariantCulture);
            cache.Delete(accountid, "currency", cacheKey);
        }

        #region properties
        public int currencyid
        {
            get { return nCurrencyid; }
            set { nCurrencyid = value; }
        }
        public int globalcurrencyid
        {
            get { return nGlobalCurrencyID; }
            set { nGlobalCurrencyID = value; }
        }
        
        public int accountid
        {
            get { return nAccountid; }
            set { nAccountid = value; }
        }

        public byte positiveFormat
        {
            get { return PositiveFormat; }
        }

        public byte negativeFormat
        {
            get { return NegativeFormat; }
        }

        public bool archived
        {
            get { return bArchived; }
        }

        public DateTime createdon
        {
            get { return dtCreatedon; }
            set { dtCreatedon = value; }
        }

        public int? createdby
        {
            get { return nCreatedby; }
            set { nCreatedby = value; }
        }
       
        public DateTime? modifiedon
        {
            get { return dtModifiedon; }
        }
        public int? modifiedby
        {
            get { return nModifiedby; }
        }
        #endregion
    }

    [Serializable()]
    public class cExchangeRate
    {
        private int nCurrencyid;
        private int nToCurrencyid;
        private double dExchangerate;
        private string sConnString;

        public cExchangeRate(int currencyid, int tocurrencyid, double exchangerate, string connstring)
        {
            nCurrencyid = currencyid;
            nToCurrencyid = tocurrencyid;
            dExchangerate = exchangerate;
            sConnString = connstring;
        }

        public void updateCurrencyid(int currencyid)
        {
            nCurrencyid = currencyid;
        }
        #region properties
        public int currencyid
        {
            get { return nCurrencyid; }
        }
        public int tocurrencyid
        {
            get { return nToCurrencyid; }
        }
        public double exchangerate
        {
            get { return dExchangerate; }
        }
        #endregion
    }

    public enum CurrencyType
    {
        Static = 1, 
        Monthly, 
        Range
    }
    

    [Serializable()]
    public struct sOnlineCurrencyInfo
    {
        public SortedList lstonlinecurrencies;
        public List<int> lstcurrencyids;
        public Dictionary<int, cCurrencyMonth> lstcurmonths;
        public SortedList<int, int> lstcurmonthids;
        public Dictionary<int, cCurrencyRange> lstcurranges;
        public SortedList<int, int> lstcurrangeids;
        public Dictionary<string, double> lststaticcurs;
    }

    [Serializable()]
    public static class cNegativeFormat
    {
        public static readonly Dictionary<int, string> lstNegativeFormats;

        static cNegativeFormat()
        {
            lstNegativeFormats = new Dictionary<int, string>();

            lstNegativeFormats.Add(1, "-X1.1");
            lstNegativeFormats.Add(2, "(X1.1)");
            lstNegativeFormats.Add(3, "X-1.1");
            lstNegativeFormats.Add(4, "X1.1-");
            lstNegativeFormats.Add(5, "(1.1X)");
            lstNegativeFormats.Add(6, "-1.1X");
            lstNegativeFormats.Add(7, "1.1-X");
            lstNegativeFormats.Add(8, "1.1X-");
            lstNegativeFormats.Add(9, "(X 1.1)");
            lstNegativeFormats.Add(10, "-X 1.1");
            lstNegativeFormats.Add(11, "X -1.1");
            lstNegativeFormats.Add(12, "X 1.1-");
            lstNegativeFormats.Add(13, "(1.1 X)");
            lstNegativeFormats.Add(14, "-1.1 X");
            lstNegativeFormats.Add(15, "1.1- X");
            lstNegativeFormats.Add(16, "1.1 X-");
        }
    }

    [Serializable()]
    public static class cPositiveFormat
    {
        public static readonly Dictionary<int, string> lstPositiveFormats;

        static cPositiveFormat()
        {
            lstPositiveFormats = new Dictionary<int, string>();

            lstPositiveFormats.Add(1, "X1.1");
            lstPositiveFormats.Add(2, "1.1X");
            lstPositiveFormats.Add(3, "X 1.1");
            lstPositiveFormats.Add(4, "1.1 X");
        }
    }
}
