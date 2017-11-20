namespace ApiLibrary.DataObjects.Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using ApiLibrary.DataObjects.Base;

    /// <summary>
    /// User defined field.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class UserDefinedField : DataClassBase
    {
        /// <summary>
        /// The user define id.
        /// </summary>
        [DataMember]
        [DataClass]
        public int userdefineid;

        /// <summary>
        /// Gets or sets the field name.
        /// </summary>
        [DataMember]
        public string field;

        /// <summary>
        /// Gets or sets the field id.
        /// </summary>
        [DataMember]
        public Guid fieldid;

        /// <summary>
        /// The table name.
        /// </summary>
        [DataMember]
        public string tablename;

        /// <summary>
        /// Gets or sets the table id.
        /// </summary>
        [DataMember]
        public Guid tableid;

        /// <summary>
        /// Gets or sets the field type.
        /// </summary>
        [DataMember]
        public FieldType fieldtype;

        /// <summary>
        /// The value of this instance of the UDF.
        /// </summary>
        [DataMember]
        public object Value;

        /// <summary>
        /// The record id.
        /// </summary>
        [DataMember]
        [DataClass(IsKeyField = true)]
        public int recordId;

        /// <summary>
        /// Gets or sets the display field.
        /// </summary>
        [DataMember]
        public Guid DisplayField;

        /// <summary>
        /// Gets or sets the related table.
        /// </summary>
        [DataMember]
        public Guid RelatedTable;

        /// <summary>
        /// The UDF data type.
        /// </summary>
        public enum FieldType
        {
            /// <summary>
            /// The not set.
            /// </summary>
            NotSet = 0,

            /// <summary>
            /// The text.
            /// </summary>
            Text = 1,

            /// <summary>
            /// The integer.
            /// </summary>
            Integer = 2,

            /// <summary>
            /// The date time.
            /// </summary>
            DateTime = 3,

            /// <summary>
            /// The list.
            /// </summary>
            List = 4,

            /// <summary>
            /// The tick box.
            /// </summary>
            TickBox = 5,

            /// <summary>
            /// The currency.
            /// </summary>
            Currency = 6,

            /// <summary>
            /// The number.
            /// </summary>
            Number = 7,

            /// <summary>
            /// The hyperlink.
            /// </summary>
            Hyperlink = 8,

            /// <summary>
            /// The relationship.
            /// </summary>
            Relationship = 9,

            /// <summary>
            /// The large text.
            /// </summary>
            LargeText = 10,

            /// <summary>
            /// The run workflow.
            /// </summary>
            RunWorkflow = 11,

            /// <summary>
            /// The relationship textbox.
            /// </summary>
            RelationshipTextbox = 12,

            /// <summary>
            /// The auto complete textbox.
            /// </summary>
            AutoCompleteTextbox = 13,

            /// <summary>
            /// The otm summary.
            /// </summary>
            OtmSummary = 15,

            /// <summary>
            /// The dynamic hyperlink.
            /// </summary>
            DynamicHyperlink = 16,

            /// <summary>
            /// The currency list.
            /// </summary>
            CurrencyList = 17,

            /// <summary>
            /// The comment.
            /// </summary>
            Comment = 19,

            /// <summary>
            /// The spacer.
            /// </summary>
            Spacer = 20,

            /// <summary>
            /// A readonly display field that shows data from a related Many to One
            /// </summary>
            LookupDisplayField = 21
        }

        /// <summary>
        /// Gets or sets the user defined match fields.
        /// </summary>
        [DataMember]
        public List<UserDefinedMatchField> UserDefinedMatchFields { get; set; }
    }
}
