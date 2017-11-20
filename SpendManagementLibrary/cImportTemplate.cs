using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cImportTemplate
    {
        private int nTemplateID;
        private string sTemplateName;
        private ApplicationType eAppType;
        private bool bIsAutomated;
        private int nNHSTrustID;
        private List<cImportTemplateMapping> lstMappings;
        private DateTime dtCreatedOn;
        private int nCreatedBy;
        private DateTime? dtModifiedOn;
        private int? nModifiedBy;

        public cImportTemplate(int TemplateID, string TemplateName, ApplicationType AppType, bool IsAutomated, int NHSTrustID, Guid signOffOwnerFieldId, Guid lineManagerFieldId, List<cImportTemplateMapping> Mappings, DateTime createdOn, int createdBy, DateTime? modifiedOn, int? modifiedBy)
        {
            this.nTemplateID = TemplateID;
            this.sTemplateName = TemplateName;
            this.eAppType = AppType;
            this.bIsAutomated = IsAutomated;
            this.nNHSTrustID = NHSTrustID;
            this.SignOffOwnerFieldId = signOffOwnerFieldId;
            this.LineManagerFieldId = lineManagerFieldId;
            this.lstMappings = Mappings;
            this.dtCreatedOn = createdOn;
            this.nCreatedBy = createdBy;
            this.dtModifiedOn = modifiedOn;
            this.nModifiedBy = modifiedBy;
        }

        #region Properties
        /// <summary>
        /// Unique ID of the import template
        /// </summary>
        public int TemplateID
        {
            get { return nTemplateID; }
        }

        /// <summary>
        /// Name of the import template
        /// </summary>
        public string TemplateName
        {
            get { return sTemplateName; }
        }

        /// <summary>
        /// Type of third party application
        /// </summary>
        public ApplicationType appType
        {
            get { return eAppType; }
        }

        /// <summary>
        /// Flag to see if the import template will be run automatically
        /// </summary>
        public bool IsAutomated
        {
            get { return bIsAutomated; }
        }

        /// <summary>
        /// ID of an associated NHS Trust
        /// </summary>
        public int NHSTrustID
        {
            get { return nNHSTrustID; }
        }

        /// <summary>
        /// List of field mappings for the import template
        /// </summary>
        public List<cImportTemplateMapping> Mappings
        {
            get { return lstMappings; }
        }

        /// <summary>
        /// Date object created on
        /// </summary>
        public DateTime createdOn
        {
            get { return dtCreatedOn; }
        }

        /// <summary>
        /// User who created the object
        /// </summary>
        public int createdBy
        {
            get { return nCreatedBy; }
        }

        /// <summary>
        /// Date object modified on
        /// </summary>
        public DateTime? modifiedOn
        {
            get { return dtModifiedOn; }
        }

        /// <summary>
        /// User who modified the object
        /// </summary>
        public int? modifiedBy
        {
            get { return nModifiedBy; }
        }

        public Guid SignOffOwnerFieldId { get; private set; }
        public Guid LineManagerFieldId { get; private set; }

        #endregion

        public cImportTemplateMapping GetMappingByColRef(string ElementType, int ColRef)
        {
            foreach (cImportTemplateMapping val in lstMappings)
            {
                if (val.ElementType.ToString() == ElementType && val.ColRef == ColRef)
                {
                    return val;
                }
            }
            return null;
        }
    }


    [Serializable()]
    public class cImportTemplateMapping
    {
        private readonly int _TemplateMappingID;

        private readonly int _TemplateID;

        private readonly Guid _FieldID;

        private readonly cField _clsField;

        private readonly string sDestinationField;

        private readonly int _ColRef;

        private readonly ImportElementType _ElementType;

        private readonly bool _Mandatory;

        private readonly DataType _DataType;

        private readonly bool _Populated;

        private readonly bool _OverridePK;

        private readonly cField _MatchField;

        private readonly cTable _LookupTable;

        private readonly bool _AllowDynamicMapping;

        private readonly bool _ImportField;

        private Guid _DefaultMappingFieldID;

        private cField _DefaultMappingField;

        private cTable _DefaultMappingTable;

        public cImportTemplateMapping(
            int TemplateMappingID,
            int TemplateID,
            Guid FieldID,
            cField field,
            string DestinationField,
            int ColRef,
            ImportElementType ElementType,
            bool Mandatory,
            DataType dataType,
            cTable lookup_table,
            cField match_field,
            bool override_primarykey,
            bool populated,
            bool allowDynamicMapping,
            bool importField)
        {
            this._TemplateMappingID = TemplateMappingID;
            _TemplateID = TemplateID;
            _FieldID = FieldID;
            _clsField = field;
            sDestinationField = DestinationField;
            _ColRef = ColRef;
            _ElementType = ElementType;
            _Mandatory = Mandatory;
            _DataType = dataType;
            _LookupTable = lookup_table;
            _MatchField = match_field;
            _OverridePK = override_primarykey;
            _Populated = populated;
            _AllowDynamicMapping = allowDynamicMapping;
            _ImportField = importField;
        }

        #region Properties

        /// <summary>
        /// ID of the template field mapping
        /// </summary>
        public int TemplateMappingID
        {
            get
            {
                return this._TemplateMappingID;
            }
        }

        /// <summary>
        /// ID of the import template associated
        /// </summary>
        public int TemplateID
        {
            get
            {
                return _TemplateID;
            }
        }

        /// <summary>
        /// Unique Identifier of the field object for the mapping 
        /// </summary>
        public Guid FieldID
        {
            get
            {
                return _FieldID;
            }
        }

        public cField Field
        {
            get
            {
                return _clsField;
            }
        }
        /// <summary>
        /// String value of the destination field the mapping is linked to
        /// </summary>
        public string DestinationField
        {
            get
            {
                return sDestinationField;
            }
        }

        /// <summary>
        /// Int value of the column reference for the mapping in the third party file format
        /// </summary>
        public int ColRef
        {
            get
            {
                return _ColRef;
            }
        }

        /// <summary>
        /// The type of import element the field mapping is
        /// </summary>
        public ImportElementType ElementType
        {
            get
            {
                return _ElementType;
            }
        }

        /// <summary>
        /// Is the mapping field mandatory
        /// </summary>
        public bool Mandatory
        {
            get
            {
                return _Mandatory;
            }
        }

        /// <summary>
        /// The data type for the  mapping field
        /// </summary>
        public DataType dataType
        {
            get
            {
                return _DataType;
            }
        }

        /// <summary>
        /// Gets the table used for lookup fields
        /// </summary>
        public cTable LookupTable
        {
            get
            {
                return _LookupTable;
            }
        }

        /// <summary>
        /// Gets the field used for lookup matching
        /// </summary>
        public cField MatchField
        {
            get
            {
                return _MatchField;
            }
        }

        /// <summary>
        /// Gets whether the lookup field uses the value supplied rather than the primary key of the lookup table
        /// </summary>
        public bool OverridePrimaryKey
        {
            get
            {
                return _OverridePK;
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the fields are populated in the outbound file
        /// </summary>
        public bool Populated
        {
            get
            {
                return _Populated;
            }
        }

        /// <summary>
        /// Gets indication of whether user can override the default mapping with destination of their own
        /// </summary>
        public bool AllowDynamicMapping
        {
            get
            {
                return _AllowDynamicMapping;
            }
        }

        /// <summary>
        /// Gets indication of whether the field should be imported or skipped.
        /// </summary>
        public bool ImportField
        {
            get
            {
                return _ImportField;
            }
        }

        /// <summary>
        /// Gets or Sets default mapping field when user mapping overrides it to save a safe copy
        /// </summary>
        public cField DefaultMappingField
        {
            get
            {
                return _DefaultMappingField;
            }
            set
            {
                _DefaultMappingField = value;
                this._DefaultMappingFieldID = this._DefaultMappingField != null ? this._DefaultMappingField.FieldID : Guid.Empty;
            }
        }

        /// <summary>
        /// Gets the default mapping field ID when user mapping has overridden it
        /// </summary>
        public Guid DefaultMappingFieldId
        {
            get
            {
                return _DefaultMappingFieldID;
            }
        }

        /// <summary>
        /// Gets or Sets the default mapping table when mapping is not the default ESR table
        /// </summary>
        public cTable DefaultMappingTable
        {
            get
            {
                return _DefaultMappingTable;
            }
            set
            {
                _DefaultMappingTable = value;
            }
        }

        #endregion
    }

    [Serializable()]
    public enum ApplicationType
    {
        ESROutboundImport = 0,
        ExcelImport = 1,
        EsrOutboundImportV2 = 2
    }

    [Serializable()]
    public enum ImportElementType
    {
        None = 0,
        Employee,
        Assignment,
        Location,
        Organisation,
        Position,
        Phone,
        Address,
        Vehicle,
        Costing
    }

    [Serializable()]
    public enum ImportHistoryStatus
    {
        /// <summary>
        /// Import was a success.
        /// </summary>
        Success = 0,

        /// <summary>
        /// import was a failure .
        /// </summary>
        Failure = 1,

        /// <summary>
        /// import was a success with errors.
        /// </summary>
        Success_With_Errors = 2,

        /// <summary>
        /// import was a success with warnings.
        /// </summary>
        Success_With_Warnings = 3,

        /// <summary>
        /// import is in progress.
        /// </summary>
        InProgress = 4
    }

    [Serializable()]
    public struct XMLMapFields
    {
        public string DestinationField;
        public int ColRef;
        public bool Mandatory;
        public DataType dataType;
        public string referenceTable;
        public string referenceField;
        public string lookupTable;
        public string matchField;
        public bool populated;
        public bool overridePrimaryKey;
        public bool allowDynamicMapping;
        public string defaultTableName;
        public string defaultFieldName;
        public bool importField;
        public string relatedExpensesTable;
    }
    
}
