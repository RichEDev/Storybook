namespace SpendManagementLibrary
{
    using System;
    using SpendManagementLibrary.Definitions.JoinVia;

    /// <summary>
    /// Trigger display field, displays other data from a ManyToOne object
    /// </summary>
    public class LookupDisplayField : cAttribute
    {
        /// <summary>
        /// The attribute id. of the Trigger (Many to One) Object.
        /// </summary>
        private int? _triggerAttributeId;

        /// <summary>
        /// The join via id. from the source data
        /// </summary>
        private int? _triggerJoinViaId;

        /// <summary>
        /// The display field id. from the source data
        /// </summary>
        private Guid? _triggerDisplayFieldId;

        /// <summary>
        /// The _trigger join via.
        /// </summary>
        private JoinVia _triggerJoinVia;

        /// <summary>
        /// Initializes a new instance of the <see cref="LookupDisplayField"/> class.
        /// </summary>
        /// <param name="attributeid">
        /// The attribute id.
        /// </param>
        /// <param name="attributename">
        /// The attribute name.
        /// </param>
        /// <param name="displayname">
        /// The display name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="tooltip">
        /// The tooltip.
        /// </param>
        /// <param name="createdon">
        /// The created on.
        /// </param>
        /// <param name="createdby">
        /// The created by.
        /// </param>
        /// <param name="modifiedon">
        /// The modified on.
        /// </param>
        /// <param name="modifiedby">
        /// The modified by.
        /// </param>
        /// <param name="fieldid">
        /// The field id.
        /// </param>
        /// <param name="triggerAttributeId">
        /// The trigger attribute id.
        /// </param>
        /// <param name="triggerJoinVia">
        /// The trigger join via object.
        /// </param>
        /// <param name="triggerDisplayFieldId">
        /// The trigger display field id.
        /// </param>
        public LookupDisplayField(int attributeid, string attributename, string displayname, string description, string tooltip, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, Guid fieldid, int? triggerAttributeId, JoinVia triggerJoinVia, Guid? triggerDisplayFieldId)
            : base(attributeid, attributename, displayname, description, tooltip, false, FieldType.LookupDisplayField, createdon, createdby, modifiedon, modifiedby, fieldid, false, false, false, false, false,  false,  false,  false, false, false)
        {
            this._triggerAttributeId = triggerAttributeId;
            this._triggerDisplayFieldId = triggerDisplayFieldId;
            if (triggerJoinVia != null)
            {
                this._triggerJoinViaId = triggerJoinVia.JoinViaID;
                this._triggerJoinVia = triggerJoinVia;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LookupDisplayField"/> class.
        /// </summary>
        /// <param name="attribute">
        /// The attribute to use as a lookup source.
        /// </param>
        /// <param name="triggerAttributeId">
        /// attribute id.
        /// </param>
        /// <param name="triggerJoinVia">
        /// join via object.
        /// </param>
        /// <param name="triggerDisplayFieldId">
        /// display field id.
        /// </param>
        /// <param name="displayInMobile">Whether the attribute should be displayed on mobile.</param>
        /// <param name="builtIn">True if the <seealso cref="cAttribute"/>is a system field.</param>
        public LookupDisplayField(cAttribute attribute, int? triggerAttributeId, JoinVia triggerJoinVia, Guid? triggerDisplayFieldId, bool displayInMobile, bool builtIn)
            : base(attribute.attributeid, attribute.attributename, attribute.displayname, attribute.description, attribute.tooltip, attribute.mandatory, attribute.fieldtype, attribute.createdon, attribute.createdby, attribute.modifiedon, attribute.modifiedby, attribute.fieldid, attribute.iskeyfield, attribute.isauditidentifer, attribute.isunique, attribute.AllowEdit, attribute.AllowDelete, displayInMobile, builtIn, false, attribute.IsSystemAttribute, false)
        {
            this._triggerAttributeId = triggerAttributeId;
            this._triggerDisplayFieldId = triggerDisplayFieldId;
            if (triggerJoinVia != null)
            {
                this._triggerJoinViaId = triggerJoinVia.JoinViaID;
                this._triggerJoinVia = triggerJoinVia;
            }
        }

        /// <summary>
        /// Gets or sets the trigger attribute id.
        /// </summary>
        public int? TriggerAttributeId
        {
            get { return this._triggerAttributeId; }
            set { this._triggerAttributeId = value; }
        }

        /// <summary>
        /// Gets or sets the trigger join via id.
        /// </summary>
        public int? TriggerJoinViaId
        {
            get { return this._triggerJoinViaId; }
            set { this._triggerJoinViaId = value; }
        }

        /// <summary>
        /// Gets or sets the trigger display field id.
        /// </summary>
        public Guid? TriggerDisplayFieldId
        {
            get { return this._triggerDisplayFieldId; }
            set { this._triggerDisplayFieldId = value; }
        }

        /// <summary>
        /// Gets or sets the trigger field join via.
        /// </summary>
        public JoinVia TriggerJoinVia
        {
            get { return this._triggerJoinVia; }
            set { this._triggerJoinVia = value; }
        }
    }
}