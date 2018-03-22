﻿namespace SpendManagementLibrary
{
    using System;

    /// <summary>
    /// cOneToManyRelationship class which inherits cAttribute
    /// </summary>
    public class cOneToManyRelationship : cAttribute
    {
        [NonSerialized()]
        private cTable tblRelatedTable;
        private int nViewID;
        private int nEntityid;
        private int nParentEntityId;

        /// <summary>
        /// Create a new instance of <see cref="cOneToManyRelationship"/>
        /// </summary>
        /// <param name="attributeid">Attribute ID</param>
        /// <param name="attributename">Attribute name</param>
        /// <param name="displayname">Friendly attribute name to display</param>
        /// <param name="description">Attribute description</param>
        /// <param name="tooltip">Tooltip associated with attribute</param>
        /// <param name="mandatory">Is attribute mandatory</param>
        /// <param name="createdon">Date attribute created</param>
        /// <param name="createdby">User ID of attribute creator</param>
        /// <param name="modifiedon">Date attribute last modified</param>
        /// <param name="modifiedby">User ID of last user to modify attribute</param>
        /// <param name="fieldid">Field ID for reporting</param>
        /// <param name="isauditidentity">Is attribute used as field identifier in the audit log</param>
        /// <param name="builtIn">Whether the attribute is a system attribute</param>
        /// <param name="system_attribute">Indicates whether the attribute is generated by the application and not a user</param>
        public cOneToManyRelationship(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, bool builtIn, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, cTable relatedtable, Guid fieldid, int viewid, int entityid, bool isauditidentity, int parent_entityid, bool allowedit, bool allowdelete, bool system_attribute)
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, FieldType.Relationship, createdon, createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, false, allowedit, allowdelete, false, builtIn, false, system_attribute, false)
        {
            this.tblRelatedTable = relatedtable;
            this.nViewID = viewid;
            this.nEntityid = entityid;
            this.nParentEntityId = parent_entityid;

            this.sCssClass = "";
        }

        #region properties
        /// <summary>
        /// Gets the table definition attribute relates to
        /// </summary>
        public cTable relatedtable
        {
            get { return this.tblRelatedTable; }
        }
        /// <summary>
        /// Gets the custom entity view id for relationship
        /// </summary>
        public int viewid
        {
            get { return this.nViewID; }
        }
        /// <summary>
        /// Gets the related entity id field
        /// </summary>
        public int entityid
        {
            get { return this.nEntityid; }
        }
        /// <summary>
        /// Gtes Entity ID that attribute belongs to
        /// </summary>
        public int parent_entityid
        {
            get { return this.nParentEntityId; }
        }
        #endregion
    }
}