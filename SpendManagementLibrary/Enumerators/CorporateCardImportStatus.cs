namespace SpendManagementLibrary.Enumerators
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The reasons that a claim may not be un-submitted..
    /// </summary>
    public enum CorporateCardImportStatus
    {
        /// <summary>
        /// The file imported successfully.
        /// </summary>
        [Display(Name = "Imported")]
        Imported = 0,

        /// <summary>
        /// The file failed validation.
        /// </summary>
        [Display(Name="Failed Validation")]
        FailedValidation = 1
    }
}
