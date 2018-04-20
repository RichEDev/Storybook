namespace SQLDataAccess.UserDefinedFieldValues
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.Tables;
    using BusinessLogic.Tables.Type;
    using BusinessLogic.UserDefinedFields;

    using QueryBuilder.Builders;
    using QueryBuilder.Common;
    using QueryBuilder.Comparisons;
    using QueryBuilder.Operators.Comparison;

    using SQLDataAccess.Helpers;

    /// <summary>
    /// The sql user defined field values factory.
    /// </summary>
    public class SqlUserDefinedFieldValuesFactory : UserDefinedFieldValueRepository
    {
        /// <summary>
        /// The _customer data connection.
        /// </summary>
        private readonly ICustomerDataConnection<SqlParameter> _customerDataConnection;

        /// <summary>
        /// The _table repository.
        /// </summary>
        private readonly TableRepository _tableRepository;
        
        /// <summary>
        /// The _user defined field repository.
        /// </summary>
        private readonly UserDefinedFieldRepository _userDefinedFieldRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlUserDefinedFieldValuesFactory"/> class. 
        /// </summary>
        /// <param name="customerDataConnection">An instance of the <see cref="ICustomerDataConnection{SqlParameter}"/></param>
        /// <param name="userDefinedFieldRepository">An instance of the <see cref="UserDefinedFieldRepository"/></param>
        /// <param name="tableRepository">An instance of the <see cref="TableRepository"/></param>
        public SqlUserDefinedFieldValuesFactory(ICustomerDataConnection<SqlParameter> customerDataConnection, UserDefinedFieldRepository userDefinedFieldRepository, TableRepository tableRepository)
        {
            this._customerDataConnection = customerDataConnection;
            this._userDefinedFieldRepository = userDefinedFieldRepository;
            this._tableRepository = tableRepository;
        }

        /// <summary>
        ///     Save the given values to the database
        /// </summary>
        /// <param name="id">The record identifier</param>
        /// <param name="userDefinedFieldValues">The <see cref="UserDefinedFieldValueCollection" /> values to be saved</param>
        /// <param name="moduleElement">The module to save this for</param>
        public override void Save(int id, UserDefinedFieldValueCollection userDefinedFieldValues, ModuleElements moduleElement)
        {
            ITable table = null;
            var encryptedColumn = false;
            string primaryKeyColumnName = string.Empty;

            // TODO -- find a way to ditch this if statement
            // TODO -- DO NOT add extra else if's, refactor first.
            if (moduleElement == ModuleElements.ProjectCodes)
            {
                table = this._tableRepository["userdefinedProjectcodes"];
                primaryKeyColumnName = "projectcodeid";
            }

            if (table == null)
            {
                throw new ArgumentOutOfRangeException(nameof(moduleElement), $"Cannot find matching table for {nameof(moduleElement)} with a value of {moduleElement}");
            }

            SqlName baseTable = new SqlName(table.Name);
            UpdateQueryBuilder updateQuery = new UpdateQueryBuilder {Table = baseTable};
            InsertQueryBuilder insertQuery = new InsertQueryBuilder(baseTable);

            var userDefinedFields = this._userDefinedFieldRepository[table];

            if (userDefinedFields.Count > 0)
            {
                updateQuery.Where.Add(new ValueComparison<EqualTo, int>(new Column(primaryKeyColumnName), new SqlValue<int>(id)));

                foreach (IField userDefinedField in userDefinedFields)
                {
                    int fieldId;
                    if (int.TryParse(userDefinedField.Name.Replace("udf", string.Empty), out fieldId))
                    {
                        UserdefinedAttribute userdefinedAttribute = (UserdefinedAttribute)userDefinedField.FieldAttributes.Get(typeof(UserdefinedAttribute));

                        // Ensure that hyperlink udf's are not saved when form is submitted by a user
                        if (userdefinedAttribute.IsHyperLink == false)
                        {
                            object value = userDefinedFieldValues[fieldId];
                            if (userDefinedField.Encrypted)
                            {
                                updateQuery.ColumnValues.Add(new EncryptedColumnValue(new ColumnValue<object>(new Column(userDefinedField.Name), new SqlValue<object>(value))));
                                insertQuery.ColumnValues.Add(new EncryptedColumnValue(new ColumnValue<object>(new Column(userDefinedField.Name), new SqlValue<object>(value))));
                                encryptedColumn = true;
                            }
                            else
                            {
                                updateQuery.ColumnValues.Add(new ColumnValue<object>(new Column(userDefinedField.Name), new SqlValue<object>(value)));    
                                insertQuery.ColumnValues.Add(new ColumnValue<object>(new Column(userDefinedField.Name), new SqlValue<object>(value)));    
                            }
                            
                        }
                    }
                }

                if (updateQuery.ColumnValues.Count > 0)
                {
                    string sql = updateQuery.Sql();

                    this._customerDataConnection.Parameters.Add(updateQuery.Parameters);
                    if (encryptedColumn)
                    {
                        this._customerDataConnection.Parameters.Add(new SqlParameter("@salt", SqlDbType.NVarChar));
                        this._customerDataConnection.Parameters["@salt"].Value = "2FD583C9-BF7E-4B4E-B6E6-5FC9375AD069";
                    }

                    // Update existing rows matching the primary key
                    bool hasUpdated = this._customerDataConnection.ExecuteNonQuery(sql) > 0;

                    if (hasUpdated == false)
                    {
                        insertQuery.ColumnValues.Add(new ColumnValue<int>(new Column(primaryKeyColumnName), new SqlValue<int>(id)));

                        // Clear all parameters from the update query
                        this._customerDataConnection.Parameters.Clear();

                        sql = insertQuery.Sql();
                        this._customerDataConnection.Parameters.Add(insertQuery.Parameters);
                        if (encryptedColumn)
                        {
                            this._customerDataConnection.Parameters.Add(new SqlParameter("@salt", SqlDbType.NVarChar));
                            this._customerDataConnection.Parameters["@salt"].Value = "2FD583C9-BF7E-4B4E-B6E6-5FC9375AD069";
                        }

                        this._customerDataConnection.ExecuteNonQuery(sql);
                    }
                }

                // Clear parameters
                this._customerDataConnection.Parameters.Clear();
            }
        }

        /// <summary>
        ///     Based on the element, get the column names of the UDF's associated with that type of element.
        /// </summary>
        /// <param name="moduleElement">The type of parent for the UDF</param>
        /// <returns>A list of column names for the associated user defined thing</returns>
        protected override IUserDefinedFieldValues Get(ModuleElements moduleElement)
        {
            string tableName = string.Empty;
            var tableid = this.GetTableID(moduleElement);

            var fields = new List<string>();

            this._customerDataConnection.Parameters.Clear();
            this._customerDataConnection.Parameters.Add(new SqlParameter("@tableId", SqlDbType.UniqueIdentifier) { Value = tableid });

            using (
                var reader =
                    this._customerDataConnection.GetReader(
                        "select 'udf' + CAST(userdefineid AS Nvarchar(20)) as FieldName from userdefined inner join tables on tables.tableid = userdefined.tableid WHERE tables.tableid = @tableid"))
            {
                while (reader.Read())
                {
                    fields.Add(reader.GetString(0));
                }
            }

            return new UserDefinedFieldValues(tableName, fields);
        }

        /// <summary>
        /// Gets the <see cref="ITable"/> <c>Id</c> for the table which stores userdefined field values for <paramref name="moduleElement"/>.
        /// </summary>
        /// <param name="moduleElement">
        /// The module element the userdefined fields relate to.
        /// </param>
        /// <returns>
        /// The the unique identifier <c>Id</c> <see cref="Guid"/> of the <see cref="ITable"/>.
        /// </returns>
        private Guid GetTableID(ModuleElements moduleElement)
        {
            Guid tableid = Guid.Empty;
            if (moduleElement == ModuleElements.ProjectCodes)
            {
                tableid = new Guid("e1ef483c-7870-42ce-be54-ecc5c1d5fb34");
            }

            return tableid;
        }
    }
}
