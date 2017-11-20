namespace SpendManagementApi.Models.Types
{
    using System;

    using SpendManagementApi.Interfaces;

    using SML = SpendManagementLibrary;

    /// <summary>
    /// A class to hold the filter rule values that belong to a <see cref="FilterRule">FilterRule</see>.
    /// </summary>
    public class FilterRuleValues : BaseExternalType, IApiFrontForDbObject<SML.cFilterRuleValue, FilterRuleValues>
    {
        /// <summary>
        /// The filter rule id.
        /// </summary>
        public int FilterRuleId { get; set; }

        /// <summary>
        /// The parent id.
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// The child id.
        /// </summary>
        public int ChildId { get; set; }

        /// <summary>
        /// The filter id.
        /// </summary>
        public int FilterId { get; set; }

        /// <summary>
        /// When the value was created.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Who created the filter rule value.
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public FilterRuleValues From(SML.cFilterRuleValue dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.FilterId = dbType.filterruleid;
            this.ParentId = dbType.parentid;
            this.ChildId = dbType.childid;
            this.FilterId = dbType.filterid;
            this.CreatedOn = dbType.createdon;
            this.CreatedBy = dbType.createdby;

            return this;

        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public SML.cFilterRuleValue To(IActionContext actionContext)
        {
            throw new NotImplementedException();
        }
    }
}