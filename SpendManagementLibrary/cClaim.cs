using System;
using System.Collections.Generic;

namespace SpendManagementLibrary
{
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    [Serializable()]
    public class cClaim : IValidatable
    {
        protected int _claimId;
        protected int _claimNo;
        protected int _employeeId;
        protected string _name;
        protected string _description;
        protected int _stage;
        protected bool _approved;
        protected bool _paid;
        protected DateTime _dateSubmitted;
        protected DateTime _datePaid;
        protected ClaimStatus _status;

        /// <summary>
        /// Gets or sets the status, whether a claim can be accessed or not.
        /// </summary>
        protected ClaimToAccessStatus claimAccessStatus { get; set; }
        protected int _teamId;
        protected int _checkerId;
        protected bool _submitted;
        protected DateTime _createdOn;
        protected int _createdBy;
        protected DateTime _modifiedOn;
        protected int _modifiedBy;
        protected int _currencyId;
        private int _accountId;
        protected ClaimType _claimType;
        protected bool _splitApprovalStage;

        private readonly bool _hasClaimHistory;
        private readonly string _currentApprover;
        private readonly int _totalStageCount;
        private readonly bool _hasReturnedItems;
        private readonly bool _hasCashItems;
        private readonly bool _hasCreditCardItems;
        private readonly bool _hasPurchaseCardItems;
        private readonly bool _hasFlaggedItems;
        private readonly int _numberOfItems;
        private readonly DateTime? _startDate;
        private readonly DateTime? _endDate;
        private readonly decimal _total;
        private readonly decimal _amountPayable;
        private readonly int _numberOfReceipts;
        private readonly int _numberOfUnapprovedItems;
        private readonly decimal _creditCardTotal;
        private readonly decimal _purchaseCardTotal;

        public cClaim(int accountid, int claimid, int claimno, int employeeid, string name, string description, int stage, bool approved, bool paid, DateTime datesubmitted, DateTime datepaid, ClaimStatus status, int teamid, int checkerid, bool submitted, bool splitapprovalstage, DateTime createdon, int createdby, DateTime modifiedon, int modifiedby, int currencyid, string referenceNumber, bool hasclaimhistory, string currentapprover, int totalstagecount, bool hasreturneditems, bool hascashitems, bool hascreditcarditems, bool haspurchasecarditems, bool hasflaggeditems, int numberOfItems, DateTime? startdate, DateTime? enddate, decimal total, decimal amountpayable, int numberofreceipts, int numberofunapproveditems, decimal creditcardtotal, decimal purchasecardtotal, bool payBeforeValidate)
        {
            this._accountId = accountid;
            this._claimId = claimid;
            this._claimNo = claimno;
            this._employeeId = employeeid;
            this._name = name;
            this._description = description;
            this._stage = stage;
            this._approved = approved;
            this._paid = paid;
            this._dateSubmitted = datesubmitted;
            this._datePaid = datepaid;
            this._status = status;
            this._teamId = teamid;
            this._checkerId = checkerid;
            this._submitted = submitted;
            this._splitApprovalStage = splitapprovalstage;
            this._createdOn = createdon;
            this._createdBy = createdby;
            this._modifiedOn = modifiedon;
            this._modifiedBy = modifiedby;
            this._currencyId = currencyid;
            this.ReferenceNumber = referenceNumber;
            this.PayBeforeValidate = payBeforeValidate;
            this._hasClaimHistory = hasclaimhistory;
            this._currentApprover = currentapprover;
            this._totalStageCount = totalstagecount;
            this._hasReturnedItems = hasreturneditems;
            this._hasCashItems = hascashitems;
            this._hasCreditCardItems = hascreditcarditems;
            this._hasPurchaseCardItems = haspurchasecarditems;
            this._hasFlaggedItems = hasflaggeditems;
            this._numberOfItems = numberOfItems;
            this._startDate = startdate;
            this._endDate = enddate;
            this._total = total;
            this._amountPayable = amountpayable;
            this._numberOfReceipts = numberofreceipts;
            this._numberOfUnapprovedItems = numberofunapproveditems;
            this._creditCardTotal = creditcardtotal;
            this._purchaseCardTotal = purchasecardtotal;
        }
        public bool containsCashAndCredit()
        {
            return HasCashItems && (HasCreditCardItems || HasPurchaseCardItems);
        }

        
        public void submitClaim()
        {
            _submitted = true;
            _dateSubmitted = DateTime.Today;
            _status = ClaimStatus.Submitted;
        }
        

        
        public void changeStatus(ClaimStatus status)
        {
            _status = status;
        }
        public void unSubmitClaim()
        {
            _checkerId = 0;
            _submitted = false;
            _approved = false;
            _stage = 0;
        }
        public void claimPaid()
        {
            _paid = true;
            _datePaid = DateTime.Now;
            _status = ClaimStatus.ClaimPaid;
        }
        #region expense items
        

        
        
        
        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the claim access status.
        /// </summary>
        public ClaimToAccessStatus ClaimAccessStatus
        {
            get
            {
                return this.claimAccessStatus;
            }

            set
            {
                this.claimAccessStatus = value;
            }
        }

