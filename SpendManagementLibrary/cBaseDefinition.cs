using System;

namespace SpendManagementLibrary
{
    /// <summary>
    /// The abstarct base object for the base definitions
    /// </summary>
    [Serializable]
    public abstract class cBaseDefinition
    {
        private int nID;
        private string sDescription;
        private DateTime? dtCreatedOn;
        private int? nCreatedBy;
        private DateTime? dtModifiedOn;
        private int? nModifiedBy;
        private bool bArchived;

        /// <summary>
        /// Constructor for the base definition object
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Description"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="CreatedBy"></param>
        /// <param name="ModifiedOn"></param>
        /// <param name="ModifiedBy"></param>
        /// <param name="Archived"></param>
        public cBaseDefinition(int ID, string Description, DateTime? CreatedOn, int? CreatedBy, DateTime? ModifiedOn, int? ModifiedBy, bool Archived)
        {
            nID = ID;
            sDescription = Description;
            dtCreatedOn = CreatedOn;
            nCreatedBy = CreatedBy;
            dtModifiedOn = ModifiedOn;
            nModifiedBy = ModifiedBy;
            bArchived = Archived;
        }

        #region Properties

        /// <summary>
        /// Unique ID of the definition
        /// </summary>
        public int ID
        {
            get { return nID; }
        }

        /// <summary>
        /// Description of the base definition
        /// </summary>
        public string Description
        {
            get { return sDescription; }
            set { sDescription = value; }
        }

        /// <summary>
        /// The created on date
        /// </summary>
        public DateTime? CreatedOn
        {
            get { return dtCreatedOn; }
            set { dtCreatedOn = value; }
        }

        /// <summary>
        /// Created By ID
        /// </summary>
        public int? CreatedBy
        {
            get { return nCreatedBy; }
            set { nCreatedBy = value; }
        }

        /// <summary>
        /// The modified on date
        /// </summary>
        public DateTime? ModifiedOn
        {
            get { return dtModifiedOn; }
            set { dtModifiedOn = value; }
        }

        /// <summary>
        /// Modified on ID
        /// </summary>
        public int? ModifiedBy
        {
            get { return nModifiedBy; }
            set { nModifiedBy = value; }
        }

        /// <summary>
        /// Flag to see whether base definition it is archived 
        /// </summary>
        public bool Archived
        {
            get {return bArchived;}
        }

        #endregion
    }

    /// <summary>
    /// The base definition contract status 
    /// </summary>
    public class cContractStatus : cBaseDefinition
    {
        private bool bIsArchiveStatus;

        #region properties

        /// <summary>
        /// Gets indicator of whether status definition is considered an archived status
        /// </summary>
        public bool IsArchiveStatus
        {
            get { return bIsArchiveStatus; }
        }

        #endregion

       
        /// <summary>
        /// cContractStatus constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Description"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="CreatedBy"></param>
        /// <param name="ModifiedOn"></param>
        /// <param name="ModifiedBy"></param>
        /// <param name="isArchiveState"></param>
        /// <param name="Archived"></param>
        public cContractStatus(int ID, string Description, DateTime? CreatedOn, int? CreatedBy, DateTime? ModifiedOn, int? ModifiedBy, bool isArchiveState, bool Archived) : base(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, Archived)
        {
            bIsArchiveStatus = isArchiveState;
        }
    }

    /// <summary>
    /// The base definition contract status 
    /// </summary>
    public class cContractCategory : cBaseDefinition
    {
        #region properties

        #endregion


        /// <summary>
        /// cContractCategory constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Description"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="CreatedBy"></param>
        /// <param name="ModifiedOn"></param>
        /// <param name="ModifiedBy"></param>
        /// <param name="Archived"></param>
        public cContractCategory(int ID, string Description, DateTime? CreatedOn, int? CreatedBy, DateTime? ModifiedOn, int? ModifiedBy, bool Archived)
            : base(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, Archived)
        {
           
        }
    }

    /// <summary>
    /// The base definition contract type 
    /// </summary>
    public class cContractType : cBaseDefinition
    {
        #region properties

        #endregion

        /// <summary>
        /// cContractType constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Description"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="CreatedBy"></param>
        /// <param name="ModifiedOn"></param>
        /// <param name="ModifiedBy"></param>
        /// <param name="Archived"></param>
        public cContractType(int ID, string Description, DateTime? CreatedOn, int? CreatedBy, DateTime? ModifiedOn, int? ModifiedBy, bool Archived)
            : base(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, Archived)
        {

        }
    }

    /// <summary>
    /// The base definition contract status 
    /// </summary>
    public class cInvoiceFrequencyType : cBaseDefinition
    {
        private int nFrequencyMonths;

        #region properties

        /// <summary>
        /// Gets the number of months represented by the frequency entity
        /// </summary>
        public int FrequencyInMonths
        {
            get { return nFrequencyMonths; }
        }

        #endregion

