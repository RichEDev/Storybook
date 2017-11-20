namespace SpendManagementApi.Models.Types.Flags
{
    using System;

    using SpendManagementApi.Interfaces;

    using SpendManagementLibrary.Flags;

    /// <summary>
    /// The jorney step flagged item.
    /// </summary>
    public class JorneyStepFlaggedItem : IApiFrontForDbObject<SpendManagementLibrary.Flags.JourneyStepFlaggedItem, JorneyStepFlaggedItem>
    {

        /// <summary>
        /// Gets or sets the flag description
        /// </summary>  
        public string FlagDescription { get; set; }

        /// <summary>
        /// Gets or sets the step number the journey step was exceeded on.
        /// </summary>     
        public int StepNumber { get; set; }

        /// <summary>
        /// Gets or sets the number of miles/passengers etc it was exceeded by.
        /// </summary>   
        public decimal ExceededAmount { get; set; }

        /// <summary>
        /// Gets or sets the flagged item ID
        /// </summary>   
        public int FlaggedItemID { get; set; }

        /// <summary>
        /// Gets or sets the claimant justification of the journey step
        /// </summary>
        public string ClaimantJustification { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public JorneyStepFlaggedItem From(JourneyStepFlaggedItem dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.FlagDescription = dbType.FlagDescription;
            this.StepNumber = dbType.StepNumber;
            this.ExceededAmount = dbType.ExceededAmount;
            this.FlaggedItemID = dbType.FlaggedItemID;
            this.ClaimantJustification = dbType.ClaimantJustification;

            return this;

        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public JourneyStepFlaggedItem To(IActionContext actionContext)
        {
            throw new NotImplementedException();
        }
    }
}