        public int accountid
        {
            get { return _accountId; }
        }
        public int claimid
        {
            get { return _claimId; }
            set { _claimId = value; }
        }
        public int claimno
        {
            get { return _claimNo; }
        }
        public int employeeid
        {
            get { return _employeeId; }
        }
        public string name
        {
            get { return _name; }
        }
        public string description
        {
            get { return _description; }
            set { _description = value; }
        }
        public int stage
        {
            get { return _stage; }
            set { _stage = value; }
        }
        public bool approved
        {
            get { return _approved; }
        }
        public bool paid
        {
            get { return _paid; }
        }
        public DateTime datesubmitted
        {
            get
            {
                return _dateSubmitted;
            }
        }
        public DateTime datepaid
        {
            get { return _datePaid; }
        }
        public ClaimStatus status
        {
            get { return _status; }
        }
        public int teamid
        {
            get { return _teamId; }
            set { _teamId = value; }
        }
        public int checkerid
        {
            get { return _checkerId; }
            set { _checkerId = value; }
        }
        public bool submitted
        {
            get { return _submitted; }
        }

        public bool splitApprovalStage
        {
            get
            {
                return _splitApprovalStage;
            }
        }

        public DateTime createdon
        {
            get { return _createdOn; }
        }
        public int createdby
        {
            get { return _createdBy; }
        }
        public DateTime modifiedon
        {
            get { return _modifiedOn; }
        }
        public int modifiedby
        {
            get { return _modifiedBy; }
        }
        public int currencyid
        {
            get { return _currencyId; }
        }


        public ClaimType claimtype
        {
            get
            {

               

                if (HasCashItems && (HasCreditCardItems || HasPurchaseCardItems))
                {
                    return ClaimType.Mixed;
                }
                else if (HasCashItems && !HasCreditCardItems && !HasPurchaseCardItems)
                {
                    return ClaimType.Cash;
                }
                else if (!HasCashItems && HasCreditCardItems && !HasPurchaseCardItems)
                {
                    return ClaimType.Credit;
                }
                else
                {
                    return ClaimType.Purchase;
                }
            }
        }

        /// <summary>
        /// Gets the reference number for the claim.
        /// </summary>
        public string ReferenceNumber { get; private set; }

        /// <summary>
        /// Gets whether the claim is at current, submitted or previous
        /// </summary>
        public ClaimStage ClaimStage
        {
            get
            {
                if (!submitted && !paid)
                {
                    return ClaimStage.Current;
                }
                else if (submitted && !paid)
                {
                    return ClaimStage.Submitted;
                }
                else
                {
                    return ClaimStage.Previous;
                }
            }
        }
        /// <summary>
        /// Gets whether there is any history associated with this claim
        /// </summary>
        public bool HasClaimHistory
        {
            get { return _hasClaimHistory; }
        }

        /// <summary>
        /// Gets the approver currently responsible for authorising the claim
        /// </summary>
        public string CurrentApprover
        {
            get { return _currentApprover; }
        }

        /// <summary>
        /// Gets the total number of stages the claim has to go through to be authorised
        /// </summary>
        public int TotalStageCount
        {
            get { return _totalStageCount; }
        }

        /// <summary>
        /// Gets whether the claim includes any expense items that have been returned for amendment
        /// </summary>
        public bool HasReturnedItems
        {
            get { return _hasReturnedItems; }
        }

        /// <summary>
        /// Gets whether there are any cash card items on the claim
        /// </summary>
        public bool HasCashItems
        {
            get { return _hasCashItems; }
        }
        /// <summary>
        /// Gets whether there are any credit card items on the claim
        /// </summary>
        public bool HasCreditCardItems
        {
            get { return _hasCreditCardItems; }
        }

        /// <summary>
        /// Gets whether there are any purchase card items on the claim
        /// </summary>
        public bool HasPurchaseCardItems
        {
            get { return _hasPurchaseCardItems; }
        }

        /// <summary>
        /// Gets whether there are any flagged items on the claim
        /// </summary>
        public bool HasFlaggedItems
        {
            get { return _hasFlaggedItems; }
        }

        /// <summary>
        /// Gets the total number of items on the claim
        /// </summary>
        public int NumberOfItems
        {
            get { return _numberOfItems; }
        }

        /// <summary>
        /// Gets the earliest item date on the claim
        /// </summary>
        public DateTime? StartDate
        {
            get { return _startDate; }
        }

