using System;
namespace SpendManagementApi.Models.Types.MyDetails
{
    using SpendManagementApi.Interfaces;
    using SpendManagementLibrary;

    /// <summary>
    /// Represent Employee ESR Assignment
    /// </summary>
    public class ESRAssignment : IApiFrontForDbObject<cESRAssignment, ESRAssignment>
    {
        /// <summary>
        /// Gets or sets the assignment number.
        /// </summary>
        public string AssignmentNumber { get; set; }

        /// <summary>
        /// Gets or sets the Assignment Startdate.
        /// </summary>
        public DateTime AssignmentStartdate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether primary assignment.
        /// </summary>
        public bool PrimaryAssignment { get; set; }

        /// <summary>
        /// Gets or sets the effective startdate.
        /// </summary>
        public DateTime? EffectiveStartdate { get; set; }

        /// <summary>
        /// Gets or sets the effective enddate.
        /// </summary>
        public DateTime? EffectiveEnddate { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public ESRAssignment From(cESRAssignment dbType, IActionContext actionContext)
        {
            this.AssignmentNumber = dbType.assignmentnumber;
            this.AssignmentStartdate = dbType.earliestassignmentstartdate;
            this.EffectiveStartdate = dbType.EffectiveStartDate;
            this.EffectiveEnddate = dbType.EffectiveEndDate;
            this.Active = dbType.active;
            this.PrimaryAssignment = dbType.primaryassignment;
            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public cESRAssignment To(IActionContext actionContext)
        {
            //TODO: Not needed
            throw new NotImplementedException();
        }
    }
}