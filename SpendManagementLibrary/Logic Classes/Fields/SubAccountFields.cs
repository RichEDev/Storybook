namespace SpendManagementLibrary.Logic_Classes.Fields
{
    using System;
    using System.Collections.Generic;

    public class SubAccountFields : IFields
    {
        private readonly IFields _fields;

        private readonly IRelabler<cField> _fieldRelabler;

        /// <summary>
        /// Create a new instance of <see cref="IFields"/> that deals with Sub account specific descriptions for fields
        /// </summary>
        /// <param name="fields">An instance of <see cref="IFields"/></param>
        /// <param name="fieldRelabler">An instance of <see cref="IRelabler{T}"/>Where T is <seealso cref="cField"/></param>
        public SubAccountFields(IFields fields, IRelabler<cField> fieldRelabler )
        {
            this._fields = fields;
            this._fieldRelabler = fieldRelabler;
        }

        public int Count { get; }

        /// <inheritdoc />
        public SortedList<Guid, cField> getAllFieldsByViewGroup(Guid id)
        {
            return this._fieldRelabler.Convert<Guid>(this._fields.getAllFieldsByViewGroup(id));
        }

        public List<cField> GetAllRelatedFields(Guid relatedTableId)
        {
            return this._fieldRelabler.Convert(this._fields.GetAllRelatedFields(relatedTableId));
        }


        public cField GetBy(Guid tableID, string fieldName)
        {
            return this._fieldRelabler.Convert(this._fields.GetBy(tableID, fieldName));
        }


        public cField GetCustomFieldByTableAndFieldName(Guid tableID, string fieldName)
        {
            return this._fieldRelabler.Convert(
                this._fields.GetCustomFieldByTableAndFieldName(tableID, fieldName));
        }

        public cField getFieldByFieldName(string fieldName)
        {
            return this._fieldRelabler.Convert(
                this._fields.getFieldByFieldName(fieldName));
        }

        public cField GetFieldByID(Guid fieldID)
        {
            return this._fieldRelabler.Convert(this._fields.GetFieldByID(fieldID));
        }

        public cField getFieldByName(string name)
        {
            return this._fieldRelabler.Convert(this._fields.getFieldByFieldName(name));
        }

        public cField getFieldByTableAndClassPropertyName(Guid tableid, string classPropertyName)
        {
            return
                this._fieldRelabler.Convert(
                    this._fields.getFieldByTableAndClassPropertyName(tableid, classPropertyName));
        }

        public cField GetFieldByTableAndDescription(Guid tableId, string description)
        {
            return this._fieldRelabler.Convert(this._fields.GetFieldByTableAndDescription(tableId, description));
        }

        public cField GetFieldByTableAndFieldDescription(Guid tableID, string description)
        {
            return this._fieldRelabler.Convert(this._fields.GetFieldByTableAndDescription(tableID, description));
        }

        public cField GetFieldByTableAndFieldName(string tableName, string fieldName)
        {
            return this._fieldRelabler.Convert(this._fields.GetFieldByTableAndFieldName(tableName, fieldName));
        }

        public cField GetFieldByTableAndFieldName(Guid tableID, string fieldName)
        {
            return this._fieldRelabler.Convert(this._fields.GetFieldByTableAndFieldName(tableID, fieldName));
        }

        public List<cField> GetFieldsByTableID(Guid tableID)
        {
            return this._fieldRelabler.Convert(this._fields.GetFieldsByTableID(tableID));
        }

        public List<cField> GetFieldsByTableIDForViews(Guid tableID)
        {
            return this._fieldRelabler.Convert(this._fields.GetFieldsByTableIDForViews(tableID));
        }

        public SortedList<string, cField> getFieldsByViewGroup(Guid id)
        {
            return this._fieldRelabler.Convert<string>(this._fields.getFieldsByViewGroup(id));
        }


        public List<cField> getLookupFields(Guid tableid)
        {
            return this._fieldRelabler.Convert(this._fields.getLookupFields(tableid));
        }

        public List<cField> getPrintoutFields()
        {
            return this._fieldRelabler.Convert(this._fields.getPrintoutFields());
        }

    }

}