        /// <summary>
        /// cInvoiceFrequencyType constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Description"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="CreatedBy"></param>
        /// <param name="ModifiedOn"></param>
        /// <param name="ModifiedBy"></param>
        /// <param name="Archived"></param>
        /// <param name="FrequencyInMonths"></param>
        public cInvoiceFrequencyType(int ID, string Description, DateTime? CreatedOn, int? CreatedBy, DateTime? ModifiedOn, int? ModifiedBy, bool Archived, int FrequencyInMonths)
            : base(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, Archived)
        {
            nFrequencyMonths = FrequencyInMonths;
        }
    }

    /// <summary>
    /// The base definition contract status 
    /// </summary>
    public class cInvoiceStatus : cBaseDefinition
    {
        private bool bIsArchiveStatus;

        #region properties

        /// <summary>
        /// Gets indicator of whether status definition is considered an archived status
        /// </summary>
        public bool IsArchiveStatus
        {
            get { return bIsArchiveStatus; }
        }

        #endregion

        /// <summary>
        /// cInvoiceStatus constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Description"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="CreatedBy"></param>
        /// <param name="ModifiedOn"></param>
        /// <param name="ModifiedBy"></param>
        /// <param name="isArchiveState"></param>
        /// <param name="Archived"></param>
        public cInvoiceStatus(int ID, string Description, DateTime? CreatedOn, int? CreatedBy, DateTime? ModifiedOn, int? ModifiedBy, bool isArchiveState, bool Archived)
            : base(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, Archived)
        {
            bIsArchiveStatus = isArchiveState;
        }
    }

    /// <summary>
    /// The base definition licence renewal type 
    /// </summary>
    public class cLicenceRenewalType : cBaseDefinition
    {
        #region properties

        #endregion

        /// <summary>
        /// cLicenceRenewalType constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Description"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="CreatedBy"></param>
        /// <param name="ModifiedOn"></param>
        /// <param name="ModifiedBy"></param>
        /// <param name="Archived"></param>
        public cLicenceRenewalType(int ID, string Description, DateTime? CreatedOn, int? CreatedBy, DateTime? ModifiedOn, int? ModifiedBy, bool Archived)
            : base(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, Archived)
        {

        }
    }

    /// <summary>
    /// The base definition Inflator Metric
    /// </summary>
    public class cInflatorMetric : cBaseDefinition
    {
        private decimal fPercentage;
        private bool bRequiresExtraPct;

        #region properties

        /// <summary>
        /// Gets the percentage value that the inflator represents
        /// </summary>
        public decimal Percentage
        {
            get { return fPercentage; }
        }
        /// <summary>
        /// Gets whether metric has additional % value (e.g. RPI+%)
        /// </summary>
        public bool RequiresExtraPct
        {
            get { return bRequiresExtraPct; }
        }

        #endregion

        /// <summary>
        /// cInflatorMetric constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Description"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="CreatedBy"></param>
        /// <param name="ModifiedOn"></param>
        /// <param name="ModifiedBy"></param>
        /// <param name="Archived"></param>
        /// <param name="Percentage"></param>
        /// <param name="RequiresExtraPct"></param>
        public cInflatorMetric(int ID, string Description, DateTime? CreatedOn, int? CreatedBy, DateTime? ModifiedOn, int? ModifiedBy, bool Archived, decimal Percentage, bool RequiresExtraPct)
            : base(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, Archived)
        {
            fPercentage = Percentage;
            bRequiresExtraPct = RequiresExtraPct;
        }
    }

    /// <summary>
    /// The base definition term type 
    /// </summary>
    public class cTermType : cBaseDefinition
    {
        #region properties

        #endregion

        /// <summary>
        /// cTermType constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Description"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="CreatedBy"></param>
        /// <param name="ModifiedOn"></param>
        /// <param name="ModifiedBy"></param>
        /// <param name="Archived"></param>
        public cTermType(int ID, string Description, DateTime? CreatedOn, int? CreatedBy, DateTime? ModifiedOn, int? ModifiedBy, bool Archived)
            : base(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, Archived)
        {

        }
    }

    /// <summary>
    /// The base definition Financial Status 
    /// </summary>
    public class cFinancialStatus : cBaseDefinition
    {
        #region properties

        #endregion

        /// <summary>
        /// cFinancialStatus constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Description"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="CreatedBy"></param>
        /// <param name="ModifiedOn"></param>
        /// <param name="ModifiedBy"></param>
        /// <param name="Archived"></param>
        public cFinancialStatus(int ID, string Description, DateTime? CreatedOn, int? CreatedBy, DateTime? ModifiedOn, int? ModifiedBy, bool Archived)
            : base(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, Archived)
        {

        }
    }

    /// <summary>
    /// The base definition Task Type 
    /// </summary>
    [Serializable()]
    public class cTaskType : cBaseDefinition
    {
        #region properties

        #endregion

        /// <summary>
        /// cTaskType constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Description"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="CreatedBy"></param>
        /// <param name="ModifiedOn"></param>
        /// <param name="ModifiedBy"></param>
        /// <param name="Archived"></param>
        public cTaskType(int ID, string Description, DateTime? CreatedOn, int? CreatedBy, DateTime? ModifiedOn, int? ModifiedBy, bool Archived)
            : base(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, Archived)
        {

        }
    }

