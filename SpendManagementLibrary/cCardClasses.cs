namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SpendManagementLibrary.Cards;

    [Serializable()]
    public class cCardProvider
    {
        private int nCardProviderid;
        private string sCardProvider;
        private CorporateCardType ctCardtype;
        private DateTime dtCreatedOn;
        private int nCreatedBy;
        private DateTime? dtModifiedOn;
        private int? nModifiedBy;

        public cCardProvider()
        {
        }

        public cCardProvider(int cardproviderid, string cardprovider, CorporateCardType cardtype, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, bool autoImport)
        { 
            nCardProviderid = cardproviderid;
            sCardProvider = cardprovider;
            ctCardtype = cardtype;
            dtCreatedOn = createdon;
            nCreatedBy = createdby;
            dtModifiedOn = modifiedon;
            nModifiedBy = modifiedby;
            this.AutoImport = autoImport;
        }

        #region properties
        public int cardproviderid
        {
            get { return nCardProviderid; }
        }
        public string cardprovider
        {
            get { return sCardProvider; }
        }
        public CorporateCardType cardtype
        {
            get { return ctCardtype; }
        }
        public DateTime createdon
        {
            get { return dtCreatedOn; }
        }
        public int createdby
        {
            get { return nCreatedBy; }
        }
        public DateTime? modifiedon
        {
            get { return dtModifiedOn; }
        }
        public int? modifiedby
        {
            get { return nModifiedBy; }
        }

        /// <summary>
        /// Gets a value indicating whether the corporate card is set up as auto import.
        /// </summary>
        public bool AutoImport { get; }
        #endregion
    }

    public struct sCardProviderInfo
    {
        public Dictionary<int, cCardProvider> lstproviders;
        public List<int> lstproviderids;
    }

    [Serializable()]
    public class cCardStatement
    {
        /// <summary>
        /// The account id.
        /// </summary>
        private readonly int _accountId;
        private int nStatementid;
        private string sName;
        private DateTime? dtStatementdate;
        private DateTime dtCreatedOn;
        private int? nCreatedBy;
        private DateTime? dtModifiedOn;
        private int? nModifiedBy;

        public cCardStatement(int accountid, int corporateCardId, int statementid, string name, DateTime? statementdate, DateTime createdon, int? createdby, DateTime? modifiedon, int? modifiedby)
        {
            nStatementid = statementid;
            sName = name;
            dtStatementdate = statementdate;
            this._accountId = accountid;
            this.CorporateCardId = corporateCardId;
            dtCreatedOn = createdon;
            nCreatedBy = createdby;
            dtModifiedOn = modifiedon;
            nModifiedBy = modifiedby;
        }

        #region properties
        public int statementid
        {
            get { return nStatementid; }
        }
        public string name
        {
            get { return sName; }
        }
        public DateTime? statementdate
        {
            get { return dtStatementdate; }
        }

        /// <summary>
        /// Gets the corporate card for the statement.
        /// </summary>
        public cCorporateCard Corporatecard
        {
            get
            {
                var cards = new CorporateCards(this._accountId);
                return cards.GetCorporateCardById(this.CorporateCardId);
            }
        }

        public DateTime createdon
        {
            get { return dtCreatedOn; }
        }
        public int createdby
        {
            get { return nCreatedBy.HasValue ? this.nCreatedBy.Value : 0; }
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

        /// <summary>
        /// Gets or sets the corporate card id.
        /// </summary>
        public int CorporateCardId { get; set; }
    }

    [Serializable()]
    public class cCardTransaction
    {
        private int nTransactionid;
        private int nStatementid;
        private string sReference;
        private string sCardNumber;
        private int? nCorporateCardId;
        private DateTime? dtTransactionDate;
        private string sDescription;
        private decimal dTransactionAmount;
        private decimal dOriginalAmount;
        private string sCurrencyCode;
        private int? nGlobalCurrencyId;
        private decimal dExchangeRate;
        private string sCountryCode;
        private int? nGlobalCountryId;
        private DateTime dtCreatedOn;
        private int nCreatedBy;
        private DateTime? dtModifiedOn;
        private int? nModifiedBy;

        private Dictionary<string, object> lstMoreDetails;

        public cCardTransaction(int transactionid, int statementid, string reference, string cardnumber, int? corporatecardid, DateTime? transactiondate, string description, decimal transactionamount, decimal originalamount, string currencycode, int? globalcurrencyid, decimal exchangerate, Dictionary<string, object> moredetails, string countrycode, int? globalcountryid, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby)
        {
            nTransactionid = transactionid;
            nStatementid = statementid;
            sReference = reference;
            sCardNumber = cardnumber;
            nCorporateCardId = corporatecardid;
            dtTransactionDate = transactiondate;
            sDescription = description;
            dTransactionAmount = transactionamount;
            dOriginalAmount = originalamount;
            sCurrencyCode = currencycode;
            nGlobalCurrencyId = globalcurrencyid;
            dExchangeRate = exchangerate;
            lstMoreDetails = moredetails;
            sCountryCode = countrycode;
            nGlobalCountryId = globalcountryid;
            dtCreatedOn = createdon;
            nCreatedBy = createdby;
            dtModifiedOn = modifiedon;
            nModifiedBy = modifiedby;
        }

        #region properties
        public int transactionid
        {
            get { return nTransactionid; }
        }
        public int statementid
        {
            get { return nStatementid; }
        }
        public string reference
        {
            get { return sReference; }
        }
        public string cardnumber
        {
            get { return sCardNumber; }
        }
        public int? corporatecardid
        {
            get { return nCorporateCardId; }
        }
        public DateTime? transactiondate
        {
            get { return dtTransactionDate; }
        }
        public string description
        {
            get
            {
                return sDescription;
            }
        }
        public decimal transactionamount
        {
            get { return dTransactionAmount; }
        }
        public decimal originalamount
        {
            get { return dOriginalAmount; }
        }
        public string currencycode
        {
            get { return sCurrencyCode; }
        }
        public int? globalcurrencyid
        {
            get { return nGlobalCurrencyId; }
        }
        public decimal exchangerate
        {
            get { return dExchangeRate; }
        }
        public Dictionary<string, object> moredetails
        {
            get { return lstMoreDetails; }
        }
        public decimal reverseexchangerate
        {
            get { return Math.Round(originalamount / transactionamount, 5); }
        }
        public string countrycode
        {
            get { return sCountryCode; }
        }
        public int? globalcountryid
        {
            get { return nGlobalCountryId; }
        }
        public DateTime createdon
        {
            get { return dtCreatedOn; }
        }
        public int createdby
        {
            get { return nCreatedBy; }
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

    }

    [Serializable()]
    public class cCorporateCard
    {
        private cCardProvider cpCardProvider;
        private bool bClaimantSettlesBill;
        private DateTime dtCreatedOn;
        private int nCreatedBy;
        private DateTime? dtModifiedOn;
        private int? nModifiedBy;
        private int? nAllocatedItem;
        private bool bBlockCash;
        private bool bReconciledByAdministrator;
        private bool bSingleClaim;

        public cCorporateCard(cCardProvider cardprovider, bool claimantsettlesbill, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, int? allocateditem, bool blockcash, bool reconciledbyadministrator, bool singleclaim, string fileIdentifier)
        {
            cpCardProvider = cardprovider;
            bClaimantSettlesBill = claimantsettlesbill;
            dtCreatedOn = createdon;
            nCreatedBy = createdby;
            dtModifiedOn = modifiedon;
            nModifiedBy = modifiedby;
            nAllocatedItem = allocateditem;
            bBlockCash = blockcash;
            bReconciledByAdministrator = reconciledbyadministrator;
            bSingleClaim = singleclaim;
            this.FileIdentifier = fileIdentifier;
        }

        #region properties
        public cCardProvider cardprovider
        {
            get { return cpCardProvider; }
        }
        public bool claimantsettlesbill
        {
            get { return bClaimantSettlesBill; }
        }
        public DateTime createdon
        {
            get { return dtCreatedOn; }
        }
        public int createdby
        {
            get { return nCreatedBy; }
        }
        public DateTime? modifiedon
        {
            get { return dtModifiedOn; }
        }
        public int? modifiedby
        {
            get { return nModifiedBy; }
        }
        public int? allocateditem
        {
            get { return nAllocatedItem; }
        }
        public bool blockcash
        {
            get { return bBlockCash; }
        }
        public bool reconciledbyadministrator
        {
            get { return bReconciledByAdministrator; }
        }
        public bool singleclaim
        {
            get { return bSingleClaim; }
        }

        /// <summary>
        /// Gets the card provider customer identifier.
        /// </summary>
        public string FileIdentifier { get; }
        #endregion

    }

    /// <summary>
    /// This object holds all the information and mappings about the import template
    /// </summary>
    [Serializable()]
    public class cCardTemplate
    {
        private string sName;
        private int nNumberHeaderRecords;
        private int nNumberFooterRecords;
        private ImportType itFileType;
        private string sDelimiter;
        private SortedList<CardRecordType, cCardRecordType> lstRecordTypes;
        private int nSheet;
        private string sQuotedCharacters;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filetype"></param>
        /// <param name="delimiter"></param>
        /// <param name="numberheaderrecords"></param>
        /// <param name="numberfooterrecords"></param>
        /// <param name="sheet"></param>
        /// <param name="RecordTypes"></param>
        public cCardTemplate(string name, int numberHeaderRecords, int numberFooterRecords, ImportType filetype, string delimiter, int sheet, SortedList<CardRecordType, cCardRecordType> RecordTypes, string quotedCharacters)
        {
            sName = name;
            nNumberHeaderRecords = numberHeaderRecords;
            nNumberFooterRecords = numberFooterRecords;
            itFileType = filetype;
            sDelimiter = delimiter;
            lstRecordTypes = RecordTypes;
            nSheet = sheet;
            sQuotedCharacters = quotedCharacters;
        }


        #region properties

        /// <summary>
        /// The name of the template
        /// </summary>
        public string name
        {
            get { return sName; }
        }

        /// <summary>
        /// Number of header records
        /// </summary>
        public int numHeaderRecords
        {
            get { return nNumberHeaderRecords; }
        }

        /// <summary>
        /// Number of footer records
        /// </summary>
        public int numFooterRecords
        {
            get { return nNumberFooterRecords; }
        }

        /// <summary>
        /// The file type of the import
        /// </summary>
        public ImportType filetype
        {
            get { return itFileType; }
        }

        //The character or characters to delimit the flat file by
        public string delimiter
        {
            get { return sDelimiter; }
        }

        /// <summary>
        /// The sheet index of an Excel import
        /// </summary>
        public int sheet
        {
            get { return nSheet; }
        }

        /// <summary>
        /// List of all the record types used for the import template
        /// </summary>
        public SortedList<CardRecordType, cCardRecordType> RecordTypes
        {
            get { return lstRecordTypes; }
        }

        /// <summary>
        /// The characters that may be enclosing field values that need to be removed from the data
        /// </summary>
        public string QuotedCharacters
        {
            get { return sQuotedCharacters; }
        }

        #endregion
    }

    /// <summary>
    /// The object that stores the information on the card record types 
    /// </summary>
    [Serializable()]
    public class cCardRecordType
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eRecordType"></param>
        /// <param name="sUniqueValue"></param>
        /// <param name="sExcludeValue"></param>
        /// <param name="nExcludeValueColumnIndex"></param>
        /// <param name="ExcludeValues"></param>
        /// <param name="lstFields"></param>
        /// <param name="lstHeaderFields"></param>
        /// <param name="lstFooterFields"></param>
        public cCardRecordType(CardRecordType eRecordType, string sUniqueValue, List<ExcludeValueObject> lstExcludeValues, List<cCardTemplateField> lstFields, List<cValidationField> lstHeaderFields, List<cValidationField> lstFooterFields)
        {
            RecordType = eRecordType;
            UniqueValue = sUniqueValue;
            ExcludeValues = lstExcludeValues;
            Fields = lstFields;
            HeaderFields = lstHeaderFields;
            FooterFields = lstFooterFields;
        }

        #region Properties

        /// <summary>
        /// The card statement record type
        /// </summary>
        public CardRecordType RecordType { get; set; }

        /// <summary>
        /// The unique value of the card record type
        /// </summary>
        public string UniqueValue { get; set; }

        /// <summary>
        /// List of values and their associated indexes to search for if the row is to be omitted
        /// </summary>
        public List<ExcludeValueObject> ExcludeValues { get; set; }

        /// <summary>
        /// The list of the fields that will be used to map the data for the record type
        /// </summary>
        public List<cCardTemplateField> Fields { get; set; }

        /// <summary>
        /// The list of header fields that are used to map the record type
        /// </summary>
        public List<cValidationField> HeaderFields { get; set; }

        /// <summary>
        /// The list of footer fields that are used to map the record type
        /// </summary>
        public List<cValidationField> FooterFields { get; set; }

        #endregion

        /// <summary>
        /// Get a field by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public cCardTemplateField getFieldByName(string name)
        {
            return this.Fields.FirstOrDefault(field => field.name.ToLower() == name.ToLower());
        }

        /// <summary>
        /// Get a field by its database mapped field
        /// </summary>
        /// <param name="mappedfield"></param>
        /// <returns></returns>
        public cCardTemplateField getFieldByMappedField(string mappedfield)
        {
            return this.Fields.FirstOrDefault(field => field.mappedfield.ToLower() == mappedfield.ToLower());
        }

        /// <summary>
        /// Get the additional table name 
        /// </summary>
        public string additionaltable
        {
            get
            {
                string tbl = "";
                foreach (cCardTemplateField field in Fields)
                {
                    if (field.mappedtable != "card_transactions")
                    {
                        tbl = field.mappedtable;
                    }
                    break;
                }
                return tbl;
            }
        }

        /// <summary>
        /// Get the start and end indexs of the fields for a fixed width flat file
        /// </summary>
        /// <returns></returns>
        public int[,] getStartAndEndLengths()
        {
            cFlatField flatfield;
            List<int[]> lengths = new List<int[]>();
            foreach (cCardTemplateField field in Fields)
            {
                if (field.GetType() == typeof(cFlatField))
                {
                    flatfield = (cFlatField)field;
                    lengths.Add(new int[] { flatfield.startposition, flatfield.endposition });
                }
            }
            int[,] arrlengths = new int[lengths.Count, 2];
            for (int i = 0; i < lengths.Count; i++)
            {
                arrlengths[i, 0] = lengths[i][0];
                arrlengths[i, 1] = lengths[i][1];
            }
            return arrlengths;
        }

        /// <summary>
        /// Get the index for the sign of thetransaction and current amount for a fixed width flat file
        /// </summary>
        /// <returns>An array of indexes of the location of the sign</returns>
        public int[] GetSignValues()
        {
            cFlatField flatfield;
            List<int[]> lengths = new List<int[]>();
            foreach (cCardTemplateField field in Fields)
            {
                if (field.GetType() == typeof(cFlatField))
                {
                    flatfield = (cFlatField)field;
                    lengths.Add(new int[] { flatfield.sign });
                }
            }
            int[] sign = new int[lengths.Count];
            for (int i = 0; i < lengths.Count; i++)
            {
                sign[i] = lengths[i][0];
            }

            return sign;
        }
    }

    /// <summary>
    /// Type to define the card record type
    /// </summary>
    public enum CardRecordType
    {
        /// <summary>
        /// None Value
        /// </summary>
        None = 0,
        /// <summary>
        /// The company record in the credit card statement
        /// </summary>
        CardCompany = 1,

        /// <summary>
        /// The transaction record in the credit card statement 
        /// </summary>
        CardTransaction = 2
    }

    /// <summary>
    ///  Object storing the value and associated index to search for if the row is to be omitted
    /// </summary>
    public struct ExcludeValueObject
    {
        public int excludeValueIndex;
        public string excludeValue;
    }

    /// <summary>
    /// Object to store the card companies found in the VCF4 credit card format
    /// </summary>
    public class cCardCompany
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cardCompanyID"></param>
        /// <param name="companyName"></param>
        /// <param name="companyNumber"></param>
        /// <param name="usedForImport"></param>
        public cCardCompany(int nCardCompanyID, string sCompanyName, string sCompanyNumber, bool bUsedForImport, DateTime? dtCreatedOn, int? nCreatedBy, DateTime? dtModifiedOn, int? nModifiedBy)
        {
            cardCompanyID = nCardCompanyID;
            companyName = sCompanyName;
            companyNumber = sCompanyNumber;
            usedForImport = bUsedForImport;
            createdOn = dtCreatedOn;
            createdBy = nCreatedBy;
            modifiedOn = dtModifiedOn;
            modifiedBy = nModifiedBy;
        }

        /// <summary>
        /// Parameterless constructor for use with Ajax
        /// </summary>
        public cCardCompany() { }

        #region Properties

        /// <summary>
        /// ID of the Card Company
        /// </summary>
        public int cardCompanyID { get; set; }

        /// <summary>
        /// The name of the company from the vcf4 file
        /// </summary>
        public string companyName { get; set; }

        /// <summary>
        /// The number of the company from the vcf4 file used to reference the transactions 
        /// </summary>
        public string companyNumber { get; set; }

        /// <summary>
        /// This is a check to see if the company is used for the card import 
        /// </summary>
        public bool usedForImport { get; set; }

        /// <summary>
        /// Created On date
        /// </summary>
        public DateTime? createdOn { get; set; }

        /// <summary>
        /// Created By
        /// </summary>
        public int? createdBy { get; set; }

        /// <summary>
        /// Modified On Date
        /// </summary>
        public DateTime? modifiedOn { get; set; }

        /// <summary>
        /// Modified By
        /// </summary>
        public int? modifiedBy { get; set; }

        #endregion


    }

    /// <summary>
    /// This is the object that contains the validation datya for the header and footer fields of the credit card import
    /// </summary>
    [Serializable()]
    public class cValidationField
    {
        private string sFieldType;
        private string sExpectedText;
        private string sTransactionCode;
        private int nRecordTypeColumnIndex;
        private int nLookupColumnIndex;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fieldtype"></param>
        /// <param name="expectedtext"></param>
        /// <param name="TransactionCode"></param>
        /// <param name="RecordTypeStartIndex"></param>
        /// <param name="RecordTypeEndIndex"></param>
        public cValidationField(string fieldType, string expectedText, string TransactionCode, int RecordTypeColumnIndex, int LookupColumnIndex)
        {
            sFieldType = fieldType;
            sExpectedText = expectedText;
            sTransactionCode = TransactionCode;
            nRecordTypeColumnIndex = RecordTypeColumnIndex;
            nLookupColumnIndex = LookupColumnIndex;
        }

        #region properties

        /// <summary>
        /// The data type of the field
        /// </summary>
        public string fieldType
        {
            get { return sFieldType; }
        }

        /// <summary>
        /// The text to be expected on the header
        /// </summary>
        public string expectedText
        {
            get { return sExpectedText; }
        }
        /// <summary>
        /// The type of transaction type:
        /// Header = 8
        /// Footer = 9 etc
        /// </summary>
        public string TransactionCode
        {
            get { return sTransactionCode; }
        }

        /// <summary>
        /// The start index of the record type field value
        /// </summary>
        public int RecordTypeColumnIndex
        {
            get { return nRecordTypeColumnIndex; }
        }

        /// <summary>
        /// The index of the lookup column to match to the header field value
        /// </summary>
        public int LookupColumnIndex
        {
            get { return nLookupColumnIndex; }
        }

        #endregion
    }

    [Serializable()]
    public class cCardTemplateField
    {
        protected string sName;
        protected string sMappedTable;
        protected string sMappedField;
        protected string sFieldType;
        protected int nMaxLength;
        protected bool bDisplayInUnallocatedCardGrid;
        protected string sLabel;
        protected CurrencyLookup clCurrencylookup;
        protected CountryLookup clCountryLookup;
        protected bool bDisplayInMoreDetailsTable;
        protected string sFormat;
        protected string sDefaultValue;
        protected int nFieldIndex;
        protected int? nNumDecimalPlaces;
        public cCardTemplateField()
        {
        }
        public cCardTemplateField(string name, string mappedtable, string mappedfield, string fieldtype, int maxlength, bool displayinunallocatedcardgrid, string label, bool displayinmoredetailstable, string format, string defaultvalue, int FieldIndex, int? NumDecimalPlaces)
        {
            sName = name;
            sMappedTable = mappedtable;
            sMappedField = mappedfield;
            sFieldType = fieldtype;
            nMaxLength = maxlength;
            bDisplayInUnallocatedCardGrid = displayinunallocatedcardgrid;
            sLabel = label;
            sDefaultValue = defaultvalue;
            bDisplayInMoreDetailsTable = displayinmoredetailstable;
            sFormat = format;
            nFieldIndex = FieldIndex;
            nNumDecimalPlaces = NumDecimalPlaces;
        }

        public cCardTemplateField(string name, string mappedtable, string mappedfield, string fieldtype, int maxlength, bool displayinunallocatedcardgrid, string label, bool displayinmoredetailstable, string format, string defaultvalue, int FieldIndex, CurrencyLookup currencylookup, CountryLookup countrylookup, int? NumDecimalPlaces)
        {
            sName = name;
            sMappedTable = mappedtable;
            sMappedField = mappedfield;
            sFieldType = fieldtype;
            nMaxLength = maxlength;
            clCurrencylookup = currencylookup;
            bDisplayInUnallocatedCardGrid = displayinunallocatedcardgrid;
            sLabel = label;
            bDisplayInMoreDetailsTable = displayinmoredetailstable;
            sFormat = format;
            sDefaultValue = defaultvalue;
            clCountryLookup = countrylookup;
            nFieldIndex = FieldIndex;
            nNumDecimalPlaces = NumDecimalPlaces;
        }


        #region properties
        public string name
        {
            get { return sName; }
        }
        public string mappedtable
        {
            get { return sMappedTable; }
        }
        public string mappedfield
        {
            get { return sMappedField; }
        }
        public string fieldtype
        {
            get { return sFieldType; }
        }
        public int maxlength
        {
            get { return nMaxLength; }
        }
        public CurrencyLookup currencylookup
        {
            get { return clCurrencylookup; }
        }
        public bool displayinunallocatedcardgrid
        {
            get { return bDisplayInUnallocatedCardGrid; }
        }
        public string label
        {
            get { return sLabel; }
        }
        public bool displayinmoredetailstable
        {
            get { return bDisplayInMoreDetailsTable; }
        }
        public string format
        {
            get { return sFormat; }
        }
        public CountryLookup countrylookup
        {
            get { return clCountryLookup; }
        }
        public string defaultvalue
        {
            get { return sDefaultValue; }
        }
        public int FieldIndex
        {
            get { return nFieldIndex; }
        }
        public int? numDecimalPlaces
        {
            get { return nNumDecimalPlaces; }
        }
        #endregion
    }

    [Serializable()]
    public class cFlatField : cCardTemplateField
    {
        private int nStartPosition;
        private int nEndPosition;
        private int nSign;

        public cFlatField(string name, string mappedtable, string mappedfield, string fieldtype, int maxlength, int startposition, int endposition, bool displayinunallocatedcardgrid, string label, bool displayinmoredetailsgrid, string format, string defaultvalue, int? NumDecimalPlaces, int sign)
        {
            sName = name;
            sMappedTable = mappedtable;
            sMappedField = mappedfield;
            sFieldType = fieldtype;
            nMaxLength = maxlength;
            nStartPosition = startposition;
            nEndPosition = endposition;
            bDisplayInUnallocatedCardGrid = displayinunallocatedcardgrid;
            bDisplayInMoreDetailsTable = displayinmoredetailsgrid;
            sLabel = label;
            sFormat = format;
            sDefaultValue = defaultvalue;
            nNumDecimalPlaces = NumDecimalPlaces;
            nSign = sign;
        }

        public cFlatField(string name, string mappedtable, string mappedfield, string fieldtype, int maxlength, int startposition, int endposition, bool displayinunallocatedcardgrid, string label, bool displayinmoredetailsgrid, string format, string defaultvalue, CurrencyLookup currencylookup, CountryLookup countrylookup, int? NumDecimalPlaces, int sign)
        {
            sName = name;
            sMappedTable = mappedtable;
            sMappedField = mappedfield;
            sFieldType = fieldtype;
            nMaxLength = maxlength;
            nStartPosition = startposition;
            nEndPosition = endposition;
            bDisplayInMoreDetailsTable = displayinmoredetailsgrid;
            clCurrencylookup = currencylookup;
            bDisplayInUnallocatedCardGrid = displayinunallocatedcardgrid;
            sLabel = label;
            sFormat = format;
            clCountryLookup = countrylookup;
            sDefaultValue = defaultvalue;
            nNumDecimalPlaces = NumDecimalPlaces;
            nSign = sign;
        }

        #region properties
        public int startposition
        {
            get { return nStartPosition; }
        }
        public int endposition
        {
            get { return nEndPosition; }
        }
        public string label
        {
            get { return sLabel; }
        }
        public int sign
        {
            get { return nSign; }
        }
        #endregion
    }

    [Serializable()]
    public enum CurrencyLookup
    {
        Numeric,
        Alpha,
        Label,
        None
    }

    [Serializable()]
    public enum CountryLookup
    {
        /// <summary>
        /// ISO 3166-1 alpha-2 (2 letter country code)
        /// </summary>
        Alpha,
        /// <summary>
        /// ISO 3166-1 English short name lower case (despite it's name it is Camel case)
        /// </summary>
        Label,
        /// <summary>
        /// ISO 3166-1 alpha-3 (3 letter country code)
        /// </summary>
        Alpha3,
        /// <summary>
        /// ISO 3166-1 numeric-3 (3 interger country code)
        /// </summary>
        Numeric3,
        None
    }

    [Serializable()]
    public struct sCardInfo
    {
        public Dictionary<int, cEmployeeCorporateCard> lstempcorpcards;
        public List<int> lstempcorpcardids;
        public Dictionary<int, cCardProvider> lstproviders;
        public List<int> lstproviderids;
        public Dictionary<int, cCorporateCard> lstcorporatecards;
        public List<int> lstcorporatecardids;
        public Dictionary<string, byte[]> lsttemplates;
        public Dictionary<int, cCardStatement> lststatements;
        public List<int> lststatementids;
        public Dictionary<int, cCardTransaction> lsttransactions;
        public List<int> lsttransactionids;
    }
}
