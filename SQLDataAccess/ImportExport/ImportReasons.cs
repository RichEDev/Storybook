namespace SQLDataAccess.ImportExport
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    using BusinessLogic;
    using BusinessLogic.Constants.Tables;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Fields;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.ImportExport;
    using BusinessLogic.Reasons;
    using BusinessLogic.Tables;
    using BusinessLogic.Tables.Type;

    using QueryBuilder.Builders;
    using QueryBuilder.Common;
    using QueryBuilder.Comparisons;

    /// <summary>
    /// Import a file into expenses.
    /// </summary>
    public class ImportReasons : IImportFile
    {
        private readonly FieldRepository _fieldRepository;

        private readonly TableRepository _tableRepository;

        private readonly ICustomerDataConnection<SqlParameter> _customerDataConnection;

        private readonly IDataFactoryArchivable<IReason, int, int> _reasonsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportReasons"/> class.
        /// </summary>
        /// <param name="fieldRepository">
        /// An instance of <see cref="FieldRepository"/>.
        /// </param>
        /// <param name="tableRepository">
        /// An instance of <see cref="TableRepository"/>.
        /// </param>
        /// <param name="customerDataConnection">
        /// The <see cref="ICustomerDataConnection{T}"/> to use.
        /// </param>
        /// <param name="reasonsFactory">
        /// An instance of <see cref="IDataFactoryArchivable{TComplexType,TPrimaryKeyDataType,TReturnType}"/> where TComplexType is <seealso cref="IReason"/>.
        /// </param>
        public ImportReasons(FieldRepository fieldRepository, TableRepository tableRepository, ICustomerDataConnection<SqlParameter> customerDataConnection, IDataFactoryArchivable<IReason, int, int> reasonsFactory)
        {
            Guard.ThrowIfNull(fieldRepository, nameof(fieldRepository));
            Guard.ThrowIfNull(tableRepository, nameof(tableRepository));
            Guard.ThrowIfNull(customerDataConnection, nameof(customerDataConnection));
            Guard.ThrowIfNull(reasonsFactory, nameof(reasonsFactory));

            this._fieldRepository = fieldRepository;
            this._tableRepository = tableRepository;
            this._customerDataConnection = customerDataConnection;
            this._reasonsFactory = reasonsFactory;
        }

        /// <summary>
        /// Import a given file into expenses.
        /// </summary>
        /// <param name="defaultValues">
        /// The default values for any non-matched columns.
        /// </param>
        /// <param name="matchedColumns">
        /// The import file columns matched to <see cref="Guid"/> file ID's within expenses.
        /// </param>
        /// <param name="fileContents">
        /// The file content.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/> of import stats results (one per row).
        /// </returns>
        public List<string> Import(
            SortedList<Guid, string> defaultValues,
            List<ImportField> matchedColumns,
            List<List<object>> fileContents)
        {
            Guard.ThrowIfNull(defaultValues, "defaultValues");
            Guard.ThrowIfNull(matchedColumns, "matchedColumns");
            Guard.ThrowIfNull(fileContents, "fileContents");
            ITable baseTable = this._tableRepository[Reasons.TableId];
            IField keyfield = this._fieldRepository[baseTable.KeyFieldId];
            IField primaryKeyField = this._fieldRepository[baseTable.PrimaryKeyId];
            int keyindex = this.GetKeyFieldIndex(keyfield, matchedColumns);
            var status = new List<string>();

            if (keyindex != -1)
            {
                var existingValues = this.GetExistingValues(fileContents, baseTable, primaryKeyField, keyfield, keyindex);

                for (int i = 0; i < fileContents.Count; i++)
                {
                    var existingRecord =
                        existingValues.FirstOrDefault(t => t.Item2 == fileContents[i][keyindex].ToString());

                    if (existingRecord != null) 
                    {
                        // record exists so update
                        var reason = this._reasonsFactory[existingRecord.Item1];

                        this.Update(reason, fileContents[i], matchedColumns);
                        this._reasonsFactory.Save(reason);
                    }
                    else
                    {
                        var reason = new Reason();

                        this.Update(reason, fileContents[i], matchedColumns);
                        this._reasonsFactory.Save(reason);
                    }

                    status.Add("Row " + i + " imported successfully.");
                }

                status.Add("File imported successfully.");
            }
            else
            {
                status.Add("File not imported no key field selected.");
            }

            return status;
        }

        /// <summary>
        /// Get existing record ID's from the datastore.
        /// </summary>
        /// <param name="fileContents">
        /// The file contents.
        /// </param>
        /// <param name="baseTable">
        /// An instance of <see cref="ITable"/> which represents the base table of the import (in this case project Codes).
        /// </param>
        /// <param name="primaryKeyField">
        /// An instance of <see cref="IField"/> which represents the primary key of <param name="baseTable"></param>
        /// </param>
        /// <param name="keyfield">
        /// The keyfield.
        /// </param>
        /// <param name="keyindex">
        /// The keyindex.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        private List<Tuple<int, string>> GetExistingValues(
            List<List<object>> fileContents,
            ITable baseTable,
            IField primaryKeyField,
            IField keyfield,
            int keyindex)
        {
            var existQuery = new SelectQueryBuilder();
            existQuery.From.Add(new SqlName(baseTable.Name));
            existQuery.Columns.Add(new Column(primaryKeyField.Name, baseTable.Name));
            existQuery.Columns.Add(new Column(keyfield.Name, baseTable.Name));
            var values = new SqlValueCollection<string>();
            foreach (List<object> fileContent in fileContents)
            {
                values.Add(new SqlValue<string>(fileContent[keyindex].ToString()));
            }

            existQuery.Where.Add(new InComparison<string>(new Column(keyfield.Name, baseTable.Name), values));

            this._customerDataConnection.Parameters.Add(existQuery.Parameters);

            var existingValues = new List<Tuple<int, string>>();

            using (var reader = this._customerDataConnection.GetReader(existQuery.Sql()))
            {
                // At this point we have the id and key field data for any existing project codes.
                while (reader.Read())
                {
                    existingValues.Add(new Tuple<int, string>(reader.GetInt32(0), reader.GetString(1)));
                }
            }

            return existingValues;
        }

        /// <summary>
        /// The get key field index from the import fields.
        /// </summary>
        /// <param name="keyfield">
        /// An instance of <see cref="IField"/> which represents the key field of the import table.
        /// </param>
        /// <param name="matchImportFields">
        /// A <see cref="List{T}"/> of <seealso cref="ImportField"/>.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> which is the index of the <paramref name="keyfield"/> in the <paramref name="matchImportFields"/>.
        /// </returns>
        private int GetKeyFieldIndex(IField keyfield, List<ImportField> matchImportFields)
        {
            foreach (ImportField s in matchImportFields)
            {
                Guid id = s.DestinationColumn;
                if (keyfield.Id == id)
                {
                    return matchImportFields.IndexOf(s);
                }
            }

            return -1;
        }

        /// <summary>
        /// Update an existing <see cref="IReason"/>.
        /// </summary>
        /// <param name="reason">
        /// The instance of <see cref="IReason"/> to update.
        /// </param>
        /// <param name="fileContent">
        /// The content of the file, used to update the field.
        /// </param>
        /// <param name="matchingGrid">
        /// The import columns matched to the <see cref="Guid"/> ID's of <seealso cref="IField"/>.
        /// </param>
        /// <param name="defaultValues">
        /// The default values.
        /// </param>
        private void Update(IReason reason, List<object> fileContent, List<ImportField> matchingGrid)
        {
            var idx = 0;

            foreach (ImportField importField in matchingGrid)
            {
                var importValueMissing = string.IsNullOrEmpty(fileContent[idx].ToString());
                string defaultValue = importValueMissing ? importField.DefaultValue : string.Empty;

                if (importField.DestinationColumn == Reasons.Fields.AccountCodeVat)
                {
                    reason.AccountCodeVat = importValueMissing ? defaultValue : fileContent[idx].ToString();
                }

                if (importField.DestinationColumn == Reasons.Fields.AccountCodeNoVat)
                {
                    reason.AccountCodeNoVat = importValueMissing ? defaultValue : fileContent[idx].ToString();
                }

                if (importField.DestinationColumn == Reasons.Fields.Description)
                {
                    reason.Description = importValueMissing ? defaultValue : fileContent[idx].ToString();
                }

                if (importField.DestinationColumn == Reasons.Fields.Reason)
                {
                    reason.Name = importValueMissing ? defaultValue : fileContent[idx].ToString();
                }

                idx++;
            }
        }
    }
}
