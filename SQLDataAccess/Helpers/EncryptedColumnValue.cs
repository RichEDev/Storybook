namespace SQLDataAccess.Helpers
{
    using QueryBuilder.Common;

    /// <summary>
    /// Represents a column name and an associated value which is encrypted in the database.
    /// </summary>
    public class EncryptedColumnValue : IColumnValue
    {
        /// <summary>
        /// A private instance of <see cref="IColumnValue"/>.
        /// </summary>
        private readonly IColumnValue _columnValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedColumnValue"/> class.
        /// </summary>
        /// <param name="columnValue">
        /// An instance of <see cref="IColumnValue"/>.
        /// </param>
        public EncryptedColumnValue(IColumnValue columnValue)
        {
            this._columnValue = columnValue;
        }

        /// <summary>
        /// Gets the name of the column
        /// </summary>
        public string ColumnName => this._columnValue.ColumnName;

        /// <summary>
        /// Gets the value associated to the column.
        /// </summary>
        public string Value => $"ENCRYPTBYPASSPHRASE(@salt, {this._columnValue.Value} ) ";

        /// <summary>
        /// Parameterizes an object and populates <paramref name="parameterCollection" /> with a <see cref="T:System.Data.SqlClient.SqlParameter" /> for each value found.
        /// </summary>
        /// <param name="parameterCollection">An instance of <see cref="T:QueryBuilder.Common.ParameterCollection" /> being used for this Sql statement.</param>
        public void Parameterize(ParameterCollection parameterCollection)
        {
            this._columnValue.Parameterize(parameterCollection);
        }
    }
}
