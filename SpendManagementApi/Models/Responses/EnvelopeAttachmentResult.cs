namespace SpendManagementApi.Models.Responses
{
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// The envelope attachment result.
    /// </summary>  
    public class EnvelopeAttachmentResult : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Expedite.EnvelopeAttachmentResult, EnvelopeAttachmentResult>
    {
        /// <summary>
        /// Gets or sets whether the Attachment was a success.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the envelope number.
        /// </summary>
        public string EnvelopeNumber { get; set; }

        /// <summary>
        /// Gets or sets the reason why the attachment was not a success, if attachment failed.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public EnvelopeAttachmentResult From(SpendManagementLibrary.Expedite.EnvelopeAttachmentResult dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.Success = dbType.Success;
            this.EnvelopeNumber = dbType.EnvelopeNumber;
            this.Reason = dbType.Reason;
         
            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Expedite.EnvelopeAttachmentResult To(IActionContext actionContext)
        {
            throw new System.NotImplementedException();
        }
    }
}