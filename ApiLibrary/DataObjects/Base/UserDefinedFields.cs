namespace ApiLibrary.DataObjects.Base
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using ApiLibrary.DataObjects.Spend_Management;

    /// <summary>
    /// The user defined fields.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class UserDefinedFields : DataClassBase
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="UserDefinedFields"/> class.
        /// </summary>
        /// <param name="userDefinedTable">
        /// The user Defined Table.
        /// </param>
        /// <param name="userDefinedTableName">
        /// The user Defined Table Name.
        /// </param>
        public UserDefinedFields(Guid userDefinedTable, string userDefinedTableName)
        {
            this.Fields = new List<UserDefinedField>();
            this.UserDefinedTableId = userDefinedTable;
            this.UserDefinedTable = userDefinedTableName;
        }

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        [DataMember(IsRequired = true)]
        public List<UserDefinedField> Fields { get; set; }

        /// <summary>
        /// Gets or sets the user defined table name.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string UserDefinedTable { get; set; }

        /// <summary>
        /// Gets or sets the user defined table id.
        /// </summary>
        [DataMember(IsRequired = true)]
        public Guid UserDefinedTableId { get; set; }

        /// <summary>
        /// Gets or sets the record id.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int RecordId { get; set; }
    }
}