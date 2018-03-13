namespace SpendManagementApi.Repositories
{
    using System;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Utilities;

    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Web.Http.Description;

    using Spend_Management;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Types;
    using SpendManagementLibrary;

    /// <summary>
    /// Manages data access for custom entity records
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    internal class CustomEntityRecordRepository : BaseRepository<CustomEntityRecord>, ISupportsActionContext
    {
        /// <summary>
        /// The data class for this repository
        /// </summary>
        private cCustomEntities _customEntities;

        /// <summary>
        /// Initialises a new instance of the <see cref="CustomEntityRecordRepository"/> class with the passed user and action context.
        /// </summary>
        /// <param name="user">
        /// The current user
        /// </param>
        /// <param name="actionContext">
        /// The action context
        /// </param>
        public CustomEntityRecordRepository(ICurrentUser user, IActionContext actionContext = null)
            : base(user, actionContext, record => record.Id, record => record.Id.ToString(CultureInfo.InvariantCulture))
        {
            this._customEntities = this.ActionContext.CustomEntities;
        }

        /// <summary>
        /// Gets a list of all custom entity records
        /// </summary>
        /// <returns>List of custom entity views</returns>
        public override IList<CustomEntityRecord> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a list of all custom entity records for a given view
        /// </summary>
        /// <param name="entityId">The identifier of the entity</param>
        /// <param name="viewId">The identifier of the view</param>
        /// <returns>A list of <see cref="CustomEntityRecord"/></returns>
        public List<CustomEntityRecord> GetAll(int entityId, int viewId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cCustomEntity entity = this._customEntities.getEntityById(entityId);
            cCustomEntityView view = entity.getViewById(viewId);
            cGridNew grid = new cCustomEntities(user).getEntityRecordsGrid(user, view, entity, true);

            if (grid.SortedColumn == null)
            {
                grid.SortedColumn = grid.getDefaultSortableColumn(this.User);
            }

            DataSet data = grid.generateDataSet();

            var records = new List<CustomEntityRecord>();
            foreach (DataRow row in data.Tables[0].Rows)
            {
                var record = new CustomEntityRecord
                {
                    Id = (int)row[grid.KeyField],
                    Data = new SortedList<object, object>()
                };

                foreach (DataColumn column in data.Tables[0].Columns)
                {
                    record.Data.Add(column.ColumnName, row[column.ColumnName]);
                }

                records.Add(record);
            }

            return records;
        }

        /// <summary>
        /// Gets a custom entity record by its identifier
        /// </summary>
        /// <param name="id">The custom entity identifier</param>
        /// <returns>The custom entity view</returns>
        public override CustomEntityRecord Get(int id)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a custom entity record by its identifier and the corresponding form/entity identifiers
        /// </summary>
        /// <param name="id">The identifier of the custom entity record</param>
        /// <param name="formId">The identifier of the custom entity form</param>
        /// <param name="entityId">The identifier of the custom entity</param>
        /// <returns>A list of custom entity views</returns>
        public CustomEntityRecord Get(int id, int formId, int entityId)
        {
            var response = new CustomEntityRecord
            {
                Id = id
            };

            var fields = new cFields(this.User.AccountID);
            var entity = this._customEntities.getEntityById(entityId);
            var form = entity.getFormById(formId);
            var recordData = new SortedList<object, object>();
            var record = this._customEntities.getEntityRecord(entity, id, form);
            
            foreach (var pair in record)
            {
                recordData.Add(pair.Key, pair.Value);
            }

            // also add the data for any many to one relationship field attribute's that have display lookup fields defined
            foreach (cCustomEntityFormTab tab in form.tabs.Values)
            {
                foreach (cCustomEntityFormSection section in tab.sections)
                {
                    foreach (cCustomEntityFormField field in section.fields.Where(field => record.ContainsKey(field.attribute.attributeid)))
                    {
                        if (field.attribute.GetType() == typeof(cManyToOneRelationship) && record[field.attribute.attributeid] != System.DBNull.Value)
                        {
                            var relationship = (cManyToOneRelationship)field.attribute;

                            // get the main display field
                            var autoCompleteDisplayField = fields.GetFieldByID(relationship.AutoCompleteDisplayField);
                            var autoCompleteTriggerFields = new List<JsAutoCompleteTriggerField>();

                            // get any trigger fields
                            if (relationship.TriggerLookupFields.Count > 0)
                            {
                                foreach (LookupDisplayField lookupDisplayField in relationship.TriggerLookupFields)
                                {
                                    autoCompleteTriggerFields.Add(new JsAutoCompleteTriggerField
                                    {
                                        ControlId = "txt" + lookupDisplayField.attributeid,
                                        DisplayFieldId = lookupDisplayField.TriggerDisplayFieldId == null ? string.Empty : lookupDisplayField.TriggerDisplayFieldId.ToString(),
                                        JoinViaId = lookupDisplayField.TriggerJoinViaId ?? 0, 
                                        DisplayValue = string.Empty
                                    });
                                }
                            }

                            // get the string values to populate the fields with
                            var fieldValues = AutoComplete.GetDisplayAndTriggerFieldValues(cMisc.GetCurrentUser(), relationship.relatedtable.TableID, record[field.attribute.attributeid].ToString(), autoCompleteDisplayField, autoCompleteTriggerFields);
                            for (int i = 1; i < fieldValues.Count; i++)
                            {
                                recordData.Add(relationship.TriggerLookupFields[i - 1].attributeid, string.IsNullOrWhiteSpace(fieldValues[i].Item1) ? "&nbsp;" : fieldValues[i].Item1);
                            }
                        }
                    }
                }
            }

            response.Data = recordData;

            // also add any associated attachments and torch generated attachments
            if (entity.EnableAttachments || entity.AllowMergeConfigAccess)
            {
                var attachmentsCollection = new cAttachments(this.User.AccountID, this.User.EmployeeID, this.User.CurrentSubAccountId, null);

                if (entity.EnableAttachments)
                {
                    var attachments = attachmentsCollection.GetPublishedAttachments(this.User, entity, id);

                    response.Attachments = new List<Attachment>();
                    foreach (var attachment in attachments)
                    {
                        response.Attachments.Add(Attachment.From(attachment));
                    }
                }

                // also add any associated torch generated attachments
                if (entity.AllowMergeConfigAccess)
                {
                    var torchAttachments = attachmentsCollection.GetPublishedAttachments(this.User, entity, id, true);

                    response.TorchAttachments = new List<Attachment>();
                    foreach (var torchAttachment in torchAttachments)
                    {
                        response.TorchAttachments.Add(Attachment.From(torchAttachment));
                    }
                }
            }

            return response;
        }

        /// <summary>
        /// Get an individual <see cref="Attachment"/>
        /// </summary>
        /// <param name="entityId">The related Custom Entity identifier</param>
        /// <param name="attachmentId">The id of the attachment</param>
        /// <returns>The <see cref="Attachment"/></returns>
        public Attachment GetAttachment(int entityId, int attachmentId)
        {
            var attachments = new cAttachments(this.User.AccountID, this.User.EmployeeID, this.User.CurrentSubAccountId, null);
            cAttachment originalAttachment = attachments.getAttachment(string.Format("custom_{0}_attachments", entityId), attachmentId);

            return Attachment.From(originalAttachment);
        }

        /// <summary>
        /// Delete a custom entity instance record
        /// </summary>
        /// <param name="recordId">The ID of the record instance to delete</param>
        /// <param name="entityId">The ID of the entity that the record is associated to.</param>
        /// <returns>An instance of the deleted record</returns>
        public CustomEntityRecord Delete(int recordId, int entityId)
        {            
            var item = base.Delete(recordId);
            var entity = this.ActionContext.CustomEntities.getEntityById(entityId);
            var result = this.ActionContext.CustomEntities.DeleteCustomEntityRecord(entity, recordId, 0);
            if (item == null || result == -1)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                    ApiResources.ApiErrorDeleteUnsuccessfulMessage);
            }            

            return item;
        }

        /// <summary>
        /// Delete a custom entity instance record
        /// </summary>
        /// <param name="recordId">The ID of the record instance to delete</param>
        /// <param name="entityId">The system ID of the entity that the record is associated to.</param>
        /// <returns>An instance of the deleted record</returns>
        public CustomEntityRecord Delete(int recordId, Guid entityId)
        {
            var entity =
                this.ActionContext.CustomEntities.CustomEntities.Values.FirstOrDefault(ent =>
                    ent.SystemCustomEntityId == entityId);
            if (entity != null)
            {
                var result = this.ActionContext.CustomEntities.DeleteCustomEntityRecord(entity, recordId, 0);
                if (result == -1)
                {
                    throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                        ApiResources.ApiErrorDeleteUnsuccessfulMessage);
                }            

                return new CustomEntityRecord();
            }

            return null;
        }

        /// <summary>
        /// Save an instance record of a system GreenLight
        /// </summary>
        /// <param name="entityId">the ID of the system greenlight to save</param>
        /// <param name="record">An instacne of <see cref="CustomEntityRecord"/>to save</param>
        /// <returns>A new instance of <see cref="CustomEntityRecord"/></returns>
        public CustomEntityRecord Save(Guid entityId, CustomEntityRecord record)
        {
            var entity = this.ActionContext.CustomEntities.CustomEntities.Values.FirstOrDefault(e =>
                e.SystemCustomEntityId.HasValue && e.SystemCustomEntityId.Value == entityId);
            if (entity == null)
            {
                return null;
            }

            var query = new cUpdateQuery(this.ActionContext.AccountId,
                cAccounts.getConnectionString(this.ActionContext.AccountId), GlobalVariables.MetabaseConnectionString,
                entity.table, this.ActionContext.Tables, this.ActionContext.Fields);
            if (record.Data == null)
            {
                return null;
            }

            foreach (KeyValuePair<object, object> values in record.Data)
            {
                var fieldId = new Guid(values.Key.ToString());
                var field = this.ActionContext.Fields.GetFieldByID(fieldId);
                var attribute = entity.GetAttributeByFieldId(fieldId);
                var actualValue = AttributeValueFactory.Convert(attribute, values.Value, this.ActionContext.Fields);
                query.addColumn(field, actualValue);
            }

            record.Id = query.executeInsertStatement();
            return record;
        }
    }
}