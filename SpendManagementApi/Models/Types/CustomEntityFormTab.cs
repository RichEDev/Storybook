namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;
    using System.Web.Http.Description;
    
    using Spend_Management;
    using SpendManagementLibrary;

    /// <summary>
    /// Custom Entity Form Tab API Class
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CustomEntityFormTab
    {
        /// <summary>
        /// Gets or sets the identifier of the tab
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the tab header text
        /// </summary>
        public string HeaderCaption { get; set; }

        /// <summary>
        /// Gets or sets a list of <see cref="CustomEntityFormSection">custom entity form sections</see>
        /// </summary>
        public List<CustomEntityFormSection> Sections { get; set; }

        /// <summary>
        /// Populates this object's properties with values from a SpendManagementLibrary <see cref="cCustomEntityFormTab">custom entity form tab object</see>
        /// </summary>
        /// <param name="tab">The SpendManagementLibrary <see cref="cCustomEntityFormTab">custom entity form tab object</see> to copy values from</param>
        /// <param name="entity">The related SpendManagementLibrary <see cref="cCustomEntity">custom entity object</see></param>
        /// <param name="user">The SpendManagementLibrary <see cref="ICurrentUser">user object</see></param>
        /// <returns>This object</returns>
        public CustomEntityFormTab From(cCustomEntityFormTab tab, cCustomEntity entity, ICurrentUser user)
        {
            this.Id = tab.tabid;
            this.HeaderCaption = tab.headercaption;
            this.Sections = new List<CustomEntityFormSection>();

            foreach (cCustomEntityFormSection section in tab.sections)
            {
                this.Sections.Add(new CustomEntityFormSection().From(section, user));
            }

            if (entity.AllowMergeConfigAccess)
            {
                this.Sections.Add(new CustomEntityFormSection
                {
                    HeaderCaption = "Documents",
                    Type = CustomEntityFormSectionType.TorchAttachments
                });
            }

            if (entity.EnableAttachments)
            {
                this.Sections.Add(new CustomEntityFormSection
                {
                    HeaderCaption = "Attachments",
                    Type = CustomEntityFormSectionType.Attachments
                });
            }

            return this;
        }
    }
}