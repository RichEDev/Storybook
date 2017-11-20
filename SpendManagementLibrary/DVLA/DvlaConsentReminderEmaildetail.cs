namespace SpendManagementLibrary.DVLA
{
    /// <summary>
    /// Class for holding the Dvla consent reminder email details.
    /// </summary>
    public class DvlaConsentReminderEmaildetail
    {
        /// <summary>
        /// Gets or sets the claimant id.
        /// </summary>
        public int ClaimantId { get; set; }

        /// <summary>
        /// Gets or sets the email id provided by the claimant on the Consent page.
        /// </summary>
        public string EmailId { get; set; }

        /// <summary>
        /// Gets or sets the security key used for performing consent actions in licence check portal.
        /// </summary>
        public string SecurityKey { get; set; }

        /// <summary>
        /// The initialise consent detials for email reminders.
        /// </summary>
        /// <param name="claimantId">
        /// The claimant id.
        /// </param>
        /// <param name="emailId">
        /// The email id provided by the claimant in the consent page.
        /// </param>
        /// <param name="securityKey">
        /// The security key to perform actions in licence check portal.
        /// </param>
        /// <returns>
        /// The <see cref="DvlaConsentReminderEmaildetail"/>.
        /// </returns>
        public static DvlaConsentReminderEmaildetail InitialiseClaimantIds(int claimantId, string emailId, string securityKey)
        {
            var claimantDetails = new DvlaConsentReminderEmaildetail
                                      {
                                          ClaimantId = claimantId,
                                          EmailId = emailId,
                                          SecurityKey = securityKey
                                       };
            return claimantDetails;
        }
    }
}
