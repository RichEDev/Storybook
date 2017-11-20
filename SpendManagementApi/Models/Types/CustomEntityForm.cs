namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http.Description;
    
    using Spend_Management;
    using SpendManagementLibrary;

    /// <summary>
    /// Custom Entity Form API Class
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CustomEntityForm : BaseExternalType
    {
        /// <summary>
        /// Gets or sets the identifier of the form
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="CustomEntityFormTab">custom entity form tabs</see> related to this form
        /// Note that only the first tab is populated, this is by design for the corporate diligence mobile app
        /// </summary>
        public List<CustomEntityFormTab> Tabs { get; set; }

        /// <summary>
        /// Populates this object's properties with values from a SpendManagementLibrary <see cref="cCustomEntityForm">custom entity form object</see>
        /// </summary>
        /// <param name="form">The SpendManagementLibrary <see cref="cCustomEntityForm">custom entity form object</see> to copy values from</param>
        /// <param name="entity">The related SpendManagementLibrary <see cref="cCustomEntity">custom entity object</see></param>
        /// <param name="user">The SpendManagementLibrary <see cref="ICurrentUser">user object</see></param>
        /// <returns>This object</returns>
        public CustomEntityForm From(cCustomEntityForm form, cCustomEntity entity, ICurrentUser user)
        {
            if (form == null)
            {
                return null;
            }

            this.Id = form.formid;
            this.Tabs = new List<CustomEntityFormTab>();

            if (form.tabs.Any())
            {
                // only add the first tab, as per the specification of the corporate diligence mobile app
                this.Tabs.Add(new CustomEntityFormTab().From(form.tabs.First().Value, entity, user));
            }

            return this;
        }
    }
}