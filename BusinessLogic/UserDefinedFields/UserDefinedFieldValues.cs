namespace BusinessLogic.UserDefinedFields
{
    using System.Collections.Generic;

    /// <summary>
    /// For a given table, stores a list of user defined field names.
    /// </summary>
    public class UserDefinedFieldValues : IUserDefinedFieldValues
    {
        /// <summary>
        /// Get's the table name.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Get's a <see cref="List{T}"/> of user defined field names.
        /// </summary>
        public List<string> FieldNames { get; }

        /// <summary>
        /// Create a new instance of <see cref="IUserDefinedFieldValues"/>
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="fieldNames">A <see cref="List{T}"/>of field names.</param>
        public UserDefinedFieldValues(string tableName, List<string> fieldNames)
        {
            this.TableName = tableName;
            this.FieldNames = fieldNames;
        }
    }
}