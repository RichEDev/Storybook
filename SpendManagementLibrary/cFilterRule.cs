namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using Helpers;
    using Interfaces;

    [Serializable()]
    public class cFilterRule : IValidatable
    {
        private int _filterId;
        private FilterType _parentFilterType;
        private FilterType _childFilterType;
        private Dictionary<int, cFilterRuleValue> _lstRuleVals;
        private int _parentUdfId;
        private int _childUdfId;
        private bool _enabled;
        private DateTime _dateCreatedOn;
        private int _createdBy;

        public cFilterRule(int filterid, FilterType parent, FilterType child, Dictionary<int, cFilterRuleValue> rulevals, int paruserdefineid, int childuserdefineid, bool enabled, DateTime createdon, int createdby)
        {
            _filterId = filterid;
            _parentFilterType = parent;
            _childFilterType = child;
            _lstRuleVals = rulevals;
            _parentUdfId = paruserdefineid;
            _childUdfId = childuserdefineid;
            _enabled = enabled;
            _dateCreatedOn = createdon;
            _createdBy = createdby;

            IsValid();
        }

        #region Properties

        public int filterid
        {
            get { return _filterId; }
        }
        public FilterType parent
        {
            get { return _parentFilterType; }
        }
        public FilterType child
        {
            get { return _childFilterType; }
        }
        public Dictionary<int, cFilterRuleValue> rulevals
        {
            get { return _lstRuleVals; }
        }
        public int paruserdefineid
        {
            get { return _parentUdfId; }
        }
        public int childuserdefineid
        {
            get { return _childUdfId; }
        }
        public bool enabled
        {
            get { return _enabled; }
        }
        public DateTime createdon
        {
            get { return _dateCreatedOn; }
        }
        public int createdby
        {
            get { return _createdBy; }
        }
        #endregion

        /// <summary>
        /// Checks various criteria to ensure the Filter Rule is valid
        /// </summary>
        /// <returns>Whether the Filter Rule is valid</returns>
        public bool IsValid()
        {
            if (this.filterid < 0)
            {
                throw new ArgumentException(ValidationResponses.InvalidId);
            }

            if (!EnumValidator.EnumValueIsDefined(typeof(FilterType), this.parent))
            {
                throw new ArgumentException(ValidationResponses.InvalidEnumValue);
            }

            if (!EnumValidator.EnumValueIsDefined(typeof(FilterType), this.child))
            {
                throw new ArgumentException(ValidationResponses.InvalidEnumValue);
            }

            if (rulevals == null)
            {
                throw new ArgumentNullException(ValidationResponses.DictionaryNull);
            }

            if (this.parent == FilterType.Userdefined)
            {
                if (this.paruserdefineid < 1)
                {
                    throw new ArgumentException(ValidationResponses.InvalidUdfId);
                }
            }

            if (this.child == FilterType.Userdefined)
            {
                if (this.childuserdefineid < 1 )
                {
                    throw new ArgumentException(ValidationResponses.InvalidUdfId);
                }
            }

            if (!DateValidators.IsDateNoGreaterThanTodayOrLess01011753(this.createdon))
            {
                throw new ArgumentException(ValidationResponses.InvalidDateRange);
            }

            if (createdby < 1)
            {
                throw new ArgumentException(ValidationResponses.InvalidCreatedById);
            }

            return true;
        }
    }

    [Serializable()]
    public class cFilterRuleValue
    {
        private int nFilterruleId;
        private int nParentId;
        private int nChildId;
        private int nFilterId;
        private DateTime dtCreatedon;
        private int nCreatedby;

        public cFilterRuleValue(int filterruleid, int parentid, int childid, int filterid, DateTime createdon, int createdby)
        {
            nFilterruleId = filterruleid;
            nParentId = parentid;
            nChildId = childid;
            nFilterId = filterid;
            dtCreatedon = createdon;
            nCreatedby = createdby;
        }

        #region Properties

        public int filterruleid
        {
            get { return nFilterruleId; }
        }
        public int parentid
        {
            get { return nParentId; }
        }
        public int childid
        {
            get { return nChildId; }
        }
        public int filterid
        {
            get { return nFilterId; }
        }
        public DateTime createdon
        {
            get { return dtCreatedon; }
        }
        public int createdby
        {
            get { return nCreatedby; }
        }
        #endregion
    }

    [Serializable()]
    public enum FilterType
    {
        All = 0,
        Costcode = 1,
        Department = 2,
        //Location = 3,
        Projectcode = 4,
        Reason = 5,
        Userdefined = 6
    }

    [Serializable()]
    public enum FilterArea
    {
        General,
        Breakdown,
        Individual
    }

    [Serializable()]
    public struct sFilterRuleControlAttributes
    {
        public string labelText;
        public string serviceMethod;
        public List<System.Web.UI.WebControls.ListItem> items;
        public int itemCount;
    }

    [Serializable()]
    public struct sFilterRuleExistence
    {
        public string message;
        public int returncode;
    }

    [Serializable()]
    public struct sFilterRuleInfo
    {
        public Dictionary<int, cFilterRule> lstfilterrules;
        public List<int> lstfilterruleids;
        public Dictionary<int, cFilterRuleValue> lstfilterrulevalues;
        public List<int> lstfilterrulevalueids;
    }
}
