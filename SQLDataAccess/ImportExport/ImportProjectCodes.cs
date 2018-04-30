namespace SQLDataAccess.ImportExport
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Fields;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.ImportExport;
    using BusinessLogic.ProjectCodes;
    using BusinessLogic.Tables;
    using BusinessLogic.Tables.Type;
    using BusinessLogic.UserDefinedFields;

    using QueryBuilder.Common;
    using QueryBuilder.Comparisons;

    /// <summary>
    /// Import a file into expenses.
    /// </summary>
    /// <typeparam name="IProjectCodesWithUserDefined"> The target file type to inport into.
    /// </typeparam>
    public class ImportProjectCodes<IProjectCodesWithUserDefined> : IImportFile<IProjectCodesWithUserDefined>
    {
        private readonly FieldRepository _fieldRepository;

        private readonly UserDefinedFieldRepository _userDefinedFieldRepository;

        private readonly TableRepository _tableRepository;

        private readonly ICustomerDataConnection<SqlParameter> _customerDataConnection;

        private readonly IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int> _projectCodesRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportProjectCodes{IProjectCodesWithUserDefined}"/> class.
        /// </summary>
        /// <param name="fieldRepository">
        /// An instance of <see cref="FieldRepository"/>.
        /// </param>
        /// <param name="userDefinedFieldRepository">
        /// An instance of <see cref="UserDefinedFieldRepository"/>.
        /// </param>
        /// <param name="tableRepository">
        /// An instance of <see cref="TableRepository"/>.
        /// </param>
        /// <param name="customerDataConnection">
        /// The <see cref="ICustomerDataConnection{T}"/> to use.
        /// </param>
        /// <param name="projectCodesRepository">
        /// An instance of <see cref="IDataFactoryCustom{TComplexType,TPrimaryKeyDataType}"/> where TComplexType is <seealso cref="IProjectCodesWithUserDefined"/>.
        /// </param>
        public ImportProjectCodes(FieldRepository fieldRepository, BusinessLogic.UserDefinedFields.UserDefinedFieldRepository userDefinedFieldRepository, TableRepository tableRepository, ICustomerDataConnection<SqlParameter> customerDataConnection, IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int> projectCodesRepository)
        {
            Guard.ThrowIfNull(fieldRepository, "fieldRepository");
            Guard.ThrowIfNull(userDefinedFieldRepository, "userDefinedFieldRepository");
            Guard.ThrowIfNull(tableRepository, "tableRepository");
            Guard.ThrowIfNull(customerDataConnection, "customerDataConnection");
            Guard.ThrowIfNull(projectCodesRepository, "projectCodesRepository");
            this._fieldRepository = fieldRepository;
            this._userDefinedFieldRepository = userDefinedFieldRepository;
            this._tableRepository = tableRepository;
            this._customerDataConnection = customerDataConnection;
            this._projectCodesRepository = projectCodesRepository;
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
        /// The <see cref="List{T}"/> of imort stats results (one per row).
        /// </returns>
        public List<string> Import(
            SortedList<Guid, string> defaultValues,
            List<ImportField> matchedColumns,
            List<List<object>> fileContents)
        {
            Guard.ThrowIfNull(defaultValues, "defaultValues");
            Guard.ThrowIfNull(matchedColumns, "matchedColumns");
            Guard.ThrowIfNull(fileContents, "fileContents");
            ITable baseTable = this._tableRepository[new Guid("e1ef483c-7870-42ce-be54-ecc5c1d5fb34")];
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
                        var existingProjectCode = this._projectCodesRepository[existingRecord.Item1];

                        this.Update(existingProjectCode, fileContents[i], matchedColumns, defaultValues);
                        this._projectCodesRepository.Save(existingProjectCode);
                    }
                    else 
                    {
                        var existingProjectCode = new ProjectCodeWithUserDefinedFields();

                        this.Update(existingProjectCode, fileContents[i], matchedColumns, defaultValues);
                        this._projectCodesRepository.Save(existingProjectCode);
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
            var existQuery = new QueryBuilder.Builders.SelectQueryBuilder();
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
        /// Update an existing <see cref="IProjectCodesWithUserDefined"/>.
        /// </summary>
        /// <param name="existingProjectCode">
        /// The instance of <see cref="IProjectCodesWithUserDefined"/> to update.
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
        private void Update(IProjectCodeWithUserDefinedFields existingProjectCode, List<object> fileContent, List<ImportField> matchingGrid, SortedList<Guid, string> defaultValues)
        {
            var idx = 0;
            foreach (ImportField importField in matchingGrid)
            {
                var importValueMissing = string.IsNullOrEmpty(fileContent[idx].ToString());
                string defaultValue = importValueMissing ? importField.DefaultValue : string.Empty;

                if (importField.DestinationColumn == BusinessLogic.Constants.Tables.ProjectCodes.Fields.Archived)
                {
                    if (importValueMissing)
                    {
                        existingProjectCode.Archived = defaultValue.ToLower() == "true";
                    }
                    else
                    {
                        existingProjectCode.Archived = (bool)fileContent[idx];    
                    }
                }

                if (importField.DestinationColumn == BusinessLogic.Constants.Tables.ProjectCodes.Fields.Description)
                {
                    existingProjectCode.Description = importValueMissing ? defaultValue : fileContent[idx].ToString();
                }

                if (importField.DestinationColumn == BusinessLogic.Constants.Tables.ProjectCodes.Fields.Projectcode)
                {
                    existingProjectCode.Name = importValueMissing ? defaultValue : fileContent[idx].ToString();
                }

                if (!BusinessLogic.Constants.Tables.ProjectCodes.Fields.Contains(importField.DestinationColumn))
                {
                    // It's a user defined field
                    var field = this._userDefinedFieldRepository[importField.DestinationColumn];
                    UserdefinedAttribute userdefinedAttribute = field.FieldAttributes.Get(typeof(UserdefinedAttribute)) as UserdefinedAttribute;
                    if (userdefinedAttribute != null)
                    {
                        object value;
                        if (field is DateTimeField)
                        {
                            value = importValueMissing ? DateTime.Parse(defaultValue) : DateTime.Parse(fileContent[idx].ToString());
                        }
                        else
                        {
                            value = importValueMissing ? defaultValue : fileContent[idx];
                        }

                        if (existingProjectCode.UserDefinedFieldValues.Keys.Contains(userdefinedAttribute.UserDefinedFieldId))
                        {
                            var udfValues = existingProjectCode.UserDefinedFieldValues.ToSortedList();
                            udfValues[userdefinedAttribute.UserDefinedFieldId] = value;
                                
                            existingProjectCode.UserDefinedFieldValues = new UserDefinedFieldValueCollection(udfValues);
                        }
                        else
                        {
                            var udfValues = existingProjectCode.UserDefinedFieldValues.ToSortedList();
                            udfValues.Add(userdefinedAttribute.UserDefinedFieldId, value);
                            existingProjectCode.UserDefinedFieldValues = new UserDefinedFieldValueCollection(udfValues);
                        }    
                    }
                }

                idx++;
            }
        }
    }
}