    /// <summary>
    /// The base definition unit
    /// </summary>
    [Serializable()]
    public class cUnit : cBaseDefinition
    {
        #region properties

        #endregion

        /// <summary>
        /// cUnit constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Description"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="CreatedBy"></param>
        /// <param name="ModifiedOn"></param>
        /// <param name="ModifiedBy"></param>
        /// <param name="Archived"></param>
        public cUnit(int ID, string Description, DateTime? CreatedOn, int? CreatedBy, DateTime? ModifiedOn, int? ModifiedBy, bool Archived)
            : base(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, Archived)
        {

        }
    }

    /// <summary>
    /// The base definition Product Category
    /// </summary>
    [Serializable()]
    public class cProductCategory : cBaseDefinition
    {
        #region properties

        #endregion

        /// <summary>
        /// cProductCategory constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Description"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="CreatedBy"></param>
        /// <param name="ModifiedOn"></param>
        /// <param name="ModifiedBy"></param>
        /// <param name="Archived"></param>
        public cProductCategory(int ID, string Description, DateTime? CreatedOn, int? CreatedBy, DateTime? ModifiedOn, int? ModifiedBy, bool Archived)
            : base(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, Archived)
        {

        }
    }

    /// <summary>
    /// The base definition Supplier Status
    /// </summary>
    [Serializable()]
    public class cSupplierStatus : cBaseDefinition
    {
        /// <summary>
        /// Sequence
        /// </summary>
        private short nSequence;

        /// <summary>
        /// Deny Contract Add
        /// </summary>
        private bool bDenyAdd;

        #region properties

        /// <summary>
        /// Gets the current status sequence
        /// </summary>
        public short Sequence
        {
            get { return nSequence; }
        }
        
        /// <summary>
        /// Gets indicator as to whether a contract can be added when supplier is set to this status
        /// </summary>
        public bool DenyContractAdd
        {
            get { return bDenyAdd; }
        }

        #endregion

        /// <summary>
        /// cProductCategory constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Description"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="CreatedBy"></param>
        /// <param name="ModifiedOn"></param>
        /// <param name="ModifiedBy"></param>
        /// <param name="Archived"></param>
        /// <param name="Sequence">Sequence to force particular sort order</param>
        /// <param name="DenyContractAdd">Is a contract permitted to be added for this status</param>
        public cSupplierStatus(int ID, string Description, DateTime? CreatedOn, int? CreatedBy, DateTime? ModifiedOn, int? ModifiedBy, bool Archived, short Sequence, bool DenyContractAdd)
            : base(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, Archived)
        {
            nSequence = Sequence;
            bDenyAdd = DenyContractAdd;
        }
    }

    /// <summary>
    /// The base definition Supplier Category
    /// </summary>
    [Serializable()]
    public class cSupplierCategory : cBaseDefinition
    {
        #region properties

        #endregion

        /// <summary>
        /// cSupplierCategory constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Description"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="CreatedBy"></param>
        /// <param name="ModifiedOn"></param>
        /// <param name="ModifiedBy"></param>
        /// <param name="Archived"></param>
        public cSupplierCategory(int ID, string Description, DateTime? CreatedOn, int? CreatedBy, DateTime? ModifiedOn, int? ModifiedBy, bool Archived)
            : base(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, Archived)
        {

        }
    }

    /// <summary>
    /// The base definition Product Licence Type
    /// </summary>
    [Serializable()]
    public class cProductLicenceType : cBaseDefinition
    {
        #region properties

        #endregion

        /// <summary>
        /// cProductLicenceType constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Description"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="CreatedBy"></param>
        /// <param name="ModifiedOn"></param>
        /// <param name="ModifiedBy"></param>
        /// <param name="Archived"></param>
        public cProductLicenceType(int ID, string Description, DateTime? CreatedOn, int? CreatedBy, DateTime? ModifiedOn, int? ModifiedBy, bool Archived)
            : base(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, Archived)
        {

        }
    }

    /// <summary>
    /// The base definition Sales tax
    /// </summary>
    [Serializable()]
    public class cSalesTax : cBaseDefinition
    {
        private decimal dSalesTax;

        #region properties

        /// <summary>
        /// The sales tax amount
        /// </summary>
        public decimal SalesTax 
        { 
            get { return dSalesTax; } 
        }

        #endregion

        /// <summary>
        /// cSalesTax constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Description"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="CreatedBy"></param>
        /// <param name="ModifiedOn"></param>
        /// <param name="ModifiedBy"></param>
        /// <param name="Archived"></param>
        /// <param name="SalesTax"></param>
        public cSalesTax(int ID, string Description, DateTime? CreatedOn, int? CreatedBy, DateTime? ModifiedOn, int? ModifiedBy, bool Archived, decimal SalesTax)
            : base(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, Archived)
        {
            dSalesTax = SalesTax;
        }
    }
}
