namespace BusinessLogic.Fields
{
    using System;

    using BusinessLogic.Fields.Type.Attributes;
    using BusinessLogic.Fields.Type.Base;

    /// <summary>
    /// Create instances of <see cref="Field"/> based on the "type" <see cref="string"/> given.
    /// </summary>
    public interface IFieldFactory
    {
        /// <summary>
        /// Create a new <see cref="Field"/> based on the parameters supplied.
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
        T New<T>(string fieldType, Guid id, string name, string description, string comment, Guid tableId, string classPropertyName, FieldAttributes fieldAttributes, Guid viewGroupId, int width, int length, bool valueList) where T : Field;

        /// <summary>
        /// Create a new <see cref="Field"/>
        /// </summary>
        /// <typeparam name="T">The new <see cref="Field"/>Type to convert to.</typeparam>
        /// <param name="field">The current <see cref="Field"/></param>
        /// <returns>A new field of type <see cref="T"/></returns>
        T New<T>(Field field) where T : Field, new();

        /// <summary>
        /// Create a new <see cref="Field"/> based on the parameters supplied.
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
        /// <returns>A new instance of <see cref="Field"/></returns>
        T New<T>(Guid id, string name, string description, string comment, Guid tableId, string classPropertyName, FieldAttributes fieldAttributes, Guid viewGroupId, int width, int length) where T : Field, new();

        /// <summary>
        /// Return a new <see cref="FieldAttributes" />object based on the parameters given.
        /// </summary>
        /// <param name="normalView">True to include <see cref="NormalViewAttribute"/></param>
        /// <param name="idField">True to include a <see cref="IdFieldAttribute"/></param>
        /// <param name="genList">True to include a <see cref="GenListAttribute"/></param>
        /// <param name="canTotal">True to include a <see cref="CanTotalAttribute"/></param>
        /// <param name="printOut">True to include a <see cref="PrintOutAttribute"/></param>
        /// <param name="useForLookup">True to include a <see cref="UseForLookupAttribute"/></param>
        /// <param name="lookUpFieldId">The FieldID <see cref="Guid"/>Used for the <see cref="UseForLookupAttribute"/></param>
        /// <param name="lookupTableId">The Table ID <see cref="Guid"/>Used for the <see cref="UseForLookupAttribute"/></param>
        /// <param name="allowImport">True to include a <see cref="AllowImportAttribute"/></param>
        /// <param name="isForeignKey">True to include a <see cref="ForeignKeyAttribute"/></param>
        /// <param name="relatedTableId">The Table ID used for the <see cref="ForeignKeyAttribute"/></param>
        /// <param name="reLabel">True to include a <see cref="RelabelAttribute"/></param>
        /// <param name="reLabelParam">The Label to use for the <see cref="RelabelAttribute"/></param>
        /// <param name="workflowSearchable">True to include a <see cref="WorkflowSearchAttribute"/></param>
        /// <param name="workflowUpdate">True to include a <see cref="WorkflowUpdateAttribute"/></param>
        /// <param name="mandatory">True to include a <see cref="MandatoryAttribute"/></param>
        /// <param name="fieldSource">The source <see cref="FieldSource"/> of the Field.</param>
        /// <returns>A new instance of <see cref="FieldAttributes"/></returns>
        FieldAttributes PopulateFieldAttributes(bool normalView, bool idField, bool genList, bool canTotal, bool printOut, bool useForLookup, Guid lookUpFieldId, Guid lookupTableId, bool allowImport, bool isForeignKey, Guid relatedTableId, bool reLabel, string reLabelParam, bool workflowSearchable, bool workflowUpdate, bool mandatory, FieldSource fieldSource);
    }
}