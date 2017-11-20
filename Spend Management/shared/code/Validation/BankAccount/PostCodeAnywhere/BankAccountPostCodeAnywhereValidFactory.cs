namespace Spend_Management.shared.code.Validation.BankAccount.PostCodeAnywhere
{
    using System;
    using System.Data;
    using SpendManagementLibrary.Account;

    /// <summary>
    /// Factory class to create <see cref="IBankAccountValid"/> objects from the Post Code Anywhere response.
    /// </summary>
    public static class BankAccountPostCodeAnywhereValidFactory
    {
        /// <summary>
        /// Convert the given <see cref="DataSet"/> into an instance of <seealso cref="BankAccountPostCodeAnywhereValid"/>
        /// </summary>
        /// <param name="validationDataSet">The <see cref="DataSet"/>to merge</param>
        /// <returns>A valid <see cref="BankAccountPostCodeAnywhereValid"/></returns>
        public static BankAccountPostCodeAnywhereValid Convert(DataSet validationDataSet)
        {
            if (validationDataSet == null || validationDataSet.Tables.Count == 0 || validationDataSet.Tables[0].Rows.Count == 0)
            {
                throw new Exception("Postcode anywhere failed to return data.");
            }

            var validationRow = validationDataSet.Tables[0].Rows[0];

            var result = new BankAccountPostCodeAnywhereValid
            {
                IsCorrect = bool.Parse(validationRow["IsCorrect"].ToString()),
                IsDirectDebitCapable = bool.Parse(validationRow["IsDirectDebitCapable"].ToString()),
                StatusInformation = validationRow["StatusInformation"].ToString(),
                CorrectedSortCode = validationRow["CorrectedSortCode"].ToString(),
                CorrectedAccountNumber = validationRow["CorrectedAccountNumber"].ToString(),
                IBAN = validationRow["IBAN"].ToString(),
                Bank = validationRow["Bank"].ToString(),
                BankBIC = validationRow["BankBIC"].ToString(),
                Branch = validationRow["Branch"].ToString(),
                BranchBIC = validationRow["BranchBIC"].ToString(),
                ContactAddressLine1 = validationRow["ContactAddressLine1"].ToString(),
                ContactAddressLine2 = validationRow["ContactAddressLine2"].ToString(),
                ContactPostTown = validationRow["ContactPostTown"].ToString(),
                ContactPostcode = validationRow["ContactPostcode"].ToString(),
                ContactPhone = validationRow["ContactPhone"].ToString(),
                ContactFax = validationRow["ContactFax"].ToString(),
                FasterPaymentsSupported = bool.Parse(validationRow["FasterPaymentsSupported"].ToString()),
                CHAPSSupported = bool.Parse(validationRow["CHAPSSupported"].ToString())
            };

            return result;
        }
    }
}