        /// <summary>
        /// Gets the latest item date on the claim
        /// </summary>
        public DateTime? EndDate
        {
            get { return _endDate; }
        }

        /// <summary>
        /// Gets the total monetary value of the claim
        /// </summary>
        public decimal Total
        {
            get { return _total; }
        }
        /// <summary>
        /// Gets the monetary amount the claimant will receive back
        /// </summary>
        public decimal AmountPayable
        {
            get { return _amountPayable; }
        }

        /// <summary>
        /// Gets the total number of receipts on the claim
        /// </summary>
        public int NumberOfReceipts
        {
            get { return _numberOfReceipts; }
        }

        /// <summary>
        /// Gets the number of items that have not yet been approved
        /// </summary>
        public int NumberOfUnapprovedItems
        {
            get { return _numberOfUnapprovedItems; }
        }

        /// <summary>
        /// Gets the total monetary value of purchase card items
        /// </summary>
        public decimal PurchaseCardTotal
        {
            get { return _purchaseCardTotal; }
        }

        /// <summary>
        /// Gets the total monetary value of credit card items
        /// </summary>
        public decimal CreditCardTotal
        {
            get { return _creditCardTotal; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether pay before validate is active.
        /// </summary>
        public bool PayBeforeValidate { get; set; }

        #endregion

        /// <summary>
        /// Checks various criteria to ensure the Claim is valid
        /// </summary>
        /// <returns>Whether the Claim is valid</returns>
        public bool IsValid()
        {
            if (this._accountId < 0)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatIntegerGreaterThanOrEqualTo("Account Id", 0));   
            }

            if (this._claimId < 0)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatIntegerGreaterThan("Claim Id", 0));
            }

