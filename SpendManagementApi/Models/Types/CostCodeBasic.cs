namespace SpendManagementApi.Models.Responses
{
    using System.ComponentModel.DataAnnotations;

    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Utilities;

    using SpendManagementLibrary;

    /// <summary>
    /// The cost code basic class. 
    /// </summary>
    public class CostCodeBasic
    {
        /// <summary>
        /// The Id of this object.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name / label for this CostCode object.
        /// </summary>
        [Required, MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string Label { get; set; }

        /// <summary>
        /// A description of this CostCode object.
        /// </summary>
        [MaxLength(4000, ErrorMessage = ApiResources.ErrorMaxLength + @"4000")]
        public string Description { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public CostCodeBasic From(cCostCode dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.Id = dbType.CostcodeId;
            this.Label = dbType.Costcode;
            this.Description = dbType.Description;
      
            return this;
        }   
    }
}