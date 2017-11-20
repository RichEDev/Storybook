namespace SpendManagementApi.Models.Types.Expedite
{
    /// <summary>
    /// Defines the method of creation for a Receipt.
    /// </summary>
    public enum ReceiptCreationMethod
    {
        /// <summary>
        /// Unknown - currently for any converted receipts 
        /// for which we cannot determine the creation method.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Uploaded by the claimant using the uploader in Expenses.
        /// </summary>
        UploadedByClaimant = 1,

        /// <summary>
        /// Uploaded through the API by an Expedite staff member or machine.
        /// </summary>
        UploadedByExpedite = 2,

        /// <summary>
        /// Uploaded with a mobile device.
        /// </summary>
        UploadedByMobile = 3,

        /// <summary>
        /// Emailed in to the service.
        /// </summary>
        EmailedIn = 4
    }
}
