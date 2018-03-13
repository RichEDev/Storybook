namespace SpendManagementLibrary.Logic_Classes.Fields
{
    using System;
    using System.Collections.Generic;

    public interface IFields
    {
        /// <summary>
        /// Gets the number of fields in the private collection
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Get all the <see cref="cField"/> that have a matching ViewGroupId property
        /// </summary>
        /// <param name="id">The <see cref="Guid"/>to match</param>
        /// <returns>A <see cref="SortedList{TKey,TValue}"/> of <seealso cref="Guid"/>Id annd <seealso cref="cField"/></returns>
        SortedList<Guid, cField> getAllFieldsByViewGroup(Guid id);

        /// <summary>
        /// Get all fields where the related property matches the given <see cref="Guid"/>ID
        /// </summary>
        /// <param name="relatedTableId">The <see cref="Guid"/>ID to match</param>
        /// <returns>A <see cref="List{T}"/>of <seealso cref="cField"/>That match the given property</returns>
        List<cField> GetAllRelatedFields(Guid relatedTableId);

        /// <summary>
        /// Get a specific <see cref="cField"/> based on the Table Id and Field Name properties
        /// </summary>
        /// <param name="tableID">the <see cref="Guid"/>table id to match</param>
        /// <param name="fieldName">The field name to match</param>
        /// <returns>The first matching <see cref="cField"/> or null</returns>
        cField GetBy(Guid tableID, string fieldName);

        /// <summary>
        /// Get a <see cref="cField"/> using the tableId and field Name
        /// </summary>
        /// <param name="tableID">The <see cref="Guid"/>Id to match against the tableid</param>
        /// <param name="fieldName">The field name to match</param>
        /// <returns>The first instance of <see cref="cField"/>That matches or null</returns>
        cField GetCustomFieldByTableAndFieldName(Guid tableID, string fieldName);

        /// <summary>
        /// Get a specific <see cref="cField"/> based on the Field Name property
        /// </summary>
        /// <param name="fieldName">The field name to match</param>
        /// <returns>The first matching <see cref="cField"/> or null</returns>
        cField getFieldByFieldName(string fieldName);

        /// <summary>
        /// Get a specific <see cref="cField"/> based on the Field Id property
        /// </summary>
        /// <param name="fieldID">the <see cref="Guid"/>field id to match</param>
        /// <returns>The first matching <see cref="cField"/> or null</returns>
        cField GetFieldByID(Guid fieldID);

        /// <summary>
        /// Get a specific <see cref="cField"/> based on the Field Name property
        /// </summary>
        /// <param name="name">the <see cref="string"/>field name to match</param>
        /// <returns>The first matching <see cref="cField"/> or null</returns>
        cField getFieldByName(string name);

        /// <summary>
        /// Get a specific <see cref="cField"/> based on the Table Id and Class property Name properties
        /// </summary>
        /// <param name="tableid">the <see cref="Guid"/>table id to match</param>
        /// <param name="classPropertyName">The class property name to match</param>
        /// <returns>The first matching <see cref="cField"/> or null</returns>
        cField getFieldByTableAndClassPropertyName(Guid tableid, string classPropertyName);

        /// <summary>
        /// Get a specific <see cref="cField"/> based on the Table Id and description properties
        /// </summary>
        /// <param name="tableId">the <see cref="Guid"/>table id to match</param>
        /// <param name="description">The table description to match</param>
        /// <returns>The first matching <see cref="cField"/> or null</returns>
        cField GetFieldByTableAndDescription(Guid tableId, string description);

        /// <summary>
        /// Get a specific <see cref="cField"/> based on the Table Id and Field description properties
        /// </summary>
        /// <param name="tableID">the <see cref="Guid"/>table id to match</param>
        /// <param name="description">The field description to match</param>
        /// <returns>The first matching <see cref="cField"/> or null</returns>
        cField GetFieldByTableAndFieldDescription(Guid tableID, string description);

        /// <summary>
        /// Get a specific <see cref="cField"/> based on the Table name and Field Name properties
        /// </summary>
        /// <param name="tableName">the <see cref="Guid"/>table id to match</param>
        /// <param name="fieldName">The field name to match</param>
        /// <returns>The first matching <see cref="cField"/> or null</returns>
        cField GetFieldByTableAndFieldName(string tableName, string fieldName);

        /// <summary>
        /// Get a specific <see cref="cField"/> based on the Table Id and Field Name properties
        /// </summary>
        /// <param name="tableID">the <see cref="Guid"/>table id to match</param>
        /// <param name="fieldName">The field name to match</param>
        /// <returns>The first matching <see cref="cField"/> or null</returns>
        cField GetFieldByTableAndFieldName(Guid tableID, string fieldName);

        /// <summary>
        /// Get a <see cref="List{T}"/> of <see cref="cField"/> based on the Table Id 
        /// </summary>
        /// <param name="tableID">the <see cref="Guid"/>table id to match</param>
        /// <returns>A <see cref="List{T}"/>of <see cref="cField"/> that have the matching Table ID</returns>
        List<cField> GetFieldsByTableID(Guid tableID);

        /// <summary>
        /// Get a <see cref="List{T}"/> of <see cref="cField"/> based on the Table Id 
        /// </summary>
        /// <param name="tableID">the <see cref="Guid"/>table id to match</param>
        /// <returns>A <see cref="List{T}"/>of <see cref="cField"/> that have the matching Table ID</returns>
        List<cField> GetFieldsByTableIDForViews(Guid tableID);

        /// <summary>
        /// Get a <see cref="SortedList{TKey,TValue}"/> of <see cref="string"/>Name <see cref="cField"/> based on the Table Id 
        /// </summary>
        /// <param name="id">the <see cref="Guid"/>table id to match</param>
        /// <returns>A <see cref="SortedList{TKey,TValue}"/> of <see cref="string"/>Name <see cref="cField"/> that have the matching Table ID</returns>
        SortedList<string, cField> getFieldsByViewGroup(Guid id);

        /// <summary>
        /// Get a <see cref="List{T}"/> of <see cref="cField"/> based on the Table Id 
        /// </summary>
        /// <param name="tableid">the <see cref="Guid"/>table id to match</param>
        /// <returns>A <see cref="List{T}"/>of <see cref="cField"/> that have the matching Table ID</returns>
        List<cField> getLookupFields(Guid tableid);

        /// <summary>
        /// Get a <see cref="List{T}"/> of <see cref="cField"/> that are required for Printout
        /// </summary>
        /// <returns>A <see cref="List{T}"/>of <see cref="cField"/> that are required for printout</returns>
        List<cField> getPrintoutFields();

        /// <summary>
        /// Create a query and search in a specific field for a given value.
        /// </summary>
        /// <param name="fieldId">The ID of the fields to search</param>
        /// <param name="searchType">TRhe search type as defined by <see cref="ConditionType"/></param>
        /// <param name="searchValue">The value to search for</param>
        /// <param name="maxResults">The maximum number of results to return</param>
        /// <returns>An array of objects</returns>
        string[] SearchFieldByFieldID(Guid fieldId, ConditionType searchType, string searchValue, int maxResults);
    }
}