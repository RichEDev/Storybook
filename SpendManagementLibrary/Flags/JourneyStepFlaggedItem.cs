using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SpendManagementLibrary.Flags
{
    [Serializable]
    [DataContract]
    public class JourneyStepFlaggedItem
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="JourneyStepFlaggedItem"/> class.
        /// </summary>
        /// <param name="stepNumber">The journey step number</param>
        /// <param name="exceededamount">The number of miles/passengers the flag rule was exceeded by</param>
        /// <param name="flagDescription">The flag description</param>
        /// <param name="flaggedItemId">The flagged item id</param>
        /// <param name="claimantJustification">The justification provided by the claimant</param>
        public JourneyStepFlaggedItem(int stepNumber, decimal exceededamount, string flagDescription, int flaggedItemId, string claimantJustification)
        {
            this.StepNumber = stepNumber;
            this.ExceededAmount = exceededamount;
            this.FlagDescription = flagDescription;
            this.FlaggedItemID = flaggedItemId;
            this.ClaimantJustification = claimantJustification;
        }

        /// <summary>
        /// Gets or sets the flag description
        /// </summary>
        [DataMember]
        public string FlagDescription { get; set; }
        /// <summary>
        /// Gets the step number the journey step was exceeded on.
        /// </summary>
        [DataMember]
        public int StepNumber { get; set; }

        /// <summary>
        /// Gets the number of miles/passengers etc it was exceeded by.
        /// </summary>
        [DataMember]
        public decimal ExceededAmount { get; set; }

        /// <summary>
        /// Gets or sets the flagged item ID
        /// </summary>
        [DataMember]
        public int FlaggedItemID { get; set; }

        /// <summary>
        /// Gets or sets the claimant justification of the journey step
        /// </summary>
        [DataMember]
        public string ClaimantJustification { get; set; }
    }
}
