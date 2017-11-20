namespace BusinessLogic.Fields
{
    using System;
    using BusinessLogic.Fields.Type;
    using BusinessLogic.Fields.Type.Attributes;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.Fields.Type.ValueList;

    /// <summary>
    /// Create instances of <see cref="Field"/> based on the "type" <see cref="string"/> given.
    /// </summary>
    public class FieldFactory : IFieldFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldFactory"/> class. 
        /// </summary>
        public FieldFactory()
        {
            this.FieldListValuesRepository = null;
        }

        /// <summary>
        /// Gets or sets the <see cref="IFieldListValuesRepository"/> for this instance of <see cref="IFieldFactory"/>
        /// </summary>
        public IFieldListValuesRepository FieldListValuesRepository { get; set; }

        /// <summary>
        /// Create a new <see cref="IField"/> based on the parameters supplied.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IField"/> to return</typeparam>
        /// <param name="fieldType">The field type string.</param>
        /// <param name="id">The <see cref="Guid"/> of this Field</param>
        /// <param name="name">The name of the Field</param>
        /// <param name="description">The description of this Field</param>
        /// <param name="comment">The comment of the Field.</param>
        /// <param name="tableId">The <see cref="Guid"/> of the table this field is a part of.</param>
        /// <param name="classPropertyName">The class property name for this field</param>
        /// <param name="fieldAttributes">The <see cref="FieldAttributes"/> for this field.</param>
        /// <param name="viewGroupId">The <see cref="Guid"/> used for the </param>
        /// <param name="width">The width of the field</param>
        /// <param name="length">The length of the field</param>
        /// <param name="valueList">True if this Field is a Value List.</param>
        /// <returns>A new instance of <see cref="IField"/></returns>
        public T New<T>(string fieldType, Guid id, string name, string description, string comment, Guid tableId, string classPropertyName, FieldAttributes fieldAttributes, Guid viewGroupId, int width, int length, bool valueList) where T : Field
        {
            Field decorateField = default(T);
            Field field = new Field(id, name, description, comment, tableId, classPropertyName, fieldAttributes, viewGroupId, width, length);

            switch (fieldType)
            {
                case "A":
                    decorateField = new AmountField(new NumericField(field));
                    break;
                case "C":
                    decorateField = new CurrencyField(new NumericField(field));
                    break;
                case "FC":
                    decorateField = new FunctionCurrency(new CurrencyField(new NumericField(field)));
                    break;
                case "M":
                    decorateField = new MoneyField(new NumericField(field));
                    break;
                case "D":
                    decorateField = new DateField(new DateTimeField(field));
                    break;
                case "DT":
                    decorateField = new DateTimeField(new DateTimeField(field));
                    break;
                case "T":
                    decorateField = new TimeField(new DateTimeField(new DateTimeField(field)));
                    break;
                case "F":
                    decorateField = new FloatField(new DecimalField(field));
                    break;
                case "FD":
                    decorateField = new FunctionDecimalField(new DecimalField(field));
                    break;
                case "FI":
                    decorateField = new FunctionIntegerField(new IntegerField(field));
                    break;
                case "I":
                    decorateField = new IntegerField(field);
                    break;
                case "N":
                    decorateField = new NumberField(new IntegerField(field));
                    break;
                case "S":
                    decorateField = new StringField(field);
                    break;
                case "FS":
                    decorateField = new FunctionStringField(new StringField(field));
                    break;
                case "LT":
                    decorateField = new LargeText(new StringField(field));
                    break;
                case "FU":
                    decorateField = new FunctionUnique(new GuidField(field));
                    break;
                case "G":
                    decorateField = new GuidField(field);
                    break;
                case "U":
                    decorateField = new UniqueField(new GuidField(field));
                    break;
                case "X":
                case "Y":
                    decorateField = new BooleanField(field);
                    break;
                case "B":
                    decorateField = new LongField(field);
                    break;
                case "VB":
                    decorateField = new VarBinaryField(field);
                    break;
            }

            return (T)this.CreateValueListField(fieldType, decorateField, valueList);
        }

        /// <summary>
        /// Create a new <see cref="IField"/> based on the parameters supplied.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IField"/> to return</typeparam>
        /// <param name="id">The <see cref="Guid"/> of this Field</param>
        /// <param name="name">The name of the Field</param>
        /// <param name="description">The description of this Field</param>
        /// <param name="comment">The comment of the Field.</param>
        /// <param name="tableId">The <see cref="Guid"/> of the table this field is a part of.</param>
        /// <param name="classPropertyName">The class property name for this field</param>
        /// <param name="fieldAttributes">The <see cref="FieldAttributes"/> for this field.</param>
        /// <param name="viewGroupId">The <see cref="Guid"/> used for the </param>
        /// <param name="width">The width of the field</param>
        /// <param name="length">The length of the field</param>
        /// <returns>A new instance of <see cref="IField"/></returns>
        public T New<T>(Guid id, string name, string description, string comment, Guid tableId, string classPropertyName, FieldAttributes fieldAttributes, Guid viewGroupId, int width, int length) where T : Field, new()
        {
            var result = new T
            {
                Id = id,
                Description = description,
                Comment = comment,
                TableId = tableId,
                ClassPropertyName = classPropertyName,
                FieldAttributes = fieldAttributes,
                ViewGroupId = viewGroupId,
                Width = width,
                Length = length
            };
            return result;
        }

        /// <summary>
        /// Create a new <see cref="Field"/>
        /// </summary>
        /// <typeparam name="T">The new <see cref="Field"/>Type to convert to.</typeparam>
        /// <param name="field">The current <see cref="Field"/></param>
        /// <returns>A new field of type <see cref="T"/></returns>
        public T New<T>(Field field) where T : Field, new()
        {
            var result = new T
            {
                Id = field.Id,
                Description = field.Description,
                Comment = field.Comment,
                TableId = field.TableId,
                ClassPropertyName = field.ClassPropertyName,
                FieldAttributes = field.FieldAttributes,
                ViewGroupId = field.ViewGroupId,
                Width = field.Width,
                Length = field.Length
            };
            return result;
        }

        /// <summary>
        /// Return a new <see cref="FieldAttributes" />object based on the parameters given.
        /// </summary>
        /// <param name="normalView">True to include <see cref="NormalViewAttribute"/></param>
        /// <param name="identifierField">True to include a <see cref="IdFieldAttribute"/></param>
        /// <param name="genList">True to include a <see cref="GenListAttribute"/></param>
        /// <param name="canTotal">True to include a <see cref="CanTotalAttribute"/></param>
        /// <param name="printOut">True to include a <see cref="PrintOutAttribute"/></param>
        /// <param name="useForLookup">True to include a <see cref="UseForLookupAttribute"/></param>
        /// <param name="lookUpFieldId">The FieldID <see cref="Guid"/>Used for the <see cref="UseForLookupAttribute"/></param>
        /// <param name="lookupTableId">The Table ID <see cref="Guid"/>Used for the <see cref="UseForLookupAttribute"/></param>
        /// <param name="allowImport">True to include a <see cref="AllowImportAttribute"/></param>
        /// <param name="foreignKey">True to include a <see cref="ForeignKeyAttribute"/></param>
        /// <param name="relatedTableId">The Table ID used for the <see cref="ForeignKeyAttribute"/></param>
        /// <param name="relabel">True to include a <see cref="RelabelAttribute"/></param>
        /// <param name="relabelParam">The Label to use for the <see cref="RelabelAttribute"/></param>
        /// <param name="workflowSearchable">True to include a <see cref="WorkflowSearchAttribute"/></param>
        /// <param name="workflowUpdate">True to include a <see cref="WorkflowUpdateAttribute"/></param>
        /// <param name="mandatory">True to include a <see cref="MandatoryAttribute"/></param>
        /// <param name="fieldSource">The source <see cref="FieldSource"/> of the Field.</param>
        /// <returns>A new instance of <see cref="FieldAttributes"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="fieldSource"/> is not catered for in this method.</exception>
        public FieldAttributes PopulateFieldAttributes(bool normalView, bool identifierField, bool genList, bool canTotal, bool printOut, bool useForLookup, Guid lookUpFieldId, Guid lookupTableId, bool allowImport, bool foreignKey, Guid relatedTableId, bool relabel, string relabelParam, bool workflowSearchable, bool workflowUpdate, bool mandatory, FieldSource fieldSource)
        {
            var fieldAttributes = new FieldAttributes();
            if (normalView)
            {
                fieldAttributes.Add(new NormalViewAttribute());
            }

            if (identifierField)
            {
                fieldAttributes.Add(new IdFieldAttribute());
            }

            if (genList)
            {
                fieldAttributes.Add(new GenListAttribute());
            }

            if (canTotal)
            {
                fieldAttributes.Add(new CanTotalAttribute());
            }

            if (printOut)
            {
                fieldAttributes.Add(new PrintOutAttribute());
            }

            if (useForLookup)
            {
                fieldAttributes.Add(new UseForLookupAttribute(lookUpFieldId, lookupTableId));
            }

            if (allowImport)
            {
                fieldAttributes.Add(new AllowImportAttribute());
            }

            if (foreignKey)
            {
                fieldAttributes.Add(new ForeignKeyAttribute(relatedTableId));
            }

            if (relabel)
            {
                fieldAttributes.Add(new RelabelAttribute(relabelParam));
            }

            if (workflowSearchable)
            {
                fieldAttributes.Add(new WorkflowSearchAttribute());
            }

            if (workflowUpdate)
            {
                fieldAttributes.Add(new WorkflowUpdateAttribute());
            }

            if (mandatory)
            {
                fieldAttributes.Add(new MandatoryAttribute());
            }

            switch (fieldSource)
            {
                case FieldSource.Field:
                    break;
                case FieldSource.Custom:
                    fieldAttributes.Add(new CustomField());
                    break;
                case FieldSource.CustomEntity:
                    fieldAttributes.Add(new CustomEntityField());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fieldSource), fieldSource, null);
            }

            return fieldAttributes;
        }

        /// <summary>
        /// Create a Value List if required..
        /// </summary>
        /// <param name="fieldType">
        /// The field type.
        /// </param>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="valueList">
        /// True if the field is a  value list.
        /// </param>
        /// <returns>
        /// The <see cref="Field"/>.
        /// Either the original field or a <see cref="IFieldValueList"/>
        /// </returns>
        private Field CreateValueListField(string fieldType, Field field, bool valueList)
        {
            if (valueList && field != null)
            {
                var listValues = this.FieldListValuesRepository.Get(field.Id);
                switch (fieldType)
                {
                    case "X":
                    case "Y":
                        field = new BooleanFieldValueList(field, listValues);
                        break;
                    case "U":
                    case "G":
                    case "FU":
                        field = new GuidFieldValueList(field, listValues);
                        break;
                    case "I":
                        field = new IntegerFieldValueList(field, listValues);
                        break;
                    case "S":
                        field = new StringFieldValueList(field, listValues);
                        break;
                }
            }

            return field;
        }
    }
}