            if (this._claimNo <= 0)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatIntegerGreaterThan("Claim Number", 0));
            }

            if (this._employeeId <= 0)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatIntegerGreaterThan("Employee Id", 0));
            }

            if (string.IsNullOrEmpty(this._name))
            {
                throw new ArgumentNullException(ValidatorMessages.Mandatory("Claim name"));
            }

            if (this.name.Length > 50)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.StringLength("Claim name", 50));
            }

            if (this._description.Length > 2000)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.StringLength("Claim description", 2000));
            }

            if (this._stage < 0)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatIntegerGreaterThanOrEqualTo("Claim stage", 0));
            }

            if (DateValidators.IsDateNoGreaterThanTodayOrLess01011753(this._dateSubmitted))
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatDateAndTime("Date submitted"));
            }

            if (DateValidators.IsDateNoGreaterThanTodayOrLess01011753(this._datePaid))
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatDateAndTime("Date paid"));
            }

            if (!EnumValidator.EnumValueIsDefined(typeof(ClaimStatus), this._status))
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatInteger("Status"));
            }

            if (this._teamId < 0)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatIntegerGreaterThanOrEqualTo("Team id", 0));
            }

            if (this._checkerId < 0)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatIntegerGreaterThanOrEqualTo("Checker id", 0));
            }

            if (DateValidators.IsDateNoGreaterThanTodayOrLess01011753(this._createdOn))
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatDateAndTime("Created on"));
            }

            if (this._createdBy <= 0)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatIntegerGreaterThan("Created by", 0));
            }

            if (DateValidators.IsDateNoGreaterThanTodayOrLess01011753(this._modifiedOn))
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatDateAndTime("Modified on"));
            }

            if (this._modifiedBy < 0)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatIntegerGreaterThanOrEqualTo("Modified by", 0));
            }

            if (this._currencyId < 0)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatIntegerGreaterThanOrEqualTo("Currency id", 0));
            }

            if (this.ReferenceNumber.Length > 11)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.StringLength("Reference number", 11));
            }

            if (this._currentApprover.Length > 350)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.StringLength("Current approver", 350));
            }

            if (this._totalStageCount < 0)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatIntegerGreaterThanOrEqualTo("Total stage count", 0));
            }

            if (this._numberOfItems < 0)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatIntegerGreaterThanOrEqualTo("Number of items", 0));
            }

            if (DateValidators.IsDateNoGreaterThanTodayOrLess01011753(this._startDate))
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatDateAndTime("Start date"));
            }

            if (DateValidators.IsDateNoGreaterThanTodayOrLess01011753(this._endDate))
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatDateAndTime("End date"));
            }

            if (this._total < 0)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatIntegerGreaterThanOrEqualTo("Total", 0));
            }

            if (this._amountPayable < 0)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatIntegerGreaterThanOrEqualTo("Amount payable", 0));
            }

            if (this._numberOfReceipts < 0)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatIntegerGreaterThanOrEqualTo("Number of receipts", 0));
            }

            if (this._numberOfUnapprovedItems < 0)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatIntegerGreaterThanOrEqualTo("Number of unapproved items", 0));
            }

            if (this._creditCardTotal < 0)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatIntegerGreaterThanOrEqualTo("Credit card total", 0));
            }

            if (this._purchaseCardTotal < 0)
            {
                throw new ArgumentOutOfRangeException(ValidatorMessages.FormatIntegerGreaterThanOrEqualTo("Purchase card total", 0));
            }

            return true;
        }
    }


    [Serializable()]
    public class cClaimHistory
    {
        private int nClaimHistoryId;
        private int nClaimid;
        private string sComment;
        private int nEmployeeid;
        private DateTime dtDatestamp;
        private int nStage;
        private string sRefNum;
        private string sEnteredBy;

        public cClaimHistory(int claimhistoryid, int claimid, string comment, int employeeid, DateTime datestamp, int stage, string refnum = "", string enteredby = "")
        {
            nClaimHistoryId = claimhistoryid;
            nClaimid = claimid;
            sComment = comment;
            nEmployeeid = employeeid;
            dtDatestamp = datestamp;
            nStage = stage;
            sRefNum = refnum;
            sEnteredBy = enteredby;
        }

        #region Properties

        /// <summary>
        /// Gets the claimhistoryid.
        /// </summary>
        public int claimhistoryid
        {
            get { return nClaimHistoryId; }
        }

        /// <summary>
        /// Gets the claimid.
        /// </summary>
        public int claimid
        {
            get { return nClaimid; }
        }

        /// <summary>
        /// Gets the comment.
        /// </summary>
        public string comment
        {
            get { return sComment; }
        }

        /// <summary>
        /// Gets the employeeid.
        /// </summary>
        public int employeeid
        {
            get { return nEmployeeid; }
        }

        /// <summary>
        /// Gets the datestamp.
        /// </summary>
        public DateTime datestamp
        {
            get { return dtDatestamp; }
        }

        /// <summary>
        /// Gets the stage.
        /// </summary>
        public int stage
        {
            get { return nStage; }
        }

        /// <summary>
        /// Gets the refnum.
        /// </summary>
        public string refnum
        {
            get { return sRefNum; }
        }

        /// <summary>
        /// Gets the enteredby.
        /// </summary>
        public string enteredby
        {
            get { return sEnteredBy; }
        }

        #endregion
    }

    [Serializable()]
    public struct sOnlineClaimInfo
    {
        public Dictionary<int, cClaim> lstonlineclaims;
        public List<int> lstclaimids;
    }

    /// <summary>
    /// Claim Envelope Information struct
    /// </summary>
    [Serializable]
    public struct ClaimEnvelopeInfo
    {
        /// <summary>
        /// Gets or sets the Claim Envelope ID
        /// </summary>
        public int ClaimEnvelopeId;

        /// <summary>
        /// Gets or sets the Envelope Number
        /// </summary>
        public string EnvelopeNumber;

        /// <summary>
        /// The physical state of the envelope
        /// </summary>
        public string PhysicalState { get; set; }

        /// <summary>
        /// Any excess charges from postage or handling.
        /// </summary>
        public string ExcessCharge { get; set; }

        /// <summary>
        /// Details about if the envelope was processed / received / scanned after it had been marked lost.
        /// </summary>
        public string ProcessedAfterMarkedLost { get; set; }
    }

    /// <summary>
    /// Claim Envelope Summary struct, for use on the claim summary pages
    /// </summary>
    [Serializable]
    public struct ClaimEnvelopeSummary
    {
        /// <summary>
        /// The label text for envelope numbers. This is normally "Envelope Number:"
        /// </summary>
        public string LabelText;

        /// <summary>
        /// The first envelope number for the claim
        /// </summary>
        public string FirstEnvelopeNumber;

        /// <summary>
        /// Any additional envelope numbers for the claim
        /// </summary>
        public string AdditionalEnvelopesText;

        /// <summary>
        /// The list of envelope numbers, represented as HtmlGenericControls
        /// </summary>
        public List<System.Web.UI.HtmlControls.HtmlGenericControl> AdditionalEnvelopeList;
    }

    public enum ClaimStage
    {
        Current = 1,
        Submitted = 2,
        Previous = 3,
        Any = 4
    }

    public enum ClaimType
    {
        Mixed = 0,
        Cash = 1,
        Credit = 2,
        Purchase = 3
    }

    public enum ClaimStatus
    {
        None = 0,
        Submitted = 1,
        BeingProcessed = 2,
        NextStageAwaitingAction = 3,
        ItemReturnedAwaitingEmployee = 4,
        ItemCorrectedAwaitingApprover = 5,
        ClaimApproved = 6,
        ClaimPaid = 7,
        AwaitingAllocation = 8,
        PaymentProcessed = 9 //if the expenses are paid and marked as paid through expedite payment service , then this claim status =9.
    }
}
