namespace DutyOfCareAPI.DutyOfCare
{
    using System;

    /// <summary>
    /// Endorsements are convictions recorded against the licence 
    /// </summary>
    public class EndorsementDetails
    {
        /// <summary>
        ///  Identifies the endorsement code 
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The date of conviction
        /// </summary>
        public DateTime ConvictionDate { get; set; }

        /// <summary>
        /// True or False based upon if the endorsement resulted in the driver being disqualified from driving
        /// </summary>
        public bool IsDisqualified { get; set; }

        /// <summary>
        /// The end date of the disqualification period
        /// </summary>
        public DateTime DisqualificationEndDate { get; set; }

        /// <summary>
        /// Number of penalty points present on the endorsement
        /// </summary>
        public int NumberOfPoints { get; set; }

        /// <summary>
        ///  The date of the offence
        /// </summary>
        public DateTime OffenceDate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndorsementDetails"/> class. 
        /// Endorsement constructor with endorsement details for driving licence
        /// </summary>
        /// <param name="code">
        /// Identifies the endorsement code 
        /// </param>
        /// <param name="convictionDate">
        /// The date of conviction
        /// </param>
        /// <param name="isDisqualified">
        /// True or False based upon if the endorsement resulted in the driver being disqualified from driving
        /// </param>
        /// <param name="disqualificationEndDate">
        /// The end date of the disqualification period
        /// </param>
        /// <param name="numberOfPoints">
        /// Number of penalty points present on the endorsement
        /// </param>
        /// <param name="offenceDate">
        /// The date of the offence
        /// </param>
        public EndorsementDetails(string code,DateTime convictionDate,bool isDisqualified,DateTime disqualificationEndDate,int numberOfPoints,DateTime offenceDate)
        {
            this.Code = code;
            this.ConvictionDate = convictionDate;
            this.IsDisqualified = isDisqualified;
            this.DisqualificationEndDate = disqualificationEndDate;
            this.NumberOfPoints = numberOfPoints;
            this.OffenceDate = offenceDate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndorsementDetails"/> class.
        /// </summary>
        public EndorsementDetails()
        {
        }
    }
}