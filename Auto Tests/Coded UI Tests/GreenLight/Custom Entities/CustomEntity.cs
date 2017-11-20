using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auto_Tests.Tools;
using System.Data;

namespace Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entities
{
    #region Basic custom entity object
    /// <summary>
    /// Custom Entity class
    /// </summary>
    public class CustomEntity : ICloneable
    {
        /// <summary>
        /// Constructor - sets up basic custom entity using userId of 0
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="pluralname"></param>
        /// <param name="description"></param>
        /// <param name="enableCurrencies"></param>
        /// <param name="defaultCurrencyId"></param>
        /// <param name="date"></param>
        /// <param name="tableid"></param>
        /// <param name="enableAttachments"></param>
        /// <param name="allowDocMerge"></param>
        /// <param name="audienceViewType"></param>
        /// <param name="_userId"></param>
        public CustomEntity(string entityName, string pluralname, string description, bool enableCurrencies, string defaultCurrencyId, DateTime date, Guid tableid, bool enableAttachments = false, bool allowDocMerge = false, AudienceViewType audienceViewType = global::AudienceViewType.NoAudience, int _userId = 0)
        {
            this.entityName = entityName;
            this.description = description;
            this.pluralName = pluralname;
            this.enableCurrencies = enableCurrencies;
            this.defaultCurrencyId = defaultCurrencyId;
            this.date = date;
            this.enableAttachments = enableAttachments;
            this.allowDocumentMerge = allowDocMerge;
            this.AudienceViewType = audienceViewType;
            this.createdBy = Convert.ToString(_userId);
            this.tableId = tableid;
        }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public CustomEntity() { }

        /// <summary>
        /// Parametered constructor that allows CE to be created with name and description
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="description"></param>
        public CustomEntity(string entityName, string description)
        {
            this.entityName = entityName;
            this.description = description;
        }

        /// <summary>
        /// Custom Entity Id
        /// </summary>
        public int entityId { get; set; }

        /// <summary>
        /// User Id of who created the Custom Entity
        /// </summary>
        public int userId { get; set; }

        /// <summary>
        /// Custom Entity name
        /// </summary>
        public string entityName { get; set; }

        /// <summary>
        /// Description for the Custom Entity
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Plural name for the Custom entity
        /// </summary>
        public string pluralName { get; set; }

        /// <summary>
        /// Enable attributes
        /// </summary>
        public bool enableAttachments { get; set; }

        /// <summary>
        /// Can this custom entity handle document merges
        /// </summary>
        public bool allowDocumentMerge { get; set; }

        /// <summary>
        /// Can this custom entity handle currencies.
        /// </summary>
        public bool enableCurrencies { get; set; }

        /// <summary>
        /// Default currency belonging to the Custom entity if enabled.
        /// </summary>
        public string defaultCurrencyId { get; set; }

        /// <summary>
        /// Can this custom entity handle audiences
        /// </summary>
        public AudienceViewType AudienceViewType { get; set; }

        public Guid OldTableId { get; set; }

        /// <summary>
        /// TableId that relates to this Custom Entity
        /// </summary>
        public Guid tableId { get; set; }

        /// <summary>
        /// Custom entity Creation date
        /// </summary>
        public DateTime date { get; set; }

        /// <summary>
        /// Owner of Custom Entity
        /// </summary>
        public string createdBy { get; set; }

        /// <summary>
        /// User who last modified the Custom Entity
        /// </summary>
        public string modifiedBy { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether enable popup window.
        /// </summary>
        public bool EnablePopupWindow { get; set; }

        /// <summary>
        /// Attributes that are associated with this custom entity
        /// </summary>
        public List<CustomEntitiesUtilities.CustomEntityAttribute> attribute { get; set; }

        /// <summary>
        /// Forms that are associated with this custom entity
        /// </summary>
        public List<CustomEntitiesUtilities.CustomEntityForm> form { get; set; }

        /// <summary>
        /// Views that are associated with this custom entity
        /// </summary>
        public List<CustomEntitiesUtilities.CustomEntityView> view { get; set; }

        /// <summary>
        /// Gets or sets the default popup window.
        /// </summary>
        public int? DefaultPopupView { get; set; }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
    #endregion  
}
