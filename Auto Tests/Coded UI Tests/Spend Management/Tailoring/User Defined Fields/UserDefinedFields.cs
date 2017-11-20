namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Tailoring.User_Defined_Fields
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entities;
    using Auto_Tests.Tools;
    using System.ComponentModel;

    /// <summary>
    /// user defined fields
    /// </summary>
    public class UserDefinedFields : CustomEntitiesUtilities.CustomEntityAttribute
    {
        /// <summary>
        /// the allow search
        /// </summary>
        public bool AllowSearch { get; set; }

        /// <summary>
        /// the specific
        /// </summary>
        public bool Specific { get; set; }

        /// <summary>
        /// the hyperlink text
        /// </summary>
        public string HyperLinkText { get; set; }

        /// <summary>
        /// the hyperlink path
        /// </summary>
        public string HyperLinkPath { get; set; }

        /// <summary>
        /// the groupid
        /// </summary>
        public int GroupID { get; set; }

        /// <summary>
        /// the order
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// the table id
        /// </summary>
        public Guid TableId { get; set; }

        /// <summary>
        /// the allow employee to populate
        /// </summary>
        public bool allowEmployeeToPopulate { get; set; }

        /// <summary>
        /// user defined fields default constructor
        /// </summary>
        public UserDefinedFields() { }

        public static string SqlItems
        {
            get
            {
                return "SELECT attribute_name, fieldtype, specific, mandatory, description, [order], tooltip, maxlength, format, defaultvalue, fieldid, tableid, groupID, precision, userdefineid, allowSearch, hyperlinkText, hyperlinkPath, relatedTable, displayField FROM userdefined";
            }
        }

        /// <summary>
        /// the user defined field grid values
        /// </summary>
        public List<string> UserDefinedFieldsGridValues
        {
            get
            {
                return new List<string>()
                {
                    DisplayName,
                    _description,
                    EnumHelper.GetEnumDescription(_fieldType),
                    _mandatory.ToString(),
                    EnumHelper.GetEnumDescription(GetFriendlyName(TableId))
                };
            }
        }

        public AppliesTo GetFriendlyName(Guid tableId)
        {
            if (tableId == Guid.Parse("7e9e6bee-f8ca-45d8-b914-1a9b105e47b2"))
            {
                return AppliesTo.Cars;
            }
            if (tableId == Guid.Parse("f70d6e0d-8e38-4a1d-a681-cc9d310c2ae9"))
            {
                return AppliesTo.Claims;
            }
            if (tableId == Guid.Parse("e4cca1ba-a065-4116-860b-abaa1e7bb2ef"))
            {
                return AppliesTo.CostCodes;
            }
            if (tableId == Guid.Parse("155ae388-1b60-4fb2-a1bd-c46f543fa401"))
            {
                return AppliesTo.Departments;
            }
            if (tableId == Guid.Parse("972ac42d-6646-4efc-9323-35c2c9f95b62"))
            {
                return AppliesTo.Employees;
            }
            if (tableId == Guid.Parse("65394331-792e-40b8-af8b-643505550783"))
            {
                return AppliesTo.ExpenseItemCategories;
            }
            if (tableId == Guid.Parse("7d323dae-3494-4d9b-b1a0-85f5a2d69e1b"))
            {
                return AppliesTo.Addresses;
            }
            return AppliesTo.ProjectCodes;
        }

        /// <summary>
        /// User defined fields custom constructor
        /// </summary>
        /// <param name="createdby"></param>
        /// <param name="modifiedBy"></param>
        /// <param name="DisplayName"></param>
        /// <param name="Description"></param>
        /// <param name="Tooltip"></param>
        /// <param name="Mandatory"></param>
        /// <param name="FieldType"></param>
        /// <param name="Date"></param>
        /// <param name="MaxLength"></param>
        /// <param name="Format"></param>
        /// <param name="DefaultValue"></param>
        /// <param name="Precision"></param>
        /// <param name="fieldid"></param>
        /// <param name="systemAttribute"></param>
        /// <param name="RelationshipType"></param>
        /// <param name="EntityId"></param>
        /// <param name="RelatedTable"></param>
        /// <param name="RelatedEntity"></param>
        /// <param name="allowsearch"></param>
        /// <param name="specific"></param>
        /// <param name="hyperlinktext"></param>
        /// <param name="hyperlinkpath"></param>
        /// <param name="groupid"></param>
        /// <param name="DelegateId"></param>
        /// <param name="AllowEmployeeToPopulate"></param>
        public UserDefinedFields(int createdby, string modifiedBy, string DisplayName, string Description, string Tooltip, bool Mandatory, FieldType FieldType, DateTime Date, int MaxLength,
               Format Format, string DefaultValue, short? Precision, Guid fieldid, bool systemAttribute, short? RelationshipType, int EntityId, Guid RelatedTable, CustomEntity RelatedEntity, bool allowsearch,
               bool specific, string hyperlinktext, string hyperlinkpath, int groupid, int? DelegateId = null, bool AllowEmployeeToPopulate = false)
            : base(createdby, modifiedBy, DisplayName, Description, Tooltip, Mandatory, FieldType, Date, MaxLength,
                Format, DefaultValue, Precision, fieldid, systemAttribute, DelegateId)
        {
            AllowSearch = allowsearch;
            Specific = specific;
            HyperLinkPath = HyperLinkPath;
            HyperLinkText = hyperlinktext;
            GroupID = groupid;
            allowEmployeeToPopulate = AllowEmployeeToPopulate;
        }
    }

    /// <summary>
    /// User defined Field for list items
    /// </summary>
    public class UserDefinedFieldTypeList : UserDefinedFields
    {
        public List<CustomEntitiesUtilities.EntityListItem> UserDefinedFieldListItems { get; set; }

        /// <summary>
        /// Default constructor - used when reading data from lithium
        /// </summary>
        public UserDefinedFieldTypeList() { }
    }

    public enum AppliesTo
    {
        [Description("Addresses")]
        Addresses = 0,

        [Description("Cars")]
        Cars,

        [Description("Claims")]
        Claims,

        [Description("Cost Codes")]
        CostCodes,

        [Description("Departments")]
        Departments,

        [Description("Employees")]
        Employees,

        [Description("Expense Item Categories")]
        ExpenseItemCategories,

        [Description("Expense Items")]
        ExpenseItems,

        [Description("Project Codes")]
        ProjectCodes
    }
}
