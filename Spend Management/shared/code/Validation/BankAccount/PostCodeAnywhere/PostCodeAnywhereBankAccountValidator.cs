namespace Spend_Management.shared.code.Validation.BankAccount.PostCodeAnywhere
{
    using System;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Account;

    /// <summary>
    /// An instance of a validator for bank accounts, as supplied by Postcode Anywhere.
    /// </summary>
    public class PostCodeAnywhereBankAccountValidator : IBankAccountValidator
    {
        /// <summary>
        /// A private instance of <see cref="cAccount"/>
        /// </summary>
        private readonly cAccount _account;

        /// <summary>
        /// Create a new instance of <see cref="PostCodeAnywhereBankAccountValidator"/>
        /// </summary>
        /// <param name="account">An instance of <see cref="cAccount"/>used to attach to the correct data.</param>
        public PostCodeAnywhereBankAccountValidator(cAccount account)
        {
            this._account = account;
        }

        /// <summary>
        /// validate a specific bank account with a validation service
        /// </summary>
        /// <param name="bankAccount">An instance of <see cref="BankAccount"/>to validate</param>
        /// <returns>An instance of <see cref="BankAccountPostCodeAnywhereValid"/></returns>
        public IBankAccountValid Validate(SpendManagementLibrary.Account.BankAccount bankAccount)
        {
            if (string.IsNullOrEmpty(this._account.PostCodeAnywherePaymentServiceKey))
            {
                return new BankAccountPostCodeAnywhereValid {IsCorrect = true, StatusInformation = BankAccountConstants.NoService, NoService = true};
            }

            return this.BankAccountValidationInteractiveValidatev200(this._account.PostCodeAnywherePaymentServiceKey,
                bankAccount.AccountNumber, bankAccount.SortCode);
        }


        /// <summary>
        /// Validate a bank account using PostCode anywhere payment service
        /// </summary>
        /// <param name="key">The postcode anywhere key</param>
        /// <param name="accountnumber">The account number to verify</param>
        /// <param name="sortcode">The cosrt code to verify</param>
        /// <returns>An instance of <see cref="DataSet"/> containing the following columns
        ///            IsCorrect
        ///            IsDirectDebitCapable
        ///            StatusInformation
        ///            CorrectedSortCode
        ///            CorrectedAccountNumber
        ///            IBAN
        ///            Bank
        ///            BankBIC
        ///            Branch
        ///            BranchBIC
        ///            ContactAddressLine1
        ///            ContactAddressLine2
        ///            ContactPostTown
        ///            ContactPostcode
        ///            ContactPhone
        ///            ContactFax
        ///            FasterPaymentsSupported
        ///            CHAPSSupported
        /// </returns>
        private IBankAccountValid BankAccountValidationInteractiveValidatev200(string key, string accountnumber, string sortcode)
        {
            //Build the url
            var url =
                $"https://services.postcodeanywhere.co.uk/BankAccountValidation/Interactive/Validate/v2.00/dataset.ws?&Key={System.Web.HttpUtility.UrlEncode(key)}&AccountNumber={System.Web.HttpUtility.UrlEncode(accountnumber)}&SortCode={System.Web.HttpUtility.UrlEncode(sortcode)}";

            //Create the dataset
            var dataSet = new System.Data.DataSet();
            dataSet.ReadXml(url);

            //Check for an error
            if (dataSet.Tables.Count == 1 && dataSet.Tables[0].Columns.Count == 4 && dataSet.Tables[0].Columns[0].ColumnName == "Error")
            {
                var errorNumber = dataSet.Tables[0].Rows[0]["Error"].ToString();
                switch (errorNumber)
                {
                    // Possible user / value errors so return them to the user.
                    case "1001":
                        return new BankAccountInvalid(false, "The SortCode parameter was not supplied.");
                    case "1002":
                        return new BankAccountInvalid(false, "The SortCode parameter was not valid.");
                    case "1003":
                        return new BankAccountInvalid(false, "The AccountNumber parameter was not supplied.");
                    case "1004":
                        return new BankAccountInvalid(false, "The AccountNumber parameter was not valid.");
                    // Service / technical errors so not shown to the user.
                    case "-1":
                        throw new BankAccountValidationException("Unknown error - The cause of the error is unknown but details have been passed to our support staff who will investigate.");
                    case "2":
                        throw new BankAccountValidationException("Unknown key - The key you are using to access the service was not found.");
                    case "3":
                        throw new BankAccountValidationException("Account out of credit - Your account is either out of credit or has insufficient credit to service this request.");
                    case "4":
                        throw new BankAccountValidationException("Request not allowed from this IP - The request was disallowed from the IP address.");
                    case "5":
                        throw new BankAccountValidationException("Request not allowed from this URL - The request was disallowed from the URL.");
                    case "6":
                        throw new BankAccountValidationException("Web service not available on this key - The requested web service is disallowed on this key.");
                    case "7":
                        throw new BankAccountValidationException("Web service not available on your plan - The requested web service is not currently available on your payment plan.");
                    case "8":
                        throw new BankAccountValidationException("Key daily limit exceeded - The daily limit on the key has been exceeded.");
                    case "9":
                        throw new BankAccountValidationException("Surge protector running - The surge protector is currently enabled and has temporarily suspended access to the account.");
                    case "10":
                        throw new BankAccountValidationException("Surge protector triggered - An unusually large number of requests have been processed for your account so the surge protector has been enabled.");
                    case "11":
                        throw new BankAccountValidationException("No valid license available - The request requires a valid license but none were found.");
                    case "12":
                        throw new BankAccountValidationException("Management key required - To use this web service you require a management key.Management can be enabled on any key, but we advise you to use management keys with care.");
                    case "13":
                        throw new BankAccountValidationException("Demo limit exceeded - The daily demonstration limit for this service or account has been exceeded.");
                    case "14":
                        throw new BankAccountValidationException("Free service limit exceeded - You have used too many free web services.");
                    case "15":
                        throw new BankAccountValidationException("Wrong type of key - The type of key you're using isn't supported by this web service.");
                    case "16":
                        throw new BankAccountValidationException("Key expired - The key you are trying to use has expired.");
                    case "17":
                        throw new BankAccountValidationException("Key daily limit exceeded - The daily limit on the key has been exceeded.");
                    case "18":
                        throw new BankAccountValidationException("Missing or invalid parameters - A required parameter was not supplied of the value of a parameter cannnot be converted into the right type.");
                    default:
                        throw new Exception("Error with Postcode Anywhere Validation service");

                }
            }

            //Return the dataset
            return BankAccountPostCodeAnywhereValidFactory.Convert(dataSet);
        }
    }
}
