namespace SQLDataAccess.CustomEntities
{
    using System;
    using System.Data.SqlClient;

    using BusinessLogic.DataConnections;
    using BusinessLogic.Fields;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.Fields.Type.ValueList;

    using Common.Logging;

    /// <summary>
    /// The sql custom entity value field factory.
    /// </summary>
    public class SqlCustomEntityValueFieldFactory : SqlCustomEntityFieldsFactory
    {
        /// <summary>
        /// The _field list values repository.
        /// </summary>
        private readonly FieldListValuesRepository _fieldListValuesRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCustomEntityValueFieldFactory"/> class.
        /// </summary>
        /// <param name="customerDataConnection">The customer data connection</param>
        /// <param name="fieldListValuesRepository">An instance of <see cref="FieldListValuesRepository">FieldListValuesRepository</see></param>
        /// <param name="fieldFactory">An instance of <see cref="FieldFactory">FieldFactory</see></param>
        /// <param name="customEntityFieldListValuesRepository">An instance of <see cref="CustomEntityFieldListValuesRepository">CustomEntityFieldListValuesRepository</see></param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        public SqlCustomEntityValueFieldFactory(ICustomerDataConnection<SqlParameter> customerDataConnection, FieldListValuesRepository fieldListValuesRepository, FieldFactory fieldFactory, CustomEntityFieldListValuesRepository customEntityFieldListValuesRepository, ILog logger)
            : base(customerDataConnection, fieldFactory, customEntityFieldListValuesRepository, logger)
        {
            this._fieldListValuesRepository = fieldListValuesRepository;
        }

        /// <summary>
        /// Gets a Field from the database by its Name
        /// </summary>
        /// <param name="name">
        /// The field name to lookup.
        /// </param>
        /// <returns>
        /// A <see cref="IField">IField</see>.
        /// </returns>
        public override IField this[string name]
        {
            get
            {
                var result = base[name];
                this.UpdateListValues(result);
                return result;
            }
        }

        /// <summary>
        /// Gets a Field from the database by its Name
        /// </summary>
        /// <param name="id">
        /// The field name to lookup.
        /// </param>
        /// <returns>
        /// A <see cref="IField">IField</see>.
        /// </returns>
        public override IField this[Guid id]
        {
            get
            {
                var result = base[id];
                this.UpdateListValues(result);
                return result;
            }
        }

        /// <summary>
        /// Update the list values.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        private void UpdateListValues(IField field)
        {
            var fieldValueList = field as IFieldValueList;
            if (fieldValueList != null)
            { 
                fieldValueList.ListItemValues = this._fieldListValuesRepository.Get(field.Id);
            }
        }
    }
}
