namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;

    [Serializable()]
    public class cMileageCat
    {
        private int nMileageid;
        private string sCarsize;
        private string sComment;
        private ThresholdType eThresholdType;
        private bool bCalcmilestotal;
        private List<cMileageDaterange> lstDateRanges;
        private MileageUOM eMileUom;
        private bool bCatValid;
        private string sCatValidComment;
        private int nCurrencyid;
        private DateTime dtCreatedon;
        private int nCreatedby;
        private DateTime? dtModifiedon;
        private int? nModifiedby;

        /// <summary>
        /// The user rates table.
        /// </summary>
        private string userRatestable;

        /// <summary>
        /// The user rates from engine size.
        /// </summary>
        private int userRatesFromEngineSize;

        /// <summary>
        /// The user rates to engine size.
        /// </summary>
        private int userRatesToEngineSize;

        public cMileageCat()
        { }

        public cMileageCat(int mileageid, string carsize, string comment, ThresholdType thresholdType, bool calcmilestotal, List<cMileageDaterange> dateRanges, MileageUOM mileUom, bool catvalid, string catvalidcomment, int currencyid, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, string userRatestable, int userRatesFromEngineSize, int userRatesToEngineSize, int? financialYearID)
        {
            nMileageid = mileageid;
            sCarsize = carsize;
            sComment = comment;
            eThresholdType = thresholdType;
            bCalcmilestotal = calcmilestotal;
            lstDateRanges = dateRanges;
            eMileUom = mileUom;
            bCatValid = catvalid;
            sCatValidComment = catvalidcomment;
            nCurrencyid = currencyid;
            dtCreatedon = createdon;
            nCreatedby = createdby;
            dtModifiedon = modifiedon;
            nModifiedby = modifiedby;
            this.userRatestable = userRatestable;
            this.userRatesFromEngineSize = userRatesFromEngineSize;
            this.userRatesToEngineSize = userRatesToEngineSize;
            this.FinancialYearID = financialYearID;
        }

        #region properties
        public int mileageid
        {
            get { return nMileageid; }
        }
        public string carsize
        {
            get { return sCarsize; }
        }
        public string comment
        {
            get { return sComment; }
        }
        public ThresholdType thresholdType
        {
            get { return eThresholdType; }
        }
        public bool calcmilestotal
        {
            get { return bCalcmilestotal; }
        }
        public List<cMileageDaterange> dateRanges
        {
            get { return lstDateRanges; }
        }
        public MileageUOM mileUom
        {
            get { return eMileUom; }
            internal set
            {
                this.eMileUom = value;
            }
        }
        public bool catvalid
        {
            get { return bCatValid; }
            set { bCatValid = value; }
        }
        public string catvalidcomment
        {
            get { return sCatValidComment; }
            set { sCatValidComment = value; }
        }
        public int currencyid
        {
            get { return nCurrencyid; }
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

        /// <summary>
        /// Gets or sets the user rates table.
        /// </summary>
        public string UserRatestable
        {
            get
            {
                return this.userRatestable;
            }

            set
            {
                this.userRatestable = value;
            }
        }

        /// <summary>
        /// Gets or sets the user rates from engine size.
        /// </summary>
        public int UserRatesFromEngineSize
        {
            get
            {
                return this.userRatesFromEngineSize;
            }

            set
            {
                this.userRatesFromEngineSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the user rates to engine size.
        /// </summary>
        public int UserRatesToEngineSize
        {
            get
            {
                return this.userRatesToEngineSize;
            }

            set
            {
                this.userRatesToEngineSize = value;
            }
        }

        public int? FinancialYearID { get; set; }

        #endregion


    }

    [Serializable()]
    public class cMileageDaterange
    {
        private int nMileageDateid;
        private int nMileageid;
        private DateTime? dtDateValue1;
        private DateTime? dtDateValue2;
        private List<cMileageThreshold> lstThresholds;
        DateRangeType eDateRangeType;
        private DateTime dtCreatedon;
        private int nCreatedby;
        private DateTime? dtModifiedon;
        private int? nModifiedby;

        public cMileageDaterange(int mileagedateid, int mileageid, DateTime? dateValue1, DateTime? dateValue2, List<cMileageThreshold> thresholds, DateRangeType daterangetype, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby)
        {
            nMileageDateid = mileagedateid;
            nMileageid = mileageid;
            dtDateValue1 = dateValue1;
            dtDateValue2 = dateValue2;
            lstThresholds = thresholds;
            eDateRangeType = daterangetype;
            dtCreatedon = createdon;
            nCreatedby = createdby;
            dtModifiedon = modifiedon;
            nModifiedby = modifiedby;
            sortThresholds();
        }

        private void sortThresholds()
        {

                var sorted = new SortedList<decimal, cMileageThreshold>();
                foreach (cMileageThreshold threshold in lstThresholds)
                {
                    switch (threshold.RangeType)
                    {
                        case RangeType.LessThan:
                            sorted.Add(1, threshold);
                            break;
                        case RangeType.Any:
                            sorted.Add(999999, threshold);
                            break;
                        case RangeType.Between:
                        case RangeType.GreaterThanOrEqualTo:
                            sorted.Add((decimal)threshold.RangeValue1, threshold);
                            break;
                    }
                }
                lstThresholds.Clear();
                foreach (cMileageThreshold threshold in sorted.Values)
                {
                    lstThresholds.Add(threshold);
                }
            lstThresholds.Clear();
            foreach (cMileageThreshold threshold in sorted.Values)
            {
                lstThresholds.Add(threshold);
            }

          
        }
        #region properties

        public int mileagedateid
        {
            get { return nMileageDateid; }
        }
        public int mileageid
        {
            get { return nMileageid; }
        }
        public DateTime? dateValue1
        {
            get { return dtDateValue1; }
        }
        public DateTime? dateValue2
        {
            get { return dtDateValue2; }
        }
        public List<cMileageThreshold> thresholds
        {
            get { return lstThresholds; }
        }
        public DateRangeType daterangetype
        {
            get { return eDateRangeType; }
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
        #endregion
    }

    public enum ThresholdType
    {
        Annual = 0,
        Journey
    }

    [Serializable()]
    public enum MileageUOM
    {
        Mile = 0,
        KM = 1
    }

    [Serializable()]
    public enum DateRangeType
    {
        Before = 0,
        AfterOrEqualTo,
        Between,
        Any
    }

    [Serializable()]
    public struct sMileageExistenceCheck
    {
        public int id;
        public string message;
    }

    [Serializable()]
    public struct sJourneyValues
    {
        public decimal fuelcost;
        public decimal costperunit;
        public decimal grandtotal;
    }

    [Serializable()]
    public struct sMileageInfo
    {
        public Dictionary<int, cMileageCat> lstmileagecats;
        public Dictionary<int, cMileageDaterange> lstdateranges;
        public Dictionary<int, cMileageThreshold> lstthresholds;
        public List<int> lstmileagecatids;
        public List<int> lstmileagedateids;
        public List<int> lstthresholdids;
    }
}
