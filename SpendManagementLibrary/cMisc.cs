using System;
using System.Collections.Generic;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cFieldToDisplay
    {
        private Guid nFieldid;
        private string sCode;
        private string sDescription;
        private bool bDisplay;
        private bool bMandatory;
        private bool bIndividual;
        private bool bDisplaycc;
        private bool bMandatorycc;
        private bool bDisplaypc;
        private bool bMandatorypc;
        private DateTime dtCreatedOn;
        private int nCreatedBy;
        private DateTime dtModifiedOn;
        private int nModifiedBy;

        public cFieldToDisplay()
        {

        }

        public cFieldToDisplay(Guid fieldid, string code, string description, bool display, bool mandatory, bool individual, bool displaycc, bool mandatorycc, bool displaypc, bool mandatorypc, DateTime createdon, int createdby, DateTime modifiedon, int modifiedby)
        {
            nFieldid = fieldid;
            sCode = code;
            sDescription = description;
            bDisplay = display;
            bMandatory = mandatory;
            bIndividual = individual;
            bDisplaycc = displaycc;
            bMandatorycc = mandatorycc;
            bDisplaypc = displaypc;
            bMandatorypc = mandatorypc;
            dtCreatedOn = createdon;
            nCreatedBy = createdby;
            dtModifiedOn = modifiedon;
            nModifiedBy = modifiedby;
        }

        #region properties
        public Guid fieldid
        {
            get { return nFieldid; }
            set { nFieldid = value; }
        }
        public string code
        {
            get { return sCode; }
            set { sCode = value; }
        }
        public string description
        {
            get { return sDescription; }
            set { sDescription = value; }
        }
        public bool display
        {
            get { return bDisplay; }
            set { bDisplay = value; }
        }
        public bool mandatory
        {
            get { return bMandatory; }
            set { bMandatory = value; }
        }
        public bool individual
        {
            get { return bIndividual; }
            set { bIndividual = value; }
        }
        public bool displaycc
        {
            get { return bDisplaycc; }
            set { bDisplaycc = value; }
        }
        public bool mandatorycc
        {
            get { return bMandatorycc; }
            set { bMandatorycc = value; }
        }
        public bool displaypc
        {
            get { return bDisplaypc; }
            set { bDisplaypc = value; }
        }
        public bool mandatorypc
        {
            get { return bMandatorypc; }
            set { bMandatorypc = value; }
        }
        public DateTime createdon
        {
            get { return dtCreatedOn; }
            set { dtCreatedOn = value;}
        }
        public int createdby
        {
            get { return nCreatedBy; }
            set { nCreatedBy = value;}
        }
        public DateTime modifiedon
        {
            get { return dtModifiedOn; }
            set { dtModifiedOn = value;}
        }
        public int modifiedby
        {
            get { return nModifiedBy; }
            set { nModifiedBy = value;}
        }
        #endregion
    }
    [Serializable()]
    public struct sCompanyDetails
    {
        public string companyname;
        public string address1;
        public string address2;
        public string city;
        public string county;
        public string postcode;
        public string telno;
        public string faxno;
        public string email;
        public string bankref;
        public string accoutno;
        public string accounttype;
        public string sortcode;
        public string companynumber;
    }

    public enum ImportType
    {
        Excel = 1,
        FlatFile,
        XML,
        ESREmployees,
        FixedWidth
    }

    



    public enum ReturnCode
    {
        OK = 0,
        AlreadyExists = -1
    }

    public enum Aggregate
    {
        None = 0,
        SUM,
        AVG,
        COUNT,
        MAX,
        MIN
    }

    [Serializable()]
    public enum Grid
    {
        ViewCurrent = 1,
        Allowances,
        BudgetHolders,
        Categories,
        CostCodes,
        Countries,
        Currencies,
        Departments,
        EMails,
        Employees,
        FAQS,
        Floats,
        Groups,
        ItemRoles,
        Locations,
        MileageCategories,
        MileageDateRanges,
        MileageThresholds,
        P11dCategories,
        ProjectCodes,
        QuickEntryForms,
        Reasons,
        Roles,
        Subcats,
        Teams,
        UserdefinedFields,
        UserDefinedFieldList,
        ViewSubmitted,
        ViewPrevious,
        CheckPayUnallocated,
        CheckPayClaims,
        Schedules,
        Delegates,
        CheckPayExpenses,
        CheckPayHistory,
        CheckPayApproved,
        CheckPayReturned,
        EmailSuffixes,
        UserDefined,
        FAQCategories,
        RangeCurrencies,
        UserCostCodes,
        UserCars,
        UserStages,
        CountrySubcats,
        MonthlyCurrencyBreakdown,
        StaticCurrencyBreakdown,
        FilterRule,
        FilterRuleValues,
        PoolCars

    }


    [Serializable()]
    public class cGridSort
    {
        private Grid gGrid;
        private string sColumnname;
        private byte bSortOrder;

        public cGridSort(Grid grid, string columnname, byte sortorder)
        {
            gGrid = grid;
            sColumnname = columnname;
            bSortOrder = sortorder;
        }

        #region properties
        public Grid grid
        {
            get { return gGrid; }
        }
        public string columnname
        {
            get { return sColumnname; }
            set { sColumnname = value; }
        }
        public byte sortorder
        {
            get { return bSortOrder; }
            set { bSortOrder = value; }
        }
        #endregion
    }
}
