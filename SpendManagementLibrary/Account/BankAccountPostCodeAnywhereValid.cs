namespace SpendManagementLibrary.Account
{
    using System;

    /// <summary>
    /// An instance of <see cref="IBankAccountValid"/> returned by Postcode Anywhere Payment Service.
    /// </summary>
    [Serializable]
    public class BankAccountPostCodeAnywhereValid : IBankAccountValid
    {
        /// <summary>
        ///     Gets or sets whether the account number a sortcode are valid.
        /// </summary>
        public bool IsCorrect { get; set; }

        /// <summary>
        /// True if the validation service is /was not available when the bank account was validated.
        /// </summary>
        public bool NoService { get; set; }

        /// <summary>
        ///     Gets whether the account can accept direct debits.Certain accounts(e.g.savings) will not accept direct debits.
        /// </summary>
        public bool IsDirectDebitCapable { get;  set; }

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
        public string StatusInformation { get;  set; }

        /// <summary>
        ///     Gets the correct version of the SortCode.This will be 6 digits long with no hyphens.It may differ from the original
        ///     sortcode.
        /// </summary>
        public string CorrectedSortCode { get;  set; }

        /// <summary>
        ///     Gets the correct version of the AccountNumber. This will be 8 digits long and in the form expected for BACs
        ///     submission.
        /// </summary>
        public string CorrectedAccountNumber { get;  set; }

        /// <summary>
        ///     Gets the correctly formatted IBAN for the account.
        /// </summary>
        public string IBAN { get;  set; }

        /// <summary>
        ///     Gets the name of the banking institution.
        /// </summary>
        public string Bank { get;  set; }

        /// <summary>
        ///     Gets the banking institution's BIC, also know as the SWIFT BIC.
        /// </summary>
        public string BankBIC { get;  set; }

        /// <summary>
        ///     Gets the name of the account holding branch.
        /// </summary>
        public string Branch { get;  set; }

        /// <summary>
        ///     Gets the branch's BIC.
        /// </summary>
        public string BranchBIC { get;  set; }

        /// <summary>
        ///     Gets line 1 of the branch's contact address. NB: This is the address to be used for BACs enquiries and may be a
        ///     contact centre rather than the branch's address.
        /// </summary>
        public string ContactAddressLine1 { get;  set; }

        /// <summary>
        ///     Gets line 2 of the branch's contact address.
        /// </summary>
        public string ContactAddressLine2 { get;  set; }

        /// <summary>
        ///     Gets the branch's contact post town.
        /// </summary>
        public string ContactPostTown { get;  set; }

        /// <summary>
        ///     Gets the branch's contact postcode.
        /// </summary>
        public string ContactPostcode { get;  set; }

        /// <summary>
        ///     Gets the branch's contact phone number.
        /// </summary>
        public string ContactPhone { get;  set; }

        /// <summary>
        ///     Gets the branch's contact fax number.
        /// </summary>
        public string ContactFax { get;  set; }

        /// <summary>
        ///     Gets that the account supports the faster payments service.
        /// </summary>
        public bool FasterPaymentsSupported { get;  set; }

        /// <summary>
        ///     Gets that the account supports the CHAPS service.
        /// </summary>
        public bool CHAPSSupported { get;  set; }

    }
}