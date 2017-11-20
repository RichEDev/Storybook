namespace BusinessLogic.UserDefinedFields
{
    using System.Collections.Generic;

    /// <summary>
    /// For a given table, stores a list of user defined field names.
    /// </summary>
    public interface IUserDefinedFieldValues
    {
        /// <summary>
        /// Get's the table name.
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// Get's a <see cref="List{T}"/> of user defined field names.
        /// </summary>
        List<string> FieldNames { get; }
    }
}
