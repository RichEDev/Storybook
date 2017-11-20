namespace SpendManagementLibrary.Account
{
    using System;
    /// <summary>
    /// An instance of <see cref="IBankAccountValid"/> where the validation has failed.
    /// </summary>
    [Serializable]
    public class BankAccountInvalid : IBankAccountValid
    {
        /// <summary>
        ///     Gets or sets whether the account number a sortcode are valid.
        /// </summary>
        public bool IsCorrect { get; set; }

        /// <summary>
        /// True if the validation service is /was not available when the bank account was validated.
        /// </summary>
        public bool NoService => false;

        /// <summary>
        ///     Gets more detail about the outcome of the validation process.Describes reasons validation failed or changes made to
        ///     pass validation.DetailsChanged indicates that the account and sortcode should be changed for BACs submission (check
        ///     CorrectedAccountNumber and CorrectedSortCode). CautiousOK is set where the sortcode exists but no validation rules
        ///     are set for the bank(very rare).
        ///     None
        ///     UnknownSortCode
        ///     InvalidAccountNumber
        ///     OK
        ///     CautiousOK
        ///     DetailsChanged
        /// </summary>
        public string StatusInformation { get; set; }

        /// <summary>
        ///     Gets the correct version of the SortCode.This will be 6 digits long with no hyphens.It may differ from the original
        ///     sortcode.
        /// </summary>
        public string CorrectedSortCode { get; set; }

        /// <summary>
        ///     Gets the correct version of the AccountNumber. This will be 8 digits long and in the form expected for BACs
        ///     submission.
        /// </summary>
        public string CorrectedAccountNumber { get; set; }

        public BankAccountInvalid(bool isCorrect, string statusInformation)
        {
            this.IsCorrect = isCorrect;
            this.StatusInformation = statusInformation;
        }
    }
}