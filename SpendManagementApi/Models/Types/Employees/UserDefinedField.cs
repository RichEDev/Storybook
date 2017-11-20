namespace SpendManagementApi.Models.Types.Employees
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using SpendManagementApi.Attributes.Validation;
    using SpendManagementApi.Common.Enums;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Utilities;

    using SpendManagementLibrary;

    /// <summary>
    /// Represents a User Defined Field. This is a method of extending the system to allow for user properties to be saved along with Expenses objects.
    /// </summary>
    public class UserDefinedField : BaseExternalType, IEquatable<UserDefinedField>
    {
        /// <summary>
        /// The user defined field Id.
        /// </summary>
        public int UserDefinedId { get; set; }

        /// <summary>
        /// The element that the user defined field applies to.
        /// </summary>
        [Required, ValidEnumValue(ErrorMessage = ApiResources.ApiErrorEnum)]
        public UDFAppliesTo AppliesTo { get; set; }

        /// <summary>
        /// The field order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Whether the field is archived.
        /// </summary>
        public bool Archived { get; set; }

        /// <summary>
        /// Display Name.
        /// </summary>
        public string DisplayName {
            get
            {
                return this.Attribute == null ? string.Empty : this.Attribute.displayname;
            }
        }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description {
            get
            {
                return this.Attribute == null ? string.Empty : this.Attribute.description;
            }
        }

        /// <summary>
        /// Tooltip.
        /// </summary>
        public string Tooltip
        {
            get
            {
                return this.Attribute == null ? string.Empty : this.Attribute.tooltip;
            }
        }

        /// <summary>
        /// Whether the field is mandatory.
        /// </summary>
        public bool Mandatory
        {
            get
            {
                return this.Attribute != null && this.Attribute.mandatory;
            }
        }

        /// <summary>
        /// Field Type.
        /// </summary>
        public FieldType FieldType 
        { 
            get
            {
                return this.Attribute == null ? FieldType.NotSet : this.Attribute.fieldtype;
            }
        }

        /// <summary>
        /// The url for the hyperlink.
        /// </summary>
        public string HyperlinkPath
        {
            get
            {
                if (this.Attribute == null)
                {
                    return null;
                }

                if (this.Attribute.fieldtype != FieldType.Hyperlink)
                {
                    return null;
                }

                var hyperlinkAttribute = (cHyperlinkAttribute)this.Attribute;
                return hyperlinkAttribute.hyperlinkPath;
            }
        }

        /// <summary>
        /// The text for the hyperlink.
        /// </summary>
        public string HyperlinkText
        {
            get
            {
                if (this.Attribute == null)
                {
                    return null;
                }

                if (this.Attribute.fieldtype != FieldType.Hyperlink)
                {
                    return null;
                }

                var hyperlinkAttribute = (cHyperlinkAttribute)this.Attribute;
                return hyperlinkAttribute.hyperlinkText;
            }
        }

        /// <summary>
        /// The precision of the number.
        /// </summary>
        public int? Precision
        {
            get
            {
                if (this.Attribute == null)
                {
                    return null;
                }

                if (!(this.Attribute.fieldtype == FieldType.Currency || this.Attribute.fieldtype == FieldType.Number))
                {
                    return null;
                }

                var number = (cNumberAttribute)this.Attribute;
                return number.precision;
            }
        }

        /// <summary>
        /// The maximum length allowed for the text.
        /// </summary>
        public int? MaxLength
        {
            get
            {
                if (this.Attribute == null)
                {
                    return null;
                }

                if (!(this.Attribute.fieldtype == FieldType.LargeText || this.Attribute.fieldtype == FieldType.Text))
                {
                    return null;
                }

                var text = (cTextAttribute)this.Attribute;
                return text.format == AttributeFormat.FormattedText ? null : text.maxlength;
            }
        }

        /// <summary>
        /// The maximum length allowed for the text.
        /// </summary>
        public string Default
        {
            get
            {
                if (this.Attribute == null)
                {
                    return null;
                }

                if (this.Attribute.fieldtype != FieldType.TickBox)
                {
                    return null;
                }

                var yesNo = (cTickboxAttribute)this.Attribute;
                return yesNo.defaultvalue;
            }
        }

        /// <summary>
        /// List of elements if FieldType = List.
        /// </summary>
        public List<UserDefinedFieldListElement> ListElements { get; set; }

        /// <summary>
        /// The associated attribute.
        /// </summary>
        internal cAttribute Attribute { get; set; }

        /// <summary>
        /// The subcategories.
        /// </summary>
        internal List<int> Subcatids { get; set; }

        /// <summary>
        /// Whether Specific is set.
        /// </summary>
        internal bool Specific { get; set; }

        /// <summary>
        /// The grouping.
        /// </summary>
        internal cUserdefinedFieldGrouping Grouping { get; set; }

        /// <summary>
        /// Whether search is allowed.
        /// </summary>
        internal bool AllowSearch { get; set; }

        /// <summary>
        /// Whether the employee is allowed to populate.
        /// </summary>
        internal bool AllowEmployeeToPopulate { get; set; }

        /// <summary>
        /// Overrides Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns>Returns a boolean indicating whether items are equal</returns>
        public bool Equals(UserDefinedField other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as UserDefinedField);
        }
    }

    internal static class UserDefinedFieldConversion
    {
        internal static TResult Cast<TResult>(this cUserDefinedField userDefinedField, IActionContext actionContext)
            where TResult : UserDefinedField, new()
        {
            var result = new TResult{
                AllowEmployeeToPopulate = userDefinedField.AllowEmployeeToPopulate,
                AppliesTo = (UDFAppliesTo)Enum.Parse(typeof(UDFAppliesTo), actionContext.Tables.GetTableByUserdefineTableID(userDefinedField.table.TableID).Description.Replace(" ", string.Empty)),           
                AllowSearch = userDefinedField.AllowSearch,
                Archived = userDefinedField.Archived,
                CreatedById = userDefinedField.createdby,
                CreatedOn = userDefinedField.createdon,
                ModifiedById = userDefinedField.modifiedby,
                ModifiedOn = userDefinedField.modifiedon,
                Order = userDefinedField.order,
                Specific = userDefinedField.Specific,
                UserDefinedId = userDefinedField.userdefineid,
                Grouping = userDefinedField.Grouping,
                Subcatids = userDefinedField.selectedSubcats,
                Attribute = userDefinedField.attribute
            };
            if (userDefinedField.attribute is cListAttribute)
            {
                result.ListElements =
                    (userDefinedField.attribute as cListAttribute).items.Values.Select(
                        item => item.Cast<UserDefinedFieldListElement>()).ToList();
            }
            return result;
        }

        internal static cUserDefinedField Cast(
            this UserDefinedField userDefinedField, cTable table)
        {
            return new cUserDefinedField(
                userDefinedField.UserDefinedId, 
                table,
                userDefinedField.Order,
                userDefinedField.Subcatids,
                userDefinedField.CreatedOn,
                userDefinedField.CreatedById,
                userDefinedField.ModifiedOn,
                userDefinedField.ModifiedById,
                userDefinedField.Attribute,
                userDefinedField.Grouping,
                userDefinedField.Archived,
                userDefinedField.Specific,
                userDefinedField.AllowSearch,
                userDefinedField.AllowEmployeeToPopulate);
        }
    }
}